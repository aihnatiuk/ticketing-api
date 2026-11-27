namespace BuildingBlocks.Contracts;

/// <summary>
/// Marks a type as an integration event.
/// </summary>
/// <param name="publishingModuleName">A name of the module that publishes the event. Used to determine the exchange name to bind the consumer queue.</param>
/// <param name="eventName">A name of the integration event. Used to create a queue name on the consumer side.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IntegrationEventAttribute(string publishingModuleName, string eventName)
    : Attribute
{
    public string PublishingModuleName { get; } = publishingModuleName;

    public string EventName { get; } = eventName;
}
