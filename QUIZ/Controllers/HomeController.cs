using QUIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QUIZ.Controllers
{
    public class HomeController : Controller
    {
        DBQUIZEntities db = new DBQUIZEntities();
        public ActionResult tlogin()
        {
            return View();
        }


        public ActionResult slogin()
        {
            return View();
        }


        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddCategroy()
        {
            Session["ad_id"] = 1;

            int adid = Convert.ToInt32(Session["ad_id"].ToString());
            List<TBL_categroy> li = db.TBL_categroy.Where(x => x.cat_fk_adid == adid).OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;
            return View();
        }



        [HttpPost]
        public ActionResult AddCategroy(TBL_categroy cat)
        {
          
            List<TBL_categroy> li = db.TBL_categroy.OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;
            TBL_categroy c = new TBL_categroy();
            c.cat_name = cat.cat_name;
            c.cat_fk_adid = Convert.ToInt32(Session["ad_id"].ToString());
            db.TBL_categroy.Add(c);
            db.SaveChanges();
            return RedirectToAction("AddCategroy");
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