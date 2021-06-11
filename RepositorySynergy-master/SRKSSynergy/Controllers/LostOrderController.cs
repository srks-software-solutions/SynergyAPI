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
    public class LostOrderController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //
        // GET: /LostOrder/

        public ActionResult LOA(int qgid = 0, int tsotid = 0, int productid = 0, int prodmodelid = 0, int masterprodid=0)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            DateTime Loadat = System.DateTime.Now;
            ViewBag.ShowQuoteDate = Loadat.ToString("dd-MM-yyyy");
            //Channelpartner Name
            var channm = db.ChannelPartners.Where(m => m.CPID == loginname.CPID).Select(m => m.CPName).SingleOrDefault();
            var name  = channm;
            ViewData["Channelname"] = name;
            //Model & Quantity
            //var byjt = db.LostOrderAnalysis.Where(m => m.QGID == qgid ).Select(m => m.BYJTChances).SingleOrDefault();
            
            //var ModQty = db.QGEquipTableData.Where(m => m.QGID == qgid).Select(m => new { m.ProductModel.ProductModelName, m.Quantity }).ToList();       

            //To get Section and Polishing
            string MasterProdNameSection = null;
            string ProductNameMachine = null;
            var ModQty = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m=>m.ProductID==productid).Where(m=>m.ProductModelID==prodmodelid).Where(m=>m.MasterProductID==masterprodid).Where(m=>m.IsSOTStatus==2).Select(m => new { m.ProductModel.ProductModelName, m.Quantity, m.MasterProductID,m.ProductID }).ToList();       // to check only the Lost Order Details

            string ml = null;
            string qt = null;
            if (ModQty != null)
            {
                foreach (var mq in ModQty)
                {
                    //if (ml != mq.ProductModelName)
                    //{
                    //    if (ml != null)
                    //        ml = ml + "/" + mq.ProductModelName;
                    //    else
                    ml = mq.ProductModelName;
                    //}
                    //if (qt != mq.Quantity.ToString())
                    //{
                    //if (qt != null)
                    //    qt = qt + "/" + mq.Quantity.ToString();
                    //else
                    qt = mq.Quantity.ToString();
                    //}

                    var secname = db.MasterProducts.Where(m => m.MasterProductID == mq.MasterProductID).Select(m => m.MasterProductName).SingleOrDefault();
                    MasterProdNameSection = secname;

                    var machname = db.Products.Where(m => m.ProductID == mq.ProductID).Select(m => m.ProductName).SingleOrDefault();
                    ProductNameMachine = machname;
                }
            }

            ViewBag.section = MasterProdNameSection;
            ViewBag.machine = ProductNameMachine;

            ViewBag.tsotid = tsotid;
            ViewBag.Modl = ml;
            ViewBag.Qty = qt;
            //Quotation Number & Organization Name & Quotation Number & Phone Number
            var quot = db.QGEquipGeneralData.Where(m => m.QGID == qgid).Select(m => new { m.QuotationNumber, m.MDBGeneralData.OrganizationName, m.MDBGeneralData.Isd1,m.MDBGeneralData.Std1,m.MDBGeneralData.PhoneLL1}).Single();
            ViewBag.Qutonum = quot.QuotationNumber;
            ViewBag.OragName = quot.OrganizationName;
            var isd = quot.Isd1;
            var std = quot.Std1;
            var phone = quot.PhoneLL1;
            ViewBag.Number = isd + "-" + std + "-" + phone;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LOA(LOABarReport Lost, int quantity = 0,int qgid=0, int tsotid = 0)
        {
            int status = 2;
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();
           // var byjt = db.LostOrderAnalysis.Where(m => m.QGID == qgid).Select(m => m.BYJTChances).SingleOrDefault();

            int LOACount = db.LostOrderAnalysis.Where(m => m.QGID == qgid).Where(m => m.TSOTID == tsotid).Count();
            if (LOACount == 0)
            {
                if (Lost.Competitor != null && Lost.Competitor != "")
                {
                    //string[] serialno = Lost.EquipModel.Split('/');
                    //foreach (var s in serialno)
                    //{
                    // Lost.EquipModel = s;
                    Lost.qty = Convert.ToString(quantity);
                    Lost.TSOTID = tsotid;
                    Lost.CPID = loginname.CPID;
                    //  Lost.BYJTChances = byjt;
                    db.LostOrderAnalysis.Add(Lost);
                    db.SaveChanges();
                    //}
                }
            }

          //  var sotbyjt = db.QGEquipTableData.Where(m => m.QGID == qgid).ToList();  // old
            //new
            //var sotbyjt = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m=>m.IsSOTStatus==2).ToList();
           // var ejb4 = db.SOT.Where(m => m.QGID == qgid && m.Status == 2).ToList();
            //if (status == 2)
            //{
                //foreach (var qg in sotbyjt)
                //{
                //    //qg.IsStatus = 1;
                //    db.Entry(qg).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
            //}
         
            //var ejb4=db.SOT.Where(m => m.QGID == qgid && m.Status==2).ToList();
            //foreach (var qg in ejb4)
            //{
            //    qg.BYJTChances = 0;
            //    db.Entry(qg).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //new Code for SOTRM
            //var SOTRMList = db.SOTRM.Where(m => m.QGID == qgid && m.Status == 2).ToList();
            //foreach (var sotrmlist in SOTRMList)
            //{
            //    sotrmlist.BYJTChances = 0;
            //    db.Entry(sotrmlist).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            
            ////Remove Quotation from OA View When BYJT Chances becomes 0 that QuotStatus == 3
            //var quogen = db.QGEquipTableData.Where(m => m.QGID == qgid).Where(m=>m.IsSOTStatus==2).ToList();
                //.Where(m => m.IsStatus==1).ToList();
            //foreach (var qg in quogen)
            //{
            //   // qg.QuotStatus = 3;
            //    db.Entry(qg).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
                

            //int tsotid=db.SOT_Temp_tbl.Where(m => m.TSOTID == qgid).Select(m=>m.TSOTID).SingleOrDefault();
            //if (tsotid != 0)
            //{

            //Delete from SOT temp 

            SOT_Temp_tbl tempsot = db.SOT_Temp_tbl.Find(tsotid);
            if (tempsot != null)
            {
                db.SOT_Temp_tbl.Remove(tempsot);
                db.SaveChanges();
            }
            else
            {
                var list = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).Where(m => m.Islatestquo == 0).OrderByDescending(m => m.TSOTID).ToList();
                return RedirectToAction("SOTRMConversionList", "SOTRM", list);
            }
            //}

            //var list = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).ToList();
            //int count = list.Count;

            //foreach (var l in list)
            //{
            //    if (l.BYJTChances == 0)
            //    {
            //        return RedirectToAction("LOA", "LostOrder", new { qgid = l.QGID });
            //    }
            //    else if (l.BYJTChances == 100)
            //    {
            //        return RedirectToAction("OAGenerate", "OA", new { qgid1 = l.QGID });
            //    }
            //}

            //return RedirectToAction("Index","SOT",null);
           // var list = db.SOT_Temp_tbl.ToList();
            var list1 = db.SOT_Temp_tbl.Where(m => m.CPID == loginname.CPID).Where(m => m.Islatestquo == 0).OrderByDescending(m => m.TSOTID).ToList();
            return RedirectToAction("SOTRMConversionList", "SOTRM", list1);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}
