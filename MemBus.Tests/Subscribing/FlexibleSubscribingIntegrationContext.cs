using System.Collections.Generic;
using System.Linq;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;

namespace MemBus.Tests.Subscribing
{
    public class MessageReceiver
    {
        public readonly List<object> Messages = new List<object>();

        public void Clear()
        {
            Messages.Clear();
        }

        public void Add(object msg)
        {
            Messages.Add(msg);
        }

        public void AssertContainsMessageOfType<T>(int count = 1)
        {
            Messages.OfType<T>().ShouldHaveCount(count);
        }
    }

    public class FlexibleSubscribingIntegrationContext
    {
        protected readonly IBus Bus;
        protected readonly List<object> Messages = new List<object>();

        protected FlexibleSubscribingIntegrationContext()
        {
            Bus = BusSetup
                .StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(ConfigureAdapter)
                .Construct();
            Bus.Subscribe<object>(MessageCapturing);
            // ReSharper disable VirtualMemberCallInConstructor
            foreach (var endpoints in GetEndpoints())
            {
                Bus.Subscribe(endpoints);
            }

            AdditionalSetup();
            // ReSharper restore VirtualMemberCallInConstructor
        }

        protected virtual void AdditionalSetup()
        {
        }

        protected virtual IEnumerable<object> GetEndpoints()
        {
            yield break;
        }

        private void MessageCapturing(object message)
        {
            Messages.Add(message);
        }

        protected virtual void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
        }

        protected void Clear()
        {
            Messages.Clear();
        }
    }
}