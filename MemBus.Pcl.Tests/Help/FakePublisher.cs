namespace MemBus.Tests.Help
{
    public class FakePublisher : IPublisher
    {
        public void Publish(object message)
        {
            Message = message;
        }

        public object Message { get; set; }

        public void VerifyMessageIsOfType<T>()
        {
            Message.ShouldNotBeNull();
            Message.ShouldBeOfType<T>();
        }
    }
}
