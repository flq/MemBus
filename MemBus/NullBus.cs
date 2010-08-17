using System;
using MemBus.Subscribing;

namespace MemBus
{
    internal class NullBus : IInternalBus
    {
        public void Dispose()
        {
        }

        public void Publish(object message) { }

        public void UpwardsPublish(object message) { }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            throw new InvalidOperationException("Not meant to be called");
        }

        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            throw new InvalidOperationException("Not meant to be called");
        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization)
        {
            throw new InvalidOperationException("Not meant to be called");
        }

        public IObservable<M> Observe<M>()
        {
            throw new InvalidOperationException("Not meant to be called");
        }

        public IBus SpawnChild()
        {
            throw new InvalidOperationException("Not meant to be called");
        }

        public void ScratchFromChilds(IInternalBus bus)
        {
            
        }
    }
}