using System;
using System.Runtime.Serialization;

namespace MemBus.Support
{
    [Serializable]
    public class InvalidAutomatonException : Exception
    {

        public InvalidAutomatonException()
        {
        }

        public InvalidAutomatonException(string message) : base(message)
        {
        }

        public InvalidAutomatonException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidAutomatonException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}