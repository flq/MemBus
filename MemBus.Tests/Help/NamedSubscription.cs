using System;

namespace MemBus.Tests.Help
{
    public class NamedSubscription : ISubscription
    {
        private readonly string _name;
        private readonly Action _action;
        private readonly ISubscription _inner;

        public NamedSubscription(string name, Action action, ISubscription inner)
        {
            _name = name;
            _action = action;
            _inner = inner;
        }

        public NamedSubscription(string name, ISubscription inner) : this(name, null, inner)
        {
        }

        public ISubscription Inner
        {
            get { return _inner; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void Push(object message)
        {
            if (_action != null)
                _action();
            _inner.Push(message);
            Pushed++;
        }

        public bool Handles(Type messageType)
        {
            return _inner.Handles(messageType);
        }

        protected int Pushed { get; private set; }

       
    }
}