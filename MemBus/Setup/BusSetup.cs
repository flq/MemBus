using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Support;

// ReSharper disable CheckNamespace
namespace MemBus
{
    public class BusSetup
    {
        private readonly List<ISetup<IConfigurableBus>> configurators = new List<ISetup<IConfigurableBus>>();

        private BusSetup() { }

        [Api]
        public BusSetup Apply(params ISetup<IConfigurableBus>[] configurators)
        {
            this.configurators.AddRange(configurators);
            return this;
        }

        [Api]
        public BusSetup Apply<T>(params ISetup<IConfigurableBus>[] configurators) where T : ISetup<IConfigurableBus>, new()
        {
            this.configurators.Add(new T());
            return Apply(configurators);
        }

        public BusSetup Apply<T>(Action<T> additionalConfig) where T : ISetup<IConfigurableBus>, new()
        {
            var t = new T();
            additionalConfig(t);
            return Apply(t);
        }

        public IBus Construct()
        {
            var bus = new Bus();
            Accept(bus);
            return bus;
        }

        public IBus ConstructCloneable()
        {
            var bus = new Bus(this);
            Accept(bus);
            return bus;
        }

        /// <summary>
        /// Start with a configuration setup
        /// </summary>
        public static BusSetup StartWith<T>(params ISetup<IConfigurableBus>[] configurators) where T : ISetup<IConfigurableBus>, new()
        {
            return new BusSetup().Apply<T>(configurators);
        }

        public static BusSetup StartWith<T>(Action<IConfigurableBus> configure) where T : ISetup<IConfigurableBus>, new()
        {
            return StartWith<T>(new AdHocConfigurator<IConfigurableBus>(configure));
        }

        public static BusSetup StartWith<T1, T2>(params ISetup<IConfigurableBus>[] configurators)
            where T1 : ISetup<IConfigurableBus>, new()
            where T2 : ISetup<IConfigurableBus>, new()
        {
            return new BusSetup().Apply<T1>().Apply<T2>(configurators);
        }

        internal void Accept(IConfigurableBus configurableBus)
        {
            foreach (var c in configurators)
                c.Accept(configurableBus);
        }
    }
}
// ReSharper restore CheckNamespace