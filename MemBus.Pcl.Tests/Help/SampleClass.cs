using System;

namespace MemBus.Tests.Help
{
    public class SampleClass
    {
        #pragma warning disable 0067
        public event EventHandler MyEvent;

        public string HelloEntry;

        public string Name { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
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

        public void SetFoo(IFoo foo)
        {
            
        }
    }

    public interface IFoo
    {
    }

    class Foo : IFoo
    {
    }
}