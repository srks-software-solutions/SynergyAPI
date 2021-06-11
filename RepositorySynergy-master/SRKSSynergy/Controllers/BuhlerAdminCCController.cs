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
    public class BuhlerAdminCCController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        public ActionResult Create()
        {
            ViewBag.ZoneID = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
            return View();
        }
        [HttpPost]
        public ActionResult Create(BuhlerAdminCC mail, string cctype,int zoneid=0)
        {

            //check if mailid is already present in table and send message if present.

            int mailIDExists = 0;
            string newmaild = mail.EmailId;
            List<string> mailids = db.BuhlerAdminCC.Where(m => m.BAID > 0).Select(m => m.EmailId).ToList();

            foreach (var i in mailids)
            {
                string emailid = i;
                if(emailid == newmaild)
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

                if (cctype == "Zone")
                {
                    mail.ZonalManagerID = zoneid;
                }
                else { mail.ZonalManagerID = 0; }

                mail.CreatedOn = System.DateTime.Now;

                if (cpid != 0)
                {
                    mail.CreatedBy = Convert.ToString(cpid);
                }
                else
                {
                    mail.CreatedBy = Convert.ToString(0);
                }
                mail.Type = cctype;
                mail.IsDeleted = 0;
                db.BuhlerAdminCC.Add(mail);
                db.SaveChanges();

                return RedirectToAction("Index", "BuhlerAdminCC", null);
            }
            return RedirectToAction("Create", "BuhlerAdminCC", null);
        }

        public ActionResult Index()
        {
            var list = db.BuhlerAdminCC.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        public ActionResult Edit(int id)
        {
            BuhlerAdminCC BuhlerAdminCC = db.BuhlerAdminCC.Find(id);
            if (BuhlerAdminCC == null)
            {
                return HttpNotFound();
            }
            ViewBag.ZoneID = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
            return View(BuhlerAdminCC);
        }
     
        [HttpPost]
        public ActionResult Edit(BuhlerAdminCC mail, string cctype, int zoneid=0)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);
             mail.Type = cctype;
            if (cctype == "Zone")
            { mail.ZonalManagerID = zoneid; }
            else { mail.ZonalManagerID = 0; }

            mail.ModifiedOn = System.DateTime.Now;
            if (cpid != 0)
            {
                mail.ModifiedBy = Convert.ToString(cpid);
            }
            else
            {
                mail.ModifiedBy = Convert.ToString(0);
            }
            db.Entry(mail).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "BuhlerAdminCC", null);
        }

        public ActionResult Delete(int id)
        {
            BuhlerAdminCC BuhlerAdminCC = db.BuhlerAdminCC.Find(id);
            if (BuhlerAdminCC == null)
            {
                return HttpNotFound();
            }
            return View(BuhlerAdminCC);
        }

        [HttpPost]
        public ActionResult Delete(BuhlerAdminCC mail, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                BuhlerAdminCC BuhlerAdminCC = db.BuhlerAdminCC.Find(id);
                BuhlerAdminCC.IsDeleted = 1;
                db.Entry(BuhlerAdminCC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
