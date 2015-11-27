using System;
using System.Collections.Generic;
using MemBus.Configurators;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

// ReSharper disable CheckNamespace
namespace MemBus
{
    /// <summary>
    /// Your main entry point to set up an <see cref="IBus"/> instance. Use the different Apply overloads and
    /// at the end call <see cref="Construct"/>
    /// </summary>
    public class BusSetup
    {
        private readonly List<ISetup<IConfigurableBus>> _configurators = new List<ISetup<IConfigurableBus>>();

        private BusSetup() { }

        /// <summary>
        /// Apply an arbitrary number of <see cref="ISetup{T}"/> of <see cref="IConfigurableBus"/> instances.
        /// Examples are <see cref="Conservative"/> or <see cref="AsyncConfiguration"/>
        /// </summary>
        [Api]
        public BusSetup Apply(params ISetup<IConfigurableBus>[] configurators)
        {
            _configurators.AddRange(configurators);
            return this;
        }

        
        /// <summary>
        /// Apply variant where you specify a configurator as type, and others as instance
        /// </summary>
        [Api]
        public BusSetup Apply<T>(params ISetup<IConfigurableBus>[] configurators) where T : ISetup<IConfigurableBus>, new()
        {
            _configurators.Add(new T());
            return Apply(configurators);
        }

        /// <summary>
        /// Apply a configurator that requires additional configuration which you can provide through the additionalConfig
        /// parameter. One example for this pattern is the <see cref="FlexibleSubscribeAdapter"/>
        /// </summary>
        public BusSetup Apply<T>(Action<T> additionalConfig) where T : ISetup<IConfigurableBus>, new()
        {
            var t = new T();
            additionalConfig(t);
            return Apply(t);
        }

        /// <summary>
        /// Directly access the configuration interface of the bus.
        /// </summary>
        public BusSetup Apply(Action<IConfigurableBus> adHocConfig)
        {
            var t = new AdHocConfigurator<IConfigurableBus>(adHocConfig);        
            return Apply(t);
        }

        /// <summary>
        /// Once you have applied a number of customizations, call this to obtain the <see cref="IBus"/> instance to be used by your App.
        /// </summary>
        public IBus Construct()
        {
            var bus = new Bus();
            Accept(bus);
            return bus;
        }

        /// <summary>
        /// Start with a configuration setup like e.g. <see cref="Conservative"/> or <see cref="Fast"/>
        /// </summary>
        public static BusSetup StartWith<T>(params ISetup<IConfigurableBus>[] configurators) where T : ISetup<IConfigurableBus>, new()
        {
            return new BusSetup().Apply<T>(configurators);
        }

        /// <summary>
        /// Start with some Ad-Hoc configuration
        /// </summary>
        public static BusSetup StartWith<T>(Action<IConfigurableBus> configure) where T : ISetup<IConfigurableBus>, new()
        {
            return StartWith<T>(new AdHocConfigurator<IConfigurableBus>(configure));
        }

        /// <summary>
        /// Apply two configurators as type arguments
        /// </summary>
        public static BusSetup StartWith<T1, T2>(params ISetup<IConfigurableBus>[] configurators)
            where T1 : ISetup<IConfigurableBus>, new()
            where T2 : ISetup<IConfigurableBus>, new()
        {
            return new BusSetup().Apply<T1>().Apply<T2>(configurators);
        }

        private void Accept(IConfigurableBus configurableBus)
        {
            foreach (var c in _configurators)
                c.Accept(configurableBus);
        }
    }
}
// ReSharper restore CheckNamespace