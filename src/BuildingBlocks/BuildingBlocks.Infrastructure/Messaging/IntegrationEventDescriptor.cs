using BuildingBlocks.Contracts;

namespace BuildingBlocks.Infrastructure.Messaging;

internal record struct IntegrationEventDescriptor(string PublishingModuleName, string EventName)
{
    public static IntegrationEventDescriptor From(Type eventType)
    {
        if (
            eventType
                .GetCustomAttributes(typeof(IntegrationEventAttribute), inherit: false)
                .FirstOrDefault()
            is not IntegrationEventAttribute attribute
        )
        {
            throw new InvalidOperationException(
                $"Integration event type '{eventType.FullName}' is missing the required {nameof(IntegrationEventAttribute)}."
            );
        }

        if (string.IsNullOrWhiteSpace(attribute.PublishingModuleName))
        {
            throw new InvalidOperationException(
                $"{nameof(IntegrationEventAttribute)} on integration event type '{eventType.FullName}' must specify a non-empty publishing module name."
            );
        }

        if (string.IsNullOrWhiteSpace(attribute.EventName))
        {
            throw new InvalidOperationException(
                $"{nameof(IntegrationEventAttribute)} on integration event type '{eventType.FullName}' must specify a non-empty event name."
            );
        }

        return new IntegrationEventDescriptor(attribute.PublishingModuleName, attribute.EventName);
    }
}
