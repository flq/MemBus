using System;
using MemBus.Setup;
using MemBus.Support;

namespace MemBus
{

    /// <summary>
    /// Used to specify filters in the context of <see cref="IConfigurablePublishing"/> and <see cref="IConfigurableSubscribing"/>
    /// </summary>
    public struct MessageInfo
    {
        private readonly object message;

        internal MessageInfo(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            this.message = message;
        }

        /// <summary>
        /// True if the message associated with this info is asssignable to T
        /// </summary>
        public bool IsType<T>()
        {
            return typeof(T).IsAssignableFrom(message.GetType());
        }

        /// <summary>
        /// Like IsType but allows you ti supply additional matching
        /// </summary>
        [Api]
        public bool IsType<T>(Func<T,bool> additionalMatch)
        {
            if (additionalMatch == null) throw new ArgumentNullException("additionalMatch");
            if (IsType<T>())
                return additionalMatch((T) message);
            return false;
        }

        /// <summary>
        /// The Name of this message's type
        /// </summary>
        public string Name { get { return message.GetType().Name; } }

    }
}