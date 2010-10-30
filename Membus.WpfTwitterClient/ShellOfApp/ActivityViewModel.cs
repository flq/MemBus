using System;
using System.Linq;
using Caliburn.Micro;
using MemBus.Support;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.ShellOfApp
{
    public class ActivityViewModel : PropertyChangedBase, IDisposable
    {
        private readonly DisposeContainer disposer = new DisposeContainer();

        public ActivityViewModel(IObservable<ApplicationActivityMessage> activityMessages)
        {
            var busyStream1 = activityMessages
                .Where(msg => msg.GettingBusy)
                .ObserveOnDispatcher().Subscribe(onGettingBusy);
            var busyStream2 = activityMessages
                .Where(msg => msg.GettingCalm)
                .ObserveOnDispatcher().Subscribe(onGettingCalm);
            disposer.Add(busyStream1, busyStream2);
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
                NotifyOfPropertyChange(() => IsBusy);
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
                NotifyOfPropertyChange(() => BusyMessage);
            }
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


        void IDisposable.Dispose()
        {
            disposer.Dispose();
        }
    }
}