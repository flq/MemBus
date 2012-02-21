using System;
using System.Collections.Generic;
using System.Linq;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class VoidMethodBasedBuilder : ISubscriptionBuilder
    {
        private readonly string methodName;

        public VoidMethodBasedBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().VoidMethodCandidatesForSubscriptionBuilders(methodName).ToList();

            return candidates.Count == 0 ? new ISubscription[0] : candidates.ConstructSubscriptions(targetToAdapt);
        }
    }

    public class ReturningMethodBasedBuilder : ISubscriptionBuilder
    {
        private readonly string methodName;
        private IPublisher _publisher;

        public ReturningMethodBasedBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        // Note: dynamically invoked by FlexibleSubscribeAdapter
        public void SetPublisher(IPublisher publisher)
        {   
            _publisher = publisher;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().ReturningMethodCandidatesForSubscriptionBuilders(methodName).ToList();

            return candidates.Count == 0 ? new ISubscription[0] : candidates.ConstructPublishingSubscriptions(targetToAdapt, _publisher);
        }
    }
}