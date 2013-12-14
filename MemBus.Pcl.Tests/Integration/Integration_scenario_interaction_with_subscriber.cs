using System;
using MemBus.Configurators;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Integration
{
    [TestFixture]
    public class Integration_Scenario_Interaction_With_Subscriber
    {
        private IBus _bus;

        [TestFixtureSetUp]
        public void Given()
        {
            _bus = BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(a => a.ByMethodName("Handle"))
                .Apply<BlockSpecialsIfsuspended>()
                .Construct();
        }

        [Test]
        public void Basic_functionality_works()
        {
            var sub = new TheSubscriber();
            _bus.Subscribe(sub);
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(1);
            sub.SpecialMsgCount.ShouldBeEqualTo(1);
        }

        [Test]
        public void Dispose_works()
        {
            var sub = new TheSubscriber();
            _bus.Subscribe(sub);
            sub.Dispose();
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(0);
            sub.SpecialMsgCount.ShouldBeEqualTo(0);
        }

        [Test]
        public void The_special_shape_is_applied()
        {
            var sub = new TheSubscriber();
            _bus.Subscribe(sub);
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(1, "Normal message sent");
            sub.SpecialMsgCount.ShouldBeEqualTo(1, "Special message sent");
            sub.Suspend();
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(2, "2nd normal message unharmed");
            sub.SpecialMsgCount.ShouldBeEqualTo(1, "Special blocked, must not be delivered");
            sub.Resume();
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(3, "3rd normal message");
            sub.SpecialMsgCount.ShouldBeEqualTo(2, "Special was resumed");
        }
    }

    public class BlockSpecialsIfsuspended : ISetup<IConfigurableBus>
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigureSubscribing(SubscribeConfig);
        }

        private static void SubscribeConfig(IConfigurableSubscribing cs)
        {
            cs.MessageMatch(mi => mi.Name.StartsWith("Special"), ConfigureSpecial);
        }

        private static void ConfigureSpecial(IConfigureSubscription cs)
        {
            cs.ShapeOutwards(new DenyIfSuspended(), new ShapeToDispose());
        }

        private class DenyIfSuspended : ISubscriptionShaper, ISubscription
        {
            private readonly ISubscription _inner;
            private readonly ICancelSpecials _cancelSpecials;

            public DenyIfSuspended() { }

            private DenyIfSuspended(ISubscription inner, ICancelSpecials cancelSpecials)
            {
                _inner = inner;
                _cancelSpecials = cancelSpecials;
            }

            ISubscription ISubscriptionShaper.EnhanceSubscription(ISubscription subscription)
            {
                var theCancelSpecials = (subscription as IKnowsSubscribedInstance).IfNotNull(ks => ks.Instance as ICancelSpecials);
                return theCancelSpecials != null ? new DenyIfSuspended(subscription, theCancelSpecials) : subscription;
            }

            void ISubscription.Push(object message)
            {
                if (!_cancelSpecials.IsSuspended)
                  _inner.Push(message);
            }

            bool ISubscription.Handles(Type messageType)
            {
                return _inner.Handles(messageType);
            }
        }
    }

    public class TheSubscriber : ICancelSpecials, IAcceptDisposeToken
    {
        public int NormalMsgCount;
        public int SpecialMsgCount;
        private IDisposable _disposeToken;
        private bool _isSuspended;

        public void Suspend()
        {
            _isSuspended = true;
        }

        public void Resume()
        {
            _isSuspended = false;
        }

        public void Handle(NormalMessage msg)
        {
            NormalMsgCount++;
        }

        public void Handle(SpecialMessage msg)
        {
            SpecialMsgCount++;
        }

        public void Dispose()
        {
            _disposeToken.Dispose();
        }

        bool ICancelSpecials.IsSuspended
        {
            get { return _isSuspended; }
        }

        void IAcceptDisposeToken.Accept(IDisposable disposeToken)
        {
            _disposeToken = disposeToken;
        }
    }

    public interface ICancelSpecials
    {
        bool IsSuspended { get; }
    }

    public class NormalMessage { }
    public class SpecialMessage { }
}