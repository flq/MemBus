using System;
using System.Collections.Generic;

namespace MemBus
{
    public class Bus : IConfigurableBus
    {
        private readonly List<ISubscriptionResolver> resolvers = new List<ISubscriptionResolver>();
        private readonly List<IPublishPipelineMember> pipelineMembers = new List<IPublishPipelineMember>();

        void IConfigurableBus.InsertResolver(ISubscriptionResolver resolver)
        {
            resolvers.Add(resolver);
        }

        void IConfigurableBus.InsertPublishPipeline(IPublishPipelineMember publishPipelineMember)
        {
            pipelineMembers.Add(publishPipelineMember);
        }
    }
}