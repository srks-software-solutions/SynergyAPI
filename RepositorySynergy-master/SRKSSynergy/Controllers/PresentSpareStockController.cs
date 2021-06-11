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
    public class PresentSpareStockController : Controller
    {

        private SRKS_Synergy db = new SRKS_Synergy();

        const int pageSize = 100;
        public ActionResult PresentSpareStock(int page = 1, int sortBy = 1, bool isAsc = true, string equipname = null)
        {
            IEnumerable<ProductModelSpare> productsname = db.ProductModelSpare.Where(m => m.IsDeleted == 0);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesName) : productsname.OrderByDescending(p => p.ProductModelSparesName);
                    break;

                case 2:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesDesc) : productsname.OrderByDescending(p => p.ProductModelSparesDesc);
                    break;                              

                case 3:
                    productsname = isAsc ? productsname.OrderBy(p => p.BuhlerPresentStock) : productsname.OrderByDescending(p => p.BuhlerPresentStock);
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

            ViewBag.Search = equipname;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (equipname != null)
                ViewBag.IsSearch = true;
            return View(productsname);

        }
    }
}

    

