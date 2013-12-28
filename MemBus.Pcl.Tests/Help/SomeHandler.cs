using System;
using System.Collections;
using System.Reactive.Linq;
using MemBus.Subscribing;

namespace MemBus.Tests.Help
{
    public class SomeHandler : IAcceptDisposeToken, IWeirdHandler<MessageA>
    {
        public int MsgACalls;
        public int MsgBCalls;
        public int MsgCCalls;

        public readonly MessageC MsgC = new MessageC();

        private IDisposable _disposeToken;

        public void Handle(MessageA msg)
        {
            MsgACalls++;
        }

        IObservable<MessageA> IWeirdHandler<MessageA>.Producer()
        {
            return Observable.Return(new MessageA());
        }

        public MessageC Route(MessageB msg)
        {
            MsgBCalls++;
            return MsgC;
        }

        public void SomeOtherMethod(MessageC msg)
        {
            MsgCCalls++;
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

    public class HandlerWithRouteReturningPrimitive
    {
        public int MsgCall;

        public int Route(string msg)
        {
            MsgCall++;
            return MsgCall;
        }
    }

    public class HandlerReturningNull
    {
        public int MsgCall;

        public object Route(string msg)
        {
            MsgCall++;
            return null;
        }
    }

    public class EnumeratingHandler
    {
        public IEnumerable Route(MessageA msg)
        {
            yield return new MessageB();
            yield return new MessageC();
        }
    }
}