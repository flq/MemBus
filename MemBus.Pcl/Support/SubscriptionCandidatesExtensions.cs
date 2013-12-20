using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemBus.Subscribing;

namespace MemBus.Support
{
    public static class SubscriptionCandidatesExtensions
    {
        public static bool InterfaceIsSuitableAsIoCHandler(this Type itf)
        {
            var methods = itf.GetRuntimeMethods().ToList();
            return methods.Count == 1 && 
                   methods[0].GetParameters().Length == 1 &&
                   methods[0].ReturnType == typeof (void);
        }

        public static IEnumerable<MethodInfo> MethodCandidatesForSubscriptionBuilders(this Type reflectedType,Func<MethodInfo, bool> methodSelector)
        {
            var disposeTokenMethod = reflectedType.ImplementsInterface<IAcceptDisposeToken>()
                ? (mi => mi.Name == "Accept" && mi.GetParameters().Length == 1 &&
                         mi.GetParameters()[0].ParameterType == typeof(IDisposable))
                : new Func<MethodInfo, bool>(mi => false);

            return reflectedType.GetRuntimeMethods().ReduceToValidMessageEndpoints(
                mi => mi.DeclaringType == reflectedType && methodSelector(mi),
                disposeTokenMethod);
        }

        /// <summary>
        /// Picks out those methods that MemBus can use as targets/sources of message-passing tasks
        /// </summary>
        public static IEnumerable<MethodInfo> ReduceToValidMessageEndpoints(
            this IEnumerable<MethodInfo> methods,
            Func<MethodInfo, bool> additionalMethodSelector = null,
            Func<MethodInfo, bool> additionalMethodExclusion = null)
        {
            additionalMethodSelector = additionalMethodSelector ?? (info => true);
            additionalMethodExclusion = additionalMethodExclusion ?? (info => false);

            return
                from mi in methods
                where
                    !mi.IsGenericMethod &&
                    !mi.IsStatic &&
                    mi.IsPublic &&
                    mi.HasOneParameterOrNoneAndReturnsIObservable() &&
                    !additionalMethodExclusion(mi) &&
                    additionalMethodSelector(mi)
                select mi;
        }

        private static bool HasOneParameterOrNoneAndReturnsIObservable(this MethodInfo mi)
        {
            return
                mi.GetParameters().Length == 1 ||
                (
                    mi.GetParameters().Length == 0 &&
                    mi.ReturnType.IsConstructedGenericType &&
                    mi.ReturnType.GetGenericTypeDefinition() == typeof(IObservable<>)
                    );
        }
    }

    
}