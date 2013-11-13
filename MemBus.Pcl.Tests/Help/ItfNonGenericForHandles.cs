using System;

namespace MemBus.Tests.Help
{
    public interface ItfNonGenericForHandles
    {
        void Handle(MessageA msg);
    }

    class AHandlerThroughSimpleInterface : ItfNonGenericForHandles
    {
        public int MsgCount;

        public void Handle(MessageA msg)
        {
            MsgCount++;
        }
    }
}