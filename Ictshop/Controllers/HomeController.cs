using Ictshop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ictshop.Controllers
{
    public class HomeController : Controller
    {
        ShopShoe db = new ShopShoe();
        public ActionResult Index(int ? page )
        {

            if (page == null) page = 1;
            var giay = (from s in db.Sanphams select s).OrderBy(m => m.Tensp);
            int pageSize = 2;
            int pageNum = page ?? 1;
            return View(giay.ToPagedList(pageNum, pageSize));

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}