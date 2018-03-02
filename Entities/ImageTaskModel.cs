using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mtn.Library.Entities
{
    /// <summary>
    /// Model to convert and create images in batch
    /// </summary>
    public class ImageTaskModel
    {
        /// <summary>
        /// Image format
        /// </summary>
        public Enums.ImageFormat ImageFormat { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Folder to save image
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// If true, will use only Folder to save image
        /// </summary>
        public bool OverrideCurrentPath { get; set; }

        /// <summary>
        /// Max width to scale
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// Max height to scale
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// If true will scale Image before crop
        /// </summary>
        public bool ScaleFirst { get; set; }


        /// <summary>
        /// If true will crop Image using CropArea as reference
        /// </summary>
        public bool CropImage { get; set; }

        /// <summary>
        /// If true will scale Image using MaxWidth and MaxHeight
        /// </summary>
        public bool ScaleImage { get; set; }

        /// <summary>
        /// If CropImage is true, CropArea will indicate the retangle to crop
        /// </summary>
        public System.Drawing.Rectangle CropArea { get; set; }
        
        /// <summary>
        /// If true, will return the result without save on disk 
        /// </summary>
        public bool DoNotSave { get; set; }

        /// <summary>
        /// For Jpeg, set quality
        /// </summary>
        public long Quality { get; set; }
    }
}
