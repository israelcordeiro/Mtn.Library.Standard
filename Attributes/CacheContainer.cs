using System;
using Mtn.Library.Enums;

namespace Mtn.Library.Attributes
{
    /// <summary>
    ///     <para>Represents a type of container to the cache with default parameters set by the developer.</para>
    /// </summary>
    public class CacheContainer : Attribute
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="cacheType">
        ///     <para>Represents the type of storage caching, whether it is in memory, disk, etc..</para>
        /// </param>
        public CacheContainer(CacheType cacheType)
        {
            CacheType = cacheType;
        }

        /// <summary>
        ///     <para>Represents the type of storage caching, whether it is in memory, disk, etc..</para>
        /// </summary>
        public CacheType CacheType { get; set; }

        /// <summary>
        ///     <para>
        ///         Indicates how long until the cache expires, this time it is renewed every call, ie it indicates how long
        ///         without calling the item should be removed from the cache.
        ///     </para>
        /// </summary>
        public float SlideExpiration { get; set; }

        /// <summary>
        ///     <para>
        ///         Indicates how much time remains in the cache being removed after this period. It will not be used if the
        ///         parameter SlideExpiration is greater than zero .
        ///     </para>
        /// </summary>
        public float AbsoluteExpirationTime { get; set; }

        /// <summary>
        ///     <para>Indicates the path where the cache will be saved.Only for File and MDB container type</para>
        /// </summary>
        public string StoragePath { get; set; }

        /// <summary>
        ///     <para>
        ///         Indicates whether to keep the cache after restart the application (eg IISReset).Only for File and MDB
        ///         container type
        ///     </para>
        /// </summary>
        public bool KeepCache { get; set; }

        /// <summary>
        ///     <para>
        ///         Indicates whether to maintain an audit file to another, use with extreme caution because it can generate
        ///         extremely large files in a short time. Only for MDB container type
        ///     </para>
        /// </summary>
        public bool Audit { get; set; }
    }
}