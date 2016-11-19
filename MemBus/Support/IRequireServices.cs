namespace MemBus.Support
{
    /// <summary>
    /// Specify that you want access to the <see cref="IServices"/> instance of MemBus.
    /// You can use this on <see cref="ISubscriptionShaper"/> and <see cref="ISubscriptionResolver"/> instances.
    /// </summary>
    public interface IRequireServices
    {
        void AddServices(IServices svc);
    }

    /// <summary>
    /// Specify that you want access to the <see cref="IBus"/> instance of MemBus.
    /// You can use this on <see cref="IPublishPipelineMember"/> instances.
    /// </summary>
    public interface IRequireBus
    {
        void AddBus(IBus bus);
    }
}
