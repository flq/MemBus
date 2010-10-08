using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
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
        private readonly MessageStreams messageStreams;
        private readonly IDisposable screenStreamDispose;

        public ShellViewModel(MessageStreams messageStreams)
        {
            this.messageStreams = messageStreams;
            screenStreamDispose = messageStreams.Item1
                .Where(msg => msg.ScreenAvailable)
                .SubscribeOnDispatcher()
                .Subscribe(onNextScreenRequest);


            DisplayName = "MemBus OnTweet!";
        }

        private void onNextScreenRequest(RequestToActivateScreen request)
        {
            return;
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