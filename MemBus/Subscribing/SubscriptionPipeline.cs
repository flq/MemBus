using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Subscribing;
using System.Linq;

namespace MemBus
{
    internal class SubscriptionPipeline : IConfigurableSubscribing, IDisposable
    {
        private readonly List<ShapeProvider> shapeProviders = new List<ShapeProvider>();
        private SubscriptionShaperAggregate introductionShape;

        /// <summary>
        /// Return a shaper aggregate that is applied directly to a newly introduced subscription.
        /// The returned instance can be further modified without affecting the infrastructure
        /// </summary>
        public SubscriptionShaperAggregate GetIntroductionShape()
        {
            return introductionShape != null ? introductionShape.Clone() : new SubscriptionShaperAggregate(new [] {new ShapeToPassthrough()});
        }

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

        void IConfigurableSubscribing.DefaultShapeOutwards(params ISubscriptionShaper[] shapers)
        {
            var sp = new ShapeProvider(msg => true);
            ((IConfigureSubscription)sp).ShapeOutwards(shapers);
            shapeProviders.Insert(0, sp);
        }

        

        void IConfigurableSubscribing.MessageMatch(Func<MessageInfo, bool> match, Action<IConfigureSubscription> configure)
        {
            var sp = new ShapeProvider(match);
            configure(sp);
            shapeProviders.Add(sp);
        }

        void IConfigurableSubscribing.ShapeToApplyWhenIntroducingSubscription(params ISubscriptionShaper[] shapers)
        {
            introductionShape = new SubscriptionShaperAggregate(shapers);
        }

        public void Dispose()
        {
            shapeProviders.Clear();
        }
    }

    internal class ShapeProvider : IConfigureSubscription, ISubscriptionShaper
    {
        private readonly Func<MessageInfo, bool> match;
        private readonly SubscriptionShaperAggregate shaperAggregate = new SubscriptionShaperAggregate();

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
                shaperAggregate.Add(s);
        }

        public IEnumerable<ISubscription> Enhance(IEnumerable<ISubscription> subscriptions)
        {
            return subscriptions.Select(EnhanceSubscription);
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return shaperAggregate.EnhanceSubscription(subscription);
        }
    }
}