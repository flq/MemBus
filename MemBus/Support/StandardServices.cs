using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Support
{
    /// <inheritdoc />
    /// <summary>
    /// Use this context class if a standard, untyped context is completely sufficient for you
    /// </summary>
    public class StandardServices : IServices, IDisposable, IEnumerable<object>
    {
        /// <summary>
        /// The list of objects attached to this context. These may be context extensions
        /// or other objects
        /// </summary>
        private readonly Dictionary<Type, IAttachedObject> _attachedObjects =
            new Dictionary<Type, IAttachedObject>();

        /// <summary>
        /// Get the Object from the context of type T
        /// </summary>
        /// <typeparam name="T">the type of the object to retrieve</typeparam>
        /// <returns>The instance of said type or teh default value of T</returns>
        public T Get<T>()
        {
            return _attachedObjects.TryGetValue(typeof(T), out var obj) ? (T) obj.TheObject : default;
        }

        /// <summary>
        /// Add any object of type T
        /// </summary>
        /// <typeparam name="T">The type of teh object to be added</typeparam>
        /// <param name="object">The instance to be added</param>
        public void Add<T>(T @object)
        {
            Add(@object, true);
        }

        ///<summary>
        /// Add.
        ///</summary>
        public void Add<T>(T @object, bool disposable)
        {
            if (_attachedObjects.ContainsKey(typeof(T)))
                throw new ArgumentException($"An object of Type {typeof(T).Name} is already attached to this context");
            _attachedObjects.Add(typeof(T), new AttachedObject<T>(disposable, @object));
        }

        /// <summary>
        /// Replace a certain object of type T with a new one of the same type
        /// </summary>
        /// <typeparam name="T">The type of the object to be added</typeparam>
        /// <param name="object">The instance</param>
        /// <returns>This instance</returns>
        public StandardServices Replace<T>(T @object)
        {
            if (@object != null)
            {
                ForceAdd(@object);
            }

            return this;
        }

        /// <summary>
        /// Remove an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to be removed</typeparam>
        public void Remove<T>()
        {
            if (!_attachedObjects.ContainsKey(typeof(T)))
                return;
            _attachedObjects.Remove(typeof(T));
        }


        /// <inheritdoc />
        public string WhatDoIHave =>
            string.Join("\n", _attachedObjects.Select(
                entry => $"Access Type: {entry.Key.FullName}, Stored Object: {entry.Value.TheObject}"));

        ///<summary>
        /// Force add.
        ///</summary>
        ///<param name="object"></param>
        ///<typeparam name="T"></typeparam>
        private void ForceAdd<T>(T @object)
        {
            _attachedObjects[typeof(T)] = (AttachedObject<T>) @object;
        }

        ///<summary>
        /// If an added object is an extension, Unattach will be called, if the object implements Dispose,
        /// Dispose will be called, if both then both.
        ///</summary>
        public void Dispose()
        {
            var disposedObjects = new HashSet<object>();
            foreach (IAttachedObject o in _attachedObjects.Values)
            {
                if (!(o.TheObject is IDisposable disp) || !o.CanBeDisposed || disposedObjects.Contains(disp))
                    continue;
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
                return new AttachedObject<T>(true, theObject);
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