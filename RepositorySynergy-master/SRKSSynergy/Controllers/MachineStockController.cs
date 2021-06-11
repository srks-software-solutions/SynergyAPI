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
using System.Net.Mail;
using SRKSSynergy.Helpers;
using System.Web.UI.WebControls;
using System.Data;

namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class MachineStockController : Controller
    {

        private SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /MachineStock/

        //Json AutoComplete customer name
        public JsonResult Autocomplete(string term)
        {
            var result = (from m in db.MachineInventory
                          where m.MachineSerialNo.Contains(term) && m.IsDeleted == 0 && m.IsDispatched == 0
                          select new { m.MachineSerialNo }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult Autocompletec(string term)
        {
            var result = (from r in db.MDBGeneralData
                          where (r.OrganizationName.ToLower().Contains(term.ToLower()) && (r.IsDeleted == 0))
                          select new { r.OrganizationName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Json AutoComplete
        public JsonResult AutocompleteOA(string term)
        {
            var result = (from r in db.OAEquipGeneralData
                          where (r.OANumber.ToLower().Contains(term.ToLower()) && (r.ApprovalStatus == 1))
                          select new { r.OANumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Customer Name based on OANumber
        [HttpGet]
        public JsonResult GetOANumber(string id)
        {
            var selectedRow = (from t in db.OAEquipGeneralData where t.OANumber == id && t.ApprovalStatus==1 select t).SingleOrDefault();
            string jsonData = null;
            if (selectedRow != null)
            {
                jsonData = selectedRow.MDBGeneralData.OrganizationName;
            }

                //var jsonData = new
                //{
                //    OrganizationName = selectedRow.MDBGeneralData.OrganizationName,
                //};

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductModels(string id)
        {
            try
            {
                var selectedRow = (from t in db.OAEquipGeneralData where t.OANumber == id select t).SingleOrDefault();
                var prodModel = db.OAEquipTableData.Where(m => m.OAID == selectedRow.OAID).Select(m => new { m.ProductModelID, m.ProductModel.ProductModelName }).ToList();

                //SelectList jsonData = //new SelectList(prodModel, "ProductModelID", "ProductModelName");//ViewData["ProdModel"] = new SelectList(prodModel, "ProductModelID", "ProductModelName");
                return Json(prodModel, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /Equipment/
        const int pageSize = 1000;
        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string macslno = null)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.FirstName, m.MiddleName, m.LastName, m.Designation, m.CPID }).SingleOrDefault();
            ViewBag.LoginName = loginname.FirstName + " " + loginname.MiddleName + " " + loginname.LastName;
            //Paging and Sorting //
            IEnumerable<MachineInventory> macin = db.MachineInventory.Where(
               p => macslno == null
                  || p.MachineSerialNo.Contains(macslno)).Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    macin = isAsc ? macin.OrderBy(p => p.MachineInventoryID) : macin.OrderByDescending(p => p.MachineInventoryID);
                    break;

                case 2:
                    macin = isAsc ? macin.OrderBy(p => p.MachineSerialNo) : macin.OrderByDescending(p => p.MachineSerialNo);
                    break;

                case 3:
                    macin = isAsc ? macin.OrderBy(p => p.ProductModel.ProductModelName) : macin.OrderByDescending(p => p.ProductModel.ProductModelName);
                    break;

                case 4:
                    macin = isAsc ? macin.OrderBy(p => p.Type) : macin.OrderByDescending(p => p.Type);
                    break;

                case 5:
                    macin = isAsc ? macin.OrderBy(p => p.PlaceStocked) : macin.OrderByDescending(p => p.PlaceStocked);
                    break;

                case 6:
                    macin = isAsc ? macin.OrderBy(p => p.Remarks) : macin.OrderByDescending(p => p.Remarks);
                    break;

                case 7:
                    macin = isAsc ? macin.OrderBy(p => p.CustomerName) : macin.OrderByDescending(p => p.CustomerName);
                    break;

                default:
                    macin = isAsc ? macin.OrderBy(p => p.MachineInventoryID) : macin.OrderByDescending(p => p.MachineInventoryID);
                    break;
            }
            #endregion


            return View(macin);
        }

        public ActionResult ExportData()
        {

            GridView gv = new GridView();

            {
                DataTable dts = new DataTable();
                dts.Columns.Add("Model No", typeof(String));
                dts.Columns.Add("Machine Serial No", typeof(String));
                dts.Columns.Add("Type", typeof(String));
                dts.Columns.Add("Place Stocked", typeof(String));
                dts.Columns.Add("Remarks", typeof(String));

                var duplicate = (from s in db.MachineInventory
                                 where s.MachineInventoryID != null && s.IsDispatched == 0
                                 select new
                                 {
                                     s.ProductModel.ProductModelName,
                                     s.MachineSerialNo,
                                     s.Type,
                                     s.PlaceStocked,
                                     s.Remarks,
                                 });

                var dt1 = duplicate.ToList();
                foreach (var d in duplicate)
                {
                    DataRow dr = dts.NewRow();
                    dr[0] = d.ProductModelName;
                    dr[1] = d.MachineSerialNo;
                    dr[2] = d.Type;
                    dr[3] = d.PlaceStocked;
                    dr[4] = d.Remarks;


                    dts.Rows.Add(dr);
                }
                gv.DataSource = dts;
                gv.DataBind();
            }


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Index.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddMachineStock()
        {
            ViewData["ProductModelID"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddMachineStock(MachineInventory Macin)
        {
            var duplicate = (from s in db.MachineInventory
                             where s.MachineSerialNo == Macin.MachineSerialNo && s.IsDeleted==0
                             select s).ToList();
            //if (Request.Form["btnsave"] != null && Macin.CustomerName == null)
            //{

            if (ModelState.IsValid)
            {
                if (duplicate.Count > 0)
                {
                    //to check weather the serial no. machine is dispachted and deleted in machine dispatch
                    if (duplicate.Count == 1)
                    {
                        foreach(var dup in duplicate)
                        {
                            var delanddispatchcount = db.MachineDispatch.Where(m => m.MachineInventoryID == dup.MachineInventoryID).Where(m => m.IsDeleted == 1 && m.IsDispatched == 1).ToList();
                            if (delanddispatchcount.Count > 0)
                            {
                                db.MachineInventory.Add(Macin);
                                db.SaveChanges();
                                var mcnt = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelID == Macin.ProductModelID).ToList();
                                foreach (var m in mcnt)
                                {
                                    m.MachineCount = m.MachineCount + 1;
                                    db.Entry(m).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                TempData["Success"] = "Machine Added to Inventory Successfully!!!!";
                                return RedirectToAction("AddMachineStock");
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Duplicate = "Machine Serial Number already exists!!!!";
                    }
                }
                else
                {
                    db.MachineInventory.Add(Macin);
                    db.SaveChanges();
                    //To Get count of Machine
                    var mcnt = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelID == Macin.ProductModelID).ToList();
                    foreach (var m in mcnt)
                    {
                        m.MachineCount = m.MachineCount + 1;
                        db.Entry(m).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["Success"] = "Machine Added to Inventory Successfully!!!!";
                    return RedirectToAction("AddMachineStock");
                }
            }


            ViewData["ProductModelID"] = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName");
            return View(Macin);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditMachineStock(int id = 0)
        {
            MachineInventory Macin = db.MachineInventory.Find(id);
            if (Macin == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);
            return View(Macin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditMachineStock(MachineInventory Macin)
        {
            if (Macin.ProductModelID != 0)
            {
                db.Entry(Macin).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Machine Edited to Inventory Successfully!!!!";
                return RedirectToAction("Index");
            }
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);
            return View(Macin);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult DispatchMachineStock(int id = 0)
        {
            MachineInventory Macin = db.MachineInventory.Find(id);
            var prid = db.MachineInventory.Where(m => m.MachineInventoryID == id).Select(m => m.ProductModel.ProductModelName).FirstOrDefault();
            ViewBag.pnm = prid;
            if (Macin == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);

            return View(Macin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DispatchMachineStock(MachineInventory Macin, int? hiddnProdVal, string OANumber = null)
        {
            if (Macin.CustomerName != null)
            {
                int macinvid = Macin.MachineInventoryID;
                //Saving MDBID from Customer Name
                string custnm = Macin.CustomerName;
                //var mdbchk = db.MDBGeneralData.Where(m => m.OrganizationName == custnm).Where(m => m.IsDeleted == 0).Select(m => m.MDBID).Count();
                //if (mdbchk != 1)
                //{
                //    TempData["fail"] = "Double entry of Same customer exists!!!!";
                //    ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);
                //    return View(Macin);
                //}
                var Mdb = db.OAEquipGeneralData.Where(m => m.OANumber == OANumber).Select(m => new { m.MDBID, m.OAID }).SingleOrDefault();
                Macin.MDBID = Mdb.MDBID;
                //var oanum = db.OAEquipGeneralData.Where(m => m.MDBID == Mdb).Where(m => m.ApprovalStatus == 1).Select(m =>m.OAID).FirstOrDefault();
                //if (oanum == null)
                //{
                //    TempData["NoOA"] = "Order Acknowledgement has not been generated for the selected Customer!!!!";
                //    ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BYJT Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);
                //    return View(Macin);
                //}
                //var prdtid = db.OAEquipTableData.Where(m => m.OAID == Mdb.OAID).Select(m => m.ProductModelID).FirstOrDefault();

                if (Macin.ProductModelID == hiddnProdVal)
                {
                    db.Entry(Macin).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Success"] = "Machine Dispatch details added Successfully!!!!";
                    return RedirectToAction("MachineDispatch", "MachineDispatchDetails", new { MacinvID = macinvid, ProductModelID = hiddnProdVal });

                }
                else
                {
                    TempData["fail"] = "Equipment in Order does not match with Equipment your trying to dispatch";
                    ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);
                    return View(Macin);
                }

            }
            TempData["Failure"] = "Please enter Customer Name befor Dispatch!!!!";
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName", Macin.ProductModelID);
            return View(Macin);
        }

        //
        // GET: /MachineStock/Delete/5
        public ActionResult Delete(int id = 0)
        {
            MachineInventory Macinv = db.MachineInventory.Find(id);
            if (Macinv == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductModelID = new SelectList(db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter"), "ProductModelID", "ProductModelName", Macinv.ProductModelID);
            return View(Macinv);
        }

        //
        // POST: /Machine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Request.Form["Yes"] != null)
            {
                MachineInventory Macinv = db.MachineInventory.Find(id);
                db.MachineInventory.Remove(Macinv);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //
        //Get: /Quotation/MachineCount
        public ActionResult MachineCount()
        {
            var machine = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.Products.ProductName == "BSOH Sorter").ToList();

            //var maccount = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).ToList();
            //foreach (var s in maccount)
            //{
            //    if (s.ProductModel.ProductModelName == "CCD 240")
            //    {
            //        var maccount1 = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).Where(m => m.ProductModel.ProductModelName == "CCD 240").Count();
            //        ViewBag.CCD240 = maccount1;
            //    }
            //    else if (s.ProductModel.ProductModelName == "CCD 320")
            //    {
            //        var maccount2 = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).Where(m => m.ProductModel.ProductModelName == "CCD 320").Count();
            //        ViewBag.CCD320 = maccount2;
            //    }
            //    else if (s.ProductModel.ProductModelName == "CCD 320 Tertiary")
            //    {
            //        var maccount3 = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).Where(m => m.ProductModel.ProductModelName == "CCD 320 Tertiary").Count();
            //        ViewBag.CCD320T = maccount3;
            //    }
            //    else if (s.ProductModel.ProductModelName == "CMOS 160")
            //    {
            //        var maccount4 = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).Where(m => m.ProductModel.ProductModelName == "CMOS 160").Count();
            //        ViewBag.CMOS160 = maccount4;
            //    }
            //    else if (s.ProductModel.ProductModelName == "CMOS 240")
            //    {
            //        var maccount5 = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).Where(m => m.ProductModel.ProductModelName == "CMOS 240").Count();
            //        ViewBag.CMOS240 = maccount5;
            //    }
            //    else if (s.ProductModel.ProductModelName == "CMOS 320")
            //    {
            //        var maccount6 = db.MachineInventory.Where(m => m.IsDeleted == 0).Where(m => m.IsDispatched == 0).Where(m => m.ProductModel.ProductModelName == "CMOS 320").Count();
            //        ViewBag.CMOS320 = maccount6;
            //    }
            //}
            return View(machine);
        }

    }
}
