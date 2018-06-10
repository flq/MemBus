using System;
using System.Threading;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class UiDispatchingSubscription : ISubscription, IDenyShaper
    {
        private readonly TaskScheduler _taskScheduler;
        private readonly ISubscription _subscription;

        public UiDispatchingSubscription(TaskScheduler taskScheduler, ISubscription subscription)
        {
            _taskScheduler = taskScheduler;
            _subscription = subscription;
        }

        public void Push(object message)
        {
            Task.Factory.StartNew(() => 
                                  _subscription.Push(message), CancellationToken.None, TaskCreationOptions.None,_taskScheduler)
                .Wait();
        }

        public bool Handles(Type messageType)
        {
            return _subscription.Handles(messageType);
        }


        public bool Deny
        {
            get { return _subscription.CheckDenyOrAllIsGood(); }
        }
    }
}