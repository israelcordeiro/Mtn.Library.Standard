using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mtn.Library.Extensions;

namespace Mtn.Library.Enums
{
    /// <summary>
    /// Format type to send data
    /// </summary>
    public enum HttpFormatType
    {
        /// <summary>
        /// Json
        /// </summary>
        [Description("application/json")]
        Json,
        /// <summary>
        /// multipart/form-data 
        /// </summary>
        [Description("multipart/form-data")]
        FormData,
        /// <summary>
        /// multipart/form-data 
        /// </summary>
        [Description("application/x-www-form-urlencoded")]
        UrlEncoded
    }

    /// <summary>
    /// Format type to send data
    /// </summary>
    public enum HttpMethodType
    {
        /// <summary>
        /// Post
        /// </summary>
        [Description("POST")]
        Post,
        /// <summary>
        /// Get
        /// </summary>
        [Description("GET")]
        Get
    }
}