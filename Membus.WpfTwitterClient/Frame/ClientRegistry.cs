using System;
using MemBus;
using MemBus.Configurators;
using Membus.WpfTwitterClient;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Twitter;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Membus.WpfTwitterClient.Frame
{
    internal class ClientRegistry : Registry
    {
        public ClientRegistry()
        {
            Scan(s =>
                     {
                         s.AssembliesFromApplicationBaseDirectory();
                         s.AddAllTypesOf(typeof (IHandles<>));
                     });
            ForSingletonOf<IBus>().Use(constructBus);
            For(typeof (IObservable<>)).Use(typeof (MessageObservable<>));
            For<IConfigReader>().Use<AppConfigReader>();
            For<TwitterKeys>().Use(ctx => ctx.GetInstance<IConfigReader>().GetSection<TwitterKeys>());
        }

        private static IBus constructBus()
        {
            return BusSetup.StartWith<AsyncRichClientFrontend, ClientPublishingConventions>(
                new ServiceLocatorSupport(new ServiceLocator(() => ObjectFactory.Container)))
                .Construct();
        }
    }
}