//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.ApplicationServer.Caching;


//namespace Mtn.Library.Service
//{
//    public partial class Cache
//    {
//        #region AppFabricContainer
//        /// <summary>
//        /// <para>Hash Container.</para>
//        /// </summary>
//        internal class AppFabricContainer : ContainerTemplate
//        {
//            private readonly DataCacheFactory _mCacheFactory = new DataCacheFactory();
//            private DataCache _mCacheContainer;
//            private string _mRegion;
//            public AppFabricContainer(string cacheName)
//            {
//                _mCacheContainer = _mCacheFactory.GetCache(cacheName);
//                _mRegion = cacheName + "Region";
//            }

            
//            #region Add
//            /// <summary>
//            /// <para>Adds item to cache.</para>
//            /// </summary>
//            /// <param name="value">
//            /// <para>Represents the object to be put in cache.</para>
//            /// </param>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            public override void Add(object value, params string[] keys)
//            {
//                lock (_mCacheContainer)
//                {
//                    string key = GetByKey(keys);
                    
//                    CacheItem item = new CacheItem(key, value, TimeSpan.Zero);
//                    _mCacheContainer.Put(key, item, _mRegion);
//                }
//            }
//            /// <summary>
//            /// <para>Adds item to cache.</para>
//            /// </summary>
//            /// <param name="value">
//            /// <para>Represents the object to be put in cache.</para>
//            /// </param>
//            /// <param name="SlideExpiration">
//            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
//            /// </param>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            public override void Add(object value, TimeSpan SlideExpiration, params string[] keys)
//            {
//                lock (_mCacheContainer)
//                {
//                    string key = GetByKey(keys);
//                    CacheItem item = new CacheItem(key, value, SlideExpiration);
//                    _mCacheContainer.Put(key, item, _mRegion);
//                }
//            }

//            /// <summary>
//            /// <para>Adds item to cache.</para>
//            /// </summary>
//            /// <param name="value">
//            /// <para>Represents the object to be put in cache.</para>
//            /// </param>
//            /// <param name="SlideExpiration">
//            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
//            /// </param>
//            /// <param name="absoluteExpirationTime">
//            /// <para>indicates how much time remains in the cache being removed after this period. It will not be used if the parameter SlideExpiration is greater than zero .</para>
//            /// </param>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            public override void Add(object value, TimeSpan SlideExpiration, TimeSpan absoluteExpirationTime, params string[] keys)
//            {
//                lock (_mCacheContainer)
//                {
//                    string key = GetByKey(keys);
//                    CacheItem item = new CacheItem(key, value, SlideExpiration, absoluteExpirationTime);
//                    _mCacheContainer.Put(key, item, _mRegion);
//                }
//            }
//            #endregion

//            #region Remove
//            /// <summary>
//            /// <para>Remove item from cache.</para>
//            /// </summary>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            public override void Remove(params string[] keys)
//            {
//                lock (_mCacheContainer)
//                {
//                    _mCacheContainer.Remove(GetByKey(keys));
//                }
//            }
//            #endregion

//            #region CheckExpired
//            /// <summary>
//            /// <para>Check all items expired and remove from cache. </para>
//            /// </summary>
//            public override void CheckExpired()
//            {
//                lock (_mCacheContainer)
//                {

//                    var keyExpired = (from cache in _mCacheContainer.GetObjectsInRegion(_mRegion)
//                                      where (cache.Value as CacheItem).Expired == true
//                                      select cache.Value).ToArray();

//                    foreach (var keyToRemove in keyExpired)
//                        _mCacheContainer.Remove((keyToRemove as CacheItem).Key);
//                }
//            }
//            #endregion

//            #region ContainsKey
//            /// <summary>
//            /// <para>Check if cache has item. </para>
//            /// </summary>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            /// <returns>
//            /// <para>true if contains at least one item in cache for this key.</para>
//            /// </returns>
//            public override bool ContainsKey(params string[] keys)
//            {
//                return (_mCacheContainer.Get(GetByKey(keys),_mRegion) != null);
//            }
//            #endregion

//            #region Clear
//            /// <summary>
//            /// <para>Clear cache container</para>
//            /// </summary>
//            public override void Clear()
//            {
//                lock (_mCacheContainer)
//                {
//                    _mCacheContainer.RemoveRegion(_mRegion);
//                }
//            }
//            /// <summary>
//            /// <para>Clear cache with the prefix</para>
//            /// <param name="prefix">
//            /// <para>Prefix of cache key</para>
//            /// </param>
//            /// </summary>
//            public override void Clear(string prefix)
//            {
//                lock (_mCacheContainer)
//                {
//                    var keysList = this._mCacheContainer.GetObjectsInRegion(_mRegion).Where(k => k.Key.StartsWith(prefix));
//                    foreach (var keyItem in keysList)
//                        _mCacheContainer.Remove(keyItem.Key);
//                }
//            }
//            #endregion

//            #region Iterator
//            /// <summary>
//            /// <para>Iterator, return cache item by key</para>
//            /// <param name="keys">
//            /// <para>Cache key.</para>
//            /// </param>
//            /// </summary>
//            public override object this[params string[] keys]
//            {
//                get
//                {
//                    CacheItem itemCache;
//                    itemCache = (CacheItem) _mCacheContainer.Get(GetByKey(keys),_mRegion);
//                    if (itemCache == null)
//                        return null;
//                    return itemCache.Value;
//                }
//            }
//            #endregion

//            #region Count
//            /// <summary>
//            /// <para>Returns total items in cache container</para>
//            /// </summary>
//            public override int Count
//            {
//                get
//                {
//                    return _mCacheContainer.GetObjectsInRegion(_mRegion).Count();
//                }
//            }
//            #endregion

//        }
//        #endregion
//    }
//}
