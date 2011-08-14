namespace MemBus.Setup
{
    /// <summary>
    /// An interface that follws an extension pattern in which you accept an instance of the specified type
    /// </summary>
    public interface ISetup<T>
    {
        /// <summary>
        /// Accept an instance of type T
        /// </summary>
        void Accept(T setup);
    }
}