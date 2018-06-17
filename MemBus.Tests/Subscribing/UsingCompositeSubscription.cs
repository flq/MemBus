using MemBus.Tests.Help;
using System.Linq;
using Xunit;

namespace MemBus.Tests.Subscribing
{
    
    public class UsingCompositeSubscription
    {
        [Fact]
        public void empty_composite_would_handle_any_message()
        {
            var c = new CompositeSubscription();
            (c as ISubscription).Handles(typeof(MessageA)).ShouldBeTrue();
        }

        [Fact]
        public void push_message_on_empty_composite_is_legal()
        {
            var c = new CompositeSubscription();
            (c as ISubscription).Push(new MessageA());
        }

        [Fact]
        public void adding_disparate_handlers_is_not_validated_by_composite()
        {
            var c = new CompositeSubscription {new MockSubscription<MessageA>(), new MockSubscription<MessageB>()};
            c.ShouldHaveCount(2);
        }

        [Fact]
        public void composite_enumerator_works()
        {
            var c = new CompositeSubscription { new MockSubscription<MessageA>(), new MockSubscription<MessageA>() };
            c.ShouldHaveCount(2);
        }

        [Fact]
        public void push_delegates_to_all_childs()
        {
            var c = new CompositeSubscription {new MockSubscription<MessageA>(), new MockSubscription<MessageA>()};
            c.Push(new MessageA());
            c.OfType<MockSubscription<MessageA>>().All(m=>m.Received == 1).ShouldBeTrue();
        }

        [Fact]
        public void dispose_of_subscription_removes_it_from_composite()
        {
            var subscription = new MockSubscription<MessageA>();
            var c = new CompositeSubscription { subscription };
            c.ShouldHaveCount(1);
            subscription.Dispose();
            c.ShouldHaveCount(0);
        }
    }
}