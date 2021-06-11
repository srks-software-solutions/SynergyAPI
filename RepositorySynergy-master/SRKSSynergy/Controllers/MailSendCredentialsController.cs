using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SRKSSynergy.Controllers;
using SRKSSynergy.Models;

namespace SRKSSynergy.Controllers
{
    public class MailSendCredentialsController : Controller
    {
        SRKS_Synergy db = new SRKS_Synergy();
        
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(MailSendCredentials mail)
        {
            
            var cpid = Convert.ToInt32(Session["logincpid"]);
            mail.CreatedOn=System.DateTime.Now;
            
            if(cpid!=0)
            {
                mail.CreatedBy =Convert.ToString(cpid);
            }
            else
            {
                mail.CreatedBy = Convert.ToString(0);
            }     
            mail.IsDeleted=0;
            mail.IsStatus=0;
            mail.IsDrop = 0;
            db.MailSendCredentials.Add(mail);
            db.SaveChanges();

         return  RedirectToAction("Index","MailSendCredentials",null);
        }

        public ActionResult Index()
        {
            var list = db.MailSendCredentials.Where(m => m.IsDeleted == 0).ToList();
            return View(list);
        }

        public ActionResult Edit(int id)
        {
            MailSendCredentials MailSendCredentials = db.MailSendCredentials.Find(id);
            if (MailSendCredentials == null)
            {
                return HttpNotFound();
            }           
            return View(MailSendCredentials);
        }
        [HttpPost]
        public ActionResult Edit(MailSendCredentials mail)
        {
            var cpid = Convert.ToInt32(Session["logincpid"]);

            mail.ModifiedOn = System.DateTime.Now;
            if (cpid != 0)
            {
                mail.ModifiedBy = Convert.ToString(cpid);
            }
            else
            {
                mail.ModifiedBy = Convert.ToString(0);
            }
            db.Entry(mail).State = EntityState.Modified;
            db.SaveChanges();

          return RedirectToAction("Index", "MailSendCredentials", null);
        }
        
        public ActionResult Delete(int id)
        {
            MailSendCredentials MailSendCredentials = db.MailSendCredentials.Find(id);
            if (MailSendCredentials == null)
            {
                return HttpNotFound();
            }
            return View(MailSendCredentials);
        }

        [HttpPost]
        public ActionResult Delete(MailSendCredentials mail,int id)
        {
           if (Request.Form["Yes"] != null)
            {
                MailSendCredentials MailSendCredentials = db.MailSendCredentials.Find(id);
                MailSendCredentials.IsDeleted = 1;
                db.Entry(MailSendCredentials).State = EntityState.Modified;
                db.SaveChanges();               
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");        
        }
    }
}
