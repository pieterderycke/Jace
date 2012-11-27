using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !WINDOWS_PHONE
using System.Collections.Concurrent;
#endif

namespace Jace.Util
{
    /// <summary>
    /// An in-memory based cache to store objects. The implementation is thread safe and supports
    /// the multiple platforms supported by Jace (.NET, WinRT, WP7 and WP8).
    /// </summary>
    /// <typeparam name="TKey">The type of the keys.</typeparam>
    /// <typeparam name="TValue">The type of the values.</typeparam>
    public class MemoryCache<TKey, TValue>
    {
#if WINDOWS_PHONE
        private readonly Dictionary<TKey, TValue> dictionary;
#else
        private readonly ConcurrentDictionary<TKey, TValue> dictionary;
#endif

        /// <summary>
        /// Create a new instance of the <see cref="MemoryCache"/>.
        /// </summary>
        public MemoryCache()
        {
#if WINDOWS_PHONE
            this.dictionary = new Dictionary<TKey, TValue>();
#else
            this.dictionary = new ConcurrentDictionary<TKey, TValue>();
#endif
        }

        /// <summary>
        /// Get the value in the cache for the given key.
        /// </summary>
        /// <param name="key">The key to lookup in the cache.</param>
        /// <returns>The value for the given key.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
        }

        /// <summary>
        /// Returns true if an item with the given key is present in the cache.
        /// </summary>
        /// <param name="key">The key to lookup in the cache.</param>
        /// <returns>True if an item is present in the cache for the given key.</returns>
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// If for a given key an item is present in the cache, this method will return
        /// the value for the given key. If no item is present in the cache for the given
        /// key, the valueFactory is executed to produce the value. This value is stored in
        /// the cache and returned to the caller.
        /// </summary>
        /// <param name="key">The key to lookup in the cache.</param>
        /// <param name="valueFactory">The factory to produce the value matching with the key.</param>
        /// <returns>The value for the given key.</returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
#if WINDOWS_PHONE
            lock (dictionary)
            {
                if (dictionary.ContainsKey(key))
                {
                    return dictionary[key];
                }
                else
                {
                    TValue value = valueFactory(key);
                    dictionary.Add(key, value);
                    return value;
                }
            }
#else
            return dictionary.GetOrAdd(key, valueFactory);
#endif
        }
    }
}
