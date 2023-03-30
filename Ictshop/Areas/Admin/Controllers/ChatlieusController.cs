using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Areas.Admin.Controllers
{
    public class ChatlieusController : Controller
    {
        private ShopShoe db = new ShopShoe();

        public ActionResult Index()
        {
            return View(db.Chatlieux.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chatlieu chatlieu = db.Chatlieux.Find(id);
            if (chatlieu == null)
            {
                return HttpNotFound();
            }
            return View(chatlieu);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Macl,Tencl")] Chatlieu chatlieu)
        {
            if (ModelState.IsValid)
            {
                db.Chatlieux.Add(chatlieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chatlieu);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chatlieu chatlieu = db.Chatlieux.Find(id);
            if (chatlieu == null)
            {
                return HttpNotFound();
            }
            return View(chatlieu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Macl,Tencl")] Chatlieu chatlieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatlieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chatlieu);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chatlieu chatlieu = db.Chatlieux.Find(id);
            if (chatlieu == null)
            {
                return HttpNotFound();
            }
            return View(chatlieu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chatlieu chatlieu = db.Chatlieux.Find(id);
            db.Chatlieux.Remove(chatlieu);
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
