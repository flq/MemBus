using System;
using System.Threading;
using System.Threading.Tasks;

namespace MemBus.Subscribing
{
    public class UiDispatchingSubscription : ISubscription
    {
        private readonly TaskScheduler taskScheduler;
        private readonly ISubscription subscription;

        public UiDispatchingSubscription(TaskScheduler taskScheduler, ISubscription subscription)
        {
            this.taskScheduler = taskScheduler;
            this.subscription = subscription;
        }

        public void Push(object message)
        {
            Task.Factory.StartNew(() => 
                                  subscription.Push(message), CancellationToken.None, TaskCreationOptions.None,taskScheduler)
                .Wait();
        }

        public Type Handles
        {
            get { return subscription.Handles; }
        }
    }
}