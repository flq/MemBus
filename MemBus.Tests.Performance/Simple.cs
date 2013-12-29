using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MemBus.Support;

namespace MemBus.Tests.Performance
{
    public class Simple : IScenario
    {
        private int _aCount;
        private int _bCount;
        private int _cCount;

        private readonly DisposeContainer _c = new DisposeContainer();

        public void Run(IBus bus, TextWriter w)
        {
            _c.Add(bus.Subscribe<MessageA>(OnMessageA));
            _c.Add(bus.Subscribe<MessageB>(OnMessageB));
            _c.Add(bus.Subscribe<MessageC>(OnMessageC));

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

            while (_aCount != MessageA.Count && _bCount != MessageB.Count && _cCount != MessageC.Count)
            {
                WriteInfo(w);
                Thread.Sleep(1000);
            }
            WriteInfo(w);
        }

        private void WriteInfo(TextWriter w)
        {
            w.WriteLine("From MsgA:{0}({1}), B:{2}({3}), C:{4}({5})", _aCount, MessageA.Count, _bCount,
                MessageB.Count, _cCount, MessageC.Count);
        }

        public void Reset()
        {
            _c.Dispose();
            MessageA.Reset();
            MessageB.Reset();
            MessageC.Reset();
            _aCount = 0;
            _bCount = 0;
            _cCount = 0;
        }

        private void OnMessageC(MessageC obj)
        {
            Interlocked.Increment(ref _cCount);
        }

        private void OnMessageB(MessageB obj)
        {
            //Thread.Sleep(1);
            Interlocked.Increment(ref _bCount);
        }

        private void OnMessageA(MessageA obj)
        {
            Interlocked.Increment(ref _aCount);
        }

        class MessageA
        {
            public static int Count;

            public static void Reset()
            {
                Count = 0;
            }

            public MessageA()
            {
                Interlocked.Increment(ref Count);
            }
        }

        class MessageB
        {
            public static int Count;

            public static void Reset()
            {
                Count = 0;
            }

            public MessageB()
            {
                Interlocked.Increment(ref Count);
            }
        }

        class MessageC
        {
            public static int Count;

            public static void Reset()
            {
                Count = 0;
            }

            public MessageC()
            {
                Interlocked.Increment(ref Count);
            }
        }
    }
}