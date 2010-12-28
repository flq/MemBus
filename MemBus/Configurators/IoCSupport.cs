using MemBus.Setup;

namespace MemBus.Configurators
{
    /// <summary>
    /// Add ServiceLocator Support
    /// </summary>
    public class IoCSupport : ISetup<IConfigurableBus>
    {
        private readonly IocAdapter adapter;

        public IoCSupport(IocAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void Accept(IConfigurableBus setup)
        {
            setup.AddService(adapter);
            setup.AddResolver(new IoCBasedResolver(adapter));
        }
    }
}