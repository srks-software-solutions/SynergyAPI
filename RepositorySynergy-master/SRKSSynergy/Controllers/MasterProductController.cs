using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MasterProductController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /MasterProduct/
        public ActionResult Index()
        {
            return View(db.MasterProducts.ToList());
        }


        //
        //GET: /MasterProduct/Add
        public ActionResult AddSection()
        {
            return View();
        }

        //
        //POST: /MasterProduct/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSection(MasterProducts mast)
        {
            if (mast.MasterProductName != null)
            {
                db.MasterProducts.Add(mast);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mast);
        }

        //
        // GET: /Products/Edit/5

        public ActionResult EditSection(int id = 0)
        {

            MasterProducts mastpr = db.MasterProducts.Find(id);
            if (mastpr == null)
            {
                return HttpNotFound();
            }
            return View(mastpr);
        }

        //
        // POST: /Products/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSection(MasterProducts products)
        {
            if (products.MasterProductName != null)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(products);
        }

        //
        //Discontinue Section
        public ActionResult DiscontinueSection(int id)
        {
            var deac = db.MasterProducts.Where(m => m.MasterProductID == id).Where(m => m.IsDeleted == 0).Single();
            return View(deac);
        }

        //
        //Post: Discontinue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DiscontinueSection(MasterProducts prdmd)
        {
            if (Request.Form["Yes"] != null)
            {
                prdmd.IsDeleted = 1;
                db.Entry(prdmd).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


    }
}
