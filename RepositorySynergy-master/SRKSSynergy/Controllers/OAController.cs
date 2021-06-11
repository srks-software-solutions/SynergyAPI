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
using System.Net.Mail;
using SRKSSynergy.Helpers;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class OAController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //json euip model
        public JsonResult AutocompleteEquipModel(string term)
        {
            var result = (from r in db.ProductModel
                          where r.ProductModelName.ToLower().Contains(term.ToLower())
                          select new { r.ProductModelName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete quotation number
        public JsonResult Autocomplete2(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.QGEquipGeneralData
                          where r.QuotationNumber.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) && (r.Islatest != 1)
                          select new { r.QuotationNumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete customer name
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

        //Json AutoComplete customer name Machine Dispatch Details
        public JsonResult Autocompletemach(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          && (r.IsDeleted == 0)

                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete Channel Partner
        public JsonResult Autocompleteapp(string term)
        {
            var result = (from o in db.OAEquipGeneralData
                          where o.MDBGeneralData.OrganizationName.Contains(term.ToLower()) && o.ApprovalStatus != 2 && o.ApprovalStatus != 1 && o.OAStatus != 0
                          select new { o.MDBGeneralData.OrganizationName }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete Channel Partner
        public JsonResult Autocompleteappprint(string term)
        {
            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower())
                          && (r.IsDeleted == 0)

                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete OA number
        public JsonResult Autocompleteexisoanum(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.QGEquipGeneralData
                          where r.QuotationNumber.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          //&& (r.Islatest != 1)
                          select new { r.QuotationNumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete Customer name
        public JsonResult Autocompleteexiscust(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult Autocompleteview(string term)
        {
            var channame = db.ChannelPartners.Where(m => m.CPName == term).Select(m => m.CPID).SingleOrDefault();

            var result = (from r in db.ChannelPartners
                          where r.CPName.ToLower().Contains(term.ToLower())
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /OA/OAView
        const int pageSize = 2000;
        bool getdetailsclick = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult OAView(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<QGEquipGeneralData> quotations = db.QGEquipGeneralData.Where(p => cunam == null || p.MDBGeneralData.OrganizationName.Contains(cunam))
                .Where(m => m.CPID == loginname.CPID).Where(m => m.Islatest != 1).Where(m => m.QuotStatus == 0).Where(m => m.Ordergenerated != 1).Where(m => m.IsRiceMill != 1).Include(q => q.MDBGeneralData);

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
                    quotations = isAsc ? quotations.OrderBy(p => p.Subjectinfo) : quotations.OrderByDescending(p => p.Subjectinfo);
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

            // ViewBag.CurrentPage = page;
            //  ViewBag.PageSize = pageSize;

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
        // GET: /OA/OAApproval
        const int pageSize1 = 100000;
        bool getdetailsclick1 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult OAApproval(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<OAEquipGeneralData> quotations = db.OAEquipGeneralData.Where(
                    p => cunam == null
                        //removed approval cond 3 for cancellation
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.ApprovalStatus != 2).Where(m => m.ApprovalStatus != 1).Where(m => m.OAStatus != 0).Include(q => q.MDBGeneralData).OrderByDescending(m => m.OADate);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.OANumber) : quotations.OrderByDescending(p => p.OANumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.Subjectinfo) : quotations.OrderByDescending(p => p.Subjectinfo);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.CPQuotationNumber) : quotations.OrderByDescending(p => p.CPQuotationNumber);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.OADate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize1);

            quotations = quotations
                .Skip((page - 1) * pageSize1)
                .Take(pageSize1)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize1;

            ViewBag.Search = cunam;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;

            if (getdetailsclick1)
                ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclick1 = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.OADate).ToList());
        }

        //
        // GET: /OA/OAApproval
        const int pageSize1p = 10000000;
        bool getdetailsclick1p = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult OAApprovalPrint(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<OAEquipGeneralData> quotations = db.OAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.ApprovalStatus != 2).Where(m => m.ApprovalStatus != 0).Where(m => m.OAStatus != 0).Include(q => q.MDBGeneralData).OrderByDescending(m => m.OADate);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.OANumber) : quotations.OrderByDescending(p => p.OANumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.Subjectinfo) : quotations.OrderByDescending(p => p.Subjectinfo);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.CPQuotationNumber) : quotations.OrderByDescending(p => p.CPQuotationNumber);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.OADate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize1p);

            quotations = quotations
                .Skip((page - 1) * pageSize1p)
                .Take(pageSize1p)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize1p;

            ViewBag.Search = cunam;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;

            if (getdetailsclick1p)
                ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclick1p = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderBy(m => m.ApprovalStatus).ToList());
        }

        //
        //  GET: /OA/OAApproval
        const int pageSize2 = 2000;
        bool getdetailsclick2 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult OAExisting(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();


            IEnumerable<OAEquipGeneralData> quotations = db.OAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.Islatest != 1).Where(m => m.OAStatus != 0).Where(m => m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.OANumber) : quotations.OrderByDescending(p => p.OANumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.Subjectinfo) : quotations.OrderByDescending(p => p.Subjectinfo);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.CPQuotationNumber) : quotations.OrderByDescending(p => p.CPQuotationNumber);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.OADate);
                    break;

                case 7:
                    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.ApprovalStatus);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSizem);

            //quotations = quotations
            //    .Skip((page - 1) * pageSizem)
            //    .Take(pageSizem)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSizem;

            //ViewBag.Search = cunam;

            //ViewBag.SortBy = sortBy;
            //ViewBag.IsAsc = isAsc;

            //if (getdetailsclickm)
            //    ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclickm = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.OAID).ToList());
        }

        //Quotation numb check valid
        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetExistingquotationnumber(String id = null)
        {
            var mdid = db.QGEquipGeneralData.Where(m => m.QuotationNumber == id).SingleOrDefault();
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

        //
        // GET: /OA/OAGenerate
        public ActionResult OAGenerate(int qgid1 = 0, int qgid = 0, String quotnumber = null)
        {
            if (qgid1 == 0)
            {
                qgid = qgid;
            }
            else
            {
                qgid = qgid1;
            }

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            OA quo = new OA();
            var oaid1 = db.OAEquipGeneralData.Where(m => m.QGID == qgid).Select(m => m.OAID).SingleOrDefault();
            var qgdbs = db.OAEquipGeneralData.Find(oaid1);
            var oapaymentid = db.OAEquipPayment.Where(m => m.OAID == oaid1).Count();
            var oatabledataid = db.OAEquipTableData.Where(m => m.OAID == oaid1).Count();
            int overallpricep = 0;int op = 0;
            string overallpricestring = null;
            if (qgid != 0)
            {
                //Showing Tin Number if Present in Master Data Base
                int mdbidt = db.QGEquipGeneralData.Where(m => m.QGID == qgid).Select(m => m.MDBID).Single();
                var tinnum = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbidt).Select(m => m.TIN).SingleOrDefault();
                ViewBag.TinNumber = tinnum;
                if (qgdbs == null)
                {
                    db.QgtoOAGen(qgid);
                    oaid1 = db.OAEquipGeneralData.Where(m => m.QGID == qgid).Select(m => m.OAID).SingleOrDefault();
                }
                if (oapaymentid == 0)
                {   
                    db.QgtoOAPay(qgid, oaid1);
                }

                if (oatabledataid == 0)
                {
                    db.QgtoOATAb(qgid, oaid1);
                    var overallpricepayment = db.OAEquipTableData.Where(m => m.OAID == oaid1).Select(m => m.TotalPrice).ToList();
                    foreach(var t in overallpricepayment)
                    {
                        //overallpricep += /*Convert.ToInt32(t);*/ /*int.Parse(t.Substring(0, t.Length - 1).Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, string.Empty));*/
                        var opsp = t.Split(',');
                        string ovpr = string.Join("",opsp);
                        overallpricep += Convert.ToInt32(ovpr);
                    }
                    overallpricestring = overallpricep.ToString("#,#", CultureInfo.InvariantCulture);
                    var oapayment = db.OAEquipPayment.Where(m => m.OAID == oaid1).FirstOrDefault();
                    oapayment.overallprice = overallpricestring;
                    db.Entry(oapayment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var qgdb = db.OAEquipGeneralData.Find(oaid1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.OAEquipTableData.Where(m => m.OAID == oaid1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.OAEquipGeneralData = qgdb;
                    quo.OAEquipPayment = db.OAEquipPayment.Where(m => m.OAID == oaid1).Single();
                    if (modelcnt == 4)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.OAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.OAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.OAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = modelcount[2];
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = null;
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
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
                    ViewBag.QuotationNumber = quotnum;
                    ViewBag.OANumber = quotationnumber();
                    ViewBag.OAID = quo.OAEquipGeneralData.OAID;
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
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                var qgdb1 = db.OAEquipGeneralData.Where(m => m.QuotationNumber == quotnumber).Select(m => m.OAID).SingleOrDefault();
                var qgdb = db.OAEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.OAEquipTableData.Where(m => m.OAID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.OAEquipGeneralData = qgdb;
                    quo.OAEquipPayment = db.OAEquipPayment.Find(qgdb1);
                    if (modelcnt == 4)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.OAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.OAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.OAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = modelcount[2];
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = null;
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
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
                    ViewBag.QuotationNumber = quotnum;
                    ViewBag.OANumber = quotationnumber();
                    ViewBag.OAID = quo.OAEquipGeneralData.OAID;
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
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        // POST: /OA/OAGenerate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OAGenerate(OA maindatabase, int? oaid, string cpnam = null)
        {

            if (oaid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

                if (maindatabase.OAEquipTableData1.ProductModelID != 0)
                {

                    var Oagen = maindatabase.OAEquipGeneralData;
                    db.Entry(Oagen).State = EntityState.Modified;
                    db.SaveChanges();

                    var Oapay = maindatabase.OAEquipPayment;
                    db.Entry(Oapay).State = EntityState.Modified;
                    db.SaveChanges();

                    var Oatab1 = maindatabase.OAEquipTableData1;
                    db.Entry(Oatab1).State = EntityState.Modified;
                    db.SaveChanges();
                    if (maindatabase.OAEquipTableData2.ProductModelID != 0)
                    {
                        if (maindatabase.OAEquipTableData2.UnitPrice != null && maindatabase.OAEquipTableData2.UnitPrice != "")
                        {
                            var oatch = db.OAEquipTableData.Where(m => m.ProductModelID == maindatabase.OAEquipTableData2.ProductModelID).Where(m => m.OAID == maindatabase.OAEquipTableData2.OAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent = true;
                                OAEquipTableData oatb2 = new OAEquipTableData();
                                oatb2 = maindatabase.OAEquipTableData2;
                                oatb2.OAID = maindatabase.OAEquipGeneralData.OAID;
                                oatb2.ProductModelID = maindatabase.OAEquipTableData2.ProductModelID;

                                db.OAEquipTableData.Add(oatb2);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(maindatabase.OAEquipTableData2).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.OAEquipTableData2.OATBID;
                            OAEquipTableData oad = db.OAEquipTableData.Find(id);
                            db.OAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.OAEquipTableData3.ProductModelID != 0)
                    {
                        if (maindatabase.OAEquipTableData3.UnitPrice != null && maindatabase.OAEquipTableData3.UnitPrice != "")
                        {
                            var oatch = db.OAEquipTableData.Where(m => m.ProductModelID == maindatabase.OAEquipTableData3.ProductModelID).Where(m => m.OAID == maindatabase.OAEquipTableData3.OAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                OAEquipTableData oatb3 = new OAEquipTableData();
                                oatb3 = maindatabase.OAEquipTableData3;
                                oatb3.OAID = maindatabase.OAEquipGeneralData.OAID;
                                oatb3.ProductModelID = maindatabase.OAEquipTableData3.ProductModelID;
                                db.OAEquipTableData.Add(oatb3);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(maindatabase.OAEquipTableData3).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.OAEquipTableData3.OATBID;
                            OAEquipTableData oad = db.OAEquipTableData.Find(id);
                            db.OAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.OAEquipTableData4.ProductModelID != 0)
                    {
                        if (maindatabase.OAEquipTableData4.UnitPrice != null && maindatabase.OAEquipTableData4.UnitPrice != "")
                        {
                            var oatch = db.OAEquipTableData.Where(m => m.ProductModelID == maindatabase.OAEquipTableData4.ProductModelID).Where(m => m.OAID == maindatabase.OAEquipTableData4.OAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent2 = true;
                                OAEquipTableData oatb4 = new OAEquipTableData();
                                oatb4 = maindatabase.OAEquipTableData4;
                                oatb4.OAID = maindatabase.OAEquipGeneralData.OAID;
                                oatb4.ProductModelID = maindatabase.OAEquipTableData4.ProductModelID;
                                db.OAEquipTableData.Add(oatb4);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(maindatabase.OAEquipTableData4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.OAEquipTableData4.OATBID;
                            OAEquipTableData oad = db.OAEquipTableData.Find(id);
                            db.OAEquipTableData.Remove(oad);
                            db.SaveChanges();

                        }
                    }
                    //Updating Tin Number in MDB Statutory Table
                    var mqgid = maindatabase.OAEquipGeneralData.QGID;
                    int mdbid = db.QGEquipGeneralData.Where(m => m.QGID == mqgid).Select(m => m.MDBID).Single();
                    var tinc = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbid).Count();
                    var tinnum = maindatabase.OAEquipGeneralData.TinNumber;
                    if (tinc == 0)
                    {
                        MDBStatutoryNumber mdbst = new MDBStatutoryNumber();
                        mdbst.MDBID = mdbid;
                        mdbst.CompanyPAN = "XXXXXXXXXXXXX";
                        mdbst.TIN = tinnum;
                        db.MDBStatutoryNumber.Add(mdbst);
                        db.SaveChanges();
                    }
                    var cpname = db.ChannelPartners.Where(m => m.CPID == maindatabase.OAEquipGeneralData.CPID).Select(m => m.CPName).SingleOrDefault();
                    maindatabase.OAEquipGeneralData.CPName = cpname;
                    db.Entry(maindatabase.OAEquipGeneralData).State = EntityState.Modified;
                    db.SaveChanges();

                    //Removing Quotation from OA View after it is sent for Approval
                    int qgid = maindatabase.OAEquipGeneralData.QGID;
                    //var sotbyjt = db.SOT.Where(m => m.QGID == qgid).ToList();
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == qgid).ToList();
                    foreach (var qg in quogen)
                    {
                        //qg.Ordergenerated = 1;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    var oaappst = maindatabase.OAEquipGeneralData;
                    oaappst.OAStatus = 1;
                    db.Entry(oaappst).State = EntityState.Modified;
                    db.SaveChanges();

                    var sotbyjt = db.SOT.Where(m => m.QGID == qgid).ToList();
                    foreach (var qg in sotbyjt)
                    {
                        qg.BYJTChances = 100;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //
                    //Remove Quotation from OA View When BYJT Chances becomes 100 that QuotStatus == 4
                    var quotsot = db.QGEquipGeneralData.Where(m => m.QGID == qgid).ToList();
                    foreach (var qg in quogen)
                    {
                        qg.QuotStatus = 4;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }

                // delete and updater
                int tsotid = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.OAEquipGeneralData.QGID).Select(m => m.TSOTID).FirstOrDefault();//Code Modified By sneha 19th July 2017
                if (tsotid != 0)
                {
                    SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(tsotid);
                    db.SOT_Temp_tbl.Remove(tempsot);
                    db.SaveChanges();
                }

                //var list = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).ToList();
                //int count = list.Count;

                //foreach (var l in list)
                //{
                //    if (l.BYJTChances == 0)
                //    {
                //        return RedirectToAction("LOA", "LostOrder", new { qgid = l.QGID});
                //    }
                //    else if(l.BYJTChances == 100)
                //    {
                //        return RedirectToAction("OAGenerate", "OA", new { qgid1 = l.QGID });
                //    }
                //}         

                TempData["oagen"] = "Order Acknowledgement Sent for Approval!!!!";

                //if (tsotid != 0)
                //{
                //    return RedirectToAction("Index", "SOT", null);
                //}
                //else
                //{
                //return RedirectToAction("OAView");
                var list = db.SOT_Temp_tbl.ToList();
                return RedirectToAction("SOTTempList", "SOT", list);
                //}
            }
            else
            {
                var mdbid = maindatabase.OAEquipGeneralData.MDBID;
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.OAID = maindatabase.OAEquipGeneralData.OAID;
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
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        //
        //Generate the quotation number
        public string quotationnumber()
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String quotationnumber;
            var quotmod = from OANumber in db.OAEquipGeneralData
                          select OANumber;
            quotmod = quotmod.OrderByDescending(m => m.OANumber);
            //var check = quotmod.ToList();
            var quotval1 = quotmod.Select(m => m.OANumber).First();
            if (quotval1 == null || quotval1 == "")
            {
                quotationnumber = "OA-" + "RC" + year + "-00001-00";
            }
            else
            {
                var quotval = quotmod.Select(m => m.OANumber).First();
                string[] split = quotval.Split('-');
                if (split[1].Substring(2, 2) == System.DateTime.Now.Year.ToString().Substring(2, 2))
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
                    quotationnumber = "OA-" + "RC" + year + "-00001-00";
                }
            }
            return quotationnumber;
        }

        //Navigate to Revise Quotation after Clicking Revise button in Existing Quotation Page
        public ActionResult NavigateOAGenerate(int qgid = 0)
        {
            if (qgid != 0)
            {
                var qgdb = db.QGEquipGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String updateparent = "<script>window.opener.setRevisionQuotationID(" + qgid + ");window.close();</script>";
                    return Content(updateparent);
                }
            }
            return View("OAView");
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

        //
        // GET: /OA/OARevise
        public ActionResult OARevise(int oaid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (oaid != 0)
            {
                var tin = db.OAEquipGeneralData.Where(m => m.OAID == oaid).Select(m => m.TinNumber).Single();
                ViewBag.TinNumber = tin;
                OA quo = new OA();
                var qgdb = db.OAEquipGeneralData.Find(oaid);
                if (qgdb != null)
                {

                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.OAEquipTableData.Where(m => m.OAID == oaid);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.OAEquipGeneralData = qgdb;
                    quo.OAEquipPayment = db.OAEquipPayment.Where(m => m.OAID == oaid).Single();
                    if (modelcnt == 4)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.OAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.OAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.OAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = modelcount[2];
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = null;
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
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
                    ViewBag.QuotationNumber = quotnum;
                    ViewBag.OANumber = Revisequotationnumber(oanum);
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
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                OA quo = new OA();
                var qgdb1 = db.OAEquipGeneralData.Where(m => m.OANumber == quotnumber).Select(m => m.OAID).SingleOrDefault();
                var qgdb = db.OAEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.OAEquipTableData.Where(m => m.OAID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.OAEquipGeneralData = qgdb;
                    quo.OAEquipPayment = db.OAEquipPayment.Find(qgdb1);
                    if (modelcnt == 4)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.OAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.OAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.OAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = modelcount[2];
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = null;
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
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
                    ViewBag.QuotationNumber = quotnum;
                    ViewBag.OANumber = Revisequotationnumber(oanum);
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
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        //Generate the Revision Quotation Number
        public string Revisequotationnumber(String oanum)
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String quotationnumber;
            string[] split = oanum.Split('-');
            int k = Convert.ToInt32(split[3]);
            int ad = k + 1;
            string cpik = null;
            string len = ad.ToString();
            String yr = split[1].Substring(2, 2);
            if (len.Length == 1)
            {
                cpik = "0" + ad;
            }
            else
            {
                cpik = ad.ToString();
            }
            if (year == yr)
            {
                oanum = split[0] + "-" + "RC" + year + "-" + split[2] + "-" + cpik;
            }
            else
            {
                oanum = split[0] + "-" + split[1] + "-" + split[2] + "-" + cpik;
            }
            quotationnumber = oanum;
            return quotationnumber;
        }

        //
        // POST: /OA/OARevise
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OARevise(OA maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.OAEquipTableData1.ProductModelID != 0)
                {

                    var revqg = maindatabase.OAEquipGeneralData.OANumber;
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
                    var previosqg = db.OAEquipGeneralData.Where(m => m.OANumber == fullqn).First();
                    previosqg.Islatest = 1;
                    db.Entry(previosqg).State = EntityState.Modified;
                    db.SaveChanges();

                    maindatabase.OAEquipGeneralData.CPID = loginname.CPID;
                    maindatabase.OAEquipGeneralData.MDBID = (int)(mdbid);
                    db.OAEquipGeneralData.Add(maindatabase.OAEquipGeneralData);
                    db.SaveChanges();
                    var qg = from s in db.OAEquipGeneralData
                             select s;
                    qg = qg.OrderByDescending(m => m.OAID);
                    int qgid = qg.Select(m => m.OAID).First();
                    maindatabase.OAEquipPayment.OAID = qgid;
                    db.OAEquipPayment.Add(maindatabase.OAEquipPayment);
                    db.SaveChanges();
                    maindatabase.OAEquipTableData1.OAID = qgid;
                    db.OAEquipTableData.Add(maindatabase.OAEquipTableData1);
                    db.SaveChanges();
                    if (maindatabase.OAEquipTableData2.ProductModelID != 0)
                    {
                        maindatabase.OAEquipTableData2.OAID = qgid;
                        db.OAEquipTableData.Add(maindatabase.OAEquipTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.OAEquipTableData3.ProductModelID != 0)
                    {
                        maindatabase.OAEquipTableData3.OAID = qgid;
                        db.OAEquipTableData.Add(maindatabase.OAEquipTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.OAEquipTableData4.ProductModelID != 0)
                    {
                        maindatabase.OAEquipTableData4.OAID = qgid;
                        db.OAEquipTableData.Add(maindatabase.OAEquipTableData4);
                        db.SaveChanges();
                    }
                    var oaappst = maindatabase.OAEquipGeneralData;
                    oaappst.OAStatus = 1;
                    db.Entry(oaappst).State = EntityState.Modified;
                    db.SaveChanges();
                    //Updating Tin Number in MDB Statutory Table
                    var mqgid = maindatabase.OAEquipGeneralData.QGID;
                    int mdbid1 = db.QGEquipGeneralData.Where(m => m.QGID == mqgid).Select(m => m.MDBID).Single();
                    var mdball = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbid1).ToList();
                    foreach (var tin in mdball)
                    {
                        tin.TIN = maindatabase.OAEquipGeneralData.TinNumber;
                        db.Entry(tin).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //String updateparent = "<script>window.open('/Quotation/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = '/Quotation/Create';</script>";
                    //return Content(updateparent);
                }
                return RedirectToAction("OAExisting");
            }
            else
            {
                OA mdb = new OA();
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
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        //
        // Get: /OA/OACheck for Admin
        [Authorize(Roles = "Administrator")]
        public ActionResult OACheck(int oaid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (oaid != 0)
            {
                var tin = db.OAEquipGeneralData.Where(m => m.OAID == oaid).Select(m => m.TinNumber).Single();
                ViewBag.TinNumber = tin;
                OA quo = new OA();
                var qgdb = db.OAEquipGeneralData.Find(oaid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.OAEquipTableData.Where(m => m.OAID == oaid);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.OAEquipGeneralData = qgdb;
                    quo.OAEquipPayment = db.OAEquipPayment.Where(m => m.OAID == oaid).Single();
                    if (modelcnt == 4)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.OAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.OAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.OAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = modelcount[2];
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = null;
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
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
                    ViewBag.QuotationNumber = quotnum;
                    ViewBag.OANumber = oanum;
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
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                OA quo = new OA();
                var qgdb1 = db.OAEquipGeneralData.Where(m => m.OANumber == quotnumber).Select(m => m.OAID).SingleOrDefault();
                var qgdb = db.OAEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.OAEquipTableData.Where(m => m.OAID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.OAEquipGeneralData = qgdb;
                    quo.OAEquipPayment = db.OAEquipPayment.Find(qgdb1);
                    if (modelcnt == 4)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.OAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.OAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.OAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = modelcount[2];
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = modelcount[1];
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.OAEquipTableData1 = modelcount[0];
                        quo.OAEquipTableData2 = null;
                        quo.OAEquipTableData3 = null;
                        quo.OAEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
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
                    ViewBag.QuotationNumber = quotnum;
                    ViewBag.OANumber = oanum;
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
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        // Post: /OA/OACheck for Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult OACheck(OA maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.OAEquipTableData1.ProductModelID != 0)
                {
                    var Oagen = maindatabase.OAEquipGeneralData;
                    db.Entry(Oagen).State = EntityState.Modified;
                    db.SaveChanges();

                    var Oapay = maindatabase.OAEquipPayment;
                    db.Entry(Oapay).State = EntityState.Modified;
                    db.SaveChanges();

                    var Oatab1 = maindatabase.OAEquipTableData1;
                    db.Entry(Oatab1).State = EntityState.Modified;
                    db.SaveChanges();
                    if (maindatabase.OAEquipTableData2.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.OAEquipTableData2).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.OAEquipTableData3.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.OAEquipTableData3).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.OAEquipTableData4.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.OAEquipTableData4).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Removing Quotation from OA View after it is sent for Approval
                    int qgid = maindatabase.OAEquipGeneralData.QGID;
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == qgid).ToList();
                    foreach (var qg in quogen)
                    {
                        qg.Ordergenerated = 1;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var oaappst = maindatabase.OAEquipGeneralData;
                    oaappst.OAStatus = 1;
                    DateTime quotdat = System.DateTime.Now;
                    oaappst.Approvaldate = quotdat.ToString("yyyy-MM-dd");
                    db.Entry(oaappst).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Request.Form["btnapproval"] != null)
                    {
                        var oagen = maindatabase.OAEquipGeneralData;
                        oagen.ApprovalStatus = 1;
                        db.Entry(oagen).State = EntityState.Modified;
                        db.SaveChanges();
                        int oaid1 = oagen.OAID;
                        return RedirectToAction("OAApproveComment", "OA", new { oaid = oaid1 });
                        //TempData["oaapproved"] = "Order Acknowledgement Approved Successfully!!!!";
                        //
                        //Opening PDF in new window
                        //var oaid1 = maindatabase.OAEquipGeneralData.OAID;
                        //String updateparent = "<script>window.location.href='OAApproval',window.open('/OA/OAReportGeneration?oaid=" + oaid1 + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes');</script>";
                        //return Content(updateparent);
                    }
                    else if (Request.Form["btnrejected"] != null)
                    {
                        var oagen = maindatabase.OAEquipGeneralData;
                        oagen.ApprovalStatus = 2;
                        db.Entry(oagen).State = EntityState.Modified;
                        db.SaveChanges();
                        int oaid1 = oagen.OAID;
                        return RedirectToAction("OARejectComment", "OA", new { oaid = oaid1 });
                        //TempData["oarejected"] = "Order Acknowledgement Rejected Successfully!!!!";
                    }
                }
                return RedirectToAction("OAApproval");
            }
            else
            {
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.OAID = maindatabase.OAEquipGeneralData.OAID;
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
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        //
        //Report Generation
        public ActionResult OAReportGeneration(int? oaid)
        {
            if (oaid != 0)
            {
                OA quo = new OA();
                RepModel RM = new RepModel();
                var qgdb = db.OAEquipGeneralData.Find(oaid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var cpid = qgdb.CPID;
                    var Logo = db.ChannelPartners.Find(cpid);

                    var channelPartnerLogo = "";
                    var Logopath = "";

                    var add1 = "Check Your Address";

                    // Logopath = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    // add1 = Logo.footaddress;

                    channelPartnerLogo = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                    add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";

                    var apvl = db.OAEquipGeneralData.Where(m => m.OAID == oaid).Select(m => m.ApprovalStatus).Single();
                    ViewBag.Apstat = apvl;

                    //Logopath = Server.MapPath("~/App_Data/Buhler_RGB_100.png");
                    //add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";
                    var models = db.OAEquipTableData.Where(m => m.OAID == oaid);
                    var modelcount = models.ToList();
                    quo.OAEquipGeneralData = qgdb;
                    var paymentterms = db.OAEquipPayment.Where(m => m.OAID == oaid).First();
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    RM.Logo = Logopath;
                    RM.footaddress = add1;
                    RM.QGID = qgdb.QGID;
                    RM.Tinnumber = qgdb.TinNumber;
                    RM.Approvaldate = Convert.ToDateTime(qgdb.Approvaldate).ToString("dd-MM-yyyy");
                    RM.annexure = paymentterms.annexure;
                    RM.CPQuotationNumber = qgdb.CPQuotationNumber;
                    RM.MDBID = mdbdet.MDBID.ToString();
                    RM.QuotationNumber = qgdb.QuotationNumber;
                    RM.OANmber = qgdb.OANumber;
                    RM.QuotationDate = Convert.ToDateTime(qgdb.QuotationDate).ToString("dd-MM-yyyy");
                    RM.PaymentTerms = paymentterms.PaymentTerms;
                    RM.Delivery = paymentterms.Delivery;
                    RM.DateofDispatch = paymentterms.DateofDispatch;
                    RM.Transport = paymentterms.Transport;
                    RM.Freight = paymentterms.Freight;
                    RM.CST = paymentterms.CST;
                    RM.TransitInsu = paymentterms.TransitInsu;
                    RM.Commodity = paymentterms.Commodity;
                    RM.Validity = paymentterms.Validity;
                    RM.Subjectinfo = qgdb.Subjectinfo;
                    RM.KindAttention = qgdb.KindAttention;
                    RM.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    RM.State = mdbdet.State;
                    RM.Pincode = mdbdet.Pincode;
                    RM.overallprice = paymentterms.overallprice;
                    RM.City = mdbdet.City;
                    RM.AddressLine1 = mdbdet.AddressLine1;
                    RM.AddressLine2 = mdbdet.AddressLine2;
                    RM.AddressLine3 = mdbdet.AddressLine3;
                    RM.OrganizationName = mdbdet.OrganizationName;
                    RM.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
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
                    RM.OAEquipTableData = modelcount;
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

                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.CPContactPersonData.Where(m => m.CPID == cpid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                    RM.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                    RM.Designation = loginname.Designation;
                    var FileName = RM.OANmber;
                    ViewBag.CPID = loginname.CPID;
                    var relativePath = "OAReportGeneration";
                    string content;
                    var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                    ViewData.Model = RM;

                    String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                    //String footerpath = Server.MapPath("~/App_Data/FooterFile.html");
                    string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
                        //"<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 140px\"><div style=\"width: 700px; float: left; height: 140px\"></div><div style=\"width: 300px; height: 140px; float: right\"><img src= \"" + Logopath + "\" width=\"200\" height=\"60\" /></div></div></body>";
                    //string htmlfooter = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 60px;width: 1000px\"><div style=\"width: 400px; float: left; height: 100px\"> " + add1 + " </div><div style=\"width: 300px; height: 60px; float: right\">On Behalf of <img src= \"" + Logopath1 + "\" width=\"300\" height=\"100\" /></div></div></body>";

                    if (System.IO.File.Exists(headerpath))
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
                        var pdfconverter = new PDFConverter2();
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

        //
        //OA Rejection comment
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult OARejectComment(int oaid)
        {
            var oa = db.OAEquipGeneralData.Where(m => m.OAID == oaid).Single();
            ViewBag.OAID = oaid;
            return View(oa);
        }

        //
        //OA Rejection Comment
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult OARejectComment(OAEquipGeneralData oagendata)
        {
            db.Entry(oagendata).State = EntityState.Modified;
            db.SaveChanges();
            TempData["oarejected"] = "Order Acknowledgement Rejected Successfully!!!!";
            return RedirectToAction("OAApproval");
        }


        //
        //OA Approval comment
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult OAApproveComment(int oaid)
        {
            var oa = db.OAEquipGeneralData.Where(m => m.OAID == oaid).Single();
            ViewBag.OAID = oaid;
            return View(oa);
        }

        //
        //OA Approval Comment
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult OAApproveComment(OAEquipGeneralData oagendata)
        {
            db.Entry(oagendata).State = EntityState.Modified;
            db.SaveChanges();
            TempData["oarejected"] = "Order Acknowledgement Approved Successfully!!!!";
            double count = 1;
            var check = db.OAEquipTableData.Where(m => m.OAID == oagendata.OAID).ToList();
            foreach (var oa in check)
            {
                var productmodel = db.ProductModel.ToList();
                foreach (var pd in productmodel)
                {
                    if (oa.ProductModelID == pd.ProductModelID)
                    {
                        pd.ApprovedCount = Convert.ToInt32(count) + Convert.ToInt32(pd.ApprovedCount);
                        if (pd.ApprovedCount >= pd.MachineCount)
                        {
                            pd.PR = pd.ApprovedCount - pd.MachineCount;
                        }
                        else
                        {
                            pd.PR = 0;
                        }
                        db.Entry(pd).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("OAApproval");
        }
        public string monthtodate(string month)
        {
            string mondat = null;
            string[] mon = month.Split(new char[] { ' ', ',' });

            switch (mon[0])
            {
                case "Jan": mondat = mon[1] + "-01-01";
                    break;
                case "Feb": mondat = mon[1] + "-02-01";
                    break;
                case "Mar": mondat = mon[1] + "-03-01";
                    break;
                case "Apr": mondat = mon[1] + "-04-01";
                    break;
                case "May": mondat = mon[1] + "-05-01";
                    break;
                case "Jun": mondat = mon[1] + "-06-01";
                    break;
                case "Jul": mondat = mon[1] + "-07-01";
                    break;
                case "Aug": mondat = mon[1] + "-08-01";
                    break;
                case "Sep": mondat = mon[1] + "-09-01";
                    break;
                case "Oct": mondat = mon[1] + "-10-01";
                    break;
                case "Nov": mondat = mon[1] + "-11-01";
                    break;
                case "Dec": mondat = mon[1] + "-12-01";
                    break;
            }
            return mondat;
        }

        //for to date
        public string monthtodate2(string month)
        {
            string mondat = null;
            string[] mon = month.Split(new char[] { ' ', ',' });

            switch (mon[0])
            {
                case "Jan": mondat = mon[1] + "-01-31";
                    break;
                case "Feb": mondat = mon[1] + "-02-29";
                    break;
                case "Mar": mondat = mon[1] + "-03-31";
                    break;
                case "Apr": mondat = mon[1] + "-04-30";
                    break;
                case "May": mondat = mon[1] + "-05-31";
                    break;
                case "Jun": mondat = mon[1] + "-06-30";
                    break;
                case "Jul": mondat = mon[1] + "-07-31";
                    break;
                case "Aug": mondat = mon[1] + "-08-31";
                    break;
                case "Sep": mondat = mon[1] + "-09-30";
                    break;
                case "Oct": mondat = mon[1] + "-10-31";
                    break;
                case "Nov": mondat = mon[1] + "-11-30";
                    break;
                case "Dec": mondat = mon[1] + "-12-31";
                    break;
            }
            return mondat;
        }
        //
        //ALL Orders View
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult OAAllView(string cpnam = null, int status = -1, string frommonth = null, string tomonth = null, string equip = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;

            var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
            ViewBag.EquipModel = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            ViewBag.equip = equip;
            ViewBag.fromdte = frommonth;
            ViewBag.todate = tomonth;
            var oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).OrderByDescending(m => m.OAID).ToList();
            if (cpnam != null && cpnam != "")
            {
                ViewBag.IsSearch = true;
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                oagendata = db.OAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OAStatus == 1).OrderByDescending(m => m.OAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("OAAllView", "OA");
                }
                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
                ViewBag.status = status;
            }
            if (status != -1)
            {
                int stats = Convert.ToInt32(status);
                oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OAID).ToList();

                ViewBag.status = status;
                TempData["byjt"] = "Order Acknowledgement for selected status is";
            }
            if (equip != null && equip != "")
            {
                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip && s.OAStatus == 1
                             select s).ToList();
                ViewBag.status = status;
                //return View(model);
            }
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.status = status;
                oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }
            }
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && status != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                int stats = Convert.ToInt32(status);
                ViewBag.status = status;
                oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.OADate >= frmo && m.OADate <= tomo).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }
            }
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                int stats = Convert.ToInt32(status);
                ViewBag.status = status;
                //oagendata = db.OAEquipGeneralData.Where(m => m.OADate >= frmo && m.OADate <= tomo).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OADate).ToList();
                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.OADate >= frmo && s.OADate <= tomo && s.OAStatus == 1
                             where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }
            }
            if (cpnam != null && cpnam != "" && status != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.status = status;
                int stats = Convert.ToInt32(status);
                oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.CPID == channame).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }

            }
            if (cpnam != null && cpnam != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.status = status;
                //int stats = Convert.ToInt32(status);
                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.CPID == channame && s.OAStatus == 1
                             where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();

                //var sotlist = db.OAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();
                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }
                //return View(sotlist);
            }
            if (equip != null && equip != "" && status != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.status = status;
                int stats = Convert.ToInt32(status);
                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.ApprovalStatus == stats && s.OAStatus == 1
                             where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();

                //var sotlist = db.OAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();
                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }
                //return View(sotlist);
            }
            if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.status = status;
                int stats = Convert.ToInt32(status);
                oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("OAAllView", "OA");
                }
                //return View(sotlist);
            }
            if (cpnam != null && cpnam != "" && status != -1 && equip != null && equip != "")
            {
                ViewBag.IsSearch = true;
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                int stats = Convert.ToInt32(status);

                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.ApprovalStatus == stats
                             where s.CPID == channame && s.OAStatus == 1
                             where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                //oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).OrderByDescending(m => m.OAID).ToList();
                if (oagendata == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("OAAllView", "OA");
                }
                var cpid = chpart.CPID;

                TempData["byjt"] = null;
                TempData["cp&byjt"] = "Order Acknowledgement for selected Channel Partner and Status is";

                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
                ViewBag.status = status;
            }
            if (cpnam != null && cpnam != "" && status != -1 && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.IsSearch = true;
                int stats = Convert.ToInt32(status);
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.OAID == b.OAID
                             where s.ApprovalStatus == stats && s.OAStatus == 1
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.CPID == channame && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("OAAllView", "OA");
                }
                var cpid = chpart.CPID;

                TempData["byjt"] = null;
                TempData["cp&byjt"] = "Order Acknowledgement for selected Channel Partner and Status is";

                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
                ViewBag.status = status;
            }
            if (status != -1 && frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));

                //ViewBag.IsSearch = true;
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                int stats = Convert.ToInt32(status);

                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.ApprovalStatus == stats && s.OAStatus == 1
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                //oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).OrderByDescending(m => m.OAID).ToList();
                if (oagendata == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("OAAllView", "OA");
                }
                //var cpid = chpart.CPID;

                TempData["byjt"] = null;
                TempData["cp&byjt"] = "Order Acknowledgement for selected Channel Partner and Status is";

                //ViewBag.OrganizationName = chpart.CPName;
                //ViewBag.AddressLine1 = chpart.AddressLine1;
                //ViewBag.AddressLine2 = chpart.AddressLine2;
                //ViewBag.AddressLine3 = chpart.AddressLine3;
                //ViewBag.City = chpart.City;
                //ViewBag.Pincode = chpart.PinCode;
                //ViewBag.State = chpart.State;
                //ViewBag.status = status;
            }
            if (cpnam != null && cpnam != "" && status != -1 && frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.IsSearch = true;

                int stats = Convert.ToInt32(status);
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.OAID == b.OAID
                             where s.ApprovalStatus == stats && s.OAStatus == 1
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.CPID == channame && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                //oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("OAAllView", "OA");
                }
                var cpid = chpart.CPID;

                TempData["byjt"] = null;
                TempData["cp&byjt"] = "Order Acknowledgement for selected Channel Partner and Status is";

                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
                ViewBag.status = status;
            }
            if (cpnam != null && cpnam != "" && status != -1 && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate2(tomonth));
                ViewBag.IsSearch = true;
                int stats = Convert.ToInt32(status);
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

                oagendata = (from s in db.OAEquipGeneralData
                             from b in db.OAEquipTableData
                             where s.OAID == b.OAID
                             where s.ApprovalStatus == stats && s.OAStatus == 1
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.CPID == channame && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("OAAllView", "OA");
                }
                var cpid = chpart.CPID;

                TempData["byjt"] = null;
                TempData["cp&byjt"] = "Order Acknowledgement for selected Channel Partner and Status is";

                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
                ViewBag.status = status;
            }
            return View(oagendata);
        }

        public ActionResult ExportData(string cpnam = null, int status = -1, string frommonth = null, string tomonth = null, string equip = null)
        {
            GridView gv = new GridView();
            if ((cpnam != null && cpnam != "" || status != -1 || equip != null && equip != "" || frommonth != null && frommonth != "" && tomonth != null && tomonth != ""))
            {
                if (cpnam != null && cpnam != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;

                    var duplicate = (from s in db.OAEquipGeneralData
                                     where s.CPID == channame
                                     where s.OAStatus == 1
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         //s.ChannelPartners.CPName,
                                         cname,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.OrganizationName;
                        dr[3] = d.QuotationNumber;
                        dr[4] = d.OADate;
                        if (status == -1)
                        {
                            dr[5] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[5] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[5] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[5] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[5] = "Pending";
                        }
                        dr[6] = d.cname;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

                if (status != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    int stats = Convert.ToInt32(status);
                    var oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OAID).ToList();

                    var duplicate = (from s in db.OAEquipGeneralData
                                     where s.ApprovalStatus == status && s.OAStatus == 1
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                     });

                    var dt1 = duplicate.ToList();
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.OrganizationName;
                        dr[3] = d.QuotationNumber;
                        dr[4] = d.OADate;
                        if (status == -1)
                        {
                            dr[5] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[5] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[5] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[5] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[5] = "Pending";
                        }
                        dr[6] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (cpnam != null && cpnam != "" && status != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));


                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;

                    int stats = Convert.ToInt32(status);
                    var oagendata = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == stats).Where(m => m.CPName == cpnam).OrderByDescending(m => m.OAID).ToList();

                    var duplicate = (from s in db.OAEquipGeneralData
                                     where (s.CPID == channame)
                                     where s.ApprovalStatus == status && s.OAStatus == 1
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         cname,
                                     });

                    var dt1 = duplicate.ToList();
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.OrganizationName;
                        dr[3] = d.QuotationNumber;
                        dr[4] = d.OADate;
                        if (status == -1)
                        {
                            dr[5] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[5] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[5] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[5] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[5] = "Pending";
                        }
                        dr[6] = d.cname;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (equip != null && equip != "")
                {
                    DataTable dts = new DataTable();

                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    var duplicate = (from s in db.OAEquipGeneralData
                                     from b in db.OAEquipTableData
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip && s.OAStatus == 1
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                     });

                    var dt1 = duplicate.ToList();
                    int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (stats == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (stats == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (stats == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (equip != null && equip != "" && status != -1)
                {
                    DataTable dts = new DataTable();

                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));
                    var duplicate = (from s in db.OAEquipGeneralData
                                     from b in db.OAEquipTableData
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip && s.OAStatus == 1
                                     where s.ApprovalStatus == status
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                     });

                    var dt1 = duplicate.ToList();
                    int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (stats == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (stats == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (stats == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (equip != null && equip != "" && cpnam != null && cpnam != "")
                {

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;
                    DataTable dts = new DataTable();

                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));
                    var duplicate = (from s in db.OAEquipGeneralData
                                     from b in db.OAEquipTableData
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip && s.OAStatus == 1
                                     where (s.CPID == channame)
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         cname,
                                     });

                    var dt1 = duplicate.ToList();
                    int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (stats == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (stats == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (stats == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.cname;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    //var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    //var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    //var cname = chpart.CPName;
                    var duplicate = (from s in db.OAEquipGeneralData
                                     where s.OADate >= frmo && s.OADate <= tomo && s.OAStatus == 1
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.OrganizationName;
                        dr[3] = d.QuotationNumber;
                        dr[4] = d.OADate;
                        if (status == -1)
                        {
                            dr[5] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[5] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[5] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[5] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[5] = "Pending";
                        }
                        dr[6] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && status != -1)
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    //var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    //var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    //var cname = chpart.CPName;
                    var duplicate = (from s in db.OAEquipGeneralData
                                     where s.ApprovalStatus == status
                                     where s.OADate >= frmo && s.OADate <= tomo && s.OAStatus == 1
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.OrganizationName;
                        dr[3] = d.QuotationNumber;
                        dr[4] = d.OADate;
                        if (status == -1)
                        {
                            dr[5] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[5] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[5] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[5] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[5] = "Pending";
                        }
                        dr[6] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "")
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    //var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    //var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    //var cname = chpart.CPName;
                    var duplicate = (from s in db.OAEquipGeneralData
                                     from b in db.OAEquipTableData
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip && s.OAStatus == 1
                                     where s.OADate >= frmo && s.OADate <= tomo
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                         //cname,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && cpnam != null && cpnam != "")
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    //dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;
                    var duplicate = (from s in db.OAEquipGeneralData
                                     where (s.CPID == channame)
                                     where s.OADate >= frmo && s.OADate <= tomo && s.OAStatus == 1
                                     select new
                                      {
                                          s.OANumber,
                                          s.Subjectinfo,
                                          //equip,
                                          s.MDBGeneralData.OrganizationName,
                                          s.QuotationNumber,
                                          s.OADate,
                                          s.ApprovalStatus,
                                          //s.ChannelPartners.CPName,
                                          cname,
                                      });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        //dr[2] = d.equip;
                        dr[2] = d.OrganizationName;
                        dr[3] = d.QuotationNumber;
                        dr[4] = d.OADate;
                        if (status == -1)
                        {
                            dr[5] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[5] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[5] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[5] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[5] = "Pending";
                        }
                        dr[6] = d.cname;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (cpnam != null && cpnam != "" && status != -1 && equip != null && equip != "")
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;
                    var duplicate = (from s in db.OAEquipGeneralData
                                     where s.ApprovalStatus == status
                                     from b in db.OAEquipTableData
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip && s.OAStatus == 1
                                     where (s.CPID == channame)
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                         cname,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.cname;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "" && status != -1)
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    //var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    //var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    //var cname = chpart.CPName;                     
                    var duplicate = (from s in db.OAEquipGeneralData
                                     from b in db.OAEquipTableData
                                     where s.ApprovalStatus == status
                                     where s.OADate >= frmo && s.OADate <= tomo && s.OAStatus == 1
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                         //cname,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "" && status != -1 && cpnam != null && cpnam != "")
                {
                    DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Order Acknowledgement No", typeof(String));
                    dts.Columns.Add("Subject", typeof(String));
                    dts.Columns.Add("Equip Model", typeof(String));
                    dts.Columns.Add("Customer Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("OA Date", typeof(String));
                    dts.Columns.Add("Status", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;
                    var duplicate = (from s in db.OAEquipGeneralData
                                     from b in db.OAEquipTableData
                                     where s.OADate >= frmo && s.OADate <= tomo
                                     where s.ApprovalStatus == status
                                     where s.CPID == channame && s.OAStatus == 1
                                     where s.OAID == b.OAID && b.ProductModel.ProductModelName == equip
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
                                         equip,
                                         s.MDBGeneralData.OrganizationName,
                                         s.QuotationNumber,
                                         s.OADate,
                                         s.ApprovalStatus,
                                         s.ChannelPartners.CPName,
                                         cname,
                                     });

                    var dt1 = duplicate.ToList();
                    //int stats = Convert.ToInt32(status);
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OANumber;
                        dr[1] = d.Subjectinfo;
                        dr[2] = d.equip;
                        dr[3] = d.OrganizationName;
                        dr[4] = d.QuotationNumber;
                        dr[5] = d.OADate;
                        if (status == -1)
                        {
                            dr[6] = d.ApprovalStatus;
                            if (d.ApprovalStatus == 1)
                            {
                                dr[6] = "Approved";
                            }
                            else if (d.ApprovalStatus == 2)
                            {
                                dr[6] = "Rejected";
                            }
                            else if (d.ApprovalStatus == 0)
                            {
                                dr[6] = "Pending";
                            }
                        }
                        if (status == 1)
                        {
                            dr[6] = "Approved";
                        }
                        else if (status == 2)
                        {
                            dr[6] = "Rejected";
                        }
                        else if (status == 0)
                        {
                            dr[6] = "Pending";
                        }
                        dr[7] = d.cname;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
            }
            else
            {

                DataTable dts = new DataTable();
                dts.Columns.Add("Order Acknowledgement No", typeof(String));
                dts.Columns.Add("Subject", typeof(String));
                dts.Columns.Add("Customer Name", typeof(String));
                dts.Columns.Add("Quotation Number", typeof(String));
                dts.Columns.Add("OA Date", typeof(String));
                dts.Columns.Add("Status", typeof(String));
                dts.Columns.Add("Channel Partner", typeof(String));

                var duplicate = (from s in db.OAEquipGeneralData
                                 where s.OAStatus == 1
                                 select new
                                 {
                                     s.OANumber,
                                     s.Subjectinfo,
                                     s.MDBGeneralData.OrganizationName,
                                     s.QuotationNumber,
                                     s.OADate,
                                     s.ApprovalStatus,
                                     s.ChannelPartners.CPName,
                                 });

                var dt1 = duplicate.ToList();
                //int stats = Convert.ToInt32(status);
                foreach (var d in duplicate)
                {
                    DataRow dr = dts.NewRow();
                    dr[0] = d.OANumber;
                    dr[1] = d.Subjectinfo;
                    dr[2] = d.OrganizationName;
                    dr[3] = d.QuotationNumber;
                    dr[4] = d.OADate;
                    if (status == -1)
                    {
                        dr[5] = d.ApprovalStatus;
                        if (d.ApprovalStatus == 1)
                        {
                            dr[5] = "Approved";
                        }
                        else if (d.ApprovalStatus == 2)
                        {
                            dr[5] = "Rejected";
                        }
                        else if (d.ApprovalStatus == 0)
                        {
                            dr[5] = "Pending";
                        }
                    }
                    if (status == 1)
                    {
                        dr[5] = "Approved";
                    }
                    else if (status == 2)
                    {
                        dr[5] = "Rejected";
                    }
                    else if (status == 0)
                    {
                        dr[5] = "Pending";
                    }
                    dr[6] = d.CPName;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();

            }
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=OAAllView.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("OAAllView");
        }

        //
        //ChannelPartner Pending/ approved/ Rejected Report
        //
        public ActionResult OAChannelStatus(int status = -1)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var oagendata = db.OAEquipGeneralData.Where(m => m.CPID == loginname.CPID).Where(m => m.OAStatus == 1).OrderByDescending(m => m.OAID).ToList();
            if (status != -1)
            {
                oagendata = db.OAEquipGeneralData.Where(m => m.CPID == loginname.CPID).Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == status).OrderByDescending(m => m.OAID).ToList();
                if (oagendata.Count != 0)
                {
                    TempData["status"] = "Order Acknowledgement for selected status is";
                }
                else
                {
                    TempData["nostatus"] = "No Order Acknowledgement Exists for this selected value";
                }
            }
            return View(oagendata);
        }

        //ChannelPartner Pending/ approved/ Rejected Report
        //
        const int pageSizem = 25;
        bool getdetailsclickm = false;
        public ActionResult MachineDispatchTracker(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            //var oagendata = db.MachineDispatch.Where(m => m.CPID == loginname.CPID).ToList();
            IEnumerable<MachineDispatch> quotations = db.MachineDispatch.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.MachineDispatchID) : quotations.OrderByDescending(p => p.MachineDispatchID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.OANumber) : quotations.OrderByDescending(p => p.OANumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.InvoiceDate) : quotations.OrderByDescending(p => p.InvoiceDate);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.InvoiceNumber) : quotations.OrderByDescending(p => p.InvoiceNumber);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.Transporter) : quotations.OrderByDescending(p => p.Transporter);
                    break;

                case 7:
                    quotations = isAsc ? quotations.OrderBy(p => p.LRNumber) : quotations.OrderByDescending(p => p.LRNumber);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.MachineDispatchID) : quotations.OrderByDescending(p => p.MachineDispatchID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSizem);

            quotations = quotations
                .Skip((page - 1) * pageSizem)
                .Take(pageSizem)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSizem;

            ViewBag.Search = cunam;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;

            if (getdetailsclickm)
                ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclickm = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.OAID).ToList());

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult OACancel(int oaid)
        {
            var oa = db.OAEquipGeneralData.Where(m => m.OAID == oaid).Single();
            ViewBag.OAID = oaid;
            return View(oa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult OACancel(OAEquipGeneralData oaequip, string comments = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            oaequip.ApprovalStatus = 3;//cancellation of OA order
            oaequip.OARejectComm = comments;
            db.Entry(oaequip).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("OAApproval", "OA", null);
        }

    }
    //Pdf
    public interface IPDFConverter2
    {
        byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml);
    }

    public class PDFConverter2 : IPDFConverter2
    {
        private const string HtmlToPdfExePath = "wkhtmltopdf.exe";
        private readonly ILog log = LogManager.GetLogger(typeof(PDFConverter2));

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
            //args += "--margin-top 5mm --margin-bottom 5mm --margin-right 5mm --margin-left 5mm --footer-html \"" + FooterHtml + "\" --footer-right \"Page [page]/[topage]\" --footer-font-name \"Calibri\" --footer-font-size \"10\" --header-html \"" + HeaderHtml + "\" ";
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
