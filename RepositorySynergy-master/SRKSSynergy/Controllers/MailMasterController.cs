using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class MailMasterController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /MailMaster/

        public ActionResult Index()
        {
            //ViewBag.Logout = Session["Username"];
            //if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            //{
            //    return RedirectToAction("login", "Login", null);
            //}
            var mailm = db.MailMasters.Where(m => m.IsDeleted == 0).ToList();
            return View(mailm);

          //  return View();
        }


        [HttpGet]
        public ActionResult AddEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmail(MailMasters mailm, string emailtyp = null)
        {
            int mailem = db.MailMasters.Where(m => m.EmailAddress == mailm.EmailAddress).Where(m => m.MailType == emailtyp).Where(m => m.IsDeleted == 0).Count();
            if (mailem != 0)
            {
                TempData["SameData"] = "Email Address with Same Email Type already exists!!";
                return View();
            }

            mailm.MailType = emailtyp;
            
            db.MailMasters.Add(mailm);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id = 0)
        {
            
            MailMasters mailm = db.MailMasters.Find(id);
            if (mailm == null)
            {
                return HttpNotFound();
            }
            return View(mailm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int a = 0)
        {
            
            MailMasters mailm = db.MailMasters.Find(id);
            mailm.IsDeleted = 1;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
