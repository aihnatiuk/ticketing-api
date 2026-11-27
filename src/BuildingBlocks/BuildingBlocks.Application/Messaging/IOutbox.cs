using BuildingBlocks.Contracts;

namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Stores integration events in the module's outbox table so they are committed atomically with the
/// module's state changes and delivered to the message broker afterwards.
/// </summary>
/// <typeparam name="TModule">The module type that owns the outbox.</typeparam>
public interface IOutbox<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Adds the message to the database context. Isn't persisted until the database context is saved.
    /// </summary>
    void Add<TMessage>(TMessage message)
        where TMessage : class;
}
