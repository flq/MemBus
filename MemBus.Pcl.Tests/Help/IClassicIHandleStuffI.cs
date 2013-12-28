using System;

namespace MemBus.Tests.Help
{
    public interface IClassicIHandleStuffI<in T>
    {
        void Gimme(T theThing);
    }

    public interface IWeirdHandler<T>
    {
        void Handle(T msg);
        IObservable<T> Producer();
    }

    public interface IInvalidHandlerInterfaceBecauseNoParameter
    {
        void Gimme();
    }

    class InvalidHandlerInterfaceBecauseNoParameter : IInvalidHandlerInterfaceBecauseNoParameter
    {
        public void Gimme() { }
    }

    public interface IInvalidHandlerInterfaceBecauseTwoMethodsOfrequestedPattern
    {
        void Gimme(object thing);
        void Gamme(object thang);
    }

    class InvalidHandlerInterfaceBecauseTwoMethodsOfrequestedPattern : IInvalidHandlerInterfaceBecauseTwoMethodsOfrequestedPattern
    {
        public void Gimme(object thing){ }

        public void Gamme(object thang) { }
    }

    public interface IInvalidHandlerInterfaceBecauseTwoParams
    {
        void Gimme(object thing, object thang);
    }

    class InvalidHandlerInterfaceBecauseTwoParams : IInvalidHandlerInterfaceBecauseTwoParams
    {
        public void Gimme(object thing, object thang) { }
    }

    public interface IInvalidHandlerInterfaceBecauseReturnType
    {
        int Gimme(object thing);
    }

    class InvalidHandlerInterfaceBecauseReturnType : IInvalidHandlerInterfaceBecauseReturnType
    {
        public int Gimme(object thing) { return 0; }
    }
}