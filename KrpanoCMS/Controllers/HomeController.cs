using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace KrpanoCMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           // ExecuteCommand();

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        static void ExecuteCommand()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            var krpanoPath = System.Web.HttpContext.Current.Server.MapPath(@"/Krpano");
            var command = @"cd """ + krpanoPath +
                          @""" && start krpanotools64.exe makepano -config=templates\flat.config dog.jpg";
            cmd.StandardInput.WriteLine(command);
            //cmd.StandardInput.WriteLine(@"ping abv.bg");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Debug.WriteLine(cmd.StandardOutput.ReadToEnd());
        }


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

                var filePathOriginal = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(originalPath), fileName);
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