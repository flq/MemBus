using System;
using MemBus.Subscribing;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_disposable_method_subscription
    {
        [Test]
        public void handle_type_derived_from_Action()
        {
            int callCount = 0;
            Action<MessageA> a = msg => callCount++;
            var sub = new DisposableMethodSubscription<MessageA>(a);
            sub.Handles.ShouldBeEqualTo(typeof(MessageA));
        }

        [Test]
        public void passed_in_action_is_called()
        {
            int callCount = 0;
            Action<MessageA> a = msg => callCount++;
            var sub = new DisposableMethodSubscription<MessageA>(a);
            sub.Push(new MessageA());
            callCount.ShouldBeEqualTo(1);
        }

        [Test]
        public void calling_disposer_raises_disposed_evt()
        {
            var disposeCalled = false;
            ISubscription sub = new DisposableMethodSubscription<MessageA>(null);
            sub.Disposed += (s, e) => disposeCalled = true;
            sub.GetDisposer().Dispose();
            disposeCalled.ShouldBeTrue();
            
        }
    }
}