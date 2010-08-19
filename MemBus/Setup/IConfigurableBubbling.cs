namespace MemBus.Setup
{
    public interface IConfigurableBubbling
    {
        /// <summary>
        /// Usually, messages published on a spawned bus are not registered by parent buses.
        /// Add exceptions to this rule by calling this configuration method. When a message of that
        /// type is published on a child bus, it will also reach subscribers of parents
        /// </summary>
        void BubblingForMessage<T>();

        /// <summary>
        /// Usually messages published on a parenting bus will descend to child buses and be published there as well.
        /// Add an exception to this rule with this method, such that the message is not published to childs.
        /// </summary>
        void BlockDescentOfMessage<T>();
    }
}