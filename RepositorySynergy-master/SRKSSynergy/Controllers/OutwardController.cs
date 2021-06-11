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

namespace SRKSSynergy.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OutwardController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json
        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var result = (from r in db.MDBGeneralData
                          where (r.OrganizationName.ToLower().Contains(term.ToLower()) && r.CPID == loginname.CPID) && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        ////Json AutoComplete customer name
        public JsonResult Autocompletecust(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var custnmae = db.OutwardSpare.Where(m => m.IsDeleted == 0).Select(m => m.CustomerName).FirstOrDefault();
            ViewBag.custname = custnmae;
            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower())
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemDescription(int id)
        {
            var selectedRow = (from t in db.ProductModelSpare where t.ProductModelSparesID == id select t).SingleOrDefault();

            var jsonData = new
            {

                presentstock = selectedRow.BuhlerPresentStock,
                unitprice = selectedRow.AgentPrice,
                Desc = selectedRow.ProductModelSparesDesc

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        ////[HttpGet]
        //public JsonResult GetCustomername(int id)
        //{
        //    var selectedRow1 = (from r in db.OutwardSpare where r.IsDeleted == 0 select r).SingleOrDefault();

        //    var jsonData = new
        //    {
        //        customername = selectedRow1.CustomerName
        //    };
        //    return Json(jsonData, JsonRequestBehavior.AllowGet);
        //}

        // GET: /Equipment/

        const int pageSize = 1000;
        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string custnm = null)
        {
            //Paging and Sorting //
            IEnumerable<OutwardSpare> productsname = db.OutwardSpare.Where(
                    p => custnm == null
                    || p.CustomerName.Contains(custnm)).Where(m => m.IsDeleted == 0).Where(m => m.Quantity != 0);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    productsname = isAsc ? productsname.OrderBy(p => p.OutwardMonth) : productsname.OrderByDescending(p => p.OutwardMonth);
                    break;

                case 2:
                    productsname = isAsc ? productsname.OrderBy(p => p.CustomerName) : productsname.OrderByDescending(p => p.CustomerName);
                    break;

                case 3:
                    productsname = isAsc ? productsname.OrderBy(p => p.OrderNo) : productsname.OrderByDescending(p => p.OrderNo);
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
                    productsname = isAsc ? productsname.OrderBy(p => p.InvoiceDate) : productsname.OrderByDescending(p => p.InvoiceDate);
                    break;
                case 9:
                    productsname = isAsc ? productsname.OrderBy(p => p.InvoiceNo) : productsname.OrderByDescending(p => p.InvoiceNo);
                    break;
                case 10:
                    productsname = isAsc ? productsname.OrderBy(p => p.Remarks) : productsname.OrderByDescending(p => p.Remarks);
                    break;
                default:
                    productsname = isAsc ? productsname.OrderBy(p => p.DispatchDate) : productsname.OrderByDescending(p => p.DispatchDate);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)productsname.Count() / pageSize);

            //productsname = productsname
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize;

            ViewBag.Search = custnm;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (custnm != null)
                ViewBag.IsSearch = true;
            return View(productsname);

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
        public ActionResult AddOutward()
        {
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOutward(Outward outw, int presentstock1 = 0, int presentstock2 = 0, int presentstock3 = 0, int presentstock4 = 0, int presentstock5 = 0)
        {
            if (outw.OutwardSpare1.CustomerName != null && outw.OutwardSpare1.OrderNo != null)
            {
                ViewBag.custname = outw.OutwardSpare1.CustomerName;
                var emptype = outw.OutwardSpare1.OutwardMonth.Substring(0, 3);
                //getting month number from month name
                int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                outw.OutwardSpare1.OutMonthNo = mon;
                string c1 = outw.OutwardSpare1.CustomerName;
                int cpid = db.MDBGeneralData.Where(m => m.OrganizationName == c1).Select(m => m.CPID).FirstOrDefault();
                outw.OutwardSpare1.CPID = cpid;
                outw.OutwardSpare1.QuantityOrdered = outw.OutwardSpare1.Quantity;
                db.OutwardSpare.Add(outw.OutwardSpare1);
                db.SaveChanges();
            }
            if (outw.OutwardSpare2.CustomerName != null && outw.OutwardSpare2.OrderNo != null && outw.OutwardSpare2.OutwardMonth != null && outw.OutwardSpare2.Quantity != null)
            {
                string c1 = outw.OutwardSpare2.CustomerName;
                var emptype = outw.OutwardSpare2.OutwardMonth.Substring(0, 3);
                //getting month number from month name
                int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                outw.OutwardSpare2.OutMonthNo = mon;
                int cpid = db.MDBGeneralData.Where(m => m.OrganizationName == c1).Select(m => m.CPID).FirstOrDefault();
                outw.OutwardSpare2.CPID = cpid;
                outw.OutwardSpare2.QuantityOrdered = outw.OutwardSpare2.Quantity;
                db.OutwardSpare.Add(outw.OutwardSpare2);
                db.SaveChanges();
            }
            if (outw.OutwardSpare3.CustomerName != null && outw.OutwardSpare3.OrderNo != null && outw.OutwardSpare3.OutwardMonth != null && outw.OutwardSpare3.Quantity != null)
            {
                string c1 = outw.OutwardSpare3.CustomerName;
                var emptype = outw.OutwardSpare3.OutwardMonth.Substring(0, 3);
                //getting month number from month name
                int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                outw.OutwardSpare3.OutMonthNo = mon;
                int cpid = db.MDBGeneralData.Where(m => m.OrganizationName == c1).Select(m => m.CPID).FirstOrDefault();
                outw.OutwardSpare3.CPID = cpid;
                outw.OutwardSpare3.QuantityOrdered = outw.OutwardSpare3.Quantity;
                db.OutwardSpare.Add(outw.OutwardSpare3);
                db.SaveChanges();
            }

            if (outw.OutwardSpare4.CustomerName != null && outw.OutwardSpare4.OrderNo != null && outw.OutwardSpare4.OutwardMonth != null && outw.OutwardSpare4.Quantity != null)
            {
                string c1 = outw.OutwardSpare4.CustomerName;
                var emptype = outw.OutwardSpare4.OutwardMonth.Substring(0, 3);
                //getting month number from month name
                int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                outw.OutwardSpare4.OutMonthNo = mon;
                int cpid = db.MDBGeneralData.Where(m => m.OrganizationName == c1).Select(m => m.CPID).FirstOrDefault();
                outw.OutwardSpare4.CPID = cpid;
                outw.OutwardSpare4.QuantityOrdered = outw.OutwardSpare4.Quantity;
                db.OutwardSpare.Add(outw.OutwardSpare4);
                db.SaveChanges();
            }

            if (outw.OutwardSpare5.CustomerName != null && outw.OutwardSpare5.OrderNo != null && outw.OutwardSpare5.OutwardMonth != null && outw.OutwardSpare5.Quantity != null)
            {
                string c1 = outw.OutwardSpare5.CustomerName;
                var emptype = outw.OutwardSpare5.OutwardMonth.Substring(0, 3);
                //getting month number from month name
                int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                outw.OutwardSpare5.OutMonthNo = mon;
                int cpid = db.MDBGeneralData.Where(m => m.OrganizationName == c1).Select(m => m.CPID).FirstOrDefault();
                outw.OutwardSpare5.CPID = cpid;
                outw.OutwardSpare5.QuantityOrdered = outw.OutwardSpare5.Quantity;
                db.OutwardSpare.Add(outw.OutwardSpare5);
                db.SaveChanges();
            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");

            return RedirectToAction("Index");

        }

        public ActionResult ModifyOutward(int id = 0)
        {
            OutwardSpare OutwardSpare = db.OutwardSpare.Find(id);
            if (OutwardSpare == null)
            {
                return HttpNotFound();
            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            var pdp = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardSpare.ProductModelSparesID).Select(m => new { m.ProductModelSparesDesc, m.AgentPrice }).SingleOrDefault();
            ViewBag.ItemDesc = pdp.ProductModelSparesDesc;
            ViewBag.Agentprice = pdp.AgentPrice;

            return View(OutwardSpare);
        }
        //
        // POST: /Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyOutward(OutwardSpare OutwardSpare)
        {
            int buhprsntstk = 0, qty1 = 0;
            if (ModelState.IsValid)
            {
                var getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardSpare.ProductModelSparesID)
                          .Select(m => new { m.BuhlerPresentStock, m.ProductModelSparesID }).FirstOrDefault();

                if (getstk.BuhlerPresentStock != 0)
                {

                    var inward = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardSpare.ProductModelSparesID).FirstOrDefault();
                    if (OutwardSpare.Quantity != null)
                    {
                        var quanout = OutwardSpare.Quantity;
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



                        if (qty1 == 0)
                            OutwardSpare.IsDispatch = 1;
                        OutwardSpare.Quantity = qty1;
                        db.Entry(OutwardSpare).State = EntityState.Modified;
                        db.SaveChanges();

                        //
                        //Emailing Dispatch details to Channel Partner
                        //
                        string mfrno = OutwardSpare.InvoiceNo;
                        var inv = OutwardSpare.InvoiceDate;
                        var invdt = Convert.ToDateTime(inv).ToString("dd-MM-yyyy");
                        int cpid = OutwardSpare.CPID;
                        var cpnm = db.CPContactPersonData.Where(m => m.CPID == cpid).Select(m => new { m.Title, m.FirstName, m.LastName, m.EmailID }).FirstOrDefault();
                        var chanm = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();

                        //var email = cpnm.EmailID;
                        //var name = cpnm.Title + " " + cpnm.FirstName + " " + cpnm.LastName;
                        //Channel Partner
                        int prdspare = OutwardSpare.ProductModelSparesID;
                        var outmfr = db.ProductModelSpare.Where(m => m.ProductModelSparesID == prdspare).Single();
                        var cusnm = OutwardSpare.CustomerName;
                        var itm = inward.ProductModelSparesName;
                        var desc = inward.ProductModelSparesDesc;
                        var qty = quanout1 - qty1;
                        var disdte = OutwardSpare.DispatchDate;
                        var dispdate = Convert.ToDateTime(disdte).ToString("dd-MM-yyyy");
                        var rmk = OutwardSpare.Remarks;

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
                            mail.To.Add(email);
                            mail.Subject = "Spare Dispatch Details";

                            //mail.CC.Add("srinivas.kulkarni@buhlergroup.com");
                            //for CC get spareadmin emails
                            var spareAdminEmailToCC = db.BuhlerAdminCC.Where(s => s.Type == "spareadmin").ToList();
                            foreach (var mc in spareAdminEmailToCC)
                            {
                                string spareadminemail = mc.EmailId;
                                mail.CC.Add(new MailAddress(spareadminemail));
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
                                            "<td class=\"auto-style2\" colspan=\"10\">Against Invoice Spare Dispatch Details</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class=\"auto-style6\">Invoice No</td>" +
                                            "<td class=\"auto-style6\">Channel Partner</td>" +
                                            "<td class=\"auto-style6\">Customer Name</td>" +
                                            "<td class=\"auto-style6\">PartCode <br />" +
                                            "</td>" +
                                            "<td class=\"auto-style6\">Description</td>" +
                                            "<td class=\"auto-style6\">Quantity<br />" +
                                            "Number</td>" +
                                            "<td class=\"auto-style6\">Invoice Date<br />" +
                                            "</td>" +
                                            "<td class=\"auto-style6\">Dispatch Date<br />" +
                                            "</td>" +
                                            "<td class=\"auto-style6\">Courier Details</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + mfrno + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + chanm + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + cusnm + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + itm + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + desc + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + qty + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + invdt + "</td>" +
                                            "<td class=\"auto-style5\" style=\"height: 30px\">" + dispdate + "</td>" +
                                           "<td class=\"auto-style5\" style=\"height: 30px\">" + rmk + "</td>" +
                                        "</tr>" +
                                    "</table>" +
                                    "<p>&nbsp;</p>" +
                                    "<p class=\"auto-style8\">Note :<span class=\"auto-style9\"> Please donot reply to this " +
                                    "Email ID, this is a Autogenerated Email</span></p>" +
                                    "</body>";

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
                        //                
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.NotInwarded = "This Particular Item is not inwarded";
                        ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                        return View(OutwardSpare);
                    }
                }
                else
                {
                    TempData["nostock"] = "There no stock available for the below Spare";
                    ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                    return View(OutwardSpare);
                }
            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return View(OutwardSpare);

            //if (ModelState.IsValid)
            //{
            //    var inward = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardSpare.ProductModelSparesID).FirstOrDefault();
            //    if (OutwardSpare.Quantity <= inward.BuhlerPresentStock)
            //    {
            //        //var quanin = inward.QuantityRemaining;
            //        var quanout = OutwardSpare.Quantity;
            //        //var quanrem = quanin - quanout;
            //       // var id = inward.InwardID;
            //        //InwardSpare InwardSpare = db.InwardSpare.Find(id);
            //        //InwardSpare.QuantityRemaining = quanrem;
            //        //db.Entry(InwardSpare).State = EntityState.Modified;
            //        //db.SaveChanges();

            //        var getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == OutwardSpare.ProductModelSparesID)
            //          .Select(m => new { m.BuhlerPresentStock, m.ProductModelSparesID }).FirstOrDefault();
            //        int buhprsntstk = getstk.BuhlerPresentStock - quanout;
            //        var prdmdlsprid = getstk.ProductModelSparesID;
            //        ProductModelSpare prdmdlspr = db.ProductModelSpare.Find(prdmdlsprid);
            //        prdmdlspr.BuhlerPresentStock = buhprsntstk;
            //        db.Entry(prdmdlspr).State = EntityState.Modified;
            //        db.SaveChanges();
            //       // OutwardSpare.IsDispatch = 1;
            //        db.Entry(OutwardSpare).State = EntityState.Modified;
            //        db.SaveChanges();


            //        return RedirectToAction("Index");
            //    }
            //    else
            //    {
            //        ViewBag.NotInwarded = "This Particular Item is not inwarded";
            //        ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            //        return View(OutwardSpare);
            //    }



            //}
            //ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            //return View(OutwardSpare);
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
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
    }
}