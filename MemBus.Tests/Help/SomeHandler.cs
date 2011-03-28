using System;
using MemBus.Subscribing;

namespace MemBus.Tests.Help
{
    public class SomeHandler : IAcceptDisposeToken
    {
        public int MsgACalls;
        private IDisposable _disposeToken;

        public void Handle(MessageA msg)
        {
            MsgACalls++;
        }

        public void InvokeDisposeToken()
        {
            if (_disposeToken != null)
                _disposeToken.Dispose();
        }

        public void Accept(IDisposable disposeToken)
        {
            _disposeToken = disposeToken;
        }
    }
}