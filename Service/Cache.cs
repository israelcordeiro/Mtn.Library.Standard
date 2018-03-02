using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mtn.Library.Extensions;
using Mtn.Library.Utils;

namespace Mtn.Library.Service
{

    /// <summary>
    /// <para>Class cache manager.</para>
    /// </summary>
    public partial class Cache
    {
        private static Cache _instance;
        /// <summary>    
        /// <para>Cache Service.Should be used for caching functionality for Methods used by AjaxMethod or ServiceProxy.</para>
        /// </summary>
        public static Cache Instance
        {
            get { return _instance ?? (_instance = new Cache()); }
        } 
        internal static void ResetCache() 
        {
            try
            {
                _instance = null;
            }
            catch
            {
                // ignored
            }
        }

        #region Attributes
        private static readonly Dictionary<string, ContainerTemplate> MCacheContainer = new Dictionary<string, ContainerTemplate>(StringComparer.InvariantCultureIgnoreCase);
        #endregion

        #region Add
        /// <summary>
        /// <para>Adds item to cache.</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        /// <param name="value">
        /// <para>Represents the object to be put in cache.</para>
        /// </param>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public void Add(Enum containerType, object value, params string[] keys)
        {   
            this.Add(containerType,value, TimeSpan.Zero, keys);
        }

        /// <summary>
        /// <para>Adds item to cache.</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        /// <param name="value">
        /// <para>Represents the object to be put in cache.</para>
        /// </param>
        /// <param name="SlideExpiration">
        /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
        /// </param>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public void Add(Enum containerType, object value, TimeSpan SlideExpiration, params string[] keys)
        {
            this.Add(containerType, value, TimeSpan.Zero, TimeSpan.Zero, keys);
        }
        /// <summary>
        /// <para>Adds item to cache.</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
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
        public void Add(Enum containerType, object value, TimeSpan SlideExpiration, TimeSpan absoluteExpirationTime, params string[] keys)
        {   
            ContainerTemplate container = this.GetContainer(containerType);

            if(container.UseContainerTime)
                container.Add(value, container.SlideExpiration, container.AbsoluteExpirationTime, keys);
            else
                container.Add(value, SlideExpiration, absoluteExpirationTime, keys);
        }
        #endregion

        #region Remove
        /// <summary>
        /// <para>Remove item from cache.</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public void Remove(Enum containerType, params string[] keys)
        {
            
            ContainerTemplate container = this.GetContainer(containerType);

            if(container != null)
                container.Remove(keys);
        }
        #endregion

        #region ContainsKey
        /// <summary>
        /// <para>Check if cache has item. </para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        /// <returns>
        /// <para>true if contains at least one item in cache for this key.</para>
        /// </returns>
        public bool ContainsKey(Enum containerType, params string[] keys)
        {
            ContainerTemplate container = this.GetContainer(containerType);

            if(container != null)
                return container.ContainsKey(keys);
            else
                return false;
        }
        #endregion

