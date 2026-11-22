using BuildingBlocks.Contracts;
using BuildingBlocks.Infrastructure.Messaging.RabbitMq;

namespace BuildingBlocks.Infrastructure.Messaging;

/// <summary>
/// The messaging identity of a module.
/// </summary>
internal static class ModuleMessagingSettings<TModule>
    where TModule : IModule
{
    public static string Exchange => RabbitMqNaming.EventsExchange(TModule.ModuleName);

    public static string DeadLetterExchange =>
        RabbitMqNaming.DeadLetterExchange(TModule.ModuleName);
}
