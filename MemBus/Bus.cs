using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    internal class Bus : IConfigurableBus, IBus
    {
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline publishPipeline;
        private readonly SubscriptionPipeline subscriptionPipeline;
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();

        private readonly DisposeContainer disposer;

        private volatile bool isDisposed;

        internal Bus():this(null) { }
        
        internal Bus(Bus template)
        {
            if (template == null)
            {
                //This is the root!
                publishPipeline = new PublishPipeline(this);
                subscriptionPipeline = new SubscriptionPipeline(services);
                disposer = new DisposeContainer(resolvers, publishPipeline, subscriptionPipeline, services);
            }
            else
            {
                //Take over pipelines
                //TODO: More correct is to clone the pipelines, and then properly dispose them
                publishPipeline = template.publishPipeline;
                subscriptionPipeline = template.subscriptionPipeline;
                //New subscribers!
                resolvers = new CompositeResolver(new TableBasedResolver());
                disposer = new DisposeContainer();
            }
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
            UpwardsPublish(message);
        }

        public void UpwardsPublish(object message)
        {
            var subs = resolvers.GetSubscriptionsFor(message);
            subs = subscriptionPipeline.Shape(subs, message);
            var t = new PublishToken(message, subs);
            publishPipeline.LookAt(t);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return Subscribe(subscription, subscriptionPipeline.GetIntroductionShape());
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