using System;

namespace MemBus
{
    public class AdHocConfigurator : IBusSetupConfigurator
    {
        private readonly Action<IConfigurableBus> busAction;

        public AdHocConfigurator(Action<IConfigurableBus> busAction)
        {
            this.busAction = busAction;
        }

        public void Accept(IConfigurableBus setup)
        {
            busAction(setup);
        }
    }
}