using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
using Ictshop.Others;
using Newtonsoft.Json.Linq;
using System.IO;
using Ictshop.Migrations;
using System.Web.UI.WebControls;

namespace Ictshop.Controllers
{
    public class GioHangController : Controller
    {
        ShopShoe db = new ShopShoe();
        
        //Lấy giỏ hàng 
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì mình tiến hành khởi tao list giỏ hàng (sessionGioHang)
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int iMasp, string strURL)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMasp);
            if ( sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy ra session giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp này đã tồn tại trong session[giohang] chưa
            GioHang sanpham = lstGioHang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMasp);
                //Add sản phẩm mới thêm vào list
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Cập nhật giỏ hàng 
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            //Kiểm tra masp
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp== iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp có tồn tại trong session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());

            }
            return RedirectToAction("GioHang");
        }
        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Kiểm tra masp
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp== iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
        //Tính tổng số lượng và tổng tiền
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }
        //tạo partial giỏ hàng
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        //Xây dựng 1 view cho người dùng chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);

        }

        #region 
        //Xây dựng chức năng đặt hàng
        [HttpPost]
        public ActionResult DatHang(FormCollection donhangForm)
        {
            //Kiểm tra đăng đăng nhập
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            Console.WriteLine(donhangForm);
            string diachinhanhang = donhangForm["Diachinhanhang"].ToString();
            Session["diachinhanhang"] = diachinhanhang;
            string thanhtoan = donhangForm["MaTT"].ToString();
            int ptthanhtoan = Int32.Parse(thanhtoan);
            if(ptthanhtoan == 2)
            {
                return RedirectToAction("Momo", "GioHang");
            }
            //Thêm đơn hàng
            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["UserId"];
            List<GioHang> gh = LayGioHang();
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            ddh.Thanhtoan = ptthanhtoan;
            ddh.Diachinhanhang = diachinhanhang;
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }
            ddh.Tongtien = tongtien;
            db.Donhangs.Add(ddh);
            db.SaveChanges();
            //Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                Chitietdonhang ctDH = new Chitietdonhang();
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                ctDH.Madon = ddh.Madon;
                ctDH.Masp = item.iMasp;
                ctDH.Soluong = item.iSoLuong;
                ctDH.Dongia = (decimal)item.dDonGia;
                ctDH.Thanhtien = (decimal)thanhtien;
                ctDH.Phuongthucthanhtoan = 1;
                db.Chitietdonhangs.Add(ctDH);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Donhangs");

        }
        #endregion

        public ActionResult ThanhToanDonHang()
        {
            ViewBag.MaTT = new SelectList(new[]
                {
                    new { MaTT = 1, TenPT="Thanh toán tiền mặt" },
                    new { MaTT = 2, TenPT="Thanh toán quét mã Momo" },
                }, "MaTT", "TenPT", 1);
            ViewBag.MaNguoiDung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Hoten");

            //Kiểm tra đăng đăng nhập
            if (Session["UserId"] == null || Session["UserId"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            //Thêm đơn hàng
            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["UserId"];
            List<GioHang> gh = LayGioHang();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            Chitietdonhang ctDH = new Chitietdonhang();
            ViewBag.tongtien = tongtien;
            return View(ddh);
            db.SaveChanges();

        }
        public ActionResult Momo()
        {
            List<GioHang> gh = LayGioHang();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "test";
            string returnUrl = "https://localhost:44322/GioHang/Thanhtoanmomo";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/GioHang/LuuThanhToan"; 

            string amount = tongtien.ToString();
            string orderid = DateTime.Now.Ticks.ToString(); 
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult Thanhtoanmomo(Result result)
        {
            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["UserId"];
            List<GioHang> gh = LayGioHang();
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            ddh.Thanhtoan = 2;
            ddh.Diachinhanhang = Session["diachinhanhang"].ToString();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }
            ddh.Tongtien = tongtien;
            db.Donhangs.Add(ddh);
            db.SaveChanges();
            //Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                Chitietdonhang ctDH = new Chitietdonhang();
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                ctDH.Madon = ddh.Madon;
                ctDH.Masp = item.iMasp;
                ctDH.Soluong = item.iSoLuong;
                ctDH.Dongia = (decimal)item.dDonGia;
                ctDH.Thanhtien = (decimal)thanhtien;
                ctDH.Phuongthucthanhtoan = 1;
                db.Chitietdonhangs.Add(ctDH);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Donhangs");
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode;
            return View();
        }

        [HttpPost]
        public void Luuthanhtoan()
        {
            //cập nhật dữ liệu vào db
            String a = "";
        }
    }
}