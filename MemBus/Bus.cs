using System;
using System.Collections.Generic;
using MemBus.Support;
using Rf.Common;

namespace MemBus
{
    public class Bus : IConfigurableBus, IBus
    {
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline pipeline = new PublishPipeline();
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();

        void IConfigurableBus.InsertResolver(ISubscriptionResolver resolver)
        {
            resolvers.Add(resolver);
        }

        void IConfigurableBus.InsertPublishPipelineMember(IPublishPipelineMember publishPipelineMember)
        {
            pipeline.Add(publishPipelineMember);
        }

        void IConfigurableBus.AddSubscription(ISubscription subscription)
        {
            resolvers.Add(subscription);
        }

        void IConfigurableBus.AddAutomaton(object automaton)
        {
            automatons.Add(automaton);
            automaton.TryInvoke(a => a.AcceptBus(this));
            automaton.TryInvoke(a => a.AcceptServices(services));
        }

        public void Publish(object message)
        {
            var subs = resolvers.GetSubscriptionsFor(message);
            var t = new PublishToken(message, subs);
            pipeline.LookAt(t);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            var shape = services.Get<ISubscriptionShape>();
            if (shape == null)
                throw new MemBusException("Did not find a default subscription shape for a subscription. Please specify one at setup, or when subscribing.");
            shape.TryInvoke(s => s.AcceptServices(services));
            return Subscribe(subscription, shape);
        }

        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShape customization)
        {
            var sub = customization.ConstructSubscription(subscription);
            resolvers.Add(sub);
            return sub.GetDisposer();
        }

        public IObservable<M> Observe<M>()
        {
            return new MessageObservable<M>(this);
        }
    }
}