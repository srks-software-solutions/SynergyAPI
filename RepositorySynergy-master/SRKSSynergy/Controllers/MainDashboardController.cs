using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SRKSSynergy.Controllers
{
    public class MainDashboardController : Controller
    {
        //
        // GET: /MainDashboard/

        public ActionResult MainDashboard()
        {
            return View();
        }

        //
        // GET: /EquipmentSalesManagement/

        public ActionResult EquipmentSalesMang()
        {
            return View();
        }

        //
        // GET: /SalesOrderProcess/

        public ActionResult SalesOrderProcess()
        {
            return View();
        }

        //
        // GET: /MachineDetails/

        public ActionResult MachineDetails()
        {
            return View();
        }

        //
        // GET: /Spares&ServiceManagement/

        public ActionResult SparesServiceMang()
        {
            return View();
        }

        //
        // GET: /Spares&ServiceManagement/

        public ActionResult AnalyticalReports()
        {
            return View();
        }


        public ActionResult SparesStockTracker()
        {
            return View();
        }

        public ActionResult Outward()
        {
            return View();
        }

    }
}
