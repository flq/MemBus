using System;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    internal class Bus : IConfigurableBus, IBus
    {
        private readonly PublishChainCasing _publishChainCasing;
        private readonly Subscriber subscriber;
        private readonly IServices _services = new StandardServices();

        private readonly DisposeContainer disposer;

        private volatile bool _isDisposed;
        private bool _isDisposing;

        internal Bus()
        {
            _services.Add<IPublisher>(this);
            _publishChainCasing = new PublishChainCasing(this);
            subscriber = new Subscriber(_services);
            disposer = new DisposeContainer { _publishChainCasing, subscriber, (IDisposable)_services };
        }

        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            checkDisposed();
            configurePipeline(_publishChainCasing);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
           ((IConfigurableSubscriber)subscriber).ConfigureSubscribing(configure);
        }

        void IConfigurableSubscriber.AddResolver(ISubscriptionResolver resolver)
        {
            ((IConfigurableSubscriber)subscriber).AddResolver(resolver);
        }

        void IConfigurableSubscriber.AddSubscription(ISubscription subscription)
        {
            ((IConfigurableSubscriber)subscriber).AddSubscription(subscription);
        }

        void IConfigurableBus.AddService<T>(T service)
        {
            checkDisposed();
            _services.Add(service);
        }

        public void Publish(object message)
        {
            checkDisposed();
            var subs = subscriber.GetSubscriptionsFor(message);
            var t = new PublishToken(message, subs);
            _publishChainCasing.LookAt(t);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return subscriber.Subscribe(subscription);
        }

        public IDisposable Subscribe(object subscriber)
        {
            return this.subscriber.Subscribe(subscriber);
        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<SubscriptionCustomizer<M>> customization)
        {
            return subscriber.Subscribe(subscription, customization);
        }


        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            return subscriber.Subscribe(subscription, customization);
        }

        public IObservable<M> Observe<M>()
        {
            return subscriber.Observe<M>();
        }

        public void Dispose()
        {
            if (_isDisposing)
                return;

            try
            {
                _isDisposing = true;
                disposer.Dispose();
                
            }
            finally
            {
                _isDisposing = false;
            }
            _isDisposed = true;
        }

        internal IServices Services { get { return _services; }}

        private void checkDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Bus");
        }
    }
}