using System;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Integration
{
    internal class Subscriber : IAcceptDisposeToken, IDisposable
    {
        private IDisposable _disposeToken;
        private IDisposable _other;
        private bool _disposing;

        public bool GotMessage { get; private set; }

        public void Handle(string msg)
        {
            // A blunt way to remove a subscription during message handling
            DisposeOther();
            GotMessage = true;
        }

        public void Aquaint(IDisposable d){ _other = d; }

        public void Dispose()
        {
            if (_disposing) return;
            _disposing = true;
            _disposeToken.Dispose();
            DisposeOther();
            _disposing = false;
        }

        private void DisposeOther() { if (_other != null) _other.Dispose(); }

        void IAcceptDisposeToken.Accept(IDisposable disposeToken) { _disposeToken = disposeToken; }
    }

    [TestFixture]
    public class Unexpected_Disposal_Of_Subscriptions
    {
        private IBus _bus;
        private Subscriber _partnerInCrime1;
        private Subscriber _partnerInCrime2;

        [SetUp]
        public void Given()
        {
            _bus = BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(adp => adp.ByMethodName("Handle"))
                .Construct();
            _bus.Subscribe(_partnerInCrime1 = new Subscriber());
            _bus.Subscribe(_partnerInCrime2 = new Subscriber());

            // One disposes the other. That way we keep independent of call sequence
            _partnerInCrime1.Aquaint(_partnerInCrime2);
            _partnerInCrime2.Aquaint(_partnerInCrime1);
        }

        [Test]
        public void disposal_of_subscription_does_not_cause_exception_in_message_handling()
        {
            try
            {
                _bus.Publish("Boo!");
            }
            catch (Exception x)
            {
                Assert.Fail("Operation threw an exception of type {0}: {1}\n{2}", x.GetType().Name, x.Message, x.StackTrace);
            }
        }

        [Test]
        public void one_subscription_got_the_message()
        {
            _bus.Publish("Boo!");
            (_partnerInCrime1.GotMessage || _partnerInCrime2.GotMessage).ShouldBeTrue();
        }
    }
}