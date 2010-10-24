using System;

namespace MemBus
{
    public class MessageObservable<M> : IObservable<M>
    {
        private readonly IBus bus;
        
        public MessageObservable(IBus bus)
        {
            this.bus = bus;
        }

        public IDisposable Subscribe(IObserver<M> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            return bus.Subscribe<M>(observer.OnNext);
        }

    }
}