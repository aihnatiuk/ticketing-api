using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Messaging.Outbox;

/// <summary>
/// Polls the module's outbox table and delivers pending messages to the message broker in order.
/// </summary>
internal sealed class OutboxProcessor<TModule, TContext>(
    IServiceScopeFactory scopeFactory,
    IMessagePublisher<TModule> publisher,
    ILogger<OutboxProcessor<TModule, TContext>> logger
) : BackgroundService
    where TContext : DbContext
{
    private const int BatchSize = 20;
    private readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
    private readonly TimeSpan FailureDelay = TimeSpan.FromSeconds(5);

    private string _table = string.Empty;
    private string _schema = string.Empty;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
        IEntityType? entityType = context.Model.FindEntityType(typeof(OutboxMessage));
        if (entityType is null)
        {
            throw new InvalidOperationException(
                $"The DbContext {typeof(TContext).Name} does not map the {nameof(OutboxMessage)} entity. "
                    + $"Call {nameof(OutboxModelBuilderExtensions.ApplyOutboxConfiguration)} in OnModelCreating."
            );
        }

        _table = entityType.GetTableName()!;
        _schema = entityType.GetSchema() ?? context.Model.GetDefaultSchema() ?? "dbo";

        while (!stoppingToken.IsCancellationRequested)
        {
            TimeSpan delay = PollInterval;

            try
            {
                bool hasMore = await ProcessBatchAsync(stoppingToken);
                if (hasMore)
                {
                    continue;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Outbox processing of {Context} failed",
                    typeof(TContext).Name
                );
                delay = FailureDelay;
            }

            await Task.Delay(delay, stoppingToken);
        }
    }

    /// <summary>
    /// Publishes one batch of outbox messages.
    /// </summary>
    /// <returns>true if a full batch was delivered and more rows may be waiting; otherwise, false</returns>
    /// <exception cref="InvalidOperationException">The batch was only partially delivered.</exception>
    private async Task<bool> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();

        await using IDbContextTransaction transaction =
            await context.Database.BeginTransactionAsync(cancellationToken);

#pragma warning disable EF1002, EF1003, S2077 // Risk of vulnerability to SQL injection.

        // Even though each module has only one outbox processor, UPDLOCK + ROWLOCK + READPAST provide additional safety.
        List<OutboxMessage> pending = await context
            .Set<OutboxMessage>()
            .FromSqlRaw(
                $"""
                SELECT TOP ({BatchSize}) *
                FROM [{_schema}].[{_table}] WITH (UPDLOCK, ROWLOCK, READPAST)
                WHERE [ProcessedAt] IS NULL
                ORDER BY [OccurredAt]
                """
            )
            .ToListAsync(cancellationToken);

#pragma warning restore EF1002, EF1003, S2077 // Risk of vulnerability to SQL injection.

        if (pending.Count == 0)
        {
            return false;
        }

        bool allDelivered = true;
        foreach (OutboxMessage message in pending)
        {
            try
            {
                await publisher.PublishAsync(
                    message.Id,
                    message.Type,
                    message.Payload,
                    cancellationToken
                );
                message.ProcessedAt = DateTimeOffset.UtcNow;
                message.Error = null;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogWarning(
                    exception,
                    "Failed to publish outbox message {MessageId} ({Type})",
                    message.Id,
                    message.Type
                );
                message.Attempts++;
                message.Error =
                    exception.Message.Length > 2000 ? exception.Message[..2000] : exception.Message;

                // Stop at the first failure to preserve ordering.
                allDelivered = false;
                break;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (!allDelivered)
        {
            throw new InvalidOperationException(
                "Outbox batch was only partially delivered. Backing off before retrying."
            );
        }

        // Assume that if the DB returned a full batch, there may be more rows waiting.
        // So, skip the delay and immediately poll again.
        return pending.Count == BatchSize;
    }
}
