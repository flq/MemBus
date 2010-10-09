using System;
using System.Windows;
using System.Windows.Documents;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class BusyAdorner : Adorner
    {
        public BusyAdorner(UIElement adornedElement, IObservable<ApplicationActivityMessage> appBusyMessageStream) : base(adornedElement)
        {
            //app
        }
    }
}