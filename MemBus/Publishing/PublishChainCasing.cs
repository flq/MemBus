using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemBus.Setup;
using MemBus.Support;
using System.Linq;

namespace MemBus.Publishing
{
    internal class PublishChainCasing : IConfigurablePublishing, IDisposable
    {
        private readonly List<PublishChain> _pipelines = new List<PublishChain>();
        private readonly IBus _bus;

        public PublishChainCasing(IBus bus)
        {
            _bus = bus;
        }

        public void LookAt(PublishToken token)
        {
            var info = new MessageInfo(token.Message);
            for (int i = _pipelines.Count - 1; i >= 0; i--) //Backwards as we keep the default at index 0
            {
                if (!_pipelines[i].Handles(info))
                    continue;
                _pipelines[i].LookAt(token);
                break;
            }
        }

        public async Task LookAtAsync(PublishToken token)
        {
            IAsyncPublishPipelineMember publisher = new SequentialPublisher();
            await publisher.LookAtAsync(token);
        }


        void IConfigurablePublishing.DefaultPublishPipeline(params IPublishPipelineMember[] publishPipelineMembers)
        {
            foreach (var m in publishPipelineMembers.OfType<IRequireBus>())
              m.AddBus(_bus);
            if (_pipelines.Count > 0 && _pipelines[0] is DefaultPublishChain)
                _pipelines.RemoveAt(0);
            _pipelines.Insert(0, new DefaultPublishChain(publishPipelineMembers));
        }

        IConfigurePipeline IConfigurablePublishing.MessageMatch(Func<MessageInfo, bool> match)
        {
            var cP = new ConfigurePipeline(match, _bus);
            _pipelines.Add(cP.Provider);
            return cP;
        }

        void IConfigurablePublishing.ConfigureWith<T>()
        {
            var t = new T();
            t.Accept(this);
        }

        private class ConfigurePipeline : IConfigurePipeline
        {
            private readonly IBus _bus;
            private readonly PublishChain _publishChain;

            public ConfigurePipeline(Func<MessageInfo, bool> match, IBus bus)
            {
                _bus = bus;
                _publishChain = new PublishChain(match);
            }

            public PublishChain Provider { get { return _publishChain; } }

            public void ConfigureWith<T>() where T : ISetup<IConfigurePipeline>, new()
            {
                var t = new T();
                t.Accept(this);
            }

            public void PublishPipeline(params IPublishPipelineMember[] publishPipelineMembers)
            {
                foreach (var m in publishPipelineMembers)
                {
                    m.Being<IRequireBus>(_ => _.AddBus(_bus));
                    Provider.Add(m);
                }
            }
        }

        public void Dispose()
        {
            _pipelines.Clear();
        }
    }
}