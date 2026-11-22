using BuildingBlocks.Infrastructure.Messaging.RabbitMq;

namespace BuildingBlocks.Infrastructure.Messaging;

/// <summary>
/// The messaging identity of a module. One singleton exists per module, discriminated by <typeparamref name="TModule"/>.
/// </summary>
/// <param name="Name">A module name.</param>
internal sealed record ModuleMessagingSettings<TModule>(string Name)
{
    public string Exchange => RabbitMqNaming.EventsExchange(Name);

    public string DeadLetterExchange => RabbitMqNaming.DeadLetterExchange(Name);
}
