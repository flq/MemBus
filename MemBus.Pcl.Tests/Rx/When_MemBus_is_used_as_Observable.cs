using System;

using System.Text;
using MemBus.Configurators;
using MemBus.Tests.Help;
using System.Reactive.Linq;

using NUnit.Framework;

namespace MemBus.Tests.Rx
{
    [TestFixture]
    public class When_MemBus_Is_Used_As_Observable
    {
        readonly IBus _bus = BusSetup.StartWith<Conservative>().Construct();

        [Test]
        public void Apply_extension_method_where()
        {
            var msgCount = 0;
            
            var messages = 
                from msg in _bus.Observe<MessageA>() 
                where msg.Name == "A" 
                select msg;

            using (messages.Subscribe(msg => msgCount++))
            {
                _bus.Publish(new MessageA {Name = "A"});
                _bus.Publish(new MessageA {Name = "B"});
                _bus.Publish(new MessageA {Name = "A"});
                msgCount.ShouldBeEqualTo(2);
            }
        }

        [Test]
        public void Apply_extension_method_distinct()
        {
            var msgs = _bus.Observe<MessageA>().DistinctUntilChanged(m => m.Name);
            var sb = new StringBuilder();

            using (msgs.Subscribe(msg => sb.Append(msg.Name)))
            {
                _bus.Publish(new MessageA { Name = "A" });
                _bus.Publish(new MessageA { Name = "A" });
                _bus.Publish(new MessageA { Name = "B" });
                _bus.Publish(new MessageA { Name = "B" });
                _bus.Publish(new MessageA { Name = "A" });
                _bus.Publish(new MessageA { Name = "A" });
                sb.ToString().ShouldBeEqualTo("ABA");
            }
        }

        [Test]
        public void Class_can_work_as_specialized_observable()
        {
            var messages = new RxBasedFooObservable(_bus);
            int msgCount = 0;
            using (messages.Subscribe(msg => msgCount++))
            {
                _bus.Publish(new MessageA { Name = "Foo" });
                _bus.Publish(new MessageA { Name = "Bar" });
                _bus.Publish(new MessageA { Name = "Foo" });
            }
            msgCount.ShouldBeEqualTo(2);
        }

        [Test]
        public void Single_observable_supports_multiple_subscriptions()
        {
            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            var o = _bus.Observe<MessageA>();
            var d1 = o.Subscribe(msg => sb1.Append("A"));
            var d2 = o.Subscribe(msg => sb2.Append("B"));
            _bus.Publish(new MessageA());
            sb1.ToString().ShouldBeEqualTo("A");
            sb2.ToString().ShouldBeEqualTo("B");
            d1.Dispose();
            _bus.Publish(new MessageA());
            sb1.ToString().ShouldBeEqualTo("A");
            sb2.ToString().ShouldBeEqualTo("BB");
            d2.Dispose();
            _bus.Publish(new MessageA());
            sb1.ToString().ShouldBeEqualTo("A");
            sb2.ToString().ShouldBeEqualTo("BB");
        }

        
    }
}
