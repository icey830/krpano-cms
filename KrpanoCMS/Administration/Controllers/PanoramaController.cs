using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using KrpanoCMS;
using KrpanoCMS.Rename;
using Microsoft.AspNet.Identity;

namespace KrpanoCMS.Administration.Controllers
{
    public class PanoramaController : Controller
    {
        private Entities db = new Entities();

        // GET: Panorama
        public ActionResult Index()
        {
            return View(db.Panorama.ToList());
        }

        // GET: Panorama/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panorama panorama = db.Panorama.Find(id);
            if (panorama == null)
            {
                return HttpNotFound();
            }
            return View(panorama);
        }

        // GET: Panorama/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Panorama/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Panorama panorama, HttpPostedFileBase photo)
        {
            if (photo != null)
            {
                //string extension = Path.GetExtension(photo.FileName);
                //FileUploader.Upload(photo, panorama.Id + extension);

                panorama.PictureUrl = photo.FileName;
            }

            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // the principal identity is a claims identity.
                // now we need to find the NameIdentifier claim
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userIdValue = userIdClaim.Value;
                }
            }

            panorama.AddedOn = DateTime.Now;
            panorama.UserId = claimsIdentity.GetUserId();

            if (ModelState.IsValid)
            {
                db.Panorama.Add(panorama);
                db.SaveChanges();

                string extension = Path.GetExtension(photo.FileName);
                FileUploader.Upload(photo, panorama.Id + extension);

                //return RedirectToAction("Details", new { id = panorama.Id });
                return RedirectToAction("CreatePano", new { id = panorama.Id, userId = panorama.UserId });
            }

            return View(panorama);
        }


        public ActionResult CreatePano(int id, string userId)
        {
            ViewBag.PanoId = id;
            // ExecuteCommandCreatePano(id);
            return View();
        }

        public JsonResult ExecuteCommandCreatePano(int id)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            var krpanoPath = System.Web.HttpContext.Current.Server.MapPath(@"/Krpano");
            var krpanoImgPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Documents/Panoramas/" + id + ".jpg");

            var command = @"cd """ + krpanoPath + @""" && start krpanotools64.exe makepano -config=templates\flat.config """ + krpanoImgPath + @"""";
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Debug.WriteLine(cmd.StandardOutput.ReadToEnd());

            return Json(new { success = true },JsonRequestBehavior.AllowGet);
        }

        // GET: Panorama/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panorama panorama = db.Panorama.Find(id);
            if (panorama == null)
            {
                return HttpNotFound();
            }
            return View(panorama);
        }

        // POST: Panorama/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,UserId,AddedOn,PictureUrl")] Panorama panorama)
        {
            if (ModelState.IsValid)
            {
                db.Entry(panorama).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(panorama);
        }

        // GET: Panorama/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panorama panorama = db.Panorama.Find(id);
            if (panorama == null)
            {
                return HttpNotFound();
            }
            return View(panorama);
        }

        // POST: Panorama/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Panorama panorama = db.Panorama.Find(id);
            db.Panorama.Remove(panorama);
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
