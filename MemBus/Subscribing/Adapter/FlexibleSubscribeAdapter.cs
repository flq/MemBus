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
        private bool _configurationAvailable;
        private readonly SubscriptionBuilder _builder = new SubscriptionBuilder();


        void ISetup<IConfigurableBus>.Accept(IConfigurableBus setup)
        {
            if (!_configurationAvailable)
                throw new InvalidOperationException("No adapter rules were set up.");
            setup.AddService<IAdapterServices>(this);

            var bus = setup as IPublisher;

            _builder.SetPublisher(bus);
        }

        /// <summary>
        /// Look at an object and look for methods with the provided name. The method must be void
        /// and accept a single parameter
        /// </summary>
        public FlexibleSubscribeAdapter ByMethodName(string methodName)
        {
            AddToScanners(MethodScanner.ForVoidMethods(methodName));
            return this;
        }

        /// <summary>
        /// Look at an object and look for methods with the provided name. The method must NOT be void
        /// and accept a single parameter. The returning object will be treated as a message and subsequently be published
        /// </summary>
        public FlexibleSubscribeAdapter PublishMethods(string methodName)
        {
            AddToScanners(MethodScanner.ForNonVoidMethods(methodName));
            return this;
        }

        /// <summary>
        /// Look at an object and scan the available methods. For those where the methodSelector returns true,
        /// subscriptions will be created and registered in MemBus. The methods are already pre-filtered for those
        /// accepting a single parameter, being public, as well as allowing only the METHODS DECLARED ON THE TYPE OF THE INSPECTED OBJECT.
        /// Additionally, if your subscribing object implements <see cref="IAcceptDisposeToken"/>, any method that looks like the implementation
        /// of said interface will be ignored
        /// This function does not make a difference between void methods and methods returning a value.
        /// These will be registered as publishing methods or simple subscriptions.
        /// </summary>
        /// <param name="methodSelector">the method selector predicate</param>
        public FlexibleSubscribeAdapter PickUpMethods(Func<MethodInfo,bool> methodSelector)
        {
            #if WINRT
            AddToScanners(new MethodScanner(methodSelector, returnType => true));
            #else
            AddToScanners(new MethodScanner(methodSelector, returnType => true, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            #endif
            return this;
        }

        /// <summary>
        /// Look at an object and check it for interfaces. An interface should adhere to the following rules:
        /// Interface should define only one void method with a single parameter. 
        /// The interface may be generic and can be implemented multiple times.
        /// </summary>
        public FlexibleSubscribeAdapter ByInterface(Type interfaceType)
        {
            AddToScanners(new InterfaceBasedBuilder(interfaceType));
            return this;
        }

        private IEnumerable<ISubscription> SubscriptionsFor(object subscriber)
        {
            return _builder.BuildSubscriptions(subscriber);
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

        private void AddToScanners(IMethodInfoScanner builder)
        {
            _builder.AddScanner(builder);
            _configurationAvailable = true;
        }

        private static void PushDisposerToSubscriberIfPossible(object subscriber, DisposeContainer disposeContainer)
        {
            var disposeAcceptor = subscriber as IAcceptDisposeToken;
            if (disposeAcceptor != null)
                disposeAcceptor.Accept(disposeContainer);
        }
    }
}