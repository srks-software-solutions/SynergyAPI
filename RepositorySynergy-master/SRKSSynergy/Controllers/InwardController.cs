using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;
using Common.Logging;
using System.Net.Mail;

namespace SRKSSynergy.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class InwardController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json
        public JsonResult Autocomplete(string term)
        {

            var result = (from r in db.ProductModelSpare
                          where (r.ProductModelSparesDesc.ToLower().Contains(term.ToLower()) & r.ProductModelSparesDesc == r.ProductModelSparesDesc)
                          select new { r.ProductModelSparesDesc }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemDescription(int id)
        {
            var selectedRow = (from t in db.ProductModelSpare where t.ProductModelSparesID == id select t).SingleOrDefault();

            var jsonData = new
            {
                presentstock = selectedRow.BuhlerPresentStock,
                unitprice = selectedRow.AgentPrice,
                Desc = selectedRow.ProductModelSparesDesc

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //To get the Present Stock. It is not used now.
        [HttpGet]
        public JsonResult GetPresentStock(int id)
        {
            //var selectedRow = (from t in db.MinSpareEquipQuantity where t.ProductModelSparesID == id select t).FirstOrDefault();
            var query = from t in db.MinSpareEquipQuantity
                        where t.ProductModelSparesID == id
                        group t by t.ProductModelSparesID into g
                        orderby g.Sum(x => x.PresentStock)
                        select new
                        {
                            Presentstock = g.Sum(x => x.PresentStock)
                        };

            var jsonData = new
            {
                Presentstock = query.FirstOrDefault()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Equipment/
        const int pageSize = 1000;

        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string equipname = null)
        {
            //Paging and Sorting //
            IEnumerable<UserLoginData> productsname = db.InwardSpare.Where(
                    p => equipname == null
                    || p.ProductModelSpare.ProductModelSparesDesc.Contains(equipname)).Where(m => m.IsDeleted == 0).Where(m => m.QuantityOrdered != 0);

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    productsname = isAsc ? productsname.OrderBy(p => p.InwardMonth) : productsname.OrderByDescending(p => p.InwardMonth);
                    break;

                case 2:
                    productsname = isAsc ? productsname.OrderBy(p => p.InwardType) : productsname.OrderByDescending(p => p.InwardType);
                    break;

                case 3:
                    productsname = isAsc ? productsname.OrderBy(p => p.PONumber) : productsname.OrderByDescending(p => p.PONumber);
                    break;

                case 4:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSparesID) : productsname.OrderByDescending(p => p.ProductModelSparesID);
                    break;

                case 5:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSpare.ProductModelSparesDesc) : productsname.OrderByDescending(p => p.ProductModelSpare.ProductModelSparesDesc);
                    break;

                case 6:
                    productsname = isAsc ? productsname.OrderBy(p => p.QuantityOrdered) : productsname.OrderByDescending(p => p.QuantityOrdered);
                    break;

                case 7:
                    productsname = isAsc ? productsname.OrderBy(p => p.ProductModelSpare.AgentPrice) : productsname.OrderByDescending(p => p.ProductModelSpare.AgentPrice);
                    break;

                case 8:
                    productsname = isAsc ? productsname.OrderBy(p => p.TotalValue) : productsname.OrderByDescending(p => p.TotalValue);
                    break;
                case 9:
                    productsname = isAsc ? productsname.OrderBy(p => p.QuantityRemaining) : productsname.OrderByDescending(p => p.QuantityRemaining);
                    break;
                default:
                    productsname = isAsc ? productsname.OrderBy(p => p.InwardID) : productsname.OrderByDescending(p => p.InwardID);
                    break;
            }
            #endregion
            ViewBag.Search = equipname;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (equipname != null)
                ViewBag.IsSearch = true;
            return View(productsname);
            //return View(db.ProductModelSpare.ToList());
        }


        // GET: /Equipment/Details/5
        public ActionResult Details(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }


        [HttpGet]
        public ActionResult AddInward()
        {
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            ViewBag.ItenPre = db.InwardSpare.ToList().Count();
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInward(Inward Inw, int presentstock1=0, int presentstock2=0, int presentstock3=0, int presentstock4=0, int presentstock5=0)
        {
            if (Inw.InwardSpare1.InwardType != null)
            {
                Inw.InwardSpare1.QuantityRemaining = Inw.InwardSpare1.QuantityOrdered;
                db.InwardSpare.Add(Inw.InwardSpare1);

                //To update into Product model spare table
                var updprdmdlspare1 = db.ProductModelSpare.Where(m => m.ProductModelSparesID == Inw.InwardSpare1.ProductModelSparesID).Single();
                updprdmdlspare1.BuhlerPresentStock = presentstock1 + Inw.InwardSpare1.QuantityOrdered;
                db.Entry(updprdmdlspare1).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Inw.InwardSpare2.InwardType != null && Inw.InwardSpare2.PONumber != null && Inw.InwardSpare2.InwardMonth != null && Inw.InwardSpare2.QuantityOrdered != null)
            {
                Inw.InwardSpare2.QuantityRemaining = Inw.InwardSpare2.QuantityOrdered;
                db.InwardSpare.Add(Inw.InwardSpare2);
                db.SaveChanges();

                //To update into Product model spare table
                var updprdmdlspare2 = db.ProductModelSpare.Where(m => m.ProductModelSparesID == Inw.InwardSpare2.ProductModelSparesID).Single();
                updprdmdlspare2.BuhlerPresentStock = presentstock2 + Inw.InwardSpare2.QuantityOrdered;
                db.Entry(updprdmdlspare2).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Inw.InwardSpare3.InwardType != null && Inw.InwardSpare3.PONumber != null && Inw.InwardSpare3.InwardMonth != null && Inw.InwardSpare3.QuantityOrdered != null)
            {
                Inw.InwardSpare3.QuantityRemaining = Inw.InwardSpare3.QuantityOrdered;
                db.InwardSpare.Add(Inw.InwardSpare3);
                db.SaveChanges();

                //To update into Product model spare table
                var updprdmdlspare3 = db.ProductModelSpare.Where(m => m.ProductModelSparesID == Inw.InwardSpare3.ProductModelSparesID).Single();
                updprdmdlspare3.BuhlerPresentStock = presentstock3 + Inw.InwardSpare3.QuantityOrdered;
                db.Entry(updprdmdlspare3).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Inw.InwardSpare4.InwardType != null && Inw.InwardSpare4.PONumber != null && Inw.InwardSpare4.InwardMonth != null && Inw.InwardSpare4.QuantityOrdered != null)
            {
                Inw.InwardSpare4.QuantityRemaining = Inw.InwardSpare4.QuantityOrdered;
                db.InwardSpare.Add(Inw.InwardSpare4);
                db.SaveChanges();

                //To update into Product model spare table
                var updprdmdlspare4 = db.ProductModelSpare.Where(m => m.ProductModelSparesID == Inw.InwardSpare4.ProductModelSparesID).Single();
                updprdmdlspare4.BuhlerPresentStock = presentstock4 + Inw.InwardSpare4.QuantityOrdered;
                db.Entry(updprdmdlspare4).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (Inw.InwardSpare5.InwardType != null && Inw.InwardSpare5.PONumber != null && Inw.InwardSpare5.InwardMonth != null && Inw.InwardSpare5.QuantityOrdered != null)
            {
                Inw.InwardSpare5.QuantityRemaining = Inw.InwardSpare5.QuantityOrdered;
                db.InwardSpare.Add(Inw.InwardSpare5);
                db.SaveChanges();

                //To update into Product model spare table
                var updprdmdlspare5 = db.ProductModelSpare.Where(m => m.ProductModelSparesID == Inw.InwardSpare5.ProductModelSparesID).Single();
                updprdmdlspare5.BuhlerPresentStock = presentstock5 + Inw.InwardSpare5.QuantityOrdered;
                db.Entry(updprdmdlspare5).State = EntityState.Modified;
                db.SaveChanges();

            }
            else 
            {
                TempData["Inw"] = "Not valid";
            }

            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            return RedirectToAction("AddInward","Inward",null);
        }

        //
        // GET: /Equipment/Edit/5
        public ActionResult ModifyInward(int id = 0)
        {
            UserLoginData InwardSpare = db.InwardSpare.Find(id);
            if (InwardSpare == null)
            {
                return HttpNotFound();
            }
            var pdp = db.ProductModelSpare.Where(m => m.ProductModelSparesID == InwardSpare.ProductModelSparesID).Select(m => new { m.ProductModelSparesDesc, m.AgentPrice }).SingleOrDefault();
            ViewBag.ItemDesc = pdp.ProductModelSparesDesc;
            ViewBag.Agentprice = pdp.AgentPrice;

            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");

            return View(InwardSpare);
        }

        //
        // POST: /Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyInward(UserLoginData InwardSpare)
        {


            if (ModelState.IsValid)
            {
                InwardSpare.QuantityRemaining = InwardSpare.QuantityOrdered;
                db.Entry(InwardSpare).State = EntityState.Modified;
                db.SaveChanges();

                ////
                ////Emailing Dispatch details to Channel Partner
                ////
                //string inwno = InwardSpare.MFRNo;                   
                //var cpd = db.MFR.Where(m => m.MFRNumber == inwno).Select(m => m.CPID).Single();
                //int cpid = Convert.ToInt32(cpd);
                //var cpnm = db.CPContactPersonData.Where(m => m.CPID == cpid).Select(m => new { m.Title, m.FirstName, m.LastName, m.EmailID }).FirstOrDefault();

                //var email = cpnm.EmailID;
                //var name = cpnm.Title + " " + cpnm.FirstName + " " + cpnm.LastName;

                ////Channel Partner
                //int prdspare = InwardSpare.ProductModelSparesID;
                //var outmfr = db.ProductModelSpare.Where(m => m.ProductModelSparesID == prdspare).Single();            
                //var cusnme = db.MFR.Where(m => m.MFRNumber == inwno).Single();
                //var custnme = cusnme.MfrEnteredBy;
                //var itm = outmfr.ProductModelSparesName;
                //var desc = outmfr.ProductSpareNameDesc;
                //var qty = InwardSpare.QuantityOrdered;
                //var disdte = InwardSpare.InwardMonth;

                //try
                //{
                //    MailMessage mail = new MailMessage();
                //    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                //    mail.From = new MailAddress("manasa.aimit.2012@gmail.com");
                //    //mail.To.Add( email );
                //    mail.To.Add("manasa.nc@srkssolutions.com");
                //    // mail.To.Add("sharath.krishna@srkssolutions.com");
                //    //mail.CC.Add("sharath.krishna19@gmail.com");
                //    mail.CC.Add("manasa.aimit.2012@gmail.com");
                //    mail.Subject = "Outward MFR Dispatch Details";

                //    mail.Body = "<head>" +
                //            "<style type=\"text/css\">" +
                //            "td" +
                //                "{border-style: none;" +
                //                "border-color: inherit;" +
                //                "border-width: medium;" +
                //                "padding-top:1px;" +
                //                "padding-right:1px;" +
                //                "padding-left:1px;" +
                //                "color:black;" +
                //                "font-size:10.0pt;" +
                //                "font-weight:400;" +
                //                "font-style:normal;" +
                //                "text-decoration:none;" +
                //                "font-family:Arial, sans-serif;" +
                //                "text-align:general;" +
                //                "vertical-align:bottom;" +
                //                "white-space:nowrap;" +
                //                "}" +
                //            ".auto-style1 {" +
                //                "border-collapse: collapse;" +
                //                "border: 1px solid #000000;" +
                //            "}" +
                //            ".auto-style2 {" +
                //                "text-align: center;" +
                //                "font-size: x-large;" +
                //                "vertical-align: middle;" +
                //                "border-style: solid;" +
                //                "border-width: 1px;" +
                //            "}" +
                //            ".auto-style5 {" +
                //                "vertical-align: middle;" +
                //                "text-align: center;" +
                //                "border-style: solid;" +
                //                "border-width: 1px;" +
                //            "}" +
                //            ".auto-style6 {" +
                //                "vertical-align: middle;" +
                //                "text-align: center;" +
                //                "border-style: solid;" +
                //                "border-width: 1px;" +
                //                "background-color: #59C9FF;" +
                //            "}" +
                //            ".auto-style7 {" +
                //                "vertical-align: middle;" +
                //                "text-align: center;" +
                //                "font-family: Arial;" +
                //                "border-style: solid;" +
                //                "border-width: 1px;" +
                //                "background-color: #59C9FF;" +
                //            "}" +
                //            ".auto-style8 {" +
                //                "text-align: center;" +
                //                "font-size: small;" +
                //            "}" +
                //            ".auto-style9 {" +
                //                "color: #FF0000;" +
                //            "}" +
                //            "</style>" +
                //            "</head>" +
                //            "<body>" +

                //            "<p>&nbsp;</p>" +
                //            "<p>Dear " + name + ",</p>" +
                //            "<p>Outward Invoice Dispatch</p>" +
                //            "<p>&nbsp;</p>" +
                //            "<table class=\"auto-style1\" style=\"width: 100%; height: 79px\">" +
                //                "<tr>" +
                //                    "<td class=\"auto-style2\" colspan=\"10\">Outward MFR Dispatch</td>" +
                //                "</tr>" +
                //                "<tr>" +
                //                    "<td class=\"auto-style7\">MFR<br />" +
                //            "&nbsp;No</td>" +
                //                    "<td class=\"auto-style6\">Channel Partner</td>" +
                //                    "<td class=\"auto-style6\">Customer Name</td>" +
                //                    "<td class=\"auto-style6\">PartCode <br />" +
                //                    "</td>" +
                //                    "<td class=\"auto-style6\">Description</td>" +
                //                    "<td class=\"auto-style6\">Quantity<br />" +
                //                    "Number</td>" +
                //                    "<td class=\"auto-style6\">Dispatch Date<br />" +
                //                    "</td>" +
                //                    "<td class=\"auto-style6\">Courier Details<br />" +
                //                    "</td>" +
                //                "</tr>" +
                //                "<tr>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + inwno + "</td>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + name + "</td>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + custnme + "</td>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + itm + "</td>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + desc + "</td>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + qty + "</td>" +
                //                    "<td class=\"auto-style5\" style=\"height: 30px\">" + disdte + "</td>" +
                //                    //"<td class=\"auto-style5\" style=\"height: 30px\">" + rmk + "</td>" +


                //                "</tr>" +
                //            "</table>" +
                //            "<p>&nbsp;</p>" +
                //            "<p class=\"auto-style8\">Note :<span class=\"auto-style9\"> Please donot reply to this " +
                //            "Email ID, this is a Autogenerated Email</span></p>" +

                //            "</body>";



                //    mail.IsBodyHtml = true;
                //    SmtpServer.Port = 587;
                //    SmtpServer.Credentials = new System.Net.NetworkCredential("manasa.aimit.2012@gmail.com", "chandravathi");
                //    SmtpServer.EnableSsl = true;
                //    SmtpServer.Send(mail);
                //}
                //catch (Exception ex)
                //{
                //    //MessageBox.Show(ex.ToString());
                //}

                return RedirectToAction("Index");

            }
            ViewData["SparesID"] = new SelectList(db.ProductModelSpare.Where(m => m.IsDeleted == 0), "ProductModelSparesID", "ProductSpareNameDesc");
            ViewData["ItemDesc"] = db.InwardSpare.Where(m => m.InwardID == InwardSpare.InwardID).Select(m => m.ProductModelSpare.ProductSpareNameDesc);
            ViewData["Agentprice"] = db.InwardSpare.Where(m => m.InwardID == InwardSpare.InwardID).Select(m => m.ProductModelSpare.AgentPrice);
            return View(InwardSpare);
        }

        //
        // GET: /Equipment/Delete/5
        public ActionResult Delete(int id = 0)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            if (productmodel == null)
            {
                return HttpNotFound();
            }
            return View(productmodel);
        }

        //
        // POST: /Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductModel productmodel = db.ProductModel.Find(id);
            db.ProductModel.Remove(productmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        //Discontinue Equipment
        public ActionResult Discontinue(int id)
        {
            var deac = db.ProductModel.Where(m => m.ProductModelID == id).Where(m => m.IsDeleted == 0).Single();
            return View(deac);
        }

        //
        //Post: Discontinue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discontinue(ProductModel prdmd)
        {
            if (Request.Form["Yes"] != null)
            {
                prdmd.IsDeleted = 1;
                db.Entry(prdmd).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}