using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Mtn.Library.Enums;
using Mtn.Library.Extensions;

namespace Mtn.Library.Entities
{
    /// <summary>
    /// Result from image tasks
    /// </summary>
    public class ImageResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ImageResult(Stream stream = null)
        {
            if (stream != null)
            {
                StreamContent = stream;
            }
        }
        /// <summary>
        /// Image format
        /// </summary>
        public Enums.ImageFormat ImageFormat
        {
            get
            {
                return Image.GetImageFormatEnumMtn();
            }

        }

        /// <summary>
        /// File path if image are saved on disk
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Return bytes encoded in base 64
        /// </summary>
        public string Base64
        {
            get
            {
                if (StreamContent == null)
                {
                    return "";
                }
                return StreamContent.ToBase64StringMtn();

            }
        }

        /// <summary>
        /// return in DataUri format
        /// </summary>
        public string DataUri
        {
            get
            {

                var strMime = Image != null?Image.GetDecoderMtn().MimeType:"";
                
                return string.Format("data:{0};base64,{1}", strMime, Base64);

            }
        }

        /// <summary>
        /// Return Image from stream
        /// </summary>
        public System.Drawing.Image Image
        {
            get { return StreamContent == null ? null : System.Drawing.Image.FromStream(StreamContent); }
        }

        /// <summary>
        /// Stream with Image
        /// </summary>
        public Stream StreamContent { get; set; }
    }
}
