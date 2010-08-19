using System;
using System.Collections.Generic;
using MemBus.Setup;

// ReSharper disable CheckNamespace
namespace MemBus
{
    public class BusSetup
    {
        private readonly List<ISetupConfigurator<IConfigurableBus>> configurators = new List<ISetupConfigurator<IConfigurableBus>>();

        public BusSetup Apply(params ISetupConfigurator<IConfigurableBus>[] configurators)
        {
            this.configurators.AddRange(configurators);
            return this;
        }

        public BusSetup Apply<T>(params ISetupConfigurator<IConfigurableBus>[] configurators) where T : ISetupConfigurator<IConfigurableBus>, new()
        {
            this.configurators.Add(new T());
            return Apply(configurators);
        }

        public void Accept(IConfigurableBus configurableBus)
        {
            foreach (var c in configurators)
                c.Accept(configurableBus);
        }

        public static BusSetup Start()
        {
            return new BusSetup();
        }

        public IBus Construct()
        {
            var bus = new Bus();
            Accept(bus);
            return bus;
        }

        /// <summary>
        /// Start with a configuration setup
        /// </summary>
        public static BusSetup StartWith<T>(params ISetupConfigurator<IConfigurableBus>[] configurators) where T : ISetupConfigurator<IConfigurableBus>, new()
        {
            return new BusSetup().Apply<T>(configurators);
        }

        public static BusSetup StartWith<T>(Action<IConfigurableBus> configure) where T : ISetupConfigurator<IConfigurableBus>, new()
        {
            return StartWith<T>(new AdHocConfigurator<IConfigurableBus>(configure));
        }

        public static BusSetup StartWith<T1, T2>(params ISetupConfigurator<IConfigurableBus>[] configurators)
            where T1 : ISetupConfigurator<IConfigurableBus>, new()
            where T2 : ISetupConfigurator<IConfigurableBus>, new()
        {
            return new BusSetup().Apply<T1>().Apply<T2>(configurators);
        }
    }
}
// ReSharper restore CheckNamespace