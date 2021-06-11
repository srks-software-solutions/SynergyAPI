using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using SRKSSynergy.Models;
using SRKSSynergy.Helpers;
using System.Web.Security;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
//using Microsoft.Office.Interop;
using System.Drawing;
using OfficeOpenXml.Table;
using OfficeOpenXml.Drawing.Chart;
using System.Net;
using System.Text;
using System.Configuration;

namespace SRKSSynergy.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class ReportsController : Controller
    {
        DataSet ds = new DataSet();
        private SRKS_Synergy db = new SRKS_Synergy();

        //Server
        FileInfo templateFile = new FileInfo(@"H:\Synergy\Templates\Enquiry.xlsx");
        String FileDir = @"H:\Synergy\" + System.DateTime.Now.ToString("yyyy");

        ////Local
        //FileInfo templateFile = new FileInfo(@"E:\Synergy\Templates\Enquiry.xlsx");
        //String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

        ////ShivaKumar System
        //string connectionString = @"Data Source=SRKS-TECH3-PC\SRKS_TECH3;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";

        // Main Server System
        string connectionString = ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ConnectionString;
        //string connectionString = @"Data Source=ASUS\SQLEXPRESS;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=sa123;MultipleActiveResultSets=True;";
        //Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;
        // Test Server System
        //string connectionString = @"Data Source=SHWETHA\SRKSSQL;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srks4$;MultipleActiveResultSets=True;";

        //Selection of To Date & From Date

        #region

        public string monthtodate(string month)
        {
            string mondat = null;
            string[] mon = month.Split(new char[] { ' ', ',' });

            switch (mon[0])
            {
                case "Jan":
                    mondat = mon[1] + "-01-01";
                    break;
                case "Feb":
                    mondat = mon[1] + "-02-01";
                    break;
                case "Mar":
                    mondat = mon[1] + "-03-01";
                    break;
                case "Apr":
                    mondat = mon[1] + "-04-01";
                    break;
                case "May":
                    mondat = mon[1] + "-05-01";
                    break;
                case "Jun":
                    mondat = mon[1] + "-06-01";
                    break;
                case "Jul":
                    mondat = mon[1] + "-07-01";
                    break;
                case "Aug":
                    mondat = mon[1] + "-08-01";
                    break;
                case "Sep":
                    mondat = mon[1] + "-09-01";
                    break;
                case "Oct":
                    mondat = mon[1] + "-10-01";
                    break;
                case "Nov":
                    mondat = mon[1] + "-11-01";
                    break;
                case "Dec":
                    mondat = mon[1] + "-12-01";
                    break;
            }
            return mondat;
        }

        public string monthtodate2(string month)
        {
            string mondat = null;
            string[] mon = month.Split(new char[] { ' ', ',' });

            switch (mon[0])
            {
                case "Jan":
                    mondat = mon[1] + "-01-31";
                    break;
                case "Feb":
                    mondat = mon[1] + "-02-29";
                    break;
                case "Mar":
                    mondat = mon[1] + "-03-31";
                    break;
                case "Apr":
                    mondat = mon[1] + "-04-30";
                    break;
                case "May":
                    mondat = mon[1] + "-05-31";
                    break;
                case "Jun":
                    mondat = mon[1] + "-06-30";
                    break;
                case "Jul":
                    mondat = mon[1] + "-07-31";
                    break;
                case "Aug":
                    mondat = mon[1] + "-08-31";
                    break;
                case "Sep":
                    mondat = mon[1] + "-09-30";
                    break;
                case "Oct":
                    mondat = mon[1] + "-10-31";
                    break;
                case "Nov":
                    mondat = mon[1] + "-11-30";
                    break;
                case "Dec":
                    mondat = mon[1] + "-12-31";
                    break;
            }
            return mondat;
        }

        #endregion

        //Json AutoComplete Channel Partnername for Zonal Manager
        public JsonResult Autocompleteview(string term)
        {
            int zoneid = Convert.ToInt32(Session["Zoneid"]);
            var channame = db.ChannelPartners.Where(m => m.CPName == term).Select(m => m.CPID).SingleOrDefault();

            var result = (from r in db.ChannelPartners
                          where r.CPName.ToLower().Contains(term.ToLower())
                          where r.ZoneID == zoneid
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipSalesByVol(string fromdat, string todat, string macty)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            if (fromdat != null && todat != null)
            {
                DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                String frda1 = dtt.ToString("yyyy-MM-dd");
                String toda1 = dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                var start = db.EquipSalesByVol.ToList();
                foreach (var se in start)
                {
                    db.EquipSalesByVol.Remove(se);
                }
                db.SaveChanges();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    String query = "SELECT * FROM Handover WHERE Handeddate BETWEEN '" + frda1 + "' AND '" + toda1 + "' Order BY CPID ASC;";
                    var sales = db.Database.SqlQuery<Handover>(query).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;
                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7" || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }
                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = "SELECT * FROM Handover WHERE Handeddate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = " + cpids[k] + " AND modelno = '" + modelnam[y] + "';";
                            var salesqty = db.Database.SqlQuery<Handover>(queryqty).ToList();
                            int qtys = 0;
                            foreach (var sa in salesqty)
                                qtys += sa.Quantity;

                            EquipSalesByVol es = new EquipSalesByVol();
                            es.CPName = cpnam[k];
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.EquipSalesByVol.Add(es);
                            db.SaveChanges();
                        }
                    }
                }//loginname.CPID == 0 closes
                #endregion
                //Report for Zonal Manager
                #region
                else if (User.IsInRole("ZonalManager"))
                {
                    //String query = "SELECT * FROM Handover WHERE Handeddate BETWEEN '" + frda + "' AND '" + toda + "' Order BY CPID ASC;";
                    //var sales = db.Database.SqlQuery<Handover>(query).ToList();

                    var sales = (from s in db.Handover
                                 from b in db.ChannelPartners
                                 where s.Handeddate >= frda && s.Handeddate <= toda && s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                                 select s).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;
                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }
                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = "SELECT * FROM Handover WHERE Handeddate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = " + cpids[k] + " AND modelno = '" + modelnam[y] + "';";
                            var salesqty = db.Database.SqlQuery<Handover>(queryqty).ToList();
                            //var salesqty = (from s in db.Handover
                            //             from b in db.ChannelPartners
                            //                where s.Handeddate >= frda && s.Handeddate <= toda && s.CPID == cpids[k]
                            //                && s.modelno == modelnum[y] && b.ZoneID == loginname.ZoneID
                            //             select s).ToList();

                            int qtys = 0;
                            foreach (var sa in salesqty)
                                qtys += sa.Quantity;

                            EquipSalesByVol es = new EquipSalesByVol();
                            es.CPName = cpnam[k];
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.EquipSalesByVol.Add(es);
                            db.SaveChanges();
                        }
                    }
                }//loginname.CPID == 0 closes
                #endregion
                //Report for Channel Partner Admin
                #region
                else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
                {
                    String query = "SELECT * FROM Handover WHERE Handeddate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = '" + loginname.CPID + "' ";
                    var sales = db.Database.SqlQuery<Handover>(query).ToList();
                    int cpid = 0;
                    string cpnam = null;
                    int j = 0;

                    cpid = loginname.CPID;
                    cpnam = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }
                    }

                    for (int y = 0; y < j; y++)
                    {
                        String queryqty = "SELECT * FROM Handover WHERE Handeddate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = " + cpid + " AND modelno = '" + modelnam[y] + "';";
                        var salesqty = db.Database.SqlQuery<Handover>(queryqty).ToList();
                        int qtys = 0;
                        foreach (var sa in salesqty)
                            qtys += sa.Quantity;

                        EquipSalesByVol es = new EquipSalesByVol();
                        es.CPName = cpnam;
                        es.EquipModel = modelnam[y];
                        es.qty = qtys;
                        db.EquipSalesByVol.Add(es);
                        db.SaveChanges();
                    }
                }
                #endregion
                var count = (from u in db.EquipSalesByVol
                             where u.qty != 0
                             select (int?)u.qty).Sum() ?? 0;
                //var count = Convert.ToInt32(db.EquipSalesByVol.Where(m => m.qty != 0).Sum(m => m.qty));
                return RedirectToAction("EquipSalesByVolPrintReport", "Reports", new { fromdat = fromdat, todat = todat, tcount = count });
                //return RedirectToAction("EquipSalesByVolPrintReport", "Reports", new { fromdat = fromdat, todat = todat });
            }
            return View();
        }

        public ActionResult SOTByVol(string fromdat, string macty, int byjt = -1)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            #region
            List<MyHelpers.MySelectItem> monthselect = new List<MyHelpers.MySelectItem>();
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
            #endregion
            ViewData["fromdat"] = monthselect.ToList();

            if (fromdat != "" || byjt != -1)
            {
                string BYJT = null;
                if (fromdat != null || byjt != -1)
                {
                    var start = db.SOTByVol.ToList();
                    foreach (var se in start)
                    {
                        db.SOTByVol.Remove(se);
                    }
                    db.SaveChanges();
                    String query = null;


                    if (fromdat != "" && byjt == -1)
                        query = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "'  AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                    else if (fromdat != "" && byjt != -1)
                        query = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "'  AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                    else if (byjt != -1 && fromdat == "")
                        query = "SELECT * FROM SOTs WHERE Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";

                    if (byjt != -1)
                    {
                        switch (byjt)
                        {
                            case 20:
                                BYJT = "20% - (Limited Chances)";
                                break;
                            case 40:
                                BYJT = "40% - (Customer Confirmed Interest)";
                                break;
                            case 60:
                                BYJT = "60% - (Chances are Good)";
                                break;
                            case 80:
                                BYJT = "80% - (Chances are Convincing)";
                                break;
                        }
                    }

                    //Report for Buhler Admin
                    #region
                    //if (loginname.CPID == 0)
                    if (User.IsInRole("Administrator"))
                    {
                        var sales = db.Database.SqlQuery<SOT>(query).ToList();
                        int cpid = 0;
                        string[] cpnam = new string[sales.Count];
                        int[] cpids = new int[sales.Count];

                        int i = 0;
                        int j = 0;
                        foreach (var s in sales)
                        {
                            if (cpid != s.CPID)
                            {
                                cpid = s.CPID;
                                cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                                cpids[i] = cpid;
                                i++;
                            }
                        }

                        var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                        string[] modelnum = new string[models.Count];
                        string[] modelnam = new string[models.Count];

                        foreach (var mo in models)
                        {
                            if (macty == "CCD")
                            {
                                if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "CMOS")
                            {
                                if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "W")
                            {
                                if (mo.ProductModelName == "W - 7" || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "ALL")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }

                        for (int k = 0; k < i; k++)
                        {
                            for (int y = 0; y < j; y++)
                            {
                                String queryqty = null;

                                if (fromdat != "" && byjt == -1)
                                    queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "' AND CPID = " + cpids[k] + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                                else if (fromdat != "" && byjt != -1)
                                    queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "' AND CPID = " + cpids[k] + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                                else if (byjt != -1 && fromdat == "")
                                    queryqty = "SELECT * FROM SOTs WHERE CPID = " + cpids[k] + " AND Equipment = '" + modelnum[y] + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                                var salesqty = db.Database.SqlQuery<SOT>(queryqty).ToList();
                                int qtys = 0;
                                foreach (var sa in salesqty)
                                    qtys += sa.Quantity;

                                SOTByVol es = new SOTByVol();
                                es.CPName = cpnam[k];
                                es.EquipModel = modelnam[y];
                                es.qty = qtys;
                                db.SOTByVol.Add(es);
                                db.SaveChanges();
                            }
                        }

                    }// loginname.CPID == 0 closes
                    #endregion
                    //Report for Zonal Manager
                    #region
                    if (User.IsInRole("ZonalManager"))
                    {
                        int zid = loginname.ZoneID;
                        if (fromdat != "" && byjt == -1)
                            query = "SELECT * FROM SOTs s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND s.Expectedorder = '" + fromdat + "'  AND s.Islatestquo = 0 AND s.BYJTChances != 0 AND s.BYJTChances != 100 AND s.Orderactive != 'Project Delayed' AND s.Orderactive != 'Project Dropped' Order BY s.CPID ASC;"; //old working
                        //query = "SELECT * FROM SOTs s, ChannelPartners b WHERE b.ZoneID = '" + zid + "' AND Expectedorder = '" + fromdat + "'  AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY s.CPID ASC;"; //new removed CPID
                        else if (fromdat != "" && byjt != -1)
                            //query ="SELECT * FROM SOTs s, ChannelPartners b WHERE s.Expectedorder = '" + fromdat + "' AND s.BYJTChances = 40  AND s.Islatestquo = 0  AND s.Orderactive != 'Project Delayed' AND s.Orderactive != 'Project Dropped' and s.CPID=b.CPID and b.ZoneID=3 Order BY s.CPID ASC;";
                            query = "SELECT * FROM SOTs s, ChannelPartners b WHERE s.Expectedorder = '" + fromdat + "' AND s.BYJTChances = " + byjt + " AND s.Islatestquo = 0 AND s.Orderactive != 'Project Delayed' AND s.Orderactive != 'Project Dropped' AND s.CPID = b.CPID AND b.ZoneID = '" + zid + "' Order BY s.CPID ASC;"; // old
                        //query = "SELECT * FROM SOTs s, ChannelPartners b WHERE b.ZoneID = '" + zid + "' AND Expectedorder = '" + fromdat + "'  AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY s.CPID ASC;";//new removed CPID
                        else if (byjt != -1 && fromdat == "")
                            query = "SELECT * FROM SOTs s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND s.Islatestquo = 0 AND s.BYJTChances = " + byjt + " AND s.Orderactive != 'Project Delayed' AND s.Orderactive != 'Project Dropped' Order BY s.CPID ASC;";//old
                        //query = "SELECT * FROM SOTs s, ChannelPartners b WHERE b.ZoneID = '" + zid + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY s.CPID ASC;";//new removed CPID

                        var sales = db.Database.SqlQuery<SOT>(query).ToList();
                        int cpid = 0;
                        string[] cpnam = new string[sales.Count];
                        int[] cpids = new int[sales.Count];

                        int i = 0;
                        int j = 0;
                        foreach (var s in sales)
                        {
                            if (cpid != s.CPID)
                            {
                                cpid = s.CPID;
                                cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                                cpids[i] = cpid;
                                i++;
                            }
                        }

                        var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                        string[] modelnum = new string[models.Count];
                        string[] modelnam = new string[models.Count];
                        foreach (var mo in models)
                        {
                            if (macty == "CCD")
                            {
                                if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "CMOS")
                            {
                                if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "W")
                            {
                                if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "ALL")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }

                        for (int k = 0; k < i; k++)
                        {
                            for (int y = 0; y < j; y++)
                            {
                                String queryqty = null;

                                if (fromdat != "" && byjt == -1)
                                    queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "' AND CPID = " + cpids[k] + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                                else if (fromdat != "" && byjt != -1)
                                    queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "' AND CPID = " + cpids[k] + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                                else if (byjt != -1 && fromdat == "")
                                    queryqty = "SELECT * FROM SOTs WHERE CPID = " + cpids[k] + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                                var salesqty = db.Database.SqlQuery<SOT>(queryqty).ToList();
                                int qtys = 0;
                                foreach (var sa in salesqty)
                                    qtys += sa.Quantity;

                                SOTByVol es = new SOTByVol();
                                es.CPName = cpnam[k];
                                es.EquipModel = modelnam[y];
                                es.qty = qtys;
                                db.SOTByVol.Add(es);
                                db.SaveChanges();
                            }
                        }

                    }// loginname.CPID == 0 closes
                    #endregion
                    //Report Channel Partner Admin
                    #region
                    else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
                    {

                        int j = 0;
                        int cpid;
                        string cpnam = null;
                        cpid = loginname.CPID;

                        cpnam = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                        var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                        string[] modelnum = new string[models.Count];
                        string[] modelnam = new string[models.Count];

                        foreach (var mo in models)
                        {
                            if (macty == "CCD")
                            {
                                if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "CMOS")
                            {
                                if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "W")
                            {
                                if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                                {
                                    modelnum[j] = mo.ProductModelID.ToString();
                                    modelnam[j] = mo.ProductModelName;
                                    j++;
                                }
                            }
                            else if (macty == "ALL")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }

                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = null;

                            if (fromdat != "" && byjt == -1)
                                queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "' AND CPID = " + cpid + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                            else if (fromdat != "" && byjt != -1)
                                queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + fromdat + "' AND CPID = " + cpid + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                            else if (byjt != -1 && fromdat == "")
                                queryqty = "SELECT * FROM SOTs WHERE CPID = " + cpid + " AND Equipment = '" + modelnam[y] + "' AND Islatestquo = 0 AND BYJTChances = " + byjt + " AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped' Order BY CPID ASC;";
                            var salesqty = db.Database.SqlQuery<SOT>(queryqty).ToList();
                            int qtys = 0;
                            foreach (var sa in salesqty)
                                qtys += sa.Quantity;

                            SOTByVol es = new SOTByVol();
                            es.CPName = cpnam;
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.SOTByVol.Add(es);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    var count = (from u in db.SOTByVol
                                 where u.qty != 0
                                 select (int?)u.qty).Sum() ?? 0;
                    // var count = Convert.ToInt32(db.SOTByVol.Where(m => m.qty != 0).Sum(m => m.qty));
                    return RedirectToAction("SOTVolPrintReport", "Reports", new { fromdat = fromdat, byjt = BYJT, tcount = count });
                    //return RedirectToAction("SOTVolPrintReport", "Reports", new { fromdat = fromdat, byjt = BYJT });
                } //fromdat != null || byjt != -1 closes
            } // fromdat != "" || byjt != -1
            else
            {
                TempData["NoVal"] = "Please select either SOT Month or BYJT Chances to generate SOT Report";
            }
            return View();
        }

        public ActionResult LostOrderAnalysisReport(string fromdat, string todat, string CPID)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
            string cpname = null;

            if (fromdat != null && todat != null)
            {

                DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                String frda1 = dtt.ToString("yyyy-MM-dd");
                String toda1 = dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                var start6 = db.LOA.ToList();
                var loasum = db.LostOrderAnalysis.ToList();
                foreach (var se in start6)
                {
                    db.LOA.Remove(se);
                }
                db.SaveChanges();

                var start1 = db.LOACCD240.ToList();
                foreach (var se in start1)
                {
                    db.LOACCD240.Remove(se);
                }
                db.SaveChanges();

                var start2 = db.LOACCD320.ToList();
                foreach (var se in start2)
                {
                    db.LOACCD320.Remove(se);
                }
                db.SaveChanges();

                var start3 = db.LOACMOS160.ToList();
                foreach (var se in start3)
                {
                    db.LOACMOS160.Remove(se);
                }
                db.SaveChanges();

                var start4 = db.LOACMOS240.ToList();
                foreach (var se in start4)
                {
                    db.LOACMOS240.Remove(se);
                }
                db.SaveChanges();

                var start5 = db.LOACMOS320.ToList();
                foreach (var se in start5)
                {
                    db.LOACMOS320.Remove(se);
                }
                db.SaveChanges();

                var start9 = db.LOASum.ToList();
                foreach (var se in start9)
                {
                    db.LOASum.Remove(se);
                }
                db.SaveChanges();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    //ViewBag.Buhler = true;
                    string[] cpnam = null;
                    String[] cpids = null;
                    int i = 0;
                    int j = 0;


                    if (CPID != null && CPID != "")
                    {
                        int cpid = Convert.ToInt32(CPID);
                        cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                        String query = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + CPID + " and LOADate BETWEEN '" + frda1 + "' AND '" + toda1 + "';";
                        var sales = db.Database.SqlQuery<LOABarReport>(query).ToList();
                        String eqmod = null;
                        //string[] cpnam = new string[sales.Count];
                        //String[] cpids = new string[sales.Count];
                        cpnam = new string[sales.Count];
                        cpids = new string[sales.Count];

                        foreach (var s in sales)
                        {
                            if (eqmod != s.ReasonForLosing)
                            {
                                eqmod = s.ReasonForLosing;
                                cpnam[i] = s.ReasonForLosing;
                                cpids[i] = eqmod;
                                i++;
                            }
                        }
                    }
                    else
                    {
                        String query = "SELECT * FROM LostOrderAnalysis WHERE LOADate BETWEEN '" + frda1 + "' AND '" + toda1 + "';";
                        var sales = db.Database.SqlQuery<LOABarReport>(query).ToList();
                        String eqmod = null;
                        cpnam = new string[sales.Count];
                        cpids = new string[sales.Count];

                        foreach (var s in sales)
                        {
                            if (eqmod != s.ReasonForLosing)
                            {
                                eqmod = s.ReasonForLosing;
                                cpnam[i] = s.ReasonForLosing;
                                cpids[i] = eqmod;
                                i++;
                            }
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count]; ;
                    foreach (var mo in models)
                    {
                        modelnum[j] = mo.ProductModelName;
                        j++;
                    }

                    int qtys = 0;

                    for (int k = 0; k < j; k++)
                    {
                        for (int y = 0; y < i; y++)
                        {
                            if (CPID != null && CPID != "")
                            {
                                String queryqty = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + CPID + " AND LOADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND ReasonForLosing = '" + cpids[y] + "' AND EquipModel = '" + modelnum[k] + "';";
                                var salesqty = db.Database.SqlQuery<LOABarReport>(queryqty).ToList();
                                qtys = salesqty.Count;
                            }
                            else
                            {
                                String queryqty = "SELECT * FROM LostOrderAnalysis WHERE LOADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND ReasonForLosing = '" + cpids[y] + "' AND EquipModel = '" + modelnum[k] + "';";
                                var salesqty = db.Database.SqlQuery<LOABarReport>(queryqty).ToList();
                                qtys = salesqty.Count;
                            }

                            if (modelnum[k] == "CCD 240")
                            {
                                LOACCD240 es = new LOACCD240();
                                es.ReasonForLosing = cpnam[y];
                                es.TotalNumbers = qtys;
                                db.LOACCD240.Add(es);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CCD 240") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CCD 240";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CCD 320")
                            {
                                LOACCD320 es1 = new LOACCD320();
                                es1.ReasonForLosing = cpnam[y];
                                es1.TotalNumbers = qtys;
                                db.LOACCD320.Add(es1);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CCD 320") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CCD 320";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 160")
                            {
                                LOACMOS160 es2 = new LOACMOS160();
                                es2.ReasonForLosing = cpnam[y];
                                es2.TotalNumbers = qtys;
                                db.LOACMOS160.Add(es2);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 160") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 160";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 240 ")
                            {
                                LOACMOS240 es3 = new LOACMOS240();
                                es3.ReasonForLosing = cpnam[y];
                                es3.TotalNumbers = qtys;
                                db.LOACMOS240.Add(es3);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 240") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 240";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 320")
                            {
                                LOACMOS320 es4 = new LOACMOS320();
                                es4.ReasonForLosing = cpnam[y];
                                es4.TotalNumbers = qtys;
                                db.LOACMOS320.Add(es4);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 320") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 320";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            LOA es5 = new LOA();
                            es5.ReasonForLosing = cpnam[y];
                            es5.EquipModel = modelnum[k];
                            es5.TotalNumbers = qtys;
                            db.LOA.Add(es5);
                            db.SaveChanges();

                            ////adding quantity to lost order analysis table
                            //int cpid = Convert.ToInt32(CPID);
                            //cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                            //String query = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + CPID + " and LOADate BETWEEN '" + frda1 + "' AND '" + toda1 + "';";
                            //var sales = db.Database.SqlQuery<LOABarReport>(query).ToList();
                            //foreach (var s in sales)
                            //{
                            //    s.qty = Convert.ToString(qtys);
                            //    var entry1 = db.Entry<LOABarReport>(s);

                            //    // Retreive the Id through reflection
                            //    int pkey1 = s.LOID;
                            //    if (entry1.State == EntityState.Detached)
                            //    {
                            //        var set = db.Set<LOABarReport>();
                            //        LOABarReport attachedEntity = set.Find(pkey1);  // access the key
                            //        if (attachedEntity != null)
                            //        {
                            //            var attachedEntry = db.Entry(attachedEntity);
                            //            attachedEntry.CurrentValues.SetValues(s);
                            //        }
                            //        else
                            //        {
                            //            entry1.State = EntityState.Modified; // attach the entity
                            //        }
                            //    }

                            //    this.db.SaveChanges();
                            //}

                        }
                    }
                } //loginname.CPID == 0 closes
                #endregion
                //Report for Zonal Manager
                #region
                if (User.IsInRole("ZonalManager"))
                {
                    //ViewBag.Buhler = true;
                    string[] cpnam = null;
                    String[] cpids = null;
                    int i = 0;
                    int j = 0;


                    if (CPID != null && CPID != "")
                    {
                        int cpid = Convert.ToInt32(CPID);
                        cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                        String query = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + CPID + " and LOADate BETWEEN '" + frda + "' AND '" + toda + "';";
                        var sales = db.Database.SqlQuery<LOABarReport>(query).ToList();
                        String eqmod = null;
                        //string[] cpnam = new string[sales.Count];
                        //String[] cpids = new string[sales.Count];
                        cpnam = new string[sales.Count];
                        cpids = new string[sales.Count];

                        foreach (var s in sales)
                        {
                            if (eqmod != s.ReasonForLosing)
                            {
                                eqmod = s.ReasonForLosing;
                                cpnam[i] = s.ReasonForLosing;
                                cpids[i] = eqmod;
                                i++;
                            }
                        }
                    }
                    else
                    {
                        int zid = loginname.ZoneID;
                        String query = "SELECT * FROM LostOrderAnalysis as s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND s.LOADate BETWEEN '" + frda + "' AND '" + toda + "';";
                        var sales = db.Database.SqlQuery<LOABarReport>(query).ToList();
                        String eqmod = null;
                        cpnam = new string[sales.Count];
                        cpids = new string[sales.Count];

                        foreach (var s in sales)
                        {
                            if (eqmod != s.ReasonForLosing)
                            {
                                eqmod = s.ReasonForLosing;
                                cpnam[i] = s.ReasonForLosing;
                                cpids[i] = eqmod;
                                i++;
                            }
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count]; ;
                    foreach (var mo in models)
                    {
                        modelnum[j] = mo.ProductModelName;
                        j++;
                    }

                    int qtys = 0;

                    for (int k = 0; k < j; k++)
                    {
                        for (int y = 0; y < i; y++)
                        {
                            if (CPID != null && CPID != "")
                            {
                                String queryqty = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + CPID + " AND LOADate BETWEEN '" + frda + "' AND '" + toda + "' AND ReasonForLosing = '" + cpids[y] + "' AND EquipModel = '" + modelnum[k] + "';";
                                var salesqty = db.Database.SqlQuery<LOABarReport>(queryqty).ToList();
                                qtys = salesqty.Count;
                            }
                            else
                            {
                                String queryqty = "SELECT * FROM LostOrderAnalysis WHERE LOADate BETWEEN '" + frda + "' AND '" + toda + "' AND ReasonForLosing = '" + cpids[y] + "' AND EquipModel = '" + modelnum[k] + "';";
                                var salesqty = db.Database.SqlQuery<LOABarReport>(queryqty).ToList();
                                qtys = salesqty.Count;

                            }


                            if (modelnum[k] == "CCD 240")
                            {
                                LOACCD240 es = new LOACCD240();
                                es.ReasonForLosing = cpnam[y];
                                es.TotalNumbers = qtys;
                                db.LOACCD240.Add(es);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CCD 240") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CCD 240";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CCD 320")
                            {
                                LOACCD320 es1 = new LOACCD320();
                                es1.ReasonForLosing = cpnam[y];
                                es1.TotalNumbers = qtys;
                                db.LOACCD320.Add(es1);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CCD 320") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CCD 320";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 160")
                            {
                                LOACMOS160 es2 = new LOACMOS160();
                                es2.ReasonForLosing = cpnam[y];
                                es2.TotalNumbers = qtys;
                                db.LOACMOS160.Add(es2);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 160") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 160";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 240")
                            {
                                LOACMOS240 es3 = new LOACMOS240();
                                es3.ReasonForLosing = cpnam[y];
                                es3.TotalNumbers = qtys;
                                db.LOACMOS240.Add(es3);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 240") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 240";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 320")
                            {
                                LOACMOS320 es4 = new LOACMOS320();
                                es4.ReasonForLosing = cpnam[y];
                                es4.TotalNumbers = qtys;
                                db.LOACMOS320.Add(es4);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 320") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 320";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            LOA es5 = new LOA();
                            es5.ReasonForLosing = cpnam[y];
                            es5.EquipModel = modelnum[k];
                            es5.TotalNumbers = qtys;
                            db.LOA.Add(es5);
                            db.SaveChanges();

                        }
                    }
                } //loginname.CPID == 0 closes
                #endregion
                //Report for Channel Partner Admin
                #region
                else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
                {
                    string[] cpnam = null;
                    String[] cpids = null;
                    int i = 0;
                    int j = 0;
                    //My code
                    //int j = 0;
                    int chancpid;
                    chancpid = loginname.CPID;

                    cpname = db.ChannelPartners.Where(m => m.CPID == chancpid).Select(m => m.CPName).Single();

                    //if (CPID != null && CPID != "")
                    //{
                    //int cpid = Convert.ToInt32(CPID);
                    //cpname = db.ChannelPartners.Where(m => m.CPID == chancpid).Select(m => m.CPName).Single();

                    String query = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + chancpid + " and LOADate BETWEEN '" + frda + "' AND '" + toda + "';";
                    var sales = db.Database.SqlQuery<LOABarReport>(query).ToList();
                    String eqmod = null;
                    cpnam = new string[sales.Count];
                    cpids = new string[sales.Count];
                    //cpnam = new string[sales.Count];
                    //cpids = new string[sales.Count];

                    foreach (var s in sales)
                    {
                        if (eqmod != s.ReasonForLosing)
                        {
                            eqmod = s.ReasonForLosing;
                            cpnam[i] = s.ReasonForLosing;
                            cpids[i] = eqmod;
                            i++;
                        }
                    }
                    //}
                    //else
                    //{
                    //    String query = "SELECT * FROM LostOrderAnalysis WHERE LOADate BETWEEN '" + frda + "' AND '" + toda + "';";
                    //    var sales = db.Database.SqlQuery<LostOrderAnalysis>(query).ToList();
                    //    String eqmod = null;
                    //    cpnam = new string[sales.Count];
                    //    cpids = new string[sales.Count];

                    //    foreach (var s in sales)
                    //    {
                    //        if (eqmod != s.ReasonForLosing)
                    //        {
                    //            eqmod = s.ReasonForLosing;
                    //            cpnam[i] = s.ReasonForLosing;
                    //            cpids[i] = eqmod;
                    //            i++;
                    //        }
                    //    }
                    //}

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count]; ;
                    foreach (var mo in models)
                    {
                        modelnum[j] = mo.ProductModelName;
                        j++;
                    }

                    int qtys = 0;

                    for (int k = 0; k < j; k++)
                    {
                        for (int y = 0; y < i; y++)
                        {
                            //if (CPID != null && CPID != "")
                            //{
                            String queryqty = "SELECT * FROM LostOrderAnalysis WHERE CPID = " + chancpid + " AND LOADate BETWEEN '" + frda + "' AND '" + toda + "' AND ReasonForLosing = '" + cpids[y] + "' AND EquipModel = '" + modelnum[k] + "';";
                            var salesqty = db.Database.SqlQuery<LOABarReport>(queryqty).ToList();
                            qtys = salesqty.Count;
                            //}
                            //else
                            //{
                            //    String queryqty = "SELECT * FROM LostOrderAnalysis WHERE LOADate BETWEEN '" + frda + "' AND '" + toda + "' AND ReasonForLosing = '" + cpids[y] + "' AND EquipModel = '" + modelnum[k] + "';";
                            //    var salesqty = db.Database.SqlQuery<LostOrderAnalysis>(queryqty).ToList();
                            //    qtys = salesqty.Count;
                            //}


                            if (modelnum[k] == "CCD 240")
                            {
                                LOACCD240 es = new LOACCD240();
                                es.ReasonForLosing = cpnam[y];
                                es.TotalNumbers = qtys;
                                db.LOACCD240.Add(es);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CCD 240") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CCD 240";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CCD 320")
                            {
                                LOACCD320 es1 = new LOACCD320();
                                es1.ReasonForLosing = cpnam[y];
                                es1.TotalNumbers = qtys;
                                db.LOACCD320.Add(es1);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CCD 320") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CCD 320";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 160")
                            {
                                LOACMOS160 es2 = new LOACMOS160();
                                es2.ReasonForLosing = cpnam[y];
                                es2.TotalNumbers = qtys;
                                db.LOACMOS160.Add(es2);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 160") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 160";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 240")
                            {
                                LOACMOS240 es3 = new LOACMOS240();
                                es3.ReasonForLosing = cpnam[y];
                                es3.TotalNumbers = qtys;
                                db.LOACMOS240.Add(es3);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 240") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 240";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            else if (modelnum[k] == "CMOS 320")
                            {
                                LOACMOS320 es4 = new LOACMOS320();
                                es4.ReasonForLosing = cpnam[y];
                                es4.TotalNumbers = qtys;
                                db.LOACMOS320.Add(es4);
                                db.SaveChanges();

                                double sum = 0;
                                foreach (var lg in loasum)
                                {
                                    if ((lg.EquipModel == "CMOS 320") && (lg.ReasonForLosing == cpnam[y]))
                                    {
                                        sum = sum + Convert.ToDouble(lg.qty);
                                    }
                                }
                                LOASum ls = new LOASum();
                                ls.ReasonForLosing = cpnam[y];
                                ls.EquipModel = "CMOS 320";
                                ls.TotalNumbers = Convert.ToInt32(sum);
                                db.LOASum.Add(ls);
                                db.SaveChanges();
                            }
                            LOA es5 = new LOA();
                            es5.ReasonForLosing = cpnam[y];
                            es5.EquipModel = modelnum[k];
                            es5.TotalNumbers = qtys;
                            db.LOA.Add(es5);
                            db.SaveChanges();
                        }
                    }
                }
                #endregion
                var count = (from u in db.LOA
                             where u.TotalNumbers != 0
                             select (int?)u.TotalNumbers).Sum() ?? 0;
                //var count = Convert.ToInt32(db.LOA.Where(m => m.TotalNumbers != 0).Sum(m => m.TotalNumbers));
                return RedirectToAction("LOAPrintReport", "Reports", new { fromdat = fromdat, todat = todat, cpname = cpname, tcount = count });
                //return RedirectToAction("LOAPrintReport", "Reports", new { fromdat = fromdat, todat = todat, cpname = cpname });
            }
            var logcpid = Session["logincpid"];
            int zoind = Convert.ToInt32(Session["Zoneid"]);
            int cpidint = Convert.ToInt32(logcpid);
            if (User.IsInRole("ZonalManager"))
            {
                ViewBag.CPID = new SelectList(db.ChannelPartners.Where(m => m.ZoneID == zoind), "CPID", "CPName");
            }
            else if (User.IsInRole("Administrator"))
            {
                ViewBag.CPID = new SelectList(db.ChannelPartners, "CPID", "CPName");
            }
            else
            {
                ViewBag.CPID = new SelectList(db.ChannelPartners.Where(m => m.CPID == cpidint), "CPID", "CPName");
            }
            return View();
        }

        public ActionResult CPPerformanceReview(String FromDate, String ToDate, String CPName)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
            int zid = loginname.ZoneID;

            // For Specified Channel Partners
            #region
            if (FromDate != null && ToDate != null && CPName != null && CPName != "")
            {
                var count = db.ChannelPartners.Where(m => m.CPName == CPName).Count();
                if (count != 0)
                {
                    var cpid = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).First();
                    DateTime dtt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt1 = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    String frda1 = dtt.ToString("yyyy-MM-dd");
                    String toda1 = dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;

                    //Equipment sales by value and by volume
                    var start = db.EquipSalesByVal.ToList();
                    foreach (var se in start)
                    {
                        db.EquipSalesByVal.Remove(se);
                        db.SaveChanges();
                    }

                    int j = 0;

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count]; ;
                    foreach (var mo in models)
                    {
                        modelnum[j] = mo.ProductModelName;
                        j++;
                    }

                    int totalsales = 0;
                    for (int y = 0; y < j; y++)
                    {

                        String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ApprovalStatus = 1 AND CPID = " + cpid + ";";
                        var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                        int qtys = 0;
                        int value = 0;

                        foreach (var sa in salesqty)
                        {
                            string mdno = modelnum[y];
                            int modid = db.ProductModel.Where(m => m.ProductModelName == mdno).Select(m => m.ProductModelID).First();
                            int oaid = sa.OAID;
                            String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaid + " and ProductModelID = " + modid + ";";
                            var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();
                            foreach (var qt in salesqty1)
                            {

                                String unitval = db.OAEquipTableData.Where(m => m.OAID == oaid).Where(m => m.ProductModelID == modid).Select(m => m.UnitPrice).First();
                                string[] vals = unitval.Split(',');
                                string unitvalue = null;
                                foreach (var uv in vals)
                                {
                                    unitvalue += uv;
                                }

                                qtys += qt.Quantity;
                                int val = Convert.ToInt32(unitvalue) * qt.Quantity;
                                value += val;
                            }

                        }
                        totalsales += value;
                        EquipSalesByVal es = new EquipSalesByVal();
                        es.EquipModel = modelnum[y];
                        es.value = value;
                        es.valuecur = value.ToString("#,#", CultureInfo.InvariantCulture);
                        es.qty = qtys;
                        db.EquipSalesByVal.Add(es);
                        db.SaveChanges();
                    }

                    //SOT for next three months
                    DateTime todat = Convert.ToDateTime(toda);
                    int month = todat.Month;
                    int year = todat.Year;
                    string[] nextmonths = new string[3];
                    switch (month)
                    {
                        case 1:
                            nextmonths[0] = "Feb," + year;
                            nextmonths[1] = "Mar," + year;
                            nextmonths[2] = "Apr," + year;
                            break;
                        case 2:
                            nextmonths[0] = "Mar," + year;
                            nextmonths[1] = "Apr," + year;
                            nextmonths[2] = "May," + year;
                            break;
                        case 3:
                            nextmonths[0] = "Apr," + year;
                            nextmonths[1] = "May," + year;
                            nextmonths[2] = "Jun," + year;
                            break;
                        case 4:
                            nextmonths[0] = "May," + year;
                            nextmonths[1] = "Jun," + year;
                            nextmonths[2] = "Jul," + year;
                            break;
                        case 5:
                            nextmonths[0] = "Jun," + year;
                            nextmonths[1] = "Jul," + year;
                            nextmonths[2] = "Aug," + year;
                            break;
                        case 6:
                            nextmonths[0] = "Jul," + year;
                            nextmonths[1] = "Aug," + year;
                            nextmonths[2] = "Sep," + year;
                            break;
                        case 7:
                            nextmonths[0] = "Aug," + year;
                            nextmonths[1] = "Sep," + year;
                            nextmonths[2] = "Oct," + year;
                            break;
                        case 8:
                            nextmonths[0] = "Sep," + year;
                            nextmonths[1] = "Oct," + year;
                            nextmonths[2] = "Nov," + year;
                            break;
                        case 9:
                            nextmonths[0] = "Oct," + year;
                            nextmonths[1] = "Nov," + year;
                            nextmonths[2] = "Dec," + year;
                            break;
                        case 10:
                            nextmonths[0] = "Nov," + year;
                            nextmonths[1] = "Dec," + year;
                            nextmonths[2] = "Jan," + (year + 1).ToString();
                            break;
                        case 11:
                            nextmonths[0] = "Dec," + year;
                            nextmonths[1] = "Jan," + (year + 1).ToString();
                            nextmonths[2] = "Feb," + (year + 1).ToString();
                            break;
                        case 12:
                            nextmonths[0] = "Jan," + (year + 1).ToString();
                            nextmonths[1] = "Feb," + (year + 1).ToString();
                            nextmonths[2] = "Mar," + (year + 1).ToString();
                            break;
                    }

                    var start1 = db.SOTByVolCP.ToList();
                    foreach (var se in start1)
                    {
                        db.SOTByVolCP.Remove(se);
                    }
                    db.SaveChanges();

                    for (int y = 0; y < j; y++)
                    {
                        foreach (var dat in nextmonths)
                        {
                            String queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + dat + "' AND CPID = " + cpid + " AND Equipment = '" + modelnum[y] + "' AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped';";
                            var salesqty = db.Database.SqlQuery<SOT>(queryqty).ToList();
                            int qtys = 0;
                            foreach (var sa in salesqty)
                                qtys += sa.Quantity;

                            SOTByVolCP es = new SOTByVolCP();
                            es.EquipModel = modelnum[y];
                            es.qty = qtys;
                            es.Month = dat;
                            db.SOTByVolCP.Add(es);
                            db.SaveChanges();
                        }
                    }

                    //CP Performance Calculation And updation
                    var start2 = db.CPPerformance.ToList();
                    foreach (var se in start2)
                        db.CPPerformance.Remove(se);

                    int losts = 0, delayeds = 0, droppeds = 0, inquotes = 0;
                    decimal percent = 0;

                    inquotes = db.CpperExistquot(cpid, frda, toda);

                    losts = db.CpperOrderlost(cpid, frda, toda);

                    delayeds = db.CpperProdeldrop(cpid, frda, toda, "Project Delayed");

                    droppeds = db.CpperProdeldrop(cpid, frda, toda, "Project Dropped");

                    var denominator = Convert.ToDecimal(totalsales + inquotes + losts + delayeds + droppeds);

                    if (denominator == 0)
                    {
                        percent = 0;
                    }
                    else
                    {
                        percent = Math.Round(Convert.ToDecimal(((Convert.ToDecimal(totalsales) / Convert.ToDecimal(totalsales + inquotes + losts + delayeds + droppeds))) * 100), 2);
                    }

                    CPPerformance CPP = new CPPerformance();
                    CPP.OnGoingProjectsint = inquotes;
                    CPP.OnGoingProjects = inquotes.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.OrdersLostint = losts;
                    CPP.OrdersLost = losts.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.OrdersWonint = totalsales;
                    CPP.OrdersWon = totalsales.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.ProjectDelayedint = delayeds;
                    CPP.ProjectDelayed = delayeds.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.ProjectDroppedint = droppeds;
                    CPP.ProjectDropped = droppeds.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.performancepercent = percent + " %";
                    CPP.totalvalue = (inquotes + losts + totalsales + delayeds + droppeds);
                    CPP.totalvalueCur = (inquotes + losts + totalsales + delayeds + droppeds).ToString("#,#", CultureInfo.InvariantCulture);
                    db.CPPerformance.Add(CPP);
                    db.SaveChanges();

                    return RedirectToAction("CPPerformancePrintReport", "Reports", new { fromdat = FromDate, todat = ToDate, CPName = CPName });
                }
                TempData["WrongChannelPartner"] = "Please Enter Valid Channel Partner Name";
            }
            #endregion
            //For All Channel Partners
            #region
            else if (FromDate != null && ToDate != null && CPName == null || CPName == "" && User.IsInRole("Administrator"))
            {
                //var count = db.ChannelPartners.Where(m => m.CPName == CPName).Count();
                //if (count != 0)
                {
                    //var cpid = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).First();
                    DateTime dtt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt1 = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    String frda1 = dtt.ToString("yyyy-MM-dd");
                    String toda1 = dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;

                    //Equipment sales by value and by volume
                    var start = db.EquipSalesByVal.ToList();
                    foreach (var se in start)
                    {
                        db.EquipSalesByVal.Remove(se);
                        db.SaveChanges();
                    }

                    int j = 0;

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count]; ;
                    foreach (var mo in models)
                    {
                        modelnum[j] = mo.ProductModelName;
                        j++;
                    }

                    int totalsales = 0;
                    for (int y = 0; y < j; y++)
                    {

                        String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ApprovalStatus = 1;";
                        var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                        int qtys = 0;
                        int value = 0;

                        foreach (var sa in salesqty)
                        {
                            string mdno = modelnum[y];
                            int modid = db.ProductModel.Where(m => m.ProductModelName == mdno).Select(m => m.ProductModelID).First();
                            int oaid = sa.OAID;
                            String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaid + " and ProductModelID = " + modid + ";";
                            var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();
                            foreach (var qt in salesqty1)
                            {

                                String unitval = db.OAEquipTableData.Where(m => m.OAID == oaid).Where(m => m.ProductModelID == modid).Select(m => m.UnitPrice).First();
                                string[] vals = unitval.Split(',');
                                string unitvalue = null;
                                foreach (var uv in vals)
                                {
                                    unitvalue += uv;
                                }

                                qtys += qt.Quantity;
                                int val = Convert.ToInt32(unitvalue) * qt.Quantity;
                                value += val;
                            }

                        }
                        totalsales += value;
                        EquipSalesByVal es = new EquipSalesByVal();
                        es.EquipModel = modelnum[y];
                        es.value = value;
                        es.valuecur = value.ToString("#,#", CultureInfo.InvariantCulture);
                        es.qty = qtys;
                        db.EquipSalesByVal.Add(es);
                        db.SaveChanges();
                    }

                    //SOT for next three months
                    DateTime todat = Convert.ToDateTime(toda);
                    int month = todat.Month;
                    int year = todat.Year;
                    string[] nextmonths = new string[3];
                    switch (month)
                    {
                        case 1:
                            nextmonths[0] = "Feb," + year;
                            nextmonths[1] = "Mar," + year;
                            nextmonths[2] = "Apr," + year;
                            break;
                        case 2:
                            nextmonths[0] = "Mar," + year;
                            nextmonths[1] = "Apr," + year;
                            nextmonths[2] = "May," + year;
                            break;
                        case 3:
                            nextmonths[0] = "Apr," + year;
                            nextmonths[1] = "May," + year;
                            nextmonths[2] = "Jun," + year;
                            break;
                        case 4:
                            nextmonths[0] = "May," + year;
                            nextmonths[1] = "Jun," + year;
                            nextmonths[2] = "Jul," + year;
                            break;
                        case 5:
                            nextmonths[0] = "Jun," + year;
                            nextmonths[1] = "Jul," + year;
                            nextmonths[2] = "Aug," + year;
                            break;
                        case 6:
                            nextmonths[0] = "Jul," + year;
                            nextmonths[1] = "Aug," + year;
                            nextmonths[2] = "Sep," + year;
                            break;
                        case 7:
                            nextmonths[0] = "Aug," + year;
                            nextmonths[1] = "Sep," + year;
                            nextmonths[2] = "Oct," + year;
                            break;
                        case 8:
                            nextmonths[0] = "Sep," + year;
                            nextmonths[1] = "Oct," + year;
                            nextmonths[2] = "Nov," + year;
                            break;
                        case 9:
                            nextmonths[0] = "Oct," + year;
                            nextmonths[1] = "Nov," + year;
                            nextmonths[2] = "Dec," + year;
                            break;
                        case 10:
                            nextmonths[0] = "Nov," + year;
                            nextmonths[1] = "Dec," + year;
                            nextmonths[2] = "Jan," + (year + 1).ToString();
                            break;
                        case 11:
                            nextmonths[0] = "Dec," + year;
                            nextmonths[1] = "Jan," + (year + 1).ToString();
                            nextmonths[2] = "Feb," + (year + 1).ToString();
                            break;
                        case 12:
                            nextmonths[0] = "Jan," + (year + 1).ToString();
                            nextmonths[1] = "Feb," + (year + 1).ToString();
                            nextmonths[2] = "Mar," + (year + 1).ToString();
                            break;
                    }

                    var start1 = db.SOTByVolCP.ToList();
                    foreach (var se in start1)
                    {
                        db.SOTByVolCP.Remove(se);
                    }
                    db.SaveChanges();

                    for (int y = 0; y < j; y++)
                    {
                        foreach (var dat in nextmonths)
                        {
                            String queryqty = "SELECT * FROM SOTs WHERE Expectedorder = '" + dat + "' AND Equipment = '" + modelnum[y] + "' AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped';";
                            var salesqty = db.Database.SqlQuery<SOT>(queryqty).ToList();
                            int qtys = 0;
                            foreach (var sa in salesqty)
                                qtys += sa.Quantity;

                            SOTByVolCP es = new SOTByVolCP();
                            es.EquipModel = modelnum[y];
                            es.qty = qtys;
                            es.Month = dat;
                            db.SOTByVolCP.Add(es);
                            db.SaveChanges();
                        }
                    }

                    //CP Performance Calculation And updation
                    var start2 = db.CPPerformance.ToList();
                    foreach (var se in start2)
                        db.CPPerformance.Remove(se);

                    int losts = 0, delayeds = 0, droppeds = 0, inquotes = 0;
                    decimal percent = 0;

                    inquotes = db.CpperExistquotall(frda, toda);

                    losts = db.CpperOrderlostall(frda, toda);

                    delayeds = db.CpperProdeldropall(frda, toda, "Project Delayed");

                    droppeds = db.CpperProdeldropall(frda, toda, "Project Dropped");

                    var denominator = Convert.ToDecimal(totalsales + inquotes + losts + delayeds + droppeds);

                    if (denominator == 0)
                    {
                        percent = 0;
                    }
                    else
                    {
                        percent = Math.Round(Convert.ToDecimal(((Convert.ToDecimal(totalsales) / Convert.ToDecimal(totalsales + inquotes + losts + delayeds + droppeds))) * 100), 2);
                    }

                    CPPerformance CPP = new CPPerformance();
                    CPP.OnGoingProjectsint = inquotes;
                    CPP.OnGoingProjects = inquotes.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.OrdersLostint = losts;
                    CPP.OrdersLost = losts.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.OrdersWonint = totalsales;
                    CPP.OrdersWon = totalsales.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.ProjectDelayedint = delayeds;
                    CPP.ProjectDelayed = delayeds.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.ProjectDroppedint = droppeds;
                    CPP.ProjectDropped = droppeds.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.performancepercent = percent + " %";
                    CPP.totalvalue = (inquotes + losts + totalsales + delayeds + droppeds);
                    CPP.totalvalueCur = (inquotes + losts + totalsales + delayeds + droppeds).ToString("#,#", CultureInfo.InvariantCulture);
                    db.CPPerformance.Add(CPP);
                    db.SaveChanges();

                    return RedirectToAction("CPPerformancePrintReport", "Reports", new { fromdat = FromDate, todat = ToDate, CPName = CPName });
                }
                TempData["WrongChannelPartner"] = "Please Enter Valid Channel Partner Name";
            }
            #endregion
            //For All Channel Partners
            #region
            else if (FromDate != null && ToDate != null && CPName == null || CPName == "" && User.IsInRole("ZonalManager"))
            {
                //var count = db.ChannelPartners.Where(m => m.CPName == CPName).Count();
                //if (count != 0)
                {
                    //var cpid = db.ChannelPartners.Where(m => m.CPName == CPName).Select(m => m.CPID).First();
                    DateTime dtt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtt1 = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    String frda1 = dtt.ToString("yyyy-MM-dd");
                    String toda1 = dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;

                    //Equipment sales by value and by volume
                    var start = db.EquipSalesByVal.ToList();
                    foreach (var se in start)
                    {
                        db.EquipSalesByVal.Remove(se);
                        db.SaveChanges();
                    }

                    int j = 0;

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count]; ;
                    foreach (var mo in models)
                    {
                        modelnum[j] = mo.ProductModelName;
                        j++;
                    }

                    int totalsales = 0;
                    for (int y = 0; y < j; y++)
                    {

                        String queryqty = "SELECT * FROM OAEquipGeneralData s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ApprovalStatus = 1;";
                        var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                        int qtys = 0;
                        int value = 0;

                        foreach (var sa in salesqty)
                        {
                            string mdno = modelnum[y];
                            int modid = db.ProductModel.Where(m => m.ProductModelName == mdno).Select(m => m.ProductModelID).First();
                            int oaid = sa.OAID;
                            String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaid + " and ProductModelID = " + modid + ";";
                            var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();
                            foreach (var qt in salesqty1)
                            {

                                String unitval = db.OAEquipTableData.Where(m => m.OAID == oaid).Where(m => m.ProductModelID == modid).Select(m => m.UnitPrice).First();
                                string[] vals = unitval.Split(',');
                                string unitvalue = null;
                                foreach (var uv in vals)
                                {
                                    unitvalue += uv;
                                }

                                qtys += qt.Quantity;
                                int val = Convert.ToInt32(unitvalue) * qt.Quantity;
                                value += val;
                            }

                        }
                        totalsales += value;
                        EquipSalesByVal es = new EquipSalesByVal();
                        es.EquipModel = modelnum[y];
                        es.value = value;
                        es.valuecur = value.ToString("#,#", CultureInfo.InvariantCulture);
                        es.qty = qtys;
                        db.EquipSalesByVal.Add(es);
                        db.SaveChanges();
                    }

                    //SOT for next three months
                    DateTime todat = Convert.ToDateTime(toda);
                    int month = todat.Month;
                    int year = todat.Year;
                    string[] nextmonths = new string[3];
                    switch (month)
                    {
                        case 1:
                            nextmonths[0] = "Feb," + year;
                            nextmonths[1] = "Mar," + year;
                            nextmonths[2] = "Apr," + year;
                            break;
                        case 2:
                            nextmonths[0] = "Mar," + year;
                            nextmonths[1] = "Apr," + year;
                            nextmonths[2] = "May," + year;
                            break;
                        case 3:
                            nextmonths[0] = "Apr," + year;
                            nextmonths[1] = "May," + year;
                            nextmonths[2] = "Jun," + year;
                            break;
                        case 4:
                            nextmonths[0] = "May," + year;
                            nextmonths[1] = "Jun," + year;
                            nextmonths[2] = "Jul," + year;
                            break;
                        case 5:
                            nextmonths[0] = "Jun," + year;
                            nextmonths[1] = "Jul," + year;
                            nextmonths[2] = "Aug," + year;
                            break;
                        case 6:
                            nextmonths[0] = "Jul," + year;
                            nextmonths[1] = "Aug," + year;
                            nextmonths[2] = "Sep," + year;
                            break;
                        case 7:
                            nextmonths[0] = "Aug," + year;
                            nextmonths[1] = "Sep," + year;
                            nextmonths[2] = "Oct," + year;
                            break;
                        case 8:
                            nextmonths[0] = "Sep," + year;
                            nextmonths[1] = "Oct," + year;
                            nextmonths[2] = "Nov," + year;
                            break;
                        case 9:
                            nextmonths[0] = "Oct," + year;
                            nextmonths[1] = "Nov," + year;
                            nextmonths[2] = "Dec," + year;
                            break;
                        case 10:
                            nextmonths[0] = "Nov," + year;
                            nextmonths[1] = "Dec," + year;
                            nextmonths[2] = "Jan," + (year + 1).ToString();
                            break;
                        case 11:
                            nextmonths[0] = "Dec," + year;
                            nextmonths[1] = "Jan," + (year + 1).ToString();
                            nextmonths[2] = "Feb," + (year + 1).ToString();
                            break;
                        case 12:
                            nextmonths[0] = "Jan," + (year + 1).ToString();
                            nextmonths[1] = "Feb," + (year + 1).ToString();
                            nextmonths[2] = "Mar," + (year + 1).ToString();
                            break;
                    }

                    var start1 = db.SOTByVolCP.ToList();
                    foreach (var se in start1)
                    {
                        db.SOTByVolCP.Remove(se);
                    }
                    db.SaveChanges();

                    for (int y = 0; y < j; y++)
                    {
                        foreach (var dat in nextmonths)
                        {
                            String queryqty = "SELECT * FROM SOTs s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND Expectedorder = '" + dat + "' AND Equipment = '" + modelnum[y] + "' AND Islatestquo = 0 AND BYJTChances != 0 AND BYJTChances != 100 AND Orderactive != 'Project Delayed' AND Orderactive != 'Project Dropped';";
                            var salesqty = db.Database.SqlQuery<SOT>(queryqty).ToList();
                            int qtys = 0;
                            foreach (var sa in salesqty)
                                qtys += sa.Quantity;

                            SOTByVolCP es = new SOTByVolCP();
                            es.EquipModel = modelnum[y];
                            es.qty = qtys;
                            es.Month = dat;
                            db.SOTByVolCP.Add(es);
                            db.SaveChanges();
                        }
                    }

                    //CP Performance Calculation And updation
                    var start2 = db.CPPerformance.ToList();
                    foreach (var se in start2)
                        db.CPPerformance.Remove(se);

                    int losts = 0, delayeds = 0, droppeds = 0, inquotes = 0;
                    decimal percent = 0;

                    inquotes = db.CpperExistquotall(frda, toda);

                    losts = db.CpperOrderlostall(frda, toda);

                    delayeds = db.CpperProdeldropall(frda, toda, "Project Delayed");

                    droppeds = db.CpperProdeldropall(frda, toda, "Project Dropped");

                    var denominator = Convert.ToDecimal(totalsales + inquotes + losts + delayeds + droppeds);

                    if (denominator == 0)
                    {
                        percent = 0;
                    }
                    else
                    {
                        percent = Math.Round(Convert.ToDecimal(((Convert.ToDecimal(totalsales) / Convert.ToDecimal(totalsales + inquotes + losts + delayeds + droppeds))) * 100), 2);
                    }

                    CPPerformance CPP = new CPPerformance();
                    CPP.OnGoingProjectsint = inquotes;
                    CPP.OnGoingProjects = inquotes.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.OrdersLostint = losts;
                    CPP.OrdersLost = losts.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.OrdersWonint = totalsales;
                    CPP.OrdersWon = totalsales.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.ProjectDelayedint = delayeds;
                    CPP.ProjectDelayed = delayeds.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.ProjectDroppedint = droppeds;
                    CPP.ProjectDropped = droppeds.ToString("#,#", CultureInfo.InvariantCulture);
                    CPP.performancepercent = percent + " %";
                    CPP.totalvalue = (inquotes + losts + totalsales + delayeds + droppeds);
                    CPP.totalvalueCur = (inquotes + losts + totalsales + delayeds + droppeds).ToString("#,#", CultureInfo.InvariantCulture);
                    db.CPPerformance.Add(CPP);
                    db.SaveChanges();

                    return RedirectToAction("CPPerformancePrintReport", "Reports", new { fromdat = FromDate, todat = ToDate, CPName = CPName });
                }
                TempData["WrongChannelPartner"] = "Please Enter Valid Channel Partner Name";
            }
            #endregion
            return View();
        }

        public ActionResult EquipSalesByVolAck(string fromdat, string todat, string macty)
        {

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();


            if (fromdat != null && todat != null)
            {
                DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                String frda1 = dtt.ToString("yyyy-MM-dd");
                String toda1 = dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                var start = db.EquipSalesByVolAck.ToList();
                foreach (var se in start)
                {
                    db.EquipSalesByVolAck.Remove(se);
                }
                db.SaveChanges();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    String query = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND ApprovalStatus = 1 Order BY CPID ASC;";
                    var sales = db.Database.SqlQuery<OAEquipGeneralData>(query).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;

                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7" || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }

                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ApprovalStatus = 1 AND CPID = " + cpids[k] + ";";
                            var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                            foreach (var z in salesqty)
                            {
                                int oaidz = z.OAID;
                                String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaidz + "  and ProductModelID = " + modelnum[y] + " ;";
                                var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();

                                int qtys = 0;
                                foreach (var sa in salesqty1)
                                    qtys += sa.Quantity;

                                EquipSalesByVolAck es = new EquipSalesByVolAck();
                                es.CPName = cpnam[k];
                                es.EquipModel = modelnam[y];
                                es.qty = qtys;
                                db.EquipSalesByVolAck.Add(es);
                                db.SaveChanges();
                            }

                        }
                    }
                } //loginname.CPID == 0
                #endregion
                //Report for Buhler Admin
                #region
                if (User.IsInRole("ZonalManager"))
                {
                    int zid = loginname.ZoneID;
                    String query = "SELECT * FROM OAEquipGeneralData s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND ApprovalStatus = 1 Order BY s.CPID ASC;";
                    var sales = db.Database.SqlQuery<OAEquipGeneralData>(query).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;

                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }

                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ApprovalStatus = 1 AND CPID = " + cpids[k] + ";";
                            var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                            foreach (var z in salesqty)
                            {
                                int oaidz = z.OAID;
                                String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaidz + "  and ProductModelID = " + modelnum[y] + " ;";
                                var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();

                                int qtys = 0;
                                foreach (var sa in salesqty1)
                                    qtys += sa.Quantity;

                                EquipSalesByVolAck es = new EquipSalesByVolAck();
                                es.CPName = cpnam[k];
                                es.EquipModel = modelnam[y];
                                es.qty = qtys;
                                db.EquipSalesByVolAck.Add(es);
                                db.SaveChanges();
                            }

                        }
                    }
                } //loginname.CPID == 0
                #endregion
                //For Channel Partner Report
                #region
                else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
                {
                    String query = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND ApprovalStatus = 1 AND CPID = '" + loginname.CPID + "' ";
                    var sales = db.Database.SqlQuery<OAEquipGeneralData>(query).ToList();
                    int cpid = 0;
                    string cpnam = null;
                    int j = 0;

                    cpid = loginname.CPID;
                    cpnam = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }

                    }

                    for (int y = 0; y < j; y++)
                    {
                        String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ApprovalStatus = 1 AND CPID = " + cpid + "";
                        var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                        foreach (var z in salesqty)
                        {
                            int oaidz = z.OAID;
                            String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaidz + "  and ProductModelID = " + modelnum[y] + " ;";
                            var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();

                            int qtys = 0;
                            foreach (var sa in salesqty1)
                                qtys += sa.Quantity;

                            EquipSalesByVolAck es = new EquipSalesByVolAck();
                            es.CPName = cpnam;
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.EquipSalesByVolAck.Add(es);
                            db.SaveChanges();
                        }

                    }
                }
                #endregion
                //if query return null value then it will return with 0 default
                var count = (from u in db.EquipSalesByVolAck
                             where u.qty != 0
                             select (int?)u.qty).Sum() ?? 0;
                //var count = Convert.ToInt32(db.EquipSalesByVolAck.Where(m => m.qty != 0).Sum(m => m.qty));
                return RedirectToAction("EquipSalesByVolPrintReportAck", "Reports", new { fromdat = fromdat, todat = todat, tcount = count });
                //return RedirectToAction("EquipSalesByVolPrintReportAck", "Reports", new { fromdat = fromdat, todat = todat });
            }
            return View();
        }

        public ActionResult MachineInvoiced(string fromdat, string todat, string macty)
        {

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();


            if (fromdat != null && todat != null)
            {
                DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                String frda1 = dtt.ToString("yyyy-MM-dd");
                String toda1 = dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                var start = db.MachineInvoiced.ToList();
                foreach (var se in start)
                {
                    db.MachineInvoiced.Remove(se);
                }
                db.SaveChanges();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    #region
                    String query = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' Order BY CPID ASC;";
                    var sales = db.Database.SqlQuery<MachineDispatch>(query).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;

                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }

                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = " + cpids[k] + " and ProductModelID = " + modelnum[y] + ";";
                            var salesqty = db.Database.SqlQuery<MachineDispatch>(queryqty).ToList();
                            var qty1 = db.Database.SqlQuery<MachineDispatch>(queryqty).Count();

                            int qtys = 0;
                            qtys = qty1;

                            MachineInvoiced es = new MachineInvoiced();
                            es.CPName = cpnam[k];
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.MachineInvoiced.Add(es);
                            db.SaveChanges();
                            ////}

                        }
                    }
                    #endregion
                }
                #endregion
                //For Zonal Manager Report
                #region
                if (User.IsInRole("ZonalManager"))
                {
                    // #region
                    int zid = loginname.ZoneID;
                    // String query = "SELECT * FROM OAEquipGeneralData s, ChannelPartners b WHERE s.CPID = b.CPID AND b.ZoneID = '" + zid + "' AND OADate BETWEEN '" + frda + "' AND '" + toda + "' AND ApprovalStatus = 1 Order BY s.CPID ASC;";
                    // var sales = db.Database.SqlQuery<OAEquipGeneralData>(query).ToList();
                    // int cpid = 0;
                    // string[] cpnam = new string[sales.Count];
                    // int[] cpids = new int[sales.Count];

                    // int i = 0;
                    // int j = 0;

                    // foreach (var s in sales)
                    // {
                    //     if (cpid != s.CPID)
                    //     {
                    //         cpid = s.CPID;
                    //         cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                    //         cpids[i] = cpid;
                    //         i++;
                    //     }
                    // }

                    // var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    // string[] modelnum = new string[models.Count];
                    // string[] modelnam = new string[models.Count];
                    // foreach (var mo in models)
                    // {
                    //     if (macty == "CCD")
                    //     {
                    //         if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                    //         {
                    //             modelnum[j] = mo.ProductModelID.ToString();
                    //             modelnam[j] = mo.ProductModelName;
                    //             j++;
                    //         }
                    //     }
                    //     else if (macty == "CMOS")
                    //     {
                    //         if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                    //         {
                    //             modelnum[j] = mo.ProductModelID.ToString();
                    //             modelnam[j] = mo.ProductModelName;
                    //             j++;
                    //         }
                    //     }
                    //     else if (macty == "W")
                    //     {
                    //         if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                    //         {
                    //             modelnum[j] = mo.ProductModelName;
                    //             j++;
                    //         }
                    //     }
                    //     else if (macty == "ALL")
                    //     {
                    //         modelnum[j] = mo.ProductModelID.ToString();
                    //         modelnam[j] = mo.ProductModelName;
                    //         j++;
                    //     }

                    // }

                    // for (int k = 0; k < i; k++)
                    // {
                    //     for (int y = 0; y < j; y++)
                    //     {
                    //         String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda + "' AND '" + toda + "' and ApprovalStatus = 1 AND CPID = " + cpids[k] + ";";
                    //         var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                    //         foreach (var z in salesqty)
                    //         {
                    //             int oaidz = z.OAID;
                    //             String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaidz + "  and ProductModelID = " + modelnum[y] + " ;";
                    //             var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();

                    //             int qtys = 0;
                    //             foreach (var sa in salesqty1)
                    //                 qtys += sa.Quantity;

                    //             EquipSalesByVolAck es = new EquipSalesByVolAck();
                    //             es.CPName = cpnam[k];
                    //             es.EquipModel = modelnam[y];
                    //             es.qty = qtys;
                    //             db.EquipSalesByVolAck.Add(es);
                    //             db.SaveChanges();
                    //         }

                    //     }
                    // }
                    //#endregion

                    //new
                    #region

                    // String query = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' Order BY CPID ASC;";// WORKING
                    String query = "SELECT * FROM MachineDispatch as s, ChannelPartners as c WHERE c.CPID=s.CPID and s.InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and c.ZoneID='" + zid + "' Order BY s.CPID ASC;";// zone
                    //SELECT * FROM MachineDispatch as s, ChannelPartners as c WHERE c.CPID=s.CPID and s.InvoiceDate BETWEEN '2016-03-01' AND '2016-03-31' and c.ZoneID=3 Order BY s.CPID ASC;
                    //String query = "SELECT * FROM MachineDispatch s, ChannelPartners b WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and b.ZoneID='" + zid + "';";
                    var sales = db.Database.SqlQuery<MachineDispatch>(query).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;

                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }

                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            // String queryqty = "SELECT * FROM MachineDispatch s, ChannelPartners b WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND s.CPID = " + cpids[k] + " and ProductModelID = " + modelnum[y] + " and b.ZoneID='"+zid+"';";
                            //String queryqty = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = " + cpids[k] + " and ProductModelID = " + modelnum[y] + ";";// WORKING

                            String queryqty = "select * from MachineDispatch as s, ChannelPartners as c where c.CPID=s.CPID and c.ZoneID='" + zid + "' and s.InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and ProductModelID =  " + modelnum[y] + " and s.CPID = " + cpids[k] + ";";
                            //
                            var salesqty = db.Database.SqlQuery<MachineDispatch>(queryqty).ToList();
                            var qty1 = db.Database.SqlQuery<MachineDispatch>(queryqty).Count();

                            int qtys = 0;
                            qtys = qty1;

                            MachineInvoiced es = new MachineInvoiced();
                            es.CPName = cpnam[k];
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.MachineInvoiced.Add(es);
                            db.SaveChanges();
                            ////}

                        }
                    }
                    #endregion
                }
                #endregion
                //For Channel Partner Report
                #region
                else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
                {
                    //#region

                    //String query = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda + "' AND '" + toda + "' AND ApprovalStatus = 1 AND CPID = '" + loginname.CPID + "' ";
                    //var sales = db.Database.SqlQuery<OAEquipGeneralData>(query).ToList();
                    //int cpid = 0;
                    //string cpnam = null;
                    //int j = 0;

                    //cpid = loginname.CPID;
                    //cpnam = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();

                    //var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    //string[] modelnum = new string[models.Count];
                    //string[] modelnam = new string[models.Count];
                    //foreach (var mo in models)
                    //{
                    //    if (macty == "CCD")
                    //    {
                    //        if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                    //        {
                    //            modelnum[j] = mo.ProductModelID.ToString();
                    //            modelnam[j] = mo.ProductModelName;
                    //            j++;
                    //        }
                    //    }
                    //    else if (macty == "CMOS")
                    //    {
                    //        if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                    //        {
                    //            modelnum[j] = mo.ProductModelID.ToString();
                    //            modelnam[j] = mo.ProductModelName;
                    //            j++;
                    //        }
                    //    }
                    //    else if (macty == "W")
                    //    {
                    //        if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                    //        {
                    //            modelnum[j] = mo.ProductModelName;
                    //            j++;
                    //        }
                    //    }
                    //    else if (macty == "ALL")
                    //    {
                    //        modelnum[j] = mo.ProductModelID.ToString();
                    //        modelnam[j] = mo.ProductModelName;
                    //        j++;
                    //    }

                    //}

                    //for (int y = 0; y < j; y++)
                    //{
                    //    String queryqty = "SELECT * FROM OAEquipGeneralData WHERE OADate BETWEEN '" + frda + "' AND '" + toda + "' and ApprovalStatus = 1 AND CPID = " + cpid + "";
                    //    var salesqty = db.Database.SqlQuery<OAEquipGeneralData>(queryqty).ToList();

                    //    foreach (var z in salesqty)
                    //    {
                    //        int oaidz = z.OAID;
                    //        String queryqty1 = "SELECT * FROM OAEquipTableData WHERE OAID = " + oaidz + "  and ProductModelID = " + modelnum[y] + " ;";
                    //        var salesqty1 = db.Database.SqlQuery<OAEquipTableData>(queryqty1).ToList();

                    //        int qtys = 0;
                    //        foreach (var sa in salesqty1)
                    //            qtys += sa.Quantity;

                    //        EquipSalesByVolAck es = new EquipSalesByVolAck();
                    //        es.CPName = cpnam;
                    //        es.EquipModel = modelnam[y];
                    //        es.qty = qtys;
                    //        db.EquipSalesByVolAck.Add(es);
                    //        db.SaveChanges();
                    //    }
                    //}
                    //#endregion

                    //new

                    #region
                    String query = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and CPID='" + loginname.CPID + "' Order BY CPID ASC;";
                    var sales = db.Database.SqlQuery<MachineDispatch>(query).ToList();
                    int cpid = 0;
                    string[] cpnam = new string[sales.Count];
                    int[] cpids = new int[sales.Count];

                    int i = 0;
                    int j = 0;

                    foreach (var s in sales)
                    {
                        if (cpid != s.CPID)
                        {
                            cpid = s.CPID;
                            cpnam[i] = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).Single();
                            cpids[i] = cpid;
                            i++;
                        }
                    }

                    var models = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == 1).ToList();
                    string[] modelnum = new string[models.Count];
                    string[] modelnam = new string[models.Count];
                    foreach (var mo in models)
                    {
                        if (macty == "CCD")
                        {
                            if (mo.ProductModelName == "CCD 240" || mo.ProductModelName == "CCD 320" || mo.ProductModelName == "CCD 320 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "CMOS")
                        {
                            if (mo.ProductModelName == "CMOS 160" || mo.ProductModelName == "CMOS 240" || mo.ProductModelName == "CMOS 320")
                            {
                                modelnum[j] = mo.ProductModelID.ToString();
                                modelnam[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "W")
                        {
                            if (mo.ProductModelName == "W - 7 " || mo.ProductModelName == "W - 4" || mo.ProductModelName == "WVV Dual Vision" || mo.ProductModelName == "W4 Tertiary")
                            {
                                modelnum[j] = mo.ProductModelName;
                                j++;
                            }
                        }
                        else if (macty == "ALL")
                        {
                            modelnum[j] = mo.ProductModelID.ToString();
                            modelnam[j] = mo.ProductModelName;
                            j++;
                        }

                    }

                    for (int k = 0; k < i; k++)
                    {
                        for (int y = 0; y < j; y++)
                        {
                            String queryqty = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' AND CPID = " + cpids[k] + " and ProductModelID = " + modelnum[y] + ";";
                            var salesqty = db.Database.SqlQuery<MachineDispatch>(queryqty).ToList();
                            var qty1 = db.Database.SqlQuery<MachineDispatch>(queryqty).Count();

                            int qtys = 0;
                            qtys = qty1;

                            MachineInvoiced es = new MachineInvoiced();
                            es.CPName = cpnam[k];
                            es.EquipModel = modelnam[y];
                            es.qty = qtys;
                            db.MachineInvoiced.Add(es);
                            db.SaveChanges();
                            ////}

                        }
                    }
                    #endregion
                }
                #endregion

                //if query return null value then it will return with 0 default
                //String queryqty1t = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "';"; //WORKING Old

                String queryqty1t = null;
                if (User.IsInRole("ZonalManager"))
                {
                    queryqty1t = "SELECT * FROM MachineDispatch as s, ChannelPartners as c WHERE s.InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "' and s.CPID=c.CPID and c.ZoneID='" + loginname.ZoneID + "';"; //for Zone
                }
                else
                {
                    queryqty1t = "SELECT * FROM MachineDispatch WHERE InvoiceDate BETWEEN '" + frda1 + "' AND '" + toda1 + "';"; //WORKING Old
                }
                //SELECT * FROM MachineDispatch as s, ChannelPartners as c WHERE s.InvoiceDate BETWEEN '2016-03-01' AND '2016-03-31' and s.CPID=c.CPID and c.ZoneID=3
                var count = db.Database.SqlQuery<MachineDispatch>(queryqty1t).Count();
                //var count = (from u in db.EquipSalesByVolAck
                //             where u.qty != 0
                //             select (int?)u.qty).Sum() ?? 0;
                //var count = Convert.ToInt32(db.EquipSalesByVolAck.Where(m => m.qty != 0).Sum(m => m.qty));
                return RedirectToAction("MachineInvoicedPrintReport", "Reports", new { fromdat = fromdat, todat = todat, tcount = count });
            }
            return View();
        }

        public ActionResult MachineInvoicedPrintReport(String fromdat, string todat, string tcount)
        {
            var esbvol = db.MachineInvoiced.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/MachineInvoiced.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("MachineInvoiced", esbvol);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", fromdat));
            reportParameters.Add(new ReportParameter("ToDate", todat));
            reportParameters.Add(new ReportParameter("TCount", tcount));
            localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        public ActionResult EquipSalesByVolPrintReport(String fromdat, string todat, string tcount)
        {
            var esbvol = db.EquipSalesByVol.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/EquipSalesByVol.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("ESBVOLDataSet", esbvol);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", fromdat));
            reportParameters.Add(new ReportParameter("ToDate", todat));
            reportParameters.Add(new ReportParameter("TCount", tcount));
            localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        public ActionResult EquipSalesByVolPrintReportAck(String fromdat, string todat, string tcount)
        {
            var esbvol = db.EquipSalesByVolAck.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/EquipSalesByVolAck.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("EquipVolAck", esbvol);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", fromdat));
            reportParameters.Add(new ReportParameter("ToDate", todat));
            reportParameters.Add(new ReportParameter("TCount", tcount));
            localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        public ActionResult SOTVolPrintReport(String fromdat, String byjt, string tcount)
        {
            var esbvol = db.SOTByVol.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/SOTByVOL.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("SBVOLDataSet", esbvol);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", fromdat));
            reportParameters.Add(new ReportParameter("ToDate", System.DateTime.Now.ToString("dd/MM/yyyy")));
            reportParameters.Add(new ReportParameter("TCount", tcount));
            reportParameters.Add(new ReportParameter("BYJT", byjt));
            if (byjt == null)
            {
                reportParameters.Add(new ReportParameter("ShowBYJT", "true"));
            }
            else
            {
                reportParameters.Add(new ReportParameter("ShowBYJT", "false"));
            }
            if (fromdat == null)
            {
                reportParameters.Add(new ReportParameter("ShowDat", "true"));
            }
            else
            {
                reportParameters.Add(new ReportParameter("ShowDat", "false"));
            }
            localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        public ActionResult LOAPrintReport(String fromdat, string todat, string cpname, string tcount)
        {
            var esbvol = db.LOACCD240.ToList();
            var esbvol1 = db.LOACCD320.ToList();
            var esbvol2 = db.LOACMOS160.ToList();
            var esbvol3 = db.LOACMOS240.ToList();
            var esbvol4 = db.LOACMOS320.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/LOAReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("LOACCD240DataSet", esbvol);
            ReportDataSource reportDataSource1 = new ReportDataSource("LOACCD320DataSet", esbvol1);
            ReportDataSource reportDataSource2 = new ReportDataSource("LOACMOS160DataSet", esbvol2);
            ReportDataSource reportDataSource3 = new ReportDataSource("LOACMOS240DataSet", esbvol3);
            ReportDataSource reportDataSource4 = new ReportDataSource("LOACMOS320DataSet", esbvol4);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", fromdat));
            reportParameters.Add(new ReportParameter("ToDate", todat));
            reportParameters.Add(new ReportParameter("ChannelPartner", cpname));
            reportParameters.Add(new ReportParameter("TCount", tcount));
            localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(reportDataSource1);
            localReport.DataSources.Add(reportDataSource2);
            localReport.DataSources.Add(reportDataSource3);
            localReport.DataSources.Add(reportDataSource4);
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>11in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        public ActionResult CPPerformancePrintReport(String fromdat, string todat, String CPName)
        {
            var esbvol = db.EquipSalesByVal.ToList();
            var sotbl = db.SOTByVolCP.ToList();
            var cpp = db.CPPerformance.ToList();
            if (CPName == null || CPName == "")
                CPName = "ALL Channel Partners";
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/CPPerformance.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("ESBVALDataSet", esbvol);
            ReportDataSource reportDataSource1 = new ReportDataSource("SOTByVolCPDataSet", sotbl);
            ReportDataSource reportDataSource2 = new ReportDataSource("CPPDataSet", cpp);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", fromdat));
            reportParameters.Add(new ReportParameter("ToDate", todat));
            reportParameters.Add(new ReportParameter("CPName", CPName));
            localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(reportDataSource1);
            localReport.DataSources.Add(reportDataSource2);
            localReport.Refresh();
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>11in</PageWidth>" +
                "  <PageHeight>8.5in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        [HttpGet]
        public ActionResult LEReport()
        {
            ViewBag.CPID = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPID", "CPName");
            //ViewData["CPID"] = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPID", "CPName");
            return View();
        }

        [HttpPost]
        public ActionResult LEReport(String fromdat, String todat, String CPName)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            if (fromdat != null && todat != null)
            {
                DateTime dtt = DateTime.ParseExact(fromdat, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime dtt1 = DateTime.ParseExact(todat, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                String frda1 = dtt.ToString("yyyy-MM-dd");
                String toda1 = dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                if (toda < frda)
                {
                    TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                    return View();
                }

                if (User.IsInRole("Administrator"))
                {
                    if (CPName == "" || CPName == null)
                    {
                        //Excel Generation Only for Date Range No Channel Partner
                        #region
                        if (fromdat != null && todat != null)
                        {
                            System.Data.DataTable dts = new System.Data.DataTable();

                            //string orderSql = "SELECT c.CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource] FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l inner join ChannelPartners as c on c.CPID=l.CPID where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "'";

                            string orderSql = "SELECT 'Buhler India Pvt Ltd' as CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource]" +
                            "FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "' and l.CPID =0" +
                            "Union " +
                            "SELECT c.CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource] " +
                            "FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l inner join ChannelPartners as c on c.CPID=l.CPID where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "'";

                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                SqlDataAdapter sda = new SqlDataAdapter(orderSql, conn);
                                sda.Fill(dts);
                            }
                            int ct = dts.Rows.Count;
                            if (ct == 0)
                            {
                                TempData["NoData"] = "No Data Exists for the Selected Date Range!!!!";

                                return View();
                            }
                            //Using the template of the header
                            ExcelPackage templatep = new ExcelPackage(templateFile);
                            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

                            // String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

                            bool exists = System.IO.Directory.Exists(FileDir);

                            if (!exists)
                                System.IO.Directory.CreateDirectory(FileDir);

                            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                            if (newFile.Exists)
                            {
                                try
                                {
                                    newFile.Delete();  // ensures we create a new workbook
                                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                                }
                                catch
                                {
                                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";

                                    return View();
                                }
                            }
                            //Using the File for generation and populating it
                            ExcelPackage p = null;
                            p = new ExcelPackage(newFile);
                            ExcelWorksheet worksheet = null;
                            //Creating the WorkSheet for populating
                            try
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            catch { }
                            if (worksheet == null)
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            //Get the Count of Average values
                            int cnt = dts.Rows.Count;
                            int wt = 7 + cnt;

                            int Row = 8;
                            for (int i = 1; i <= cnt; i++)
                            {
                                worksheet.Cells["A" + Row].Value = i.ToString();
                                Row++;
                            }
                            //Styling the WorkSheet
                            //worksheet.Cells.Style.Font.Name = "Cambria";
                            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#EEFAFF");
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //worksheet.Cells["A8:K" + wt + ""].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            //worksheet.Cells["A8:K" + wt + ""].Style.Font.Size = 12;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Size = 14;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Bold = true;

                            worksheet.Cells["G6"].Value = System.DateTime.Now.ToString("dd-MM-yyyy");
                            //Load the datatable and set the number formats...
                            worksheet.Cells["B8"].LoadFromDataTable(dts, false, OfficeOpenXml.Table.TableStyles.Medium9);
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            p.Save();
                            TempData["Success"] = "Excel Generated Successfully!!!!";

                            string path1 = System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx");
                            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
                            string Outgoingfile = "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx";
                            if (file1.Exists)
                            {
                                Response.Clear();
                                Response.ClearContent();
                                Response.ClearHeaders();
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                                Response.AddHeader("Content-Length", file1.Length.ToString());
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.WriteFile(file1.FullName);
                                Response.Flush();
                                Response.Close();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        var cpd = db.ChannelPartners.Where(m => m.IsDeleted == null).Where(m => m.CPName == CPName).Select(m => m.CPID).SingleOrDefault();
                        int cpid = Convert.ToInt32(cpd);
                        //Excel Generation Only for Date Range with Channel Partner
                        #region
                        if (fromdat != null && todat != null)
                        {
                            System.Data.DataTable dts = new System.Data.DataTable();
                            string orderSql = "SELECT c.CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource] FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l inner join ChannelPartners as c on c.CPID=l.CPID where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "' and l.CPID='" + cpid + "'";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                SqlDataAdapter sda = new SqlDataAdapter(orderSql, conn);
                                sda.Fill(dts);
                            }
                            int ct = dts.Rows.Count;
                            if (ct == 0)
                            {
                                TempData["NoData"] = "No Data Exists for the Selected Date Range!!!!";

                                return View();
                            }
                            //Using the template of the header
                            ExcelPackage templatep = new ExcelPackage(templateFile);
                            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

                            // String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

                            bool exists = System.IO.Directory.Exists(FileDir);

                            if (!exists)
                                System.IO.Directory.CreateDirectory(FileDir);

                            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                            if (newFile.Exists)
                            {
                                try
                                {
                                    newFile.Delete();  // ensures we create a new workbook
                                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                                }
                                catch
                                {
                                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";

                                    return View();
                                }
                            }
                            //Using the File for generation and populating it
                            ExcelPackage p = null;
                            p = new ExcelPackage(newFile);
                            ExcelWorksheet worksheet = null;
                            //Creating the WorkSheet for populating
                            try
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            catch { }
                            if (worksheet == null)
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            //Get the Count of Average values
                            int cnt = dts.Rows.Count;
                            int wt = 7 + cnt;

                            int Row = 8;
                            for (int i = 1; i <= cnt; i++)
                            {
                                worksheet.Cells["A" + Row].Value = i.ToString();
                                Row++;
                            }
                            //Styling the WorkSheet
                            //worksheet.Cells.Style.Font.Name = "Cambria";
                            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#EEFAFF");
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //worksheet.Cells["A8:K" + wt + ""].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            //worksheet.Cells["A8:K" + wt + ""].Style.Font.Size = 12;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Size = 14;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Bold = true;

                            worksheet.Cells["G6"].Value = System.DateTime.Now.ToString("dd-MM-yyyy");
                            //Load the datatable and set the number formats...
                            worksheet.Cells["B8"].LoadFromDataTable(dts, false, OfficeOpenXml.Table.TableStyles.Medium9);
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            p.Save();
                            TempData["Success"] = "Excel Generated Successfully!!!!";

                            string path1 = System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx");
                            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
                            string Outgoingfile = "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx";
                            if (file1.Exists)
                            {
                                Response.Clear();
                                Response.ClearContent();
                                Response.ClearHeaders();
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                                Response.AddHeader("Content-Length", file1.Length.ToString());
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.WriteFile(file1.FullName);
                                Response.Flush();
                                Response.Close();
                            }
                        }
                        #endregion
                    }
                }

                else if (User.IsInRole("ZonalManager"))
                {
                    if (CPName == "" || CPName == null)
                    {
                        //Excel Generation Only for Date Range No Channel Partner
                        #region
                        if (fromdat != null && todat != null)
                        {
                            System.Data.DataTable dts = new System.Data.DataTable();

                            //string orderSql = "SELECT c.CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource] FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l inner join ChannelPartners as c on c.CPID=l.CPID where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "'";

                            string orderSql = "SELECT 'Buhler India Pvt Ltd' as CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource]" +
                            "FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l ,ChannelPartners as c where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "' and l.CPID =0 and c.ZoneID='" + loginname.ZoneID + "'" +
                            "Union " +
                            "SELECT c.CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource] " +
                            "FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l inner join ChannelPartners as c on c.CPID=l.CPID where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "' and c.ZoneID='" + loginname.ZoneID + "'";

                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                SqlDataAdapter sda = new SqlDataAdapter(orderSql, conn);
                                sda.Fill(dts);
                            }
                            int ct = dts.Rows.Count;
                            if (ct == 0)
                            {
                                TempData["NoData"] = "No Data Exists for the Selected Date Range!!!!";

                                return View();
                            }
                            //Using the template of the header
                            ExcelPackage templatep = new ExcelPackage(templateFile);
                            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

                            // String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

                            bool exists = System.IO.Directory.Exists(FileDir);

                            if (!exists)
                                System.IO.Directory.CreateDirectory(FileDir);

                            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                            if (newFile.Exists)
                            {
                                try
                                {
                                    newFile.Delete();  // ensures we create a new workbook
                                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                                }
                                catch
                                {
                                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";

                                    return View();
                                }
                            }
                            //Using the File for generation and populating it
                            ExcelPackage p = null;
                            p = new ExcelPackage(newFile);
                            ExcelWorksheet worksheet = null;
                            //Creating the WorkSheet for populating
                            try
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            catch { }
                            if (worksheet == null)
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            //Get the Count of Average values
                            int cnt = dts.Rows.Count;
                            int wt = 7 + cnt;

                            int Row = 8;
                            for (int i = 1; i <= cnt; i++)
                            {
                                worksheet.Cells["A" + Row].Value = i.ToString();
                                Row++;
                            }
                            //Styling the WorkSheet
                            //worksheet.Cells.Style.Font.Name = "Cambria";
                            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#EEFAFF");
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //worksheet.Cells["A8:K" + wt + ""].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            //worksheet.Cells["A8:K" + wt + ""].Style.Font.Size = 12;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Size = 14;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Bold = true;

                            worksheet.Cells["G6"].Value = System.DateTime.Now.ToString("dd-MM-yyyy");
                            //Load the datatable and set the number formats...
                            worksheet.Cells["B8"].LoadFromDataTable(dts, false, OfficeOpenXml.Table.TableStyles.Medium9);
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            p.Save();
                            TempData["Success"] = "Excel Generated Successfully!!!!";

                            string path1 = System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx");
                            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
                            string Outgoingfile = "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx";
                            if (file1.Exists)
                            {
                                Response.Clear();
                                Response.ClearContent();
                                Response.ClearHeaders();
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                                Response.AddHeader("Content-Length", file1.Length.ToString());
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.WriteFile(file1.FullName);
                                Response.Flush();
                                Response.Close();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        var cpd = db.ChannelPartners.Where(m => m.IsDeleted == null).Where(m => m.CPName == CPName).Select(m => m.CPID).SingleOrDefault();
                        int cpid = Convert.ToInt32(cpd);
                        //Excel Generation Only for Date Range with Channel Partner
                        #region
                        if (fromdat != null && todat != null)
                        {
                            System.Data.DataTable dts = new System.Data.DataTable();
                            string orderSql = "SELECT c.CPName,l.[OrganizationName],l.[AddressLine1] + l.[AddressLine2] + l.[AddressLine3] as Addresss,l.[City],l.[State],l.CreatedOn,l.[Isd1]+l.[Std1]+l.[PhoneLL1] as PhoneNumber,l.[EmailID],l.[SuggestedModel],l.[LeadSource] FROM [SRKSSynergy].[dbo].[LeadEnquiry] as l inner join ChannelPartners as c on c.CPID=l.CPID where l.CreatedOn>='" + frda1 + "' and l.CreatedOn<='" + toda1 + "' and l.CPID='" + cpid + "'";
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                SqlDataAdapter sda = new SqlDataAdapter(orderSql, conn);
                                sda.Fill(dts);
                            }
                            int ct = dts.Rows.Count;
                            if (ct == 0)
                            {
                                TempData["NoData"] = "No Data Exists for the Selected Date Range!!!!";

                                return View();
                            }
                            //Using the template of the header
                            ExcelPackage templatep = new ExcelPackage(templateFile);
                            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

                            // String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

                            bool exists = System.IO.Directory.Exists(FileDir);

                            if (!exists)
                                System.IO.Directory.CreateDirectory(FileDir);

                            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                            if (newFile.Exists)
                            {
                                try
                                {
                                    newFile.Delete();  // ensures we create a new workbook
                                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx"));
                                }
                                catch
                                {
                                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";

                                    return View();
                                }
                            }
                            //Using the File for generation and populating it
                            ExcelPackage p = null;
                            p = new ExcelPackage(newFile);
                            ExcelWorksheet worksheet = null;
                            //Creating the WorkSheet for populating
                            try
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            catch { }
                            if (worksheet == null)
                            {
                                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                            }
                            //Get the Count of Average values
                            int cnt = dts.Rows.Count;
                            int wt = 7 + cnt;

                            int Row = 8;
                            for (int i = 1; i <= cnt; i++)
                            {
                                worksheet.Cells["A" + Row].Value = i.ToString();
                                Row++;
                            }
                            //Styling the WorkSheet
                            //worksheet.Cells.Style.Font.Name = "Cambria";
                            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#EEFAFF");
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["A8:K" + wt + ""].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            //worksheet.Cells["A8:K" + wt + ""].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            //worksheet.Cells["A8:K" + wt + ""].Style.Font.Size = 12;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Size = 14;
                            //worksheet.Cells["B7,C7,D7"].Style.Font.Bold = true;

                            worksheet.Cells["G6"].Value = System.DateTime.Now.ToString("dd-MM-yyyy");
                            //Load the datatable and set the number formats...
                            worksheet.Cells["B8"].LoadFromDataTable(dts, false, OfficeOpenXml.Table.TableStyles.Medium9);
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            p.Save();
                            TempData["Success"] = "Excel Generated Successfully!!!!";

                            string path1 = System.IO.Path.Combine(FileDir, "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx");
                            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
                            string Outgoingfile = "Enquiry " + frda.ToString("dd-MM-yyyy") + " to " + toda.ToString("dd-MM-yyyy") + ".xlsx";
                            if (file1.Exists)
                            {
                                Response.Clear();
                                Response.ClearContent();
                                Response.ClearHeaders();
                                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                                Response.AddHeader("Content-Length", file1.Length.ToString());
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.WriteFile(file1.FullName);
                                Response.Flush();
                                Response.Close();
                            }
                        }
                        #endregion
                    }
                }
                //else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
                //{

                //}

            }

            ViewBag.CPID = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0), "CPID", "CPName");
            return View("LEReport");
        }

        public ActionResult YJTLeadReport(string cpnam = null, string frommonth = null, string tomonth = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();

            var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
            var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

            ViewBag.fromdte = frommonth;
            ViewBag.todate = tomonth;
            ViewBag.OrganizationName = cpnam;

            var oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LERID).ToList();

            if (cpnam != null && cpnam != "")
            {
                ViewBag.IsSearch = true;

                oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID).Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LERID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("YJTLeadReport", "Reports");
                }
                var cpid = chpart.CPID;
            }

            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                if (toda < frda)
                {
                    TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                    return View();
                }

                oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("YJTLeadReport", "Reports");
                }
            }

            if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                if (toda < frda)
                {
                    TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                    return View();
                }

                oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                if (oagendata.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                    return RedirectToAction("YJTLeadReport", "Reports");
                }
            }

            if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                ViewBag.IsSearch = true;

                String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                if (toda < frda)
                {
                    TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                    return View();
                }

                chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

                oagendata = (from s in db.LeadEnquiryRevised
                             where s.LeadDate >= frda && s.LeadDate <= toda
                             where s.CPID == channame
                             select s).ToList();
                oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("YJTLeadReport", "Reports");
                }
                var cpid = chpart.CPID;
            }
            return View(oagendata);
        }

        public ActionResult ExportDataYJTLead(string cpnam = null, string frommonth = null, string tomonth = null)
        {
            GridView gv = new GridView();

            if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                //1
                #region
                DataTable dts = new DataTable();
                dts.Columns.Add("Slno", typeof(String));
                dts.Columns.Add("Name Of Collector", typeof(String));
                dts.Columns.Add("Lead Date", typeof(String));
                dts.Columns.Add("Mill Name", typeof(String));
                dts.Columns.Add("Address", typeof(String));
                dts.Columns.Add("City", typeof(String));
                dts.Columns.Add("State", typeof(String));
                dts.Columns.Add("Owner Name", typeof(String));
                dts.Columns.Add("Owner Contact No", typeof(String));
                dts.Columns.Add("Owner Email-ID", typeof(String));
                dts.Columns.Add("Owner Telephone No", typeof(String));
                dts.Columns.Add("Enquiry Type", typeof(String));
                dts.Columns.Add("Type of Mill", typeof(String));
                dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                dts.Columns.Add("Conditioning", typeof(String));
                dts.Columns.Add("Paddy Moisture(In %)", typeof(String));
                dts.Columns.Add("Bulk Density Kgs/M3", typeof(String));
                dts.Columns.Add("Paddy Variety", typeof(String));
                dts.Columns.Add("Process Capcity( in TPH)", typeof(String));
                dts.Columns.Add("Operating Details", typeof(String));
                dts.Columns.Add("Pre Cleaning Capacity", typeof(String));
                dts.Columns.Add("20-25 TPH", typeof(String));
                dts.Columns.Add("40-50 TPH", typeof(String));
                dts.Columns.Add("Others", typeof(String));
                dts.Columns.Add("Total (In MT)", typeof(String));
                dts.Columns.Add("Each X (In Silos)", typeof(String));
                dts.Columns.Add("Suppliers Name", typeof(String));
                dts.Columns.Add("Year Installed", typeof(String));
                dts.Columns.Add("Huller No", typeof(String));
                dts.Columns.Add("Huller KW", typeof(String));
                dts.Columns.Add("Hull Seperator (In No's)", typeof(String));
                dts.Columns.Add("Paddy Table(In No's)", typeof(String));
                dts.Columns.Add("Thin/Thick Grader (In No's)", typeof(String));
                dts.Columns.Add("Drum's Each", typeof(String));
                dts.Columns.Add("Whitner No", typeof(String));
                dts.Columns.Add("Whitner KW", typeof(String));
                dts.Columns.Add("Whitner RPM", typeof(String));
                dts.Columns.Add("Polisher No", typeof(String));
                dts.Columns.Add("Polisher KW", typeof(String));
                dts.Columns.Add("Polisher RPM", typeof(String));
                dts.Columns.Add("Length Grader (In No's)", typeof(String));
                dts.Columns.Add("Grade Size 1", typeof(String));
                dts.Columns.Add("Grade Size 2", typeof(String));
                dts.Columns.Add("Grade Size 3", typeof(String));
                dts.Columns.Add("Grade Size 4", typeof(String));
                dts.Columns.Add("Color Sorter Make", typeof(String));
                dts.Columns.Add("Color Sorter Channels", typeof(String));
                dts.Columns.Add("Color Sorter Primary", typeof(String));
                dts.Columns.Add("Color Sorter Secondary", typeof(String));
                dts.Columns.Add("Packing Machine Make", typeof(String));
                dts.Columns.Add("Packing Machine (In No's)", typeof(String));
                dts.Columns.Add("Pack Size 1 (In Kg)", typeof(String));
                dts.Columns.Add("Pack Size 2 (In Kg)", typeof(String));
                dts.Columns.Add("Brand Names", typeof(String));
                dts.Columns.Add("Plant Automation Type", typeof(String));
                dts.Columns.Add("Requirement 1", typeof(String));
                dts.Columns.Add("Requirement 2", typeof(String));
                dts.Columns.Add("Requirement 3", typeof(String));
                dts.Columns.Add("Requirement 4", typeof(String));
                dts.Columns.Add("Requirement 5", typeof(String));
                dts.Columns.Add("Comments / Action Expected", typeof(String));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;

                var duplicate = (from s in db.LeadEnquiryRevised
                                 where s.CPID == channame && s.LeadDate >= frda && s.LeadDate <= toda
                                 select new
                                 {
                                     s.NameofCollector,
                                     s.LeadDate,
                                     s.MillName,
                                     s.AddressLine1,
                                     s.AddressLine2,
                                     s.City,
                                     s.Pincode,
                                     s.State,
                                     s.OwnerName,
                                     s.MobNo,
                                     s.EmailId,
                                     s.Isd,
                                     s.Std,
                                     s.TelNo,

                                     s.EnquiryTypeExistingMill,
                                     s.EnquiryTypeNewMill,
                                     s.EnquiryTypeCustomerServiceUpgrade,

                                     s.TypeOfMillPaddytoRice,
                                     s.TypeOfMillBrownRiceToWhiteRice,
                                     s.TypeOfMillReprocess,

                                     s.TypeOfPaddyProcessLongGrain,
                                     s.TypeOfPaddyProcessMediumGrain,
                                     s.TypeOfPaddyProcessRountGrain,

                                     s.ConditioningRaw,
                                     s.ConditioningSteamed,
                                     s.ConditioningParBoiled,
                                     s.ConditioningParBoiledPaddyMoisture,
                                     s.ConditioningParBoiledBulkDensity,

                                     s.PaddyVarietyBasmati,
                                     s.PaddyVarietyNonBasmati,
                                     s.PaddyVarietyVarietyName,

                                     s.EMDProcessingCapacityMill,
                                     s.EMDProcessingCapacityMillTPH,
                                     s.EMDMillOperatingDetailsHoursPerDay,

                                     s.PreCleaningCapacity,
                                     s.TPH2025,
                                     s.TPH4050,
                                     s.Others,
                                     s.OthersTPH,
                                     s.PaddyStorageMTTotal,
                                     s.TotalMT,
                                     s.NoOfSilos,

                                     s.MillDetailsSuppliersName,
                                     s.YearInstalled,

                                     s.MillSectionHuller,
                                     s.MillSectionNos,
                                     s.MillSectionKW,
                                     s.MillSectionHullSeperate,
                                     s.MillSectionHullSeperateNos,
                                     s.PaddyTable,
                                     s.PaddyTableNos,
                                     s.ThinThickGrader,
                                     s.ThinThickGraderNos,
                                     s.ThinThickGraderDumps,

                                     s.Whitner,
                                     s.WhitnerNos,
                                     s.WhitnerKW,
                                     s.WhitnerRPM,

                                     s.Polisher,
                                     s.PolisherNos,
                                     s.PolisherKW,
                                     s.PolisherRPM,

                                     s.LengthGrader,
                                     s.LengthGraderNos,
                                     s.GradeSize1,
                                     s.GradeSize2,
                                     s.GradeSize3,
                                     s.GradeSize4,

                                     s.ColorSorter,
                                     s.ColorSorterMake,
                                     s.ColorSorterChannels,
                                     s.ColorSorterPrimary,
                                     s.ColorSorterSecondary,

                                     s.PackingMachine,
                                     s.PackingMachineMake,
                                     s.PackingMachineNos,
                                     s.PackingMachineKgs1,
                                     s.PackingMachineKgs2,
                                     s.PackingMachineBrandName,

                                     s.PlantAutomationTypeRelayOrPLCBased,
                                     s.PlantAutomationTypeRelayOrPLCBasedMake,

                                     s.CustomerRequirement1,
                                     s.CustomerRequirement2,
                                     s.CustomerRequirement3,
                                     s.CustomerRequirement4,
                                     s.CustomerRequirement5,

                                     s.CommentsActionExpected,
                                 });
                var dt1 = duplicate.ToList();
                int cnt1 = dt1.Count;
                int cnt2 = dt1.Count;

                foreach (var d in duplicate)
                {
                    //int cnt3 = cnt1++;
                    int sl = cnt1++ - cnt2;
                    DataRow dr = dts.NewRow();
                    dr[0] = sl + 1;
                    dr[1] = d.NameofCollector;
                    dr[2] = d.LeadDate.ToString("dd-MM-yyyy");
                    dr[3] = d.MillName;
                    dr[4] = d.AddressLine1 + ", " + d.AddressLine2;
                    dr[5] = d.City;
                    dr[6] = d.State;
                    dr[7] = d.OwnerName;
                    dr[8] = d.MobNo;
                    dr[9] = d.EmailId;
                    dr[10] = d.Isd + "-" + d.Std + "-" + d.TelNo;

                    if (d.EnquiryTypeExistingMill == true)
                    {
                        dr[11] = "Existing Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "New Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "Customer Service Upgrade";
                    }
                    else
                    {
                        dr[11] = "Not Mentioned";
                    }

                    if (d.TypeOfMillPaddytoRice == true)
                    {
                        dr[12] = "Paddy To Rice";
                    }
                    else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                    {
                        dr[12] = "Brown-Rice To White-Rice";
                    }
                    else if (d.TypeOfMillReprocess == true)
                    {
                        dr[12] = "Reprocess";
                    }
                    else
                    {
                        dr[12] = "Not Mentioned";
                    }
                    if (d.TypeOfPaddyProcessLongGrain == true)
                    {
                        dr[13] = "Long Grain";
                    }
                    else if (d.TypeOfPaddyProcessMediumGrain == true)
                    {
                        dr[13] = "Medium Grain";
                    }
                    else if (d.TypeOfPaddyProcessRountGrain == true)
                    {
                        dr[13] = "Round Grain";
                    }
                    else
                    {
                        dr[13] = "Not Mentioned";
                    }

                    if (d.ConditioningRaw == true)
                    {
                        dr[14] = "Raw";
                    }
                    else if (d.ConditioningSteamed == true)
                    {
                        dr[14] = "Steamed";
                    }
                    else if (d.ConditioningParBoiled == true)
                    {
                        dr[14] = "Par Boiled";
                    }
                    else
                    {
                        dr[14] = "Not Mentioned";
                    }

                    dr[15] = d.ConditioningParBoiledPaddyMoisture;
                    dr[16] = d.ConditioningParBoiledBulkDensity;

                    if (d.PaddyVarietyBasmati == true)
                    {
                        dr[17] = "Basmati";
                    }
                    else if (d.PaddyVarietyNonBasmati == true)
                    {
                        dr[17] = "Non-Basmati";
                    }
                    else
                    {
                        dr[17] = d.PaddyVarietyVarietyName;
                    }
                    dr[18] = d.EMDProcessingCapacityMillTPH;
                    dr[19] = d.EMDMillOperatingDetailsHoursPerDay;
                    dr[20] = d.PreCleaningCapacity;
                    dr[21] = d.TPH2025;
                    dr[22] = d.TPH4050;
                    dr[23] = d.OthersTPH;
                    dr[24] = d.TotalMT;
                    dr[25] = d.NoOfSilos;
                    dr[26] = d.MillDetailsSuppliersName;
                    dr[27] = d.YearInstalled;

                    dr[28] = d.MillSectionNos;
                    dr[29] = d.MillSectionKW;
                    dr[30] = d.MillSectionHullSeperateNos;
                    dr[31] = d.PaddyTableNos;
                    dr[32] = d.ThinThickGraderNos;
                    dr[33] = d.ThinThickGraderDumps;
                    dr[34] = d.WhitnerNos;
                    dr[35] = d.WhitnerKW;
                    dr[36] = d.WhitnerRPM;
                    dr[37] = d.PolisherNos;
                    dr[38] = d.PolisherKW;
                    dr[39] = d.PolisherRPM;
                    dr[40] = d.LengthGraderNos;
                    dr[41] = d.GradeSize1;
                    dr[42] = d.GradeSize2;
                    dr[43] = d.GradeSize3;
                    dr[44] = d.GradeSize4;
                    dr[45] = d.ColorSorterMake;
                    dr[46] = d.ColorSorterChannels;
                    dr[47] = d.ColorSorterPrimary;
                    dr[48] = d.ColorSorterSecondary;
                    dr[49] = d.PackingMachineMake;
                    dr[50] = d.PackingMachineNos;
                    dr[51] = d.PackingMachineKgs1;
                    dr[52] = d.PackingMachineKgs2;
                    dr[53] = d.PackingMachineBrandName;
                    dr[54] = d.PlantAutomationTypeRelayOrPLCBasedMake;
                    dr[55] = d.CustomerRequirement1;
                    dr[56] = d.CustomerRequirement2;
                    dr[57] = d.CustomerRequirement3;
                    dr[58] = d.CustomerRequirement4;
                    dr[59] = d.CustomerRequirement5;
                    dr[60] = d.CommentsActionExpected;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
                #endregion
            }

            else if (cpnam != null && cpnam != "")
            {
                //2
                #region
                DataTable dts = new DataTable();
                dts.Columns.Add("Slno", typeof(String));
                dts.Columns.Add("Name Of Collector", typeof(String));
                dts.Columns.Add("Lead Date", typeof(String));
                dts.Columns.Add("Mill Name", typeof(String));
                dts.Columns.Add("Address", typeof(String));
                dts.Columns.Add("City", typeof(String));
                dts.Columns.Add("State", typeof(String));
                dts.Columns.Add("Owner Name", typeof(String));
                dts.Columns.Add("Owner Contact No", typeof(String));
                dts.Columns.Add("Owner Email-ID", typeof(String));
                dts.Columns.Add("Owner Telephone No", typeof(String));
                dts.Columns.Add("Enquiry Type", typeof(String));
                dts.Columns.Add("Type of Mill", typeof(String));
                dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                dts.Columns.Add("Conditioning", typeof(String));
                dts.Columns.Add("Paddy Moisture(In %)", typeof(String));
                dts.Columns.Add("Bulk Density Kgs/M3", typeof(String));
                dts.Columns.Add("Paddy Variety", typeof(String));
                dts.Columns.Add("Process Capcity( in TPH)", typeof(String));
                dts.Columns.Add("Operating Details", typeof(String));
                dts.Columns.Add("Pre Cleaning Capacity", typeof(String));
                dts.Columns.Add("20-25 TPH", typeof(String));
                dts.Columns.Add("40-50 TPH", typeof(String));
                dts.Columns.Add("Others", typeof(String));
                dts.Columns.Add("Total (In MT)", typeof(String));
                dts.Columns.Add("Each X (In Silos)", typeof(String));
                dts.Columns.Add("Suppliers Name", typeof(String));
                dts.Columns.Add("Year Installed", typeof(String));
                dts.Columns.Add("Huller No", typeof(String));
                dts.Columns.Add("Huller KW", typeof(String));
                dts.Columns.Add("Hull Seperator (In No's)", typeof(String));
                dts.Columns.Add("Paddy Table(In No's)", typeof(String));
                dts.Columns.Add("Thin/Thick Grader (In No's)", typeof(String));
                dts.Columns.Add("Drum's Each", typeof(String));
                dts.Columns.Add("Whitner No", typeof(String));
                dts.Columns.Add("Whitner KW", typeof(String));
                dts.Columns.Add("Whitner RPM", typeof(String));
                dts.Columns.Add("Polisher No", typeof(String));
                dts.Columns.Add("Polisher KW", typeof(String));
                dts.Columns.Add("Polisher RPM", typeof(String));
                dts.Columns.Add("Length Grader (In No's)", typeof(String));
                dts.Columns.Add("Grade Size 1", typeof(String));
                dts.Columns.Add("Grade Size 2", typeof(String));
                dts.Columns.Add("Grade Size 3", typeof(String));
                dts.Columns.Add("Grade Size 4", typeof(String));
                dts.Columns.Add("Color Sorter Make", typeof(String));
                dts.Columns.Add("Color Sorter Channels", typeof(String));
                dts.Columns.Add("Color Sorter Primary", typeof(String));
                dts.Columns.Add("Color Sorter Secondary", typeof(String));
                dts.Columns.Add("Packing Machine Make", typeof(String));
                dts.Columns.Add("Packing Machine (In No's)", typeof(String));
                dts.Columns.Add("Pack Size 1 (In Kg)", typeof(String));
                dts.Columns.Add("Pack Size 2 (In Kg)", typeof(String));
                dts.Columns.Add("Brand Names", typeof(String));
                dts.Columns.Add("Plant Automation Type", typeof(String));
                dts.Columns.Add("Requirement 1", typeof(String));
                dts.Columns.Add("Requirement 2", typeof(String));
                dts.Columns.Add("Requirement 3", typeof(String));
                dts.Columns.Add("Requirement 4", typeof(String));
                dts.Columns.Add("Requirement 5", typeof(String));
                dts.Columns.Add("Comments / Action Expected", typeof(String));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                var duplicate = (from s in db.LeadEnquiryRevised
                                 where s.CPID == channame
                                 select new
                                 {
                                     s.NameofCollector,
                                     s.LeadDate,
                                     s.MillName,
                                     s.AddressLine1,
                                     s.AddressLine2,
                                     s.City,
                                     s.Pincode,
                                     s.State,
                                     s.OwnerName,
                                     s.MobNo,
                                     s.EmailId,
                                     s.Isd,
                                     s.Std,
                                     s.TelNo,

                                     s.EnquiryTypeExistingMill,
                                     s.EnquiryTypeNewMill,
                                     s.EnquiryTypeCustomerServiceUpgrade,

                                     s.TypeOfMillPaddytoRice,
                                     s.TypeOfMillBrownRiceToWhiteRice,
                                     s.TypeOfMillReprocess,

                                     s.TypeOfPaddyProcessLongGrain,
                                     s.TypeOfPaddyProcessMediumGrain,
                                     s.TypeOfPaddyProcessRountGrain,

                                     s.ConditioningRaw,
                                     s.ConditioningSteamed,
                                     s.ConditioningParBoiled,
                                     s.ConditioningParBoiledPaddyMoisture,
                                     s.ConditioningParBoiledBulkDensity,

                                     s.PaddyVarietyBasmati,
                                     s.PaddyVarietyNonBasmati,
                                     s.PaddyVarietyVarietyName,

                                     s.EMDProcessingCapacityMill,
                                     s.EMDProcessingCapacityMillTPH,
                                     s.EMDMillOperatingDetailsHoursPerDay,

                                     s.PreCleaningCapacity,
                                     s.TPH2025,
                                     s.TPH4050,
                                     s.Others,
                                     s.OthersTPH,
                                     s.PaddyStorageMTTotal,
                                     s.TotalMT,
                                     s.NoOfSilos,

                                     s.MillDetailsSuppliersName,
                                     s.YearInstalled,

                                     s.MillSectionHuller,
                                     s.MillSectionNos,
                                     s.MillSectionKW,
                                     s.MillSectionHullSeperate,
                                     s.MillSectionHullSeperateNos,
                                     s.PaddyTable,
                                     s.PaddyTableNos,
                                     s.ThinThickGrader,
                                     s.ThinThickGraderNos,
                                     s.ThinThickGraderDumps,

                                     s.Whitner,
                                     s.WhitnerNos,
                                     s.WhitnerKW,
                                     s.WhitnerRPM,

                                     s.Polisher,
                                     s.PolisherNos,
                                     s.PolisherKW,
                                     s.PolisherRPM,

                                     s.LengthGrader,
                                     s.LengthGraderNos,
                                     s.GradeSize1,
                                     s.GradeSize2,
                                     s.GradeSize3,
                                     s.GradeSize4,

                                     s.ColorSorter,
                                     s.ColorSorterMake,
                                     s.ColorSorterChannels,
                                     s.ColorSorterPrimary,
                                     s.ColorSorterSecondary,

                                     s.PackingMachine,
                                     s.PackingMachineMake,
                                     s.PackingMachineNos,
                                     s.PackingMachineKgs1,
                                     s.PackingMachineKgs2,
                                     s.PackingMachineBrandName,

                                     s.PlantAutomationTypeRelayOrPLCBased,
                                     s.PlantAutomationTypeRelayOrPLCBasedMake,

                                     s.CustomerRequirement1,
                                     s.CustomerRequirement2,
                                     s.CustomerRequirement3,
                                     s.CustomerRequirement4,
                                     s.CustomerRequirement5,

                                     s.CommentsActionExpected,
                                 });
                var dt1 = duplicate.ToList();
                int cnt1 = dt1.Count;
                int cnt2 = dt1.Count;

                foreach (var d in duplicate)
                {
                    //int cnt3 = cnt1++;
                    int sl = cnt1++ - cnt2;
                    DataRow dr = dts.NewRow();
                    dr[0] = sl + 1;
                    dr[1] = d.NameofCollector;
                    dr[2] = d.LeadDate.ToString("dd-MM-yyyy");
                    dr[3] = d.MillName;
                    dr[4] = d.AddressLine1 + ", " + d.AddressLine2;
                    dr[5] = d.City;
                    dr[6] = d.State;
                    dr[7] = d.OwnerName;
                    dr[8] = d.MobNo;
                    dr[9] = d.EmailId;
                    dr[10] = d.Isd + "-" + d.Std + "-" + d.TelNo;

                    if (d.EnquiryTypeExistingMill == true)
                    {
                        dr[11] = "Existing Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "New Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "Customer Service Upgrade";
                    }
                    else
                    {
                        dr[11] = "Not Mentioned";
                    }

                    if (d.TypeOfMillPaddytoRice == true)
                    {
                        dr[12] = "Paddy To Rice";
                    }
                    else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                    {
                        dr[12] = "Brown-Rice To White-Rice";
                    }
                    else if (d.TypeOfMillReprocess == true)
                    {
                        dr[12] = "Reprocess";
                    }
                    else
                    {
                        dr[12] = "Not Mentioned";
                    }
                    if (d.TypeOfPaddyProcessLongGrain == true)
                    {
                        dr[13] = "Long Grain";
                    }
                    else if (d.TypeOfPaddyProcessMediumGrain == true)
                    {
                        dr[13] = "Medium Grain";
                    }
                    else if (d.TypeOfPaddyProcessRountGrain == true)
                    {
                        dr[13] = "Round Grain";
                    }
                    else
                    {
                        dr[13] = "Not Mentioned";
                    }

                    if (d.ConditioningRaw == true)
                    {
                        dr[14] = "Raw";
                    }
                    else if (d.ConditioningSteamed == true)
                    {
                        dr[14] = "Steamed";
                    }
                    else if (d.ConditioningParBoiled == true)
                    {
                        dr[14] = "Par Boiled";
                    }
                    else
                    {
                        dr[14] = "Not Mentioned";
                    }

                    dr[15] = d.ConditioningParBoiledPaddyMoisture;
                    dr[16] = d.ConditioningParBoiledBulkDensity;

                    if (d.PaddyVarietyBasmati == true)
                    {
                        dr[17] = "Basmati";
                    }
                    else if (d.PaddyVarietyNonBasmati == true)
                    {
                        dr[17] = "Non-Basmati";
                    }
                    else
                    {
                        dr[17] = d.PaddyVarietyVarietyName;
                    }
                    dr[18] = d.EMDProcessingCapacityMillTPH;
                    dr[19] = d.EMDMillOperatingDetailsHoursPerDay;
                    dr[20] = d.PreCleaningCapacity;
                    dr[21] = d.TPH2025;
                    dr[22] = d.TPH4050;
                    dr[23] = d.OthersTPH;
                    dr[24] = d.TotalMT;
                    dr[25] = d.NoOfSilos;
                    dr[26] = d.MillDetailsSuppliersName;
                    dr[27] = d.YearInstalled;

                    dr[28] = d.MillSectionNos;
                    dr[29] = d.MillSectionKW;
                    dr[30] = d.MillSectionHullSeperateNos;
                    dr[31] = d.PaddyTableNos;
                    dr[32] = d.ThinThickGraderNos;
                    dr[33] = d.ThinThickGraderDumps;
                    dr[34] = d.WhitnerNos;
                    dr[35] = d.WhitnerKW;
                    dr[36] = d.WhitnerRPM;
                    dr[37] = d.PolisherNos;
                    dr[38] = d.PolisherKW;
                    dr[39] = d.PolisherRPM;
                    dr[40] = d.LengthGraderNos;
                    dr[41] = d.GradeSize1;
                    dr[42] = d.GradeSize2;
                    dr[43] = d.GradeSize3;
                    dr[44] = d.GradeSize4;
                    dr[45] = d.ColorSorterMake;
                    dr[46] = d.ColorSorterChannels;
                    dr[47] = d.ColorSorterPrimary;
                    dr[48] = d.ColorSorterSecondary;
                    dr[49] = d.PackingMachineMake;
                    dr[50] = d.PackingMachineNos;
                    dr[51] = d.PackingMachineKgs1;
                    dr[52] = d.PackingMachineKgs2;
                    dr[53] = d.PackingMachineBrandName;
                    dr[54] = d.PlantAutomationTypeRelayOrPLCBasedMake;
                    dr[55] = d.CustomerRequirement1;
                    dr[56] = d.CustomerRequirement2;
                    dr[57] = d.CustomerRequirement3;
                    dr[58] = d.CustomerRequirement4;
                    dr[59] = d.CustomerRequirement5;
                    dr[60] = d.CommentsActionExpected;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
                #endregion

            }
            else if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                //3
                #region
                DataTable dts = new DataTable();
                dts.Columns.Add("Slno", typeof(String));
                dts.Columns.Add("Name Of Collector", typeof(String));
                dts.Columns.Add("Lead Date", typeof(String));
                dts.Columns.Add("Mill Name", typeof(String));
                dts.Columns.Add("Address", typeof(String));
                dts.Columns.Add("City", typeof(String));
                dts.Columns.Add("State", typeof(String));
                dts.Columns.Add("Owner Name", typeof(String));
                dts.Columns.Add("Owner Contact No", typeof(String));
                dts.Columns.Add("Owner Email-ID", typeof(String));
                dts.Columns.Add("Owner Telephone No", typeof(String));
                dts.Columns.Add("Enquiry Type", typeof(String));
                dts.Columns.Add("Type of Mill", typeof(String));
                dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                dts.Columns.Add("Conditioning", typeof(String));
                dts.Columns.Add("Paddy Moisture(In %)", typeof(String));
                dts.Columns.Add("Bulk Density Kgs/M3", typeof(String));
                dts.Columns.Add("Paddy Variety", typeof(String));
                dts.Columns.Add("Process Capcity( in TPH)", typeof(String));
                dts.Columns.Add("Operating Details", typeof(String));
                dts.Columns.Add("Pre Cleaning Capacity", typeof(String));
                dts.Columns.Add("20-25 TPH", typeof(String));
                dts.Columns.Add("40-50 TPH", typeof(String));
                dts.Columns.Add("Others", typeof(String));
                dts.Columns.Add("Total (In MT)", typeof(String));
                dts.Columns.Add("Each X (In Silos)", typeof(String));
                dts.Columns.Add("Suppliers Name", typeof(String));
                dts.Columns.Add("Year Installed", typeof(String));
                dts.Columns.Add("Huller No", typeof(String));
                dts.Columns.Add("Huller KW", typeof(String));
                dts.Columns.Add("Hull Seperator (In No's)", typeof(String));
                dts.Columns.Add("Paddy Table(In No's)", typeof(String));
                dts.Columns.Add("Thin/Thick Grader (In No's)", typeof(String));
                dts.Columns.Add("Drum's Each", typeof(String));
                dts.Columns.Add("Whitner No", typeof(String));
                dts.Columns.Add("Whitner KW", typeof(String));
                dts.Columns.Add("Whitner RPM", typeof(String));
                dts.Columns.Add("Polisher No", typeof(String));
                dts.Columns.Add("Polisher KW", typeof(String));
                dts.Columns.Add("Polisher RPM", typeof(String));
                dts.Columns.Add("Length Grader (In No's)", typeof(String));
                dts.Columns.Add("Grade Size 1", typeof(String));
                dts.Columns.Add("Grade Size 2", typeof(String));
                dts.Columns.Add("Grade Size 3", typeof(String));
                dts.Columns.Add("Grade Size 4", typeof(String));
                dts.Columns.Add("Color Sorter Make", typeof(String));
                dts.Columns.Add("Color Sorter Channels", typeof(String));
                dts.Columns.Add("Color Sorter Primary", typeof(String));
                dts.Columns.Add("Color Sorter Secondary", typeof(String));
                dts.Columns.Add("Packing Machine Make", typeof(String));
                dts.Columns.Add("Packing Machine (In No's)", typeof(String));
                dts.Columns.Add("Pack Size 1 (In Kg)", typeof(String));
                dts.Columns.Add("Pack Size 2 (In Kg)", typeof(String));
                dts.Columns.Add("Brand Names", typeof(String));
                dts.Columns.Add("Plant Automation Type", typeof(String));
                dts.Columns.Add("Requirement 1", typeof(String));
                dts.Columns.Add("Requirement 2", typeof(String));
                dts.Columns.Add("Requirement 3", typeof(String));
                dts.Columns.Add("Requirement 4", typeof(String));
                dts.Columns.Add("Requirement 5", typeof(String));
                dts.Columns.Add("Comments / Action Expected", typeof(String));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                DateTime frda = DateTime.Parse(frda1).Date;
                DateTime toda = DateTime.Parse(toda1).Date;


                var duplicate = (from s in db.LeadEnquiryRevised
                                 where s.LeadDate >= frda && s.LeadDate <= toda
                                 //where s.CPID == channame
                                 select new
                                 {
                                     s.NameofCollector,
                                     s.LeadDate,
                                     s.MillName,
                                     s.AddressLine1,
                                     s.AddressLine2,
                                     s.City,
                                     s.Pincode,
                                     s.State,
                                     s.OwnerName,
                                     s.MobNo,
                                     s.EmailId,
                                     s.Isd,
                                     s.Std,
                                     s.TelNo,

                                     s.EnquiryTypeExistingMill,
                                     s.EnquiryTypeNewMill,
                                     s.EnquiryTypeCustomerServiceUpgrade,

                                     s.TypeOfMillPaddytoRice,
                                     s.TypeOfMillBrownRiceToWhiteRice,
                                     s.TypeOfMillReprocess,

                                     s.TypeOfPaddyProcessLongGrain,
                                     s.TypeOfPaddyProcessMediumGrain,
                                     s.TypeOfPaddyProcessRountGrain,

                                     s.ConditioningRaw,
                                     s.ConditioningSteamed,
                                     s.ConditioningParBoiled,
                                     s.ConditioningParBoiledPaddyMoisture,
                                     s.ConditioningParBoiledBulkDensity,

                                     s.PaddyVarietyBasmati,
                                     s.PaddyVarietyNonBasmati,
                                     s.PaddyVarietyVarietyName,

                                     s.EMDProcessingCapacityMill,
                                     s.EMDProcessingCapacityMillTPH,
                                     s.EMDMillOperatingDetailsHoursPerDay,

                                     s.PreCleaningCapacity,
                                     s.TPH2025,
                                     s.TPH4050,
                                     s.Others,
                                     s.OthersTPH,
                                     s.PaddyStorageMTTotal,
                                     s.TotalMT,
                                     s.NoOfSilos,

                                     s.MillDetailsSuppliersName,
                                     s.YearInstalled,

                                     s.MillSectionHuller,
                                     s.MillSectionNos,
                                     s.MillSectionKW,
                                     s.MillSectionHullSeperate,
                                     s.MillSectionHullSeperateNos,
                                     s.PaddyTable,
                                     s.PaddyTableNos,
                                     s.ThinThickGrader,
                                     s.ThinThickGraderNos,
                                     s.ThinThickGraderDumps,

                                     s.Whitner,
                                     s.WhitnerNos,
                                     s.WhitnerKW,
                                     s.WhitnerRPM,

                                     s.Polisher,
                                     s.PolisherNos,
                                     s.PolisherKW,
                                     s.PolisherRPM,

                                     s.LengthGrader,
                                     s.LengthGraderNos,
                                     s.GradeSize1,
                                     s.GradeSize2,
                                     s.GradeSize3,
                                     s.GradeSize4,

                                     s.ColorSorter,
                                     s.ColorSorterMake,
                                     s.ColorSorterChannels,
                                     s.ColorSorterPrimary,
                                     s.ColorSorterSecondary,

                                     s.PackingMachine,
                                     s.PackingMachineMake,
                                     s.PackingMachineNos,
                                     s.PackingMachineKgs1,
                                     s.PackingMachineKgs2,
                                     s.PackingMachineBrandName,

                                     s.PlantAutomationTypeRelayOrPLCBased,
                                     s.PlantAutomationTypeRelayOrPLCBasedMake,

                                     s.CustomerRequirement1,
                                     s.CustomerRequirement2,
                                     s.CustomerRequirement3,
                                     s.CustomerRequirement4,
                                     s.CustomerRequirement5,

                                     s.CommentsActionExpected,
                                 });
                var dt1 = duplicate.ToList();
                int cnt1 = dt1.Count;
                int cnt2 = dt1.Count;

                foreach (var d in duplicate)
                {
                    //int cnt3 = cnt1++;
                    int sl = cnt1++ - cnt2;
                    DataRow dr = dts.NewRow();
                    dr[0] = sl + 1;
                    dr[1] = d.NameofCollector;
                    dr[2] = d.LeadDate.ToString("dd-MM-yyyy");
                    dr[3] = d.MillName;
                    dr[4] = d.AddressLine1 + ", " + d.AddressLine2;
                    dr[5] = d.City;
                    dr[6] = d.State;
                    dr[7] = d.OwnerName;
                    dr[8] = d.MobNo;
                    dr[9] = d.EmailId;
                    dr[10] = d.Isd + "-" + d.Std + "-" + d.TelNo;

                    if (d.EnquiryTypeExistingMill == true)
                    {
                        dr[11] = "Existing Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "New Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "Customer Service Upgrade";
                    }
                    else
                    {
                        dr[11] = "Not Mentioned";
                    }

                    if (d.TypeOfMillPaddytoRice == true)
                    {
                        dr[12] = "Paddy To Rice";
                    }
                    else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                    {
                        dr[12] = "Brown-Rice To White-Rice";
                    }
                    else if (d.TypeOfMillReprocess == true)
                    {
                        dr[12] = "Reprocess";
                    }
                    else
                    {
                        dr[12] = "Not Mentioned";
                    }
                    if (d.TypeOfPaddyProcessLongGrain == true)
                    {
                        dr[13] = "Long Grain";
                    }
                    else if (d.TypeOfPaddyProcessMediumGrain == true)
                    {
                        dr[13] = "Medium Grain";
                    }
                    else if (d.TypeOfPaddyProcessRountGrain == true)
                    {
                        dr[13] = "Round Grain";
                    }
                    else
                    {
                        dr[13] = "Not Mentioned";
                    }

                    if (d.ConditioningRaw == true)
                    {
                        dr[14] = "Raw";
                    }
                    else if (d.ConditioningSteamed == true)
                    {
                        dr[14] = "Steamed";
                    }
                    else if (d.ConditioningParBoiled == true)
                    {
                        dr[14] = "Par Boiled";
                    }
                    else
                    {
                        dr[14] = "Not Mentioned";
                    }

                    dr[15] = d.ConditioningParBoiledPaddyMoisture;
                    dr[16] = d.ConditioningParBoiledBulkDensity;

                    if (d.PaddyVarietyBasmati == true)
                    {
                        dr[17] = "Basmati";
                    }
                    else if (d.PaddyVarietyNonBasmati == true)
                    {
                        dr[17] = "Non-Basmati";
                    }
                    else
                    {
                        dr[17] = d.PaddyVarietyVarietyName;
                    }
                    dr[18] = d.EMDProcessingCapacityMillTPH;
                    dr[19] = d.EMDMillOperatingDetailsHoursPerDay;
                    dr[20] = d.PreCleaningCapacity;
                    dr[21] = d.TPH2025;
                    dr[22] = d.TPH4050;
                    dr[23] = d.OthersTPH;
                    dr[24] = d.TotalMT;
                    dr[25] = d.NoOfSilos;
                    dr[26] = d.MillDetailsSuppliersName;
                    dr[27] = d.YearInstalled;

                    dr[28] = d.MillSectionNos;
                    dr[29] = d.MillSectionKW;
                    dr[30] = d.MillSectionHullSeperateNos;
                    dr[31] = d.PaddyTableNos;
                    dr[32] = d.ThinThickGraderNos;
                    dr[33] = d.ThinThickGraderDumps;
                    dr[34] = d.WhitnerNos;
                    dr[35] = d.WhitnerKW;
                    dr[36] = d.WhitnerRPM;
                    dr[37] = d.PolisherNos;
                    dr[38] = d.PolisherKW;
                    dr[39] = d.PolisherRPM;
                    dr[40] = d.LengthGraderNos;
                    dr[41] = d.GradeSize1;
                    dr[42] = d.GradeSize2;
                    dr[43] = d.GradeSize3;
                    dr[44] = d.GradeSize4;
                    dr[45] = d.ColorSorterMake;
                    dr[46] = d.ColorSorterChannels;
                    dr[47] = d.ColorSorterPrimary;
                    dr[48] = d.ColorSorterSecondary;
                    dr[49] = d.PackingMachineMake;
                    dr[50] = d.PackingMachineNos;
                    dr[51] = d.PackingMachineKgs1;
                    dr[52] = d.PackingMachineKgs2;
                    dr[53] = d.PackingMachineBrandName;
                    dr[54] = d.PlantAutomationTypeRelayOrPLCBasedMake;
                    dr[55] = d.CustomerRequirement1;
                    dr[56] = d.CustomerRequirement2;
                    dr[57] = d.CustomerRequirement3;
                    dr[58] = d.CustomerRequirement4;
                    dr[59] = d.CustomerRequirement5;
                    dr[60] = d.CommentsActionExpected;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
                #endregion
            }
            else
            {
                //4
                #region
                DataTable dts = new DataTable();
                dts.Columns.Add("Slno", typeof(String));
                dts.Columns.Add("Name Of Collector", typeof(String));
                dts.Columns.Add("Lead Date", typeof(String));
                dts.Columns.Add("Mill Name", typeof(String));
                dts.Columns.Add("Address", typeof(String));
                dts.Columns.Add("City", typeof(String));
                dts.Columns.Add("State", typeof(String));
                dts.Columns.Add("Owner Name", typeof(String));
                dts.Columns.Add("Owner Contact No", typeof(String));
                dts.Columns.Add("Owner Email-ID", typeof(String));
                dts.Columns.Add("Owner Telephone No", typeof(String));
                dts.Columns.Add("Enquiry Type", typeof(String));
                dts.Columns.Add("Type of Mill", typeof(String));
                dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                dts.Columns.Add("Conditioning", typeof(String));
                dts.Columns.Add("Paddy Moisture(In %)", typeof(String));
                dts.Columns.Add("Bulk Density Kgs/M3", typeof(String));
                dts.Columns.Add("Paddy Variety", typeof(String));
                dts.Columns.Add("Process Capcity( in TPH)", typeof(String));
                dts.Columns.Add("Operating Details", typeof(String));
                dts.Columns.Add("Pre Cleaning Capacity", typeof(String));
                dts.Columns.Add("20-25 TPH", typeof(String));
                dts.Columns.Add("40-50 TPH", typeof(String));
                dts.Columns.Add("Others", typeof(String));
                dts.Columns.Add("Total (In MT)", typeof(String));
                dts.Columns.Add("Each X (In Silos)", typeof(String));
                dts.Columns.Add("Suppliers Name", typeof(String));
                dts.Columns.Add("Year Installed", typeof(String));
                dts.Columns.Add("Huller No", typeof(String));
                dts.Columns.Add("Huller KW", typeof(String));
                dts.Columns.Add("Hull Seperator (In No's)", typeof(String));
                dts.Columns.Add("Paddy Table(In No's)", typeof(String));
                dts.Columns.Add("Thin/Thick Grader (In No's)", typeof(String));
                dts.Columns.Add("Drum's Each", typeof(String));
                dts.Columns.Add("Whitner No", typeof(String));
                dts.Columns.Add("Whitner KW", typeof(String));
                dts.Columns.Add("Whitner RPM", typeof(String));
                dts.Columns.Add("Polisher No", typeof(String));
                dts.Columns.Add("Polisher KW", typeof(String));
                dts.Columns.Add("Polisher RPM", typeof(String));
                dts.Columns.Add("Length Grader (In No's)", typeof(String));
                dts.Columns.Add("Grade Size 1", typeof(String));
                dts.Columns.Add("Grade Size 2", typeof(String));
                dts.Columns.Add("Grade Size 3", typeof(String));
                dts.Columns.Add("Grade Size 4", typeof(String));
                dts.Columns.Add("Color Sorter Make", typeof(String));
                dts.Columns.Add("Color Sorter Channels", typeof(String));
                dts.Columns.Add("Color Sorter Primary", typeof(String));
                dts.Columns.Add("Color Sorter Secondary", typeof(String));
                dts.Columns.Add("Packing Machine Make", typeof(String));
                dts.Columns.Add("Packing Machine (In No's)", typeof(String));
                dts.Columns.Add("Pack Size 1 (In Kg)", typeof(String));
                dts.Columns.Add("Pack Size 2 (In Kg)", typeof(String));
                dts.Columns.Add("Brand Names", typeof(String));
                dts.Columns.Add("Plant Automation Type", typeof(String));
                dts.Columns.Add("Requirement 1", typeof(String));
                dts.Columns.Add("Requirement 2", typeof(String));
                dts.Columns.Add("Requirement 3", typeof(String));
                dts.Columns.Add("Requirement 4", typeof(String));
                dts.Columns.Add("Requirement 5", typeof(String));
                dts.Columns.Add("Comments / Action Expected", typeof(String));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                var duplicate = (from s in db.LeadEnquiryRevised
                                     //where s.CPID == channame
                                 select new
                                 {
                                     s.NameofCollector,
                                     s.LeadDate,
                                     s.MillName,
                                     s.AddressLine1,
                                     s.AddressLine2,
                                     s.City,
                                     s.Pincode,
                                     s.State,
                                     s.OwnerName,
                                     s.MobNo,
                                     s.EmailId,
                                     s.Isd,
                                     s.Std,
                                     s.TelNo,

                                     s.EnquiryTypeExistingMill,
                                     s.EnquiryTypeNewMill,
                                     s.EnquiryTypeCustomerServiceUpgrade,

                                     s.TypeOfMillPaddytoRice,
                                     s.TypeOfMillBrownRiceToWhiteRice,
                                     s.TypeOfMillReprocess,

                                     s.TypeOfPaddyProcessLongGrain,
                                     s.TypeOfPaddyProcessMediumGrain,
                                     s.TypeOfPaddyProcessRountGrain,

                                     s.ConditioningRaw,
                                     s.ConditioningSteamed,
                                     s.ConditioningParBoiled,
                                     s.ConditioningParBoiledPaddyMoisture,
                                     s.ConditioningParBoiledBulkDensity,

                                     s.PaddyVarietyBasmati,
                                     s.PaddyVarietyNonBasmati,
                                     s.PaddyVarietyVarietyName,

                                     s.EMDProcessingCapacityMill,
                                     s.EMDProcessingCapacityMillTPH,
                                     s.EMDMillOperatingDetailsHoursPerDay,

                                     s.PreCleaningCapacity,
                                     s.TPH2025,
                                     s.TPH4050,
                                     s.Others,
                                     s.OthersTPH,
                                     s.PaddyStorageMTTotal,
                                     s.TotalMT,
                                     s.NoOfSilos,

                                     s.MillDetailsSuppliersName,
                                     s.YearInstalled,

                                     s.MillSectionHuller,
                                     s.MillSectionNos,
                                     s.MillSectionKW,
                                     s.MillSectionHullSeperate,
                                     s.MillSectionHullSeperateNos,
                                     s.PaddyTable,
                                     s.PaddyTableNos,
                                     s.ThinThickGrader,
                                     s.ThinThickGraderNos,
                                     s.ThinThickGraderDumps,

                                     s.Whitner,
                                     s.WhitnerNos,
                                     s.WhitnerKW,
                                     s.WhitnerRPM,

                                     s.Polisher,
                                     s.PolisherNos,
                                     s.PolisherKW,
                                     s.PolisherRPM,

                                     s.LengthGrader,
                                     s.LengthGraderNos,
                                     s.GradeSize1,
                                     s.GradeSize2,
                                     s.GradeSize3,
                                     s.GradeSize4,

                                     s.ColorSorter,
                                     s.ColorSorterMake,
                                     s.ColorSorterChannels,
                                     s.ColorSorterPrimary,
                                     s.ColorSorterSecondary,

                                     s.PackingMachine,
                                     s.PackingMachineMake,
                                     s.PackingMachineNos,
                                     s.PackingMachineKgs1,
                                     s.PackingMachineKgs2,
                                     s.PackingMachineBrandName,

                                     s.PlantAutomationTypeRelayOrPLCBased,
                                     s.PlantAutomationTypeRelayOrPLCBasedMake,

                                     s.CustomerRequirement1,
                                     s.CustomerRequirement2,
                                     s.CustomerRequirement3,
                                     s.CustomerRequirement4,
                                     s.CustomerRequirement5,

                                     s.CommentsActionExpected,
                                 });
                var dt1 = duplicate.ToList();
                int cnt1 = dt1.Count;
                int cnt2 = dt1.Count;

                foreach (var d in duplicate)
                {
                    //int cnt3 = cnt1++;
                    int sl = cnt1++ - cnt2;
                    DataRow dr = dts.NewRow();
                    dr[0] = sl + 1;
                    dr[1] = d.NameofCollector;
                    dr[2] = d.LeadDate.ToString("dd-MM-yyyy");
                    dr[3] = d.MillName;
                    dr[4] = d.AddressLine1 + ", " + d.AddressLine2;
                    dr[5] = d.City;
                    dr[6] = d.State;
                    dr[7] = d.OwnerName;
                    dr[8] = d.MobNo;
                    dr[9] = d.EmailId;
                    dr[10] = d.Isd + "-" + d.Std + "-" + d.TelNo;

                    if (d.EnquiryTypeExistingMill == true)
                    {
                        dr[11] = "Existing Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "New Mill";
                    }
                    else if (d.EnquiryTypeNewMill == true)
                    {
                        dr[11] = "Customer Service Upgrade";
                    }
                    else
                    {
                        dr[11] = "Not Mentioned";
                    }

                    if (d.TypeOfMillPaddytoRice == true)
                    {
                        dr[12] = "Paddy To Rice";
                    }
                    else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                    {
                        dr[12] = "Brown-Rice To White-Rice";
                    }
                    else if (d.TypeOfMillReprocess == true)
                    {
                        dr[12] = "Reprocess";
                    }
                    else
                    {
                        dr[12] = "Not Mentioned";
                    }
                    if (d.TypeOfPaddyProcessLongGrain == true)
                    {
                        dr[13] = "Long Grain";
                    }
                    else if (d.TypeOfPaddyProcessMediumGrain == true)
                    {
                        dr[13] = "Medium Grain";
                    }
                    else if (d.TypeOfPaddyProcessRountGrain == true)
                    {
                        dr[13] = "Round Grain";
                    }
                    else
                    {
                        dr[13] = "Not Mentioned";
                    }

                    if (d.ConditioningRaw == true)
                    {
                        dr[14] = "Raw";
                    }
                    else if (d.ConditioningSteamed == true)
                    {
                        dr[14] = "Steamed";
                    }
                    else if (d.ConditioningParBoiled == true)
                    {
                        dr[14] = "Par Boiled";
                    }
                    else
                    {
                        dr[14] = "Not Mentioned";
                    }

                    dr[15] = d.ConditioningParBoiledPaddyMoisture;
                    dr[16] = d.ConditioningParBoiledBulkDensity;

                    if (d.PaddyVarietyBasmati == true)
                    {
                        dr[17] = "Basmati";
                    }
                    else if (d.PaddyVarietyNonBasmati == true)
                    {
                        dr[17] = "Non-Basmati";
                    }
                    else
                    {
                        dr[17] = d.PaddyVarietyVarietyName;
                    }
                    dr[18] = d.EMDProcessingCapacityMillTPH;
                    dr[19] = d.EMDMillOperatingDetailsHoursPerDay;
                    dr[20] = d.PreCleaningCapacity;
                    dr[21] = d.TPH2025;
                    dr[22] = d.TPH4050;
                    dr[23] = d.OthersTPH;
                    dr[24] = d.TotalMT;
                    dr[25] = d.NoOfSilos;
                    dr[26] = d.MillDetailsSuppliersName;
                    dr[27] = d.YearInstalled;

                    dr[28] = d.MillSectionNos;
                    dr[29] = d.MillSectionKW;
                    dr[30] = d.MillSectionHullSeperateNos;
                    dr[31] = d.PaddyTableNos;
                    dr[32] = d.ThinThickGraderNos;
                    dr[33] = d.ThinThickGraderDumps;
                    dr[34] = d.WhitnerNos;
                    dr[35] = d.WhitnerKW;
                    dr[36] = d.WhitnerRPM;
                    dr[37] = d.PolisherNos;
                    dr[38] = d.PolisherKW;
                    dr[39] = d.PolisherRPM;
                    dr[40] = d.LengthGraderNos;
                    dr[41] = d.GradeSize1;
                    dr[42] = d.GradeSize2;
                    dr[43] = d.GradeSize3;
                    dr[44] = d.GradeSize4;
                    dr[45] = d.ColorSorterMake;
                    dr[46] = d.ColorSorterChannels;
                    dr[47] = d.ColorSorterPrimary;
                    dr[48] = d.ColorSorterSecondary;
                    dr[49] = d.PackingMachineMake;
                    dr[50] = d.PackingMachineNos;
                    dr[51] = d.PackingMachineKgs1;
                    dr[52] = d.PackingMachineKgs2;
                    dr[53] = d.PackingMachineBrandName;
                    dr[54] = d.PlantAutomationTypeRelayOrPLCBasedMake;
                    dr[55] = d.CustomerRequirement1;
                    dr[56] = d.CustomerRequirement2;
                    dr[57] = d.CustomerRequirement3;
                    dr[58] = d.CustomerRequirement4;
                    dr[59] = d.CustomerRequirement5;
                    dr[60] = d.CommentsActionExpected;

                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
                #endregion
            }
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=YJTLeadReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("YJTLeadReport");
        }

        //new YJT lead report
        public ActionResult YJTLeadReportNew(string cpnam = null, string frommonth = null, string tomonth = null)
        {
            //string todayDate = DateTime.Now.ToShortDateString();
            DateTime todaydate = DateTime.Today;
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID, m.ZoneID }).SingleOrDefault();

            var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
            var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

            ViewBag.fromdte = frommonth;
            ViewBag.todate = tomonth;
            ViewBag.OrganizationName = cpnam;

            var test = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LERID).ToList();

            if (User.IsInRole("Administrator"))
            {
                #region
                var oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0)
                            .Where(m => m.LeadDate.Year == todaydate.Year && m.LeadDate.Month == todaydate.Month)
                            .OrderByDescending(m => m.LERID).ToList();

                if (cpnam != null && cpnam != "" && (frommonth == null && frommonth == "" && tomonth == null && tomonth == ""))
                {
                    ViewBag.IsSearch = true;

                    oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID)
                                .Where(m => m.IsDeleted == 0).
                                  Where(m => m.LeadDate.Year == todaydate.Year
                                  && m.LeadDate.Month == todaydate.Month)
                                 .OrderByDescending(m => m.LERID).ToList();


                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.LeadDate.Year == todaydate.Year
                                      && ld.LeadDate.Month == todaydate.Month
                                      orderby (ld.LERID)
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    if (test == null)
                    {
                        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    var cpid = chpart.CPID;
                    return View(test);
                }

                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && (cpnam == null || cpnam == ""))
                {
                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);

                    if (toda < frda)
                    {
                        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                        return View();
                    }
                    /////////
                    oagendata = (from ld in db.LeadEnquiryRevised
                                 where ld.IsDeleted == 0 && (ld.LeadDate >= frda && ld.LeadDate <= toda)
                                 select ld).ToList();
                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.IsDeleted == 0 && (le.FollowUpDate >= frda && le.FollowUpDate <= toda)
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    // oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                    if (test.Count == 0)
                    {
                        TempData["noexpmonth"] = "No Data Exists for the selected value";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    return View(test);
                }

                if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (toda < frda)
                    {
                        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                        return View();
                    }

                    oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.IsDeleted == 0 && (le.CPID == chpart.CPID && le.FollowUpDate >= frda && le.FollowUpDate <= toda)
                                      orderby (ld.LERID)
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    if (test.Count == 0)
                    {
                        TempData["noexpmonth"] = "No Data Exists for the selected value";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    return View(test);
                }

                if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    ViewBag.IsSearch = true;

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (toda < frda)
                    {
                        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                        return View();
                    }

                    chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

                    oagendata = (from s in db.LeadEnquiryRevised
                                 where s.LeadDate >= frda && s.LeadDate <= toda
                                 where s.CPID == channame
                                 select s).ToList();
                    //oagendata = db.LeadEnquiryRevised
                    //    .Where(m => m.CPID == chpart.CPID)
                    //    .Where(m => m.LeadDate >= frda && m.LeadDate <= toda)
                    //    .OrderByDescending(m => m.LERID).ToList();

                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.LeadDate >= frda && ld.LeadDate <= toda
                                      where ld.CPID == channame
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    if (test == null)
                    {
                        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    var cpid = chpart.CPID;
                }
                return View(test);
                #endregion
            }
            else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
            {
                #region
                test = db.LeadEnquiryRevised
                    .Where(m => m.IsDeleted == 0)
                    .Where(m => m.CPID == loginname.CPID)
                    .Where(m => m.LeadDate.Year == todaydate.Year
                        && m.LeadDate.Month == todaydate.Month)
                    .OrderByDescending(m => m.LERID).ToList();

                //if (cpnam != null && cpnam != "")
                //{
                //    ViewBag.IsSearch = true;

                //    oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID).Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LERID).ToList();
                //    if (chpart == null)
                //    {
                //        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                //        return RedirectToAction("YJTLeadReportNew", "Reports");
                //    }
                //    var cpid = chpart.CPID;
                //}

                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    ViewBag.IsSearch = true;

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (toda < frda)
                    {
                        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                        return View();
                    }

                    var oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == loginname.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.IsDeleted == 0 && (ld.CPID == loginname.CPID && ld.LeadDate >= frda && ld.LeadDate <= toda)
                                      orderby (ld.LERID)
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    foreach (var d in test)
                    {
                        if (d.LERID != null)
                        {
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;
                            for (int c = 0; c < LFCount; c++)
                            {
                                var temp = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).ToList();
                                oagendata = oagendata.Concat(temp).ToList();
                            }
                        }
                    }
                    if (oagendata.Count == 0)
                    {
                        TempData["noexpmonth"] = "No Data Exists for the selected value";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    return View(test);
                }

                //if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                //{
                //    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                //    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                //    DateTime frda = DateTime.Parse(frda1).Date;
                //    DateTime toda = DateTime.Parse(toda1).Date;

                //    if (toda < frda)
                //    {
                //        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                //        return View();
                //    }

                //    oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                //    if (oagendata.Count == 0)
                //    {
                //        TempData["noexpmonth"] = "No Data Exists for the selected value";
                //        return RedirectToAction("YJTLeadReportNew", "Reports");
                //    }
                //}

                //if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                //{
                //    ViewBag.IsSearch = true;

                //    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                //    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                //    DateTime frda = DateTime.Parse(frda1).Date;
                //    DateTime toda = DateTime.Parse(toda1).Date;

                //    if (toda < frda)
                //    {
                //        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                //        return View();
                //    }

                //    chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

                //    oagendata = (from s in db.LeadEnquiryRevised
                //                 where s.LeadDate >= frda && s.LeadDate <= toda
                //                 where s.CPID == channame
                //                 select s).ToList();
                //    oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();
                //    if (chpart == null)
                //    {
                //        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                //        return RedirectToAction("YJTLeadReportNew", "Reports");
                //    }
                //    var cpid = chpart.CPID;
                //}
                return View(test);
                #endregion
            }
            else if (User.IsInRole("ZonalManager"))
            {
                #region
                //oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LERID).ToList();

                test = (from s in db.LeadEnquiryRevised
                        from b in db.ChannelPartners
                        where s.IsDeleted == 0 && s.CPID == b.CPID && b.ZoneID == loginname.ZoneID &&
                        s.LeadDate.Year == todaydate.Year && s.LeadDate.Month == todaydate.Month
                        select s).ToList();

                //var sales = (from s in db.Handover
                //             from b in db.ChannelPartners
                //             where s.Handeddate >= frda && s.Handeddate <= toda && s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                //             select s).ToList();

                if (cpnam != null && cpnam != "")
                {
                    ViewBag.IsSearch = true;

                    //oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID).Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LERID).ToList();

                    var oagendata = (from s in db.LeadEnquiryRevised
                                     where s.IsDeleted == 0 && s.CPID == chpart.CPID
                                     select s).ToList();

                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.IsDeleted == 0 && ld.CPID == chpart.CPID
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    if (test == null)
                    {
                        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    var cpid = chpart.CPID;
                    return View(test);
                }

                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (toda < frda)
                    {
                        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                        return View();
                    }

                    //oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                    var oagendata = (from s in db.LeadEnquiryRevised
                                     from b in db.ChannelPartners
                                     where s.LeadDate >= frda && s.LeadDate <= toda && s.IsDeleted == 0 && s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                                     select s).ToList();

                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join cp in db.ChannelPartners on ld.LERID equals cp.CPID
                                      where ld.LeadDate >= frda && ld.LeadDate <= toda && ld.IsDeleted == 0 && ld.CPID == cp.CPID && cp.ZoneID == loginname.ZoneID
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    if (test.Count == 0)
                    {
                        TempData["noexpmonth"] = "No Data Exists for the selected value";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    return View(test);
                }

                if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (toda < frda)
                    {
                        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                        return View();
                    }

                    //oagendata = db.LeadEnquiryRevised.Where(m => m.IsDeleted == 0).Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                    var oagendata = (from s in db.LeadEnquiryRevised
                                     where s.LeadDate >= frda && s.LeadDate <= toda && s.IsDeleted == 0 && s.CPID == chpart.CPID
                                     select s).ToList();

                    var oagendata2 = (from ld in db.LeadEnquiryRevised
                                      join le in db.LeadFollowUptbl on ld.LERID equals le.LERID
                                      where ld.LeadDate >= frda && ld.LeadDate <= toda && ld.IsDeleted == 0 && ld.CPID == chpart.CPID
                                      select ld).ToList();

                    test = oagendata.Concat(oagendata2).ToList();

                    if (test.Count == 0)
                    {
                        TempData["noexpmonth"] = "No Data Exists for the selected value";
                        return RedirectToAction("YJTLeadReportNew", "Reports");
                    }
                    return View(test);
                }

                //if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                //{
                //    ViewBag.IsSearch = true;

                //    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                //    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                //    DateTime frda = DateTime.Parse(frda1).Date;
                //    DateTime toda = DateTime.Parse(toda1).Date;

                //    if (toda < frda)
                //    {
                //        TempData["NoData"] = "To Date Should Be Greater than From Date!!!!";
                //        return View();
                //    }

                //    chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();

                //    oagendata = db.LeadEnquiryRevised.Where(m => m.CPID == chpart.CPID).Where(m => m.LeadDate >= frda && m.LeadDate <= toda).OrderByDescending(m => m.LERID).ToList();

                //    oagendata = (from s in db.LeadEnquiryRevised
                //                 from b in db.ChannelPartners
                //                 where s.LeadDate >= frda && s.LeadDate <= toda && s.IsDeleted == 0 && s.CPID == chpart.CPID
                //                 select s).ToList();

                //    if (chpart == null)
                //    {
                //        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                //        return RedirectToAction("YJTLeadReportNew", "Reports");
                //    }
                //    var cpid = chpart.CPID;
                //}
                #endregion    
            }
            return View(test);
        }

        public ActionResult ExportDataYJTLeadNew(string cpnam = null, string frommonth = null, string tomonth = null)
        {
            GridView gv = new GridView();

            if (User.IsInRole("Administrator"))
            {
                #region

                if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    //1
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));


                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));
                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");
                    String chname = cpnam;

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");


                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@frda", frda);
                    cmd.Parameters.AddWithValue("@toda", toda);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;//

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;//

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;//

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;//

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;//

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;

                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 500;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        try
                                                        {
                                                            dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                        }
                                                        catch
                                                        {
                                                            dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                                        }

                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        try
                                                        {
                                                            dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                        }
                                                        catch
                                                        {
                                                            dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                                        }

                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }
                        //dr[43] =d.WhitnerYear;
                        //dr[44] = d.HullerYear;
                        //dr[45] = d.PolisherYear;
                        //dr[46] = d.ColorSorter1Year;
                        //dr[47] = d.ColorSorter2Year;



                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else if (cpnam != null && cpnam != "" && (frommonth == null && frommonth == "" && tomonth == null && tomonth == ""))
                {
                    //2
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));

                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String));
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cpid", 0);
                    cmd.Parameters.AddWithValue("@frda", null);
                    cmd.Parameters.AddWithValue("@toda", null);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;


                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    //if (dd >= frda && dd <= toda)
                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }


                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && (cpnam == null || cpnam == ""))
                {
                    //3
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));

                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));



                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    int cpid = 0;
                    //////////////////////////////////
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", frda);
                    cmd.Parameters.AddWithValue("@toda", toda);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();
                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();
                    ///////////////////////////

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;
                    // int count = 1;
                    foreach (var d in oagendata)
                    {
                        //count++;
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;



                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;

                        //////////////////////////////////////////
                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        try
                                                        {
                                                            dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                        }
                                                        catch
                                                        {
                                                            if (!dts.Columns.Contains("Follow Up Date"))
                                                            {
                                                                dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                                                            }
                                                            dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                        }

                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        try
                                                        {
                                                            dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                        }
                                                        catch
                                                        {
                                                            if (!dts.Columns.Contains("Follow Up Details"))
                                                            {
                                                                dts.Columns.Add("Follow Up Details", typeof(String)); //New Added
                                                            }
                                                            dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                        }
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }

                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }



                        dts.Rows.Add(dr);


                    }
                    //int a = count;
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else
                {
                    //4
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Now;
                    DateTime toda = DateTime.Now;
                    DateTime ddobj = DateTime.Now;
                    int cpid = 0;

                    if ((frda1 == "" || frda1 == null) && (toda1 == "" || toda1 == null))
                    {
                        frda = ddobj.AddYears(-10);
                        toda = ddobj;
                    }
                    else if ((frda1 == "" || frda1 == null) && (toda1 != "" || toda1 != null))
                    {
                        frda = ddobj.AddYears(-1);
                        toda = DateTime.Parse(toda1).Date;

                    }
                    else if ((frda1 != "" || frda1 != null) && (toda1 == "" || toda1 == null))
                    {
                        frda = DateTime.Parse(frda1).Date;
                        toda = ddobj;
                    }
                    else
                    {
                        frda = DateTime.Parse(frda1).Date;
                        toda = DateTime.Parse(toda1).Date;
                    }
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", frda);
                    cmd.Parameters.AddWithValue("@toda", toda);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }

                        dr[0] = sl + 1;


                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;


                        StringBuilder sb = new StringBuilder();
                        string lfdetails = null;

                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    //if (dd >= frda && dd <= toda)
                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                #endregion
            }
            else if (User.IsInRole("ChannelPartnerAdmin") || User.IsInRole("ChannelPartnerUser"))
            {
                #region
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();


                if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    //1
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added


                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    toda = toda.AddHours(23).AddMinutes(59).AddSeconds(59);
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", frda);
                    cmd.Parameters.AddWithValue("@toda", toda);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();
                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;


                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;

                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 48;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }

                                        else if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up" + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else
                {
                    //2
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    //String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    //String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    //DateTime frda = DateTime.Parse(frda1).Date;
                    //DateTime toda = DateTime.Parse(toda1).Date;
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", null);
                    cmd.Parameters.AddWithValue("@toda", null);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }

                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;



                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    //if (dd >= frda && dd <= toda)
                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }


                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                #endregion
            }
            else if (User.IsInRole("ZonalManager"))
            {
                #region
                var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();


                if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    //1
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added


                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");
                    String chname = cpnam;

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", frda);
                    cmd.Parameters.AddWithValue("@toda", toda);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow
                            dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }

                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;



                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }


                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else if (cpnam != null && cpnam != "")
                {
                    String frda1 = null;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = null;

                    //DateTime frda = DateTime.Parse(frda1).Date;
                    //DateTime toda = DateTime.Parse(toda1).Date;
                    //2
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    String chname = cpnam;
                    //String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    //String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    //DateTime frda = DateTime.Parse(frda1).Date;
                    //DateTime toda = DateTime.Parse(toda1).Date;
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    cmd.Parameters.AddWithValue("@frda", string.Empty);
                    cmd.Parameters.AddWithValue("@toda", string.Empty);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;



                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    //if (dd >= frda && dd <= toda)
                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }


                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
                {
                    //3
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added



                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    DateTime frda = DateTime.Parse(frda1).Date;
                    DateTime toda = DateTime.Parse(toda1).Date;
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", frda);
                    cmd.Parameters.AddWithValue("@toda", toda);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();

                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();
                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;


                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;



                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }


                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                else
                {
                    //4
                    #region
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Slno", typeof(String));
                    dts.Columns.Add("Lead Type", typeof(String));
                    dts.Columns.Add("Commodity Type", typeof(String));

                    dts.Columns.Add("Channel Partner", typeof(String));
                    dts.Columns.Add("Name Of Collector", typeof(String));
                    dts.Columns.Add("Lead Date", typeof(String));
                    dts.Columns.Add("Mill Name", typeof(String));
                    dts.Columns.Add("Address Line1", typeof(String));
                    dts.Columns.Add("Address Line2", typeof(String));
                    dts.Columns.Add("City", typeof(String));
                    dts.Columns.Add("State", typeof(String));
                    dts.Columns.Add("Country", typeof(String));
                    dts.Columns.Add("Owner Name", typeof(String));
                    dts.Columns.Add("Owner Contact No", typeof(String));
                    dts.Columns.Add("Owner Email-ID", typeof(String));

                    dts.Columns.Add("Enquiry Type", typeof(String));
                    dts.Columns.Add("Type of Mill", typeof(String));
                    dts.Columns.Add("Type to Paddy to be Process", typeof(String));
                    dts.Columns.Add("Conditioning", typeof(String));
                    dts.Columns.Add("Paddy Variety", typeof(String));
                    dts.Columns.Add("Operating Details", typeof(String));
                    dts.Columns.Add("Suppliers Name", typeof(String));
                    dts.Columns.Add("Year Installed", typeof(String));

                    dts.Columns.Add("Whitner No", typeof(String));
                    dts.Columns.Add("Whitner KW", typeof(String));
                    dts.Columns.Add("Whitner Supplier Name", typeof(String));
                    dts.Columns.Add("Whitner Installed Year", typeof(String));//New Added on 15-04-2017

                    dts.Columns.Add("Huller No", typeof(String));
                    dts.Columns.Add("Huller KW", typeof(String));
                    dts.Columns.Add("Huller Supplier Name", typeof(String));
                    dts.Columns.Add("Huller Installed Year", typeof(String));

                    dts.Columns.Add("Polisher No", typeof(String));
                    dts.Columns.Add("Polisher KW", typeof(String));
                    dts.Columns.Add("Polisher Supplier Name", typeof(String));
                    dts.Columns.Add("Polisher Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make", typeof(String));
                    dts.Columns.Add("Color Sorter Channels", typeof(String));
                    dts.Columns.Add("ColorSorter1 Installed Year", typeof(String));

                    dts.Columns.Add("Color Sorter Make-2", typeof(String));
                    dts.Columns.Add("Color Sorter Channels-2", typeof(String));
                    dts.Columns.Add("ColorSorter2 Installed Year", typeof(String));
                    //new added
                    dts.Columns.Add("Customer Requirement", typeof(String));
                    dts.Columns.Add("Capacity", typeof(String));
                    dts.Columns.Add("Type Of Req / Machines", typeof(String));
                    dts.Columns.Add("Machines", typeof(String));
                    dts.Columns.Add("Follow Up", typeof(String));
                    dts.Columns.Add("Event Handled By", typeof(String));
                    dts.Columns.Add("Comments / Action Expected", typeof(String));

                    dts.Columns.Add("Sorter Model", typeof(String));
                    dts.Columns.Add("Quotes To Be Submitted", typeof(String));

                    //dts.Columns.Add("Follow Up Date", typeof(String)); //New Added
                    //dts.Columns.Add("Follow Up Details", typeof(String)); //New Added

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();

                    //String frda1 = frommonth;// dtt.ToString("yyyy-MM-dd");
                    //String toda1 = tomonth;// dtt1.ToString("yyyy-MM-dd");

                    //DateTime frda = DateTime.Parse(frda1).Date;
                    //DateTime toda = DateTime.Parse(toda1).Date;
                    int cpid = 0;

                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    DataTable dt = new DataTable();
                    SqlConnection con = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("oagendatapush", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@frda", DBNull.Value);
                    cmd.Parameters.AddWithValue("@toda", DBNull.Value);
                    cmd.Parameters.AddWithValue("@cpid", cpid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();


                    var oagendata = db.LeadEnquiryRevisedTemp.ToList();

                    var dt1 = oagendata.ToList();
                    int cnt1 = dt1.Count;
                    int cnt2 = dt1.Count;

                    foreach (var d in oagendata)
                    {
                        //int cnt3 = cnt1++;
                        int sl = cnt1++ - cnt2;
                        DataRow dr = dts.NewRow();
                        cpid = d.CPID;
                        var cpname = db.ChannelPartners.Where(m => m.CPID == 9999).Select(m => m.CPName).SingleOrDefault();
                        if (cpid != 0)
                        {
                            cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                        }
                        dr[0] = sl + 1;

                        dr[2] = d.CommodityType;
                        if (cpname != null)
                        {
                            dr[3] = cpname;
                        }
                        else
                        {
                            dr[3] = "Admin";
                        }

                        dr[4] = d.NameofCollector;
                        dr[5] = d.LeadDate.ToString("dd-MM-yyyy");
                        dr[6] = d.MillName;
                        dr[7] = d.AddressLine1;
                        dr[8] = d.AddressLine2;
                        dr[9] = d.City;
                        dr[10] = d.State;
                        dr[11] = d.Country;
                        dr[12] = d.OwnerName;
                        dr[13] = d.MobNo;
                        dr[14] = d.EmailId;

                        if (d.EnquiryTypeExistingMill == true)
                        {
                            dr[15] = "Existing Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "New Mill";
                        }
                        else if (d.EnquiryTypeNewMill == true)
                        {
                            dr[15] = "Customer Service Upgrade";
                        }
                        else
                        {
                            dr[15] = "Not Mentioned";
                        }

                        if (d.TypeOfMillPaddytoRice == true)
                        {
                            dr[16] = "Paddy To Rice";
                        }
                        else if (d.TypeOfMillBrownRiceToWhiteRice == true)
                        {
                            dr[16] = "Brown-Rice To White-Rice";
                        }
                        else if (d.TypeOfMillReprocess == true)
                        {
                            dr[16] = "Reprocess";
                        }
                        else
                        {
                            dr[16] = "Not Mentioned";
                        }

                        if (d.TypeOfPaddyProcessLongGrain == true)
                        {
                            dr[17] = "Long Grain";
                        }
                        else if (d.TypeOfPaddyProcessMediumGrain == true)
                        {
                            dr[17] = "Medium Grain";
                        }
                        else if (d.TypeOfPaddyProcessRountGrain == true)
                        {
                            dr[17] = "Round Grain";
                        }
                        else
                        {
                            dr[17] = "Not Mentioned";
                        }

                        if (d.ConditioningRaw == true)
                        {
                            dr[18] = "Raw";
                        }
                        else if (d.ConditioningSteamed == true)
                        {
                            dr[18] = "Steamed";
                        }
                        else if (d.ConditioningParBoiled == true)
                        {
                            dr[18] = "Par Boiled";
                        }
                        else
                        {
                            dr[18] = "Not Mentioned";
                        }

                        if (d.PaddyVarietyBasmati == true)
                        {
                            dr[19] = "Basmati";
                        }
                        else if (d.PaddyVarietyNonBasmati == true)
                        {
                            dr[19] = "Non-Basmati";
                        }
                        else
                        {
                            dr[19] = "NA";
                        }

                        dr[20] = d.EMDMillOperatingDetailsHoursPerDay;
                        dr[21] = d.MillDetailsSuppliersName;
                        dr[22] = d.YearInstalled;

                        dr[23] = d.WhitnerNos;
                        dr[24] = d.WhitnerKW;
                        dr[25] = d.WhitenerSupplierName;
                        dr[26] = d.WhitnerYear;

                        dr[27] = d.HullerNos;
                        dr[28] = d.HullerKW;
                        dr[29] = d.HullerSupplierName;
                        dr[30] = d.HullerYear;

                        dr[31] = d.PolisherNos;
                        dr[32] = d.PolisherKW;
                        dr[33] = d.PolisherSupplierName;
                        dr[34] = d.PolisherYear;

                        dr[35] = d.ColorSorterMake;
                        dr[36] = d.ColorSorterChannels;
                        dr[37] = d.ColorSorter1Year;

                        dr[38] = d.ColorSorterMake1;
                        dr[39] = d.ColorSorterChannels1;
                        dr[40] = d.ColorSorter2Year;

                        //new added
                        dr[41] = d.CustReqYeorsNo;
                        dr[42] = d.Capacity;
                        dr[43] = d.TypeOfReq;
                        if (d.TypeOfReq == "Machines")
                        {
                            if (d.MachineClassifier == true)
                            {
                                dr[44] = "Classifier";
                            }
                            if (d.MachineDestoner == true)
                            {
                                dr[44] = dr[44] + "," + "Destoner";
                            }
                            if (d.MachineHullerSeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Huller Seperator";
                            }
                            if (d.MachinePaddySeperator == true)
                            {
                                dr[44] = dr[44] + "," + "Paddy Seperator";
                            }
                            if (d.MachineThickThinGrader == true)
                            {
                                dr[44] = dr[44] + "," + "Thick/Thin Grader";
                            }
                            if (d.MachineWhitner == true)
                            {
                                dr[44] = dr[44] + "," + "Whitner";
                            }
                            if (d.MachinePolisher == true)
                            {
                                dr[44] = dr[44] + "," + "Polisher";
                            }
                            if (d.MachineSorter == true)
                            {
                                dr[44] = dr[44] + "," + "Sorter";
                            }
                        }
                        dr[45] = d.TimeFrame;
                        dr[46] = d.EnquiryHandledBy;
                        dr[47] = d.CommentsActionExpected;

                        dr[48] = d.SorterModel;
                        dr[49] = d.QuotesToBeSubmitted;

                        if (d.LERID != null)
                        {
                            string leadtype = d.leadtype;
                            int LFCount = 0; int i = 1;
                            var LeadFollowUpDetails = db.LeadFollowUptbl.Where(m => m.IsDeleted == 0).Where(m => m.LERID == d.LERID).Select(m => new { m.FollowUpType, m.FollowUpDate, m.FollowUpDescription }).ToList();
                            LFCount = LeadFollowUpDetails.Count;

                            if (leadtype == "followup")
                            {
                                int dynamicIndex = 50;

                                foreach (var lf in LeadFollowUpDetails)
                                {
                                    DateTime dd = Convert.ToDateTime(lf.FollowUpDate);
                                    string dd1 = dd.ToString("dd-MM-yyyy");

                                    //if (dd >= frda && dd <= toda)
                                    if (leadtype == "followup")
                                    {
                                        if (LeadFollowUpDetails.Count == 1)
                                        {

                                            if (!dts.Columns.Contains("Follow Up Date 1"))
                                            {
                                                dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                            }
                                            if (!dts.Columns.Contains("Follow Up Details 1"))
                                            {
                                                dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                            }

                                            dr[50] = i + ". " + dd1;
                                            dr[51] = i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                        }


                                        else
                                            if (LeadFollowUpDetails.Count > 1)
                                        {
                                            //int dynamicIndex = 50;
                                            for (int j = 1; j <= LeadFollowUpDetails.Count; j++)
                                            {
                                                if (i == 1)
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date 1"))
                                                    {
                                                        try
                                                        {
                                                            dts.Columns["Follow Up Date"].ColumnName = "Follow Up Date " + j;
                                                        }
                                                        catch
                                                        {
                                                            dts.Columns.Add("Follow Up Date 1", typeof(String)); //New Added
                                                        }
                                                    }

                                                    if (!dts.Columns.Contains("Follow Up Details 1"))
                                                    {
                                                        try
                                                        {
                                                            dts.Columns["Follow Up Details"].ColumnName = "Follow Up Details " + j;
                                                        }
                                                        catch
                                                        {
                                                            dts.Columns.Add("Follow Up Details 1", typeof(String)); //New Added
                                                        }

                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }

                                                else
                                                {
                                                    if (!dts.Columns.Contains("Follow Up Date " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Date " + i, typeof(String)); //New Added
                                                    }
                                                    if (!dts.Columns.Contains("Follow Up Details " + i))
                                                    {
                                                        dts.Columns.Add("Follow Up Details " + i, typeof(String)); //New Added
                                                    }

                                                    dr[dynamicIndex] = "\r\n" + "" + i + ". " + dd1;
                                                    dr[dynamicIndex + 1] = "\r\n" + "" + i + ". " + lf.FollowUpType + "-->" + lf.FollowUpDescription;
                                                    dynamicIndex++;
                                                    dynamicIndex++;
                                                    break;
                                                }


                                            }

                                        }

                                        dr[1] = "Follow Up" + "(" + LeadFollowUpDetails.Count + ")";

                                        i++;
                                    }
                                    else
                                    {
                                        dr[1] = "New";
                                    }
                                }
                            }
                            else
                            {
                                dr[1] = "New";
                            }
                        }

                        //dr[50] = d.SorterModel;
                        //dr[51] = d.QuotesToBeSubmitted;

                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                    db.Database.ExecuteSqlCommand("truncate table  [dbo].[LeadEnquiryRevisedTemp]");

                    #endregion
                }
                #endregion
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=RCSingleMachineReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("YJTLeadReportNew");
        }

        /////////////  OCM Working  /////////////
        #region

        public ActionResult OCMPolisherRep(int? roaid, int? roatbid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            if (roaid != null)
            {
                var start = db.OCMPolisherReport.ToList();
                foreach (var se in start)
                {
                    db.OCMPolisherReport.Remove(se);
                }
                db.SaveChanges();

                //question
                string questiongraintype = null;
                string questionprocess = null;
                string questionpass = null;
                string questioncapacity = null;
                string questionpolishrequirement = null;
                string questionmotortype = null;
                string questionmotorq = null;
                string questionmotorrating = null;
                //end questions

                //answers
                string modelname = null;
                string answergraintype = null;
                string answerprocess = null;
                string drive = null;
                string motor = null;
                string sieve = null;
                string reducerring = null;
                string ctcoil = null;
                string accessories = null;
                string Drive = null;
                string Motor = null;
                string Sieve = null;
                string ReducerRing = null;
                string CTCoil = null;
                string MotorType = null;
                string AccessoriesToolBox = null;
                int cpid = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.CPID).SingleOrDefault();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    String query = "SELECT * FROM RiceOAEquipTableData WHERE ROATBID = '" + roatbid + "';";
                    var riceoalist = db.Database.SqlQuery<RiceOAEquipTableData>(query).ToList();
                    var data = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).ToList();
                    foreach (var da in data)
                    {
                        try
                        {
                            modelname = db.ProductModel.Where(m => m.ProductID == da.ProductID).Select(m => m.ProductModelName).FirstOrDefault();
                            answergraintype = da.PaddySize;
                            answerprocess = answergraintype + " " + da.TypeRice;
                            string trmpsz = da.PaddySize.Trim();
                            string trmtprc = da.TypeRice.Trim();
                            string trmpass = da.Pass.Trim();
                            string proname = db.Products.Where(m => m.ProductID == da.ProductID).Select(m => m.ProductName).FirstOrDefault();
                            proname = proname.Trim();
                            string pathname = proname + '-' + trmpsz + '-' + trmtprc + '-' + trmpass;
                            int passid = db.OCMPass.Where(m => m.PathName == pathname).Select(m => m.PassId).FirstOrDefault();
                            Drive = db.OCMDriveMS.Where(m => m.PassId == passid).Select(m => m.DriveMSName).FirstOrDefault();
                            Motor = da.MotorQ + " " + da.MotorRating;
                            Sieve = db.OCMSieveslot.Where(m => m.PassId == passid).Select(m => m.SieveslotName).FirstOrDefault();
                            CTCoil = "";
                            MotorType = da.MotorType;
                            ReducerRing = db.OCMReducerRingNew.Where(m => m.PassId == passid).Select(m => m.OCMReducerRingName).FirstOrDefault();
                            CTCoil = db.OCMCTCoilNew.Where(m => m.PassId == passid).Select(m => m.OCMCTCoilName).FirstOrDefault();
                            AccessoriesToolBox = db.OCMAccessoriesNew.Where(m => m.PassId == passid).Select(m => m.AccessoriesName).FirstOrDefault();
                            //PassageSticker = "Yes";


                            var custname = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.MDBGeneralData.OrganizationName).SingleOrDefault();
                            var address = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => new { m.MDBGeneralData.AddressLine1, m.MDBGeneralData.AddressLine2, m.MDBGeneralData.City, m.MDBGeneralData.Country }).SingleOrDefault();
                            var quantity = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).SingleOrDefault();
                            var cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                            questioncapacity = db.RiceOAEquipTableData.Where(m => m.ROAID == roaid).Select(m => m.Capacity).SingleOrDefault();
                            string cap = da.Capacity.Trim();
                            string promod = db.OCMCapacityKW.Where(m => m.CapacityTPHName == cap).Select(m => m.ProductModelName).FirstOrDefault();
                            string[] a = promod.Split('/');
                            promod = a[0];
                            OCMPolisherReport ocm = new OCMPolisherReport();
                            ocm.FilledBy = cpname;
                            ocm.Branch = "SAS";
                            ocm.Date = System.DateTime.Now;
                            ocm.CustomerName = custname;
                            ocm.Location = address.AddressLine1 + "," + address.AddressLine2 + "," + address.City;
                            ocm.Country = address.Country;
                            ocm.Quantity = quantity.Quantity;
                            ocm.Product = modelname;
                            ocm.Capacity = questioncapacity;
                            ocm.Process = answerprocess;

                            ocm.SievePlate = Sieve;
                            ocm.Drive = Drive + " " + da.MotorRating;
                            ocm.Motor = Motor;
                            ocm.ReducerRing = ReducerRing;
                            ocm.CTCoil = CTCoil;
                            ocm.Accessories = AccessoriesToolBox;
                            ocm.SievePlatePartNo = Sieve;
                            ocm.DrivePartNo = Drive;
                            ocm.MotorPartNo = Motor;
                            ocm.CTCoilPartNo = CTCoil;
                            ocm.AccessoriesPartNo = AccessoriesToolBox;
                            ocm.ProductModel = promod;
                            db.OCMPolisherReport.Add(ocm);
                            db.SaveChanges();

                            return RedirectToAction("OCMPolisherReport", "Reports", new { });
                        }
                        catch (Exception e)
                        {
                            TempData["reportfail"] = "Some datas are missing please contact system administrator";
                            return RedirectToAction("TASheets", "RiceOA", new { roaid, roatbid });
                        }
                    }
                    //String query = "SELECT * FROM RiceOAEquipTableData WHERE ROATBID = '" + roatbid + "';";
                    //var riceoalist = db.Database.SqlQuery<RiceOAEquipTableData>(query).ToList();
                    //DataSet ds = new DataSet();
                    //foreach (var s in riceoalist)
                    //{

                    //    questiongraintype = s.PaddySize;
                    //    questionprocess = s.TypeRice;
                    //    questionpass = s.Pass;
                    //    questioncapacity = s.Capacity;
                    //    questionpolishrequirement = s.PolishRequirement;
                    //    questionmotortype = s.MotorType;
                    //    questionmotorq = s.MotorQ;
                    //    questionmotorrating = s.MotorRating;
                    //    SqlConnection con = new SqlConnection(connectionString);
                    //    SqlCommand cmd = new SqlCommand("spSelectOCMPolisher", con);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.AddWithValue("@questiongraintype", questiongraintype);
                    //    cmd.Parameters.AddWithValue("@questionprocess", questionprocess);
                    //    cmd.Parameters.AddWithValue("@questionpass", questionpass);
                    //    cmd.Parameters.AddWithValue("@questioncapacity", questioncapacity);
                    //    cmd.Parameters.AddWithValue("@questionpolishrequirement", questionpolishrequirement);
                    //    cmd.Parameters.AddWithValue("@questionmotortype", questionmotortype);
                    //    cmd.Parameters.AddWithValue("@questionmotorq", questionmotorq);
                    //    cmd.Parameters.AddWithValue("@questionmotorrating", questionmotorrating);
                    //    con.Open();
                    //    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    //    da.Fill(ds);
                    //    con.Close();

                    //    if(ds!=null && ds.Tables.Count>0)
                    //    {
                    //        if(ds.Tables[0].Rows.Count>0)
                    //        {
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ModelName"].ToString()))
                    //            {
                    //                 modelname = Convert.ToString(ds.Tables[0].Rows[0]["ModelName"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["GrainType"].ToString()))
                    //            {
                    //                answergraintype = Convert.ToString(ds.Tables[0].Rows[0]["GrainType"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Process"].ToString()))
                    //            {
                    //                answerprocess = Convert.ToString(ds.Tables[0].Rows[0]["Process"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Drive"].ToString()))
                    //            {
                    //                 Drive = Convert.ToString(ds.Tables[0].Rows[0]["Drive"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Motor"].ToString()))
                    //            {
                    //                Motor = Convert.ToString(ds.Tables[0].Rows[0]["Motor"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Sieve"].ToString()))
                    //            {
                    //                Sieve = Convert.ToString(ds.Tables[0].Rows[0]["Sieve"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ReducerRing"].ToString()))
                    //            {
                    //                ReducerRing = Convert.ToString(ds.Tables[0].Rows[0]["ReducerRing"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["CTCoil"].ToString()))
                    //            {
                    //                CTCoil = Convert.ToString(ds.Tables[0].Rows[0]["CTCoil"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["MotorType"].ToString()))
                    //            {
                    //                MotorType = Convert.ToString(ds.Tables[0].Rows[0]["MotorType"]);
                    //            }
                    //            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Accessories1"].ToString()))
                    //            {
                    //                Accessories1 = Convert.ToString(ds.Tables[0].Rows[0]["Accessories1"]);
                    //            }

                    //            var drivepartno = db.OCMDrive.Where(m => m.DriveDescription == Drive).Where(m => m.DriveMotorType == MotorType).Select(m => m.PartNumber).SingleOrDefault();

                    //            var motorpartno = db.OCMMotor.Where(m => m.MotorDescription == Motor).Where(m => m.MotorMotorType == MotorType).Select(m => m.PartNumber).SingleOrDefault();

                    //            var sievepartno = db.OCMSieve.Where(m => m.SieveDescription == Sieve).Select(m => m.PartNumber).SingleOrDefault();

                    //            var reducerringpartno = db.OCMReducerRing.Where(m => m.ReduceRingDescription == ReducerRing).Select(m => m.PartNumber).SingleOrDefault();

                    //            var ctcoilpartno = db.OCMCTCoil.Where(m => m.CTCoilDescription == CTCoil).Select(m => m.PartNumber).SingleOrDefault();

                    //            string accessoriesdbval = Accessories1;
                    //            string[] access = accessoriesdbval.Split(',');
                    //            int i = access.Length; int j = 0;
                    //            string accdesc = null;

                    //            var accessoriespartno = db.OCMAccessories.Where(m => m.IsDeleted == 1000).Select(m => m.PartNumber).SingleOrDefault();

                    //            for (j = 0; j < i; j++)
                    //            {
                    //                accdesc = access[j];
                    //                if (i == 1)
                    //                {
                    //                    accessoriespartno = db.OCMAccessories.Where(m => m.AccessoriesDescription == accdesc).Select(m => m.PartNumber).SingleOrDefault();
                    //                }
                    //                else
                    //                {
                    //                    accessoriespartno = db.OCMAccessories.Where(m => m.AccessoriesDescription == accdesc).Select(m => m.PartNumber).SingleOrDefault();
                    //                }
                    //            }

                    //            var custname = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.MDBGeneralData.OrganizationName).SingleOrDefault();
                    //            var address = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => new { m.MDBGeneralData.AddressLine1, m.MDBGeneralData.AddressLine2, m.MDBGeneralData.City, m.MDBGeneralData.Country }).SingleOrDefault();
                    //            var quantity = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).SingleOrDefault();
                    //            var cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();

                    //            OCMPolisherReport ocm = new OCMPolisherReport();
                    //            ocm.FilledBy = "Bharath Naik";
                    //            ocm.Branch = "SAS";
                    //            ocm.Date = System.DateTime.Now;
                    //            ocm.CustomerName = custname;
                    //            ocm.Location = address.AddressLine1 + "," + address.AddressLine2 + "," + address.City;
                    //            ocm.Country = address.Country;
                    //            ocm.Quantity = quantity.Quantity;
                    //            ocm.Product = modelname;
                    //            ocm.Capacity = questioncapacity;
                    //            ocm.Process = answerprocess;

                    //            ocm.SievePlate = Sieve;
                    //            ocm.ReducerRing = ReducerRing;
                    //            ocm.Drive = Drive;
                    //            ocm.Motor =Motor;
                    //            ocm.CTCoil =CTCoil;
                    //            ocm.Accessories =Accessories1;

                    //            ocm.SievePlatePartNo = sievepartno;
                    //            ocm.ReducerRingPartNo = reducerringpartno;
                    //            ocm.DrivePartNo = drivepartno;
                    //            ocm.MotorPartNo = motorpartno;
                    //            ocm.CTCoilPartNo = ctcoilpartno;
                    //            ocm.AccessoriesPartNo = accessoriespartno;

                    //            db.OCMPolisherReport.Add(ocm);
                    //            db.SaveChanges();

                    //            return RedirectToAction("OCMPolisherReport", "Reports", new { });
                    //        }
                    //        else
                    //        {
                    //            ds = null;
                    //        }
                    //    }

                    //}
                }
                #endregion
            }
            TempData["reportfail"] = "The Model is not available, Please check the model Selected...";
            return RedirectToAction("TASheets", "RiceOA", new { roaid, roatbid });
        }

        public ActionResult OCMPolisherReport()
        {
            var ocmreportlist = db.OCMPolisherReport.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/OCMPolisherReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("OCMPolisherReport", ocmreportlist);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            localReport.DataSources.Add(reportDataSource);

            //string reportType = "word";//super for .doc(old format)
            //string reportType = "WORDOPENXML";//Perfect for .docx
            string reportType = "pdf";//cool for .pdf format

            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //To Save Option
            return File(renderedBytes, mimeType);// this shoud not be commented

        }

        public ActionResult OCMWhitnerRep(int? roaid, int? roatbid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            if (roaid != null)
            {
                var start = db.OCMWhitnerReport.ToList();
                foreach (var se in start)
                {
                    db.OCMWhitnerReport.Remove(se);
                }
                db.SaveChanges();

                //question
                string questiongraintype = null;
                string questionprocess = null;
                string questionpass = null;
                string questioncapacity = null;
                string questionmotorq = null;
                string questionmotortype = null;
                string questionmotorrating = null;
                //end questions

                //answers
                string modelname = null;
                string answergraintype = null;
                string answerprocess = null;

                string graintype = "";
                string wdorwdt = "";
                string drive = null;
                string motor = null;
                string stone = null;
                string sieve = null;
                string brake = null;
                string ctcoil = null;
                string accessories = null;
                string passageSticker = null;
                string Drive = null;
                string Motor = null;
                string Sieve = null;
                string ReducerRing = null;
                string CTCoil = null;
                string MotorType = null;
                string AccessoriesToolBox = null;
                string Stone = null;
                string Brake = null;
                string PassageSticker = null;
                int cpid = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.CPID).SingleOrDefault();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    String query = "SELECT * FROM RiceOAEquipTableData WHERE ROATBID = '" + roatbid + "';";
                    var riceoalist = db.Database.SqlQuery<RiceOAEquipTableData>(query).ToList();
                    var data = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).ToList();
                    foreach (var da in data)
                    {
                        try
                        {
                            modelname = db.ProductModel.Where(m => m.ProductID == da.ProductID).Select(m => m.ProductModelName).FirstOrDefault();
                            answergraintype = da.PaddySize;
                            answerprocess = answergraintype + " " + da.TypeRice;
                            answerprocess = answerprocess.Trim();
                            string trmpsz = da.PaddySize.Trim();
                            graintype = trmpsz;
                            string trmtprc = da.TypeRice.Trim();
                            string trmpass = da.Pass.Trim();
                            string proname = db.Products.Where(m => m.ProductID == da.ProductID).Select(m => m.ProductName).FirstOrDefault();
                            proname = proname.Trim();
                            string pathname = proname + '-' + trmpsz + '-' + trmtprc + '-' + trmpass;
                            int passid = db.OCMPass.Where(m => m.PathName == pathname).Select(m => m.PassId).FirstOrDefault();
                            Drive = db.OCMDriveMS.Where(m => m.PassId == passid).Select(m => m.DriveMSName).FirstOrDefault();
                            Motor = da.MotorQ + " " + da.MotorRating;
                            Sieve = db.OCMSieveslot.Where(m => m.PassId == passid).Select(m => m.SieveslotName).FirstOrDefault();
                            wdorwdt = da.MotorType;
                            CTCoil = "";
                            MotorType = da.MotorType;
                            Brake = db.OCMBrakechamfer.Where(m => m.PassId == passid).Select(m => m.BrakechamferName).FirstOrDefault();
                            Stone = db.OCMStoneGrit.Where(m => m.PassId == passid).Select(m => m.StoneGritName).FirstOrDefault();
                            AccessoriesToolBox = db.OCMAccessoriesNew.Where(m => m.PassId == passid).Select(m => m.AccessoriesName).FirstOrDefault();
                            string passname = db.OCMPass.Where(m => m.PassId == passid).Select(m => m.PassName).FirstOrDefault();
                            passname = passname.Trim();
                            string[] a1 = passname.Split(' ');
                            passname = a1[1];
                            int passno = Convert.ToInt32(passname);
                            string pass = "";
                            if (passno == 1)
                            {
                                pass = "1st";
                            }
                            else if (passno == 2)
                            {
                                pass = "2nd";
                            }
                            else if (passno == 3)
                            {
                                pass = "3rd";
                            }
                            else if (passno > 3)
                            {
                                pass = passno + "th";
                            }
                            PassageSticker = pass + " Pass";//"Sticker, Passage "+
                            string cap = da.Capacity.Trim();
                            string promod = db.OCMCapacityKW.Where(m => m.CapacityTPHName == cap).Select(m => m.ProductModelName).FirstOrDefault();
                            string[] a = promod.Split('/');
                            promod = a[0];
                            var custname = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.MDBGeneralData.OrganizationName).SingleOrDefault();
                            var address = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => new { m.MDBGeneralData.AddressLine1, m.MDBGeneralData.AddressLine2, m.MDBGeneralData.City, m.MDBGeneralData.Country }).SingleOrDefault();
                            var quantity = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).SingleOrDefault();
                            var cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                            questioncapacity = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).Select(m => m.Capacity).SingleOrDefault();
                            OCMWhitnerReport ocm = new OCMWhitnerReport();
                            ocm.FilledBy = cpname;
                            ocm.Branch = "SAS";
                            ocm.Date = System.DateTime.Now;
                            ocm.CustomerName = custname;
                            ocm.Location = address.AddressLine1 + "," + address.AddressLine2 + "," + address.City;
                            ocm.Country = address.Country;
                            ocm.Quantity = quantity.Quantity;
                            ocm.Product = modelname;
                            ocm.Capacity = questioncapacity;
                            ocm.Process = answerprocess;

                            ocm.Stone = "Grinding Stone (Grit 30)";
                            ocm.SievePlate = "11 mm-gap sieve basket";
                            ocm.Brake = Brake;
                            ocm.Drive = Drive + " " + da.MotorRating;
                            ocm.Motor = Motor;
                            ocm.CTCoil = CTCoil;
                            ocm.Accessories = AccessoriesToolBox;
                            ocm.StickerPassage = PassageSticker;
                            ocm.Sievemm = Sieve;
                            ocm.MandBStone = Stone;
                            ocm.SievePlatePartNo = Sieve;
                            ocm.BrakePartNo = Brake;
                            ocm.DrivePartNo = Drive;
                            ocm.MotorPartNo = Motor;
                            ocm.StonePartNo = Stone;
                            ocm.CTCoilPartNo = CTCoil;
                            ocm.GrainType = graintype;
                            ocm.WorWdMTR = wdorwdt;
                            ocm.StickerPassagePartNo = PassageSticker;
                            ocm.AccessoriesPartNo = AccessoriesToolBox;

                            ocm.ProductModel = promod;
                            db.OCMWhitnerReport.Add(ocm);
                            db.SaveChanges();

                            return RedirectToAction("OCMWhitnerReport", "Reports", new { });
                        }
                        catch (Exception e)
                        {
                            TempData["reportfail"] = "Some datas are missing please contact system administrator";
                            return RedirectToAction("TASheets", "RiceOA", new { roaid, roatbid });
                        }
                    }
                    //foreach (var s in riceoalist)
                    //{
                    //    questiongraintype = s.PaddySize;
                    //    questionprocess = s.TypeRice;
                    //    questionpass = s.Pass;
                    //    questioncapacity = s.Capacity;
                    //    questionmotorq = s.MotorQ;
                    //    questionmotortype = s.MotorType;
                    //    questionmotorrating = s.MotorRating;

                    //    //cpid = s.CPID;

                    //    SqlConnection con = new SqlConnection(connectionString);//code added by sneha
                    //    SqlCommand cmd = new SqlCommand("spSlectfromOcmWhitener", con);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.AddWithValue("@questiongraintype", questiongraintype);
                    //    cmd.Parameters.AddWithValue("@questionprocess", questionprocess);
                    //    cmd.Parameters.AddWithValue("@questionpass", questionpass);
                    //    cmd.Parameters.AddWithValue("@questioncapacity", questioncapacity);
                    //    cmd.Parameters.AddWithValue("@questionmotortype", questionmotortype);
                    //    cmd.Parameters.AddWithValue("@questionmotorq", questionmotorq);
                    //    cmd.Parameters.AddWithValue("@questionmotorrating", questionmotorrating);
                    //    con.Open();
                    //    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    //    da.Fill(ds);
                    //    con.Close();

                    //    if (ds != null && ds.Tables.Count > 0)
                    //    {
                    //        if (ds.Tables[0].Rows.Count > 0)
                    //        {
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ModelName"].ToString()))
                    //            //{
                    //            //    modelname = Convert.ToString(ds.Tables[0].Rows[0]["ModelName"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["GrainType"].ToString()))
                    //            //{
                    //            //    answergraintype = Convert.ToString(ds.Tables[0].Rows[0]["GrainType"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Process"].ToString()))
                    //            //{
                    //            //    answerprocess = Convert.ToString(ds.Tables[0].Rows[0]["Process"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Drive"].ToString()))
                    //            //{
                    //            //    Drive = Convert.ToString(ds.Tables[0].Rows[0]["Drive"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Motor"].ToString()))
                    //            //{
                    //            //    Motor = Convert.ToString(ds.Tables[0].Rows[0]["Motor"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Sieve"].ToString()))
                    //            //{
                    //            //    Sieve = Convert.ToString(ds.Tables[0].Rows[0]["Sieve"]);
                    //            //}

                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["CTCoil"].ToString()))
                    //            //{
                    //            //    CTCoil = Convert.ToString(ds.Tables[0].Rows[0]["CTCoil"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["MotorType"].ToString()))
                    //            //{
                    //            //    MotorType = Convert.ToString(ds.Tables[0].Rows[0]["MotorType"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Brake"].ToString()))
                    //            //{
                    //            //    Brake = Convert.ToString(ds.Tables[0].Rows[0]["Brake"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Stone"].ToString()))
                    //            //{
                    //            //    Stone = Convert.ToString(ds.Tables[0].Rows[0]["Stone"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["AccessoriesToolBox"].ToString()))
                    //            //{
                    //            //    AccessoriesToolBox = Convert.ToString(ds.Tables[0].Rows[0]["AccessoriesToolBox"]);
                    //            //}
                    //            //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["PassageSticker"].ToString()))
                    //            //{
                    //            //    PassageSticker = Convert.ToString(ds.Tables[0].Rows[0]["PassageSticker"]);
                    //            //}
                    //            //var drivepartno = db.OCMDrive.Where(m => m.DriveDescription == Drive).Where(m => m.DriveMotorType == MotorType).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var motorpartno = db.OCMMotor.Where(m => m.MotorDescription == Motor).Where(m => m.MotorMotorType == MotorType).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var stonepartno = db.OCMStone.Where(m => m.StoneDescription == Stone).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var sievepartno = db.OCMSieve.Where(m => m.SieveDescription == Sieve).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var brakepartno = db.OCMBrake.Where(m => m.BrakeFullName == Brake).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var ctcoilpartno = db.OCMCTCoil.Where(m => m.CTCoilDescription == CTCoil).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var accessoriespartno = db.OCMAccessories.Where(m => m.AccessoriesDescription == AccessoriesToolBox).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var passagestickerpartno = db.OCMPassageSticker.Where(m => m.PassageStickerDescription == PassageSticker).Select(m => m.PartNumber).SingleOrDefault();

                    //            //var custname = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.MDBGeneralData.OrganizationName).SingleOrDefault();
                    //            //var address = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => new { m.MDBGeneralData.AddressLine1, m.MDBGeneralData.AddressLine2, m.MDBGeneralData.City, m.MDBGeneralData.Country }).SingleOrDefault();
                    //            //var quantity = db.RiceOAEquipTableData.Where(m => m.ROATBID == roatbid).SingleOrDefault();
                    //            //var cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();

                    //            //OCMWhitnerReport ocm = new OCMWhitnerReport();
                    //            //ocm.FilledBy = "Bharath Naik";
                    //            //ocm.Branch = "SAS";
                    //            //ocm.Date = System.DateTime.Now;
                    //            //ocm.CustomerName = custname;
                    //            //ocm.Location = address.AddressLine1 + "," + address.AddressLine2 + "," + address.City;
                    //            //ocm.Country = address.Country;
                    //            //ocm.Quantity = quantity.Quantity;
                    //            //ocm.Product = modelname;
                    //            //ocm.Capacity = questioncapacity;
                    //            //ocm.Process = answerprocess;

                    //            //ocm.Stone = Stone;
                    //            //ocm.SievePlate = Sieve;
                    //            //ocm.Brake = Brake;
                    //            //ocm.Drive = Drive;
                    //            //ocm.Motor = Motor;
                    //            //ocm.CTCoil = CTCoil;
                    //            //ocm.Accessories = AccessoriesToolBox;
                    //            //ocm.StickerPassage = PassageSticker;

                    //            //ocm.SievePlatePartNo = sievepartno;
                    //            //ocm.BrakePartNo = brakepartno;
                    //            //ocm.DrivePartNo = drivepartno;
                    //            //ocm.MotorPartNo = motorpartno;
                    //            //ocm.StonePartNo = stonepartno;
                    //            //ocm.CTCoilPartNo = ctcoilpartno;
                    //            //ocm.StickerPassagePartNo = passagestickerpartno;
                    //            //ocm.AccessoriesPartNo = accessoriespartno;

                    //            //db.OCMWhitnerReport.Add(ocm);
                    //            //db.SaveChanges();

                    //            //return RedirectToAction("OCMWhitnerReport", "Reports", new { });
                    //        }
                    //        else
                    //        {
                    //            ds = null;
                    //        }
                    //    }

                    //}
                }
                #endregion
            }
            TempData["reportfail"] = "The Model is not available, Please check the model Selected...";
            return RedirectToAction("TASheets", "RiceOA", new { roaid, roatbid });
        }

        public ActionResult OCMWhitnerReport()

        {
            var ocmreportlist = db.OCMWhitnerReport.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/OCMWhitnerReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("OCMWhithnerReport", ocmreportlist);

            ReportParameterCollection reportParameters = new ReportParameterCollection();
            //reportParameters.Add(new ReportParameter("FromDate", fromdat));
            //reportParameters.Add(new ReportParameter("ToDate", todat));
            //reportParameters.Add(new ReportParameter("TCount", tcount));
            //localReport.SetParameters(reportParameters);
            localReport.DataSources.Add(reportDataSource);
            //string reportType = "word";
            // string reportType = "WORDOPENXML";//Perfect for .docx
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        public ActionResult RiceOAReport(int? roaid)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();

            if (roaid != null)
            {
                var start = db.RiceOAReportDBSheet.ToList();
                foreach (var se in start)
                {
                    db.RiceOAReportDBSheet.Remove(se);
                }
                db.SaveChanges();

                var start1 = db.RiceOAReportDataTable.ToList();
                foreach (var se in start1)
                {
                    db.RiceOAReportDataTable.Remove(se);
                }
                db.SaveChanges();

                string salesmanager = "";
                string business = "";
                string businessarea = "";
                string buisnessunit = "";
                string businessunitforsap = "";
                string marketsegmanet = "";
                string customersapidnumber = "";
                string projecttitle = "";
                string graintype = "";
                string process = "";
                string capacity = "";
                string billingaddress = "";
                string deliveraddress = "";
                string customercontactperson = "";
                string customertelephonenumber = "";
                string customerfaxno = "";
                string customermobilenumber = "";
                string customermailaddress = "";
                string packingandforwarding = "";
                string transistinsurance = "";
                string freight = "";
                string incoterms = "";
                string commiteddelivery = "";
                string paymentterms = "";
                string comments = "";
                string price = "";
                string approvaldate = "";
                string gst = "";
                string pan = "";
                string sapdlvry = "";

                int cpid = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.CPID).SingleOrDefault();

                //Report for Buhler Admin
                #region
                if (User.IsInRole("Administrator"))
                {
                    var data = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).ToList();
                    foreach (var da in data)
                    {
                        try
                        {
                            //for updating new coloumn for table RiceOAReport
                            #region

                            int mdbid = db.MDBGeneralData.Where(m => m.CompanyUniqueID == da.CompanyUniqueID).Select(m => m.MDBID).FirstOrDefault();
                            salesmanager = da.SalesManager;
                            approvaldate = da.Approvaldate;
                            business = da.Business;
                            businessarea = da.BusinessArea;
                            buisnessunit = da.BusinessUnit;
                            businessunitforsap = da.BusinessUnitForSAP;
                            marketsegmanet = da.MarketSegment;
                            customersapidnumber = da.CustomerSAPIdNo;
                            projecttitle = da.Subjectinfo;
                            graintype = db.RiceOAEquipTableData.Where(m => m.ROAID == roaid).Select(m => m.PaddySize).FirstOrDefault();
                            process = db.RiceOAEquipTableData.Where(m => m.ROAID == roaid).Select(m => m.TypeRice).FirstOrDefault();
                            capacity = db.RiceOAEquipTableData.Where(m => m.ROAID == roaid).Select(m => m.Capacity).FirstOrDefault();
                            string baa = db.MDBGeneralData.Where(m => m.CompanyUniqueID == da.CompanyUniqueID).Select(m => m.BillingAddress).FirstOrDefault();
                            billingaddress = baa.Trim();
                            string daa = db.MDBGeneralData.Where(m => m.CompanyUniqueID == da.CompanyUniqueID).Select(m => m.DeleveryAddress).FirstOrDefault();
                            deliveraddress = daa.Trim();
                            string prefix = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).Select(m => m.Title).FirstOrDefault();
                            string fname = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).Select(m => m.FirstName).FirstOrDefault();
                            customercontactperson = prefix + " " + fname;
                            customertelephonenumber = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).Select(m => m.PhoneLL1).FirstOrDefault();
                            customerfaxno = db.MDBGeneralData.Where(m => m.CompanyUniqueID == da.CompanyUniqueID).Select(m => m.FAX).FirstOrDefault();
                            customermobilenumber = db.MDBContactPersonData.Where(m => m.MDBID == mdbid).Select(m => m.Mobile1).FirstOrDefault();
                            customermailaddress = db.MDBGeneralData.Where(m => m.CompanyUniqueID == da.CompanyUniqueID).Select(m => m.EmailID).FirstOrDefault();
                            packingandforwarding = da.PackingAndForwarding;
                            transistinsurance = da.TransistInsurance;
                            freight = da.Freight;
                            incoterms = da.IncoTerms;
                            commiteddelivery = da.commitedDelivery;
                            paymentterms = da.PaymentTerms;
                            comments = db.RiceOAEquipPayment.Where(m => m.ROAID == roaid).Select(m => m.annexure).FirstOrDefault();
                            price = db.RiceOAEquipPayment.Where(m => m.ROAID == roaid).Select(m => m.overallprice).FirstOrDefault();
                            gst = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.TinNumber).FirstOrDefault();
                            pan = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.PANCardNo).FirstOrDefault();
                            sapdlvry = db.RiceOAEquipGeneralData.Where(m => m.ROAID == roaid).Select(m => m.CustomerSAPIdDelvryNo).FirstOrDefault();
                            RiceOAReportDBSheet roarep = new RiceOAReportDBSheet();
                            roarep.ROAID = Convert.ToInt32(roaid);
                            roarep.SalesManager = salesmanager;
                            roarep.Approvaldate = approvaldate;
                            roarep.Business = business;
                            roarep.BusinessArea = businessarea;
                            roarep.BusinessUnit = buisnessunit;
                            roarep.BusinessUnitForSAP = businessunitforsap;
                            roarep.MarketSegment = marketsegmanet;
                            roarep.CustomerSAPIdNo = customersapidnumber;
                            roarep.Subjectinfo = projecttitle;
                            roarep.PaddySize = graintype;
                            roarep.TypeRice = process;
                            roarep.Capacity = capacity;
                            roarep.billingaddress = billingaddress;
                            roarep.deliveraddress = deliveraddress;
                            roarep.customercontactperson = customercontactperson;
                            roarep.customertelephonenumber = customertelephonenumber;
                            roarep.customerfaxno = customerfaxno;
                            roarep.customermobilenumber = customermobilenumber;
                            roarep.customermailaddress = customermailaddress;
                            roarep.PackingAndForwarding = packingandforwarding;
                            roarep.TransistInsurance = transistinsurance;
                            roarep.Freight = freight;
                            roarep.IncoTerms = incoterms;
                            roarep.commitedDelivery = commiteddelivery;
                            roarep.PaymentTerms = paymentterms;
                            roarep.comments = comments;
                            roarep.Price = price;
                            roarep.TinNumber = gst;
                            roarep.PANCardNo = pan;
                            roarep.CustomerSAPIdDelvryNo = sapdlvry;
                            db.RiceOAReportDBSheet.Add(roarep);
                            db.SaveChanges();
                            #endregion

                            //for updating new coloumn for table RiceOAReportDataTable
                            #region
                            var tbldata = db.RiceOAEquipTableData.Where(m => m.ROAID == roaid).ToList();

                            int slno = 1;
                            if (tbldata.Count != 0)
                            {
                                foreach (var tb in tbldata)
                                {
                                    RiceOAReportDataTable rc = new RiceOAReportDataTable();
                                    rc.Slno = slno.ToString();
                                    rc.Machine = db.Products.Where(m => m.ProductID == tb.ProductID).Select(m => m.ProductName).FirstOrDefault();
                                    rc.Description = tb.ModelDesc;
                                    rc.Quantity = tb.Quantity.ToString();
                                    rc.UnitPrice = tb.UnitPrice;
                                    rc.TotalPrice = tb.TotalPrice;
                                    db.RiceOAReportDataTable.Add(rc);
                                    db.SaveChanges();
                                    slno++;

                                }
                            }



                            #endregion

                            return RedirectToAction("RiceOARep", "Reports", new { });

                        }
                        catch (Exception e)
                        {
                            TempData["reportfail"] = "Some datas are missing please contact system administrator";
                            return RedirectToAction("RiceOAReport", "Reports", new { roaid });
                        }
                    }
                }
                #endregion
            }
            TempData["reportfail"] = "Failed please check again";
            return RedirectToAction("RiceOAReport", "Reports", new { roaid });
        }

        public ActionResult RiceOARep()
        {
            var RiceOAReportlist = db.RiceOAReportDBSheet.ToList();
            var RiceOAReportTablist = db.RiceOAReportDataTable.ToList();
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/RiceOAReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("RiceOAReportDataSet", RiceOAReportlist);
            ReportDataSource RiceOAReportDataSetTable = new ReportDataSource("RiceOAReportDataSetTable", RiceOAReportTablist);
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(RiceOAReportDataSetTable);
            //string reportType = "word";
            //string reportType = "WORDOPENXML";//Perfect for .docx
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }

        #endregion
    }
}
