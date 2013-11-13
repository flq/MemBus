using System;
using System.Reactive.Linq;

namespace MemBus.Tests.Help
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