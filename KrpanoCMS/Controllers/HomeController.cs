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
            ExecuteCommand("makepano");


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

        static void ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            var exePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"/Krpano"), "krpanotools64.exe");
            var imgPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"/Content/images"), "dog.jpg");
            processInfo = new ProcessStartInfo(exePath, command + @" -config=templates\flat.config " + @"C:\Users\Fani\Documents\visual studio 2015\Projects\KrpanoCMS\KrpanoCMS\Krpano\cat.jpg");


            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.WorkingDirectory = "/Krpano";


            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Debug.WriteLine("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
            Debug.WriteLine("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
            Debug.WriteLine("ExitCode: " + exitCode, "ExecuteCommand");
            process.Close();
        }

        static void Main()
        {
            ExecuteCommand("echo testing");
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