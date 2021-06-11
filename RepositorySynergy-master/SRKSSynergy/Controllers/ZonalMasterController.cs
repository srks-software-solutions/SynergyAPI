using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;
using System.Data.Entity;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Web.UI;
using Common.Logging;

namespace SRKSSynergy.Controllers
{
    public class ZonalMasterController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /ZonalMaster/

        public ActionResult Index()
        {
            return View(db.Zone.Where(m =>m.IsDeactive ==0).ToList());
        }

        public ActionResult Create()
        {
            ViewData["ContactName"] = new SelectList(db.UserLogins.Where(m => m.IsDeactivate == 0).Where(m => m.IsZoneManager == 1), "ContactName", "ContactName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Zone zm)
        {
            var duplicate = (from s in db.Zone
                             where s.ZoneName == zm.ZoneName && s.ZonalMangerName == zm.ZonalMangerName && zm.IsDeactive == 0
                             select s).ToList();

            if (ModelState.IsValid)
            {
                if (duplicate.Count > 0)
                {
                    ViewBag.Duplicate = "Channel Parter for this Zone and Zonal Manager has already been Added!!!!";
                }
                else
                {
                    db.Zone.Add(zm);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewData["ContactName"] = new SelectList(db.UserLogins.Where(m => m.IsDeactivate == 0).Where(m => m.IsZoneManager == 1), "ContactName", "ContactName");
            return View(zm);
        }

        // GET: /Equipment/Edit/5
        public ActionResult Edit(int id = 0)
        {
            Zone zn = db.Zone.Find(id);
            if (zn == null)
            {
                return HttpNotFound();
            }
            ViewData["ContactName"] = new SelectList(db.UserLogins.Where(m => m.IsDeactivate == 0).Where(m => m.IsZoneManager == 1), "ContactName", "ContactName");
            return View(zn);
        }

        //
        // POST: /Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Zone productmodel)
        {
            if (productmodel.ZonalMangerName != null)
            {

                db.Entry(productmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewData["ContactName"] = new SelectList(db.UserLogins.Where(m => m.IsDeactivate == 0).Where(m => m.IsZoneManager == 1), "ContactName", "ContactName");
            return View(productmodel);
        }

    }
}
