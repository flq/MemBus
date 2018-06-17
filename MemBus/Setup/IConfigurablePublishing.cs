using System;
using MemBus.Publishing;

namespace MemBus.Setup
{
    /// <summary>
    /// Configuration API to the publish pipeline
    /// See <see cref="IPublishPipelineMember"/>
    /// </summary>
    public interface IConfigurablePublishing
    {
        // ReSharper disable once UnusedMember.Global
        void ConfigureWith<T>() where T : ISetup<IConfigurablePublishing>, new();
        IConfigurablePublishing DefaultPublishPipeline(params IPublishPipelineMember[] publishPipelineMembers);
        IConfigurablePublishing DefaultAsyncPublishPipeline(params IAsyncPublishPipelineMember[] publishPipelineMembers);
        IConfigurePipeline MessageMatch(Func<MessageInfo, bool> match);
    }

    /// <summary>
    /// Configure a single publish pipeline
    /// </summary>
    public interface IConfigurePipeline
    {
        // ReSharper disable once UnusedMember.Global
        void ConfigureWith<T>() where T : ISetup<IConfigurePipeline>, new();
        void PublishPipeline(params IPublishPipelineMember[] publishPipelineMembers);
        void AsyncPublishPipeline(params IAsyncPublishPipelineMember[] publishPipelineMembers);
    }
}