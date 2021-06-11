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
using System.Net;
using System.Data;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class QuotationController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        ////Json AutoComplete customer unique id
        public JsonResult Autocomplete1(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.CompanyUniqueID.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          && (r.IsDeleted == 0)
                          select new { r.CompanyUniqueID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete customer name
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            string strr = null;
            var resultNew = strr;

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName, r.City, r.Pincode });

            //foreach (var r in result)
            //{
            //    resultNew += r.OrganizationName + "-" + r.City + "-" + r.Pincode;
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete quotation number
        public JsonResult Autocomplete2(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.QGEquipGeneralData
                          where r.QuotationNumber.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID) && (r.Islatest != 1)
                          select new { r.QuotationNumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ////Json AutoComplete customer name
        public JsonResult Autocomplete3(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.MDBGeneralData
                          where r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                          && (r.IsDeleted == 0)
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Product Model Details Update Dynamically in the View
        [HttpGet]
        public JsonResult GetProductModelDetails(int id)
        {
            var selectedRow = (from t in db.ProductModel where t.ProductModelID == id select new { t.UnitPrice, t.ProductModelDesc, t.ProductModelExclusion }).SingleOrDefault();

            var jsonData = new
            {
                unitprice = selectedRow.UnitPrice,
                Desc = selectedRow.ProductModelDesc,
                Exclusion = selectedRow.ProductModelExclusion
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //Update Quotation Number in the New Quotation View
        [HttpGet]
        public JsonResult Getquotationnumber(int id = 0)
        {
            if (id != 0)
            {
                ViewBag.QuotationNumber = quotationnumber();

                var jsonData = new
                {
                    quotnumb = quotationnumber()
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

        [HttpGet]
        public JsonResult Getrevisequotationnumber(String ID)
        {
            ViewBag.ReviseQuotationNumber = Revisequotationnumber(ID);

            var jsonData = new
            {
                quotnumb = Revisequotationnumber(ID)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //Quotation numb check valid
        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetExistingquotationnumber(String id = null)
        {
            var mdid = db.QGEquipGeneralData.Where(m => m.QuotationNumber == id).SingleOrDefault();
            if (mdid != null)
            {
                var jsonData = new
                {
                    quotnumb = "true"
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var jsonData = new
                {
                    quotnumb = "false"
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        //Update Customer Number in the New Customer View
        [HttpGet]
        public JsonResult Getcustomernumber()
        {

            ViewBag.Chanpart = NewCustomer();

            var jsonData = new
            {
                cusnumb = NewCustomer()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }

        //Product Details Update on click of MasterProduct in the View
        public JsonResult GetMasterProductDetails(int id)
        {
            //IEnumerable<SelectListItem> Productlist = new List<SelectListItem>();
            ////db = new SRKS_Synergy();
            //if (!string.IsNullOrEmpty(prdts))
            //{
            //    Productlist = (from m in db.Products where m.MasterProducts.MasterProductName == prdts select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.ProductName, Value = m.ProductID.ToString() });
            //};
            //var result = Json(new SelectList(Productlist, "Value", "Text"));
            //return result;
            List<SelectListItem> itemManufacturer = new List<SelectListItem>();

            var selectList = db.Products.Where(m => m.IsDeleted == 0).Where(m => m.MasterProducts.MasterProductID == id).Select(m => new { m.ProductName, m.ProductID }).ToList();
            foreach (var item in selectList)
            {
                itemManufacturer.Add(new SelectListItem { Text = item.ProductName, Value = Convert.ToString(item.ProductID) });
            }
            return Json(itemManufacturer, JsonRequestBehavior.AllowGet);
        }

        //Product Model Details Update on click of Product in the View
        public JsonResult GetProductDetails(int id)
        {
            //IEnumerable<SelectListItem> ProductModellist = new List<SelectListItem>();
            ////db = new SRKS_Synergy();
            //if (!string.IsNullOrEmpty(prdtmd))
            //{
            //    ProductModellist = (from m in db.ProductModel where m.Products.ProductName == prdtmd select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.ProductModelName, Value = m.ProductModelID.ToString() });
            //};
            //var result = Json(new SelectList(ProductModellist, "Value", "Text"));
            //return result;
            List<SelectListItem> itemManufacturer = new List<SelectListItem>();

            var selectList = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductID == id).Select(m => new { m.ProductModelName, m.ProductModelID }).ToList();
            foreach (var item in selectList)
            {
                itemManufacturer.Add(new SelectListItem { Text = item.ProductModelName, Value = Convert.ToString(item.ProductModelID) });
            }
            return Json(itemManufacturer, JsonRequestBehavior.AllowGet);
        }

        #region Polisher Dropdown Data.

        public JsonResult GetListGTPolisher(string Section)
        {

            List<SelectListItem> listItemsGTPolisher = new List<SelectListItem>();
            listItemsGTPolisher.Add(new SelectListItem
            {
                Text = "Long/Medium",
                Value = "Long/Medium"
            });
            var data = listItemsGTPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListProcessPolisher(string Section)
        {
            List<SelectListItem> listItemsProcessPolisher = new List<SelectListItem>();
            listItemsProcessPolisher.Add(new SelectListItem
            {
                Text = "Raw,Steam/Parboiled",
                Value = "Raw,Steam/Parboiled",
            });
            var data = listItemsProcessPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListPassPolisher(string Section)
        {
            List<SelectListItem> listItemsPassPolisher = new List<SelectListItem>();
            listItemsPassPolisher.Add(new SelectListItem
            {
                Text = "1/2",
                Value = "1/2"
            });
            var data = listItemsPassPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCapacityPolisher(string Section)
        {
            List<SelectListItem> listItemscapacityPolisher = new List<SelectListItem>();
            listItemscapacityPolisher.Add(new SelectListItem
            {
                Text = "3",
                Value = "3"
            });
            listItemscapacityPolisher.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            var data = listItemscapacityPolisher;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Whitner Dropdown Data.

        public JsonResult GetListGTWhitner(string Section)
        {

            List<SelectListItem> listItemsGTWhitner = new List<SelectListItem>();
            listItemsGTWhitner.Add(new SelectListItem
            {
                Text = "Medium",
                Value = "Medium",
            });
            listItemsGTWhitner.Add(new SelectListItem
            {
                Text = "Long",
                Value = "Long",
            });
            var data = listItemsGTWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListProcessWhitner(string Section)
        {
            List<SelectListItem> listItemsProcessWhitner = new List<SelectListItem>();
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Raw",
                Value = "Raw"
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Steam",
                Value = "Steam",
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Parboiled",
                Value = "Parboiled",
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Raw/Steam",
                Value = "Raw/Steam",
            });
            listItemsProcessWhitner.Add(new SelectListItem
            {
                Text = "Raw,Steam/Parboiled",
                Value = "Raw,Steam/Parboiled",
            });
            var data = listItemsProcessWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListPassWhitner(string Section)
        {
            List<SelectListItem> listItemsPassWhitner = new List<SelectListItem>();
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "1",
                Value = "1"
            });
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "2",
                Value = "2",
            });
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "3",
                Value = "3",
            });
            listItemsPassWhitner.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });

            var data = listItemsPassWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCapacityWhitner(string Section)
        {
            List<SelectListItem> listItemscapacityWhitner = new List<SelectListItem>();
            listItemscapacityWhitner.Add(new SelectListItem
            {
                Text = "3",
                Value = "3"
            });
            listItemscapacityWhitner.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            listItemscapacityWhitner.Add(new SelectListItem
            {
                Text = "3/4",
                Value = "3/4",
            });
            var data = listItemscapacityWhitner;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Others Dropdown Data.

        public JsonResult GetListGTOthers(string Section)
        {

            List<SelectListItem> listItemsGTOthers = new List<SelectListItem>();
            listItemsGTOthers.Add(new SelectListItem
            {
                Text = "Medium",
                Value = "Medium",
            });
            listItemsGTOthers.Add(new SelectListItem
            {
                Text = "Long",
                Value = "Long",
            });
            listItemsGTOthers.Add(new SelectListItem
            {
                Text = "Long/Medium",
                Value = "Long/Medium"
            });
            var data = listItemsGTOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListProcessOthers(string Section)
        {
            List<SelectListItem> listItemsProcessOthers = new List<SelectListItem>();
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Raw",
                Value = "Raw"
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Steam",
                Value = "Steam",
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Parboiled",
                Value = "Parboiled",
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Raw/Steam",
                Value = "Raw/Steam",
            });
            listItemsProcessOthers.Add(new SelectListItem
            {
                Text = "Raw,Steam/Parboiled",
                Value = "Raw,Steam/Parboiled",
            });
            var data = listItemsProcessOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListPassOthers(string Section)
        {
            List<SelectListItem> listItemsPassOthers = new List<SelectListItem>();
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "1",
                Value = "1"
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "2",
                Value = "2",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "3",
                Value = "3",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "1/2",
                Value = "1/2",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "2/3",
                Value = "2/3",
            });
            listItemsPassOthers.Add(new SelectListItem
            {
                Text = "3/4",
                Value = "3/4",
            });

            var data = listItemsPassOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListCapacityOthers(string Section)
        {
            List<SelectListItem> listItemscapacityOthers = new List<SelectListItem>();
            listItemscapacityOthers.Add(new SelectListItem
            {
                Text = "3",
                Value = "3"
            });
            listItemscapacityOthers.Add(new SelectListItem
            {
                Text = "4",
                Value = "4",
            });
            listItemscapacityOthers.Add(new SelectListItem
            {
                Text = "3/4",
                Value = "3/4",
            });
            var data = listItemscapacityOthers;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion



        // GET: /Quotation/
        public ActionResult Create(int? mdbid, String custuniqid = null, int custname = 0, String QuotNum = null, string salesEngg = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (salesEngg == null)
            {
                ViewBag.salesEngg = "";
            }
            else {
                ViewBag.salesEngg = salesEngg;
            }
           
            //If MDBID has been selected or created
            if (mdbid.HasValue)
            {
                var mdid = db.MDBGeneralData.Find(mdbid);
                int cpmdbcount = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).Count();
                if (cpmdbcount == 0)
                {
                    //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                    MDBContactPersonData mdbcontact = new MDBContactPersonData();
                    mdbcontact.MDBID = mdbid;
                    mdbcontact.Title = "Mr/Miss";
                    mdbcontact.FirstName = "ASDFG";
                    db.MDBContactPersonData.Add(mdbcontact);
                    db.SaveChanges();

                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;

                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                }
                else
                {
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                    if (cpmdb != null)
                    {
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    }

                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                }
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                ViewBag.QuotationNumber = quotationnumber();
                ViewBag.MDBID = mdid.MDBID;
                ViewBag.OrganizationName = mdid.OrganizationName;
                ViewBag.AddressLine1 = mdid.AddressLine1;
                ViewBag.AddressLine2 = mdid.AddressLine2;
                ViewBag.AddressLine3 = mdid.AddressLine3;
                ViewBag.City = mdid.City;
                ViewBag.Pincode = mdid.Pincode;
                ViewBag.State = mdid.State;
                ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                ViewBag.NullError = false;
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");

                return View(mdb);
            }
            //If nothing has been selected
            else if (custuniqid == null && custname == 0 && QuotNum == null)
            {
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                ViewBag.OrganizationName = "OrganizationName";
                ViewBag.AddressLine1 = "Address(Line1)";
                ViewBag.AddressLine2 = "Address(Line2)";
                ViewBag.AddressLine3 = "Address(Line3)";
                ViewBag.City = "City";
                ViewBag.Pincode = "Pincode";
                ViewBag.State = "State";
                ViewBag.CompanyUniqueID = "CompanyUniqueID";
                ViewBag.NullError = false;
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View(mdb);
            }
            //If Customer Unique ID  has been selected
            else if (custuniqid != null && custname == 0)
            {
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                //var mdid = db.MDBGeneralData.Find(custuniqid);
                var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custuniqid).First();
                if (mdid != null)
                {
                    int cpmdbcount = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).Count();
                    if (cpmdbcount == 0)
                    {
                        //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        MDBContactPersonData mdbcontact = new MDBContactPersonData();
                        mdbcontact.MDBID = mdid.MDBID;
                        mdbcontact.Title = "Mr/Miss";
                        mdbcontact.FirstName = "ASDFG";
                        db.MDBContactPersonData.Add(mdbcontact);
                        db.SaveChanges();

                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    else
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        if (cpmdb != null)
                        {
                            ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        }

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                    //ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    //if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    //{
                    //    ViewBag.dear = "Madam";
                    //}
                    //else
                    //{
                    //    ViewBag.dear = "Sir";
                    //}
                    ViewBag.MDBID = mdid.MDBID;
                    var quotmod = from s in db.QGEquipGeneralData
                                  select s;
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
            }
            //If Customer Name has been selected
            else if (custuniqid == null && custname != 0)
            {
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                //var mdid = db.MDBGeneralData.Find(custname);
                //var mdid = db.MDBGeneralData.Where(m => m.OrganizationName == custname).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                var mdid = db.MDBGeneralData.Where(m => m.MDBID == custname).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                if (mdid != null)
                {
                    //////////
                    int cpmdbcount = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).Count();
                    if (cpmdbcount == 0)
                    {
                        //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        MDBContactPersonData mdbcontact = new MDBContactPersonData();
                        mdbcontact.MDBID = mdid.MDBID;
                        mdbcontact.Title = "Mr/Miss";
                        mdbcontact.FirstName = "ASDFG";
                        db.MDBContactPersonData.Add(mdbcontact);
                        db.SaveChanges();

                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    else
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        if (cpmdb != null)
                        {
                            ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        }

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    ///////

                    //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                    //ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    //if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    //{
                    //    ViewBag.dear = "Madam";
                    //}
                    //else
                    //{
                    //    ViewBag.dear = "Sir";
                    //}

                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
            }
            //If Customer Unique Id and Customer Name are selected
            else if (custuniqid != null && custname != 0)
            {
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custuniqid).First();
                var mdid1 = db.MDBGeneralData.Where(m => m.MDBID == custname).First();
                if (mdid1 != null)
                {
                    int cpmdbcount = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).Count();
                    if (cpmdbcount == 0)
                    {
                        //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        MDBContactPersonData mdbcontact = new MDBContactPersonData();
                        mdbcontact.MDBID = mdid1.MDBID;
                        mdbcontact.Title = "Mr/Miss";
                        mdbcontact.FirstName = "ASDFG";
                        db.MDBContactPersonData.Add(mdbcontact);
                        db.SaveChanges();

                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    else
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        if (cpmdb != null)
                        {
                            ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        }

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                    //ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    //if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    //{
                    //    ViewBag.dear = "Madam";
                    //}
                    //else
                    //{
                    //    ViewBag.dear = "Sir";
                    //}
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
                else if (mdid1 != null)
                {
                    int cpmdbcount = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).Count();
                    if (cpmdbcount == 0)
                    {
                        //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        MDBContactPersonData mdbcontact = new MDBContactPersonData();
                        mdbcontact.MDBID = mdid1.MDBID;
                        mdbcontact.Title = "Mr/Miss";
                        mdbcontact.FirstName = "ASDFG";
                        db.MDBContactPersonData.Add(mdbcontact);
                        db.SaveChanges();

                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    else
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        if (cpmdb != null)
                        {
                            ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        }

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).First();
                    //ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    //if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    //{
                    //    ViewBag.dear = "Madam";
                    //}
                    //else
                    //{
                    //    ViewBag.dear = "Sir";
                    //}
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
            }
            //If Quotation Number has been selected
            else if (QuotNum != null)
            {
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                var mdid = db.QGEquipGeneralData.Where(m => m.QuotationNumber == QuotNum).First();
                if (mdid != null)
                {
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    var mdid1 = db.MDBGeneralData.Find(mdid.MDBID);
                    int cpmdbcount = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).Count();
                    if (cpmdbcount == 0)
                    {
                        //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        MDBContactPersonData mdbcontact = new MDBContactPersonData();
                        mdbcontact.MDBID = mdid1.MDBID;
                        mdbcontact.Title = "Mr/Miss";
                        mdbcontact.FirstName = "ASDFG";
                        db.MDBContactPersonData.Add(mdbcontact);
                        db.SaveChanges();

                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    else
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                        if (cpmdb != null)
                        {
                            ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        }

                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                    }
                    //var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).First();
                    //ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    //if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    //{
                    //    ViewBag.dear = "Madam";
                    //}
                    //else
                    //{
                    //    ViewBag.dear = "Sir";
                    //}
                    ViewBag.OrganizationName = mdid1.OrganizationName;
                    ViewBag.AddressLine1 = mdid1.AddressLine1;
                    ViewBag.AddressLine2 = mdid1.AddressLine2;
                    ViewBag.AddressLine3 = mdid1.AddressLine3;
                    ViewBag.City = mdid1.City;
                    ViewBag.Pincode = mdid1.Pincode;
                    ViewBag.State = mdid1.State;
                    ViewBag.CompanyUniqueID = mdid1.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
                else
                {
                    ViewBag.NullError = true;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
            }
            //If all the above conditions fails
            else
            {
                Quotation mdb = new Quotation();
                //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                ViewBag.NullError = true;
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View(mdb);
            }
        }

        //Generate the quotation number
        public string quotationnumber()
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String quotationnumber;
            var quotmod = from QuotationNumber in db.QGEquipGeneralData
                          select QuotationNumber;
            quotmod = quotmod.OrderByDescending(m => m.QuotationNumber);
            var check = quotmod.FirstOrDefault();
            if (check == null)
            {
                quotationnumber = "QE-" + "RC" + year + "-00001-00";
            }
            else
            {
                var quotval = quotmod.Select(m => m.QuotationNumber).First();
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
                    quotationnumber = quotval;
                }
                else
                {
                    quotationnumber = "QE-" + "RC" + year + "-00001-00";
                }
            }
            return quotationnumber;
        }

        // POST: /MDB/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Create(Quotation maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.QGEquipTableData1.ProductModelID != 0)
                {
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).Select(m => new { m.Title, m.FirstName, m.MiddleName, m.LastName }).First();
                    var time = System.DateTime.Now.TimeOfDay;
                    string test1 = Convert.ToString(time);
                    TimeSpan tp = TimeSpan.Parse(test1);
                    maindatabase.QGEquipGeneralData.LeadTime = (tp.ToString(@"hh\:mm"));
                    maindatabase.QGEquipGeneralData.IsTime = 0;
                    maindatabase.QGEquipGeneralData.KindAttention = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    maindatabase.QGEquipGeneralData.CPID = loginname.CPID;
                    maindatabase.QGEquipGeneralData.MDBID = (int)(mdbid);
                    db.QGEquipGeneralData.Add(maindatabase.QGEquipGeneralData);
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

                    var cpid = from s in db.QGEquipGeneralData
                               select s;
                    cpid = cpid.OrderByDescending(m => m.QGID);
                    int qgid = cpid.Select(m => m.QGID).First();
                    maindatabase.QGEquipPayment.QGID = qgid;
                    db.QGEquipPayment.Add(maindatabase.QGEquipPayment);
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

                    maindatabase.QGEquipTableData1.QGID = qgid;
                   
                    var mpid = maindatabase.QGEquipTableData1.ProductModelID;
                    var mpnm = db.ProductModel.Where(m => m.ProductModelID == mpid).Select(m => m.ProductModelName).SingleOrDefault();
                    //maindatabase.QGEquipTableData1.ProductModelName = mpnm;
                    db.QGEquipTableData.Add(maindatabase.QGEquipTableData1);
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

                    if (maindatabase.QGEquipTableData2.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData2.QGID = qgid;
                        var mpid1 = maindatabase.QGEquipTableData2.ProductModelID;
                        var mpnm1 = db.ProductModel.Where(m => m.ProductModelID == mpid).Select(m => m.ProductModelName).SingleOrDefault();
                        // maindatabase.QGEquipTableData2.ProductModelName = mpnm;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData3.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData3.QGID = qgid;
                        var mpid2 = maindatabase.QGEquipTableData3.ProductModelID;
                        var mpnm2 = db.ProductModel.Where(m => m.ProductModelID == mpid).Select(m => m.ProductModelName).SingleOrDefault();
                        //maindatabase.QGEquipTableData2.ProductModelName = mpnm;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData4.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData4.QGID = qgid;
                        var mpid3 = maindatabase.QGEquipTableData4.ProductModelID;
                        var mpnm3 = db.ProductModel.Where(m => m.ProductModelID == mpid).Select(m => m.ProductModelName).SingleOrDefault();
                        //maindatabase.QGEquipTableData4.ProductModelName = mpnm;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData4);
                        db.SaveChanges();
                    }

                    var qgtblid = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m => m.QGEquipGeneralData.Islatest == 0).Select(a => new { a.ProductModel.ProductModelName, a.Quantity }).ToList();
                    var cnt = qgtblid.Count();
                    foreach (var qg in qgtblid)
                    //for (int i = 0; i < cnt; i++ )
                    {
                        SOT sot = new SOT();
                        sot.QGID = qgid;
                        sot.CPID = loginname.CPID;
                        sot.Equipment = qg.ProductModelName;
                        sot.Quantity = qg.Quantity;
                        sot.CreatedOn = System.DateTime.Now;
                        sot.ModifiedOn = System.DateTime.Now;
                        db.SOT.Add(sot);
                        db.SaveChanges();
                    }

                    //update isstatus in Lead Enquiry table // If IsStatus==3 then quotation has been generated
                    var name = db.MDBGeneralData.Where(m => m.IsDeleted == 0).Where(m => m.MDBID == mdbid).Select(m => m.OrganizationName).SingleOrDefault();
                    //int leid = db.LeadEnquiry.Where(m => m.OrganizationName == name).Select(m => m.LEID).SingleOrDefault();

                    int leridcount = db.LeadEnquiryRevised.Where(m => m.MillName == name).Where(m => m.IsDeleted == 0).Select(m => m.LERID).Count();
                    int lerid = 0;
                    if (leridcount == 1)
                    {
                        lerid = db.LeadEnquiryRevised.Where(m => m.MillName == name).Where(m => m.IsDeleted == 0).Select(m => m.LERID).SingleOrDefault();
                    }
                    else if (leridcount > 1)
                    {
                        lerid = db.LeadEnquiryRevised.Where(m => m.MillName == name).Where(m => m.IsDeleted == 0).Select(m => m.LERID).First();
                    }
                    //updating status Lead Revised  New table
                    //LeadEnquiryRevised leadrevised =db.LeadEnquiryRevised.Find(lerid);
                    //leadrevised.IsStatus = 3;// quotation generated
                    //db.Entry(leadrevised).State = EntityState.Modified;
                    //db.SaveChanges();

                    //updatin to old lead Enuiry Table
                    #region
                    //LeadEnquiry lead = new LeadEnquiry();

                    //if (leid != 0)
                    //{
                    //    LeadEnquiry LeadEnquiry = db.LeadEnquiry.Find(leid);
                    //    int iscount = Convert.ToInt32(db.LeadEnquiry.Where(m => m.LEID == leid).Select(m => m.IsCount).SingleOrDefault());
                    //    LeadEnquiry.IsCount = iscount + 1;
                    //    LeadEnquiry.IsStatus = 3;
                    //    LeadEnquiry.NotifyDate = System.DateTime.Now;
                    //    db.Entry(LeadEnquiry).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                    //else
                    //{
                    //    var name1 = db.MDBGeneralData.Where(m => m.IsDeleted == 0).Where(m => m.MDBID == mdbid).ToList();

                    //    foreach (var s in name1)
                    //    {
                    //        //storing organization details
                    //    lead.OrganizationName = s.OrganizationName;
                    //    lead.OrganizationType = s.OrganizationType;
                    //    lead.AddressLine1 = s.AddressLine1;
                    //    lead.AddressLine2 = s.AddressLine2;
                    //    lead.AddressLine3 = s.AddressLine3;
                    //    lead.City = s.City;
                    //    lead.Pincode = s.Pincode;
                    //    lead.State = s.State;
                    //    lead.Country = s.Country;
                    //    lead.Isd1 = s.Isd1;
                    //    lead.Std1 = s.Std1;
                    //    lead.PhoneLL1 = s.PhoneLL1;
                    //    lead.EmailID = s.EmailID;
                    //    lead.IsCount = 1;
                    //    lead.IsStatus = 3;
                    //    lead.NotifyDate = System.DateTime.Now;
                    //    DateTime dat = System.DateTime.Now;
                    //    var dat2 = dat.ToString("yyyy-MM-dd");
                    //    lead.CreatedOn = Convert.ToDateTime(dat2);
                    //    lead.CreatedBy = Convert.ToString(userid);
                    //    lead.IsDeleted = 0;
                    //    lead.IsCount = 0;
                    //    lead.IsHOD = 0;
                    //    lead.NotifyDate = System.DateTime.Now;
                    //    lead.IsStatus = 1;
                    //    lead.CPID = loginname.CPID;
                    //    lead.IsDrop = 0;
                    //    lead.LeadTime = (tp.ToString(@"hh\:mm"));

                    //    //storing contact details
                    //    int id = db.MDBContactPersonData.Where(m => m.MDBID == s.MDBID).Select(m=>m.MDBCPDID).SingleOrDefault();
                    //    var list11 = db.MDBContactPersonData.Where(m => m.MDBCPDID == id).SingleOrDefault();

                    //    lead.Prefix = list11.Title;
                    //    lead.FirstName = list11.FirstName;
                    //    lead.MiddleName = list11.MiddleName;
                    //    lead.LastName = list11.LastName;
                    //    lead.Isdc1 = list11.Isd1;
                    //    lead.Stdc1 = list11.Std1;
                    //    lead.PhoneLLc1 = list11.PhoneLL1;
                    //    lead.Isdm1 = list11.Isdm1;
                    //    lead.Mobile1 = list11.Mobile1;
                    //    lead.EmailIDContact = list11.EmailID;

                    //    db.LeadEnquiry.Add(lead);
                    //    db.SaveChanges();
                    //    }
                    //}

                    #endregion
                    //ends

                    // Entering into RC Single Machine or Updating if the Customer Name is Present.
                    #region
                    LeadEnquiryRevised leadrevised = new LeadEnquiryRevised();
                    if (lerid != 0)
                    {
                        LeadEnquiryRevised LeadEnquiryRevised = db.LeadEnquiryRevised.Find(lerid);
                        int iscount = Convert.ToInt32(db.LeadEnquiryRevised.Where(m => m.LERID == lerid).Select(m => m.IsCount).SingleOrDefault());
                        LeadEnquiryRevised.IsCount = iscount + 1;
                        LeadEnquiryRevised.IsStatus = 3;
                        LeadEnquiryRevised.NotifyDate = System.DateTime.Now;
                        db.Entry(LeadEnquiryRevised).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //var name1 = db.MDBGeneralData.Where(m => m.IsDeleted == 0).Where(m => m.MDBID == mdbid).ToList();

                        //foreach (var s in name1)
                        //{
                        //    //storing organization details
                        //    //leadrevised.NameofCollector=
                        //    leadrevised.MillName = s.OrganizationName;
                        //    leadrevised.MillType = s.OrganizationType;
                        //    leadrevised.AddressLine1 = s.AddressLine1;
                        //    leadrevised.AddressLine2 = s.AddressLine2;
                        //    leadrevised.City = s.City;
                        //    leadrevised.Pincode = s.Pincode;
                        //    leadrevised.State = s.State;
                        //    leadrevised.Country = s.Country;
                        //    leadrevised.IsCount = 1;
                        //    leadrevised.IsStatus = 3;
                        //    leadrevised.NotifyDate = System.DateTime.Now;
                        //    leadrevised.CreatedOn = s.CreatedOn;
                        //    leadrevised.CreatedBy = Convert.ToString(loginname.CPID);
                        //    leadrevised.IsDeleted = 0;
                        //    leadrevised.IsHOD = 0;
                        //    leadrevised.NotifyDate = System.DateTime.Now;
                        //    leadrevised.CPID = loginname.CPID;
                        //    leadrevised.IsDrop = 0;
                        //    leadrevised.LeadDate = System.DateTime.Now;
                        //    leadrevised.LeadTime = (tp.ToString(@"hh\:mm"));

                        //    //storing contact details
                        //    int id = db.MDBContactPersonData.Where(m => m.MDBID == s.MDBID).Select(m => m.MDBCPDID).SingleOrDefault();
                        //    var list11 = db.MDBContactPersonData.Where(m => m.MDBCPDID == id).SingleOrDefault();

                        //    leadrevised.Prefix = list11.Title;
                        //    leadrevised.OwnerName = list11.FirstName;
                        //    //leadrevised.MiddleName = list11.MiddleName;
                        //    //leadrevised.LastName = list11.LastName;
                        //    leadrevised.Isd = list11.Isd1;
                        //    leadrevised.Std = list11.Std1;
                        //    leadrevised.TelNo = list11.PhoneLL1;
                        //    //leadrevised.Isdm1 = list11.Isdm1;
                        //    leadrevised.MobNo = list11.Mobile1;
                        //    leadrevised.EmailId = list11.EmailID;

                        //    db.LeadEnquiryRevised.Add(leadrevised);
                        //    db.SaveChanges();
                        //}
                    }
                    #endregion


                    /// //stroing in OverAllLeadStatus Table
                    //#region
                    //OverAllLeadStatus ols = new OverAllLeadStatus();
                    //if (leid != 0)
                    //{
                    //    ols.LEID = leid;
                    //}
                    //else
                    //{
                    //    ols.LEID = lead.LEID;
                    //}
                    //ols.ID = qgid; // storing QGEquipGeneralData Primary ID(QGID)
                    //ols.IsIdStatus = 0;  // 0 value indicates Quotation Generated
                    //ols.Date = System.DateTime.Now;
                    //var tim = System.DateTime.Now.TimeOfDay;
                    //string tst1 = Convert.ToString(tim);
                    //TimeSpan tp1 = TimeSpan.Parse(tst1);
                    //ols.Time = Convert.ToDateTime(tp1.ToString("hh:mm:ss tt"));
                    //if (loginname.CPID != 0)
                    //{
                    //    ols.CPID = loginname.CPID;
                    //}
                    //else
                    //{
                    //    ols.CPID = 0;
                    //}
                    //db.OverAllLeadStatus.Add(ols);
                    //db.SaveChanges();
                    //#endregion
                    ///  // ends for OverAllStatus table.

                    //return RedirectToAction("ReportGeneration", new { qgid = qgid });
                    String updateparent = "<script>window.open('/Quotation/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = document.location.pathname;</script>";
                    return Content(updateparent);
                }
                return View(maindatabase);
            }
            else
            {
                ViewBag.OrganizationName = "OrganizationName";
                ViewBag.AddressLine1 = "Address(Line1)";
                ViewBag.AddressLine2 = "Address(Line2)";
                ViewBag.AddressLine3 = "Address(Line3)";
                ViewBag.City = "City";
                ViewBag.Pincode = "Pincode";
                ViewBag.State = "State";
                ViewBag.CompanyUniqueID = "CompanyUniqueID";
                ViewBag.NullError = true;
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View(maindatabase);
            }
        }

        public int SaveChanges()
        {
            try
            {
                return db.SaveChanges();
            }

            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public ActionResult NewCustomer()
        {
            var mdid = from CompanyUniqueID in db.MDBGeneralData
                       select CompanyUniqueID;
            mdid = mdid.OrderByDescending(m => m.MDBID);
            var check = mdid.FirstOrDefault();
            if (check == null)
            {
                ViewBag.Chanpart = "CUS-" + System.DateTime.Now.Year + "-00001";
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

        // Creating Quick Customer Generation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCustomer(QuickGenerateMDB MDB)
        {
            //checking the duplicate value in LeadEnquiry if not then details will be stored in MDB and LeadEnquiry.
            //var duplicate = (from s in db.LeadEnquiry
            //                 where s.OrganizationName == MDB.OrganizationName && s.AddressLine1 == MDB.AddressLine1 && s.AddressLine2 == MDB.AddressLine2 && s.AddressLine3 == MDB.AddressLine3 && s.City == MDB.City && s.Pincode == MDB.Pincode
            //                 select s).ToList();

            if (MDB.OrganizationName != null && MDB.FirstName != null)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                //if (duplicate.Count > 0)
                //{
                //    ViewBag.Duplicate = "Customer Already Exists for this Address and City in Lead Enquiry Please Do Convert to MDB ";
                //}
                //else
                //{
                string l1 = null;
                string l2 = null;
                //var fulladdress = MDB.AddressLine1 + "," + MDB.AddressLine2 + "," + MDB.AddressLine3 + "," + MDB.City + "," + MDB.Pincode + "," + MDB.State + "," + MDB.Country;
                var fulladdress = MDB.City + "," + MDB.Pincode + "," + MDB.State + "," + MDB.Country;

                /////////////////////////////////////////
                //
                //#region
                //int jcpid = Convert.ToInt32(Session["logincpid"]);

                //var state = db.ChannelPartners.Where(m => m.CPID == jcpid).Select(m => m.State).SingleOrDefault();
                //// code to find if pincode matches with state.
                //// Given: cpid gives state
                //int tick = 0;
                //try
                //{
                //    //string url = "http://maps.google.com/maps/api/geocode/xml?address=" + Convert.ToInt32(ler.Pincode) + "&sensor=false";
                //    // http://maps.googleapis.com/maps/api/geocode/json?address=560060&sensor=true
                //    string url = "http://maps.google.com/maps/api/geocode/json?address=" + Convert.ToInt32(MDB.Pincode) + "&sensor=false";

                //    using (var webClient = new System.Net.WebClient())
                //    {
                //        var json = webClient.DownloadString(url);
                //        // Now parse with JSON.Net
                //        // DataTable dtValue = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                //        //Object jObject = JsonConvert.DeserializeObject<JObject>(json);
                //        // DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jObject.ToString());

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
                //    return RedirectToAction("NewCustomer");
                //}
                //#endregion

                ////////////////////////////////////////

                //// Finds the Latitude and Longitude of the address
                //string url = "http://maps.google.com/maps/api/geocode/xml?address=" + fulladdress + "&sensor=false";
                //WebRequest request = WebRequest.Create(url);
                //using (WebResponse response = (HttpWebResponse)request.GetResponse())
                //{
                //    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                //    {
                //        DataSet dsResult = new DataSet();
                //        dsResult.ReadXml(reader);
                //        DataTable dtCoordinates = new DataTable();
                //        dtCoordinates.Columns.AddRange(new DataColumn[4]
                //         {
                //            new DataColumn("Id", typeof(int)),
                //            new DataColumn("Address", typeof(string)),
                //            new DataColumn("Latitude",typeof(string)),
                //            new DataColumn("Longitude",typeof(string))
                //        });
                //        foreach (DataRow row in dsResult.Tables["result"].Rows)
                //        {
                //            string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                //            DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                //            dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);

                //            l1 =Convert.ToString(location["lat"]);
                //            l2 =Convert.ToString(location["lng"]);
                //        }
                //    }
                //}
                MDBGeneralData mdbgd = new MDBGeneralData();
                mdbgd.CPID = loginname.CPID;
                mdbgd.CompanyUniqueID = MDB.CompanyUniqueID;
                mdbgd.AddressLine1 = MDB.AddressLine1;
                mdbgd.AddressLine2 = MDB.AddressLine2;
                mdbgd.AddressLine3 = MDB.AddressLine3;
                mdbgd.OrganizationName = MDB.OrganizationName;
                mdbgd.OrganizationType = MDB.OrganizationType;
                mdbgd.City = MDB.City;
                mdbgd.State = MDB.State;
                mdbgd.Pincode = MDB.Pincode;
                mdbgd.Country = MDB.Country;
                mdbgd.Isd1 = MDB.Isd1;
                mdbgd.Std1 = MDB.Std1;
                mdbgd.PhoneLL1 = MDB.PhoneLL1;
                mdbgd.EmailID = MDB.EmailID;
                mdbgd.Latitude = l1;
                mdbgd.Longitude = l2;
                db.MDBGeneralData.Add(mdbgd);
                db.SaveChanges();
                var cpid = from s in db.MDBGeneralData
                           select s;
                cpid = cpid.OrderByDescending(m => m.MDBID);
                int cpi = cpid.Select(m => m.MDBID).First();
                MDBContactPersonData mdbcd = new MDBContactPersonData();
                mdbcd.MDBID = cpi;
                mdbcd.Title = MDB.Title;
                mdbcd.FirstName = MDB.FirstName;
                mdbcd.MiddleName = MDB.MiddleName;
                mdbcd.LastName = MDB.LastName;
                mdbcd.Isd1 = MDB.Isdc1;
                mdbcd.Std1 = MDB.Stdc1;
                mdbcd.PhoneLL1 = MDB.PhoneLLc1;
                mdbcd.Isdm1 = MDB.Isdm1;
                mdbcd.Mobile1 = MDB.Mobile1;
                mdbcd.EmailID = MDB.EmailIDContact;
                db.MDBContactPersonData.Add(mdbcd);
                db.SaveChanges();

                //Storing data to Lead Enquiry table
                var time = System.DateTime.Now.TimeOfDay;
                string test1 = Convert.ToString(time);
                TimeSpan tp = TimeSpan.Parse(test1);

                //LeadEnquiry lead = new LeadEnquiry();

                //lead.OrganizationName = MDB.OrganizationName;
                //lead.OrganizationType = MDB.OrganizationType;
                //lead.AddressLine1 = MDB.AddressLine1;
                //lead.AddressLine2 = MDB.AddressLine2;
                //lead.AddressLine3 = MDB.AddressLine3;
                //lead.City = MDB.City;
                //lead.Pincode = MDB.Pincode;
                //lead.State = MDB.State;
                //lead.Country = MDB.Country;
                //lead.Isd1 = MDB.Isd1;
                //lead.Std1 = MDB.Std1;
                //lead.PhoneLL1 = MDB.PhoneLL1;
                //lead.EmailID = MDB.EmailID;
                //lead.Prefix = MDB.Title;
                //lead.FirstName = MDB.FirstName;
                //lead.MiddleName = MDB.MiddleName;
                //lead.LastName = MDB.LastName;
                //lead.Isdc1 = MDB.Isdc1;
                //lead.Stdc1 = MDB.Stdc1;
                //lead.PhoneLLc1 = MDB.PhoneLLc1;
                //lead.Isdm1 = MDB.Isdm1;
                //lead.Mobile1 = MDB.Mobile1;
                //lead.EmailIDContact = MDB.EmailIDContact;
                //DateTime dat = System.DateTime.Now;
                //var dat2 = dat.ToString("yyyy-MM-dd");
                //lead.CreatedOn = Convert.ToDateTime(dat2);
                //lead.CreatedBy = Convert.ToString(userid);
                //lead.IsDeleted = 0;
                //lead.IsCount = 0;
                //lead.IsHOD = 0;
                //lead.NotifyDate = System.DateTime.Now;
                //lead.IsStatus = 1;
                //lead.CPID = loginname.CPID;
                //lead.IsDrop = 0;
                //lead.LeadTime = (tp.ToString(@"hh\:mm"));
                //db.LeadEnquiry.Add(lead);
                //db.SaveChanges();

                String updateparent = "<script>window.opener.setMDBID(" + cpi + ");window.close();</script>";
                return Content(updateparent);
                //}
            }
            return View(MDB);
        }

        public ActionResult NewQuotation()
        {
            ViewBag.NullError = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewQuotation(string custnam = null, string custunid = null)
        {
            if (string.IsNullOrEmpty(custnam) || string.IsNullOrEmpty(custunid))
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

                string leadname = null;
                string pincode = null;
                string city = null;
                string[] custnamSplit = null;

                if (custnam != null && custnam != "")
                {
                    custnamSplit = custnam.Split('-');

                    leadname = custnamSplit[0];
                    city = custnamSplit[1];
                    pincode = custnamSplit[2];
                }

                //If Customer Unique ID  has been selected
                if (custunid != "" && custnam == "")
                {
                    try
                    {
                        var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custunid).SingleOrDefault();
                        if (mdid != null)
                        {
                            //ViewBag.NullError = false;
                            //return View();
                            String updateparent = "<script>window.opener.setCustUniID('" + custunid + "','');window.close();</script>";
                            return Content(updateparent);
                        }
                        else
                        {
                            ViewBag.NullError = true;
                            return View();
                        }
                    }
                    catch
                    {
                        TempData["DBE"] = "Customer Double Entry Exists!!!!";
                        return View();
                    }

                }
                //If Customer Name has been selected
                else if (custunid == "" && custnam != "")
                {
                    try
                    {
                        var mdid = db.MDBGeneralData.Where(m => m.OrganizationName == leadname).Where(m => m.City == city).Where(m => m.Pincode == pincode).Where(m => m.CPID == loginname.CPID).Select(m=>m.MDBID).SingleOrDefault();
                        if (mdid != null)
                        {
                            ViewBag.NullError = false;
                            String updateparent = "<script>window.opener.setCustUniID('','" + mdid + "');window.close();</script>";
                            return Content(updateparent);
                        }
                        else
                        {
                            ViewBag.NullError = true;
                            return View();
                        }
                    }
                    catch
                    {
                        TempData["DBE"] = "Customer Double Entry Exists!!!!";
                        return View();
                    }
                }
                //If Customer Unique Id and Customer Name are selected
                else if (custunid != "" && custnam != "")
                {
                    try
                    {
                        var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custunid).SingleOrDefault();
                        var mdid1 = db.MDBGeneralData.Where(m => m.OrganizationName == leadname).Where(m => m.City == city).Where(m => m.Pincode == pincode).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                        if (mdid != null)
                        {
                            //ViewBag.NullError = false;
                            //return View();
                            String updateparent = "<script>window.opener.setCustUniID('" + custunid + "','');window.close();</script>";
                            return Content(updateparent);
                        }
                        else if (mdid1 != null)
                        {
                            //ViewBag.NullError = false;
                            //return View();
                            String updateparent = "<script>window.opener.setCustUniID('','" + mdid1.OrganizationName + "');window.close();</script>";
                            return Content(updateparent);
                        }
                        else
                        {
                            ViewBag.NullError = true;
                            return View();
                        }
                    }
                    catch
                    {
                        TempData["DBE"] = "Customer Double Entry Exists!!!!";
                        return View();
                    }
                }
                ViewBag.NullError = true;
                return View();
            }
            else
            {
                TempData["DBE"] = "Please Enter Customer Name or Unique ID";
                return View();
            }
        }

        const int pageSize = 10;
        bool getdetailsclick = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult MainExistingQuotation(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<QGEquipGeneralData> quotations = db.QGEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.IsRiceMill == 0).Where(m => m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.QGID) : quotations.OrderByDescending(p => p.QGID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.QuotationNumber) : quotations.OrderByDescending(p => p.QuotationNumber);
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
                    quotations = isAsc ? quotations.OrderBy(p => p.QuotationDate) : quotations.OrderByDescending(p => p.QuotationDate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.QGID) : quotations.OrderByDescending(p => p.QGID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize);

            quotations = quotations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.Search = cunam;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;

            if (getdetailsclick)
                ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclick = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations);
        }

        // GET: /Quotation/ReviseQuotation
        public ActionResult ReviseQuotation(int qgid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (qgid != 0)
            {
                Quotation quo = new Quotation();
                var qgdb = db.QGEquipGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.QGEquipTableData.Where(m => m.QGID == qgid);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.QGEquipGeneralData = qgdb;
                    int qgpid = db.QGEquipPayment.Where(m => m.QGID == qgid).Select(m => m.QGP).FirstOrDefault();
                    quo.QGEquipPayment = db.QGEquipPayment.Find(qgpid);
                    if (modelcnt == 4)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.QGEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.QGEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.QGEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = null;
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = Revisequotationnumber(quotnum);
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                Quotation quo = new Quotation();
                var qgdb1 = db.QGEquipGeneralData.Where(m => m.QuotationNumber == quotnumber).Select(m => m.QGID).SingleOrDefault();
                var qgdb = db.QGEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.QGEquipTableData.Where(m => m.QGID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.QGEquipGeneralData = qgdb;
                    int qgpid1 = db.QGEquipPayment.Where(m => m.QGID == qgdb1).Select(m => m.QGP).FirstOrDefault();
                    quo.QGEquipPayment = db.QGEquipPayment.Find(qgpid1);
                    if (modelcnt == 4)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.QGEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.QGEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.QGEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = null;
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                    }
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = Revisequotationnumber(quotnum);
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //Generate the Revision Quotation Number
        public string Revisequotationnumber(String quotnum)
        {
            String year = System.DateTime.Now.Year.ToString().Substring(2, 2);
            String quotationnumber;
            string[] split = quotnum.Split('-');
            int k = Convert.ToInt32(split[3]);
            int ad = k + 1;
            string cpik = null;
            string len = ad.ToString();
            String yr = split[1].Substring(2, 2);
            if (len.Length == 1)
            {
                cpik = "0" + ad;
            }
            else
            {
                cpik = ad.ToString();
            }
            if (year == yr)
            {
                quotnum = split[0] + "-" + "RC" + year + "-" + split[2] + "-" + cpik;
            }
            else
            {
                quotnum = split[0] + "-" + split[1] + "-" + split[2] + "-" + cpik;
            }
            quotationnumber = quotnum;
            return quotationnumber;
        }

        // POST: /Quotation/ReviseQuotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviseQuotation(Quotation maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.QGEquipTableData1.ProductModelID != 0)
                {

                    var revqg = maindatabase.QGEquipGeneralData.QuotationNumber;
                    var revsplit = revqg.Split('-');
                    var revcomb = revsplit[0] + "-" + revsplit[1] + "-" + revsplit[2];
                    var revnum = Convert.ToInt32(revsplit[3]);
                    var prvrevisednum = "";
                    if (revnum != 0)
                    {
                        revnum -= 1;
                    }
                    if (revnum.ToString().Length == 1)
                    {
                        prvrevisednum = "0" + revnum;
                    }
                    else
                    {
                        prvrevisednum = revnum.ToString();
                    }
                    var fullqn = revcomb + "-" + prvrevisednum;
                    var previosqg = db.QGEquipGeneralData.Where(m => m.QuotationNumber == fullqn).First();
                    previosqg.Islatest = 1;
                    db.Entry(previosqg).State = EntityState.Modified;
                    db.SaveChanges();

                    //storing lead time 07-10-2015
                    var time = System.DateTime.Now.TimeOfDay;
                    string test1 = Convert.ToString(time);
                    TimeSpan tp = TimeSpan.Parse(test1);
                    maindatabase.QGEquipGeneralData.LeadTime = (tp.ToString(@"hh\:mm"));


                    maindatabase.QGEquipGeneralData.CPID = loginname.CPID;
                    maindatabase.QGEquipGeneralData.MDBID = (int)(mdbid);
                    db.QGEquipGeneralData.Add(maindatabase.QGEquipGeneralData);
                    db.SaveChanges();
                    var qg = from s in db.QGEquipGeneralData
                             select s;
                    qg = qg.OrderByDescending(m => m.QGID);
                    int qgid = qg.Select(m => m.QGID).First();
                    maindatabase.QGEquipPayment.QGID = qgid;
                    db.QGEquipPayment.Add(maindatabase.QGEquipPayment);
                    db.SaveChanges();

                    maindatabase.QGEquipTableData1.QGID = qgid;
                    db.QGEquipTableData.Add(maindatabase.QGEquipTableData1);
                    db.SaveChanges();
                    if (maindatabase.QGEquipTableData2.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData2.QGID = qgid;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData3.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData3.QGID = qgid;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData4.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData4.QGID = qgid;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData4);
                        db.SaveChanges();
                    }

                    var sotprv = db.SOT.Where(m => m.QGEquipGeneralData.QuotationNumber == fullqn).ToList();
                    foreach (var sotli in sotprv)
                    {
                        sotli.Islatestquo = 1;
                        db.Entry(sotli).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    var qgtblid = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m => m.QGEquipGeneralData.Islatest == 0).Select(a => new { a.ProductModel.ProductModelName, a.Quantity }).ToList();
                    var cnt = qgtblid.Count();
                    foreach (var qg1 in qgtblid)
                    //for (int i = 0; i < cnt; i++ )
                    {
                        SOT sot = new SOT();
                        sot.QGID = qgid;
                        sot.CPID = loginname.CPID;
                        sot.Equipment = qg1.ProductModelName;
                        sot.Quantity = qg1.Quantity;
                        //sot.CreatedOn = System.DateTime.Now;
                        sot.ModifiedOn = System.DateTime.Now;
                        db.SOT.Add(sot);
                        db.SaveChanges();
                    }
                    //return RedirectToAction("ReportGeneration", new { qgid = qgid });

                    String updateparent = "<script>window.open('/Quotation/ReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = '/Quotation/Create';</script>";
                    return Content(updateparent);
                }
                return RedirectToAction("Create");
            }
            else
            {
                Quotation mdb = new Quotation();
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.MDBID = mdid.MDBID;
                ViewBag.OrganizationName = mdid.OrganizationName;
                ViewBag.AddressLine1 = mdid.AddressLine1;
                ViewBag.AddressLine2 = mdid.AddressLine2;
                ViewBag.AddressLine3 = mdid.AddressLine3;
                ViewBag.City = mdid.City;
                ViewBag.Pincode = mdid.Pincode;
                ViewBag.State = mdid.State;
                ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                ViewBag.NullError = false;
                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        //Navigate to Revise Quotation after Clicking Revise button in Existing Quotation Page
        public ActionResult NavigateReviseQuotation(int qgid = 0)
        {
            if (qgid != 0)
            {
                var qgdb = db.QGEquipGeneralData.Find(qgid);
                var prdname = db.QGEquipTableData.Where(m => m.QGID == qgid).Select(m => m.ProductName).FirstOrDefault();
                if (qgdb != null)
                {
                    String updateparent = "<script>window.opener.setRevisionQuotationID(" + qgid + ");window.close();</script>";
                    return Content(updateparent);
                }
                //else if (qgdb != null && prdname != null)
                //{
                //    String updateparent = "<script>window.opener.setRiceMillReviseID(" + qgid + ");window.close();</script>";
                //    return Content(updateparent);
                //}

            }
            return View("MainExistingQuotation");
        }

        public ActionResult ReportGeneration(int qgid)
        {
            if (qgid != 0)
            {
                Quotation quo = new Quotation();
                ReportModel RM = new ReportModel();
                var qgdb = db.QGEquipGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var cpid = qgdb.CPID;
                    var Logo = db.ChannelPartners.Find(cpid);
                    var Logopath = "";
                    var channelPartnerLogo = "";
                    var add1 = "Check Your Address";
                    //if (cpid != 0)
                    //{
                    //    Logopath = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    //    add1 = Logo.footaddress;
                    //}
                    //else
                    //{
                    Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");
                    if(Logo != null)
                    {
                        channelPartnerLogo = Server.MapPath("~/App_Data/" + Logo.Logo);
                    }
                    
                    
                    add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";
                    //}
                    //var Logopath1 = Server.MapPath("~/App_Data/SRKSLogoJan31.png");
                    var models = db.QGEquipTableData.Where(m => m.QGID == qgid);
                    var modelcount = models.ToList();
                    quo.QGEquipGeneralData = qgdb;
                    var paymentterms = db.QGEquipPayment.Where(m => m.QGID == qgid).First();
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    RM.Logo = Logopath;
                    RM.footaddress = add1;
                    RM.QGID = qgdb.QGID;
                    RM.annexure = paymentterms.annexure;
                    RM.CPQuotationNumber = qgdb.CPQuotationNumber;
                    RM.MDBID = mdbdet.MDBID.ToString();
                    RM.QuotationNumber = qgdb.QuotationNumber;
                    RM.QuotationDate = Convert.ToDateTime(qgdb.QuotationDate).ToString("dd-MM-yyyy");
                    RM.PaymentTerms = paymentterms.PaymentTerms;
                    RM.Delivery = paymentterms.Delivery;
                    RM.DateofDispatch = paymentterms.DateofDispatch;
                    RM.Transport = paymentterms.Transport;
                    RM.Freight = paymentterms.Freight;
                    RM.CST = paymentterms.CST;
                    RM.TransitInsu = paymentterms.TransitInsu;
                    RM.Commodity = paymentterms.Commodity;
                    RM.Validity = paymentterms.Validity;
                    RM.Subjectinfo = qgdb.Subjectinfo;
                    RM.KindAttention = qgdb.KindAttention;
                    RM.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    RM.State = mdbdet.State;
                    RM.Pincode = mdbdet.Pincode;
                    RM.overallprice = paymentterms.overallprice;
                    RM.City = mdbdet.City;
                    RM.AddressLine1 = mdbdet.AddressLine1;
                    RM.AddressLine2 = mdbdet.AddressLine2;
                    RM.AddressLine3 = mdbdet.AddressLine3;
                    RM.OrganizationName = mdbdet.OrganizationName;
                    RM.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
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
                    ViewBag.IsFour = false;
                    RM.QGEquipTableData = modelcount;
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
                    else if (modelcount.Count == 4)
                    {
                        ViewBag.IsFour = true;
                    }

                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                    RM.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                    RM.Designation = loginname.Designation;
                    var FileName = RM.QuotationNumber;
                    ViewBag.CPID = loginname.CPID;
                    var relativePath = "ReportGeneration";
                    string content;
                    var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                    ViewData.Model = RM;

                    String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                    //String footerpath = Server.MapPath("~/App_Data/FooterFile.html");
                    string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
                        //"<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body><div style=\"height: 120px\"><div style=\"width: 700px; float: left; height: 120px\"></div><div style=\"width: 300px; height: 120px; float: right\"><img src= \"" + Logopath + "\" width=\"150\" height=\"35\" /></div></div></body>";
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
                        var pdfconverter = new PDFConverter();
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

        // GET: /Annexure/
        public ActionResult Annexure()
        {
            return View();
        }

        // GET: /Annexure1/
        public ActionResult Annexure1()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Rice Milling Module
        // Starts Here

        //
        // GET: /Rice Mill Quotation/

        public ActionResult RiceMillGenerate(int? mdbid, String custuniqid = null, int custname = 0, String QuotNum = null)
        {


             var userid = (Guid)Session["userid"];

                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();

                ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                ViewBag.Designation = loginname.Designation;

                DateTime quotdat = System.DateTime.Now;
                ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
                //If MDBID has been selected or created
                if (mdbid.HasValue)
                {

                    var mdid = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    ViewBag.QuotationNumber = quotationnumber();
                    ViewBag.MDBID = mdid.MDBID;
                    ViewBag.OrganizationName = mdid.OrganizationName;
                    ViewBag.AddressLine1 = mdid.AddressLine1;
                    ViewBag.AddressLine2 = mdid.AddressLine2;
                    ViewBag.AddressLine3 = mdid.AddressLine3;
                    ViewBag.City = mdid.City;
                    ViewBag.Pincode = mdid.Pincode;
                    ViewBag.State = mdid.State;
                    ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                    ViewBag.NullError = false;

                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");

                    return View(mdb);
                }
                //If nothing has been selected
                else if (custuniqid == null && custname == 0 && QuotNum == null)
                {
                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    ViewBag.OrganizationName = "OrganizationName";
                    ViewBag.AddressLine1 = "Address(Line1)";
                    ViewBag.AddressLine2 = "Address(Line2)";
                    ViewBag.AddressLine3 = "Address(Line3)";
                    ViewBag.City = "City";
                    ViewBag.Pincode = "Pincode";
                    ViewBag.State = "State";
                    ViewBag.CompanyUniqueID = "CompanyUniqueID";
                    ViewBag.NullError = false;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }
                //If Customer Unique ID  has been selected
                else if (custuniqid != null && custname == 0)
                {
                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    //var mdid = db.MDBGeneralData.Find(custuniqid);
                    var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custuniqid).First();
                    if (mdid != null)
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                        ViewBag.MDBID = mdid.MDBID;
                        var quotmod = from s in db.QGEquipGeneralData
                                      select s;
                        ViewBag.QuotationNumber = quotationnumber();
                        ViewBag.OrganizationName = mdid.OrganizationName;
                        ViewBag.AddressLine1 = mdid.AddressLine1;
                        ViewBag.AddressLine2 = mdid.AddressLine2;
                        ViewBag.AddressLine3 = mdid.AddressLine3;
                        ViewBag.City = mdid.City;
                        ViewBag.Pincode = mdid.Pincode;
                        ViewBag.State = mdid.State;
                        ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                        ViewBag.NullError = false;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                    else
                    {

                        ViewBag.NullError = true;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                }
                //If Customer Name has been selected
                else if (custuniqid == null && custname != 0)
                {
                    //string leadname = null;
                    //string pincode = null;
                    //string city = null;
                    //string[] custnamSplit = null;

                    //if (custname != null || custname != "")
                    //{
                    //    custnamSplit = custname.Split('-');

                    //    leadname = custnamSplit[0];
                    //    city = custnamSplit[1];
                    //    pincode = custnamSplit[2];
                    //}

                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    //var mdid = db.MDBGeneralData.Find(custname);
                    //var mdid = db.MDBGeneralData.Where(m => m.OrganizationName == custname).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                    var mdid = db.MDBGeneralData.Where(m => m.MDBID == custname).Where(m => m.CPID == loginname.CPID).SingleOrDefault();
                    if (mdid != null)
                    {
                        try
                        {
                            int mdidint = mdid.MDBID;
                            var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdidint).OrderBy(m => m.MDBCPDID).First();
                            ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                            if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                            {
                                ViewBag.dear = "Madam";
                            }
                            else
                            {
                                ViewBag.dear = "Sir";
                            }
                        }
                        catch (Exception e)
                        {
                            TempData["mdbcnt"] = "Please Update/Edit Contact Details for above Customer and Try....!";
                            return RedirectToAction("RiceMillGenerate", "Quotation", null);
                        }
                        ViewBag.QuotationNumber = quotationnumber();
                        ViewBag.MDBID = mdid.MDBID;
                        ViewBag.OrganizationName = mdid.OrganizationName;
                        ViewBag.AddressLine1 = mdid.AddressLine1;
                        ViewBag.AddressLine2 = mdid.AddressLine2;
                        ViewBag.AddressLine3 = mdid.AddressLine3;
                        ViewBag.City = mdid.City;
                        ViewBag.Pincode = mdid.Pincode;
                        ViewBag.State = mdid.State;
                        ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                        ViewBag.NullError = false;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                    else
                    {
                        ViewBag.NullError = true;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                }
                //If Customer Unique Id and Customer Name are selected
                else if (custuniqid != null && custname != 0)
                {
                    //string leadname = null;
                    //string pincode = null;
                    //string city = null;
                    //string[] custnamSplit = null;

                    //if (custname != null || custname != "")
                    //{
                    //    custnamSplit = custname.Split('-');

                    //    leadname = custnamSplit[0];
                    //    city = custnamSplit[1];
                    //    pincode = custnamSplit[2];
                    //}

                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    var mdid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == custuniqid).First();
                    var mdid1 = db.MDBGeneralData.Where(m => m.MDBID == custname).Where(m => m.CPID == loginname.CPID).First();
                    if (mdid != null)
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid.MDBID).OrderBy(m => m.MDBCPDID).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                        ViewBag.QuotationNumber = quotationnumber();
                        ViewBag.MDBID = mdid.MDBID;
                        ViewBag.OrganizationName = mdid.OrganizationName;
                        ViewBag.AddressLine1 = mdid.AddressLine1;
                        ViewBag.AddressLine2 = mdid.AddressLine2;
                        ViewBag.AddressLine3 = mdid.AddressLine3;
                        ViewBag.City = mdid.City;
                        ViewBag.Pincode = mdid.Pincode;
                        ViewBag.State = mdid.State;
                        ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                        ViewBag.NullError = false;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");

                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                    else if (mdid1 != null)
                    {
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                        ViewBag.QuotationNumber = quotationnumber();
                        ViewBag.MDBID = mdid.MDBID;
                        ViewBag.OrganizationName = mdid.OrganizationName;
                        ViewBag.AddressLine1 = mdid.AddressLine1;
                        ViewBag.AddressLine2 = mdid.AddressLine2;
                        ViewBag.AddressLine3 = mdid.AddressLine3;
                        ViewBag.City = mdid.City;
                        ViewBag.Pincode = mdid.Pincode;
                        ViewBag.State = mdid.State;
                        ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                        ViewBag.NullError = false;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                    else
                    {
                        ViewBag.NullError = true;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                }
                //If Quotation Number has been selected
                else if (QuotNum != null)
                {
                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    var mdid = db.QGEquipGeneralData.Where(m => m.QuotationNumber == QuotNum).First();
                    if (mdid != null)
                    {
                        ViewBag.QuotationNumber = quotationnumber();
                        ViewBag.MDBID = mdid.MDBID;
                        var mdid1 = db.MDBGeneralData.Find(mdid.MDBID);
                        var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdid1.MDBID).OrderBy(m => m.MDBCPDID).First();
                        ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                        if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                        {
                            ViewBag.dear = "Madam";
                        }
                        else
                        {
                            ViewBag.dear = "Sir";
                        }
                        ViewBag.OrganizationName = mdid1.OrganizationName;
                        ViewBag.AddressLine1 = mdid1.AddressLine1;
                        ViewBag.AddressLine2 = mdid1.AddressLine2;
                        ViewBag.AddressLine3 = mdid1.AddressLine3;
                        ViewBag.City = mdid1.City;
                        ViewBag.Pincode = mdid1.Pincode;
                        ViewBag.State = mdid1.State;
                        ViewBag.CompanyUniqueID = mdid1.CompanyUniqueID;
                        ViewBag.NullError = false;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName"); return View(mdb);
                    }
                    else
                    {
                        ViewBag.NullError = true;
                        ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                        ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                        ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                        ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                        ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                        return View(mdb);
                    }
                }
                //If all the above conditions fails
                else
                {
                    Quotation mdb = new Quotation();
                    //mdb.QGEquipPayment.annexure = "For the General terms & conditions of sale from Buhler India Pvt.Ltd., please refer to the Annexure attached, which is an integral part of this offer.\nAn order confirmation will be issued by Buhler India Pvt. Ltd. on acceptance of the order.";
                    ViewBag.NullError = true;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(mdb);
                }


        }

        //
        // POST: /Rice Mill Quotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiceMillGenerate(Quotation maindatabase, int? mdbid, string MainSection)
        {
            //Session["NoModel"] = null;
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                ViewBag.NullError = false;

                //j. Code to add MotorType when motorQ is With-Motor
                //if (maindatabase.QGEquipGeneralData.MotorQ == "With-Motor")
                //{
                //    maindatabase.QGEquipGeneralData.MotorType = "Foot Motor Only";
                //}

                //if (maindatabase.QGEquipTableData1.ProductModelID != 0 && maindatabase.QGEquipGeneralData.PaddySize != null && maindatabase.QGEquipGeneralData.TypeRice != null && maindatabase.QGEquipGeneralData.ProductVariety != null)
                //{

                if (maindatabase.QGEquipTableData1.ProductModelID != 0 && maindatabase.QGEquipGeneralData.ProductVariety != null)
                {
                    //if (MainSection != null)
                    //{
                    //    if (MainSection == "Polising")
                    //    {
                    //        String query1 = "SELECT * FROM OCMPolisher WHERE GrainType = '" + maindatabase.QGEquipGeneralData.PaddySize + "' and Process='" + maindatabase.QGEquipGeneralData.TypeRice + "' and Pass='" + maindatabase.QGEquipGeneralData.Pass + "' and Capacity='" + maindatabase.QGEquipGeneralData.Capacity + "' and PolishRequirement='" + maindatabase.QGEquipGeneralData.PolishRequirement + "' and MotorQ='" + maindatabase.QGEquipGeneralData.MotorQ + "' and MotorType='" + maindatabase.QGEquipGeneralData.MotorType + "' and MotorRating='" + maindatabase.QGEquipGeneralData.MotorRating + "';";
                    //        var ocmpolisherist = db.Database.SqlQuery<OCMPolisher>(query1).SingleOrDefault();
                    //        if (ocmpolisherist == null)
                    //        {
                    //            Session["NoModel"] = "Selected Model Not available";
                    //            ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    //            ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                    //            ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");

                    //            return RedirectToAction("RiceMillGenerate");
                    //        }
                    //    }
                    //    else if (MainSection == "Whitening")
                    //    {
                    //        String query1 = "SELECT * FROM OCMWhitner WHERE GrainType = '" + maindatabase.QGEquipGeneralData.PaddySize + "' and Process='" + maindatabase.QGEquipGeneralData.TypeRice + "' and Pass='" + maindatabase.QGEquipGeneralData.Pass + "' and Capacity='" + maindatabase.QGEquipGeneralData.Capacity + "' and MotorQ ='" + maindatabase.QGEquipGeneralData.MotorQ + "' and MotorType='" + maindatabase.QGEquipGeneralData.MotorType + "' and MotorRating='" + maindatabase.QGEquipGeneralData.MotorRating + "';";
                    //        var ocmwhitner = db.Database.SqlQuery<OCMWhitner>(query1).SingleOrDefault();
                    //        if (ocmwhitner == null)
                    //        {
                    //            Session["NoModel"] = "Selected Model Not available";
                    //            ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    //            ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new {m.MasterProductID,m.MasterProductName,m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    //            ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    //            ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID,m.ProductName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                    //            ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    //            ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID,m.ProductModelName,m.IsDeleted}).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");

                    //            return RedirectToAction("RiceMillGenerate");
                    //        }
                    //    }
                    //}

                    maindatabase.QGEquipGeneralData.CPID = loginname.CPID;
                    maindatabase.QGEquipGeneralData.MDBID = (int)(mdbid);
                    maindatabase.QGEquipGeneralData.IsRiceMill = 1;
                    db.QGEquipGeneralData.Add(maindatabase.QGEquipGeneralData);
                    db.SaveChanges();
                    var cpid = from s in db.QGEquipGeneralData
                               select s;
                    cpid = cpid.OrderByDescending(m => m.QGID);
                    int qgid = cpid.Select(m => m.QGID).First();
                    maindatabase.QGEquipPayment.QGID = qgid;
                    db.QGEquipPayment.Add(maindatabase.QGEquipPayment);
                    db.SaveChanges();

                    maindatabase.QGEquipTableData1.QGID = qgid;
                    int mpid = maindatabase.QGEquipTableData1.MasterProductID;
                    int prdid = maindatabase.QGEquipTableData1.ProductID;
                    var mpnm = db.MasterProducts.Where(m => m.MasterProductID == mpid).Select(m => m.MasterProductName).SingleOrDefault();
                    var prdnm = db.Products.Where(m => m.ProductID == prdid).Select(m => m.ProductName).SingleOrDefault();
                    maindatabase.QGEquipTableData1.MasterProductName = mpnm;
                    maindatabase.QGEquipTableData1.ProductName = prdnm;
                    db.QGEquipTableData.Add(maindatabase.QGEquipTableData1);
                    db.SaveChanges();


                    if (maindatabase.QGEquipTableData2.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData2.QGID = qgid;
                        int mpid2 = maindatabase.QGEquipTableData2.MasterProductID;
                        int prdid2 = maindatabase.QGEquipTableData2.ProductID;
                        var mpnm2 = db.MasterProducts.Where(m => m.MasterProductID == mpid2).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm2 = db.Products.Where(m => m.ProductID == prdid2).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData2.MasterProductName = mpnm2;
                        maindatabase.QGEquipTableData2.ProductName = prdnm2;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData3.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData3.QGID = qgid;
                        int mpid3 = maindatabase.QGEquipTableData3.MasterProductID;
                        int prdid3 = maindatabase.QGEquipTableData3.ProductID;
                        var mpnm3 = db.MasterProducts.Where(m => m.MasterProductID == mpid3).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm3 = db.Products.Where(m => m.ProductID == prdid3).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData3.MasterProductName = mpnm3;
                        maindatabase.QGEquipTableData3.ProductName = prdnm3;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData4.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData4.QGID = qgid;
                        int mpid4 = maindatabase.QGEquipTableData4.MasterProductID;
                        int prdid4 = maindatabase.QGEquipTableData4.ProductID;
                        var mpnm4 = db.MasterProducts.Where(m => m.MasterProductID == mpid4).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm4 = db.Products.Where(m => m.ProductID == prdid4).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData4.MasterProductName = mpnm4;
                        maindatabase.QGEquipTableData4.ProductName = prdnm4;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData4);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData5.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData5.QGID = qgid;
                        int mpid5 = maindatabase.QGEquipTableData5.MasterProductID;
                        int prdid5 = maindatabase.QGEquipTableData5.ProductID;
                        var mpnm5 = db.MasterProducts.Where(m => m.MasterProductID == mpid5).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm5 = db.Products.Where(m => m.ProductID == prdid5).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData5.MasterProductName = mpnm5;
                        maindatabase.QGEquipTableData5.ProductName = prdnm5;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData5);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData6.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData6.QGID = qgid;
                        int mpid6 = maindatabase.QGEquipTableData6.MasterProductID;
                        int prdid6 = maindatabase.QGEquipTableData6.ProductID;
                        var mpnm6 = db.MasterProducts.Where(m => m.MasterProductID == mpid6).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm6 = db.Products.Where(m => m.ProductID == prdid6).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData6.MasterProductName = mpnm6;
                        maindatabase.QGEquipTableData6.ProductName = prdnm6;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData6);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData7.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData7.QGID = qgid;
                        int mpid7 = maindatabase.QGEquipTableData7.MasterProductID;
                        int prdid7 = maindatabase.QGEquipTableData7.ProductID;
                        var mpnm7 = db.MasterProducts.Where(m => m.MasterProductID == mpid7).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm7 = db.Products.Where(m => m.ProductID == prdid7).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData7.MasterProductName = mpnm7;
                        maindatabase.QGEquipTableData7.ProductName = prdnm7;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData7);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData8.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData8.QGID = qgid;
                        int mpid8 = maindatabase.QGEquipTableData8.MasterProductID;
                        int prdid8 = maindatabase.QGEquipTableData8.ProductID;
                        var mpnm8 = db.MasterProducts.Where(m => m.MasterProductID == mpid8).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm8 = db.Products.Where(m => m.ProductID == prdid8).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData8.MasterProductName = mpnm8;
                        maindatabase.QGEquipTableData8.ProductName = prdnm8;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData8);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData9.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData9.QGID = qgid;
                        int mpid9 = maindatabase.QGEquipTableData9.MasterProductID;
                        int prdid9 = maindatabase.QGEquipTableData9.ProductID;
                        var mpnm9 = db.MasterProducts.Where(m => m.MasterProductID == mpid9).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm9 = db.Products.Where(m => m.ProductID == prdid9).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData9.MasterProductName = mpnm9;
                        maindatabase.QGEquipTableData9.ProductName = prdnm9;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData9);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData10.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData10.QGID = qgid;
                        int mpid10 = maindatabase.QGEquipTableData10.MasterProductID;
                        int prdid10 = maindatabase.QGEquipTableData10.ProductID;
                        var mpnm10 = db.MasterProducts.Where(m => m.MasterProductID == mpid10).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm10 = db.Products.Where(m => m.ProductID == prdid10).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData10.MasterProductName = mpnm10;
                        maindatabase.QGEquipTableData10.ProductName = prdnm10;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData10);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData11.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData11.QGID = qgid;
                        int mpid11 = maindatabase.QGEquipTableData11.MasterProductID;
                        int prdid11 = maindatabase.QGEquipTableData11.ProductID;
                        var mpnm11 = db.MasterProducts.Where(m => m.MasterProductID == mpid11).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm11 = db.Products.Where(m => m.ProductID == prdid11).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData11.MasterProductName = mpnm11;
                        maindatabase.QGEquipTableData11.ProductName = prdnm11;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData11);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData12.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData12.QGID = qgid;
                        int mpid12 = maindatabase.QGEquipTableData12.MasterProductID;
                        int prdid12 = maindatabase.QGEquipTableData12.ProductID;
                        var mpnm12 = db.MasterProducts.Where(m => m.MasterProductID == mpid12).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm12 = db.Products.Where(m => m.ProductID == prdid12).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData12.MasterProductName = mpnm12;
                        maindatabase.QGEquipTableData12.ProductName = prdnm12;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData12);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData13.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData13.QGID = qgid;
                        int mpid13 = maindatabase.QGEquipTableData13.MasterProductID;
                        int prdid13 = maindatabase.QGEquipTableData13.ProductID;
                        var mpnm13 = db.MasterProducts.Where(m => m.MasterProductID == mpid13).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm13 = db.Products.Where(m => m.ProductID == prdid13).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData13.MasterProductName = mpnm13;
                        maindatabase.QGEquipTableData13.ProductName = prdnm13;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData13);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData14.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData14.QGID = qgid;
                        int mpid14 = maindatabase.QGEquipTableData14.MasterProductID;
                        int prdid14 = maindatabase.QGEquipTableData14.ProductID;
                        var mpnm14 = db.MasterProducts.Where(m => m.MasterProductID == mpid14).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm14 = db.Products.Where(m => m.ProductID == prdid14).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData14.MasterProductName = mpnm14;
                        maindatabase.QGEquipTableData14.ProductName = prdnm14;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData14);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData15.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData15.QGID = qgid;
                        int mpid15 = maindatabase.QGEquipTableData15.MasterProductID;
                        int prdid15 = maindatabase.QGEquipTableData15.ProductID;
                        var mpnm15 = db.MasterProducts.Where(m => m.MasterProductID == mpid15).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm15 = db.Products.Where(m => m.ProductID == prdid15).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData15.MasterProductName = mpnm15;
                        maindatabase.QGEquipTableData15.ProductName = prdnm15;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData15);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData16.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData16.QGID = qgid;
                        int mpid16 = maindatabase.QGEquipTableData16.MasterProductID;
                        int prdid16 = maindatabase.QGEquipTableData16.ProductID;
                        var mpnm16 = db.MasterProducts.Where(m => m.MasterProductID == mpid16).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm16 = db.Products.Where(m => m.ProductID == prdid16).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData16.MasterProductName = mpnm16;
                        maindatabase.QGEquipTableData16.ProductName = prdnm16;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData16);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData17.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData17.QGID = qgid;
                        int mpid17 = maindatabase.QGEquipTableData17.MasterProductID;
                        int prdid17 = maindatabase.QGEquipTableData17.ProductID;
                        var mpnm17 = db.MasterProducts.Where(m => m.MasterProductID == mpid17).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm17 = db.Products.Where(m => m.ProductID == prdid17).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData17.MasterProductName = mpnm17;
                        maindatabase.QGEquipTableData17.ProductName = prdnm17;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData17);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData18.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData18.QGID = qgid;
                        int mpid18 = maindatabase.QGEquipTableData18.MasterProductID;
                        int prdid18 = maindatabase.QGEquipTableData18.ProductID;
                        var mpnm18 = db.MasterProducts.Where(m => m.MasterProductID == mpid18).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm18 = db.Products.Where(m => m.ProductID == prdid18).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData18.MasterProductName = mpnm18;
                        maindatabase.QGEquipTableData18.ProductName = prdnm18;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData18);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData19.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData19.QGID = qgid;
                        int mpid19 = maindatabase.QGEquipTableData19.MasterProductID;
                        int prdid19 = maindatabase.QGEquipTableData19.ProductID;
                        var mpnm19 = db.MasterProducts.Where(m => m.MasterProductID == mpid19).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm19 = db.Products.Where(m => m.ProductID == prdid19).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData19.MasterProductName = mpnm19;
                        maindatabase.QGEquipTableData19.ProductName = prdnm19;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData19);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData20.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData20.QGID = qgid;
                        int mpid20 = maindatabase.QGEquipTableData20.MasterProductID;
                        int prdid20 = maindatabase.QGEquipTableData20.ProductID;
                        var mpnm20 = db.MasterProducts.Where(m => m.MasterProductID == mpid20).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm20 = db.Products.Where(m => m.ProductID == prdid20).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData20.MasterProductName = mpnm20;
                        maindatabase.QGEquipTableData20.ProductName = prdnm20;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData20);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData21.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData21.QGID = qgid;
                        int mpid21 = maindatabase.QGEquipTableData21.MasterProductID;
                        int prdid21 = maindatabase.QGEquipTableData21.ProductID;
                        var mpnm21 = db.MasterProducts.Where(m => m.MasterProductID == mpid21).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm21 = db.Products.Where(m => m.ProductID == prdid21).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData21.MasterProductName = mpnm21;
                        maindatabase.QGEquipTableData21.ProductName = prdnm21;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData21);
                        db.SaveChanges();
                    }

                    //Insert into SOTRM table
                    var qgtblid = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m => m.QGEquipGeneralData.Islatest == 0).Select(a => new { a.ProductModel.ProductModelName, a.Quantity, a.MasterProductID, a.ProductID }).ToList();
                    var cnt = qgtblid.Count();
                    foreach (var qg in qgtblid)
                    {

                        SOTRM sotrm = new SOTRM();
                        sotrm.QGID = qgid;
                        sotrm.CPID = loginname.CPID;
                        sotrm.Equipment = qg.ProductModelName;
                        sotrm.Quantity = qg.Quantity;
                        sotrm.MasterProductID = qg.MasterProductID;
                        sotrm.ProductID = qg.ProductID;
                        db.SOTRM.Add(sotrm);
                        db.SaveChanges();
                    }
                    ////return RedirectToAction("RMReportGeneration", new { qgid = qgid });

                    String updateparent = "<script>window.open('/Quotation/RMReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = document.location.pathname;myFunction();</script>";
                    //return JavaScript("myFunction()");
                   // ViewBag.message = "1";
                    return Content(updateparent);
                }
                ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View(maindatabase);
            }
            else
            {
                ViewBag.OrganizationName = "OrganizationName";
                ViewBag.AddressLine1 = "Address(Line1)";
                ViewBag.AddressLine2 = "Address(Line2)";
                ViewBag.AddressLine3 = "Address(Line3)";
                ViewBag.City = "City";
                ViewBag.Pincode = "Pincode";
                ViewBag.State = "State";
                ViewBag.CompanyUniqueID = "CompanyUniqueID";
                ViewBag.NullError = true;

                ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View(maindatabase);
            }
        }

        //
        // GET: /Quotation/Rice Mill Revise
        public ActionResult RiceMillRevise(int qgid = 0, String quotnumber = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation }).SingleOrDefault();

            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            ViewBag.Designation = loginname.Designation;

            DateTime quotdat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = quotdat.ToString("dd-MM-yyyy");
            if (qgid != 0)
            {
                Quotation quo = new Quotation();
                var qgdb = db.QGEquipGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.QGEquipTableData.Where(m => m.QGID == qgid);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.QGEquipGeneralData = qgdb;
                    quo.QGEquipPayment = db.QGEquipPayment.Find(qgid);
                    if (modelcnt == 21)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.QGEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.QGEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.QGEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.QGEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.QGEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.QGEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.QGEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.QGEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.QGEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.QGEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.QGEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.QGEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.QGEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.QGEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.QGEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.QGEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.QGEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.QGEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.QGEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.QGEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = modelcount[17];
                        quo.QGEquipTableData19 = modelcount[18];
                        quo.QGEquipTableData20 = modelcount[19];
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = modelcount[17];
                        quo.QGEquipTableData19 = modelcount[18];
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = modelcount[17];
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = null;
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = null;
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = Revisequotationnumber(quotnum);
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            else if (quotnumber != null)
            {
                Quotation quo = new Quotation();
                var qgdb1 = db.QGEquipGeneralData.Where(m => m.QuotationNumber == quotnumber).Select(m => m.QGID).SingleOrDefault();
                var qgdb = db.QGEquipGeneralData.Find(qgdb1);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var models = db.QGEquipTableData.Where(m => m.QGID == qgdb1);
                    var modelcount = models.ToList();
                    int modelcnt = modelcount.Count;
                    quo.QGEquipGeneralData = qgdb;
                    quo.QGEquipPayment = db.QGEquipPayment.Find(qgdb1);
                    if (modelcnt == 21)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        ViewBag.ModelQty1 = modelcount[0].Quantity;
                        quo.QGEquipTableData2 = modelcount[1];
                        ViewBag.ModelQty2 = modelcount[0].Quantity;
                        quo.QGEquipTableData3 = modelcount[2];
                        ViewBag.ModelQty3 = modelcount[0].Quantity;
                        quo.QGEquipTableData4 = modelcount[3];
                        ViewBag.ModelQty4 = modelcount[0].Quantity;
                        quo.QGEquipTableData5 = modelcount[4];
                        ViewBag.ModelQty5 = modelcount[0].Quantity;
                        quo.QGEquipTableData6 = modelcount[5];
                        ViewBag.ModelQty6 = modelcount[0].Quantity;
                        quo.QGEquipTableData7 = modelcount[6];
                        ViewBag.ModelQty7 = modelcount[0].Quantity;
                        quo.QGEquipTableData8 = modelcount[7];
                        ViewBag.ModelQty8 = modelcount[0].Quantity;
                        quo.QGEquipTableData9 = modelcount[8];
                        ViewBag.ModelQty9 = modelcount[0].Quantity;
                        quo.QGEquipTableData10 = modelcount[9];
                        ViewBag.ModelQty10 = modelcount[0].Quantity;
                        quo.QGEquipTableData11 = modelcount[10];
                        ViewBag.ModelQty11 = modelcount[0].Quantity;
                        quo.QGEquipTableData12 = modelcount[11];
                        ViewBag.ModelQty12 = modelcount[0].Quantity;
                        quo.QGEquipTableData13 = modelcount[12];
                        ViewBag.ModelQty13 = modelcount[0].Quantity;
                        quo.QGEquipTableData14 = modelcount[13];
                        ViewBag.ModelQty14 = modelcount[0].Quantity;
                        quo.QGEquipTableData15 = modelcount[14];
                        ViewBag.ModelQty15 = modelcount[0].Quantity;
                        quo.QGEquipTableData16 = modelcount[15];
                        ViewBag.ModelQty16 = modelcount[0].Quantity;
                        quo.QGEquipTableData17 = modelcount[16];
                        ViewBag.ModelQty17 = modelcount[0].Quantity;
                        quo.QGEquipTableData18 = modelcount[17];
                        ViewBag.ModelQty18 = modelcount[0].Quantity;
                        quo.QGEquipTableData19 = modelcount[18];
                        ViewBag.ModelQty19 = modelcount[0].Quantity;
                        quo.QGEquipTableData20 = modelcount[19];
                        ViewBag.ModelQty20 = modelcount[0].Quantity;
                        quo.QGEquipTableData21 = modelcount[20];
                        ViewBag.ModelQty21 = modelcount[0].Quantity;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = true;
                    }
                    else if (modelcnt == 20)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = modelcount[17];
                        quo.QGEquipTableData19 = modelcount[18];
                        quo.QGEquipTableData20 = modelcount[19];
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = true;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 19)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = modelcount[17];
                        quo.QGEquipTableData19 = modelcount[18];
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = true;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 18)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = modelcount[17];
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = true;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 17)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = modelcount[16];
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = true;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 16)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = modelcount[15];
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = true;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 15)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = modelcount[14];
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = true;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 14)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = modelcount[13];
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = true;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 13)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = modelcount[12];
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = true;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 12)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = modelcount[11];
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = true;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 11)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = modelcount[10];
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = true;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 10)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = modelcount[9];
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = true;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 9)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = modelcount[8];
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = true;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 8)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = modelcount[7];
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = true;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 7)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = modelcount[6];
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = true;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 6)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = modelcount[5];
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = true;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 5)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = modelcount[4];
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = true;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 4)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = modelcount[3];
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = true;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 3)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = modelcount[2];
                        quo.QGEquipTableData4 = null;
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = true;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 2)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = modelcount[1];
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = true;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    else if (modelcnt == 1)
                    {
                        quo.QGEquipTableData1 = modelcount[0];
                        quo.QGEquipTableData2 = null;
                        quo.QGEquipTableData3 = null;
                        quo.QGEquipTableData4 = null;
                        quo.QGEquipTableData5 = null;
                        quo.QGEquipTableData6 = null;
                        quo.QGEquipTableData7 = null;
                        quo.QGEquipTableData8 = null;
                        quo.QGEquipTableData9 = null;
                        quo.QGEquipTableData10 = null;
                        quo.QGEquipTableData11 = null;
                        quo.QGEquipTableData12 = null;
                        quo.QGEquipTableData13 = null;
                        quo.QGEquipTableData14 = null;
                        quo.QGEquipTableData15 = null;
                        quo.QGEquipTableData16 = null;
                        quo.QGEquipTableData17 = null;
                        quo.QGEquipTableData18 = null;
                        quo.QGEquipTableData19 = null;
                        quo.QGEquipTableData20 = null;
                        quo.QGEquipTableData21 = null;
                        ViewBag.EquipTable2 = false;
                        ViewBag.EquipTable3 = false;
                        ViewBag.EquipTable4 = false;
                        ViewBag.EquipTable5 = false;
                        ViewBag.EquipTable6 = false;
                        ViewBag.EquipTable7 = false;
                        ViewBag.EquipTable8 = false;
                        ViewBag.EquipTable9 = false;
                        ViewBag.EquipTable10 = false;
                        ViewBag.EquipTable11 = false;
                        ViewBag.EquipTable12 = false;
                        ViewBag.EquipTable13 = false;
                        ViewBag.EquipTable14 = false;
                        ViewBag.EquipTable15 = false;
                        ViewBag.EquipTable16 = false;
                        ViewBag.EquipTable17 = false;
                        ViewBag.EquipTable18 = false;
                        ViewBag.EquipTable19 = false;
                        ViewBag.EquipTable20 = false;
                        ViewBag.EquipTable21 = false;
                    }
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    ViewBag.ContactPersonName = cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
                    if (cpmdb.Title == "Miss" || cpmdb.Title == "Mrs")
                    {
                        ViewBag.dear = "Madam";
                    }
                    else
                    {
                        ViewBag.dear = "Sir";
                    }
                    ViewBag.QuotationNumber = Revisequotationnumber(quotnum);
                    ViewBag.MDBID = mdbdet.MDBID;
                    ViewBag.OrganizationName = mdbdet.OrganizationName;
                    ViewBag.AddressLine1 = mdbdet.AddressLine1;
                    ViewBag.AddressLine2 = mdbdet.AddressLine2;
                    ViewBag.AddressLine3 = mdbdet.AddressLine3;
                    ViewBag.City = mdbdet.City;
                    ViewBag.Pincode = mdbdet.Pincode;
                    ViewBag.State = mdbdet.State;
                    ViewBag.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    ViewBag.NullError = false;
                    ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                    ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                    ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                    ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                    ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                    return View(quo);
                }
            }
            ViewBag.NullError = false;
            return View();
        }

        //
        // POST: /Quotation/ReviseQuotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RiceMillRevise(Quotation maindatabase, int? mdbid)
        {
            if (mdbid.HasValue)
            {
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
                if (maindatabase.QGEquipTableData1.ProductModelID != 0)
                {

                    var revqg = maindatabase.QGEquipGeneralData.QuotationNumber;
                    var revsplit = revqg.Split('-');
                    var revcomb = revsplit[0] + "-" + revsplit[1] + "-" + revsplit[2];
                    var revnum = Convert.ToInt32(revsplit[3]);
                    var prvrevisednum = "";
                    if (revnum != 0)
                    {
                        revnum -= 1;
                    }
                    if (revnum.ToString().Length == 1)
                    {
                        prvrevisednum = "0" + revnum;
                    }
                    else
                    {
                        prvrevisednum = revnum.ToString();
                    }
                    var fullqn = revcomb + "-" + prvrevisednum;
                    var previosqg = db.QGEquipGeneralData.Where(m => m.QuotationNumber == fullqn).First();
                    previosqg.Islatest = 1;
                    db.Entry(previosqg).State = EntityState.Modified;
                    db.SaveChanges();

                    maindatabase.QGEquipGeneralData.CPID = loginname.CPID;
                    maindatabase.QGEquipGeneralData.MDBID = (int)(mdbid);
                    maindatabase.QGEquipGeneralData.IsRiceMill = 1;
                    db.QGEquipGeneralData.Add(maindatabase.QGEquipGeneralData);
                    db.SaveChanges();
                    var qg = from s in db.QGEquipGeneralData
                             select s;
                    qg = qg.OrderByDescending(m => m.QGID);
                    int qgid = qg.Select(m => m.QGID).First();
                    maindatabase.QGEquipPayment.QGID = qgid;
                    db.QGEquipPayment.Add(maindatabase.QGEquipPayment);
                    db.SaveChanges();

                    maindatabase.QGEquipTableData1.QGID = qgid;
                    int mpid = maindatabase.QGEquipTableData1.MasterProductID;
                    int prdid = maindatabase.QGEquipTableData1.ProductID;
                    var mpnm = db.MasterProducts.Where(m => m.MasterProductID == mpid).Select(m => m.MasterProductName).SingleOrDefault();
                    var prdnm = db.Products.Where(m => m.ProductID == prdid).Select(m => m.ProductName).SingleOrDefault();
                    maindatabase.QGEquipTableData1.MasterProductName = mpnm;
                    maindatabase.QGEquipTableData1.ProductName = prdnm;
                    db.QGEquipTableData.Add(maindatabase.QGEquipTableData1);
                    db.SaveChanges();

                    if (maindatabase.QGEquipTableData2.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData2.QGID = qgid;
                        int mpid2 = maindatabase.QGEquipTableData2.MasterProductID;
                        int prdid2 = maindatabase.QGEquipTableData2.ProductID;
                        var mpnm2 = db.MasterProducts.Where(m => m.MasterProductID == mpid2).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm2 = db.Products.Where(m => m.ProductID == prdid2).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData2.MasterProductName = mpnm2;
                        maindatabase.QGEquipTableData2.ProductName = prdnm2;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData2);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData3.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData3.QGID = qgid;
                        int mpid3 = maindatabase.QGEquipTableData3.MasterProductID;
                        int prdid3 = maindatabase.QGEquipTableData3.ProductID;
                        var mpnm3 = db.MasterProducts.Where(m => m.MasterProductID == mpid3).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm3 = db.Products.Where(m => m.ProductID == prdid3).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData3.MasterProductName = mpnm3;
                        maindatabase.QGEquipTableData3.ProductName = prdnm3;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData3);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData4.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData4.QGID = qgid;
                        int mpid4 = maindatabase.QGEquipTableData4.MasterProductID;
                        int prdid4 = maindatabase.QGEquipTableData4.ProductID;
                        var mpnm4 = db.MasterProducts.Where(m => m.MasterProductID == mpid4).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm4 = db.Products.Where(m => m.ProductID == prdid4).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData4.MasterProductName = mpnm4;
                        maindatabase.QGEquipTableData4.ProductName = prdnm4;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData4);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData5.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData5.QGID = qgid;
                        int mpid5 = maindatabase.QGEquipTableData5.MasterProductID;
                        int prdid5 = maindatabase.QGEquipTableData5.ProductID;
                        var mpnm5 = db.MasterProducts.Where(m => m.MasterProductID == mpid5).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm5 = db.Products.Where(m => m.ProductID == prdid5).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData5.MasterProductName = mpnm5;
                        maindatabase.QGEquipTableData5.ProductName = prdnm5;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData5);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData6.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData6.QGID = qgid;
                        int mpid6 = maindatabase.QGEquipTableData6.MasterProductID;
                        int prdid6 = maindatabase.QGEquipTableData6.ProductID;
                        var mpnm6 = db.MasterProducts.Where(m => m.MasterProductID == mpid6).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm6 = db.Products.Where(m => m.ProductID == prdid6).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData6.MasterProductName = mpnm6;
                        maindatabase.QGEquipTableData6.ProductName = prdnm6;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData6);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData7.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData7.QGID = qgid;
                        int mpid7 = maindatabase.QGEquipTableData7.MasterProductID;
                        int prdid7 = maindatabase.QGEquipTableData7.ProductID;
                        var mpnm7 = db.MasterProducts.Where(m => m.MasterProductID == mpid7).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm7 = db.Products.Where(m => m.ProductID == prdid7).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData7.MasterProductName = mpnm7;
                        maindatabase.QGEquipTableData7.ProductName = prdnm7;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData7);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData8.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData8.QGID = qgid;
                        int mpid8 = maindatabase.QGEquipTableData8.MasterProductID;
                        int prdid8 = maindatabase.QGEquipTableData8.ProductID;
                        var mpnm8 = db.MasterProducts.Where(m => m.MasterProductID == mpid8).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm8 = db.Products.Where(m => m.ProductID == prdid8).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData8.MasterProductName = mpnm8;
                        maindatabase.QGEquipTableData8.ProductName = prdnm8;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData8);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData9.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData9.QGID = qgid;
                        int mpid9 = maindatabase.QGEquipTableData9.MasterProductID;
                        int prdid9 = maindatabase.QGEquipTableData9.ProductID;
                        var mpnm9 = db.MasterProducts.Where(m => m.MasterProductID == mpid9).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm9 = db.Products.Where(m => m.ProductID == prdid9).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData9.MasterProductName = mpnm9;
                        maindatabase.QGEquipTableData9.ProductName = prdnm9;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData9);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData10.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData10.QGID = qgid;
                        int mpid10 = maindatabase.QGEquipTableData10.MasterProductID;
                        int prdid10 = maindatabase.QGEquipTableData10.ProductID;
                        var mpnm10 = db.MasterProducts.Where(m => m.MasterProductID == mpid10).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm10 = db.Products.Where(m => m.ProductID == prdid10).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData10.MasterProductName = mpnm10;
                        maindatabase.QGEquipTableData10.ProductName = prdnm10;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData10);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData11.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData11.QGID = qgid;
                        int mpid11 = maindatabase.QGEquipTableData11.MasterProductID;
                        int prdid11 = maindatabase.QGEquipTableData11.ProductID;
                        var mpnm11 = db.MasterProducts.Where(m => m.MasterProductID == mpid11).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm11 = db.Products.Where(m => m.ProductID == prdid11).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData11.MasterProductName = mpnm11;
                        maindatabase.QGEquipTableData11.ProductName = prdnm11;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData11);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData12.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData12.QGID = qgid;
                        int mpid12 = maindatabase.QGEquipTableData12.MasterProductID;
                        int prdid12 = maindatabase.QGEquipTableData12.ProductID;
                        var mpnm12 = db.MasterProducts.Where(m => m.MasterProductID == mpid12).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm12 = db.Products.Where(m => m.ProductID == prdid12).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData12.MasterProductName = mpnm12;
                        maindatabase.QGEquipTableData12.ProductName = prdnm12;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData12);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData13.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData13.QGID = qgid;
                        int mpid13 = maindatabase.QGEquipTableData13.MasterProductID;
                        int prdid13 = maindatabase.QGEquipTableData13.ProductID;
                        var mpnm13 = db.MasterProducts.Where(m => m.MasterProductID == mpid13).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm13 = db.Products.Where(m => m.ProductID == prdid13).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData13.MasterProductName = mpnm13;
                        maindatabase.QGEquipTableData13.ProductName = prdnm13;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData13);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData14.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData14.QGID = qgid;
                        int mpid14 = maindatabase.QGEquipTableData14.MasterProductID;
                        int prdid14 = maindatabase.QGEquipTableData14.ProductID;
                        var mpnm14 = db.MasterProducts.Where(m => m.MasterProductID == mpid14).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm14 = db.Products.Where(m => m.ProductID == prdid14).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData14.MasterProductName = mpnm14;
                        maindatabase.QGEquipTableData14.ProductName = prdnm14;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData14);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData15.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData15.QGID = qgid;
                        int mpid15 = maindatabase.QGEquipTableData15.MasterProductID;
                        int prdid15 = maindatabase.QGEquipTableData15.ProductID;
                        var mpnm15 = db.MasterProducts.Where(m => m.MasterProductID == mpid15).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm15 = db.Products.Where(m => m.ProductID == prdid15).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData15.MasterProductName = mpnm15;
                        maindatabase.QGEquipTableData15.ProductName = prdnm15;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData15);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData16.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData16.QGID = qgid;
                        int mpid16 = maindatabase.QGEquipTableData16.MasterProductID;
                        int prdid16 = maindatabase.QGEquipTableData16.ProductID;
                        var mpnm16 = db.MasterProducts.Where(m => m.MasterProductID == mpid16).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm16 = db.Products.Where(m => m.ProductID == prdid16).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData16.MasterProductName = mpnm16;
                        maindatabase.QGEquipTableData16.ProductName = prdnm16;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData16);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData17.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData17.QGID = qgid;
                        int mpid17 = maindatabase.QGEquipTableData17.MasterProductID;
                        int prdid17 = maindatabase.QGEquipTableData17.ProductID;
                        var mpnm17 = db.MasterProducts.Where(m => m.MasterProductID == mpid17).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm17 = db.Products.Where(m => m.ProductID == prdid17).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData17.MasterProductName = mpnm17;
                        maindatabase.QGEquipTableData17.ProductName = prdnm17;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData17);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData18.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData18.QGID = qgid;
                        int mpid18 = maindatabase.QGEquipTableData18.MasterProductID;
                        int prdid18 = maindatabase.QGEquipTableData18.ProductID;
                        var mpnm18 = db.MasterProducts.Where(m => m.MasterProductID == mpid18).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm18 = db.Products.Where(m => m.ProductID == prdid18).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData18.MasterProductName = mpnm18;
                        maindatabase.QGEquipTableData18.ProductName = prdnm18;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData18);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData19.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData19.QGID = qgid;
                        int mpid19 = maindatabase.QGEquipTableData19.MasterProductID;
                        int prdid19 = maindatabase.QGEquipTableData19.ProductID;
                        var mpnm19 = db.MasterProducts.Where(m => m.MasterProductID == mpid19).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm19 = db.Products.Where(m => m.ProductID == prdid19).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData19.MasterProductName = mpnm19;
                        maindatabase.QGEquipTableData19.ProductName = prdnm19;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData19);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData20.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData20.QGID = qgid;
                        int mpid20 = maindatabase.QGEquipTableData20.MasterProductID;
                        int prdid20 = maindatabase.QGEquipTableData20.ProductID;
                        var mpnm20 = db.MasterProducts.Where(m => m.MasterProductID == mpid20).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm20 = db.Products.Where(m => m.ProductID == prdid20).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData20.MasterProductName = mpnm20;
                        maindatabase.QGEquipTableData20.ProductName = prdnm20;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData20);
                        db.SaveChanges();
                    }
                    if (maindatabase.QGEquipTableData21.ProductModelID != 0)
                    {
                        maindatabase.QGEquipTableData21.QGID = qgid;
                        int mpid21 = maindatabase.QGEquipTableData21.MasterProductID;
                        int prdid21 = maindatabase.QGEquipTableData21.ProductID;
                        var mpnm21 = db.MasterProducts.Where(m => m.MasterProductID == mpid21).Select(m => m.MasterProductName).SingleOrDefault();
                        var prdnm21 = db.Products.Where(m => m.ProductID == prdid21).Select(m => m.ProductName).SingleOrDefault();
                        maindatabase.QGEquipTableData21.MasterProductName = mpnm21;
                        maindatabase.QGEquipTableData21.ProductName = prdnm21;
                        db.QGEquipTableData.Add(maindatabase.QGEquipTableData21);
                        db.SaveChanges();
                    }

                    var sotprv = db.SOTRM.Where(m => m.QGEquipGeneralData.QuotationNumber == fullqn).ToList();
                    foreach (var sotli in sotprv)
                    {
                        sotli.Islatestquo = 1;
                        db.Entry(sotli).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    //Insert into SOTRM table
                    var qgtblid = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m => m.QGEquipGeneralData.Islatest == 0).Select(a => new { a.ProductModel.ProductModelName, a.Quantity, a.MasterProductID, a.ProductID }).ToList();
                    var cnt = qgtblid.Count();
                    foreach (var qg1 in qgtblid)
                    {

                        SOTRM sotrm = new SOTRM();
                        sotrm.QGID = qgid;
                        sotrm.CPID = loginname.CPID;
                        sotrm.Equipment = qg1.ProductModelName;
                        sotrm.Quantity = qg1.Quantity;
                        sotrm.MasterProductID = qg1.MasterProductID;
                        sotrm.ProductID = qg1.ProductID;
                        db.SOTRM.Add(sotrm);
                        db.SaveChanges();
                    }
                    //return RedirectToAction("ReportGeneration", new { qgid = qgid });

                    String updateparent = "<script>window.open('/Quotation/RMReportGeneration?qgid=" + qgid + "', '_blank', 'toolbar=no,status=no,menubar=no,resizable=no,fullscreen=yes,scrollbars=yes'); document.location = '/Quotation/RiceMillGenerate';</script>";
                    return Content(updateparent);
                }
                return RedirectToAction("RiceMillGenerate");
            }
            else
            {
                Quotation mdb = new Quotation();
                var mdid = db.MDBGeneralData.Find(mdbid);
                ViewBag.MDBID = mdid.MDBID;
                ViewBag.OrganizationName = mdid.OrganizationName;
                ViewBag.AddressLine1 = mdid.AddressLine1;
                ViewBag.AddressLine2 = mdid.AddressLine2;
                ViewBag.AddressLine3 = mdid.AddressLine3;
                ViewBag.City = mdid.City;
                ViewBag.Pincode = mdid.Pincode;
                ViewBag.State = mdid.State;
                ViewBag.CompanyUniqueID = mdid.CompanyUniqueID;
                ViewBag.NullError = false;
                ViewData["MasterProductID1"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID2"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID3"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID4"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID5"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID6"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID7"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID8"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID9"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID10"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID11"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID12"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID13"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID14"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID15"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID16"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID17"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID18"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID19"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID20"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");
                ViewData["MasterProductID21"] = new SelectList(db.MasterProducts.Select(m => new { m.MasterProductID, m.MasterProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "MasterProductID", "MasterProductName");

                ViewData["ProductID1"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID2"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID3"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID4"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID5"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID6"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID7"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID8"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID9"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID10"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID11"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID12"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID13"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID14"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID15"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID16"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID17"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID18"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID19"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID20"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");
                ViewData["ProductID21"] = new SelectList(db.Products.Select(m => new { m.ProductID, m.ProductName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductID", "ProductName");


                ViewData["ProductModelID1"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID2"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID3"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID4"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID5"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID6"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID7"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID8"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID9"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID10"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID11"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID12"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID13"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID14"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID15"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID16"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID17"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID18"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID19"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID20"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                ViewData["ProductModelID21"] = new SelectList(db.ProductModel.Select(m => new { m.ProductModelID, m.ProductModelName, m.IsDeleted }).Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View();
            }
        }

        const int pageSize1 = 20;
        bool getdetailsclick1 = false;
        //Sorting Paging & Searching
        [HttpGet]
        public ActionResult RMMainExistingQuotation(int page = 1, int sortBy = 1, bool isAsc = true, string cunam = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            IEnumerable<QGEquipGeneralData> quotations = db.QGEquipGeneralData.Where(
                    p => cunam == null
                    || p.MDBGeneralData.OrganizationName.Contains(cunam)).Where(m => m.IsRiceMill == 1).Where(m => m.CPID == loginname.CPID).Include(q => q.MDBGeneralData);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    quotations = isAsc ? quotations.OrderBy(p => p.QGID) : quotations.OrderByDescending(p => p.QGID);
                    break;

                case 2:
                    quotations = isAsc ? quotations.OrderBy(p => p.QuotationNumber) : quotations.OrderByDescending(p => p.QuotationNumber);
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
                    quotations = isAsc ? quotations.OrderBy(p => p.QuotationDate) : quotations.OrderByDescending(p => p.QuotationDate);
                    break;

                default:
                    quotations = isAsc ? quotations.OrderBy(p => p.QGID) : quotations.OrderByDescending(p => p.QGID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)quotations.Count() / pageSize1);

            quotations = quotations
                .Skip((page - 1) * pageSize1)
                .Take(pageSize1)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize1;

            ViewBag.Search = cunam;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;

            if (getdetailsclick1)
                ViewBag.IsSearch = true;
            if (cunam != null)
            {
                getdetailsclick1 = true;
                ViewBag.IsSearch = true;
            }
            ViewBag.NullError = false;
            ViewBag.CUNAM = cunam;
            ViewBag.IsSearch = true;
            return View(quotations);
        }

        /// <summary>
        /// Report Generation of Rice Mill
        /// </summary>
        /// <param name="qgid"></param>
        /// <returns></returns>
        public ActionResult RMReportGeneration(int? qgid)
        {
            if (qgid != 0)
            {
                Quotation quo = new Quotation();
                RRepModel RM = new RRepModel();
                var qgdb = db.QGEquipGeneralData.Find(qgid);
                if (qgdb != null)
                {
                    String quotnum = qgdb.QuotationNumber;
                    var cpid = qgdb.CPID;
                    var Logo = db.ChannelPartners.Find(cpid);

                    var channelPartnerLogo = "";
                    var Logopath = "";

                    var add1 = "Check Your Address";
                    ViewBag.capacity = qgdb.Capacity;
                    if(Logo != null)
                    {
                        channelPartnerLogo = Server.MapPath("~/Images/channelpartnerlogos/" + Logo.Logo);
                    }
                    Logopath = Server.MapPath("~/App_Data/buhler_logo.tif");

                    add1 = "13 D, KIADB Industrial Area, Attibele District, Bangalore - 562107, India";

                    //var Logopath1 = Server.MapPath("~/App_Data/SRKSLogoJan31.png");
                    var models = db.QGEquipTableData.Where(m => m.QGID == qgid);
                    var modelcount = models.ToList();
                    quo.QGEquipGeneralData = qgdb;
                    var paymentterms = db.QGEquipPayment.Where(m => m.QGID == qgid).First();
                    int mdbid = qgdb.MDBID;
                    var mdbdet = db.MDBGeneralData.Find(mdbid);
                    var cpmdb = db.MDBContactPersonData.Where(m => m.MDBID == mdbdet.MDBID).OrderBy(m => m.MDBCPDID).First();
                    RM.Logo = Logopath;
                    RM.footaddress = add1;
                    RM.RQGID = qgdb.QGID;
                    RM.annexure = paymentterms.annexure;
                    RM.CPQuotationNumber = qgdb.CPQuotationNumber;
                    RM.MDBID = mdbdet.MDBID.ToString();
                    RM.QuotationNumber = qgdb.QuotationNumber;
                    if (RM.CPQuotationNumber != null)
                    {
                        //RM.QuotationNumber = RM.CPQuotationNumber;
                        ViewData["qut"] = RM.CPQuotationNumber;
                    }
                    else
                    {
                        ViewData["qut"] = qgdb.QuotationNumber;
                    }

                    RM.QuotationDate = Convert.ToDateTime(qgdb.QuotationDate).ToString("dd-MM-yyyy");
                    RM.PaymentTerms = paymentterms.PaymentTerms;
                    RM.Delivery = paymentterms.Delivery;
                    RM.DateofDispatch = paymentterms.DateofDispatch;
                    RM.Transport = paymentterms.Transport;
                    RM.Freight = paymentterms.Freight;
                    RM.CST = paymentterms.CST;
                    RM.TransitInsu = paymentterms.TransitInsu;
                    RM.Commodity = paymentterms.Commodity;
                    RM.Validity = paymentterms.Validity;
                    RM.Subjectinfo = qgdb.Subjectinfo;
                    RM.KindAttention = qgdb.KindAttention;
                    RM.ProductVariety = qgdb.ProductVariety;
                    RM.Type = qgdb.TypeRice;
                    RM.PaddySize = qgdb.PaddySize;
                    RM.CompanyUniqueID = mdbdet.CompanyUniqueID;
                    RM.State = mdbdet.State;
                    RM.Pincode = mdbdet.Pincode;
                    RM.overallprice = paymentterms.overallprice;
                    RM.City = mdbdet.City;
                    RM.AddressLine1 = mdbdet.AddressLine1;
                    RM.AddressLine2 = mdbdet.AddressLine2;
                    RM.AddressLine3 = mdbdet.AddressLine3;
                    RM.OrganizationName = mdbdet.OrganizationName;
                    RM.ContactPersonName = cpmdb.Title + "." + cpmdb.FirstName + " " + cpmdb.MiddleName + " " + cpmdb.LastName;
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
                    ViewBag.IsFour = false;
                    ViewBag.IsFive = false;
                    ViewBag.IsSix = false;
                    ViewBag.IsSeven = false;
                    ViewBag.IsEight = false;
                    ViewBag.IsNine = false;
                    ViewBag.IsTen = false;
                    ViewBag.IsEleven = false;
                    ViewBag.IsTwelve = false;
                    ViewBag.IsThirteen = false;
                    ViewBag.IsFourteen = false;
                    ViewBag.IsFifteen = false;
                    ViewBag.IsSixteen = false;
                    ViewBag.IsSeventeen = false;
                    ViewBag.IsEighteen = false;
                    ViewBag.IsNinteen = false;
                    ViewBag.IsTwenty = false;
                    ViewBag.IsTwentyone = false;
                    RM.QGEquipTableData = modelcount;
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
                    else if (modelcount.Count == 4)
                    {
                        ViewBag.IsFour = true;
                    }
                    else if (modelcount.Count == 5)
                    {
                        ViewBag.IsFive = true;
                    }
                    else if (modelcount.Count == 6)
                    {
                        ViewBag.IsSix = true;
                    }
                    else if (modelcount.Count == 7)
                    {
                        ViewBag.IsSeven = true;
                    }
                    else if (modelcount.Count == 8)
                    {
                        ViewBag.IsEight = true;
                    }
                    else if (modelcount.Count == 9)
                    {
                        ViewBag.IsNine = true;
                    }
                    else if (modelcount.Count == 10)
                    {
                        ViewBag.IsTen = true;
                    }
                    else if (modelcount.Count == 11)
                    {
                        ViewBag.IsEleven = true;
                    }
                    else if (modelcount.Count == 12)
                    {
                        ViewBag.IsTwelve = true;
                    }
                    else if (modelcount.Count == 13)
                    {
                        ViewBag.IsThirteen = true;
                    }
                    else if (modelcount.Count == 14)
                    {
                        ViewBag.IsFourteen = true;
                    }
                    else if (modelcount.Count == 15)
                    {
                        ViewBag.IsFifteen = true;
                    }
                    else if (modelcount.Count == 16)
                    {
                        ViewBag.IsSixteen = true;
                    }
                    else if (modelcount.Count == 17)
                    {
                        ViewBag.IsSeventeen = true;
                    }
                    else if (modelcount.Count == 18)
                    {
                        ViewBag.IsEighteen = true;
                    }
                    else if (modelcount.Count == 19)
                    {
                        ViewBag.IsNinteen = true;
                    }
                    else if (modelcount.Count == 20)
                    {
                        ViewBag.IsTwenty = true;
                    }
                    else if (modelcount.Count == 21)
                    {
                        ViewBag.IsTwentyone = true;
                    }


                    var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
                    RM.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
                    RM.Designation = loginname.Designation;
                    var FileName = RM.QuotationNumber;
                    ViewBag.CPID = loginname.CPID;
                    var relativePath = "RMReportGeneration";
                    string content;
                    var view = ViewEngines.Engines.FindView(this.ControllerContext, relativePath, null);
                    ViewData.Model = RM;

                    //For Channel Partner Logo Purpose

                    //string channelPartnerLogo = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"), Logo.Logo);

                    String headerpath = Server.MapPath("~/App_Data/HeaderFile.html");
                    //String footerpath = Server.MapPath("~/App_Data/FooterFile.html");
                    string htmlheader = "<!DOCTYPE html><html><head><meta charset=\"utf-8\" /></head><body> <div style=\"height: 120px\"> <div style=\"width: 700px; float: left; height: 120px\"> </div> <div style=\"width: 300px; height: 120px; float: right\"> <table > <tr><th></th><th><img src= \"" + channelPartnerLogo + "\" width=\"150\" height=\"35\" /></th> </tr> </table> </div> </div> </body>";
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
                        var pdfconverter = new PDFConverter();
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
            return View("RiceMillGenerate");
        }

        //done on 28-12-2016
        //selection of Model only display sorting option
        [HttpPost]
        public ActionResult ProductVarietyPulses(string prdvariety)
        {
            IEnumerable<SelectListItem> MasterProductlist = new List<SelectListItem>();
            db = new SRKS_Synergy();
            if (!string.IsNullOrEmpty(prdvariety))
            {
                MasterProductlist = (from m in db.MasterProducts where m.IsDeleted == 0 select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.MasterProductName, Value = m.MasterProductID.ToString() });
                //if (prdvariety == "Pulses")
                //{
                //    MasterProductlist = (from m in db.MasterProducts where m.MasterProductName == "Sorting" select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.MasterProductName, Value = m.MasterProductID.ToString() });
                //}
                //else
                //{

                //}
            };
            var result = Json(new SelectList(MasterProductlist, "Value", "Text"));
            return result;
        }



        public interface IPDFConverter
        {
            byte[] Convert(string source, string commandLocation, String Footeraddress, String HeaderHtml);
        }

        public class PDFConverter : IPDFConverter
        {
            private const string HtmlToPdfExePath = "wkhtmltopdf.exe";
            private readonly ILog log = LogManager.GetLogger(typeof(PDFConverter));

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
}
