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
    public class TargetSettingsLeadController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

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
            return View();
        }

        [HttpPost]
        public ActionResult Create(TargetSettingsLead tsl, string CPName = null)
        {
            //var cpid = Convert.ToInt32(Session["logincpid"]);
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int monthid = Convert.ToInt32(monthtomonthid(tsl.Month));

            if (CPName == null || CPName == "")
            {
                var cpidlist = db.ChannelPartners.ToList();
                foreach (var id in cpidlist)
                {
                    int count = db.TargetSettingsLead.Where(m => m.CPID == id.CPID).Where(m => m.Year == tsl.Year).Where(m => m.Month == tsl.Month).Where(m => m.MachineType == tsl.MachineType).Where(m => m.TargetType == tsl.TargetType).Count();
                    if (count == 0)
                    {
                        tsl.CPID = id.CPID;
                        tsl.MonthId = monthid;
                        tsl.IsDeleted = 0;
                        tsl.CreatedOn = System.DateTime.Now;
                        db.TargetSettingsLead.Add(tsl);
                        db.SaveChanges();

                        TempData["success"] = "Data Saved Successfully...!";
                    }
                    else
                    {
                        int tslid = db.TargetSettingsLead.Where(m => m.CPID == id.CPID).Where(m => m.Year == tsl.Year).Where(m => m.Month == tsl.Month).Where(m => m.MachineType == tsl.MachineType).Where(m => m.TargetType == tsl.TargetType).Select(m => m.TSID).SingleOrDefault();
                        TargetSettingsLead ts = db.TargetSettingsLead.Find(tslid);
                        ts.Targets = tsl.Targets;
                        db.Entry(tsl).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["success"] = "Data Modified With Previous Value...!";
                    }
                }
            }
            else
            {
                var cpid = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).SingleOrDefault();

                int count = db.TargetSettingsLead.Where(m => m.CPID == cpid).Where(m => m.Year == tsl.Year).Where(m => m.Month == tsl.Month).Where(m => m.MachineType == tsl.MachineType).Where(m => m.TargetType == tsl.TargetType).Count();
                if (count == 0)
                {
                    tsl.CPID = cpid;
                    tsl.MonthId = monthid;
                    tsl.IsDeleted = 0;
                    tsl.CreatedOn = System.DateTime.Now;
                    db.TargetSettingsLead.Add(tsl);
                    db.SaveChanges();

                    TempData["success"] = "Data Saved Successfully...!";
                }
                else
                {
                    int tslid = db.TargetSettingsLead.Where(m => m.CPID == cpid).Where(m => m.Year == tsl.Year).Where(m => m.Month == tsl.Month).Where(m => m.MachineType == tsl.MachineType).Where(m => m.TargetType == tsl.TargetType).Select(m => m.TSID).SingleOrDefault();
                    TargetSettingsLead ts = db.TargetSettingsLead.Find(tslid);
                    ts.Targets = tsl.Targets;
                    db.Entry(ts).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["success"] = "Data Modified With Previous Value...!";
                }
            }
            return RedirectToAction("Index", "TargetSettingsLead", null);
        }

        public ActionResult Index()
        {
            var list = db.TargetSettingsLead.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        //for to date
        public int monthtomonthid(string month)
        {
            int mondat = 0;

            switch (month)
            {
                case "January": mondat = 1;
                    break;
                case "February": mondat = 2;
                    break;
                case "March": mondat = 3;
                    break;
                case "April": mondat = 4;
                    break;
                case "May": mondat = 5;
                    break;
                case "June": mondat = 6;
                    break;
                case "July": mondat = 7;
                    break;
                case "August": mondat = 8;
                    break;
                case "September": mondat = 9;
                    break;
                case "October": mondat = 10;
                    break;
                case "November": mondat = 11;
                    break;
                case "December": mondat = 12;
                    break;
            }
            return mondat;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            TargetSettingsLead tsl = db.TargetSettingsLead.Find(id);
            if (tsl == null)
            {
                return HttpNotFound();
            }
            var cpname = db.ChannelPartners.Where(m => m.CPID == tsl.CPID).Select(m => m.CPName).SingleOrDefault();
            ViewBag.cpname = cpname;
            return View(tsl);
        }

        [HttpPost]
        public ActionResult Edit(TargetSettingsLead tsl, string CPName = null)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            int monthid = Convert.ToInt32(monthtomonthid(tsl.Month));
            if (tsl.TSID != 0)
            {
                int id = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).SingleOrDefault();

                tsl.ModifiedBy = Convert.ToString(userid);
                tsl.ModifiedOn = System.DateTime.Now;
                db.Entry(tsl).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Record Modified Successfully!!!!";
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            TargetSettingsLead tsl = db.TargetSettingsLead.Find(id);
            if (tsl == null)
            {
                return HttpNotFound();
            }
            return View(tsl);
        }

        [HttpPost]
        public ActionResult Delete(TargetSettingsLead ts1, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                TargetSettingsLead ts = db.TargetSettingsLead.Find(id);
                ts.IsDeleted = 1;
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Record Deleted Successfully!!!!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
