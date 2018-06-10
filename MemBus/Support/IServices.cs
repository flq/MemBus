namespace MemBus.Support
{
  public interface IServices
  {
    void Add<T>(T @object);
    void Remove<T>();
    T Get<T>();
    /// <summary>
    /// Gives you a summary of contained services
    /// </summary>
    string WhatDoIHave { get; }
  }

  public interface IServices<TARGET> : IServices
  {
    void AddExtension<T>(T extension) where T : IServicesExtension<TARGET>;    
    void RemoveExtension<T>() where T : IServicesExtension<TARGET>;
    TARGET Replace<T>(T @object);
    TARGET CloneContext();
  }

  public interface IServicesExtension<T>
  {
    void Attach(T target);
    void Remove(T target);
  }
}