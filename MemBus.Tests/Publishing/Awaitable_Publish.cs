using System.Threading.Tasks;
using MemBus.Configurators;
using Xunit;

namespace MemBus.Tests.Publishing
{
    public class Awaitable_Publish
    {
        [Fact]
        public async Task using_the_awaitable_publish()
        {
            var b = BusSetup.StartWith<Conservative>().Construct();
            var messageReceived = false;
            b.Subscribe((string h) => messageReceived = true);
            await b.PublishAsync("Hello");
            Assert.True(messageReceived);
        }
    }

}