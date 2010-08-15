using System;
using System.Collections.Generic;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    public class Bus : IConfigurableBus, IBus
    {
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline publishPipeline;
        private readonly SubscriptionPipeline subscriptionPipeline = new SubscriptionPipeline();
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();
        

        internal Bus()
        {
            publishPipeline = new PublishPipeline(this);
        }

        void IConfigurableBus.InsertResolver(ISubscriptionResolver resolver)
        {
            resolver.TryInvoke(r => r.Services = services);
            resolvers.Add(resolver);
        }

        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            configurePipeline(publishPipeline);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
            configure(subscriptionPipeline);
        }

        void IConfigurableBus.AddSubscription(ISubscription subscription)
        {
            resolvers.Add(subscription);
        }

        void IConfigurableBus.AddAutomaton(object automaton)
        {
            automatons.Add(automaton);
            automaton.TryInvoke(a => a.Bus = this);
            automaton.TryInvoke(a => a.Services = services);
        }

        void IConfigurableBus.AddService<T>(T service)
        {
            services.Add(service);
        }

        public void Publish(object message)
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

        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            var sub = customization.EnhanceSubscription(new MethodInvocation<M>(subscription));
            resolvers.Add(sub);
            return sub.TryReturnDisposerOfSubscription();
        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization)
        {
            var subC = new SubscriptionCustomizer<M>(subscriptionPipeline.GetIntroductionShape(), services);
            customization(subC);
            return Subscribe(subscription, subC);
        }

        public IObservable<M> Observe<M>()
        {
            return new MessageObservable<M>(this);
        }
    }
}