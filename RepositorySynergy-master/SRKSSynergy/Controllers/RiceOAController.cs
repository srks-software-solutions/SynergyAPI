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
using System.Data.Entity.Validation;
using System.Globalization;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class RiceOAController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        //Getting Polisher Model
        #region
        public JsonResult GetPolisherModel(string ProdID, string GType, string process, string capacity, string pass, string polishrequirement, string motortype, string prodmodelid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            int pmid = Convert.ToInt32(prodmodelid);
            ProductModel ProductModel = db.ProductModel.Find(pmid);
            var prodname = ProductModel.ProductModelName;
            String[] prodnamesplit = prodname.Split('/');
            var motorrating = prodnamesplit[1];
            var motorq = prodnamesplit[2];

            motorrating = motorrating.Trim();
            motorq = motorq.Trim();

            var result = (from r in db.OCMPolisher
                          where r.GrainType == GType && (r.Process == process) && (r.Pass == pass) && (r.Capacity == capacity)
                          && (r.PolishRequirement == polishrequirement) && (r.MotorType == motortype) && (r.MotorQ == motorq) && (r.MotorRating == motorrating)
                          && (r.IsDeleted == 0)
                          select new { r.OPID }).SingleOrDefault();
            if (result == null)
            {
                var msg = "No Model Available. Select Proper Model....!";
                var data = new
                {
                    err = msg,
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWhitnerMoodel(string ProdID, string GType, string process, string capacity, string pass, string motortype, string prodmodelid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            int pmid = Convert.ToInt32(prodmodelid);
            ProductModel ProductModel = db.ProductModel.Find(pmid);
            var prodname = ProductModel.ProductModelName;
            String[] prodnamesplit = prodname.Split('/');

            var motorrating = prodnamesplit[1];
            var motorq = prodnamesplit[2];

            motorrating = motorrating.Trim();
            motorq = motorq.Trim();

            var result = (from r in db.OCMWhitner
                          where r.GrainType == GType && (r.Process == process) && (r.Pass == pass) && (r.Capacity == capacity)
                          && (r.MotorType == motortype) && (r.MotorQ == motorq) && (r.MotorRating == motorrating)
                          && (r.IsDeleted == 0)
                          select new { r.OWID }).SingleOrDefault();
            if (result == null)
            {
                var msg = "No Model Available. Select Proper Model....!";
                var data = new
                {
                    err = msg,
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Json AutoComplete customer unique id
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
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete quotation number
        public JsonResult Autocomplete2(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.RiceOAEquipGeneralData
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

        //Json AutoComplete RiceO Customer Name
        public JsonResult Autocompleteapp(string term)
        {
            var result = (from o in db.RiceOAEquipGeneralData
                          where o.MDBGeneralData.OrganizationName.Contains(term.ToLower()) && o.ApprovalStatus != 2 && o.ApprovalStatus != 1 && o.OAStatus != 0
                          select new { o.MDBGeneralData.OrganizationName }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete Channel Partner
        public JsonResult Autocompleteappprint(string term)
        {
            var result = (from o in db.RiceOAEquipGeneralData
                          where o.MDBGeneralData.OrganizationName.Contains(term.ToLower()) && o.ApprovalStatus != 2 && o.ApprovalStatus != 0 && o.OAStatus != 0
                          select new { o.MDBGeneralData.OrganizationName }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Product Model Details Update Dynamically in the View
        [HttpGet]
        public JsonResult GetProductModelDetails(int id)
        {
            var selectedRow = (from t in db.ProductModel where t.ProductModelID == id select t).SingleOrDefault();

            var jsonData = new
            {
                unitprice = selectedRow.UnitPrice,
                Desc = selectedRow.ProductModelDesc,
                Exclusion = selectedRow.ProductModelExclusion
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //json euip model
        public JsonResult AutocompleteEquipModel(string term)
        {
            var result = (from r in db.ProductModel
                          where r.ProductModelName.ToLower().Contains(term.ToLower())
                          select new { r.ProductModelName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Product Details Update on click of MasterProduct in the View
        [HttpPost]
        public ActionResult GetMasterProductDetails(string prdts)
        {
            IEnumerable<SelectListItem> Productlist = new List<SelectListItem>();
            db = new SRKS_Synergy();
            if (!string.IsNullOrEmpty(prdts))
            {
                Productlist = (from m in db.Products where m.MasterProducts.MasterProductName == prdts select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.ProductName, Value = m.ProductID.ToString() });
            };
            var result = Json(new SelectList(Productlist, "Value", "Text"));
            return result;
        }

        //Product Model Details Update on click of Product in the View
        [HttpPost]
        public ActionResult GetProductDetails(string prdtmd)
        {
            IEnumerable<SelectListItem> ProductModellist = new List<SelectListItem>();
            db = new SRKS_Synergy();
            if (!string.IsNullOrEmpty(prdtmd))
            {
                ProductModellist = (from m in db.ProductModel where m.Products.ProductName == prdtmd select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.ProductModelName, Value = m.ProductModelID.ToString() });
            };
            var result = Json(new SelectList(ProductModellist, "Value", "Text"));
            return result;
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

        // GET: /RiceOA/RiceOAView
        const int pageSize = 25000;
        bool getdetailsclick = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult RiceOAView(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            IEnumerable<QGEquipGeneralData> quotations = db.QGEquipGeneralData.Where(p => cunam == null || p.MDBGeneralData.OrganizationName.Contains(cunam))
                .Where(m => m.CPID == loginname.CPID).Where(m => m.Islatest != 1).Where(m => m.QuotStatus == 0).Where(m => m.Ordergenerated != 1).Where(m => m.IsRiceMill == 1).Include(q => q.MDBGeneralData);

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

        // GET: /RiceOA/RiceOAApproval
        const int pageSize1 = 30000;
        bool getdetailsclick1 = false;
        //Sorting Paging & Searching
        //[HttpGet]
        public ActionResult RiceOAApproval(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<RiceOAEquipGeneralData> quotations = db.RiceOAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m=>m.Islatest==0).Where(m => m.ApprovalStatus != 2).Where(m => m.ApprovalStatus != 1).Where(m => m.OAStatus != 0).Include(q => q.MDBGeneralData).OrderByDescending(m => m.OADate);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.ROAID) : quotations.OrderByDescending(p => p.ROAID);
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
                    quotations = isAsc ? quotations.OrderBy(p => p.ROAID) : quotations.OrderByDescending(p => p.ROAID);
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
        const int pageSize1p = 30000;
        bool getdetailsclick1p = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult RiceOAApprovalPrint(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<RiceOAEquipGeneralData> quotations = db.RiceOAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.ApprovalStatus != 2).Where(m => m.ApprovalStatus != 0).Where(m => m.OAStatus != 0).Include(q => q.MDBGeneralData).OrderByDescending(m => m.OADate);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.ROAID) : quotations.OrderByDescending(p => p.ROAID);
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
                    quotations = isAsc ? quotations.OrderBy(p => p.ROAID) : quotations.OrderByDescending(p => p.ROAID);
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
            return View(quotations.OrderByDescending(m => m.Approvaldate).ToList());
        }

        //GET: /OA/OAApproval
        //const int pageSize2 = 25;
        bool getdetailsclick2 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult RiceOAExisting(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();


            IEnumerable<RiceOAEquipGeneralData> quotations = db.RiceOAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.Islatest != 1).Where(m => m.OAStatus != 0).Where(m => m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.ROAID) : quotations.OrderByDescending(p => p.ROAID);
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
                    quotations = isAsc ? quotations.OrderBy(p => p.ROAID) : quotations.OrderByDescending(p => p.ROAID);
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
            return View(quotations.OrderByDescending(m => m.ROAID).ToList());
        }

        //Quotation numb check valid
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

        //  GET: /RiceOA/RiceOAGenerate
        public ActionResult RiceOAGenerate(int qgid = 0, int productid = 0, int prodmodelid = 0, int masterprodid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            int overallpricep = 0; int op = 0;
            string overallpricestring = null;
            //Pushing data into RiceOA table from QGTable
            RiceOA quo = new RiceOA();
            //var oaid2 = db.RiceOAEquipGeneralData.Where(m => m.RQGID == qgid).Select(m => m.ROAID).SingleOrDefault();

            //Fetching date from the Quotation Generaal and Table Data.

            //var QGEquipTableDataList1 = db.QGEquipTableData.Where(m => m.QGID == qgid).ToList();
            //foreach (var tbl in QGEquipTableDataList1)
            //{
                // Getting the RiceOA individual details
                //var RiceTblData1 = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == tbl.QGID).Where(m => m.ProductID == tbl.ProductID).Where(m => m.ProductModelID == tbl.ProductModelID).Where(m => m.MasterProductID == tbl.MasterProductID).Where(m => m.IsSOTStatus == 0).Select(m => m.ROATBID).SingleOrDefault();
            int y = 0;
            var oaid2 = y;
            if (masterprodid == 5 || masterprodid == 7)
            {
                var riceoalist = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == qgid).Where(m => m.ProductID == productid).Where(m => m.ProductModelID == prodmodelid).Where(m => m.MasterProductID == masterprodid).ToList();

                    foreach (var r in riceoalist)
                    {
                        if (oaid2 == 0)
                        {
                            oaid2 = r.RiceOAEquipGeneralData.ROAID;
                        }
                    }
            }
            else
            {
                oaid2 = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == qgid).Where(m => m.ProductID == productid).Where(m => m.ProductModelID == prodmodelid).Where(m => m.MasterProductID == masterprodid).Select(m => m.RiceOAEquipGeneralData.ROAID).SingleOrDefault();
            }
            //}

            if (qgid != 0)
            {
                //Showing Tin Number if Present in Master Data Base
                int mdbidt = db.QGEquipGeneralData.Where(m => m.QGID == qgid).Select(m => m.MDBID).Single();
                var tinnum = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbidt).Select(m => m.TIN).SingleOrDefault();
                ViewBag.TinNumber = tinnum;

                //for PAN Number
                var pannum = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbidt).Select(m => m.CompanyPAN).SingleOrDefault();
                ViewBag.PANNumber = pannum;

                var qgdbs = db.RiceOAEquipGeneralData.Find(oaid2);
                if (qgdbs == null)
                {
                    int SOTTempCount = 0;
                    var SOTTempDetails = db.SOT_Temp_tbl.Where(m => m.QGID == qgid).Where(m => m.BYJTChances == 100).ToList();
                    SOTTempCount = SOTTempDetails.Count();

                    var oast = db.RiceOAEquipGeneralData.Where(m => m.RQGID == qgid).Where(m => m.OAStatus == 0).Count();

                    //declaration for whitener and polisher multiple storage
                    int ModelQty = 0;

                    for (int i = 0; i < SOTTempCount; i++)
                    {
                        if (i == 0)
                        {
                            //var oast = db.RiceOAEquipGeneralData.Where(m => m.RQGID == qgid).Where(m => m.OAStatus == 0).Count();
                            if (oast == 0)
                            {
                                db.QgtoRiceOAGen(qgid);
                                oaid2 = db.RiceOAEquipGeneralData.Where(m => m.RQGID == qgid).OrderByDescending(m => m.ROAID).Select(m => m.ROAID).FirstOrDefault();
                                db.QgtoRiceOAPay(qgid, oaid2);

                                ModelQty = db.SOT_Temp_tbl.Where(m => m.QGID == qgid).Where(m => m.ProductID == productid).Where(m => m.ProductModelID == prodmodelid).Where(m => m.MasterProductID == masterprodid).Select(m => m.Quantity).FirstOrDefault();

                                var ProductModelDetails = db.ProductModel.Where(m => m.ProductModelID == prodmodelid).SingleOrDefault();

                                if (masterprodid == 5 || masterprodid == 7)
                                {
                                    for (int q = 1; q <= ModelQty; q++)
                                    {
                                        int qty = 1;
                                        int DupCheck = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == qgid).Where(m => m.ProductID == productid).Where(m => m.ProductModelID == prodmodelid).Where(m => m.MasterProductID == masterprodid).Count();
                                        if (DupCheck != ModelQty)
                                        {
                                            db.QgtoRiceOATAbOCM(qgid, oaid2, productid, prodmodelid, masterprodid, qty, ProductModelDetails.UnitPrice);
                                            var overallpricepayment = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid2).Select(m => m.TotalPrice).ToList();
                                            foreach (var t in overallpricepayment)
                                            {
                                                //overallpricep += /*Convert.ToInt32(t);*/ /*int.Parse(t.Substring(0, t.Length - 1).Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, string.Empty));*/
                                                var opsp = t.Split(',');
                                                string ovpr = string.Join("", opsp);
                                                overallpricep += Convert.ToInt32(ovpr);
                                            }
                                            overallpricestring = overallpricep.ToString("#,#", CultureInfo.InvariantCulture);
                                            var oapayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid2).FirstOrDefault();
                                            oapayment.overallprice = overallpricestring;
                                            db.Entry(oapayment).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    int DupCheck = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == qgid).Where(m => m.ProductID == productid).Where(m => m.ProductModelID == prodmodelid).Where(m => m.MasterProductID == masterprodid).Count();
                                    if (DupCheck == 0)
                                    {
                                        db.QgtoRiceOATAb(qgid, oaid2, productid, prodmodelid, masterprodid);
                                        var overallpricepayment = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid2).Select(m => m.TotalPrice).ToList();
                                        foreach (var t in overallpricepayment)
                                        {
                                            //overallpricep += /*Convert.ToInt32(t);*/ /*int.Parse(t.Substring(0, t.Length - 1).Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, string.Empty));*/
                                            var opsp = t.Split(',');
                                            string ovpr = string.Join("", opsp);
                                            overallpricep += Convert.ToInt32(ovpr);
                                        }
                                        overallpricestring = overallpricep.ToString("#,#", CultureInfo.InvariantCulture);
                                        var oapayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid2).FirstOrDefault();
                                        oapayment.overallprice = overallpricestring;
                                        db.Entry(oapayment).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            var oastDetails = db.RiceOAEquipGeneralData.Where(m => m.RQGID == qgid).OrderByDescending(m => m.ROAID).FirstOrDefault();
                            if (oastDetails != null)
                            {
                                oaid2 = oastDetails.ROAID;
                            }

                            foreach (var sot in SOTTempDetails)
                            {
                                if (sot.MasterProductID == 5 || sot.MasterProductID == 7)
                                {
                                    var ProductModelDetails = db.ProductModel.Where(m => m.ProductModelID ==sot.ProductModelID).SingleOrDefault();

                                    for (int q = 1; q <= sot.Quantity; q++)
                                    {
                                        int qty = 1;
                                        int DupCheck = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == qgid).Where(m => m.ProductID == sot.ProductID).Where(m => m.ProductModelID == sot.ProductModelID).Where(m => m.MasterProductID == sot.MasterProductID).Count();
                                        if (DupCheck != sot.Quantity)
                                        {
                                            try
                                            {
                                                db.QgtoRiceOATAbOCM(qgid, oaid2, sot.ProductID, sot.ProductModelID, sot.MasterProductID, qty, ProductModelDetails.UnitPrice);
                                                var overallpricepayment = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid2).Select(m => m.TotalPrice).ToList();
                                                foreach (var t in overallpricepayment)
                                                {
                                                    //overallpricep += /*Convert.ToInt32(t);*/ /*int.Parse(t.Substring(0, t.Length - 1).Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, string.Empty));*/
                                                    var opsp = t.Split(',');
                                                    string ovpr = string.Join("", opsp);
                                                    overallpricep += Convert.ToInt32(ovpr);
                                                }
                                                overallpricestring = overallpricep.ToString("#,#", CultureInfo.InvariantCulture);
                                                var oapayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid2).FirstOrDefault();
                                                oapayment.overallprice = overallpricestring;
                                                db.Entry(oapayment).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                            catch(Exception e)
                                            {

                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    int DupCheck = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == qgid).Where(m => m.ProductID == sot.ProductID).Where(m => m.ProductModelID == sot.ProductModelID).Where(m => m.MasterProductID == sot.MasterProductID).Count();
                                    if (DupCheck == 0)
                                    {
                                        db.QgtoRiceOATAb(qgid, oaid2, sot.ProductID, sot.ProductModelID, sot.MasterProductID);
                                        var overallpricepayment = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid2).Select(m => m.TotalPrice).ToList();
                                        foreach (var t in overallpricepayment)
                                        {
                                            //overallpricep += /*Convert.ToInt32(t);*/ /*int.Parse(t.Substring(0, t.Length - 1).Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, string.Empty));*/
                                            var opsp = t.Split(',');
                                            string ovpr = string.Join("", opsp);
                                            overallpricep += Convert.ToInt32(ovpr);
                                        }
                                        overallpricestring = overallpricep.ToString("#,#", CultureInfo.InvariantCulture);
                                        var oapayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid2).FirstOrDefault();
                                        oapayment.overallprice = overallpricestring;
                                        db.Entry(oapayment).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Fetching date from the Quotation Generaal and Table Data.
                    var QGEquipTableDataList = db.QGEquipTableData.Where(m => m.QGID == qgid).ToList();

                    foreach (var tbl in QGEquipTableDataList)
                    {
                        // Getting the RiceOA individual details
                        var RiceTblData = db.RiceOAEquipTableData.Where(m => m.RiceOAEquipGeneralData.RQGID == tbl.QGID).Where(m => m.ProductID == tbl.ProductID).Where(m => m.ProductModelID == tbl.ProductModelID).Where(m => m.MasterProductID == tbl.MasterProductID).Where(m => m.IsSOTStatus == 0).Select(m => m.ROATBID).SingleOrDefault();

                        // updating the RiceOA individual details
                        if (RiceTblData != 0)
                        {
                            RiceOAEquipTableData roatbiddetails = db.RiceOAEquipTableData.Find(RiceTblData);
                            roatbiddetails.IsSOTStatus = tbl.IsSOTStatus;
                            db.Entry(roatbiddetails).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                var qgdb = db.RiceOAEquipGeneralData.Find(oaid2);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid2).Where(m=>m.IsSOTStatus==1); //it will take only the item models which are 100%
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.RiceOAEquipGeneralData = qgdb;
                    quo.RiceOAEquipPayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid2).Single();

                    #region
                    if (modelcnt == 21)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];

                        //string prodmodelname2 = quo.RiceOAEquipTableData2.ProductModel.ProductModelName;
                        //String[] prodmodelname2split = prodmodelname2.Split('/');
                        //if (prodmodelname2split[2] == "With-Motor")
                        //{
                        //    ViewBag.table2 = 1;
                        //}
                        //else if (prodmodelname2split[2] == "WithOut-Motor")
                        //{
                        //    ViewBag.table2 = 2;
                        //}
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];

                        //string prodmodelname1 = quo.RiceOAEquipTableData1.ProductModel.ProductModelName;
                        //String[] prodmodelname1split = prodmodelname1.Split('/');
                        //if (prodmodelname1split[2] == "With-Motor")
                        //{
                        //    ViewBag.table = 1;
                        //}
                        //else if (prodmodelname1split[2] == "WithOut-Motor")
                        //{
                        //    ViewBag.table2 = 2;
                        //}

                        quo.RiceOAEquipTableData2 = null;
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    #endregion

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
                    ViewBag.OAID = quo.RiceOAEquipGeneralData.ROAID;
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.ProductVariety = quo.RiceOAEquipGeneralData.ProductVariety;
                    ViewBag.Type = quo.RiceOAEquipGeneralData.TypeRice;
                    ViewBag.PaddySize = quo.RiceOAEquipGeneralData.PaddySize;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;

                    ViewBag.NullError = false;

                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                var qgdb1 = db.RiceOAEquipGeneralData.Where(m => m.QuotationNumber == quotnumber).Select(m => m.ROAID).SingleOrDefault();
                var qgdb = db.RiceOAEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.RiceOAEquipGeneralData = qgdb;
                    quo.RiceOAEquipPayment = db.RiceOAEquipPayment.Find(qgdb1);
                    if (modelcnt == 21)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = null;
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
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
                    ViewBag.OAID = quo.RiceOAEquipGeneralData.ROAID;
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

                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        // POST: /RiceOA/RiceOAGenerate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiceOAGenerate(RiceOA maindatabase, int? oaid, string MainSection)
        {
            if (oaid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

                if (maindatabase.RiceOAEquipTableData1.ProductModelID!= 0)
                {
                    var Oagen = maindatabase.RiceOAEquipGeneralData;
                    var oagenlist = db.RiceOAEquipGeneralData.Where(m => m.ROAID == Oagen.ROAID).SingleOrDefault();

                    oagenlist.OANumber = maindatabase.RiceOAEquipGeneralData.OANumber;
                    oagenlist.OADate = maindatabase.RiceOAEquipGeneralData.OADate;

                    if (maindatabase.RiceOAEquipGeneralData.OADate != null)
                    {
                        DateTime oadate = Convert.ToDateTime(maindatabase.RiceOAEquipGeneralData.OADate);
                        string oadate1 = oadate.ToString("yyyy-MM-dd");
                        oagenlist.Approvaldate = oadate1;
                    }

                    oagenlist.TinNumber = maindatabase.RiceOAEquipGeneralData.TinNumber;
                    oagenlist.PANCardNo = maindatabase.RiceOAEquipGeneralData.PANCardNo;
                    oagenlist.PackingAndForwarding = maindatabase.RiceOAEquipGeneralData.PackingAndForwarding;
                    oagenlist.TransistInsurance = maindatabase.RiceOAEquipGeneralData.TransistInsurance;
                    oagenlist.Freight = maindatabase.RiceOAEquipGeneralData.Freight;
                    oagenlist.OAStatus = 1;
                    db.Entry(oagenlist).State = EntityState.Modified;
                    db.SaveChanges();

                    //to save Data in MDBGeneralData     //Here We are storing the Billing and Delivery Address
                    var mdbdetails = db.MDBGeneralData.Where(m => m.MDBID == Oagen.MDBID).SingleOrDefault();
                    mdbdetails.BillingAddress=Oagen.MDBGeneralData.BillingAddress;
                    mdbdetails.DeleveryAddress = Oagen.MDBGeneralData.DeleveryAddress;

                    db.Entry(mdbdetails).State = EntityState.Modified;
                    //db.SaveChanges();

                    //to Check the Exact Errors from table and its fields
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
                    //catch (DbEntityValidationException dbEx)
                    //{

                    //   var errorMessages = dbEx.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    //   var fullErrorMessage = string.Join("; ", errorMessages);
                    //   var exceptionMessage = string.Concat(dbEx.Message, " The validation errors are: ", fullErrorMessage);
                    //   throw new DbEntityValidationException(exceptionMessage, dbEx.EntityValidationErrors);

                    //   //foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //   //{
                    //   //    foreach (var validationError in validationErrors.ValidationErrors)
                    //   //    {
                    //   //        Debug.WriteLine("Property: {0} Error: {1}",
                    //   //                   validationError.PropertyName, validationError.ErrorMessage);
                    //   //    }
                    //   //}
                    //}

                    var Oapay = maindatabase.RiceOAEquipPayment;
                    db.Entry(Oapay).State = EntityState.Modified;
                    db.SaveChanges();

                    #region
                    if (maindatabase.RiceOAEquipTableData1.UnitPrice != null && maindatabase.RiceOAEquipTableData1.UnitPrice != "")
                    {

                        var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData1.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData1.ROAID).Count();
                        if (oatch == 0)
                        {
                            //ViewBag.Ispresent = true;
                            RiceOAEquipTableData Oatab1 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData1.ROATBID);
                            //var Oatab1 = maindatabase.RiceOAEquipTableData1;

                            Oatab1.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                            Oatab1.ProductModelID = maindatabase.RiceOAEquipTableData1.ProductModelID;
                            Oatab1.MasterProductName = maindatabase.RiceOAEquipTableData1.MasterProductName;
                            Oatab1.ProductName = maindatabase.RiceOAEquipTableData1.ProductName;
                            Oatab1.Quantity = maindatabase.RiceOAEquipTableData1.Quantity;
                            Oatab1.UnitPrice = maindatabase.RiceOAEquipTableData1.UnitPrice;
                            Oatab1.TotalPrice = maindatabase.RiceOAEquipTableData1.TotalPrice;


                            if (maindatabase.RiceOAEquipTableData1.MasterProductID == 7 || maindatabase.RiceOAEquipTableData1.MasterProductID == 5)
                            {
                            int prodmodelid1 = maindatabase.RiceOAEquipTableData1.ProductModelID;
                            ProductModel ProductModel1 = db.ProductModel.Find(prodmodelid1);
                            var prodname1 = ProductModel1.ProductModelName;
                            String[] prodnamesplit1 = prodname1.Split('/');
                            var motorrating2 = "";
                            var motorq2 = "";
                            if (prodnamesplit1.Length == 1)
                            {
                                motorrating2 = prodnamesplit1[0];
                            }
                            else
                            {
                                motorrating2 = prodnamesplit1[1];
                                motorq2 = prodnamesplit1[2];
                                motorrating2 = motorrating2.Trim();
                                motorq2 = motorq2.Trim();
                            }

                            Oatab1.MotorQ = motorq2;
                            Oatab1.MotorRating = motorrating2;
                            }

                            db.Entry(Oatab1).State = EntityState.Modified;
                            //db.Entry<RiceOAEquipTableData>(Oatab1).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            RiceOAEquipTableData Oatab1 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData1.ROATBID);

                            Oatab1.Quantity = maindatabase.RiceOAEquipTableData1.Quantity;
                            Oatab1.UnitPrice = maindatabase.RiceOAEquipTableData1.UnitPrice;
                            Oatab1.TotalPrice = maindatabase.RiceOAEquipTableData1.TotalPrice;

                            if (maindatabase.RiceOAEquipTableData1.MasterProductID == 7 || maindatabase.RiceOAEquipTableData1.MasterProductID == 5)
                            {
                                //to store MotorQ and MototrRating
                                int prodmodelid1 = maindatabase.RiceOAEquipTableData1.ProductModelID;
                                ProductModel ProductModel1 = db.ProductModel.Find(prodmodelid1);
                                var prodname1 = ProductModel1.ProductModelName;
                                String[] prodnamesplit1 = prodname1.Split('/');
                                var motorrating1 = "";
                                var motorq1 = "";
                                if (prodnamesplit1.Length == 1)
                                {
                                    motorrating1 = prodnamesplit1[0];
                                }
                                else {
                                    motorrating1 = prodnamesplit1[1];
                                    motorq1 = prodnamesplit1[2];
                                    motorrating1 = motorrating1.Trim();
                                    motorq1 = motorq1.Trim();
                                }

                                Oatab1.Pass = maindatabase.RiceOAEquipTableData1.Pass;
                                Oatab1.MotorType = maindatabase.RiceOAEquipTableData1.MotorType;
                                Oatab1.PolishRequirement = maindatabase.RiceOAEquipTableData1.PolishRequirement;
                                Oatab1.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                Oatab1.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;
                                Oatab1.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;

                                Oatab1.MotorQ = motorq1;
                                Oatab1.MotorRating = motorrating1;
                            }

                            db.Entry(Oatab1).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        int id = maindatabase.RiceOAEquipTableData1.ROATBID;
                        RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                        db.RiceOAEquipTableData.Remove(oad);
                        db.SaveChanges();
                    }
                    #endregion

                    //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                    var SOTRMList1 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData1.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData1.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData1.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                    if (SOTRMList1 != null)
                    {
                        SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList1.TSOTID);
                        db.SOT_Temp_tbl.Remove(tempsot);
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData2.ProductModelID != 0)
                    {
                        #region
                        if (maindatabase.RiceOAEquipTableData2.UnitPrice != null && maindatabase.RiceOAEquipTableData2.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData2.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData2.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent = true;
                                RiceOAEquipTableData oatb2 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData2.ROATBID);
                                //oatb2 = maindatabase.RiceOAEquipTableData2;
                                oatb2.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb2.ProductModelID = maindatabase.RiceOAEquipTableData2.ProductModelID;
                                oatb2.MasterProductName = maindatabase.RiceOAEquipTableData2.MasterProductName;
                                oatb2.ProductName = maindatabase.RiceOAEquipTableData2.ProductName;

                                oatb2.Quantity = maindatabase.RiceOAEquipTableData2.Quantity;
                                oatb2.UnitPrice = maindatabase.RiceOAEquipTableData2.UnitPrice;
                                oatb2.TotalPrice = maindatabase.RiceOAEquipTableData2.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData2.MasterProductID == 7 || maindatabase.RiceOAEquipTableData2.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid2 = maindatabase.RiceOAEquipTableData2.ProductModelID;
                                    ProductModel ProductModel2 = db.ProductModel.Find(prodmodelid2);
                                    var prodname2 = ProductModel2.ProductModelName;
                                    String[] prodnamesplit2 = prodname2.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb2.MotorQ = motorq2;
                                    oatb2.MotorRating = motorrating2;
                                }
                                db.Entry(oatb2).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab2 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData2.ROATBID);

                                Oatab2.Quantity = maindatabase.RiceOAEquipTableData2.Quantity;
                                Oatab2.UnitPrice = maindatabase.RiceOAEquipTableData2.UnitPrice;
                                Oatab2.TotalPrice = maindatabase.RiceOAEquipTableData2.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData2.MasterProductID == 7 || maindatabase.RiceOAEquipTableData2.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid2 = maindatabase.RiceOAEquipTableData2.ProductModelID;
                                    ProductModel ProductModel2 = db.ProductModel.Find(prodmodelid2);
                                    var prodname2 = ProductModel2.ProductModelName;
                                    String[] prodnamesplit2 = prodname2.Split('/');
                                    //var motorrating2 = prodnamesplit2[1];
                                    //var motorq2 = prodnamesplit2[2];

                                    //motorrating2 = motorrating2.Trim();
                                    //motorq2 = motorq2.Trim();
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab2.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab2.Pass = maindatabase.RiceOAEquipTableData2.Pass;
                                    Oatab2.MotorType = maindatabase.RiceOAEquipTableData2.MotorType;
                                    Oatab2.PolishRequirement = maindatabase.RiceOAEquipTableData2.PolishRequirement;
                                    Oatab2.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab2.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab2.MotorQ = motorq2;
                                    Oatab2.MotorRating = motorrating2;
                                }

                                db.Entry(Oatab2).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData2.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList2 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData2.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData2.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData2.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList2 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList2.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData3.ProductModelID != 0)
                    {
                        #region
                        if (maindatabase.RiceOAEquipTableData3.UnitPrice != null && maindatabase.RiceOAEquipTableData3.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData3.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData3.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb3 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData3.ROATBID);
                                //RiceOAEquipTableData oatb3 = new RiceOAEquipTableData();
                                oatb3 = maindatabase.RiceOAEquipTableData3;
                                oatb3.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb3.ProductModelID = maindatabase.RiceOAEquipTableData3.ProductModelID;
                                oatb3.MasterProductName = maindatabase.RiceOAEquipTableData3.MasterProductName;
                                oatb3.ProductName = maindatabase.RiceOAEquipTableData3.ProductName;

                                oatb3.Quantity = maindatabase.RiceOAEquipTableData3.Quantity;
                                oatb3.UnitPrice = maindatabase.RiceOAEquipTableData3.UnitPrice;
                                oatb3.TotalPrice = maindatabase.RiceOAEquipTableData3.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData3.MasterProductID == 7 || maindatabase.RiceOAEquipTableData3.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid3 = maindatabase.RiceOAEquipTableData3.ProductModelID;
                                    ProductModel ProductModel3 = db.ProductModel.Find(prodmodelid3);
                                    var prodname3 = ProductModel3.ProductModelName;
                                    String[] prodnamesplit2 = prodname3.Split('/');
                                    //var motorrating3 = prodnamesplit3[1];
                                    //var motorq3 = prodnamesplit3[2];

                                    //motorrating3 = motorrating3.Trim();
                                    //motorq3 = motorq3.Trim();
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb3.MotorQ = motorq2;
                                    oatb3.MotorRating = motorrating2;
                                }
                                db.Entry(oatb3).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab3 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData3.ROATBID);

                                Oatab3.Quantity = maindatabase.RiceOAEquipTableData3.Quantity;
                                Oatab3.UnitPrice = maindatabase.RiceOAEquipTableData3.UnitPrice;
                                Oatab3.TotalPrice = maindatabase.RiceOAEquipTableData3.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData3.MasterProductID == 7 || maindatabase.RiceOAEquipTableData3.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid3 = maindatabase.RiceOAEquipTableData3.ProductModelID;
                                    ProductModel ProductModel3 = db.ProductModel.Find(prodmodelid3);
                                    var prodname3 = ProductModel3.ProductModelName;
                                    String[] prodnamesplit2 = prodname3.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab3.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab3.Pass = maindatabase.RiceOAEquipTableData3.Pass;
                                    Oatab3.MotorType = maindatabase.RiceOAEquipTableData3.MotorType;
                                    Oatab3.PolishRequirement = maindatabase.RiceOAEquipTableData3.PolishRequirement;
                                    Oatab3.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab3.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab3.MotorQ = motorq2;
                                    Oatab3.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab3).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData3.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList3 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData3.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData3.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData3.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList3 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList3.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData4.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData4.UnitPrice != null && maindatabase.RiceOAEquipTableData4.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData4.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData4.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent2 = true;
                                RiceOAEquipTableData oatb4 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData4.ROATBID);
                                //RiceOAEquipTableData oatb4 = new RiceOAEquipTableData();
                                oatb4 = maindatabase.RiceOAEquipTableData4;
                                oatb4.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb4.ProductModelID = maindatabase.RiceOAEquipTableData4.ProductModelID;
                                oatb4.MasterProductName = maindatabase.RiceOAEquipTableData4.MasterProductName;
                                oatb4.ProductName = maindatabase.RiceOAEquipTableData4.ProductName;

                                oatb4.Quantity = maindatabase.RiceOAEquipTableData4.Quantity;
                                oatb4.UnitPrice = maindatabase.RiceOAEquipTableData4.UnitPrice;
                                oatb4.TotalPrice = maindatabase.RiceOAEquipTableData4.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData4.MasterProductID == 7 || maindatabase.RiceOAEquipTableData4.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid4 = maindatabase.RiceOAEquipTableData4.ProductModelID;
                                    ProductModel ProductModel4 = db.ProductModel.Find(prodmodelid4);
                                    var prodname4 = ProductModel4.ProductModelName;
                                    String[] prodnamesplit2 = prodname4.Split('/');
                                    //var motorrating4 = prodnamesplit4[1];
                                    //var motorq4 = prodnamesplit4[2];

                                    //motorrating4 = motorrating4.Trim();
                                    //motorq4 = motorq4.Trim();
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb4.MotorQ = motorq2;
                                    oatb4.MotorRating = motorrating2;
                                }
                                db.Entry(oatb4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab4 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData4.ROATBID);

                                Oatab4.Quantity = maindatabase.RiceOAEquipTableData4.Quantity;
                                Oatab4.UnitPrice = maindatabase.RiceOAEquipTableData4.UnitPrice;
                                Oatab4.TotalPrice = maindatabase.RiceOAEquipTableData4.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData4.MasterProductID == 7 || maindatabase.RiceOAEquipTableData4.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid4 = maindatabase.RiceOAEquipTableData4.ProductModelID;
                                    ProductModel ProductModel4 = db.ProductModel.Find(prodmodelid4);
                                    var prodname4 = ProductModel4.ProductModelName;
                                    String[] prodnamesplit2 = prodname4.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab4.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab4.Pass = maindatabase.RiceOAEquipTableData4.Pass;
                                    Oatab4.MotorType = maindatabase.RiceOAEquipTableData4.MotorType;
                                    Oatab4.PolishRequirement = maindatabase.RiceOAEquipTableData4.PolishRequirement;
                                    Oatab4.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab4.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab4.MotorQ = motorq2;
                                    Oatab4.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab4).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData4.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();

                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList4 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData4.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData4.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData4.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList4 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList4.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData5.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData5.UnitPrice != null && maindatabase.RiceOAEquipTableData5.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData5.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData5.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb5 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData5.ROATBID);
                                //RiceOAEquipTableData oatb5 = new RiceOAEquipTableData();
                                oatb5 = maindatabase.RiceOAEquipTableData5;
                                oatb5.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb5.ProductModelID = maindatabase.RiceOAEquipTableData5.ProductModelID;
                                oatb5.MasterProductName = maindatabase.RiceOAEquipTableData5.MasterProductName;
                                oatb5.ProductName = maindatabase.RiceOAEquipTableData5.ProductName;

                                oatb5.Quantity = maindatabase.RiceOAEquipTableData5.Quantity;
                                oatb5.UnitPrice = maindatabase.RiceOAEquipTableData5.UnitPrice;
                                oatb5.TotalPrice = maindatabase.RiceOAEquipTableData5.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData5.MasterProductID == 7 || maindatabase.RiceOAEquipTableData5.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid5 = maindatabase.RiceOAEquipTableData5.ProductModelID;
                                    ProductModel ProductModel5 = db.ProductModel.Find(prodmodelid5);
                                    var prodname5 = ProductModel5.ProductModelName;
                                    String[] prodnamesplit2 = prodname5.Split('/');
                                    //var motorrating5 = prodnamesplit5[1];
                                    //var motorq5 = prodnamesplit5[2];

                                    //motorrating5 = motorrating5.Trim();
                                    //motorq5 = motorq5.Trim();
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb5.MotorQ = motorq2;
                                    oatb5.MotorRating = motorrating2;
                                }
                                db.Entry(oatb5).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab5 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData5.ROATBID);

                                Oatab5.Quantity = maindatabase.RiceOAEquipTableData5.Quantity;
                                Oatab5.UnitPrice = maindatabase.RiceOAEquipTableData5.UnitPrice;
                                Oatab5.TotalPrice = maindatabase.RiceOAEquipTableData5.TotalPrice;


                                if (maindatabase.RiceOAEquipTableData5.MasterProductID == 7 || maindatabase.RiceOAEquipTableData5.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid5 = maindatabase.RiceOAEquipTableData5.ProductModelID;
                                    ProductModel ProductModel5 = db.ProductModel.Find(prodmodelid5);
                                    var prodname5 = ProductModel5.ProductModelName;
                                    String[] prodnamesplit2 = prodname5.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab5.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab5.Pass = maindatabase.RiceOAEquipTableData5.Pass;
                                    Oatab5.MotorType = maindatabase.RiceOAEquipTableData5.MotorType;
                                    Oatab5.PolishRequirement = maindatabase.RiceOAEquipTableData5.PolishRequirement;
                                    Oatab5.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab5.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab5.MotorQ = motorq2;
                                    Oatab5.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab5).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData5.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList5 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData5.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData5.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData5.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList5 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList5.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData6.ProductModelID != 0)
                    {
                        #region
                        if (maindatabase.RiceOAEquipTableData6.UnitPrice != null && maindatabase.RiceOAEquipTableData6.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData6.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData6.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb6 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData6.ROATBID);
                                //RiceOAEquipTableData oatb6 = new RiceOAEquipTableData();
                                oatb6 = maindatabase.RiceOAEquipTableData6;
                                oatb6.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb6.ProductModelID = maindatabase.RiceOAEquipTableData6.ProductModelID;
                                oatb6.MasterProductName = maindatabase.RiceOAEquipTableData6.MasterProductName;
                                oatb6.ProductName = maindatabase.RiceOAEquipTableData6.ProductName;

                                oatb6.Quantity = maindatabase.RiceOAEquipTableData6.Quantity;
                                oatb6.UnitPrice = maindatabase.RiceOAEquipTableData6.UnitPrice;
                                oatb6.TotalPrice = maindatabase.RiceOAEquipTableData6.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData6.MasterProductID == 7 || maindatabase.RiceOAEquipTableData6.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid6 = maindatabase.RiceOAEquipTableData6.ProductModelID;
                                    ProductModel ProductModel6 = db.ProductModel.Find(prodmodelid6);
                                    var prodname6 = ProductModel6.ProductModelName;
                                    String[] prodnamesplit2 = prodname6.Split('/');
                                    //var motorrating6 = prodnamesplit6[1];
                                    //var motorq6 = prodnamesplit6[2];

                                    //motorrating6 = motorrating6.Trim();
                                    //motorq6 = motorq6.Trim();
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb6.MotorQ = motorq2;
                                    oatb6.MotorRating = motorrating2;
                                }
                                db.Entry(oatb6).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab6 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData6.ROATBID);

                                Oatab6.Quantity = maindatabase.RiceOAEquipTableData6.Quantity;
                                Oatab6.UnitPrice = maindatabase.RiceOAEquipTableData6.UnitPrice;
                                Oatab6.TotalPrice = maindatabase.RiceOAEquipTableData6.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData6.MasterProductID == 7 || maindatabase.RiceOAEquipTableData6.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid6 = maindatabase.RiceOAEquipTableData6.ProductModelID;
                                    ProductModel ProductModel6 = db.ProductModel.Find(prodmodelid6);
                                    var prodname6 = ProductModel6.ProductModelName;
                                    String[] prodnamesplit2 = prodname6.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab6.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab6.Pass = maindatabase.RiceOAEquipTableData6.Pass;
                                    Oatab6.MotorType = maindatabase.RiceOAEquipTableData6.MotorType;
                                    Oatab6.PolishRequirement = maindatabase.RiceOAEquipTableData6.PolishRequirement;
                                    Oatab6.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab6.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab6.MotorQ = motorq2;
                                    Oatab6.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab6).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData6.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList6 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData6.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData6.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData6.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList6 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList6.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData7.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData7.UnitPrice != null && maindatabase.RiceOAEquipTableData7.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData7.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData7.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb7 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData7.ROATBID);
                                //RiceOAEquipTableData oatb7 = new RiceOAEquipTableData();
                                oatb7 = maindatabase.RiceOAEquipTableData7;
                                oatb7.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb7.ProductModelID = maindatabase.RiceOAEquipTableData7.ProductModelID;
                                oatb7.MasterProductName = maindatabase.RiceOAEquipTableData7.MasterProductName;
                                oatb7.ProductName = maindatabase.RiceOAEquipTableData7.ProductName;

                                oatb7.Quantity = maindatabase.RiceOAEquipTableData7.Quantity;
                                oatb7.UnitPrice = maindatabase.RiceOAEquipTableData7.UnitPrice;
                                oatb7.TotalPrice = maindatabase.RiceOAEquipTableData7.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData7.MasterProductID == 7 || maindatabase.RiceOAEquipTableData7.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid7 = maindatabase.RiceOAEquipTableData7.ProductModelID;
                                    ProductModel ProductModel7 = db.ProductModel.Find(prodmodelid7);
                                    var prodname7 = ProductModel7.ProductModelName;
                                    String[] prodnamesplit2 = prodname7.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb7.MotorQ = motorq2;
                                    oatb7.MotorRating = motorrating2;
                                }
                                db.Entry(oatb7).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab7 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData7.ROATBID);

                                Oatab7.Quantity = maindatabase.RiceOAEquipTableData7.Quantity;
                                Oatab7.UnitPrice = maindatabase.RiceOAEquipTableData7.UnitPrice;
                                Oatab7.TotalPrice = maindatabase.RiceOAEquipTableData7.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData7.MasterProductID == 7 || maindatabase.RiceOAEquipTableData7.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid7 = maindatabase.RiceOAEquipTableData7.ProductModelID;
                                    ProductModel ProductModel7 = db.ProductModel.Find(prodmodelid7);
                                    var prodname7 = ProductModel7.ProductModelName;
                                    String[] prodnamesplit2 = prodname7.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab7.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab7.Pass = maindatabase.RiceOAEquipTableData7.Pass;
                                    Oatab7.MotorType = maindatabase.RiceOAEquipTableData7.MotorType;
                                    Oatab7.PolishRequirement = maindatabase.RiceOAEquipTableData7.PolishRequirement;
                                    Oatab7.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab7.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab7.MotorQ = motorq2;
                                    Oatab7.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab7).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData7.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList7 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData7.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData7.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData7.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList7 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList7.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData8.ProductModelID != 0)
                    {
                        #region
                        if (maindatabase.RiceOAEquipTableData8.UnitPrice != null && maindatabase.RiceOAEquipTableData8.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData8.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData8.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb8 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData8.ROATBID);
                                //RiceOAEquipTableData oatb8 = new RiceOAEquipTableData();
                                oatb8 = maindatabase.RiceOAEquipTableData8;
                                oatb8.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb8.ProductModelID = maindatabase.RiceOAEquipTableData8.ProductModelID;
                                oatb8.MasterProductName = maindatabase.RiceOAEquipTableData8.MasterProductName;
                                oatb8.ProductName = maindatabase.RiceOAEquipTableData8.ProductName;

                                oatb8.Quantity = maindatabase.RiceOAEquipTableData8.Quantity;
                                oatb8.UnitPrice = maindatabase.RiceOAEquipTableData8.UnitPrice;
                                oatb8.TotalPrice = maindatabase.RiceOAEquipTableData8.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData8.MasterProductID == 7 || maindatabase.RiceOAEquipTableData8.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid8 = maindatabase.RiceOAEquipTableData8.ProductModelID;
                                    ProductModel ProductModel8 = db.ProductModel.Find(prodmodelid8);
                                    var prodname8 = ProductModel8.ProductModelName;
                                    String[] prodnamesplit2 = prodname8.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb8.MotorQ = motorq2;
                                    oatb8.MotorRating = motorrating2;
                                }
                                db.Entry(oatb8).State = EntityState.Modified;
                                db.SaveChanges(); ;
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab8 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData8.ROATBID);

                                Oatab8.Quantity = maindatabase.RiceOAEquipTableData8.Quantity;
                                Oatab8.UnitPrice = maindatabase.RiceOAEquipTableData8.UnitPrice;
                                Oatab8.TotalPrice = maindatabase.RiceOAEquipTableData8.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData8.MasterProductID == 7 || maindatabase.RiceOAEquipTableData8.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid8 = maindatabase.RiceOAEquipTableData8.ProductModelID;
                                    ProductModel ProductModel8 = db.ProductModel.Find(prodmodelid8);
                                    var prodname8 = ProductModel8.ProductModelName;
                                    String[] prodnamesplit2 = prodname8.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab8.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab8.Pass = maindatabase.RiceOAEquipTableData8.Pass;
                                    Oatab8.MotorType = maindatabase.RiceOAEquipTableData8.MotorType;
                                    Oatab8.PolishRequirement = maindatabase.RiceOAEquipTableData8.PolishRequirement;
                                    Oatab8.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab8.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab8.MotorQ = motorq2;
                                    Oatab8.MotorRating = motorrating2;
                                }

                                db.Entry(Oatab8).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData8.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList8 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData8.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData8.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData8.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList8 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList8.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData9.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData9.UnitPrice != null && maindatabase.RiceOAEquipTableData5.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData9.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData9.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb9 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData9.ROATBID);
                                //RiceOAEquipTableData oatb9 = new RiceOAEquipTableData();
                                oatb9 = maindatabase.RiceOAEquipTableData9;
                                oatb9.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb9.ProductModelID = maindatabase.RiceOAEquipTableData9.ProductModelID;
                                oatb9.MasterProductName = maindatabase.RiceOAEquipTableData9.MasterProductName;
                                oatb9.ProductName = maindatabase.RiceOAEquipTableData9.ProductName;

                                oatb9.Quantity = maindatabase.RiceOAEquipTableData9.Quantity;
                                oatb9.UnitPrice = maindatabase.RiceOAEquipTableData9.UnitPrice;
                                oatb9.TotalPrice = maindatabase.RiceOAEquipTableData9.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData9.MasterProductID == 7 || maindatabase.RiceOAEquipTableData9.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid9 = maindatabase.RiceOAEquipTableData9.ProductModelID;
                                    ProductModel ProductModel9 = db.ProductModel.Find(prodmodelid9);
                                    var prodname9 = ProductModel9.ProductModelName;
                                    String[] prodnamesplit2 = prodname9.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb9.MotorQ = motorq2;
                                    oatb9.MotorRating = motorrating2;
                                }
                                db.Entry(oatb9).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab9 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData9.ROATBID);

                                Oatab9.Quantity = maindatabase.RiceOAEquipTableData9.Quantity;
                                Oatab9.UnitPrice = maindatabase.RiceOAEquipTableData9.UnitPrice;
                                Oatab9.TotalPrice = maindatabase.RiceOAEquipTableData9.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData9.MasterProductID == 7 || maindatabase.RiceOAEquipTableData9.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid9 = maindatabase.RiceOAEquipTableData9.ProductModelID;
                                    ProductModel ProductModel9 = db.ProductModel.Find(prodmodelid9);
                                    var prodname9 = ProductModel9.ProductModelName;
                                    String[] prodnamesplit2 = prodname9.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab9.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab9.Pass = maindatabase.RiceOAEquipTableData9.Pass;
                                    Oatab9.MotorType = maindatabase.RiceOAEquipTableData9.MotorType;
                                    Oatab9.PolishRequirement = maindatabase.RiceOAEquipTableData9.PolishRequirement;
                                    Oatab9.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab9.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab9.MotorQ = motorq2;
                                    Oatab9.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab9).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData9.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList9 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData9.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData9.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData9.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList9 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList9.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData10.ProductModelID != 0)
                    {
                        #region
                        if (maindatabase.RiceOAEquipTableData10.UnitPrice != null && maindatabase.RiceOAEquipTableData10.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData10.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData10.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb10 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData10.ROATBID);
                                //RiceOAEquipTableData oatb10 = new RiceOAEquipTableData();
                                oatb10 = maindatabase.RiceOAEquipTableData10;
                                oatb10.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb10.ProductModelID = maindatabase.RiceOAEquipTableData10.ProductModelID;
                                oatb10.MasterProductName = maindatabase.RiceOAEquipTableData10.MasterProductName;
                                oatb10.ProductName = maindatabase.RiceOAEquipTableData10.ProductName;

                                oatb10.Quantity = maindatabase.RiceOAEquipTableData10.Quantity;
                                oatb10.UnitPrice = maindatabase.RiceOAEquipTableData10.UnitPrice;
                                oatb10.TotalPrice = maindatabase.RiceOAEquipTableData10.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData10.MasterProductID == 7 || maindatabase.RiceOAEquipTableData10.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid10 = maindatabase.RiceOAEquipTableData10.ProductModelID;
                                    ProductModel ProductModel10 = db.ProductModel.Find(prodmodelid10);
                                    var prodname10 = ProductModel10.ProductModelName;
                                    String[] prodnamesplit2 = prodname10.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb10.MotorQ = motorq2;
                                    oatb10.MotorRating = motorrating2;
                                }
                                db.Entry(oatb10).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab10 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData10.ROATBID);

                                Oatab10.Quantity = maindatabase.RiceOAEquipTableData10.Quantity;
                                Oatab10.UnitPrice = maindatabase.RiceOAEquipTableData10.UnitPrice;
                                Oatab10.TotalPrice = maindatabase.RiceOAEquipTableData10.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData10.MasterProductID == 7 || maindatabase.RiceOAEquipTableData10.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid10 = maindatabase.RiceOAEquipTableData10.ProductModelID;
                                    ProductModel ProductModel10 = db.ProductModel.Find(prodmodelid10);
                                    var prodname10 = ProductModel10.ProductModelName;
                                    String[] prodnamesplit2 = prodname10.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab10.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab10.Pass = maindatabase.RiceOAEquipTableData10.Pass;
                                    Oatab10.MotorType = maindatabase.RiceOAEquipTableData10.MotorType;
                                    Oatab10.PolishRequirement = maindatabase.RiceOAEquipTableData10.PolishRequirement;
                                    Oatab10.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab10.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab10.MotorQ = motorq2;
                                    Oatab10.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab10).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData10.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList10 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData10.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData10.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData10.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList10 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList10.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData11.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData11.UnitPrice != null && maindatabase.RiceOAEquipTableData11.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData11.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData11.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb11 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData11.ROATBID);
                                //RiceOAEquipTableData oatb11 = new RiceOAEquipTableData();
                                oatb11 = maindatabase.RiceOAEquipTableData11;
                                oatb11.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb11.ProductModelID = maindatabase.RiceOAEquipTableData11.ProductModelID;
                                oatb11.MasterProductName = maindatabase.RiceOAEquipTableData11.MasterProductName;
                                oatb11.ProductName = maindatabase.RiceOAEquipTableData11.ProductName;

                                oatb11.Quantity = maindatabase.RiceOAEquipTableData11.Quantity;
                                oatb11.UnitPrice = maindatabase.RiceOAEquipTableData11.UnitPrice;
                                oatb11.TotalPrice = maindatabase.RiceOAEquipTableData11.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData11.MasterProductID == 7 || maindatabase.RiceOAEquipTableData11.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid11 = maindatabase.RiceOAEquipTableData11.ProductModelID;
                                    ProductModel ProductModel11 = db.ProductModel.Find(prodmodelid11);
                                    var prodname11 = ProductModel11.ProductModelName;
                                    String[] prodnamesplit2 = prodname11.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb11.MotorQ = motorq2;
                                    oatb11.MotorRating = motorrating2;
                                }
                                db.Entry(oatb11).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab11 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData11.ROATBID);

                                Oatab11.Quantity = maindatabase.RiceOAEquipTableData11.Quantity;
                                Oatab11.UnitPrice = maindatabase.RiceOAEquipTableData11.UnitPrice;
                                Oatab11.TotalPrice = maindatabase.RiceOAEquipTableData11.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData11.MasterProductID == 7 || maindatabase.RiceOAEquipTableData11.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid11 = maindatabase.RiceOAEquipTableData11.ProductModelID;
                                    ProductModel ProductModel11 = db.ProductModel.Find(prodmodelid11);
                                    var prodname11 = ProductModel11.ProductModelName;
                                    String[] prodnamesplit2 = prodname11.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab11.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab11.Pass = maindatabase.RiceOAEquipTableData11.Pass;
                                    Oatab11.MotorType = maindatabase.RiceOAEquipTableData11.MotorType;
                                    Oatab11.PolishRequirement = maindatabase.RiceOAEquipTableData11.PolishRequirement;
                                    Oatab11.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab11.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab11.MotorQ = motorq2;
                                    Oatab11.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab11).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData11.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList11 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData11.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData11.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData11.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList11 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList11.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData12.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData12.UnitPrice != null && maindatabase.RiceOAEquipTableData12.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData12.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData12.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb12 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData12.ROATBID);
                                //RiceOAEquipTableData oatb12 = new RiceOAEquipTableData();
                                oatb12 = maindatabase.RiceOAEquipTableData12;
                                oatb12.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb12.ProductModelID = maindatabase.RiceOAEquipTableData12.ProductModelID;
                                oatb12.MasterProductName = maindatabase.RiceOAEquipTableData12.MasterProductName;
                                oatb12.ProductName = maindatabase.RiceOAEquipTableData12.ProductName;

                                oatb12.Quantity = maindatabase.RiceOAEquipTableData12.Quantity;
                                oatb12.UnitPrice = maindatabase.RiceOAEquipTableData12.UnitPrice;
                                oatb12.TotalPrice = maindatabase.RiceOAEquipTableData12.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData12.MasterProductID == 7 || maindatabase.RiceOAEquipTableData12.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid12 = maindatabase.RiceOAEquipTableData12.ProductModelID;
                                    ProductModel ProductModel12 = db.ProductModel.Find(prodmodelid12);
                                    var prodname12 = ProductModel12.ProductModelName;
                                    String[] prodnamesplit2 = prodname12.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb12.MotorQ = motorq2;
                                    oatb12.MotorRating = motorrating2;
                                }
                                db.Entry(oatb12).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab12 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData12.ROATBID);

                                Oatab12.Quantity = maindatabase.RiceOAEquipTableData12.Quantity;
                                Oatab12.UnitPrice = maindatabase.RiceOAEquipTableData12.UnitPrice;
                                Oatab12.TotalPrice = maindatabase.RiceOAEquipTableData12.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData12.MasterProductID == 7 || maindatabase.RiceOAEquipTableData12.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid12 = maindatabase.RiceOAEquipTableData12.ProductModelID;
                                    ProductModel ProductModel12 = db.ProductModel.Find(prodmodelid12);
                                    var prodname12 = ProductModel12.ProductModelName;
                                    String[] prodnamesplit2 = prodname12.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab12.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab12.Pass = maindatabase.RiceOAEquipTableData12.Pass;
                                    Oatab12.MotorType = maindatabase.RiceOAEquipTableData12.MotorType;
                                    Oatab12.PolishRequirement = maindatabase.RiceOAEquipTableData12.PolishRequirement;
                                    Oatab12.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab12.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab12.MotorQ = motorq2;
                                    Oatab12.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab12).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData12.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList12 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData12.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData12.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData12.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList12 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList12.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData13.ProductModelID != 0)
                    {
                        #region
                        if (maindatabase.RiceOAEquipTableData13.UnitPrice != null && maindatabase.RiceOAEquipTableData13.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData13.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData13.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb13 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData13.ROATBID);
                                //RiceOAEquipTableData oatb13 = new RiceOAEquipTableData();
                                oatb13 = maindatabase.RiceOAEquipTableData13;
                                oatb13.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb13.ProductModelID = maindatabase.RiceOAEquipTableData13.ProductModelID;
                                oatb13.MasterProductName = maindatabase.RiceOAEquipTableData13.MasterProductName;
                                oatb13.ProductName = maindatabase.RiceOAEquipTableData13.ProductName;

                                oatb13.Quantity = maindatabase.RiceOAEquipTableData13.Quantity;
                                oatb13.UnitPrice = maindatabase.RiceOAEquipTableData13.UnitPrice;
                                oatb13.TotalPrice = maindatabase.RiceOAEquipTableData13.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData13.MasterProductID == 7 || maindatabase.RiceOAEquipTableData13.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid13 = maindatabase.RiceOAEquipTableData13.ProductModelID;
                                    ProductModel ProductModel13 = db.ProductModel.Find(prodmodelid13);
                                    var prodname13 = ProductModel13.ProductModelName;
                                    String[] prodnamesplit2 = prodname13.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }
                                    oatb13.MotorQ = motorq2;
                                    oatb13.MotorRating = motorrating2;
                                }
                                db.Entry(oatb13).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab13 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData13.ROATBID);

                                Oatab13.Quantity = maindatabase.RiceOAEquipTableData13.Quantity;
                                Oatab13.UnitPrice = maindatabase.RiceOAEquipTableData13.UnitPrice;
                                Oatab13.TotalPrice = maindatabase.RiceOAEquipTableData13.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData13.MasterProductID == 7 || maindatabase.RiceOAEquipTableData13.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid13 = maindatabase.RiceOAEquipTableData13.ProductModelID;
                                    ProductModel ProductModel13 = db.ProductModel.Find(prodmodelid13);
                                    var prodname13 = ProductModel13.ProductModelName;
                                    String[] prodnamesplit2 = prodname13.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab13.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab13.Pass = maindatabase.RiceOAEquipTableData13.Pass;
                                    Oatab13.MotorType = maindatabase.RiceOAEquipTableData13.MotorType;
                                    Oatab13.PolishRequirement = maindatabase.RiceOAEquipTableData13.PolishRequirement;
                                    Oatab13.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab13.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab13.MotorQ = motorq2;
                                    Oatab13.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab13).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData13.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion


                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList13 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData13.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData13.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData13.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList13 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList13.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData14.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData14.UnitPrice != null && maindatabase.RiceOAEquipTableData14.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData14.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData14.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb14 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData14.ROATBID);
                                //RiceOAEquipTableData oatb14 = new RiceOAEquipTableData();
                                oatb14 = maindatabase.RiceOAEquipTableData14;
                                oatb14.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb14.ProductModelID = maindatabase.RiceOAEquipTableData14.ProductModelID;
                                oatb14.MasterProductName = maindatabase.RiceOAEquipTableData14.MasterProductName;
                                oatb14.ProductName = maindatabase.RiceOAEquipTableData14.ProductName;

                                oatb14.Quantity = maindatabase.RiceOAEquipTableData14.Quantity;
                                oatb14.UnitPrice = maindatabase.RiceOAEquipTableData14.UnitPrice;
                                oatb14.TotalPrice = maindatabase.RiceOAEquipTableData14.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData14.MasterProductID == 7 || maindatabase.RiceOAEquipTableData14.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid14 = maindatabase.RiceOAEquipTableData14.ProductModelID;
                                    ProductModel ProductModel14 = db.ProductModel.Find(prodmodelid14);
                                    var prodname14 = ProductModel14.ProductModelName;
                                    String[] prodnamesplit2 = prodname14.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb14.MotorQ = motorq2;
                                    oatb14.MotorRating = motorrating2;
                                }
                                db.Entry(oatb14).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab14 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData14.ROATBID);

                                Oatab14.Quantity = maindatabase.RiceOAEquipTableData14.Quantity;
                                Oatab14.UnitPrice = maindatabase.RiceOAEquipTableData14.UnitPrice;
                                Oatab14.TotalPrice = maindatabase.RiceOAEquipTableData14.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData14.MasterProductID == 7 || maindatabase.RiceOAEquipTableData14.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid14 = maindatabase.RiceOAEquipTableData14.ProductModelID;
                                    ProductModel ProductModel14 = db.ProductModel.Find(prodmodelid14);
                                    var prodname14 = ProductModel14.ProductModelName;
                                    String[] prodnamesplit2 = prodname14.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab14.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab14.Pass = maindatabase.RiceOAEquipTableData14.Pass;
                                    Oatab14.MotorType = maindatabase.RiceOAEquipTableData14.MotorType;
                                    Oatab14.PolishRequirement = maindatabase.RiceOAEquipTableData14.PolishRequirement;
                                    Oatab14.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab14.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab14.MotorQ = motorq2;
                                    Oatab14.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab14).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData14.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion


                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList14 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData14.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData14.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData14.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList14 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList14.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData15.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData15.UnitPrice != null && maindatabase.RiceOAEquipTableData15.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData15.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData15.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb15 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData15.ROATBID);
                                //RiceOAEquipTableData oatb15 = new RiceOAEquipTableData();
                                oatb15 = maindatabase.RiceOAEquipTableData15;
                                oatb15.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb15.ProductModelID = maindatabase.RiceOAEquipTableData15.ProductModelID;
                                oatb15.MasterProductName = maindatabase.RiceOAEquipTableData15.MasterProductName;
                                oatb15.ProductName = maindatabase.RiceOAEquipTableData15.ProductName;

                                oatb15.Quantity = maindatabase.RiceOAEquipTableData15.Quantity;
                                oatb15.UnitPrice = maindatabase.RiceOAEquipTableData15.UnitPrice;
                                oatb15.TotalPrice = maindatabase.RiceOAEquipTableData15.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData15.MasterProductID == 7 || maindatabase.RiceOAEquipTableData15.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid15 = maindatabase.RiceOAEquipTableData15.ProductModelID;
                                    ProductModel ProductModel15 = db.ProductModel.Find(prodmodelid15);
                                    var prodname15 = ProductModel15.ProductModelName;
                                    String[] prodnamesplit2 = prodname15.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb15.MotorQ = motorq2;
                                    oatb15.MotorRating = motorrating2;
                                }
                                db.Entry(oatb15).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab15 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData15.ROATBID);

                                Oatab15.Quantity = maindatabase.RiceOAEquipTableData15.Quantity;
                                Oatab15.UnitPrice = maindatabase.RiceOAEquipTableData15.UnitPrice;
                                Oatab15.TotalPrice = maindatabase.RiceOAEquipTableData15.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData15.MasterProductID == 7 || maindatabase.RiceOAEquipTableData15.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid15 = maindatabase.RiceOAEquipTableData15.ProductModelID;
                                    ProductModel ProductModel15 = db.ProductModel.Find(prodmodelid15);
                                    var prodname15 = ProductModel15.ProductModelName;
                                    String[] prodnamesplit2 = prodname15.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab15.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab15.Pass = maindatabase.RiceOAEquipTableData15.Pass;
                                    Oatab15.MotorType = maindatabase.RiceOAEquipTableData15.MotorType;
                                    Oatab15.PolishRequirement = maindatabase.RiceOAEquipTableData15.PolishRequirement;
                                    Oatab15.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab15.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab15.MotorQ = motorq2;
                                    Oatab15.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab15).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData15.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion


                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList15 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData15.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData15.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData15.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList15 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList15.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData16.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData16.UnitPrice != null && maindatabase.RiceOAEquipTableData16.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData16.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData16.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb16 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData16.ROATBID);
                                //RiceOAEquipTableData oatb16 = new RiceOAEquipTableData();
                                oatb16 = maindatabase.RiceOAEquipTableData16;
                                oatb16.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb16.ProductModelID = maindatabase.RiceOAEquipTableData16.ProductModelID;
                                oatb16.MasterProductName = maindatabase.RiceOAEquipTableData16.MasterProductName;
                                oatb16.ProductName = maindatabase.RiceOAEquipTableData16.ProductName;

                                oatb16.Quantity = maindatabase.RiceOAEquipTableData16.Quantity;
                                oatb16.UnitPrice = maindatabase.RiceOAEquipTableData16.UnitPrice;
                                oatb16.TotalPrice = maindatabase.RiceOAEquipTableData16.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData16.MasterProductID == 7 || maindatabase.RiceOAEquipTableData16.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid16 = maindatabase.RiceOAEquipTableData16.ProductModelID;
                                    ProductModel ProductModel16 = db.ProductModel.Find(prodmodelid16);
                                    var prodname16 = ProductModel16.ProductModelName;
                                    String[] prodnamesplit2 = prodname16.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb16.MotorQ = motorq2;
                                    oatb16.MotorRating = motorrating2;
                                }
                                db.Entry(oatb16).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab16 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData16.ROATBID);

                                Oatab16.Quantity = maindatabase.RiceOAEquipTableData16.Quantity;
                                Oatab16.UnitPrice = maindatabase.RiceOAEquipTableData16.UnitPrice;
                                Oatab16.TotalPrice = maindatabase.RiceOAEquipTableData16.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData16.MasterProductID == 7 || maindatabase.RiceOAEquipTableData16.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid16 = maindatabase.RiceOAEquipTableData16.ProductModelID;
                                    ProductModel ProductModel16 = db.ProductModel.Find(prodmodelid16);
                                    var prodname16 = ProductModel16.ProductModelName;
                                    String[] prodnamesplit2 = prodname16.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab16.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab16.Pass = maindatabase.RiceOAEquipTableData16.Pass;
                                    Oatab16.MotorType = maindatabase.RiceOAEquipTableData16.MotorType;
                                    Oatab16.PolishRequirement = maindatabase.RiceOAEquipTableData16.PolishRequirement;
                                    Oatab16.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab16.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab16.MotorQ = motorq2;
                                    Oatab16.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab16).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData16.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion


                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList16 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData16.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData16.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData16.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList16 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList16.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData17.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData17.UnitPrice != null && maindatabase.RiceOAEquipTableData17.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData17.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData17.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb17 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData17.ROATBID);
                                //RiceOAEquipTableData oatb17 = new RiceOAEquipTableData();
                                oatb17 = maindatabase.RiceOAEquipTableData17;
                                oatb17.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb17.ProductModelID = maindatabase.RiceOAEquipTableData17.ProductModelID;
                                oatb17.MasterProductName = maindatabase.RiceOAEquipTableData17.MasterProductName;
                                oatb17.ProductName = maindatabase.RiceOAEquipTableData17.ProductName;

                                oatb17.Quantity = maindatabase.RiceOAEquipTableData17.Quantity;
                                oatb17.UnitPrice = maindatabase.RiceOAEquipTableData17.UnitPrice;
                                oatb17.TotalPrice = maindatabase.RiceOAEquipTableData17.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData17.MasterProductID == 7 || maindatabase.RiceOAEquipTableData17.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid17 = maindatabase.RiceOAEquipTableData17.ProductModelID;
                                    ProductModel ProductModel17 = db.ProductModel.Find(prodmodelid17);
                                    var prodname17 = ProductModel17.ProductModelName;
                                    String[] prodnamesplit2 = prodname17.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb17.MotorQ = motorq2;
                                    oatb17.MotorRating = motorrating2;
                                }
                                db.Entry(oatb17).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab17 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData17.ROATBID);

                                Oatab17.Quantity = maindatabase.RiceOAEquipTableData17.Quantity;
                                Oatab17.UnitPrice = maindatabase.RiceOAEquipTableData17.UnitPrice;
                                Oatab17.TotalPrice = maindatabase.RiceOAEquipTableData17.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData17.MasterProductID == 7 || maindatabase.RiceOAEquipTableData17.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid17 = maindatabase.RiceOAEquipTableData17.ProductModelID;
                                    ProductModel ProductModel17 = db.ProductModel.Find(prodmodelid17);
                                    var prodname17 = ProductModel17.ProductModelName;
                                    String[] prodnamesplit2 = prodname17.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab17.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab17.Pass = maindatabase.RiceOAEquipTableData17.Pass;
                                    Oatab17.MotorType = maindatabase.RiceOAEquipTableData17.MotorType;
                                    Oatab17.PolishRequirement = maindatabase.RiceOAEquipTableData17.PolishRequirement;
                                    Oatab17.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab17.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab17.MotorQ = motorq2;
                                    Oatab17.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab17).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData17.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList17 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData17.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData17.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData17.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList17 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList17.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData18.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData18.UnitPrice != null && maindatabase.RiceOAEquipTableData18.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData18.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData18.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb18 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData18.ROATBID);
                                //RiceOAEquipTableData oatb18 = new RiceOAEquipTableData();
                                oatb18 = maindatabase.RiceOAEquipTableData18;
                                oatb18.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb18.ProductModelID = maindatabase.RiceOAEquipTableData18.ProductModelID;
                                oatb18.MasterProductName = maindatabase.RiceOAEquipTableData18.MasterProductName;
                                oatb18.ProductName = maindatabase.RiceOAEquipTableData18.ProductName;

                                oatb18.Quantity = maindatabase.RiceOAEquipTableData18.Quantity;
                                oatb18.UnitPrice = maindatabase.RiceOAEquipTableData18.UnitPrice;
                                oatb18.TotalPrice = maindatabase.RiceOAEquipTableData18.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData18.MasterProductID == 7 || maindatabase.RiceOAEquipTableData18.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid18 = maindatabase.RiceOAEquipTableData18.ProductModelID;
                                    ProductModel ProductModel18 = db.ProductModel.Find(prodmodelid18);
                                    var prodname18 = ProductModel18.ProductModelName;
                                    String[] prodnamesplit2 = prodname18.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb18.MotorQ = motorq2;
                                    oatb18.MotorRating = motorrating2;
                                }
                                db.Entry(oatb18).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab18 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData18.ROATBID);

                                Oatab18.Quantity = maindatabase.RiceOAEquipTableData18.Quantity;
                                Oatab18.UnitPrice = maindatabase.RiceOAEquipTableData18.UnitPrice;
                                Oatab18.TotalPrice = maindatabase.RiceOAEquipTableData18.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData18.MasterProductID == 7 || maindatabase.RiceOAEquipTableData18.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid18 = maindatabase.RiceOAEquipTableData18.ProductModelID;
                                    ProductModel ProductModel18 = db.ProductModel.Find(prodmodelid18);
                                    var prodname18 = ProductModel18.ProductModelName;
                                    String[] prodnamesplit2 = prodname18.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab18.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab18.Pass = maindatabase.RiceOAEquipTableData18.Pass;
                                    Oatab18.MotorType = maindatabase.RiceOAEquipTableData18.MotorType;
                                    Oatab18.PolishRequirement = maindatabase.RiceOAEquipTableData18.PolishRequirement;
                                    Oatab18.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab18.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab18.MotorQ = motorq2;
                                    Oatab18.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab18).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData18.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion


                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList18 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData18.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData18.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData18.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList18 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList18.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData19.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData19.UnitPrice != null && maindatabase.RiceOAEquipTableData19.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData19.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData19.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb19 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData19.ROATBID);
                                //RiceOAEquipTableData oatb19 = new RiceOAEquipTableData();
                                oatb19 = maindatabase.RiceOAEquipTableData19;
                                oatb19.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb19.ProductModelID = maindatabase.RiceOAEquipTableData19.ProductModelID;
                                oatb19.MasterProductName = maindatabase.RiceOAEquipTableData19.MasterProductName;
                                oatb19.ProductName = maindatabase.RiceOAEquipTableData19.ProductName;

                                oatb19.Quantity = maindatabase.RiceOAEquipTableData19.Quantity;
                                oatb19.UnitPrice = maindatabase.RiceOAEquipTableData19.UnitPrice;
                                oatb19.TotalPrice = maindatabase.RiceOAEquipTableData19.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData19.MasterProductID == 7 || maindatabase.RiceOAEquipTableData19.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid19 = maindatabase.RiceOAEquipTableData19.ProductModelID;
                                    ProductModel ProductModel19 = db.ProductModel.Find(prodmodelid19);
                                    var prodname19 = ProductModel19.ProductModelName;
                                    String[] prodnamesplit2 = prodname19.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb19.MotorQ = motorq2;
                                    oatb19.MotorRating = motorrating2;
                                }
                                db.Entry(oatb19).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab19 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData19.ROATBID);

                                Oatab19.Quantity = maindatabase.RiceOAEquipTableData19.Quantity;
                                Oatab19.UnitPrice = maindatabase.RiceOAEquipTableData19.UnitPrice;
                                Oatab19.TotalPrice = maindatabase.RiceOAEquipTableData19.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData19.MasterProductID == 7 || maindatabase.RiceOAEquipTableData19.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid19 = maindatabase.RiceOAEquipTableData19.ProductModelID;
                                    ProductModel ProductModel19 = db.ProductModel.Find(prodmodelid19);
                                    var prodname19 = ProductModel19.ProductModelName;
                                    String[] prodnamesplit2 = prodname19.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab19.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab19.Pass = maindatabase.RiceOAEquipTableData19.Pass;
                                    Oatab19.MotorType = maindatabase.RiceOAEquipTableData19.MotorType;
                                    Oatab19.PolishRequirement = maindatabase.RiceOAEquipTableData19.PolishRequirement;
                                    Oatab19.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab19.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab19.MotorQ = motorq2;
                                    Oatab19.MotorRating = motorrating2;
                                }
                                db.Entry(Oatab19).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData19.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList19 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData19.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData19.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData19.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList19 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList19.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData20.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData20.UnitPrice != null && maindatabase.RiceOAEquipTableData20.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData20.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData20.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb20 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData20.ROATBID);
                                //RiceOAEquipTableData oatb20 = new RiceOAEquipTableData();
                                oatb20 = maindatabase.RiceOAEquipTableData20;
                                oatb20.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb20.ProductModelID = maindatabase.RiceOAEquipTableData20.ProductModelID;
                                oatb20.MasterProductName = maindatabase.RiceOAEquipTableData20.MasterProductName;
                                oatb20.ProductName = maindatabase.RiceOAEquipTableData20.ProductName;

                                oatb20.Quantity = maindatabase.RiceOAEquipTableData20.Quantity;
                                oatb20.UnitPrice = maindatabase.RiceOAEquipTableData20.UnitPrice;
                                oatb20.TotalPrice = maindatabase.RiceOAEquipTableData20.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData20.MasterProductID == 7 || maindatabase.RiceOAEquipTableData20.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid20 = maindatabase.RiceOAEquipTableData20.ProductModelID;
                                    ProductModel ProductModel20 = db.ProductModel.Find(prodmodelid20);
                                    var prodname20 = ProductModel20.ProductModelName;
                                    String[] prodnamesplit2 = prodname20.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb20.MotorQ = motorq2;
                                    oatb20.MotorRating = motorrating2;
                                }
                                db.Entry(oatb20).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab20 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData20.ROATBID);

                                Oatab20.Quantity = maindatabase.RiceOAEquipTableData20.Quantity;
                                Oatab20.UnitPrice = maindatabase.RiceOAEquipTableData20.UnitPrice;
                                Oatab20.TotalPrice = maindatabase.RiceOAEquipTableData20.TotalPrice;


                                if (maindatabase.RiceOAEquipTableData20.MasterProductID == 7 || maindatabase.RiceOAEquipTableData20.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid20 = maindatabase.RiceOAEquipTableData20.ProductModelID;
                                    ProductModel ProductModel20 = db.ProductModel.Find(prodmodelid20);
                                    var prodname20 = ProductModel20.ProductModelName;
                                    String[] prodnamesplit2 = prodname20.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab20.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab20.Pass = maindatabase.RiceOAEquipTableData20.Pass;
                                    Oatab20.MotorType = maindatabase.RiceOAEquipTableData20.MotorType;
                                    Oatab20.PolishRequirement = maindatabase.RiceOAEquipTableData20.PolishRequirement;
                                    Oatab20.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab20.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab20.MotorQ = motorq2;
                                    Oatab20.MotorRating = motorrating2;
                                }

                                db.Entry(Oatab20).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            int id = maindatabase.RiceOAEquipTableData20.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList20 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData20.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData20.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData20.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList20 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList20.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData21.ProductModelID != 0)
                    {
                        #region

                        if (maindatabase.RiceOAEquipTableData21.UnitPrice != null && maindatabase.RiceOAEquipTableData5.UnitPrice != "")
                        {
                            var oatch = db.RiceOAEquipTableData.Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData21.ProductModelID).Where(m => m.ROAID == maindatabase.RiceOAEquipTableData21.ROAID).Count();
                            if (oatch == 0)
                            {
                                ViewBag.Ispresent1 = true;
                                RiceOAEquipTableData oatb21 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData21.ROATBID);
                                //RiceOAEquipTableData oatb21 = new RiceOAEquipTableData();
                                oatb21 = maindatabase.RiceOAEquipTableData21;
                                oatb21.ROAID = maindatabase.RiceOAEquipGeneralData.ROAID;
                                oatb21.ProductModelID = maindatabase.RiceOAEquipTableData21.ProductModelID;
                                oatb21.MasterProductName = maindatabase.RiceOAEquipTableData21.MasterProductName;
                                oatb21.ProductName = maindatabase.RiceOAEquipTableData21.ProductName;

                                oatb21.Quantity = maindatabase.RiceOAEquipTableData21.Quantity;
                                oatb21.UnitPrice = maindatabase.RiceOAEquipTableData21.UnitPrice;
                                oatb21.TotalPrice = maindatabase.RiceOAEquipTableData21.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData21.MasterProductID == 7 || maindatabase.RiceOAEquipTableData21.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid21 = maindatabase.RiceOAEquipTableData21.ProductModelID;
                                    ProductModel ProductModel21 = db.ProductModel.Find(prodmodelid21);
                                    var prodname21 = ProductModel21.ProductModelName;
                                    String[] prodnamesplit2 = prodname21.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    oatb21.MotorQ = motorq2;
                                    oatb21.MotorRating = motorrating2;
                                }
                                db.Entry(oatb21).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                RiceOAEquipTableData Oatab21 = db.RiceOAEquipTableData.Find(maindatabase.RiceOAEquipTableData21.ROATBID);

                                Oatab21.Quantity = maindatabase.RiceOAEquipTableData21.Quantity;
                                Oatab21.UnitPrice = maindatabase.RiceOAEquipTableData21.UnitPrice;
                                Oatab21.TotalPrice = maindatabase.RiceOAEquipTableData21.TotalPrice;

                                if (maindatabase.RiceOAEquipTableData21.MasterProductID == 7 || maindatabase.RiceOAEquipTableData21.MasterProductID == 5)
                                {
                                    //to store MotorQ and MototrRating
                                    int prodmodelid21 = maindatabase.RiceOAEquipTableData21.ProductModelID;
                                    ProductModel ProductModel21 = db.ProductModel.Find(prodmodelid21);
                                    var prodname21 = ProductModel21.ProductModelName;
                                    String[] prodnamesplit2 = prodname21.Split('/');
                                    var motorrating2 = "";
                                    var motorq2 = "";
                                    if (prodnamesplit2.Length == 1)
                                    {
                                        motorrating2 = prodnamesplit2[0];
                                    }
                                    else
                                    {
                                        motorrating2 = prodnamesplit2[1];
                                        motorq2 = prodnamesplit2[2];
                                        motorrating2 = motorrating2.Trim();
                                        motorq2 = motorq2.Trim();
                                    }

                                    Oatab21.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                                    Oatab21.Pass = maindatabase.RiceOAEquipTableData21.Pass;
                                    Oatab21.MotorType = maindatabase.RiceOAEquipTableData21.MotorType;
                                    Oatab21.PolishRequirement = maindatabase.RiceOAEquipTableData21.PolishRequirement;
                                    Oatab21.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                                    Oatab21.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;

                                    Oatab21.MotorQ = motorq2;
                                    Oatab21.MotorRating = motorrating2;
                                }

                                db.Entry(Oatab21).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            int id = maindatabase.RiceOAEquipTableData21.ROATBID;
                            RiceOAEquipTableData oad = db.RiceOAEquipTableData.Find(id);
                            db.RiceOAEquipTableData.Remove(oad);
                            db.SaveChanges();
                        }
                        #endregion

                        //Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                        var SOTRMList21 = db.SOT_Temp_tbl.Where(m => m.QGID == maindatabase.RiceOAEquipGeneralData.RQGID).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData21.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData21.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData21.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                        if (SOTRMList21 != null)
                        {
                            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList21.TSOTID);
                            db.SOT_Temp_tbl.Remove(tempsot);
                            db.SaveChanges();
                        }
                    }

                    //Updating Tin Number in MDB Statutory Table
                    var mqgid = maindatabase.RiceOAEquipGeneralData.RQGID;
                    int mdbid = db.QGEquipGeneralData.Where(m => m.QGID == mqgid).Select(m => m.MDBID).Single();
                    var tinc = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbid).Count();
                    var tinnum = maindatabase.RiceOAEquipGeneralData.TinNumber;
                    var pannum = maindatabase.RiceOAEquipGeneralData.PANCardNo;
                    if (tinc == 0)
                    {
                        MDBStatutoryNumber mdbst = new MDBStatutoryNumber();
                        mdbst.MDBID = mdbid;
                        mdbst.CompanyPAN = pannum;
                        mdbst.TIN = tinnum;
                        db.MDBStatutoryNumber.Add(mdbst);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        //updating for the PAN and TIN Numbers
                        var MDBSNID = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbid).Select(m=>m.MDBSNID).SingleOrDefault();
                        MDBStatutoryNumber mdbst = db.MDBStatutoryNumber.Find(MDBSNID);
                        mdbst.CompanyPAN = pannum;
                        mdbst.TIN = tinnum;
                        db.Entry(mdbst).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //Removing Quotation from OA View after it is sent for Approval
                    int qgid = maindatabase.RiceOAEquipGeneralData.RQGID;
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == qgid).ToList();
                    var IsLatest = db.SOT_Temp_tbl.Where(m => m.QGID == qgid).ToList();//code added by sneha 24 july 2014
                    foreach (var qg in quogen)
                    {
                        qg.Ordergenerated = 1;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    foreach (var ILatest in IsLatest)//Code added bY sneha 24 july 2014
                    {
                        ILatest.Islatestquo= 1;
                        db.Entry(ILatest).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //Commented On 14-12-2016

                    //var oaappst = maindatabase.RiceOAEquipGeneralData;
                    //oaappst.OAStatus = 1;
                    //db.Entry(oaappst).State = EntityState.Modified;
                    //db.SaveChanges();

                    var sotset = db.SOT.Where(m => m.QGID == qgid).ToList();
                    foreach (var by in sotset)
                    {
                        by.BYJTChances = 100;
                        db.Entry(by).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    ////Updating to SOTRM list NOTE: if IsLatestquo value is 1 then the Item Model has been generated.
                    //var SOTRMList = db.SOT_Temp_tbl.Where(m => m.QGID == qgid).Where(m => m.ProductID == maindatabase.RiceOAEquipTableData1.ProductID).Where(m => m.ProductModelID == maindatabase.RiceOAEquipTableData1.ProductModelID).Where(m => m.MasterProductID == maindatabase.RiceOAEquipTableData1.MasterProductID).Where(m => m.BYJTChances == 100).SingleOrDefault();
                    ////foreach (var srmlist in SOTRMList)
                    ////{
                    //if (SOTRMList != null)
                    //{
                    //    SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(SOTRMList.TSOTID);
                    //    db.SOT_Temp_tbl.Remove(tempsot);
                    //    db.SaveChanges();
                    //}
                    ////}
                }
                TempData["oagen"] = "Order Acknowledgement Sent for Approval!!!!";

                return RedirectToAction("RiceOAChannelStatus");
            }
            else
            {
                var mdbid = maindatabase.RiceOAEquipGeneralData.MDBID;
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.OAID = maindatabase.RiceOAEquipGeneralData.ROAID;
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

                ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        public string quotationnumber()
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String quotationnumber;
            var quotmod = from OANumber in db.RiceOAEquipGeneralData
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
            return View("RiceOAView");
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
            if (len.Length == 1)
            {
                cpik = "0" + ad;
            }
            else
            {
                cpik = ad.ToString();
            }
            oanum = split[0] + "-" + "RC" + year + "-" + split[2] + "-" + cpik;
            quotationnumber = oanum;
            return quotationnumber;
        }

        // GET: /RiceOA/RiceOARevise
        public ActionResult RiceOARevise(int oaid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (oaid != 0)
            {
                var tin = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.TinNumber).Single();
                ViewBag.TinNumber = tin;
                int mdid=db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.MDBID).Single();
                var pannum = db.MDBStatutoryNumber.Where(m => m.MDBID == mdid).Select(m => m.CompanyPAN).SingleOrDefault();
                ViewBag.PANNumber = pannum;
                RiceOA quo = new RiceOA();
                var qgdb = db.RiceOAEquipGeneralData.Find(oaid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid).Where(m=>m.IsSOTStatus==1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.RiceOAEquipGeneralData = qgdb;
                    quo.RiceOAEquipPayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid).Single();
                    if (modelcnt == 21)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = null;
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
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
                    ViewBag.ProductVariety = quo.RiceOAEquipGeneralData.ProductVariety;
                    ViewBag.Type = quo.RiceOAEquipGeneralData.TypeRice;
                    ViewBag.PaddySize = quo.RiceOAEquipGeneralData.PaddySize;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                RiceOA quo = new RiceOA();
                var qgdb1 = db.RiceOAEquipGeneralData.Where(m => m.OANumber == quotnumber).Select(m => m.ROAID).SingleOrDefault();
                var qgdb = db.RiceOAEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == qgdb1).Where(m=>m.IsSOTStatus==1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.RiceOAEquipGeneralData = qgdb;
                    quo.RiceOAEquipPayment = db.RiceOAEquipPayment.Find(qgdb1);
                    if (modelcnt == 21)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = null;
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
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
                    ViewBag.ProductVariety = quo.RiceOAEquipGeneralData.ProductVariety;
                    ViewBag.Type = quo.RiceOAEquipGeneralData.TypeRice;
                    ViewBag.PaddySize = quo.RiceOAEquipGeneralData.PaddySize;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        // POST: /RiceOA/RiceOARevise
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiceOARevise(RiceOA maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.RiceOAEquipTableData1.ProductModelID != 0)
                {

                    var revqg = maindatabase.RiceOAEquipGeneralData.OANumber;
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
                    // var previosqg = db.RiceOAEquipGeneralData.Where(m => m.OANumber == fullqn).First();
                    var previosqg = db.RiceOAEquipGeneralData.Where(m => m.ROAID == maindatabase.RiceOAEquipTableData1.ROAID).SingleOrDefault();
                    previosqg.Islatest = 1;
                    db.Entry(previosqg).State = EntityState.Modified;
                    db.SaveChanges();

                    //inserting new row into RICEOA general table
                    maindatabase.RiceOAEquipGeneralData.CPID = loginname.CPID;
                    maindatabase.RiceOAEquipGeneralData.MDBID = (int)(mdbid);
                    maindatabase.RiceOAEquipGeneralData.QuotationDate = DateTime.Now;
                    maindatabase.RiceOAEquipGeneralData.PackingAndForwarding = maindatabase.RiceOAEquipGeneralData.PackingAndForwarding;
                    maindatabase.RiceOAEquipGeneralData.TransistInsurance = maindatabase.RiceOAEquipGeneralData.TransistInsurance;
                    maindatabase.RiceOAEquipGeneralData.Freight = maindatabase.RiceOAEquipGeneralData.Freight;
                    maindatabase.RiceOAEquipGeneralData.MDBGeneralData = db.RiceOAEquipGeneralData.Where(m => m.ROAID == maindatabase.RiceOAEquipTableData1.ROAID).Select(m=>m.MDBGeneralData).SingleOrDefault();
                    db.RiceOAEquipGeneralData.Add(maindatabase.RiceOAEquipGeneralData);
                    //  db.SaveChanges();

                    //to Check the Exact Errors from table and its fields
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Debug.WriteLine("Property: {0} Error: {1}",
                                           validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }

                    //inserting new row to payment table
                    var qg = from s in db.RiceOAEquipGeneralData
                             select s;
                    qg = qg.OrderByDescending(m => m.ROAID);
                    int qgid = qg.Select(m => m.ROAID).First();
                    maindatabase.RiceOAEquipPayment.ROAID = qgid;
                    db.RiceOAEquipPayment.Add(maindatabase.RiceOAEquipPayment);
                    //db.SaveChanges();

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Debug.WriteLine("Property: {0} Error: {1}",
                                           validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }

                    //Inserting the new row for table data
                    if (maindatabase.RiceOAEquipTableData1.MasterProductID == 7 || maindatabase.RiceOAEquipTableData1.MasterProductID == 5)
                    {
                        int prodmodelid1 = maindatabase.RiceOAEquipTableData1.ProductModelID;
                        ProductModel ProductModel1 = db.ProductModel.Find(prodmodelid1);
                        var prodname1 = ProductModel1.ProductModelName;
                        String[] prodnamesplit1 = prodname1.Split('/');
                        var motorrating1 = prodnamesplit1[1];
                        var motorq1 = prodnamesplit1[2];

                        motorrating1 = motorrating1.Trim();
                        motorq1 = motorq1.Trim();
                        maindatabase.RiceOAEquipTableData1.IsSOTStatus = 1;
                        maindatabase.RiceOAEquipTableData1.MotorRating = motorrating1;
                        maindatabase.RiceOAEquipTableData1.MotorQ = motorq1;
                        maindatabase.RiceOAEquipTableData1.ProductModel = ProductModel1;
                        maindatabase.RiceOAEquipTableData1.ROAID = qgid;
                        db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData1);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Debug.WriteLine("Property: {0} Error: {1}",
                                               validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        //maindatabase.RiceOAEquipTableData1.ROAID = qgid;
                        RiceOAEquipTableData oa = new RiceOAEquipTableData();
                        oa.ROAID = qgid;
                        oa.ModelNum = maindatabase.RiceOAEquipTableData1.ModelNum;
                        oa.Quantity = maindatabase.RiceOAEquipTableData1.Quantity;
                        oa.UnitPrice = maindatabase.RiceOAEquipTableData1.UnitPrice;
                        oa.TotalPrice = maindatabase.RiceOAEquipTableData1.TotalPrice;
                        oa.ModelDesc = maindatabase.RiceOAEquipTableData1.ModelDesc;
                        oa.Exclusion = maindatabase.RiceOAEquipTableData1.Exclusion;
                        oa.ProductModelID = maindatabase.RiceOAEquipTableData1.ProductModelID;
                        oa.ProductID = maindatabase.RiceOAEquipTableData1.ProductID;
                        oa.MasterProductID = maindatabase.RiceOAEquipTableData1.MasterProductID;
                        oa.MasterProductName = maindatabase.RiceOAEquipTableData1.MasterProductName;
                        oa.ProductName = maindatabase.RiceOAEquipTableData1.ProductName;
                        oa.Pass = maindatabase.RiceOAEquipTableData1.Pass;
                        oa.MotorType = maindatabase.RiceOAEquipTableData1.MotorType;
                        oa.PolishRequirement = maindatabase.RiceOAEquipTableData1.PolishRequirement;
                        oa.TypeRice = maindatabase.RiceOAEquipTableData1.TypeRice;
                        oa.PaddySize = maindatabase.RiceOAEquipTableData1.PaddySize;
                        oa.Capacity = maindatabase.RiceOAEquipTableData1.Capacity;
                        oa.MotorQ = maindatabase.RiceOAEquipTableData1.MotorQ;

                        oa.MotorRating = maindatabase.RiceOAEquipTableData1.MotorRating;
                        oa.IsSOTStatus = maindatabase.RiceOAEquipTableData1.IsSOTStatus;
                        db.RiceOAEquipTableData.Add(oa);
                        // db.SaveChanges();

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Debug.WriteLine("Property: {0} Error: {1}",
                                               validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData2.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData2.MasterProductID == 7 || maindatabase.RiceOAEquipTableData2.MasterProductID == 5)
                        {
                            try
                            {
                                int prodmodelid2 = maindatabase.RiceOAEquipTableData2.ProductModelID;
                                ProductModel ProductModel2 = db.ProductModel.Find(prodmodelid2);
                                var prodname2 = ProductModel2.ProductModelName;
                                String[] prodnamesplit2 = prodname2.Split('/');
                                var motorrating2 = prodnamesplit2[1];
                                var motorq2 = prodnamesplit2[2];

                                motorrating2 = motorrating2.Trim();
                                motorq2 = motorq2.Trim();

                                maindatabase.RiceOAEquipTableData2.MotorRating = motorrating2;
                                maindatabase.RiceOAEquipTableData2.MotorQ = motorq2;
                                maindatabase.RiceOAEquipTableData2.IsSOTStatus = 1;
                                maindatabase.RiceOAEquipTableData2.ROAID = qgid;
                                db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData2);
                                db.SaveChanges();
                            }
                            catch (DbEntityValidationException e)
                            {
                                foreach (var eve in e.EntityValidationErrors)
                                {
                                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                    foreach (var ve in eve.ValidationErrors)
                                    {
                                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                            ve.PropertyName, ve.ErrorMessage);
                                    }
                                }

                            }
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData2.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData2);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData3.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData3.MasterProductID == 7 || maindatabase.RiceOAEquipTableData3.MasterProductID == 5)
                        {
                            int prodmodelid3 = maindatabase.RiceOAEquipTableData3.ProductModelID;
                            ProductModel ProductModel3 = db.ProductModel.Find(prodmodelid3);
                            var prodname3 = ProductModel3.ProductModelName;
                            String[] prodnamesplit3 = prodname3.Split('/');
                            var motorrating3 = prodnamesplit3[1];
                            var motorq3 = prodnamesplit3[2];

                            motorrating3 = motorrating3.Trim();
                            motorq3 = motorq3.Trim();

                            maindatabase.RiceOAEquipTableData3.MotorRating = motorrating3;
                            maindatabase.RiceOAEquipTableData3.MotorQ = motorq3;
                            maindatabase.RiceOAEquipTableData3.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData3.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData3);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData3.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData3);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData4.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData4.MasterProductID == 7 || maindatabase.RiceOAEquipTableData4.MasterProductID == 5)
                        {
                            int prodmodelid4 = maindatabase.RiceOAEquipTableData4.ProductModelID;
                            ProductModel ProductModel4 = db.ProductModel.Find(prodmodelid4);
                            var prodname4 = ProductModel4.ProductModelName;
                            String[] prodnamesplit4 = prodname4.Split('/');
                            var motorrating4 = prodnamesplit4[1];
                            var motorq4 = prodnamesplit4[2];

                            motorrating4 = motorrating4.Trim();
                            motorq4 = motorq4.Trim();

                            maindatabase.RiceOAEquipTableData4.MotorRating = motorrating4;
                            maindatabase.RiceOAEquipTableData4.MotorQ = motorq4;
                            maindatabase.RiceOAEquipTableData4.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData4.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData4);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData4.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData4);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData5.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData5.MasterProductID == 7 || maindatabase.RiceOAEquipTableData5.MasterProductID == 5)
                        {
                            int prodmodelid5 = maindatabase.RiceOAEquipTableData5.ProductModelID;
                            ProductModel ProductModel5 = db.ProductModel.Find(prodmodelid5);
                            var prodname5 = ProductModel5.ProductModelName;
                            String[] prodnamesplit5 = prodname5.Split('/');
                            var motorrating5 = prodnamesplit5[1];
                            var motorq5 = prodnamesplit5[2];

                            motorrating5 = motorrating5.Trim();
                            motorq5 = motorq5.Trim();

                            maindatabase.RiceOAEquipTableData5.MotorRating = motorrating5;
                            maindatabase.RiceOAEquipTableData5.MotorQ = motorq5;
                            maindatabase.RiceOAEquipTableData5.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData5.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData5);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData5.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData5);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData6.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData6.MasterProductID == 7 || maindatabase.RiceOAEquipTableData6.MasterProductID == 5)
                        {
                            int prodmodelid6 = maindatabase.RiceOAEquipTableData6.ProductModelID;
                            ProductModel ProductModel6 = db.ProductModel.Find(prodmodelid6);
                            var prodname6 = ProductModel6.ProductModelName;
                            String[] prodnamesplit6 = prodname6.Split('/');
                            var motorrating6 = prodnamesplit6[1];
                            var motorq6 = prodnamesplit6[2];

                            motorrating6 = motorrating6.Trim();
                            motorq6 = motorq6.Trim();

                            maindatabase.RiceOAEquipTableData6.MotorRating = motorrating6;
                            maindatabase.RiceOAEquipTableData6.MotorQ = motorq6;
                            maindatabase.RiceOAEquipTableData6.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData6.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData6);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData7.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData7.MasterProductID == 7 || maindatabase.RiceOAEquipTableData7.MasterProductID == 5)
                        {
                            int prodmodelid7 = maindatabase.RiceOAEquipTableData7.ProductModelID;
                            ProductModel ProductModel7 = db.ProductModel.Find(prodmodelid7);
                            var prodname7 = ProductModel7.ProductModelName;
                            String[] prodnamesplit7 = prodname7.Split('/');
                            var motorrating7 = prodnamesplit7[1];
                            var motorq7 = prodnamesplit7[2];

                            motorrating7 = motorrating7.Trim();
                            motorq7 = motorq7.Trim();

                            maindatabase.RiceOAEquipTableData7.MotorRating = motorrating7;
                            maindatabase.RiceOAEquipTableData7.MotorQ = motorq7;
                            maindatabase.RiceOAEquipTableData7.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData7.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData7);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData7.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData7);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData8.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData8.MasterProductID == 7 || maindatabase.RiceOAEquipTableData8.MasterProductID == 5)
                        {
                            int prodmodelid8 = maindatabase.RiceOAEquipTableData8.ProductModelID;
                            ProductModel ProductModel8 = db.ProductModel.Find(prodmodelid8);
                            var prodname8 = ProductModel8.ProductModelName;
                            String[] prodnamesplit8 = prodname8.Split('/');
                            var motorrating8 = prodnamesplit8[1];
                            var motorq8 = prodnamesplit8[2];

                            motorrating8 = motorrating8.Trim();
                            motorq8 = motorq8.Trim();

                            maindatabase.RiceOAEquipTableData8.MotorRating = motorrating8;
                            maindatabase.RiceOAEquipTableData8.MotorQ = motorq8;
                            maindatabase.RiceOAEquipTableData8.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData8.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData8);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData8.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData8);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData9.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData9.MasterProductID == 7 || maindatabase.RiceOAEquipTableData9.MasterProductID == 5)
                        {
                            int prodmodelid9 = maindatabase.RiceOAEquipTableData9.ProductModelID;
                            ProductModel ProductModel9 = db.ProductModel.Find(prodmodelid9);
                            var prodname9 = ProductModel9.ProductModelName;
                            String[] prodnamesplit9 = prodname9.Split('/');
                            var motorrating9 = prodnamesplit9[1];
                            var motorq9 = prodnamesplit9[2];

                            motorrating9 = motorrating9.Trim();
                            motorq9 = motorq9.Trim();

                            maindatabase.RiceOAEquipTableData9.MotorRating = motorrating9;
                            maindatabase.RiceOAEquipTableData9.MotorQ = motorq9;
                            maindatabase.RiceOAEquipTableData9.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData9.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData9);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData9.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData9);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData10.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData10.MasterProductID == 7 || maindatabase.RiceOAEquipTableData10.MasterProductID == 5)
                        {
                            int prodmodelid10 = maindatabase.RiceOAEquipTableData10.ProductModelID;
                            ProductModel ProductModel10 = db.ProductModel.Find(prodmodelid10);
                            var prodname10 = ProductModel10.ProductModelName;
                            String[] prodnamesplit10 = prodname10.Split('/');
                            var motorrating10 = prodnamesplit10[1];
                            var motorq10 = prodnamesplit10[2];

                            motorrating10 = motorrating10.Trim();
                            motorq10 = motorq10.Trim();

                            maindatabase.RiceOAEquipTableData10.MotorRating = motorrating10;
                            maindatabase.RiceOAEquipTableData10.MotorQ = motorq10;
                            maindatabase.RiceOAEquipTableData10.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData10.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData10);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData10.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData10);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData11.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData11.MasterProductID == 7 || maindatabase.RiceOAEquipTableData11.MasterProductID == 5)
                        {
                            int prodmodelid11 = maindatabase.RiceOAEquipTableData11.ProductModelID;
                            ProductModel ProductModel11 = db.ProductModel.Find(prodmodelid11);
                            var prodname11 = ProductModel11.ProductModelName;
                            String[] prodnamesplit11 = prodname11.Split('/');
                            var motorrating11 = prodnamesplit11[1];
                            var motorq11 = prodnamesplit11[2];

                            motorrating11 = motorrating11.Trim();
                            motorq11 = motorq11.Trim();

                            maindatabase.RiceOAEquipTableData11.MotorRating = motorrating11;
                            maindatabase.RiceOAEquipTableData11.MotorQ = motorq11;
                            maindatabase.RiceOAEquipTableData11.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData11.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData11);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData11.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData11);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData12.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData12.MasterProductID == 7 || maindatabase.RiceOAEquipTableData12.MasterProductID == 5)
                        {
                            int prodmodelid12 = maindatabase.RiceOAEquipTableData12.ProductModelID;
                            ProductModel ProductModel12 = db.ProductModel.Find(prodmodelid12);
                            var prodname12 = ProductModel12.ProductModelName;
                            String[] prodnamesplit12 = prodname12.Split('/');
                            var motorrating12 = prodnamesplit12[1];
                            var motorq12 = prodnamesplit12[2];

                            motorrating12 = motorrating12.Trim();
                            motorq12 = motorq12.Trim();

                            maindatabase.RiceOAEquipTableData12.MotorRating = motorrating12;
                            maindatabase.RiceOAEquipTableData12.MotorQ = motorq12;
                            maindatabase.RiceOAEquipTableData12.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData12.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData12);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData12.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData12);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData13.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData13.MasterProductID == 7 || maindatabase.RiceOAEquipTableData13.MasterProductID == 5)
                        {
                            int prodmodelid13 = maindatabase.RiceOAEquipTableData13.ProductModelID;
                            ProductModel ProductModel13 = db.ProductModel.Find(prodmodelid13);
                            var prodname13 = ProductModel13.ProductModelName;
                            String[] prodnamesplit13 = prodname13.Split('/');
                            var motorrating13 = prodnamesplit13[1];
                            var motorq13 = prodnamesplit13[2];

                            motorrating13 = motorrating13.Trim();
                            motorq13 = motorq13.Trim();

                            maindatabase.RiceOAEquipTableData13.MotorRating = motorrating13;
                            maindatabase.RiceOAEquipTableData13.MotorQ = motorq13;
                            maindatabase.RiceOAEquipTableData13.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData13.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData13);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData13.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData13);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData14.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData14.MasterProductID == 7 || maindatabase.RiceOAEquipTableData14.MasterProductID == 5)
                        {
                            int prodmodelid14 = maindatabase.RiceOAEquipTableData14.ProductModelID;
                            ProductModel ProductModel14 = db.ProductModel.Find(prodmodelid14);
                            var prodname14 = ProductModel14.ProductModelName;
                            String[] prodnamesplit14 = prodname14.Split('/');
                            var motorrating14 = prodnamesplit14[1];
                            var motorq14 = prodnamesplit14[2];

                            motorrating14 = motorrating14.Trim();
                            motorq14 = motorq14.Trim();

                            maindatabase.RiceOAEquipTableData14.MotorRating = motorrating14;
                            maindatabase.RiceOAEquipTableData14.MotorQ = motorq14;
                            maindatabase.RiceOAEquipTableData14.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData14.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData14);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData14.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData14);
                            db.SaveChanges();
                        }
                    }

                    if (maindatabase.RiceOAEquipTableData15.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData15.MasterProductID == 7 || maindatabase.RiceOAEquipTableData15.MasterProductID == 5)
                        {
                            int prodmodelid15 = maindatabase.RiceOAEquipTableData15.ProductModelID;
                            ProductModel ProductModel15 = db.ProductModel.Find(prodmodelid15);
                            var prodname15 = ProductModel15.ProductModelName;
                            String[] prodnamesplit15 = prodname15.Split('/');
                            var motorrating15 = prodnamesplit15[1];
                            var motorq15 = prodnamesplit15[2];

                            motorrating15 = motorrating15.Trim();
                            motorq15 = motorq15.Trim();

                            maindatabase.RiceOAEquipTableData15.MotorRating = motorrating15;
                            maindatabase.RiceOAEquipTableData15.MotorQ = motorq15;
                            maindatabase.RiceOAEquipTableData15.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData15.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData15);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData15.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData15);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData16.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData16.MasterProductID == 7 || maindatabase.RiceOAEquipTableData16.MasterProductID == 5)
                        {
                            int prodmodelid16 = maindatabase.RiceOAEquipTableData16.ProductModelID;
                            ProductModel ProductModel16 = db.ProductModel.Find(prodmodelid16);
                            var prodname16 = ProductModel16.ProductModelName;
                            String[] prodnamesplit16 = prodname16.Split('/');
                            var motorrating16 = prodnamesplit16[1];
                            var motorq16 = prodnamesplit16[2];

                            motorrating16 = motorrating16.Trim();
                            motorq16 = motorq16.Trim();

                            maindatabase.RiceOAEquipTableData16.MotorRating = motorrating16;
                            maindatabase.RiceOAEquipTableData16.MotorQ = motorq16;
                            maindatabase.RiceOAEquipTableData16.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData16.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData16);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData16.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData16);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData17.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData17.MasterProductID == 7 || maindatabase.RiceOAEquipTableData17.MasterProductID == 5)
                        {
                            int prodmodelid17 = maindatabase.RiceOAEquipTableData17.ProductModelID;
                            ProductModel ProductModel17 = db.ProductModel.Find(prodmodelid17);
                            var prodname17 = ProductModel17.ProductModelName;
                            String[] prodnamesplit17 = prodname17.Split('/');
                            var motorrating17 = prodnamesplit17[1];
                            var motorq17 = prodnamesplit17[2];

                            motorrating17 = motorrating17.Trim();
                            motorq17 = motorq17.Trim();

                            maindatabase.RiceOAEquipTableData17.MotorRating = motorrating17;
                            maindatabase.RiceOAEquipTableData17.MotorQ = motorq17;
                            maindatabase.RiceOAEquipTableData17.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData17.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData17);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData17.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData17);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData18.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData18.MasterProductID == 7 || maindatabase.RiceOAEquipTableData18.MasterProductID == 5)
                        {
                            int prodmodelid18 = maindatabase.RiceOAEquipTableData18.ProductModelID;
                            ProductModel ProductModel18 = db.ProductModel.Find(prodmodelid18);
                            var prodname18 = ProductModel18.ProductModelName;
                            String[] prodnamesplit18 = prodname18.Split('/');
                            var motorrating18 = prodnamesplit18[1];
                            var motorq18 = prodnamesplit18[2];

                            motorrating18 = motorrating18.Trim();
                            motorq18 = motorq18.Trim();

                            maindatabase.RiceOAEquipTableData18.MotorRating = motorrating18;
                            maindatabase.RiceOAEquipTableData18.MotorQ = motorq18;
                            maindatabase.RiceOAEquipTableData18.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData18.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData18);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData18.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData18);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData19.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData19.MasterProductID == 7 || maindatabase.RiceOAEquipTableData19.MasterProductID == 5)
                        {
                            int prodmodelid19 = maindatabase.RiceOAEquipTableData19.ProductModelID;
                            ProductModel ProductModel19 = db.ProductModel.Find(prodmodelid19);
                            var prodname19 = ProductModel19.ProductModelName;
                            String[] prodnamesplit19 = prodname19.Split('/');
                            var motorrating19 = prodnamesplit19[1];
                            var motorq19 = prodnamesplit19[2];

                            motorrating19 = motorrating19.Trim();
                            motorq19 = motorq19.Trim();

                            maindatabase.RiceOAEquipTableData19.MotorRating = motorrating19;
                            maindatabase.RiceOAEquipTableData19.MotorQ = motorq19;
                            maindatabase.RiceOAEquipTableData19.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData19.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData19);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData19.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData19);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData20.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData20.MasterProductID == 7 || maindatabase.RiceOAEquipTableData20.MasterProductID == 5)
                        {
                            int prodmodelid20 = maindatabase.RiceOAEquipTableData20.ProductModelID;
                            ProductModel ProductModel20 = db.ProductModel.Find(prodmodelid20);
                            var prodname20 = ProductModel20.ProductModelName;
                            String[] prodnamesplit20 = prodname20.Split('/');
                            var motorrating20 = prodnamesplit20[1];
                            var motorq20 = prodnamesplit20[2];

                            motorrating20 = motorrating20.Trim();
                            motorq20 = motorq20.Trim();

                            maindatabase.RiceOAEquipTableData20.MotorRating = motorrating20;
                            maindatabase.RiceOAEquipTableData20.MotorQ = motorq20;
                            maindatabase.RiceOAEquipTableData20.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData20.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData20);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData20.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData20);
                            db.SaveChanges();
                        }
                    }
                    if (maindatabase.RiceOAEquipTableData21.ProductModelID != 0)
                    {
                        if (maindatabase.RiceOAEquipTableData21.MasterProductID == 7 || maindatabase.RiceOAEquipTableData21.MasterProductID == 5)
                        {
                            int prodmodelid21 = maindatabase.RiceOAEquipTableData21.ProductModelID;
                            ProductModel ProductModel21 = db.ProductModel.Find(prodmodelid21);
                            var prodname21 = ProductModel21.ProductModelName;
                            String[] prodnamesplit21 = prodname21.Split('/');
                            var motorrating21 = prodnamesplit21[1];
                            var motorq21 = prodnamesplit21[2];

                            motorrating21 = motorrating21.Trim();
                            motorq21 = motorq21.Trim();

                            maindatabase.RiceOAEquipTableData21.MotorRating = motorrating21;
                            maindatabase.RiceOAEquipTableData21.MotorQ = motorq21;
                            maindatabase.RiceOAEquipTableData21.IsSOTStatus = 1;
                            maindatabase.RiceOAEquipTableData21.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData21);
                            db.SaveChanges();
                        }
                        else
                        {
                            maindatabase.RiceOAEquipTableData21.ROAID = qgid;
                            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData21);
                            db.SaveChanges();
                        }
                    }

                    var oaappst = maindatabase.RiceOAEquipGeneralData;
                    oaappst.OAStatus = 1;
                    db.Entry(oaappst).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Debug.WriteLine("Property: {0} Error: {1}",
                                           validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }

                    //Updating Tin Number in MDB Statutory Table
                    var mqgid = maindatabase.RiceOAEquipGeneralData.RQGID;
                    int mdbid1 = db.QGEquipGeneralData.Where(m => m.QGID == mqgid).Select(m => m.MDBID).Single();
                    var mdball = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbid1).ToList();
                    foreach (var tin in mdball)
                    {
                        tin.TIN = maindatabase.RiceOAEquipGeneralData.TinNumber;
                        tin.CompanyPAN = maindatabase.RiceOAEquipGeneralData.PANCardNo;
                        db.Entry(tin).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Debug.WriteLine("Property: {0} Error: {1}",
                                               validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    TempData["oagen"] = "Order Acknowledgement Sent for Approval!!!!";
                    //String updateparent = "<script>window.open('/Quotation/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = '/Quotation/Create';</script>";
                    //return Content(updateparent);
                }
                return RedirectToAction("RiceOAExisting");
            }
            else
            {
                RiceOA mdb = new RiceOA();
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

                ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View();
            }


            //if (mdbid.HasValue)
            //{
            //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            //    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            //    if (maindatabase.RiceOAEquipTableData1.ProductModelID != 0)
            //    {

            //        var revqg = maindatabase.RiceOAEquipGeneralData.OANumber;
            //        var revsplit = revqg.Split('-');
            //        var revcomb = revsplit[0] + "-" + revsplit[1] + "-" + revsplit[2];
            //        var revnum = Convert.ToInt32(revsplit[3]);
            //        var prvrevisednum = "";
            //        if (revnum != 0)
            //        {
            //            revnum -= 1;
            //        }
            //        if (revnum.ToString().Length == 1)
            //        {
            //            prvrevisednum = "0" + revnum;
            //        }
            //        else
            //        {
            //            prvrevisednum = revnum.ToString();
            //        }
            //        var fullqn = revcomb + "-" + prvrevisednum;
            //        var previosqg = db.RiceOAEquipGeneralData.Where(m => m.ROAID == maindatabase.RiceOAEquipTableData1.ROAID).First();
            //        previosqg.Islatest = 1;
            //        db.Entry(previosqg).State = EntityState.Modified;
            //        db.SaveChanges();

            //        MDBGeneralData mdb = db.MDBGeneralData.Find(mdbid);
            //        mdb.BillingAddress = maindatabase.RiceOAEquipGeneralData.MDBGeneralData.BillingAddress;
            //        mdb.DeleveryAddress = maindatabase.RiceOAEquipGeneralData.MDBGeneralData.DeleveryAddress;
            //        db.Entry(mdb).State = EntityState.Modified;
            //        db.SaveChanges();

            //        //maindatabase.RiceOAEquipGeneralData.MDBGeneralData = mdb;
            //        //maindatabase.RiceOAEquipGeneralData.MDBGeneralData = mdb;

            //        maindatabase.RiceOAEquipGeneralData.CPID = loginname.CPID;
            //        maindatabase.RiceOAEquipGeneralData.MDBID = (int)(mdbid);

            //        //new
            //        maindatabase.RiceOAEquipGeneralData.MDBGeneralData.OrganizationName = mdb.OrganizationName;
            //        maindatabase.RiceOAEquipGeneralData.MDBGeneralData.OrganizationType = mdb.OrganizationType;

            //        db.RiceOAEquipGeneralData.Add(maindatabase.RiceOAEquipGeneralData);
            //        try
            //        {
            //            db.SaveChanges();
            //        }
            //        catch (DbEntityValidationException dbEx)
            //        {
            //            foreach (var validationErrors in dbEx.EntityValidationErrors)
            //            {
            //                foreach (var validationError in validationErrors.ValidationErrors)
            //                {
            //                    Debug.WriteLine("Property: {0} Error: {1}",
            //                               validationError.PropertyName, validationError.ErrorMessage);
            //                }
            //            }

            //            maindatabase.RiceOAEquipGeneralData.MDBGeneralData.OrganizationName = mdb.OrganizationName;
            //            maindatabase.RiceOAEquipGeneralData.MDBGeneralData.OrganizationType = mdb.OrganizationType;
            //            //db.RiceOAEquipGeneralData.Add(maindatabase.RiceOAEquipGeneralData);
            //            //db.SaveChanges();
            //        }


            //        var qg = from s in db.RiceOAEquipGeneralData
            //                 select s;
            //        qg = qg.OrderByDescending(m => m.ROAID);
            //        int qgid = qg.Select(m => m.ROAID).First();
            //        maindatabase.RiceOAEquipPayment.ROAID = qgid;

            //        //RiceOAEquipPayment a = maindatabase.RiceOAEquipPayment;
            //        //db.RiceOAEquipPayment.Add(a);
            //        //db.SaveChanges();

            //        db.RiceOAEquipPayment.Add(maindatabase.RiceOAEquipPayment);
            //        try
            //        {
            //            db.SaveChanges();
            //        }
            //        catch (DbEntityValidationException dbEx)
            //        {
            //            foreach (var validationErrors in dbEx.EntityValidationErrors)
            //            {
            //                foreach (var validationError in validationErrors.ValidationErrors)
            //                {
            //                    Trace.TraceInformation("Property: {0} Error: {1}",
            //                                            validationError.PropertyName,
            //                                            validationError.ErrorMessage);
            //                }
            //            }
            //        }
            //        maindatabase.RiceOAEquipTableData1.ROAID = qgid;
            //        db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData1);

            //        try
            //        {
            //            db.SaveChanges();
            //        }
            //        catch (DbEntityValidationException dbEx)
            //        {
            //            foreach (var validationErrors in dbEx.EntityValidationErrors)
            //            {
            //                foreach (var validationError in validationErrors.ValidationErrors)
            //                {
            //                    Debug.WriteLine("Property: {0} Error: {1}",
            //                               validationError.PropertyName, validationError.ErrorMessage);
            //                }
            //            }
            //        }

            //        if (maindatabase.RiceOAEquipTableData2.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData2.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData2);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData3.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData3.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData3);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData4.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData4.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData4);
            //            db.SaveChanges();
            //        }

            //        if (maindatabase.RiceOAEquipTableData5.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData5.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData5);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData6.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData6.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData6);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData7.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData7.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData7);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData8.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData8.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData8);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData9.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData9.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData9);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData10.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData10.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData10);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData11.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData11.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData11);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData12.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData12.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData12);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData13.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData13.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData13);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData14.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData14.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData14);
            //            db.SaveChanges();
            //        }

            //        if (maindatabase.RiceOAEquipTableData15.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData15.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData15);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData16.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData16.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData16);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData17.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData17.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData17);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData18.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData18.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData18);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData19.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData19.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData19);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData20.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData20.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData20);
            //            db.SaveChanges();
            //        }
            //        if (maindatabase.RiceOAEquipTableData21.ProductModelID != 0)
            //        {
            //            maindatabase.RiceOAEquipTableData21.ROAID = qgid;
            //            db.RiceOAEquipTableData.Add(maindatabase.RiceOAEquipTableData21);
            //            db.SaveChanges();
            //        }

            //        var oaappst = maindatabase.RiceOAEquipGeneralData;
            //        oaappst.OAStatus = 1;
            //        db.Entry(oaappst).State = EntityState.Modified;
            //        db.SaveChanges();
            //        //Updating Tin Number in MDB Statutory Table
            //        var mqgid = maindatabase.RiceOAEquipGeneralData.RQGID;
            //        int mdbid1 = db.QGEquipGeneralData.Where(m => m.QGID == mqgid).Select(m => m.MDBID).Single();
            //        var mdball = db.MDBStatutoryNumber.Where(m => m.MDBID == mdbid1).ToList();
            //        foreach (var tin in mdball)
            //        {
            //            tin.TIN = maindatabase.RiceOAEquipGeneralData.TinNumber;
            //            db.Entry(tin).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }
            //        TempData["oagen"] = "Order Acknowledgement Sent for Approval!!!!";
            //        //String updateparent = "<script>window.open('/Quotation/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = '/Quotation/Create';</script>";
            //        //return Content(updateparent);
            //    }
            //    return RedirectToAction("RiceOAExisting");
            //}
            //else
            //{
            //    RiceOA mdb = new RiceOA();
            //    var mdid = db.MDBGeneralData.Find(mdbid);
            //    ViewBag.MDBID = mdid.MDBID;
            //    ViewBag.OrganizationName = mdid.OrganizationName;
            //    ViewBag.AddressLine1 = mdid.AddressLine1;
            //    ViewBag.AddressLine2 = mdid.AddressLine2;
            //    ViewBag.AddressLine3 = mdid.AddressLine3;
            //    ViewBag.City = mdid.City;
            //    ViewBag.Pincode = mdid.Pincode;
            //    ViewBag.State = mdid.State;
            //    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
            //    ViewBag.NullError = false;

            //    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
            //    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

            //    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
            //    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

            //    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            //    return View();
            //}
        }

        //
        // Get: /RiceOA/RiceOACheck for Admin
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOACheck(int oaid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (oaid != 0)
            {
                var tin = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.TinNumber).Single();
                ViewBag.TinNumber = tin;

                //for PAN Number
                var pannum = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.PANCardNo).Single();
                ViewBag.PANNumber = pannum;

                RiceOA quo = new RiceOA();
                var qgdb = db.RiceOAEquipGeneralData.Find(oaid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid).Where(m=>m.IsSOTStatus==1); //query to shortlist only byjt chances are 100%
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.RiceOAEquipGeneralData = qgdb;
                    quo.RiceOAEquipPayment = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid).Single();
                    if (modelcnt == 21)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = null;
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }

                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    try
                    {
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
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = "Please Enter Contact Details for the Above Customer...!";
                        return RedirectToAction("RiceOAApproval", "RiceOA", null);
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
                    ViewBag.ProductVariety = quo.RiceOAEquipGeneralData.ProductVariety;
                    ViewBag.Type = quo.RiceOAEquipGeneralData.TypeRice;
                    ViewBag.PaddySize = quo.RiceOAEquipGeneralData.PaddySize;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                RiceOA quo = new RiceOA();
                var qgdb1 = db.RiceOAEquipGeneralData.Where(m => m.OANumber == quotnumber).Select(m => m.ROAID).SingleOrDefault();
                var qgdb = db.RiceOAEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == qgdb1).Where(m => m.IsSOTStatus == 1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.RiceOAEquipGeneralData = qgdb;
                    quo.RiceOAEquipPayment = db.RiceOAEquipPayment.Find(qgdb1);
                    if (modelcnt == 21)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.RiceOAEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = modelcount[19];
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = modelcount[18];
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = modelcount[17];
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = modelcount[16];
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = modelcount[15];
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = modelcount[14];
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = modelcount[13];
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = modelcount[12];
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = modelcount[11];
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = modelcount[10];
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = modelcount[9];
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = modelcount[8];
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = modelcount[7];
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = modelcount[6];
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = modelcount[5];
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = modelcount[4];
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = modelcount[3];
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = modelcount[2];
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = modelcount[1];
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.RiceOAEquipTableData1 = modelcount[0];
                        quo.RiceOAEquipTableData2 = null;
                        quo.RiceOAEquipTableData3 = null;
                        quo.RiceOAEquipTableData4 = null;
                        quo.RiceOAEquipTableData5 = null;
                        quo.RiceOAEquipTableData6 = null;
                        quo.RiceOAEquipTableData7 = null;
                        quo.RiceOAEquipTableData8 = null;
                        quo.RiceOAEquipTableData9 = null;
                        quo.RiceOAEquipTableData10 = null;
                        quo.RiceOAEquipTableData11 = null;
                        quo.RiceOAEquipTableData12 = null;
                        quo.RiceOAEquipTableData13 = null;
                        quo.RiceOAEquipTableData14 = null;
                        quo.RiceOAEquipTableData15 = null;
                        quo.RiceOAEquipTableData16 = null;
                        quo.RiceOAEquipTableData17 = null;
                        quo.RiceOAEquipTableData18 = null;
                        quo.RiceOAEquipTableData19 = null;
                        quo.RiceOAEquipTableData20 = null;
                        quo.RiceOAEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
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
                    ViewBag.ProductVariety = quo.RiceOAEquipGeneralData.ProductVariety;
                    ViewBag.Type = quo.RiceOAEquipGeneralData.TypeRice;
                    ViewBag.PaddySize = quo.RiceOAEquipGeneralData.PaddySize;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        // Post: /RiceOA/RiceOACheck for Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOACheck(RiceOA maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.RiceOAEquipTableData1.ProductModelID != 0)
                {
                   // var Oagen = maindatabase.RiceOAEquipGeneralData;
                    //db.Entry(Oagen).State = EntityState.Modified;

                    var Oagen = maindatabase.RiceOAEquipGeneralData;
                    var oagenlist = db.RiceOAEquipGeneralData.Where(m => m.ROAID == Oagen.ROAID).SingleOrDefault();
                    oagenlist.SalesManager = maindatabase.RiceOAEquipGeneralData.SalesManager;
                    oagenlist.Business=maindatabase.RiceOAEquipGeneralData.Business;
                    oagenlist.BusinessArea=maindatabase.RiceOAEquipGeneralData.BusinessArea;
                    oagenlist.BusinessUnit=maindatabase.RiceOAEquipGeneralData.BusinessUnit;
                    oagenlist.BusinessUnitForSAP=maindatabase.RiceOAEquipGeneralData.BusinessUnitForSAP;
                    oagenlist.MarketSegment=maindatabase.RiceOAEquipGeneralData.MarketSegment;
                    oagenlist.CustomerSAPIdDelvryNo = maindatabase.RiceOAEquipGeneralData.CustomerSAPIdDelvryNo;
                    oagenlist.CustomerSAPIdNo=maindatabase.RiceOAEquipGeneralData.CustomerSAPIdNo;
                    oagenlist.IncoTerms=maindatabase.RiceOAEquipGeneralData.IncoTerms;
                    oagenlist.commitedDelivery=maindatabase.RiceOAEquipGeneralData.commitedDelivery;
                    oagenlist.PaymentTerms=maindatabase.RiceOAEquipGeneralData.PaymentTerms;
                    db.Entry(oagenlist).State = EntityState.Modified;
                    db.SaveChanges();

                    ////to Check the Exact Errors from table and its fields
                    //try
                    //{
                    //    db.SaveChanges();
                    //}
                    //catch (DbEntityValidationException dbEx)
                    //{
                    //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //    {
                    //        foreach (var validationError in validationErrors.ValidationErrors)
                    //        {
                    //            Debug.WriteLine("Property: {0} Error: {1}",
                    //                       validationError.PropertyName, validationError.ErrorMessage);
                    //        }
                    //    }
                    //}

                    var Oapay = maindatabase.RiceOAEquipPayment;
                    var OapayList = db.RiceOAEquipGeneralData.Where(m => m.ROAID == Oapay.ROAPID).SingleOrDefault();
                    Oapay.annexure = maindatabase.RiceOAEquipPayment.annexure;
                    db.Entry(Oapay).State = EntityState.Modified;
                    db.SaveChanges();

                    var Oatab1 = maindatabase.RiceOAEquipTableData1;
                    db.Entry(Oatab1).State = EntityState.Modified;
                    db.SaveChanges();
                    if (maindatabase.RiceOAEquipTableData2.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData2).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData3.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData3).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData4.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData4).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData5.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData5).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData6.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData6).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData7.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData7).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData8.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData8).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData9.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData9).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData10.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData10).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData11.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData11).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData12.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData12).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData13.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData13).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData14.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData14).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData15.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData15).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData16.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData16).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData17.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData17).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData18.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData18).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (maindatabase.RiceOAEquipTableData19.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData19).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData20.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData20).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (maindatabase.RiceOAEquipTableData21.ProductModelID != 0)
                    {
                        db.Entry(maindatabase.RiceOAEquipTableData21).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //Removing Quotation from OA View after it is sent for Approval
                    int qgid = maindatabase.RiceOAEquipGeneralData.RQGID;
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == qgid).ToList();
                    foreach (var qg in quogen)
                    {
                        qg.Ordergenerated = 1;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //var oaappst = maindatabase.RiceOAEquipGeneralData;
                    var oaappst = db.RiceOAEquipGeneralData.Where(m => m.ROAID == Oagen.ROAID).SingleOrDefault();
                    oaappst.OAStatus = 1;
                    DateTime quotdat = System.DateTime.Now;
                    oaappst.Approvaldate = quotdat.ToString("yyyy-MM-dd");
                    db.Entry(oaappst).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Request.Form["btnapproval"] != null)
                    {
                        //var oagen = maindatabase.RiceOAEquipGeneralData;
                        var oagendetails = db.RiceOAEquipGeneralData.Where(m => m.ROAID == Oagen.ROAID).SingleOrDefault();
                        oagendetails.ApprovalStatus = 1;
                        db.Entry(oagendetails).State = EntityState.Modified;
                        db.SaveChanges();
                        int oaid1 = oagendetails.ROAID;
                        return RedirectToAction("RiceOAApproveComment", "RiceOA", new { oaid = oaid1 });

                        //return Content(updateparent);
                    }
                    else if (Request.Form["btnrejected"] != null)
                    {
                        //var oagen = maindatabase.RiceOAEquipGeneralData;
                        var oagendetails = db.RiceOAEquipGeneralData.Where(m => m.ROAID == Oagen.ROAID).SingleOrDefault();
                        oagendetails.ApprovalStatus = 2;
                        db.Entry(oagendetails).State = EntityState.Modified;
                        db.SaveChanges();
                        int oaid1 = oagendetails.ROAID;
                        return RedirectToAction("RiceOARejectComment", "RiceOA", new { oaid = oaid1 });
                        //TempData["oarejected"] = "Order Acknowledgement Rejected Successfully!!!!";
                    }
                }
                return RedirectToAction("RiceOAApproval");
            }
            else
            {
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.OAID = maindatabase.RiceOAEquipGeneralData.ROAID;
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
                ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                ViewData["ProductID1"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID2"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID3"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID4"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID5"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID6"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID7"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID8"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID9"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID10"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID11"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID12"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID13"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID14"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID15"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID16"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID17"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID18"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID19"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID20"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID21"] = new SelectList(db.Products.Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        //
        //Report Generation
        public ActionResult RiceOAReportGeneration(int? oaid)
        {
            if (oaid != 0)
            {
                RiceOA quo = new RiceOA();
                RRepModel RM = new RRepModel();
                var qgdb = db.RiceOAEquipGeneralData.Find(oaid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    String oanum = qgdb.OANumber;
                    var cpid = qgdb.CPID;
                    var Logo = db.ChannelPartners.Find(cpid);
                    var channelPartnerLogo = "";
                    var Logopath = "";
                    var add1 = "Check Your Address";

                    channelPartnerLogo = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                    add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";
                    // Logopath = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    // add1 = Logo.footaddress;

                    ///var apvl = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.ApprovalStatus).Single();
                    //ViewBag.Apstat = apvl;

                    //Logopath = Server.MapPath("~/App_Data/Buhler_RGB_100.png");
                    //add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";
                    var models = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid).Where(m=>m.IsSOTStatus==1);
                    var modelcount = models.ToList();
                    quo.RiceOAEquipGeneralData = qgdb;
                    var paymentterms = db.RiceOAEquipPayment.Where(m => m.ROAID == oaid).First();
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    RM.Logo = Logopath;
                    RM.footaddress = add1;
                    RM.RQGID = qgdb.RQGID;
                    RM.Tinnumber = qgdb.TinNumber;
                    RM.Approvaldate = qgdb.Approvaldate;
                    RM.annexure = paymentterms.annexure;
                    RM.CPQuotationNumber = qgdb.CPQuotationNumber;
                    RM.MDBID = mdbdet.MDBID.ToString();
                    RM.QuotationNumber = qgdb.QuotationNumber;

                    if (RM.CPQuotationNumber != null)
                    {
                        //RM.QuotationNumber = RM.CPQuotationNumber;
                        ViewData["qut"] = RM.CPQuotationNumber;
                    }
                    else
                    {
                        ViewData["qut"] = qgdb.QuotationNumber;
                    }

                    RM.OANmber = qgdb.OANumber;
                    RM.PaddySize = qgdb.PaddySize;
                    RM.Type = qgdb.TypeRice;
                    RM.ProductVariety = qgdb.ProductVariety;
                    RM.QuotationDate = Convert.ToDateTime(qgdb.OADate).ToString("dd-MM-yyyy");
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
                    ViewBag.IsFive = false;
                    ViewBag.IsSix = false;
                    ViewBag.IsSeven = false;
                    ViewBag.IsEight = false;
                    ViewBag.IsNine = false;
                    ViewBag.IsTen = false;
                    ViewBag.IsEleven = false;
                    ViewBag.IsTwelve = false;
                    ViewBag.IsThirteen = false;
                    ViewBag.IsFourteen = false;
                    ViewBag.IsFifteen = false;
                    ViewBag.IsSixteen = false;
                    ViewBag.IsSeventeen = false;
                    ViewBag.IsEighteen = false;
                    ViewBag.IsNinteen = false;
                    ViewBag.IsTwenty = false;
                    ViewBag.IsTwentyone = false;

                    RM.RiceOAEquipTableData = modelcount;
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
                    if (modelcount.Count == 9)
                    {
                        ViewBag.IsNine = true;
                    }
                    else if (modelcount.Count == 10)
                    {
                        ViewBag.IsTen = true;
                    }
                    else if (modelcount.Count == 11)
                    {
                        ViewBag.IsEleven = true;
                    }
                    else if (modelcount.Count == 12)
                    {
                        ViewBag.IsTweleve = true;
                    }
                    else if (modelcount.Count == 13)
                    {
                        ViewBag.IsThirteen = true;
                    }
                    else if (modelcount.Count == 14)
                    {
                        ViewBag.IsFourteen = true;
                    }
                    else if (modelcount.Count == 15)
                    {
                        ViewBag.Isfifteen = true;
                    }
                    else if (modelcount.Count == 16)
                    {
                        ViewBag.IsSixteen = true;
                    }
                    else if (modelcount.Count == 17)
                    {
                        ViewBag.IsSeventeen = true;
                    }
                    else if (modelcount.Count == 18)
                    {
                        ViewBag.IsEighteen = true;
                    }
                    else if (modelcount.Count == 19)
                    {
                        ViewBag.IsNinteen = true;
                    }
                    else if (modelcount.Count == 20)
                    {
                        ViewBag.IsTwenty = true;
                    }
                    else if (modelcount.Count == 21)
                    {
                        ViewBag.IsTwentyOne = true;
                    }
                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.CPContactPersonData.Where(m => m.CPID == cpid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                    RM.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                    RM.Designation = loginname.Designation;
                    var FileName = RM.OANmber;
                    ViewBag.CPID = loginname.CPID;
                    var relativePath = "RiceOAReportGeneration";
                    string content;
                    var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                    ViewData.Model = RM;

                    String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                    //String footerpath = Server.MapPath("~/App_Data/FooterFile.html");
                    string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
                        //"<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 120px\"><div style=\"width: 700px; float: left; height: 120px\"></div><div style=\"width: 300px; height: 120px; float: right\"><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></div></div></body>";
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
                        var pdfconverter = new PDFConverter3();
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
        //RiceOA Rejection comment
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOARejectComment(int oaid)
        {
            var oa = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Single();
            ViewBag.OAID = oaid;
            return View(oa);
        }

        //
        //RiceOA Rejection Comment
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOARejectComment(RiceOAEquipGeneralData oagendata)
        {
            oagendata.ApprovalStatus = 2;
            db.Entry(oagendata).State = EntityState.Modified;
            db.SaveChanges();
            TempData["oarejected"] = "Order Acknowledgement Rejected Successfully!!!!";
            return RedirectToAction("RiceOAApproval");
        }

        //
        //RiceOA Approval comment
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOAApproveComment(int oaid)
        {
            var oa = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Single();
            ViewBag.OAID = oaid;
            return View(oa);
        }

        //
        //RiceOA Approval Comment
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOAApproveComment(RiceOAEquipGeneralData oagendata)
        {
            oagendata.ApprovalStatus = 1;
            db.Entry(oagendata).State = EntityState.Modified;
            db.SaveChanges();

            TempData["oarejected"] = "Order Acknowledgement Approved Successfully!!!!";
            double count = 1;
            var check = db.RiceOAEquipTableData.Where(m => m.ROAID == oagendata.ROAID).ToList();
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
            return RedirectToAction("RiceOAApproval");
        }

        //OA Approval Comment
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

        //ALL Orders View
        [Authorize(Roles = "Administrator")]
        public ActionResult RiceOAAllView(string cpnam = null, int status = -1, string frommonth = null, string tomonth = null, string equip = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;

            var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
            ViewBag.EquipModel = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
            ViewBag.equip = equip;
            ViewBag.fromdte = frommonth;
            ViewBag.todate = tomonth;

            var oagendata = db.RiceOAEquipGeneralData.Where(m => m.OAStatus == 1).OrderByDescending(m => m.ROAID).ToList();
            if (cpnam != null && cpnam != "")
            {
                ViewBag.IsSearch = true;
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OAStatus == 1).OrderByDescending(m => m.ROAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
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
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.ROAID).ToList();

                ViewBag.status = status;
                TempData["byjt"] = "Order Acknowledgement for selected status is";
            }
            if (equip != null && equip != "")
            {
                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                ViewBag.status = status;
                //return View(model);
            }
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                ViewBag.status = status;
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                }
            }
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && status != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                int stats = Convert.ToInt32(status);
                ViewBag.status = status;
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.OADate >= frmo && m.OADate <= tomo).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                }
            }
            if (cpnam != null && cpnam != "" && status != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                ViewBag.status = status;
                int stats = Convert.ToInt32(status);
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                }

            }
            if (cpnam != null && cpnam != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                ViewBag.status = status;
                //int stats = Convert.ToInt32(status);
                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.CPID == channame
                             where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();

                //var sotlist = db.OAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();
                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                    //return View(sotlist);
                }
            }
            if (equip != null && equip != "" && status != -1)
            {
                ViewBag.status = status;
                int stats = Convert.ToInt32(status);
                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.ApprovalStatus == stats
                             where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();

                //var sotlist = db.OAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();
                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                }
                //return View(sotlist);
            }
            if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                ViewBag.status = status;
                int stats = Convert.ToInt32(status);
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OADate).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                }
                //return View(sotlist);
            }
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                int stats = Convert.ToInt32(status);
                ViewBag.status = status;
                //oagendata = db.OAEquipGeneralData.Where(m => m.OADate >= frmo && m.OADate <= tomo).Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.OADate).ToList();
                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
                }
            }
            if (cpnam != null && cpnam != "" && status != -1 && equip != null && equip != "")
            {
                ViewBag.IsSearch = true;
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                int stats = Convert.ToInt32(status);

                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.ApprovalStatus == stats
                             where s.CPID == channame
                             where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                //oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).OrderByDescending(m => m.OAID).ToList();
                if (oagendata == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
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
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                //ViewBag.IsSearch = true;
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                int stats = Convert.ToInt32(status);

                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.ApprovalStatus == stats
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                //oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).OrderByDescending(m => m.OAID).ToList();
                if (oagendata == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
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
            if (cpnam != null && cpnam != "" && status != -1 && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {

                ViewBag.IsSearch = true;
                int stats = Convert.ToInt32(status);
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.ROAID == b.ROAID
                             where s.ApprovalStatus == stats
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.CPID == channame && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.ROAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
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
            if (cpnam != null && cpnam != "" && status != -1 && frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && equip != null && equip != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                ViewBag.IsSearch = true;

                int stats = Convert.ToInt32(status);
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                oagendata = (from s in db.RiceOAEquipGeneralData
                             from b in db.RiceOAEquipTableData
                             where s.ROAID == b.ROAID
                             where s.ApprovalStatus == stats
                             where s.OADate >= frmo && s.OADate <= tomo
                             where s.CPID == channame && b.ProductModel.ProductModelName == equip
                             select s).ToList();
                //oagendata = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPID == channame).Where(m => m.OADate >= frmo && m.OADate <= tomo).OrderByDescending(m => m.OAID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("RiceOAAllView", "RiceOA");
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

                    var duplicate = (from s in db.RiceOAEquipGeneralData
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
                    var oagendata = db.RiceOAEquipGeneralData.Where(m => m.ApprovalStatus == stats).OrderByDescending(m => m.ROAID).ToList();

                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     where s.ApprovalStatus == status
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
                    var oagendata = db.RiceOAEquipGeneralData.Where(m => m.ApprovalStatus == stats).Where(m => m.CPName == cpnam).OrderByDescending(m => m.ROAID).ToList();

                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     where (s.CPID == channame)
                                     where s.ApprovalStatus == status
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

                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     from b in db.RiceOAEquipTableData
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     from b in db.RiceOAEquipTableData
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     from b in db.RiceOAEquipTableData
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     where s.OADate >= frmo && s.OADate <= tomo
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

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var chpart = db.ChannelPartners.Where(m => m.CPName == cpnam).SingleOrDefault();
                    var cname = chpart.CPName;
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     where s.ApprovalStatus == status
                                     where s.OADate >= frmo && s.OADate <= tomo
                                     select new
                                     {
                                         s.OANumber,
                                         s.Subjectinfo,
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     from b in db.RiceOAEquipTableData
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     where (s.CPID == channame)
                                     where s.OADate >= frmo && s.OADate <= tomo
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     where s.ApprovalStatus == status
                                     from b in db.RiceOAEquipTableData
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     from b in db.RiceOAEquipTableData
                                     where s.ApprovalStatus == status
                                     where s.OADate >= frmo && s.OADate <= tomo
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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
                    var duplicate = (from s in db.RiceOAEquipGeneralData
                                     from b in db.RiceOAEquipTableData
                                     where s.OADate >= frmo && s.OADate <= tomo
                                     where s.ApprovalStatus == status
                                     where s.CPID == channame
                                     where s.ROAID == b.ROAID && b.ProductModel.ProductModelName == equip
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

                var duplicate = (from s in db.RiceOAEquipGeneralData
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
                        dr[6] = d.ApprovalStatus;
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

            return RedirectToAction("RiceOAAllView");
        }

        //ChannelPartner Pending/ approved/ Rejected Report
        public ActionResult RiceOAChannelStatus(int status = -1)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var oagendata = db.RiceOAEquipGeneralData.Where(m => m.CPID == loginname.CPID).Where(m => m.OAStatus == 1).OrderByDescending(m => m.ROAID).ToList();
            if (status != -1)
            {
                oagendata = db.RiceOAEquipGeneralData.Where(m => m.CPID == loginname.CPID).Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == status).OrderByDescending(m => m.ROAID).ToList();
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
        const int pageSizem = 25000;
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

        [HttpPost]
        public void updateRiceOAstatus(string id, string maildetails, string date)
        {
            string OAID = id;
            int sid = 1;
            if (OAID != "" || OAID != null)
            {
                RiceOAEquipGeneralData RiceOA = db.RiceOAEquipGeneralData.Find(Convert.ToInt32(OAID));
                RiceOA.IsDispatch = sid;
                RiceOA.MailDetails = maildetails;
                RiceOA.MailDate = date;
                db.Entry(RiceOA).State = EntityState.Modified;
                db.SaveChanges();
                sendmail(OAID);
            }
        }

        [HttpPost]
        public void EditRiceOAstatus(string id, string maildetails, string date)
        {
            string OAID = id;

            if (OAID != "" || OAID != null)
            {
                RiceOAEquipGeneralData RiceOA = db.RiceOAEquipGeneralData.Find(Convert.ToInt32(OAID));
                RiceOA.MailDetails = maildetails;
                RiceOA.MailDate = date;
                db.Entry(RiceOA).State = EntityState.Modified;
                db.SaveChanges();
                sendmail(OAID);
            }
        }


        [HttpPost]
        public string OAdataAutoFill(string id)
        {
            int OAID = Convert.ToInt32(id);
            string date = "";
            string details = "";
            string data = "";
            if (OAID != 0 || OAID != null)
            {
                date = db.RiceOAEquipGeneralData.Where(m => m.ROAID == OAID).Select(m => m.MailDate).SingleOrDefault();
                details = db.RiceOAEquipGeneralData.Where(m => m.ROAID == OAID).Select(m => m.MailDetails).SingleOrDefault();
                data = date + '%' + details;
            }
            return data;
        }

        public void sendmail(string OAID)
        {

                        int oaid =Convert.ToInt32( OAID);
                        var cpd = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.CPID).Single();
                        int cpid = Convert.ToInt32(cpd);
                        var OANumber = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.OANumber).Single();
                        var OrganizationName = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m=>m.MDBGeneralData.OrganizationName).Single();
                        var Subject =  db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.Subjectinfo).Single();
                        var CPQno =  db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.CPQuotationNumber).Single();
                        var disdte =db.RiceOAEquipGeneralData.Where(m=>m.ROAID==oaid).Select(m=>m.MailDate).Single();//dispatchdate
                        var dispatchdate = Convert.ToDateTime(disdte).ToString("dd-MM-yyyy");
                        var rmk = db.RiceOAEquipGeneralData.Where(m => m.ROAID == oaid).Select(m => m.MailDetails).Single();
                        var machinename = "";
                        var machine = db.RiceOAEquipTableData.Where(m => m.ROAID == oaid).Select(m => m.ProductModelID).Distinct().ToList();
                        foreach(var ma in machine)
                        {
                            var proname = db.ProductModel.Where(m => m.ProductModelID == ma).Select(m => m.ProductModelName).SingleOrDefault();

                            machinename = machinename + proname + "<br />";
                        }

                        string frommail = null;
                        string mailpass = null;
                        //to fetch FROM mail credentials
                        var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
                        if (from != null)
                        {
                            frommail = from.FromMail;
                            mailpass = from.Password;
                        }
                        try
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                            mail.From = new MailAddress(frommail);
                            //mail.From = new MailAddress("bbandispatches@gmail.com");
                            //mail.From = new MailAddress("janardhangrd@gmail.com");

                            string name = db.ChannelPartners.Where(s => s.CPID == cpid).Select(s => s.CPName).FirstOrDefault();
                            var email = db.CPMailIdTO.Where(s => s.CPID == cpid).Select(s => s.EmailId).FirstOrDefault();
                           // mail.To.Add(email); ----

                            mail.To.Add("bharath.naik@buhlergroup.com");
                            mail.To.Add("praveen.shivanna@buhlergroup.com");
                            mail.CC.Add("sharath.krishna@srkssolutions.com");
                            mail.CC.Add("suhas.cm@srkssolutions.com");
                            mail.Subject = "Rice Mill Machine Dispatch Details";

                            // mail.CC.Add("srinivas.kulkarni@buhlergroup.com");
                            //for CC get spareadmin emails
                            var spareAdminEmailToCC = db.BuhlerAdminCC.ToList();
                            foreach (var mc in spareAdminEmailToCC)
                            {
                                string spareadminemail = mc.EmailId;
                              //  mail.CC.Add(new MailAddress(spareadminemail));
                            }

                            mail.Body = "<head>" +
                                    "<style type=\"text/css\">" +
                                        "{border-style: none;" +
                                        "border-color: inherit;" +
                                        "border-width: medium;" +
                                        "padding-top:1px;" +
                                        "padding-right:1px;" +
                                        "padding-left:1px;" +
                                        "color:black;" +
                                        "font-size:10.0pt;" +
                                        "font-weight:400;" +
                                        "font-style:normal;" +
                                        "text-decoration:none;" +
                                        "font-family:Arial, sans-serif;" +
                                        "text-align:general;" +
                                        "vertical-align:bottom;" +
                                        "white-space:nowrap;" +
                                        "}" +
                                    ".auto-style1 {" +
                                        "border-collapse: collapse;" +
                                        "border: 1px solid #000000;" +
                                    "}" +
                                    ".auto-style2 {" +
                                        "text-align: center;" +
                                        "font-size: x-large;" +
                                        "vertical-align: middle;" +
                                        "border-style: solid;" +
                                        "border-width: 1px;" +
                                    "}" +
                                    ".auto-style5 {" +
                                        "vertical-align: middle;" +
                                        "text-align: left;" +
                                        "border-style: solid;" +
                                        "border-width: 1px;" +
                                        "padding: 5px;"+
                                    "}" +
                                    ".auto-style6 {" +
                                        "vertical-align: middle;" +
                                        "text-align: left;" +
                                        "border-style: solid;" +
                                        "border-width: 1px;" +
                                        "background-color: #59C9FF;" +
                                        "padding: 5px;" +
                                    "}" +
                                    ".auto-style7 {" +
                                        "vertical-align: middle;" +
                                        "text-align: center;" +
                                        "font-family: Arial;" +
                                        "border-style: solid;" +
                                        "border-width: 1px;" +
                                        "background-color: #59C9FF;" +
                                    "}" +
                                    ".auto-style8 {" +
                                        "text-align: center;" +
                                        "font-size: small;" +
                                    "}" +
                                    ".auto-style9 {" +
                                        "color: #FF0000;" +
                                    "}" +
                                    "</style>" +
                                    "</head>" +
                                    "<body>" +
                                    "<p>Dear " + name + ",</p>" +
                                    "<p>Please find the below details for Rice Mill Machine Dispatch Details</p>" +
                                    "<p>&nbsp;</p>" +
                                    "<table class=\"auto-style1\" style=\"width: 100%; height: 30px\">" +
                                        "<tr>" +
                                            "<td class=\"auto-style2\" colspan=\"10\">Rice Mill Machine Dispatch Details</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class=\"auto-style6\">Order Acknowledgement No</td>" +
                                            "<td class=\"auto-style6\">Customer Name</td>" +
                                            "<td class=\"auto-style6\">Machine Details</td>" +
                                            "<td class=\"auto-style6\">Dispatch Date<br /></td>" +
                                            "<td class=\"auto-style6\">Details	</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + OANumber + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + OrganizationName + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + machinename + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + dispatchdate + "</td>" +
                                           "<td class=\"auto-style5\" style=\"height: 30px\">" + rmk + "</td>" +
                                        "</tr>" +
                                    "</table>" +
                                    "<p>&nbsp;</p>" +
                                    "<p class=\"auto-style8\">Note :<span class=\"auto-style9\"> Please donot reply to this " +
                                    "Email ID, this is a Autogenerated Email</span></p>" +
                                    "</body>";
                            //mail.To.Add("suhas.cm@srkssolutions.com");
                            mail.IsBodyHtml = true;
                            SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
                            //SmtpServer.Credentials = new System.Net.NetworkCredential("bbandispatches@gmail.com", "bban2014");
                            //SmtpServer.Credentials = new System.Net.NetworkCredential("janardhangrd@gmail.com", "j2n26dh2n");
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.ToString());
                        }
        }

        //TA Sheets View
        public ActionResult TASheets(int roaid = 0)
        {
            int page = 1; int sortBy = 1; bool isAsc = true; string cunam = null;

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<RiceOAEquipTableData> quotations = db.RiceOAEquipTableData.Where(p => roaid == 0 || p.RiceOAEquipGeneralData.ROAID == roaid);
            int a = quotations.Count();
            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.ROATBID) : quotations.OrderByDescending(p => p.ROATBID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.RiceOAEquipGeneralData.OANumber) : quotations.OrderByDescending(p => p.RiceOAEquipGeneralData.OANumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.RiceOAEquipGeneralData.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.RiceOAEquipGeneralData.MDBGeneralData.OrganizationName);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.RiceOAEquipGeneralData.Subjectinfo) : quotations.OrderByDescending(p => p.RiceOAEquipGeneralData.Subjectinfo);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.RiceOAEquipGeneralData.CPQuotationNumber) : quotations.OrderByDescending(p => p.RiceOAEquipGeneralData.CPQuotationNumber);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.RiceOAEquipGeneralData.OADate) : quotations.OrderByDescending(p => p.RiceOAEquipGeneralData.OADate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.ROATBID) : quotations.OrderByDescending(p => p.ROATBID);
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
            return View(quotations.OrderByDescending(m => m.RiceOAEquipGeneralData.OADate).ToList());
        }

        #region Polisher Dropdown Data.

        public JsonResult GetListGTPolisher(string Section)
        {

            List<SelectListItem> listItemsGTPolisher = new List<SelectListItem>();
            listItemsGTPolisher.Add(new SelectListItem
            {
                Text = "Long/Medium",
                Value = "Long/Medium"
            });
            var data = listItemsGTPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListProcessPolisher(string Section)
        {
            List<SelectListItem> listItemsProcessPolisher = new List<SelectListItem>();
            listItemsProcessPolisher.Add(new SelectListItem
            {
                Text = "Raw,Steam/Parboiled",
                Value = "Raw,Steam/Parboiled",
            });
            var data = listItemsProcessPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListPassPolisher(string Section)
        {
            List<SelectListItem> listItemsPassPolisher = new List<SelectListItem>();
            listItemsPassPolisher.Add(new SelectListItem
            {
                Text = "1/2",
                Value = "1/2"
            });
            var data = listItemsPassPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCapacityPolisher(string Section)
        {
            List<SelectListItem> listItemscapacityPolisher = new List<SelectListItem>();
            listItemscapacityPolisher.Add(new SelectListItem
            {
                Text = "3",
                Value = "3"
            });
            listItemscapacityPolisher.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            var data = listItemscapacityPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Whitner Dropdown Data.

        public JsonResult GetListGTWhitner(string Section)
        {

            List<SelectListItem> listItemsGTWhitner = new List<SelectListItem>();
            listItemsGTWhitner.Add(new SelectListItem
            {
                Text = "Medium",
                Value = "Medium",
            });
            listItemsGTWhitner.Add(new SelectListItem
            {
                Text = "Long",
                Value = "Long",
            });
            var data = listItemsGTWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListProcessWhitner(string Section)
        {
            List<SelectListItem> listItemsProcessWhitner = new List<SelectListItem>();
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Raw",
                Value = "Raw"
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Steam",
                Value = "Steam",
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Parboiled",
                Value = "Parboiled",
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Raw,Steam/Parboiled",
                Value = "Raw,Steam/Parboiled",
            });
            var data = listItemsProcessWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListPassWhitner(string Section)
        {
            List<SelectListItem> listItemsPassWhitner = new List<SelectListItem>();
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "1",
                Value = "1"
            });
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "2",
                Value = "2",
            });
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "3",
                Value = "3",
            });
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });

            var data = listItemsPassWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCapacityWhitner(string Section)
        {
            List<SelectListItem> listItemscapacityWhitner = new List<SelectListItem>();
            listItemscapacityWhitner.Add(new SelectListItem
            {
                Text = "3",
                Value = "3"
            });
            listItemscapacityWhitner.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            listItemscapacityWhitner.Add(new SelectListItem
            {
                Text = "3/4",
                Value = "3/4",
            });
            var data = listItemscapacityWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Others Dropdown Data.

        public JsonResult GetListGTOthers(string Section)
        {

            List<SelectListItem> listItemsGTOthers = new List<SelectListItem>();
            listItemsGTOthers.Add(new SelectListItem
            {
                Text = "Medium",
                Value = "Medium",
            });
            listItemsGTOthers.Add(new SelectListItem
            {
                Text = "Long",
                Value = "Long",
            });
            listItemsGTOthers.Add(new SelectListItem
            {
                Text = "Long/Medium",
                Value = "Long/Medium"
            });
            var data = listItemsGTOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListProcessOthers(string Section)
        {
            List<SelectListItem> listItemsProcessOthers = new List<SelectListItem>();
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Raw",
                Value = "Raw"
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Steam",
                Value = "Steam",
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Parboiled",
                Value = "Parboiled",
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Raw/Steam",
                Value = "Raw/Steam",
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Raw,Steam/Parboiled",
                Value = "Raw,Steam/Parboiled",
            });
            var data = listItemsProcessOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListPassOthers(string Section)
        {
            List<SelectListItem> listItemsPassOthers = new List<SelectListItem>();
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "1",
                Value = "1"
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "2",
                Value = "2",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "3",
                Value = "3",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "1/2",
                Value = "1/2",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "2/3",
                Value = "2/3",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "3/4",
                Value = "3/4",
            });

            var data = listItemsPassOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCapacityOthers(string Section)
        {
            List<SelectListItem> listItemscapacityOthers = new List<SelectListItem>();
            listItemscapacityOthers.Add(new SelectListItem
            {
                Text = "3",
                Value = "3"
            });
            listItemscapacityOthers.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            listItemscapacityOthers.Add(new SelectListItem
            {
                Text = "3/4",
                Value = "3/4",
            });
            var data = listItemscapacityOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
    //  Pdf
    public interface IPDFConverter3
    {
        byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml);
    }

    public class PDFConverter3 : IPDFConverter3
    {
        private const string HtmlToPdfExePath = "wkhtmltopdf.exe";
        private readonly ILog log = LogManager.GetLogger(typeof(PDFConverter3));

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
