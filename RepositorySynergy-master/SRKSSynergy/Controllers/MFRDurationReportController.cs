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
    public class MFRDurationReportController : Controller
    {
        //
        // GET: /MFRDuration/
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete ChannelPartner Name
        public JsonResult Autocomplete(string term)
        {
            var channame = db.ChannelPartners.Where(m => m.CPName == term).Select(m => m.CPID).SingleOrDefault();

            var result = (from r in db.ChannelPartners
                          where r.CPName.ToLower().Contains(term.ToLower())
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete Customer Name
        public JsonResult Autocompletecus(string term)
        {
            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower())
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult MFRDuration(int page = 1, int rowsPerPage = 20, string cpnam = null, string cusnam = null, int ProductModelSparesID = 0, int dispst = -1, string fm = null, string tm = null)
        {


            int fm1 = 0, tm1 = 0;
            int fy1 = 0, ty1 = 0;
            var spmfr = db.OutwardMFR.OrderByDescending(m => m.MFRId).ToList();
            //Data Required for Export to excel
            ViewBag.cpnam = cpnam;
            ViewBag.cusnam = cusnam;
            ViewBag.prdid = ProductModelSparesID;
            ViewBag.dispt = dispst;
            ViewBag.fm = fm;
            ViewBag.tm = tm;

            //Month Name to Month Number
            #region
            if (fm != "" && fm != null && tm != "" && tm != null)
            {
                var fmmon = fm.Substring(0, 3);
                //getting month number from month name
                fm1 = DateTime.ParseExact(fmmon, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                string[] arr1 = fm.Split(' ');
                fy1 = Convert.ToInt32(arr1[1]);

                var tmmon = tm.Substring(0, 3);
                //getting month number from month name
                string[] arr2 = tm.Split(' ');
                ty1 = Convert.ToInt32(arr2[1]);
                tm1 = DateTime.ParseExact(tmmon, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;

            }
            #endregion
            //Channel Partner
            #region
            //if (cpnam != "" &&  cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm =="" && tm == "")
            if (cpnam != "" && cpnam != null)
            {
                var cp = db.ChannelPartners.Where(m => m.CPName == cpnam).Single();
                if (cp == null)
                {
                    TempData["wrongcp"] = "Please enter a valid Channel Partner Name and search again";
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return RedirectToAction("SpareMFR", "SpareReport");
                }

                //Channel Partner and Customer Name
                #region
                if (cpnam != "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    spmfr = db.OutwardMFR.Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner and d Code
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.ProductModelSparesID == ProductModelSparesID).Where(m => m.ChannelPartners.CPName == cpnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.ChannelPartners.CPName == cpnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner and Month range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,Customer Name and d Code
                #region
                if (cpnam != "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    spmfr = db.OutwardMFR.Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,Customer Name and Dispatch Status
                #region
                if (cpnam != "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,d Code and Dispatch Status
                #region
                if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,Customer Name and Month Range
                #region
                if (cpnam != "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,d Code and Month Range
                #region
                if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,Dispatch Status and Month Range
                #region
                if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.IsDispatch == dispst).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                spmfr = db.OutwardMFR.Where(m => m.ChannelPartners.CPName == cpnam).ToList();
            }
            #endregion
            //Customer Name
            #region
            //else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm =="" && tm == "")
            else if (cusnam != "" && cusnam != null)
            {
                var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                if (cn == null)
                {
                    TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                    return RedirectToAction("SpareMFR", "SpareReport");
                }

                //Customer Name and d Code
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.ProductModelSparesID == ProductModelSparesID).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Customer Name and Dispatch Status
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Customer Name and Month range
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.CustomerName == cusnam).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Customer,d Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.CustomerName == cusnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Customer Name,d Code and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.CustomerName == cusnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Customer Name,Dispatch Status and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.CustomerName == cusnam).Where(m => m.IsDispatch == dispst).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                spmfr = db.OutwardMFR.Where(m => m.CustomerName == cusnam).ToList();
            }
            #endregion
            //d Code
            #region
            //else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
            else if (ProductModelSparesID != 0)
            {
                //d Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //d Code and Month range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    // //return View(spmfr);
                }
                #endregion

                //d Code,Dispatch Status and Month Range
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.IsDispatch == dispst).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                // spmfr = db.OutwardMFR.Where(m => m.ProductModelSparesID == ProductModelSparesID).ToList();
            }
            #endregion
            //Dispatch Status
            #region
            else if (dispst != -1)
            {
                //Dispatch and Month range
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion
                spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).ToList();
            }
            #endregion
            //Month range
            #region
            else if (fm1 != 0 && tm1 != 0)
            {
                spmfr = db.OutwardMFR.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
            }
            #endregion
            ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");

            var pager = new Pager(spmfr.Count(), page, rowsPerPage);
            var viewModel = new IndexViewModel
            {
                OutwardMFR = spmfr.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager

            };

            ViewBag.SlNo = (page - 1) * rowsPerPage + 1;
            return View(viewModel);
            ////return View(spmfr);
        }

        public ActionResult ExportData(string cpnam = null, string cusnam = null, int ProductModelSparesID = 0, int dispst = -1, string fm = null, string tm = null)
        {
            GridView gv = new GridView();
            DataTable dts = new DataTable();
            int slno = 1;
            dts.Columns.Add("Sl No", typeof(String));
            dts.Columns.Add("Customer Name", typeof(String));
            dts.Columns.Add("Channel Partner Name", typeof(String));
            dts.Columns.Add("Machine Sl No.", typeof(String));
            dts.Columns.Add("MFR Generated Date", typeof(String));
            dts.Columns.Add("MFR No", typeof(String));
            dts.Columns.Add("d No.", typeof(String));
            dts.Columns.Add("Spare Description", typeof(String));
            dts.Columns.Add("Quantity", typeof(String));
            dts.Columns.Add("No. of Days taken for approved", typeof(String));
            dts.Columns.Add("No. of days to dispatch the spares ", typeof(String));
            dts.Columns.Add("No. of spares day to receveid Spares", typeof(String));
            dts.Columns.Add("No. of Days Machine issued closed", typeof(String));
            dts.Columns.Add("Total day to close the MFR issue", typeof(String));
            dts.Columns.Add("No. of days Faulty board sent to BBAN", typeof(String));
            //All are not Selected
            #region
            if (cpnam == "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm == "" && tm == "")
            {
                var dt = (from s in db.OutwardMFR
                          select new
                          {
                              s.ChannelPartners.CPName,
                              s.CustomerName,
                              s.MFRNo,
                              s.MFRId,
                              s.ProductModelSpare.ProductModelSparesName,
                              s.ProductModelSpare.ProductModelSparesDesc,
                              s.QuantityOrdered,
                              s.OutwardMonth,
                              s.IsDispatch,
                              s.MachineIssueDate,
                              s.DispatchDate,
                              s.SparesRecievedDate,
                              s.FaultSpareDate
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                    double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                    string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                    if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                    {
                        DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                        c = (b - a).TotalDays;

                        s = c.ToString() + " days";
                    }
                    else
                    {
                        s = "";
                    }
                    if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                    {
                        DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a1 > b1)
                        {
                            c1 = (a1 - b1).TotalDays;
                        }
                        else
                        {
                            c1 = (b1 - a1).TotalDays;
                        }
                        s1 = c1.ToString() + " days";
                    }
                    else
                    {
                        s1 = "";
                    }

                    if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                    {
                        DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a2 > b2)
                        {
                            c2 = (a2 - b2).TotalDays;
                        }
                        else
                        {
                            c2 = (b2 - a2).TotalDays;
                        }
                        s2 = c2.ToString() + " days";
                    }
                    else
                    {
                        s2 = "";
                    }
                    if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                    {
                        DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a3 > b3)
                        {
                            c3 = (a3 - b3).TotalDays;
                        }
                        else
                        {
                            c3 = (b3 - a3).TotalDays;
                        }
                        s3 = c3.ToString() + " days";
                    }
                    else
                    {
                        s3 = "";
                    }


                    if (s3 != "")
                    {
                        double c4 = c3 + c2 + c1 + c;
                        s4 = c4.ToString() + " days";
                    }
                    else
                    {
                        s4 = "";
                    }
                    if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                    {
                        DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a5 > b5)
                        {
                            c5 = (a5 - b5).TotalDays;
                        }
                        else
                        {
                            c5 = (b5 - a5).TotalDays;
                        }
                        s5 = c5.ToString() + " days";
                    }
                    else
                    {
                        s5 = "";
                    }
                    DataRow dr = dts.NewRow();
                    dr[0] = slno;
                    dr[1] = d.CustomerName;
                    dr[2] = d.CPName;
                    dr[3] = objMfr.MacSlNo;
                    dr[4] = objMfr.MfrDate;
                    dr[5] = d.MFRNo;
                    dr[6] = d.ProductModelSparesName;
                    dr[7] = d.ProductModelSparesDesc;
                    dr[8] = d.QuantityOrdered;
                    dr[9] = s;
                    dr[10] = s1;
                    dr[11] = s2;
                    dr[12] = s3;
                    dr[13] = s4;
                    dr[14] = s5;

                    dts.Rows.Add(dr);
                    slno++;
                }
                gv.DataSource = dts;
                gv.DataBind();
            }
            #endregion

            int fm1 = 0, tm1 = 0;
            //Month Name to Month Number
            #region
            if (fm != "" && fm != null && tm != "" && tm != null)
            {
                var fmmon = fm.Substring(0, 3);
                //getting month number from month name
                fm1 = DateTime.ParseExact(fmmon, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;

                var tmmon = tm.Substring(0, 3);
                //getting month number from month name
                tm1 = DateTime.ParseExact(tmmon, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            }
            #endregion

            //Channel Partner
            #region
            //if (cpnam != "" &&  cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm =="" && tm == "")
            if (cpnam != "" && cpnam != null)
            {
                var cp = db.ChannelPartners.Where(m => m.CPName == cpnam).Single();
                if (cp == null)
                {
                    TempData["wrongcp"] = "Please enter a valid Channel Partner Name and search again";
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return RedirectToAction("SpareMFR", "SpareReport");
                }

                //Channel Partner and Customer Name
                #region
                if (cpnam != "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Channel Partner and d Code
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner and Month range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Customer Name and d Code
                #region
                else if (cpnam != "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Customer Name and Dispatch Status
                #region
                else if (cpnam != "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,d Code and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Customer Name and Month Range
                #region
                else if (cpnam != "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                    if (cn == null)
                    {
                        TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                        ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                        return RedirectToAction("SpareMFR", "SpareReport");
                    }
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,d Code and Month Range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Dispatch Status and Month Range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.IsDispatch == dispst) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner
                #region
                else
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion
            }
            #endregion

            //Customer Name
            #region
            //else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm =="" && tm == "")
            else if (cusnam != "" && cusnam != null)
            {
                var cn = db.MDBGeneralData.Where(m => m.OrganizationName == cusnam).Single();
                if (cn == null)
                {
                    TempData["wrongcp"] = "Please enter a valid Customer Name and search again";
                    return RedirectToAction("SpareMFR", "SpareReport");
                }

                //Customer Name and d Code
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name and Dispatch Status
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name and Month range
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer,d Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name,d Code and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name,Dispatch Status and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.IsDispatch == dispst) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer
                #region
                else
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion
            }
            #endregion

            //d Code
            #region
            //else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
            else if (ProductModelSparesID != 0)
            {
                //d Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //d Code and Month range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //d Code,Dispatch Status and Month Range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.IsDispatch == dispst) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //d Code
                #region
                else
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion
            }
            #endregion

            //Dispatch Status
            #region
            else if (dispst != -1)
            {
                //Dispatch and Month range
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.IsDispatch == dispst && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Dispatch Status
                #region
                else
                {
                    var dt = (from s in db.OutwardMFR
                              where s.IsDispatch == dispst
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.MFRNo,
                                  s.MFRId,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.MachineIssueDate,
                                  s.DispatchDate,
                                  s.SparesRecievedDate,
                                  s.FaultSpareDate
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                        string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                        if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                            c = (b - a).TotalDays;

                            s = c.ToString() + " days";
                        }
                        else
                        {
                            s = "";
                        }
                        if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                        {
                            DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a1 > b1)
                            {
                                c1 = (a1 - b1).TotalDays;
                            }
                            else
                            {
                                c1 = (b1 - a1).TotalDays;
                            }
                            s1 = c1.ToString() + " days";
                        }
                        else
                        {
                            s1 = "";
                        }

                        if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                        {
                            DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a2 > b2)
                            {
                                c2 = (a2 - b2).TotalDays;
                            }
                            else
                            {
                                c2 = (b2 - a2).TotalDays;
                            }
                            s2 = c2.ToString() + " days";
                        }
                        else
                        {
                            s2 = "";
                        }
                        if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                        {
                            DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a3 > b3)
                            {
                                c3 = (a3 - b3).TotalDays;
                            }
                            else
                            {
                                c3 = (b3 - a3).TotalDays;
                            }
                            s3 = c3.ToString() + " days";
                        }
                        else
                        {
                            s3 = "";
                        }


                        if (s3 != "")
                        {
                            double c4 = c3 + c2 + c1 + c;
                            s4 = c4.ToString() + " days";
                        }
                        else
                        {
                            s4 = "";
                        }
                        if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                        {
                            DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                            if (a5 > b5)
                            {
                                c5 = (a5 - b5).TotalDays;
                            }
                            else
                            {
                                c5 = (b5 - a5).TotalDays;
                            }
                            s5 = c5.ToString() + " days";
                        }
                        else
                        {
                            s5 = "";
                        }
                        DataRow dr = dts.NewRow();
                        dr[0] = slno;
                        dr[1] = d.CustomerName;
                        dr[2] = d.CPName;
                        dr[3] = objMfr.MacSlNo;
                        dr[4] = objMfr.MfrDate;
                        dr[5] = d.MFRNo;
                        dr[6] = d.ProductModelSparesName;
                        dr[7] = d.ProductModelSparesDesc;
                        dr[8] = d.QuantityOrdered;
                        dr[9] = s;
                        dr[10] = s1;
                        dr[11] = s2;
                        dr[12] = s3;
                        dr[13] = s4;
                        dr[14] = s5;

                        dts.Rows.Add(dr);
                        slno++;
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion
            }
            #endregion

            //Month range
            #region
            else if (fm1 != 0 && tm1 != 0)
            {
                var dt = (from s in db.OutwardMFR
                          where s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1
                          select new
                          {
                              s.ChannelPartners.CPName,
                              s.CustomerName,
                              s.MFRNo,
                              s.MFRId,
                              s.ProductModelSpare.ProductModelSparesName,
                              s.ProductModelSpare.ProductModelSparesDesc,
                              s.QuantityOrdered,
                              s.OutwardMonth,
                              s.IsDispatch,
                              s.MachineIssueDate,
                              s.DispatchDate,
                              s.SparesRecievedDate,
                              s.FaultSpareDate
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                    double c = 0, c1 = 0, c2 = 0, c3 = 0, c5 = 0;
                    string s = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                    if ((objMfr.MfrDate != null || objMfr.MfrDate != "") && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                    {
                        DateTime a = Convert.ToDateTime(objMfr.MfrDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                        c = (b - a).TotalDays;

                        s = c.ToString() + " days";
                    }
                    else
                    {
                        s = "";
                    }
                    if ((d.DispatchDate != null) && (MFRBBan.MfrAdminDat != null || MFRBBan.MfrAdminDat == ""))
                    {
                        DateTime a1 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b1 = Convert.ToDateTime(MFRBBan.MfrAdminDat, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a1 > b1)
                        {
                            c1 = (a1 - b1).TotalDays;
                        }
                        else
                        {
                            c1 = (b1 - a1).TotalDays;
                        }
                        s1 = c1.ToString() + " days";
                    }
                    else
                    {
                        s1 = "";
                    }

                    if ((d.SparesRecievedDate != null) && (d.DispatchDate != null))
                    {
                        DateTime a2 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b2 = Convert.ToDateTime(d.DispatchDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a2 > b2)
                        {
                            c2 = (a2 - b2).TotalDays;
                        }
                        else
                        {
                            c2 = (b2 - a2).TotalDays;
                        }
                        s2 = c2.ToString() + " days";
                    }
                    else
                    {
                        s2 = "";
                    }
                    if ((d.MachineIssueDate != null) && (d.SparesRecievedDate != null))
                    {
                        DateTime a3 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b3 = Convert.ToDateTime(d.SparesRecievedDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a3 > b3)
                        {
                            c3 = (a3 - b3).TotalDays;
                        }
                        else
                        {
                            c3 = (b3 - a3).TotalDays;
                        }
                        s3 = c3.ToString() + " days";
                    }
                    else
                    {
                        s3 = "";
                    }


                    if (s3 != "")
                    {
                        double c4 = c3 + c2 + c1 + c;
                        s4 = c4.ToString() + " days";
                    }
                    else
                    {
                        s4 = "";
                    }
                    if ((d.FaultSpareDate != null) && (d.MachineIssueDate != null))
                    {
                        DateTime a5 = Convert.ToDateTime(d.FaultSpareDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        DateTime b5 = Convert.ToDateTime(d.MachineIssueDate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                        if (a5 > b5)
                        {
                            c5 = (a5 - b5).TotalDays;
                        }
                        else
                        {
                            c5 = (b5 - a5).TotalDays;
                        }
                        s5 = c5.ToString() + " days";
                    }
                    else
                    {
                        s5 = "";
                    }
                    DataRow dr = dts.NewRow();
                    dr[0] = slno;
                    dr[1] = d.CustomerName;
                    dr[2] = d.CPName;
                    dr[3] = objMfr.MacSlNo;
                    dr[4] = objMfr.MfrDate;
                    dr[5] = d.MFRNo;
                    dr[6] = d.ProductModelSparesName;
                    dr[7] = d.ProductModelSparesDesc;
                    dr[8] = d.QuantityOrdered;
                    dr[9] = s;
                    dr[10] = s1;
                    dr[11] = s2;
                    dr[12] = s3;
                    dr[13] = s4;
                    dr[14] = s5;

                    dts.Rows.Add(dr);
                    slno++;
                }
                gv.DataSource = dts;

                gv.DataBind();
            }
            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MFRDuration.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("SpareMFR");
        }

    }
}
