using System;
using System.Collections.Generic;
using MemBus.Setup;

namespace MemBus.Support
{
    internal class MessageBubbling : IConfigurableBubbling
    {
        private readonly HashSet<Type> allowedBubblingTypes = new HashSet<Type>();
        private readonly HashSet<Type> blockedDescents = new HashSet<Type>();


        public bool BubblingAllowed(Type messageType)
        {
            return allowedBubblingTypes.Contains(messageType);
        }

        public bool DescentAllowed(Type messageType)
        {
            return !blockedDescents.Contains(messageType);
        }

        void IConfigurableBubbling.BubblingForMessage<T>()
        {
            allowedBubblingTypes.Add(typeof(T));
        }

        void IConfigurableBubbling.BlockDescentOfMessage<T>()
        {
            blockedDescents.Add(typeof (T));
        }
    }
}