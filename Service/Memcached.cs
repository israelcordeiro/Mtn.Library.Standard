//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using Enyim.Caching;
//using Enyim.Caching.Configuration;
//using Enyim.Caching.Memcached;


//namespace Mtn.Library.Service
//{
//    public partial class Cache
//    {
//        #region Memcached
//        /// <summary>
//        /// <para>Hash Container.</para>
//        /// </summary>
//        internal class MemcachedContainer : ContainerTemplate
//        {
//            private readonly MemcachedClientConfiguration _mCacheFactory = new MemcachedClientConfiguration();
//            private readonly MemcachedClient _mCacheContainer; 
//            public MemcachedContainer(string cacheName)
//            {
//                _mCacheContainer = Utils.Parameter.MemcachedClient;
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
//                    _mCacheContainer.Store(StoreMode.Set, key,value);
//                }
//            }
//            /// <summary>
//            /// <para>Adds item to cache.</para>
//            /// </summary>
//            /// <param name="value">
//            /// <para>Represents the object to be put in cache.</para>
//            /// </param>
//            /// <param name="slideExpiration">
//            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
//            /// </param>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            public override void Add(object value, TimeSpan slideExpiration, params string[] keys)
//            {
//                lock (_mCacheContainer)
//                {
//                    string key = GetByKey(keys);
//                    _mCacheContainer.Store(StoreMode.Set, key, value, slideExpiration);
//                }
//            }

//            /// <summary>
//            /// <para>Adds item to cache.</para>
//            /// </summary>
//            /// <param name="value">
//            /// <para>Represents the object to be put in cache.</para>
//            /// </param>
//            /// <param name="slideExpiration">
//            /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
//            /// </param>
//            /// <param name="absoluteExpirationTime">
//            /// <para>indicates how much time remains in the cache being removed after this period. It will not be used if the parameter slideExpiration is greater than zero .</para>
//            /// </param>
//            /// <param name="keys">
//            /// <para>Represents the cache key.</para>
//            /// </param>
//            public override void Add(object value, TimeSpan slideExpiration, TimeSpan absoluteExpirationTime, params string[] keys)
//            {
//                lock (_mCacheContainer)
//                {
//                    string key = GetByKey(keys);
                    
//                    DateTime dtAbsolute = DateTime.Now;
//                    if (slideExpiration == TimeSpan.Zero)
//                    {
//                        dtAbsolute.Add(absoluteExpirationTime);
//                        _mCacheContainer.Store(StoreMode.Set, key, value, absoluteExpirationTime);
//                    }
//                    else
//                    {
//                        _mCacheContainer.Store(StoreMode.Set, key, value, slideExpiration);
//                    }
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
//                // Todo: Realy need do anything ?
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
//                return (_mCacheContainer.Get(GetByKey(keys)) != null);
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
//                    _mCacheContainer.FlushAll();
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
//                throw new NotImplementedException();
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
//                    var itemCache =  _mCacheContainer.Get(GetByKey(keys));
//                    if (itemCache == null)
//                        return null;
//                    return itemCache;
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
//                    throw new NotImplementedException();
//                    return 0;
//                }
//            }
//            #endregion

//        }
//        #endregion
//    }
//}
