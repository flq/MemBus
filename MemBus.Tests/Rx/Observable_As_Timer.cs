using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using Xunit;

namespace MemBus.Tests.Rx
{
    public class Observable_As_Timer
    {
        readonly IBus _bus = BusSetup
            .StartWith<Conservative>()
            .Apply<FlexibleSubscribeAdapter>(cfg => cfg.RegisterMethods(info => true))
            .Construct();

        [Fact]
        public void Timer_from_observable_functionality()
        {
            var cd = new CountdownEvent(5);
            
            _bus.Subscribe(new Timers());

            using (_bus.Subscribe((MessageA _) => cd.Signal()))
            {
                var sw = Stopwatch.StartNew();
                var result = cd.Wait(TimeSpan.FromMilliseconds(700));
                if (!result)
                    throw new ArgumentException("TImer did not complete");
                var elapsed = sw.ElapsedMilliseconds;
                var diff = Math.Abs(elapsed - 410);
                Console.WriteLine(diff);
                Assert.True(diff < 70);
                Console.WriteLine(elapsed);
            }
        }

        private class Timers
        {
            public IObservable<MessageA> ASignal()
            {
                return Observable
                .Timer(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(100), Scheduler.Default)
                .Take(5)
                .Select(_ => new MessageA());
            } 
        }
    }
}

