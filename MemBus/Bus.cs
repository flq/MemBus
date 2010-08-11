using System;
using System.Collections.Generic;
using MemBus.Subscribing;
using MemBus.Support;
using Rf.Common;

namespace MemBus
{
    public class Bus : IConfigurableBus, IBus
    {
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline pipeline;
        private readonly ShapeProvider shapeProvider = new ShapeProvider();
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();
        

        internal Bus()
        {
            pipeline = new PublishPipeline(this);
        }

        void IConfigurableBus.InsertResolver(ISubscriptionResolver resolver)
        {
            resolver.TryInvoke(r => r.Services = services);
            resolvers.Add(resolver);
        }

        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            configurePipeline(pipeline);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
            //TODO: Allow configuration of subscribing
            throw new NotImplementedException();
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
            subs = shapeProvider.Shape(subs, message);
            var t = new PublishToken(message, subs);
            pipeline.LookAt(t);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            var shape = getSubscriptionMatroschka();
            shape.TryInvoke(s => s.Services = services);
            return Subscribe(subscription, shape.Clone());
        }

        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            var sub = customization.EnhanceSubscription(new MethodInvocation<M>(subscription));
            resolvers.Add(sub);
            return sub is IDisposableSubscription ? ((IDisposableSubscription)sub).GetDisposer() : null;
        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization)
        {
            var subC = new SubscriptionCustomizer<M>(getSubscriptionMatroschka(), services);
            customization(subC);
            return Subscribe(subscription, subC);
        }

        private SubscriptionMatroschka getSubscriptionMatroschka()
        {
            var shape = services.Get<SubscriptionMatroschka>();
            if (shape == null)
                throw new MemBusException("Did not find a default subscription shape for a subscription. Please specify one at setup, or when subscribing.");
            return shape;
        }

        public IObservable<M> Observe<M>()
        {
            return new MessageObservable<M>(this);
        }
    }
}