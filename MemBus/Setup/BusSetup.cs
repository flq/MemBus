using System;
using System.Collections.Generic;

namespace MemBus
{
    public class BusSetup
    {
        private readonly List<IBusSetupConfigurator> configurators = new List<IBusSetupConfigurator>();

        public BusSetup Apply<T>(params IBusSetupConfigurator[] configurators) where T : IBusSetupConfigurator, new()
        {
            this.configurators.Add(new T());
            this.configurators.AddRange(configurators);
            return this;
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
        public static BusSetup StartWith<T>(params IBusSetupConfigurator[] configurators) where T : IBusSetupConfigurator, new()
        {
            return new BusSetup().Apply<T>(configurators);
            
        }

        public static BusSetup StartWith<T1, T2>(params IBusSetupConfigurator[] configurators)
            where T1 : IBusSetupConfigurator, new()
            where T2 : IBusSetupConfigurator, new()
        {
            return new BusSetup().Apply<T1>().Apply<T2>(configurators);
        }
    }
}