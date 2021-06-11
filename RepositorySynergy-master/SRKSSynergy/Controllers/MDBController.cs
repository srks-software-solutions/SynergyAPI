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
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class MDBController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete MDB Data Base
        public JsonResult Autocomplete(string term)
       {        
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where (r.OrganizationName.ToLower().Contains(term.ToLower()) && r.CPID == loginname.CPID) && (r.IsDeleted == 10) 
                          select new { r.OrganizationName }).Distinct();

               if (User.IsInRole("ZonalManager"))
                {
                    result = (from s in db.MDBGeneralData
                              from b in db.ChannelPartners
                              where (s.OrganizationName.ToLower().Contains(term.ToLower()) && s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.IsDeleted==0)
                              select new { s.OrganizationName }).Distinct();
                }
               else if (User.IsInRole("ChannelPartnerUser") || User.IsInRole("ChannelPartnerAdmin"))
               {
                   result = (from s in db.MDBGeneralData
                             from b in db.ChannelPartners
                             where (s.OrganizationName.ToLower().Contains(term.ToLower()) && s.CPID == b.CPID && s.IsDeleted==0)
                             select new { s.OrganizationName }).Distinct();
               }
                else
                {
                    result = (from s in db.MDBGeneralData 
                              where (s.OrganizationName.ToLower().Contains(term.ToLower()) && s.IsDeleted==0 )
                              select new { s.OrganizationName }).Distinct();// db.MDBGeneralData.Where(m => m.IsDeleted == 0).ToList(); ;
                }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete for index
        public JsonResult AutocompleteIndex(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where (r.OrganizationName.ToLower().Contains(term.ToLower()) && r.CPID == loginname.CPID) && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();
                       
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult AutocompleteCP(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID ,m.ZoneID}).SingleOrDefault();
            if (User.IsInRole("ZonalManager"))
            {
                var result = (from r in db.ChannelPartners
                              where (r.CPName.ToLower().Contains(term.ToLower()) && r.ZoneID == loginname.ZoneID)
                              select new { r.CPName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = (from r in db.ChannelPartners
                              where (r.CPName.ToLower().Contains(term.ToLower()) && r.CPID != loginname.CPID)
                              select new { r.CPName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        
        //
        // GET: /MDB/
        //const int pageSize = 10;
        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string custnm = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var getcpid = loginname.CPID;
            //Paging and Sorting //
            IEnumerable<MDBGeneralData> chanpart = db.MDBGeneralData.Where(
                    p => custnm == null
                    || p.OrganizationName.Contains(custnm)).Where(m=>m.CPID == getcpid);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.MDBID) : chanpart.OrderByDescending(p => p.MDBID);
                    break;

                case 2:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CompanyUniqueID) : chanpart.OrderByDescending(p => p.CompanyUniqueID);
                    break;

                case 3:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationName) : chanpart.OrderByDescending(p => p.OrganizationName);
                    break;

                case 4:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationType) : chanpart.OrderByDescending(p => p.OrganizationType);
                    break;

                case 5:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.SearchTerm) : chanpart.OrderByDescending(p => p.SearchTerm);
                    break;

                case 6:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.Address) : chanpart.OrderByDescending(p => p.Address);
                    break;

                case 7:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
                    break;

                case 8:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 9:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 10:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LandLine1) : chanpart.OrderByDescending(p => p.LandLine1);
                    break;

                case 11:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.EmailID) : chanpart.OrderByDescending(p => p.EmailID);
                    break;

                case 12:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.PostalCourier) : chanpart.OrderByDescending(p => p.PostalCourier);
                    break;

                default:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.MDBID) : chanpart.OrderByDescending(p => p.MDBID);
                    break;
            }
            #endregion

            //ViewBag.TotalPages = (int)Math.Ceiling((double)chanpart.Count() / pageSize);

            //chanpart = chanpart
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
            return View(chanpart);
            //end of paging and sorting//
            //return View(db.ChannelPartners.ToList());
        }    
        //
        // GET: /MDB/Details/5
        public ActionResult Details(int id = 0)
        {
            MDBGeneralData mdbgeneraldata = db.MDBGeneralData.Find(id);
            if (mdbgeneraldata == null)
            {
                return HttpNotFound();
            }
            return View(mdbgeneraldata);
        }

        //
        // GET: /MDB/Create
        public ActionResult Create()
        {
            var mdid = from s in db.MDBGeneralData
                       select s;
            mdid = mdid.OrderByDescending(m => m.MDBID);
            var check = mdid.FirstOrDefault();
            if (check == null)
            {
                ViewBag.Chanpart = "CUS-"+System.DateTime.Now.Year+"-00001";
            }
            else
            {
                var mdbi = mdid.Select(m => m.CompanyUniqueID).First();
                string[] split = mdbi.Split('-');
                //if (split[1] == System.DateTime.Now.Year.ToString())
                //{
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
                    mdbi = split[0] + "-" + System.DateTime.Now.Year + "-" + cpik;
                    ViewBag.Chanpart = mdbi;
                //}
                //else
                //{
                //    ViewBag.Chanpart = "CUS-" + System.DateTime.Now.Year + "-00001";
                //}
            }
            return View();
        }

        //MDB ID Generation
        public string custnumber()
        {
            String customernumber;
            var mdid = from s in db.MDBGeneralData
                       select s;
            mdid = mdid.OrderByDescending(m => m.MDBID);
            var check = mdid.FirstOrDefault();
            if (check == null)
            {
                customernumber = "CUS-" + System.DateTime.Now.Year + "-00001";
            }
            else
            {
                var mdbi = mdid.Select(m => m.CompanyUniqueID).First();
                string[] split = mdbi.Split('-');
                if (split[1] == System.DateTime.Now.Year.ToString())
                {
                    //cpi = split[1];
                    //var cpi1 = split[1] + 1;
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
                    mdbi = split[0] + "-" + split[1] + "-" + cpik;
                    customernumber = mdbi;
                }
                else
                {
                    customernumber = "CUS-" + System.DateTime.Now.Year + "-00001";
                }
            }
            return customernumber;
        }

        //
        // POST: /MDB/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MainDataBase maindatabase)
        {
            var duplicate = (from s in db.MDBGeneralData
                             where s.OrganizationName == maindatabase.MDBGeneralData.OrganizationName && s.AddressLine1 == maindatabase.MDBGeneralData.AddressLine1 && s.AddressLine2 == maindatabase.MDBGeneralData.AddressLine2 && s.AddressLine3 == maindatabase.MDBGeneralData.AddressLine3 && s.City == maindatabase.MDBGeneralData.City && s.Pincode == maindatabase.MDBGeneralData.Pincode
                             select s).ToList();

            if (maindatabase.MDBGeneralData.OrganizationName != null && maindatabase.MDBContactPersonData1.FirstName != null)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                var getcpid = loginname.CPID;
                var name = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                if (duplicate.Count > 0)
                {
                    ViewBag.Duplicate = "Customer Already Exists for this Address and City";
                }
                else
                {

                    string l1 = null;
                    string l2 = null;

                    var state = db.ChannelPartners.Where(m => m.CPID == getcpid).Select(m => m.State).SingleOrDefault();

                    // code to find if pincode matches with state.
                    // Given: cpid gives state
                    //#region

                    //int tick = 0;
                    //try
                    //{
                    //    string url = "http://maps.google.com/maps/api/geocode/json?address=" + Convert.ToInt32(maindatabase.MDBGeneralData.Pincode) + "&sensor=false";

                    //    using (var webClient = new System.Net.WebClient())
                    //    {
                    //        var json = webClient.DownloadString(url);
                           
                    //        JavaScriptSerializer jss = new JavaScriptSerializer();
                    //        GoogleGeoCodeResponse test = jss.Deserialize<GoogleGeoCodeResponse>(json);

                    //        for (int i = 0; i < test.results.Count(); i++)
                    //        {
                    //            string address = test.results[i].formatted_address;
                    //            //usually addresses are of format: ("formatted_address" : "Haryana 134202, India")
                    //            //code to handle address of format: ("formatted_address" : "125075, India")

                    //            char firstChar = address[0];
                    //            if (char.IsLetter(firstChar))
                    //            {
                    //                //prepare state name
                    //                string justState = null;
                    //                justState = state.Substring(0, state.IndexOf('('));

                    //                bool containsS = address.IndexOf(justState, StringComparison.OrdinalIgnoreCase) >= 0;
                    //                // if (containsS || containsC)
                    //                if (containsS)
                    //                {
                    //                    l1 = test.results[i].geometry.location.lat.ToString();
                    //                    l2 = test.results[i].geometry.location.lng.ToString();
                    //                    tick = 1;
                    //                    break;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                bool containsC = address.IndexOf("India", StringComparison.OrdinalIgnoreCase) >= 0;
                    //                if (containsC)
                    //                {
                    //                    l1 = test.results[i].geometry.location.lat.ToString();
                    //                    l2 = test.results[i].geometry.location.lng.ToString();
                    //                    tick = 1;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }

                    //}
                    //catch (Exception ex)
                    //{
                    //    ex.GetHashCode();
                    //}

                    //if (tick == 0)
                    //{
                    //    Session["WrongPincode"] = "Invalid Pincode";
                    //    return RedirectToAction("LECreateRevised");
                    //}
                    //#endregion
                    try
                    {
                    maindatabase.MDBGeneralData.CPID = getcpid;
                    maindatabase.MDBGeneralData.ContactName = name;
                    db.MDBGeneralData.Add(maindatabase.MDBGeneralData);
                    db.SaveChanges();
                    var cpid = from s in db.MDBGeneralData
                               select s;
                    cpid = cpid.OrderByDescending(m => m.MDBID);
                    int cpi = cpid.Select(m => m.MDBID).First();
                    if (maindatabase.MDBBankDetail1.BankName != null)
                    {
                        maindatabase.MDBBankDetail1.MDBID = cpi;
                        db.MDBBankDetail.Add(maindatabase.MDBBankDetail1);
                        db.SaveChanges();
                    }
                    if (maindatabase.MDBBankDetail2.BankName != null)
                    {
                        maindatabase.MDBBankDetail2.MDBID = cpi;
                        db.MDBBankDetail.Add(maindatabase.MDBBankDetail2);
                        db.SaveChanges();
                    }
                    maindatabase.MDBContactPersonData1.MDBID = cpi;
                    db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData1);
                   
                        db.SaveChanges();
                    

                    if (maindatabase.MDBContactPersonData2.FirstName != null)
                    {
                        maindatabase.MDBContactPersonData2.MDBID = cpi;
                        db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.MDBContactPersonData3.FirstName != null)
                    {
                        maindatabase.MDBContactPersonData3.MDBID = cpi;
                        db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.MDBContactPersonData4.FirstName != null)
                    {
                        maindatabase.MDBContactPersonData4.MDBID = cpi;
                        db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData4);
                        db.SaveChanges();
                    }
                    if (maindatabase.MDBContactPersonData5.FirstName != null)
                    {
                        maindatabase.MDBContactPersonData5.MDBID = cpi;
                        db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData5);
                        db.SaveChanges();
                    }
                    if (maindatabase.MDBContactPersonData6.FirstName != null)
                    {
                        maindatabase.MDBContactPersonData6.MDBID = cpi;
                        db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData6);
                        db.SaveChanges();
                    }
                    maindatabase.MDBStatutoryNumber.MDBID = cpi;
                    db.MDBStatutoryNumber.Add(maindatabase.MDBStatutoryNumber);
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
                    return RedirectToAction("Index");
                }
            }
            return View(maindatabase);
        }

        //
        // GET: /MDB/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                MainDataBase cp = new MainDataBase();
                var mdid = db.MDBGeneralData.Find(id);
                cp.MDBGeneralData = mdid;
                var mdidm = db.MDBGeneralData.Where(m => m.MDBID == id);
                var conper = db.MDBContactPersonData.Where(m => m.MDBID == id).ToList();
                int cntper = conper.Count;
                if (cntper == 6)
                {
                    cp.MDBContactPersonData1 = conper[0];
                    cp.MDBContactPersonData2 = conper[1];
                    cp.MDBContactPersonData3 = conper[2];
                    cp.MDBContactPersonData4 = conper[3];
                    cp.MDBContactPersonData5 = conper[4];
                    cp.MDBContactPersonData6 = conper[5];
                }
                else if (cntper == 5)
                {
                    cp.MDBContactPersonData1 = conper[0];
                    cp.MDBContactPersonData2 = conper[1];
                    cp.MDBContactPersonData3 = conper[2];
                    cp.MDBContactPersonData4 = conper[3];
                    cp.MDBContactPersonData5 = conper[4];
                    cp.MDBContactPersonData6 = null;
                }
                else if (cntper == 4)
                {
                    cp.MDBContactPersonData1 = conper[0];
                    cp.MDBContactPersonData2 = conper[1];
                    cp.MDBContactPersonData3 = conper[2];
                    cp.MDBContactPersonData4 = conper[3];
                    cp.MDBContactPersonData5 = null;
                    cp.MDBContactPersonData6 = null;
                }
                else if (cntper == 3)
                {
                    cp.MDBContactPersonData1 = conper[0];
                    cp.MDBContactPersonData2 = conper[1];
                    cp.MDBContactPersonData3 = conper[2];
                    cp.MDBContactPersonData4 = null;
                    cp.MDBContactPersonData5 = null;
                    cp.MDBContactPersonData6 = null;
                }
                else if (cntper == 2)
                {
                    cp.MDBContactPersonData1 = conper[0];
                    cp.MDBContactPersonData2 = conper[1];
                    cp.MDBContactPersonData3 = null;
                    cp.MDBContactPersonData4 = null;
                    cp.MDBContactPersonData5 = null;
                    cp.MDBContactPersonData6 = null;
                }
                else if (cntper == 1)
                {
                    cp.MDBContactPersonData1 = conper[0];
                    cp.MDBContactPersonData2 = null;
                    cp.MDBContactPersonData3 = null;
                    cp.MDBContactPersonData4 = null;
                    cp.MDBContactPersonData5 = null;
                    cp.MDBContactPersonData6 = null;
                }
                var mdidor = db.MDBBankDetail.Where(m => m.MDBID == id).ToList();
                int mdbcnt = mdidor.Count;

                if (mdbcnt == 1)
                {
                    cp.MDBBankDetail1 = mdidor[0];
                    cp.MDBBankDetail2 = null;
                }
                else if (mdbcnt == 2)
                {
                    cp.MDBBankDetail1 = mdidor[0];
                    cp.MDBBankDetail2 = mdidor[1];
                }
                var mdidst = db.MDBStatutoryNumber.Where(m => m.MDBID == id);
                var mdidst1 = mdidst.OrderBy(m => m.MDBSNID).SingleOrDefault();
                if(mdidst1 != null)
                cp.MDBStatutoryNumber = mdidst1;
                return View(cp);
            }
            else
                return View();
        }

        //
        // POST: /MDB/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MainDataBase maindatabase)
        {
            //var duplicate = (from s in db.MDBGeneralData
            //                 where s.OrganizationName == maindatabase.MDBGeneralData.OrganizationName && s.AddressLine1 == maindatabase.MDBGeneralData.AddressLine1 && s.AddressLine2 == maindatabase.MDBGeneralData.AddressLine2 && s.AddressLine3 == maindatabase.MDBGeneralData.AddressLine3 && s.City == maindatabase.MDBGeneralData.City && s.Pincode == maindatabase.MDBGeneralData.Pincode
            //                 select s).ToList();

            if (maindatabase.MDBGeneralData.OrganizationName != null && maindatabase.MDBContactPersonData1.FirstName != null)
            {
                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                    var getcpid = loginname.CPID;

                    //if (duplicate.Count > 0)
                    //{
                    //    ViewBag.Duplicate = "Customer Already Exists for this Address and City";
                    //}
                    //else
                    //{
                        maindatabase.MDBGeneralData.CPID = getcpid;

                        var cpi = maindatabase.MDBGeneralData.MDBID;
                        db.Entry(maindatabase.MDBGeneralData).State = EntityState.Modified;
                        db.SaveChanges();


                        if (maindatabase.MDBContactPersonData1.MDBCPDID != 0)
                        {
                            db.Entry(maindatabase.MDBContactPersonData1).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (maindatabase.MDBContactPersonData1.FirstName != null)
                        {
                            maindatabase.MDBContactPersonData1.MDBID = cpi;
                            db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData1);
                            db.SaveChanges();
                        }

                        if (maindatabase.MDBContactPersonData2.MDBCPDID != 0)
                        {
                            db.Entry(maindatabase.MDBContactPersonData2).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (maindatabase.MDBContactPersonData2.FirstName != null)
                        {
                            maindatabase.MDBContactPersonData2.MDBID = cpi;
                            db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData2);
                            db.SaveChanges();
                        }

                        if (maindatabase.MDBContactPersonData3.MDBCPDID != 0)
                        {
                            db.Entry(maindatabase.MDBContactPersonData3).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (maindatabase.MDBContactPersonData3.FirstName != null)
                        {
                            maindatabase.MDBContactPersonData3.MDBID = cpi;
                            db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData3);
                            db.SaveChanges();
                        }

                        if (maindatabase.MDBContactPersonData4.MDBCPDID != 0)
                        {
                            db.Entry(maindatabase.MDBContactPersonData4).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (maindatabase.MDBContactPersonData4.FirstName != null)
                        {
                            maindatabase.MDBContactPersonData4.MDBID = cpi;
                            db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData4);
                            db.SaveChanges();
                        }

                        if (maindatabase.MDBContactPersonData5.MDBCPDID != 0)
                        {
                            db.Entry(maindatabase.MDBContactPersonData5).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (maindatabase.MDBContactPersonData5.FirstName != null)
                        {
                            maindatabase.MDBContactPersonData5.MDBID = cpi;
                            db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData5);
                            db.SaveChanges();
                        }

                        if (maindatabase.MDBContactPersonData6.MDBCPDID != 0)
                        {
                            db.Entry(maindatabase.MDBContactPersonData6).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (maindatabase.MDBContactPersonData6.FirstName != null)
                        {
                            maindatabase.MDBContactPersonData6.MDBID = cpi;
                            db.MDBContactPersonData.Add(maindatabase.MDBContactPersonData6);
                            db.SaveChanges();
                        }

                        if (maindatabase.MDBBankDetail1.BankName != null && maindatabase.MDBBankDetail1.BankName != "")
                        {
                            maindatabase.MDBBankDetail1.MDBID = cpi;
                            var id = db.MDBBankDetail.Where(m => m.MDBID == cpi).Count();
                            if (id == 0)
                            {
                                db.MDBBankDetail.Add(maindatabase.MDBBankDetail1);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(maindatabase.MDBBankDetail1).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        if (maindatabase.MDBBankDetail2.BankName != null && maindatabase.MDBBankDetail2.BankName != "")
                        {
                            maindatabase.MDBBankDetail2.MDBID = cpi;
                            var id = db.MDBBankDetail.Where(m => m.MDBID == cpi).Count();
                            if (id == 0)
                            {
                                db.MDBBankDetail.Add(maindatabase.MDBBankDetail2);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(maindatabase.MDBBankDetail2).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        if (maindatabase.MDBStatutoryNumber.TIN != null && maindatabase.MDBStatutoryNumber.TIN != null)
                        {
                            maindatabase.MDBStatutoryNumber.MDBID = cpi;
                            var id = db.MDBStatutoryNumber.Where(m => m.MDBID == cpi).Count();
                            if (id == 0)
                            {
                                db.MDBStatutoryNumber.Add(maindatabase.MDBStatutoryNumber);
                                db.SaveChanges();
                            }
                            else
                            { 
                                db.Entry(maindatabase.MDBStatutoryNumber).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        return RedirectToAction("Index");
                    //}
            }
            return View(maindatabase);
        }

        //Update Customer Number in the MDB View
        [HttpGet]
        public JsonResult Getcustomernumber()
        {
            
                ViewBag.Chanpart = custnumber();

                var jsonData = new
                {
                    cusnumb = custnumber()
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        //Discontinue Customer
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var deac = db.MDBGeneralData.Where(m => m.MDBID == id).Where(m => m.IsDeleted == 0).Single();
            return View(deac);
        }

        //
        //Post: Discontinue Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(MDBGeneralData prdmd)
        {
            if (Request.Form["Yes"] != null)
            {
                prdmd.IsDeleted = 1;
                db.Entry(prdmd).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("CPMDB");
        }

        //
        // GET: /CPMDB/
        public ActionResult CPMDB(int page = 1, int rowsPerPage=20, int sortBy = 1, bool isAsc = true, string State = null, string custnm = null, string customername = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID,m.ZoneID }).SingleOrDefault();
            var getcpid = loginname.CPID;
            //Paging and Sorting //
            IEnumerable<MDBGeneralData> chanpart = null;
            if (custnm == null && State == null && customername==null)
            {
                if (User.IsInRole("ZonalManager"))
                {
                    chanpart = (from s in db.MDBGeneralData
                               from b in db.ChannelPartners
                               where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                               select s).ToList();
                }
                else
                {
                    chanpart = db.MDBGeneralData.Where(m => m.IsDeleted == 0);
                }
            }
            else
            {
                var cpid = db.ChannelPartners.Where(m => m.CPName == custnm).Select(m => m.CPID).SingleOrDefault();          
                if (customername != null && customername != "")
                {
                    chanpart = db.MDBGeneralData.Where(m => m.OrganizationName == customername).Where(m => m.IsDeleted == 0);
                    ViewBag.IsSearch = false;      
                }                
               else if (cpid != null && cpid != 0)
                {
                    chanpart = db.MDBGeneralData.Where(m => m.CPID == cpid).Where(m => m.IsDeleted == 0);
                    var chpart = db.ChannelPartners.Where(p => p.CPID == cpid).SingleOrDefault();
                    if (chpart == null)
                    {
                        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                        return RedirectToAction("CPMDB", "MDB");
                    }
                    ViewBag.OrganizationName = chpart.CPName;
                    ViewBag.AddressLine1 = chpart.AddressLine1;
                    ViewBag.AddressLine2 = chpart.AddressLine2;
                    ViewBag.AddressLine3 = chpart.AddressLine3;
                    ViewBag.City = chpart.City;
                    ViewBag.Pincode = chpart.PinCode;
                    ViewBag.State = chpart.State;
                    ViewBag.IsSearch = true;
                }
               else if (State != null && State!="" && State!="#")
                {
                    chanpart = db.MDBGeneralData.Where(m => m.State == State).Where(m => m.IsDeleted == 0);
                    ViewBag.IsSearch = true;
                }                                         
            }

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.MDBID) : chanpart.OrderByDescending(p => p.MDBID);
                    break;

                case 2:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CompanyUniqueID) : chanpart.OrderByDescending(p => p.CompanyUniqueID);
                    break;

                case 3:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationName) : chanpart.OrderByDescending(p => p.OrganizationName);
                    break;

                case 4:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationType) : chanpart.OrderByDescending(p => p.OrganizationType);
                    break;

                case 5:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.SearchTerm) : chanpart.OrderByDescending(p => p.SearchTerm);
                    break;

                case 6:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.Address) : chanpart.OrderByDescending(p => p.Address);
                    break;

                case 7:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
                    break;

                case 8:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 9:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 10:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LandLine1) : chanpart.OrderByDescending(p => p.LandLine1);
                    break;

                case 11:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.EmailID) : chanpart.OrderByDescending(p => p.EmailID);
                    break;

                case 12:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.PostalCourier) : chanpart.OrderByDescending(p => p.PostalCourier);
                    break;

                default:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.MDBID) : chanpart.OrderByDescending(p => p.MDBID);
                    break;
            }
            #endregion

            var pager = new Pager(chanpart.Count(), page, rowsPerPage);
            var viewModel = new IndexViewModel
            {
                CMDB = chanpart.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            ViewBag.SlNo = (page - 1) * rowsPerPage + 1;
            return View(viewModel);
           
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult ExportData(string custnm = null, string State = null)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("ZonalManager"))
            {
                GridView gv = new GridView();
                if (custnm != null && custnm != "" && State != null && State != "")
                {
                    if (custnm != null && custnm != "" && State != null && State != "")
                    {
                        DataTable dts = new DataTable();
                        dts.Columns.Add("Customer ID", typeof(String));
                        dts.Columns.Add("Company Name", typeof(String));
                        dts.Columns.Add("City", typeof(String));
                        dts.Columns.Add("State", typeof(String));
                        dts.Columns.Add("Phone", typeof(String));
                        dts.Columns.Add("email id", typeof(String));
                        dts.Columns.Add("Channel Partner", typeof(String));


                        var channame = db.ChannelPartners.Where(m => m.CPName == custnm).Select(m => m.CPID).SingleOrDefault();
                        var duplicate = (from s in db.MDBGeneralData
                                         from b in db.ChannelPartners
                                         where s.CPID != 0 && (s.OrganizationName == custnm) && (s.State == State) && s.CPID == b.CPID
                                         select new
                                         {
                                             s.CompanyUniqueID,
                                             s.OrganizationName,
                                             s.City,
                                             s.State,
                                             s.PhoneLL1,
                                             s.EmailID,
                                             b.CPName,
                                         });

                        var dt1 = duplicate.ToList();
                        foreach (var d in duplicate)
                        {
                            DataRow dr = dts.NewRow();
                            dr[0] = d.CompanyUniqueID;
                            dr[1] = d.OrganizationName;
                            dr[2] = d.City;
                            dr[3] = d.State;
                            dr[4] = d.PhoneLL1;
                            dr[5] = d.EmailID;
                            dr[6] = d.CPName;

                            dts.Rows.Add(dr);
                        }
                        gv.DataSource = dts;
                        gv.DataBind();
                    }

                    if (custnm != null && custnm != "")
                    {
                        DataTable dts = new DataTable();
                        dts.Columns.Add("Customer ID", typeof(String));
                        dts.Columns.Add("Company Name", typeof(String));
                        dts.Columns.Add("City", typeof(String));
                        dts.Columns.Add("State", typeof(String));
                        dts.Columns.Add("Phone", typeof(String));
                        dts.Columns.Add("Email id", typeof(String));
                        dts.Columns.Add("Channel Partner", typeof(String));


                        var channame = db.ChannelPartners.Where(m => m.CPName == custnm).Select(m => m.CPID).SingleOrDefault();
                        var duplicate = (from s in db.MDBGeneralData
                                         from b in db.ChannelPartners
                                         where s.CPID != 0 && (s.OrganizationName == custnm) && s.CPID == b.CPID
                                         select new
                                         {
                                             s.CompanyUniqueID,
                                             s.OrganizationName,
                                             s.City,
                                             s.State,
                                             s.PhoneLL1,
                                             s.EmailID,
                                             b.CPName,
                                         });

                        var dt1 = duplicate.ToList();
                        foreach (var d in duplicate)
                        {
                            DataRow dr = dts.NewRow();
                            dr[0] = d.CompanyUniqueID;
                            dr[1] = d.OrganizationName;
                            dr[2] = d.City;
                            dr[3] = d.State;
                            dr[4] = d.PhoneLL1;
                            dr[5] = d.EmailID;
                            dr[6] = d.CPName;

                            dts.Rows.Add(dr);
                        }
                        gv.DataSource = dts;
                        gv.DataBind();
                    }
                    if (State != null && State != "")
                    {
                        DataTable dts = new DataTable();
                        dts.Columns.Add("Customer ID", typeof(String));
                        dts.Columns.Add("Company Name", typeof(String));
                        dts.Columns.Add("City", typeof(String));
                        dts.Columns.Add("State", typeof(String));
                        dts.Columns.Add("Phone", typeof(String));
                        dts.Columns.Add("email id", typeof(String));
                        dts.Columns.Add("Channel Partner", typeof(String));



                        var channame = db.ChannelPartners.Where(m => m.CPName == custnm).Select(m => m.CPID).SingleOrDefault();
                        var duplicate = (from s in db.MDBGeneralData
                                         from b in db.ChannelPartners
                                         where s.CPID != 0 && (s.State == State) && s.CPID == b.CPID
                                         select new
                                         {
                                             s.CompanyUniqueID,
                                             s.OrganizationName,
                                             s.City,
                                             s.State,
                                             s.PhoneLL1,
                                             s.EmailID,
                                             b.CPName,
                                         });

                        var dt1 = duplicate.ToList();
                        foreach (var d in duplicate)
                        {
                            DataRow dr = dts.NewRow();
                            dr[0] = d.CompanyUniqueID;
                            dr[1] = d.OrganizationName;
                            dr[2] = d.City;
                            dr[3] = d.State;
                            dr[4] = d.PhoneLL1;
                            dr[5] = d.EmailID;
                            dr[6] = d.CPName;

                            dts.Rows.Add(dr);
                        }
                        gv.DataSource = dts;
                        gv.DataBind();
                    }
                }
                else
                {

                    DataTable dts = new DataTable();
                    dts.Columns.Add("Customer ID", typeof(String));
                    dts.Columns.Add("Company Name", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Phone", typeof(String));
                    dts.Columns.Add("email id", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));


                    var channame = db.ChannelPartners.Where(m => m.CPName == custnm).Select(m => m.CPID).SingleOrDefault();
                    var duplicate = (from s in db.MDBGeneralData
                                     from b in db.ChannelPartners
                                     where s.CPID != 0 && s.CPID == b.CPID
                                     select new
                                     {
                                         s.CompanyUniqueID,
                                         s.OrganizationName,
                                         s.City,
                                         s.State,
                                         s.PhoneLL1,
                                         s.EmailID,
                                         b.CPName,
                                     });

                    var dt1 = duplicate.ToList();
                    foreach (var d in duplicate)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.CompanyUniqueID;
                        dr[1] = d.OrganizationName;
                        dr[2] = d.City;
                        dr[3] = d.State;
                        dr[4] = d.PhoneLL1;
                        dr[5] = d.EmailID;
                        dr[6] = d.CPName;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=CPMDB.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

                return RedirectToAction("CPMDB");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult ViewDetails(int id)
        {
            var deac = db.MDBGeneralData.Where(m => m.MDBID == id).Where(m => m.IsDeleted == 0).Single();

            var cd = db.MDBContactPersonData.Where(m => m.MDBID == id).FirstOrDefault();
            
            ViewBag.cdname=cd.Title + cd.FirstName+cd.MiddleName + cd.LastName;
            ViewBag.cddesignation=cd.Designation;
            ViewBag.cddept=cd.Department;                                          
            ViewBag.cdphone1=cd.Isd1+cd.Std1+cd.PhoneLL1;
            ViewBag.cdphne2=cd.Isd2+cd.Std2+cd.PhoneLL2;
            ViewBag.cdmob1=cd.Isdm1+cd.Mobile1;
            ViewBag.cdmob2=cd.Isdm2+cd.Mobile2;
            ViewBag.cdemail=cd.EmailID;
            ViewBag.cdkeyact=cd.KeyActivity;
            ViewBag.cdcomment=cd.Comments;

            var srn = db.MDBStatutoryNumber.Where(m => m.MDBID == id).FirstOrDefault();
            if (srn != null)
            {
                if (srn.CompanyPAN != null)
                {
                    ViewBag.srncpan = srn.CompanyPAN;
                }
                if (srn.TIN != null)
                {
                    ViewBag.srntin = srn.TIN;
                }
                if (srn.RegistrationNumber != null)
                {
                    ViewBag.srnregno = srn.RegistrationNumber;
                }
                if (srn.ServiceTaxNumber != null)
                {
                    ViewBag.srnservicetax = srn.ServiceTaxNumber;
                }
                if (srn.ImporterExporterCode != null)
                {
                    ViewBag.srnimport = srn.ImporterExporterCode;
                }
                if (srn.CSTNumber != null)
                {
                    ViewBag.srncstno = srn.CSTNumber;
                }
                if (srn.TaxDeductionAccountNumber != null)
                {
                    ViewBag.srntaxdedu = srn.TaxDeductionAccountNumber;
                }
                if (srn.Others != null)
                {
                    ViewBag.srnothers = srn.Others;
                }
            }

            var bank = db.MDBBankDetail.Where(m => m.MDBID == id).FirstOrDefault();
            if (bank != null)
            {
                if (bank.BankName != null)
                {
                    ViewBag.bankname = bank.BankName;
                }
                if (bank.BranchName != null)
                {
                    ViewBag.bankbranch = bank.BranchName;
                }
                if (bank.Accounttype != null)
                {
                    ViewBag.bankacctype = bank.Accounttype;
                }
                if (bank.AccountNumber != null)
                {
                    ViewBag.bankaccno = bank.AccountNumber;
                }
                if (bank.IFSCCode != null)
                {
                    ViewBag.bankifsc = bank.IFSCCode;
                }
                if (bank.AddressLine1 != null)
                {
                    ViewBag.bankaddress = bank.AddressLine1 + bank.AddressLine2 + bank.AddressLine3;
                }
                if (bank.City != null)
                {
                    ViewBag.bankcity = bank.City;
                }
                if (bank.PinCode != null)
                {
                    ViewBag.bankpincode = bank.PinCode;
                }
                if (bank.State != null)
                {
                    ViewBag.bankstate = bank.State;
                }
                if (bank.Country != null)
                {
                    ViewBag.bankcountry = bank.Country;
                }
                if (bank.Isd1 != null)
                {
                    ViewBag.bankphone1 = bank.Isd1 + bank.Std1 + bank.PhoneLL1;
                }
                if (bank.Isd2 != null)
                {
                    ViewBag.bankphone2 = bank.Isd2 + bank.Std2 + bank.PhoneLL2;
                }
                if (bank.Isdf != null)
                {
                    ViewBag.bankfax = bank.Isdf + bank.Stdf + bank.FAX;
                }
                if (bank.Email != null)
                {
                    ViewBag.bankemail = bank.Email;
                }
                if (bank.Website != null)
                {
                    ViewBag.bankwebsite = bank.Website;
                }
                if (bank.BankChequeinfavor != null)
                {
                    ViewBag.bankchequeof = bank.BankChequeinfavor;
                }
            }
            return View(deac);           
        }

    }
}