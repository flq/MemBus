using System;

namespace MemBus.Messages
{
    public class ExceptionOccurred
    {
        public Exception Exception { get; private set; }

        public ExceptionOccurred(Exception exception)
        {
            Exception = exception;
        }
    }
}