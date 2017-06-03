using System;
using System.Collections.Generic;
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
            return View();
        }
    }
}