using System;
using MemBus.Configurators;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;
using MemBus.Tests.Help;
using Xunit;

namespace MemBus.Tests.Integration
{
    public class IntegrationScenarioInteractionWithSubscriber
    {
        private readonly IBus _bus;

        public IntegrationScenarioInteractionWithSubscriber()
        {
            _bus = BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
                .Apply<BlockSpecialsIfsuspended>()
                .Construct();
        }

        [Fact]
        public void Basic_functionality_works()
        {
            var sub = new TheSubscriber();
            _bus.Subscribe(sub);
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(1);
            sub.SpecialMsgCount.ShouldBeEqualTo(1);
        }

        [Fact]
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

        [Fact]
        public void The_special_shape_is_applied()
        {
            var sub = new TheSubscriber();
            _bus.Subscribe(sub);
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(1);
            sub.SpecialMsgCount.ShouldBeEqualTo(1);
            sub.Suspend();
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(2);
            sub.SpecialMsgCount.ShouldBeEqualTo(1);
            sub.Resume();
            _bus.Publish(new NormalMessage());
            _bus.Publish(new SpecialMessage());
            sub.NormalMsgCount.ShouldBeEqualTo(3);
            sub.SpecialMsgCount.ShouldBeEqualTo(2);
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