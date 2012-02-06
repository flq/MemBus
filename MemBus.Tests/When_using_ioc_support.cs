using System;
using System.Collections.Generic;
using MemBus.Configurators;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    internal class TestAdapter : IocAdapter
    {
        public Type TypePassedIn;

        public IocSubscribed TheSubscribed = new IocSubscribed();

        public IEnumerable<object> GetAllInstances(Type desiredType)
        {
            TypePassedIn = desiredType;
            if (TypePassedIn.Equals(typeof(GimmeMsg<string>)))
              yield return TheSubscribed;
        }
    }

    public interface TooManyGenericTypes<T, T1> { void Do(T msg); }
    public interface NoSuitableMethod<T> { void Do(T msg, string key); }
    public class NotOpenGeneric { }

    public interface GimmeMsg<in T> { void Gimme(T msg); }

    public class IocSubscribed : GimmeMsg<string>
    {
        public string CapturedMessage;

        public void Gimme(string msg)
        {
            CapturedMessage = msg;
        }
    }


    [TestFixture]
    public class When_using_ioc_support
    {
        private IBus _bus;
        private TestAdapter _testAdapter;

        [TestFixtureSetUp]
        public void Given()
        {
            _testAdapter = new TestAdapter();
            _bus = BusSetup
                .StartWith<Conservative>()
                .Apply<IoCSupport>(s => s.SetAdapter(_testAdapter).SetHandlerInterface(typeof(GimmeMsg<>)))
                .Construct();
        }

        [Test]
        public void fail_if_no_adapter_was_specified()
        {
            var x = Assert.Throws<ArgumentException>(()=>BusSetup.StartWith<Conservative>().Apply<IoCSupport>().Construct());
            x.Message.ShouldContain("IocAdapter");
        }

        [Test]
        public void fail_if_no_handler_type_has_been_specified()
        {
            var x = Assert.Throws<ArgumentException>(() => BusSetup.StartWith<Conservative>().Apply<IoCSupport>(s => s.SetAdapter(_testAdapter)).Construct());
            x.Message.ShouldContain("handler type");
        }

        [Test]
        [TestCase(typeof(NotOpenGeneric))]
        [TestCase(typeof(TooManyGenericTypes<,>))]
        [TestCase(typeof(NoSuitableMethod<>))]
        public void fail_with_bad_types(Type handlerType)
        {
            Assert.Throws<ArgumentException>(() => 
                BusSetup
                .StartWith<Conservative>()
                .Apply<IoCSupport>(s => s
                    .SetAdapter(_testAdapter)
                    .SetHandlerInterface(handlerType))
                .Construct());
        }

        [Test]
        public void ioc_asked_for_the_right_type()
        {
            _bus.Publish(1);
            _testAdapter.TypePassedIn.ShouldBeEqualTo(typeof(GimmeMsg<int>));
        }

        [Test]
        public void ioc_handler_is_being_used()
        {
            _bus.Publish("Foo");
            _testAdapter.TheSubscribed.CapturedMessage.ShouldBeEqualTo("Foo");
        }
    }
}