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
        private readonly List<PublishChain> _asyncPipelines = new List<PublishChain>();
        private readonly IBus _bus;

        public PublishChainCasing(IBus bus)
        {
            _bus = bus;
        }

        public void LookAt(PublishToken token)
        {
            var info = new MessageInfo(token.Message);
            //Backwards as the default pipeline lives at index 0
            for (int i = _pipelines.Count - 1; i >= 0; i--)
            {
                if (!_pipelines[i].Handles(info))
                    continue;
                _pipelines[i].LookAt(token);
                break;
            }
        }

        public async Task LookAtAsync(AsyncPublishToken token)
        {
            var info = new MessageInfo(token.Message);
            //Backwards as the default pipeline lives at index 0
            for (int i = _asyncPipelines.Count - 1; i >= 0; i--)
            {
                if (!_asyncPipelines[i].Handles(info))
                    continue;
                await _asyncPipelines[i].LookAtAsync(token);
                break;
            }
        }


        IConfigurablePublishing IConfigurablePublishing.DefaultPublishPipeline(
            params IPublishPipelineMember[] publishPipelineMembers)
        {
            foreach (var m in publishPipelineMembers.OfType<IRequireBus>())
                m.AddBus(_bus);
            if (_pipelines.Count > 0 && _pipelines[0] is DefaultPublishChain)
                _pipelines.RemoveAt(0);
            _pipelines.Insert(0, new DefaultPublishChain(publishPipelineMembers));
            return this;
        }

        IConfigurablePublishing IConfigurablePublishing.DefaultAsyncPublishPipeline(
            params IAsyncPublishPipelineMember[] publishPipelineMembers)
        {
            if (_asyncPipelines.Count > 0 && _asyncPipelines[0] is DefaultPublishChain)
                _asyncPipelines.RemoveAt(0);
            _asyncPipelines.Insert(0, new DefaultPublishChain(publishPipelineMembers));
            return this;
        }

        IConfigurePipeline IConfigurablePublishing.MessageMatch(Func<MessageInfo, bool> match)
        {
            var cP = new ConfigurePipeline(
                match, 
                _bus,
                chain => _pipelines.Add(chain),
                chain => _asyncPipelines.Add(chain));
            return cP;
        }

        void IConfigurablePublishing.ConfigureWith<T>()
        {
            var t = new T();
            t.Accept(this);
        }

        private class ConfigurePipeline : IConfigurePipeline
        {
            private readonly Func<MessageInfo, bool> _match;
            private readonly IBus _bus;
            private readonly Action<PublishChain> _addSync;
            private readonly Action<PublishChain> _addAsync;

            public ConfigurePipeline(
                Func<MessageInfo, bool> match, 
                IBus bus,
                Action<PublishChain> addSync,
                Action<PublishChain> addAsync)
            {
                _match = match;
                _bus = bus;
                _addSync = addSync;
                _addAsync = addAsync;   
            }

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
                }
                _addSync(new PublishChain(_match, publishPipelineMembers));
            }

            public void AsyncPublishPipeline(params IAsyncPublishPipelineMember[] publishPipelineMembers)
            {
                foreach (var m in publishPipelineMembers)
                {
                    m.Being<IRequireBus>(_ => _.AddBus(_bus));
                }
                _addAsync(new PublishChain(_match, publishPipelineMembers));
            }
        }

        public void Dispose()
        {
            _pipelines.Clear();
        }
    }
}