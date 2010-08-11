using System;
using System.Threading;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class UiDispatchingSubscription : ISubscription, IDenyShaper
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

        public bool Deny
        {
            get { return subscription.CheckDenyOrAllIsGood(); }
        }
    }
}