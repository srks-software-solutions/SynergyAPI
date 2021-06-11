using SRKSSynergy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SRKSSynergy.Controllers
{
    public class GlobalJsonCallsController : Controller
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

    }
}
