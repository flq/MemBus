using System;
using System.Collections.Generic;
using System.Reflection;
using MemBus.Setup;
using System.Linq;
using MemBus.Support;

namespace MemBus.Subscribing
{
    internal interface IAdapterServices
    {
        IDisposable WireUpSubscriber(ISubscriptionResolver subscriptionResolver, object subscriber);
    }

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

            var bus = setup as IPublisher;

            foreach (var b in builders)
                b.TryInvoke(obj => obj.SetPublisher(bus));
        }

        /// <summary>
        /// Look at an object and look for methods with the provided name. The method must be void
        /// and accept a single parameter
        /// </summary>
        public FlexibleSubscribeAdapter ByMethodName(string methodName)
        {
            AddToBuilders(new VoidMethodBasedBuilder(methodName));
            return this;
        }

        /// <summary>
        /// Look at an object and look for methods with the provided name. The method must NOT be void
        /// and accept a single parameter. The returning object will be treated as a message and subsequently be published
        /// </summary>
        public FlexibleSubscribeAdapter PublishMethods(string methodName)
        {
            AddToBuilders(new ReturningMethodBasedBuilder(methodName));
            return this;
        }

        /// <summary>
        /// Look at an object and scan the available methods. FOr those where the methodSelector returntrs true,
        /// subscriptions will be created and registered in MemBus. The methods are already pre-filtered for those
        /// accepting a single parameter. This function does not make a difference between void methods and methods returning a value.
        /// These will be registered as publishing methods or simple subscriptions.
        /// 
        /// </summary>
        /// <param name="methodSelector">the method selector predicate</param>
        public FlexibleSubscribeAdapter PickUpMethods(Func<MethodInfo,bool> methodSelector)
        {
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