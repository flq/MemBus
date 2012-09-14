using System.Threading;
using System.Windows.Threading;
using MemBus.Configurators;
using MemBus.Tests.Help;
using MemBus.Tests.Frame;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests.Integration
{
    [TestFixture]
    public class When_using_the_bus_in_the_ui
    {

        [Test]
        public void Subscription_push_can_be_dispatched_on_designated_thread_blocking_scenario()
        {
            var threadId = -2;
            var threadIdFromTest = -1;
            IBus bus = null;

            var resetEvent = new ManualResetEvent(false);

            var uiThread = new Thread(
                () =>
                    {
                        Helpers.CreateDispatchContext();
                        var frame = new DispatcherFrame();
                        threadId = Thread.CurrentThread.ManagedThreadId;
                        bus = BusSetup.StartWith<RichClientFrontend>().Construct();
                        bus.Subscribe<MessageB>(
                            msg =>
                                {
                                    threadIdFromTest = Thread.CurrentThread.ManagedThreadId;
                                    frame.Continue = false;
                                },
                            c => c.DispatchOnUiThread());
                        resetEvent.Set();
                        Dispatcher.PushFrame(frame);
                    });
            uiThread.Start();
            resetEvent.WaitOne();
            bus.Publish(new MessageB());
            uiThread.Join();
            threadIdFromTest.ShouldBeEqualTo(threadId);
        }

        [Test]
        public void Subscription_push_can_be_dispatched_on_designated_thread_async_scenario()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var threadIdFromTest = -1;
            Helpers.CreateDispatchContext();

            var frame = new DispatcherFrame();
            var bus = BusSetup.StartWith<AsyncRichClientFrontend>().Construct();
            bus.Subscribe<MessageB>(
                msg =>
                    {
                        threadIdFromTest = Thread.CurrentThread.ManagedThreadId;
                        frame.Continue = false;
                    },
                c => c.DispatchOnUiThread());
            bus.Publish(new MessageB());
            Dispatcher.PushFrame(frame);
            threadIdFromTest.ShouldBeEqualTo(threadId);
        }

    }
}