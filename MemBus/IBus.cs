using System;

namespace MemBus
{
    public interface IBus
    {
        void Publish(object message);
        IDisposable Subscribe<M>(Action<M> subscription);
        IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShape customization);
        IObservable<M> Observe<M>();
    }
}