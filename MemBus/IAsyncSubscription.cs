using System;
using System.Threading.Tasks;

namespace MemBus
{
    /// <summary>
    /// Basic IAsyncSubscription interface
    /// </summary>
    public interface IAsyncSubscription
    {
        /// <summary>
        /// Accept the message
        /// </summary>
        Task PushAsync(object message);

        /// <summary>
        /// State whether this type of message is handled
        /// </summary>
        bool Handles(Type messageType);
    }
}