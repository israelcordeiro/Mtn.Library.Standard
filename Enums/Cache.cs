namespace Mtn.Library.Enums
{
    #region Enums
    /// <summary>
    /// <para>Cache type</para>
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// <para>Default.</para>
        /// </summary>
        Default,
        /// <summary>
        /// <para>Hash type, put in memory</para>
        /// </summary>
        Hash,
        /// <summary>
        /// <para>Use multiples files, very usefull to large objects like a web page, Result object must be marked as [Serializable] or be a primitive type</para>
        /// </summary>
        FileSystemMultipleFiles,
        /// <summary>
        /// <para>Use MDB (ms acess files), Be carefull with audit, can create a very big file in a short time.</para>
        /// </summary>
        FileSystemMdbFile,
        /// <summary>
        /// <para>Hash type, put in memory, sorting, faster to read, slow to insert</para>
        /// </summary>
        SortedHash,
        /// <summary>
        /// <para>AppFabric type, use the same configuration from the application</para>
        /// </summary>
        AppFabric
    }
    
    /// <summary>
    /// <para>Indicates the type of time verification will be effected.</para>
    /// </summary>
    public enum CacheTimeType
    {
        /// <summary>
        /// <para>Indicates that remain in the cache are removed after a certain period.</para>
        /// </summary>
        Absolute,
        /// <summary>
        /// <para>Indicates that it takes time until the cache expires, this time it is renewed every call, that is, if no call this period the item should be removed from the cache.</para>
        /// </summary>
        UpdateByAccess,
        /// <summary>
        /// <para>Indicates that remain in the cache and not automatically removed,  that is, will remain forever in the cache.</para>
        /// </summary>
        Forever
    }
    #endregion
}
