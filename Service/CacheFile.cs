using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif

namespace Mtn.Library.Service
{
    public partial class Cache
    {

        #region FileContainer
        /// <summary>
        /// <para>File Container.</para>
        /// </summary>
        internal class FileContainer : ContainerTemplate
        {
            private readonly Dictionary<string, CacheItem> _mCacheContainer = new Dictionary<string, CacheItem>(StringComparer.InvariantCultureIgnoreCase);
            private readonly string _mStoragePath = "";
            private readonly string _mFileNameBase = "";
            private readonly bool _mKeepCache;

            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="storagePath">
            /// <para>Path where the cache items will be saved.</para>
            /// </param>
            /// <param name="keybase">
            /// <para>Name to identify the container, normally the enum representing the container.</para>
            /// </param>
            /// <param name="keepCache">
            /// <para>Indicates whether to keep the cache even after reset the application.</para>
            /// </param>
            public FileContainer(string storagePath, string keybase, bool keepCache)
            {
                if(storagePath.IsNullOrEmptyMtn(true))
                    _mStoragePath = System.IO.Directory.GetCurrentDirectory() + "\\CacheMtn\\";
                else
                    _mStoragePath = storagePath ;

                if(_mStoragePath.LastIndexOf("\\", System.StringComparison.Ordinal) != (_mStoragePath.Length -1))
                    _mStoragePath+= "\\";

                _mFileNameBase = "Mtn_CACHE_FILE" + (keybase.IsNullOrEmptyMtn(true)?"":"_" + keybase);
                _mKeepCache = keepCache;
                if(!Directory.Exists(_mStoragePath))
                {
                    try
                    {
                        Directory.CreateDirectory(_mStoragePath);
                    }
                    catch (Exception ex)
                    {
                        Service.Statistics.Add(ex.GetAllMessagesMtn());
                    }
                }
                else if(_mKeepCache == true)
                {
                     
                     DirectoryInfo source = new DirectoryInfo(_mStoragePath);
                     var files = source.GetFiles().Where(fi => fi.Name.StartsWith(_mFileNameBase) && fi.Name.EndsWith(".cache")).OrderBy(fo => fo.CreationTime);
                        
                        foreach(var file in files)
                        {
                            try
                            {
                                var fileCache = file.OpenRead();
                                BinaryFormatter formatter = new BinaryFormatter();
                                var retVal = formatter.Deserialize(fileCache);
                                fileCache.Close();
                                var item = (retVal as CacheItem);
                                _mCacheContainer.Add(item.Key, new CacheItem(item.Key, file.FullName, item.SlideExpiration,item.AbsoluteExpirationTime));
                            }
                            catch
                            {
                            }
                        }
                }
                else
                {
                    try
                    {
                        string filenameToDelete = string.Format("{0}*.{1}", _mFileNameBase, "cache");
                        var filesToDelete = Directory.GetFiles(_mStoragePath, filenameToDelete, SearchOption.TopDirectoryOnly);
                        foreach(string fileToDelete in filesToDelete)
                            File.Delete(fileToDelete);
                    }
                    catch (Exception ex)
                    {
                        Service.Statistics.Add(ex.GetAllMessagesMtn());
                    }
                }
            }

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
                this.Add(value, TimeSpan.Zero, TimeSpan.Zero, keys);
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
                this.Add(value, SlideExpiration, TimeSpan.Zero, keys);
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
                lock(_mCacheContainer)
                {
                    string key = GetByKey(keys);
                    string filename = "";
                    if(_mCacheContainer.ContainsKey(key))
                    {
                        filename = (string) _mCacheContainer[key].Value;
                        _mCacheContainer.Remove(key);
                    }
                    else
                        filename = _mStoragePath + _mFileNameBase + "_" + Guid.NewGuid().ToString() + ".cache";

                    CacheItem item = new CacheItem(key, filename, SlideExpiration, absoluteExpirationTime);
                    
                    _mCacheContainer.Add(key, item);

                    var fileCache = File.Create(filename);
                    BinaryFormatter formatter = new BinaryFormatter();
                    if(_mKeepCache)
                        formatter.Serialize(fileCache, new CacheItem(key, value, SlideExpiration, absoluteExpirationTime));
                    else
                        formatter.Serialize(fileCache, value);
                    fileCache.Close();

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
                lock(_mCacheContainer)
                {
                    try
                    {
                        File.Delete((string) _mCacheContainer[GetByKey(keys)].Value);
                    }
                    catch (Exception ex)
                    {
                        Service.Statistics.Add(ex.GetAllMessagesMtn());
                    }
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
                lock(_mCacheContainer)
                {

                    var keyExpired = (from cache in _mCacheContainer
                                      where cache.Value.Expired == true
                                      select cache).ToArray();

                    lock (_mCacheContainer)
                    {
                        foreach (var keyToRemove in keyExpired)
                        {   
                            try
                            {
                                File.Delete((string)keyToRemove.Value.Value);
                            }
                            catch (Exception ex)
                            {
                                Service.Statistics.Add(ex.GetAllMessagesMtn());
                            }
                            _mCacheContainer.Remove(keyToRemove.Key);
                        }
                    }
                    
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
                lock(_mCacheContainer)
                {
                    _mCacheContainer.Clear();
                }
            }
            /// <summary>
            /// <para>Clear cache with the prefix.</para>
            /// <param name="prefix">
            /// <para>Clear cache container</para>
            /// </param>
            /// </summary>
            public override void Clear(string prefix)
            {
                lock(_mCacheContainer)
                {
                    var keysList = this._mCacheContainer.Where(k => k.Key.StartsWith(prefix));
                    foreach(var keyItem in keysList)
                        _mCacheContainer.Remove(keyItem.Key);

                    _mCacheContainer.Clear();
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
                    if(!_mCacheContainer.TryGetValue(GetByKey(keys), out itemCache))
                        return null;

                    try
                    {
                        var fileCache = File.OpenRead((string) itemCache.Value);
                        BinaryFormatter formatter = new BinaryFormatter();
                        var retVal = formatter.Deserialize(fileCache);
                        fileCache.Close();

                        if(_mKeepCache)
                        {
                            var item = (retVal as CacheItem);
                            this.Add(item.Value,itemCache.SlideExpiration,itemCache.AbsoluteExpirationTime, keys);
                            return item.Value;
                        }
                        else
                            return retVal;
                    }
                    catch
                    { 
                        return null; 
                    }
                }
            }
            #endregion

            #region Count
            /// <summary>
            /// <para>Return total items in cache container</para>
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
