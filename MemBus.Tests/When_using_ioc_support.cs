using System;
using System.Collections.Generic;
using System.Linq;
using MemBus.Configurators;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    internal class TestAdapter : IocAdapter
    {
        public IEnumerable<object> GetAllInstances(Type desiredType)
        {
            return Enumerable.Empty<object>();
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
            //_bus = BusSetup
            //    .StartWith<Conservative>()
            //    .Apply<IoCSupport>()
            //    .Construct();
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
            var x = Assert.Throws<ArgumentException>(() => BusSetup.StartWith<Conservative>().Apply<IoCSupport>(s => s.SetAdapter(_testAdapter).SetHandlerInterface(handlerType)).Construct());
        }
    }

    public interface TooManyGenericTypes<T, T1>
    {
        void Do(T msg);
    }

    public interface NoSuitableMethod<T>
    {
        void Do(T msg, string key);
    }

    public class NotOpenGeneric
    {
    }
}