using System;
using Caliburn.Micro;
using Membus.WpfTwitterClient.Frame.UI;
using System.Linq;

namespace Membus.WpfTwitterClient
{
    [Single]
    public class ShellViewModel : Conductor<Screen>
    {
        private readonly IDisposable screenStreamDispose;

        public ShellViewModel(IObservable<RequestToActivateScreen> screenStream)
        {
            screenStreamDispose = screenStream
                .SubscribeOnDispatcher()
                .Where(msg=>msg.ScreenAvailable)
                .Subscribe(onNextScreenRequest);
            DisplayName = "MemBus OnTweet!";
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