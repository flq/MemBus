using System;
using System.Text;

namespace MemBus.Messages
{
    public class ExceptionOccurred
    {
        public Exception Exception { get; private set; }

        public ExceptionOccurred(Exception exception)
        {
            Exception = exception;
        }

        public override string ToString()
        {
            var b = new StringBuilder();

            if (Exception is AggregateException)
            {
                var ax = (AggregateException) Exception;
                foreach (var x in ax.InnerExceptions)
                    printException(b, x);
            }
            else
                printException(b, Exception);
            
            return b.ToString();
        }

        private static void printException(StringBuilder b, Exception x)
        {
            b.AppendFormat("Type of Exception: {0}", x.GetType().Name);
            b.AppendFormat("Message: {0}", x.Message);
        }
    }
}