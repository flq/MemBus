using MemBus.Subscribing;
using MemBus.Tests.Help;
using System.Linq;

using Xunit;



namespace MemBus.Tests.Subscribing
{
    
    public class Using_Disposable_Method_Subscription
    {
        [Fact]
        public void the_basic_method_subscription_is_contravariant()
        {
            var sub = new MethodInvocation<MessageA>(msg => { });
            sub.Handles(typeof(MessageASpecialization)).ShouldBeTrue();
        }

        
        [Fact]
        public void Handle_type_derived_from_action()
        {
            int callCount = 0;
            var sub = new DisposableSubscription(new MethodInvocation<MessageA>(msg => callCount++));
            sub.Handles(typeof(MessageA)).ShouldBeTrue();
        }

        [Fact]
        public void passed_in_action_is_called()
        {
            int callCount = 0;
            var sub = new DisposableSubscription(new MethodInvocation<MessageA>(msg => callCount++));
            sub.Push(new MessageA());
            callCount.ShouldBeEqualTo(1);
        }

        [Fact]
        public void calling_disposer_raises_disposed_evt()
        {
            var disposeCalled = false;
            IDisposableSubscription sub = new DisposableSubscription(null);
            sub.Disposed += (s, e) => disposeCalled = true;
            sub.GetDisposer().Dispose();
            disposeCalled.ShouldBeTrue();
        }

        
        [Fact]
        public void Adapter_subscriptions_can_also_be_disposed()
        {
            var b = new MethodScanner("Handle").MakeBuilder();
            var disposableSub = new DisposableSubscription(b.BuildSubscriptions(new SomeHandler()).First());
            ISubscriptionResolver resolver = new CompositeSubscription();
            resolver.Add(disposableSub);

            var subs = resolver.GetSubscriptionsFor(new MessageA());
            subs.ShouldHaveCount(1);

            disposableSub.GetDisposer().Dispose();

            subs = resolver.GetSubscriptionsFor(new MessageA());
            subs.ShouldHaveCount(0);
        }
    }
}