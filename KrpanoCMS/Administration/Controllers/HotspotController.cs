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

        // GET: Hotspot
        public async Task<ActionResult> Index()
        {
            return View(await db.Hotspot.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotspot hotspot = await db.Hotspot.FindAsync(id);
            if (hotspot == null)
            {
                return HttpNotFound();
            }
            return View(hotspot);
        }

        public ActionResult Create()
        {
            return View();
        }

        public JsonResult GetAllHotspotByPanorama(int? panoramaId)
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
            db.SaveChangesAsync();

            return Json(new { success = true, hotspot = hotspot }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            Hotspot hotspot = db.Hotspot.Find(id);

            return Json(new { success = true, hotspot = hotspot }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Edit(Hotspot hotspot)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, hotspot = hotspot }, JsonRequestBehavior.AllowGet);

            db.Entry(hotspot).State = EntityState.Modified;
            db.SaveChangesAsync();

            return Json(new { success = true, hotspot = hotspot }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            Hotspot hotspot = db.Hotspot.Find(id);

            if (hotspot == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            Hotspot hotspot = db.Hotspot.Find(id);
            db.Hotspot.Remove(hotspot);
            db.SaveChangesAsync();

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
