using System;
using System.Collections.Generic;
using System.Linq;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class InterfaceBasedBuilder : ISubscriptionBuilder
    {
        private readonly ISubscriptionBuilder innerBuilder;

        public InterfaceBasedBuilder(Type interfaceType)
        {
            var suitableMethodsFound = interfaceType.InterfaceIsSuitableAsHandlerType();

            if (!suitableMethodsFound)
                throw new InvalidOperationException("Membus cannot handle Interface {0} as subscription. Interface should define only one void method with one parameter. Interface may be generic and can be implemented multiple times.".Fmt(interfaceType.Name));

            if (interfaceType.IsGenericTypeDefinition)
                innerBuilder = new OpenInterfaceBuilder(interfaceType);
            else
                innerBuilder = new ClosedInterfaceBuilder(interfaceType);
            
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");
            return innerBuilder.BuildSubscriptions(targetToAdapt);
        }

        private class OpenInterfaceBuilder : ISubscriptionBuilder
        {
            private readonly Type interfaceType;

            public OpenInterfaceBuilder(Type interfaceType)
            {
                this.interfaceType = interfaceType;
            }

            public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
            {
                var foundItfs = from itf in targetToAdapt.GetType().GetInterfaces()
                                where itf.IsGenericType && itf.GetGenericTypeDefinition().Equals(interfaceType)
                                select itf;
                if (foundItfs.Count() == 0)
                    return new ISubscription[0];

                return
                    foundItfs
                    .Select(itf => new ClosedInterfaceBuilder(itf))
                    .SelectMany(b => b.BuildSubscriptions(targetToAdapt));
            }
        }

        private class ClosedInterfaceBuilder : ISubscriptionBuilder
        {
            private readonly Type interfaceType;

            public ClosedInterfaceBuilder(Type interfaceType)
            {
                this.interfaceType = interfaceType;
            }

            public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
            {
                var hasInterface = targetToAdapt.GetType().GetInterfaces().Any(t => t.Equals(interfaceType));
                if (!hasInterface)
                    return new ISubscription[0];

                var itfMi = interfaceType.MethodsSuitableForSubscription().First();

                var candidates = from mi in targetToAdapt.GetType().GetMethods()
                                 where mi.Name == itfMi.Name &&
                                       mi.GetParameters().Length == 1 &&
                                       mi.GetParameters()[0].ParameterType == itfMi.GetParameters()[0].ParameterType &&
                                       mi.ReturnType == itfMi.ReturnType
                                 select mi;
                return candidates.ConstructSubscriptions(targetToAdapt);
            }
        }
    }
}