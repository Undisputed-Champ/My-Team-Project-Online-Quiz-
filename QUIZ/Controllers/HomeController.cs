using QUIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace QUIZ.Controllers
{
    public class HomeController : Controller
    {
        DBQUIZEntities1 db = new DBQUIZEntities1();
        public ActionResult tlogin()
        {
           
            return View();
        }

        [HttpPost]
        public ActionResult tlogin(TBL_ADMIN a) {

            TBL_ADMIN ad = db.TBL_ADMIN.Where(x => x.AD_Name == a.AD_Name && x.AD_PASSWORD == a.AD_PASSWORD).SingleOrDefault();
            if(ad!=null)
            {
                Session["ad_id"] = ad.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid username and Password";
            }
            return View();
        }
        [HttpGet]
        public ActionResult sregister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult sregister( TBL_STUDENT svw,HttpPostedFileBase imgfile)
        {
            TBL_STUDENT s = new TBL_STUDENT();
            try
            {
                s.S_NAME = svw.S_NAME;
                s.S_PASSWORD = svw.S_PASSWORD;
                s.S_IMAGE = uploadimage(imgfile);
                db.TBL_STUDENT.Add(s);
                db.SaveChanges();
                return RedirectToAction("slogin");

            }
            catch(Exception)
            {
                ViewBag.msg = "Data could not insert";
            }
            
            return View();
        }

        public string uploadimage(HttpPostedFileBase imgfile)
        {
            string path = "-1";
            try
            {
                if(imgfile!=null && imgfile.ContentLength>0)
                {
                    string extension = Path.GetExtension(imgfile.FileName);
                    if(extension.ToLower().Equals("jpg")||extension.ToLower().Equals("jpeg")||extension.ToLower().Equals("png"))
                    {
                        Random r = new Random();
                     
                        path=  Path.Combine(Server.MapPath("~/Content/img"), r + Path.GetFileName(imgfile.FileName));
                        imgfile.SaveAs(path);
                        path = "~/Content/img" + r + Path.GetFileName(imgfile.FileName);
                    }

                }
            }catch( Exception)
                {
                ViewBag.msg = "Successful not";

            }
            return path;
        }


        public ActionResult slogin()
        {
            return View();
        }


        [HttpPost]

        
        public ActionResult slogin( TBL_STUDENT s)
        {
            
            
            TBL_STUDENT std = db.TBL_STUDENT.Where(x => x.S_NAME == s.S_NAME && x.S_PASSWORD == s.S_PASSWORD).SingleOrDefault();

            if(std==null)
            {
                ViewBag.msg = "Invalid Email or Password";
            }
            else
            {
                Session["std_id"] = std.S_ID;
                return RedirectToAction("StudentExam");
            }
            return View();
        }

        public ActionResult StudentExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }
            return View();
        }

        public ActionResult QuizStart()
        {
            if(Session["std_id"]==null)
            {
                return RedirectToAction("slogin");
            }
            TBL_QUESTIONS q = null;
            if(TempData["questions"]!=null)
            {
                Queue<TBL_QUESTIONS> qlist = (Queue<TBL_QUESTIONS>)TempData["questions"];
                if(qlist.Count>0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();
                    TempData["questions"] = qlist;
                    TempData.Keep();
                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else
            {
                return RedirectToAction("StudentExam");
            }

            return View();
        }
        public ActionResult EndExam()
        {
            return View();
        }
        [HttpPost]
        public ActionResult QuizStart(TBL_QUESTIONS q)
        {
            return RedirectToAction("QuizStart");
        }

        [HttpPost]
        public ActionResult StudentExam(String room)
        {
            List<TBL_categroy> list = db.TBL_categroy.ToList();
            foreach (var item in list)
            {
                if (item.cat_encyptedstring == room)
                {
                    List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.q_fk_catid == item.cat_id).ToList();
                    Queue<TBL_QUESTIONS> queue = new Queue<TBL_QUESTIONS>();
                    foreach(TBL_QUESTIONS a in li)
                    {
                        queue.Enqueue(a);
                    }
                    TempData["question"] = queue;
                    TempData["score"] = 0;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }


                else
                {
                    ViewBag.error = "No  Room Found......";
                }
            }
            return View();
        }


        public ActionResult Dashboard()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddCategroy()
        {
           // Session["ad_id"] = 1;
           if(Session["ad_id"]==null)
            {
                return RedirectToAction("Index");
            }

            int adid = Convert.ToInt32(Session["ad_id"].ToString());
            List<TBL_categroy> li = db.TBL_categroy.Where(x => x.cat_FK_adid == adid).OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;
            return View();
        }



        [HttpPost]
        public ActionResult AddCategroy(TBL_categroy cat)
        {
          
            List<TBL_categroy> li = db.TBL_categroy.OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;
            Random r = new Random();
            TBL_categroy c = new TBL_categroy();
            c.cat_name = cat.cat_name;
            c.cat_encyptedstring = cyptop.Encrypt(cat.cat_name.Trim()+r.Next().ToString(),true);
            c.cat_FK_adid = Convert.ToInt32(Session["ad_id"].ToString());
            db.TBL_categroy.Add(c);
            db.SaveChanges();
            return RedirectToAction("AddCategroy");
        }
        [HttpGet]
      public ActionResult Addquestions()
        {


            int sid = Convert.ToInt32(Session["ad_id"]);
            List<TBL_categroy> li = db.TBL_categroy.Where(x => x.cat_FK_adid == sid).ToList();
            ViewBag.List = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult Addquestions(TBL_QUESTIONS q)
        {


            int sid = Convert.ToInt32(Session["ad_id"]);
            List<TBL_categroy> li = db.TBL_categroy.Where(x => x.cat_FK_adid == sid).ToList();
            ViewBag.List = new SelectList(li, "cat_id", "cat_name");
       

            TBL_QUESTIONS QA = new TBL_QUESTIONS();

            QA.Q_TEXT = q.Q_TEXT;
            QA.OPA = q.OPA;

            QA.OPB = q.OPB;
            QA.OPC = q.OPC;
            QA.OPD = q.OPD;
            QA.COD = q.COD;
            QA.q_fk_catid = q.q_fk_catid;



            db.TBL_QUESTIONS.Add(QA);
            db.SaveChanges();

            TempData["msg"] = "Question added successfully...";
            TempData.Keep();

            return RedirectToAction("Addquestions");
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
        public ActionResult Index()
        {
            if(Session["ad_id"]!=null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();


           
        }
    
    public ActionResult LOGOUT()
    {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");



    }
}
}