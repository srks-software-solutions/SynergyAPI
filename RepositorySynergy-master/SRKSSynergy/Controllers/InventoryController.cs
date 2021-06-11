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
    [Authorize]
    public class InventoryController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var channame = db.ChannelPartners.Where(m => m.CPName == term).Select(m => m.CPID).SingleOrDefault();

            var result = (from r in db.ChannelPartners
                          where r.CPName.ToLower().Contains(term.ToLower())
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Inventory/
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult MinSpareQuantity(int ProductModelID = 0)
        {
            if (ProductModelID != 0)
            {
                ViewBag.Prod = ProductModelID;
                var minstoc = db.MinSpareEquipQuantity.Where(m => m.ProductModelID == ProductModelID).ToList();
                var minstlastdt = db.MinSpareEquipQuantity.Where(m => m.ProductModelID == ProductModelID).Where(m => m.ProductModelSparesID == 1).Single();
                if (minstlastdt.ModifiedOn != null)
                {
                    ViewBag.last = Convert.ToDateTime(minstlastdt.ModifiedOn).ToString("dd-MM-yyyy");
                }
                else
                {
                    ViewBag.last = "Not updated";
                }
                ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View(minstoc);
            }
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m =>m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MinSpareQuantity(IEnumerable<MinSpareEquipQuantity> minspqt, int ProductModelID = 0)
        {
            if (ProductModelID != 0)
            {
                foreach (var min in minspqt)
                {
                    db.Entry(min).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["yesavail"] = "Stock saved successfully!!!!";
                }
            }
            else
            { 
                TempData["NOModel"] = "Please select Equipment before submitting!!!!";
            }
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            return RedirectToAction("MinSpareQuantity");
        }

        //
        //Get: /Inventory/
        public ActionResult ChannelAvailQuantity(string availmonth)
        {
            if (availmonth != null && availmonth != "")
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                ViewBag.month = availmonth;
                
                var avail = db.AvailSpareQuantity.Where(m => m.CPID == loginname.CPID).Where(m => m.month == availmonth).ToList();
                if (avail.Count == 0)
                {
                    avail = db.AvailSpareQuantity.Where(m => m.CPID == loginname.CPID).Where(m => m.IsOld == 0).ToList();
                    ViewBag.last = "Not updated";
                }
                else
                {
                    var minstlastdt = db.AvailSpareQuantity.Where(m => m.CPID == loginname.CPID).Where(m => m.month == availmonth).Where(m => m.ProductModelSparesID == 1).Single();
                    if (minstlastdt.ModifiedOn != null)
                    {
                        ViewBag.last = Convert.ToDateTime(minstlastdt.ModifiedOn).ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        ViewBag.last = "Not updated";
                    }
                }
                return View(avail);
            }
            return View();
        }

        //
        //POST: /Inventory/
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChannelAvailQuantity(IEnumerable<AvailSpareQuantity> availsp, string availmonth)
        {
            if (availmonth != null && availmonth != "")
            {
                //var avmon = db.AvailSpareQuantity.Where(m => m.month == availmonth).ToList();
                //int count = avmon.Count();
                //if (count != 0)
                //{
                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

                //    var avail = db.AvailSpareQuantity.Where(m => m.CPID == loginname.CPID).Where(m => m.month == availmonth).ToList();
                //    TempData["avail"] = "Data has already been entered for selected month. Please select other month and enter data!!!!";
                //    return RedirectToAction("ChannelAvailQuantity");
                //}
                //else
                //{
                    var avail = db.AvailSpareQuantity.Where(m => m.CPID == loginname.CPID).Where(m => m.month == availmonth).Where(m => m.ProductModelSparesID == 1).Count();
                    if (avail == 0)
                    {
                        foreach (var av in availsp)
                        {
                            av.IsOld = 1;
                            av.month = availmonth;
                            db.AvailSpareQuantity.Add(av);
                            db.SaveChanges();
                            TempData["yesavail"] = "Available Stock saved successfully!!!!";
                        }
                    }
                    else
                    {
                        foreach (var av in availsp)
                        {
                            av.IsOld = 1;
                            av.month = availmonth;
                            //db.AvailSpareQuantity.Add(av);
                            //db.SaveChanges();
                            db.Entry(av).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["yesavail"] = "Available Stock updated successfully!!!!";
                        }
                    }

                 //}
            }
            return RedirectToAction("ChannelAvailQuantity");
        }

        //
        //GET: /InventoryMonthRep/
        public ActionResult InventoryMonthRep()
        {
            return View();
        }

        //
        //POST: /InventoryMonthRep/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InventoryMonthRep(String cpnam)
        {
            int cpid = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).FirstOrDefault();
            if (cpid != 0)
            {
                var availspin = db.AvailSpareQuantity.Where(m => m.CPID == cpid).ToList();
                return View(availspin);
            }
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
