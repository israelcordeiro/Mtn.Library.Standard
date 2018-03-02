namespace Mtn.Library.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum JsonProvider
    {
        /// <summary>
        /// Default from config or normally jsonnet if find the assembly, or microsoft javascript serializer
        /// </summary>
        SystemDefault = 0,
        /// <summary>
        /// Microsoft Json
        /// </summary>
        MicrosoftJavascriptSerializer = 1,
        /// <summary>
        /// Newtonsoft Json.Net
        /// </summary>
        JsonNet = 2,
        /// <summary>
        /// FastJson
        /// </summary>
        FastJson = 3
    }

    /// <summary>
    /// Indicates the type of compression.
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// Deflate
        /// </summary>
        Deflate,
        /// <summary>
        /// GZip
        /// </summary>
        GZip
    }
}
