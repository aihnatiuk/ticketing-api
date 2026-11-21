using System.Text.Json;
using BuildingBlocks.Application.Messaging;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Messaging.Outbox;

internal sealed class DbContextOutbox<TModule>(DbContext context) : IOutbox<TModule>
{
    public void Add<TMessage>(TMessage message)
        where TMessage : class
    {
        Type messageType = message.GetType();

        context
            .Set<OutboxMessage>()
            .Add(
                new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = messageType.FullName!,
                    Payload = JsonSerializer.Serialize(message, messageType),
                    OccurredAt = DateTimeOffset.UtcNow,
                }
            );
    }
}
