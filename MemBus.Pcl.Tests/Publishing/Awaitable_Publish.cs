using System.Threading.Tasks;
using MemBus.Configurators;
using NUnit.Framework;
using Nito.AsyncEx;

namespace MemBus.Tests.Publishing
{
    [TestFixture]
    public class Awaitable_Publish
    {
        [Test]
		public void using_the_awaitable_publish()
        {
			AsyncContext.Run (async () => {
				var b = BusSetup.StartWith<Conservative>().Construct();
				var messageReceived = false;
				b.Subscribe((string h) => messageReceived = true);
				await b.PublishAsync("Hello");
				Assert.IsTrue(messageReceived);
			});
        }




    }

}