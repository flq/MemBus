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

        public static ISubscription ConstructPublishingSubscription(this MethodInfo targetMethod, object target, IPublisher publisher)
        {
            var parameterType = targetMethod.GetParameters()[0].ParameterType;
            var fittingDelegateType = typeof(Func<,>).MakeGenericType(parameterType,typeof(object));
            var p = Expression.Parameter(parameterType);
            Expression call = Expression.Call(Expression.Constant(target), targetMethod, p);
            if (!targetMethod.ReturnType.GetTypeInfo().IsClass)
            {
                call = Expression.Convert(call, typeof(object));
            }
            var @delegate = Expression.Lambda(fittingDelegateType, call, p);

            var fittingMethodSubscription = typeof(PublishingMethodInvocation<>).MakeGenericType(parameterType);
            var sub = Activator.CreateInstance(fittingMethodSubscription, @delegate.Compile(), publisher);

            return (ISubscription)sub;
        }

        public static IEnumerable<ISubscription> ConstructSubscriptions(this IEnumerable<MethodInfo> infos, object target)
        {
            return infos.Select(i => i.ConstructSubscription(target));
        }

        public static IEnumerable<ISubscription> ConstructPublishingSubscriptions(this IEnumerable<MethodInfo> infos, object target, IPublisher publisher)
        {
            return infos.Select(i => i.ConstructPublishingSubscription(target, publisher));
        }
    }
}