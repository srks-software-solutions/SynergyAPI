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

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class SOTRMController : Controller
    {
        // GET: /SOTRM/
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete MilName of LeadEnquiryRevised
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            if (User.IsInRole("Administrator"))
            {
                var result = (from r in db.SOTRM
                              where r.QGEquipGeneralData.MDBGeneralData.OrganizationName.ToLower().Contains(term.ToLower())
                              select new { r.QGEquipGeneralData.MDBGeneralData.OrganizationName }).Distinct();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (User.IsInRole("ZonalManager"))
            {
                var result = (from r in db.SOTRM
                              where r.QGEquipGeneralData.MDBGeneralData.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                              select new { r.QGEquipGeneralData.MDBGeneralData.OrganizationName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
            {
                var result = (from r in db.SOTRM
                              where r.QGEquipGeneralData.MDBGeneralData.OrganizationName.ToLower().Contains(term.ToLower()) && (r.CPID == loginname.CPID)
                              select new { r.QGEquipGeneralData.MDBGeneralData.OrganizationName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = "Please Select Valid Name";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SOTRMList(int page = 1, int sortBy = 1, bool isAsc = false, string custnm = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            var getcpid = loginname.CPID;

            if (User.IsInRole("Administrator"))
            {
                IEnumerable<SOTRM> query = db.SOTRM.Where(m => m.Islatestquo != 1)
                              .Where(p => custnm == null || p.QGEquipGeneralData.MDBGeneralData.OrganizationName.Contains(custnm))
                              .GroupBy(m => m.QGID)
                              .Select(g => g.FirstOrDefault()).OrderByDescending(g=>g.SOTRMID)
                              .ToList();
                int qcount = query.Count();
                ViewBag.countval = qcount;

                #region Sorting
                switch (sortBy)
                {
                    case 1:
                        query = isAsc ? query.OrderBy(p => p.SOTRMID) : query.OrderByDescending(p => p.SOTRMID);
                        break;

                    case 2:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.MDBGeneralData.OrganizationName) : query.OrderByDescending(p => p.QGEquipGeneralData.MDBGeneralData.OrganizationName);
                        break;

                    case 3:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.SalesName) : query.OrderByDescending(p => p.QGEquipGeneralData.SalesName);
                        break;

                    case 4:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.QuotationNumber) : query.OrderByDescending(p => p.QGEquipGeneralData.QuotationNumber);
                        break;

                    default:
                        query = isAsc ? query.OrderBy(p => p.SOTRMID) : query.OrderByDescending(p => p.SOTRMID);
                        break;
                }
                #endregion

                ViewBag.Search = custnm;
                ViewBag.SortBy = sortBy;
                ViewBag.IsAsc = isAsc;
                if (custnm != null)
                    ViewBag.IsSearch = true;

                return View(query);
            }
            else if (User.IsInRole("ZonalManager"))
            {
                int zid = loginname.ZoneID;
                IEnumerable<SOTRM> query = (from s in db.SOTRM
                                     from b in db.ChannelPartners
                                     where s.Islatestquo != 1 && s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.QGEquipGeneralData.MDBGeneralData.OrganizationName.Contains(custnm)
                                     group s by s.QGID into g
                                            select g.FirstOrDefault()).OrderByDescending(g => g.SOTRMID).ToList();
                int qcount = query.Count();
                ViewBag.countval = qcount;

                #region Sorting
                switch (sortBy)
                {
                    case 1:
                        query = isAsc ? query.OrderBy(p => p.SOTRMID) : query.OrderByDescending(p => p.SOTRMID);
                        break;

                    case 2:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.MDBGeneralData.OrganizationName) : query.OrderByDescending(p => p.QGEquipGeneralData.MDBGeneralData.OrganizationName);
                        break;

                    case 3:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.SalesName) : query.OrderByDescending(p => p.QGEquipGeneralData.SalesName);
                        break;

                    case 4:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.QuotationNumber) : query.OrderByDescending(p => p.QGEquipGeneralData.QuotationNumber);
                        break;

                    default:
                        query = isAsc ? query.OrderBy(p => p.SOTRMID) : query.OrderByDescending(p => p.SOTRMID);
                        break;
                }
                #endregion

                ViewBag.Search = custnm;
                ViewBag.SortBy = sortBy;
                ViewBag.IsAsc = isAsc;
                if (custnm != null)
                    ViewBag.IsSearch = true;

                return View(query);
            }
            else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
            {
                IEnumerable<SOTRM> query = db.SOTRM.Where(m => m.CPID == loginname.CPID).Where(m => m.Islatestquo != 1 && m.Status != 4 && m.Status != 2 && m.Orderactive != "Project Dropped")//Code Modified by sneha 17-7-2017&& m.Status != 4 && m.Status != 2 && m.Orderactive != "Project Dropped"
                                             .Where(p => custnm == null || p.QGEquipGeneralData.MDBGeneralData.OrganizationName.Contains(custnm))
                                             .GroupBy(m => m.QGID)
                                             .Select(g => g.FirstOrDefault()).OrderByDescending(g => g.SOTRMID)
                                             .ToList();
                int qcount = query.Count();
                ViewBag.countval = qcount;

                #region Sorting
                switch (sortBy)
                {
                    case 1:
                        query = isAsc ? query.OrderBy(p => p.SOTRMID) : query.OrderByDescending(p => p.SOTRMID);
                        break;

                    case 2:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.MDBGeneralData.OrganizationName) : query.OrderByDescending(p => p.QGEquipGeneralData.MDBGeneralData.OrganizationName);
                        break;

                    case 3:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.SalesName) : query.OrderByDescending(p => p.QGEquipGeneralData.SalesName);
                        break;

                    case 4:
                        query = isAsc ? query.OrderBy(p => p.QGEquipGeneralData.QuotationNumber) : query.OrderByDescending(p => p.QGEquipGeneralData.QuotationNumber);
                        break;

                    default:
                        query = isAsc ? query.OrderBy(p => p.SOTRMID) : query.OrderByDescending(p => p.SOTRMID);
                        break;
                }
                #endregion

                ViewBag.Search = custnm;
                ViewBag.SortBy = sortBy;
                ViewBag.IsAsc = isAsc;
                if (custnm != null)
                    ViewBag.IsSearch = true;

                return View(query);
            }

            return View();

        }

        //GET: /SOTRM/
        public ActionResult Sotrmcplist(int QGid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            string prode = "Project Delayed";
            string prodr = "Project Dropped";
            var sotrm_cplist = db.SOTRM.Where(q => q.CPID == loginname.CPID).Where(m => m.BYJTChances != 100).Where(m => m.BYJTChances != 0).Where(m => m.IsStatus != 1)
                .Where(m => m.Orderactive != prode).Where(m => m.Orderactive != prodr)
                .Where(m => m.Islatestquo != 1).Where(m => m.QGID == QGid).ToList();

            int slno = 1;
            if (sotrm_cplist.Count == 0)
            {
                TempData["msg"] = "No Item Present";
                return RedirectToAction("SOTRMList", "SOTRM", null);
            }

            foreach (var sot in sotrm_cplist)
            {
                string chance = sot.BYJTChances.ToString();
                List<MyHelpers.MySelectItem> chanceselect = new List<MyHelpers.MySelectItem>();
                chanceselect = populatechanceselect(chance);
                ViewData["chances" + slno] = chanceselect.ToList();

                string compete = sot.TOPCompetitors;
                List<MyHelpers.MySelectItem> competeselect = new List<MyHelpers.MySelectItem>();
                competeselect = populatecompeteselect(compete);
                ViewData["comp" + slno] = competeselect.ToList();

                string mon = sot.Expectedorder;
                List<MyHelpers.MySelectItem> monthselect = new List<MyHelpers.MySelectItem>();
                monthselect = populatemonthselect(mon);
                ViewData["mon" + slno] = monthselect.ToList();

                string active = sot.Orderactive;
                List<MyHelpers.MySelectItem> orderactive = new List<MyHelpers.MySelectItem>();
                orderactive = populateorderactive(active);
                ViewData["active" + slno] = orderactive.ToList();
                slno++;
            }

            return View(sotrm_cplist);
        }

        //POST: /SOTRM/Model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sotrmcplist(IEnumerable<SOTRM> sotrm)
        {
            int byjt = -1;
            String month = null;
            foreach (var s in sotrm)
            {
                try
                {
                    byjt = Convert.ToInt32(s.BYJTChances.ToString());
                }
                catch
                {
                    byjt = -1;
                }

                if (byjt == 0)
                {
                    if (s.Status != 4)
                    {
                        var sotbyjt = sotrm.Where(m => m.QGID == s.QGID && m.Status != 4).Where(m => m.Equipment == s.Equipment).ToList();
                        foreach (var sotbyjtobj in sotbyjt)
                        {
                            s.Status = 2;
                            month = monthtodate(sotbyjtobj.Expectedorder);
                            sotbyjtobj.Expecteddate = Convert.ToDateTime(month);
                            sotbyjtobj.BYJTChances = byjt;
                            s.BYJTChances = byjt;
                            if (byjt > 0)
                            {
                                month = monthtodate(s.Expectedorder);
                                s.Expecteddate = Convert.ToDateTime(month);
                                s.BYJTChances = byjt;
                                s.Status = 4;
                            }
                            db.Entry(sotbyjtobj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    var ProdModelId = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelName == s.Equipment).Select(m => m.ProductModelID).SingleOrDefault();

                    //new Code Updating the QGEquipTableData
                    //Making the IsSOTStatus 2 for 0% model item
                    var quogen = db.QGEquipTableData.Where(m => m.QGID == s.QGID).Where(m => m.ProductID == s.ProductID).Where(m => m.MasterProductID == s.MasterProductID).Where(m=>m.ProductModelID==ProdModelId).ToList();
                    foreach (var qgtable in quogen)
                    {
                        qgtable.IsSOTStatus = 2;
                        db.Entry(qgtable).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //Storing in TempData table
                    SOT_Temp_tbl temp = new SOT_Temp_tbl();
                    temp.CPID = s.CPID;
                    temp.BYJTChances = byjt;
                    temp.Equipment = s.Equipment;
                    temp.QGID = s.QGID;
                    temp.TOPCompetitors = s.TOPCompetitors;
                    temp.Quantity = s.Quantity;
                    temp.Orderactive = s.Orderactive;
                    temp.Expectedorder = s.Expectedorder;
                    temp.Islatestquo = s.Islatestquo;
                    temp.Expecteddate = s.Expecteddate;
                    temp.Status = s.Status;

                    temp.ProductID = s.ProductID;
                    temp.MasterProductID = s.MasterProductID;
                    temp.ProductModelID = ProdModelId;

                    db.SOT_Temp_tbl.Add(temp);
                    db.SaveChanges();
                }

                if (byjt == 100)
                {
                    var sotbyjt = sotrm.Where(m => m.QGID == s.QGID).Where(m => m.Equipment == s.Equipment).ToList();
                    foreach (var qg in sotbyjt)
                    {
                        month = monthtodate(qg.Expectedorder);
                        qg.Expecteddate = Convert.ToDateTime(month);
                        qg.BYJTChances = byjt;
                        s.BYJTChances = byjt;
                        s.Status = 4;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var ProdModelId = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelName == s.Equipment).Select(m => m.ProductModelID).SingleOrDefault();

                    //Making the IsSOTStatus 1 for 100% model item
                    var quogen = db.QGEquipTableData.Where(m => m.QGID == s.QGID).Where(m => m.ProductID == s.ProductID).Where(m => m.MasterProductID == s.MasterProductID).Where(m => m.ProductModelID == ProdModelId).ToList();
                    foreach (var qgtable in quogen)
                    {
                        qgtable.IsSOTStatus = 1;
                        db.Entry(qgtable).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //Storing in TempData table
                    SOT_Temp_tbl temp = new SOT_Temp_tbl();
                    temp.CPID = s.CPID;
                    temp.BYJTChances = byjt;
                    temp.Equipment = s.Equipment;
                    temp.QGID = s.QGID;
                    temp.TOPCompetitors = s.TOPCompetitors;
                    temp.Quantity = s.Quantity;
                    temp.Orderactive = s.Orderactive;
                    temp.Expectedorder = s.Expectedorder;
                    temp.Islatestquo = s.Islatestquo;
                    temp.Expecteddate = s.Expecteddate;
                    temp.Status = s.Status;

                    temp.ProductID = s.ProductID;
                    temp.MasterProductID = s.MasterProductID;
                    temp.ProductModelID = ProdModelId;

                    db.SOT_Temp_tbl.Add(temp);
                    db.SaveChanges();
                }

                //For data other than 0, 100, Project Delayed and Dropped saving code
                if (s.Orderactive != "Project Dropped")
                {
                    month = monthtodate(s.Expectedorder);
                    s.Expecteddate = Convert.ToDateTime(month);
                }

                if (byjt == 0)
                {
                    s.Status = 2;
                }
                else if (byjt == 100)
                {
                    s.Status = 4;
                }
                else {
                    s.Status = 3;//other then 0 and 100%
                }
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();

                //Remove Quotation from OA View When Project Delayed == 1 & Project Dropped == 2
                if (s.Orderactive == "Project Delayed")
                {
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == s.QGID && s.Status == 4).ToList();
                    foreach (var qg in quogen)
                    {
                        qg.QuotStatus = 1;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                if (s.Orderactive == "Project Dropped")
                {
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == s.QGID && s.Status == 4).ToList();
                    foreach (var qg in quogen)
                    {
                        qg.QuotStatus = 2;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var list = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).ToList();
            int count = list.Count;
            foreach (var l in list)
            {
                if (l.BYJTChances == 0 || l.BYJTChances == 100)
                {
                    return RedirectToAction("SOTRMConversionList", "SOTRM", null);
                }
            }
            return RedirectToAction("SOTRMList");
        }

        public ActionResult SOTListAdmin(int qgid)
        {
            var sotrm_cplist = db.SOTRM
                .Where(m => m.Islatestquo != 1).Where(m => m.IsStatus!=1).Where(m => m.QGID == qgid).OrderByDescending(m => m.SOTRMID).ToList();
            return View(sotrm_cplist);
        }

        public List<MyHelpers.MySelectItem> populatechanceselect(string chance)
        {
            List<MyHelpers.MySelectItem> chanceselect = new List<MyHelpers.MySelectItem>();
            if (chance == "0")
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = true,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = false,
                });
            }
            else if (chance == "20")
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = true,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = false,
                });
            }
            else if (chance == "40")
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = true,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = false,
                });
            }
            else if (chance == "60")
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = true,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = false,
                });
            }
            else if (chance == "80")
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = true,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = false,
                });
            }
            else if (chance == "100")
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = true,
                });
            }
            else
            {
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = true,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "0% - (Order Lost)",
                    Value = "0",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "20% - (Limited Chances)",
                    Value = "20",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "40% - (Customer Confirmed Interest)",
                    Value = "40",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "60% - (Chances are Good)",
                    Value = "60",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "80% - (Chances are Convincing)",
                    Value = "80",
                    Selected = false,
                });
                chanceselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "100% - (Order Won: Written Order with Advance)",
                    Value = "100",
                    Selected = false,
                });
            }
            return chanceselect;
        }

        public List<MyHelpers.MySelectItem> populatecompeteselect(string compete)
        {
            List<MyHelpers.MySelectItem> competeselect = new List<MyHelpers.MySelectItem>();
            if (compete == "ANCOO")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "ANZAI")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "APPLE")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "ASM")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "ATS")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = true,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "DAEWON")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "DELTA")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "FASO")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "FOWLER WESTRUP")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "MARK")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "MILLTECH-PIXEL")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "ORANGE")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "OED")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "SATAKE")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "SPECTRUM")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "TAIHE-PRECISION")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "UNIQUE")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "VETAL")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "WEILAI")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete == "ZEN")
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            else if (compete != null)
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = true,
                });
            }
            else
            {
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = true,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANCOO",
                    Value = "ANCOO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ANZAI",
                    Value = "ANZAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "APPLE",
                    Value = "APPLE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ASM",
                    Value = "ASM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ATS",
                    Value = "ATS",
                    Selected = false,
                });
                //competeselect.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "BUHLER SORTEX",
                //    Value = "BUHLER SORTEX",
                //    Selected = false,
                //});
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DAEWON",
                    Value = "DAEWON",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "DELTA",
                    Value = "DELTA",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FASO",
                    Value = "FASO",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "FOWLER WESTRUP",
                    Value = "FOWLER WESTRUP",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MARK",
                    Value = "MARK",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "MILLTECH-PIXEL",
                    Value = "MILLTECH-PIXEL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ORANGE",
                    Value = "ORANGE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OED",
                    Value = "OED",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SATAKE",
                    Value = "SATAKE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "SPECTRUM",
                    Value = "SPECTRUM",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "TAIHE-PRECISION",
                    Value = "TAIHE-PRECISION",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "UNIQUE",
                    Value = "UNIQUE",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "VETAL",
                    Value = "VETAL",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "WEILAI",
                    Value = "WEILAI",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "ZEN",
                    Value = "ZEN",
                    Selected = false,
                });
                competeselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "OTHERS",
                    Value = "OTHERS",
                    Selected = false,
                });
            }
            return competeselect;
        }

        public List<MyHelpers.MySelectItem> populatemonthselect(string mon)
        {
            List<MyHelpers.MySelectItem> monthselect = new List<MyHelpers.MySelectItem>();
            if (mon == "Jan,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2016",
                    Value = "Jan,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2016",
                    Value = "Feb,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2016",
                    Value = "Mar,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2016",
                    Value = "Apr,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2016",
                    Value = "May,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2016",
                    Value = "Jun,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2016",
                    Value = "Jul,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2016",
                    Value = "Aug,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2016",
                    Value = "Sep,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2016",
                    Value = "Oct,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2016",
                    Value = "Nov,2016",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2016",
                    Value = "Dec,2016",
                    Selected = false,
                });
            }
            else if (mon == "Feb,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Mar,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Apr,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "May,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Jun,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Jul,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Aug,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Sep,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Oct,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Nov,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Dec,2014")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Jan,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Feb,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Mar,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Apr,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "May,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Jun,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Jul,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Aug,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Sep,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Oct,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Nov,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            else if (mon == "Dec,2015")
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = true,
                });
            }
            else
            {
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = true,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2014",
                    Value = "Jan,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2014",
                    Value = "Feb,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2014",
                    Value = "Mar,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2014",
                    Value = "Apr,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2014",
                    Value = "May,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2014",
                    Value = "Jun,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2014",
                    Value = "Jul,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2014",
                    Value = "Aug,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2014",
                    Value = "Sep,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2014",
                    Value = "Oct,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2014",
                    Value = "Nov,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2014",
                    Value = "Dec,2014",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jan,2015",
                    Value = "Jan,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Feb,2015",
                    Value = "Feb,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Mar,2015",
                    Value = "Mar,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Apr,2015",
                    Value = "Apr,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "May,2015",
                    Value = "May,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jun,2015",
                    Value = "Jun,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Jul,2015",
                    Value = "Jul,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Aug,2015",
                    Value = "Aug,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Sep,2015",
                    Value = "Sep,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Oct,2015",
                    Value = "Oct,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Nov,2015",
                    Value = "Nov,2015",
                    Selected = false,
                });
                monthselect.Add(new MyHelpers.MySelectItem
                {
                    Text = "Dec,2015",
                    Value = "Dec,2015",
                    Selected = false,
                });
            }
            return monthselect;
        }

        public List<MyHelpers.MySelectItem> populateorderactive(string active)
        {
            List<MyHelpers.MySelectItem> orderactive = new List<MyHelpers.MySelectItem>();
            if (active == "Budgetary")
            {
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Budgetary",
                    Value = "Budgetary",
                    Selected = true,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Technical Evaluation",
                    Value = "Technical Evaluation",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Final Negotiation",
                    Value = "Final Negotiation",
                    Selected = false,
                });
                //orderactive.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "Project Delayed",
                //    Value = "Project Delayed",
                //    Selected = false,
                //});
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Project Dropped",
                    Value = "Project Dropped",
                    Selected = false,
                });
            }
            else if (active == "Technical Evaluation")
            {
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Budgetary",
                    Value = "Budgetary",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Technical Evaluation",
                    Value = "Technical Evaluation",
                    Selected = true,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Final Negotiation",
                    Value = "Final Negotiation",
                    Selected = false,
                });
                //orderactive.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "Project Delayed",
                //    Value = "Project Delayed",
                //    Selected = false,
                //});
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Project Dropped",
                    Value = "Project Dropped",
                    Selected = false,
                });
            }
            else if (active == "Final Negotiation")
            {
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Budgetary",
                    Value = "Budgetary",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Technical Evaluation",
                    Value = "Technical Evaluation",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Final Negotiation",
                    Value = "Final Negotiation",
                    Selected = true,
                });
                //orderactive.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "Project Delayed",
                //    Value = "Project Delayed",
                //    Selected = false,
                //});
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Project Dropped",
                    Value = "Project Dropped",
                    Selected = false,
                });
            }
            //else if (active == "Project Delayed")
            //{
            //    orderactive.Add(new MyHelpers.MySelectItem
            //    {
            //        Text = "-- Select --",
            //        Value = "",
            //        Selected = false,
            //    });
            //    orderactive.Add(new MyHelpers.MySelectItem
            //    {
            //        Text = "Budgetary",
            //        Value = "Budgetary",
            //        Selected = false,
            //    });
            //    orderactive.Add(new MyHelpers.MySelectItem
            //    {
            //        Text = "Technical Evaluation",
            //        Value = "Technical Evaluation",
            //        Selected = false,
            //    });
            //    orderactive.Add(new MyHelpers.MySelectItem
            //    {
            //        Text = "Final Negotiation",
            //        Value = "Final Negotiation",
            //        Selected = false,
            //    });
            //    orderactive.Add(new MyHelpers.MySelectItem
            //    {
            //        Text = "Project Delayed",
            //        Value = "Project Delayed",
            //        Selected = true,
            //    });
            //    orderactive.Add(new MyHelpers.MySelectItem
            //    {
            //        Text = "Project Dropped",
            //        Value = "Project Dropped",
            //        Selected = false,
            //    });
            //}
            else if (active == "Project Dropped")
            {
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Budgetary",
                    Value = "Budgetary",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Technical Evaluation",
                    Value = "Technical Evaluation",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Final Negotiation",
                    Value = "Final Negotiation",
                    Selected = false,
                });
                //orderactive.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "Project Delayed",
                //    Value = "Project Delayed",
                //    Selected = false,
                //});
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Project Dropped",
                    Value = "Project Dropped",
                    Selected = true,
                });
            }
            else
            {
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "-- Select --",
                    Value = "",
                    Selected = true,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Budgetary",
                    Value = "Budgetary",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Technical Evaluation",
                    Value = "Technical Evaluation",
                    Selected = false,
                });
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Final Negotiation",
                    Value = "Final Negotiation",
                    Selected = false,
                });
                //orderactive.Add(new MyHelpers.MySelectItem
                //{
                //    Text = "Project Delayed",
                //    Value = "Project Delayed",
                //    Selected = false,
                //});
                orderactive.Add(new MyHelpers.MySelectItem
                {
                    Text = "Project Dropped",
                    Value = "Project Dropped",
                    Selected = false,
                });
            }
            return orderactive;
        }

        public string monthtodate(string month)
        {
            string mondat = null;
            string[] mon = month.Split(new char[]{' ',','});

            switch (mon[0])
            {
                case "Jan": mondat = mon[1] + "-01-01";
                    break;
                case "Feb": mondat = mon[1] + "-02-01";
                    break;
                case "Mar": mondat = mon[1] + "-03-01";
                    break;
                case "Apr": mondat = mon[1] + "-04-01";
                    break;
                case "May": mondat = mon[1] + "-05-01";
                    break;
                case "Jun": mondat = mon[1] + "-06-01";
                    break;
                case "Jul": mondat = mon[1] + "-07-01";
                    break;
                case "Aug": mondat = mon[1] + "-08-01";
                    break;
                case "Sep": mondat = mon[1] + "-09-01";
                    break;
                case "Oct": mondat = mon[1] + "-10-01";
                    break;
                case "Nov": mondat = mon[1] + "-11-01";
                    break;
                case "Dec": mondat = mon[1] + "-12-01";
                    break;
            }
            return mondat;
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        //new on 3-12-2016
        public ActionResult SOTRMConversionList()
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
            SOT_Temp_tbl sottemptable=new SOT_Temp_tbl();
            //var list = db.SOT_Temp_tbl.GroupBy(m => new {m.QGID, m.BYJTChances}).ToList();
            var list = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).Where(m => m.Islatestquo == 0).OrderByDescending(m => m.TSOTID).ToList();
            return View(list);

        }

    }
}
