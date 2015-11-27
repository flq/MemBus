using System;

namespace MemBus.Setup
{
    internal class AdHocConfigurator<T> : ISetup<T>
    {
        private readonly Action<T> _action;

        public AdHocConfigurator(Action<T> action)
        {
            _action = action;
        }

        public void Accept(T setup)
        {
            _action(setup);
        }
    }
}