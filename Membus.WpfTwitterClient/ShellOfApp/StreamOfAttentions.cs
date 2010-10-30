using System.Linq;
using MemBus;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.ShellOfApp
{
    public class StreamOfAttentions : RxEnabledObservable<RequestForAttention>
    {
        public StreamOfAttentions(IBus bus) : base(bus)
        {
        }

        protected override System.IObservable<RequestForAttention> constructObservable(System.IObservable<RequestForAttention> startingPoint)
        {
            return startingPoint.SubscribeOnDispatcher();
        }
    }
}