using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;
using System.Linq;

namespace MemBus
{
    internal class Bus : IConfigurableBus, IBus
    {
        private readonly BusSetup busSetup;
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline publishPipeline;
        private readonly SubscriptionPipeline subscriptionPipeline;
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();

        private readonly DisposeContainer disposer;

        private volatile bool isDisposed;

        internal Bus()
        {
            publishPipeline = new PublishPipeline(this);
            subscriptionPipeline = new SubscriptionPipeline(services);
            disposer = new DisposeContainer(resolvers, publishPipeline, subscriptionPipeline, services);
        }

        public Bus(BusSetup busSetup) : this()
        {
            this.busSetup = busSetup;
        }


        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            checkDisposed();
            configurePipeline(publishPipeline);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
            checkDisposed();
            configure(subscriptionPipeline);
        }

        void IConfigurableBus.AddResolver(ISubscriptionResolver resolver)
        {
            checkDisposed();
            resolver.TryInvoke(r => r.Services = services);
            resolvers.Add(resolver);
        }

        void IConfigurableBus.AddSubscription(ISubscription subscription)
        {
            checkDisposed();
            resolvers.Add(subscription);
        }

        void IConfigurableBus.AddAutomaton(object automaton)
        {
            checkDisposed();
            automatons.Add(automaton);
            automaton.TryInvoke(a => a.Bus = this);
            automaton.TryInvoke(a => a.Services = services);
        }

        void IConfigurableBus.AddService<T>(T service)
        {
            checkDisposed();
            services.Add(service);
        }

        public void Publish(object message)
        {
            checkDisposed();
            var subs = resolvers.GetSubscriptionsFor(message);
            subs = subscriptionPipeline.Shape(subs, message);
            var t = new PublishToken(message, subs);
            publishPipeline.LookAt(t);
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
            var disposeShape = new ShapeToDispose();
            var subs = svc.SubscriptionsFor(subscriber).Select(disposeShape.EnhanceSubscription).ToList();
            foreach (var s in subs)
                resolvers.Add(s);
            return new DisposeContainer(subs.Select(s => ((IDisposableSubscription) s).GetDisposer()));

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

        public IBus Clone()
        {
            if (busSetup == null)
              throw new InvalidOperationException("This bus was not constructed as cloneable");
            var b = new Bus();
            busSetup.Accept(b);
            return b;
        }

        public void Dispose()
        {
            disposer.Dispose();
            isDisposed = true;
        }

        private void checkDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Bus");
        }
    }
}