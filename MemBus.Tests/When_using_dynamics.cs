using System;
using MemBus.Support;
using MemBus.Tests.Help;
using MemBus;
using MemBus.Tests.Frame;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests
{
    [TestFixture]
    public class When_using_dynamics
    {
        private SampleClass s = new SampleClass();

        [Test]
        public void Existence_of_properties_can_be_checked()
        {
            s.RespondsTo(d=>d.Name).ShouldBeTrue();
            s.RespondsTo(d=>d.Boy).ShouldBeFalse();
        }

        [Test]
        public void Existence_of_methods_can_be_checked()
        {
            s.RespondsTo(d => d.Hello("bla")).ShouldBeTrue();
            s.RespondsTo(d => d.Sort()).ShouldBeFalse();
        }

        [Test]
        public void Existence_of_events_can_be_checked()
        {
            s.RespondsTo(d => d.MyEvent += null).ShouldBeTrue();
            s.RespondsTo(d => d.MyOtherEvent += null).ShouldBeFalse();
        }

        [Test]
        public void Existence_of_methods_also_consider_param_types()
        {
            s.RespondsTo(d => d.Hello(1)).ShouldBeFalse();
        }

        [Test]
        public void Existence_of_methods_also_consider_param_count()
        {
            s.RespondsTo(d => d.Hello("bla", "bli")).ShouldBeFalse();
        }

        [Test]
        public void Sufficient_that_params_are_assignable()
        {
            s.RespondsTo(d => d.SetFoo(new Foo())).ShouldBeTrue();
        }

        [Test]
        public void Passing_null_as_param_is_ok_for_classes()
        {
            s.RespondsTo(d => d.Hello(null)).ShouldBeTrue();
        }

        [Test]
        public void Passing_null_as_param_is_bad_for_structs()
        {
            s.RespondsTo(d => d.SetDate(null)).ShouldBeFalse();
        }

        [Test]
        public void Passing_null_as_property_is_ok_for_classes()
        {
            s.RespondsTo(d => d.Name = null).ShouldBeTrue();
        }

        [Test]
        public void Passing_null_as_property_is_bad_for_structs()
        {
            s.RespondsTo(d => d.AValue = null).ShouldBeFalse();
        }

        [Test]
        public void Existence_of_setters_are_honoured()
        {
            s.RespondsTo(d=>d.Name = "Jones").ShouldBeTrue();
            s.RespondsTo(d => d.Birthdate = DateTime.Now).ShouldBeFalse();
            s.RespondsTo(d => d.Name = true).ShouldBeFalse();
        }

        [Test]
        public void Try_Invoke_existing_setter()
        {
            s.TryInvoke(d => d.Name = "Jones").ShouldBeTrue();
            s.Name.ShouldBeEqualTo("Jones");
        }

        [Test]
        public void Try_invoke_no_setter()
        {
            s.TryInvoke(d => d.Birthdate = DateTime.Now).ShouldBeFalse();
        }

        [Test]
        public void Try_Invoke_ForMethods()
        {
            s.TryInvoke(d => d.Hello("Jones"));
            s.HelloEntry.ShouldBeEqualTo("Jones");
        }
    }

    
    
}