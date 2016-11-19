using System;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Use this shape to specify that the enclosed subscription works on the UI thread.
    /// Please note that you will have to provide a TaskScheduler that will dispatch onto a given UI thread.
    /// </summary>
    public class ShapeToUiDispatch : ISubscriptionShaper, IRequireServices
    {
        private TaskScheduler _taskScheduler;

        public ShapeToUiDispatch(TaskScheduler taskScheduler)
        {
            this._taskScheduler = taskScheduler;
        }

        /// <summary>
        /// Use this constructor if the bus provides the necessary Task Scheduler for UI thread dispatching.
        /// </summary>
        public ShapeToUiDispatch()
        {
            
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new UiDispatchingSubscription(_taskScheduler, subscription);
        }

        void IRequireServices.AddServices(IServices svc)
        {
            _taskScheduler = _taskScheduler ?? svc.Get<TaskScheduler>();
            if (_taskScheduler == null)
                throw new InvalidOperationException("No knowledge of a UI thread is available.");
        }
    }
}