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

        private class PipelineMemberToPublishAMessage : IPublishPipelineMember, IRequireBus
        {
            private readonly object _message;
            private IBus _bus;

            public PipelineMemberToPublishAMessage(object message)
            {
                _message = message;
            }

            public void LookAt(PublishToken token)
            {
                _bus.Publish(_message);
            }

            void IRequireBus.AddBus(IBus bus)
            {
                _bus = bus;
            }
        }
    }
}