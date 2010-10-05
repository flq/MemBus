using System;
using MemBus.Subscribing;

namespace MemBus
{
    public interface IBus : IDisposable
    {
        void Publish(object message);
        IDisposable Subscribe<M>(Action<M> subscription);
        IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization);
        IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization);
        IObservable<M> Observe<M>();
        IBus Clone();
    }
}