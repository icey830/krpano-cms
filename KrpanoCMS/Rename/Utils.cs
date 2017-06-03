using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace KrpanoCMS.Rename
{
    public class FileUploader
    {
        /// <summary>
        /// Save photo in different size
        /// </summary>
        /// <param name="photo">Photo that is uploaded</param>
        /// <returns>0 or 1 for success</returns>
        public static int Upload(HttpPostedFileBase photo, string customFileName)
        {
            //put resize values in configuration!
            try
            {
                //put this in transaction :) ( if can't save photo, dont save the article?)
                if (photo == null || photo.ContentLength <= 0 || String.IsNullOrEmpty(photo.FileName) ||
                    photo.FileName == null)
                    return 0;

                const string originalPath = @"~/Documents/Panoramas";

                var fileName = customFileName.IsNullOrWhiteSpace() ? "" : customFileName;

                var filePathOriginal = Path.Combine(HttpContext.Current.Server.MapPath(originalPath), fileName);
                photo.SaveAs(filePathOriginal);

                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //TODO implement email message alert
            }
        }

       
    }
}