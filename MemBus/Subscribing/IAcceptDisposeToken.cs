using System;

namespace MemBus.Subscribing
{
    /// <summary>
    /// In combination with flexible subscriptions,
    /// when you implement this interface, the disposeToken
    /// will allow you to detach your instance from all
    /// messaging-based connections to MemBus
    /// </summary>
    public interface IAcceptDisposeToken
    {
        void Accept(IDisposable disposeToken);
    }
}