using System;
using System.Threading.Tasks;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    internal class Bus : IConfigurableBus, IBus
    {
        private readonly PublishChainCasing _publishChainCasing;
        private readonly Subscriber _subscriber;
        private readonly IServices _services = new StandardServices();

        private readonly DisposeContainer _disposer;

        private volatile bool _isDisposed;
        private bool _isDisposing;

        internal Bus()
        {
            _services.Add<IPublisher>(this);
            _services.Add(new ObservableRelay(this));
            _publishChainCasing = new PublishChainCasing(this);
            _subscriber = new Subscriber(_services);
            _disposer = new DisposeContainer { _publishChainCasing, _subscriber, (IDisposable)_services };
        }

        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            CheckDisposed();
            configurePipeline(_publishChainCasing);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
           ((IConfigurableSubscriber)_subscriber).ConfigureSubscribing(configure);
        }

        void IConfigurableSubscriber.AddResolver(ISubscriptionResolver resolver)
        {
            ((IConfigurableSubscriber)_subscriber).AddResolver(resolver);
        }

        void IConfigurableSubscriber.AddSubscription(ISubscription subscription)
        {
            ((IConfigurableSubscriber)_subscriber).AddSubscription(subscription);
        }

        void IConfigurableBus.AddService<T>(T service)
        {
            CheckDisposed();
            _services.Add(service);
        }

        public void Publish(object message)
        {
            CheckDisposed();
            var subs = _subscriber.GetSubscriptionsFor(message);
            var t = new PublishToken(message, subs);
            _publishChainCasing.LookAt(t);
        }

        public async Task PublishAsync(object message)
        {
            CheckDisposed();
            var subs = _subscriber.GetSubscriptionsFor(message);
            var t = new PublishToken(message, subs);
            await _publishChainCasing.LookAtAsync(t);
        }

        public IDisposable Publish<M>(IObservable<M> observable)
        {
            CheckDisposed();
            return _services.Get<ObservableRelay>().Connect(observable);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return _subscriber.Subscribe(subscription);
        }

        public IDisposable Subscribe(object subscriber)
        {
            return _subscriber.Subscribe(subscriber);
        }

        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            return _subscriber.Subscribe(subscription, customization);
        }

        public IObservable<M> Observe<M>()
        {
            return _subscriber.Observe<M>();
        }

        public void Dispose()
        {
            if (_isDisposing)
                return;

            try
            {
                _isDisposing = true;
                _disposer.Dispose();
                
            }
            finally
            {
                _isDisposing = false;
            }
            _isDisposed = true;
        }

        internal IServices Services { get { return _services; }}

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Bus");
        }
    }
}