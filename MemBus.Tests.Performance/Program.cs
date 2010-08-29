using System;
using MemBus.Configurators;

namespace MemBus.Tests.Performance
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                var bus = BusSetup.StartWith<AsyncConfiguration>().Construct();
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
            Count++;
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
            Count++;
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
            Count++;
        }
    }
    
}
