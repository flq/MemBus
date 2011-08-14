using System;
using System.Collections.Generic;
using MemBus.Setup;
using System.Linq;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Add this through the <see cref="BusSetup"/> to your configuration to support subscribing objects with the 
    /// <see cref="ISubscriber.Subscribe(object)"/>, <see cref="ISubscriber.Subscribe(object)"/> overload.
    /// </summary>
    public class FlexibleSubscribeAdapter : ISetup<IConfigurableBus>, IAdapterServices
    {
        private bool configurationAvailable;
        private readonly List<ISubscriptionBuilder> builders = new List<ISubscriptionBuilder>();


        void ISetup<IConfigurableBus>.Accept(IConfigurableBus setup)
        {
            if (!configurationAvailable)
                throw new InvalidOperationException("No adapter rules were set up.");
            setup.AddService<IAdapterServices>(this);
        }

        /// <summary>
        /// Look at an object and look for methods with the provided name. The method must be void
        /// and accept a single parameter
        /// </summary>
        public FlexibleSubscribeAdapter ByMethodName(string methodName)
        {
            AddToBuilders(new MethodBasedBuilder(methodName));
            return this;
        }

        /// <summary>
        /// Look at an object and check it for interfaces. An interface should adhere to the following rules:
        /// Interface should define only one void method with a single parameter. 
        /// The interface may be generic and can be implemented multiple times.
        /// </summary>
        public FlexibleSubscribeAdapter ByInterface(Type interfaceType)
        {
            AddToBuilders(new InterfaceBasedBuilder(interfaceType));
            return this;
        }

        private IEnumerable<ISubscription> SubscriptionsFor(object subscriber)
        {
            return builders.SelectMany(b => b.BuildSubscriptions(subscriber));
        }


        IDisposable IAdapterServices.WireUpSubscriber(ISubscriptionResolver subscriptionResolver, object subscriber)
        {
            var disposeShape = new ShapeToDispose();
            var subs = SubscriptionsFor(subscriber).Select(disposeShape.EnhanceSubscription).ToList();
            foreach (var s in subs)
                subscriptionResolver.Add(s);

            var disposeContainer = new DisposeContainer(subs.Select(s => ((IDisposableSubscription)s).GetDisposer()));

            PushDisposerToSubscriberIfPossible(subscriber, disposeContainer);

            return disposeContainer;
        }

        private void AddToBuilders(ISubscriptionBuilder builder)
        {
            builders.Add(builder);
            configurationAvailable = true;
        }

        private static void PushDisposerToSubscriberIfPossible(object subscriber, DisposeContainer disposeContainer)
        {
            var disposeAcceptor = subscriber as IAcceptDisposeToken;
            if (disposeAcceptor != null)
                disposeAcceptor.Accept(disposeContainer);
        }
    }
}