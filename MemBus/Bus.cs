using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    internal class Bus : IConfigurableBus, IInternalBus
    {
        private readonly IInternalBus parent;
        private readonly CompositeResolver resolvers = new CompositeResolver();
        private readonly PublishPipeline publishPipeline;
        private readonly SubscriptionPipeline subscriptionPipeline = new SubscriptionPipeline();
        private readonly MessageBubbling messageBubbling = new MessageBubbling();
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();

        private readonly ConcurrentDictionary<int,IInternalBus> childBuses = new ConcurrentDictionary<int,IInternalBus>();

        private readonly DisposeContainer disposer;

        private volatile bool isDisposed;

        internal Bus():this(null) { }
        
        internal Bus(Bus parent)
        {
            this.parent = (IInternalBus)parent ?? new NullBus();
            if (parent == null)
            {
                //This is the root!
                publishPipeline = new PublishPipeline(this);
                disposer = new DisposeContainer(resolvers, publishPipeline, subscriptionPipeline, services);
            }
            else
            {
                //Take over pipelines
                //TODO: More correct is to clone the pipelines, and then properly dispose them
                publishPipeline = parent.publishPipeline;
                subscriptionPipeline = parent.subscriptionPipeline;
                //bubbling rules
                messageBubbling = parent.messageBubbling;
                //New subscribers!
                resolvers = new CompositeResolver(new TableBasedResolver());
                disposer = new DisposeContainer();
            }
        }

        void IConfigurableBus.ConfigureBubbling(Action<IConfigurableBubbling> configure)
        {
            configure(messageBubbling);
        }

        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            checkDisposed();
            configurePipeline(publishPipeline);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
            checkDisposed();
            configure(subscriptionPipeline);
        }

        void IConfigurableBus.AddResolver(ISubscriptionResolver resolver)
        {
            checkDisposed();
            resolver.TryInvoke(r => r.Services = services);
            resolvers.Add(resolver);
        }

        void IConfigurableBus.AddSubscription(ISubscription subscription)
        {
            checkDisposed();
            resolvers.Add(subscription);
        }

        void IConfigurableBus.AddAutomaton(object automaton)
        {
            checkDisposed();
            automatons.Add(automaton);
            automaton.TryInvoke(a => a.Bus = this);
            automaton.TryInvoke(a => a.Services = services);
        }

        void IConfigurableBus.AddService<T>(T service)
        {
            checkDisposed();
            services.Add(service);
        }

        public void Publish(object message)
        {
            checkDisposed();
            if (!childBuses.IsEmpty && messageBubbling.DescentAllowed(message.GetType()))
                publishToChilds(message);
            UpwardsPublish(message);
        }

        public void UpwardsPublish(object message)
        {
            var subs = resolvers.GetSubscriptionsFor(message);
            subs = subscriptionPipeline.Shape(subs, message);
            var t = new PublishToken(message, subs);
            publishPipeline.LookAt(t);
            if (messageBubbling.BubblingAllowed(message.GetType()))
                parent.UpwardsPublish(message);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return Subscribe(subscription, subscriptionPipeline.GetIntroductionShape());
        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization)
        {
            var subC = new SubscriptionCustomizer<M>(subscriptionPipeline.GetIntroductionShape(), services);
            customization(subC);
            return Subscribe(subscription, subC);
        }


        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            checkDisposed();
            var sub = customization.EnhanceSubscription(new MethodInvocation<M>(subscription));
            resolvers.Add(sub);
            return sub.TryReturnDisposerOfSubscription();
        }

        public IObservable<M> Observe<M>()
        {
            checkDisposed();
            return new MessageObservable<M>(this);
        }

        public IBus SpawnChild()
        {
            var spawnChild = new Bus(this);
            childBuses.TryAdd(spawnChild.GetHashCode(), spawnChild);
            return spawnChild;
        }

        public void ScratchFromChilds(IInternalBus bus)
        {
            IInternalBus b;
            childBuses.TryRemove(bus.GetHashCode(), out b);
        }

        public void Dispose()
        {
            disposer.Dispose();
            parent.ScratchFromChilds(this);
            isDisposed = true;
        }

        private void publishToChilds(object message)
        {
            childBuses.Values.Each(b=>b.Publish(message));
        }

        private void checkDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Bus");
        }
    }
}