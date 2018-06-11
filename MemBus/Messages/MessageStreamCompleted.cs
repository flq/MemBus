namespace MemBus.Messages
{
    /// <summary>
    /// Sent if an observable that enters MemBus via <see cref="IPublisher.Publish"/>
    /// notifies observables via "OnCompleted".
    /// </summary>
    // ReSharper disable once UnusedTypeParameter
    public class MessageStreamCompleted<M>
    {
    }
}

