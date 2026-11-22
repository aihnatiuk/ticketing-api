using BuildingBlocks.Contracts;

namespace BuildingBlocks.Infrastructure.Messaging;

/// <summary>
/// Delivers an outbox message of the module to the module's events exchange.
/// </summary>
/// <typeparam name="TModule">The module that owns the exchange.</typeparam>
internal interface IMessagePublisher<TModule>
    where TModule : IModule
{
    Task PublishAsync(
        Guid messageId,
        string routingKey,
        string payload,
        CancellationToken cancellationToken = default
    );
}
