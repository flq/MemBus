using System;
using System.Collections.Generic;

namespace MemBus.Tests.Help
{
    public class MockConfigurableBus : IConfigurableBus
    {
        public MockConfigurableBus(BusSetup busSetup)
        {
            PublishPipeline = new List<IPublishPipelineMember>();
            Resolvers = new List<ISubscriptionResolver>();
            busSetup.Accept(this);
        }

        public List<IPublishPipelineMember> PublishPipeline { get; private set; }
        public List<ISubscriptionResolver> Resolvers { get; private set; }

        public void InsertResolver(ISubscriptionResolver resolver)
        {
            Resolvers.Add(resolver);
        }

        public void InsertPublishPipeline(IPublishPipelineMember publishPipelineMember)
        {
            PublishPipeline.Add(publishPipelineMember);
        }
    }
}