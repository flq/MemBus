using System;
using System.Linq;
using Caliburn.Micro;
using MemBus.Support;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.ShellOfApp
{
    [Single]
    public class ShellViewModel : Conductor<Screen>
    {
        public ActivityViewModel ActivityViewModel { get; private set; }
        private readonly DisposeContainer disposeContainer = new DisposeContainer();

        public ShellViewModel(IObservable<RequestToActivateMainScreen> activationStream, ActivityViewModel activityVm)
        {
            ActivityViewModel = activityVm;
            var screenStreamDispose = activationStream
                .Where(msg => msg.ScreenAvailable)
                .SubscribeOnDispatcher().Subscribe(onNextScreenRequest);

            disposeContainer.Add(screenStreamDispose, activityVm);

            DisplayName = "MemBus OnTweet!";
        }

        private void onNextScreenRequest(RequestToActivateMainScreen request)
        {
            DisplayName = request.Screen.DisplayName;
            ActivateItem(request.Screen);
        }

        protected override void OnDeactivate(bool close)
        {
            disposeContainer.Dispose();
            base.OnDeactivate(close);
        }
    }
}