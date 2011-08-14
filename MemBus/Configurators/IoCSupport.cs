using MemBus.Setup;

namespace MemBus.Configurators
{
    /// <summary>
    /// Add ServiceLocator Support
    /// </summary>
    public class IoCSupport : ISetup<IConfigurableBus>
    {
        private readonly IocAdapter adapter;

        /// <summary>
        /// Add an IoCadapter that will be used to resolve subscriptions. subscriptions will be resolved based on the <see cref="IHandles{T}"/> interface
        /// </summary>
        public IoCSupport(IocAdapter adapter)
        {
            this.adapter = adapter;
        }

        void ISetup<IConfigurableBus>.Accept(IConfigurableBus setup)
        {
            setup.AddService(adapter);
            setup.AddResolver(new IoCBasedResolver(adapter));
        }
    }
}