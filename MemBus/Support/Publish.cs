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
            private readonly object _message;
            private IBus _bus;

            public PipelineMemberToPublishAMessage(object message)
            {
                this._message = message;
            }

            public IBus Bus { set { _bus = value; }}

            public void LookAt(PublishToken token)
            {
                _bus.Publish(_message);
            }
        }
    }
}