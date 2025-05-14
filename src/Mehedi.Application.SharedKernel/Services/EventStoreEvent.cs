using Mehedi.Core.SharedKernel;

namespace Mehedi.Application.SharedKernel.Services;

/// <summary>
/// Represents the event store for storing events.
/// </summary>
public record EventStoreEvent : BaseDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the EventStore class.
    /// </summary>
    /// <param name="aggregateId">The aggregate ID.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="data">The data.</param>
    public EventStoreEvent(Guid aggregateId, string messageType, string data) : 
        base(messageType, aggregateId)
    {
        AggregateId = aggregateId;
        MessageType = messageType;
        Data = data;
    }

    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    public Guid Id { get; private init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public string Data { get; private init; }
}
