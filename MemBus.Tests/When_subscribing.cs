using System.Text;
using MemBus.Configurators;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class sWhen_subscribing
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
    }
}