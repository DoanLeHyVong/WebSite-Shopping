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
    public class DonhangsController : Controller
    {
        private ShopShoe db = new ShopShoe();

        // GET: Admin/Donhangs
        public ActionResult Index()
        {
            var donhangs = db.Donhangs.Include(d => d.Nguoidung);
            return View(donhangs.ToList());
        }

        // GET: Admin/Donhangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donhang donhang = db.Donhangs.Find(id);
            if (donhang == null)
            {
                return HttpNotFound();
            }
            return View(donhang);
        }

        // GET: Admin/Donhangs/Create
        public ActionResult Create()
        {
            ViewBag.MaNguoidung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Anhdaidien");
            return View();
        }


        // xác nhận đơn hàng và lưu vào database số lượng tồn  - số lượng đã đặt
        public ActionResult Xacnhan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donhang donhang = db.Donhangs.Find(id);
            
            if (donhang == null)
            {
                return HttpNotFound();
            }
            donhang.Tinhtrang = 1;
            db.SaveChanges();

            var chitietdonhang = db.Chitietdonhangs.Where(x => x.Madon == id).ToList();
            foreach (var ctdh in chitietdonhang)
            {
                // update so luong
                Sanpham sp = db.Sanphams.FirstOrDefault(x => x.Masp == ctdh.Masp);
                sp.Soluong = sp.Soluong - ctdh.Soluong;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Admin/Donhangs/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Madon,Ngaydat,Tinhtrang,Thanhtoan,MaNguoidung,Diachinhanhang")] Donhang donhang)
        {
            if (ModelState.IsValid)
            {
                db.Donhangs.Add(donhang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoidung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Anhdaidien", donhang.MaNguoidung);
            return View(donhang);
        }

        // GET: Admin/Donhangs/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donhang donhang = db.Donhangs.Find(id);
            if (donhang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNguoidung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Anhdaidien", donhang.MaNguoidung);
            return View(donhang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Madon,Ngaydat,Tinhtrang,Thanhtoan,MaNguoidung,Diachinhanhang")] Donhang donhang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donhang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoidung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Anhdaidien", donhang.MaNguoidung);
            return View(donhang);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donhang donhang = db.Donhangs.Find(id);
            if (donhang == null)
            {
                return HttpNotFound();
            }
            return View(donhang);
        }

        // POST: Admin/Donhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donhang donhang = db.Donhangs.Find(id);
            db.Donhangs.Remove(donhang);
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
