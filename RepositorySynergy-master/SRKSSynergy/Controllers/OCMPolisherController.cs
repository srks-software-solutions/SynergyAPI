using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class OCMPolisherController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        public ActionResult Create()
        {
            ViewData["MotorDescription"] = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m=>m.MasterProductID==7), "MotorDescription", "MotorDescription");
            ViewData["DriveDescription"] = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "DriveDescription", "DriveDescription");
            ViewData["SieveDescription"] = new SelectList(db.OCMSieve.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "SieveDescription", "SieveDescription");

            ViewData["ReduceRingDescription"] = new SelectList(db.OCMReducerRing.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "ReduceRingDescription", "ReduceRingDescription");
            ViewData["CTCoilDescription"] = new SelectList(db.OCMCTCoil.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "CTCoilDescription", "CTCoilDescription");
            ViewData["AccessoriesDescription"] = new SelectList(db.OCMAccessories.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "AccessoriesDescription", "AccessoriesDescription");

            return View();
        }

        [HttpPost]
        public ActionResult Create(OCMPolisher polish)
        {
            if (polish.MotorQ == "With-Motor")
            {
                polish.MotorType = "Foot Motor Only";
            }
            polish.CreatedOn = System.DateTime.Now;
            polish.IsDeleted = 0;
            db.OCMPolisher.Add(polish);
            db.SaveChanges();

            return RedirectToAction("Index", "OCMPolisher", null);
        }

        public ActionResult Index()
        {
            var list = db.OCMPolisher.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            OCMPolisher motor = db.OCMPolisher.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }
            ViewBag.MotorDescription = new SelectList(db.OCMMotor.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "MotorDescription", "MotorDescription");
            ViewBag.DriveDescription = new SelectList(db.OCMDrive.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "DriveDescription", "DriveDescription");
            ViewBag.SieveDescription = new SelectList(db.OCMSieve.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "SieveDescription", "SieveDescription");

            ViewBag.ReduceRingDescription = new SelectList(db.OCMReducerRing.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "ReduceRingDescription", "ReduceRingDescription");
            ViewBag.CTCoilDescription = new SelectList(db.OCMCTCoil.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "CTCoilDescription", "CTCoilDescription");
            ViewBag.AccessoriesDescription = new SelectList(db.OCMAccessories.Where(m => m.IsDeleted == 0).Where(m => m.MasterProductID == 7), "AccessoriesDescription", "AccessoriesDescription");

            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OCMPolisher polish)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);
            if (polish.MotorQ == "With-Motor")
            {
                polish.MotorType = "Foot Motor Only";
            }
            polish.ModifiedOn = System.DateTime.Now;
            polish.ModifiedBy = Convert.ToInt32(cpid);
            db.Entry(polish).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "OCMPolisher", null);
        }

        public ActionResult OCMPolisherRep(int? roaid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            if (roaid != null)
            {
                var start = db.OCMPolisherReport.ToList();
                foreach (var se in start)
                {
                    db.OCMPolisherReport.Remove(se);
                }
                db.SaveChanges();

                //question
                string questiongraintype = null;
                string questionprocess = null;
                string questionpass = null;
                string questioncapacity = null;
                string questionpolishrequirement = null;
                string questionmotortype = null;
                string questionmotorrating = null;
                //end questions

                //answers
                string modelname = null;
                string answergraintype = null;
                string answerprocess = null;
                string drive = null;
                string motor = null;
                string sieve = null;
                string reducerring = null;
                string ctcoil = null;
                string accessories = null;

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    String query = "SELECT * FROM RiceOAEquipGeneralData WHERE ROAID = '" + roaid + "';";
                    var riceoalist = db.Database.SqlQuery<RiceOAEquipGeneralData>(query).ToList();

                    foreach (var s in riceoalist)
                    {
                        questiongraintype = s.TypeRice;
                        questionprocess = s.PaddySize;
                        questionpass = s.Pass;
                        questioncapacity = s.Capacity;
                        questionpolishrequirement = s.PolishRequirement;
                        questionmotortype = s.MotorType;
                        questionmotorrating = s.MotorRating;
                    }

                    String query1 = "SELECT * FROM OCMPolisher WHERE GrainType = '" + questiongraintype + "' and Process='" + questionprocess + "' and Pass='" + questionpass + "' and Capacity='" + questioncapacity + "' and PolishRequirement='" + questionpolishrequirement + "' and MotorType='" + questionmotortype + "' and MotorRating='" + questionmotorrating + "';";
                    var ocmpolisherist = db.Database.SqlQuery<OCMPolisher>(query1).SingleOrDefault();

                    //foreach (var s in ocmpolisherist)
                    //{
                    modelname = ocmpolisherist.ModelName;
                    answergraintype = ocmpolisherist.GrainType;
                    answerprocess = ocmpolisherist.Process;

                    drive = ocmpolisherist.Drive;
                    motor = ocmpolisherist.Motor;
                    sieve = ocmpolisherist.Sieve;
                    reducerring = ocmpolisherist.ReducerRing;
                    ctcoil = ocmpolisherist.CTCoil;
                    accessories = ocmpolisherist.Accessories1;
                    //}

                    var custname = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.MDBGeneralData.OrganizationName).SingleOrDefault();
                    var address = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => new { m.MDBGeneralData.AddressLine1, m.MDBGeneralData.AddressLine2, m.MDBGeneralData.City,m.MDBGeneralData.Country }).SingleOrDefault();
                    var quantity = db.RiceOAEquipTableData.Where(m => m.ROAID == roaid).SingleOrDefault();

                    OCMPolisherReport ocm = new OCMPolisherReport();
                    ocm.FilledBy=Convert.ToString(loginname);
                    ocm.Branch="Buhler";
                    ocm.Date=System.DateTime.Now;
                    ocm.CustomerName = custname;
                    ocm.Location=address.AddressLine1 +","+ address.AddressLine2 +","+ address.City;
                    ocm.Country=address.Country;
                    ocm.Quantity=quantity.Quantity;
                    ocm.Product=modelname;
                    ocm.Capacity=questioncapacity;
                    ocm.Process = answerprocess;
                    ocm.SievePlate = sieve;
                    ocm.ReducerRing = reducerring;
                    ocm.Drive = drive;
                    ocm.Motor = motor;
                    ocm.CTCoil = ctcoil;
                    ocm.Accessories = accessories;

                    db.OCMPolisherReport.Add(ocm);
                    db.SaveChanges();
                }
                #endregion

                return RedirectToAction("OCMPolisherReport", "OCMPolisher", new {  });
            }
            return View();
        }


        public ActionResult OCMPolisherReport()
        {
            var ocmreportlist = db.OCMPolisherReport.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/OCMPolisher/OCMPolisherReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("OCMPolisherReport", ocmreportlist);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            //reportParameters.Add(new ReportParameter("FromDate", fromdat));
            //reportParameters.Add(new ReportParameter("ToDate", todat));
            //reportParameters.Add(new ReportParameter("TCount", tcount));
            //localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }
    }
}
