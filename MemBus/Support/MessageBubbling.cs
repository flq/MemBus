using System;
using System.Collections.Generic;

namespace MemBus.Support
{
    internal class MessageBubbling
    {
        private readonly HashSet<Type> allowedBubblingTypes = new HashSet<Type>();
        public void AddAllowanceForMessageType<T>()
        {
            allowedBubblingTypes.Add(typeof(T));
        }

        public bool BubblingAllowed(Type messageType)
        {
            return allowedBubblingTypes.Contains(messageType);
        }
    }
}