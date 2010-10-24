using MemBus.Publishing;
using MemBus.Setup;
using Membus.WpfTwitterClient.Startup;

namespace Membus.WpfTwitterClient.Frame.Config
{
    public class ClientPublishingConventions : ISetup<IConfigurableBus>
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishing(
                    p => p.MessageMatch(
                        m => m.IsType<RequestToStartup>(),
                        l => l.PublishPipeline(new SequentialPublisher())));
        }
    }
}