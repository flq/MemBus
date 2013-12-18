using System.Threading.Tasks;
using MemBus.Configurators;
using NUnit.Framework;

namespace MemBus.Tests.Publishing
{
    [TestFixture]
    public class Awaitable_Publish
    {
        [Test]
        public async Task using_the_awaitable_publish()
        {
            var b = BusSetup.StartWith<Conservative>().Construct();
            var messageReceived = false;
            b.Subscribe((string h) => messageReceived = true);
            await b.PublishAsync("Hello");
            Assert.IsTrue(messageReceived);
        }




    }

}