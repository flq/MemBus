using System;
using System.Reactive.Linq;

namespace MemBus.Tests.Help
{
    public class RxBasedFooObservable : RxEnabledObservable<Membus.Reactive.Tests.Help.MessageA>
    {
        public RxBasedFooObservable(IBus bus) : base(bus)
        {
        }

        protected override IObservable<Membus.Reactive.Tests.Help.MessageA> constructObservable(IObservable<Membus.Reactive.Tests.Help.MessageA> startingPoint)
        {
            return startingPoint.Where(msgA => msgA.Name == "Foo");
        }
    }
}