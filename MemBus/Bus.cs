using System;
using System.Collections.Generic;

namespace MemBus
{
    public class Bus : IConfigurableBus, IBus
    {
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline pipeline = new PublishPipeline();
        private readonly List<object> automatons = new List<object>();

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
        }

        public void Publish(object message)
        {
            var subs = resolvers.GetSubscriptionsFor(message);
            var t = new PublishToken(message, subs);
            pipeline.LookAt(t);
        }
    }
}