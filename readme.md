# MemBus Readme #

[![Build status](https://ci.appveyor.com/api/projects/status/81caaynficf9n79d?svg=true)](https://ci.appveyor.com/project/flq/membus)

In-memory publish-subscribe bus with pluggable subscription and publishing pipeline, first-class support of Observables, contravariant messaging and resolution of handlers through pluggable IOC.
Will cover your event aggregation needs and then more.

The concepts of __configuration__, __publishing__, __subscribing__ and __modifying subscriptions__ are all separated from each other and pluggable.

The provided way to start a MemBus instance is going through the BusSetup.

    var _bus = BusSetup.StartWith<Conservative>().Construct();

**Conservative** is one of the already available Configurators contained in the ["Configurators"](MemBus.Pcl/Configurators) namespace.

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

There are three ways to subscribe handlers in MemBus

* Through an Action - this is the straightforward way as seen in the intro
* By observing the bus via `Observe<M>` - returns an observable of type M. If you take a reference on Rx-* packages, you can set up all sorts of goodness on top of it, like filtering, merging, joining, handling messages on a synchronization context, etc, etc.
* Passing any object instance - this functionality is based on the **FlexibleSubscribeAdapter** and is described next.

## Subscribing based on Convention.

You may notice that one Subscribe overload accepts any object. This works when you setup MemBus with the **FlexibleSubscribeAdapter**:

    bus = BusSetup.StartWith<Conservative>()
           .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
           .Construct()
    
    class Subscriber {
      public void Handle(Foo msg) {}
    }
    var disposable = _bus.Subscribe(new Subscriber());

The FlexibleSubscribeAdapter allows you to set up the convention by which subscribing methods are picked up. The configuration allows wiring up through **"RegisterMethods"** or **"ByInterface(Type)"**. The interface may be generic, in that case you specify the open generic. The one rule of subscribing also applies in this scenario: Your subscriptions must match the `Action<T>` signature.

The **IDisposable** returned by the **Subcribe(object)** disposes of all subscriptions that were found on said object.

If your object implements **IAcceptsDisposeToken**, the disposable that is returned by the subscribe call will be passed into the object being subscribed. That way objects have a way to take themselves out of the messaging, e.g. when they handle a couple of messages only relevant in a limited time of your App.

Your subscribing objects can also work with **IObservable**. This means they can either accept IObservable instance, return an instance, or both. These methods will also be picked up and hooked into MemBus. That way you can write instances that do not have any dependency on MemBus but can 

* Receive messages
* Send messages upon reception of one (Also multiple ones by returning something *enumerable*).
* Receive Observables so you can use Rx tastiness on it
* Publish Observables
* The two above together for Rx-transformation tastiness.

## Publishing

There isn't a lot one can say about Publishing. You may pass 

* any object instance into the __"Publish"__ method.
* An Observable - in this case MemBus will dispatch any observed messages to the MemBus infrastructure. If the Observable raises an exception, the **ExceptionOccurred** messagewill be sent. Once the observale completes, the **MessageStreamCompleted** message will be dispatched.
* Publish in an awaitable fashion - in this case, any configuration with regard to handling the message will be short-circuited an an awaitable version of a **SequentialPublisher** will be used.

## Publishing to a DI Container

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

## Licensing ##

Copyright 2014 Frank-Leonardo Quednau ([realfiction.net](http://realfiction.net)) 
Licensed under the Apache License, Version 2.0 (the "License"); 
you may not use this solution except in compliance with the License. 
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, 
software distributed under the License is distributed on an "AS IS" 
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 
