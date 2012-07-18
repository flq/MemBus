using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemBus.Support;

namespace MemBus.Subscribing
{
    internal class VoidMethodBasedBuilder : ISubscriptionBuilder
    {
        private readonly Func<MethodInfo,bool> _methodSelector;

        public VoidMethodBasedBuilder(Func<MethodInfo,bool> methodSelector)
        {
            _methodSelector = methodSelector;
        }

        public VoidMethodBasedBuilder(string methodName) : this(mi => mi.Name == methodName)
        {
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().VoidMethodCandidatesForSubscriptionBuilders(_methodSelector).ToList();

            return candidates.Count == 0 ? new ISubscription[0] : candidates.ConstructSubscriptions(targetToAdapt);
        }
    }

    internal class ReturningMethodBasedBuilder : ISubscriptionBuilder
    {
        private readonly Func<MethodInfo, bool> _methodSelector;
        private IPublisher _publisher;

        public ReturningMethodBasedBuilder(Func<MethodInfo, bool> methodSelector)
        {
            _methodSelector = methodSelector;
        }

        public ReturningMethodBasedBuilder(string methodName)
            : this(mi => mi.Name == methodName)
        {
            
        }

        // Note: dynamically invoked by FlexibleSubscribeAdapter
        public void SetPublisher(IPublisher publisher)
        {   
            _publisher = publisher;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().ReturningMethodCandidatesForSubscriptionBuilders(_methodSelector).ToList();

            return candidates.Count == 0 ? new ISubscription[0] : candidates.ConstructPublishingSubscriptions(targetToAdapt, _publisher);
        }
    }
}