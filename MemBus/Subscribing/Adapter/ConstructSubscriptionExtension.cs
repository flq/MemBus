using System;
using System.Collections.Generic;
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
            var @delegate = Delegate.CreateDelegate(fittingDelegateType, target, info);

            var fittingMethodSubscription = typeof(MethodInvocation<>).MakeGenericType(parameterType);
            var sub = Activator.CreateInstance(fittingMethodSubscription, @delegate);

            return (ISubscription)sub;
        }

        public static IEnumerable<ISubscription> ConstructSubscriptions(this IEnumerable<MethodInfo> infos, object target)
        {
            return infos.Select(i => i.ConstructSubscription(target));
        }
    }
}