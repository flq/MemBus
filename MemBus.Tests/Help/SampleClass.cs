using System;

namespace MemBus.Tests.Help
{
    public class SampleClass
    {
        public string HelloEntry;

        public string Name { get; set; }

        public DateTime Birthdate { get; private set; }

        public bool AValue { get; set; }

        public bool Hello(string bla)
        {
            HelloEntry = bla;
            return true;
        }

        public void SetDate(DateTime date)
        {
            
        }
    }
}