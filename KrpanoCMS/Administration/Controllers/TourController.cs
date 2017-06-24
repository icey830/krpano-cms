using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using KrpanoCMS.Administration.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace KrpanoCMS.Administration.Controllers
{
    public class TourController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View(db.Tour.ToList().Where(item => item.UserId == User.Identity.GetUserId()).OrderByDescending(item => item.Id));
        }

        public ActionResult Tiles()
        {
            return View(db.Tour.ToList().Where(item => item.UserId == User.Identity.GetUserId()).OrderByDescending(item => item.Id));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tour.Find(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }

        public ActionResult Create()
        {
            TourModel model = new TourModel();

            var panoramaList = db.Panorama.ToList().Where(item => item.UserId == User.Identity.GetUserId()).ToList();

            model.PanoramaListId = panoramaList.Select(item => item.Id).ToList();
            ViewBag.ItemInfo = panoramaList.ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TourModel model)
        {
            model.Tour.UserId = User.Identity.GetUserId();
            
            if (ModelState.IsValid)
            {
                db.Tour.Add(model.Tour);
                db.SaveChanges();

                foreach (var panoId in model.PanoramaListId)
                {
                    var item = new TourPano()
                    {
                        FkPanoId = panoId,
                        FkTourId = model.Tour.Id
                    };
                    db.TourPano.Add(item);
                    db.SaveChanges();
                }

                CreateTour(model.PanoramaListId);
                var rootFolderPath = System.Web.HttpContext.Current.Server.MapPath(@"/Documents/Panoramas/");
                Directory.Move(rootFolderPath + "vtour", rootFolderPath + "vtour" + model.Tour.Id);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        private static void CreateTour(List<int> panoramaIds)
        {
            PanoramaController.ExecuteKrpanotools(panoramaIds.ToArray(), "sphere", 360, 180);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tour tour = db.Tour.Find(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tour tour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tour).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tour);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tour.Find(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tour tour = db.Tour.Find(id);
            db.Tour.Remove(tour);
            db.SaveChanges();
            return RedirectToAction("Index");
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
