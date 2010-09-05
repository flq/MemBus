using System;
using System.Threading;
using MemBus.Configurators;

namespace MemBus.Tests.Performance
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                var bus = BusSetup.StartWith<Fast>().Construct();
                var s = new Simple();
                s.Run(bus, Console.Out);
                Console.ReadLine();
                s.Reset();
            }
        }        
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
