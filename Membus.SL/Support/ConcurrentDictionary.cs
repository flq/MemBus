using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace System.Collections.Concurrent
{

    /// <summary>
    /// A somewhat naive implementation for the Silverlight version of MemBus
    /// Most operations happen under a lock, so it is indeed far from the
    /// lck-free .NET 4 version, but it should be thread-safe as in not behaving
    /// too stupid if accessed from multiple threads. If used excessively in multi-thread
    /// scenarios it may be bad with regard to performance.
    /// </summary>
    public class ConcurrentDictionary<K,V>
    {
        private readonly Dictionary<K,V> _dict = new Dictionary<K,V>();
        private readonly object _l = new object();
        
        public V this[K key]
        {
            get { return _dict[key]; }
        }

        public V AddOrUpdate(K key, Func<K,V> addValueFactory, Func<K,V,V> updateValueFactory)
        {
            lock (_l)
            {
                V value;
                if (_dict.TryGetValue(key, out value))
                    _dict[key] = updateValueFactory(key, value);
                else
                    _dict[key] = addValueFactory(key);
            }
            return _dict[key];
        }

        public V GetOrAdd(K key, Func<K,V> valueFactory)
        {
            lock (_l)
            {
                V value;
                if (!_dict.TryGetValue(key, out value))
                    _dict[key] = valueFactory(key);
            }
            return _dict[key];
        }

        public bool TryRemove(K key, out V value)
        {
            lock (_l)
            {
                if (_dict.TryGetValue(key, out value))
                {
                    _dict.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public bool TryAdd(K key, V value)
        {
            lock (_l)
            {
                if (_dict.ContainsKey(key))
                    return false;
                _dict.Add(key, value);
            }
            return true;
        }

        public bool IsEmpty
        {
            get
            {
                var count = _dict.Count;
                return count == 0;
            }
        }

        public IEnumerable<V> Values
        {
            get
            {
                V[] vs = null;
                lock (_l)
                {
                    vs = _dict.Values.ToArray();
                }
                return vs.AsEnumerable();
            }
        }
    }
}
// ReSharper restore CheckNamespace