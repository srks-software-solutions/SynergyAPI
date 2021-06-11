using SRKSSynergy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SRKSSynergy.Controllers
{
    public class OCMPassageStickerController : Controller
    {
        //
        // GET: /OCMPassageSticker/
        SRKS_Synergy db = new SRKS_Synergy();
        public ActionResult Index()
        {
            var list = db.OCMPassageSticker.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }
        public ActionResult Create()
        {
            ViewData["MasterProductID"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(OCMPassageSticker ps)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            ps.IsDeleted = 0;
            ps.CreatedBy = Convert.ToString(cpid);
            ps.CreatedOn = System.DateTime.Now;
            db.OCMPassageSticker.Add(ps);
            db.SaveChanges();

            return RedirectToAction("Index", "OCMPassageSticker", null);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            OCMPassageSticker ps = db.OCMPassageSticker.Find(id);
            if (ps == null)
            {
                return HttpNotFound();
            }
            ViewBag.MasterProductID = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName", ps.MasterProductID);
            return View(ps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OCMPassageSticker ps)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            ps.ModifiedOn = System.DateTime.Now;
            ps.ModifiedBy = Convert.ToString(cpid);
            db.Entry(ps).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "OCMPassageSticker", null);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            OCMPassageSticker ts = db.OCMPassageSticker.Find(id);
            if (ts == null)
            {
                return HttpNotFound();
            }
            return View(ts);
        }
    }
}
