using System;
using MemBus.Subscribing;

namespace MemBus
{
    public interface IConfigurableBus
    {
        void InsertResolver(ISubscriptionResolver resolver);
        void ConfigurePublishPipeline(Action<IConfigurablePublishPipeline> configurePipeline);
        void AddSubscription(ISubscription subscription);
        void AddAutomaton(object automaton);
        void AddService<T>(T service);
    }

    public interface IConfigurablePublishPipeline
    {
        void InsertPublishPipelineMember(IPublishPipelineMember publishPipelineMember);
    }
}