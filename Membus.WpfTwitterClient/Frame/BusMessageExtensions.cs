using MemBus;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.Frame
{
    public static class BusMessageExtensions
    {
        public static void PublishNewActivity(this IBus bus, string message)
        {
            bus.Publish(new ApplicationActivityMessage(message));
        }

        public static void PublishActivityEnds(this IBus bus)
        {
            bus.Publish(new ApplicationActivityMessage());
        }
    }
}