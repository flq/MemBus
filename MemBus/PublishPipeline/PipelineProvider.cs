using System;
using System.Collections.Generic;

namespace MemBus
{
    internal class PipelineProvider
    {
        private readonly Func<MessageInfo, bool> match;
        private readonly List<IPublishPipelineMember> pipelineMembers = new List<IPublishPipelineMember>();

        public PipelineProvider(Func<MessageInfo, bool> match, IEnumerable<IPublishPipelineMember> members)
        {
            this.match = match;
            pipelineMembers.AddRange(members);
        }

        public bool Handles(MessageInfo msgInfo)
        {
            return match(msgInfo);
        }

        public void LookAt(PublishToken token)
        {
            for (var i = 0; i <= pipelineMembers.Count - 1; i++)
                pipelineMembers[i].LookAt(token);
        }


    }
}