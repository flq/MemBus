using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_Configurations
    {
        [Test]
        public void calibration_of_default_configuration()
        {
            var m = new MockConfigurableBus(BusSetup.Start().Apply<DefaultSetup>());
            m.Resolvers.ShouldHaveCount(1);
            m.PublishPipeline.ShouldHaveCount(1);

        }
    }
}