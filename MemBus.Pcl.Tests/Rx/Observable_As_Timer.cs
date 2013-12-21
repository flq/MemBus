using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using MemBus.Configurators;
using MemBus.Tests.Help;
using MemBus.Messages;
using NUnit.Framework;
using Should;

namespace MemBus.Tests.Rx
{
    [TestFixture]
    public class Observable_As_Timer
    {
        readonly IBus _bus = BusSetup.StartWith<Conservative>().Construct();

        [Test]
        public void Timer_from_observable_functionality()
        {
            var timer = Observable
                .Timer(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(100), Scheduler.Default)
                .Take(5)
                .Select(_ => new MessageA());
            var cd = new CountdownEvent(5);
            
            using (_bus.Subscribe((MessageA _) => cd.Signal()))
            {
                _bus.Publish(timer);
                var sw = Stopwatch.StartNew();
                var result = cd.Wait(TimeSpan.FromMilliseconds(600));
                if (!result)
                    Assert.Fail("Timer did not complete");
                var elapsed = sw.ElapsedMilliseconds;
                Math.Abs(elapsed - 410).ShouldBeLessThan(70);
                Debug.WriteLine(elapsed);
            }
        }
    }
}

