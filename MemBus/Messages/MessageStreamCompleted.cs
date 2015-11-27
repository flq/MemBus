namespace MemBus.Messages
{
    /// <summary>
    /// Sent if an observable that enters MemBus via <see cref="IPublisher.Subscribe{M}"/>
    /// notifies observables via "OnCompleted".
    /// </summary>
    public class MessageStreamCompleted<M>
    {
    }
}

