using System;
using System.Linq;
using MemBus;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.ShellOfApp
{
    public class StreamOfScreensToActivate : RxEnabledObservable<RequestToActivateMainScreen>
    {
        public StreamOfScreensToActivate(IBus bus) : base(bus)
        {
        }

        protected override IObservable<RequestToActivateMainScreen> constructObservable(IObservable<RequestToActivateMainScreen> startingPoint)
        {
            return startingPoint
                .Where(msg => msg.ScreenAvailable)
                .ObserveOnDispatcher();
        }
    }
}