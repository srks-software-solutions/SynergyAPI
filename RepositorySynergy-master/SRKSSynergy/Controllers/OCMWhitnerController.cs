using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class OCMWhitnerController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        //fetching motor details
        public JsonResult GetMotor(string motorq, string motortype)
        {
            string result = null;
            if (motorq == "With-Motor")
            {
                var selectedRow = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m => m.MotorMotorType == "Foot Motor Only"), "MotorDescription", "MotorDescription");
                return Json(selectedRow, JsonRequestBehavior.AllowGet);
            }
            else if (motorq == "WithOut-Motor")
            {
                if (motortype == "Foot Motor Only")
                {
                    var selectedRow = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m => m.MotorMotorType == "Foot Motor Only"), "MotorDescription", "MotorDescription");
                    return Json(selectedRow, JsonRequestBehavior.AllowGet);
                }
                else if (motortype == "Flanged Motor Only")
                {
                    var selectedRow = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m => m.MotorMotorType == "Flanged Motor Only"), "MotorDescription", "MotorDescription");
                    return Json(selectedRow, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //fetching motor details
        public JsonResult GetDrive(string motorq, string motortype)
        {
            string result = null;
            if (motorq == "With-Motor")
            {
                var selectedRow = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.DriveMotorType == "Foot Motor Only"), "DriveDescription", "DriveDescription");
                return Json(selectedRow, JsonRequestBehavior.AllowGet);
            }
            else if (motorq == "WithOut-Motor")
            {
                if (motortype == "Foot Motor Only")
                {
                    var selectedRow = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.DriveMotorType == "Foot Motor Only"), "DriveDescription", "DriveDescription");
                    return Json(selectedRow, JsonRequestBehavior.AllowGet);
                }
                else if (motortype == "Flanged Motor Only")
                {
                    var selectedRow = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.DriveMotorType == "Flanged Motor Only"), "DriveDescription", "DriveDescription");
                    return Json(selectedRow, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            ViewData["MotorDescription"] = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "MotorDescription", "MotorDescription");
            ViewData["DriveDescription"] = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "DriveDescription", "DriveDescription");
            ViewData["SieveDescription"] = new SelectList(db.OCMSieve.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "SieveDescription", "SieveDescription");
            ViewData["BrakeDescription"] = new SelectList(db.OCMBrake.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "BrakeFullName", "BrakeFullName");
            ViewData["StoneDescription"] = new SelectList(db.OCMStone.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "StoneDescription", "StoneDescription");
            ViewData["CTCoilDescription"] = new SelectList(db.OCMCTCoil.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "CTCoilDescription", "CTCoilDescription");
            ViewData["PassageSticker"] = new SelectList(db.OCMPassageSticker.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "PassageStickerDescription", "PassageStickerDescription");
            ViewData["AccessoriesDescription"] = new SelectList(db.OCMAccessories.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "AccessoriesDescription", "AccessoriesDescription");
            return View();
        }

        [HttpPost]
        public ActionResult Create(OCMWhitner whitner)
        {
            if (whitner.MotorQ == "With-Motor")
            {
                whitner.MotorType = "Foot Motor Only";
            }

            whitner.CreatedOn = System.DateTime.Now;
            whitner.IsDeleted = 0;
            db.OCMWhitner.Add(whitner);
            db.SaveChanges();

            return RedirectToAction("Index", "OCMWhitner", null);
        }

        public ActionResult Index()
        {
            var list = db.OCMWhitner.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            OCMWhitner motor = db.OCMWhitner.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }
            ViewBag.MotorDescription = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "MotorDescription", "MotorDescription");
            ViewBag.DriveDescription = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "DriveDescription", "DriveDescription");
            ViewBag.SieveDescription = new SelectList(db.OCMSieve.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "SieveDescription", "SieveDescription");
            ViewBag.BrakeDescription = new SelectList(db.OCMBrake.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "BrakeFullName", "BrakeFullName");
            ViewBag.StoneDescription = new SelectList(db.OCMStone.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "StoneDescription", "StoneDescription");
            ViewBag.CTCoilDescription = new SelectList(db.OCMCTCoil.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "CTCoilDescription", "CTCoilDescription");
            ViewBag.AccessoriesDescription = new SelectList(db.OCMAccessories.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 5), "AccessoriesDescription", "AccessoriesDescription");

            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OCMWhitner whitner)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            if (whitner.MotorQ == "With-Motor")
            {
                whitner.MotorType = "Foot Motor Only";
            }
            whitner.ModifiedOn = System.DateTime.Now;
            whitner.ModifiedBy = Convert.ToInt32(cpid);
            db.Entry(whitner).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "OCMWhitner", null);
        }


    }
}
