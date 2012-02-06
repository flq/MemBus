using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace MemBus.Subscribing
{
    public static class ConstructSubscriptionExtension
    {
        public static ISubscription ConstructSubscription(this MethodInfo info, object target)
        {
            var parameterType = info.GetParameters()[0].ParameterType;
            var fittingDelegateType = typeof(Action<>).MakeGenericType(parameterType);
            var p = Expression.Parameter(parameterType);
            var call = Expression.Call(Expression.Constant(target), info, p);
            var @delegate = Expression.Lambda(fittingDelegateType, call, p);

            var fittingMethodSubscription = typeof(MethodInvocation<>).MakeGenericType(parameterType);
            var sub = Activator.CreateInstance(fittingMethodSubscription, @delegate.Compile());

            return (ISubscription)sub;
        }

        public static IEnumerable<ISubscription> ConstructSubscriptions(this IEnumerable<MethodInfo> infos, object target)
        {
            return infos.Select(i => i.ConstructSubscription(target));
        }
    }
}