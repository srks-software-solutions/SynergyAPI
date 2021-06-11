using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Models;
using System.IO;

namespace SRKSSynergy.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ChannelPartnerController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();

        //Json AutoComplete
        public JsonResult Autocomplete(string term)
        {
            var result = (from r in db.ChannelPartners
                          where r.CPName.ToLower().Contains(term.ToLower())
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ChannelPartner/
        const int pageSize = 10;
        public ActionResult Index(int page = 1, int sortBy = 1, bool isAsc = true, string channm = null)
        {
            //Paging and Sorting //
            IEnumerable<ChannelPartners> chanpart = db.ChannelPartners.Where(
                    p => channm == null
                    || p.CPName.Contains(channm));

            #region Sorting
            switch (sortBy)
            {
                case 1:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CPID) : chanpart.OrderByDescending(p => p.CPID);
                    break;

                case 2:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CPUniqueID) : chanpart.OrderByDescending(p => p.CPUniqueID);
                    break;

                case 3:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CPName) : chanpart.OrderByDescending(p => p.CPName);
                    break;

                case 4:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CPOrgType) : chanpart.OrderByDescending(p => p.CPOrgType);
                    break;

                case 5:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.Address) : chanpart.OrderByDescending(p => p.Address);
                    break;

                case 6:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.City) : chanpart.OrderByDescending(p => p.City);
                    break;

                case 7:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.PinCode) : chanpart.OrderByDescending(p => p.PinCode);
                    break;

                case 8:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.State) : chanpart.OrderByDescending(p => p.State);
                    break;

                case 9:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.LandLine1) : chanpart.OrderByDescending(p => p.LandLine1);
                    break;

                case 10:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.Email) : chanpart.OrderByDescending(p => p.Email);
                    break;

                case 11:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.TIN) : chanpart.OrderByDescending(p => p.TIN);
                    break;

                case 12:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.postcour) : chanpart.OrderByDescending(p => p.postcour);
                    break;

                default:
                    chanpart = isAsc ? chanpart.OrderBy(p => p.CPID) : chanpart.OrderByDescending(p => p.CPID);
                    break;
            }
            
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)chanpart.Count() / pageSize);

            chanpart = chanpart
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.Search = channm;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (channm != null)
                ViewBag.IsSearch = true;
            return View(chanpart);
            //end of paging and sorting//
            //return View(db.ChannelPartners.ToList());
        }

        //
        // GET: /ChannelPartner/Details/5
        public ActionResult Details(int id = 0)
        {
            ChannelPartners channelpartners = db.ChannelPartners.Find(id);
            if (channelpartners == null)
            {
                return HttpNotFound();
            }
            return View(channelpartners);
        }

        //
        // GET: /ChannelPartner/Create
        public ActionResult Create()
        {
            var cpid = from s in db.ChannelPartners
                       select s;
            cpid = cpid.OrderByDescending(m => m.CPID);
            var check = cpid.FirstOrDefault();
            if (check == null)
            {
                ViewBag.Chanpart = "CP-"+ System.DateTime.Now.Year +"-001";
            }
            else
            {
                var cpi = cpid.Select(m => m.CPUniqueID).First();
                string[] split = cpi.Split('-');
                int k = Convert.ToInt32(split[2]);
                int ad = k + 1;
                string cpik = null;
                string len = ad.ToString();
                if (len.Length == 1)
                {
                    cpik = "00" + ad;
                }
                else if (len.Length == 2)
                {
                    cpik = "0" + ad;
                }
                else
                {
                    cpik = ad.ToString();
                }
                cpi = split[0] + "-" + System.DateTime.Now.Year + "-" + cpik;
                ViewBag.Chanpart = cpi;
            }
            ViewData["ZoneName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
            return View();
        }

        //
        // POST: /ChannelPartner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MainChannelPartner maindatabase, HttpPostedFileBase file)
        {
            if (maindatabase.ChannelPartners.CPName != null && maindatabase.CPContactPersonData1.FirstName != null && maindatabase.CPBankDetails1.BankName != null)
            {
                //if (file != null)
                //{
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/images/channelpartnerlogos"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                    maindatabase.ChannelPartners.Logo = pic;
                //}
                db.ChannelPartners.Add(maindatabase.ChannelPartners);
                db.SaveChanges();
                var cpid = from s in db.ChannelPartners
                           select s;
                cpid = cpid.OrderByDescending(m => m.CPID);
                int cpi = cpid.Select(m=>m.CPID).First();

                maindatabase.CPBankDetails1.CPID = cpi;
                db.CPBankDetails.Add(maindatabase.CPBankDetails1);
                db.SaveChanges();
                if (maindatabase.CPBankDetails2.BankName != null )
                {
                    maindatabase.CPBankDetails2.CPID = cpi;
                    db.CPBankDetails.Add(maindatabase.CPBankDetails2);
                    db.SaveChanges();
                }
                maindatabase.CPContactPersonData1.CPID = cpi;
                db.CPContactPersonData.Add(maindatabase.CPContactPersonData1);
                db.SaveChanges();
                if (maindatabase.CPContactPersonData2.FirstName != null)
                {
                    maindatabase.CPContactPersonData2.CPID = cpi;
                    db.CPContactPersonData.Add(maindatabase.CPContactPersonData2);
                    db.SaveChanges();
                }
                return RedirectToAction("CPUserRegistration", "Account", new { cpid = cpi });
            }
            ViewData["ZoneName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
            return View(maindatabase);
        }

        //
        // GET: /ChannelPartner/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                MainChannelPartner cp = new MainChannelPartner();
                var mdid = db.ChannelPartners.Find(id);
                cp.ChannelPartners = mdid;
                var mdid1 = db.CPBankDetails.Where(m => m.CPID == id).ToList();
                int cont = mdid1.Count;
                if (cont == 1)
                {
                    cp.CPBankDetails1 = mdid1[0];
                    cp.CPBankDetails2 = null;
                }
                else
                {
                    cp.CPBankDetails1 = mdid1[0];
                    cp.CPBankDetails2 = mdid1[1];
                }
                var mdid4 = db.CPContactPersonData.Where(m => m.CPID == id).ToList();
                int cntcp = mdid4.Count;
                if (cntcp == 1)
                {
                    cp.CPContactPersonData1 = mdid4[0];
                    cp.CPContactPersonData2 = null;
                }
                else
                {
                    cp.CPContactPersonData1 = mdid4[0];
                    cp.CPContactPersonData2 = mdid4[1];
                }
                ViewData["ZoneName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
                return View(cp);
            }
            else
            {
                ViewData["ZoneName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
                return View();
            }
        }

        //
        // POST: /ChannelPartner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MainChannelPartner mchannelPartner, HttpPostedFileBase file)
        {
            if (mchannelPartner.ChannelPartners.CPName != null && mchannelPartner.CPContactPersonData1.FirstName != null && mchannelPartner.CPBankDetails1.BankName != null)
            {
                try
                {
                    if (file != null)
                    {
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(
                                               Server.MapPath("~/images/channelpartnerlogos"), pic);
                        // file is uploaded
                        file.SaveAs(path);
                        mchannelPartner.ChannelPartners.Logo = pic;
                    }

                    var cpi = mchannelPartner.ChannelPartners.CPID;
                    db.Entry(mchannelPartner.ChannelPartners).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Entry(mchannelPartner.CPBankDetails1).State = EntityState.Modified;
                    db.SaveChanges();
                    if (mchannelPartner.CPBankDetails2.BankName != null)
                    {
                        db.Entry(mchannelPartner.CPBankDetails2).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (mchannelPartner.CPBankDetails2.BankName != null)
                    {
                        mchannelPartner.CPBankDetails2.CPID = cpi;
                        db.CPBankDetails.Add(mchannelPartner.CPBankDetails2);
                        db.SaveChanges();
                    }
                    db.Entry(mchannelPartner.CPContactPersonData1).State = EntityState.Modified;
                    db.SaveChanges();
                    if (mchannelPartner.CPContactPersonData2.CPCPDID != 0)
                    {
                        db.Entry(mchannelPartner.CPContactPersonData2).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (mchannelPartner.CPContactPersonData2.FirstName != null)
                    {
                        mchannelPartner.CPContactPersonData2.CPID = cpi;
                        db.CPContactPersonData.Add(mchannelPartner.CPContactPersonData2);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch
                {}
            }
            ViewData["ZoneName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
            return View(mchannelPartner);
        }

        //
        // GET: /ChannelPartner/Delete/5
        public ActionResult Delete(int id = 0)
        {
            ChannelPartners channelpartners = db.ChannelPartners.Find(id);
            if (channelpartners == null)
            {
                return HttpNotFound();
            }
            return View(channelpartners);
        }

        //
        // POST: /ChannelPartner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChannelPartners channelpartners = db.ChannelPartners.Find(id);
            db.ChannelPartners.Remove(channelpartners);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChannelPartnersIndexPagination(int? page,int? rowsPerPage) 
        {
            var objCP = db.ChannelPartners.Where(m => m.IsDeleted == 0).ToList();

            var dummyItems = objCP;//.GetRange(1, 20); //Enumerable.Range(1, 150).Select(x => "Item " + x);
            var pager = new Pager(dummyItems.Count(), page, rowsPerPage);

            var viewModel = new IndexViewModel
			{
				CPItems = dummyItems.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
				Pager = pager
			};

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}