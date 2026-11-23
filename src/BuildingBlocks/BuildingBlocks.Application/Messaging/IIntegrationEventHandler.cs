namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Handles an integration event received from the message broker.
/// Implementations must be idempotent because delivery is at-least-once.
/// </summary>
/// <typeparam name="TEvent">The integration event contract type.</typeparam>
public interface IIntegrationEventHandler<in TEvent>
    where TEvent : class
{
    Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken = default);
}
