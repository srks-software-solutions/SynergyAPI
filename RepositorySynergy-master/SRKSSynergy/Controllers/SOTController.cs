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
    public class SOTController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
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
                              where (r.CPName.ToLower().Contains(term.ToLower()))
                              select new { r.CPName }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
                    Text = "QED",
                    Value = "QED",
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
            if (mon == "Jan,2014")
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
                    Selected = true,
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

         // GET: /SOT/
         public ActionResult Index()
         {
             var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
             var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
             string prode = "Project Delayed";
             string prodr = "Project Dropped";

             // delete and updater
             //var tsotidlist = db.SOT_Temp_tbl.ToList();
             //foreach (var l in tsotidlist)
             //{
             //    SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(l.TSOTID);
             //    db.SOT_Temp_tbl.Remove(tempsot);
             //    db.SaveChanges();
             //}

             var sotlist = db.SOT.Where(q => q.CPID == loginname.CPID).Where(m => m.BYJTChances != 100).Where(m => m.BYJTChances != 0).Where(m => m.Orderactive != prode).Where(m => m.Orderactive != prodr).Where(m => m.Islatestquo != 1).OrderByDescending(m => m.SOTID).ToList();

             int slno = 1;
             foreach (var sot in sotlist)
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

             return View(sotlist);
         }

        //POST: /SOT/Model
        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult Index(IEnumerable<SOT> sot)
        {
            int byjt = -1;
            String month = null;
            foreach (var s in sot)
            {
                try
                {
                    byjt = Convert.ToInt32(s.BYJTChances.ToString());

                }
                catch
                {
                    byjt = -1;
                }

                if (byjt == 0 )
                {
                   if(s.Status!=4){
                       var sotbyjt = sot.Where(m => m.QGID == s.QGID && m.Status != 4).Where(m => m.Equipment == s.Equipment).ToList();
                             foreach (var qg in sotbyjt)
                            {
                                s.Status = 2;
                                month = monthtodate(qg.Expectedorder);
                                qg.Expecteddate = Convert.ToDateTime(month);
                                qg.BYJTChances = null;
                                s.BYJTChances = null;
                                if (byjt > 0)
                                {
                                    month = monthtodate(s.Expectedorder);
                                    s.Expecteddate = Convert.ToDateTime(month);
                                    s.BYJTChances = byjt;
                                    s.Status = 4;
                                }
                                db.Entry(qg).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                    }

                   var ProdModelId = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelName == s.Equipment).SingleOrDefault();    //Select(m => m.ProductModelID).SingleOrDefault();

                   //new Code Updating the QGEquipTableData 
                   //Making the IsSOTStatus 2 for 0% model item
                   //var quogen = db.QGEquipTableData.Where(m => m.QGID == s.QGID).Where(m => m.ProductID == s.ProductID).Where(m => m.MasterProductID == s.MasterProductID).Where(m => m.ProductModelID == ProdModelId).ToList();
                   var quogen = db.QGEquipTableData.Where(m => m.QGID == s.QGID).Where(m => m.ProductModelID == ProdModelId.ProductModelID).ToList();
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
                   db.SOT_Temp_tbl.Add(temp);
                   db.SaveChanges();
                    //return RedirectToAction("LOA", "LostOrder", new { qgid = s.QGID});
                }
                if (byjt == 100)
                {
                    var sotbyjt = sot.Where(m => m.QGID == s.QGID).Where(m => m.Equipment == s.Equipment).ToList();
                    foreach (var qg in sotbyjt)
                    {
                        month = monthtodate(qg.Expectedorder);
                        qg.Expecteddate = Convert.ToDateTime(month);
                        qg.BYJTChances = null;
                        s.BYJTChances = null;
                        s.Status = 4;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    s.BYJTChances = null;

                    var ProdModelId = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelName == s.Equipment).SingleOrDefault();   //Select(m => m.ProductModelID).SingleOrDefault();

                    //Making the IsSOTStatus 1 for 100% model item
                    //var quogen = db.QGEquipTableData.Where(m => m.QGID == s.QGID).Where(m => m.ProductID == s.ProductID).Where(m => m.MasterProductID == s.MasterProductID).Where(m => m.ProductModelID == ProdModelId).ToList();
                    var quogen = db.QGEquipTableData.Where(m => m.QGID == s.QGID).Where(m => m.ProductModelID == ProdModelId.ProductModelID).ToList();
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
                    db.SOT_Temp_tbl.Add(temp);
                    db.SaveChanges();

                    //return RedirectToAction("OAGenerate", "OA", new { qgid = s.QGID });
                }

                //For data other than 0, 100, Project Delayed and Dropped saving code
                if (s.Orderactive != "Project Dropped")
                {
                    month = monthtodate(s.Expectedorder);
                    s.Expecteddate = Convert.ToDateTime(month);
                }

                if (byjt == 0 || byjt == 100)
                {
                    s.BYJTChances = null;
                }
                else
                {
                    s.BYJTChances = byjt;
                }
                if (byjt == 0)
                {
                    s.Status = 2;
                }
                else
                {
                    s.Status = 4;
                }
                //s.Status = 4;
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();

                //
                //Remove Quotation from OA View When Project Delayed == 1 & Project Dropped == 2
                if (s.Orderactive == "Project Delayed")
                {
                    var quogen = db.QGEquipGeneralData.Where(m => m.QGID == s.QGID && s.Status==4).ToList();
                    foreach (var qg in quogen)
                    {
                        qg.QuotStatus = 1;
                        db.Entry(qg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //var tb = db.QGEquipTableData.Where(m => m.QGID == s.QGID && s.Status == 4).ToList();
                    //foreach (var qg in quogen)
                    //{
                    //    qg.QuotStatus = 1;
                    //    db.Entry(qg).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
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
                    //var quogentd = db.QGEquipTableData.Where(m => m.QGID == s.QGID && s.Status == 4).ToList();
                    //foreach (var qg in quogentd)
                    //{
                    //    qg.IsSOTStatus = 4;
                    //    db.Entry(qg).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                }
                //End in Updating the OA View
              }

            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var list = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).ToList();
            int count = list.Count;

            foreach (var l in list)
            {
                if (l.BYJTChances == 0 || l.BYJTChances == 100)
                {
                    return RedirectToAction("SOTTempList", "SOT", null);
                }
            }
             return RedirectToAction("Index");
        }

        // GET: /SOT/Details/5
        public ActionResult Details(int id = 0)
        {
            SOT sot = db.SOT.Find(id);
            if (sot == null)
            {
                return HttpNotFound();
            }
            return View(sot);
        }

        // GET: /SOT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SOT/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SOT sot)
        {
            if (ModelState.IsValid)
            {
                db.SOT.Add(sot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sot);
        }

        // GET: /SOT/Edit/5
        public ActionResult Edit(IList<SOT> sot)
        {
            foreach (var s in sot)
            {
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public string monthtodate(string month)
        {
            string mondat = null;
            string[] mon = month.Split(new char[]{' ',','});

            switch(mon[0])
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

        // GET: /SOT/Delete/5
        public ActionResult Delete(int id = 0)
        {
            SOT sot = db.SOT.Find(id);
            if (sot == null)
            {
                return HttpNotFound();
            }
            return View(sot);
        }

        // POST: /SOT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SOT sot = db.SOT.Find(id);
            db.SOT.Remove(sot);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult SOTReport(string cpnam = null, string Equipname = null, string frommonth = null, string tomonth = null, int byjt = -1)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID,m.ZoneID }).SingleOrDefault();
            List<SOT> sotlist =null;
            if (User.IsInRole("ZonalManager"))
            {
                sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
            }
            else
            {
                sotlist = db.SOT.Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.Expecteddate).ToList();
            }
            ViewBag.CPname = cpnam;
            ViewBag.Equipment = Equipname;
            ViewBag.fromExpectedMonth = frommonth;
            ViewBag.toExpectedMonth = tomonth;
            ViewBag.BYJT = byjt;
            //
            //Channel partner Name
            //
            if (cpnam != null && cpnam != "")
            {
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }
                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Equipment Name
            //
            if (Equipname != null && Equipname != "")
            {
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                    sotlist = db.SOT.Where(m => m.Equipment == Equipname).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                }
                if (sotlist.Count == 0)
                {
                    TempData["noeuipment"] = "No Data Exists for the selected value";
                }
                else
                {
                    ViewBag.equip = Equipname;
                    TempData["euipment"] = "SOT Report for";
                }
            }
            //
            //Expected Order Month
            //
            if (frommonth != null && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                    sotlist = db.SOT.Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.Expecteddate).ToList();
                }
                if (sotlist.Count == 0)
                {
                    TempData["noexpmonth"] = "No Data Exists for the selected value";
                }
                else
                {
                    //ViewBag.expmon = expmonth;
                    TempData["expmonth"] = "SOT Report for";
                }
            }
            //
            //byjt Chances
            //
            if (byjt != -1)
            {
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.BYJTChances >= byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                    sotlist = db.SOT.Where(m => m.BYJTChances >= byjt).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                }
                if (sotlist.Count == 0)
                {
                    TempData["nobyjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    ViewBag.byjt = byjt;
                    TempData["byjt"] = "SOT Report for BYJT Chances at";
                }
            }
            ////
            ////Channel partner Name, Equipment Name, Expected Order Month & byjt Chances
            ////
            if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "" && byjt != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.CPID == channame && s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.BYJTChances == byjt).Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").Where(m => m.Equipment == Equipname).OrderByDescending(m => m.Expecteddate).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }
                ViewBag.allcp = chpart.CPName;
                ViewBag.allbyjt = byjt;
                ViewBag.allEquipname = Equipname;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["noall"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["all"] = "SOT Report of";
                }


                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Channel partner Name & Equipment Name
            //
            if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "")
            {
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.CPID == channame && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.Equipment == Equipname).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["nocp&ep"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["cp&ep"] = "SOT Report for";
                }
                ViewBag.allEquipname = Equipname;
                ViewBag.allcp = chpart.CPName;

                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Channel partner Name & Expected order Month
            //
            if (cpnam != null && cpnam != "" && frommonth != null && frommonth != "" && tomonth != null && tomonth != "" )
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.CPID == channame && s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.Expecteddate).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }

                ViewBag.allcp = chpart.CPName;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["nocp&exp"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["cp&exp"] = "SOT Report of";
                }

                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Channel partner Name & byjt Chances
            //
            if (cpnam != null && cpnam != "" && byjt != -1)
            {
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.CPID == channame && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.BYJTChances == byjt).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }

                ViewBag.allcp = chpart.CPName;
                ViewBag.allbyjt = byjt;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["nocp&byjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["cp&byjt"] = "SOT Report of";
                }

                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Equipment Name & Expected Order Month
            //
            if (Equipname != null && Equipname != "" && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.Equipment == Equipname).Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.Expecteddate).ToList();
                }
                //ViewBag.allbyjt = byjt;
                ViewBag.allEquipname = Equipname;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["noep&exp"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["ep&exp"] = "SOT Report of";
                }
            }
            //
            //Equipment Name & byjt Chances
            //
            if (Equipname != null && Equipname != "" && byjt != -1)
            {
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.Equipment == Equipname).Where(m => m.BYJTChances == byjt).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                }
                ViewBag.allbyjt = byjt;
                ViewBag.allEquipname = Equipname;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["noep&byjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["ep&byjt"] = "SOT Report of";
                }
            }
            //
            //Expected order Month & byjt Chances
            //
            if (frommonth != "" && tomonth != null && tomonth != "" && byjt != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").Where(m => m.BYJTChances == byjt).OrderByDescending(m => m.Expecteddate).ToList();
                }
                ViewBag.allbyjt = byjt;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["noexp&byjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["exp&byjt"] = "SOT Report of";
                }

            }
            //
            //Channel partner Name & Equipment Name & Expected order month
            //
            if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frommonth != "" && tomonth != null && tomonth != "")
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.CPID == channame && s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").Where(m => m.Equipment == Equipname).OrderByDescending(m => m.Expecteddate).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }

                ViewBag.allcp = chpart.CPName;
                ViewBag.allEquipname = Equipname;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                TempData["cp&ep"] = null;
                TempData["exp&byjt"] = null;
                TempData["cp&exp"] = null;
                TempData["ep&byjt"] = null;
                TempData["ep&exp"] = null;
                TempData["cp&byjt"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["nocpepexp"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["cpepexp"] = "SOT Report of";
                }

                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Channel partner Name & Equipment Name & byjt Chances
            //
            if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && byjt != -1)
            {
                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.CPID == channame && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.BYJTChances == byjt).Where(m => m.Equipment == Equipname).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.SOTID).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }

                ViewBag.allcp = chpart.CPName;
                ViewBag.allbyjt = byjt;
                ViewBag.allEquipname = Equipname;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                TempData["cp&ep"] = null;
                TempData["exp&byjt"] = null;
                TempData["cp&exp"] = null;
                TempData["ep&byjt"] = null;
                TempData["ep&exp"] = null;
                TempData["cp&byjt"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["nocpepbyjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["cpepbyjt"] = "SOT Report of";
                }

                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Channel partner Name & Expected Order Month & byjt Chances
            //
            if (cpnam != null && cpnam != "" && frommonth != "" && tomonth != null && tomonth != "" && byjt != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.CPID == channame && s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.CPID == channame).Where(m => m.BYJTChances == byjt).Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").OrderByDescending(m => m.Expecteddate).ToList();
                }
                var chpart = db.ChannelPartners.Where(p => p.CPID == channame).SingleOrDefault();
                if (chpart == null)
                {
                    TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                    return RedirectToAction("SOTReport", "SOT");
                }

                ViewBag.allcp = chpart.CPName;
                ViewBag.allbyjt = byjt;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                TempData["cp&ep"] = null;
                TempData["exp&byjt"] = null;
                TempData["cp&exp"] = null;
                TempData["ep&byjt"] = null;
                TempData["ep&exp"] = null;
                TempData["cp&byjt"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["nocpexpbyjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["cpexpbyjt"] = "SOT Report of";
                }

                var cpid = chpart.CPID;
                ViewBag.OrganizationName = chpart.CPName;
                ViewBag.AddressLine1 = chpart.AddressLine1;
                ViewBag.AddressLine2 = chpart.AddressLine2;
                ViewBag.AddressLine3 = chpart.AddressLine3;
                ViewBag.City = chpart.City;
                ViewBag.Pincode = chpart.PinCode;
                ViewBag.State = chpart.State;
            }
            //
            //Equipment Name & Expected Order Month & byjt Chances
            //
            if (Equipname != null && Equipname != "" && frommonth != "" && tomonth != null && tomonth != "" && byjt != -1)
            {
                DateTime frmo = Convert.ToDateTime(monthtodate(frommonth));
                DateTime tomo = Convert.ToDateTime(monthtodate(tomonth));
                if (User.IsInRole("ZonalManager"))
                {
                    sotlist =(from s in db.SOT
                         from b in db.ChannelPartners
                             where s.Equipment == Equipname && s.Expecteddate >= frmo && s.Expecteddate <= tomo && s.BYJTChances == byjt && s.Islatestquo !=1 && s.BYJTChances !=0 && s.BYJTChances != 100 && s.Orderactive != "Project Delayed"
                              && s.Orderactive != "Project Dropped" &&  s.CPID == b.CPID && b.ZoneID == loginname.ZoneID
                              select s).ToList();
                }
                else
                {
                sotlist = db.SOT.Where(m => m.Expecteddate >= frmo && m.Expecteddate <= tomo).Where(m => m.Islatestquo != 1).Where(m => m.BYJTChances != 0).Where(m => m.BYJTChances != 100).Where(m => m.Orderactive != "Project Delayed").Where(m => m.Orderactive != "Project Dropped").Where(m => m.Equipment == Equipname).Where(m => m.BYJTChances == byjt).OrderByDescending(m => m.Expecteddate).ToList();
                }
                ViewBag.allbyjt = byjt;
                ViewBag.allEquipname = Equipname;
                //ViewBag.allexpmonth = expmonth;

                TempData["byjt"] = null;
                TempData["expmonth"] = null;
                TempData["euipment"] = null;
                TempData["cp&ep"] = null;
                TempData["exp&byjt"] = null;
                TempData["cp&exp"] = null;
                TempData["ep&byjt"] = null;
                TempData["ep&exp"] = null;
                TempData["cp&byjt"] = null;
                if (sotlist.Count == 0)
                {
                    TempData["noepexpbyjt"] = "No Data Exists for the selected value";
                }
                else
                {
                    TempData["epexpbyjt"] = "SOT Report of";
                }

            }
            //
            //Channel Partner Details Display
            //
                if (cpnam != null && cpnam != "")
                {
                    ViewBag.IsSearch = true;
                }
                ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0), "ProductModelID", "ProductModelName");
                return View(sotlist);
        }

        public ActionResult ExportData(string cpnam = null, string Equipname = null, string expmonth = null,string frm = null, string tom =null, int byjt = -1)
        {
            GridView gv = new GridView();

            if (cpnam != null || cpnam != "" || Equipname != null || Equipname != "" || frm != null || frm != "" || tom != null || tom != "" || byjt != -1)
            {
                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    //DataRow drt = dts.NewRow();
                    //drt[0] = cpnam;
                    //dts.Rows.Add(drt);
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner",typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam

                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }



                //
                //Channel partner Name
                //

                if (cpnam != null && cpnam != "")
                {
                    DataTable dts = new DataTable();

                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.CPID == channame) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam

                              });
                    var dt1 = dt.ToList();

                    DataRow drt = dts.NewRow();
                    drt[0] = "Channel Partner ";
                    drt[1] = cpnam;
                    dts.Rows.Add(drt);

                    gv.DataSource = dts;
                    gv.DataBind();
                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                //
                //Equipment Name
                //

                if (Equipname != null && Equipname != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));


                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

                //
                //Expected Order Month
                //

                if (frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));


                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,

                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                //
                //byjt Chances
                //

                if (byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));


                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }




                //
                //Channel partner Name, Equipment Name, Expected Order Month & byjt Chances
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.CPID == channame) && (s.BYJTChances == byjt) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped") && (s.Equipment == Equipname)
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                //
                //Channel partner Name & Equipment Name
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.CPID == channame) && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Expected order Month
                //

                if (cpnam != null && cpnam != "" && frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.CPID == channame) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }
                //
                //Channel partner Name & byjt Chances
                //

                if (cpnam != null && cpnam != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.CPID == channame) && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

                //
                //Equipment Name & Expected Order Month
                //

                if (Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.Equipment == Equipname) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

                //
                //Equipment Name & byjt Chances
                //

                if (Equipname != null && Equipname != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.Equipment == Equipname) && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }




                //
                //Expected order Month & byjt Chances
                //

                if (frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Equipment Name & Expected order month
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.CPID == channame) && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

                //
                //Channel partner Name & Equipment Name & byjt Chances
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.CPID == channame) && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Expected Order Month & byjt Chances
                //

                if (cpnam != null && cpnam != "" && frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.CPID == channame) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Equipment Name & Expected Order Month & byjt Chances
                //
                //if (Equipname != null && Equipname != "" && expmonth != null && expmonth != "" && byjt != -1)

                if (Equipname != null && Equipname != "" && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));
                    dts.Columns.Add("Channel Partner", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              where s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.Equipment == Equipname) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments,
                                  cpnam,
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dr[9] = d.cpnam;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

            }
            else
            {
                DataTable dts = new DataTable();
                dts.Columns.Add("Organisation Name", typeof(String));
                dts.Columns.Add("Quotation Number", typeof(String));
                dts.Columns.Add("Quote Date", typeof(String));
                dts.Columns.Add("Equipment", typeof(String));
                dts.Columns.Add("Quantity", typeof(String));
                dts.Columns.Add("Order Activeness", typeof(String));
                dts.Columns.Add("Byjt Chances", typeof(String));
                dts.Columns.Add("Expected Order Month", typeof(String));
                dts.Columns.Add("Comments", typeof(String));
                dts.Columns.Add("Channel Partner", typeof(String));

                DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                var dt = (from s in db.SOT
                          where s.Islatestquo != 1 && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                          orderby s.Expecteddate descending
                          select new
                          {
                              s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                              s.QGEquipGeneralData.QuotationNumber,
                              s.QGEquipGeneralData.QuotationDate,
                              s.Equipment,
                              s.Quantity,
                              s.Orderactive,
                              s.BYJTChances,
                              s.Expectedorder,
                              s.AdditionalComments,
                              cpnam,
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    DataRow dr = dts.NewRow();
                    dr[0] = d.OrganizationName;
                    dr[1] = d.QuotationNumber;
                    dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                    dr[3] = d.Equipment;
                    dr[4] = d.Quantity;
                    dr[5] = d.Orderactive;
                    dr[6] = d.BYJTChances;
                    dr[7] = d.Expectedorder;
                    dr[8] = d.AdditionalComments;
                    dr[9] = d.cpnam;
                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;

                gv.DataBind();
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=SOTReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("SOTReport");
        }

        public ActionResult ExportDatazonal(string cpnam = null, string Equipname = null, string expmonth = null, string frm = null, string tom = null, int byjt = -1)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID }).SingleOrDefault();
            GridView gv = new GridView();
            if (cpnam != null || cpnam != "" || Equipname != null || Equipname != "" || frm != null || frm != "" || tom != null || tom != "" || byjt != -1)
            {
                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;

                    gv.DataBind();
                }



                //
                //Channel partner Name
                //

                if (cpnam != null && cpnam != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.CPID == channame) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }




                //
                //Equipment Name
                //

                if (Equipname != null && Equipname != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));


                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Expected Order Month
                //

                if (frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));


                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //byjt Chances
                //

                if (byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));


                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }




                //
                //Channel partner Name, Equipment Name, Expected Order Month & byjt Chances
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.CPID == channame) && (s.BYJTChances == byjt) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped") && (s.Equipment == Equipname)
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Equipment Name
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.CPID == channame) && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Expected order Month
                //

                if (cpnam != null && cpnam != "" && frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.CPID == channame) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }




                //
                //Channel partner Name & byjt Chances
                //

                if (cpnam != null && cpnam != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.CPID == channame) && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }





                //
                //Equipment Name & Expected Order Month
                //

                if (Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.Equipment == Equipname) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Equipment Name & byjt Chances
                //

                if (Equipname != null && Equipname != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.Equipment == Equipname) && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }




                //
                //Expected order Month & byjt Chances
                //

                if (frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances == byjt) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Equipment Name & Expected order month
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && frm != null && frm != "" && tom != null && tom != "")
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.CPID == channame) && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Equipment Name & byjt Chances
                //

                if (cpnam != null && cpnam != "" && Equipname != null && Equipname != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.CPID == channame) && (s.Equipment == Equipname) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.SOTID descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Channel partner Name & Expected Order Month & byjt Chances
                //

                if (cpnam != null && cpnam != "" && frm != null && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.CPID == channame) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new

                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }



                //
                //Equipment Name & Expected Order Month & byjt Chances
                //
                //if (Equipname != null && Equipname != "" && expmonth != null && expmonth != "" && byjt != -1)

                if (Equipname != null && Equipname != "" && frm != "" && tom != null && tom != "" && byjt != -1)
                {
                    DataTable dts = new DataTable();
                    dts.Columns.Add("Organisation Name", typeof(String));
                    dts.Columns.Add("Quotation Number", typeof(String));
                    dts.Columns.Add("Quote Date", typeof(String));
                    dts.Columns.Add("Equipment", typeof(String));
                    dts.Columns.Add("Quantity", typeof(String));
                    dts.Columns.Add("Order Activeness", typeof(String));
                    dts.Columns.Add("Byjt Chances", typeof(String));
                    dts.Columns.Add("Expected Order Month", typeof(String));
                    dts.Columns.Add("Comments", typeof(String));

                    DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                    DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                    var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                    var dt = (from s in db.SOT
                              from b in db.ChannelPartners
                              where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.BYJTChances == byjt) && (s.Equipment == Equipname) && (s.Expecteddate >= frmo && s.Expecteddate <= tomo) && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                              orderby s.Expecteddate descending
                              select new
                              {
                                  s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                                  s.QGEquipGeneralData.QuotationNumber,
                                  s.QGEquipGeneralData.QuotationDate,
                                  s.Equipment,
                                  s.Quantity,
                                  s.Orderactive,
                                  s.BYJTChances,
                                  s.Expectedorder,
                                  s.AdditionalComments
                              });
                    var dt1 = dt.ToList();

                    foreach (var d in dt1)
                    {
                        DataRow dr = dts.NewRow();
                        dr[0] = d.OrganizationName;
                        dr[1] = d.QuotationNumber;
                        dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                        dr[3] = d.Equipment;
                        dr[4] = d.Quantity;
                        dr[5] = d.Orderactive;
                        dr[6] = d.BYJTChances;
                        dr[7] = d.Expectedorder;
                        dr[8] = d.AdditionalComments;
                        dts.Rows.Add(dr);
                    }
                    gv.DataSource = dts;
                    gv.DataBind();
                }

            }
            else
            {
                DataTable dts = new DataTable();
                dts.Columns.Add("Organisation Name", typeof(String));
                dts.Columns.Add("Quotation Number", typeof(String));
                dts.Columns.Add("Quote Date", typeof(String));
                dts.Columns.Add("Equipment", typeof(String));
                dts.Columns.Add("Quantity", typeof(String));
                dts.Columns.Add("Order Activeness", typeof(String));
                dts.Columns.Add("Byjt Chances", typeof(String));
                dts.Columns.Add("Expected Order Month", typeof(String));
                dts.Columns.Add("Comments", typeof(String));

                DateTime frmo = Convert.ToDateTime(monthtodate(frm));
                DateTime tomo = Convert.ToDateTime(monthtodate(tom));

                var channame = db.ChannelPartners.Where(m => m.CPName == cpnam).Select(m => m.CPID).SingleOrDefault();
                var dt = (from s in db.SOT
                          from b in db.ChannelPartners
                          where s.CPID == b.CPID && b.ZoneID == loginname.ZoneID && s.Islatestquo != 1 && (s.BYJTChances != 0) && (s.BYJTChances != 100) && (s.Orderactive != "Project Delayed") && (s.Orderactive != "Project Dropped")
                          orderby s.Expecteddate descending
                          select new
                          {
                              s.QGEquipGeneralData.MDBGeneralData.OrganizationName,
                              s.QGEquipGeneralData.QuotationNumber,
                              s.QGEquipGeneralData.QuotationDate,
                              s.Equipment,
                              s.Quantity,
                              s.Orderactive,
                              s.BYJTChances,
                              s.Expectedorder,
                              s.AdditionalComments
                          });
                var dt1 = dt.ToList();

                foreach (var d in dt1)
                {
                    DataRow dr = dts.NewRow();
                    dr[0] = d.OrganizationName;
                    dr[1] = d.QuotationNumber;
                    dr[2] = d.QuotationDate.Value.ToString("yyyy-MM-dd");
                    dr[3] = d.Equipment;
                    dr[4] = d.Quantity;
                    dr[5] = d.Orderactive;
                    dr[6] = d.BYJTChances;
                    dr[7] = d.Expectedorder;
                    dr[8] = d.AdditionalComments;
                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;

                gv.DataBind();
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=SOTReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("SOTReport");
        }

        public ActionResult SOTTempList()
        {
            int cpid = Convert.ToInt32(Session["logincpid"]);
            dynamic sotData = 0;
            var list = db.SOT_Temp_tbl.Where(m => m.CPID == cpid).OrderByDescending(m => m.TSOTID).ToList();//code modified by sneha 17 july 2017
            var list2 = list.GroupBy(s => s.QGID).Select(x => x.FirstOrDefault()).ToList();
            return View(list2);
           
        }
    } 
}