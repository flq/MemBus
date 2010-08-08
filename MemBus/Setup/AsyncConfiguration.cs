﻿using MemBus.Messages;
using MemBus.Subscribing;

namespace MemBus
{
    /// <summary>
    /// A parallel publisher is used that publishes messages in parallel. With this setup <see cref="IBus.Publish"/> does NOT block.
    /// Exceptions will become available once all subscriptions are done processing the message as <see cref="ExceptionOccurred"/> message.
    /// </summary>
    public class AsyncConfiguration : IBusSetupConfigurator
    {
        public virtual void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishPipeline(p => p.InsertPublishPipelineMember(new ParallelNonBlockingPublisher()));
            setup.InsertResolver(new TableBasedResolver());
            setup.AddService(new SubscriptionMatroschkaFactory { new ShapeToDispose() });
        }
    }
}