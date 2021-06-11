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
    public class TargetSettingController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
            if (User.IsInRole("ZonalManager"))
            {
                var result = (from r in db.ChannelPartners
                              where (r.CPName.ToLower().Contains(term.ToLower()) && r.ZoneID == loginname.ZoneID)
                              select new { r.CPName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from r in db.ChannelPartners
                              where (r.CPName.ToLower().Contains(term.ToLower()) && r.CPID != loginname.CPID)
                              select new { r.CPName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewData["CPName"] = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPID", "CPName");
            return View();
        }
        [HttpPost]
        public ActionResult Create(TargetSettings ts, string CPName, string tmonth)
        {
            int id = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).SingleOrDefault();
            var cpid = Convert.ToInt32(Session["logincpid"]);
            ts.CPID = id;
            ts.TargetMonth = tmonth;
            ts.CreatedBy= Convert.ToString(cpid);
            ts.CreatedOn = System.DateTime.Now;
            ts.Status = 0;
            db.TargetSettings.Add(ts);
            db.SaveChanges();
            return RedirectToAction("Index", "TargetSetting", null);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var list = db.TargetSettings.Where(m => m.Status == 0).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            TargetSettings ts = db.TargetSettings.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            var cpname = db.ChannelPartners.Where(m => m.CPID == ts.CPID).Select(m => m.CPName).SingleOrDefault();
            ViewBag.cpname = cpname;
            ViewBag.tmonth = ts.TargetMonth;
            return View(ts);
        }

        [HttpPost]        
        public ActionResult Edit(TargetSettings ts,string CPName,string tmonth)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            if (ts.TSID != 0)
            {
                int id = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).SingleOrDefault();                
                ts.CPID = id;
                ts.TargetMonth = tmonth;                
                ts.ModifiedBy = Convert.ToString(userid);
                ts.ModifiedOn = System.DateTime.Now;
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Modified Successfully!!!!";
                return RedirectToAction("Index");
            }            
            return View("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            TargetSettings ts = db.TargetSettings.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            return View(ts);
        }
        
        [HttpPost]       
        public ActionResult Delete(TargetSettings ts1, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                TargetSettings ts = db.TargetSettings.Find(id);
                ts.Status = 1;
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Deleted Successfully!!!!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
