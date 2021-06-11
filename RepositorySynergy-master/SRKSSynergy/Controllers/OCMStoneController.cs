using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class OCMStoneController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        public ActionResult Create()
        {
            ViewData["MasterProductID"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(OCMStone motor)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            motor.IsDeleted = 0;
            motor.CreatedBy = Convert.ToString(cpid);
            motor.CreatedOn = System.DateTime.Now;
            db.OCMStone.Add(motor);
            db.SaveChanges();

            return RedirectToAction("Index", "OCMStone", null);
        }

        public ActionResult Index()
        {
            var list = db.OCMStone.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            OCMStone motor = db.OCMStone.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }
            ViewBag.MasterProductID = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName", motor.MasterProductID);
            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OCMStone motor)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            motor.ModifiedOn = System.DateTime.Now;
            motor.ModifiedBy = Convert.ToString(cpid);
            db.Entry(motor).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "OCMStone", null);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            OCMStone ts = db.OCMStone.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            return View(ts);
        }

        [HttpPost]
        public ActionResult Delete(OCMStone am1, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                OCMStone am = db.OCMStone.Find(id);
                am.IsDeleted = 1;
                db.Entry(am).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Deleted Successfully!!!!";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "OCMStone", null);
        }
    }
}
