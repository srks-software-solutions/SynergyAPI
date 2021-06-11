using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class OCMDriveController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        public ActionResult Create()
        {
            ViewData["MasterProductID"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //ViewData["ProductModelID1"] 
            return View();
        }

        [HttpPost]
        public ActionResult Create(OCMDrive motor)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            motor.IsDeleted = 0;
            motor.CreatedBy = Convert.ToString(cpid);
            motor.CreatedOn = System.DateTime.Now;
            db.OCMDrive.Add(motor);
            db.SaveChanges();

            return RedirectToAction("Index", "OCMDrive", null);
        }

        public ActionResult Index()
        {
            var list = db.OCMDrive.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            OCMDrive motor = db.OCMDrive.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }
            ViewBag.MasterProductID = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName", motor.MasterProductID);
            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OCMDrive motor)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            motor.ModifiedOn = System.DateTime.Now;
            motor.ModifiedBy = Convert.ToString(cpid);
            db.Entry(motor).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "OCMDrive", null);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            OCMDrive ts = db.OCMDrive.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            return View(ts);
        }

        [HttpPost]
        public ActionResult Delete(OCMDrive am1, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                OCMDrive am = db.OCMDrive.Find(id);
                am.IsDeleted = 1;
                db.Entry(am).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Deleted Successfully!!!!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "OCMDrive", null);
        }

    }
}
