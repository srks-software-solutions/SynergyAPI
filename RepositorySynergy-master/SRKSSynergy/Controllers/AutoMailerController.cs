using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class AutoMailerController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        [HttpGet]
        public ActionResult Create()
        {           
            return View();
        }
        [HttpPost]
        public ActionResult Create(AutoMailer am, string module)
        {            
            var cpid = Convert.ToInt32(Session["logincpid"]);
            am.Module = module;        
            am.CreatedBy = Convert.ToString(cpid);
            am.CreatedOn = System.DateTime.Now;
            am.IsDeleted = 0;
            am.IsStatus = 0;
            db.AutoMailer.Add(am);
            db.SaveChanges();
            return RedirectToAction("Index", "AutoMailer", null);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var list = db.AutoMailer.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            AutoMailer am = db.AutoMailer.Find(id);
            if (am == null)
            {
                return HttpNotFound();
            }                        
            var module = db.AutoMailer.Where(m => m.AMID == id).Select(m => m.Module).SingleOrDefault();
            ViewBag.module = module;

            return View(am);
        }

        [HttpPost]
        public ActionResult Edit(AutoMailer am, string module)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            if (am.AMID != 0)
            {
                am.Module = module;
                am.ModifiedBy = Convert.ToString(userid);
                am.ModifiedOn = System.DateTime.Now;
                db.Entry(am).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Modified Successfully!!!!";
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            AutoMailer ts = db.AutoMailer.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            return View(ts);
        }

        [HttpPost]
        public ActionResult Delete(AutoMailer am1, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                AutoMailer am = db.AutoMailer.Find(id);
                am.IsDeleted = 1;
                db.Entry(am).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Deleted Successfully!!!!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}