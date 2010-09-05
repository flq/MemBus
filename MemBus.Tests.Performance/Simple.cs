using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MemBus.Messages;
using MemBus.Support;

namespace MemBus.Tests.Performance
{
    public class Simple : IScenario
    {
        private int aCount;
        private int bCount;
        private int cCount;

        private DisposeContainer c = new DisposeContainer();

        public void Run(IBus bus, TextWriter w)
        {
            c.Add(bus.Subscribe<MessageA>(onMessageA));
            c.Add(bus.Subscribe<MessageB>(onMessageB));
            c.Add(bus.Subscribe<MessageC>(onMessageC));

            var r = new Random();
            var dict = new Dictionary<int, Func<object>>
                           {
                               {0, () => new MessageA()},
                               {1, () => new MessageB()},
                               {2, () => new MessageC()},
                           };
            int count = 0;
            var sw = Stopwatch.StartNew();
            while (count < 100000)
            {
                bus.Publish(dict[r.Next(0, 3)]());
                count++;
            }

            w.WriteLine("Through {0}", sw.ElapsedMilliseconds);

            count = 0;
            while (count < 10)
            {
                w.WriteLine("From MsgA:{0}({1}), B:{2}({3}), C:{4}({5})", aCount, MessageA.Count, bCount,
                            MessageB.Count, cCount, MessageC.Count);
                Thread.Sleep(1000);
                count++;
            }
        }

        public void Reset()
        {
            c.Dispose();
            MessageA.Reset();
            MessageB.Reset();
            MessageC.Reset();
            aCount = 0;
            bCount = 0;
            cCount = 0;
        }

        private void onMessageC(MessageC obj)
        {
            Interlocked.Increment(ref cCount);
        }

        private void onMessageB(MessageB obj)
        {
            Thread.Sleep(1);
            Interlocked.Increment(ref bCount);
        }

        private void onMessageA(MessageA obj)
        {
            Interlocked.Increment(ref aCount);
        }

        private void onException(ExceptionOccurred o)
        {
            Console.WriteLine(o);
        }

    }
}