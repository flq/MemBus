namespace MemBus
{
    public interface IConfigurableBus
    {
        void InsertResolver(ISubscriptionResolver resolver);
        void InsertPublishPipeline(IPublishPipelineMember publishPipelineMember);
    }
}