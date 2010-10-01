using System;
using Caliburn.Micro;
using MemBus;
using MemBus.Support;
using StructureMap;

namespace Membus.WpfTwitterClient.Frame.UI
{
    [Single]
    public class ScreenConstructor : Handles<RequestToActivateScreen>
    {
        private readonly IContainer container;
        private readonly IBus bus;

        public ScreenConstructor(IContainer container, IBus bus)
        {
            this.container = container;
            this.bus = bus;
        }

        protected override bool matches(RequestToActivateScreen message)
        {
            return !message.ScreenAvailable;
        }

        protected override void push(RequestToActivateScreen message)
        {
            var scr = container.GetInstance(message.TypeOfScreen) as Screen;
            if (scr == null)
                throw new InvalidCastException("The requested type {0} cannot be instantiated to be a Screen".Fmt(message.TypeOfScreen.Name));
            message.SetScreen(scr);
            bus.Publish(message);
        }
    }
}