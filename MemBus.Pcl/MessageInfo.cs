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
        private readonly object _message;

        internal MessageInfo(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            this._message = message;
        }

        /// <summary>
        /// True if the message associated with this info is asssignable to T
        /// </summary>
        public bool IsType<T>()
        {
            return _message is T;
        }

        /// <summary>
        /// Like IsType but allows you ti supply additional matching
        /// </summary>
        [Api]
        public bool IsType<T>(Func<T,bool> additionalMatch)
        {
            if (additionalMatch == null) throw new ArgumentNullException("additionalMatch");
            if (IsType<T>())
                return additionalMatch((T) _message);
            return false;
        }

        /// <summary>
        /// The Name of this message's type
        /// </summary>
        public string Name { get { return _message.GetType().Name; } }

    }
}