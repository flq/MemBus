using System;

namespace MemBus.Subscribing
{
    public interface IAcceptDisposeToken
    {
        void Accept(IDisposable disposeToken);
    }
}