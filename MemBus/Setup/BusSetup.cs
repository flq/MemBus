using System;
using System.Collections.Generic;

namespace MemBus
{
    public class BusSetup
    {
        private readonly List<IBusSetupConfigurator> configurators = new List<IBusSetupConfigurator>();

        public BusSetup Apply<T>() where T : IBusSetupConfigurator, new()
        {
            configurators.Add(new T());
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
    }
}