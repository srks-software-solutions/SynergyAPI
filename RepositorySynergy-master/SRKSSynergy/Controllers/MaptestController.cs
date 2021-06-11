using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SRKSSynergy.Controllers
{
    public class MaptestController : Controller
    {
        //
        // GET: /Maptest/
        //string connectionString = @"Data Source=SRKSSERVER01\SRKSSQL;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";
        string connectionString = @"Data Source=USER-PC\SRKS;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";

        public ActionResult Maptest()
        {
            return View();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // This method is used to convert datatable to json string
        public string ConvertDataTabletoString()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string order = "select itle=City,lat=Latitude,lng=Longitude from [SRKSSynergy].[dbo].[MDBGeneralData] where Latitude IS NOT NULL ";
                using (SqlCommand cmd = new SqlCommand(order, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return serializer.Serialize(rows);
                }
            }
        }

    }
}
