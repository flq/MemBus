using System;
using MemBus.Setup;

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
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// True if the message associated with this info is asssignable to T
        /// </summary>
        public bool IsType<T>() => _message is T;

        /// <summary>
        /// Like IsType but allows you ti supply additional matching
        /// </summary>
        public bool IsType<T>(Func<T,bool> additionalMatch)
        {
            if (additionalMatch == null) throw new ArgumentNullException(nameof(additionalMatch));
            return IsType<T>() && additionalMatch((T) _message);
        }

        /// <summary>
        /// The Name of this message's type
        /// </summary>
        public string Name => _message.GetType().Name;

    }
}