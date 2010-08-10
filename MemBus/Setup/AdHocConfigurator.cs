using System;

namespace MemBus
{
    public class AdHocConfigurator<T> : ISetupConfigurator<T>
    {
        private readonly Action<T> action;

        public AdHocConfigurator(Action<T> action)
        {
            this.action = action;
        }

        public void Accept(T setup)
        {
            action(setup);
        }
    }
}