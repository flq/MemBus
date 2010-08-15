using System;
using MemBus.Subscribing;

namespace MemBus.Setup
{
    public interface IConfigurableSubscribing
    {
        void DefaultShapeOutwards(params ISubscriptionShaper[] shapers);
        void MessageMatch(Func<MessageInfo, bool> match, Action<IConfigureSubscription> configure);
    }

    public interface IConfigureSubscription
    {
        void ShapeOutwards(params ISubscriptionShaper[] shapers);
    }
}