        #region CheckExpired
        /// <summary>
        /// <para>Check all items expired and remove from cache. </para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        public void CheckExpired(Enum containerType)
        {
            ContainerTemplate container = this.GetContainer(containerType);

            if(container != null)
                container.CheckExpired();
        }
        /// <summary>
        /// <para>Check all items expired and remove from cache. </para>
        /// </summary>
        public void CheckAllExpired()
        {
            foreach(var container in MCacheContainer)
                container.Value.CheckExpired();
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Cache()
        {
            Entities.Task task = new Entities.Task();
            task.Job = CheckAllExpired;
            task.UseLimit = false;
            task.WorkInterval = TimeSpan.FromSeconds(Parameter.GetCacheTime);
            task.Hierarchy = 0;
            Scheduler.AddTask(task);

        }
        
        #endregion

        #region Clear 
        /// <summary>
        /// <para>Clear cache container</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        public void Clear(Enum containerType)
        {
             ContainerTemplate container = this.GetContainer(containerType);

            if(container != null)
                container.Clear();
        }
        #endregion

        #region Count
        /// <summary>
        /// <para>Returns total items in cache container.</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        /// <returns>
        /// <para>Returns total items in cache container.</para>
        /// </returns>
        public int Count(Enum containerType)
        {
            ContainerTemplate container = this.GetContainer(containerType);

            if(container != null)
                return container.Count;
            else
                return 0;
        }
        
        #endregion

        #region GetContainer 
        /// <summary>
        /// <para>Returns the cache container</para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        private ContainerTemplate GetContainer(Enum containerType)
        {
            ContainerTemplate container;
            string containerName = containerType.ToString();
            if(MCacheContainer.TryGetValue(containerName, out container))
                return container; 
            var containerTypeAttr = containerType.GetType()
                                       .GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public)
                                       .Where(fldAt => fldAt.Name.Equals(containerName))
                                       .Select(fld => fld.GetCustomAttributes(typeof(Attributes.CacheContainer), true)
                                                                           .FirstOrDefault())
                                           .Select(ct => (ct as Attributes.CacheContainer))
                                           .FirstOrDefault();
            lock(MCacheContainer)
            {
                Enums.CacheType ? type = null;
                string storage="";
                bool audit = false;
                bool keepCache = false;
               
                if(containerTypeAttr != null)
                {
                    type = containerTypeAttr.CacheType;
                    storage = containerTypeAttr.StoragePath;
                    audit = containerTypeAttr.Audit;
                    keepCache = containerTypeAttr.KeepCache;
                }
                else
                {
                    type = (Enums.CacheType) containerType;
                }
                
                switch(type.Value)
                {
                    case Enums.CacheType.Hash:
                        container = new HashContainer();
                        break;
                    case Enums.CacheType.SortedHash:
                        container = new SortedHashContainer();
                        break;
                    case Enums.CacheType.FileSystemMultipleFiles:
                        container = new FileContainer(storage, containerName, keepCache);
                        break;
                    //case CacheType.AppFabric:
                    //    container = new AppFabricContainer(containerName);
                    //    break;
                    default:
                        container = new HashContainer();
                        break;
                }

                if (containerTypeAttr != null)
                {
                    container.UseContainerTime = true;
                    container.SlideExpiration = TimeSpan.FromMinutes(containerTypeAttr.SlideExpiration);
                    container.AbsoluteExpirationTime = TimeSpan.FromMinutes(containerTypeAttr.AbsoluteExpirationTime);
                }

                try
                {
                    if (!MCacheContainer.ContainsKey(containerName))
                        MCacheContainer.Add(containerName, container);
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
            
            return container;

        }
        #endregion

        #region Iterator
        /// <summary>
        /// <para>Iterator </para>
        /// </summary>
        /// <param name="containerType">
        /// <para>Container type. Recommend using a CacheContainer.</para>
        /// </param>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public object this[Enum containerType, params string[] keys]
        {
            get
            {
                if(keys == null || keys.Length == 0)
                    return null;

                ContainerTemplate container = this.GetContainer(containerType);
                
                if(container == null)
                    return null;

                container.CheckExpired();

                object ct = container[keys];

                return ct;
            }
            set
            {
                this.Add(containerType, value, keys);
            }
        }
        #endregion
    }

    

    #region CacheItem
    
    /// <summary>
    /// <para>Class that represents a cache item</para>
    /// </summary>
    [Serializable]
    public class CacheItem
    {
        #region Atributtes
        private TimeSpan _mSlideExpiration = TimeSpan.Zero;
        private TimeSpan _mAbsoluteExpirationTime = TimeSpan.Zero;
        private DateTime _mLastAccessTime = DateTime.Now;
        private readonly DateTime _mCreatedTime = DateTime.Now;
        private object _mValue;
        private string _mKey;
        #endregion

        #region Properties
        /// <summary>
        /// <para>Created Datetime of item </para>
        /// </summary>
        public DateTime CreatedTime
        {
            get { return _mCreatedTime; }
        }
        /// <summary>
        /// <para>Value of item. </para>
        /// </summary>
        public object Value
        {
            get { _mLastAccessTime = DateTime.Now; return _mValue; }
            set { _mValue = value; }
        }

        /// <summary>
        /// <para>Cache key of item. </para>
        /// </summary>
        public string Key
        {
            get { return _mKey; }
            set { _mKey = value; }
        }

        /// <summary>
        /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
        /// </summary>
        public TimeSpan SlideExpiration
        {
            get { return _mSlideExpiration; }
            set { _mSlideExpiration = value; }
        }
        /// <summary>
        /// <para>Last Access Datetime .</para>
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return _mLastAccessTime; }
            set { _mLastAccessTime = value; }
        }
        /// <summary>
        /// <para>Indicates how much time remains in the cache being removed after this period. It will not be used if the parameter SlideExpiration is greater than zero .</para>
        /// </summary>
        public TimeSpan AbsoluteExpirationTime
        {
            get { return _mAbsoluteExpirationTime; }
            set { _mAbsoluteExpirationTime = value; }
        }

        /// <summary>
        /// <para>Indicates if cache item is expired..</para>
        /// </summary>
        public bool Expired
        {
            get
            {
                bool ret = false;
                if(this.SlideExpiration != TimeSpan.Zero)
                    ret = (DateTime.Now - this.LastAccessTime) > this.SlideExpiration;
                else if(this.AbsoluteExpirationTime != TimeSpan.Zero)
                    ret = (DateTime.Now - this.CreatedTime) > this.AbsoluteExpirationTime;
                return ret;
            }
        }
        #endregion

