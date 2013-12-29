using System;
using System.Diagnostics;
using System.Threading;
using MemBus.Configurators;

namespace MemBus.Tests.Performance
{
    class Program
    {
        private static void Main()
        {
            var bus = BusSetup.StartWith<Fast>().Construct();
            var s = new CompositeSubscriptionPerformanceTest();
            for (var i = 0; i < 10;i++)
              Run(s, bus);
            Console.ReadLine();
        }

        private static void Run(IScenario s, IBus bus)
        {
            var sw = Stopwatch.StartNew();
            s.Run(bus, Console.Out);
            Console.WriteLine("Done in " + sw.Elapsed.TotalSeconds + " seconds");
            Console.WriteLine("--");
            s.Reset();
        }
    }
    
}
