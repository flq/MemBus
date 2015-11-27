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
            if (interfaceType.GetTypeInfo().IsGenericTypeDefinition)
                _innerBuilder = new OpenInterfaceBuilder(interfaceType);
            else
                _innerBuilder = new ClosedInterfaceBuilder(interfaceType);
            
        }

        public IEnumerable<ClassifiedMethodInfo> GetMethodInfos(object targetToAdapt)
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

            public IEnumerable<ClassifiedMethodInfo> GetMethodInfos(object targetToAdapt)
            {
                var interfaces = targetToAdapt.GetType().GetTypeInfo().ImplementedInterfaces;
                var foundItfs = (from itf in interfaces
                                where itf.IsGenericType() && itf.GetGenericTypeDefinition() == _interfaceType
                                select itf).ToList();
                if (!foundItfs.Any())
                    return Enumerable.Empty<ClassifiedMethodInfo>();

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

            public IEnumerable<ClassifiedMethodInfo> GetMethodInfos(object targetToAdapt)
            {
                var hasInterface = targetToAdapt.GetType().GetTypeInfo().ImplementedInterfaces.Any(t => t == _interfaceType);
                if (!hasInterface)
                    return Enumerable.Empty<ClassifiedMethodInfo>();

                var runtimeInterfaceMap = targetToAdapt.GetType().GetTypeInfo().GetRuntimeInterfaceMap(_interfaceType);
                var candidates = runtimeInterfaceMap.InterfaceMethods.ReduceToValidMessageEndpoints();
                return candidates.Select(ClassifiedMethodInfo.New);
            }
        }
    }
}