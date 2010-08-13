using System.Text;
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

            var bus = BusSetup.StartWith<Conservative>(
              b => b.ConfigureSubscribing(
                s => s.MessageMatch(mi => mi.IsType<MessageB>(),
                                    sc => sc.ShapeOutwards(
                                       new TestShaper("B", () => sb.Append("B")),
                                       new TestShaper("A", () => sb.Append("A"))
                                    ))
              ))
              .Construct();
            bus.Subscribe<MessageB>(msg => { });
            bus.Publish(new MessageB());
            sb.ToString().ShouldBeEqualTo("AB"); 
        }
    }
}