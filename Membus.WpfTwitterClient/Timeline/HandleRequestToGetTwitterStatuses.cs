using MemBus;
using Membus.WpfTwitterClient.Frame.Twitter;

namespace Membus.WpfTwitterClient.Timeline
{
    public class HandleRequestToGetTwitterStatuses : Handles<RequestToGetTwitterStatuses>
    {
        private readonly ITwitterSession session;
        private readonly IBus bus;

        public HandleRequestToGetTwitterStatuses(ITwitterSession session, IBus bus)
        {
            this.session = session;
            this.bus = bus;
        }

        protected override void push(RequestToGetTwitterStatuses message)
        {
            session.LoadHomeTimeline(response => bus.Publish(new NewTwitterStatuses(response)));
        }
    }
}