using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Support;

namespace MemBus.Publishing
{
    public class PublishPipeline : IConfigurablePublishing
    {
        private readonly List<PipelineProvider> pipelines = new List<PipelineProvider>();
        private readonly IBus bus;

        public PublishPipeline(IBus bus)
        {
            this.bus = bus;
        }

        public void LookAt(PublishToken token)
        {
            var info = new MessageInfo(token.Message);
            for (int i = pipelines.Count - 1; i >= 0; i--) //Backwards as we keep the default at index 0
            {
                if (!pipelines[i].Handles(info))
                    continue;
                pipelines[i].LookAt(token);
                break;
            }
        }

        void IConfigurablePublishing.DefaultPublishPipeline(params IPublishPipelineMember[] publishPipelineMembers)
        {
            foreach (var m in publishPipelineMembers)
              m.TryInvoke(p => p.Bus = bus);
            pipelines.Insert(0, new PipelineProvider(info=>true, publishPipelineMembers));
        }

        void IConfigurablePublishing.MessageMatch(Func<MessageInfo, bool> match, Action<IConfigurePipeline> configure)
        {
            var cP = new ConfigurePipeline(match);
            configure(cP);
            pipelines.Add(cP.Provider);
        }

        void IConfigurablePublishing.ConfigureWith<T>()
        {
            var t = new T();
            t.Accept(this);
        }

        private class ConfigurePipeline : IConfigurePipeline
        {
            private readonly Func<MessageInfo, bool> match;
            private readonly List<IPublishPipelineMember> members = new List<IPublishPipelineMember>();

            public ConfigurePipeline(Func<MessageInfo, bool> match)
            {
                this.match = match;
            }

            public PipelineProvider Provider { get { return new PipelineProvider(match, members); } }

            public void ConfigureWith<T>() where T : ISetupConfigurator<IConfigurePipeline>, new()
            {
                var t = new T();
                t.Accept(this);
            }

            public void PublishPipeline(params IPublishPipelineMember[] publishPipelineMembers)
            {
                members.AddRange(publishPipelineMembers);
            }
        }
    }
}