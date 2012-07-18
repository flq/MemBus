using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class InterfaceBasedBuilder : IMethodInfoScanner
    {
        private readonly IMethodInfoScanner _innerBuilder;

        public InterfaceBasedBuilder(Type interfaceType)
        {
            var suitableMethodsFound = interfaceType.InterfaceIsSuitableAsHandlerType();

            if (!suitableMethodsFound)
                throw new InvalidOperationException("Membus cannot handle Interface {0} as subscription. Interface should define only one void method with one parameter. Interface may be generic and can be implemented multiple times.".Fmt(interfaceType.Name));

            if (interfaceType.IsGenericTypeDefinition)
                _innerBuilder = new OpenInterfaceBuilder(interfaceType);
            else
                _innerBuilder = new ClosedInterfaceBuilder(interfaceType);
            
        }

        public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
        {
            if (targetToAdapt == null) throw new ArgumentNullException("targetToAdapt");
            return _innerBuilder.GetMethodInfos(targetToAdapt);
        }

        private class OpenInterfaceBuilder : IMethodInfoScanner
        {
            private readonly Type _interfaceType;

            public OpenInterfaceBuilder(Type interfaceType)
            {
                _interfaceType = interfaceType;
            }

            public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
            {
                var foundItfs = (from itf in targetToAdapt.GetType().GetInterfaces()
                                where itf.IsGenericType && itf.GetGenericTypeDefinition().Equals(_interfaceType)
                                select itf).ToList();
                if (!foundItfs.Any())
                    return Enumerable.Empty<MethodInfo>();

                return
                    foundItfs
                    .Select(itf => new ClosedInterfaceBuilder(itf))
                    .SelectMany(b => b.GetMethodInfos(targetToAdapt));
            }
        }

        private class ClosedInterfaceBuilder : IMethodInfoScanner
        {
            private readonly Type _interfaceType;

            public ClosedInterfaceBuilder(Type interfaceType)
            {
                _interfaceType = interfaceType;
            }

            public IEnumerable<MethodInfo> GetMethodInfos(object targetToAdapt)
            {
                var hasInterface = targetToAdapt.GetType().GetInterfaces().Any(t => t.Equals(_interfaceType));
                if (!hasInterface)
                    return Enumerable.Empty<MethodInfo>();

                var itfMi = _interfaceType.MethodsSuitableForSubscription().First();

                var candidates = from mi in targetToAdapt.GetType().GetInterfaceMap(_interfaceType).InterfaceMethods
                                 where mi.Name == itfMi.Name &&
                                       mi.GetParameters().Length == 1 &&
                                       mi.GetParameters()[0].ParameterType == itfMi.GetParameters()[0].ParameterType &&
                                       mi.ReturnType == itfMi.ReturnType
                                 select mi;
                return candidates;
            }
        }
    }
}