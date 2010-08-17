using System;

namespace MemBus.Setup
{
    public interface IConfigurableBus
    {
        void InsertResolver(ISubscriptionResolver resolver);
        void ConfigurePublishing(Action<IConfigurablePublishing> configure);
        void ConfigureSubscribing(Action<IConfigurableSubscribing> configure);
        void AddSubscription(ISubscription subscription);
        void AddAutomaton(object automaton);
        /// <summary>
        /// Usually, messages published on a spawned bus are not registered by parent buses.
        /// Add exceptions to this rule by calling this configuration method. When a message of that
        /// type is published on a child bus, it will also reach subscribers of the parent
        /// </summary>
        void AddBubblingForMessageType<T>();
        void AddService<T>(T service);
    }


}