using System.Text;
using MemBus.Configurators;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests.Subscribing
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

    public class BusSetupWithTestShapers : ISetup<IConfigurableBus>
    {
        private readonly StringBuilder _sb;

        public BusSetupWithTestShapers(StringBuilder sb)
        {
            _sb = sb;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigureSubscribing(
                s => s.MessageMatch(mi => mi.IsType<MessageB>(),
                sc => sc.ShapeOutwards(
                                        new TestShaper("B", () => _sb.Append("B")),
                                        new TestShaper("A", () => _sb.Append("A"))
                                       )));
        }
    }

    public class BusSetupWithDefaultShape : ISetup<IConfigurableBus>
    {
        private readonly StringBuilder _sb;

        public BusSetupWithDefaultShape(StringBuilder sb)
        {
            _sb = sb;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigureSubscribing(
                s =>
                    {
                        s.DefaultShapeOutwards(new TestShaper("Bar", () => _sb.Append("Bar")));
                        s.MessageMatch(mi => mi.IsType<MessageB>(),
                                       sc => sc.ShapeOutwards(new TestShaper("Foo", () => _sb.Append("Foo"))));
                    });
        }
    }

    public class BusSetupPutShapeOnMsgA : ISetup<IConfigurableBus>
    {
        private readonly TestShaper _shaper;


        public BusSetupPutShapeOnMsgA(TestShaper shaper)
        {
            _shaper = shaper;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.AddService(new StringBuilder());
            setup.ConfigureSubscribing(
                s => s.MessageMatch(mi => mi.IsType<MessageA>(), c => c.ShapeOutwards(_shaper)));
        }
    }
}