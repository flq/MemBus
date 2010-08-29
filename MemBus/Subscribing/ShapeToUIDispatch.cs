using System;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Use this shape to specify that the enclosed subscription works on the UI thread.
    /// </summary>
    public class ShapeToUiDispatch : ISubscriptionShaper
    {
        private TaskScheduler taskScheduler;

        public ShapeToUiDispatch(TaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
        }

        /// <summary>
        /// Use this constructor if the bus provides the necessary Task Scheduler for UI thread dispatching.
        /// </summary>
        public ShapeToUiDispatch()
        {
            
        }

        public IServices Services
        {
            set
            {
                taskScheduler = taskScheduler ?? value.Get<TaskScheduler>();
                if (taskScheduler == null)
                    throw new InvalidOperationException("No knowledge of a UI thread is available.");
            }
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new UiDispatchingSubscription(taskScheduler, subscription);
        }
    }
}