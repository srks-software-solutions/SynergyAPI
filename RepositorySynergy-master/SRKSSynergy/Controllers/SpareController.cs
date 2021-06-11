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
    public class SpareController : Controller
    {
        
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json
        public JsonResult Autocomplete(string term)
        {
            var result = (from r in db.ProductModelSpare
                          where r.ProductModelSparesName.ToLower().Contains(term.ToLower())
                          select new { r.ProductModelSparesName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Spare/

        const int pageSize = 10;

        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string sparename = null)
        {
            //Paging and Sorting //
            IEnumerable<ProductModelSpare> productsname = db.ProductModelSpare.Where(
                    p => sparename == null
                    || p.ProductModelSparesName.Contains(sparename)).Where(p=>p.IsDeleted==0);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesID) : productsname.OrderByDescending(p => p.ProductModelSparesID);
                    break;

                case 2:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesName) : productsname.OrderByDescending(p => p.ProductModelSparesName);
                    break;

                case 3:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesDesc) : productsname.OrderByDescending(p => p.ProductModelSparesDesc);
                    break;

                case 4:
                    productsname = isAsc ? productsname.OrderBy(p => p.QuoteVaildTill) : productsname.OrderByDescending(p => p.QuoteVaildTill);
                    break;

                case 5:
                    productsname = isAsc ? productsname.OrderBy(p => p.DeliveryTime) : productsname.OrderByDescending(p => p.DeliveryTime);
                    break;

                case 6:
                    productsname = isAsc ? productsname.OrderBy(p => p.AgentPrice) : productsname.OrderByDescending(p => p.AgentPrice);
                    break;

                case 7:
                    productsname = isAsc ? productsname.OrderBy(p => p.CustomerPrice) : productsname.OrderByDescending(p => p.CustomerPrice);
                    break;
                default:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesID) : productsname.OrderByDescending(p => p.ProductModelSparesID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)productsname.Count() / pageSize);

            productsname = productsname
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.Search = sparename;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (sparename != null)
                ViewBag.IsSearch = true;
            return View(productsname);
        }

        //[HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductModelSpare pdtspare)
        {
            var duplicate = (from s in db.ProductModelSpare
                             where s.ProductModelSparesName == pdtspare.ProductModelSparesName && s.ProductModelSparesDesc == pdtspare.ProductModelSparesDesc                          select s).ToList();

            if (ModelState.IsValid)
            {
                if (duplicate.Count > 0)
                {
                    ViewBag.Duplicate = "Spare Part Model Name already exists for this Commodity";
                }
                else
                {
                    String price1 = pdtspare.AgentPrice;
                    String[] agtpric = price1.Split(',');
                    string exactagtpr = null;
                    foreach (String pri in agtpric)
                    {
                        exactagtpr += pri;
                    }
                    String price2 = pdtspare.CustomerPrice;
                    String[] cuspric = price2.Split(',');
                    string exactcuspr = null;
                    foreach (String pri in cuspric)
                    {
                        exactcuspr += pri;
                    }
                    String NameDesc = pdtspare.ProductModelSparesName + '~' + pdtspare.ProductModelSparesDesc;
                    pdtspare.AgentPrice = exactagtpr;
                    pdtspare.CustomerPrice = exactcuspr;
                    pdtspare.ProductSpareNameDesc = NameDesc;
                    db.ProductModelSpare.Add(pdtspare);
                    db.SaveChanges();

                    int pid = pdtspare.ProductModelSparesID;

                    //Adding spare into Available spare Quantity when spare is added
                    int cpcount = db.ChannelPartners.Count();
                        for (int i = 1; i <= cpcount; i++)
                        {
                            int cpid = i;
                            AvailSpareQuantity avl = new AvailSpareQuantity();
                            avl.CPID = cpid;
                            avl.ProductModelSparesID = pid;
                            avl.IsOld = 0;
                            avl.MinCpStock = 0;
                            avl.AvailableStock = 0;
                            db.AvailSpareQuantity.Add(avl);
                            db.SaveChanges();
                        }
                    //Adding spare and equipment when spare is added
                    int prdcount = db.ProductModel.Count();
                    for (int k = 1; k <= prdcount; k++)
                    {
                        int prdid = k;
                        MinSpareEquipQuantity msq = new MinSpareEquipQuantity();
                        msq.Minimumstock = 0;
                        msq.ProductModelID = prdid;
                        msq.ProductModelSparesID = pid;
                        msq.IsOld = 0;
                        db.MinSpareEquipQuantity.Add(msq);
                        db.SaveChanges();
                    }
                        return RedirectToAction("Index");
                }
            }
            return View(pdtspare);
        }

        //
        //GET: Spare/Edit/
        public ActionResult Edit(int id = 0)
        {
            ProductModelSpare productmodel = db.ProductModelSpare.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }

        //
        // POST: /Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductModelSpare productmodel)
        {
            //var duplicate = (from s in db.ProductModelSpare
            //                 where s.ProductModelSparesName == productmodel.ProductModelSparesName && s.ProductModelSparesDesc == productmodel.ProductModelSparesDesc
            //                 select s).ToList();

            if (ModelState.IsValid)
            {
                //if (duplicate.Count > 0)
                //{
                //    ViewBag.Duplicate = "Equipment Name already exists for this Commodity";
                //}
                //else
                //{
                    String price1 = productmodel.AgentPrice;
                    String[] agtpric = price1.Split(',');
                    string exactagtpr = null;
                    foreach (String pri in agtpric)
                    {
                        exactagtpr += pri;
                    }
                    String price2 = productmodel.CustomerPrice;
                    String[] cuspric = price2.Split(',');
                    string exactcuspr = null;
                    foreach (String pri in cuspric)
                    {
                        exactcuspr += pri;
                    }
                    String NameDesc = productmodel.ProductModelSparesName + '~' + productmodel.ProductModelSparesDesc;
                    productmodel.AgentPrice = exactagtpr;
                    productmodel.CustomerPrice = exactcuspr;
                    productmodel.ProductSpareNameDesc = NameDesc;
                    db.Entry(productmodel).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                //}
            }
            return View(productmodel);
        }

        [HttpGet]
        public ActionResult Delete(int ProductModelSparesID=0)
        {
            var obj= db.ProductModelSpare.Find(ProductModelSparesID);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult Delete(ProductModelSpare ProductModelSpare, int ProductModelSparesID)
        {
            if (Request.Form["Yes"] != null)
            {
                var obj = db.ProductModelSpare.Find(ProductModelSparesID);
                obj.IsDeleted = 1;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
