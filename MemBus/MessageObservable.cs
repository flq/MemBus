using System;

namespace MemBus
{
    public class MessageObservable<M> : IObservable<M>
    {
        private readonly IDisposable disposer;
        private IObserver<M> observer;

        public MessageObservable(IBus bus)
        {
            disposer = bus.Subscribe<M>(onMessage);
        }

        private void onMessage(M obj)
        {
            if (observer != null)
                observer.OnNext(obj);
        }

        public IDisposable Subscribe(IObserver<M> observer)
        {
            this.observer = observer;
            return disposer;
        }
    }
}