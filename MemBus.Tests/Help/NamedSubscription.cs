using System;

namespace MemBus.Tests.Help
{
    public class NamedSubscription : ISubscription
    {
        private readonly string name;
        private readonly ISubscription inner;

        public NamedSubscription(string name, ISubscription inner)
        {
            this.name = name;
            this.inner = inner;
        }

        public ISubscription Inner
        {
            get { return inner; }
        }

        public string Name
        {
            get { return name; }
        }

        public void Push(object message)
        {
            Pushed++;
        }

        protected int Pushed { get; private set; }

        public Type Handles
        {
            get { return typeof(object); }
        }
    }
}