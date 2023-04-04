using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
using PagedList;

namespace Ictshop.Areas.Admin.Controllers
{
    public class HomeController : Controller
        
    {
        ShopShoe db = new ShopShoe();

        // GET: Admin/Home

        //public ActionResult Index()
        //{
        //    if (Session["UserId"] != null)
        //    {
        //        var user = (Nguoidung)Session["UserId"];
        //        if (user.IDQuyen == 2) // kiểm tra xem người dùng có quyền admin không
        //        {
        //            return View(); // hiển thị trang admin
        //        }
        //        return RedirectToAction("Error", "Home");
        //    }
        //}
        public ActionResult Index(int ?page)
        {
            if (Session["UserId"] != null)
            {
                var user = (Nguoidung)Session["UserId"];
                if (user.IDQuyen != 2) 
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            if (page == null) page = 1;
            var sp = db.Sanphams.OrderBy(x => x.Masp);
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(sp.ToPagedList(pageNumber, pageSize));

        }

        // Xem chi tiết người dùng GET: Admin/Home/Details/5 
        public ActionResult Details(int id)
        {
            var dt = db.Sanphams.Find(id);
            return View(dt);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Mahang = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang");
            ViewBag.Mahdh = new SelectList(db.Chatlieux, "Macl", "Tencl");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Masp,Tensp,Giatien,Soluong,Mota,Sanphammoi,Anhbia,Mahang,Macl")] Sanpham sanpham)
        {
            if (ModelState.IsValid)
            {
                db.Sanphams.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Mahang = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang", sanpham.Mahang);
            ViewBag.Mahdh = new SelectList(db.Chatlieux, "Macl", "Tencl", sanpham.Macl);
            return View(sanpham);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var dt = db.Sanphams.Find(id);
            var hangselected = new SelectList(db.Hangsanxuats, "Mahang", "Tenhang", dt.Mahang);
            ViewBag.Mahang = hangselected;
            var hdhselected = new SelectList(db.Chatlieux, "Macl", "Tencl", dt.Macl);
            ViewBag.Mahdh = hdhselected;
            // 
            return View(dt);
        }

        [HttpPost]
        public ActionResult Edit(Sanpham sanpham)
        {
            try
            {
                // Sửa sản phẩm theo mã sản phẩm
                var oldItem = db.Sanphams.Find(sanpham.Masp);
                oldItem.Tensp = sanpham.Tensp;
                oldItem.Giatien = sanpham.Giatien;
                oldItem.Soluong = sanpham.Soluong;
                oldItem.Mota = sanpham.Mota;
                oldItem.Anhbia = sanpham.Anhbia;
                oldItem.Mahang = sanpham.Mahang;
                oldItem.Macl = sanpham.Macl;
                // lưu lại
                db.SaveChanges();
                // xong chuyển qua index
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var dt = db.Sanphams.Find(id);
            return View(dt);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                //Lấy được thông tin sản phẩm theo ID(mã sản phẩm)
                var dt = db.Sanphams.Find(id);
                // Xoá
                db.Sanphams.Remove(dt);
                // Lưu lại
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