        #region Contructors
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="key">
        /// <para>Cache key of item. </para>
        /// </param>
        /// <param name="value">
        /// <para>Represents the object to be put in cache.</para>
        /// </param>
        /// <param name="SlideExpiration">
        /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
        /// </param>
        public CacheItem( string key,object value, TimeSpan SlideExpiration)
        {   
            this._mSlideExpiration = SlideExpiration;            
            this._mValue = value;
            this._mKey = key;
        }
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="key">
        /// <para>Cache key of item. </para>
        /// </param>
        /// <param name="value">
        /// <para>Represents the object to be put in cache.</para>
        /// </param>
        /// <param name="SlideExpiration">
        /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
        /// </param>
        /// <param name="absoluteExpirationTime">
        /// <para>indicates how much time remains in the cache being removed after this period. It will not be used if the parameter SlideExpiration is greater than zero .</para>
        /// </param>
        public CacheItem(string key, object value,TimeSpan SlideExpiration, TimeSpan absoluteExpirationTime)
        {
            this._mSlideExpiration = SlideExpiration;
            this._mAbsoluteExpirationTime = absoluteExpirationTime;            
            this._mValue = value;
            this._mKey = key;
        }

        #endregion
    }
    #endregion

    #region ContainerTemplate
    /// <summary>
    /// <para>Cache Container Template.</para>
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    internal abstract class ContainerTemplate
    {
        /// <summary>
        /// <para>Adds item to cache.</para>
        /// </summary>
        /// <param name="value">
        /// <para>Represents the object to be put in cache.</para>
        /// </param>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public abstract void Add(object value, params string[] keys);
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
        public abstract void Add(object value, TimeSpan SlideExpiration,  params string[] keys);
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
        public abstract void Add(object value, TimeSpan SlideExpiration, TimeSpan absoluteExpirationTime, params string[] keys);
        /// <summary>
        /// <para>Remove item from cache.</para>
        /// </summary>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public abstract void Remove(params string[] keys);
        /// <summary>
        /// <para>Check all items expired and remove from cache. </para>
        /// </summary>
        public abstract void CheckExpired();
        /// <summary>
        /// <para>Check if cache has item. </para>
        /// </summary>
        /// <param name="keys">
        /// <para>Represents the cache key.</para>
        /// </param>
        public abstract bool ContainsKey(params string[] keys);
        /// <summary>
        /// <para>Clear cache container</para>
        /// </summary>
        public abstract void Clear();
        /// <summary>
        /// <para>Clear cache with the prefix</para>
        /// <param name="prefix">
        /// <para>Prefix of cache key</para>
        /// </param>
        /// </summary>
        public abstract void Clear(string prefix);
        /// <summary>
        /// <para>Iterator, return cache item by key</para>
        /// <param name="keys">
        /// <para>Cache key.</para>
        /// </param>
        /// </summary>
        public abstract object this[params string[] keys] { get; }
        /// <summary>
        /// <para>Return total items in cache container</para>
        /// </summary>
        public abstract int Count { get; }
        #region Atributtes
        private TimeSpan _mSlideExpiration = TimeSpan.Zero;
        private TimeSpan _mAbsoluteExpirationTime = TimeSpan.Zero;
        private DateTime _mLastAccessTime = DateTime.Now;
        private readonly DateTime _mCreatedTime = DateTime.Now;        
        #endregion

        #region Properties
        /// <summary>
        /// <para>Created Datetime </para>
        /// </summary>
        public DateTime CreatedTime
        {
            get { return _mCreatedTime; }
        }
        /// <summary>
        /// <para>Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long without calling the item should be removed from the cache.</para>
        /// </summary>
        public TimeSpan SlideExpiration
        {
            get { return _mSlideExpiration; }
            set { _mSlideExpiration = value; }
        }
        /// <summary>
        /// <para> Indicates the last access.</para>
        /// </summary>
        public DateTime LastAccessTime
        {
            get { return _mLastAccessTime; }
            set { _mLastAccessTime = value; }
        }
        /// <summary>
        /// <para>Indicates how much time remains in the cache being removed after this period. It will not be used if the parameter SlideExpiration is greater than zero .</para>
        /// </summary>
        public TimeSpan AbsoluteExpirationTime
        {
            get { return _mAbsoluteExpirationTime; }
            set { _mAbsoluteExpirationTime = value; }
        }
        public bool UseContainerTime { get; set; }

        #endregion
        /// <summary>
        /// <para>Returns the cache key using by string array.</para>
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>
        /// <para>Returns the cache key using by string array.</para>
        /// </returns>
        protected static string GetByKey(string[] keys)
        {
            if(keys == null || keys.Length == 0)
                throw new ArgumentNullException("keys cannot be null");

            if(keys.Length == 1)
                return keys[0];

            string key = "";
            for(int i = 0; i < keys.Length; i++)
                key += keys[i].ToLower();
                
            return key.Length == 0?null: key;
        }

    }
    #endregion

    
}
