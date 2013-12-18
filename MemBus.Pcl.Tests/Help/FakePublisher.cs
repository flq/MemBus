using System;

namespace MemBus.Tests.Help
{
    public class FakePublisher : IPublisher, IDisposable
    {
        public void Publish(object message)
        {
            Message = message;
        }

        public IDisposable Publish<M>(IObservable<M> observable)
        {
            return this;
        }

        public object Message { get; set; }

        public void VerifyMessageIsOfType<T>()
        {
            Message.ShouldNotBeNull();
            Message.ShouldBeOfType<T>();
        }

        void IDisposable.Dispose()
        {
            
        }
    }
}
