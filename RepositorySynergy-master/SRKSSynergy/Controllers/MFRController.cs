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
using System.Globalization;
using System.Net.Mail;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class MFRController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocompletemfr(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.Handover
                          where r.HODNumber.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          select new { r.HODNumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult Autocompletecp(string term)
        {
            var channame = db.ChannelPartners.Where(m => m.CPName == term).Select(m => m.CPID).SingleOrDefault();

            var result = (from r in db.ChannelPartners
                          where r.CPName.ToLower().Contains(term.ToLower())
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete Customer name
        public JsonResult Autocompleteexiscust(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          //&& (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        //Generate the HOD number
        public string mfrnumber()
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String mfrnumber;
            var quotmod = from MFRNumber in db.MFR
                          select MFRNumber;
            quotmod = quotmod.OrderByDescending(m => m.MFRNumber);
            var check = quotmod.FirstOrDefault();
            if (check == null)
            {
                mfrnumber = "MFR-" + "RC" + year + "-00001-00";
            }
            else
            {
                var quotval = quotmod.Select(m => m.MFRNumber).First();
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
                    mfrnumber = quotval;
                }
                else
                {
                    mfrnumber = "MFR-" + "RC" + year + "-00001-00";
                }
            }
            return mfrnumber;
        }

        //Update Quotation Number in the New Quotation View
        [HttpGet]
        public JsonResult Getmfrnumber(string oanum)
        {
            if (oanum != null && oanum != "")
            {
                ViewBag.MfrNumber = mfrnumber();

                var jsonData = new
                {
                    quotnumb = mfrnumber()
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
        // GET: /MFR/MFRGenerate
        //const int pageSize3 = 10;
        //bool getdetailsclick3 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult MFRGenerate(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();


            IEnumerable<Handover> quotations = db.Handover.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.CPID == loginname.CPID);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.HID) : quotations.OrderByDescending(p => p.HID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.HODNumber) : quotations.OrderByDescending(p => p.HODNumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAnum) : quotations.OrderByDescending(p => p.OAnum);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.modelno) : quotations.OrderByDescending(p => p.modelno);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.MacSlNo) : quotations.OrderByDescending(p => p.MacSlNo);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.Handeddate) : quotations.OrderByDescending(p => p.Handeddate);
                    break;

                //case 7:
                //    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.ApprovalStatus);
                //    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.HID) : quotations.OrderByDescending(p => p.HID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize3);

            //quotations = quotations
            //    .Skip((page - 1) * pageSize3)
            //    .Take(pageSize3)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize3;

            //ViewBag.Search = cunam;

            //ViewBag.SortBy = sortBy;
            //ViewBag.IsAsc = isAsc;

            //if (getdetailsclick3)
            //    ViewBag.IsSearch = true;
            //if (cunam != null)
            //{
            //    getdetailsclick3 = true;
            //    ViewBag.IsSearch = true;
            //}
            //ViewBag.NullError = false;
            //ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.HID).ToList());
        }

        //
        // GET: /MFR/
        public ActionResult MFR(string oanum)
        {
            if (oanum == null || oanum == "")
            {
               return  RedirectToAction("MFRGenerate");
            }
            else {
                // here oanum = Hod Number
                List<SelectListItem> listItems1 = new List<SelectListItem>();
                listItems1.Add(new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });
                ViewBag.Product = listItems1;
                //Getting Login Name and Channel partner name
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                //Channel Partner
                var channm = db.ChannelPartners.Where(m => m.CPID == loginname.CPID).Select(m => m.CPName).Single();
                ViewBag.CPName = channm;
                //Order Number
                //QGID->MDB->Company Name, Contact person data, Phone Number & City
                if (oanum != null && oanum != "")
                {
                    ViewBag.MfrNumber = mfrnumber();
                    ViewBag.HodNumber = oanum;
                    var oanumber = db.Handover.Where(m => m.HODNumber == oanum).Select(m => new { m.OAnum, m.modelno, m.Handeddate, m.MacSlNo }).Single();

                    var oacnt = db.OAEquipGeneralData.Where(m => m.OANumber == oanumber.OAnum).Where(m => m.CPID == loginname.CPID).Count();
                    if (oacnt != 0)
                    {
                        var qgid = db.OAEquipGeneralData.Where(m => m.OANumber == oanumber.OAnum).Select(m => new { m.MDBID, m.QGID, m.OAID }).SingleOrDefault();
                        ViewBag.mdbid = qgid.MDBID;
                        //ViewBag.Product = new SelectList(from r in db.OAEquipTableData
                        //                                 where r.OAID == qgid.OAID
                        //                                 select r.ProductModel.ProductModelName);
                        ViewBag.Product = oanumber.modelno;
                        ViewBag.Macslno = oanumber.MacSlNo;
                        var mdb = db.QGEquipGeneralData.Where(m => m.QGID == qgid.QGID).Select(m => new { m.MDBID, m.MDBGeneralData.OrganizationName, m.MDBGeneralData.City, m.MDBGeneralData.Isd1, m.MDBGeneralData.Std1, m.MDBGeneralData.PhoneLL1 }).Single();
                        ViewBag.Company = mdb.OrganizationName;
                        ViewBag.City = mdb.City;
                        var Tel = mdb.Isd1 + "-" + mdb.Std1 + "-" + mdb.PhoneLL1;
                        //Contact Person Name & Mobile Number
                        var contact = db.MDBContactPersonData.Where(m => m.MDBID == mdb.MDBID).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Mobile1, m.Isdm1 }).Single();
                        ViewBag.Contact = contact.FirstName + " " + contact.MiddleName + " " + contact.LastName;
                        var Mob = contact.Isdm1 + "-" + contact.Mobile1;
                        if (mdb.PhoneLL1 == "" && mdb.PhoneLL1 == null)
                        {
                            ViewBag.Phone = Mob;
                        }
                        else
                        {
                            ViewBag.Phone = Mob + "/" + Tel;
                        }
                        //Model Number
                        //var emodel = db.OAEquipTableData.Where(m => m.OAID == qgid.OAID).Select(m => m.ProductModel.ProductModelName).Single();
                        //ViewBag.EModel = emodel;
                        //Commission Date from HOD
                        var comdat = oanumber.Handeddate;
                        ViewBag.CommissionDate = Convert.ToDateTime(comdat).ToString("dd/MM/yyyy");
                        ViewBag.OAnum = oanum;
                    }
                    else
                    {
                        TempData["OANum"] = "For this Order Number Hand Over Document is not Created / Not valid Order Number!!!";
                    }
                }

                DateTime Mfrdat = System.DateTime.Now;
                ViewBag.ShowQuoteDate = Mfrdat.ToString("dd-MM-yyyy");
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                return View();

            }

        }

        //
        //POST: /MFR/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MFR(MFRALL mfrall, string oanum,string spr1,string spr2)
       {
            // here oanum = Hod Number
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            if (mfrall.MFR.MfrTo != null && mfrall.MFR.MfrTo != "" && mfrall.MFRParts1.ProductModelSparesID != 0)
            {
                mfrall.MFR.CPID = loginname.CPID;

                //Sent for Approval
                mfrall.MFR.IsMFR = 1;
                int mdbid = mfrall.MFR.MDBID;
                string macsl = mfrall.MFR.MacSlNo;
                string model = mfrall.MFR.MfrModelNo;

                if (spr1 != null)
                {
                    mfrall.MFR.IsSpareTakenFromExistingCPStock = 1;
                }
                else mfrall.MFR.IsSpareTakenFromExistingCPStock = 0;

                if (spr2 != null)
                {
                    mfrall.MFR.IsSpareNeededFromBBAN = 1;
                }
                else mfrall.MFR.IsSpareNeededFromBBAN = 0;

                db.MFR.Add(mfrall.MFR);
                db.SaveChanges();

                if (mfrall.MFRParts1.ProductModelSparesID != 0)
                {
                    mfrall.MFRParts1.MFRID = mfrall.MFR.MFRID;
                    db.MFRParts.Add(mfrall.MFRParts1);
                    db.SaveChanges();
                }
                if (mfrall.MFRParts2.ProductModelSparesID != 0)
                {
                    mfrall.MFRParts2.MFRID = mfrall.MFR.MFRID;
                    db.MFRParts.Add(mfrall.MFRParts2);
                    db.SaveChanges();
                }
                if (mfrall.MFRParts3.ProductModelSparesID != 0)
                {
                    mfrall.MFRParts3.MFRID = mfrall.MFR.MFRID;
                    db.MFRParts.Add(mfrall.MFRParts3);
                    db.SaveChanges();
                }
                if (mfrall.MFRParts4.ProductModelSparesID != 0)
                {
                    mfrall.MFRParts4.MFRID = mfrall.MFR.MFRID;
                    db.MFRParts.Add(mfrall.MFRParts4);
                    db.SaveChanges();
                }
                if (mfrall.MFRParts5.ProductModelSparesID != 0)
                {
                    mfrall.MFRParts5.MFRID = mfrall.MFR.MFRID;
                    db.MFRParts.Add(mfrall.MFRParts5);
                    db.SaveChanges();
                }

                //for mail data:
                var custObj = db.MDBGeneralData.Find(mfrall.MFR.MDBID);
                var cpObj = db.ChannelPartners.Find(mfrall.MFR.CPID);
                var oanumber = db.Handover.Where(m => m.HODNumber == mfrall.MFR.OrderNum).Single();
                string frommail = null;
                string mailpass = null;
                var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m => m.MSID == 1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
                if (from != null)
                {
                    frommail = from.FromMail;
                    mailpass = from.Password;
                }
                #region mail module
                string cpname = db.ChannelPartners.Where(s => s.CPID == loginname.CPID).Select(s => s.CPName).FirstOrDefault();
                var cpemail = db.CPMailIdTO.Where(s => s.CPID == loginname.CPID).Select(s => s.EmailId).FirstOrDefault();

                //Auto Mail after a Shift Report Generation
                #region
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //Attachment attach = new Attachment(excelpath);

                var maildata=

                //mail.From = new MailAddress("atcqualityteam@gmail.com");
                mail.From = new MailAddress(frommail);
                mail.CC.Add(cpemail);
                var spareAdminEmailToCC =  db.BuhlerAdminCC.Where(s => s.Type == "Admin").ToList();
                foreach (var mc in spareAdminEmailToCC)
                {
                    string spareadminemail = mc.EmailId;
                    mail.To.Add(new MailAddress(spareadminemail));
                }
                mail.CC.Add(new MailAddress("bbandispatches@gmail.com"));
                // mail.To.Add(new MailAddress("praveen.shivanna@buhlergroup.com"));
                // mail.To.Add("shankar.chandrashekhar@buhlergroup.com");
                // mail.CC.Add(new MailAddress("bharath.naik@buhlergroup.com"));
                // mail.CC.Add(new MailAddress("rakesh.kaul@buhlergroup.com"));
                // mail.CC.Add(new MailAddress("srinivas.kulkarni@buhlergroup.com"));

                mail.Subject = "MFR Generation";
                mail.Body = "<p><b>Dear All,</b></p>" +
                            "<b></b>" +
                            "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please find the MFR Generation details below&nbsp;"  + ".</b></p>"

                            +
                                "<style>"+
                               "table { border-collapse: collapse; } table, td, th { border: 1px solid black; } td,th { left:auto; }" +
                               "</style>"
                            +

                            //data table
                            "<table style='width:50%'>"        +
                            //"<tr> <th align='left'>Date</th> " + "<td>" + System.DateTime.Now.ToString("dd MMM yyyy") + "</td> </tr>" +
                            "<tr> <th align='left'>MFR No</th> "              + "<td>" + mfrall.MFR.MFRNumber        + "</td> </tr>" +
                            "<tr> <th align='left'>Customer Name</th>  " + "<td>" + custObj.OrganizationName + "</td> </tr>" +
                            "<tr> <th align='left'>MFR Date</th> " + "<td>" + mfrall.MFR.MfrDate + "</td> </tr>" +
                            "<tr> <th align='left'>Channel Partner Name</th>" + "<td>" + cpObj.CPName + "</td> </tr>" +
                            "<tr> <th align='left'>Model</th>" + "<td>" + oanumber.modelno + "</td> </tr>" +
                            "<tr> <th align='left'>Model Sl. No.</th>" + "<td>" + oanumber.MacSlNo + "</td> </tr>" +
                            "<tr> <th align='left'>Commission Date</th>" + "<td>" + mfrall.MFR.CommissionedDate + "</td> </tr>" +
                            "<tr> <th align='left'>Fault 1</th>" + "<td>" + mfrall.MFR.Fault + "</td> </tr>" +
                            "</table>"

                            +"</b>" +
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

                TempData["MFRGen"] = "MFR Sent for Approval Successfully";
                return RedirectToAction("MFRChannelView");
            }
            ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return RedirectToAction("MFR");
        }

        //Update Product Description Dynamically in the View on Product spare selection
        [HttpGet]
        public JsonResult GetProductModelDetails(int id)
        {
            var selectedRow = (from t in db.ProductModelSpare where t.ProductModelSparesID == id select t).SingleOrDefault();

            var jsonData = new
            {
                Desc = selectedRow.ProductModelSparesDesc,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //MFRView
        //const int pageSize = 10;
        //bool getdetailsclick = false;
        public ActionResult MFRView(int page = 1, int sortBy = 1, bool isAsc = false, string cunam = null)
        {
            //int channame = db.ChannelPartners.Where(m => m.CPName == cunam).Select(m => m.CPID).Single();

            IEnumerable<MFR> quotations = db.MFR.Where(m => m.ApprovalStatus == 0).OrderByDescending(m=>m.MFRID).ToList();

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRNumber) : quotations.OrderByDescending(p => p.MFRNumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MfrDate) : quotations.OrderByDescending(p => p.MfrDate);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.CPID) : quotations.OrderByDescending(p => p.CPID);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.CommissionedBy) : quotations.OrderByDescending(p => p.CommissionedBy);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize);

            //quotations = quotations
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize;

            //ViewBag.Search = cunam;

            //ViewBag.SortBy = sortBy;
            //ViewBag.IsAsc = isAsc;

            //if (getdetailsclick)
            //    ViewBag.IsSearch = true;
            //if (cunam != null)
            //{
            //    getdetailsclick = true;
            //    ViewBag.IsSearch = true;
            //}
            //ViewBag.NullError = false;
            //ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations);
        }

        //MFRChannelView
        //const int pageSize1 = 10;
        //bool getdetailsclick1 = false;
        public ActionResult MFRChannelView(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            //int channame = db.ChannelPartners.Where(m => m.CPName == cunam).Select(m => m.CPID).Single();
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<MFR> quotations = db.MFR.Where(m => m.IsMFR == 1).Where(m=>m.CPID==loginname.CPID).ToList();

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRNumber) : quotations.OrderByDescending(p => p.MFRNumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.OrderNum) : quotations.OrderByDescending(p => p.OrderNum);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.MfrTo) : quotations.OrderByDescending(p => p.MfrTo);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.MfrDate) : quotations.OrderByDescending(p => p.MfrDate);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.CommissionedBy) : quotations.OrderByDescending(p => p.CommissionedBy);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize1);

            //quotations = quotations
            //    .Skip((page - 1) * pageSize1)
            //    .Take(pageSize1)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize1;

            //ViewBag.Search = cunam;

            //ViewBag.SortBy = sortBy;
            //ViewBag.IsAsc = isAsc;

            //if (getdetailsclick1)
            //    ViewBag.IsSearch = true;
            //if (cunam != null)
            //{
            //    getdetailsclick1 = true;
            //    ViewBag.IsSearch = true;
            //}
            //ViewBag.NullError = false;
            //ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations);
        }

        //MFRChannelView
        //const int pageSizea = 10;
        //bool getdetailsclicka = false;
        public ActionResult MFRAdminView(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            //int channame = db.ChannelPartners.Where(m => m.CPName == cunam).Select(m => m.CPID).Single();

            IEnumerable<MFR> quotations = db.MFR.Where(m => m.IsMFR == 1).ToList();

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRNumber) : quotations.OrderByDescending(p => p.MFRNumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.MfrDate) : quotations.OrderByDescending(p => p.MfrDate);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.CPID) : quotations.OrderByDescending(p => p.CPID);
                    break;

                //case 5:
                //    quotations = isAsc ? quotations.OrderBy(p => p.MfrDate) : quotations.OrderByDescending(p => p.MfrDate);
                //    break;

                //case 5:
                //    quotations = isAsc ? quotations.OrderBy(p => p.CommissionedBy) : quotations.OrderByDescending(p => p.CommissionedBy);
                //    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSizea);

            //quotations = quotations
            //    .Skip((page - 1) * pageSizea)
            //    .Take(pageSize3)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSizea;

            //ViewBag.Search = cunam;

            //ViewBag.SortBy = sortBy;
            //ViewBag.IsAsc = isAsc;

            //if (getdetailsclicka)
            //    ViewBag.IsSearch = true;
            //if (cunam != null)
            //{
            //    getdetailsclicka = true;
            //    ViewBag.IsSearch = true;
            //}
            //ViewBag.NullError = false;
            //ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations);
        }

        //GET: /MFRAdmin/
        public ActionResult MFRAdmin(int mfrid = 0)
        {
            if (mfrid != 0)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                ViewBag.LoginName1 = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                //oanum contains OrderNum = HODNumber
                var oanum = db.MFR.Where(m => m.MFRID == mfrid).Select(m => new { m.OrderNum, m.CPID, m.MfrEnteredBy, m.MFRNumber }).Single();
                ViewBag.Mfrnum = oanum.MFRNumber;
                ViewBag.HODNumber = oanum.OrderNum;
                //ordernum == order acknowledgement number
                var ordernum = db.Handover.Where(m => m.HODNumber == oanum.OrderNum).Select(m => new { m.OAnum, m.Handeddate, m.modelno, m.MacSlNo, m.MDBID }).Single();
                ViewBag.mdbid = ordernum.MDBID;
                //Getting Login Name and Channel partner name
                ViewBag.LoginName = oanum.MfrEnteredBy;
                //Channel Partner
                var channm = db.ChannelPartners.Where(m => m.CPID == oanum.CPID).Select(m => m.CPName).Single();
                ViewBag.CPName = channm;
                ViewBag.cpid = oanum.CPID;

                //Order Number
                //QGID->MDB->Company Name, Contact person data, Phone Number & City

                var qgid = db.OAEquipGeneralData.Where(m => m.OANumber == ordernum.OAnum).Select(m => new { m.QGID, m.OAID }).SingleOrDefault();
                var mdb = db.QGEquipGeneralData.Where(m => m.QGID == qgid.QGID).Select(m => new { m.MDBID, m.MDBGeneralData.OrganizationName, m.MDBGeneralData.City, m.MDBGeneralData.Isd1, m.MDBGeneralData.Std1, m.MDBGeneralData.PhoneLL1 }).Single();
                ViewBag.Company = mdb.OrganizationName;
                ViewBag.City = mdb.City;
                var Tel = mdb.Isd1 + "-" + mdb.Std1 + "-" + mdb.PhoneLL1;
                //Contact Person Name & Mobile Number
                var contact = db.MDBContactPersonData.Where(m => m.MDBID == mdb.MDBID).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Mobile1, m.Isdm1 }).Single();
                ViewBag.Contact = contact.FirstName + " " + contact.MiddleName + " " + contact.LastName;
                var Mob = contact.Isdm1 + "-" + contact.Mobile1;
                if (mdb.PhoneLL1 == "" && mdb.PhoneLL1 == null)
                {
                    ViewBag.Phone = Mob;
                }
                else
                {
                    ViewBag.Phone = Mob + "/" + Tel;
                }
                //Model Number
                //var emodel = db.OAEquipTableData.Where(m => m.OAID == qgid.OAID).Select(m => m.ProductModel.ProductModelName).Single();
                //ViewBag.EModel = emodel;
                //Commission Date from HOD
                var comdat = ordernum.Handeddate;
                ViewBag.CommissionDate = Convert.ToDateTime(comdat).ToString("dd/MM/yyyy");
                //ViewBag.MacSrlNo = comdat.MacSlNo;
                MFRALL mfrall = new MFRALL();
                var mfid = db.MFR.Find(mfrid);
                if (mfid != null)
                {
                    mfrall.MFR = mfid;
                    var models = db.MFRParts.Where(m => m.MFRID == mfrid);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;

                    if (modelcnt == 5)
                    {
                        mfrall.MFRParts1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        mfrall.MFRParts2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[1].Quantity;
                        mfrall.MFRParts3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[2].Quantity;
                        mfrall.MFRParts4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[3].Quantity;
                        mfrall.MFRParts5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[4].Quantity;
                        ViewBag.EquipTable1 = true;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                    }
                    if (modelcnt == 4)
                    {
                        mfrall.MFRParts1 = modelcount[0];
                        mfrall.MFRParts2 = modelcount[1];
                        mfrall.MFRParts3 = modelcount[2];
                        mfrall.MFRParts4 = modelcount[3];
                        mfrall.MFRParts5 = null;
                        ViewBag.EquipTable1 = true;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                    }
                    if (modelcnt == 3)
                    {
                        mfrall.MFRParts1 = modelcount[0];
                        mfrall.MFRParts2 = modelcount[1];
                        mfrall.MFRParts3 = modelcount[2];
                        mfrall.MFRParts4 = null;
                        mfrall.MFRParts5 = null;
                        ViewBag.EquipTable1 = true;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                    }
                    if (modelcnt == 2)
                    {
                        mfrall.MFRParts1 = modelcount[0];
                        mfrall.MFRParts2 = modelcount[1];
                        mfrall.MFRParts3 = null;
                        mfrall.MFRParts4 = null;
                        mfrall.MFRParts5 = null;
                        ViewBag.EquipTable1 = true;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                    }
                    if (modelcnt == 1)
                    {
                        mfrall.MFRParts1 = modelcount[0];
                        mfrall.MFRParts2 = null;
                        mfrall.MFRParts3 = null;
                        mfrall.MFRParts4 = null;
                        mfrall.MFRParts5 = null;
                        ViewBag.EquipTable1 = true;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                    }
                }
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                return View(mfrall);
            }
            return View();
        }

        //Product Model Details Update Dynamically in the View
        [HttpGet]
        public JsonResult GetProductModelDetailsmac(string id, string oanum)
        {
            var selectedRow = (from t in db.Handover where t.modelno == id && (t.HODNumber == oanum) select t).SingleOrDefault();

            var jsonData = new
            {
                MacSlNO = "noval",
            };

            if (selectedRow != null)
            {
                jsonData = new
                {
                    MacSlNO = selectedRow.MacSlNo,
                };
            }
            else
            {
                jsonData = new
                {
                    MacSlNO = "noval",
                };
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //Post: / MFRAdmin/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MFRAdmin(MFRALL mfrall)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
            ViewBag.LoginName1 = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;

            int mid = mfrall.MFR.MFRID;

            if (mfrall.MFRBBAN.DispatchDat != "" && mfrall.MFRBBAN.DispatchDat != null) //Here dispatched date is known as General Date and default storing system date.
            {
                if (mfrall.MFRBBAN.RecdOn != "" && mfrall.MFRBBAN.RecdOn != null)
                {
                    mfrall.MFRBBAN.MFRID = mfrall.MFR.MFRID;
                    db.MFRBBAN.Add(mfrall.MFRBBAN);
                    db.SaveChanges();
                }

                if (Request.Form["btnapproval"] != null)
                {
                    var mfrgen = mfrall.MFR;
                    int mfrid1 = mfrgen.MFRID;
                    //mfrgen.ApprovalStatus = 1;

                    var Mb = db.MFRParts.Where(m => m.MFRID == mfrall.MFRBBAN.MFRID).Where(m => m.ProductModelSpare.IsDeleted == 0).Select(a => new { a.ProductModelSpare.ProductModelSparesID, a.ProductModelSpare.ProductModelSparesDesc, a.Quantity, a.ProductModelSpare.AgentPrice }).ToList();
                    var custname = db.MDBGeneralData.Where(m => m.MDBID == mfrall.MFR.MDBID).Select(m => m.OrganizationName).Single();
                    int i = 1;
                    foreach (var x in Mb)
                    {
                        int getstk = 0;
                        OutwardMFR OutwardMFR = new OutwardMFR();

                        // Get the Product model spare stock from ProductModelSpare table
                        if (i == 1)
                        {
                            getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == mfrall.MFRParts1.ProductModelSparesID)
                            .Select(m => m.BuhlerPresentStock).Single();
                        }
                        else if (i == 2)
                        {
                            getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == mfrall.MFRParts2.ProductModelSparesID)
                            .Select(m => m.BuhlerPresentStock).Single();
                        }
                        if (i == 3)
                        {
                            getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == mfrall.MFRParts3.ProductModelSparesID)
                            .Select(m => m.BuhlerPresentStock).Single();
                        }
                        else if (i == 4)
                        {
                            getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == mfrall.MFRParts4.ProductModelSparesID)
                            .Select(m => m.BuhlerPresentStock).Single();
                        }
                        else if (i == 5)
                        {
                            getstk = db.ProductModelSpare.Where(m => m.ProductModelSparesID == mfrall.MFRParts5.ProductModelSparesID)
                            .Select(m => m.BuhlerPresentStock).Single();
                        }

                        if (getstk != null)
                        {
                            //
                            //To Outward MFR
                            //
                            OutwardMFR.OutwardMonth = System.DateTime.Today.ToString("MMM yyyy");

                            var emptype = OutwardMFR.OutwardMonth.Substring(0, 3);
                            //getting month number from month name
                            int mon = DateTime.ParseExact(emptype, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                            OutwardMFR.OutMonthNo = mon;
                            OutwardMFR.MFRNo = mfrall.MFR.MFRNumber;
                            OutwardMFR.CPID = mfrall.MFR.CPID;
                            OutwardMFR.CustomerName = custname.ToString();
                            OutwardMFR.ProductModelSparesID = x.ProductModelSparesID;
                            OutwardMFR.Quantity = x.Quantity;
                            OutwardMFR.QuantityOrdered = x.Quantity;
                            var agent = Convert.ToInt32(x.AgentPrice);
                            var quan = Convert.ToInt32(x.Quantity);
                            var res = agent * quan;
                            OutwardMFR.TotalValue = res.ToString();
                            OutwardMFR.FaultSpareRecivedDateAdmin = null;
                            db.OutwardMFR.Add(OutwardMFR);
                            db.SaveChanges();
                        }// getstk > x.Quantity
                        else
                        {
                            TempData["NotInw"] = "No Sufficient stock is available for selected spare model";
                            ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                            ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                            ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                            ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                            ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
                            return RedirectToAction("MFRView");
                        }
                        //} //getstk != null
                        i++;
                    }

                    mfrgen.MDBGeneralData = db.MDBGeneralData.Where(m => m.MDBID == mfrgen.MDBID).Single();
                    db.Entry(mfrgen).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("MFRApproveComment", "MFR", new { mfrid = mfrid1 });
                }
                else if (Request.Form["btnrejected"] != null)
                {
                    var mfrgen = mfrall.MFR;
                    mfrgen.ApprovalStatus = 2;
                    db.Entry(mfrgen).State = EntityState.Modified;
                    db.SaveChanges();
                    int mfrid1 = mfrgen.MFRID;
                    return RedirectToAction("MFRRejectComment", "MFR", new { mfrid = mfrid1 });
                }
            }
            else
            {
                ViewData["ProductModelSparesID1"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID2"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID3"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID4"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                ViewData["ProductModelSparesID5"] = new SelectList(db.ProductModelSpare, "ProductModelSparesID", "ProductSpareNameDesc");
                TempData["date"] = "Please select AOD Date and Dispatch Date!!!!";
                return RedirectToAction("MFRAdmin", "MFR", new { mfrid = mid });
            }
            return RedirectToAction("MFRView");
        }

        //
        // GET: /HOD/HODView
        //const int pageSize2 = 10;
        //bool getdetailsclick2 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult MFRReprint(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();


            IEnumerable<MFR> quotations = db.MFR.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.CPID == loginname.CPID);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRNumber) : quotations.OrderByDescending(p => p.MFRNumber);
                    break;

                case 3:
                    quotations = isAsc ? quotations.OrderBy(p => p.OrderNum) : quotations.OrderByDescending(p => p.OrderNum);
                    break;

                case 4:
                    quotations = isAsc ? quotations.OrderBy(p => p.MDBGeneralData.OrganizationName) : quotations.OrderByDescending(p => p.MDBGeneralData.OrganizationName);
                    break;

                case 5:
                    quotations = isAsc ? quotations.OrderBy(p => p.MfrModelNo) : quotations.OrderByDescending(p => p.MfrModelNo);
                    break;

                case 6:
                    quotations = isAsc ? quotations.OrderBy(p => p.MacSlNo) : quotations.OrderByDescending(p => p.MacSlNo);
                    break;

                case 7:
                    quotations = isAsc ? quotations.OrderBy(p => p.MfrDate) : quotations.OrderByDescending(p => p.MfrDate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.MFRID) : quotations.OrderByDescending(p => p.MFRID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize2);

            //quotations = quotations
            //    .Skip((page - 1) * pageSize2)
            //    .Take(pageSize2)
            //    .ToList();

            //ViewBag.CurrentPage = page;
            //ViewBag.PageSize = pageSize2;

            //ViewBag.Search = cunam;

            //ViewBag.SortBy = sortBy;
            //ViewBag.IsAsc = isAsc;

            //if (getdetailsclick2)
            //    ViewBag.IsSearch = true;
            //if (cunam != null)
            //{
            //    getdetailsclick2 = true;
            //    ViewBag.IsSearch = true;
            //}
            //ViewBag.NullError = false;
            //ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.MFRID).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        //MFR REPORT
        public ActionResult MFRReport(int? mfrid)
        {
            if (mfrid != 0)
            {
                MFRALL quo = new MFRALL();
                ReportModelMFR RM = new ReportModelMFR();
                var qgdb = db.MFR.Find(mfrid);
                if (qgdb != null)
                {
                    var cpid = qgdb.CPID;
                    var Logo = db.ChannelPartners.Find(cpid);
                    var channelPartnerLogo = "";
                    var Logopath = "";
                    var add1 = "Check Your Address";

                    channelPartnerLogo = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                    add1 = Logo.footaddress;
                    var models = db.MFRParts.Where(m => m.MFRID == mfrid);
                    var modelcount = models.ToList();
                    quo.MFR = qgdb;

                    //here oanum == hodnummber
                    var oanum = db.MFR.Where(m => m.MFRID == mfrid).Select(m => m.OrderNum).Single();
                    //order == order acknowledgement number
                    var order = db.Handover.Where(m => m.HODNumber == oanum).Select(m => m.OAnum).Single();
                    int mdbid = db.OAEquipGeneralData.Where(m => m.OANumber == order).Select(m => m.MDBID).Single();
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    RM.Logo = Logopath;
                    //RM.footaddress = add1;
                    RM.MFRNumber = qgdb.MFRNumber;
                    RM.MFREnteredBy = qgdb.MfrEnteredBy;
                    RM.MFRTo = qgdb.MfrTo;
                    RM.ChannelPartner = Logo.CPName;
                    RM.MfrDate = qgdb.MfrDate;
                    RM.McBrkDwn = qgdb.MacBreakDown;
                    RM.McOpTem = qgdb.MacOperatTemp;
                    RM.OrganizationName = mdbdet.OrganizationName;
                    RM.City = mdbdet.City;
                    RM.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    RM.Mobile = cpmdb.Mobile1 + "/" + cpmdb.Isd1 + " " + cpmdb.Std1 + " " + cpmdb.PhoneLL1;
                    RM.Modelno = qgdb.MfrModelNo;
                    RM.MacSlNo = qgdb.MacSlNo;
                    RM.CommissionedBy = qgdb.CommissionedBy;
                    //DateTime dtt = DateTime.ParseExact(qgdb.CommissionedDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //String frda1 = dtt.ToString("yyyy-MM-dd");
                    //DateTime frda = DateTime.Parse(frda1).Date;
                    //RM.CommDate = Convert.ToDateTime(qgdb.CommissionedDate).ToString("dd/MM/yyyy");
                    RM.CommDate = qgdb.CommissionedDate;
                    RM.MFRNumber = qgdb.MFRNumber;
                    RM.HODNumber = oanum;
                    RM.Fault1 = qgdb.Fault;
                    RM.Ask1 = qgdb.Ask1;
                    RM.Ask2 = qgdb.Ask2;
                    RM.Ask3 = qgdb.Ask3;
                    RM.Ask4 = qgdb.Ask4;
                    RM.Ask5 = qgdb.Ask5;
                    RM.Remedial = qgdb.Remedial;
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
                    RM.MFRParts = modelcount;
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

                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                    //RM.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                    //RM.Designation = loginname.Designation;
                    //var FileName = RM.QuotationNumber;
                    //ViewBag.CPID = loginname.CPID;
                    var relativePath = "MFRReport";
                    string content;
                    var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                    ViewData.Model = RM;

                    String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                    //String footerpath = Server.MapPath("~/App_Data/FooterFile.html");
                    string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
                        //"<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 140px\"><div style=\"width: 700px; float: left; height: 140px\"><label style=\"text-align:centre;padding-left:30px;font-size:20px\">Machine Fault Report</label></div><div style=\"width: 300px; height: 140px; float: right\"><img src= \"" + Logopath + "\" width=\"200\" height=\"60\" /></div></div></body>";
                    //string htmlfooter = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 60px;width: 1000px\"><div style=\"width: 400px; float: left; height: 100px\"> " + add1 + " </div><div style=\"width: 300px; height: 60px; float: right\">On Behalf of <img src= \"" + Logopath1 + "\" width=\"300\" height=\"100\" /></div></div></body>";

                    if (System.IO.File.Exists(headerpath))
                        System.IO.File.Delete(headerpath);
                    System.IO.File.Create(headerpath).Close();

                    System.IO.File.WriteAllText(headerpath, htmlheader);

                    //if (System.IO.File.Exists(footerpath))
                    //    System.IO.File.Delete(footerpath);
                    //System.IO.File.Create(footerpath).Close();

                    //System.IO.File.WriteAllText(footerpath, htmlfooter);

                    //use the passed-in parameter and populate your model
                    using (var writer = new StringWriter())
                    {
                        var context = new ViewContext(this.ControllerContext, view.View, ViewData, TempData, writer);
                        view.View.Render(context, writer);
                        writer.Flush();
                        content = writer.ToString();
                        var pdfconverter = new PDFConverterMFR();
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
        //MFR Approval comment
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult MFRApproveComment(int mfrid)
        {
            var oa = db.MFR.Where(m => m.MFRID == mfrid).Single();
            ViewBag.mfrid = mfrid;
            return View(oa);
        }

        //
        //MFR Approval Comment
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult MFRApproveComment(MFR mfr)
        {
            mfr.ApprovalStatus = 1;
            db.Entry(mfr).State = EntityState.Modified;
            db.SaveChanges();

            //for mail data:
            var custObj = db.MDBGeneralData.Find(mfr.MDBID);
            var cpObj = db.ChannelPartners.Find(mfr.CPID);
            var oanumber = db.Handover.Where(m => m.HODNumber == mfr.OrderNum).Single();

            #region mail module

            //Auto Mail after a Shift Report Generation
            #region
            MailMessage mail = new MailMessage();
            var cpemail = db.CPMailIdTO.Where(s => s.CPID == mfr.CPID).Select(s => s.EmailId).FirstOrDefault();

            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            string frommail = "";
            string mailpass = "";
            //Attachment attach = new Attachment(excelpath);
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m => m.MSID == 1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                frommail = from.FromMail;
                mailpass = from.Password;
            }
            var maildata =

            mail.From = new MailAddress(frommail);
            mail.CC.Add(cpemail);
            var spareAdminEmailToCC = db.BuhlerAdminCC.Where(s => s.Type == "Admin").ToList();
            foreach (var mc in spareAdminEmailToCC)
            {
                string spareadminemail = mc.EmailId;
                mail.To.Add(new MailAddress(spareadminemail));
            }
            mail.CC.Add(new MailAddress("bbandispatches@gmail.com"));

            mail.Subject = "MFR Status";
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

            TempData["mfrap/rejected"] = "MFR Approved Successfully!!!!";
            return RedirectToAction("MFRView");
        }


        //
        //MFR Reject comment
        //
        [Authorize(Roles = "Administrator")]
        public ActionResult MFRRejectComment(int mfrid)
        {
            var oa = db.MFR.Where(m => m.MFRID == mfrid).Single();
            ViewBag.mfrid = mfrid;
            return View(oa);
        }

        //
        //MFR Reject Comment
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult MFRRejectComment(MFR mfr)
        {
            mfr.ApprovalStatus = 2;
            db.Entry(mfr).State = EntityState.Modified;
            db.SaveChanges();

            //for mail data:
            var custObj = db.MDBGeneralData.Find(mfr.MDBID);
            var cpObj = db.ChannelPartners.Find(mfr.CPID);
            var oanumber = db.Handover.Where(m => m.HODNumber == mfr.OrderNum).Single();
            string frommail = null;
            string mailpass = null;
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Where(m => m.MSID == 1).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                frommail = from.FromMail;
                mailpass = from.Password;
            }
            #region mail module
            string cpname = db.ChannelPartners.Where(s => s.CPID == mfr.CPID).Select(s => s.CPName).FirstOrDefault();
            var cpemail = db.CPMailIdTO.Where(s => s.CPID == mfr.CPID).Select(s => s.EmailId).FirstOrDefault();

            #region
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            var maildata =

            mail.From = new MailAddress(frommail);
            mail.CC.Add(cpemail);
            var spareAdminEmailToCC = db.BuhlerAdminCC.Where(s => s.Type == "Admin").ToList();
            foreach (var mc in spareAdminEmailToCC)
            {
                string spareadminemail = mc.EmailId;
                mail.To.Add(new MailAddress(spareadminemail));
            }
            mail.CC.Add(new MailAddress("bbandispatches@gmail.com"));
            //#region mail module

            ////Auto Mail after a Shift Report Generation
            //#region
            //MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            ////Attachment attach = new Attachment(excelpath);

            //var maildata =

            //mail.From = new MailAddress("atcqualityteam@gmail.com");

            //mail.To.Add(new MailAddress("praveen.shivanna@buhlergroup.com"));
            //mail.To.Add("shankar.chandrashekhar@buhlergroup.com");

            //mail.CC.Add(new MailAddress("bharath.naik@buhlergroup.com"));
            //mail.CC.Add(new MailAddress("rakesh.kaul@buhlergroup.com"));
            //mail.CC.Add(new MailAddress("bbandispatches@gmail.com"));
            //mail.CC.Add(new MailAddress("srinivas.kulkarni@buhlergroup.com"));

            mail.Subject = "MFR Status";
            mail.Body = "<p><b>Dear All,</b></p>" +
                        "<b></b>" +
                        "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please find the MFR Generation status below&nbsp;" + ".</b></p>"

                        +
                            "<style>" +
                           "table { border-collapse: collapse; } table, td, th { border: 1px solid black; } td,th { left:auto; }" +
                           "</style>"
                        +

                        //data table
                        "<table style='width:500%'>" +
                        //"<tr> <th align='left'>Date</th> " + "<td>" + System.DateTime.Now.ToString("dd MMM yyyy") + "</td> </tr>" +
                        "<tr> <th align='left'>MFR No</th> " + "<td>" + mfr.MFRNumber + "</td> </tr>" +
                        "<tr> <th align='left'>Customer Name</th>  " + "<td>" + custObj.OrganizationName + "</td> </tr>" +
                        "<tr> <th align='left'>MFR Date</th> " + "<td>" + mfr.MfrDate + "</td> </tr>" +
                        "<tr> <th align='left'>Channel Partner Name</th>" + "<td>" + cpObj.CPName + "</td> </tr>" +
                        "<tr> <th align='left'>Model</th>" + "<td>" + oanumber.modelno + "</td> </tr>" +
                        "<tr> <th align='left'>Model Sl. No.</th>" + "<td>" + oanumber.MacSlNo + "</td> </tr>" +
                        "<tr> <th align='left'>Commission Date</th>" + "<td>" + mfr.CommissionedDate + "</td> </tr>" +
                        "<tr> <th align='left'>Fault 1</th>" + "<td>" + mfr.Fault + "</td> </tr>" +

                        "<tr> <th align='left'>Status</th>" + "<td> Rejected </td> </tr>" +
                        //"<tr> <th align='left'>Description</th>" + "<td> Spares Outstock </td> </tr>" +
                        "<tr> <th align='left'>Remarks</th>" + "<td>" + mfr.MFRComment + "</td> </tr>" +

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

            TempData["mfrap/rejected"] = "MFR Rejected Successfully!!!!";
            return RedirectToAction("MFRView");
        }

    }

    public interface IPDFConverterMFR
    {
        byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml);
    }

    public class PDFConverterMFR : IPDFConverterMFR
    {
        private const string HtmlToPdfExePath = "wkhtmltopdf.exe";
        private readonly ILog log = LogManager.GetLogger(typeof(PDFConverterMFR));

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
