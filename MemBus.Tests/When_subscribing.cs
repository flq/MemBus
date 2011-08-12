using System.Text;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_subscribing
    {
        [Test]
        public void Conventions_allow_changing_the_shape()
        {
            var sb = new StringBuilder();
            var bus = BusSetup.StartWith<Conservative>(new BusSetupWithTestShapers(sb)).Construct();

            using (bus.Subscribe<MessageB>(msg => { }))
                bus.Publish(new MessageB());
            
            sb.ToString().ShouldBeEqualTo("AB"); 
        }

        [Test]
        public void The_default_is_applied_when_no_specials_apply()
        {
            var sb = new StringBuilder();
            var bus = BusSetup.StartWith<Conservative>(new BusSetupWithDefaultShape(sb)).Construct();

            using (bus.Subscribe<MessageA>(msg => { }))
                bus.Publish(new MessageA());

            sb.ToString().ShouldBeEqualTo("Bar"); 
        }

        [Test]
        public void A_shape_gets_access_to_services()
        {
            var testShaper = new TestShaper("Test");
            BusSetup.StartWith<Conservative>(new BusSetupPutShapeOnMsgA(testShaper)).Construct();
            testShaper.Services.ShouldNotBeNull();
        }

        [Test]
        public void The_instance_of_a_static_method_is_null()
        {
            var sub = new MethodInvocation<object>(Substatic);
            ((IKnowsSubscribedInstance)sub).Instance.ShouldBeNull();
            
        }

        [Test]
        public void Meth_invocation_implements_knows_instance()
        {
            var sub = new MethodInvocation<object>(Sub);
            ((IKnowsSubscribedInstance)sub).Instance.ShouldBeOfType<When_subscribing>();
        }

        public void Sub(object msg) { }
        public static void Substatic(object msg) {}
    }
}