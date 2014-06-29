using System.Reactive.Linq;
using System;
using System.Threading;
using System.Windows.Threading;
using MemBus.Configurators;
using MemBus.Tests.Help;

using NUnit.Framework;

namespace MemBus.Tests.Integration
{
	[TestFixture, Ignore("Ignored while working on mono")]
    public class When_Using_The_Bus_In_The_Ui
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
                        var sync = Helpers.CreateDispatchContext();
                        var frame = new DispatcherFrame();
                        threadId = Thread.CurrentThread.ManagedThreadId;
                        bus = BusSetup.StartWith<RichClientFrontend>().Construct();
                        bus.Observe<MessageB>().ObserveOn(sync).Subscribe(msg =>
                        {
                            threadIdFromTest = Thread.CurrentThread.ManagedThreadId;
                            frame.Continue = false;
                        });
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
            var sync = Helpers.CreateDispatchContext();

            var frame = new DispatcherFrame();
            var bus = BusSetup.StartWith<AsyncRichClientFrontend>().Construct();
            bus.Observe<MessageB>().ObserveOn(sync).Subscribe(
                msg =>
                    {
                        threadIdFromTest = Thread.CurrentThread.ManagedThreadId;
                        frame.Continue = false;
                    });
            bus.Publish(new MessageB());
            Dispatcher.PushFrame(frame);
            threadIdFromTest.ShouldBeEqualTo(threadId);
        }

    }
}