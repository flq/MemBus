using System;
using System.Collections.Generic;
using MemBus.Subscribing;
using System.Linq;

namespace MemBus
{
    internal class SubscriptionPipeline : IConfigurableSubscribing
    {
        private readonly List<ShapeProvider> shapeProviders = new List<ShapeProvider>();

        public IEnumerable<ISubscription> Shape(IEnumerable<ISubscription> subscriptions, object message)
        {
            var info = new MessageInfo(message);
            var resultingSubscriptions = subscriptions;
            for (int i = shapeProviders.Count - 1; i >= 0; i--) //Backwards as we keep the default at index 0
            {
                if (!shapeProviders[i].Handles(info))
                    continue;
                resultingSubscriptions = shapeProviders[i].Enhance(subscriptions);
                break;
            }
            return resultingSubscriptions;
        }

        void IConfigurableSubscribing.MessageMatch(Func<MessageInfo, bool> match, Action<IConfigureSubscription> configure)
        {
            var sp = new ShapeProvider(match);
            configure(sp);
            shapeProviders.Add(sp);
        }
    }

    internal class ShapeProvider : IConfigureSubscription
    {
        private readonly Func<MessageInfo, bool> match;
        private readonly SubscriptionMatroschka matroschka = new SubscriptionMatroschka();

        public ShapeProvider(Func<MessageInfo,bool> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            this.match = match;
        }

        public bool Handles(MessageInfo info)
        {
            return match(info);
        }


        void IConfigureSubscription.ShapeOutwards(params ISubscriptionShaper[] shapers)
        {
            foreach (var s in shapers)
                matroschka.Add(s);
        }

        public IEnumerable<ISubscription> Enhance(IEnumerable<ISubscription> subscriptions)
        {
            return subscriptions.Select(matroschka.EnhanceSubscription);
        }
    }
}