# README #

In-memory publish-subscribe bus with pluggable subscription and publishing pipeline, first-class support of Observables, contravariant messaging and resolution of handlers through pluggable IOC.
Will cover your event aggregation needs and then more.

The concepts of __configuration__, __publishing__, __subscribing__ and __modifying subscriptions__ are all separated from each other and pluggable.

The provided way to start a MemBus instance is going through the BusSetup.

    var _bus = BusSetup.StartWith<Conservative>().Construct();

**Conservative** is one of the already available Configurators contained in the ["Configurators"](https://github.com/flq/MemBus/tree/master/MemBus/Configurators) namespace.

As you can see from such a setup, what MemBus needs as a minimum is a default publish pipeline, a default subscriptionresolver, and a default strategy what to do with new subscriptions.

The bus that is now set up, and is of type **IBus**. IBus derives from **IPublisher** and **ISubscriber**.

## Basic Usage

    //Now you can do something like that:
    using (_bus.Subscribe((Foo x) => Console.Writeline(x.Text))) {
      _bus.Publish(new Foo("hello world"));
    }

Any subscription operation you undertake returns something of type **IDisposable**.

Publish/Subscribe message matching is by default done on the type of the message. Please note that message subscription is **contravariant**. I.e. a Method accepting a message of type object, will receive all messages published on MemBus.

There are no other rules apart from the following:

* Subscriptions match the `Action<T>` delegate
* There are no other rules apart from the previous

## Subscription overloads
If you look at the Subscribe signature, there are a couple of overloads that help you in your messaging needs:

<pre>
// Only get messages of type MessageB where the Id is "A"
b.Subscribe<MessageB>(msg => received++, c=>c.SetFilter(msg=>msg.Id == "A"));
// Receive MessageB on the DispatcherThread
b.Subscribe<MessageB>(msg => received++, c=>c.DispatchOnUiThread());
</pre>

However, much of that functionality can be obtained more elegantly via IObservable instances.

## Using Message reception via IObservable interface
It can be quite useful to obtain an `IObservable<T>` from your bus. There are 2 ways to do so:

    var observable = bus.Observe<FooMessage>();
    var observable = new MessageObservable<T>(bus);

This allows you to bring the Rx tastiness on top of messaging.

## Subscribing based on Convention.

You may notice that one Subscribe overload accepts any object. This works when you setup MemBus with the **FlexibleSubscribeAdapter**:

    bus = BusSetup.StartWith<Conservative>()
           .Apply<FlexibleSubscribeAdapter>(a => a.ByMethodName("Handle"))
           .Construct()
    
    class Subscriber {
      public void Handle(Foo msg) {}
    }
    var disposabble = _bus.Subscribe(new Subscriber());

The FlexibleSubscribeAdapter allows you to set up the convention by which subscibing methods are picked up. The configuration allows wiring up **"ByMethodName(string)"** or **"ByInterface(Type)"**. The interface may be generic, in that case you specify the open generic. The one rule of subscribing also applies in this scenario: Your subscriptions must match the `Action<T>` signature.

The **IDisposable** returned by the **Subcribe(object)** disposes of all subscriptions that were found on said object.

If your object implements **IAcceptsDisposeToken**, the disposable that is returned by the subscribe call will be passed into the object being subscribed. That way objects have a way to take themselves out of the messaging, e.g. when they handle a couple of messages only relevant in a limited time of your App.

## Publishing

There isn't a lot one can say about Publishing. You may pass any object instance into the __"Publish"__ method.

##Publishing to a DI Container

One use case of using MemBus is to dispatch handling of a message to an IOC container. Given a message, the implementation of some type is looked up, instantiated by the container and the message is delivered to the handling method.

In order to do that you need to configure MemBus with the __IocSupport__ option:

    _bus = BusSetup
        .StartWith<Conservative>()
        .Apply<IoCSupport>(s => s.SetAdapter(_testAdapter).SetHandlerInterface(typeof(GimmeMsg<>)))
        .Construct();

The Adapter is some instance that needs to implement [IocAdapter][1]. Implement this interface to bridge the request to your container of choice. The interface is very straightforward:

    public interface IocAdapter
    {
        IEnumerable<object> GetAllInstances(Type desiredType);
    }

Secondly, you declare the interface that will be requested from the IoCContainer. You need to apply the following rules with regard to the chosen interface:

* It needs to be generic with one type argument
* It provides a single void method with one argument. The argument type typically corresponds with the generic type argument.

<hr>

## Release History

### 1.3.0

MemBus now also comes with support for Silverlight 5. __Caution__: 
Only the Silverlight version has a dependency on Rx.Main, if you use the silverlight version, __manually add a dependency to Rx.Main__

### 1.2.0

Removed cloning feature from Bus, as it was only partially implemented and does not seem very useful in real life.
Added documentation xml.

### 1.1.2

Fixed bug related to removing and adding subscription where under certain conditions
the subscription will not be picked up for handling a message

### 1.1.0

PublishToken now has a Cancel property. 
If a publish pipeline member sets this to true, all subsequent pipeline members will not be called anymore

### 1.0.4

Added Infrastructure class "DeferredPublishPipelineMember" to separate the time-wise coupling of setting up the bus and being able
to construct an instance of "IPublishpipelineMember"

### 1.0.3

Removal of classes from MemBus that have no direct relationship to the core function of MemBus.

## Licensing ##

Copyright 2010 Frank-Leonardo Quednau ([realfiction.net](http://realfiction.net)) 
Licensed under the Apache License, Version 2.0 (the "License"); 
you may not use this solution except in compliance with the License. 
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, 
software distributed under the License is distributed on an "AS IS" 
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 