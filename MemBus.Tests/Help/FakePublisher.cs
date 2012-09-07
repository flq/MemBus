using MemBus;
using MemBus.Tests.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemBus.Tests.Help
{
    public class FakePublisher : IPublisher
    {
        public void Publish(object message)
        {
            Message = message;
        }

        public object Message { get; set; }

        public void VerifyMessageIsOfType<T>()
        {
            Message.ShouldNotBeNull();
            Message.ShouldBeOfType<T>();
        }
    }
}
