using System;

namespace MemBus.Setup
{
    public class AdHocConfigurator<T> : ISetup<T>
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