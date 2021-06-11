using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
//using Dapper;
using System.Linq;
using System.Web;
using SRKSSynergy.Models;
using System.Data;
using System.Net.Mail;

namespace SRKSSynergy.Chart
{
    public class mydataservice
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Only Stored Procedure and No argument passsing
//        public IEnumerable Listdata()
//        {
//            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
//            {
//                string Query = @"select cp.CPName,le.cpid, COUNT(*) as TotalCount from [LeadEnquiry] as le 
//                                inner join [ChannelPartners] as cp on cp.CPID=le.CPID group by le.CPID,cp.CPName";
//                // this is query which in in stored procedure
//                var list = con.Query<Outputclass>("SP_ChartData").AsEnumerable();

//                // List of type Outputclass which it will return .
//                return list;
//            }
//        }

        //Stored procedure and Passing Argument. (Lead Generation Chart)
        public IEnumerable Listdata1()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
            {
                var one = startDate;
                var two = enddate;
                SqlCommand cmd = new SqlCommand("SP_ChartData1", con);
                cmd.Parameters.AddWithValue("one", @one);
                cmd.Parameters.AddWithValue("two",@two);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<OutPutClass1> dataList = new List<OutPutClass1>();
                foreach (DataRow dtrow in dt.Rows)
                {
                    OutPutClass1 details = new OutPutClass1();
                    details.CPName = dtrow[0].ToString();
                    details.TotalCount = Convert.ToInt32(dtrow[2]);
                    dataList.Add(details);
                }
                return dataList;
            }
        }

        //Stored procedure and Passing Argument. (Open/Close Leads Chart)
        public IEnumerable Listdata3()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
            {
                var one = startDate;
                var two = enddate;
                SqlCommand cmd = new SqlCommand("SP_OpenCloseLeads", con);
                cmd.Parameters.AddWithValue("one", @one);
                cmd.Parameters.AddWithValue("two", @two);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<OutPutClass4> dataList = new List<OutPutClass4>();
                foreach (DataRow dtrow in dt.Rows)
                {
                    OutPutClass4 details = new OutPutClass4();
                    details.Name = dtrow[0].ToString();
                    details.LeadsGenerated = Convert.ToInt32(dtrow[2]);
                    details.OpenLeads = Convert.ToInt32(dtrow[3]);
                    details.ClosedLeads = Convert.ToInt32(dtrow[4]);
                    details.DroppedLeads = Convert.ToInt32(dtrow[5]);
                    details.QuotationGenerated = Convert.ToInt32(dtrow[6]);
                    dataList.Add(details);
                }
                return dataList;
            }
        }

        //Stored procedure and Passing Argument. (Quotation Generation Chart)
        public IEnumerable Listdata2()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
            {
                var one = startDate;
                var two = enddate;
                SqlCommand cmd = new SqlCommand("SP_QuotationChart", con);
                cmd.Parameters.AddWithValue("one", @one);
                cmd.Parameters.AddWithValue("two", @two);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<OutPutClass3> dataList = new List<OutPutClass3>();
                foreach (DataRow dtrow in dt.Rows)
                {
                    OutPutClass3 details = new OutPutClass3();
                    details.CPName = dtrow[0].ToString();
                    details.TotalCount = Convert.ToInt32(dtrow[2]);
                    dataList.Add(details);
                }
                return dataList;
            }
        }


        //Stored procedure and Passing Argument. (Orders Generation Chart)
        public IEnumerable Listdata4()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
            {
                var one = startDate;
                var two = enddate;
                SqlCommand cmd = new SqlCommand("SP_OrderGeneratedChart", con);
                cmd.Parameters.AddWithValue("one", @one);
                cmd.Parameters.AddWithValue("two", @two);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<OutPutClass2> dataList = new List<OutPutClass2>();
                foreach (DataRow dtrow in dt.Rows)
                {
                    OutPutClass2 details = new OutPutClass2();
                    details.CPName = dtrow[0].ToString();
                 //  details.TotalOrders = Convert.ToInt32(dtrow[2]);
                    details.TotalApproveCount = Convert.ToInt32(dtrow[2]);
                    details.TotalRejectCount = Convert.ToInt32(dtrow[3]);
                    dataList.Add(details);
                }
                return dataList;
            }
        }               
    }
}