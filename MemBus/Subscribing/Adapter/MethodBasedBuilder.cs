using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MemBus.Subscribing
{
    public class MethodBasedBuilder : ISubscriptionBuilder
    {
        private readonly string methodName;

        public MethodBasedBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");

            var candidates =
                (from mi in targetToAdapt.GetType().GetMethods()
                where 
                  mi.Name == methodName && 
                  !mi.IsGenericMethod &&
                  mi.GetParameters().Length == 1 && 
                  mi.ReturnType.Equals(typeof (void))
                select mi).ToList();

            if (candidates.Count == 0)
                return new ISubscription[0];

            return candidates.ConstructSubscriptions(targetToAdapt);
        }
    }
}