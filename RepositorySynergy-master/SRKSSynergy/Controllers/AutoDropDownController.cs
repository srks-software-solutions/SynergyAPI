using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class AutoDropDownController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /AutoDropDown/

        [HttpGet]
        public JsonResult GetProductModelDetails(int id)
        {
            var selectedRow = (from t in db.ProductModel where t.ProductModelID == id select new { t.UnitPrice, t.ProductModelDesc, t.ProductModelExclusion }).SingleOrDefault();

            var jsonData = new
            {
                unitprice = selectedRow.UnitPrice,
                Desc = selectedRow.ProductModelDesc,
                Exclusion = selectedRow.ProductModelExclusion
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}
