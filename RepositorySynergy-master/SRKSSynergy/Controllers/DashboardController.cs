using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SRKSSynergy.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        public ActionResult Dashboardpage()
        {
            return View();
        }

    }
}
