using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Owns the two lazily created, auto-recovering connections shared by all modules: one for publishing
/// and one for consuming, as recommended in https://www.rabbitmq.com/docs/connections#flow-control
/// Modules open their own channels on these connections.
/// </summary>
internal sealed class RabbitMqConnectionProvider(IOptions<RabbitMqOptions> options)
    : IAsyncDisposable
{
    private readonly LazyConnection _publisher = new(options, "ticketing-publisher");
    private readonly LazyConnection _consumer = new(options, "ticketing-consumer");

    public Task<IConnection> GetPublisherConnectionAsync(CancellationToken cancellationToken) =>
        _publisher.GetAsync(cancellationToken);

    public Task<IConnection> GetConsumerConnectionAsync(CancellationToken cancellationToken) =>
        _consumer.GetAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await _publisher.DisposeAsync();
        await _consumer.DisposeAsync();
    }

    private sealed class LazyConnection(IOptions<RabbitMqOptions> options, string name)
        : IAsyncDisposable
    {
        private readonly SemaphoreSlim _gate = new(1, 1);
        private IConnection? _connection;

        public async Task<IConnection> GetAsync(CancellationToken cancellationToken)
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            await _gate.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true })
                {
                    return _connection;
                }

                RabbitMqOptions settings = options.Value;
                var factory = new ConnectionFactory
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    ClientProvidedName = name,
                    AutomaticRecoveryEnabled = true,
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);

                return _connection;
            }
            finally
            {
                _gate.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }

            _gate.Dispose();
        }
    }
}
