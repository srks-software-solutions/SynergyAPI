using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;
using SRKSSynergy.Controllers;
using System.Data.Entity;

namespace SRKSSynergy.Controllers
{
    public class CPEmailIDController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var result = (from r in db.ChannelPartners
                          where (r.CPName.Contains(term))
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            ViewBag.CPID = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPID", "CPName");
            //ViewData["CPName"] = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPName", "CPName");
            //ViewData["ZonalMangerName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZonalMangerName");
            return View();
        }
        [HttpPost]
        public ActionResult Create(CPMailIdTO mail, string cpname)
        {
            //check if mailid is already present in table and send message if present.

            int mailIDExists = 0;
            string newmaild = mail.EmailId;
            List<string> mailids = db.CPMailIdTO.Where(m => m.CEID > 0).Select(m => m.EmailId).ToList();

            foreach (var i in mailids)
            {
                string emailid = i;
                if (emailid == newmaild)
                {
                    mailIDExists = 1;
                    break;
                }
            }
            if (mailIDExists == 1)
            {
                Session["tick"] = "EmailID exists. Please re-enter details.";
            }
            if (mailIDExists == 0)
            {
                var cpid = Convert.ToInt32(Session["logincpid"]);
                mail.CreatedOn = System.DateTime.Now;
                if (cpid != 0)
                {
                    mail.CreatedBy = Convert.ToString(cpid);
                }
                else
                {
                    mail.CreatedBy = Convert.ToString(0);
                }

                var dbcpid = db.ChannelPartners.Where(m => m.CPName == cpname).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
                mail.CPID = dbcpid.CPID;
                mail.ZonalManagerID = dbcpid.ZoneID;

                mail.IsDeleted = 0;
                db.CPMailIdTO.Add(mail);
                db.SaveChanges();
                return RedirectToAction("Index", "CPEmailID", null);
            }
            return RedirectToAction("Create", "CPEmailID", null);
        }

        public ActionResult Index()
        {
            var list = db.CPMailIdTO.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        public ActionResult Edit(int id)
        {
            CPMailIdTO CPMailIdTO = db.CPMailIdTO.Find(id);
            if (CPMailIdTO == null)
            {
                return HttpNotFound();
            }
            int cpid = Convert.ToInt32(CPMailIdTO.CPID);
            string cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
            ViewBag.cpname = cpname;

            ViewBag.CPqqName = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPName", "CPName", CPMailIdTO.CPID);
            ViewBag.ZonalMangerName = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZonalMangerName", CPMailIdTO.ZonalManagerID);

            return View(CPMailIdTO);
        }
        [HttpPost]
        public ActionResult Edit(CPMailIdTO mail, string cpname)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            mail.ModifiedOn = System.DateTime.Now;
            if (cpid != 0)
            {
                mail.ModifiedBy = Convert.ToString(cpid);
            }
            else
            {
                mail.ModifiedBy = Convert.ToString(0);
            }

            var dbcpid = db.ChannelPartners.Where(m => m.CPName == cpname).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
            mail.CPID = dbcpid.CPID;
            mail.ZonalManagerID = dbcpid.ZoneID;

            db.Entry(mail).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "CPEmailID", null);
        }

        public ActionResult Delete(int id)
        {
            CPMailIdTO CPMailIdTO = db.CPMailIdTO.Find(id);
            if (CPMailIdTO == null)
            {
                return HttpNotFound();
            }
            return View(CPMailIdTO);
        }

        [HttpPost]
        public ActionResult Delete(CPMailIdTO mail, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                CPMailIdTO CPMailIdTO = db.CPMailIdTO.Find(id);
                CPMailIdTO.IsDeleted = 1;
                db.Entry(CPMailIdTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
