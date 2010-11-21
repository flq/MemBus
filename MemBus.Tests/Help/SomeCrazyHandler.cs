using System;

namespace MemBus.Tests.Help
{
    public class SomeCrazyHandler : IClassicIHandleStuffI<MessageC>, IClassicIHandleStuffI<MessageB>
    {
        public int MessageCCount;
        public int MessageACount;
        public int MessageBCount;
        public int MsgSpecialACount;

        public void Gimme(MessageC theThing)
        {
            MessageCCount++;
        }

        public void Gimme(MessageB theThing)
        {
            MessageBCount++;
        }

        public void Handle(MessageA msg)
        {
            MessageACount++;
        }

        public void Handle(MessageB msg)
        {
            MessageBCount++;
        }

        public void Schmandle(MessageASpecialization msg)
        {
            MsgSpecialACount++;
        }
    }
}