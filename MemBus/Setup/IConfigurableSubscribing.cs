using System;
using MemBus.Subscribing;

namespace MemBus.Setup
{
    public interface IConfigurableSubscribing
    {
        /// <summary>
        /// These shapes are applied to subscription if none other matched during publishing of the pipeline
        /// </summary>
        void DefaultShapeOutwards(params ISubscriptionShaper[] shapers);

        /// <summary>
        /// Describe what shapes are applied to a subscription when a match is given to the message that is currently being published
        /// </summary>
        void MessageMatch(Func<MessageInfo, bool> match, Action<IConfigureSubscription> configure);

        /// <summary>
        /// These shapes are applied directly when introducing a subscription to the Bus
        /// </summary>
        void ApplyOnNewSubscription(params ISubscriptionShaper[] shapers);
    }

    public interface IConfigureSubscription
    {
        void ShapeOutwards(params ISubscriptionShaper[] shapers);
    }
}