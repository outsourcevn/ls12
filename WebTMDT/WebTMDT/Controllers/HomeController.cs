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

        public IEnumerable<CatViewModel> CreateVM(int parentid, IEnumerable<Category> source)
        {
            return from men in source
                   where men.F3 == parentid
                   select new CatViewModel(){
                              CatId = men.F1, 
                              CatName = men.F2
                              //// other properties
                              //ChildrenCat = CreateVM(men.F1, source)
                          };
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