using System;
using System.Text;
using MemBus.Configurators;
using MemBus.Setup;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_subscribing
    {
        private static IBus giveBus(Action<IConfigurableSubscribing> configure)
        {
            return BusSetup.StartWith<Conservative>(b => b.ConfigureSubscribing(configure))
              .Construct();
        }

        [Test]
        public void Conventions_allow_changing_the_shape()
        {
            var sb = new StringBuilder();

            var bus = giveBus(
                s => s.MessageMatch(mi => mi.IsType<MessageB>(),
                                    sc => sc.ShapeOutwards(
                                       new TestShaper("B", () => sb.Append("B")),
                                       new TestShaper("A", () => sb.Append("A"))
                                    )));

            using (bus.Subscribe<MessageB>(msg => { }))
            {
                bus.Publish(new MessageB());
            }
            sb.ToString().ShouldBeEqualTo("AB"); 
        }

        [Test]
        public void The_default_is_applied_when_no_specials_apply()
        {
            var sb = new StringBuilder();

            var bus = giveBus(
                s =>
                    {
                        s.DefaultShapeOutwards(new TestShaper("Bar", ()=>sb.Append("Bar")));
                        s.MessageMatch(mi => mi.IsType<MessageB>(),
                                       sc => sc.ShapeOutwards(new TestShaper("Foo", () => sb.Append("Foo"))));
                    });

            using (bus.Subscribe<MessageA>(msg => { }))
            {
                bus.Publish(new MessageA());
            }
            sb.ToString().ShouldBeEqualTo("Bar"); 
        }

        [Test]
        public void A_shape_gets_access_to_services()
        {
            var testShaper = new TestShaper("Test");
            var b = BusSetup.StartWith<Conservative>(
                cb =>
                    {
                        cb.AddService(new StringBuilder());
                        cb.ConfigureSubscribing(
                            s => s.MessageMatch(mi => mi.IsType<MessageA>(), c => c.ShapeOutwards(testShaper)));
                    }).Construct();
            testShaper.Services.ShouldNotBeNull();
        }
    }
}