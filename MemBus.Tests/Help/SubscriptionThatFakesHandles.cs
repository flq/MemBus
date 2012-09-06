using MemBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membus.Tests.Help
{
    public class SubscriptionThatFakesHandles<T> : ISubscription
    {
        public bool Handles(Type messageType)
        {
            return messageType == typeof(T);
        }

        void ISubscription.Push(object message)
        {
            throw new NotImplementedException();
        }
    }
}
