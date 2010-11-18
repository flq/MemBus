namespace MemBus.Tests.Help
{
    public class MessageA
    {
    }

    public class MessageB
    {
        public string Id { get; set; }
    }

    public class MessageC
    {
    }

    public class MessageASpecialization : MessageA
    {
    }

    public class Transport
    {
        public bool On;
    }
}