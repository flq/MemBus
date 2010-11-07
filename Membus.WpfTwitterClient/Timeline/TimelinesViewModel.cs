using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Membus.WpfTwitterClient.Frame;
using MemBus;
using MemBus.Support;
using Twitterizer;

namespace Membus.WpfTwitterClient.Timeline
{
    public class TimelinesViewModel : Screen
    {
        private readonly IBus bus;
        private readonly IObservable<NewTwitterStatuses> streamOfTwitterStatuses;
        private IDisposable streamDispose;

        public TimelinesViewModel(IBus bus, IObservable<NewTwitterStatuses> streamOfTwitterStatuses)
        {
            this.bus = bus;
            this.streamOfTwitterStatuses = streamOfTwitterStatuses;
            DisplayName = "Hello from timeline";
        }

        public ObservableCollection<TwitterStatus> Statuses
        {
            get; private set;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Statuses = new ObservableCollection<TwitterStatus>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            streamDispose = streamOfTwitterStatuses.ObserveOnDispatcher().Subscribe(onNewTwitterStatuses);
            askForNewStatuses();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (streamDispose != null)
              streamDispose.Dispose();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        private void askForNewStatuses()
        {
            bus.PublishNewActivity("Retrieving statuses");
            bus.Publish(new RequestToGetTwitterStatuses());
        }

        private void onNewTwitterStatuses(NewTwitterStatuses statuses)
        {
            bus.PublishActivityEnds();
            statuses.Statuses.Each(Statuses.Add);
        }
    }
}