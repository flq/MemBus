using System;
using MemBus.Publishing;

namespace MemBus.Support
{
    public class Publish
    {
        /// <summary>
        /// Provides a publish pipeline member that will publish a provided message
        /// </summary>
        public static IPublishPipelineMember This(object message)
        {
            return new PipelineMemberToPublishAMessage(message);
        }

        /// <summary>
        /// Public because the TryInvoke currently does not handle private types
        /// </summary>
        [MustBePublic]
        public class PipelineMemberToPublishAMessage : IPublishPipelineMember
        {
            private readonly object message;
            private IBus bus;

            public PipelineMemberToPublishAMessage(object message)
            {
                this.message = message;
            }

            public IBus Bus { set { bus = value; }}

            public void LookAt(PublishToken token)
            {
                bus.Publish(message);
            }
        }
    }
}