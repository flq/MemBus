using System;
using System.Threading;
using MemBus;
using MemBus.Configurators;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Membus.Tests.WpfClient
{
    internal class ClientRegistry : Registry
    {
        public ClientRegistry()
        {
            ForSingletonOf<IBus>().Use(constructBus);
            For(typeof (IObservable<>)).Use(typeof (MessageObservable<>));
        }

        private static IBus constructBus()
        {
            return BusSetup.StartWith<AsyncRichClientFrontend, ClientPublishingConventions>(
                new ServiceLocatorSupport(new ServiceLocator(() => ObjectFactory.Container)))
                .Construct();
        }
    }
}