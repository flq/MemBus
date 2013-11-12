using System;

namespace MemBus
{
    public class MessageObservable<M> : IObservable<M>
    {
        private readonly ISubscriber subscriber;
        
        public MessageObservable(ISubscriber subscriber)
        {
            this.subscriber = subscriber;
        }

        public IDisposable Subscribe(IObserver<M> observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");
            return subscriber.Subscribe<M>(observer.OnNext);
        }

    }
}