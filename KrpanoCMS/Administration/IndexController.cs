using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KrpanoCMS.Administration
{
    public class IndexController : Controller
    {
        // GET: Index
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            ExecuteCommand("echo testing");

            return View();
        }

        static void ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("Krpano/krpanotools64.exe", "Krpano/templates/flat.config " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode, "ExecuteCommand");
            process.Close();
        }

        static void Main()
        {
            ExecuteCommand("echo testing");
        }
    }
}