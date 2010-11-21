namespace MemBus.Tests.Help
{
    public interface IClassicIHandleStuffI<in T>
    {
        void Gimme(T theThing);
    }

    public interface IInvalidHandlerInterfaceBecauseNoParameter
    {
        void Gimme();
    }

    public interface IInvalidHandlerInterfaceBecauseTwoMethodsOfrequestedPattern
    {
        void Gimme(object thing);
        void Gamme(object thang);
    }

    public interface IInvalidHandlerInterfaceBecauseTwoParams
    {
        void Gimme(object thing, object thang);
    }

    public interface IInvalidHandlerInterfaceBecauseReturnType
    {
        int Gimme(object thing);
    }
}