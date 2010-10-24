using System;
using MemBus;
using Membus.Reactive.Tests.Help;
using System.Linq;

namespace Membus.Reactive.Tests
{
    public class RxBasedFooObservable : RxEnabledObservable<MessageA>
    {
        public RxBasedFooObservable(IBus bus) : base(bus)
        {
        }

        protected override IObservable<MessageA> constructObservable(IObservable<MessageA> startingPoint)
        {
            return startingPoint.Where(msgA => msgA.Name == "Foo");
        }
    }
}