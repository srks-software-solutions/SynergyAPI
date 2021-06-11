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
using System.Web.Services;

namespace SRKSSynergy.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OutwardMFRController : Controller
    {

        private SRKS_Synergy db = new SRKS_Synergy();

        //For Payment Type
        public static SelectList YesOrNoDropdownList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "1", Text = "Yes" });
            list.Add(new SelectListItem { Value = "0", Text = "No" });
            return new SelectList(list, "Value", "Text");
        }

        //
        // GET: /OutwardMFR/

        //Json
        public JsonResult Autocomplete(string term)
        {
            var result = (from r in db.ProductModel
                          where r.ProductModelName.ToLower().Contains(term.ToLower())
                          select new { r.ProductModelName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete customer name
        public JsonResult Autocompletecust(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower())
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Equipment/
        //const int pageSize = 10;
        //[Authorize(Roles = "Administrator,ChannelPartnerAdmin")]
        [AllowAnonymous]
        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = false, string equipname = null,string Isdisp=null)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("ChannelPartnerAdmin"))
            {
                string logcpid = Session["logincpid"].ToString();
                int cpid = Convert.ToInt32(logcpid);
                string isdisp = Isdisp;
                IEnumerable<OutwardMFR> productsname = null;
                if (isdisp == null || isdisp =="")
                {
                    if (cpid == 0)//admin
                    {
                        productsname = db.OutwardMFR.Where(
                          p => equipname == null
                          || p.CustomerName.Contains(equipname))
                      .Where(m => m.IsDeleted == 0);//.Where(m => m.IsDispatch != 1).Where(m => m.Quantity != 0)
                    }
                    else
                    {//channel partners
                        productsname = db.OutwardMFR.Where(
                           p => equipname == null
                           || p.CustomerName.Contains(equipname))
                       .Where(m => m.IsDeleted == 0).Where(m => m.CPID == cpid);//.Where(m => m.IsDispatch != 1).Where(m => m.Quantity != 0)

                    }
                    }
                else
                {
                    int disp = Convert.ToInt32(isdisp);
                    productsname = db.OutwardMFR.Where(
                        p => equipname == null
                        || p.CustomerName.Contains(equipname)).Where(m => m.IsDispatch == disp)
                    .Where(m => m.IsDeleted == 0).Where(m=>m.CPID==cpid);//.Where(m => m.Quantity != 0)
                }
                //Paging and Sorting //


                #region Sorting
                switch (sortBy)
                {
                    case 1:
                        productsname = isAsc ? productsname.OrderBy(p => p.MFRId) : productsname.OrderByDescending(p => p.MFRId);
                        break;

                    case 2:
                        productsname = isAsc ? productsname.OrderBy(p => p.CustomerName) : productsname.OrderByDescending(p => p.CustomerName);
                        break;

                    case 3:
                        productsname = isAsc ? productsname.OrderBy(p => p.MFRNo) : productsname.OrderByDescending(p => p.MFRNo);
                        break;

                    case 4:
                        productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSpare.ProductModelSparesName) : productsname.OrderByDescending(p => p.ProductModelSpare.ProductModelSparesName);
                        break;

                    case 5:
                        productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSpare.ProductModelSparesID) : productsname.OrderByDescending(p => p.ProductModelSpare.ProductModelSparesID);
                        break;

                    case 6:
                        productsname = isAsc ? productsname.OrderBy(p => p.Quantity) : productsname.OrderByDescending(p => p.Quantity);
                        break;

                    case 7:
                        productsname = isAsc ? productsname.OrderBy(p => p.TotalValue) : productsname.OrderByDescending(p => p.TotalValue);
                        break;

                    case 8:
                        productsname = isAsc ? productsname.OrderBy(p => p.Remarks) : productsname.OrderByDescending(p => p.Remarks);
                        break;

                    default:
                        productsname = isAsc ? productsname.OrderBy(p => p.DispatchDate) : productsname.OrderByDescending(p => p.DispatchDate);
                        break;
                }
                #endregion


                //ViewBag.TotalPages = (int)Math.Ceiling((double)productsname.Count() / pageSize);

                //productsname = productsname
                //    .Skip((page - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToList();

                //ViewBag.CurrentPage = page;
                //ViewBag.PageSize = pageSize;

                ViewBag.Search = equipname;

                ViewBag.SortBy = sortBy;
                ViewBag.IsAsc = isAsc;
                if (equipname != null)
                    ViewBag.IsSearch = true;
                return View(productsname);
                //end of paging and sorting//
                //return View(db.ProductModel.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        //
        // GET: /Equipment/Details/5
        public ActionResult Details(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }


        //[HttpGet]
        public ActionResult AddMFR()
        {
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMFR(OutwardMFR OutwardMFR)
        {
            //var duplicate = (from s in db.ProductModel
            //                 where s.ProductModelName == productmodel.ProductModelName && s.ProductID == productmodel.ProductID
            //                 select s).ToList();

            if (OutwardMFR.CustomerName != null && OutwardMFR.MFRNo != null)
            {
                //if (duplicate.Count > 0)
                //{
                //    ViewBag.Duplicate = "Equipment Name already exists for this Commodity";
                //}

                //var inward = db.InwardSpare.Where(m => m.ProductModelSparesID == OutwardMFR.ProductModelSparesID).FirstOrDefault();
                //if (inward != null)
                //{
                //    var quanin = inward.QuantityRemaining;
                //    var quanout = OutwardMFR.Quantity;
                //    if (quanin > quanout)
                //    {
                //        var quanrem = quanin - quanout;
                //        var id = inward.InwardID;
                //        InwardSpare InwardSpare = db.InwardSpare.Find(id);
                //        InwardSpare.QuantityRemaining = quanrem;
                //        db.Entry(InwardSpare).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                var emptype = OutwardMFR.OutwardMonth.Substring(0, 3);
                //getting month number from month name
                int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                OutwardMFR.OutMonthNo = mon;
                db.OutwardMFR.Add(OutwardMFR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return View(OutwardMFR);
        }

        //
        // GET: /Equipment/Edit/5
        public ActionResult ModifyMFR(int id = 0)
        {
            OutwardMFR OutwardMFR = db.OutwardMFR.Find(id);
            if (OutwardMFR == null)
            {
                return HttpNotFound();
            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            var pdp = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardMFR.ProductModelSparesID).Select(m => new { m.ProductModelSparesDesc, m.AgentPrice }).SingleOrDefault();
            ViewBag.ItemDesc = pdp.ProductModelSparesDesc;
            ViewBag.Agentprice = pdp.AgentPrice;

            return View(OutwardMFR);
        }

        //
        // POST: /Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyMFR(OutwardMFR OutwardMFR)
        {
            int buhprsntstk = 0, qty1 = 0;
            if (ModelState.IsValid)
            {
                var getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardMFR.ProductModelSparesID)
                          .Select(m => new { m.BuhlerPresentStock, m.ProductModelSparesID }).FirstOrDefault();

                if (getstk.BuhlerPresentStock != 0)
                {

                    var inward = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardMFR.ProductModelSparesID).FirstOrDefault();
                    if (OutwardMFR.Quantity != null)
                    {
                        var quanout = OutwardMFR.Quantity;
                        int quanout1 = quanout;


                        if (quanout > getstk.BuhlerPresentStock)
                        {
                            //buhprsntstk = quanout - getstk.BuhlerPresentStock;
                            buhprsntstk = 0;
                            qty1 = quanout - getstk.BuhlerPresentStock;
                        }
                        else
                        {
                            buhprsntstk = getstk.BuhlerPresentStock - quanout;
                            qty1 = 0;
                        }

                        var prdmdlsprid = getstk.ProductModelSparesID;
                        ProductModelSpare prdmdlspr = db.ProductModelSpare.Find(prdmdlsprid);
                        prdmdlspr.BuhlerPresentStock = buhprsntstk;
                        db.Entry(prdmdlspr).State = EntityState.Modified;
                        db.SaveChanges();

                        var qty = quanout1 - qty1;

                        if (qty1 == 0)
                            OutwardMFR.IsDispatch = 1;
                        //OutwardMFR.QuantityOrdered = quanout1;
                        OutwardMFR.Quantity = qty1; // old working
                        db.Entry(OutwardMFR).State = EntityState.Modified;
                        db.SaveChanges();

                        //
                        //Emailing Dispatch details to Channel Partner
                        //
                        string mfrno = OutwardMFR.MFRNo;
                        var cpd = db.MFR.Where(m => m.MFRNumber == mfrno).Select(m => m.CPID).Single();
                        int cpid = Convert.ToInt32(cpd);
                        var cpnm = db.CPContactPersonData.Where(m => m.CPID == cpid).Select(m => new { m.Title, m.FirstName, m.LastName, m.EmailID }).FirstOrDefault();

                        //var email = cpnm.EmailID;
                        //var name = cpnm.Title + " " + cpnm.FirstName + " " + cpnm.LastName;
                        //Channel Partner
                        int prdspare = OutwardMFR.ProductModelSparesID;
                        var outmfr = db.ProductModelSpare.Where(m => m.ProductModelSparesID == prdspare).Single();
                        var cusnm = OutwardMFR.CustomerName;
                        var itm = inward.ProductModelSparesName;
                        var desc = inward.ProductModelSparesDesc;

                        var disdte = OutwardMFR.DispatchDate;
                        var dispdate = Convert.ToDateTime(disdte).ToString("dd-MM-yyyy");
                        var rmk = OutwardMFR.Remarks;
                        string frommail = null;
                        string mailpass = null;
                        //to fetch FROM mail credentials
                        var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m=>m.MSID==1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
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
                            mail.To.Add(email);
                            var spareAdminEmailToCC = db.BuhlerAdminCC.ToList();
                            foreach (var mc in spareAdminEmailToCC)
                            {
                                string spareadminemail = mc.EmailId;
                                mail.CC.Add(new MailAddress(spareadminemail));
                            }
                            //mail.To.Add("shankar.chandrashekhar@buhlergroup.com");
                            //mail.To.Add("praveen.shivanna@buhlergroup.com");
                           // mail.To.Add("suhas.cm@srkssolutions.com");


                            // mail.CC.Add("srinivas.kulkarni@buhlergroup.com");
                            //for CC get spareadmin emails

                            mail.Subject = "Spare Dispatch Details";
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
                                    "<p>Dear " + name + ",</p>" +
                                    "<p>Please find the below mail for Spare Dispatch Details</p>" +
                                    "<p>&nbsp;</p>" +
                                    "<table class=\"auto-style1\" style=\"width: 100%; height: 30px\">" +
                                        "<tr>" +
                                            "<td class=\"auto-style2\" colspan=\"10\">Spare Dispatch Details</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class=\"auto-style6\">MFR No</td>" +
                                            "<td class=\"auto-style6\">Channel Partner</td>" +
                                            "<td class=\"auto-style6\">Customer Name</td>" +
                                            "<td class=\"auto-style6\">PartCode <br />" +
                                            "</td>" +
                                            "<td class=\"auto-style6\">Description</td>" +
                                            "<td class=\"auto-style6\">Quantity<br />" +
                                            "Number</td>" +
                                            "<td class=\"auto-style6\">Dispatch Date<br />" +
                                            "</td>" +
                                            "<td class=\"auto-style6\">Courier Details</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + mfrno + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + name + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + cusnm + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + itm + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + desc + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + qty + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + dispdate + "</td>" +
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
                            SmtpServer.Credentials = new System.Net.NetworkCredential(frommail,mailpass);
                            //SmtpServer.Credentials = new System.Net.NetworkCredential("bbandispatches@gmail.com", "bban2014");
                            //SmtpServer.Credentials = new System.Net.NetworkCredential("janardhangrd@gmail.com", "j2n26dh2n");
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.ToString());
                        }
                        //
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.NotInwarded = "This Particular Item is not inwarded";
                        ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                        return View(OutwardMFR);
                    }
                }
                else
                {
                    TempData["nostock"] = "There no stock available for the below Spare";
                    ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(OutwardMFR);
                }
            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return View(OutwardMFR);
        }

        public ActionResult MFRView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MFRView(int id = 0)
        {
            return View();
        }

        //
        // GET: /Equipment/Delete/5
        public ActionResult Delete(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }

        //
        // POST: /Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            db.ProductModel.Remove(productmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        //
        //Discontinue Equipment
        public ActionResult Discontinue(int id)
        {
            var deac = db.ProductModel.Where(m => m.ProductModelID == id).Where(m => m.IsDeleted == 0).Single();
            return View(deac);
        }

        //
        //Post: Discontinue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discontinue(ProductModel prdmd)
        {
            if (Request.Form["Yes"] != null)
            {
                prdmd.IsDeleted = 1;
                db.Entry(prdmd).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        //
        [AllowAnonymous]
        [HttpGet]
        public ActionResult SparesRecieved(int id)
        {
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                //ViewData["YesNo"] = new SelectList((IEnumerable<SelectListItem>)YesOrNoDropdownList().ToList(), "Value", "Text");//new SelectList(YesOrNoDropdownList(), "Value", "Text");
                var obj = db.OutwardMFR.Find(id);
                return View(obj);
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SparesRecieved(OutwardMFR oMFR,string Submit)
        {
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                if (Submit == "Submit")
                {
                    var objoMFR = db.OutwardMFR.Find(oMFR.MFRId);
                    objoMFR.IsSparesRecieved = oMFR.IsSparesRecieved;
                    objoMFR.SparesRecievedDate = oMFR.SparesRecievedDate;
                    db.Entry(objoMFR).State = EntityState.Modified;
                    db.SaveChanges();

                    #region mail module

                    string spareRcvd = "Yes";
                    //if (oMFR.IsSparesRecieved == 1)
                    //{
                    //    spareRcvd = "Yes";
                    //}
                    //else if (oMFR.IsSparesRecieved == 0)
                    //{
                    //    spareRcvd = "No";
                    //}

                    //for mail data:
                    var mfr = db.MFR.Where(m => m.MFRNumber == oMFR.MFRNo).FirstOrDefault();
                    var custObj = db.MDBGeneralData.Find(mfr.MDBID);
                    var cpObj = db.ChannelPartners.Find(mfr.CPID);
                    var oanumber = db.Handover.Where(m => m.HODNumber == mfr.OrderNum).Single();

                    //Auto Mail after a Shift Report Generation
                    #region
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    //Attachment attach = new Attachment(excelpath);
                    string frommail = "", mailpass="";
                    var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m => m.MSID == 1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
                    if (from != null)
                    {
                        frommail = from.FromMail;
                        mailpass = from.Password;
                    }
                    var maildata =

                    mail.From = new MailAddress(frommail); //("atcqualityteam@gmail.com");
                    //mail.To.Add("suhas.cm@srkssolutions.com");
                   // mail.CC.Add("sharath.krishna@srkssolutions.com");
                    var spareAdminEmailToCC = db.BuhlerAdminCC.ToList();
                    foreach (var mc in spareAdminEmailToCC)
                    {
                        string spareadminemail = mc.EmailId;
                        mail.To.Add(new MailAddress(spareadminemail));
                    }
                    mail.CC.Add(new MailAddress(cpObj.Email));


                    mail.Subject = "MFR Spare Recieved Details";
                    mail.Body = "<p><b>Dear All,</b></p>" +
                     "<b></b>" +
                     "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please find the MFR Generation status below&nbsp;" + ".</b></p>"

                     +
                         "<style>" +
                        "table { border-collapse: collapse; } table, td, th { border: 1px solid black; } td,th { left:auto; }" +
                        "</style>"
                     +

                     //data table
                     "<table style='width:50%'>" +
                        //"<tr> <th align='left'>Date</th> " + "<td>" + System.DateTime.Now.ToString("dd MMM yyyy") + "</td> </tr>" +
                     "<tr> <th align='left'>MFR No</th> " + "<td>" + mfr.MFRNumber + "</td> </tr>" +
                     "<tr> <th align='left'>Customer Name</th>  " + "<td>" + custObj.OrganizationName + "</td> </tr>" +
                     "<tr> <th align='left'>MFR Date</th> " + "<td>" + mfr.MfrDate + "</td> </tr>" +
                     "<tr> <th align='left'>Channel Partner Name</th>" + "<td>" + cpObj.CPName + "</td> </tr>" +
                     "<tr> <th align='left'>Model</th>" + "<td>" + oanumber.modelno + "</td> </tr>" +
                     "<tr> <th align='left'>Model Sl. No.</th>" + "<td>" + oanumber.MacSlNo + "</td> </tr>" +
                     "<tr> <th align='left'>Commission Date</th>" + "<td>" + mfr.CommissionedDate + "</td> </tr>" +
                     "<tr> <th align='left'>Fault 1</th>" + "<td>" + mfr.Fault + "</td> </tr>" +

                     "<tr> <th align='left'>Status</th>" + "<td> Approved </td> </tr>" +
                        //"<tr> <th align='left'>Description</th>" + "<td> Spares Instock </td> </tr>" +
                     "<tr> <th align='left'>Remarks</th>" + "<td>" + mfr.MFRComment + "</td> </tr>" +

                     "<tr> <th align='left'>Item Code</th>" + "<td>" + objoMFR.ProductModelSpare.ProductModelSparesName + "</td> </tr>" +
                     "<tr> <th align='left'>Spare Model Descriptor</th>" + "<td>" + objoMFR.ProductModelSpare.ProductModelSparesDesc + "</td> </tr>" +

                     "<tr> <th align='left'>Spare Recieving Date</th>" + "<td>" + objoMFR.SparesRecievedDate.Value.ToShortDateString() + "</td> </tr>" +
                     "<tr> <th align='left'>Is Spare Recieved</th>" + "<td>" + spareRcvd + "</td> </tr>" +


                     "</table>"

                    + "</b>" +
                    "<p><font><span style=\"font-family:arial,helvetica,sans-serif\"><span style=\"color:rgb(11,83,148)\"><span style=\"background-color:rgb(255,255,255)\"><i><span><font size=\"2\"><span style=\"font-family:comic sans ms,sans-serif\">“ Automatic System generated Mail and No incoming mail facility is available” </span></font></span></i><b><span><br>" +
                    "</span></b></span></span></span></font></p>";
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(attach);
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(frommail, mailpass);   //("atcqualityteam@gmail.com", "atc635126");
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    #endregion

                    #endregion mail module

                    return RedirectToAction("Index");
                }
                else if (Submit == "Back")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View();
            }
        }


        //
        [AllowAnonymous]
        [HttpGet]
        public ActionResult MachineIssue(int id)
        {
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                var obj = db.OutwardMFR.Find(id);
                return View(obj);
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult MachineIssue(OutwardMFR oMFR, string Submit)
        {
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                if (Submit == "Submit")
                {
                    var objoMFR = db.OutwardMFR.Find(oMFR.MFRId);
                    objoMFR.IsMachineIssueOpen = oMFR.IsMachineIssueOpen;
                    objoMFR.MachineIssueDescription = oMFR.MachineIssueDescription;
                    objoMFR.MachineIssueDate = oMFR.MachineIssueDate;
                    db.Entry(objoMFR).State = EntityState.Modified;
                    db.SaveChanges();

                    string mchnIssue = "";
                    if (oMFR.IsMachineIssueOpen == 1)
                    {
                        mchnIssue = "Open";
                    }
                    else if (oMFR.IsMachineIssueOpen == 0)
                    {
                        mchnIssue = "Close";
                    }

                    #region mail module

                    //for mail data:
                    var mfr = db.MFR.Where(m => m.MFRNumber == oMFR.MFRNo).FirstOrDefault();
                    var custObj = db.MDBGeneralData.Find(mfr.MDBID);
                    var cpObj = db.ChannelPartners.Find(mfr.CPID);
                    var oanumber = db.Handover.Where(m => m.HODNumber == mfr.OrderNum).Single();

                    //Auto Mail after a Shift Report Generation
                    #region
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    //Attachment attach = new Attachment(excelpath);

                    string frommail = "", mailpass = "";
                    var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m => m.MSID == 1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
                    if (from != null)
                    {
                        frommail = from.FromMail;
                        mailpass = from.Password;
                    }
                    var maildata =

                    mail.From = new MailAddress(frommail);
                    var spareAdminEmailToCC = db.BuhlerAdminCC.ToList();
                   // mail.To.Add(new MailAddress("suhas.cm@srkssolutions.com"));
                    foreach (var mc in spareAdminEmailToCC)
                    {
                        string spareadminemail = mc.EmailId;
                        mail.To.Add(new MailAddress(spareadminemail));
                    }
                    mail.CC.Add(new MailAddress(cpObj.Email));
                    //mail.To.Add("suhas.cm@srkssolutions.com");
                    //mail.CC.Add("sharath.krishna@srkssolutions.com");

                    mail.Subject = "MFR Machine Issue Details";
                    mail.Body = "<p><b>Dear All,</b></p>" +
                     "<b></b>" +
                     "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please find the MFR Generation status below&nbsp;" + ".</b></p>"

                     +
                         "<style>" +
                        "table { border-collapse: collapse; } table, td, th { border: 1px solid black; } td,th { left:auto; }" +
                        "</style>"
                     +

                     //data table
                     "<table style='width:50%'>" +
                        //"<tr> <th align='left'>Date</th> " + "<td>" + System.DateTime.Now.ToString("dd MMM yyyy") + "</td> </tr>" +
                     "<tr> <th align='left'>MFR No</th> " + "<td>" + mfr.MFRNumber + "</td> </tr>" +
                     "<tr> <th align='left'>Customer Name</th>  " + "<td>" + custObj.OrganizationName + "</td> </tr>" +
                     "<tr> <th align='left'>MFR Date</th> " + "<td>" + mfr.MfrDate + "</td> </tr>" +
                     "<tr> <th align='left'>Channel Partner Name</th>" + "<td>" + cpObj.CPName + "</td> </tr>" +
                     "<tr> <th align='left'>Model</th>" + "<td>" + oanumber.modelno + "</td> </tr>" +
                     "<tr> <th align='left'>Model Sl. No.</th>" + "<td>" + oanumber.MacSlNo + "</td> </tr>" +
                     "<tr> <th align='left'>Commission Date</th>" + "<td>" + mfr.CommissionedDate + "</td> </tr>" +
                     "<tr> <th align='left'>Fault 1</th>" + "<td>" + mfr.Fault + "</td> </tr>" +

                     "<tr> <th align='left'>Status</th>" + "<td> Approved </td> </tr>" +
                        //"<tr> <th align='left'>Description</th>" + "<td> Spares Instock </td> </tr>" +
                     "<tr> <th align='left'>Remarks</th>" + "<td>" + mfr.MFRComment + "</td> </tr>" +

                     "<tr> <th align='left'>Item Code</th>" + "<td>" + objoMFR.ProductModelSpare.ProductModelSparesName + "</td> </tr>" +
                     "<tr> <th align='left'>Spare Model Descriptor</th>" + "<td>" + objoMFR.ProductModelSpare.ProductModelSparesDesc + "</td> </tr>" +

                     "<tr> <th align='left'>Machine IssueDate Date</th>" + "<td>" + objoMFR.MachineIssueDate.Value.ToShortDateString() + "</td> </tr>" +
                     "<tr> <th align='left'>Machine IssueDate Description</th>" + "<td>" + objoMFR.MachineIssueDescription + "</td> </tr>" +
                     "<tr> <th align='left'>Machine Issue Status</th>" + "<td>" + mchnIssue + "</td> </tr>" +


                     "</table>"

                    + "</b>" +
                    "<p><font><span style=\"font-family:arial,helvetica,sans-serif\"><span style=\"color:rgb(11,83,148)\"><span style=\"background-color:rgb(255,255,255)\"><i><span><font size=\"2\"><span style=\"font-family:comic sans ms,sans-serif\">“ Automatic System generated Mail and No incoming mail facility is available” </span></font></span></i><b><span><br>" +
                    "</span></b></span></span></span></font></p>";
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(attach);
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    #endregion

                    #endregion mail module

                    return RedirectToAction("Index");
                }
                else if (Submit == "Back")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View();
            }
        }


        //
        [AllowAnonymous]
        [HttpGet]
        public ActionResult FaultSpareDispatch(int id)
        {
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                var obj = db.OutwardMFR.Find(id);
                return View(obj);
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult FaultSpareDispatch(OutwardMFR oMFR, string Submit)
        {
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                if (Submit == "Submit")
                {
                    var objoMFR = db.OutwardMFR.Find(oMFR.MFRId);
                    objoMFR.FaultSpareDate = oMFR.FaultSpareDate;
                    objoMFR.FaultSpareDescription = oMFR.FaultSpareDescription;
                    db.Entry(objoMFR).State = EntityState.Modified;
                    db.SaveChanges();
                    #region mail module

                    //for mail data:
                    var mfr = db.MFR.Where(m => m.MFRNumber == oMFR.MFRNo).FirstOrDefault();
                    var custObj = db.MDBGeneralData.Find(mfr.MDBID);
                    var cpObj = db.ChannelPartners.Find(mfr.CPID);
                    var oanumber = db.Handover.Where(m => m.HODNumber == mfr.OrderNum).Single();

                    //Auto Mail after a Shift Report Generation
                    #region
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    //Attachment attach = new Attachment(excelpath);
                    string frommail = "", mailpass = "";
                    var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m => m.MSID == 1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
                    if (from != null)
                    {
                        frommail = from.FromMail;
                        mailpass = from.Password;
                    }
                    var maildata =

                    mail.From = new MailAddress(frommail);
                    var spareAdminEmailToCC = db.BuhlerAdminCC.ToList();
                    foreach (var mc in spareAdminEmailToCC)
                    {
                        string spareadminemail = mc.EmailId;
                        mail.To.Add(new MailAddress(spareadminemail));
                    }
                    mail.CC.Add(new MailAddress(cpObj.Email));
                    //mail.To.Add("suhas.cm@srkssolutions.com");
                    //mail.CC.Add("sharath.krishna@srkssolutions.com");

                    mail.Subject = "MFR Fault Spare Dispatch Details";
                    mail.Body = "<p><b>Dear All,</b></p>" +
                     "<b></b>" +
                     "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please find the MFR Generation status below&nbsp;" + ".</b></p>"

                     +
                         "<style>" +
                        "table { border-collapse: collapse; } table, td, th { border: 1px solid black; } td,th { left:auto; }" +
                        "</style>"
                     +

                     //data table
                     "<table style='width:50%'>" +
                        //"<tr> <th align='left'>Date</th> " + "<td>" + System.DateTime.Now.ToString("dd MMM yyyy") + "</td> </tr>" +
                     "<tr> <th align='left'>MFR No</th> " + "<td>" + mfr.MFRNumber + "</td> </tr>" +
                     "<tr> <th align='left'>Customer Name</th>  " + "<td>" + custObj.OrganizationName + "</td> </tr>" +
                     "<tr> <th align='left'>MFR Date</th> " + "<td>" + mfr.MfrDate + "</td> </tr>" +
                     "<tr> <th align='left'>Channel Partner Name</th>" + "<td>" + cpObj.CPName + "</td> </tr>" +
                     "<tr> <th align='left'>Model</th>" + "<td>" + oanumber.modelno + "</td> </tr>" +
                     "<tr> <th align='left'>Model Sl. No.</th>" + "<td>" + oanumber.MacSlNo + "</td> </tr>" +
                     "<tr> <th align='left'>Commission Date</th>" + "<td>" + mfr.CommissionedDate + "</td> </tr>" +
                     "<tr> <th align='left'>Fault 1</th>" + "<td>" + mfr.Fault + "</td> </tr>" +

                     "<tr> <th align='left'>Status</th>" + "<td> Approved </td> </tr>" +
                        //"<tr> <th align='left'>Description</th>" + "<td> Spares Instock </td> </tr>" +
                     "<tr> <th align='left'>Remarks</th>" + "<td>" + mfr.MFRComment + "</td> </tr>" +

                     "<tr> <th align='left'>Item Code</th>" + "<td>" + objoMFR.ProductModelSpare.ProductModelSparesName + "</td> </tr>" +
                     "<tr> <th align='left'>Spare Model Descriptor</th>" + "<td>" + objoMFR.ProductModelSpare.ProductModelSparesDesc + "</td> </tr>" +

                     "<tr> <th align='left'>Fault Spare Dispatch Date</th>" + "<td>" + objoMFR.FaultSpareDate.Value.ToShortDateString() + "</td> </tr>" +
                     "<tr> <th align='left'>Fault Spare Description</th>" + "<td>" + objoMFR.FaultSpareDescription + "</td> </tr>" +

                     "</table>"

                    + "</b>" +
                    "<p><font><span style=\"font-family:arial,helvetica,sans-serif\"><span style=\"color:rgb(11,83,148)\"><span style=\"background-color:rgb(255,255,255)\"><i><span><font size=\"2\"><span style=\"font-family:comic sans ms,sans-serif\">“ Automatic System generated Mail and No incoming mail facility is available” </span></font></span></i><b><span><br>" +
                    "</span></b></span></span></span></font></p>";
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(attach);
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(frommail,mailpass);
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                    #endregion

                    #endregion mail module


                    return RedirectToAction("Index");
                }
                else if (Submit == "Back")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public void updateMFRstatus(string id)
        {
            string MFRid = id;
            int sid = 2;
            if(MFRid != "" || MFRid != null)
            {
            OutwardMFR outMFR =db.OutwardMFR.Find(Convert.ToInt32( MFRid));
            outMFR.IsDispatch = sid;
            outMFR.FaultSpareRecivedDateAdmin = DateTime.Now;
            db.Entry(outMFR).State = EntityState.Modified;
            db.SaveChanges();
            }
        }


        [HttpPost]
        public void updateremark(string id)
        {
            string MFRid = id;
            string remark = "";
            if (MFRid != "" || MFRid != null)
            {
                var outMFR = db.OutwardMFR.Find(Convert.ToInt32(MFRid));
                 remark = outMFR.Remarks;
            }
            //return Json(remark, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


