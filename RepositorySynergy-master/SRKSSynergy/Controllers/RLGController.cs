using OfficeOpenXml;
using OfficeOpenXml.Style;
using SRKSSynergy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SRKSSynergy.Controllers
{
    public class RLGController : Controller
    {

        //Server
        //FileInfo templateFile = new FileInfo(@"H:\Synergy\Templates\RevisedLeadGenerationReport.xlsx");
        //String FileDir = @"H:\Synergy\ReportsList\RevisedLeadGenerationReports\" + System.DateTime.Now.ToString("yyyy-MM-dd");

        //Local
        FileInfo templateFile = new FileInfo(@"H:\Synergy\Templates\RevisedLeadGenerationReport.xlsx");
        String FileDir = @"H:\Synergy\ReportsList\RevisedLeadGenerationReports\" + System.DateTime.Now.ToString("yyyy-MM-dd");

        //ShivaKumar System
        //string connectionString = @"Data Source=SRKS_TECH-4-PC;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srks4$;MultipleActiveResultSets=True;";

        // Main Server System
        string connectionString = @"Data Source=SRKSSERVER01\SRKSSQL;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";

        // GET: /RLG/
        private SRKS_Synergy db = new SRKS_Synergy();

        public ActionResult RevisedLeadGenerationReport()
        {
            //Step1: Select Template File
            DateTime frda = DateTime.Now;
            //FileInfo templateFile = null;
            //templateFile = new FileInfo(@"E:\Synergy\Templates\RevisedLeadGenerationReport.xlsx");

            //Step2: Name worksheets in Template
            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews1 = templatep.Workbook.Worksheets["My Worksheet1"];
            ExcelWorksheet Templatews2 = templatep.Workbook.Worksheets["My Worksheet2"];
            ExcelWorksheet Templatews3 = templatep.Workbook.Worksheets["My Worksheet3"];
            ExcelWorksheet Templatews4 = templatep.Workbook.Worksheets["My Worksheet4"];
            ExcelWorksheet Templatews5 = templatep.Workbook.Worksheets["My Worksheet5"];

            //Step3: Create folder to save your Downloaded Report (Folder with Today's date) at E:\Synergy\
            //String FileDir = @"E:\Synergy\ReportsList\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            System.IO.Directory.CreateDirectory(FileDir);

            //Step4: Create Workbook
            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "RevisedLeadGenerationReport" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd")
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "RevisedLeadGenerationReport" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //" to " + toda.ToString("yyyy-MM-dd") +
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    return View();
                }
            }

            //Step5: Create Worksheets and apply templates
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet1 = null;
            ExcelWorksheet worksheet2 = null;
            ExcelWorksheet worksheet3 = null;
            ExcelWorksheet worksheet4 = null;
            ExcelWorksheet worksheet5 = null;

            try
            {
                worksheet1 = p.Workbook.Worksheets.Add("Summary", Templatews1);
                worksheet2 = p.Workbook.Worksheets.Add("SmartLine Region", Templatews2);
                worksheet3 = p.Workbook.Worksheets.Add("SmartLine Leads", Templatews3);
                worksheet4 = p.Workbook.Worksheets.Add("BSOH Region", Templatews4);
                worksheet5 = p.Workbook.Worksheets.Add("BSOH Leads", Templatews5);
            }
            catch
            {
                TempData["Excelopen"] = "Could not Create Worksheets!!!!";
                return View();
            }

            //Step 6: Get Data from DB.

            ////Step 6.1 : Create DataTable
            //DataTable DTNorth = new DataTable();
            //DTNorth.Columns.Add("Leads", typeof(int));
            //DTNorth.Columns.Add("Quotes", typeof(int));
            //DTNorth.Columns.Add("Orders", typeof(int));
            //DTNorth.Columns.Add("Despatches", typeof(int));

            List<Int32> LeadsNorth = new List<Int32>();

            //Step 6.1 :Get current Week. GET First and Last Day of current Week Functions ready.
            DateTime dt = DateTime.Now;
            //int currentweek = WeekOfYear(dt); //17
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            int currentweek = cal.GetWeekOfYear(dt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek); //18
            int dayofYear = DateTime.Now.DayOfYear;

            //Get FirstDate of this Week
            System.Globalization.CultureInfo ci = null;

            // Instantiate a culture using CreateSpecificCulture.
            ci = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            int OverallReportCount = db.OverallReport.Where(m => m.IsDeleted == 0).Count();
            if (OverallReportCount > 0)
            {
                int lastWeekInTable = db.OverallReport.OrderByDescending(i => i.CreatedOn).Select(m => m.WeekNumber).Take(1).SingleOrDefault();

                for (int j = lastWeekInTable; j <= currentweek; j++)
                {
                    DateTime FirstDate = FirstDateOfWeek(dt.Year, j, ci);
                    DateTime LastDate = FirstDate.AddDays(6);

                    //DateTime FirstDate1 = FirstDateOfWeek(dt.Year, j, ci);
                    //DateTime LastDate1 = FirstDate1.AddDays(6);

                    //string FirstDate2 = FirstDate1.ToString("dd-MM-yyyy");
                    //string LastDate2 = LastDate1.ToString("dd-MM-yyyy");

                    //DateTime FirstDate = Convert.ToDateTime(FirstDate2);
                    //DateTime LastDate = Convert.ToDateTime(LastDate2);

                    CalculationsInThisWeek(j, FirstDate, LastDate);
                }
            }
            else
            {
                for (int j = 1; j <= currentweek; j++)
                {
                    DateTime FirstDate = FirstDateOfWeek(dt.Year, j, ci);
                    DateTime LastDate = FirstDate.AddDays(6);

                    //DateTime FirstDate1 = FirstDateOfWeek(dt.Year, j, ci);
                    //DateTime LastDate1 = FirstDate1.AddDays(6);

                    //string FirstDate2 = FirstDate1.ToString("dd-MM-yyyy");
                    //string LastDate2 = LastDate1.ToString("dd-MM-yyyy");

                    //DateTime FirstDate = Convert.ToDateTime(FirstDate2);
                    //DateTime LastDate = Convert.ToDateTime(LastDate2);

                    CalculationsInThisWeek(j, FirstDate, LastDate);
                }
            }

            //GetData to Push it into Excel.
            DataTable RegionWiseData = new DataTable();
            DataTable LeadsWiseData = new DataTable();

            String TableSQL1 = "SELECT SUM([SLC]) as SLead , SUM([SOB]) as SOrder , SUM([SQS]) as SQuote , SUM([BLC]) as BLead, "
                             + "SUM([BQS]) as BQuote , SUM([BOB]) as BOrder, SUM([BMD]) as BMachine , CP.ZoneID , OVR.WeekNumber "
                             + "FROM [SRKSSynergy].[dbo].[OverallReport] as OVR "
                             + "Join [SRKSSynergy].[dbo].[ChannelPartners] as CP "
                             + "on OVR.CPID=CP.CPID  "
                             + "Group by OVR.WeekNumber , CP.ZoneID";
            SqlDataAdapter sqlAdapterRegion = new SqlDataAdapter(TableSQL1, connectionString);
            sqlAdapterRegion.Fill(RegionWiseData);

            String TableSQL2 = "SELECT SUM([SLC]) as SLead, SUM([BLC]) as BLead, CP.ZoneID as ZoneID,CP.CPName as CPName, OVR.WeekNumber as WeekNumber "
                             + "FROM [SRKSSynergy].[dbo].[OverallReport] as OVR "
                             + "Join [SRKSSynergy].[dbo].[ChannelPartners] as CP "
                             + "on OVR.CPID= CP.CPID "
                             + "Group by  CP.ZoneID, OVR.WeekNumber , CP.CPName ";
            SqlDataAdapter sqlAdapterLeads = new SqlDataAdapter(TableSQL2, connectionString);
            sqlAdapterLeads.Fill(LeadsWiseData);

            string[] ColVal = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD" };

            //   var data0 = from tsl in db.TargetSettingsLead
            //               where tsl.MachineType == "SmartLine Lead"
            //               //orderby tsl.TargetType
            //               select new { tsl.Targets,tsl.TargetType}  into x
            //               group x by x.TargetType;

            //   foreach (var data00 in data0)
            //   {
            //       int a = Convert.ToInt32(data00);
            //   }
            //var data1 = from ri in db.TargetSettingsLead
            //   where ri.MachineType == "SmartLine Lead"
            //   group ri by ri.TargetType into g
            //   select new {
            //       //Item = g.Key,
            //       Quantity = g.Sum(i => i.Targets)
            //   };

            //foreach (var data10 in data1)
            //{
            //    int a = Convert.ToInt32(data10);
            //}

            //To Push Targets into Summary Worksheet.
            #region
            DataTable SmartLineData = new DataTable();
            DataTable BSOHData = new DataTable();

            //String TableSQL3 = "SELECT SUM([Targets]) as Targets "
            //                 + "FROM [SRKSSynergy].[dbo].[TargetSettingsLead] as tsl "
            //                 + "where tsl.MachineType = 'SmartLine Lead' "
            //                 + "Group by  tsl.TargetType "
            //                 + "Order by tsl.TargetType";
            //SqlDataAdapter sqlAdapterSmartline = new SqlDataAdapter(TableSQL3, connectionString);
            //sqlAdapterSmartline.Fill(SmartLineData);

            //String TableSQL4 = "SELECT SUM([Targets]) as Targets "
            //                 + "FROM [SRKSSynergy].[dbo].[TargetSettingsLead] as tsl "
            //                 + "where tsl.MachineType = 'BSOH Lead' "
            //                 + "Group by  tsl.TargetType "
            //                 + "Order by tsl.TargetType";
            //SqlDataAdapter sqlAdapterBSOH = new SqlDataAdapter(TableSQL4, connectionString);
            //sqlAdapterBSOH.Fill(BSOHData);
            int currentmonthid = System.DateTime.Now.Month;

            if (SmartLineData.Rows.Count == 0)
            {
                int Row = 8;
                int colvalInt = 1;//that is column b in excel and alphabet array

                //12 Months
                for (int j = 1; j <= currentmonthid; j++)
                {
                    String TableSQL3 = "SELECT SUM([Targets]) as Targets "
                            + "FROM [SRKSSynergy].[dbo].[TargetSettingsLead] as tsl "
                            + "where tsl.MachineType = 'SmartLine Lead' and tsl.MonthId='" + j + "' "
                            + "Group by  tsl.TargetType "
                            + "Order by tsl.TargetType";
                    SqlDataAdapter sqlAdapterSmartline = new SqlDataAdapter(TableSQL3, connectionString);
                    sqlAdapterSmartline.Fill(SmartLineData);

                    if (SmartLineData.Rows.Count != 0)
                    {
                        int colvalintTest = colvalInt;
                        //int tleads = Convert.ToInt32(SmartLineData.Rows[0][0]);
                        if (SmartLineData.Rows[0][0] != null || SmartLineData.Rows[0][0] != "")
                        {
                            worksheet1.Cells[ColVal[colvalInt] + Row].Value = Convert.ToInt32(SmartLineData.Rows[0][0]);//for SLeads
                        }
                        else
                        {
                            worksheet1.Cells[ColVal[colvalInt] + Row].Value = 0;//for SLeads
                        }

                        if (SmartLineData.Rows[1][0] != null || SmartLineData.Rows[1][0] != "")
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 1)].Value = Convert.ToInt32(SmartLineData.Rows[1][0]);//for SQuotes
                        }
                        else
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 1)].Value = 0;//for SQuotes
                        }

                        if (SmartLineData.Rows[2][0] != null || SmartLineData.Rows[2][0] != "")
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 2)].Value = Convert.ToInt32(SmartLineData.Rows[2][0]);//for SOrders
                        }
                        else
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 2)].Value = 0;//for SOrders
                        }

                        // worksheet1.Cells[ColVal[colvalInt] + (Row + 3)].Value = Convert.ToInt32(SmartLineData.Rows[3][0]);//for SDespatches
                        colvalInt += 2;
                    }
                    SmartLineData.Clear();
                }
            }

            if (BSOHData.Rows.Count == 0)
            {
                int Row = 20;
                int colvalInt = 1;//that is column b in excel and alphabet array

                //12 Months
                for (int j = 1; j <= currentmonthid; j++)
                {
                    String TableSQL4 = "SELECT SUM([Targets]) as Targets "
                             + "FROM [SRKSSynergy].[dbo].[TargetSettingsLead] as tsl "
                             + "where tsl.MachineType = 'BSOH Lead' and tsl.MonthId='" + j + "'"
                             + "Group by  tsl.TargetType "
                             + "Order by tsl.TargetType";
                    SqlDataAdapter sqlAdapterBSOH = new SqlDataAdapter(TableSQL4, connectionString);
                    sqlAdapterBSOH.Fill(BSOHData);

                    if (BSOHData.Rows.Count != 0)
                    {
                        //int colvalintTest = colvalInt;
                        if (BSOHData.Rows[0][0] != null || BSOHData.Rows[0][0] != "")
                        {
                            worksheet1.Cells[ColVal[colvalInt] + Row].Value = Convert.ToInt32(BSOHData.Rows[0][0]);//for SLeads
                        }
                        else
                        {
                            worksheet1.Cells[ColVal[colvalInt] + Row].Value = 0;//for SLeads
                        }

                        if (BSOHData.Rows[1][0] != null || BSOHData.Rows[1][0] != "")
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 1)].Value = Convert.ToInt32(BSOHData.Rows[1][0]);//for SQuotes
                        }
                        else
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 1)].Value = 1;//for SQuotes
                        }

                        if (BSOHData.Rows[2][0] != null || BSOHData.Rows[2][0] != "")
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 2)].Value = Convert.ToInt32(BSOHData.Rows[2][0]);//for SOrders
                        }
                        else
                        {
                            worksheet1.Cells[ColVal[colvalInt] + (Row + 2)].Value = 0;//for SOrders
                        }

                        //worksheet1.Cells[ColVal[colvalInt] + (Row + 3)].Value = Convert.ToInt32(BSOHData.Rows[3][0]);//for SDespatches
                        colvalInt += 2;
                    }
                    BSOHData.Clear();
                }
            }

            #endregion

            //var data2 = db.TargetSettingsLead.Where(m=>m.MachineType == "SmartLine Lead").GroupBy(m=>m.TargetType).Select(m=>m.Targets)

            if (RegionWiseData.Rows.Count != 0)
            {
                #region Going  for SmartLine Region worksheet
                //Zones are 4, So 0 - 4 to shift cursor to Next cell in excel : Zone wise
                int dataCount = 0;// Like rows in DataTable
                int Row = 6;

                for (int i = 0; i < 4; i++)
                {

                    //Completing this Loop, completes one Zone
                    for (int j = 1; j <= currentweek; j++)
                    {
                        //-1 because j starts from 1(we want data in DataTable from 0) and +3 because we want column from D

                        //#region Testing
                        //string column = ColVal[j - 1 + 3];
                        //int data = Convert.ToInt32(RegionWiseData.Rows[j - 1][0]);
                        //#endregion

                        worksheet2.Cells[ColVal[j - 1 + 3] + Row].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][0]);//for SLeads
                        worksheet2.Cells[ColVal[j - 1 + 3] + (Row + 1)].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][1]);//for SQuotes
                        worksheet2.Cells[ColVal[j - 1 + 3] + (Row + 2)].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][2]);//for SOrders
                        worksheet2.Cells[ColVal[j - 1 + 3] + (Row + 3)].Value = 0;//for SDespatches

                        dataCount++;
                    }

                    Row += 7; // We want our Next Data in Next Zone ie Row 06,13,20,27 so +7.
                }
                #endregion

                #region Going  for BSOH Region worksheet
                //Zones are 4, So 0 - 4 to shift cursor to Next cell in excel : Zone wise
                dataCount = 0;// Like rows in DataTable
                Row = 6;

                for (int i = 0; i < 4; i++)
                {

                    //Completing this Loop, completes one Zone
                    for (int j = 1; j <= currentweek; j++)
                    {
                        //-1 because j starts from 1(we want data in DataTable from 0) and +3 because we want column from D

                        worksheet4.Cells[ColVal[j - 1 + 3] + Row].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][3]);//for BSOHLeads
                        worksheet4.Cells[ColVal[j - 1 + 3] + (Row + 1)].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][4]);//for BSOHQuotes
                        worksheet4.Cells[ColVal[j - 1 + 3] + (Row + 2)].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][5]);//for BSOHOrders
                        worksheet4.Cells[ColVal[j - 1 + 3] + (Row + 3)].Value = Convert.ToInt32(RegionWiseData.Rows[dataCount][6]);//for BSOHDespatches

                        dataCount++;
                    }

                    Row += 7; // We want our Next Data in Next Zone ie Row 06,13,20,27 so +7.
                }
                #endregion
            }

            if (LeadsWiseData.Rows.Count != 0)
            {
                int Row = 5;
                #region Going  for SmartLine Leads worksheet
                //Zones are 4, So 1 - 4 to shift cursor to Next cell in excel : Zone wise
                for (int i = 1; i <= 4; i++)
                {
                    //var x = (from r in LeadsWiseData.AsEnumerable()
                    //         where r["ZoneID"].ToString() == i.ToString()
                    //         select r["CPName"]).Distinct().ToList();

                    var x = (from r in db.ChannelPartners
                             where (r.ZoneID == i && r.IsDeleted==0)
                             select new { r.CPName, r.CPID }).ToList();

                    //to insert ZoneName after merging Cells
                    //worksheet3.Cells["B5:B7"].Merge = true;

                    worksheet3.Cells["B" + Row + ":B" + (Row + x.Count()) ].Merge = true;
                    worksheet3.Cells["B" + Row].Value = db.Zone.Where(m => m.ZoneID == i).Select(m => m.ZoneName).SingleOrDefault();

                    foreach (var a in x)
                    {
                        //Completing this loop completes One CP for all weeks
                        for (int j = 1; j <= currentweek; j++)
                        {
                            //insert CPName only once. i.e when j = 1
                            if (j == 1)
                            {
                                //worksheet3.Cells["C" + Row].Value = (from r in LeadsWiseData.AsEnumerable()
                                //                                     where r["ZoneID"] == i.ToString() && r["CPName"] == a && r["WeekNumber"] == j.ToString()
                                //                                     select r["CPName"]);

                                string year = DateTime.Now.Year.ToString();
                                worksheet3.Cells["C" + Row].Value = a.CPName.ToString();
                                var data = db.TargetSettingsLead.Where(m => m.IsDeleted == 0 && m.CPID == a.CPID && m.Year == year && m.MachineType == "SmartLine Lead");
                                if (data.Count() > 0)
                                {
                                    int targ = 0;
                                    foreach (var na in data)
                                    {
                                        targ = na.Targets;
                                        break;
                                    }
                                    worksheet3.Cells["D" + Row].Value = Convert.ToInt32(targ);
                                }
                                else
                                {
                                    worksheet3.Cells["D" + Row].Value = 0;
                                }
                            }
                            //worksheet3.Cells[ColVal[j - 1 + 4] + Row].Value = (from r in LeadsWiseData.AsEnumerable()
                            //                                                   where r["ZoneID"].ToString() == i.ToString() && r["CPName"] == a && r["WeekNumber"] == j.ToString()
                            //                                                   select r["BLead"]).ToString();
                            var da = (from r in LeadsWiseData.AsEnumerable()
                                      where r["ZoneID"].ToString() == i.ToString() && r["CPName"].ToString() == a.CPName.ToString() && r["WeekNumber"].ToString() == j.ToString()
                                      select r["BLead"]).Take(1).SingleOrDefault();

                            worksheet3.Cells[ColVal[j - 1 + 4] + Row].Value = Convert.ToInt32(da);
                            //dataCount++;
                        }
                        Row++;
                    }

                    //End of Zone
                    Row++; //Its like inserting a Blank row.
                }
                //Row = 5;
                Row++;

                //Insert Total Row
                worksheet3.Cells[ColVal[2] + Row].Value = "Total";

                //Insert Formula
                //Extra 1 for Total
                for (int j = 1; j <= currentweek + 1; j++)
                {
                    //worksheet3.Cells[ColVal[j - 1 + 3] + Row].Value = "=SUM(D5:D22)" ;
                    worksheet3.Cells[ColVal[j - 1 + 3] + Row].Formula = "=SUM(" + ColVal[j - 1 + 3] + 5 + ":" + ColVal[j - 1 + 3] + (Row - 1) + ")";

                }
                Row++;
                //Insert Monthly Total Row
                worksheet3.Cells[ColVal[2] + Row].Value = "Monthly Total";

                //Insert Formula
                //Extra 1 for Total
                int colj = 4;//E
                int[] MonthWidth = { 5, 4, 4, 5, 4, 4, 5, 4, 4, 5, 4, 4 };
                for (int j = 1; j <= 12; j++)
                {
                    //worksheet3.Cells[ColVal[j - 1 + 3] + Row].Value = "=SUM(D5:D22)" ;
                    string jj = ColVal[(colj)];
                    string jj1 = ColVal[(colj + MonthWidth[j - 1] - 1)];
                    worksheet3.Cells[ColVal[colj] + (Row) + ":" + ColVal[(colj + MonthWidth[j - 1] - 1)] + (Row)].Merge = true;
                    //worksheet3.Cells[ColVal[j - 1 + 4] + Row].Formula = "=SUM(" + ColVal[colj] + (Row-1) + ":" + ColVal[colj] + (Row-1) + ")";

                    colj += MonthWidth[j - 1];
                }

                //Loop to push Formula
                colj = 4;
                for (int j = 1; j <= 12; j++)
                {
                    //string jj = ColVal[(colj )];
                    //worksheet3.Cells[ColVal[colj] + (Row) + ":" + ColVal[(colj)] + (Row)].Merge = true;
                    //int TestRow = Row-1;
                    //string jj = ColVal[colj];
                    //string jj1 = ColVal[colj + MonthWidth[j - 1] - 1];
                    worksheet3.Cells[ColVal[colj] + Row].Formula = "=SUM(" + ColVal[colj] + (Row - 1) + ":" + ColVal[colj + MonthWidth[j - 1] - 1] + (Row - 1) + ")";

                    colj += MonthWidth[j - 1];
                }

                var finalCell = ColVal[currentweek + 4];

                worksheet3.Cells["B" + Row + ":BD" + (Row)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet3.Cells["B" + Row + ":BD" + (Row)].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet3.Cells["B" + Row + ":BD" + (Row)].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                worksheet3.Cells["B" + Row + ":BD" + (Row)].Style.Border.Top.Style = ExcelBorderStyle.Medium;

                worksheet3.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet3.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet3.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                worksheet3.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Top.Style = ExcelBorderStyle.Medium;

                worksheet3.Cells["B5:B" + (Row)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                #endregion

                Row = 5;
                #region Going  for BSOH Leads worksheet
                //Zones are 4, So 1 - 4 to shift cursor to Next cell in excel : Zone wise
                for (int i = 1; i <= 4; i++)
                {
                    //var x = (from r in LeadsWiseData.AsEnumerable()
                    //         where r["ZoneID"].ToString() == i.ToString()
                    //         select r["CPName"]).Distinct().ToList();

                    var x = (from r in db.ChannelPartners
                             where (r.ZoneID == i && r.IsDeleted==0)
                             select new { r.CPName, r.CPID }).ToList();

                    //to insert ZoneName after merging Cells
                    //worksheet3.Cells["B5:B7"].Merge = true;

                    worksheet5.Cells["B" + Row + ":B" + (Row + x.Count())].Merge = true;
                    worksheet5.Cells["B" + Row].Value = db.Zone.Where(m => m.ZoneID == i).Select(m => m.ZoneName).SingleOrDefault();

                    foreach (var a in x)
                    {
                        //Completing this loop completes One CP for all weeks
                        for (int j = 1; j <= currentweek; j++)
                        {
                            //insert CPName only once. i.e when j = 1
                            if (j == 1)
                            {
                                //worksheet3.Cells["C" + Row].Value = (from r in LeadsWiseData.AsEnumerable()
                                //                                     where r["ZoneID"] == i.ToString() && r["CPName"] == a && r["WeekNumber"] == j.ToString()
                                //                                     select r["CPName"]);

                                worksheet5.Cells["C" + Row].Value = a.CPName.ToString();
                                string year = DateTime.Now.Year.ToString();
                                var data = db.TargetSettingsLead.Where(m => m.IsDeleted == 0 && m.CPID == a.CPID && m.Year == year && m.MachineType == "BSOH Lead");
                                if (data.Count() > 0)
                                {
                                    int targ = 0;
                                    foreach (var na in data)
                                    {
                                        targ = na.Targets;
                                        break;
                                    }
                                    worksheet5.Cells["D" + Row].Value = Convert.ToInt32(targ);
                                }
                                else
                                {
                                    worksheet5.Cells["D" + Row].Value = 0;
                                }
                            }
                            //worksheet3.Cells[ColVal[j - 1 + 4] + Row].Value = (from r in LeadsWiseData.AsEnumerable()
                            //                                                   where r["ZoneID"].ToString() == i.ToString() && r["CPName"] == a && r["WeekNumber"] == j.ToString()
                            //                                                   select r["BLead"]).ToString();
                            var da = (from r in LeadsWiseData.AsEnumerable()
                                      where r["ZoneID"].ToString() == i.ToString() && r["CPName"].ToString() == a.CPName.ToString() && r["WeekNumber"].ToString() == j.ToString()
                                      select r["BLead"]).Take(1).SingleOrDefault();

                            worksheet5.Cells[ColVal[j - 1 + 4] + Row].Value = Convert.ToInt32(da);
                            //dataCount++;
                        }
                        Row++;
                    }
                    //End of Zone
                    Row++; //Its like inserting a Blank row.
                }
                Row++;

                //Insert Total Row
                worksheet5.Cells[ColVal[2] + Row].Value = "Total";

                //Insert Formula
                //Extra 1 for Total
                for (int j = 1; j <= currentweek + 1; j++)
                {
                    worksheet5.Cells[ColVal[j - 1 + 3] + Row].Formula = "=SUM(" + ColVal[j - 1 + 3] + 5 + ":" + ColVal[j - 1 + 3] + (Row - 1) + ")";
                }

                Row++;
                //Insert Monthly Total Row
                worksheet5.Cells[ColVal[2] + Row].Value = "Monthly Total";

                //Insert Formula
                //Extra 1 for Total
                colj = 4;//E
                for (int j = 1; j <= 12; j++)
                {
                    string jj = ColVal[(colj)];
                    string jj1 = ColVal[(colj + MonthWidth[j - 1] - 1)];
                    worksheet5.Cells[ColVal[colj] + (Row) + ":" + ColVal[(colj + MonthWidth[j - 1] - 1)] + (Row)].Merge = true;
                    colj += MonthWidth[j - 1];
                }

                //Loop to push Formula
                colj = 4;
                for (int j = 1; j <= 12; j++)
                {
                    worksheet5.Cells[ColVal[colj] + Row].Formula = "=SUM(" + ColVal[colj] + (Row - 1) + ":" + ColVal[colj + MonthWidth[j - 1] - 1] + (Row - 1) + ")";
                    colj += MonthWidth[j - 1];
                }

                finalCell = ColVal[currentweek + 4];

                worksheet5.Cells["B" + Row + ":BD" + (Row)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet5.Cells["B" + Row + ":BD" + (Row)].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet5.Cells["B" + Row + ":BD" + (Row)].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                worksheet5.Cells["B" + Row + ":BD" + (Row)].Style.Border.Top.Style = ExcelBorderStyle.Medium;

                worksheet5.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet5.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet5.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                worksheet5.Cells["B" + (Row - 1) + ":BD" + (Row - 1)].Style.Border.Top.Style = ExcelBorderStyle.Medium;

                worksheet5.Cells["B5:B" + (Row)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                #endregion

                //worksheet5.Cells["A1:H16"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                //worksheet5.Cells["E5:" + ColVal[currentweek] + currentweek].Style.Border.Top.Style = ExcelBorderStyle.Medium;

            }

            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "RevisedLeadGenerationReport" + frda.ToString("yyyy-MM-dd") + ".xlsx");
            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
            string Outgoingfile = "RevisedLeadGenerationReport" + frda.ToString("yyyy-MM-dd") + ".xlsx";
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

            return View();
        }


        //First Date Of Week
        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1 || firstWeek > 50)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        int CalculationsInThisWeek(int Week, DateTime startDate, DateTime endDate)
        {
            //Calculate Month and Year
            int month = 0, year = 0;
            if (Week == 1)
            {
                month = endDate.Month;
                year = endDate.Year;
            }
            else
            {
                month = startDate.Month;
                year = startDate.Year;
            }


            List<int> cpids = db.ChannelPartners.Where(m => m.IsDeleted == 0).Select(m => m.CPID).ToList();
            foreach (int cpid in cpids)
            {
                int SmartLineleads = 0, BSOHLeads = 0;//for Leads
                int SmartLineQuotes = 0, BSOHQuotes = 0;//for Quotes
                int SmartLineOrdersSubmitted = 0, BSOHOrdersSubmitted = 0;//for OrdersSubmitted
                int SmartLineMachineDispatch = 0, BSOHMachineDispatch = 0;//for MachinesDispatched

                ////Step 1: GetData
                #region For SmartLine & BSOH Leads

                var leads1 = (from lead in db.LeadEnquiryRevised
                              where lead.CPID == cpid && lead.CreatedOn >= startDate && lead.CreatedOn <= endDate
                              select lead).ToList();
                foreach (var s in leads1)
                {
                    if (s.MachineClassifier == true || s.MachineDestoner == true || s.MachineHullerSeperator == true || s.MachinePaddySeperator == true || s.MachineThickThinGrader == true || s.MachineWhitner == true || s.MachinePolisher == true)
                    {
                        SmartLineleads = SmartLineleads + 1;
                    }
                    else if (s.MachineSorter == true || s.TypeOfReq == "Sorters")
                    {
                        BSOHLeads = BSOHLeads + 1;
                    }
                }

                #endregion End of Leads

                #region For SmartLine & BSOH Quotes

                //SmartLineQuotes = db.RiceOAEquipGeneralData.Where(m => m.CPID == cpid && m.OAStatus == 1 && m.OADate >= startDate && m.OADate <= endDate).ToList().Count();
                SmartLineQuotes = db.RiceOAEquipGeneralData.Where(m => m.CPID == cpid).Where(m => m.OAStatus == 1).Where(m => m.OADate >= startDate).Where(m => m.OADate <= endDate).ToList().Count();

                //BSOHQuotes = db.OAEquipGeneralData.Where(m => m.CPID == cpid && m.OAStatus == 1 && m.OADate >= startDate && m.OADate <= endDate).ToList().Count();

                BSOHQuotes = db.OAEquipGeneralData.Where(m => m.CPID == cpid).Where(m => m.OAStatus == 1).Where(m => m.OADate >= startDate).Where(m => m.OADate <= endDate).ToList().Count();

                #endregion End of Leads

                #region For SmartLine & BSOH OrdersBooked

                //SmartLineOrdersSubmitted = db.OASAPDetails.Where(m => m.CPID == cpid && m.IsRice == 0 && m.SAPDate >= startDate && m.SAPDate <= endDate).ToList().Count();
                SmartLineOrdersSubmitted = db.OASAPDetails.Where(m => m.CPID == cpid).Where(m => m.IsRice == 0).Where(m => m.SAPDate >= startDate).Where(m => m.SAPDate <= endDate).ToList().Count();

                //BSOHOrdersSubmitted = db.OASAPDetails.Where(m => m.CPID == cpid && m.IsRice == 1 && m.SAPDate >= startDate && m.SAPDate <= endDate).ToList().Count();
                BSOHOrdersSubmitted = db.OASAPDetails.Where(m => m.CPID == cpid).Where(m => m.IsRice == 1).Where(m => m.SAPDate >= startDate).Where(m => m.SAPDate <= endDate).ToList().Count();

                #endregion

                #region For SmartLine & BSOH Machine Dispatch

                //BSOHMachineDispatch = db.MachineDispatch.Where(m => m.CPID == cpid && m.DispatchDate >= startDate && m.DispatchDate <= endDate).ToList().Count();
                BSOHMachineDispatch = db.MachineDispatch.Where(m => m.CPID == cpid).Where(m => m.DispatchDate >= startDate).Where(m => m.DispatchDate <= endDate).ToList().Count();

                #endregion

                //Step 2: Push Data into DB

                #region Check if row exists in DB for current week, if so Update else Insert

                var DataForWeek = db.OverallReport.Where(m => m.IsDeleted == 0).Where(m => m.WeekNumber == Week).Where(m => m.CPID == cpid).ToList();
                if (DataForWeek.Count() > 0)
                {
                    OverallReport or = db.OverallReport.Find(DataForWeek[0].ORID);

                    or.SLC = SmartLineleads;
                    or.BLC = BSOHLeads;
                    or.CPID = cpid;
                    or.StartDate = startDate;
                    or.EndDate = endDate;
                    or.WeekNumber = Week;
                    or.Month = month;
                    or.Year = year;
                    or.SQS = SmartLineQuotes;
                    or.BQS = BSOHQuotes;
                    or.SOB = SmartLineOrdersSubmitted;
                    or.BOB = BSOHOrdersSubmitted;
                    or.BMD = BSOHMachineDispatch;
                    or.CreatedOn = DateTime.Now;
                    or.CreatedBy = "1";
                    or.ModifiedOn = null;
                    or.ModifiedBy = null;
                    or.IsDeleted = 0;

                    db.Entry(or).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    OverallReport or = new OverallReport();
                    or.ORID = 0;
                    or.SLC = SmartLineleads;
                    or.BLC = BSOHLeads;
                    or.CPID = cpid;
                    or.StartDate = startDate;
                    or.EndDate = endDate;
                    or.WeekNumber = Week;
                    or.Month = month;
                    or.Year = year;
                    or.SQS = SmartLineQuotes;
                    or.BQS = BSOHQuotes;
                    or.SOB = SmartLineOrdersSubmitted;
                    or.BOB = BSOHOrdersSubmitted;
                    or.BMD = BSOHMachineDispatch;
                    or.CreatedOn = DateTime.Now;
                    or.CreatedBy = "1";
                    or.ModifiedOn = null;
                    or.ModifiedBy = null;
                    or.IsDeleted = 0;

                    db.OverallReport.Add(or);
                    db.SaveChanges();
                }
                #endregion
            }
            return 0;
        }
    }

}
