using System.Threading.Tasks;

namespace MemBus.Subscribing
{
    public class ShapeToUiDispatch : ISubscriptionShaper
    {
        private readonly TaskScheduler taskScheduler;

        public ShapeToUiDispatch(TaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new UiDispatchingSubscription(taskScheduler, subscription);
        }
    }
}