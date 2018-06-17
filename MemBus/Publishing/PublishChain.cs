using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemBus.Publishing
{
    internal class DefaultPublishChain : PublishChain
    {
        public DefaultPublishChain(IEnumerable<IPublishPipelineMember> members) : base(_ => true, members)
        {
        }
        
        public DefaultPublishChain(IEnumerable<IAsyncPublishPipelineMember> members) : base(_ => true, members)
        {
        }
    }

    internal class PublishChain
    {
        private readonly Func<MessageInfo, bool> _match;
        private readonly List<IPublishPipelineMember> _pipelineMembers = new List<IPublishPipelineMember>();
        private readonly List<IAsyncPublishPipelineMember> _asyncpipelineMembers =
            new List<IAsyncPublishPipelineMember>();

        public PublishChain(Func<MessageInfo, bool> match) : this(match, new IPublishPipelineMember[] { })
        {
        }


        public PublishChain(Func<MessageInfo, bool> match, IEnumerable<IPublishPipelineMember> members)
        {
            _match = match;
            _pipelineMembers.AddRange(members);
        }

        public PublishChain(Func<MessageInfo, bool> match, IEnumerable<IAsyncPublishPipelineMember> members)
        {
            _match = match;
            _asyncpipelineMembers.AddRange(members);
        }

        public bool Handles(MessageInfo msgInfo)
        {
            return _match(msgInfo);
        }

        public void LookAt(PublishToken token)
        {
            for (var i = 0; i <= _pipelineMembers.Count - 1; i++)
            {
                if (token.Cancel)
                    break;
                _pipelineMembers[i].LookAt(token);
            }
        }
        
        public async Task LookAtAsync(AsyncPublishToken token)
        {
            for (var i = 0; i <= _asyncpipelineMembers.Count - 1; i++)
            {
                if (token.Cancel)
                    break;
                await _asyncpipelineMembers[i].LookAtAsync(token);
            }
        }

        public void Add(IPublishPipelineMember publishPipelineMember)
        {
            _pipelineMembers.Add(publishPipelineMember);
        }

        public void Add(IAsyncPublishPipelineMember publishPipelineMember)
        {
            _asyncpipelineMembers.Add(publishPipelineMember);
        }
    }
}