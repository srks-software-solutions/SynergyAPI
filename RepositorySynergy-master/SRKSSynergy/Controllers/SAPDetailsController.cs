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

namespace SRKSSynergy.Controllers
{
    public class SAPDetailsController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        const int pageSize1p = 10000000;
        bool getdetailsclick1p = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult SAPOA(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
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

        [HttpGet]
        public ActionResult SAPRiceOA(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
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
            return View(quotations.OrderBy(m => m.ApprovalStatus).ToList());
        }

        //OA
        [HttpGet]
        public ActionResult SAPDetailsData(int oaid)
        {
            OAEquipGeneralData list = db.OAEquipGeneralData.Find(oaid);
            return View(list);
        }

        [HttpPost]
        public ActionResult SAPDetailsData(OAEquipGeneralData oaeg, string sapno, string sapdate)
        {
            int qgid = oaeg.QGID;

            int isrice = db.QGEquipGeneralData.Where(m => m.QGID == qgid).Select(m => m.IsRiceMill).SingleOrDefault();
            int dupcheck = db.OASAPDetails.Where(m => m.OrderID == oaeg.OAID).Count();
            if (dupcheck == 0)
            {
                OASAPDetails oasap = new OASAPDetails();

                oasap.OrderID = oaeg.OAID;
                oasap.CustID = oaeg.CompanyUniqueID;
                oasap.CPID = oaeg.CPID;
                oasap.IsRice = isrice;
                oasap.OADate = oaeg.OADate;
                oasap.SAPNO = sapno;
                oasap.SAPDate = Convert.ToDateTime(sapdate);
                oasap.CreatedOn = System.DateTime.Now;
                oasap.IsDeleted = 0;
                db.OASAPDetails.Add(oasap);
                db.SaveChanges();
            }
            else
            {
                int sapid = db.OASAPDetails.Where(m => m.OrderID == oaeg.OAID).Select(m => m.SAPID).SingleOrDefault();
                OASAPDetails oasap = db.OASAPDetails.Find(sapid);
                oasap.SAPNO = sapno;
                oasap.SAPDate = Convert.ToDateTime(sapdate);
                db.Entry(oasap).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("SAPOA", "SAPDetails", null);
        }

        [HttpGet]
        public ActionResult SAPDetailsDataEdit(int oaid)
        {
            int sapid = db.OASAPDetails.Where(m => m.OrderID == oaid).Select(m => m.SAPID).SingleOrDefault();
            OASAPDetails list = db.OASAPDetails.Find(sapid);

            DateTime sapdate = Convert.ToDateTime(list.SAPDate);
            string sapdate1 = sapdate.ToString("dd-MM-yyyy");
            ViewBag.sapdate = sapdate1;

            DateTime dispatchdate = Convert.ToDateTime(list.DispatchedDate);
            string dispatchdate1 = dispatchdate.ToString("dd-MM-yyyy");
            ViewBag.dispatchdate = dispatchdate1;

            return View(list);
        }

        [HttpPost]
        public ActionResult SAPDetailsDataEdit(OASAPDetails oasap)
        {
            oasap.ModifiedOn = System.DateTime.Now;
            db.Entry(oasap).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("SAPOA", "SAPDetails", null);
        }

        //Rice OA
        [HttpGet]
        public ActionResult SAPDetailsDataRiceOA(int oaid)
        {
            RiceOAEquipGeneralData list = db.RiceOAEquipGeneralData.Find(oaid);
            return View(list);
        }

        [HttpPost]
        public ActionResult SAPDetailsDataRiceOA(RiceOAEquipGeneralData oaeg, string sapno, string sapdate)
        {
            int qgid = oaeg.RQGID;

            int isrice = db.QGEquipGeneralData.Where(m => m.QGID == qgid).Select(m => m.IsRiceMill).SingleOrDefault();
            int dupcheck = db.OASAPDetails.Where(m => m.OrderID == oaeg.ROAID).Count();
            if (dupcheck == 0)
            {
                OASAPDetails oasap = new OASAPDetails();

                oasap.OrderID = oaeg.ROAID;
                oasap.CustID = oaeg.CompanyUniqueID;
                oasap.CPID = oaeg.CPID;
                oasap.IsRice = isrice;
                oasap.OADate = oaeg.OADate;
                oasap.SAPNO = sapno;
                oasap.SAPDate = Convert.ToDateTime(sapdate);
                oasap.CreatedOn = System.DateTime.Now;
                oasap.IsDeleted = 0;
                db.OASAPDetails.Add(oasap);
                db.SaveChanges();
            }
            else
            {
                int sapid = db.OASAPDetails.Where(m => m.OrderID == oaeg.ROAID).Select(m => m.SAPID).SingleOrDefault();
                OASAPDetails oasap = db.OASAPDetails.Find(sapid);
                oasap.SAPNO = sapno;
                oasap.SAPDate = Convert.ToDateTime(sapdate);
                db.Entry(oasap).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("SAPRiceOA", "SAPDetails", null);
        }

        [HttpGet]
        public ActionResult SAPDetailsDataEditRiceOA(int oaid)
        {
            int sapid = db.OASAPDetails.Where(m => m.OrderID == oaid).Select(m => m.SAPID).SingleOrDefault();
            OASAPDetails list = db.OASAPDetails.Find(sapid);

            DateTime sapdate = Convert.ToDateTime(list.SAPDate);
            string sapdate1 = sapdate.ToString("dd-MM-yyyy");
            ViewBag.sapdate = sapdate1;

            DateTime dispatchdate = Convert.ToDateTime(list.DispatchedDate);
            string dispatchdate1 = dispatchdate.ToString("dd-MM-yyyy");
            ViewBag.dispatchdate = dispatchdate1;

            return View(list);
        }

        [HttpPost]
        public ActionResult SAPDetailsDataEditRiceOA(OASAPDetails oasap)
        {
            oasap.ModifiedOn = System.DateTime.Now;
            db.Entry(oasap).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("SAPRiceOA", "SAPDetails", null);
        }

    }
}
