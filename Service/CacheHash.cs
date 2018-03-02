using System;
using System.Collections.Generic;
using System.Linq;


namespace Mtn.Library.Service
{
    public partial class Cache
    {
        #region HashContainer
        /// <summary>
        /// <para>Hash Container.</para>
        /// </summary>
        internal class HashContainer : ContainerTemplate
        {
            private readonly Dictionary<string, CacheItem> _mCacheContainer = new Dictionary<string, CacheItem>(StringComparer.InvariantCultureIgnoreCase);

            #region Add
            /// <summary>
            /// <para>Adds item to cache.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Represents the object to be put in cache.</para>
            /// </param>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Add(object value, params string[] keys)
            {
                lock (_mCacheContainer)
                {
                    string key = GetByKey(keys);
                    if (_mCacheContainer.ContainsKey(key))
                        _mCacheContainer.Remove(key);

                    CacheItem item = new CacheItem(key, value, TimeSpan.Zero);
                    _mCacheContainer.Add(key, item);
                }
            }
            /// <summary>
            /// <para>Adds item to cache.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Represents the object to be put in cache.</para>
            /// </param>
            /// <param name="SlideExpiration">
            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
            /// </param>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Add(object value, TimeSpan SlideExpiration, params string[] keys)
            {
                lock (_mCacheContainer)
                {
                    string key = GetByKey(keys);
                    if (_mCacheContainer.ContainsKey(key))
                        _mCacheContainer.Remove(key);

                    CacheItem item = new CacheItem(key, value, SlideExpiration);
                    _mCacheContainer.Add(key, item);
                }
            }

            /// <summary>
            /// <para>Adds item to cache.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Represents the object to be put in cache.</para>
            /// </param>
            /// <param name="SlideExpiration">
            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
            /// </param>
            /// <param name="absoluteExpirationTime">
            /// <para>indicates how much time remains in the cache being removed after this period. It will not be used if the parameter SlideExpiration is greater than zero .</para>
            /// </param>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Add(object value, TimeSpan SlideExpiration, TimeSpan absoluteExpirationTime, params string[] keys)
            {
                lock (_mCacheContainer)
                {
                    string key = GetByKey(keys);
                    if (_mCacheContainer.ContainsKey(key))
                        _mCacheContainer.Remove(key);

                    CacheItem item = new CacheItem(key, value, SlideExpiration, absoluteExpirationTime);
                    _mCacheContainer.Add(key, item);
                }
            }
            #endregion

            #region Remove
            /// <summary>
            /// <para>Remove item from cache.</para>
            /// </summary>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            public override void Remove(params string[] keys)
            {
                lock (_mCacheContainer)
                {
                    _mCacheContainer.Remove(GetByKey(keys));
                }
            }
            #endregion

            #region CheckExpired
            /// <summary>
            /// <para>Check all items expired and remove from cache. </para>
            /// </summary>
            public override void CheckExpired()
            {
                lock (_mCacheContainer)
                {

                    var keyExpired = (from cache in _mCacheContainer
                                      where cache.Value.Expired == true
                                      select cache).ToArray();

                    foreach (var keyToRemove in keyExpired)
                        _mCacheContainer.Remove(keyToRemove.Key);
                }
            }
            #endregion

            #region ContainsKey
            /// <summary>
            /// <para>Check if cache has item. </para>
            /// </summary>
            /// <param name="keys">
            /// <para>Represents the cache key.</para>
            /// </param>
            /// <returns>
            /// <para>true if contains at least one item in cache for this key.</para>
            /// </returns>
            public override bool ContainsKey(params string[] keys)
            {
                return _mCacheContainer.ContainsKey(GetByKey(keys));
            }
            #endregion

            #region Clear
            /// <summary>
            /// <para>Clear cache container</para>
            /// </summary>
            public override void Clear()
            {
                lock (_mCacheContainer)
                {
                    _mCacheContainer.Clear();
                }
            }
            /// <summary>
            /// <para>Clear cache with the prefix</para>
            /// <param name="prefix">
            /// <para>Prefix of cache key</para>
            /// </param>
            /// </summary>
            public override void Clear(string prefix)
            {
                lock (_mCacheContainer)
                {
                    var keysList = this._mCacheContainer.Where(k => k.Key.StartsWith(prefix));
                    foreach (var keyItem in keysList)
                        _mCacheContainer.Remove(keyItem.Key);
                }
            }
            #endregion

            #region Iterator
            /// <summary>
            /// <para>Iterator, return cache item by key</para>
            /// <param name="keys">
            /// <para>Cache key.</para>
            /// </param>
            /// </summary>
            public override object this[params string[] keys]
            {
                get
                {
                    CacheItem itemCache;
                    if (!_mCacheContainer.TryGetValue(GetByKey(keys), out itemCache))
                        return null;

                    return itemCache.Value;
                }
            }
            #endregion

            #region Count
            /// <summary>
            /// <para>Returns total items in cache container</para>
            /// </summary>
            public override int Count
            {
                get
                {
                    return _mCacheContainer.Count;
                }
            }
            #endregion

        }
        #endregion
    }
}
