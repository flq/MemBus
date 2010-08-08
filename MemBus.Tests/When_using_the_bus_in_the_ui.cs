using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
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