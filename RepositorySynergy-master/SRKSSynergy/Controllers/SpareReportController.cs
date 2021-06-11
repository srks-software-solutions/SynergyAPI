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
    public class SpareReportController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //
        // GET: /SpareReport/

        public ActionResult Index()
        {
            return View();
        }

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
        public ActionResult SpareMFR(int page = 1, int rowsPerPage=20,string cpnam =null,string cusnam=null,int ProductModelSparesID=0,int dispst=-1,string fm=null,string tm=null)
        {


            int fm1 = 0, tm1 = 0;
            int fy1 = 0, ty1 = 0;
            var spmfr = db.OutwardMFR.ToList();
            //Data Required for Export to excel
            ViewBag.cpnam = cpnam;
            ViewBag.cusnam = cusnam;
            ViewBag.prdid = ProductModelSparesID;
            ViewBag.dispt = dispst;
            ViewBag.fm = fm;
            ViewBag.tm = tm;

            //Month Name to Month Number
            #region
            if (fm != "" && fm !=null && tm != "" && tm !=null)
            {
                var fmmon = fm.Substring(0, 3);
                //getting month number from month name
                fm1 = DateTime.ParseExact(fmmon, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                string[] arr1 = fm.Split(' ');
                fy1 =Convert.ToInt32( arr1[1]);

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
            if (cpnam != "" && cpnam !=null)
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

                //Channel Partner and Item Code
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.ProductModelSparesID == ProductModelSparesID).Where(m =>m.ChannelPartners.CPName== cpnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m =>m.ChannelPartners.CPName==cpnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner and Month range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m =>m.ChannelPartners.CPName == cpnam).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,Customer Name and Item Code
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

                //Channel Partner,Item Code and Dispatch Status
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
                    spmfr = db.OutwardMFR.Where(m => m.OutMonthNo>=fm1 && m.OutMonthNo<=tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Channel Partner,Item Code and Month Range
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

                //Customer Name and Item Code
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

                //Customer,Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.CustomerName == cusnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Customer Name,Item Code and Month Range
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
            //Item Code
            #region
            //else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
            else if (ProductModelSparesID != 0)
            {
                //Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.IsDispatch == dispst).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    //return View(spmfr);
                }
                #endregion

                //Item Code and Month range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardMFR.Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                   // //return View(spmfr);
                }
                #endregion

                //Item Code,Dispatch Status and Month Range
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

            ViewBag.SlNo = (page-1) * rowsPerPage + 1;
            return View(viewModel);
            ////return View(spmfr);
        }

        public ActionResult ExportData(string cpnam = null, string cusnam = null, int ProductModelSparesID = 0, int dispst = -1, string fm = null, string tm = null)
        {
            GridView gv = new GridView();
            DataTable dts = new DataTable();
            int slno = 1;
            dts.Columns.Add("Serial No", typeof(String));
            dts.Columns.Add("Customer Name", typeof(String));
            dts.Columns.Add("Channel Partner Name", typeof(String));
            dts.Columns.Add("Machine serial Number", typeof(String));
            dts.Columns.Add("MFR Generated Date", typeof(String));
            dts.Columns.Add("MFR No", typeof(String));
            dts.Columns.Add("Item No", typeof(String));
            dts.Columns.Add("Spare Description", typeof(String));
            dts.Columns.Add("Quantity", typeof(String));
            dts.Columns.Add("MFR Approve/Reject Date", typeof(String));
            dts.Columns.Add("Status (Approved/Rejected)", typeof(String));
            dts.Columns.Add("Comment of Approval /Reject", typeof(String));
            dts.Columns.Add("Spare Dispatch Date", typeof(String));
            dts.Columns.Add("Spares Courier Information", typeof(String));
            dts.Columns.Add("Spare Recieved", typeof(String));
            dts.Columns.Add("Engineer Visit Date", typeof(String));
            dts.Columns.Add("Machine Issue Description", typeof(String));
            dts.Columns.Add("Status", typeof(String));
            dts.Columns.Add("Courier Info(Faulty Board)", typeof(String));
            dts.Columns.Add("Faulty Spare Dispatch Date", typeof(String));
            dts.Columns.Add("Faulty Recd Date", typeof(String));
            int fm1 = 0, tm1 = 0;
            //All are not Selected
            #region
            if (cpnam == "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm == "" && tm == "")
            {
                var dt = (from s in db.OutwardMFR
                          select new
                          {
                              s.ChannelPartners.CPName,
                              s.CustomerName,
                               s.MFRNo,s.MFRId,
                              s.ProductModelSpare.ProductSpareNameDesc,
                              s.Quantity,
                              s.OutwardMonth,
                              s.IsDispatch,
                              s.ProductModelSpare.ProductModelSparesName,
                              s.ProductModelSpare.ProductModelSparesDesc,
                              s.QuantityOrdered,
                              s.DispatchDate,
                              s.MachineIssueDate,
                              s.MachineIssueDescription,
                              s.FaultSpareDescription,
                              s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                    string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                    if (objMfr.ApprovalStatus == 0)
                     {
                        statusaprvorrejted = "Pending";
                     }
                     if (objMfr.ApprovalStatus == 1)
                     {
                         statusaprvorrejted = "Approved";
                     }
                     if (objMfr.ApprovalStatus == 2)
                     {
                       statusaprvorrejted = "Rejected";
                     }
                     string sprsrcvd = "";
                     if (d.IsDispatch == 2)
                     {
                         sprsrcvd = "Yes";
                     }
                     else {
                         sprsrcvd = "No";
                     }
                     string sts = "";
                     if (d.IsDispatch == 0)
                     {
                         sts = "Open";
                     }
                     else
                     {
                         sts = "Close";
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
                    dr[9] = aprvldat;
                    dr[10] = statusaprvorrejted;
                    dr[11] = objMfr.MFRComment;
                    dr[12] = d.DispatchDate;
                    dr[13] = d.Remarks;
                    dr[14] = sprsrcvd;
                    dr[15] = d.MachineIssueDate;
                    dr[16] = d.MachineIssueDescription;
                    dr[17] = sts;
                    dr[18] = d.FaultSpareDescription;
                    dr[19] = d.FaultSpareDate;
                    dr[20] = d.FaultSpareRecivedDateAdmin;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
            }
            #endregion

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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Channel Partner and Item Code
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                              where s.ChannelPartners.CPName == cpnam && (s.OutMonthNo >= fm1 && s.OutMonthNo <=tm1)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Customer Name and Item Code
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Item Code and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.IsDispatch == dispst)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Item Code and Month Range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ChannelPartners.CPName == cpnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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

                //Customer Name and Item Code
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer,Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) &&(s.IsDispatch == dispst)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name,Item Code and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.OutMonthNo >=fm1 && s.OutMonthNo<=tm1)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion
            }
            #endregion

            //Item Code
            #region
            //else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
            else if (ProductModelSparesID != 0)
            {
                //Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.IsDispatch ==dispst)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Item Code and Month range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Item Code,Dispatch Status and Month Range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.IsDispatch ==dispst) && (s.OutMonthNo>=fm1 && s.OutMonthNo<=tm1)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Item Code
                #region
                else
                {
                    var dt = (from s in db.OutwardMFR
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                              where s.IsDispatch == dispst && (s.OutMonthNo>=fm1 && s.OutMonthNo<=tm1)
                              select new
                              {

                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                                   s.MFRNo,s.MFRId,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.IsDispatch,
                                  s.ProductModelSpare.ProductModelSparesName,
                                  s.ProductModelSpare.ProductModelSparesDesc,
                                  s.QuantityOrdered,
                                  s.DispatchDate,
                                  s.MachineIssueDate,
                                  s.MachineIssueDescription,
                                  s.FaultSpareDescription,
                                  s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                        string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                        if (objMfr.ApprovalStatus == 0)
                        {
                            statusaprvorrejted = "Pending";
                        }
                        if (objMfr.ApprovalStatus == 1)
                        {
                            statusaprvorrejted = "Approved";
                        }
                        if (objMfr.ApprovalStatus == 2)
                        {
                            statusaprvorrejted = "Rejected";
                        }
                        string sprsrcvd = "";
                        if (d.IsDispatch == 2)
                        {
                            sprsrcvd = "Yes";
                        }
                        else
                        {
                            sprsrcvd = "No";
                        }
                        string sts = "";
                        if (d.IsDispatch == 0)
                        {
                            sts = "Open";
                        }
                        else
                        {
                            sts = "Close";
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
                        dr[9] = aprvldat;
                        dr[10] = statusaprvorrejted;
                        dr[11] = objMfr.MFRComment;
                        dr[12] = d.DispatchDate;
                        dr[13] = d.Remarks;
                        dr[14] = sprsrcvd;
                        dr[15] = d.MachineIssueDate;
                        dr[16] = d.MachineIssueDescription;
                        dr[17] = sts;
                        dr[18] = d.FaultSpareDescription;
                        dr[19] = d.FaultSpareDate;
                        dr[20]=d.FaultSpareRecivedDateAdmin;

                        dts.Rows.Add(dr);
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
                               s.MFRNo,s.MFRId,
                              s.ProductModelSpare.ProductSpareNameDesc,
                              s.Quantity,
                              s.OutwardMonth,
                              s.IsDispatch,
                              s.ProductModelSpare.ProductModelSparesName,
                              s.ProductModelSpare.ProductModelSparesDesc,
                              s.QuantityOrdered,
                              s.DispatchDate,
                              s.MachineIssueDate,
                              s.MachineIssueDescription,
                              s.FaultSpareDescription,
                              s.FaultSpareDate,s.Remarks,s.FaultSpareRecivedDateAdmin
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    var objMfr = db.MFR.Where(m => m.MFRNumber == d.MFRNo).FirstOrDefault(); var MFRBBan = db.MFRBBAN.Where(m => m.MFRID == objMfr.MFRID).FirstOrDefault();
                    string statusaprvorrejted = "";string aprvldat = MFRBBan.MfrAdminDat;
                    if (objMfr.ApprovalStatus == 0)
                    {
                        statusaprvorrejted = "Pending";
                    }
                    if (objMfr.ApprovalStatus == 1)
                    {
                        statusaprvorrejted = "Approved";
                    }
                    if (objMfr.ApprovalStatus == 2)
                    {
                        statusaprvorrejted = "Rejected";
                    }
                    string sprsrcvd = "";
                    if (d.IsDispatch == 2)
                    {
                        sprsrcvd = "Yes";
                    }
                    else
                    {
                        sprsrcvd = "No";
                    }
                    string sts = "";
                    if (d.IsDispatch == 0)
                    {
                        sts = "Open";
                    }
                    else
                    {
                        sts = "Close";
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
                    dr[9] = aprvldat;
                    dr[10] = statusaprvorrejted;
                    dr[11] = objMfr.MFRComment;
                    dr[12] = d.DispatchDate;
                    dr[13] = d.Remarks;
                    dr[14] = sprsrcvd;
                    dr[15] = d.MachineIssueDate;
                    dr[16] = d.MachineIssueDescription;
                    dr[17] = sts;
                    dr[18] = d.FaultSpareDescription;
                    dr[19] = d.FaultSpareDate;
                    dr[20]=d.FaultSpareRecivedDateAdmin;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;

                gv.DataBind();
            }
            #endregion



            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=SpareMFR.xls");
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

        [Authorize(Roles = "Administrator")]
        public ActionResult SpareInvoice(string cpnam = null, string cusnam = null, int ProductModelSparesID = 0, int dispst = -1, string fm = null, string tm = null)
        {
            int fm1 = 0, tm1 = 0;
            var spmfr = db.OutwardSpare.ToList();
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
                    spmfr = db.OutwardSpare.Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner and Item Code
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.ProductModelSparesID == ProductModelSparesID).Where(m => m.ChannelPartners.CPName == cpnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.ChannelPartners.CPName == cpnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner and Month range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner,Customer Name and Item Code
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
                    spmfr = db.OutwardSpare.Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
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
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner,Item Code and Dispatch Status
                #region
                if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
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
                    spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner,Item Code and Month Range
                #region
                if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Channel Partner,Dispatch Status and Month Range
                #region
                if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ChannelPartners.CPName == cpnam).Where(m => m.IsDispatch == dispst).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                spmfr = db.OutwardSpare.Where(m => m.ChannelPartners.CPName == cpnam).ToList();
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

                //Customer Name and Item Code
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.ProductModelSparesID == ProductModelSparesID).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Customer Name and Dispatch Status
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.CustomerName == cusnam).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Customer Name and Month range
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.CustomerName == cusnam).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Customer,Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.CustomerName == cusnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Customer Name,Item Code and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.CustomerName == cusnam).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Customer Name,Dispatch Status and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.CustomerName == cusnam).Where(m => m.IsDispatch == dispst).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                spmfr = db.OutwardSpare.Where(m => m.CustomerName == cusnam).ToList();
            }
            #endregion
            //Item Code
            #region
            //else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
            else if (ProductModelSparesID != 0)
            {
                //Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Item Code and Month range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                //Item Code,Dispatch Status and Month Range
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm != "" && tm != "")
                {
                    spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).Where(m => m.ProductModelSpare.ProductModelSparesID == ProductModelSparesID).Where(m => m.IsDispatch == dispst).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion

                spmfr = db.OutwardSpare.Where(m => m.ProductModelSparesID == ProductModelSparesID).ToList();
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
                    spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
                    ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
                    return View(spmfr);
                }
                #endregion
                spmfr = db.OutwardSpare.Where(m => m.IsDispatch == dispst).ToList();
            }
            #endregion
            //Month range
            #region
            else if (fm1 != 0 && tm1 != 0)
            {
                spmfr = db.OutwardSpare.Where(m => m.OutMonthNo >= fm1 && m.OutMonthNo <= tm1).ToList();
            }
            #endregion
            ViewBag.ProductModelSparesID = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductModelSparesDesc");
            return View(spmfr);
        }

        public ActionResult ExportDataInv(string cpnam = null, string cusnam = null, int ProductModelSparesID = 0, int dispst = -1, string fm = null, string tm = null)
        {
            GridView gv = new GridView();
            DataTable dts = new DataTable();
            dts.Columns.Add("Channel Partner Name", typeof(String));
            dts.Columns.Add("Customer Name", typeof(String));
            dts.Columns.Add("Order No", typeof(String));
            dts.Columns.Add("Invoice No", typeof(String));
            dts.Columns.Add("Item Code", typeof(String));
            dts.Columns.Add("Quantity", typeof(String));
            dts.Columns.Add("Month", typeof(String));
            dts.Columns.Add("Invoice Date", typeof(String));
            dts.Columns.Add("Dispatch Date", typeof(String));
            dts.Columns.Add("Dispatch Status", typeof(String));

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
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Channel Partner and Item Code
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner and Month range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Customer Name and Item Code
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
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
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
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Item Code and Dispatch Status
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
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
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.CustomerName == cusnam) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Item Code and Month Range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner,Dispatch Status and Month Range
                #region
                else if (cpnam != "" && cusnam == "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam && (s.IsDispatch == dispst) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                #endregion

                //Channel Partner
                #region
                else
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ChannelPartners.CPName == cpnam
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
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

                //Customer Name and Item Code
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name and Dispatch Status
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name and Month range
                #region
                else if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer,Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name,Item Code and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam && (s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer Name,Dispatch Status and Month Range
                #region
                if (cpnam == "" && cusnam != "" && ProductModelSparesID == 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam && (s.IsDispatch == dispst) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Customer
                #region
                else
                {
                    var dt = (from s in db.OutwardSpare
                              where s.CustomerName == cusnam
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion
            }
            #endregion

            //Item Code
            #region
            //else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm == "" && tm == "")
            else if (ProductModelSparesID != 0)
            {
                //Item Code and Dispatch Status
                #region
                if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm == "" && tm == "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.IsDispatch == dispst)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Item Code and Month range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst == -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Item Code,Dispatch Status and Month Range
                #region
                else if (cpnam == "" && cusnam == "" && ProductModelSparesID != 0 && dispst != -1 && fm != "" && tm != "")
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID && (s.IsDispatch == dispst) && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Item Code
                #region
                else
                {
                    var dt = (from s in db.OutwardSpare
                              where s.ProductModelSpare.ProductModelSparesID == ProductModelSparesID
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
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
                    var dt = (from s in db.OutwardSpare
                              where s.IsDispatch == dispst && (s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1)
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }
                #endregion

                //Dispatch Status
                #region
                else
                {
                    var dt = (from s in db.OutwardSpare
                              where s.IsDispatch == dispst
                              select new
                              {
                                  s.ChannelPartners.CPName,
                                  s.CustomerName,
                                  s.OrderNo,
                                  s.InvoiceNo,
                                  s.ProductModelSpare.ProductSpareNameDesc,
                                  s.Quantity,
                                  s.OutwardMonth,
                                  s.InvoiceDate,
                                  s.DispatchDate,
                                  s.IsDispatch
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        string sts = null;
                        dr[0] = d.CPName;
                        dr[1] = d.CustomerName;
                        dr[2] = d.OrderNo;
                        dr[3] = d.InvoiceNo;
                        dr[4] = d.ProductSpareNameDesc;
                        dr[5] = d.Quantity;
                        dr[6] = d.OutwardMonth;
                        dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                        dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                        if (d.IsDispatch == 0)
                            sts = "Open";
                        else
                            sts = "Close";
                        dr[9] = sts;
                        dts.Rows.Add(dr);
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
                var dt = (from s in db.OutwardSpare
                          where s.OutMonthNo >= fm1 && s.OutMonthNo <= tm1
                          select new
                          {
                              s.ChannelPartners.CPName,
                              s.CustomerName,
                              s.OrderNo,
                              s.InvoiceNo,
                              s.ProductModelSpare.ProductSpareNameDesc,
                              s.Quantity,
                              s.OutwardMonth,
                              s.InvoiceDate,
                              s.DispatchDate,
                              s.IsDispatch
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    DataRow dr = dts.NewRow();
                    string sts = null;
                    dr[0] = d.CPName;
                    dr[1] = d.CustomerName;
                    dr[2] = d.OrderNo;
                    dr[3] = d.InvoiceNo;
                    dr[4] = d.ProductSpareNameDesc;
                    dr[5] = d.Quantity;
                    dr[6] = d.OutwardMonth;
                    dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                    dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                    if (d.IsDispatch == 0)
                        sts = "Open";
                    else
                        sts = "Close";
                    dr[9] = sts;
                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;

                gv.DataBind();
            }
            #endregion

            //All are not Selected
            #region
            if (cpnam == "" && cusnam == "" && ProductModelSparesID == 0 && dispst == -1 && fm == "" && tm == "")
            {
                var dt = (from s in db.OutwardSpare
                          select new
                          {
                              s.ChannelPartners.CPName,
                              s.CustomerName,
                              s.OrderNo,
                              s.InvoiceNo,
                              s.ProductModelSpare.ProductSpareNameDesc,
                              s.Quantity,
                              s.OutwardMonth,
                              s.InvoiceDate,
                              s.DispatchDate,
                              s.IsDispatch
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    DataRow dr = dts.NewRow();
                    string sts = null;
                    dr[0] = d.CPName;
                    dr[1] = d.CustomerName;
                    dr[2] = d.OrderNo;
                    dr[3] = d.InvoiceNo;
                    dr[4] = d.ProductSpareNameDesc;
                    dr[5] = d.Quantity;
                    dr[6] = d.OutwardMonth;
                    dr[7] = d.InvoiceDate.ToString("dd-MM-yyyy");
                    dr[8] = d.DispatchDate.ToString("dd-MM-yyyy");
                    if (d.IsDispatch == 0)
                        sts = "Open";
                    else
                        sts = "Close";
                    dr[9] = sts;
                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
            }
            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=SpareInvoice.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("SpareInvoice");
        }

    }
}
