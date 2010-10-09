using System;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class ApplicationActivityMessage
    {
        public bool GettingBusy { get; private set; }
        public bool GettingCalm { get { return !GettingBusy; } }
        public string BusyText { get; private set; }

        /// <summary>
        /// Send a message that will state <see cref="GettingCalm"/>
        /// </summary>
        public ApplicationActivityMessage()
        {
            GettingBusy = false;
        }

        public ApplicationActivityMessage(string busyText)
        {
            GettingBusy = true;
            BusyText = busyText;
        }
    }
}