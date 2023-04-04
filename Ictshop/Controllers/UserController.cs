using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
namespace Ictshop.Controllers
{
    public class UserController : Controller
    {
        ShopShoe db = new ShopShoe();

        public static bool ValidateVNPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+84", "0");
            Regex regex = new
            Regex(@"^(0)(86|96|97|98|32|33|34|35|36|37|38|39|91|94|83|84|85|81|82|90|93|70|79|77|76|78|92|56|58|99|59|55|87)\d{7}$");
            return regex.IsMatch(phoneNumber);
        }
        public bool ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }
        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }

        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung)
        {
            try
            {
                Session["userReg"] = nguoidung;

                // Thêm người dùng  mới
                db.Nguoidungs.Add(nguoidung);
                if (db.Nguoidungs.Any(x => x.Email == nguoidung.Email))
                {
                    ViewBag.BiTrungEmail = "Email này đã tồn tại!!!";
                    return View("DangKy", nguoidung);
                }



                if (nguoidung.Matkhau.Length < 8)
                {
                    ViewBag.errorPass = "Mật khẩu phải có ít nhất 8 ký tự";
                    return View(nguoidung);
                }
                else if (!nguoidung.Matkhau.Any(char.IsUpper))
                {
                    ViewBag.errorPass = "Mật khẩu phải có ít nhất một chữ cái hoa";
                    return View(nguoidung);
                }
                else if (!nguoidung.Matkhau.Any(char.IsLower))
                {
                    ViewBag.errorPass = "Mật khẩu phải có ít nhất một chữ cái thường";
                    return View(nguoidung);
                }
                else if (!nguoidung.Matkhau.Any(char.IsDigit))
                {
                    ViewBag.errorPass = "Mật khẩu phải có ít nhất một số";
                    return View(nguoidung);
                }
                else if (!nguoidung.Matkhau.Any(c => !char.IsLetterOrDigit(c)))
                {
                    ViewBag.errorPass = "Mật khẩu phải có ít nhất một ký tự đặc biệt";
                    return View(nguoidung);
                }else if (ValidateVNPhoneNumber(nguoidung.Dienthoai) == false)
                {
                    ViewBag.sdt = "Số điện thoại không phù hợp";
                }else if (ValidateEmail(nguoidung.Email) == false)
                {
                    ViewBag.email = "Email không hợp lệ";
                } 
                else
                {
                    db.SaveChanges();
                    ViewBag.regOK = "Đăng kí thành công. Đăng nhập ngay";
                    ViewBag.isReg = true;
                    return View("Dangky");
                }
                return View(nguoidung);                            
            }
            catch(Exception ex)
            {
                ViewBag.regOK = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();

        }


        [HttpPost]
        public ActionResult Dangnhap(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Nguoidungs.FirstOrDefault(u => u.Email == model.userMail && u.Matkhau == model.password);
                if (user != null)
                {
                    Session["UserId"] = user.IDQuyen;
                    if (user.IDQuyen == 2)
                    {
                        Session["UserId"] = user;
                        return RedirectToAction("Index", "Admin/Home");

                    }
                    else
                    {
                        Session["UserId"] = user;
                        return RedirectToAction("Index", "Home");

                    }
                }
                else
                {
                    ViewBag.Fail = "Tài khoản hoặc mật khẩu không chính xác.";
                }             
            }
            return View("Dangnhap");

        }
        public ActionResult DangXuat()
        {
            Session["UserId"] = null;
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Profile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoiDung = db.Nguoidungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen, Anhdaidien,Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                //@ViewBag.show = "Chỉnh sửa hồ sơ thành công";
                //return View(nguoidung);
                return RedirectToAction("Profile", new { id = nguoidung.MaNguoiDung });

            }
            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
    }
}