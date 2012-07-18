using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemBus.Support;

namespace MemBus.Subscribing
{
    internal class VoidMethodBasedBuilder : IMethodInfoScanner
    {
        private readonly Func<MethodInfo,bool> _methodSelector;

        public VoidMethodBasedBuilder(Func<MethodInfo,bool> methodSelector)
        {
            _methodSelector = methodSelector;
        }

        public VoidMethodBasedBuilder(string methodName) : this(mi => mi.Name == methodName)
        {
        }

        public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().VoidMethodCandidatesForSubscriptionBuilders(_methodSelector).ToList();

            return candidates;
        }
    }

    internal class ReturningMethodBasedBuilder : IMethodInfoScanner
    {
        private readonly Func<MethodInfo, bool> _methodSelector;

        public ReturningMethodBasedBuilder(Func<MethodInfo, bool> methodSelector)
        {
            _methodSelector = methodSelector;
        }

        public ReturningMethodBasedBuilder(string methodName)
            : this(mi => mi.Name == methodName)
        {
            
        }

        public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates = targetToAdapt.GetType().ReturningMethodCandidatesForSubscriptionBuilders(_methodSelector).ToList();

            return candidates;
        }
    }
}