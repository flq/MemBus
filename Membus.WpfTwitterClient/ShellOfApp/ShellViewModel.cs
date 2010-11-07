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
        public AttentionViewModel AttentionViewModel { get; private set; }

        private readonly DisposeContainer disposeContainer = new DisposeContainer();

        public ShellViewModel(StreamOfScreensToActivate activationStream, ActivityViewModel activityVm, AttentionViewModel attentionVm)
        {
            ActivityViewModel = activityVm;
            AttentionViewModel = attentionVm;
            var screenStreamDispose = activationStream.Subscribe(onNextScreenRequest);
            disposeContainer.Add(screenStreamDispose, activityVm, attentionVm);

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