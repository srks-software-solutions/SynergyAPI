using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SRKSSynergy.Controllers
{
     [Authorize(Roles = "Administrator")]
    public class ConfigurationController : Controller
    {
        //
        // GET: /Configuration/

        public ActionResult Configuration()
        {
            return View();
        }

        public ActionResult Setting1()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }
        public ActionResult MachineAndSpare()
        {
            return View();
        }
        public ActionResult OCMModule()
        {
            return View();
        }

        public ActionResult OCMSetting()
        {
            return View();
        }

    }
}
