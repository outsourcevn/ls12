using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTMDT.Models;

namespace WebTMDT.Controllers
{
    public class HomeController : Controller
    {
        private langson12Entities db = new langson12Entities();

        public ActionResult Index()
        {
            var Cat = db.Categories.ToList();
            ViewBag.Category = Cat;
            return View();
        }

        [ChildActionOnly]
        public IEnumerable<CatViewModel> CreateVM(int parentid, IEnumerable<Category> source)
        {
            var data = from men in source
                       where men.F3 == parentid
                       select new CatViewModel()
                       {
                           CatId = men.F1,
                           CatName = men.F2
                           //// other properties
                           //ChildrenCat = CreateVM(men.F1, source)
                       };
            return data;
        }

        [HttpPost]
        public ActionResult SelectDanhMuc3(int id)
        {
            var data = from men in db.Categories
                       where men.F3 == id
                       select new DanhMucCon()
                       {
                           Id = men.F1,
                           Name = men.F2
                       };
            return Json(data, JsonRequestBehavior.AllowGet);
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