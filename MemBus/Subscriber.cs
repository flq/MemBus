using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    public class Subscriber : ISubscriber, IDisposable, IConfigurableSubscriber, ISubscriptionResolver
    {
        private readonly IServices _services;
        private readonly SubscriptionPipeline _subscriptionPipeline;
        private readonly CompositeResolver _resolvers = new CompositeResolver();
        private bool _isDisposed;

        public Subscriber(IServices services)
        {
            _services = services;
            _subscriptionPipeline = new SubscriptionPipeline(services);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return Subscribe(subscription, _subscriptionPipeline.GetIntroductionShape());

        }

        public IDisposable Subscribe(object subscriber)
        {
            var svc = _services.Get<IAdapterServices>();
            if (svc == null)
                throw new InvalidOperationException(
                    "No subscription adapter rules were formulated. Apply the FlexibleSubscribeAdapter to state rules how some instance may be wired up into MemBus.");
            var disposeContainer = svc.WireUpSubscriber(_resolvers, subscriber);
            return disposeContainer;

        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<SubscriptionCustomizer<M>> customization)
        {
            var subC = new SubscriptionCustomizer<M>(_subscriptionPipeline.GetIntroductionShape(), _services);
            customization(subC);
            return Subscribe(subscription, subC);
        }


        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            CheckDisposed();
            var sub = customization.EnhanceSubscription(new MethodInvocation<M>(subscription));
            _resolvers.Add(sub);
            return sub.TryReturnDisposerOfSubscription();
        }

        public IObservable<M> Observe<M>()
        {
            CheckDisposed();
            return new MessageObservable<M>(this);
        }

        public void Dispose()
        {
            _resolvers.Dispose();
            _subscriptionPipeline.Dispose();
            _isDisposed = true;
        }

        void IConfigurableSubscriber.ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
            CheckDisposed();
            configure(_subscriptionPipeline);
        }

        void IConfigurableSubscriber.AddResolver(ISubscriptionResolver resolver)
        {
            CheckDisposed();
            resolver.TryInvoke(r => r.Services = _services);
            _resolvers.Add(resolver);
        }

        void IConfigurableSubscriber.AddSubscription(ISubscription subscription)
        {
            ((ISubscriptionResolver) this).Add(subscription);
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var subs = _resolvers.GetSubscriptionsFor(message);
            subs = _subscriptionPipeline.Shape(subs, message);
            return subs;
        }

        bool ISubscriptionResolver.Add(ISubscription subscription)
        {
            CheckDisposed();
            return _resolvers.Add(subscription);
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Publisher");
        }
    }
}