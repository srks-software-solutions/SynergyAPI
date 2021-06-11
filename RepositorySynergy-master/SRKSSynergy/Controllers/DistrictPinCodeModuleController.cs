using Excel;
using SRKSSynergy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SRKSSynergy.Controllers
{
    public class DistrictPinCodeModuleController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        [HttpGet]
        public ActionResult PINCodeExcelUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PINCodeExcelUpload(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                    var getcpid = loginname.CPID;

                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = file.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;

                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["Error"] = "PLease select a valid Excel File";
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;

                    DataSet ds = reader.AsDataSet();
                    reader.Close();
                    #region
                    try
                    {
                        string Errors = null;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //Fecthing Details
                            string State = ds.Tables[0].Rows[i][6].ToString();
                            string District = ds.Tables[0].Rows[i][5].ToString();
                            string PostalDivision = ds.Tables[0].Rows[i][7].ToString();
                            string PostalRegion = ds.Tables[0].Rows[i][8].ToString();
                            string PostalCircle = ds.Tables[0].Rows[i][9].ToString();
                            string Taluk = ds.Tables[0].Rows[i][4].ToString();
                            int PINCode = Convert.ToInt32(ds.Tables[0].Rows[i][2].ToString());
                            string OfficeName = ds.Tables[0].Rows[i][0].ToString();
                            string OfficeStatus = ds.Tables[0].Rows[i][1].ToString();
                            string TelePhone = ds.Tables[0].Rows[i][3].ToString();                                                        
                          
                            DateTime createdon = System.DateTime.Now;
                            int isdeleted = 0;
                            int createdby = loginname.CPID;

                            DistrictPinCodeDetails edt = new DistrictPinCodeDetails();                            
                            edt.State = State;
                            edt.District = District;
                            edt.PostalDivision = PostalDivision;
                            edt.PostalRegion = PostalRegion;
                            edt.PostalCircle = PostalCircle;
                            edt.Taluk = Taluk;
                            edt.PINCode = PINCode;
                            edt.OfficeName = OfficeName;
                            edt.OfficeStatus = OfficeStatus;
                            edt.TelePhone = TelePhone;
                            edt.IsDeleted = isdeleted;
                            edt.CreatedOn = createdon;                            
                            edt.CreatedBy = createdby;
                           
                            try
                            {
                                //var repeateddata = db.DistrictPinCodeDetails_tbl.Where(m => m.IsDeleted == 0 && m.PINCode == PINCode).ToList();
                                //if (repeateddata.Count == 0)
                                //{
                                    db.DistrictPinCodeDetails_tbl.Add(edt);
                                    db.SaveChanges();
                                //}
                            }
                            catch (DbEntityValidationException dbEx)
                            {
                                foreach (var validationErrors in dbEx.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                                validationError.PropertyName,
                                                                validationError.ErrorMessage);
                                    }
                                }
                            }
                        }

                        Session["MachErrDetails"] = Errors;
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                    #endregion
                }

            }

            return RedirectToAction("PINCodeExcelUpload");
        }


    }
}
