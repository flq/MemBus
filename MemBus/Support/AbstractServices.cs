using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Support
{
  /// <summary>
  /// Use this context class if a standard, untyped context is completely sufficient for you
  /// </summary>
  public class StandardServices : AbstractServices<StandardServices> 
  {

  }

  /// <summary>
  /// Context class with an extension pattern as seen in WCF. You can Add and remove instances of any type.
  /// However, you can keep ONLY ONE instance of every kind.
  /// </summary>
  /// <typeparam name="TARGET">The class that is inheriting from Context</typeparam>
  public abstract class AbstractServices<TARGET> 
    : IServices<TARGET>, IDisposable, IEnumerable<object> where TARGET : AbstractServices<TARGET>
  {

    /// <summary>
    /// The list of objects attached to this context. These may be context extensions
    /// or other objects
    /// </summary>
    private Dictionary<Type,IAttachedObject> _attachedObjects =
      new Dictionary<Type, IAttachedObject>();

    /// <summary>
    /// Adds a OCntext extension to this context instance
    /// </summary>
    /// <typeparam name="T">The type of the extension</typeparam>
    /// <param name="extension">The extension which must implement the <see cref="IServicesExtension{T}"/> interface</param>
    public virtual void AddExtension<T>(T extension) where T : IServicesExtension<TARGET>
    {
      if (_attachedObjects.ContainsKey(typeof(T)))
        throw new ArgumentException(string.Format("An object of Type {0} is already attached to this context", typeof(T).Name));
      _attachedObjects.Add(typeof(T), (AttachedObject<T>)extension);
      extension.Attach((TARGET)this);
    }

    /// <summary>
    /// Remove an extension
    /// </summary>
    /// <typeparam name="T">The type of the extension to remove</typeparam>
    public virtual void RemoveExtension<T>() where T : IServicesExtension<TARGET>
    {
      if (!_attachedObjects.ContainsKey(typeof(T))) return;
      Remove((IServicesExtension<TARGET>)_attachedObjects[typeof(T)].TheObject);
    }

    /// <summary>
    /// Remove a specific extension
    /// </summary>
    /// <param name="extension">The extension instance to remove</param>
    public virtual void Remove(IServicesExtension<TARGET> extension)
    {
      var ext = _attachedObjects.Values.Where(e => e.TheObject == extension).FirstOrDefault();
      if (ext == null) return;
      ((IServicesExtension<TARGET>)ext.TheObject).Remove((TARGET)this);
      _attachedObjects.Remove(ext.TheObject.GetType());
    }

    /// <summary>
    /// Get the Object from the context of type T
    /// </summary>
    /// <typeparam name="T">the type of the object to retrieve</typeparam>
    /// <returns>The instance of said type or teh default value of T</returns>
    public virtual T Get<T>()
    {
      IAttachedObject obj;
      _attachedObjects.TryGetValue(typeof (T), out obj);
      return obj != null ? (T)obj.TheObject : default(T);
    }

    /// <summary>
    /// Add any object of type T
    /// </summary>
    /// <typeparam name="T">The type of teh object to be added</typeparam>
    /// <param name="object">The instance to be added</param>
    public virtual void Add<T>(T @object)
    {
      Add(@object, true);
    }

    ///<summary>
    /// Add.
    ///</summary>
    public virtual void Add<T>(T @object, bool disposable)
    {
      if (_attachedObjects.ContainsKey(typeof(T)))
        throw new ArgumentException($"An object of Type {typeof(T).Name} is already attached to this context");
      _attachedObjects.Add(typeof(T), new AttachedObject<T>(disposable, @object));
    }

    ///<summary>
    /// Force add.
    ///</summary>
    ///<param name="object"></param>
    ///<typeparam name="T"></typeparam>
    protected void ForceAdd<T>(T @object)
    {
      _attachedObjects[typeof (T)] = (AttachedObject<T>)@object;
      if ((@object != null) && @object.GetType().ImplementsInterface<IServicesExtension<TARGET>>())
      {
        ((IServicesExtension<TARGET>)@object).Attach((TARGET)this);
      }
    }

    /// <summary>
    /// Replace a certain object of type T with a new one of the same type
    /// </summary>
    /// <typeparam name="T">The type of the object to be added</typeparam>
    /// <param name="object">The instance</param>
    /// <returns>This instance</returns>
    public virtual TARGET Replace<T>(T @object)
    {
      if (@object != null)
      {
        ForceAdd(@object);
      }
      return (TARGET)this;
    }

    /// <summary>
    /// Remove an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to be removed</typeparam>
    public virtual void Remove<T>()
    {
      if (!_attachedObjects.ContainsKey(typeof(T))) return;
      _attachedObjects.Remove(typeof(T));
    }

    /// <summary>
    /// Get a clone of this context. Its initial state will be the same as the cloned context,
    /// however you can modify the copy in any way you want.
    /// </summary>
    /// <returns>A new instance in the same state as the original context</returns>
    public virtual TARGET CloneContext()
    {
      var obj = (TARGET) MemberwiseClone();
      obj._attachedObjects = new Dictionary<Type, IAttachedObject>(_attachedObjects);
      return obj;
    }


    /// <inheritdoc />
    public string WhatDoIHave => 
      string.Join("\n", _attachedObjects.Select(
        entry => $"Access Type: {entry.Key.FullName}, Stored Object: {entry.Value.TheObject}"));

    ///<summary>
    /// If an added object is an extension, Unattach will be called, if the object implements Dispose,
    /// Dispose will be called, if both then both.
    ///</summary>
    public void Dispose()
    {
      var disposedObjects = new HashSet<object>();
      foreach (IAttachedObject o in _attachedObjects.Values)
      {
        var extension = o.TheObject as IServicesExtension<TARGET>;
        if (extension != null)
          extension.Remove((TARGET)this);
        var disp = o.TheObject as IDisposable;
        if (disp == null || !o.CanBeDisposed || disposedObjects.Contains(disp)) continue;
        disp.Dispose();
        disposedObjects.Add(disp);
      }
      _attachedObjects.Clear();
    }

    ///<summary>
    /// IAttachedObject interface.
    ///</summary>
    private interface IAttachedObject
    {
      ///<summary>
      /// Can be disposed.
      ///</summary>
      bool CanBeDisposed { get; }

      ///<summary>
      /// The object.
      ///</summary>
      object TheObject { get; }
    }

    ///<summary>
    /// Attached object.
    ///</summary>
    ///<typeparam name="T"></typeparam>
    private class AttachedObject<T> : IAttachedObject
    {
      ///<summary>
      /// The object.
      ///</summary>
      private readonly T theObject;

      public AttachedObject(bool canBeDisposed, T theObject)
      {
          CanBeDisposed = canBeDisposed;
          this.theObject = theObject;
      }

        ///<summary>
      /// Can be disposed.
      ///</summary>
      public bool CanBeDisposed { get; private set; }

      object IAttachedObject.TheObject
      {
        get { return theObject; }
      }

      ///<summary>
      /// The object.
      ///</summary>
      public static implicit operator T(AttachedObject<T> attachedObject)
      {
        return attachedObject.theObject;
      }

      ///<summary>
      /// Attached object.
      ///</summary>
      public static implicit operator AttachedObject<T>(T theObject)
      {
        return new AttachedObject<T>(true,theObject);
      }
    }

      public IEnumerator<object> GetEnumerator()
      {
          return _attachedObjects.Values.Select(a => a.TheObject).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
          return GetEnumerator();
      }
  }
}