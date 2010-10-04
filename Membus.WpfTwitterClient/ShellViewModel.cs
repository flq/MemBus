using System;
using System.Windows;
using System.Windows.Documents;
using Caliburn.Micro;
using Membus.WpfTwitterClient.Frame.UI;
using System.Linq;
using MessageStreams = System.Tuple<
  System.IObservable<
    Membus.WpfTwitterClient.Frame.UI.RequestToActivateScreen>, 
  System.IObservable<
    Membus.WpfTwitterClient.Frame.UI.ApplicationBusyMessage>>;

namespace Membus.WpfTwitterClient
{
    [Single]
    public class ShellViewModel : Conductor<Screen>
    {
        private readonly BusyAdorner busy;
        private readonly MessageStreams messageStreams;
        private readonly IDisposable screenStreamDispose;

        public ShellViewModel(BusyAdorner busy, MessageStreams messageStreams)
        {
            this.busy = busy;
            this.messageStreams = messageStreams;
            screenStreamDispose = messageStreams.Item1
                .Where(msg => msg.ScreenAvailable)
                .SubscribeOnDispatcher()
                .Subscribe(onNextScreenRequest);

            DisplayName = "MemBus OnTweet!";
        }

        public void AquaintWithAdornerLayer(FrameworkElement adornedElement)
        {
            var l = AdornerLayer.GetAdornerLayer(adornedElement);
            if (l == null)
                throw new ArgumentNullException("adornedElement", "No layer found to activate certain features.");
            l.Add(new BusyAdorner(adornedElement, messageStreams.Item2));

        }

        private void onNextScreenRequest(RequestToActivateScreen request)
        {
            DisplayName = request.Screen.DisplayName;
            ActivateItem(request.Screen);
        }

        protected override void OnDeactivate(bool close)
        {
            screenStreamDispose.Dispose();
            base.OnDeactivate(close);
        }
    }
}