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
    public class WarrantyController : Controller
    {

        private SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /Warranty/
        [Authorize(Roles = "Administrator")]
        public ActionResult Warranty()
        {
            var war = db.Warranty.ToList();
            return View(war);
        }

         protected override void Dispose(bool disposing)
         {
             db.Dispose();
             base.Dispose(disposing);
         }


    }
}
