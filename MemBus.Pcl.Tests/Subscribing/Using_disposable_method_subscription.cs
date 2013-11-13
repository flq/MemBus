using MemBus.Subscribing;
using MemBus.Tests.Help;
using MemBus.Tests.Frame;
using System.Linq;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#else
using NUnit.Framework;
#endif


namespace MemBus.Tests.Subscribing
{
    [TestFixture]
    public class Using_disposable_method_subscription
    {
        [Test]
        public void the_basic_method_subscription_is_contravariant()
        {
            var sub = new MethodInvocation<MessageA>(msg => { });
            sub.Handles(typeof(MessageASpecialization)).ShouldBeTrue();
        }

        
        [Test]
        public void Handle_type_derived_from_action()
        {
            int callCount = 0;
            var sub = new DisposableSubscription(new MethodInvocation<MessageA>(msg => callCount++));
            sub.Handles(typeof(MessageA)).ShouldBeTrue();
        }

        [Test]
        public void passed_in_action_is_called()
        {
            int callCount = 0;
            var sub = new DisposableSubscription(new MethodInvocation<MessageA>(msg => callCount++));
            sub.Push(new MessageA());
            callCount.ShouldBeEqualTo(1);
        }

        [Test]
        public void calling_disposer_raises_disposed_evt()
        {
            var disposeCalled = false;
            IDisposableSubscription sub = new DisposableSubscription(null);
            sub.Disposed += (s, e) => disposeCalled = true;
            sub.GetDisposer().Dispose();
            disposeCalled.ShouldBeTrue();
        }

        
        [Test]
        public void Adapter_subscriptions_can_also_be_disposed()
        {
            var b = MethodScanner.ForVoidMethods("Handle").MakeBuilder();
            var disposableSub = new DisposableSubscription(b.BuildSubscriptions(new SomeHandler()).First());
            var resolver = new StandardResolver();
            resolver.Add(disposableSub);

            var subs = resolver.GetSubscriptionsFor(new MessageA());
            subs.ShouldHaveCount(1);

            disposableSub.GetDisposer().Dispose();

            subs = resolver.GetSubscriptionsFor(new MessageA());
            subs.ShouldHaveCount(0);
        }
    }
}