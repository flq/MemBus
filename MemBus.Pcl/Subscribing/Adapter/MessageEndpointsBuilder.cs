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
        private IBus _bus;

        public void AddScanner(IMethodInfoScanner scanner)
        {
            _scanner.Add(scanner);
        }

        public void SetPublisher(IBus publisher)
        {
            _bus = publisher;
        }

        public IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt)
        {
            var groups = _scanner
                .SelectMany(s => s.GetMethodInfos(targetToAdapt))
                .Distinct()
                .GroupBy(mi => mi.Classifier).OrderBy(g => g.Key).ToList();
            // Ordering to ensure that generated subscriptions can already listen to generated message producers.

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
                            yield return ConstructPublishingSubscription(mi.MethodInfo, targetToAdapt, _bus);
                        break;
                    case MethodInfoClassifier.ObservableSink:
                        foreach (var mi in group)
                        {
                            // Could also construct a cached method looked up by message type.
                            // However, typical usage would be a wire-once thing.
                            var msgObsInstance = ConstructMessageObservable(mi);
                            mi.MethodInfo.Invoke(targetToAdapt, new []{ msgObsInstance });
                        }
                        break;
                    case MethodInfoClassifier.ObservableSource:
                        foreach (var mi in group)
                        {
                            var observable = mi.MethodInfo.Invoke(targetToAdapt, null);
                            var busPublish = ConstructBusPublishMethod(mi);
                            busPublish.Invoke(_bus, new[] {observable});
                        }
                        break;
                    case MethodInfoClassifier.ObservableMap:
                        foreach (var mi in group)
                        {
                            var msgObsInstance = ConstructMessageObservable(mi);
                            var observable = mi.MethodInfo.Invoke(targetToAdapt, new[] { msgObsInstance });
                            var busPublish = ConstructBusPublishMethod(mi);
                            busPublish.Invoke(_bus, new[] { observable });
                        }
                        break;
                }
            }

            
        }

        private MethodInfo ConstructBusPublishMethod(ClassifiedMethodInfo mi)
        {
            var busPublish = _bus.GetType().GetRuntimeMethods()
                .First(m => m.Name == "Publish" && m.IsGenericMethod)
                .MakeGenericMethod(GetObservableMessageTypeFromReturn(mi.MethodInfo));
            return busPublish;
        }

        private object ConstructMessageObservable(ClassifiedMethodInfo mi)
        {
            var msgObs = typeof (MessageObservable<>).MakeGenericType(GetObservableMessageType(mi.MethodInfo));
            var msgObsInstance = Activator.CreateInstance(msgObs, _bus);
            return msgObsInstance;
        }

        private Type GetObservableMessageType(MethodInfo mi)
        {
            return mi.GetParameters()[0].ParameterType.GenericTypeArguments[0];
        }

        private Type GetObservableMessageTypeFromReturn(MethodInfo mi)
        {
            return mi.ReturnType.GenericTypeArguments[0];
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