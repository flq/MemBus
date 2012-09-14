using System;
using MemBus.Configurators;
using MemBus.Tests.Help;
using MemBus.Tests.Frame;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests.Integration
{
    [TestFixture]
    public class When_using_disposable_buses
    {
        [Test]
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