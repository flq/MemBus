using System;
using System.Collections;
using System.Collections.Generic;
using MemBus.Support;

namespace MemBus
{
    public class PublishPipeline : IEnumerable<IPublishPipelineMember>, IConfigurablePublishPipeline
    {
        private readonly List<IPublishPipelineMember> members = new List<IPublishPipelineMember>();
        private readonly IBus bus;

        public PublishPipeline(IBus bus)
        {
            this.bus = bus;
        }

        public IEnumerator<IPublishPipelineMember> GetEnumerator()
        {
            return members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IPublishPipelineMember publishPipelineMember)
        {
            publishPipelineMember.TryInvoke(p => p.Bus = bus);
            members.Add(publishPipelineMember);
        }

        public void LookAt(PublishToken token)
        {
            foreach (var m in members)
                m.LookAt(token);
        }

        void IConfigurablePublishPipeline.InsertPublishPipelineMember(IPublishPipelineMember publishPipelineMember)
        {
            Add(publishPipelineMember);
        }
    }
}