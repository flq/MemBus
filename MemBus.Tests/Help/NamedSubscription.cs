using System;

namespace MemBus.Tests.Help
{
    public class NamedSubscription : ISubscription
    {
        private readonly string name;
        private readonly Action action;
        private readonly ISubscription inner;

        public NamedSubscription(string name, Action action, ISubscription inner)
        {
            this.name = name;
            this.action = action;
            this.inner = inner;
        }

        public NamedSubscription(string name, ISubscription inner) : this(name, null, inner)
        {
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
            if (action != null)
                action();
            inner.Push(message);
            Pushed++;
        }

        protected int Pushed { get; private set; }

        public Type Handles
        {
            get { return inner.Handles; }
        }
    }
}