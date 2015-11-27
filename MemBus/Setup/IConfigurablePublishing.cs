using System;
using MemBus.Publishing;

namespace MemBus.Setup
{
    public interface IConfigurablePublishing
    {
        void ConfigureWith<T>() where T : ISetup<IConfigurablePublishing>, new();
        void DefaultPublishPipeline(params IPublishPipelineMember[] publishPipelineMembers);
        IConfigurePipeline MessageMatch(Func<MessageInfo, bool> match);
    }

    public interface IConfigurePipeline
    {
        void ConfigureWith<T>() where T : ISetup<IConfigurePipeline>, new();
        void PublishPipeline(params IPublishPipelineMember[] publishPipelineMembers);
    }
}