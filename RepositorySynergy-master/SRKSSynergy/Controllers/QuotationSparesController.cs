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
    public class QuotationSparesController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

               ////Json AutoComplete customer unique id
        public JsonResult Autocomplete1(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.CompanyUniqueID.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) 
                          && (r.IsDeleted == 0)
                          select new { r.CompanyUniqueID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete customer name
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) 
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName}).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete quotation number
        public JsonResult Autocomplete2(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.QGSpareGeneralData
                          where r.QuotationNumber.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) && (r.Islatest != 1)
                          select new { r.QuotationNumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete customer name
        public JsonResult Autocomplete3(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) 
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Quotation/
        public ActionResult Create(int? mdbid, String custuniqid = null, String custname = null,String QuotNum = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName,m.MiddleName,m.LastName,m.Designation,m.CPID }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            //If MDBID has been selected or created
            if (mdbid.HasValue)
            {
                
                var mdid = db.MDBGeneralData.Find(mdbid);
                var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).First();
                ViewBag.ContactPersonName =cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                {
                    ViewBag.dear = "Madam";
                }
                else
                {
                    ViewBag.dear = "Sir";
                }
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                ViewBag.QuotationNumber = quotationnumber();
                ViewBag.MDBID = mdid.MDBID;
                ViewBag.OrganizationName = mdid.OrganizationName;
                ViewBag.AddressLine1 = mdid.AddressLine1;
                ViewBag.AddressLine2 = mdid.AddressLine2;
                ViewBag.AddressLine3 = mdid.AddressLine3;
                ViewBag.City = mdid.City;
                ViewBag.Pincode = mdid.Pincode;
                ViewBag.State = mdid.State;
                ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                ViewBag.NullError = false;
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                return View(mdb);
            }
                //If nothing has been selected
            else if (custuniqid == null && custname == null && QuotNum == null)
            {
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                ViewBag.OrganizationName = "OrganizationName";
                ViewBag.AddressLine1 = "Address(Line1)";
                ViewBag.AddressLine2 = "Address(Line2)";
                ViewBag.AddressLine3 = "Address(Line3)";
                ViewBag.City = "City";
                ViewBag.Pincode = "Pincode";
                ViewBag.State = "State";
                ViewBag.CompanyUniqueID = "CompanyUniqueID";
                ViewBag.NullError = false;
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                return View(mdb);
            }
            //If Customer Unique ID  has been selected
            else if (custuniqid != null && custname == null)
            {
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                //var mdid = db.MDBGeneralData.Find(custuniqid);
                var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custuniqid).First();
                if (mdid != null)
                {
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.MDBID = mdid.MDBID;
                    var quotmod = from s in db.QGSpareGeneralData
                           select s;
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
                else
                {
                    
                    ViewBag.NullError = true;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
            }
            //If Customer Name has been selected
            else if (custuniqid == null && custname != null)
            {
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                //var mdid = db.MDBGeneralData.Find(custname);
                var mdid = db.MDBGeneralData.Where(m => m.OrganizationName == custname).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                if (mdid != null)
                {
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
            }
            //If Customer Unique Id and Customer Name are selected
            else if (custuniqid != null && custname != null)
            {
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custuniqid).First();
                var mdid1 = db.MDBGeneralData.Where(m => m.OrganizationName == custname).First();
                if (mdid != null)
                {
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
                else if (mdid1 != null)
                {
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
            }
            //If Quotation Number has been selected
            else if (QuotNum != null)
            {
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                var mdid = db.QGSpareGeneralData.Where(m => m.QuotationNumber == QuotNum).First();
                if (mdid != null)
                {
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    var mdid1 = db.MDBGeneralData.Find(mdid.MDBID);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.OrganizationName = mdid1.OrganizationName;
                    ViewBag.AddressLine1 = mdid1.AddressLine1;
                    ViewBag.AddressLine2 = mdid1.AddressLine2;
                    ViewBag.AddressLine3 = mdid1.AddressLine3;
                    ViewBag.City = mdid1.City;
                    ViewBag.Pincode = mdid1.Pincode;
                    ViewBag.State = mdid1.State;
                    ViewBag.CompanyUniqueID = mdid1.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(mdb);
                }
            }
                //If all the above conditions fails
            else
            {
                QuotationSpares mdb = new QuotationSpares();
                //mdb.QGSparePayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                ViewBag.NullError = true;
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                return View(mdb);
            }
        }

        //
        //Generate the quotation number
        public string quotationnumber()
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2,2);
            String quotationnumber;
            var quotmod = from QuotationNumber in db.QGSpareGeneralData
                          select QuotationNumber;
            quotmod = quotmod.OrderByDescending(m => m.QuotationNumber);
            var check = quotmod.FirstOrDefault();
            if (check == null)
            {
                quotationnumber = "QS-" + "RC"+ year + "-00001-00";
            }
            else
            {
                var quotval = quotmod.Select(m => m.QuotationNumber).First();
                string[] split = quotval.Split('-');
                if (split[1].Substring(2,2) == System.DateTime.Now.Year.ToString().Substring(2,2))
                {
                    int k = Convert.ToInt32(split[2]);
                    int ad = k + 1;
                    string cpik = null;
                    string len = ad.ToString();
                    if (len.Length == 1)
                    {
                        cpik = "0000" + ad;
                    }
                    else if (len.Length == 2)
                    {
                        cpik = "000" + ad;
                    }
                    else if (len.Length == 3)
                    {
                        cpik = "00" + ad;
                    }
                    else if (len.Length == 4)
                    {
                        cpik = "0" + ad;
                    }
                    else
                    {
                        cpik = ad.ToString();
                    }
                    quotval = split[0] + "-" + "RC" + year + "-" + cpik + "-00";
                    quotationnumber = quotval;
                }
                else
                {
                    quotationnumber = "QS-" + "RC" + year + "-00001-00";
                }
            }
            return quotationnumber;
        }

        //
        // POST: /MDB/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuotationSpares maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.QGSpareTableData1.ProductModelSparesID != 0)
                {
                    maindatabase.QGSpareGeneralData.CPID = loginname.CPID;
                    maindatabase.QGSpareGeneralData.MDBID = (int)(mdbid);
                    db.QGSpareGeneralData.Add(maindatabase.QGSpareGeneralData);
                    db.SaveChanges();
                    var cpid = from s in db.QGSpareGeneralData
                               select s;
                    cpid = cpid.OrderByDescending(m => m.QGID);
                    int qgid = cpid.Select(m => m.QGID).First();
                    maindatabase.QGSparePayment.QGID = qgid;
                    db.QGSparePayment.Add(maindatabase.QGSparePayment);
                    db.SaveChanges();
                    maindatabase.QGSpareTableData1.QGID = qgid;
                    db.QGSpareTableData.Add(maindatabase.QGSpareTableData1);
                    db.SaveChanges();
                    if (maindatabase.QGSpareTableData2.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData2.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData3.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData3.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData4.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData4.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData4);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData5.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData5.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData5);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData6.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData6.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData6);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData7.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData7.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData7);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData8.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData8.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData8);
                        db.SaveChanges();
                    }
                    //var qgtblid = db.QGSpareTableData.Where(m => m.QGID == qgid).Where(m => m.QGSpareGeneralData.Islatest == 0).Select(a => new { a.ProductModel.ProductModelName, a.Quantity }).ToList();
                    //var cnt = qgtblid.Count();
                    //foreach(var qg in qgtblid)
                    ////for (int i = 0; i < cnt; i++ )
                    //{
                        
                    //    SOT sot = new SOT();
                    //    sot.QGID = qgid;
                    //    sot.CPID = loginname.CPID;
                    //    sot.Equipment = qg.ProductModelName;
                    //    sot.Quantity = qg.Quantity;
                    //    db.SOT.Add(sot);
                    //    db.SaveChanges();
                    //}
                    //return RedirectToAction("ReportGeneration", new { qgid = qgid });

                    String updateparent = "<script>window.open('/QuotationSpares/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = document.location.pathname;</script>";
                    return Content(updateparent);
                }
                return View(maindatabase);
            }
            else
            {
                ViewBag.OrganizationName = "OrganizationName";
                ViewBag.AddressLine1 = "Address(Line1)";
                ViewBag.AddressLine2 = "Address(Line2)";
                ViewBag.AddressLine3 = "Address(Line3)";
                ViewBag.City = "City";
                ViewBag.Pincode = "Pincode";
                ViewBag.State = "State";
                ViewBag.CompanyUniqueID = "CompanyUniqueID";
                ViewBag.NullError = true;
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                return View(maindatabase);
            }
        }

        public ActionResult NewCustomer()
        {
            var mdid = from CompanyUniqueID in db.MDBGeneralData
                       select CompanyUniqueID;
            mdid = mdid.OrderByDescending(m => m.MDBID);
            var check = mdid.FirstOrDefault();
            if (check == null)
            {
                ViewBag.Chanpart = "CUS-" + System.DateTime.Now.Year + "-00001";
            }
            else
            {
                var mdbi = mdid.Select(m => m.CompanyUniqueID).First();
                string[] split = mdbi.Split('-');
                //if (split[1] == System.DateTime.Now.Year.ToString())
                //{
                    int k = Convert.ToInt32(split[2]);
                    int ad = k + 1;
                    string cpik = null;
                    string len = ad.ToString();
                    if (len.Length == 1)
                    {
                        cpik = "0000" + ad;
                    }
                    else if (len.Length == 2)
                    {
                        cpik = "000" + ad;
                    }
                    else if (len.Length == 3)
                    {
                        cpik = "00" + ad;
                    }
                    else if (len.Length == 4)
                    {
                        cpik = "0" + ad;
                    }
                    else
                    {
                        cpik = ad.ToString();
                    }
                    mdbi = split[0] + "-" + System.DateTime.Now.Year + "-" + cpik;
                    ViewBag.Chanpart = mdbi;
                //}
                //else
                //{
                //    ViewBag.Chanpart = "CUS-" + System.DateTime.Now.Year + "-00001";
                //}
            }
            return View();
        }

        //
        // Creating Quick Customer Generation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCustomer(QuickGenerateMDB MDB)
        {
            var duplicate = (from s in db.MDBGeneralData
                             where s.OrganizationName == MDB.OrganizationName && s.AddressLine1 == MDB.AddressLine1 && s.AddressLine2 == MDB.AddressLine2 && s.AddressLine3 == MDB.AddressLine3 && s.City == MDB.City && s.Pincode == MDB.Pincode
                             select s).ToList();

            if (MDB.OrganizationName != null && MDB.FirstName != null)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (duplicate.Count > 0)
                {
                    ViewBag.Duplicate = "Customer Already Exists for this Address and City";
                }
                else
                { 
                    MDBGeneralData mdbgd = new MDBGeneralData();
                    mdbgd.CPID = loginname.CPID;
                    mdbgd.CompanyUniqueID = MDB.CompanyUniqueID;
                    mdbgd.AddressLine1 = MDB.AddressLine1;
                    mdbgd.AddressLine2 = MDB.AddressLine2;
                    mdbgd.AddressLine3 = MDB.AddressLine3;
                    mdbgd.OrganizationName = MDB.OrganizationName;
                    mdbgd.OrganizationType = MDB.OrganizationType;
                    mdbgd.City = MDB.City;
                    mdbgd.State = MDB.State;
                    mdbgd.Pincode = MDB.Pincode;
                    mdbgd.Country = MDB.Country;
                    mdbgd.Isd1 = MDB.Isd1;
                    mdbgd.Std1 = MDB.Std1;
                    mdbgd.PhoneLL1 = MDB.PhoneLL1;
                    mdbgd.EmailID = MDB.EmailID;
                    db.MDBGeneralData.Add(mdbgd);
                    db.SaveChanges();
                    var cpid = from s in db.MDBGeneralData
                               select s;
                    cpid = cpid.OrderByDescending(m => m.MDBID);
                    int cpi = cpid.Select(m => m.MDBID).First();
                    MDBContactPersonData mdbcd = new MDBContactPersonData();
                    mdbcd.MDBID = cpi;
                    mdbcd.Title = MDB.Title;
                    mdbcd.FirstName = MDB.FirstName;
                    mdbcd.MiddleName = MDB.MiddleName;
                    mdbcd.LastName = MDB.LastName;
                    mdbcd.Isd1 = MDB.Isdc1;
                    mdbcd.Std1 = MDB.Stdc1;
                    mdbcd.PhoneLL1 = MDB.PhoneLLc1;
                    mdbcd.Isdm1 = MDB.Isdm1;
                    mdbcd.Mobile1 = MDB.Mobile1;
                    mdbcd.EmailID = MDB.EmailIDContact;
                    db.MDBContactPersonData.Add(mdbcd);
                    db.SaveChanges();
                    String updateparent = "<script>window.opener.setMDBID(" + cpi + ");window.close();</script>";
                    return Content(updateparent);
                }
            }
            return View(MDB);
        }

        public ActionResult NewQuotation()
        {
            ViewBag.NullError = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewQuotation(String custnam = null, String custunid = null)
        {
            //If Customer Unique ID  has been selected
            if (custunid != "" && custnam == "")
            {
                var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custunid).SingleOrDefault();
                if (mdid != null)
                {
                    //ViewBag.NullError = false;
                    //return View();
                    String updateparent = "<script>window.opener.setCustUniID('" + custunid + "','');window.close();</script>";
                    return Content(updateparent);
                }
                else
                {
                    ViewBag.NullError = true;
                    return View();
                }
            }
            //If Customer Name has been selected
            else if (custunid == "" && custnam != "")
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

                var mdid = db.MDBGeneralData.Where(m => m.OrganizationName == custnam).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                if (mdid != null)
                {
                    //ViewBag.NullError = false;
                    //return View();
                    String updateparent = "<script>window.opener.setCustUniID('','" + custnam + "');window.close();</script>";
                    return Content(updateparent);
                }
                else
                {
                    ViewBag.NullError = true;
                    return View();
                }
            }
            //If Customer Unique Id and Customer Name are selected
            else if (custunid != "" && custnam != "")
            {
                var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custunid).SingleOrDefault();
                var mdid1 = db.MDBGeneralData.Where(m => m.OrganizationName == custnam).SingleOrDefault();
                if (mdid != null)
                {
                    //ViewBag.NullError = false;
                    //return View();
                    String updateparent = "<script>window.opener.setCustUniID('" + custunid + "','');window.close();</script>";
                    return Content(updateparent);
                }
                else if (mdid1 != null)
                {
                    //ViewBag.NullError = false;
                    //return View();
                    String updateparent = "<script>window.opener.setCustUniID('','" + custnam + "');window.close();</script>";
                    return Content(updateparent);
                }
                else
                {
                    ViewBag.NullError = true;
                    return View();
                }
            }
            ViewBag.NullError = true;
            return View();
        }

        const int pageSize = 10;
        bool getdetailsclick = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult MainExistingQuotation(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<QGSpareGeneralData> quotations = db.QGSpareGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m=>m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.QGID) : quotations.OrderByDescending(p => p.QGID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.QuotationNumber) : quotations.OrderByDescending(p => p.QuotationNumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.Subject) : quotations.OrderByDescending(p => p.Subject);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.CPQuotationNumber) : quotations.OrderByDescending(p => p.CPQuotationNumber);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.QuotationDate) : quotations.OrderByDescending(p => p.QuotationDate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.QGID) : quotations.OrderByDescending(p => p.QGID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize);

            quotations = quotations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.Search = cunam;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;

            if (getdetailsclick)
                ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclick = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations);
        }

        //
        // GET: /Quotation/ReviseQuotation
        public ActionResult ReviseQuotation(int qgid = 0,String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (qgid != 0)
            {
                QuotationSpares quo = new QuotationSpares();
                var qgdb = db.QGSpareGeneralData.Find(qgid);
                if(qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.QGSpareTableData.Where(m => m.QGID == qgid);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.QGSpareGeneralData = qgdb;
                    quo.QGSparePayment = db.QGSparePayment.Find(qgid);
                    if (modelcnt == 8)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.QGSpareTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.QGSpareTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.QGSpareTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.QGSpareTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.QGSpareTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.QGSpareTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.QGSpareTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = modelcount[4];
                        quo.QGSpareTableData6 = modelcount[5];
                        quo.QGSpareTableData7 = modelcount[6];
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = modelcount[4];
                        quo.QGSpareTableData6 = modelcount[5];
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = modelcount[4];
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = null;
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = null;
                        quo.QGSpareTableData4 = null;
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = null;
                        quo.QGSpareTableData3 = null;
                        quo.QGSpareTableData4 = null;
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = Revisequotationnumber(quotnum);
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(quo);
                }
            }
            else if(quotnumber != null)
            {
                QuotationSpares quo = new QuotationSpares();
                var qgdb1 = db.QGSpareGeneralData.Where(m => m.QuotationNumber == quotnumber).Select(m => m.QGID).SingleOrDefault();
                var qgdb = db.QGSpareGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.QGSpareTableData.Where(m => m.QGID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.QGSpareGeneralData = qgdb;
                    quo.QGSparePayment = db.QGSparePayment.Find(qgdb1);
                    if (modelcnt == 8)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.QGSpareTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.QGSpareTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.QGSpareTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.QGSpareTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.QGSpareTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.QGSpareTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.QGSpareTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = modelcount[4];
                        quo.QGSpareTableData6 = modelcount[5];
                        quo.QGSpareTableData7 = modelcount[6];
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = modelcount[4];
                        quo.QGSpareTableData6 = modelcount[5];
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = modelcount[4];
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = modelcount[3];
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = modelcount[2];
                        quo.QGSpareTableData4 = null;
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = modelcount[1];
                        quo.QGSpareTableData3 = null;
                        quo.QGSpareTableData4 = null;
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.QGSpareTableData1 = modelcount[0];
                        quo.QGSpareTableData2 = null;
                        quo.QGSpareTableData3 = null;
                        quo.QGSpareTableData4 = null;
                        quo.QGSpareTableData5 = null;
                        quo.QGSpareTableData6 = null;
                        quo.QGSpareTableData7 = null;
                        quo.QGSpareTableData8 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                    }
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = Revisequotationnumber(quotnum);
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        //Generate the Revision Quotation Number
        public string Revisequotationnumber(String quotnum)
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2,2);
            String quotationnumber;
            string[] split = quotnum.Split('-');
                    int k = Convert.ToInt32(split[3]);
                    int ad = k + 1;
                    string cpik = null;
                    string len = ad.ToString();
                    if (len.Length == 1)
                    {
                        cpik = "0" + ad;
                    }
                    else
                    {
                        cpik = ad.ToString();
                    }
                    quotnum = split[0] + "-" + "RC" + year + "-" + split[2] + "-" + cpik;
                    quotationnumber = quotnum;
            return quotationnumber;
        }

        //
        // POST: /Quotation/ReviseQuotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviseQuotation(QuotationSpares maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.QGSpareTableData1.ProductModelSparesID != 0)
                {

                    var revqg = maindatabase.QGSpareGeneralData.QuotationNumber;
                    var revsplit = revqg.Split('-');
                    var revcomb = revsplit[0] + "-" + revsplit[1] + "-" + revsplit[2];
                    var revnum = Convert.ToInt32(revsplit[3]);
                    var prvrevisednum = "";
                    if (revnum != 0)
                    {
                        revnum -= 1;
                    }
                    if (revnum.ToString().Length == 1)
                    {
                        prvrevisednum = "0" + revnum;
                    }
                    else
                    {
                        prvrevisednum = revnum.ToString();
                    }
                    var fullqn = revcomb + "-" + prvrevisednum;
                    var previosqg = db.QGSpareGeneralData.Where(m => m.QuotationNumber == fullqn).First();
                    previosqg.Islatest = 1;
                    db.Entry(previosqg).State = EntityState.Modified;
                    db.SaveChanges();

                    maindatabase.QGSpareGeneralData.CPID = loginname.CPID;
                    maindatabase.QGSpareGeneralData.MDBID = (int)(mdbid);
                    db.QGSpareGeneralData.Add(maindatabase.QGSpareGeneralData);
                    db.SaveChanges();
                    var qg = from s in db.QGSpareGeneralData
                               select s;
                    qg = qg.OrderByDescending(m => m.QGID);
                    int qgid = qg.Select(m => m.QGID).First();
                    maindatabase.QGSparePayment.QGID = qgid;
                    db.QGSparePayment.Add(maindatabase.QGSparePayment);
                    db.SaveChanges();
                    maindatabase.QGSpareTableData1.QGID = qgid;
                    db.QGSpareTableData.Add(maindatabase.QGSpareTableData1);
                    db.SaveChanges();
                    if (maindatabase.QGSpareTableData2.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData2.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData3.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData3.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData4.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData4.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData4);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData5.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData5.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData5);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData6.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData6.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData6);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData7.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData7.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData7);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGSpareTableData8.ProductModelSparesID != 0)
                    {
                        maindatabase.QGSpareTableData8.QGID = qgid;
                        db.QGSpareTableData.Add(maindatabase.QGSpareTableData8);
                        db.SaveChanges();
                    }
                                        
                    String updateparent = "<script>window.open('/QuotationSpares/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = '/QuotationSpares/Create';</script>";
                    return Content(updateparent);
                }
                return RedirectToAction("Create");
            }
            else
            {
                QuotationSpares mdb = new QuotationSpares();
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.MDBID = mdid.MDBID;
                ViewBag.OrganizationName = mdid.OrganizationName;
                ViewBag.AddressLine1 = mdid.AddressLine1;
                ViewBag.AddressLine2 = mdid.AddressLine2;
                ViewBag.AddressLine3 = mdid.AddressLine3;
                ViewBag.City = mdid.City;
                ViewBag.Pincode = mdid.Pincode;
                ViewBag.State = mdid.State;
                ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                ViewBag.NullError = false;
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID6"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID7"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID8"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                return View();
            }
        }

        //
        //Navigate to Revise Quotation after Clicking Revise button in Existing Quotation Page
        public ActionResult NavigateReviseQuotation(int qgid = 0)
        {
            if (qgid != 0)
            {
                var qgdb = db.QGSpareGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String updateparent = "<script>window.opener.setRevisionQuotationID(" + qgid + ");window.close();</script>";
                    return Content(updateparent);
                }
            }
            return View("MainExistingQuotation");
        }

        //Product Model Details Update Dynamically in the View
        [HttpGet]
        public JsonResult GetProductModelDetails(int id)
        {
            var selectedRow = (from t in db.ProductModelSpare where t.ProductModelSparesID == id select t).SingleOrDefault(); 

            var jsonData = new
            {
                unitprice = selectedRow.CustomerPrice,
                Desc = selectedRow.ProductModelSparesDesc,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //Update Quotation Number in the New Quotation View
        [HttpGet]
        public JsonResult Getquotationnumber(int id = 0)
        {
            if (id != 0)
            {
                ViewBag.QuotationNumber = quotationnumber();

                var jsonData = new
                {
                    quotnumb = quotationnumber()
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var jsonData = new
                {
                    quotnumb = ""
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Getrevisequotationnumber(String ID)
        {
            ViewBag.ReviseQuotationNumber = Revisequotationnumber(ID);

            var jsonData = new
            {
                quotnumb = Revisequotationnumber(ID)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportGeneration(int? qgid)
        {
            if (qgid != 0)
            {
                QuotationSpares quo = new QuotationSpares();
                ReportModel1 RM1 = new ReportModel1();
                var qgdb = db.QGSpareGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var cpid = qgdb.CPID;
                    var Logo = db.ChannelPartners.Find(cpid);

                    var channelPartnerLogo = "";
                    var Logopath = "";

                    var add1 = "Check Your Address";
                    if (cpid != 0)
                    {

                        channelPartnerLogo = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                        Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                        add1 = Logo.footaddress;
                    }
                    else
                    {

                        //channelPartnerLogo = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                        Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                        add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";
                    }
                    var models = db.QGSpareTableData.Where(m => m.QGID == qgid);
                    var modelcount = models.ToList();
                    quo.QGSpareGeneralData = qgdb;
                    var paymentterms = db.QGSparePayment.Where(m => m.QGID == qgid).First();
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    RM1.Logo = Logopath;
                    RM1.footaddress = add1;
                    RM1.QGID = qgdb.QGID;
                    RM1.annexure = paymentterms.annexure;
                    RM1.CPQuotationNumber = qgdb.CPQuotationNumber;
                    RM1.MDBID = mdbdet.MDBID.ToString();
                    RM1.QuotationNumber = qgdb.QuotationNumber;
                    RM1.QuotationDate = Convert.ToDateTime(qgdb.QuotationDate).ToString("dd-MM-yyyy");
                    RM1.PaymentTerms = paymentterms.PaymentTerms;
                    RM1.Delivery = paymentterms.Delivery;
                    RM1.DateofDispatch = paymentterms.DateofDispatch;
                    RM1.Transport = paymentterms.Transport;
                    RM1.Freight = paymentterms.Freight;
                    RM1.CST = paymentterms.CST;
                    RM1.TransitInsu = paymentterms.TransitInsu;
                    RM1.Commodity = paymentterms.Commodity;
                    RM1.Validity = paymentterms.Validity;
                    RM1.Subject = qgdb.Subject;
                    RM1.KindAttention = qgdb.KindAttention;
                    RM1.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    RM1.State = mdbdet.State;
                    RM1.Pincode = mdbdet.Pincode;
                    RM1.overallprice = paymentterms.overallprice;
                    RM1.City = mdbdet.City;
                    RM1.AddressLine1 = mdbdet.AddressLine1;
                    RM1.AddressLine2 = mdbdet.AddressLine2;
                    RM1.AddressLine3 = mdbdet.AddressLine3;
                    RM1.OrganizationName = mdbdet.OrganizationName;
                    RM1.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.IsOne = false;
                    ViewBag.IsTwo = false;
                    ViewBag.IsThree = false;
                    ViewBag.IsFour = false;
                    ViewBag.IsFive = false;
                    ViewBag.IsSix = false;
                    ViewBag.IsSeven = false;
                    ViewBag.IsEight = false;
                    RM1.QGSpareTableData = modelcount;
                    if (modelcount.Count == 1)
                    {
                        ViewBag.IsOne = true;
                    }
                    else if (modelcount.Count == 2)
                    {
                        ViewBag.IsTwo = true;
                    }
                    else if (modelcount.Count == 3)
                    {
                        ViewBag.IsThree = true;
                    }
                    else if (modelcount.Count == 4)
                    {
                        ViewBag.IsFour = true;
                    }
                    else if (modelcount.Count == 5)
                    {
                        ViewBag.IsFive = true;
                    }
                    else if (modelcount.Count == 6)
                    {
                        ViewBag.IsSix = true;
                    }
                    else if (modelcount.Count == 7)
                    {
                        ViewBag.IsSeven = true;
                    }
                    else if (modelcount.Count == 8)
                    {
                        ViewBag.IsEight = true;
                    }

                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                    RM1.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                    RM1.Designation = loginname.Designation;
                    var FileName = RM1.QuotationNumber;
                    ViewBag.CPID = loginname.CPID;
                    var relativePath = "ReportGeneration";
                    string content;
                    var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                    ViewData.Model = RM1;

                    String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                    string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
                        //"<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 140px\"><div style=\"width: 700px; float: left; height: 140px\"></div><div style=\"width: 300px; height: 140px; float: right\"><img src= \"" + Logopath + "\" width=\"200\" height=\"60\" /></div></div></body>";

                    if(System.IO.File.Exists(headerpath))
                    System.IO.File.Delete(headerpath);
                    System.IO.File.Create(headerpath).Close();
                    System.IO.File.WriteAllText(headerpath, htmlheader);
                    //use the passed-in parameter and populate your model
                    using (var writer = new StringWriter())
                    {
                        var context = new ViewContext(this.ControllerContext, view.View, ViewData, TempData, writer);
                        view.View.Render(context, writer);
                        writer.Flush();
                        content = writer.ToString();
                        var pdfconverter = new PDFConverter1();
                        byte[] pdfBuf = pdfconverter.Convert(content, Server.MapPath("~/App_Data/"), add1, headerpath);
                        MemoryStream workStream = new MemoryStream();
                        workStream.Write(pdfBuf, 0, pdfBuf.Length);
                        workStream.Position = 0;
                        if (pdfBuf == null)
                            return HttpNotFound();
                        return new FileStreamResult(workStream, "application/pdf");
                    }
                }
            }
            return View("Create");
        }

        //Quotation numb check valid
        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetExistingquotationnumber(String id = null)
        {
           var mdid = db.QGSpareGeneralData.Where(m => m.QuotationNumber == id).SingleOrDefault();
           if (mdid != null)
           {
               var jsonData = new
               {
                   quotnumb = "true"
               };
               return Json(jsonData, JsonRequestBehavior.AllowGet);
           }
           else
           {
               var jsonData = new
               {
                   quotnumb = "false"
               };
               return Json(jsonData, JsonRequestBehavior.AllowGet);
           }
        }

        //Update Customer Number in the New Customer View
        [HttpGet]
        public JsonResult Getcustomernumber()
        {

            ViewBag.Chanpart = NewCustomer();

            var jsonData = new
            {
                cusnumb = NewCustomer()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /Annexure/

        public ActionResult Annexure()
        {
            return View();
        }

        //
        // GET: /Annexure1/

        public ActionResult Annexure1()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public interface IPDFConverter1
    {
        byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml);
    }

    public class PDFConverter1 : IPDFConverter1
    {
        private const string HtmlToPdfExePath = "wkhtmltopdf.exe";
        private readonly ILog log = LogManager.GetLogger(typeof(PDFConverter1));

        public byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml)
        {
            Process p;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Path.Combine(commandLocation, HtmlToPdfExePath);
            psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);

            // run the conversion utility
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // note: that we tell wkhtmltopdf to be quiet and not run scripts
            string args = "-q -n ";
            args += "--disable-smart-shrinking ";
            args += "--orientation Portrait ";
            args += "--print-media-type ";
            args += "--margin-top 5mm --margin-bottom 5mm --margin-right 5mm --margin-left 5mm --footer-center \"" + Footeraddress + "\" --footer-right \"Page [page]/[topage]\" --footer-font-name \"Calibri\" --footer-font-size \"10\" --header-html \"" + HeaderHtml + "\" ";
            args += "--outline-depth 0 ";
            args += "--page-size A4 ";
            args += " - -";

            psi.Arguments = args;

            p = Process.Start(psi);

            try
            {
                using (StreamWriter stdin = p.StandardInput)
                {
                    stdin.AutoFlush = true;
                    stdin.Write(source);
                }

                //read output
                byte[] buffer = new byte[32768];
                byte[] file;
                using (var ms = new MemoryStream())
                {
                    while (true)
                    {
                        int read = p.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            break;
                        ms.Write(buffer, 0, read);
                    }
                    file = ms.ToArray();
                }

                p.StandardOutput.Close();
                // wait or exit
                p.WaitForExit(30000);

                // read the exit code, close process
                int returnCode = p.ExitCode;
                p.Close();

                if (returnCode == 0)
                    return file;
                else
                    log.Error("Could not create PDF, returnCode:" + returnCode);
            }
            catch (Exception ex)
            {
                log.Error("Could not create PDF", ex);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
            return null;
        }
    }

}
