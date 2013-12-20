using System;
using System.Collections;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Basic <see cref="ISubscription"/> implementation that wraps a object(Arg a) method
    /// as a subscription, and, when invoking the method, using the return argument
    /// as a message to be published
    /// </summary>
    public class PublishingMethodInvocation<T> : ISubscription, IKnowsSubscribedInstance
    {
        private readonly Func<T,object> _action;
        private readonly IPublisher _publisher;

        /// <summary>
        /// ctor for any delegate. Can fail with <see cref="InvalidCastException"/>
        /// </summary>
        public PublishingMethodInvocation(Delegate action, IPublisher publisher) : this((Func<T,object>)action, publisher)
        {
        }

        /// <summary>
        /// ctor for <see cref="Func{T,T2}"/> and a publisher
        /// </summary>
        public PublishingMethodInvocation(Func<T, object> action, IPublisher publisher)
        {
            _action = action;
            _publisher = publisher;
        }

        /// <summary>
        /// <see cref="ISubscription.Push"/>
        /// </summary>
        public void Push(object message)
        {
            var obj = _action((T)message);
            if (obj == null)
                return;
            var msgs = obj as IEnumerable;
            if (msgs != null)
                foreach (var msg in msgs)
                    _publisher.Publish(msg);
            else
                _publisher.Publish(obj);
        }

        /// <summary>
        /// <see cref="ISubscription.Handles"/>
        /// </summary>
        public bool Handles(Type messageType)
        {
            return messageType.CanBeCastTo<T>();
        }

        object IKnowsSubscribedInstance.Instance
        {
            get
            {
                return _action.ExtractTarget();
            }
        }
    }
}