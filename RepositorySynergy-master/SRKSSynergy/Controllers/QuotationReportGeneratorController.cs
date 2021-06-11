using OfficeOpenXml;
using OfficeOpenXml.Style;
using SRKSSynergy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SRKSSynergy.Controllers
{
    public class QuotationReportGeneratorController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();

        //Server       
        FileInfo templateFile = new FileInfo(@"G:\SynergyTemplate\QuotationFormat.xlsx");
        String FileDir = @"G:\Synergy\" + System.DateTime.Now.ToString("yyyy");

        // Main Server System
        string connectionString = @"Data Source=SRKSSERVER01\SRKSSQL;Initial Catalog=SRKSSynergy;Integrated Security=SSPI;User ID=sa;Password=srksnov16$;MultipleActiveResultSets=True;";

        //public CellBorderMaker()
        //{
        // modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                        modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                        modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                        modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //}


        //
        // GET: /QuotationReportGenerator/

        public ActionResult Index()
        {
            return View();
        }

        public void QuotationReportGenerator() 
        {
            var result = (from qged in db.QGEquipGeneralData
                          join qgep in db.QGEquipPayment on qged.QGID equals qgep.QGID
                          join cp in db.ChannelPartners on qged.CPID equals cp.CPID
                          join mdbgd in db.MDBGeneralData on qged.MDBID equals mdbgd.MDBID
                          join qgetd in db.QGEquipTableData on qged.QGID equals qgetd.QGID
                          join pm in db.ProductModel on qgetd.ProductModelID equals pm.ProductModelID

                          select new
                          {
                              //qged.QGID,
                              QuotationNumber = qged.QuotationNumber,
                              //QuotationDate = 
                              qged.QuotationDate,
                              ProductVariety = qged.ProductVariety,

                              //qged.CPID,

                              ChannelPartnerName = cp.CPName,

                              //mdbgd.MDBID,

                              MillName = mdbgd.OrganizationName,
                              //Address = 
                              mdbgd.AddressLine1, //+"\r\n" +mdbgd.AddressLine2.ToString() ,
                              mdbgd.AddressLine2,

                              State = mdbgd.State,

                              //qgetd.MasterProductID,

                              Section = qgetd.MasterProductName,
                              Machine = qgetd.ProductName,

                              //qgetd.ProductModelID,

                              Model = pm.ProductModelName,
                              Quantity = qgetd.Quantity,
                              Remarks = qgep.annexure

                          }).ToList();
            //DataTable dt = ToDataTable(obj);

            DataTable dts = new DataTable();
            dts.Columns.Add("Slno", typeof(String));
            dts.Columns.Add("QuotationNumber", typeof(String));
            dts.Columns.Add("QuotationDate", typeof(String));
            dts.Columns.Add("ChannelPartnerName", typeof(String));
            dts.Columns.Add("MillName", typeof(String));
            dts.Columns.Add("Address", typeof(String));
            dts.Columns.Add("State", typeof(String));
            dts.Columns.Add("ProductVariety", typeof(String));
            dts.Columns.Add("Section", typeof(String));
            dts.Columns.Add("Machine", typeof(String));
            dts.Columns.Add("Model", typeof(String));
            dts.Columns.Add("Quanity", typeof(String));
            dts.Columns.Add("Remarks", typeof(String));

            int slno = 0;

            foreach (var qtn in result)
            {
                slno += 1;
                DataRow dr = dts.NewRow();

                dr[0] = slno;
                dr[1] = qtn.QuotationNumber;

                //DateTime dt = qtn.QuotationDate;//.ToString("MM/dd/yyyy");
                if (qtn.QuotationDate != null)
                {
                    DateTime dd = Convert.ToDateTime(qtn.QuotationDate);
                    string dd1 = dd.ToString("dd-MM-yyyy");
                    dr[2] = dd1;
                }
                else
                {
                    dr[2] = null;
                }


                dr[3] = qtn.ChannelPartnerName;
                dr[4] = qtn.MillName;
                dr[5] = qtn.AddressLine1 + "\r\n" + qtn.AddressLine2;
                dr[6] = qtn.State;
                dr[7] = qtn.ProductVariety;
                dr[8] = qtn.Section;
                dr[9] = qtn.Machine;
                dr[10] = qtn.Model;
                dr[11] = qtn.Quantity;
                dr[12] = qtn.Remarks;
                dts.Rows.Add(dr);
            }

            //Using the template of the header
            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

            // String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

            bool exists = System.IO.Directory.Exists(FileDir);

            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "QuotationFormat" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx"));
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "QuotationFormat" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";

                    //return View();
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
            int dynamicID = 2;

            for (int i = 0; i < dts.Rows.Count; i++)
            {
                worksheet.Cells["A" + dynamicID].Value = dts.Rows[i]["Slno"].ToString();//i.ToString();
                worksheet.Cells["B" + dynamicID].Value = dts.Rows[i]["QuotationNumber"].ToString();
                worksheet.Cells["C" + dynamicID].Value = dts.Rows[i]["QuotationDate"].ToString();
                worksheet.Cells["D" + dynamicID].Value = dts.Rows[i]["ChannelPartnerName"].ToString();
                worksheet.Cells["E" + dynamicID].Value = dts.Rows[i]["MillName"].ToString();
                worksheet.Cells["F" + dynamicID].Value = dts.Rows[i]["Address"].ToString();
                worksheet.Cells["G" + dynamicID].Value = dts.Rows[i]["State"].ToString();
                worksheet.Cells["H" + dynamicID].Value = dts.Rows[i]["ProductVariety"].ToString();
                worksheet.Cells["I" + dynamicID].Value = dts.Rows[i]["Section"].ToString();
                worksheet.Cells["J" + dynamicID].Value = dts.Rows[i]["Machine"].ToString();
                worksheet.Cells["K" + dynamicID].Value = dts.Rows[i]["Model"].ToString();
                worksheet.Cells["L" + dynamicID].Value = dts.Rows[i]["Quanity"].ToString();
                worksheet.Cells["M" + dynamicID].Value = dts.Rows[i]["Remarks"].ToString();



                dynamicID++;
            }

            //Load the datatable and set the number formats...

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            p.Save();
        }

        [HttpGet]
        public ActionResult TotalNumberOfQuotations()
        {

            ViewData["CPID"] = new SelectList(db.ChannelPartners.Where(m => m.IsDeleted == 0).ToList(), "CPID", "CPName");
            return View();
        }

        [HttpPost]
        public ActionResult TotalNumberOfQuotations(string fromdat, string todat, int CPID = 0) 
        {

            var result = (from qged  in db.QGEquipGeneralData
                          join qgep  in db.QGEquipPayment       on qged.QGID            equals qgep.QGID
                          join cp    in db.ChannelPartners      on qged.CPID            equals cp.CPID
                          join mdbgd in db.MDBGeneralData       on qged.MDBID           equals mdbgd.MDBID
                          join qgetd in db.QGEquipTableData     on qged.QGID            equals qgetd.QGID
                          join pm    in db.ProductModel         on qgetd.ProductModelID equals pm.ProductModelID

                          select new
                          {
                              //qged.QGID,
                              QuotationNumber = qged.QuotationNumber,
                              //QuotationDate = 
                              qged.QuotationDate,
                              ProductVariety = qged.ProductVariety,

                              //qged.CPID,

                              ChannelPartnerName = cp.CPName,

                              //mdbgd.MDBID,

                              MillName = mdbgd.OrganizationName,
                              //Address = 
                              mdbgd.AddressLine1, //+"\r\n" +mdbgd.AddressLine2.ToString() ,
                              mdbgd.AddressLine2,

                              State = mdbgd.State,

                              //qgetd.MasterProductID,

                              Section = qgetd.MasterProductName,
                              Machine = qgetd.ProductName,

                              //qgetd.ProductModelID,

                              Model = pm.ProductModelName,
                              Quantity = qgetd.Quantity,
                              Remarks = qgep.annexure

                          }).ToList();

            if (!string.IsNullOrEmpty(fromdat) && !string.IsNullOrEmpty(todat)) //date specified
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
                    return RedirectToAction("TotalNumberOfQuotations", "QuotationReportGenerator", null);//View();
                }

                if (CPID > 0)//date n specific cp
                {
                    result = (from qged in db.QGEquipGeneralData
                              join qgep in db.QGEquipPayment on qged.QGID equals qgep.QGID
                              join cp in db.ChannelPartners on qged.CPID equals cp.CPID
                              join mdbgd in db.MDBGeneralData on qged.MDBID equals mdbgd.MDBID
                              join qgetd in db.QGEquipTableData on qged.QGID equals qgetd.QGID
                              join pm in db.ProductModel on qgetd.ProductModelID equals pm.ProductModelID
                              
                              where( qged.CPID == CPID &&  (qged.QuotationDate>=frda && qged.QuotationDate<=toda) )
                              
                              select new
                              {
                                  QuotationNumber = qged.QuotationNumber,
                                  qged.QuotationDate,
                                  ProductVariety = qged.ProductVariety,
                                  ChannelPartnerName = cp.CPName,
                                  MillName = mdbgd.OrganizationName,
                                  mdbgd.AddressLine1, //+"\r\n" +mdbgd.AddressLine2.ToString() ,
                                  mdbgd.AddressLine2,
                                  State = mdbgd.State,
                                  Section = qgetd.MasterProductName,
                                  Machine = qgetd.ProductName,
                                  Model = pm.ProductModelName,
                                  Quantity = qgetd.Quantity,
                                  Remarks = qgep.annexure

                              } ).ToList();
                }
                else //date n all cp
                {
                    result = (from qged in db.QGEquipGeneralData
                              join qgep in db.QGEquipPayment on qged.QGID equals qgep.QGID
                              join cp in db.ChannelPartners on qged.CPID equals cp.CPID
                              join mdbgd in db.MDBGeneralData on qged.MDBID equals mdbgd.MDBID
                              join qgetd in db.QGEquipTableData on qged.QGID equals qgetd.QGID
                              join pm in db.ProductModel on qgetd.ProductModelID equals pm.ProductModelID

                              where (qged.QuotationDate >= frda && qged.QuotationDate <= toda)

                              select new
                              {
                                  QuotationNumber = qged.QuotationNumber,
                                  qged.QuotationDate,
                                  ProductVariety = qged.ProductVariety,
                                  ChannelPartnerName = cp.CPName,
                                  MillName = mdbgd.OrganizationName,
                                  mdbgd.AddressLine1, 
                                  mdbgd.AddressLine2,
                                  State = mdbgd.State,
                                  Section = qgetd.MasterProductName,
                                  Machine = qgetd.ProductName,
                                  Model = pm.ProductModelName,
                                  Quantity = qgetd.Quantity,
                                  Remarks = qgep.annexure

                              }).ToList();
                }
            }
            else if (string.IsNullOrEmpty(fromdat) && string.IsNullOrEmpty(todat))//no date specified
            {
                if (CPID > 0)// specific cp till date
                {
                    result = (from qged in db.QGEquipGeneralData
                              join qgep in db.QGEquipPayment on qged.QGID equals qgep.QGID
                              join cp in db.ChannelPartners on qged.CPID equals cp.CPID
                              join mdbgd in db.MDBGeneralData on qged.MDBID equals mdbgd.MDBID
                              join qgetd in db.QGEquipTableData on qged.QGID equals qgetd.QGID
                              join pm in db.ProductModel on qgetd.ProductModelID equals pm.ProductModelID

                              where (qged.CPID == CPID)

                              select new
                              {
                                  QuotationNumber = qged.QuotationNumber,
                                  qged.QuotationDate,
                                  ProductVariety = qged.ProductVariety,
                                  ChannelPartnerName = cp.CPName,
                                  MillName = mdbgd.OrganizationName,
                                  mdbgd.AddressLine1, //+"\r\n" +mdbgd.AddressLine2.ToString() ,
                                  mdbgd.AddressLine2,
                                  State = mdbgd.State,
                                  Section = qgetd.MasterProductName,
                                  Machine = qgetd.ProductName,
                                  Model = pm.ProductModelName,
                                  Quantity = qgetd.Quantity,
                                  Remarks = qgep.annexure

                              }).ToList();
                }
                else // all cp till date
                {
                    result = (from qged in db.QGEquipGeneralData
                              join qgep in db.QGEquipPayment on qged.QGID equals qgep.QGID
                              join cp in db.ChannelPartners on qged.CPID equals cp.CPID
                              join mdbgd in db.MDBGeneralData on qged.MDBID equals mdbgd.MDBID
                              join qgetd in db.QGEquipTableData on qged.QGID equals qgetd.QGID
                              join pm in db.ProductModel on qgetd.ProductModelID equals pm.ProductModelID

                              select new
                              {
                                  QuotationNumber = qged.QuotationNumber,
                                  qged.QuotationDate,
                                  ProductVariety = qged.ProductVariety,
                                  ChannelPartnerName = cp.CPName,
                                  MillName = mdbgd.OrganizationName,
                                  mdbgd.AddressLine1, //+"\r\n" +mdbgd.AddressLine2.ToString() ,
                                  mdbgd.AddressLine2,
                                  State = mdbgd.State,
                                  Section = qgetd.MasterProductName,
                                  Machine = qgetd.ProductName,
                                  Model = pm.ProductModelName,
                                  Quantity = qgetd.Quantity,
                                  Remarks = qgep.annexure

                              }).ToList();
                }
            }
            else 
            {
                TempData["NoData"] = "Please Specify To Date and From Date Properly.";
                return RedirectToAction("TotalNumberOfQuotations", "QuotationReportGenerator",null);//View();
            }

            DataTable dts = new DataTable();
            dts.Columns.Add("Slno", typeof(String));
            dts.Columns.Add("QuotationNumber", typeof(String));
            dts.Columns.Add("QuotationDate", typeof(String));
            dts.Columns.Add("ChannelPartnerName", typeof(String));
            dts.Columns.Add("MillName", typeof(String));
            dts.Columns.Add("Address", typeof(String));
            dts.Columns.Add("State", typeof(String));
            dts.Columns.Add("ProductVariety", typeof(String));
            dts.Columns.Add("Section", typeof(String));
            dts.Columns.Add("Machine", typeof(String));
            dts.Columns.Add("Model", typeof(String));
            dts.Columns.Add("Quanity", typeof(String));
            dts.Columns.Add("Remarks", typeof(String));

            int slno = 0;

            foreach (var qtn in result)
            {
                slno += 1;
                DataRow dr = dts.NewRow();

                dr[0] = slno;
                dr[1] = qtn.QuotationNumber;

                //DateTime dt = qtn.QuotationDate;//.ToString("MM/dd/yyyy");
                if (qtn.QuotationDate != null)
                {
                    DateTime dd = Convert.ToDateTime(qtn.QuotationDate);
                    string dd1 = dd.ToString("dd-MM-yyyy");
                    dr[2] = dd1;
                }
                else
                {
                    dr[2] = null;
                }


                dr[3] = qtn.ChannelPartnerName;
                dr[4] = qtn.MillName;
                dr[5] = qtn.AddressLine1 + "\r\n" + qtn.AddressLine2;
                dr[6] = qtn.State;
                dr[7] = qtn.ProductVariety;
                dr[8] = qtn.Section;
                dr[9] = qtn.Machine;
                dr[10] = qtn.Model;
                dr[11] = qtn.Quantity;
                dr[12] = qtn.Remarks;
                dts.Rows.Add(dr);
            }

            //Using the template of the header
            ExcelPackage templatep = new ExcelPackage(templateFile);
           ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

            // String FileDir = @"E:\Synergy\" + System.DateTime.Now.ToString("yyyy");

            bool exists = System.IO.Directory.Exists(FileDir);

            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "QuotationFormat" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx"));
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "QuotationFormat" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";

                    //return View();
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
            int dynamicID = 2;

            for (int i = 0; i < dts.Rows.Count; i++)
            {
                worksheet.Cells["A" + dynamicID].Value = dts.Rows[i]["Slno"].ToString();//i.ToString();
                worksheet.Cells["B" + dynamicID].Value = dts.Rows[i]["QuotationNumber"].ToString();
                worksheet.Cells["C" + dynamicID].Value = dts.Rows[i]["QuotationDate"].ToString();
                worksheet.Cells["D" + dynamicID].Value = dts.Rows[i]["ChannelPartnerName"].ToString();
                worksheet.Cells["E" + dynamicID].Value = dts.Rows[i]["MillName"].ToString();
                worksheet.Cells["F" + dynamicID].Value = dts.Rows[i]["Address"].ToString();
                worksheet.Cells["G" + dynamicID].Value = dts.Rows[i]["State"].ToString();
                worksheet.Cells["H" + dynamicID].Value = dts.Rows[i]["ProductVariety"].ToString();
                worksheet.Cells["I" + dynamicID].Value = dts.Rows[i]["Section"].ToString();
                worksheet.Cells["J" + dynamicID].Value = dts.Rows[i]["Machine"].ToString();
                worksheet.Cells["K" + dynamicID].Value = dts.Rows[i]["Model"].ToString();
                worksheet.Cells["L" + dynamicID].Value = dts.Rows[i]["Quanity"].ToString();
                worksheet.Cells["M" + dynamicID].Value = dts.Rows[i]["Remarks"].ToString();



                dynamicID++;
            }

            //Load the datatable and set the number formats...

           // worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            p.Save();
            return Download(p.File.FullName);
            //return RedirectToAction("TotalNumberOfQuotations", "QuotationReportGenerator",null);
        }

        public FileResult Download(string fileLocation)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
            string fileName = Path.GetFileName(fileLocation);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }

}
