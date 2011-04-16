using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    public class Subscriber : ISubscriber, IDisposable, IConfigurableSubscriber, ISubscriptionResolver
    {
        private readonly IServices services;
        private readonly SubscriptionPipeline subscriptionPipeline;
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private bool isDisposed;

        public Subscriber(IServices services)
        {
            this.services = services;
            subscriptionPipeline = new SubscriptionPipeline(services);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return Subscribe(subscription, subscriptionPipeline.GetIntroductionShape());

        }

        public IDisposable Subscribe(object subscriber)
        {
            var svc = services.Get<IAdapterServices>();
            if (svc == null)
                throw new InvalidOperationException(
                    "No subscription adapter rules were formulated. Apply the FlexibleSubscribeAdapter to state rules how some instance may be wired up into MemBus.");
            var disposeContainer = svc.WireUpSubscriber(resolvers, subscriber);
            return disposeContainer;

        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization)
        {
            var subC = new SubscriptionCustomizer<M>(subscriptionPipeline.GetIntroductionShape(), services);
            customization(subC);
            return Subscribe(subscription, subC);
        }


        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            checkDisposed();
            var sub = customization.EnhanceSubscription(new MethodInvocation<M>(subscription));
            resolvers.Add(sub);
            return sub.TryReturnDisposerOfSubscription();
        }

        public IObservable<M> Observe<M>()
        {
            checkDisposed();
            return new MessageObservable<M>(this);
        }

        public void Dispose()
        {
            resolvers.Dispose();
            subscriptionPipeline.Dispose();
            isDisposed = true;
        }

        void IConfigurableSubscriber.ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
            checkDisposed();
            configure(subscriptionPipeline);
        }

        void IConfigurableSubscriber.AddResolver(ISubscriptionResolver resolver)
        {
            checkDisposed();
            resolver.TryInvoke(r => r.Services = services);
            resolvers.Add(resolver);
        }

        void IConfigurableSubscriber.AddSubscription(ISubscription subscription)
        {
            ((ISubscriptionResolver) this).Add(subscription);
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var subs = resolvers.GetSubscriptionsFor(message);
            subs = subscriptionPipeline.Shape(subs, message);
            return subs;
        }

        bool ISubscriptionResolver.Add(ISubscription subscription)
        {
            checkDisposed();
            return resolvers.Add(subscription);
        }

        private void checkDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Publisher");
        }
    }
}