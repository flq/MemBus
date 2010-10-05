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
        IBus bus = BusSetup.StartWith<Conservative>().Construct();

        [Test]
        public void Apply_extension_method_where()
        {
            var msgCount = 0;
            
            var messages = 
                from msg in bus.Observe<MessageA>() 
                where msg.Name == "A" 
                select msg;

            using (messages.Subscribe(msg => msgCount++))
            {
                bus.Publish(new MessageA {Name = "A"});
                bus.Publish(new MessageA {Name = "B"});
                bus.Publish(new MessageA {Name = "A"});
                msgCount.ShouldBeEqualTo(2);
            }
        }

        [Test]
        public void Apply_extension_method_distinct()
        {
            var msgs = bus.Observe<MessageA>().DistinctUntilChanged(m => m.Name);
            var sb = new StringBuilder();

            using (msgs.Subscribe(msg => sb.Append(msg.Name)))
            {
                bus.Publish(new MessageA { Name = "A" });
                bus.Publish(new MessageA { Name = "A" });
                bus.Publish(new MessageA { Name = "B" });
                bus.Publish(new MessageA { Name = "B" });
                bus.Publish(new MessageA { Name = "A" });
                bus.Publish(new MessageA { Name = "A" });
                sb.ToString().ShouldBeEqualTo("ABA");
            }
        }

        [Test]
        public void Single_observable_supports_multiple_subscriptions()
        {
            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            var o = bus.Observe<MessageA>();
            var d1 = o.Subscribe(msg => sb1.Append("A"));
            var d2 = o.Subscribe(msg => sb2.Append("B"));
            bus.Publish(new MessageA());
            sb1.ToString().ShouldBeEqualTo("A");
            sb2.ToString().ShouldBeEqualTo("B");
            d1.Dispose();
            bus.Publish(new MessageA());
            sb1.ToString().ShouldBeEqualTo("A");
            sb2.ToString().ShouldBeEqualTo("BB");
            d2.Dispose();
            bus.Publish(new MessageA());
            sb1.ToString().ShouldBeEqualTo("A");
            sb2.ToString().ShouldBeEqualTo("BB");
        }

        
    }
}
