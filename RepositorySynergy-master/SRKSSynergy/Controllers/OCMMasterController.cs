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

namespace SRKSSynergy.Controllers
{
    public class OCMMasterController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();
        // GET: /OCMMaster/

        #region //this is for OCM Grain Type
        public ActionResult OCMGrainType()
        {

            var obj = db.OCMGrainType.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.GrainTypeId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMGrainTypeAdd(string gname, string gsname)
        {
            string uid = Session["userid"].ToString();

            int count = db.OCMGrainType.Where(m => m.GrainName == gname && m.GrainShortName == gsname).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMGrainType obj = new Models.OCMGrainType();
                    obj.GrainName = gname;
                    obj.GrainShortName = gsname;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMGrainType.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMGrainTypeShow(string gid)
        {
            int g = Convert.ToInt32(gid);
            string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).SingleOrDefault();
            string gsname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainShortName).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }


        [HttpPost]
        public void OCMGrainTypeUpdate(string gid, string gname, string gsname)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMGrainType gtyp = db.OCMGrainType.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.GrainName = gname;
                gtyp.GrainShortName = gsname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMGrainTypeDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMGrainType gtyp = db.OCMGrainType.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region // this is for OCM Process
        public ActionResult OCMProcess()
        {

            var obj = db.OCMProcess.Where(m => m.IsDeleted == 0).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMProcessAdd(string pname, string psname, string gid)
        {
            string uid = Session["userid"].ToString();
            int g = Convert.ToInt32(gid);
            string pssname=db.OCMProcessname.Where(m => m.ProcessName == pname).Select(m => m.ProcessShortName).FirstOrDefault();
             string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).FirstOrDefault();
                string fname = gname + '-' + pname;
                int count = db.OCMProcess.Where(m => m.ProcessName == pname && m.ProcessShortName == pssname && m.GrainTypeId == g && m.GrainName == gname && m.PathName == fname).Count();
                if (count == 0)
                {
                    if (pname != "" || pname != null)
                    {

                        OCMProcess obj = new Models.OCMProcess();
                        obj.ProcessName = pname;
                        obj.ProcessShortName = pssname;
                        obj.GrainTypeId = g;
                        obj.GrainName = gname;
                        obj.PathName = fname;
                        obj.IsDeleted = 0;
                        obj.CreatedOn = DateTime.Now;
                        obj.CreatedBy = uid;
                        db.OCMProcess.Add(obj);
                        db.SaveChanges();
                    }
                }
        }

        [HttpPost]
        public string OCMProcessShow(string pid)
        {
            int p = Convert.ToInt32(pid);
            string pname = db.OCMProcess.Where(m => m.ProcessId == p).Select(m => m.ProcessName).SingleOrDefault();
            string psname = db.OCMProcess.Where(m => m.ProcessId == p).Select(m => m.ProcessShortName).SingleOrDefault();
            int gid = db.OCMProcess.Where(m => m.ProcessId == p).Select(m => m.GrainTypeId).SingleOrDefault();
            string gtid = gid.ToString();
            string result = pname + '%' + psname + '%' + gtid;

            return result.ToString();
        }


