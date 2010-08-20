using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemBus;
using MemBus.Configurators;
using Membus.Reactive.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace Membus.Reactive.Tests
{
    [TestFixture]
    public class When_MemBus_is_used_as_Observable
    {
        [Test]
        public void Apply_extension_method_where()
        {
            int msgCount = 0;
            var mb = BusSetup.StartWith<Conservative>().Construct();
            var msgs = 
                from msg in mb.Observe<MessageA>() 
                where msg.Name == "A" 
                select msg;

            using (msgs.Subscribe(msg => msgCount++))
            {
                mb.Publish(new MessageA {Name = "A"});
                mb.Publish(new MessageA {Name = "B"});
                mb.Publish(new MessageA {Name = "A"});
                msgCount.ShouldBeEqualTo(2);
            }
        }
    }
}
