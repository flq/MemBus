using System;
using MemBus.Configurators;
using MemBus.Tests.Help;
using Xunit;

namespace MemBus.Tests.Integration
{
    
    public class When_Using_Disposable_Buses
    {
        [Fact]
        public void A_disposed_bus_throws()
        {
            var bus = BusSetup.StartWith<Conservative>().Construct();
            bus.Dispose();
            (new Action(() => bus.Publish(new MessageA()))).Throws<ObjectDisposedException>();
            (new Action(() => bus.Subscribe<MessageA>(msg=>{}))).Throws<ObjectDisposedException>();
            (new Action(() => bus.Observe<MessageA>())).Throws<ObjectDisposedException>();
        }
       
    }
}