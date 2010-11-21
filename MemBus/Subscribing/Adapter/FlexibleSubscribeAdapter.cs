using System;
using System.Collections.Generic;
using MemBus.Setup;
using System.Linq;

namespace MemBus.Subscribing
{
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
            addToBuilders(new MethodBasedBuilder(methodName));
            return this;
        }

        /// <summary>
        /// Look at an object and check it for interfaces. An interface should adhere to the following rules:
        /// Interface should define only one void method with a single parameter. 
        /// The interface may be generic and can be implemented multiple times.
        /// </summary>
        public FlexibleSubscribeAdapter ByInterface(Type interfaceType)
        {
            addToBuilders(new InterfaceBasedBuilder(interfaceType));
            return this;
        }

        IEnumerable<ISubscription> IAdapterServices.SubscriptionsFor(object subscriber)
        {
            return builders.SelectMany(b => b.BuildSubscriptions(subscriber));
        }

        private void addToBuilders(ISubscriptionBuilder builder)
        {
            builders.Add(builder);
            configurationAvailable = true;
        }
    }
}