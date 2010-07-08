using System.Collections;
using System.Collections.Generic;

namespace MemBus
{
    public class PublishPipeline : IEnumerable<IPublishPipelineMember>
    {
        private readonly List<IPublishPipelineMember> members = new List<IPublishPipelineMember>();

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
            members.Add(publishPipelineMember);
        }

        public void LookAt(PublishToken token)
        {
            foreach (var m in members)
                m.LookAt(token);
        }
    }
}