using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Support;

namespace MemBus.Publishing
{
    public class PublishPipeline : IConfigurablePublishing, IDisposable
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

        IConfigurePipeline IConfigurablePublishing.MessageMatch(Func<MessageInfo, bool> match)
        {
            var cP = new ConfigurePipeline(match, bus);
            pipelines.Add(cP.Provider);
            return cP;
        }

        void IConfigurablePublishing.ConfigureWith<T>()
        {
            var t = new T();
            t.Accept(this);
        }

        private class ConfigurePipeline : IConfigurePipeline
        {
            private readonly Func<MessageInfo, bool> match;
            private readonly IBus bus;
            private readonly PipelineProvider pipelineProvider;

            public ConfigurePipeline(Func<MessageInfo, bool> match, IBus bus)
            {
                this.match = match;
                this.bus = bus;
                pipelineProvider = new PipelineProvider(match);
            }

            public PipelineProvider Provider { get { return pipelineProvider; } }

            public void ConfigureWith<T>() where T : ISetup<IConfigurePipeline>, new()
            {
                var t = new T();
                t.Accept(this);
            }

            public void PublishPipeline(params IPublishPipelineMember[] publishPipelineMembers)
            {
                foreach (var m in publishPipelineMembers)
                {
                    m.TryInvoke(p => p.Bus = bus);
                    Provider.Add(m);
                }
            }
        }

        public void Dispose()
        {
            pipelines.Clear();
        }
    }
}