using System;
using System.Runtime.Serialization;

namespace MemBus.Support
{
    [Serializable]
    public class MemBusException : Exception
    {

        public MemBusException()
        {
        }

        public MemBusException(string message) : base(message)
        {
        }

        public MemBusException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MemBusException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}