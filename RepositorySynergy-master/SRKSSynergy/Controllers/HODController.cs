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
     public class HODController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocompletehod(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.OAEquipGeneralData
                          where r.OANumber.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) && (r.OAStatus == 1) && (r.IsHOD != 1) && (r.Islatest != 1) && (r.ApprovalStatus == 1)
                          select new { r.OANumber }).Distinct();
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

        //
        // GET: /HOD/HODView
        //const int pageSize2 = 10;
        //bool getdetailsclick2 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult HODView(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();


            IEnumerable<OAEquipGeneralData> quotations = db.OAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.Islatest != 1).Where(m => m.ApprovalStatus == 1).Where(m => m.OAStatus != 0).Where(m => m.IsHOD != 1).Where(m => m.IsMacineDispatch !=0).Where(m => m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

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

                //case 7:
                //    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.ApprovalStatus);
                //    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.OAID) : quotations.OrderByDescending(p => p.OAID);
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
            //ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.OAID).ToList());
        }

        //
        //Generate the HOD number
        public string hodnumber()
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String hodnumber;
            var quotmod = from HODNumber in db.Handover
                          select HODNumber;
            quotmod = quotmod.OrderByDescending(m => m.HODNumber);
            var check = quotmod.FirstOrDefault();
            if (check == null)
            {
                hodnumber = "HD-" + "RC" + year + "-00001-00";
            }
            else
            {
                var quotval = quotmod.Select(m => m.HODNumber).First();
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
                    hodnumber = quotval;
                }
                else
                {
                    hodnumber = "HD-" + "RC" + year + "-00001-00";
                }
            }
            return hodnumber;
        }

        //Update HOD Number in the New Quotation View
        [HttpGet]
        public JsonResult Gethodnumber(string oanum)
        {
            if (oanum != null && oanum != "")
            {
                ViewBag.HodNumber = hodnumber();

                var jsonData = new
                {
                    quotnumb = hodnumber()
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
        // GET: /HOD/
        public ActionResult HOD(string oanum)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            List<SelectListItem> listItems1 = new List<SelectListItem>();
            listItems1.Add(new SelectListItem
            {
                Text = "Select",
                Value = "0"
            });
            ViewBag.Product = listItems1;

                DateTime quotdat = System.DateTime.Now;
                ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");

                if (oanum != null && oanum != "")
                {
                    ViewBag.Order = oanum;
                    ViewBag.HodNumber = hodnumber();

                    //Assigning Machine Serial No from Dispatch just for single instance
                    var mcno = db.MachineDispatch.Where(m => m.OANumber == oanum).Where(m => m.IsDeleted == 0).Select(m => m.MachineInventory.MachineSerialNo).SingleOrDefault();
                    ViewBag.Macslno = mcno;

                    var oacnt = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Where(m => m.CPID == loginname.CPID).Count();
                    if (oacnt != 0)
                    {
                        var qgid = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Where(m => m.OAStatus == 1).Where(m => m.Islatest != 1).Where(m => m.ApprovalStatus == 1).Select(m => new {m.MDBID, m.QGID, m.OAID }).Single();
                        ViewBag.Product = new SelectList(from r in db.OAEquipTableData
                                                         where r.OAID == qgid.OAID && (r.IsModelHOD != 1)
                                                         select r.ProductModel.ProductModelName);
                        var mdb = db.QGEquipGeneralData.Where(m => m.QGID == qgid.QGID).Select(m => new {m.MDBGeneralData.MDBID, m.MDBGeneralData.OrganizationName, m.MDBGeneralData.City, m.MDBGeneralData.Country }).ToList();
                        ViewBag.mdbid = qgid.MDBID;
                        foreach (var s in mdb)
                        {
                            ViewBag.cust = s.OrganizationName;
                            var city = s.City;
                            var country = s.Country;
                            ViewBag.city = city + "/" + country;
                        }
                    }
                    else
                    {
                        TempData["OANum"] = "This Order Number is not Created / Not valid Order Number!!!";
                    }
                }
            return View();
        }

        //
        //Post: /HOD/
        //
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult HOD(Handover hod, string oanum)
         {
             if (oanum != null && oanum != "")
             {
                 var macslcount = db.Handover.Where(m => m.MacSlNo == hod.MacSlNo).Count();
                 if (macslcount == 0)
                 {
                     int cpid1 = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Select(m => m.CPID).Single();
                     int mdbid = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Select(m => m.MDBID).Single();
                     hod.CPID = cpid1;
                     hod.OAnum = oanum;
                     db.Handover.Add(hod);
                     db.SaveChanges();

                     //update isstatus in Lead Enquiry table // If IsStatus==3 then quotation has been generated
                     var mdbdata = db.MDBGeneralData.Where(m => m.IsDeleted == 0).Where(m => m.MDBID == mdbid).SingleOrDefault();
                     //var name = mdbdata.OrganizationName;

                     //#region
                     //if (name != null)
                     //{
                     //    int leidcount = db.LeadEnquiry.Where(m => m.OrganizationName == name).Count();
                     //    if (leidcount == 0)
                     //    {
                     //        LeadEnquiry le = new LeadEnquiry();
                     //        le.OrganizationName = mdbdata.OrganizationName;
                     //        le.OrganizationType = mdbdata.OrganizationType;
                     //        le.AddressLine1 = mdbdata.AddressLine1;
                     //        le.AddressLine2 = mdbdata.AddressLine2;
                     //        le.AddressLine3 = mdbdata.AddressLine3;
                     //        le.City = mdbdata.City;
                     //        le.Pincode = mdbdata.Pincode;
                     //        le.State = mdbdata.State;
                     //        le.Country = mdbdata.Country;
                     //        le.Isd1 = mdbdata.Isd1;
                     //        le.Std1 = mdbdata.Std1;
                     //        le.PhoneLL1 = mdbdata.PhoneLL1;
                     //        le.EmailID = mdbdata.EmailID;

                     //        le.SuggestedModel=hod.modelno;
                     //        le.DateOfMeeting=mdbdata.CreatedOn;
                     //        le.LeadSource="forwarded from HOD";
                     //        le.MeetingDesc = "forwarded from HOD"; ;
                     //        le.CreatedOn=mdbdata.CreatedOn; ;
                     //        le.CreatedBy=mdbdata.CreatedBy;
                     //        le.IsDeleted=0;
                     //        le.IsStatus=3;
                     //        le.IsDrop=0;
                     //        le.CPID=mdbdata.CPID;

                     //        var time = System.DateTime.Now.TimeOfDay;
                     //        string test1 = Convert.ToString(time);
                     //        TimeSpan tp = TimeSpan.Parse(test1);
                     //        le.LeadTime = (tp.ToString(@"hh\:mm")); ;
                     //        le.IsTime=1;
                     //        le.IsCount=0;
                     //        le.IsHOD=1;
                     //        le.NotifyDate = System.DateTime.Now;
                     //        le.Latitude=mdbdata.Latitude;
                     //        le.Longitude=mdbdata.Longitude;

                     //        //storing contact details
                     //        int id = db.MDBContactPersonData.Where(m => m.MDBID == mdbdata.MDBID).Select(m => m.MDBCPDID).SingleOrDefault();
                     //        var list11 = db.MDBContactPersonData.Where(m => m.MDBCPDID == id).SingleOrDefault();

                     //        le.Prefix = list11.Title;
                     //        le.FirstName = list11.FirstName;
                     //        le.MiddleName = list11.MiddleName;
                     //        le.LastName = list11.LastName;
                     //        le.Isdc1 = list11.Isd1;
                     //        le.Stdc1 = list11.Std1;
                     //        le.PhoneLLc1 = list11.PhoneLL1;
                     //        le.Isdm1 = list11.Isdm1;
                     //        le.Mobile1 = list11.Mobile1;
                     //        le.EmailIDContact = list11.EmailID;

                     //        db.LeadEnquiry.Add(le);
                     //        db.SaveChanges();
                     //    }

                     //    var leid = db.LeadEnquiry.Where(m => m.OrganizationName == name).Select(m => m.LEID).SingleOrDefault();
                     //    LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(leid);
                     //    int iscount = Convert.ToInt32(db.LeadEnquiry.Where(m => m.LEID == leid).Select(m => m.IsCount).SingleOrDefault());
                     //    LeadEnquiry.IsStatus = 4;
                     //    LeadEnquiry.IsHOD = 1;
                     //    LeadEnquiry.NotifyDate = System.DateTime.Now;
                     //    db.Entry(LeadEnquiry).State = EntityState.Modified;
                     //    db.SaveChanges();
                     //}

                     //#endregion

                     //ends
                     var prdcnt = 0;
                     //
                     //Removing Orders from the AutoSuggest in HOD
                     var oanumisold = db.OAEquipGeneralData.Where(m => m.OANumber == hod.OAnum).Single();
                     var oatb = db.OAEquipTableData.Where(m => m.OAID == oanumisold.OAID).Where(m => m.ProductModel.ProductModelName == hod.modelno).Single();
                     var ct = db.OAEquipTableData.Where(m => m.OAID == oanumisold.OAID).Count();
                     oatb.IsQuantity = oatb.IsQuantity + 1;
                     if (oatb.Quantity == oatb.IsQuantity)
                     {
                         oatb.IsModelHOD = 1;
                         db.Entry(oatb).State = EntityState.Modified;
                         db.SaveChanges();
                     }
                     var prdlis = db.OAEquipTableData.Where(m => m.OAID == oanumisold.OAID).ToList();
                     foreach (var prd in prdlis)
                     {
                         if (prd.IsModelHOD == 1)
                         {
                             prdcnt = prdcnt + 1;
                         }
                         if (ct == prdcnt)
                         {
                             oanumisold.IsHOD = 1;
                             db.Entry(oanumisold).State = EntityState.Modified;
                             db.SaveChanges();
                         }
                     }

                     ////
                     ////Updating Minimum stock qty for channel partner
                     ////
                     //var oacpid = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Select(m => new { m.OAID, m.CPID }).Single();
                     //var oaid = oacpid.OAID;
                     //var cpid = oacpid.CPID;
                     //var eqqty = db.OAEquipTableData.Where(m => m.OAID == oaid).Select(m => new { m.ProductModelID, m.Quantity }).ToList();
                     //foreach (var s in eqqty)
                     //{
                     //    var prd = s.ProductModelID;
                     //    var qty = s.Quantity;
                     //    var cpcount = db.AvailSpareQuantity.Where(m => m.CPID == cpid).ToList();
                     //    var cnt = cpcount.Count();
                     //    if (cnt != 0)
                     //    {
                     //        var minstad = db.MinSpareEquipQuantity.Where(m => m.ProductModelID == prd).ToList();
                     //        foreach (var st in minstad)
                     //        {
                     //            var val1 = st.Minimumstock;
                     //            var val2 = val1 * qty;
                     //            var prsp = st.ProductModelSparesID;
                     //            var prdt = st.ProductModelID;
                     //            var avst = db.AvailSpareQuantity.Where(m => m.ProductModelSparesID == prsp).Where(m => m.month == null).Where(m => m.CPID == cpid).Single();
                     //            var val3 = avst.MinCpStock;
                     //            var val4 = val3 + val2;
                     //            avst.MinCpStock = val4;
                     //            db.Entry(avst).State = EntityState.Modified;
                     //            db.SaveChanges();
                     //        }
                     //    }
                     //}
                     //

                     // updating commission date insert into machine dispatch
                     var camdte = db.MachineDispatch.Where(m => m.OANumber == oanum).Single();
                     camdte.CommissionDate = hod.Handeddate;
                     db.Entry(camdte).State = EntityState.Modified;
                     db.SaveChanges();

                     //Adding Rows in Warranty Management
                     var oan = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Select(m => new { m.OAID, m.MDBGeneralData.CompanyUniqueID, m.MDBGeneralData.OrganizationName, m.CPID }).Single();
                     //var mod = db.OAEquipTableData.Where(m => m.OAID == oan.OAID).Select(m => m.ProductModel.ProductModelName).Single();
                     var hd = db.Handover.Where(m => m.OAnum == oanum).Where(m => m.HODNumber == hod.HODNumber).Where(m => m.modelno == hod.modelno).Select(m => new { m.Buhlercustorderno, m.MacSlNo, m.Handeddate, m.modelno, m.Quantity }).Single();
                     var hdda = hd.Handeddate.ToString();
                     string[] split = hdda.Split('/');
                     int d = Convert.ToInt32(split[1]);
                     int md = d - 1;
                     string[] split1 = split[2].Split(' ');
                     int y = Convert.ToInt32(split1[0]);
                     int my = y + 0001;
                     string exdat = md + "-" + split[0] + "-" + my;
                     //if quantity is one
                     if (hd.Quantity == 1)
                     {
                         Warranty war = new Warranty();
                         war.CustomerNumber = oan.CompanyUniqueID;
                         war.CustomerName = oan.OrganizationName;
                         war.OrderNumber = oanum;
                         war.BuhlerOrderConfirm = hd.Buhlercustorderno;
                         war.Model = hd.modelno;
                         war.CPID = oan.CPID;
                         war.MachineNummber = hd.MacSlNo;
                         war.HandoverDate = hd.Handeddate;
                         war.WarrantyExpiry = exdat;
                         db.Warranty.Add(war);
                         db.SaveChanges();
                     }
                     else
                     {
                         string[] serialno = hd.MacSlNo.Split('/');
                         foreach (var s in serialno)
                         {
                             Warranty war = new Warranty();
                             war.CustomerNumber = oan.CompanyUniqueID;
                             war.CustomerName = oan.OrganizationName;
                             war.OrderNumber = oanum;
                             war.BuhlerOrderConfirm = hd.Buhlercustorderno;
                             war.Model = hd.modelno;
                             war.CPID = oan.CPID;
                             war.MachineNummber = s;
                             war.HandoverDate = hd.Handeddate;
                             war.WarrantyExpiry = exdat;
                             db.Warranty.Add(war);
                             db.SaveChanges();
                         }
                     }



                     //HOD Report
                     int hid = hod.HID;
                     //String updateparent = "<script>window.open('/HOD/HODReport?hid=" + hid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = document.location.pathname;</script>";
                     //return Content(updateparent);
                     TempData["OANumber"] = "HOD genreated";
                     return RedirectToAction("HODView");
                 }
             }
             else
             {
                 TempData["Machineslno"] = "HOD for this Serial Number has already been generated. Please enter another Machine Serial Number !!!!";
                 return RedirectToAction("HOD");

             }
                 TempData["OANumber"] = "Please selct Order acknowledgement Number before proceeding further!!!!";
                 return RedirectToAction("HODView");
         }

        //Product Model Details Update Dynamically in the View
        [HttpGet]
        public JsonResult GetProductModelDetails(string id, string oanum)
         {
             int oaid = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Select(m => m.OAID).Single();
             var selectedRow = (from t in db.OAEquipTableData where t.ProductModel.ProductModelName == id && (t.OAID == oaid) select t).SingleOrDefault();

             var jsonData = new
             {
                 Quantity = selectedRow.Quantity,
             };
             return Json(jsonData, JsonRequestBehavior.AllowGet);
         }

        //
        // GET: /HOD/HODView
        //const int pageSize3 = 10;
        //bool getdetailsclick3 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult HODReprint( string cunam = null)//int page = 1, int sortBy = 1, bool isAsc = true,
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<Handover> quotations =null;

            //admin login
            if (User.IsInRole("Administrator"))
            {
                quotations=db.Handover.Where(
                        p => cunam == null
                        || p.MDBGeneralData.OrganizationName.Contains(cunam));
            }

            //channel partner login
            if (User.IsInRole("ChannelPartnerAdmin"))
            {
                quotations= db.Handover.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.CPID == loginname.CPID);
            }





            //#region Sorting
            //switch (sortBy)
            //{
            //    case 1:
            //        quotations = isAsc ? quotations.OrderBy(p => p.HID) : quotations.OrderByDescending(p => p.HID);
            //        break;

            //    case 2:
            //        quotations = isAsc ? quotations.OrderBy(p => p.HODNumber) : quotations.OrderByDescending(p => p.HODNumber);
            //        break;

            //    case 3:
            //        quotations = isAsc ? quotations.OrderBy(p => p.OAnum) : quotations.OrderByDescending(p => p.OAnum);
            //        break;

            //    case 4:
            //        quotations = isAsc ? quotations.OrderBy(p => p.modelno) : quotations.OrderByDescending(p => p.modelno);
            //        break;

            //    case 5:
            //        quotations = isAsc ? quotations.OrderBy(p => p.MacSlNo) : quotations.OrderByDescending(p => p.MacSlNo);
            //        break;

            //    case 6:
            //        quotations = isAsc ? quotations.OrderBy(p => p.Handeddate) : quotations.OrderByDescending(p => p.Handeddate);
            //        break;

            //    //case 7:
            //    //    quotations = isAsc ? quotations.OrderBy(p => p.OADate) : quotations.OrderByDescending(p => p.ApprovalStatus);
            //    //    break;

            //    default:
            //        quotations = isAsc ? quotations.OrderBy(p => p.HID) : quotations.OrderByDescending(p => p.HID);
            //        break;
            //}
            //#endregion

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
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations.OrderByDescending(m => m.HID).ToList());
        }

        //
        //Report Generation HOD
        //
        public ActionResult HODReport(int? hid)
         {
             if (hid != 0)
             {
                     Handover RM = new Handover();
                     var hdid = db.Handover.Find(hid);
                     var cpid = hdid.CPID;
                     var Logo = db.ChannelPartners.Find(cpid);

                     var channelPartnerLogo="";
                     var Logopath = "";

                     var add1 = "Check Your Address";

                     channelPartnerLogo= Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                     Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                     add1 = Logo.footaddress;
                     //Logopath = Server.MapPath("~/App_Data/Buhler_RGB_100.png");
                     //add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";

                     //var nm ="Buhler (India) Pvt Ltd.";
                     //var ad = "13D, KIADB Industrial Area, Attibele - 562 107, India";
                     //var ph = "Tel (080) 2289 0000 Fax:(080) 2289 0005";
                     ViewBag.Logo = Logopath;
                     ViewBag.footaddress = add1;
                     var oanum = hdid.OAnum;
                     int qid = db.OAEquipGeneralData.Where(m => m.OANumber == oanum).Where(m => m.OAStatus == 1).Where(m => m.Islatest != 1).Select(m => m.QGID).SingleOrDefault();
                     var mdb = db.QGEquipGeneralData.Where(m => m.QGID == qid).Select(m => new { m.MDBGeneralData.OrganizationName, m.MDBGeneralData.City, m.MDBGeneralData.Country }).ToList();
                     foreach (var s in mdb)
                     {
                         ViewBag.cust = s.OrganizationName;
                         var city = s.City;
                         var country = s.Country;
                         ViewBag.city = city + "/" + country;
                     }
                     RM.HODNumber = hdid.HODNumber;
                     RM.MacSlNo = hdid.MacSlNo;
                     RM.Custpurorderno = hdid.Custpurorderno;
                     RM.Buhlercustorderno = hdid.Buhlercustorderno;
                     RM.Project = hdid.Project;
                     RM.Buhlerrep = hdid.Buhlerrep;
                     RM.Custrep = hdid.Custrep;
                     RM.Handeddate = hdid.Handeddate;
                     RM.Handing = hdid.Handing;
                     RM.Commis = hdid.Commis;
                     RM.Termi = hdid.Termi;
                     RM.Erection = hdid.Erection;
                     RM.Reached = hdid.Reached;
                     RM.Notreached = hdid.Notreached;
                     RM.Trail = hdid.Trail;
                     RM.Wimaterial = hdid.Wimaterial;
                     RM.Womaterial = hdid.Womaterial;
                     RM.Wcarr = hdid.Wcarr;
                     RM.Wnotcarr = hdid.Wnotcarr;
                     RM.Completionerection = hdid.Completionerection;
                     RM.Capacity = hdid.Capacity;
                     RM.Machinehanded = hdid.Machinehanded;
                     RM.Buyer = hdid.Buyer;
                     RM.Followcomm = hdid.Followcomm;
                     RM.Seperate = hdid.Seperate;
                     RM.Comments = hdid.Comments;
                     RM.Yes = hdid.Yes;
                     RM.No = hdid.No;
                     var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                     var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                     var FileName = "HOD Report";
                     ViewBag.CPID = loginname.CPID;
                     var relativePath = "HODReport";
                     string content;
                     var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                     ViewData.Model = RM;

                     String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                     string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
                         //"<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 100px\"><div style=\"width: 700px; float: left; height: 100px\"></div><div style=\"width: 300px; height: 100px; float: right\"><img src= \"" + Logopath + "\" width=\"200\" height=\"50\" /></div></div></body>";

                     if (System.IO.File.Exists(headerpath))
                         System.IO.File.Delete(headerpath);
                     System.IO.File.Create(headerpath).Close();

                     System.IO.File.WriteAllText(headerpath, htmlheader);


                     using (var writer = new StringWriter())
                     {
                         var context = new ViewContext(this.ControllerContext, view.View, ViewData, TempData, writer);
                         view.View.Render(context, writer);
                         writer.Flush();
                         content = writer.ToString();
                         var pdfconverter = new PDFConverterh();
                         byte[] pdfBuf = pdfconverter.Convert(content, Server.MapPath("~/App_Data/"), add1, headerpath);
                         MemoryStream workStream = new MemoryStream();
                         workStream.Write(pdfBuf, 0, pdfBuf.Length);
                         workStream.Position = 0;
                         if (pdfBuf == null)
                             return HttpNotFound();
                         return new FileStreamResult(workStream, "application/pdf");
                     }
                 //}
             }
             return View("HODView");
         }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        const int pageSize1p = 30000;
        bool getdetailsclick1p = false;
        [HttpGet]
        public ActionResult RMHOD(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            ViewBag.UserId = userid;
            IEnumerable<RiceOAEquipGeneralData> quotations = db.RiceOAEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.ApprovalStatus != 2).Where(m => m.ApprovalStatus != 0).Where(m => m.OAStatus != 0).Where(m=>m.IsDispatch!=0).Where(m=>m.IsDispatch==1 || m.IsDispatch==2).Include(q => q.MDBGeneralData).OrderByDescending(m => m.OADate);

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

        [HttpPost]
        public void updateRiceOAstatus(string id, string maildetails, string date,string UID)
        {
            string OAID = id;
            int sid = 2;
            if (OAID != "" || OAID != null)
            {
                RiceOAEquipGeneralData RiceOA = db.RiceOAEquipGeneralData.Find(Convert.ToInt32(OAID));
                RiceOA.IsDispatch = sid;
                var ROAID = OAID;
                var cpid = RiceOA.CPID;
                var mdbid = RiceOA.MDBID;
                var cd = DateTime.Now;
                var cb = UID;
                RiceMillHOD RMHOD = new RiceMillHOD();
                RMHOD.ROAID=ROAID;
                RMHOD.RemarkDetails = maildetails;
                RMHOD.HODDate = Convert.ToDateTime(date);
                RMHOD.CPID = cpid;
                RMHOD.MDBID = mdbid;
                RMHOD.CreatedBy = cb;
                RMHOD.CreatedOn = cd;
                RMHOD.IsStatus = sid;
                db.Entry(RiceOA).State = EntityState.Modified;
                db.RiceMillHOD.Add(RMHOD);
                db.SaveChanges();
            }
        }

    }

     public interface IPDFConverterh
     {
         byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml);
     }

     public class PDFConverterh : IPDFConverterh
     {
         private const string HtmlToPdfExePath = "wkhtmltopdf.exe";
         private readonly ILog log = LogManager.GetLogger(typeof(PDFConverterh));

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
             args += "--margin-top 5mm --margin-bottom 5mm --margin-right 5mm --margin-left 5mm --footer-center \"" + Footeraddress + "\" --footer-right \"Page [page]/[topage]\" --footer-font-name \"Calibri\" --footer-font-size \"12\" --header-html \"" + HeaderHtml + "\" ";
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
