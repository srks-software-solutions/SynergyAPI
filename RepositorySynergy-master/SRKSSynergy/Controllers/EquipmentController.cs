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
    public class EquipmentController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json
        public JsonResult Autocomplete(string term)
        {

            var result = (from r in db.ProductModel
                          where r.ProductModelName.ToLower().Contains(term.ToLower())
                          select new { r.ProductModelName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Equipment/
       // const int pageSize = 10;

        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string equipname = null)
        {
            //Paging and Sorting //
            IEnumerable<ProductModel> productsname = db.ProductModel.Where(
                    p => equipname == null
                    || p.ProductModelName.Contains(equipname)).Where(m => m.IsDeleted == 0);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelID) : productsname.OrderByDescending(p => p.ProductModelID);
                    break;

                case 2:
                    productsname = isAsc ? productsname.OrderBy(p => p.Products.ProductName) : productsname.OrderByDescending(p => p.Products.ProductName);
                    break;

                case 3:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelName) : productsname.OrderByDescending(p => p.ProductModelName);
                    break;

                case 4:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelDesc) : productsname.OrderByDescending(p => p.ProductModelDesc);
                    break;

                case 5:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelExclusion) : productsname.OrderByDescending(p => p.ProductModelExclusion);
                    break;

                case 6:
                    productsname = isAsc ? productsname.OrderBy(p => p.QuoteVaildTill) : productsname.OrderByDescending(p => p.QuoteVaildTill);
                    break;

                case 7:
                    productsname = isAsc ? productsname.OrderBy(p => p.DeliveryTime) : productsname.OrderByDescending(p => p.DeliveryTime);
                    break;

                case 8:
                    productsname = isAsc ? productsname.OrderBy(p => p.UnitPrice) : productsname.OrderByDescending(p => p.UnitPrice);
                    break;
                default:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelID) : productsname.OrderByDescending(p => p.ProductModelID);
                    break;
            }
            #endregion

         //   ViewBag.TotalPages = (int)Math.Ceiling((double)productsname.Count() / pageSize);

            //productsname = productsname
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize;

            ViewBag.Search = equipname;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (equipname != null)
                ViewBag.IsSearch = true;
            return View(productsname);
            //end of paging and sorting//
            //return View(db.ProductModel.ToList());
        }

        //
        // GET: /Equipment/Details/5
        public ActionResult Details(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }


        //[HttpGet]
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductModel productmodel)
        {
            var duplicate = (from s in db.ProductModel
                             where s.ProductModelName == productmodel.ProductModelName && s.ProductID == productmodel.ProductID
                             select s).ToList();

            if (ModelState.IsValid)
            {
                if (duplicate.Count > 0)
                {
                    ViewBag.Duplicate = "Equipment Name already exists for this Commodity";
                }
                else
                { 
                    db.ProductModel.Add(productmodel);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productmodel.ProductID);
            return View(productmodel);
        }
        
        //
        // GET: /Equipment/Edit/5
        public ActionResult Edit(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productmodel.ProductID);
            return View(productmodel);
        }

        //
        // POST: /Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductModel productmodel)
        {
            if (productmodel.ProductModelName!= null)
            {

                db.Entry(productmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productmodel.ProductID);
            return View(productmodel);
        }

        //
        // GET: /Equipment/Delete/5
        public ActionResult Delete(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }

        //
        // POST: /Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            db.ProductModel.Remove(productmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        //Discontinue Equipment
        public ActionResult Discontinue(int id)
        {
            var deac = db.ProductModel.Where(m => m.ProductModelID == id).Where(m => m.IsDeleted == 0).Single();
            return View(deac);
        }

        //
        //Post: Discontinue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discontinue(ProductModel prdmd)
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