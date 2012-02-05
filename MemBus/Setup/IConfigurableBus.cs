using System;

namespace MemBus.Setup
{
    public interface IConfigurableSubscriber
    {
        void ConfigureSubscribing(Action<IConfigurableSubscribing> configure);
        void AddResolver(ISubscriptionResolver resolver);
        void AddSubscription(ISubscription subscription);
    }

    public interface IConfigurableBus : IConfigurableSubscriber
    {
        void ConfigurePublishing(Action<IConfigurablePublishing> configure);
        [Obsolete("This method will be removed in the near future")]
        void AddAutomaton(object automaton);
        void AddService<T>(T service);
    }
}