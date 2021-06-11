using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;
using SRKSSynergy.Controllers;
using System.Data.Entity;
using System.Web.Security;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Net;
using System.IO;
using System.Text;
using SRKSSynergy.MailServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class LeadEnquiryController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        public JsonResult EnquiryHandledByList()
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int cpid = Convert.ToInt32(Session["logincpid"]);

            string Item1 = "Buhler";

            List<SelectListItem> EnquiryHandledBy = new List<SelectListItem>();

            var CPName = db.ChannelPartners.Where(m => m.IsDeleted == 0).Where(m => m.CPID == cpid).SingleOrDefault();
            if (cpid == 0)
            {
                EnquiryHandledBy.Add(new SelectListItem { Text = Item1, Value = Item1 });
            }
            else
            {
                for (int i = 1; i <= 2; i++)
                {
                    if (i == 1)
                    {
                        EnquiryHandledBy.Add(new SelectListItem { Text = Item1, Value = Item1 });
                    }
                    else if (i == 2)
                    {
                        EnquiryHandledBy.Add(new SelectListItem { Text = CPName.CPName, Value = CPName.CPName });
                    }
                }
            }

            return Json(EnquiryHandledBy, JsonRequestBehavior.AllowGet);
        }

        // to get District and Pin Codes
        public ActionResult GettingDistrict(string prdts)
        {
            IEnumerable<SelectListItem> Distlist = new List<SelectListItem>();
            db = new SRKS_Synergy();
            if (!string.IsNullOrEmpty(prdts))
            {
                var list = db.DistrictPinCodeDetails_tbl.Where(m => m.State == prdts).ToList();
                
                Distlist = list.AsEnumerable().Select(m => new SelectListItem() { Text = m.District, Value = m.District }).ToList<SelectListItem>();

                Distlist = Distlist.GroupBy(x => x.Text).Select(x => x.First()).ToList();
                               
            };
            var result = Json(new SelectList(Distlist, "Value", "Text"));
            return result;
        }

        public ActionResult GettingPINCode(string prdts)
        {
            IEnumerable<SelectListItem> PINlist = new List<SelectListItem>();
            db = new SRKS_Synergy();
            if (!string.IsNullOrEmpty(prdts))
            {
                var list = db.DistrictPinCodeDetails_tbl.Where(m => m.District == prdts).ToList();

                PINlist = list.AsEnumerable().Select(m => new SelectListItem() { Text = m.PINCode.ToString(), Value = m.PINCode.ToString() }).ToList<SelectListItem>();

                PINlist = PINlist.GroupBy(x => x.Text).Select(x => x.First()).ToList();

            };
            var result = Json(new SelectList(PINlist, "Value", "Text"));
            return result;
        }

        //google map Json code
        #region

        //For Lead Generation (BLUE)     
        [HttpGet]
        public JsonResult LeadMapdetails(int cpid = 0)
        {
            var selectedRow = (from t in db.LeadEnquiry
                               where t.IsDeleted == 10 && t.Latitude != null
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName

                               }).ToList();
            if (cpid == 0)
            {
                selectedRow = (from t in db.LeadEnquiry
                               where t.IsDeleted == 0 && t.Latitude != null
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName

                               }).ToList();
            }
            else
            {
                selectedRow = (from t in db.LeadEnquiry
                               where t.IsDeleted == 0 && t.Latitude != null && t.CPID == cpid
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName

                               }).ToList();
            }
            return Json(selectedRow, JsonRequestBehavior.AllowGet);
        }

        // For OA Approved (YELLOW)
        [HttpGet]
        public JsonResult OAApproveMapdetails(int cpid = 0)
        {
            var selectedRow = (from t in db.MDBGeneralData
                               where t.IsDeleted == 10 && t.Latitude != null
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName
                               }).ToList();

            string[] selectedRowres = null; int i = 0;

            var mdblist = db.OAEquipGeneralData.Where(m => m.CPID == cpid).Where(m => m.OAStatus == 1).ToList();
            if (cpid == 0)
            {
                foreach (var mdb in mdblist)
                {
                    selectedRow = (from t in db.MDBGeneralData
                                   where t.IsDeleted == 0 && t.Latitude != null && t.MDBID == mdb.MDBID
                                   select new
                                   {
                                       PlaceName = t.City,
                                       GeoLat = t.Latitude,
                                       GeoLong = t.Longitude,
                                       OrgName = t.OrganizationName
                                   }).ToList();
                }
                //selectedRow.ToList().ForEach(s => { s.IsStatus = count; s.IsDrop = countdrop; });
            }
            else
            {
                //foreach (var mdb in mdblist)
                //{
                selectedRow = (from t in db.MDBGeneralData
                               from a in db.OAEquipGeneralData
                               where t.IsDeleted == 0 && t.Latitude != null && t.CPID == cpid && a.CPID == cpid && t.MDBID == a.MDBID && a.ApprovalStatus == 1
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName
                               }).ToList();
                //selectedRowres[i] = selectedRow.ToArray[];
                // i++;
                //}                         
            }
            return Json(selectedRow, JsonRequestBehavior.AllowGet);
        }

        // For LOA Lost Orders
        [HttpGet]
        public JsonResult LostOrderMapdetails(int cpid = 0)
        {
            var selectedRow = (from t in db.MDBGeneralData
                               where t.IsDeleted == 10 && t.Latitude != null
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName
                               }).ToList();

            //taking QGID from LostOrderAnalysis table.
            var qgid = db.LostOrderAnalysis.Where(m => m.CPID == cpid).ToList();
            int mdbiddb = 0;
            foreach (var q in qgid)
            {
                int qgiddb = q.QGID;
                var mdbidlist = db.QGEquipGeneralData.Where(m => m.QGID == qgiddb).SingleOrDefault();
                var mdbid = db.MDBGeneralData.Where(m => m.IsDeleted == 0).Where(m => m.MDBID == mdbidlist.MDBID).SingleOrDefault();

                if (cpid == 0)
                {
                    selectedRow = (from t in db.MDBGeneralData
                                   where t.IsDeleted == 0 && t.Latitude != null && t.MDBID == mdbid.MDBID
                                   select new
                                   {
                                       PlaceName = t.City,
                                       GeoLat = t.Latitude,
                                       GeoLong = t.Longitude,
                                       OrgName = t.OrganizationName
                                   }).ToList();
                }
                else
                {
                    selectedRow = (from t in db.MDBGeneralData
                                   where t.IsDeleted == 0 && t.Latitude != null && t.CPID == cpid && t.MDBID == mdbid.MDBID
                                   select new
                                   {
                                       PlaceName = t.City,
                                       GeoLat = t.Latitude,
                                       GeoLong = t.Longitude,
                                       OrgName = t.OrganizationName
                                   }).ToList();
                }

            }
            return Json(selectedRow, JsonRequestBehavior.AllowGet);
        }
        
        // For Machine Invoiced (GREEN)
        [HttpGet]
        public JsonResult MachineInvoicedMapdetails(int cpid = 0)
        {
            var selectedRow = (from t in db.MDBGeneralData
                               where t.IsDeleted == 10 && t.Latitude != null
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName
                               }).ToList();

            string[] selectedRowres = null; int i = 0;

            var mdblist = db.OAEquipGeneralData.Where(m => m.CPID == cpid).Where(m => m.OAStatus == 1).ToList();
            if (cpid == 0)
            {
                foreach (var mdb in mdblist)
                {
                    selectedRow = (from t in db.MDBGeneralData
                                   where t.IsDeleted == 0 && t.Latitude != null && t.MDBID == mdb.MDBID
                                   select new
                                   {
                                       PlaceName = t.City,
                                       GeoLat = t.Latitude,
                                       GeoLong = t.Longitude,
                                       OrgName = t.OrganizationName
                                   }).ToList();
                }
                //selectedRow.ToList().ForEach(s => { s.IsStatus = count; s.IsDrop = countdrop; });
            }
            else
            {
                //foreach (var mdb in mdblist)
                //{
                selectedRow = (from t in db.MDBGeneralData
                               from a in db.OAEquipGeneralData
                               where t.IsDeleted == 0 && t.Latitude != null && t.CPID == cpid && a.CPID == cpid && t.MDBID == a.MDBID && a.IsMacineDispatch == 1
                               select new
                               {
                                   PlaceName = t.City,
                                   GeoLat = t.Latitude,
                                   GeoLong = t.Longitude,
                                   OrgName = t.OrganizationName
                               }).ToList();
                //selectedRowres[i] = selectedRow.ToArray[];
                // i++;
                //}                         
            }
            return Json(selectedRow, JsonRequestBehavior.AllowGet);
        }

        #endregion


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


        //Json AutoComplete MilName of LeadEnquiryRevised
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.LeadEnquiryRevised
                          where r.MillName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          select new { r.MillName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        //public JsonResult GettingStateNames(string term)
        //{
        //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
        //    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

        //    var result = (from r in db.DistrictPinCodeDetails_tbl
        //                  where r.State.ToLower().Contains(term.ToLower())
        //                  select new { r.State }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public JsonResult GetDistrictName(string id)
        //{
        //    var result = (from r in db.DistrictPinCodeDetails_tbl
        //                  where r.State == id
        //                  select new { r.District }).Distinct();

        //    //var selectedRow = (from t in db.ProductModel where t.ProductModelID == id select t).SingleOrDefault();

        //    //var jsonData = new
        //    //{
        //    //    unitprice = result.               
        //    //};
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        // Google map filters
        public ActionResult MapFilters()
        {
            //ViewBag.CPID = new SelectList(db.ChannelPartners, "CPID", "CPName");
            var logcpid = Session["logincpid"];
            int zoind = Convert.ToInt32(Session["Zoneid"]);
            int cpidint = Convert.ToInt32(logcpid);

            if (User.IsInRole("ZonalManager"))
            {
                ViewBag.CPID = new SelectList(db.ChannelPartners.Where(m => m.ZoneID == zoind), "CPID", "CPName");
            }
            else
            {
                ViewBag.CPID = new SelectList(db.ChannelPartners, "CPID", "CPName");
            }

            return View();
        }

        //Post 
        [HttpPost]
        public ActionResult MapFilters(string CPID)
        {
            int cpid1 = 0;
            if (CPID != null)
            {
                cpid1 = Convert.ToInt32(CPID);
            }
            else
            {
                cpid1 = 0;
            }
            return RedirectToAction("Map", "LeadEnquiry", new { cpid = cpid1 });

            //return RedirectToAction("Map", "LeadEnquiry", null);
        }

        //get for Map
        public ActionResult Map(int cpid = 0)
        {
            ViewBag.cpid = cpid;
            //var selectedRow = (from t in db.MDBGeneralData
            //                   where t.IsDeleted == 10
            //                   select new
            //                   {
            //                       PlaceName = t.City,
            //                       GeoLat = t.Latitude,
            //                       GeoLong = t.Longitude

            //                   }).ToList();
            //if (cpid != null)
            //{
            //     selectedRow = (from t in db.MDBGeneralData
            //                       where t.IsDeleted == 0 && t.Latitude != null
            //                       select new
            //                       {
            //                           PlaceName = t.City,
            //                           GeoLat = t.Latitude,
            //                           GeoLong = t.Longitude

            //                       }).ToList();
            //}
            //else
            //{
            //     selectedRow = (from t in db.MDBGeneralData
            //                       where t.IsDeleted == 0 && t.Latitude != null && t.CPID==cpid
            //                       select new
            //                       {
            //                           PlaceName = t.City,
            //                           GeoLat = t.Latitude,
            //                           GeoLong = t.Longitude

            //                       }).ToList();
            //}
            //return Json(selectedRow, JsonRequestBehavior.AllowGet);
            return View();
        }
        
        // GET: /LeadEnquiry/
        //[HttpGet]
        //public ActionResult LECreate()
        //{
        //    ViewData["ProductModelName"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelName", "ProductModelName");
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult LECreate(LeadEnquiry enquiry)
        //{
        //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
        //    int cpid = Convert.ToInt32(Session["logincpid"]);

        //    string l1 = null;
        //    string l2 = null;

        //    string fulladdress = enquiry.City + "," + enquiry.Pincode + "," + enquiry.State + "," + enquiry.Country;
        //    var state = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.State).SingleOrDefault();

        //    // code to find if pincode matches with state.
        //    // Given: cpid gives state
        //    //#region

        //    //int tick = 0;
        //    //try
        //    //{
        //    //    //string url = "http://maps.google.com/maps/api/geocode/xml?address=" + Convert.ToInt32(ler.Pincode) + "&sensor=false";
        //    //    // http://maps.googleapis.com/maps/api/geocode/json?address=560060&sensor=true
        //    //    string url = "http://maps.google.com/maps/api/geocode/json?address=" + Convert.ToInt32(enquiry.Pincode) + "&sensor=false";

        //    //    using (var webClient = new System.Net.WebClient())
        //    //    {
        //    //        var json = webClient.DownloadString(url);
        //    //        // Now parse with JSON.Net
        //    //        // DataTable dtValue = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

        //    //        //Object jObject = JsonConvert.DeserializeObject<JObject>(json);
        //    //        // DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jObject.ToString());

        //    //        JavaScriptSerializer jss = new JavaScriptSerializer();
        //    //        GoogleGeoCodeResponse test = jss.Deserialize<GoogleGeoCodeResponse>(json);

        //    //        //string address = test.results[0].formatted_address;
        //    //        //int count = test.results.Count();

        //    //        for (int i = 0; i < test.results.Count(); i++)
        //    //        {
        //    //            string address = test.results[i].formatted_address;
        //    //            //usually addresses are of format: ("formatted_address" : "Haryana 134202, India")
        //    //            //code to handle address of format: ("formatted_address" : "125075, India")

        //    //            char firstChar = address[0];
        //    //            if (char.IsLetter(firstChar))
        //    //            {
        //    //                //prepare state name
        //    //                string justState = null;
        //    //                justState = state.Substring(0, state.IndexOf('('));

        //    //                bool containsS = address.IndexOf(justState, StringComparison.OrdinalIgnoreCase) >= 0;
        //    //                // bool containsC = address.IndexOf("India", StringComparison.OrdinalIgnoreCase) >= 0;
        //    //                //if (containsS || containsC)
        //    //                if (containsS)
        //    //                {
        //    //                    l1 = test.results[i].geometry.location.lat.ToString();
        //    //                    l2 = test.results[i].geometry.location.lng.ToString();
        //    //                    tick = 1;
        //    //                    break;
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                bool containsC = address.IndexOf("India", StringComparison.OrdinalIgnoreCase) >= 0;
        //    //                if (containsC)
        //    //                {
        //    //                    l1 = test.results[i].geometry.location.lat.ToString();
        //    //                    l2 = test.results[i].geometry.location.lng.ToString();
        //    //                    tick = 1;
        //    //                    break;
        //    //                }
        //    //            }
        //    //        }
        //    //    }

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ex.GetHashCode();
        //    //}

        //    //if (tick == 0)
        //    //{
        //    //    Session["WrongPincode"] = "Invalid Pincode";
        //    //    return RedirectToAction("LECreateRevised");
        //    //}
        //    //#endregion


        //    // Finds the Latitude and Longitude of the address
        //    //#region  
        //    //try
        //    //{
        //    //    string url = "http://maps.google.com/maps/api/geocode/xml?address=" + fulladdress + "&sensor=false";
        //    //    WebRequest request = WebRequest.Create(url);
        //    //    using (WebResponse response = (HttpWebResponse)request.GetResponse())
        //    //    {
        //    //        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        //    //        {
        //    //            DataSet dsResult = new DataSet();
        //    //            dsResult.ReadXml(reader);
        //    //            DataTable dtCoordinates = new DataTable();
        //    //            dtCoordinates.Columns.AddRange(new DataColumn[2] 
        //    //            { 
        //    //                //new DataColumn("Id", typeof(int)),
        //    //                //new DataColumn("Address", typeof(string)),
        //    //                new DataColumn("Latitude",typeof(string)),
        //    //                new DataColumn("Longitude",typeof(string))
        //    //            });
        //    //            foreach (DataRow row in dsResult.Tables["result"].Rows)
        //    //            {
        //    //                string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
        //    //                DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
        //    //                //dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);

        //    //                dtCoordinates.Rows.Add(location["lat"], location["lng"]);

        //    //                l1 = Convert.ToString(location["lat"]);
        //    //                l2 = Convert.ToString(location["lng"]);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    fulladdress = enquiry.Pincode;
        //    //    string url = "http://maps.google.com/maps/api/geocode/xml?address=" + fulladdress + "&sensor=false";
        //    //    WebRequest request = WebRequest.Create(url);
        //    //    using (WebResponse response = (HttpWebResponse)request.GetResponse())
        //    //    {
        //    //        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        //    //        {
        //    //            DataSet dsResult = new DataSet();
        //    //            dsResult.ReadXml(reader);
        //    //            DataTable dtCoordinates = new DataTable();
        //    //            dtCoordinates.Columns.AddRange(new DataColumn[2] 
        //    //            { 
        //    //                //new DataColumn("Id", typeof(int)),
        //    //                //new DataColumn("Address", typeof(string)),
        //    //                new DataColumn("Latitude",typeof(string)),
        //    //                new DataColumn("Longitude",typeof(string))
        //    //            });
        //    //            foreach (DataRow row in dsResult.Tables["result"].Rows)
        //    //            {
        //    //                string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
        //    //                DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
        //    //                //dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);

        //    //                dtCoordinates.Rows.Add(location["lat"], location["lng"]);

        //    //                l1 = Convert.ToString(location["lat"]);
        //    //                l2 = Convert.ToString(location["lng"]);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //#endregion

        //    //converting to upper case.
        //    enquiry.OrganizationName = enquiry.OrganizationName.ToUpper();
        //    enquiry.OrganizationType = enquiry.OrganizationType.ToUpper();
        //    enquiry.AddressLine1 = enquiry.AddressLine1.ToUpper();
        //    enquiry.AddressLine2 = enquiry.AddressLine2.ToUpper();
        //    if (enquiry.AddressLine3 != null)
        //    {
        //        enquiry.AddressLine3 = enquiry.AddressLine3.ToUpper();
        //    }
        //    enquiry.City = enquiry.City.ToUpper();
        //    enquiry.State = enquiry.State.ToUpper();
        //    enquiry.Country = enquiry.Country.ToUpper();
        //    enquiry.Prefix = enquiry.Prefix.ToUpper();
        //    enquiry.FirstName = enquiry.FirstName.ToUpper();
        //    if (enquiry.MiddleName != null)
        //    {
        //        enquiry.MiddleName = enquiry.MiddleName.ToUpper();
        //    }
        //    if (enquiry.LastName != null)
        //    {
        //        enquiry.LastName = enquiry.LastName.ToUpper();
        //    }
        //    if (enquiry.LeadSource != null)
        //    {
        //        enquiry.LeadSource = enquiry.LeadSource.ToUpper();
        //    }
        //    //end upper case

        //    //if (mdate != null)
        //    //{

        //    //    enquiry.DateOfMeeting =Convert.ToDateTime(mdate);
        //    //}
        //    //else
        //    //{
        //    //    enquiry.DateOfMeeting = System.DateTime.Now;
        //    //}

        //    enquiry.Latitude = l1; //Storing lat and long
        //    enquiry.Longitude = l2;

        //    var time = System.DateTime.Now.TimeOfDay;
        //    string test1 = Convert.ToString(time);
        //    TimeSpan tp = TimeSpan.Parse(test1);
        //    enquiry.LeadTime = (tp.ToString(@"hh\:mm"));
        //    enquiry.IsTime = 0;
        //    enquiry.IsDeleted = 0;
        //    enquiry.IsStatus = 0;
        //    enquiry.IsDrop = 0;
        //    enquiry.IsCount = 0;
        //    enquiry.IsHOD = 0;
        //    enquiry.NotifyDate = System.DateTime.Now;
        //    DateTime dat = System.DateTime.Now;
        //    var dat2 = dat.ToString("yyyy-MM-dd");
        //    enquiry.CreatedOn = Convert.ToDateTime(dat2);
        //    enquiry.CreatedBy = Convert.ToString(userid);
        //    if (cpid != 0)
        //    {
        //        enquiry.CPID = cpid;
        //    }
        //    else
        //    {
        //        enquiry.CPID = 0;
        //    }
        //    db.LeadEnquiry.Add(enquiry);
        //    db.SaveChanges();

        //    //stroing in OverAllLeadStatus Table
        //    //#region
        //    //OverAllLeadStatus ols = new OverAllLeadStatus();
        //    //ols.LEID = enquiry.LEID;
        //    //ols.ID = enquiry.LEID;
        //    //ols.IsIdStatus = 0;  // 0 value indicates Lead Generated
        //    //ols.Date = System.DateTime.Now;
        //    //var tim = System.DateTime.Now.TimeOfDay;
        //    //string tst1 = Convert.ToString(tim);
        //    //TimeSpan tp1 = TimeSpan.Parse(tst1);
        //    //ols.Time = Convert.ToDateTime(tp1.ToString("hh:mm:ss tt"));
        //    //if (cpid != 0)
        //    //{
        //    //    ols.CPID = cpid;
        //    //}
        //    //else
        //    //{
        //    //    ols.CPID = 0;
        //    //}
        //    //db.OverAllLeadStatus.Add(ols);
        //    //db.SaveChanges();
        //    //#endregion
        //    // ends for OverAllStatus table.

        //    //Mail Module
        //    string module = enquiry.SuggestedModel;
        //    string orgname = enquiry.OrganizationName;
        //    DateTime lastdate = System.DateTime.Now;
        //    int flag = 1;
        //    //mailservices1 objMD = new mailservices1();
        //    //objMD.sendMailLEAD(cpid, module, lastdate, flag, orgname);
        //    //end
        //    TempData["Success"] = "Data Saved Successfully!!!!";
        //    return RedirectToAction("LEIndex");
        //}

        //public ActionResult LEIndex()
        //{
        //    //// Storing Latitude and Longitude                 
        //    //#region
        //    //var master = db.MDBGeneralData.Where(m => m.IsDeleted == 222).ToList();
        //    //int countmdbid = master.Count;
        //    //try
        //    //{
        //    //    foreach (var MDB in master)
        //    //    {
        //    //        int mdbid = MDB.MDBID;
        //    //        var fulladdress = MDB.AddressLine1 + "," + MDB.AddressLine2 + "," + MDB.City + "," + MDB.Pincode + "," + MDB.State + "," + MDB.Country;

        //    //        mailservices1 objMD = new mailservices1();
        //    //        objMD.StoreLatLang(fulladdress, mdbid);
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    foreach (var MDB in master)
        //    //    {
        //    //        int mdbid = MDB.MDBID;
        //    //        var fulladdress = MDB.State + "," + MDB.Country;

        //    //        mailservices1 objMD = new mailservices1();
        //    //        objMD.StoreLatLang(fulladdress, mdbid);
        //    //    }

        //    //}
        //    //#endregion
        //    ////ends latitude and longitude

        //    int cpid = Convert.ToInt32(Session["logincpid"]);
        //    var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m=>m.CPID==cpid).ToList();
        //    return View(list);
        //}        

        //[HttpGet]
        //public ActionResult LEEdit(int id = 0)
        //{          
        //    LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(id);
        //    if (LeadEnquiry == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ProductModelName = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelName", "ProductModelName");
        //    return View(LeadEnquiry);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LEEdit(LeadEnquiry LeadEnquiry)
        //{
        //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
        //    if (LeadEnquiry.LEID != 0)
        //    {
        //        //converting to upper case.
        //        LeadEnquiry.OrganizationName = LeadEnquiry.OrganizationName.ToUpper();
        //        LeadEnquiry.OrganizationType = LeadEnquiry.OrganizationType.ToUpper();
        //        LeadEnquiry.AddressLine1 = LeadEnquiry.AddressLine1.ToUpper();
        //        LeadEnquiry.AddressLine2 = LeadEnquiry.AddressLine2.ToUpper();
        //        if (LeadEnquiry.AddressLine3 != null)
        //        {
        //            LeadEnquiry.AddressLine3 = LeadEnquiry.AddressLine3.ToUpper();
        //        }
        //        LeadEnquiry.City = LeadEnquiry.City.ToUpper();
        //        LeadEnquiry.State = LeadEnquiry.State.ToUpper();
        //        LeadEnquiry.Country = LeadEnquiry.Country.ToUpper();
        //        LeadEnquiry.Prefix = LeadEnquiry.Prefix.ToUpper();
        //        LeadEnquiry.FirstName = LeadEnquiry.FirstName.ToUpper();
        //        if (LeadEnquiry.MiddleName != null)
        //        {
        //            LeadEnquiry.MiddleName = LeadEnquiry.MiddleName.ToUpper();
        //        }
        //        if (LeadEnquiry.LastName != null)
        //        {
        //            LeadEnquiry.LastName = LeadEnquiry.LastName.ToUpper();
        //        }
        //        if (LeadEnquiry.LeadSource != null)
        //        {
        //            LeadEnquiry.LeadSource = LeadEnquiry.LeadSource.ToUpper();
        //        }
        //        //end upper case              

        //        LeadEnquiry.ModifiedBy = Convert.ToString(userid);
        //        LeadEnquiry.ModifiedOn = System.DateTime.Now;
        //        db.Entry(LeadEnquiry).State = EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["Success"] = "Record Modified Successfully!!!!";
        //        return RedirectToAction("LEIndex");
        //    }
        //    ViewBag.ProductModelName = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelName", "ProductModelName");
        //    return View("LEIndex");
        //}

        //// GET: 
        //public ActionResult LEDelete(int id = 0)
        //{
        //    LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(id);
        //    if (LeadEnquiry == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(LeadEnquiry);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LEDelete(LeadEnquiry leadenquiry, int id)
        //{
        //    if (Request.Form["Yes"] != null)
        //    {
        //        LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(id);
        //        LeadEnquiry.IsDeleted = 1;
        //        db.Entry(LeadEnquiry).State = EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["Success"] = "Record Deleted Successfully!!!!";
        //        return RedirectToAction("LEIndex");
        //    }
        //    return RedirectToAction("LEIndex");
        //}

        //public ActionResult DropEnquiry(int id)
        //{           
        //        LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(id);
        //        int isdrop = db.LeadEnquiry.Where(m => m.LEID==id).Select(m => m.IsDrop).SingleOrDefault();
        //        LeadEnquiry.IsDrop =isdrop+1;
        //        LeadEnquiry.IsStatus = 2;
        //        db.Entry(LeadEnquiry).State = EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["Success"] = "Record Dropped,You cannot Convert and Generate Quotation...!!!!";
        //    return RedirectToAction("LEIndex");
        //}

        //public ActionResult EnableEnquiry(int id)
        //{
        //    LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(id);         
        //    LeadEnquiry.IsStatus = 0;
        //    db.Entry(LeadEnquiry).State = EntityState.Modified;
        //    db.SaveChanges();
        //    TempData["Success"] = "Record Is Enabled, You Can Convert Now...!!!!";
        //    return RedirectToAction("LEIndex");
        //}

        //public ActionResult ConvertToMDB(int id = 0)
        //{
        //    int cpid = Convert.ToInt32(Session["logincpid"]);
        //    LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(id);

        //    var mdid = from CompanyUniqueID in db.MDBGeneralData select CompanyUniqueID;
        //    mdid = mdid.OrderByDescending(m => m.MDBID);
        //    var check = mdid.FirstOrDefault();
        //      var mdbi = mdid.Select(m => m.CompanyUniqueID).First();
        //        string[] split = mdbi.Split('-');                
        //        int k = Convert.ToInt32(split[2]);
        //        int ad = k + 1;
        //        string cpik = null;
        //        string len = ad.ToString();
        //        if (len.Length == 1)
        //        {
        //            cpik = "0000" + ad;
        //        }
        //        else if (len.Length == 2)
        //        {
        //            cpik = "000" + ad;
        //        }
        //        else if (len.Length == 3)
        //        {
        //            cpik = "00" + ad;
        //        }
        //        else if (len.Length == 4)
        //        {
        //            cpik = "0" + ad;
        //        }
        //        else
        //        {
        //            cpik = ad.ToString();
        //        }
        //        mdbi = split[0] + "-" + System.DateTime.Now.Year + "-" + cpik;

        //        MDBGeneralData mdb = new MDBGeneralData();
        //       // Data oving from one Lead table to MDB table
        //        mdb.CompanyUniqueID = mdbi;
        //        mdb.OrganizationName = LeadEnquiry.OrganizationName;
        //        mdb.OrganizationType = LeadEnquiry.OrganizationType;

        //        mdb.AddressLine1 = LeadEnquiry.AddressLine1;
        //        mdb.AddressLine2 = LeadEnquiry.AddressLine2;                
        //        mdb.AddressLine3 = LeadEnquiry.AddressLine3;               

        //        mdb.City = LeadEnquiry.City;
        //        mdb.Pincode = LeadEnquiry.Pincode;
        //        mdb.State = LeadEnquiry.State;
        //        mdb.Country = LeadEnquiry.Country;

        //        mdb.Isd1 = LeadEnquiry.Isd1;
        //        mdb.Std1 = LeadEnquiry.Std1;
        //        mdb.PhoneLL1 = LeadEnquiry.PhoneLL1;
        //        mdb.EmailID = LeadEnquiry.EmailID;

        //        mdb.CPID = LeadEnquiry.CPID;

        //        db.MDBGeneralData.Add(mdb);
        //        SaveChanges();

        //        int mdbid = mdb.MDBID;

        //        MDBContactPersonData mdbcontact = new MDBContactPersonData();
        //        mdbcontact.MDBID = mdbid;
        //        mdbcontact.Title = LeadEnquiry.Prefix;
        //        //mdbcontact.Titleothers = LeadEnquiry.Prefix;
        //        mdbcontact.FirstName = LeadEnquiry.FirstName;
        //        mdbcontact.MiddleName = LeadEnquiry.MiddleName;
        //        mdbcontact.LastName = LeadEnquiry.LastName;

        //        db.MDBContactPersonData.Add(mdbcontact);
        //        db.SaveChanges();

        //       //Closing the Status in LeadEnquiry Form
        //        LeadEnquiry.IsStatus = 1;
        //        db.Entry(LeadEnquiry).State = EntityState.Modified;
        //        db.SaveChanges();

        //        TempData["Success"] = "Record Converted to Master Database and Saved Successfully...!!!";
        //        return RedirectToAction("LEIndex","LeadEnquiry",null);
        //    }

        //
        //public int SaveChanges()
        //{
        //    try
        //    {
        //        return db.SaveChanges();
        //    }

        //    catch (DbEntityValidationException ex)
        //    {
        //        // Retrieve the error messages as a list of strings.
        //        var errorMessages = ex.EntityValidationErrors
        //            .SelectMany(x => x.ValidationErrors)
        //                .Select(x => x.ErrorMessage);

        //        // Join the list to a single string.
        //        var fullErrorMessage = string.Join("; ", errorMessages);

        //        // Combine the original exception message with the new one.           
        //        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

        //        // Throw a new DbEntityValidationException with the improved exception message.
        //        throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
        //    }
        //}

        public ActionResult TotalLeads()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;

            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date

            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            int exp = list.Count();

            var a = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate1);
            int count = 0;
            int ExitCount = 0;
            int cpid = 0;
            foreach (var j in list)
            {
                cpid = j.CPID;
                foreach (var b in a)
                {
                    if (cpid == b.CPID)
                    {
                        count++;
                    }
                }
                j.IsStatus = count;
                //
                //Code for updating the list
                list.Where(m => m.CPID == cpid).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate1).ToList().ForEach(s => s.IsStatus = count);

                ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
                ViewBag.Todate = enddate.ToString("dd-MM-yyyy");
                count = 0;
                ExitCount++;
                if (ExitCount == exp)
                    return View(list);
            }
            return View();
        }

        public ActionResult TotalLeadsWithParticularDates()
        {
            return View();
        }

        public ActionResult DisplayTotalLeadsWithParticularDates(string fromdat, string todat)
        {

            DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            String frda1 = dtt.ToString("yyyy-MM-dd");
            String toda1 = dtt1.ToString("yyyy-MM-dd");

            DateTime frda = DateTime.Parse(frda1).Date;
            DateTime toda = DateTime.Parse(toda1).Date;

            Session["Fromdate"] = fromdat;
            Session["Todate"] = todat;


            DateTime now = DateTime.Now;

            var startDate = frda;
            var enddate = toda;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            //var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn > startDate && m.CreatedOn < enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            //  int exp = list.Count();

            //DateTime fromdate = Convert.ToDateTime(fromdat);
            //DateTime todate = Convert.ToDateTime(todat);

            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn >= frda && m.CreatedOn <= toda).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            int exp = list.Count();

            var a = (from r in db.LeadEnquiry
                     where r.IsDeleted == 0 && r.CreatedOn >= frda && r.CreatedOn <= toda
                     select r).ToList();


            int count = 0; int tc = 0;
            int ExitCount = 0;
            int cpid = 0;
            #region

            foreach (var j in list)
            {
                cpid = j.CPID;
                foreach (var b in a)
                {
                    if (cpid == b.CPID)
                    {
                        count++;
                    }
                }
                j.IsStatus = count;

                //to get total leads within date range               
                tc = tc + count;
                ViewBag.tc = tc;
                //
                //Code for updating the list
                list.Where(m => m.CPID == cpid).Where(m => m.CreatedOn >= frda && m.CreatedOn <= toda).ToList().ForEach(s => s.IsStatus = count);


                count = 0;
                ExitCount++;
                if (ExitCount == exp)
                    return View(list);
            }
            #endregion

            return View();
        }

        public ActionResult OpenCloseLeads()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            int exp = list.Count();

            ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Todate = enddate.ToString("dd-MM-yyyy");

            return View(list);
        }

        public ActionResult OpenCloseLeadsWithParticularDates()
        {
            return View();
        }

        public ActionResult DisplayOpenCloseLeadsWithParticularDates(string fromdat, string todat)
        {
            DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            String frda1 = dtt.ToString("yyyy-MM-dd");
            String toda1 = dtt1.ToString("yyyy-MM-dd");

            DateTime frda = DateTime.Parse(frda1).Date;
            DateTime toda = DateTime.Parse(toda1).Date;

            ViewBag.Fromdateshow = fromdat;
            ViewBag.Todateshow = todat;

            var startDate = frda;
            var enddate1 = toda;

            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn > startDate && m.CreatedOn < enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            //var list = (from r in db.LeadEnquiry
            //            where r.IsDeleted == 0 && r.CreatedOn >= startDate && r.CreatedOn <= enddate1
            //            select r).ToList();
            int exp = list.Count();

            ViewBag.Fromdate = frda;
            ViewBag.Todate = toda;

            return View(list);
        }

        //Orders Generated VIew
        public ActionResult OrdersGenerated()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);

            //var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m=>m.IsTime==0).Where(m=>m.Ordergenerated==1).Where(m => m.QuotationDate >= startDate && m.QuotationDate <= enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            var list = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == 1 || m.ApprovalStatus == 2).Where(m => m.OADate >= startDate && m.OADate <= enddate1).ToList();
            int exp = list.Count();

            ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Todate = enddate.ToString("dd-MM-yyyy");

            return View(list);
        }

        public ActionResult GeneratedOrdersView()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var list = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == 1 || m.ApprovalStatus == 2).Where(m => m.OADate >= startDate && m.OADate <= enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            //var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            int exp = list.Count();

            ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Todate = enddate.ToString("dd-MM-yyyy");

            return View(list);
        }

        public ActionResult OrderGeneratedParticularDates()
        {
            return View();
        }

        public ActionResult OrdersGeneratedWithDateRange(string fromdat, string todat)
        {
            DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            String frda1 = dtt.ToString("yyyy-MM-dd");
            String toda1 = dtt1.ToString("yyyy-MM-dd");

            DateTime frda = DateTime.Parse(frda1).Date;
            DateTime toda = DateTime.Parse(toda1).Date;

            ViewBag.Fromdateshow = fromdat;
            ViewBag.Todateshow = todat;

            var startDate = frda;
            var enddate1 = toda;

            var list = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == 1 || m.ApprovalStatus == 2).Where(m => m.OAStatus == 1).Where(m => m.OADate > startDate && m.OADate < enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            //var list = (from r in db.LeadEnquiry
            //            where r.IsDeleted == 0 && r.CreatedOn >= startDate && r.CreatedOn <= enddate1
            //            select r).ToList();
            int exp = list.Count();

            ViewBag.Fromdate = frda;
            ViewBag.Todate = toda;

            return View(list);
        }

        [HttpGet]
        public ActionResult jViewLeadsIndividual(int id)
        {

            string fdate = Session["Fromdate"].ToString();
            string tdate = Session["Todate"].ToString();

            DateTime dtt = DateTime.ParseExact(fdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtt1 = DateTime.ParseExact(tdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            String frda1 = dtt.ToString("yyyy-MM-dd");
            String toda1 = dtt1.ToString("yyyy-MM-dd");

            DateTime frda = DateTime.Parse(frda1).Date;
            DateTime toda = DateTime.Parse(toda1).Date;

            DateTime startDate = frda;
            DateTime enddate = toda;

            var list = (from r in db.LeadEnquiry
                        where r.IsDeleted == 0 && r.CPID == id && r.CreatedOn >= startDate && r.CreatedOn <= enddate
                        select r).ToList();
            //db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CPID == id).Where(m => m.CreatedOn > startDate && m.CreatedOn < enddate1).ToList();
            return View(list);
        }

        public ActionResult QuotationGenerated()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate >= startDate && m.QuotationDate <= enddate1).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            int exp = list.Count();

            ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Todate = enddate.ToString("dd-MM-yyyy");

            return View(list);
        }

        public ActionResult QuotationWithParticularDates()
        {
            return View();
        }

        public ActionResult DisplayQuotationWithParticularDates(string fromdat, string todat)
        {
            DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            String frda1 = dtt.ToString("yyyy-MM-dd");
            String toda1 = dtt1.ToString("yyyy-MM-dd");

            DateTime frda = DateTime.Parse(frda1).Date;
            DateTime toda = DateTime.Parse(toda1).Date;


            Session["Fromdate"] = fromdat;
            Session["Todate"] = todat;

            var startDate = frda;
            var enddate1 = toda;

            var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate >= frda && m.QuotationDate <= toda).GroupBy(m => m.CPID).Select(m => m.FirstOrDefault()).ToList();
            //var list = (from r in db.QGEquipGeneralData
            //            where r.IsRiceMill == 0 && r.QuotationDate >= startDate && r.QuotationDate <= enddate1
            //            group r by r.CPID).ToList();

            ViewBag.Fromdate = frda;
            ViewBag.Todate = toda;

            int exp = list.Count();
            return View(list);
        }

        [HttpGet]
        public ActionResult jViewQuotationIndividual(int id)
        {
            //string fdate = Session["Fromdate"].ToString();
            //string tdate = Session["Todate"].ToString();

            //DateTime startDate = Convert.ToDateTime(fdate);
            //DateTime enddate = Convert.ToDateTime(tdate);

            ///
            string fdate = Session["Fromdate"].ToString();
            string tdate = Session["Todate"].ToString();

            DateTime dtt = DateTime.ParseExact(fdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtt1 = DateTime.ParseExact(tdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            String frda1 = dtt.ToString("yyyy-MM-dd");
            String toda1 = dtt1.ToString("yyyy-MM-dd");

            DateTime frda = DateTime.Parse(frda1).Date;
            DateTime toda = DateTime.Parse(toda1).Date;

            DateTime startDate = frda;
            DateTime enddate = toda;
            ///

            //var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate > startDate && m.QuotationDate < enddate1).Where(m => m.CPID == id).ToList();
            var list = (from r in db.QGEquipGeneralData
                        where r.IsRiceMill == 0 && r.CPID == id && r.QuotationDate >= startDate && r.QuotationDate <= enddate
                        select r).ToList();

            ViewBag.Fromdate = frda;
            ViewBag.Todate = toda;

            ViewBag.id = id;
            return View(list);
        }

        //New Generated Leads
        public ActionResult LeadGeneratedNotify()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var yesterday = DateTime.Today.AddDays(-1);

            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CreatedOn > startDate && m.CreatedOn < enddate).OrderByDescending(m => m.LEID).ToList();
            foreach (var k in list)
            {
                int leid = k.LEID;
                var list1 = db.LeadEnquiry.Where(m => m.LEID == leid).SingleOrDefault();
                list1.IsTime = 1;
                db.Entry(list1).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View(list);
        }
        //Dropped Leads 
        public ActionResult DropLeadsNotify()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var yesterday = DateTime.Today.AddDays(-1);

            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.IsTime == 0).Where(m => m.IsStatus == 2).Where(m => m.CreatedOn > yesterday && m.CreatedOn < enddate).OrderByDescending(m => m.LEID).ToList();
            foreach (var k in list)
            {
                int leid = k.LEID;
                var list1 = db.LeadEnquiry.Where(m => m.LEID == leid).SingleOrDefault();
                list1.IsTime = 1;
                db.Entry(list1).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(list);
        }

        //New Quotation Generated
        public ActionResult NewQuotationNotify()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var yesterday = DateTime.Today.AddDays(-1);

            var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate >= startDate && m.QuotationDate <= enddate1).OrderByDescending(m => m.QGID).ToList();
            //var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate >=startDate && m.QuotationDate <=enddate).OrderByDescending(m => m.QGID).ToList();
            foreach (var k in list)
            {
                int qgid = k.QGID;
                var list1 = db.QGEquipGeneralData.Where(m => m.QGID == qgid).SingleOrDefault();
                list1.IsTime = 1;
                db.Entry(list1).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(list);
        }

        // Method/view for checking the Organization name in LeadEnquiry table.
        public ActionResult LESearch()
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var getcpid = loginname.CPID;

            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CPID == getcpid).ToList();
            return View(list);
        }

        [HttpPost]
        public ActionResult LESearch(int page = 1, int sortBy = 1, bool isAsc = true, string custnm = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var getcpid = loginname.CPID;
            //Paging and Sorting //
            IEnumerable<LeadEnquiry> chanpart = db.LeadEnquiry.Where(
                    p => custnm == null
                    || p.OrganizationName.Contains(custnm)).Where(m => m.CPID == getcpid);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LEID) : chanpart.OrderByDescending(p => p.LEID);
                    break;

                case 2:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationName) : chanpart.OrderByDescending(p => p.OrganizationName);
                    break;

                case 3:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationType) : chanpart.OrderByDescending(p => p.OrganizationType);
                    break;

                case 4:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.AddressLine1) : chanpart.OrderByDescending(p => p.AddressLine1);
                    break;

                case 5:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
                    break;

                case 6:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 7:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.PhoneLL1) : chanpart.OrderByDescending(p => p.PhoneLL1);
                    break;

                case 8:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.EmailID) : chanpart.OrderByDescending(p => p.EmailID);
                    break;

                default:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LEID) : chanpart.OrderByDescending(p => p.LEID);
                    break;
            }
            #endregion
            ViewBag.Search = custnm;
            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (custnm != null)
                ViewBag.IsSearch = true;
            return View(chanpart);
        }

        [HttpGet]
        public ActionResult ViewLeadsIndividual(int id)
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);

            ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Todate = enddate.ToString("dd-MM-yyyy");

            var list = (from r in db.LeadEnquiry
                        where r.IsDeleted == 0 && r.CPID == id && r.CreatedOn >= startDate && r.CreatedOn <= enddate
                        select r).ToList();
            //db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CPID == id).Where(m => m.CreatedOn > startDate && m.CreatedOn < enddate1).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult ViewQuotationIndividually(int id)
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);

            ViewBag.Fromdate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Todate = enddate.ToString("dd-MM-yyyy");

            //var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate > startDate && m.QuotationDate < enddate1).Where(m=>m.CPID==id).ToList();

            var list = (from r in db.QGEquipGeneralData
                        where r.IsRiceMill == 0 && r.CPID == id && r.QuotationDate >= startDate && r.QuotationDate <= enddate1
                        select r).ToList();
            ViewBag.id = id;
            //var prodmodelname = (string)null;
            //string[] prodname = new string[100];
            //int i = 0;
            //foreach (var l in list)
            //{
            //    int qgid1 = l.QGID;
            //    string quono = l.QuotationNumber;            

            //    var qgtbid = db.QGEquipTableData.Where(m => m.QGID == qgid1).ToList();

            //    foreach (var q in qgtbid)
            //    {
            //         prodmodelname = db.QGEquipTableData.Where(m=>m.QGTBID==q.QGTBID).Select(m => m.ProductModel.ProductModelName).SingleOrDefault();                                        
            //    }

            //    prodname[i] = prodmodelname;
            //    i++;

            //    ViewBag.prodmodelname = prodname[i];
            //    list.Where(m => m.QuotationDate > startDate && m.QuotationDate < enddate1).ToList().ForEach(s => { s.SalesName = prodname[i]; });
            //}
            return View(list);
        }

        [HttpGet]
        public ActionResult LECreateRevised()
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int cpid = Convert.ToInt32(Session["logincpid"]);

            DateTime dd = System.DateTime.Now;
            string dd1 = dd.ToString("dd-MM-yyyy");
            ViewBag.date = dd1;

            //var statelist = db.DistrictPinCodeDetails_tbl.Where(m => m.IsDeleted == 0).ToList();

            //States_tbl States_tbl = new States_tbl();

            //foreach (var st in statelist)
            //{
            //    int stcount=db.States_tbl.Where(m=>m.IsDeleted==0).Where(m=>m.State==st.State).Count();
            //    if(stcount==0)
            //    {
            //        States_tbl.State=st.State;
            //        States_tbl.IsDeleted=0;
            //        States_tbl.CreatedBy=cpid;
            //        States_tbl.CreatedOn=System.DateTime.Now;
            //        db.States_tbl.Add(States_tbl);
            //        db.SaveChanges();
            //    }
            //}

            ViewData["State"] = new SelectList(db.States_tbl.Where(m => m.IsDeleted == 0), "State", "State");

            ViewData["District"] = new SelectList(db.DistrictPinCodeDetails_tbl.Where(m => m.IsDeleted == 100), "District", "District");

            ViewData["PINCode"] = new SelectList(db.DistrictPinCodeDetails_tbl.Where(m => m.IsDeleted == 100), "PINCode", "PINCode");

            var CPName = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
            ViewBag.CPName = CPName;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LECreateRevised(LeadEnquiryRevised ler, bool addcitypinchk, string citytxt = null, string pincodetxt=null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int cpid = Convert.ToInt32(Session["logincpid"]);
            string sessionChanelPartnerId = Session["logincpid"].ToString();
            if (sessionChanelPartnerId == "" || sessionChanelPartnerId == null)
            {
                Session.Contents.RemoveAll();
                return RedirectToAction("LogOff", "Account", null);
            }
            string l1 = null;
            string l2 = null;

            if (addcitypinchk == true)
            {
                var stateid = db.States_tbl.Where(m => m.State == ler.State).Select(m => m.StateID).SingleOrDefault();

                //Adding State, City and PIN Code into DistrictPinCodeDetails_tbl
                DistrictPinCodeDetails DistrictPinCodeDetails_tbl = new DistrictPinCodeDetails();
                DistrictPinCodeDetails_tbl.State = ler.State;
                DistrictPinCodeDetails_tbl.District = citytxt.ToUpper();
                DistrictPinCodeDetails_tbl.PINCode = Convert.ToInt32(pincodetxt);
                DistrictPinCodeDetails_tbl.IsDeleted = 0;
                DistrictPinCodeDetails_tbl.CreatedOn = System.DateTime.Now;
                DistrictPinCodeDetails_tbl.CreatedBy = cpid;
                DistrictPinCodeDetails_tbl.StateID = stateid;
                db.DistrictPinCodeDetails_tbl.Add(DistrictPinCodeDetails_tbl);
                db.SaveChanges();
            }

            string country = "INDIA";
            var state = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.State).SingleOrDefault();

            string fulladdress = ler.MillName + "," + ler.AddressLine1 + "," + ler.AddressLine2 + "," + ler.City + "," + ler.Pincode + "," + state + "," + country;

            // code to find if pincode matches with state.
            // Given: cpid gives state
            #region

            //int tick = 0;
            //try
            //{
            //    //string url = "http://maps.google.com/maps/api/geocode/xml?address=" + Convert.ToInt32(ler.Pincode) + "&sensor=false";
            //                 // http://maps.googleapis.com/maps/api/geocode/json?address=560060&sensor=true
            //    string url = "http://maps.google.com/maps/api/geocode/json?address=" + Convert.ToInt32(ler.Pincode) + "&sensor=false";

            //    using (var webClient = new System.Net.WebClient())
            //    {
            //        var json = webClient.DownloadString(url);
            //        // Now parse with JSON.Net
            //       // DataTable dtValue = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

            //        //Object jObject = JsonConvert.DeserializeObject<JObject>(json);
            //       // DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jObject.ToString());

            //        JavaScriptSerializer jss = new JavaScriptSerializer();
            //        GoogleGeoCodeResponse test = jss.Deserialize<GoogleGeoCodeResponse>(json);

            //        //string address = test.results[0].formatted_address;
            //        //int count = test.results.Count();

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
            //                // bool containsC = address.IndexOf("India", StringComparison.OrdinalIgnoreCase) >= 0;
            //                //if (containsS || containsC)
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
            #endregion

            //// Finds the Latitude and Longitude of the address
            #region
            //try
            //{
            //    string url = "http://maps.google.com/maps/api/geocode/xml?address=" + fulladdress + "&sensor=false";
            //    WebRequest request = WebRequest.Create(url);
            //    using (WebResponse response = (HttpWebResponse)request.GetResponse())
            //    {
            //        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            //        {
            //            DataSet dsResult = new DataSet();
            //            dsResult.ReadXml(reader);
            //            DataTable dtCoordinates = new DataTable();
            //            dtCoordinates.Columns.AddRange(new DataColumn[2] 
            //            { 
            //                //new DataColumn("Id", typeof(int)),
            //                //new DataColumn("Address", typeof(string)),
            //                new DataColumn("Latitude",typeof(string)),
            //                new DataColumn("Longitude",typeof(string))
            //            });
            //            foreach (DataRow row in dsResult.Tables["result"].Rows)
            //            {
            //                string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
            //                DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
            //                //dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);

            //                dtCoordinates.Rows.Add(location["lat"], location["lng"]);

            //                l1 = Convert.ToString(location["lat"]);
            //                l2 = Convert.ToString(location["lng"]);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    fulladdress = ler.Pincode;
            //    string url = "http://maps.google.com/maps/api/geocode/xml?address=" + fulladdress + "&sensor=false";
            //    WebRequest request = WebRequest.Create(url);
            //    using (WebResponse response = (HttpWebResponse)request.GetResponse())
            //    {
            //        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            //        {
            //            DataSet dsResult = new DataSet();
            //            dsResult.ReadXml(reader);
            //            DataTable dtCoordinates = new DataTable();
            //            dtCoordinates.Columns.AddRange(new DataColumn[2] 
            //            { 
            //                //new DataColumn("Id", typeof(int)),
            //                //new DataColumn("Address", typeof(string)),
            //                new DataColumn("Latitude",typeof(string)),
            //                new DataColumn("Longitude",typeof(string))
            //            });
            //            foreach (DataRow row in dsResult.Tables["result"].Rows)
            //            {
            //                string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
            //                DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
            //                //dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);

            //                dtCoordinates.Rows.Add(location["lat"], location["lng"]);

            //                l1 = Convert.ToString(location["lat"]);
            //                l2 = Convert.ToString(location["lng"]);
            //            }
            //        }
            //    }
            //}
            #endregion

            //checking condition for Duplicate value
            int count = db.LeadEnquiryRevised.Where(m => m.MillName == ler.MillName).Where(m=>m.State==ler.State).Where(m => m.City == ler.City).Where(m => m.Pincode == ler.Pincode).Where(m => m.CPID == cpid).Count();

            if (count == 0 || count == null)
            {
                //converting to Capital Letters
                #region
                ler.NameofCollector = ler.NameofCollector.ToUpper();
                ler.MillName = ler.MillName.ToUpper();
                ler.MillType = ler.MillType.ToUpper();
                ler.AddressLine1 = ler.AddressLine1.ToUpper();
                ler.AddressLine2 = ler.AddressLine2.ToUpper();
               
                if (addcitypinchk == true)
                {
                    ler.City = citytxt.ToUpper();
                    ler.Pincode = pincodetxt;
                }
                else
                {
                    ler.City = ler.City.ToUpper();
                }

                ler.OwnerName = ler.OwnerName.ToUpper();
                ler.Country = "INDIA";
                #endregion

                if (ler.Capacity == "Less Than 4TPH" && ler.TypeOfReq == "Sorters")
                {
                    //LeadEnquiry.TypeOfReq = "Sorters";
                    ler.AssignTo = "Channel Partner";
                }
                else if (ler.Capacity == "4 to 8 TPH" && ler.TypeOfReq == "Sorters")
                {
                    //LeadEnquiry.TypeOfReq = "Sorters";
                    ler.AssignTo = "Channel Partner";
                }
                else if (ler.Capacity == "Greater than 8 TPH")
                {
                    ler.TypeOfReq = "Sorters";
                    ler.AssignTo = "Buhler";
                }
                else if (ler.Capacity == "Less Than 4TPH" && ler.TypeOfReq == "Machines" && ler.MachineSorter == true)
                {
                    if (ler.MachineClassifier == true || ler.MachineDestoner == true || ler.MachineHullerSeperator == true || ler.MachinePaddySeperator == true || ler.MachineThickThinGrader == true || ler.MachineWhitner == true || ler.MachinePolisher == true)
                    {
                        //LeadEnquiry.TypeOfReq = "Sorters";
                        ler.AssignTo = "Buhler";
                    }
                    else
                    {
                        ler.AssignTo = "Channel Partner";
                    }
                }
                //else if (ler.Capacity == "Less Than 4TPH" && ler.TypeOfReq == "Machines" && ler.MachineSorter == true)
                //{
                //    //LeadEnquiry.TypeOfReq = "Sorters";
                //    ler.AssignTo = "Channel Partner";
                //}
                else if (ler.Capacity == "4 to 8 TPH" && ler.TypeOfReq == "Machines" && ler.MachineSorter == true)
                {
                    if (ler.MachineClassifier == true || ler.MachineDestoner == true || ler.MachineHullerSeperator == true || ler.MachinePaddySeperator == true || ler.MachineThickThinGrader == true || ler.MachineWhitner == true || ler.MachinePolisher == true)
                    {
                        //LeadEnquiry.TypeOfReq = "Sorters";
                        ler.AssignTo = "Buhler";
                    }
                    else
                    {
                        ler.AssignTo = "Channel Partner";
                    }
                }
                //else if (ler.Capacity == "4 to 8 TPH" && ler.TypeOfReq == "Machines" && ler.MachineSorter == true)
                //{
                //    //LeadEnquiry.TypeOfReq = "Sorters";
                //    ler.AssignTo = "Channel Partner";
                //}              
                if(ler.CommodityType == "Pulses")
                {
                    ler.TypeOfReq = "Sorters";
                }

                var time = System.DateTime.Now.TimeOfDay;
                string test1 = Convert.ToString(time);
                TimeSpan tp = TimeSpan.Parse(test1);
                //ler.LeadDate=LeadDate;      
                try
                {
                    ler.LeadDate = System.DateTime.Now;
                    ler.LeadTime = (tp.ToString(@"hh\:mm"));
                }
                catch { }
                ler.IsTime = 0;
                ler.IsDeleted = 0;
                ler.IsStatus = 0;
                ler.IsDrop = 0;
                ler.IsCount = 0;
                ler.Country = country;
                ler.IsHOD = 0;
                ler.Latitude = l1;
                ler.Longitude = l2;
                try
                {
                    ler.NotifyDate = System.DateTime.Now;
                    DateTime dat = System.DateTime.Now;
                    var dat2 = dat.ToString("yyyy-MM-dd");
                    ler.CreatedOn = Convert.ToDateTime(dat2);
                }
                catch { }
                
                ler.CreatedBy = Convert.ToString(userid);
                if (cpid != 0)
                {
                    ler.CPID = cpid;
                }
                else
                {
                    ler.CPID = 0;
                }
                db.LeadEnquiryRevised.Add(ler);
                db.SaveChanges();
            }
            if (count != 0)
            {
                TempData["fail"] = "Already Inserted";
            }

            return RedirectToAction("LEIndexRevised", "LeadEnquiry", null);
        }

        [HttpGet]
        public ActionResult LEEditRevised(int id = 0)
        {
            LeadEnquiryRevised LeadEnquiry = db.LeadEnquiryRevised.Find(id);
            if (LeadEnquiry == null)
            {
                return HttpNotFound();
            }
            //DateTime dd = System.DateTime.Now;
            //string dd1 = dd.ToString("dd-MM-yyyy");
            //ViewBag.date = dd1;
            var leaddate = db.LeadEnquiryRevised.Where(m => m.LERID ==id).Select(m => m.LeadDate).SingleOrDefault();//code added By sneha
            string LeadDate = leaddate.ToString("dd-MM-yyyy");
            ViewBag.Date = LeadDate;

            ViewBag.State = new SelectList(db.States_tbl.Where(m => m.IsDeleted == 0), "State", "State", LeadEnquiry.State);

            ViewBag.District = new SelectList(db.DistrictPinCodeDetails_tbl.Where(m => m.IsDeleted == 10), "District", "District");

            ViewBag.PINCode = new SelectList(db.DistrictPinCodeDetails_tbl.Where(m => m.IsDeleted == 10), "PINCode", "PINCode");
           

            // ViewBag.ProductModelName = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelName", "ProductModelName");
            return View(LeadEnquiry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LEEditRevised(LeadEnquiryRevised LeadEnquiry, bool addcitypinchk, string citytxt = null, string pincodetxt = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int cpid = Convert.ToInt32(Session["logincpid"]);

            if (addcitypinchk == true)
            {
                var stateid = db.States_tbl.Where(m => m.State == LeadEnquiry.State).Select(m => m.StateID).SingleOrDefault();

                //Adding State, City and PIN Code into DistrictPinCodeDetails_tbl
                DistrictPinCodeDetails DistrictPinCodeDetails_tbl = new DistrictPinCodeDetails();
                DistrictPinCodeDetails_tbl.State = LeadEnquiry.State;
                DistrictPinCodeDetails_tbl.District = citytxt.ToUpper();
                DistrictPinCodeDetails_tbl.PINCode = Convert.ToInt32(pincodetxt);
                DistrictPinCodeDetails_tbl.IsDeleted = 0;
                DistrictPinCodeDetails_tbl.CreatedOn = System.DateTime.Now;
                DistrictPinCodeDetails_tbl.CreatedBy = cpid;
                DistrictPinCodeDetails_tbl.StateID = stateid;
                db.DistrictPinCodeDetails_tbl.Add(DistrictPinCodeDetails_tbl);
                db.SaveChanges();
            }

            //checking condition for Duplicate value
            //int count = db.LeadEnquiryRevised.Where(m => m.MillName == LeadEnquiry.MillName).Where(m => m.City == LeadEnquiry.City).Where(m => m.Pincode == LeadEnquiry.Pincode).Count();

            //if (count == 0 || count == null)
            //{
            if (LeadEnquiry.LERID != 0)
            {
                //converting to Capital Letters
                #region

                LeadEnquiry.NameofCollector = LeadEnquiry.NameofCollector.ToUpper();
                LeadEnquiry.MillName = LeadEnquiry.MillName.ToUpper();
                LeadEnquiry.MillType = LeadEnquiry.MillType.ToUpper();
                LeadEnquiry.AddressLine1 = LeadEnquiry.AddressLine1.ToUpper();
                LeadEnquiry.AddressLine2 = LeadEnquiry.AddressLine2.ToUpper();

                if (addcitypinchk == true)
                {
                    LeadEnquiry.City = citytxt.ToUpper();
                    LeadEnquiry.Pincode = pincodetxt;
                }
                else
                {
                    LeadEnquiry.City = LeadEnquiry.City.ToUpper();
                }

                LeadEnquiry.OwnerName = LeadEnquiry.OwnerName.ToUpper();

                #endregion

                if (LeadEnquiry.Capacity == "Less Than 4TPH" && LeadEnquiry.TypeOfReq == "Sorters")
                {
                    //LeadEnquiry.TypeOfReq = "Sorters";
                    LeadEnquiry.AssignTo = "Channel Partner";
                }
                else if (LeadEnquiry.Capacity == "4 to 8 TPH" && LeadEnquiry.TypeOfReq == "Sorters")
                {
                    //LeadEnquiry.TypeOfReq = "Sorters";
                    LeadEnquiry.AssignTo = "Channel Partner";
                }
                else if (LeadEnquiry.Capacity == "Greater than 8 TPH")
                {
                    LeadEnquiry.TypeOfReq = "Sorters";
                    LeadEnquiry.AssignTo = "Buhler";
                }
                else if (LeadEnquiry.Capacity == "Less Than 4TPH" && LeadEnquiry.TypeOfReq == "Machines" && LeadEnquiry.MachineSorter == true)
                {
                    if (LeadEnquiry.MachineClassifier == true || LeadEnquiry.MachineDestoner == true || LeadEnquiry.MachineHullerSeperator == true || LeadEnquiry.MachinePaddySeperator == true || LeadEnquiry.MachineThickThinGrader == true || LeadEnquiry.MachineWhitner == true || LeadEnquiry.MachinePolisher == true)
                    {
                        //LeadEnquiry.TypeOfReq = "Sorters";
                        LeadEnquiry.AssignTo = "Buhler";
                    }
                    else
                    {
                        LeadEnquiry.AssignTo = "Channel Partner";
                    }
                }
                //else if (ler.Capacity == "Less Than 4TPH" && ler.TypeOfReq == "Machines" && ler.MachineSorter == true)
                //{
                //    //LeadEnquiry.TypeOfReq = "Sorters";
                //    ler.AssignTo = "Channel Partner";
                //}
                else if (LeadEnquiry.Capacity == "4 to 8 TPH" && LeadEnquiry.TypeOfReq == "Machines" && LeadEnquiry.MachineSorter == true)
                {
                    if (LeadEnquiry.MachineClassifier == true || LeadEnquiry.MachineDestoner == true || LeadEnquiry.MachineHullerSeperator == true || LeadEnquiry.MachinePaddySeperator == true || LeadEnquiry.MachineThickThinGrader == true || LeadEnquiry.MachineWhitner == true || LeadEnquiry.MachinePolisher == true)
                    {
                        //LeadEnquiry.TypeOfReq = "Sorters";
                        LeadEnquiry.AssignTo = "Buhler";
                    }
                    else
                    {
                        LeadEnquiry.AssignTo = "Channel Partner";
                    }
                }
                //else if (ler.Capacity == "4 to 8 TPH" && ler.TypeOfReq == "Machines" && ler.MachineSorter == true)
                //{
                //    //LeadEnquiry.TypeOfReq = "Sorters";
                //    ler.AssignTo = "Channel Partner";
                //}

                if (LeadEnquiry.CommodityType == "Pulses")
                {
                    LeadEnquiry.TypeOfReq = "Sorters";
                }
                try
                {
                    LeadEnquiry.LeadDate = System.DateTime.Now;
                    LeadEnquiry.ModifiedOn = System.DateTime.Now;
                }
                catch { }
                LeadEnquiry.ModifiedBy = Convert.ToString(userid);
                db.Entry(LeadEnquiry).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Modified Successfully!!!!";
                return RedirectToAction("LEIndexRevised");
            }
            //}
            //if (count != 0)
            //{
            //    TempData["fail"] = "Already Inserted";
            //}
            //ViewBag.ProductModelName = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelName", "ProductModelName");
            return View("LEIndexRevised");
        }

        // GET: 
        public ActionResult LEDeleteRevised(int id = 0)
        {
            LeadEnquiryRevised LeadEnquiry = db.LeadEnquiryRevised.Find(id);
            if (LeadEnquiry == null)
            {
                return HttpNotFound();
            }
            return View(LeadEnquiry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LEDeleteRevised(LeadEnquiryRevised leadenquiry, int id)
        {
            if (Request.Form["Yes"] != null)
            {
                LeadEnquiryRevised LeadEnquiry = db.LeadEnquiryRevised.Find(id);
                LeadEnquiry.IsDeleted = 1;
                db.Entry(LeadEnquiry).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Record Deleted Successfully!!!!";
                return RedirectToAction("LEIndexRevised");
            }
            return RedirectToAction("LEIndexRevised");
        }
        
        public ActionResult LEIndexRevised(int page = 1, int sortBy = 1, bool isAsc = false, string custnm = null)
        {
            //int cpid = Convert.ToInt32(Session["logincpid"]);
            //var list = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == cpid).ToList();
            //return View(list); 

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var getcpid = loginname.CPID;

            //Paging and Sorting //
            IEnumerable<LeadEnquiryRevised> chanpart = db.LeadEnquiryRevised.Where(p => custnm == null || p.MillName.Contains(custnm)).Where(m=>m.IsDeleted==0).Where(m => m.CPID == getcpid).OrderByDescending(m=>m.LERID);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LERID) : chanpart.OrderByDescending(p => p.LERID);
                    break;

                case 2:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.MillName) : chanpart.OrderByDescending(p => p.MillName);
                    break;

                case 3:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.TelNo) : chanpart.OrderByDescending(p => p.TelNo);
                    break;

                case 4:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.EmailId) : chanpart.OrderByDescending(p => p.EmailId);
                    break;

                case 5:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OwnerName) : chanpart.OrderByDescending(p => p.OwnerName);
                    break;

                case 6:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.Pincode) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 7:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
                    break;

                default:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LERID) : chanpart.OrderByDescending(p => p.LERID);
                    break;
            }
            #endregion
            ViewBag.Search = custnm;
            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (custnm != null)
                ViewBag.IsSearch = true;
            return View(chanpart);

        }

        public ActionResult DropEnquiryRevised(int id)
        {
            LeadEnquiryRevised LeadEnquiry = db.LeadEnquiryRevised.Find(id);
            int isdrop = db.LeadEnquiryRevised.Where(m => m.LERID == id).Select(m => m.IsDrop).SingleOrDefault();
            LeadEnquiry.IsDrop = isdrop + 1;
            LeadEnquiry.IsStatus = 2;
            db.Entry(LeadEnquiry).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Success"] = "Record Dropped,You cannot Convert and Generate Quotation...!!!!";
            return RedirectToAction("LEIndexRevised");
        }

        public ActionResult EnableEnquiryRevised(int id)
        {
            LeadEnquiryRevised LeadEnquiry = db.LeadEnquiryRevised.Find(id);
            LeadEnquiry.IsStatus = 0;
            db.Entry(LeadEnquiry).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Success"] = "Record Is Enabled, You Can Convert Now...!!!!";
            return RedirectToAction("LEIndexRevised");
        }

        public ActionResult ConvertToMDBRevised(string type,int id = 0)
        {
            int cpid = Convert.ToInt32(Session["logincpid"]);
            LeadEnquiryRevised LeadEnquiry = db.LeadEnquiryRevised.Find(id);

            int count = db.MDBGeneralData.Where(m => m.OrganizationName == LeadEnquiry.MillName).Where(m=>m.City==LeadEnquiry.City).Where(m=>m.Pincode==LeadEnquiry.Pincode).Where(m=>m.CPID==cpid).Where(m => m.IsDeleted == 0).Count();

            int mdbid2 = 0; int mdbid = 0;
            if (count == 0)
            {
                var mdid = from CompanyUniqueID in db.MDBGeneralData where CompanyUniqueID!=null select CompanyUniqueID;
                mdid = mdid.OrderByDescending(m => m.MDBID);
                var check = mdid.FirstOrDefault();
                var mdbi = mdid.Select(m => m.CompanyUniqueID).First();
                string[] split = mdbi.Split('-');
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
                MDBGeneralData mdb = new MDBGeneralData();
                try
                {
                    // Data oving from one Lead table to MDB table
                    mdb.CompanyUniqueID = mdbi;

                    if (LeadEnquiry.MillName != null)
                    {
                        string millName = "";
                        //mdb.OrganizationName = LeadEnquiry.MillName;
                        millName = LeadEnquiry.MillName;
                        foreach (var special in new string[] { @"\n", @"\t", @"\r\n" })
                        {
                            string cmp = "";
                            switch (special)
                            {
                                case "\\n":
                                    cmp = "\n";
                                    break;
                                case "\\t":
                                    cmp = "\t";
                                    break;
                                case "\\r":
                                    cmp = "\r";
                                    break;
                                default:cmp = "";
                                    break;

                            }
                            if (millName.Contains(cmp) && cmp!="")
                            {
                                millName = millName.Replace(cmp, "");
                            }
                        }
                        mdb.OrganizationName = millName;
                    }
                    else
                    {
                        mdb.OrganizationName = "XXXXXXXX";
                    }

                    if (LeadEnquiry.MillType != null)
                    {
                        mdb.OrganizationType = LeadEnquiry.MillType;
                    }
                    else
                    {
                        mdb.OrganizationType = "XXXXXX";
                    }

                    if (LeadEnquiry.AddressLine1 != null)
                    {
                        mdb.AddressLine1 = LeadEnquiry.AddressLine1;
                    }
                    else
                    {
                        mdb.AddressLine1 = "ASDFGH";
                    }

                    if (LeadEnquiry.AddressLine2 != null)
                    {
                        mdb.AddressLine2 = LeadEnquiry.AddressLine2;
                    }
                    else
                    {
                        mdb.AddressLine2 = "ASDFGH";
                    }

                    if (LeadEnquiry.City != null)
                    {
                        mdb.City = LeadEnquiry.City;
                    }
                    try { 
                    if (LeadEnquiry.Pincode != null && LeadEnquiry.Pincode.Length>5)
                    {
                        mdb.Pincode = LeadEnquiry.Pincode;
                    }
                    }
                    catch
                    {
                        mdb.Pincode = "111111";
                    }



                    if (LeadEnquiry.State != null)
                    {
                        mdb.State = LeadEnquiry.State;
                    }
                  
                    if (LeadEnquiry.Country != null)
                    {
                        mdb.Country = LeadEnquiry.Country;
                    }
                    else
                    {
                        mdb.Country = "INDIA";
                    }

                    if (LeadEnquiry.Latitude != null)
                    {
                        mdb.Latitude = LeadEnquiry.Latitude;
                    }
                    else
                    {
                        mdb.Latitude = "10101.101";
                    }

                    if (LeadEnquiry.Longitude != null)
                    {
                        mdb.Longitude = LeadEnquiry.Longitude;
                    }
                    else
                    {
                        mdb.Longitude = "10101.101";
                    }

                    if (LeadEnquiry.Isd != null)
                    {
                        mdb.Isd1 = LeadEnquiry.Isd;
                    }
                    else
                    {
                        mdb.Isd1 = " ";
                    }

                    if (LeadEnquiry.Std != null)
                    {
                        mdb.Std1 = LeadEnquiry.Std;
                    }
                    else
                    {
                        mdb.Std1 = " ";
                    }

                    if (LeadEnquiry.TelNo != null)
                    {
                        mdb.PhoneLL1 = LeadEnquiry.TelNo;
                    }
                    else
                    {
                        mdb.PhoneLL1 = " ";
                    }

                    if (LeadEnquiry.EmailId != null)
                    {
                        mdb.EmailID = LeadEnquiry.EmailId;
                    }
                    else
                    {
                        mdb.EmailID = "aaaa@gmail.com";
                    }

                    mdb.CPID = LeadEnquiry.CPID;

                    db.MDBGeneralData.Add(mdb);
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

                }
                catch (Exception ex)
                {
                    TempData["message"] = "Please Enter the Mandatory Details Properly and Convert the " + LeadEnquiry.MillName + " Lead...!!";
                    return RedirectToAction("LEIndexRevised", "LeadEnquiry", null);
                }

                mdbid2 = mdb.MDBID;

                MDBContactPersonData mdbcontact = new MDBContactPersonData();
                mdbcontact.MDBID = mdbid2;

                if (LeadEnquiry.Prefix != null)
                {
                    mdbcontact.Title = LeadEnquiry.Prefix;
                }
                else
                {
                    mdbcontact.Title = "MM";
                }

                if (LeadEnquiry.OwnerName != null)
                {
                    mdbcontact.FirstName = LeadEnquiry.OwnerName;
                }
                else
                {
                    mdbcontact.FirstName = "AAABBBCCC";
                }
                if (LeadEnquiry.MobNo != null)
                {
                    mdbcontact.Mobile1 = LeadEnquiry.MobNo;
                }
               
                db.MDBContactPersonData.Add(mdbcontact);
                db.SaveChanges();

                //Closing the Status in LeadEnquiry Form
                LeadEnquiry.IsStatus = 1;
                db.Entry(LeadEnquiry).State = EntityState.Modified;
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


                mdbid = mdbid2;
            }
            else if (count != 0)
            {
                if (count == 1)
                {
                    mdbid = db.MDBGeneralData.Where(m => m.OrganizationName == LeadEnquiry.MillName).Where(m => m.City == LeadEnquiry.City).Where(m => m.Pincode == LeadEnquiry.Pincode).Where(m => m.CPID == cpid).Where(m => m.IsDeleted == 0).Select(m => m.MDBID).SingleOrDefault();
                }
                else if (count > 1)
                {
                    mdbid = db.MDBGeneralData.Where(m => m.OrganizationName == LeadEnquiry.MillName).Where(m => m.City == LeadEnquiry.City).Where(m => m.Pincode == LeadEnquiry.Pincode).Where(m => m.CPID == cpid).Where(m => m.IsDeleted == 0).Select(m => m.MDBID).First();

                }
                LeadEnquiry.IsStatus = 1;
                db.Entry(LeadEnquiry).State = EntityState.Modified;
                db.SaveChanges();
            }

            //return RedirectToAction("Create", "Quotation", new { mdbid }); // Old Code Till 13-12-2016

            if (type == "RiceMill")
            {
                return RedirectToAction("RiceMillGenerate", "Quotation", new { mdbid }); // New Code On 13-12-2016
            }
            else //Sorter
            {
                return RedirectToAction("Create", "Quotation", new { mdbid }); // Old Code Till 13-12-2016
            }
            
            //TempData["Success"] = "Record Converted to Master Database and Saved Successfully...!!!";
            //return RedirectToAction("LEIndexRevised", "LeadEnquiry", null);
        }

        //// Method/view for checking the Organization name in LeadEnquiry table.
        //public ActionResult LESearch()
        //{
        //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
        //    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
        //    var getcpid = loginname.CPID;

        //    var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CPID == getcpid).ToList();
        //    return View(list);
        //}

        //[HttpPost]
        //public ActionResult LESearch(int page = 1, int sortBy = 1, bool isAsc = true, string custnm = null)
        //{
        //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
        //    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
        //    var getcpid = loginname.CPID;
        //    //Paging and Sorting //
        //    IEnumerable<LeadEnquiry> chanpart = db.LeadEnquiry.Where(
        //            p => custnm == null
        //            || p.OrganizationName.Contains(custnm)).Where(m => m.CPID == getcpid);

        //    #region Sorting
        //    switch (sortBy)
        //    {
        //        case 1:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.LEID) : chanpart.OrderByDescending(p => p.LEID);
        //            break;

        //        case 2:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationName) : chanpart.OrderByDescending(p => p.OrganizationName);
        //            break;

        //        case 3:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.OrganizationType) : chanpart.OrderByDescending(p => p.OrganizationType);
        //            break;

        //        case 4:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.AddressLine1) : chanpart.OrderByDescending(p => p.AddressLine1);
        //            break;

        //        case 5:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
        //            break;

        //        case 6:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
        //            break;

        //        case 7:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.PhoneLL1) : chanpart.OrderByDescending(p => p.PhoneLL1);
        //            break;

        //        case 8:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.EmailID) : chanpart.OrderByDescending(p => p.EmailID);
        //            break;

        //        default:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.LEID) : chanpart.OrderByDescending(p => p.LEID);
        //            break;
        //    }
        //    #endregion
        //    ViewBag.Search = custnm;
        //    ViewBag.SortBy = sortBy;
        //    ViewBag.IsAsc = isAsc;
        //    if (custnm != null)
        //        ViewBag.IsSearch = true;
        //    return View(chanpart);
        //}

        //[HttpGet]
        //public ActionResult LEIndexRevised(int page = 1, int sortBy = 1, bool isAsc = true, string custnm = null)
        //{
        //    //int cpid = Convert.ToInt32(Session["logincpid"]);
        //    //var list = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == cpid).ToList();
        //    //return View(list); 

        //    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
        //    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
        //    var getcpid = loginname.CPID;

        //    //Paging and Sorting //
        //    IEnumerable<LeadEnquiryRevised> chanpart = db.LeadEnquiryRevised.Where(
        //            p => custnm == null
        //            || p.MillName.Contains(custnm)).Where(m => m.CPID == getcpid);

        //    #region Sorting
        //    switch (sortBy)
        //    {
        //        case 1:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.LERID) : chanpart.OrderByDescending(p => p.LERID);
        //            break;

        //        case 2:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.MillName) : chanpart.OrderByDescending(p => p.MillName);
        //            break;

        //        case 3:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.TelNo) : chanpart.OrderByDescending(p => p.TelNo);
        //            break;

        //        case 4:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.EmailId) : chanpart.OrderByDescending(p => p.EmailId);
        //            break;

        //        case 5:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.OwnerName) : chanpart.OrderByDescending(p => p.OwnerName);
        //            break;

        //        case 6:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.Pincode) : chanpart.OrderByDescending(p => p.State);
        //            break;

        //        case 7:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
        //            break;

        //        default:
        //            chanpart = isAsc ? chanpart.OrderBy(p => p.LERID) : chanpart.OrderByDescending(p => p.LERID);
        //            break;
        //    }
        //    #endregion
        //    ViewBag.Search = custnm;
        //    ViewBag.SortBy = sortBy;
        //    ViewBag.IsAsc = isAsc;
        //    if (custnm != null)
        //        ViewBag.IsSearch = true;
        //    return View(chanpart);

        //}

        // Lead Follw Up
       
        public ActionResult LeadFollowUpIndex(int page = 1, int sortBy = 1, bool isAsc = false, string custnm = null)
        {
            //int cpid = Convert.ToInt32(Session["logincpid"]);
            //var list = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == cpid).ToList();
            //return View(list); 

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            var getcpid = loginname.CPID;

            //Paging and Sorting //
            IEnumerable<LeadEnquiryRevised> chanpart = db.LeadEnquiryRevised.Where(p => custnm == null || p.MillName.Contains(custnm)).Where(m => m.CPID == getcpid).OrderByDescending(m=>m.LERID);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LERID) : chanpart.OrderByDescending(p => p.LERID);
                    break;

                case 2:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.MillName) : chanpart.OrderByDescending(p => p.MillName);
                    break;

                case 3:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.TelNo) : chanpart.OrderByDescending(p => p.TelNo);
                    break;

                case 4:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.EmailId) : chanpart.OrderByDescending(p => p.EmailId);
                    break;

                case 5:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.OwnerName) : chanpart.OrderByDescending(p => p.OwnerName);
                    break;

                case 6:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.Pincode) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 7:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
                    break;

                default:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LERID) : chanpart.OrderByDescending(p => p.LERID);
                    break;
            }
            #endregion
            ViewBag.Search = custnm;
            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (custnm != null)
                ViewBag.IsSearch = true;
            return View(chanpart);

        }

        [HttpGet]
        public ActionResult LeadFollowUpCreate(int id=0)
        {
            var LeadDetails = db.LeadEnquiryRevised.Where(m => m.LERID == id).Where(m => m.IsDeleted == 0).SingleOrDefault();

            if (LeadDetails != null)
            {
                ViewBag.Oname = LeadDetails.MillName;
                ViewBag.Otype = LeadDetails.MillType;
                ViewBag.city = LeadDetails.City;
                ViewBag.pin = LeadDetails.Pincode;
                ViewBag.mail = LeadDetails.EmailId;
                ViewBag.mobno = LeadDetails.MobNo;
            }

            if (LeadDetails == null)
            {
                TempData["fail"] = "The Lead has been Deleted / Lost";
                return RedirectToAction("LeadFollowUpIndex", "LeadEnquiry", null);
            }

            ViewBag.LEID = id;
            return View();
        }

        [HttpPost]
        public ActionResult LeadFollowUpCreate(LeadFollowUptbl LeadFollowUptbl, int leid = 0 )
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int cpid = Convert.ToInt32(Session["logincpid"]);

            LeadFollowUptbl.CPID = cpid;
            LeadFollowUptbl.LERID = leid;
            LeadFollowUptbl.IsStatus = 0;
            LeadFollowUptbl.IsDeleted = 0;
            LeadFollowUptbl.FollowUpDate = System.DateTime.Now;
            LeadFollowUptbl.CreatedOn = System.DateTime.Now;
            LeadFollowUptbl.CreatedBy = cpid;
            //LeadFollowUptbl.TimeLine=
            db.LeadFollowUptbl.Add(LeadFollowUptbl);
            db.SaveChanges();
            return RedirectToAction("LeadFollowUpIndex", "LeadEnquiry", null);
        }

        public ActionResult LeadFollowUpInformationList(int id = 0)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            int cpid = Convert.ToInt32(Session["logincpid"]);

            var LeadName = db.LeadEnquiryRevised.Where(m => m.LERID == id).Select(m => m.MillName).SingleOrDefault();
            ViewBag.LeadName = LeadName;

            var FollowUpList = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.IsStatus == 0).Where(m => m.LERID == id).ToList();
            return View(FollowUpList);
        }
    }
}

