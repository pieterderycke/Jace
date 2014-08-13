using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !WINDOWS_PHONE_8 && !WINDOWS_PHONE_7
using System.Collections.Concurrent;
using System.Threading;
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
        private const int DefaultMaximumSize = 500;
        private const int DefaultReductionSize = 50;

        private readonly int maximumSize;
        private readonly int reductionSize;

        private long counter; // We cannot use DateTime.Now, because the precission is not high enough.

#if WINDOWS_PHONE_8 || WINDOWS_PHONE_7
        private readonly Dictionary<TKey, CacheItem> dictionary;
        private readonly Object counterLock = new Object();
#else
        private readonly ConcurrentDictionary<TKey, CacheItem> dictionary;
#endif

        /// <summary>
        /// Create a new instance of the <see cref="MemoryCache"/>.
        /// </summary>
        public MemoryCache()
            : this(DefaultMaximumSize, DefaultReductionSize)
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="maximumSize">The maximum allowed number of items in the cache.</param>
        /// <param name="reductionSize">The number of items to be deleted per cleanup of the cache.</param>
        public MemoryCache(int maximumSize, int reductionSize)
        {
            if (maximumSize < 1)
                throw new ArgumentOutOfRangeException("maximumSize",
                    "The maximum allowed number of items in the cache must be at least one.");

            if (reductionSize < 1)
                throw new ArgumentOutOfRangeException("reductionSize",
                    "The cache reduction size must be at least one.");

            this.maximumSize = maximumSize;
            this.reductionSize = reductionSize;

#if WINDOWS_PHONE_8 || WINDOWS_PHONE_7
            this.dictionary = new Dictionary<TKey, CacheItem>();
#else
            this.dictionary = new ConcurrentDictionary<TKey, CacheItem>();
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
                CacheItem cacheItem = dictionary[key];
                cacheItem.Accessed();
                return cacheItem.Value;
            }
        }

        /// <summary>
        /// Gets the number of items in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                return dictionary.Count;
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
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

#if WINDOWS_PHONE_8 || WINDOWS_PHONE_7
            lock (dictionary)
            {
                if (dictionary.ContainsKey(key))
                {
                    CacheItem cacheItem = dictionary[key];
                    return cacheItem.Value;
                }
                else
                {
                    EnsureCacheStorageAvailable();

                    TValue value = valueFactory(key);
                    dictionary.Add(key, new CacheItem(this, value));
                    return value;
                }
            }
#else
            CacheItem cacheItem = dictionary.GetOrAdd(key, k => 
                {
                    EnsureCacheStorageAvailable();

                    TValue value = valueFactory(k);
                    return new CacheItem(this, valueFactory(k));
                });
            return cacheItem.Value;
#endif
        }

        /// <summary>
        /// Ensure that the cache has room for an additional item.
        /// If there is not enough room anymore, force a removal of oldest
        /// accessed items in the cache.
        /// </summary>
        private void EnsureCacheStorageAvailable()
        {
            if (dictionary.Count >= maximumSize) // >= because we want to add an item after this method
            {
                IList<TKey> keysToDelete = (from p in dictionary.ToArray()
                                            where p.Key != null && p.Value != null
                                            orderby p.Value.LastAccessed ascending
                                            select p.Key).Take(reductionSize).ToList();

                foreach (TKey key in keysToDelete)
                {
#if WINDOWS_PHONE_8 || WINDOWS_PHONE_7
                    dictionary.Remove(key);
#else
                    CacheItem cacheItem;
                    dictionary.TryRemove(key, out cacheItem);
#endif
                }
            }
        }

        private class CacheItem
        {
            private MemoryCache<TKey, TValue> cache;

            public CacheItem(MemoryCache<TKey, TValue> cache, TValue value)
            {
                this.cache = cache;
                this.Value = value;

                Accessed();
            }

            public TValue Value { get; private set; }

            public long LastAccessed { get; private set; }

            public void Accessed()
            {
#if WINDOWS_PHONE_8 || WINDOWS_PHONE_7
                lock(cache.counterLock)
                {
                    this.LastAccessed = cache.counter++;
                }
#else
                this.LastAccessed = Interlocked.Increment(ref cache.counter);
#endif
            }
        }
    }
}
