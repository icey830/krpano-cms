using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace KrpanoCMS.Administration.Controllers
{
    public class HotspotController : Controller
    {
        private Entities db = new Entities();

        public JsonResult GetAll(int? panoramaId)
        {
            if (panoramaId == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            List<Hotspot> hotspotList = db.Hotspot.Where(item => item.FkPanoramaId == panoramaId).ToList();

            return Json(new { success = true, hotspotList = hotspotList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(Hotspot hotspot)
        {
            hotspot.AddedOn = DateTime.Now;
            hotspot.FkUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            db.Hotspot.Add(hotspot);
            db.SaveChanges();

            return Json(new { success = true, hotspot = hotspot }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(Hotspot hotspot)
        {
            hotspot.AddedOn = DateTime.Now;
            hotspot.FkUserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
                return Json(new { success = false, hotspot = hotspot }, JsonRequestBehavior.AllowGet);

            db.Entry(hotspot).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, hotspot = hotspot }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            Hotspot hotspot = db.Hotspot.Find(id);
            db.Hotspot.Remove(hotspot);
            db.SaveChanges();

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
