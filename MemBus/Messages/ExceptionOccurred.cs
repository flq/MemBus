using System;
using System.Text;

namespace MemBus.Messages
{
    /// <summary>
    /// Some configurations send out this message if during publishing of a Message an exception occurs
    /// </summary>
    public class ExceptionOccurred
    {
        /// <summary>
        /// The caotured exception
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// ctor with the exception to be captured
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionOccurred(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// Some exception Information
        /// </summary>
        public override string ToString()
        {
            var b = new StringBuilder();

            if (Exception is AggregateException)
            {
                var ax = (AggregateException) Exception;
                foreach (var x in ax.InnerExceptions)
                    PrintException(b, x);
            }
            else
                PrintException(b, Exception);
            
            return b.ToString();
        }

        private static void PrintException(StringBuilder b, Exception x)
        {
            b.AppendFormat("Type of Exception: {0}", x.GetType().Name);
            b.AppendFormat("Message: {0}", x.Message);
        }
    }
}