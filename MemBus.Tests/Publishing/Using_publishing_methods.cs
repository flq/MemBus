using System.Linq;
using MemBus.Subscribing;
using MemBus.Tests.Frame;
using MemBus.Tests.Help;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests.Publishing
{
    [TestFixture]
    internal class Using_publishing_methods
    {
        private SubscriptionBuilder _builder;
        private FakePublisher _publisher;

        [TestFixtureSetUp]
        public void Given()
        {
            _publisher = new FakePublisher();
            _builder = MethodScanner.ForNonVoidMethods("Route").MakeBuilder();
            _builder.SetPublisher(_publisher);
        }

        [Test]
        public void Subscriptions_for_publishing_method_based_work_correctly()
        {
            var handler = new SomeHandler();
            var subscription = _builder.BuildSubscriptions(handler).First();
            subscription.Push(new MessageB());

            subscription.Handles(typeof(MessageB)).ShouldBeTrue();
            handler.MsgBCalls.ShouldBeEqualTo(1);
            _publisher.VerifyMessageIsOfType<MessageC>();
        }

        [Test]
        public void primitive_type_is_used_as_return_value()
        {
            var handler = new HandlerWithRouteReturningPrimitive();
            var subscription = _builder.BuildSubscriptions(handler).First();

            subscription.Push("Hello");

            subscription.Handles(typeof(string)).ShouldBeTrue();
            handler.MsgCall.ShouldBeEqualTo(1);
            _publisher.VerifyMessageIsOfType<int>();
        }
    }
}
