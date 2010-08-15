using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using MemBus;
using MemBus.Configurators;

namespace Membus.Tests.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IBus bus;

        private void getStarted(object sender, StartupEventArgs e)
        {

            bus = BusSetup.StartWith<RichClientFrontend>().Construct();
        }

        public static IBus Bus
        {
            get
            {
                return bus;
            }
        }
    }
}
