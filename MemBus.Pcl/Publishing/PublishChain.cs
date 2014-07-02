using System;
using System.Collections.Generic;

namespace MemBus.Publishing
{
    internal class DefaultPublishChain : PublishChain
    {
        public DefaultPublishChain(IEnumerable<IPublishPipelineMember> members) : base(_ => true, members)
        {
        }
    }

    internal class PublishChain
    {
        private readonly Func<MessageInfo, bool> _match;
        private readonly List<IPublishPipelineMember> _pipelineMembers = new List<IPublishPipelineMember>();

        public PublishChain(Func<MessageInfo, bool> match) : this(match, new IPublishPipelineMember[] {}) {}

        public PublishChain(Func<MessageInfo, bool> match, IEnumerable<IPublishPipelineMember> members)
        {
            _match = match;
            _pipelineMembers.AddRange(members);
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

        public void Add(IPublishPipelineMember publishPipelineMember)
        {
            _pipelineMembers.Add(publishPipelineMember);
        }
    }
}