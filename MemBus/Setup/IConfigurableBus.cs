using System;

namespace MemBus.Setup
{
    public interface IConfigurableBus
    {
        void ConfigurePublishing(Action<IConfigurablePublishing> configure);
        void ConfigureSubscribing(Action<IConfigurableSubscribing> configure);
        void ConfigureBubbling(Action<IConfigurableBubbling> configure);
        void AddResolver(ISubscriptionResolver resolver);
        void AddSubscription(ISubscription subscription);
        void AddAutomaton(object automaton);
        void AddService<T>(T service);
    }
}