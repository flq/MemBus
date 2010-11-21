namespace MemBus.Tests.Help
{
    public class SomeHandler
    {
        public int MsgACalls;

        public void Handle(MessageA msg)
        {
            MsgACalls++;
        }
    }
}