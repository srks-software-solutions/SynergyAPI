using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SRKSSynergy.Models;
using SRKSSynergy.Controllers;
using System.Data.SqlClient;

namespace SRKSSynergy.Controllers
{
    public class SynergyController : Controller
    {
         SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /Synergy/
        
        public ActionResult Synergy()
        {
            //To Update SOTTemp Table
            //string connectionString = @"Data Source=SRKS-TECH3-PC\SRKS_TECH3;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";
            //System.Data.DataTable dts = new System.Data.DataTable();
            //var SOTTempDetails = db.SOT_Temp_tbl.ToList();
            //foreach (var sottemp in SOTTempDetails)
            //{
            //    string orderSql = "select m.MasterProductID,p.ProductID,pm.ProductModelID from MasterProducts as m inner join Products as p on m.MasterProductID=p.MasterProductID inner join ProductModel as pm on p.ProductID=pm.ProductID where pm.ProductModelName='" + sottemp.Equipment + "'";
            //    using (SqlConnection conn = new SqlConnection(connectionString))
            //    {
            //        conn.Open();
            //        SqlDataAdapter sda = new SqlDataAdapter(orderSql, conn);
            //        sda.Fill(dts);
            //    }
            //    SOT_Temp_tbl temp = db.SOT_Temp_tbl.Find(sottemp.TSOTID);
            //    if (dts.Rows.Count != 0)
            //    {
            //        temp.MasterProductID = Convert.ToInt32(dts.Rows[0][0]);
            //        temp.ProductID = Convert.ToInt32(dts.Rows[0][1]);
            //        temp.ProductModelID = Convert.ToInt32(dts.Rows[0][2]);
            //        db.Entry(temp).State = EntityState.Modified;
            //        db.SaveChanges();
            //    }
            //    dts.Rows.Clear();
            //}      

            //To Update QGEquipTableData
            //string connectionString = @"Data Source=SRKS-TECH3-PC\SRKS_TECH3;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";
            //System.Data.DataTable dts = new System.Data.DataTable();
            //var QGTableDetails = db.QGEquipTableData.Where(m=>m.ProductID==0).Where(m=>m.MasterProductID==0).ToList();
            //foreach (var qgtbl in QGTableDetails)
            //{
            //    string orderSql = "select m.MasterProductID,p.ProductID from MasterProducts as m   inner join Products as p on m.MasterProductID=p.MasterProductID   inner join ProductModel as pm on p.ProductID=pm.ProductID where pm.ProductModelID='" + qgtbl.ProductModelID+ "'";
            //    using (SqlConnection conn = new SqlConnection(connectionString))
            //    {
            //        conn.Open();
            //        SqlDataAdapter sda = new SqlDataAdapter(orderSql, conn);
            //        sda.Fill(dts);
            //    }
            //    QGEquipTableData QGEquipTableData = db.QGEquipTableData.Find(qgtbl.QGTBID);
            //    if (dts.Rows.Count != 0)
            //    {
            //        QGEquipTableData.MasterProductID = Convert.ToInt32(dts.Rows[0][0]);
            //        QGEquipTableData.ProductID = Convert.ToInt32(dts.Rows[0][1]);
            //        //temp.ProductModelID = Convert.ToInt32(dts.Rows[0][2]);
            //        db.Entry(QGEquipTableData).State = EntityState.Modified;
            //        db.SaveChanges();
            //    }
            //    dts.Rows.Clear();
            //}      
            TempData["userID"] = Session["userid"].ToString();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var enddate1 = startDate.AddMonths(1).AddDays(-1);
            var yesterday = DateTime.Today.AddDays(-1);

            int cpid = Convert.ToInt32(Session["logincpid"]);
            var list = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CPID != cpid).Where(m => m.NotifyDate >= yesterday && m.NotifyDate <= enddate).ToList();
           
            return View(list);
        }

    }
}
