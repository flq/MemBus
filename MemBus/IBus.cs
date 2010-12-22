using System;
using MemBus.Subscribing;

namespace MemBus
{
    public interface IPublisher
    {
        void Publish(object message);
    }

    public interface ISubscriber
    {
        IDisposable Subscribe<M>(Action<M> subscription);
        IDisposable Subscribe(object subscriber);
        IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization);
        IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization);
        IObservable<M> Observe<M>();
    }

    public interface IBus : IDisposable, IPublisher, ISubscriber
    {
        IBus Clone();
    }
}