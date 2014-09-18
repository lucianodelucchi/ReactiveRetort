// -----------------------------------------------------------------------------
//  <copyright file="OwnImage.cs" company="">
//      Copyright (c) 
//  </copyright>
// -----------------------------------------------------------------------------
namespace ReactiveRetort.Models
{
    using System;
    using IO = System.IO;
    
    /// <summary>
    /// Description of Image.
    /// </summary>
    public class OwnImage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the filename of the image.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the full path of the image.
        /// </summary>
        public string Path { get; set; }

        private string saveAs;
        /// <summary>
        /// Gets or sets the full path of where to save the converted image.
        /// </summary>
        public string SaveAs {
            get
            {
                if (this.DestinationFolder != null)
                {
                    return IO.Path.Combine(IO.Path.GetDirectoryName(this.Path), this.DestinationFolder, this.FileName);
                }

                return saveAs;
            }
            set { saveAs = value; }
        }

        /// <summary>
        /// Gets or sets the destination folder for the converted images, 
        /// relative to folder where the original images are
        /// </summary>
        public string DestinationFolder { get; set; }

        #endregion // Properties

        protected OwnImage()
        {
            //defaults
            this.DestinationFolder = "compressed";
        }
        
        #region Creation
        public static OwnImage CreateNewOwnImage()
        {
            return new OwnImage();
        }

        public static OwnImage CreateOwnImage(string filename, string path)
        {
            return new OwnImage
            {
                Path = path,
                FileName = filename
            };
        }

        public static OwnImage CreateOwnImage(string filename, string path, string saveAs)
        {
            return new OwnImage
            {
                Path = path,
                SaveAs = saveAs,
                FileName = filename
            };
        }

        public static OwnImage CreateFrom(IO.FileInfo finfo)
        {
            return CreateOwnImage(finfo.Name, finfo.FullName);
        }
        #endregion // Creation
    }
}
