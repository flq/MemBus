using System;
using System.Reactive;
using System.Threading.Tasks;
using MemBus.Subscribing;

namespace MemBus.Tests.Help
{
    public class FakeBus : IBus, IDisposable
    {
        public void Publish(object message)
        {
            Message = message;
        }

        public IDisposable Publish<M>(IObservable<M> observable)
        {
            return this;
        }

        public Task PublishAsync(object message)
        {
            return new Task(() => Message = message);
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

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return this;
        }

        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper shaper)
        {
            return this;
        }

        public IDisposable Subscribe(object subscriber)
        {
            return this;
        }

        public IObservable<M> Observe<M>()
        {
            return new AnonymousObservable<M>(observer => this);
        }
    }
}
