using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Subscribing
{
    [TestFixture]
    public class FSA_Consuming_Observables : FlexibleSubscribingIntegrationContext
    {
        private readonly Tester _tester = new Tester();

        private class Tester : MessageReceiver
        {
            public void EOne(IObservable<string> messages)
            {
                messages.Subscribe(Add);
            }
        }

        protected override IEnumerable<object> GetEndpoints()
        {
            yield return _tester;
        }

        protected override void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
            adp.RegisterMethods(mi => mi.Name.StartsWith("E"));
        }

        [Test]
        public void string_msg_received()
        {
            Bus.Publish("Hello");

            _tester.AssertContainsMessageOfType<string>();
        }
    }

    [TestFixture]
    public class FSA_Producing_Observables : FlexibleSubscribingIntegrationContext
    {
        private readonly Tester _tester = new Tester();

        private class Tester
        {
            public IObservable<string> EOne()
            {
                return Observable.Return("Hello from EOne");
            }
        }

        protected override IEnumerable<object> GetEndpoints()
        {
            yield return _tester;
        }

        protected override void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
            adp.RegisterMethods(mi => mi.Name.StartsWith("E"));
        }

        [Test]
        public void string_msg_received()
        {
            Messages.OfType<string>().ShouldContain(o => o == "Hello from EOne");
        }
    }

    [TestFixture]
    public class FSA_Mapping_Observables : FlexibleSubscribingIntegrationContext
    {
        private readonly Tester _tester = new Tester();

        private class Tester
        {
            public IObservable<MessageB> EOne(IObservable<MessageA> aMessages)
            {
                return aMessages.Select(a=> new MessageB {Id = a.Name});
            }
        }

        protected override IEnumerable<object> GetEndpoints()
        {
            yield return _tester;
        }

        protected override void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
            adp.RegisterMethods(mi => mi.Name.StartsWith("E"));
        }

        [Test]
        public void observable_mapping_working()
        {
            Bus.Publish(new MessageA {Name = "My sweet message"});
            var msgB = Messages.OfType<MessageB>().FirstOrDefault();
            msgB.ShouldNotBeNull();
            msgB.Id.ShouldBeEqualTo("My sweet message");
        }
    }
}