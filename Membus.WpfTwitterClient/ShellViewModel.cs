using System;
using Caliburn.Micro;
using MemBus.Support;
using Membus.WpfTwitterClient.Frame.UI;
using System.Linq;
using MessageStreams = System.Tuple<
  System.IObservable<
    Membus.WpfTwitterClient.Frame.UI.RequestToActivateScreen>, 
  System.IObservable<
    Membus.WpfTwitterClient.Frame.UI.ApplicationActivityMessage>>;

namespace Membus.WpfTwitterClient
{
    [Single]
    public class ShellViewModel : Conductor<Screen>
    {
        private readonly DisposeContainer disposeContainer = new DisposeContainer();

        public ShellViewModel(MessageStreams messageStreams)
        {
            var screenStreamDispose = messageStreams.Item1
                .Where(msg => msg.ScreenAvailable)
                .SubscribeOnDispatcher()
                .Subscribe(onNextScreenRequest);
            var busyStream1 = messageStreams.Item2
                .Where(msg=>msg.GettingBusy)
                .SubscribeOnDispatcher()
                .Subscribe(onGettingBusy);
            var busyStream2 = messageStreams.Item2
                .Where(msg => msg.GettingCalm)
                .SubscribeOnDispatcher()
                .Subscribe(onGettingCalm);

            disposeContainer.Add(screenStreamDispose, busyStream1, busyStream2);

            DisplayName = "MemBus OnTweet!";
        }

        private void onNextScreenRequest(RequestToActivateScreen request)
        {
            DisplayName = request.Screen.DisplayName;
            ActivateItem(request.Screen);
        }

        private void onGettingBusy(ApplicationActivityMessage msg)
        {
            BusyMessage = string.Empty;
            IsBusy = true;
            BusyMessage = msg.BusyText;
        }

        private void onGettingCalm(ApplicationActivityMessage msg)
        {
            IsBusy = false;
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (value.Equals(isBusy))
                    return;
                isBusy = value;
                NotifyOfPropertyChange(()=>IsBusy);
            }
        }

        private string busyMessage;
        public string BusyMessage
        {
            get { return busyMessage; }
            set
            {
                if (value == null || value.Equals(busyMessage))
                    return;
                busyMessage = value;
                NotifyOfPropertyChange(()=>BusyMessage);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            disposeContainer.Dispose();
            base.OnDeactivate(close);
        }
    }
}