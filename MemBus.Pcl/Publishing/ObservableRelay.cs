using MemBus.Messages;
using System;

namespace MemBus.Publishing
{
    internal class ObservableRelay
    {
        readonly IPublisher _publisher;

        public ObservableRelay(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public IDisposable Connect<M>(IObservable<M> observable)
        {
            var obs = new BusRelayObserver<M>(_publisher);
            return observable.Subscribe(obs);
        }

       

        private class BusRelayObserver<M> : IObserver<M>
        {
            readonly IPublisher _publisher;

            public BusRelayObserver(IPublisher publisher)
            {
                _publisher = publisher;
            }

            public void OnCompleted()
            {
                _publisher.Publish(new MessageStreamCompleted<M>());
            }

            public void OnError(Exception error)
            {
                _publisher.Publish(new ExceptionOccurred(error));
            }

            public void OnNext(M value)
            {
                _publisher.Publish(value);
            }
        }
    }
}

