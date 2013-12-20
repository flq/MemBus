using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MemBus.Subscribing
{
    internal class MessageEndpointsBuilder
    {
        private readonly List<IMethodInfoScanner> _scanner = new List<IMethodInfoScanner>();
        private IBus _publisher;

        public void AddScanner(IMethodInfoScanner scanner)
        {
            _scanner.Add(scanner);
        }

        public void SetPublisher(IBus publisher)
        {
            _publisher = publisher;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            var groups = _scanner
                .SelectMany(s => s.GetMethodInfos(targetToAdapt))
                .Distinct()
                .GroupBy(mi => mi.Classifier).ToList();

            foreach (var group in groups)
            {
                switch (group.Key)
                {
                    case MethodInfoClassifier.Unusable:
                        continue;
                    case MethodInfoClassifier.MessageSink:
                        foreach (var mi in group)
                            yield return ConstructSubscription(mi.MethodInfo, targetToAdapt);
                        break;
                    case MethodInfoClassifier.MessageMap:
                        foreach (var mi in group)
                            yield return ConstructPublishingSubscription(mi.MethodInfo, targetToAdapt, _publisher);
                        break;
                }
            }

            
        }

        private static ISubscription ConstructPublishingSubscription(MethodInfo targetMethod, object target, IPublisher publisher)
        {
            var parameterType = targetMethod.GetParameters()[0].ParameterType;
            var fittingDelegateType = typeof(Func<,>).MakeGenericType(parameterType, typeof(object));
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

        private static ISubscription ConstructSubscription(MethodInfo info, object target)
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
    }
}