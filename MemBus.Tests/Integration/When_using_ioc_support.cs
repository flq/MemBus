using System;
using System.Collections.Generic;
using MemBus.Configurators;
using MemBus.Tests.Help;
using Xunit;

namespace MemBus.Tests.Integration
{
    internal class TestAdapter : IocAdapter
    {
        public Type TypePassedIn;

        public IocSubscribed TheSubscribed = new IocSubscribed();
        public IocSubscribedTypeResolver TheSubscribedTypeResolver = new IocSubscribedTypeResolver();

        public IEnumerable<object> GetAllInstances(Type desiredType)
        {
            TypePassedIn = desiredType;
            if (TypePassedIn == typeof(GimmeMsg<string>))
                yield return TheSubscribed;
            if (TypePassedIn == typeof(GimmeMsgTypeResolver<string>))
                yield return TheSubscribedTypeResolver;
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

    public interface IEnvelope<out T>
    {
        T Body { get; }
    }

    public class Envelope<T> : IEnvelope<T>
    {
        public Envelope(T body)
        {
            this.Body = body;
        }

        public T Body { get; private set; }

        public override string ToString()
        {
            return Convert.ToString(this.Body) + " in envelope";
        }
    }

    public interface GimmeMsgTypeResolver<T> { void Gimme(IEnvelope<T> msg); }

    public class IocSubscribedTypeResolver : GimmeMsgTypeResolver<string>
    {
        public string CapturedMessage;

        public void Gimme(IEnvelope<string> msg)
        {
            CapturedMessage = msg.ToString();
        }
    }

    public class When_Using_Ioc_Support
    {
        private IBus _bus;
        private TestAdapter _testAdapter;

        public When_Using_Ioc_Support()
        {
            _testAdapter = new TestAdapter();
            _bus = BusSetup
                .StartWith<Conservative>()
                .Apply<IoCSupport>(s => s.SetAdapter(_testAdapter).SetHandlerInterface(typeof(GimmeMsg<>)))
                .Construct();
        }

        [Fact]
        public void fail_if_no_adapter_was_specified()
        {
            (new Action(() => BusSetup.StartWith<Conservative>().Apply<IoCSupport>().Construct()))
                .Throws<ArgumentException>()
                .Message.ShouldContain("IocAdapter");
        }

        [Fact]
        public void fail_if_no_handler_type_has_been_specified()
        {
            (new Action(() => BusSetup.StartWith<Conservative>().Apply<IoCSupport>(s => s.SetAdapter(_testAdapter)).Construct()))
                .Throws<ArgumentException>()
                .Message.ShouldContain("handler type");
        }

        
        [Theory]
        [InlineData(typeof(NotOpenGeneric))]
        [InlineData(typeof(NoSuitableMethod<>))]
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

        [Fact]
        public void fail_with_bad_types_too_many_generics()
        {
            // For some reason that test case was getting consistently ignored by r# test runner
            fail_with_bad_types(typeof(TooManyGenericTypes<,>));
        }
        

        [Fact]
        public void ioc_asked_for_the_right_type()
        {
            _bus.Publish(1);
            _testAdapter.TypePassedIn.ShouldBeEqualTo(typeof(GimmeMsg<int>));
        }

        [Fact]
        public void ioc_handler_is_being_used()
        {
            _bus.Publish("Foo");
            _testAdapter.TheSubscribed.CapturedMessage.ShouldBeEqualTo("Foo");
        }
    }

    public class When_Using_Ioc_Support_With_Type_Resolver
    {
        private IBus _bus;
        private TestAdapter _testAdapter;

        public When_Using_Ioc_Support_With_Type_Resolver()
        {
            _testAdapter = new TestAdapter();
            _bus = BusSetup
                .StartWith<Conservative>()
                .Apply<IoCSupport>(s => s.SetAdapter(_testAdapter).SetHandlerInterface(typeof(GimmeMsgTypeResolver<>))
                    .SetMessageTypeResolver(msgT => msgT.GenericTypeArguments[0]) // unwrap from envelope
                )
                .Construct();
        }

        [Fact]
        public void ioc_asked_for_the_right_type()
        {
            _bus.Publish(new Envelope<int>(1));
            _testAdapter.TypePassedIn.ShouldBeEqualTo(typeof(GimmeMsgTypeResolver<int>));
        }

        [Fact]
        public void ioc_handler_is_being_used()
        {
            _bus.Publish(new Envelope<string>("Foo"));
            _testAdapter.TheSubscribedTypeResolver.CapturedMessage.ShouldBeEqualTo("Foo in envelope");
        }
    }
}