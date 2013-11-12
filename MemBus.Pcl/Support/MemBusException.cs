using System;

namespace MemBus.Support
{
    /// <summary>
    /// Message to denote an exception caused by Membus
    /// </summary>
    public class MemBusException : Exception
    {
        /// <summary>
        /// ctor
        /// </summary>
        public MemBusException(string message) : base(message)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public MemBusException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}