        [HttpPost]
        public void OCMProcessUpdate(string gid, string pid, string pname, string psname)
        {
            int g = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (p != 0)
            {
                OCMProcess gtyp = db.OCMProcess.Find(p);
                string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).FirstOrDefault();
                string fname = gname + '-' + pname;
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.GrainName = gname;
                gtyp.GrainTypeId = g;
                gtyp.ProcessName = pname;
                gtyp.ProcessShortName = db.OCMProcessname.Where(m => m.ProcessName == pname).Select(m => m.ProcessShortName).FirstOrDefault();
                gtyp.PathName = fname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMProcessDelete(string pid)
        {
            int p = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (p != 0)
            {
                OCMProcess gtyp = db.OCMProcess.Find(p);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //Json Autoload grain type
        [HttpPost]
        public JsonResult AutocompleteOCMProcess()
        {
            var result = db.OCMGrainType.Where(m => m.IsDeleted == 0).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutocompleteOCMProcessname()
        {

            var result = db.OCMProcessname.Where(m => m.IsDeleted == 0).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutocompleteOCMProcessbyid(string id)
        {
            int gid = Convert.ToInt32(id);
            var result = db.OCMProcess.Where(m => m.IsDeleted == 0).Where(m=>m.GrainTypeId==gid).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // this is for OCM Process Name

        [HttpPost]
        public void OCMProcessNameAdd(string pname, string psname, string gid)
        {

            string uid = Session["userid"].ToString();
            int g = Convert.ToInt32(gid);
            int count = db.OCMProcessname.Where(m => m.IsDeleted == 0).Where(m => m.ProcessName == pname && m.ProcessShortName == psname).Count();
            if (count == 0)
            {
                if (pname != "" || pname != null)
                {

                    OCMProcessname obj = new Models.OCMProcessname();
                    obj.ProcessName = pname;
                    obj.ProcessShortName = psname;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMProcessname.Add(obj);
                    db.SaveChanges();
                }
            }

        }

        [HttpPost]
        public string OCMProcessNameShow(string pid)
        {
            int p = Convert.ToInt32(pid);
            string pname = db.OCMProcessname.Where(m => m.ProcessNameId == p).Select(m => m.ProcessName).SingleOrDefault();
            string psname = db.OCMProcessname.Where(m => m.ProcessNameId == p).Select(m => m.ProcessShortName).SingleOrDefault();
           // int gid = db.OCMProcessname.Where(m => m.ProcessNameId == p).Select(m => m.GrainTypeId).SingleOrDefault();
           // string gtid = gid.ToString();
            string result = pname + '%' + psname ;

            return result.ToString();
        }


        [HttpPost]
        public void OCMProcessNameUpdate(string gid, string pid, string pname, string psname)
        {

            int g = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (p != 0)
            {
                OCMProcessname gtyp = db.OCMProcessname.Find(p);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.ProcessName = pname;
                gtyp.ProcessShortName = psname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }


        }

        [HttpPost]
        public void OCMProcessNameDelete(string pid)
        {


            int p = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (p != 0)
            {
                OCMProcessname gtyp = db.OCMProcessname.Find(p);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }


        }


        [HttpPost]
        public string brngdesgn()
        {
            StringBuilder html = new StringBuilder();
             var dsgn = db.OCMProcessname.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.ProcessNameId).ToList();
            int slno = 1;
            foreach (var item in dsgn)
            {
                html.Append("<tr><td style='width: 80px; text-align: center; font-family: Calibri; font-size: 18px'>" + slno + "</td><td style='width: 200px; text-align: center; font-family: Calibri; font-size: 18px'>" + item.ProcessName + "</td><td><button class='btn btn-primary glyphicon glyphicon-pencil' style='margin-right:20px' id='" + item.ProcessNameId + " editn' onclick='return editpgtyp(this);'> Edit </button><button class='btn btn-danger glyphicon glyphicon-trash'  id='" + item.ProcessNameId + " deln' onclick='return delpgtyp(this);'> Delete </button></td></td></tr>");
                slno++;
            }
            string r = html.ToString();
            return r;
        }
        #endregion

        #region // this is for OCM Pass
        public ActionResult OCMPass()
        {

            var obj = db.OCMPass.Where(m => m.IsDeleted == 0).ToList().OrderByDescending(m=>m.PassId);
            return View(obj);
        }

        [HttpPost]
        public void OCMPassAdd(string pname, string psname, string gid, string pcid,string mid)
        {
            string uid = Session["userid"].ToString();
            int g = Convert.ToInt32(gid);
            int pc = Convert.ToInt32(pcid);
            int proid = Convert.ToInt32(mid);
            string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).FirstOrDefault();
            string pcname = db.OCMProcess.Where(m => m.ProcessId == pc).Select(m => m.ProcessName).FirstOrDefault();
            string proname = db.Products.Where(m => m.ProductID == proid).Select(m => m.ProductName).FirstOrDefault();
            string fname = proname + '-' + gname + '-' + pcname + '-' + pname;
            string pssname=db.OCMPassname.Where(m => m.PassName == pname).Select(m => m.PassShortName).FirstOrDefault();
            int count = db.OCMPass.Where(m => m.IsDeleted == 0).Where(m => m.PassName == pname && m.PassShortName == pssname && m.GrainTypeId == g && m.GrainName == gname && m.ProcessId == pc && m.ProcessName == pcname && m.PathName == fname).Count();
            if (count == 0)
            {
                if (pname != "" || pname != null)
                {

                    OCMPass obj = new Models.OCMPass();
                    obj.PassName = pname;
                    obj.PassShortName = pssname;
                    obj.GrainTypeId = g;
                    obj.GrainName = gname;
                    obj.ProcessId = pc;
                    obj.ProcessName = pcname;
                    obj.PathName = fname;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    obj.ProductId = proid;
                    obj.ProductName = proname;
                    db.OCMPass.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMPassShow(string paid)
        {
            int pa = Convert.ToInt32(paid);
            string pname = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.PassName).SingleOrDefault();
            string psname = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.PassShortName).SingleOrDefault();
            int gid = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.GrainTypeId).SingleOrDefault();
            int pcid = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.ProcessId).SingleOrDefault();
            string gtid = gid.ToString();
            int mid = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.ProductId).SingleOrDefault();
            string result = pname + '%' + psname + '%' + gtid + '%' + pcid + '%'+ mid;

            return result.ToString();
        }

        [HttpPost]
        public void OCMPassUpdate(string gid, string pcid, string paid, string pname, string psname,string mid)
        {
            int g = Convert.ToInt32(gid);
            int pc = Convert.ToInt32(pcid);
            int pa = Convert.ToInt32(paid);
            string uid = Session["userid"].ToString();
            int proid = Convert.ToInt32(mid);
            if (pa != 0)
            {
                OCMPass gtyp = db.OCMPass.Find(pa);
                string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).FirstOrDefault();
                string pcname = db.OCMProcess.Where(m => m.ProcessId == pc).Select(m => m.ProcessName).FirstOrDefault();
                string proname = db.Products.Where(m => m.ProductID == proid).Select(m => m.ProductName).FirstOrDefault();
                string fname = proname+'-'+gname + '-' + pcname + '-' + pname;
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.GrainName = gname;
                gtyp.GrainTypeId = g;
                gtyp.ProcessName = pcname;
                gtyp.ProcessId = pc;
                gtyp.PassName = pname;
                gtyp.PassShortName = db.OCMPassname.Where(m => m.PassName == pname).Select(m => m.PassShortName).FirstOrDefault();
                gtyp.ProductName = proname;
                gtyp.PathName = fname;
                gtyp.ProductId = Convert.ToInt32(mid);
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMPPassDelete(string pid)
        {
            int pa = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (pa != 0)
            {
                OCMPass gtyp = db.OCMPass.Find(pa);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //Json Autoselect process option
        [HttpPost]
        public JsonResult AutocompleteOCMPassprocess(string gid)
        {
            int g = Convert.ToInt32(gid);
            var result = db.OCMProcess.Where(m => m.IsDeleted == 0).Where(m=>m.GrainTypeId == g).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutocompleteOCMPassname()
        {

            var result = db.OCMPassname.Where(m => m.IsDeleted == 0).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutocompleteOCMPassProcessId(string id,int proid = 0)
        {
            int psid = Convert.ToInt32(id);
            var result = db.OCMPass.Where(m => m.IsDeleted == 0).Where(m => m.ProcessId == 99999).ToList();
            if (proid == 0)
            {
                 result = db.OCMPass.Where(m => m.IsDeleted == 0).Where(m => m.ProcessId == psid).ToList();
            }
            else {
                result = db.OCMPass.Where(m => m.IsDeleted == 0).Where(m => m.ProcessId == psid).ToList();
                 //result = db.OCMPass.Where(m => m.IsDeleted == 0).Where(m => m.ProcessId == psid).Where(m => m.ProductId == proid).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutocompleteOCMMachineTypeInProcess()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = db.Products.Where(m => m.IsDeleted == 0).Where(m=>m.ProductID==10 || m.ProductID==12).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // this is for OCM Pass Name

        [HttpPost]
        public void OCMPassNameAdd(string pname, string psname, string gid)
        {

            string uid = Session["userid"].ToString();
            int g = Convert.ToInt32(gid);
            int count = db.OCMPassname.Where(m=>m.IsDeleted==0).Where(m=>m.PassName==pname && m.PassShortName==psname).Count();
            if(count==0)
            {
                if (pname != "" || pname != null)
                {

                    OCMPassname obj = new Models.OCMPassname();
                    obj.PassName = pname;
                    obj.PassShortName = psname;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMPassname.Add(obj);
                    db.SaveChanges();
                }
            }

        }

        [HttpPost]
        public string OCMPassNameShow(string pid)
        {
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPassname.Where(m => m.PassNameId == p).Select(m => m.PassName).SingleOrDefault();
            string psname = db.OCMPassname.Where(m => m.PassNameId == p).Select(m => m.PassShortName).SingleOrDefault();
            string result = pname + '%' + psname;

            return result.ToString();
        }


        [HttpPost]
        public void OCMPassNameUpdate(string gid, string pid, string pname, string psname)
        {


            int p = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (p != 0)
            {
                OCMPassname gtyp = db.OCMPassname.Find(p);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassName = pname;
                gtyp.PassShortName = psname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }


        }

        [HttpPost]
        public void OCMPassNameDelete(string pid)
        {


            int p = Convert.ToInt32(pid);
            string uid = Session["userid"].ToString();
            if (p != 0)
            {
                OCMPassname gtyp = db.OCMPassname.Find(p);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }


        }

        [HttpPost]
        public string brngdesgn1()
        {
            StringBuilder html = new StringBuilder();
            var dsgn = db.OCMPassname.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.PassNameId).ToList();
            int slno = 1;
            foreach (var item in dsgn)
            {
                html.Append("<tr><td style='width: 80px; text-align: center; font-family: Calibri; font-size: 18px'>" + slno + "</td><td style='width: 200px; text-align: center; font-family: Calibri; font-size: 18px'>" + item.PassName + "</td><td><button class='btn btn-primary glyphicon glyphicon-pencil' style='margin-right:20px' id='" + item.PassNameId + " editn' onclick='return editpgtyp(this);'> Edit </button><button class='btn btn-danger glyphicon glyphicon-trash'  id='" + item.PassNameId + " deln' onclick='return delpgtyp(this);'> Delete </button></td></td></tr>");
                slno++;
            }
            string r = html.ToString();
            return r;
        }
        #endregion

        #region // this is for OCM Capacity TPH
        public ActionResult OCMCapacityTPH()
        {

            var obj = db.OCMCapacityTPH.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.CapacityTPHId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMCapacityTPHAdd(string gname, string gsname)
        {
            string uid = Session["userid"].ToString();
            int count = db.OCMCapacityTPH.Where(m => m.IsDeleted == 0).Where(m => m.CapacityTPHName == gname).Where(m => m.CapacityTPHShortName == gsname).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMCapacityTPH obj = new Models.OCMCapacityTPH();
                    obj.CapacityTPHName = gname;
                    obj.CapacityTPHShortName = gsname;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMCapacityTPH.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMCapacityTPHShow(string gid)
        {
            int g = Convert.ToInt32(gid);
            string gname = db.OCMCapacityTPH.Where(m => m.CapacityTPHId == g).Select(m => m.CapacityTPHName).SingleOrDefault();
            string gsname = db.OCMCapacityTPH.Where(m => m.CapacityTPHId == g).Select(m => m.CapacityTPHShortName).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMCapacityTPHUpdate(string gid, string gname, string gsname)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMCapacityTPH gtyp = db.OCMCapacityTPH.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.CapacityTPHName = gname;
                gtyp.CapacityTPHShortName = gsname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMCapacityTPHDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMCapacityTPH gtyp = db.OCMCapacityTPH.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region // this is for OCM Capacity KW
        public ActionResult OCMCapacityKW()
        {

            var obj = db.OCMCapacityKW.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.CapacityKWId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMCapacityKWAdd(string cname, string gid, string pcid,string psid,string tphid,string promid)
        {
            string uid = Session["userid"].ToString();
            int ps = Convert.ToInt32(psid);
            int g = db.OCMPass.Where(m => m.PassId == ps).Select(m => m.GrainTypeId).FirstOrDefault();
            int pc = db.OCMPass.Where(m => m.PassId == ps).Select(m => m.ProcessId).FirstOrDefault();
            int t = Convert.ToInt32(tphid);
            int pid=Convert.ToInt32(promid);
            string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).FirstOrDefault();
            string pcname = db.OCMProcess.Where(m => m.ProcessId == pc).Select(m => m.ProcessName).FirstOrDefault();
            string psname = db.OCMPass.Where(m => m.PassId == ps).Select(m => m.PassName).FirstOrDefault();
            string tph = db.OCMCapacityTPH.Where(m => m.CapacityTPHId == t).Select(m => m.CapacityTPHName).FirstOrDefault();
            string proname = db.OCMPass.Where(m => m.PassId == ps).Select(m => m.ProductName).FirstOrDefault();
            string fname = proname+'-'+gname + '-' + pcname + '-' + psname +'-'+ tph;
            int count = db.OCMCapacityKW.Where(m => m.IsDeleted == 0).Where(m => m.GrainTypeId == g && m.GrainName == gname && m.ProcessId == pc && m.ProcessName == pcname && m.PassId == ps && m.PassName == psname && m.CapacityTPHId == t && m.CapacityTPHName == tph && m.PathName == fname).Count();
            if (count == 0)
            {
                if (cname != "" || cname != null)
                {

                    OCMCapacityKW obj = new Models.OCMCapacityKW();
                    obj.CapacityKWName = cname;
                    obj.GrainTypeId = g;
                    obj.GrainName = gname;
                    obj.ProcessId = pc;
                    obj.ProcessName = pcname;
                    obj.PassId = ps;
                    obj.PassName = psname;
                    obj.CapacityTPHId = t;
                    obj.CapacityTPHName = tph;
                    obj.PathName = fname;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    obj.ProductModelID = promid;
                    obj.ProductModelName = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelID == pid).Select(m => m.ProductModelName).FirstOrDefault();
                    db.OCMCapacityKW.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMCapacityKWShow(string cid)
        {
            int c = Convert.ToInt32(cid);
            string pname = db.OCMCapacityKW.Where(m => m.CapacityKWId == c).Select(m => m.CapacityKWName).SingleOrDefault();
            int gid = db.OCMCapacityKW.Where(m => m.CapacityKWId == c).Select(m => m.GrainTypeId).SingleOrDefault();
            int pcid = db.OCMCapacityKW.Where(m => m.CapacityKWId == c).Select(m => m.ProcessId).SingleOrDefault();
            int psid = db.OCMCapacityKW.Where(m => m.CapacityKWId == c).Select(m => m.PassId).SingleOrDefault();
            int tid = db.OCMCapacityKW.Where(m => m.CapacityKWId == c).Select(m => m.CapacityTPHId).SingleOrDefault();
            string proid=db.OCMCapacityKW.Where(m => m.CapacityKWId == c).Select(m => m.ProductModelID).SingleOrDefault();
            string result = pname + '%' + gid.ToString() + '%' + pcid.ToString() + '%' + psid.ToString()+'%'+tid.ToString()+'%'+proid;

            return result.ToString();
        }

        [HttpPost]
        public void OCMCapacityKWUpdate(string gid, string pcid, string paid,string tpid,string cid, string pname,string promid)
        {
            int pa = Convert.ToInt32(paid);
            int g = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.GrainTypeId).FirstOrDefault();
            int pc = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.ProcessId).FirstOrDefault();
            int pid = Convert.ToInt32(promid);
            int t = Convert.ToInt32(tpid);
            int c = Convert.ToInt32(cid);
            string uid = Session["userid"].ToString();
            if (c != 0)
            {
                OCMCapacityKW gtyp = db.OCMCapacityKW.Find(c);
                string gname = db.OCMGrainType.Where(m => m.GrainTypeId == g).Select(m => m.GrainName).FirstOrDefault();
                string pcname = db.OCMProcess.Where(m => m.ProcessId == pc).Select(m => m.ProcessName).FirstOrDefault();
                string psname = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.PassName).FirstOrDefault();
                string tph = db.OCMCapacityTPH.Where(m => m.CapacityTPHId == t).Select(m => m.CapacityTPHName).FirstOrDefault();
                string proname = db.OCMPass.Where(m => m.PassId == pa).Select(m => m.ProductName).FirstOrDefault();
                string fname = proname + '-' + gname + '-' + pcname + '-' + psname + '-' + tph;
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.GrainTypeId = g;
                gtyp.GrainName = gname;
                gtyp.ProcessId = pc;
                gtyp.ProcessName = pcname;
                gtyp.PassId = pa;
                gtyp.PassName = psname;
                gtyp.CapacityTPHId = t;
                gtyp.CapacityTPHName = tph;
                gtyp.PathName = fname;
                gtyp.CapacityKWName = pname;
                gtyp.ProductModelID = promid;
                gtyp.ProductModelName = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductModelID == pid).Select(m => m.ProductModelName).FirstOrDefault();
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMCapacityKWDelete(string cid)
        {
            int c = Convert.ToInt32(cid);
            string uid = Session["userid"].ToString();
            if (c != 0)
            {
                OCMCapacityKW gtyp = db.OCMCapacityKW.Find(c);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //Json Autoselect pass option
        [HttpPost]
        public JsonResult AutocompleteOCMCapacityPass( string pid)
        {
            int p = Convert.ToInt32(pid);
            var result = db.OCMPass.Where(m => m.IsDeleted == 0).Where(m=>m.ProcessId==p).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Json Autoselect tph option
        [HttpPost]
        public JsonResult AutocompleteOCMCapacityTPH()
        {
            var result = db.OCMCapacityTPH.Where(m => m.IsDeleted == 0).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutocompleteModel(string name)
        {
            string mname = name.Trim();
            db.Configuration.ProxyCreationEnabled = false;
            int pid = db.Products.Where(m=>m.IsDeleted==0).Where(m=>m.ProductName==mname).Select(m=>m.ProductID).FirstOrDefault();
            var result = db.ProductModel.Where(m => m.IsDeleted == 0).Where(m => m.ProductID == pid).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutocompleteOCMCapacityKW(string proid = null,string pid=null)
        {
            int paid = Convert.ToInt32(pid);
            var result = db.OCMCapacityKW.Where(m => m.IsDeleted == 0).Where(m=>m.PassId==paid).Where(m => m.ProductModelID == proid).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region //this is for OCM Drive MS
        public ActionResult OCMDriveMS()
        {

            var obj = db.OCMDriveMS.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.DriveMSId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMDriveMSAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMDriveMS.Where(m => m.IsDeleted == 0).Where(m => m.DriveMSName == gname).Where(m => m.PassId == p).Where(m => m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMDriveMS obj = new Models.OCMDriveMS();
                    obj.DriveMSName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMDriveMS.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMDriveMSShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMDriveMS.Where(m => m.DriveMSId == mid).Select(m => m.DriveMSName).SingleOrDefault();
            int gsname = db.OCMDriveMS.Where(m => m.DriveMSId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMDriveMSUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMDriveMS gtyp = db.OCMDriveMS.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.DriveMSName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMDriveMSDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMDriveMS gtyp = db.OCMDriveMS.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //Json Autoselect pass full path option
        [HttpPost]
        public JsonResult AutocompleteOCMPassFullPath()
        {
            var result = db.OCMPass.Where(m => m.IsDeleted == 0).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // this is for stone grit
        public ActionResult OCMStoneGrit()
        {

            var obj = db.OCMStoneGrit.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.StoneGritId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMStoneGritAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMStoneGrit.Where(m => m.IsDeleted == 0).Where(m => m.StoneGritName == gname && m.PassId == p && m.PassName == pname && m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMStoneGrit obj = new Models.OCMStoneGrit();
                    obj.StoneGritName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMStoneGrit.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMStoneGritShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMStoneGrit.Where(m => m.StoneGritId == mid).Select(m => m.StoneGritName).SingleOrDefault();
            int gsname = db.OCMStoneGrit.Where(m => m.StoneGritId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMStoneGritUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMStoneGrit gtyp = db.OCMStoneGrit.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.StoneGritName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMStoneGritDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMStoneGrit gtyp = db.OCMStoneGrit.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region // this is for Sieve slot
        public ActionResult OCMSieveslot()
        {

            var obj = db.OCMSieveslot.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.SieveslotId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMSieveslotAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMSieveslot.Where(m => m.IsDeleted == 0).Where(m => m.SieveslotName == gname && m.PassId == p && m.PassName == pname && m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMSieveslot obj = new Models.OCMSieveslot();
                    obj.SieveslotName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMSieveslot.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMSieveslotShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMSieveslot.Where(m => m.SieveslotId == mid).Select(m => m.SieveslotName).SingleOrDefault();
            int gsname = db.OCMSieveslot.Where(m => m.SieveslotId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMSieveslotUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMSieveslot gtyp = db.OCMSieveslot.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.SieveslotName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMSieveslotDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMSieveslot gtyp = db.OCMSieveslot.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region // this is for Brake chamfer
        public ActionResult OCMBrakechamfer()
        {

            var obj = db.OCMBrakechamfer.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.BrakechamferId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMBrakechamferAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMBrakechamfer.Where(m => m.IsDeleted == 0).Where(m => m.BrakechamferName == gname && m.PassId == p && m.PassName == pname && m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMBrakechamfer obj = new Models.OCMBrakechamfer();
                    obj.BrakechamferName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMBrakechamfer.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMBrakechamferShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMBrakechamfer.Where(m => m.BrakechamferId == mid).Select(m => m.BrakechamferName).SingleOrDefault();
            int gsname = db.OCMBrakechamfer.Where(m => m.BrakechamferId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMBrakechamferUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMBrakechamfer gtyp = db.OCMBrakechamfer.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.BrakechamferName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMBrakechamferDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMBrakechamfer gtyp = db.OCMBrakechamfer.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region // this is for Accessories
        public ActionResult OCMAccessoriesNew()
        {

            var obj = db.OCMAccessoriesNew.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.AccessoriesId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMAccessoriesAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMAccessoriesNew.Where(m => m.IsDeleted == 0).Where(m => m.AccessoriesName == gname && m.PassId == p && m.PassName == pname && m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMAccessoriesNew obj = new Models.OCMAccessoriesNew();
                    obj.AccessoriesName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMAccessoriesNew.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMAccessoriesShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMAccessoriesNew.Where(m => m.AccessoriesId == mid).Select(m => m.AccessoriesName).SingleOrDefault();
            int gsname = db.OCMAccessoriesNew.Where(m => m.AccessoriesId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMAccessoriesUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMAccessoriesNew gtyp = db.OCMAccessoriesNew.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.AccessoriesName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMAccessoriesDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMAccessoriesNew gtyp = db.OCMAccessoriesNew.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region//this is for Reducer Ring
        public ActionResult OCMReducerRing()
        {

            var obj = db.OCMReducerRingNew.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.OCMReducerRingId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMReducerRingAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMReducerRingNew.Where(m => m.IsDeleted == 0).Where(m => m.OCMReducerRingName == gname && m.PassId == p && m.PassName == pname && m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMReducerRingNew obj = new Models.OCMReducerRingNew();
                    obj.OCMReducerRingName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMReducerRingNew.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMReducerRingShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMReducerRingNew.Where(m => m.OCMReducerRingId == mid).Select(m => m.OCMReducerRingName).SingleOrDefault();
            int gsname = db.OCMReducerRingNew.Where(m => m.OCMReducerRingId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMReducerRingUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMReducerRingNew gtyp = db.OCMReducerRingNew.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.OCMReducerRingName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMReducerRingDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMReducerRingNew gtyp = db.OCMReducerRingNew.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        #region//this is for CT Coil
        public ActionResult OCMCTCoil()
        {

            var obj = db.OCMCTCoilNew.Where(m => m.IsDeleted == 0).OrderByDescending(m=>m.OCMCTCoilId).ToList();
            return View(obj);
        }

        [HttpPost]
        public void OCMCTCoilAdd(string gname, string psid)
        {
            string uid = Session["userid"].ToString();
            int p = Convert.ToInt32(psid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            int count = db.OCMCTCoilNew.Where(m => m.IsDeleted == 0).Where(m => m.OCMCTCoilName == gname && m.PassId == p && m.PassName == pname && m.PathName == path).Count();
            if (count == 0)
            {
                if (gname != "" || gname != null)
                {

                    OCMCTCoilNew obj = new Models.OCMCTCoilNew();
                    obj.OCMCTCoilName = gname;
                    obj.PassId = p;
                    obj.PassName = pname;
                    obj.PathName = path;
                    obj.IsDeleted = 0;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = uid;
                    db.OCMCTCoilNew.Add(obj);
                    db.SaveChanges();
                }
            }
        }

        [HttpPost]
        public string OCMCTCoilShow(string gid)
        {
            int mid = Convert.ToInt32(gid);
            string gname = db.OCMCTCoilNew.Where(m => m.OCMCTCoilId == mid).Select(m => m.OCMCTCoilName).SingleOrDefault();
            int gsname = db.OCMCTCoilNew.Where(m => m.OCMCTCoilId == mid).Select(m => m.PassId).SingleOrDefault();
            string result = gname + '%' + gsname;

            return result.ToString();
        }

        [HttpPost]
        public void OCMCTCoilUpdate(string gid, string gname, string pid)
        {
            int mid = Convert.ToInt32(gid);
            int p = Convert.ToInt32(pid);
            string pname = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PassName).SingleOrDefault();
            string path = db.OCMPass.Where(m => m.PassId == p).Select(m => m.PathName).SingleOrDefault();
            path = path + '-' + gname;
            string uid = Session["userid"].ToString();
            if (mid != 0)
            {
                OCMCTCoilNew gtyp = db.OCMCTCoilNew.Find(mid);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.PassId = p;
                gtyp.PassName = pname;
                gtyp.PathName = path;
                gtyp.OCMCTCoilName = gname;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void OCMCTCoilDelete(string gid)
        {
            int g = Convert.ToInt32(gid);
            string uid = Session["userid"].ToString();
            if (g != 0)
            {
                OCMCTCoilNew gtyp = db.OCMCTCoilNew.Find(g);
                gtyp.ModifiedBy = uid;
                gtyp.ModifiedOn = DateTime.Now;
                gtyp.IsDeleted = 1;
                db.Entry(gtyp).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        [HttpPost]
        public string BringAll(string passid)
        {
            int psid = Convert.ToInt32(passid);
            string result = "";

            string drive = db.OCMDriveMS.Where(m => m.PassId == psid).Select(m => m.DriveMSName).FirstOrDefault();
            string stone = db.OCMStoneGrit.Where(m => m.PassId == psid).Select(m => m.StoneGritName).FirstOrDefault();
            string OCMSieve = db.OCMSieveslot.Where(m => m.PassId == psid).Select(m => m.SieveslotName).FirstOrDefault();
            string OCMBrake = db.OCMBrakechamfer.Where(m => m.PassId == psid).Select(m => m.BrakechamferName).FirstOrDefault();
            string accsrs = db.OCMAccessoriesNew.Where(m => m.PassId == psid).Select(m => m.AccessoriesName).FirstOrDefault();

            result =  " drive:" + drive + " stone:" + stone + " OCMSieve:" + OCMSieve + " OCMBrake:" + OCMBrake + " accsrs:" + accsrs;
            return result;
        }
       [HttpPost]
       public string BringKW(string passid, string cptphid)
       {
           int psid = Convert.ToInt32(passid);
           int cptpid = Convert.ToInt32(cptphid);
           string result = "";

           string Capacity = db.OCMCapacityKW.Where(m => m.CapacityTPHId == cptpid).Where(m => m.PassId == psid).Select(m => m.CapacityKWName).FirstOrDefault();
           string drive = db.OCMDriveMS.Where(m => m.PassId == psid).Select(m => m.DriveMSName).FirstOrDefault();
           string stone = db.OCMStoneGrit.Where(m => m.PassId == psid).Select(m => m.StoneGritName).FirstOrDefault();
           string OCMSieve = db.OCMSieveslot.Where(m => m.PassId == psid).Select(m => m.SieveslotName).FirstOrDefault();
           string OCMBrake = db.OCMBrakechamfer.Where(m => m.PassId == psid).Select(m => m.BrakechamferName).FirstOrDefault();
           string accsrs = db.OCMAccessoriesNew.Where(m => m.PassId == psid).Select(m => m.AccessoriesName).FirstOrDefault();

           result = "Capacity:" + Capacity + " drive:" + drive + " stone:" + stone + " OCMSieve:" + OCMSieve + " OCMBrake:" + OCMBrake + " accsrs:" + accsrs;

           return result;
       }

    }
}
