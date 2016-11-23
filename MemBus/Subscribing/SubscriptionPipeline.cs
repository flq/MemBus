using System;
using System.Collections.Generic;
using MemBus.Setup;
using MemBus.Subscribing;
using System.Linq;
using MemBus.Support;

namespace MemBus
{
    internal class SubscriptionPipeline : IConfigurableSubscribing, IDisposable
    {
        private readonly IServices _services;
        private readonly List<ShapeProvider> _shapeProviders = new List<ShapeProvider>();
        private SubscriptionShaperAggregate _introductionShape;

        public SubscriptionPipeline(IServices services)
        {
            _services = services;
        }

        /// <summary>
        /// Return a shaper aggregate that is applied directly to a newly introduced subscription.
        /// The returned instance can be further modified without affecting the infrastructure
        /// </summary>
        public SubscriptionShaperAggregate GetIntroductionShape()
        {
            return _introductionShape != null ? _introductionShape.Clone() : new SubscriptionShaperAggregate(new [] {new ShapeToPassthrough()});
        }

        public IEnumerable<ISubscription> Shape(IEnumerable<ISubscription> subscriptions, object message)
        {
            var info = new MessageInfo(message);
            var resultingSubscriptions = subscriptions;
            for (int i = _shapeProviders.Count - 1; i >= 0; i--) //Backwards as we keep the default at index 0
            {
                if (!_shapeProviders[i].Handles(info))
                    continue;
                resultingSubscriptions = _shapeProviders[i].Enhance(subscriptions);
                break;
            }
            return resultingSubscriptions;
        }

        void IConfigurableSubscribing.DefaultShapeOutwards(params ISubscriptionShaper[] shapers)
        {
            var sp = new ShapeProvider(msg => true, _services);
            ((IConfigureSubscription)sp).ShapeOutwards(shapers);
            _shapeProviders.Insert(0, sp);
        }

        

        void IConfigurableSubscribing.MessageMatch(Func<MessageInfo, bool> match, Action<IConfigureSubscription> configure)
        {
            var sp = new ShapeProvider(match, _services);
            configure(sp);
            _shapeProviders.Add(sp);
        }

        void IConfigurableSubscribing.ApplyOnNewSubscription(params ISubscriptionShaper[] shapers)
        {
            _introductionShape = new SubscriptionShaperAggregate(shapers);
        }

        public void Dispose()
        {
            _shapeProviders.Clear();
        }
    }

    internal class ShapeProvider : IConfigureSubscription, ISubscriptionShaper
    {
        private readonly Func<MessageInfo, bool> _match;
        private readonly IServices _services;
        private readonly SubscriptionShaperAggregate _shaperAggregate = new SubscriptionShaperAggregate();

        public ShapeProvider(Func<MessageInfo,bool> match, IServices services)
        {
            if (match == null)
              throw new ArgumentNullException("match");
            _match = match; 
            _services = services;
        }

        public bool Handles(MessageInfo info)
        {
            return _match(info);
        }

        void IConfigureSubscription.ShapeOutwards(params ISubscriptionShaper[] shapers)
        {
            foreach (var s in shapers)
            {
                s.Being<IRequireServices>(d => d.AddServices(_services));
                _shaperAggregate.Add(s);
            }
        }

        public IEnumerable<ISubscription> Enhance(IEnumerable<ISubscription> subscriptions)
        {
            return subscriptions.Select(EnhanceSubscription);
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return _shaperAggregate.EnhanceSubscription(subscription);
        }
    }
}