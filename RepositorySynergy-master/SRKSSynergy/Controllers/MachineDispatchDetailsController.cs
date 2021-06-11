using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;
using SRKSSynergy.Helpers;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Web.UI;
using Common.Logging;
using System.Web.UI.WebControls;
using System.Net.Mail;
using SRKSSynergy.MailServices;

namespace SRKSSynergy.Controllers
{
    public class MachineDispatchDetailsController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete Machine Serial no
        public JsonResult AutocompleteMacSlNo(string term)
        {
            var result = (from r in db.MachineInventory
                          where r.MachineSerialNo.ToLower().Contains(term.ToLower()) && r.IsDispatched == 1
                          select new { r.MachineSerialNo }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MachineDispatch
                          where (r.MDBGeneralData.OrganizationName.Contains(term.ToLower()) && (r.IsDeleted == 0) && r.IsDispatched==1)
                          select new { r.MDBGeneralData.OrganizationName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /MachineDispatchDetails/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult MachineDispatch(int MacinvID, int ProductModelID)
        {
            if (MacinvID != 0)
            {
                var macdet = db.MachineInventory.Where(m => m.MachineInventoryID == MacinvID).Where(m => m.ProductModelID == ProductModelID).Select(m => new { m.MachineSerialNo, m.CustomerName, m.MDBID }).SingleOrDefault();

                ViewBag.CustNM = macdet.CustomerName;
                ViewBag.MacSlNo = macdet.MachineSerialNo;
                ViewBag.Macid = MacinvID;
                ViewBag.ProductModelID = ProductModelID;

                ViewData["OANumber"] = new SelectList(db.OAEquipGeneralData.Where(m => m.ApprovalStatus == 1).Where(m => m.MDBID == macdet.MDBID), "OANumber", "OANumber");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult MachineDispatch(MachineDispatch macsto)
        {
            var oaid = db.OAEquipGeneralData.Where(m => m.OANumber == macsto.OANumber).Select(m => new { m.OAID, m.MDBID, m.CPID }).Single();
            //var pid = db.OAEquipTableData.Where(m => m.OAID == oaid.OAID).Select(m => m.ProductModelID).Single();
            macsto.OAID = oaid.OAID;
            macsto.CPID = oaid.CPID;
            macsto.MDBID = oaid.MDBID;
            //macsto.ProductModelID = pid;
            int mdbid = oaid.MDBID;

            if (macsto.OANumber != null && macsto.InvoiceNumber != null)
            {
                if (macsto.OADate <= macsto.InvoiceDate)
                //if(Convert.ToDateTime(macsto.OADate) <= macsto.InvoiceDate)
                {
                    macsto.IsDispatched = 1;
                    db.MachineDispatch.Add(macsto);
                    db.SaveChanges();
                    TempData["Success"] = "Machine Dispatch Details Added Successfully!!!!";
                }
                else
                {
                    TempData["Oadate"] = "Invoice Date cannot be less than Order Confirmation Date!!!!";
                    ViewData["OANumber"] = new SelectList(db.OAEquipGeneralData.Where(m => m.ApprovalStatus == 1).Where(m => m.MDBID == mdbid), "OANumber", "OANumber");
                    return View(macsto);
                }

                //
                //Rmoving from Machine Inventory
                //
                int Macinid = macsto.MachineInventoryID;
                var macinv = db.MachineInventory.Where(m => m.MachineInventoryID == Macinid).ToList();
                foreach (var m in macinv)
                {
                    m.IsDispatched = 1;
                    db.Entry(m).State = EntityState.Modified;
                    db.SaveChanges();

                    var prdid = macsto.ProductModelID;//db.MachineInventory.Where(s => s.MachineInventoryID == Macinid).Select(s => s.ProductModelID).FirstOrDefault();
                    var maccount = db.ProductModel.Where(s => s.ProductModelID == prdid).Where(s => s.IsDeleted == 0).ToList();
                    foreach (var mc in maccount)
                    {
                        mc.MachineCount = mc.MachineCount - 1;
                        db.Entry(mc).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }


                //
                //Emailing Dispatch details to Channel Partner
                //
                #region

                int cpid = oaid.CPID;
                string name = db.ChannelPartners.Where(s => s.CPID == cpid).Select(s => s.CPName).FirstOrDefault();
                //var cpnm = db.CPContactPersonData.Where(m => m.CPID == cpid).Select(m => new { m.Title, m.FirstName, m.LastName, m.EmailID }).FirstOrDefault();

                //var email = cpnm.EmailID;
                //var name = cpnm.Title + " " + cpnm.FirstName + " " + cpnm.LastName;
                ////var email = "sharath.krishna@srkssolutions.com";

                var macslno = macsto.MachineInventory.MachineSerialNo;
                var orderno = macsto.OANumber;
                var oadate = Convert.ToDateTime(macsto.OADate).ToString("dd-MM-yyyy");
                var invnum = macsto.InvoiceNumber;
                var invdate = Convert.ToDateTime(macsto.InvoiceDate).ToString("dd-MM-yyyy");
                var lrnum = macsto.LRNumber;
                var dispdate = Convert.ToDateTime(macsto.DispatchDate).ToString("dd-MM-yyyy");
                var transp = macsto.Transporter;
                var modelc = macsto.MachineInventory.ProductModel.ProductModelName;
                var custnm = macsto.MachineInventory.CustomerName;

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                    mail.From = new MailAddress("bbandispatches@gmail.com");
                    //mail.From = new MailAddress("janardhangrd@gmail.com");

                    //mail.CC.Add("bharath.naik@buhlergroup.com");
                    //mail.CC.Add("sanjay.deshpande@buhlergroup.com");


                    //for To get emailId from CPMailIdTo using cpid
                    var email = db.CPMailIdTO.Where(s => s.CPID == cpid).Select(s => s.EmailId).FirstOrDefault();
                    mail.To.Add(email);

                    //for CC get admin emails
                    var AdminEmailToCC = db.BuhlerAdminCC.Where(s => s.Type == "Admin").ToList();
                    foreach (var mc in AdminEmailToCC)
                    {
                        string adminemail = mc.EmailId;
                        mail.CC.Add(new MailAddress(adminemail));
                    }


                    mail.Subject = "Machine Dispatch Details";

                    mail.Body = "<head>" +
                            "<style type=\"text/css\">" +
                            "td" +
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
                                "text-align: center;" +
                                "border-style: solid;" +
                                "border-width: 1px;" +
                            "}" +
                            ".auto-style6 {" +
                                "vertical-align: middle;" +
                                "text-align: center;" +
                                "border-style: solid;" +
                                "border-width: 1px;" +
                                "background-color: #59C9FF;" +
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

                            "<p>&nbsp;</p>" +
                            "<p>Dear " + name + ",</p>" +
                            "<p>Please find the Machine Dispatch Details below</p>" +
                            "<p>&nbsp;</p>" +
                            "<table class=\"auto-style1\" style=\"width: 100%; height: 79px\">" +
                                "<tr>" +
                                    "<td class=\"auto-style2\" colspan=\"10\">Machine Dispatch Details</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td class=\"auto-style7\">Machine<br />" +
                            "&nbsp;Serial No</td>" +
                                    "<td class=\"auto-style6\">Model Number</td>" +
                                    "<td class=\"auto-style6\">Customer Name</td>" +
                                    "<td class=\"auto-style6\">Order <br />" +
                                    "Acknowledgement No</td>" +
                                    "<td class=\"auto-style6\">OA Date</td>" +
                                    "<td class=\"auto-style6\">Invoice<br />" +
                                    "Number</td>" +
                                    "<td class=\"auto-style6\">Invoice<br />" +
                                    "Date</td>" +
                                    "<td class=\"auto-style6\">LR<br />" +
                                    "Number</td>" +
                                    "<td class=\"auto-style6\">Dispatch<br />" +
                                    "Date</td>" +
                                    "<td class=\"auto-style6\">Transporter</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + macslno + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + modelc + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + custnm + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + orderno + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + oadate + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + invnum + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + invdate + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + lrnum + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + dispdate + "</td>" +
                                    "<td class=\"auto-style5\" style=\"height: 30px\">" + transp + "</td>" +

                                "</tr>" +
                            "</table>" +
                            "<p>&nbsp;</p>" +
                            "<p class=\"auto-style8\">Note :<span class=\"auto-style9\"> Please donot reply to this " +
                            "Email ID, this is a Autogenerated Email</span></p>" +

                            "</body>";



                    mail.IsBodyHtml = true;
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("bbandispatches@gmail.com", "bban2014");
                    //SmtpServer.Credentials = new System.Net.NetworkCredential("janardhangrd@gmail.com", "j2n26dh2n");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                #endregion
                //
                //Adding the HOD View after Machine Dispatch
                var disp = db.OAEquipGeneralData.Where(m => m.OAID == oaid.OAID).First();
                int macno = disp.IsMacineDispatch + 1;
                disp.IsMacineDispatch = macno;
                db.Entry(disp).State = EntityState.Modified;
                db.SaveChanges();
                //End

                return RedirectToAction("Index", "MachineStock");
            }
            ViewData["OANumber"] = new SelectList(db.OAEquipGeneralData.Where(m => m.ApprovalStatus == 1).Where(m => m.MDBID == mdbid), "OANumber", "OANumber");
            return View();
        }

        //OA Date Update Dynamically on Oanumber Select
        [HttpGet]
        public JsonResult GetOAdate(string id)
        {
            var selectedRow = (from t in db.OAEquipGeneralData where t.OANumber == id select t).SingleOrDefault();

            var jsonData = new
            {
                OADate = selectedRow.Approvaldate.ToString(),
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //const int pageSize = 30;
        public ActionResult MachineDispatchView(string custnm = null, int page = 1, int sortBy = 1, bool isAsc = true, string macslno = null)
        {
            //IEnumerable<MachineDispatch> macdis = db.MachineDispatch.Where(p => custnm == null || p.MDBGeneralData.OrganizationName.Contains(custnm)).Where(m => m.IsDeleted ==100).Where(m => m.IsDispatched == 1000);
            //Paging and Sorting //
            //if (custnm != null || custnm !="")
            //{
            //     macdis = db.MachineDispatch.Where(p => custnm == null || p.MDBGeneralData.OrganizationName.Contains(custnm)).Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 1);
            //}
            //else if (macslno != null || macslno!="")
            //{
            //    macdis = db.MachineDispatch.Where(p => macslno == null || p.MachineInventory.MachineSerialNo.Contains(macslno)).Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 1);
            //}
            //else
            //{
            //    macdis = db.MachineDispatch.Where(p => custnm == null || p.MDBGeneralData.OrganizationName.Contains(custnm)).Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 1);
            //}

            IEnumerable<MachineDispatch> macdis = db.MachineDispatch.Where(p => custnm == null || p.MDBGeneralData.OrganizationName.Contains(custnm)).Where(p => macslno == null || p.MachineInventory.MachineSerialNo.Contains(macslno)).Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 1);
            #region Sorting
            switch (sortBy)
            {
                case 1:
                    macdis = isAsc ? macdis.OrderBy(p => p.MachineDispatchID) : macdis.OrderByDescending(p => p.MachineDispatchID);
                    break;

                case 2:
                    macdis = isAsc ? macdis.OrderBy(p => p.MachineInventory.MachineSerialNo) : macdis.OrderByDescending(p => p.MachineInventory.MachineSerialNo);
                    break;

                case 3:
                    macdis = isAsc ? macdis.OrderBy(p => p.OANumber) : macdis.OrderByDescending(p => p.OANumber);
                    break;

                case 4:
                    macdis = isAsc ? macdis.OrderBy(p => p.MDBGeneralData.OrganizationName) : macdis.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 5:
                    macdis = isAsc ? macdis.OrderBy(p => p.InvoiceNumber) : macdis.OrderByDescending(p => p.InvoiceNumber);
                    break;

                case 6:
                    macdis = isAsc ? macdis.OrderBy(p => p.InvoiceDate) : macdis.OrderByDescending(p => p.InvoiceDate);
                    break;

                case 7:
                    macdis = isAsc ? macdis.OrderBy(p => p.LRNumber) : macdis.OrderByDescending(p => p.LRNumber);
                    break;

                case 8:
                    macdis = isAsc ? macdis.OrderBy(p => p.DispatchDate) : macdis.OrderByDescending(p => p.DispatchDate);
                    break;

                case 9:
                    macdis = isAsc ? macdis.OrderBy(p => p.Transporter) : macdis.OrderByDescending(p => p.Transporter);
                    break;

                case 10:
                    macdis = isAsc ? macdis.OrderBy(p => p.CommissionDate) : macdis.OrderByDescending(p => p.CommissionDate);
                    break;

                default:
                    macdis = isAsc ? macdis.OrderBy(p => p.MachineDispatchID) : macdis.OrderByDescending(p => p.MachineDispatchID);
                    break;
            }
            #endregion

            return View(macdis);
        }

        public ActionResult ExportData(string custnm=null)
        {
            GridView gv = new GridView();
            if (custnm != null && custnm != "")
            {
                DataTable dts = new DataTable();
                dts.Columns.Add("Machine Serial No", typeof(String));
                dts.Columns.Add("Model No", typeof(String));
                dts.Columns.Add("Customer Name", typeof(String));
                dts.Columns.Add("Order Number", typeof(String));
                dts.Columns.Add("Invoice Number", typeof(String));
                dts.Columns.Add("Invoice Date", typeof(String));
                dts.Columns.Add("LR Number", typeof(String));
                dts.Columns.Add("Dispatch Date", typeof(String));
                dts.Columns.Add("Transporter", typeof(String));
                dts.Columns.Add("Channel Partner", typeof(String));

                //Machine serial no.
                //var macdet = db.MachineInventory.Where(m => m.MachineInventoryID==md.MachineInventoryID).Select(m=>m.CustomerName);

                var duplicate = (from s in db.MachineDispatch
                                 where s.MachineDispatchID != null
                                 select new
                                 {
                                     s.MachineInventory.MachineSerialNo,
                                     s.MachineInventory.ProductModel.ProductModelName,
                                     s.MDBGeneralData.OrganizationName,
                                     s.OANumber,
                                     s.InvoiceNumber,
                                     s.InvoiceDate,
                                     s.LRNumber,
                                     s.DispatchDate,
                                     s.Transporter,
                                     s.ChannelPartners.CPName,
                                 });

                var dt1 = duplicate.ToList();
                foreach (var d in duplicate)
                {
                    DataRow dr = dts.NewRow();
                    dr[0] = d.MachineSerialNo;
                    dr[1] = d.ProductModelName;
                    dr[2] = d.OrganizationName;
                    dr[3] = d.OANumber;
                    dr[4] = d.InvoiceNumber;
                    DateTime InvoiceDate = Convert.ToDateTime(d.InvoiceDate);
                    string InvoiceDate1 = InvoiceDate.ToString("dd-MM-yyyy");
                    dr[5] = InvoiceDate1;
                    dr[6] = d.LRNumber;
                    dr[7] = d.DispatchDate;
                    dr[8] = d.Transporter;
                    dr[9] = d.CPName;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
            }
            else
            {
                DataTable dts = new DataTable();
                dts.Columns.Add("Machine Serial No", typeof(String));
                dts.Columns.Add("Model No", typeof(String));
                dts.Columns.Add("Customer Name", typeof(String));
                dts.Columns.Add("Order Number", typeof(String));
                dts.Columns.Add("Invoice Number", typeof(String));
                dts.Columns.Add("Invoice Date", typeof(String));
                dts.Columns.Add("LR Number", typeof(String));
                dts.Columns.Add("Dispatch Date", typeof(String));
                dts.Columns.Add("Transporter", typeof(String));
                dts.Columns.Add("Channel Partner", typeof(String));

                //Machine serial no.
                //var macdet = db.MachineInventory.Where(m => m.MachineInventoryID==md.MachineInventoryID).Select(m=>m.CustomerName);

                var duplicate = (from s in db.MachineDispatch
                                 where s.MachineDispatchID != null
                                 select new
                                 {
                                     s.MachineInventory.MachineSerialNo,
                                     s.MachineInventory.ProductModel.ProductModelName,
                                     s.MDBGeneralData.OrganizationName,
                                     s.OANumber,
                                     s.InvoiceNumber,
                                     s.InvoiceDate,
                                     s.LRNumber,
                                     s.DispatchDate,
                                     s.Transporter,
                                     s.ChannelPartners.CPName,
                                 });

                var dt1 = duplicate.ToList();
                foreach (var d in duplicate)
                {
                    DataRow dr = dts.NewRow();

                    dr[0] = d.MachineSerialNo;
                    dr[1] = d.ProductModelName;
                    dr[2] = d.OrganizationName;
                    dr[3] = d.OANumber;
                    dr[4] = d.InvoiceNumber;
                    DateTime InvoiceDate = Convert.ToDateTime(d.InvoiceDate);
                    string InvoiceDate1 = InvoiceDate.ToString("dd-MM-yyyy");
                    dr[5] = InvoiceDate1;
                    dr[6] = d.LRNumber;
                    dr[7] = d.DispatchDate;
                    dr[8] = d.Transporter;
                    dr[9] = d.CPName;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
            }
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MachineDispatchView.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("MachineDispatchView");
        }

    }
}
