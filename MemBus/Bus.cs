using System;
using System.Collections.Generic;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus
{
    internal class Bus : IConfigurableBus, IBus
    {
        private readonly BusSetup busSetup;
        private readonly PublishPipeline publishPipeline;
        private readonly Subscriber subscriber;
        private readonly List<object> automatons = new List<object>();
        private readonly IServices services = new StandardServices();

        private readonly DisposeContainer disposer;

        private volatile bool isDisposed;

        internal Bus()
        {
            publishPipeline = new PublishPipeline(this);
            subscriber = new Subscriber(services);
            disposer = new DisposeContainer { publishPipeline, subscriber, (IDisposable)services };
        }

        public Bus(BusSetup busSetup) : this()
        {
            this.busSetup = busSetup;
        }


        void IConfigurableBus.ConfigurePublishing(Action<IConfigurablePublishing> configurePipeline)
        {
            checkDisposed();
            configurePipeline(publishPipeline);
        }

        public void ConfigureSubscribing(Action<IConfigurableSubscribing> configure)
        {
           ((IConfigurableSubscriber)subscriber).ConfigureSubscribing(configure);
        }

        void IConfigurableSubscriber.AddResolver(ISubscriptionResolver resolver)
        {
            ((IConfigurableSubscriber)subscriber).AddResolver(resolver);
        }

        void IConfigurableSubscriber.AddSubscription(ISubscription subscription)
        {
            ((IConfigurableSubscriber)subscriber).AddSubscription(subscription);
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
            var subs = subscriber.GetSubscriptionsFor(message);
            var t = new PublishToken(message, subs);
            publishPipeline.LookAt(t);
        }

        public IDisposable Subscribe<M>(Action<M> subscription)
        {
            return subscriber.Subscribe(subscription);
        }

        public IDisposable Subscribe(object subscriber)
        {
            return this.subscriber.Subscribe(subscriber);
        }

        public IDisposable Subscribe<M>(Action<M> subscription, Action<ISubscriptionCustomizer<M>> customization)
        {
            return subscriber.Subscribe(subscription, customization);
        }


        public IDisposable Subscribe<M>(Action<M> subscription, ISubscriptionShaper customization)
        {
            return subscriber.Subscribe(subscription, customization);
        }

        public IObservable<M> Observe<M>()
        {
            return subscriber.Observe<M>();
        }

        public void Dispose()
        {
            disposer.Dispose();
            isDisposed = true;
        }

        private void checkDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Bus");
        }
    }
}