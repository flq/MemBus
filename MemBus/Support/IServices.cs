namespace MemBus.Support
{
  public interface IServices
  {
    /// <summary>
    /// Adds an object to the services collection
    /// </summary>
    /// <param name="object">The object to add</param>
    /// <typeparam name="T">The type of the object</typeparam>
    void Add<T>(T @object);
    
    /// <summary>
    /// Removes the object of type T from the collection.
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    // ReSharper disable UnusedMember.Global
    void Remove<T>();
    
    /// <summary>
    /// Gets the object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <returns>Object of Type T or the default value of T if not present</returns>
    T Get<T>();
    
    /// <summary>
    /// Gives you a summary of contained services
    /// </summary>
    string WhatDoIHave { get; }
    // ReSharper restore UnusedMember.Global
    
  }
}