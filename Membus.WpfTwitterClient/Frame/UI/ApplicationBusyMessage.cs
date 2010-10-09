using System;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class ApplicationBusyMessage
    {
        public bool GettingBusy { get; private set; }
        public bool GettingCalm { get { return !GettingBusy; } }
    }
}