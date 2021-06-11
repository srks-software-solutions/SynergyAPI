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
using System.Net;
using System.IO;
using System.Text;
using System.Data.Entity;
namespace SRKSSynergy.MailServices
{
    public class mailservices1
    {
        SqlConnection mc = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString());
        private SRKS_Synergy db = new SRKS_Synergy();
                
        // Auto mail For Not to updation of the Leads and SOT Table
        public bool sendMailLEAD(int cpid, string module, DateTime lastdate)
        {
            string frommail =null;
            string mailpass = null;
            //to fetch FROM mail credentials
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                 frommail = from.FromMail;
                 mailpass = from.Password;
            }

            string date1 = lastdate.ToString("dd-MM-yyyy");           
            string cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
           
            MailMessage mail = new MailMessage();

            string SQLToAdd = "select [CPID],[EmailId],[ZonalManagerID] from CPMailIdTO where CPID='"+cpid+"'";
            SqlDataAdapter daToC = new SqlDataAdapter(SQLToAdd, mc.ConnectionString); ;
            System.Data.DataTable dtTO = new System.Data.DataTable();
            daToC.Fill(dtTO);

            int zoneid = Convert.ToInt32(dtTO.Rows[0][2]);

            string SQLCCAdd = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where ZonalManagerID='" + zoneid + "'";
            SqlDataAdapter daCC = new SqlDataAdapter(SQLCCAdd, mc.ConnectionString);
            System.Data.DataTable dtCC = new System.Data.DataTable();
            daCC.Fill(dtCC);

            string admin = "Admin";
            string spares = "Spares";
            string SQLCCAdd1 = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where Type='" + admin + "' or Type='"+ spares+"' and ZonalManagerID='" + 0 + "'";
            SqlDataAdapter daCC1 = new SqlDataAdapter(SQLCCAdd1, mc.ConnectionString);
            System.Data.DataTable dtCC1 = new System.Data.DataTable();
            daCC1.Fill(dtCC1);

            for (int i = 0; i < dtTO.Rows.Count; i++)
            {
                mail.To.Add(new MailAddress(dtTO.Rows[i][1].ToString()));
            }
            for (int i = 0; i < dtCC.Rows.Count; i++)
            {
                mail.CC.Add(new MailAddress(dtCC.Rows[i][0].ToString()));
            }

            for (int i = 0; i < dtCC1.Rows.Count; i++)
            {
                mail.CC.Add(new MailAddress(dtCC1.Rows[i][0].ToString()));
            }  

            mail.From = new MailAddress(frommail);
            mail.Subject = "Subject : No Updation of " + module + " Module";
            mail.IsBodyHtml = true;
            mail.Body = "<p><b>Dear " + cpname + ",</b></p>" +
                       "<b></b>" +
                       "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; This is to inform you that there is No updation in " + module + " Module since " + date1 + ".&nbsp;<span></b></p>" +
                       "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please make sure to update the "+module+" module on Regular Basis.&nbsp;<span></b></p>" +
                       "<p><b><br/><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Note: This Email has been sent automatically, Do not Reply. &nbsp;<span></b></p>" +
                       "<p><b></b></p>";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return true;
     }

        //Auto mail for not updation of the SOT module
        public bool sendMailSOT(int cpid, string module, DateTime lastdate)
        {
            string frommail = null;
            string mailpass = null;
            //to fetch FROM mail credentials
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                frommail = from.FromMail;
                mailpass = from.Password;
            }

            string date1 = lastdate.ToString("dd-MM-yyyy");
            string cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                       
            MailMessage mail = new MailMessage();

            string SQLToAdd = "select [CPID],[EmailId],[ZonalManagerID] from CPMailIdTO where CPID='" + cpid + "'";
            SqlDataAdapter daToC = new SqlDataAdapter(SQLToAdd, mc.ConnectionString); ;
            System.Data.DataTable dtTO = new System.Data.DataTable();
            daToC.Fill(dtTO);

            int zoneid = Convert.ToInt32(dtTO.Rows[0][2]);

            string SQLCCAdd = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where ZonalManagerID='" + zoneid + "'";
            SqlDataAdapter daCC = new SqlDataAdapter(SQLCCAdd, mc.ConnectionString);
            System.Data.DataTable dtCC = new System.Data.DataTable();
            daCC.Fill(dtCC);

            string admin = "Admin";
            string spares = "Spares";
            string SQLCCAdd1 = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where Type='" + admin + "' or Type='" + spares + "' and ZonalManagerID='" + 0 + "'";
            SqlDataAdapter daCC1 = new SqlDataAdapter(SQLCCAdd1, mc.ConnectionString);
            System.Data.DataTable dtCC1 = new System.Data.DataTable();
            daCC1.Fill(dtCC1);

            for (int i = 0; i < dtTO.Rows.Count; i++)
            {
                mail.To.Add(new MailAddress(dtTO.Rows[i][1].ToString()));
            }
            for (int i = 0; i < dtCC.Rows.Count; i++)
            {
                mail.CC.Add(new MailAddress(dtCC.Rows[i][0].ToString()));
            }

            for (int i = 0; i < dtCC1.Rows.Count; i++)
            {
                mail.CC.Add(new MailAddress(dtCC1.Rows[i][0].ToString()));
            }

            mail.From = new MailAddress(frommail);
            mail.Subject = "Subject : No Updation of " + module + " Module";
            mail.IsBodyHtml = true;
            mail.Body = "<p><b>Dear " + cpname + ",</b></p>" +
                       "<b></b>" +
                       "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; This is to inform you that there is No updation in " + module + " Module since " + date1 + ".&nbsp;<span></b></p>" +
                       "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please make sure to update the " + module + " module on Regular Basis.&nbsp;<span></b></p>" +
                       "<p><b><br/><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Note: This Email has been sent automatically, Do not Reply. &nbsp;<span></b></p>" +
                       "<p><b></b></p>";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return true;
        }

        // Auto Mail for Lead Generation
        public bool sendMailLEAD(int cpid, string module, DateTime lastdate, int flag, string orgname)
        {
            string frommail = null;
            string mailpass = null;
            //to fetch FROM mail credentials
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                frommail = from.FromMail;
                mailpass = from.Password;
            }

            string date1 = lastdate.ToString("dd-MM-yyyy");
            string cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();

            MailMessage mail = new MailMessage();

            string SQLToAdd = "select [CPID],[EmailId],[ZonalManagerID] from CPMailIdTO where CPID='" + cpid + "'";
            SqlDataAdapter daToC = new SqlDataAdapter(SQLToAdd, mc.ConnectionString); ;
            System.Data.DataTable dtTO = new System.Data.DataTable();
            daToC.Fill(dtTO);

            int zoneid = Convert.ToInt32(dtTO.Rows[0][2]);            

            string SQLCCAdd = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where ZonalManagerID='" + zoneid + "'";
            SqlDataAdapter daCC = new SqlDataAdapter(SQLCCAdd, mc.ConnectionString);
            System.Data.DataTable dtCC = new System.Data.DataTable();
            daCC.Fill(dtCC);

            string admin="Admin";
            string spares = "Spares";
            string SQLCCAdd1 = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where Type='" + admin + "' or Type='" + spares + "' and ZonalManagerID='" + 0 + "'";
            SqlDataAdapter daCC1 = new SqlDataAdapter(SQLCCAdd1, mc.ConnectionString);
            System.Data.DataTable dtCC1 = new System.Data.DataTable();
            daCC1.Fill(dtCC1);

            for (int i = 0; i < dtTO.Rows.Count; i++)
            {
                mail.CC.Add(new MailAddress(dtTO.Rows[i][1].ToString()));
            }
            for (int i = 0; i < dtCC.Rows.Count; i++)
            {
                mail.To.Add(new MailAddress(dtCC.Rows[i][0].ToString()));
            }
            for (int i = 0; i < dtCC1.Rows.Count; i++)
            {
                mail.To.Add(new MailAddress(dtCC1.Rows[i][0].ToString()));
            }          
          
            mail.From = new MailAddress(frommail);
            mail.Subject = "Subject : Generation of New Lead";
            mail.IsBodyHtml = true;
            mail.Body = "<p><b>Dear Zonal Manager,</b></p>" +
                         "<b></b>" +
                         "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; This is to inform you that "+cpname+" has generated a Lead on " + date1 + ".&nbsp;<span></b></p>" +
                         "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Lead Name: "+orgname +" and Suggsted Model is: "+ module +".&nbsp;<span></b></p>" +
                         "<p><b><br/><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Note: This Email has been sent automatically, Do not Reply. &nbsp;<span></b></p>" +
                         "<p><b></b></p>";
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return true;
        }

        public bool sendMailMachineDispatch(int cpid, string macslno, string orderno, string oadate, string invnum, string invdate, string lrnum, string dispdate, string transp, string modelc, string custnm)
        {
            MailMessage mail = new MailMessage();
            
            string frommail = null;
            string mailpass = null;
            //to fetch FROM mail credentials
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                frommail = from.FromMail;
                mailpass = from.Password;
            }

            // string date1 = lastdate.ToString("dd-MM-yyyy");
            string cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();

            string SQLToAdd = "select [CPID],[EmailId],[ZonalManagerID] from CPMailIdTO where CPID='" + cpid + "'";
            SqlDataAdapter daToC = new SqlDataAdapter(SQLToAdd, mc.ConnectionString); ;
            System.Data.DataTable dtTO = new System.Data.DataTable();
            daToC.Fill(dtTO);

            int zoneid = Convert.ToInt32(dtTO.Rows[0][2]);

            string SQLCCAdd = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where ZonalManagerID='" + zoneid + "'";
            SqlDataAdapter daCC = new SqlDataAdapter(SQLCCAdd, mc.ConnectionString);
            System.Data.DataTable dtCC = new System.Data.DataTable();
            daCC.Fill(dtCC);

            string admin = "Admin";
            string spares = "Spares";
            string SQLCCAdd1 = "SELECT [EmailId],[Type],[ZonalManagerID] FROM [BuhlerAdminCC] where Type='" + admin + "' or Type='" + spares + "' and ZonalManagerID='" + 0 + "'";
            SqlDataAdapter daCC1 = new SqlDataAdapter(SQLCCAdd1, mc.ConnectionString);
            System.Data.DataTable dtCC1 = new System.Data.DataTable();
            daCC1.Fill(dtCC1);

            for (int i = 0; i < dtTO.Rows.Count; i++)
            {
                mail.CC.Add(new MailAddress(dtTO.Rows[i][1].ToString()));
            }
            for (int i = 0; i < dtCC.Rows.Count; i++)
            {
                mail.To.Add(new MailAddress(dtCC.Rows[i][0].ToString()));
            }
            for (int i = 0; i < dtCC1.Rows.Count; i++)
            {
                mail.To.Add(new MailAddress(dtCC1.Rows[i][0].ToString()));
            }

            mail.From = new MailAddress(frommail);
            mail.Subject = "Subject :Machine Dispatch Details";
            mail.IsBodyHtml = true;
            mail.Body = "<head>" +
                    "<style type=\"text/css\">" +
                    "td" +
                        "{border-style: none;" +
                        "border-color: inherit;" +
                        "border-width: medium;" +
                        "padding-top:1px;" +
                        "padding-right:1px;" +
                        "padding-left:1px;" +
                        "color:black;" +
                        "font-size:10.0pt;" +
                        "font-weight:400;" +
                        "font-style:normal;" +
                        "text-decoration:none;" +
                        "font-family:Arial, sans-serif;" +
                        "text-align:general;" +
                        "vertical-align:bottom;" +
                        "white-space:nowrap;" +
                        "}" +
                    ".auto-style1 {" +
                        "border-collapse: collapse;" +
                        "border: 1px solid #000000;" +
                    "}" +
                    ".auto-style2 {" +
                        "text-align: center;" +
                        "font-size: x-large;" +
                        "vertical-align: middle;" +
                        "border-style: solid;" +
                        "border-width: 1px;" +
                    "}" +
                    ".auto-style5 {" +
                        "vertical-align: middle;" +
                        "text-align: center;" +
                        "border-style: solid;" +
                        "border-width: 1px;" +
                    "}" +
                    ".auto-style6 {" +
                        "vertical-align: middle;" +
                        "text-align: center;" +
                        "border-style: solid;" +
                        "border-width: 1px;" +
                        "background-color: #59C9FF;" +
                    "}" +
                    ".auto-style7 {" +
                        "vertical-align: middle;" +
                        "text-align: center;" +
                        "font-family: Arial;" +
                        "border-style: solid;" +
                        "border-width: 1px;" +
                        "background-color: #59C9FF;" +
                    "}" +
                    ".auto-style8 {" +
                        "text-align: center;" +
                        "font-size: small;" +
                    "}" +
                    ".auto-style9 {" +
                        "color: #FF0000;" +
                    "}" +
                    "</style>" +
                    "</head>" +
                    "<body>" +

                    "<p>&nbsp;</p>" +
                    "<p>Dear " + cpname + ",</p>" +
                    "<p>Please find the Machine Dispatch Details below</p>" +
                    "<p>&nbsp;</p>" +
                    "<table class=\"auto-style1\" style=\"width: 100%; height: 79px\">" +
                        "<tr>" +
                            "<td class=\"auto-style2\" colspan=\"10\">Machine Dispatch Details</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td class=\"auto-style7\">Machine<br />" +
                    "&nbsp;Serial No</td>" +
                            "<td class=\"auto-style6\">Model Number</td>" +
                            "<td class=\"auto-style6\">Customer Name</td>" +
                            "<td class=\"auto-style6\">Order <br />" +
                            "Acknowledgement No</td>" +
                            "<td class=\"auto-style6\">OA Date</td>" +
                            "<td class=\"auto-style6\">Invoice<br />" +
                            "Number</td>" +
                            "<td class=\"auto-style6\">Invoice<br />" +
                            "Date</td>" +
                            "<td class=\"auto-style6\">LR<br />" +
                            "Number</td>" +
                            "<td class=\"auto-style6\">Dispatch<br />" +
                            "Date</td>" +
                            "<td class=\"auto-style6\">Transporter</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + macslno + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + modelc + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + custnm + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + orderno + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + oadate + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + invnum + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + invdate + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + lrnum + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + dispdate + "</td>" +
                            "<td class=\"auto-style5\" style=\"height: 30px\">" + transp + "</td>" +

                        "</tr>" +
                    "</table>" +
                    "<p>&nbsp;</p>" +
                    "<p class=\"auto-style8\">Note :<span class=\"auto-style9\"> Please donot reply to this " +
                    "Email ID, this is a Autogenerated Email</span></p>" +

                    "</body>";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return true;
        }        

        public void SendTargetMails(string targetmonth, string startDate)
        {

            string gettargets = "SELECT * FROM TargetSettings where TargetMonth = '" + targetmonth + "'";
            SqlDataAdapter SDA = new SqlDataAdapter(gettargets, mc.ConnectionString);
            DataTable targets = new DataTable();
            SDA.Fill(targets);

            string frommail = null;
            string mailpass = null;
            //to fetch FROM mail credentials
            var from = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
            if (from != null)
            {
                frommail = from.FromMail;
                mailpass = from.Password;
            }

            List<int> targetIDs = new List<int>();

            #region for each targetsSettings
            //
            for (int i = 0; i < targets.Rows.Count; i++)
            {
                //so that each time new mail object is created.
                //problem with to and from for second mail so better create mail object here.
                MailMessage mail = new MailMessage();

                int cpid = Convert.ToInt32(targets.Rows[i][1]);
                int target = Convert.ToInt32(targets.Rows[i][4]);
                targetIDs.Add(cpid);

                //To get tomailID from CPMailidTo
                string tomail = db.CPMailIdTO.Where(m => m.CPID == cpid).Select(m => m.EmailId).SingleOrDefault();
                //insert ChannelPartner mailID as to mail id
                mail.To.Add(new MailAddress(tomail));

                // to get cpname and zoneid(to get zone manager email) from ChannelPartners
                string cpname = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.CPName).SingleOrDefault();
                int zoneid = db.ChannelPartners.Where(m => m.CPID == cpid).Select(m => m.ZoneID).SingleOrDefault();


                //for CC get zone manager email based on zoneid
                string email = db.BuhlerAdminCC.Where(m => m.ZonalManagerID == zoneid).Select(m => m.EmailId).FirstOrDefault();
                //insert zonalmanager mailID
                mail.CC.Add(new MailAddress(email));

                //for CC get admin emails
                var Aemail = "SELECT * FROM BuhlerAdminCC where Type = 'Admin'";
                SqlDataAdapter SDA2 = new SqlDataAdapter(Aemail, mc.ConnectionString);
                DataTable Adminemail = new DataTable();
                SDA2.Fill(Adminemail);
                for (int j = 0; j < Adminemail.Rows.Count; j++)
                {
                    string name = Adminemail.Rows[0][2].ToString();
                    mail.CC.Add(new MailAddress(name));
                }

                //to get count of leads from begining of this month
               // DateTime jdt = DateTime.Now;
                //var startDate = new DateTime(jdt.Year, jdt.Month, 1);
                //var endDate = startDate.AddMonths(1).AddDays(-1);

                //we may have to get leads of previous month in case of monthendmails so take startDate as argument.
                var leadcount = "SELECT * FROM LeadEnquiry where CPID = '" + cpid + "' and CreatedOn = '" + startDate + "' ";
                int leadscount = leadcount.Count();


                mail.From = new MailAddress(frommail);
                mail.Subject = "Subject : Regarding Leads achieved till date.";
                mail.IsBodyHtml = true;
                mail.Body = "<p><b>Dear " + cpname + ",</b></p>" +
                           "<b></b>" +
                           "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; This is to inform you that your target was " + target + ".&nbsp;<span></b></p>" +
                           "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Your have achieved " + leadscount + " leads.&nbsp;<span></b></p>" +
                           "<p><b><br/><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Note: This Email has been sent automatically, Do not Reply. &nbsp;<span></b></p>" +
                           "<p><b></b></p>";
                mail.Body += "<p></p><p></p><p></p>";
                mail.Body += "<p>With Regards,</p>";
                mail.Body += "<p>SYNERGY</p>"; 
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(frommail, mailpass);
                smtp.EnableSsl = true;
                smtp.Send(mail);

            }
        #endregion
            //end of targets

            //using list targetIDs, send mails to admin, for those who are not in partner table

            MailMessage mailtoAdmin = new MailMessage();
            try
            {
                //from mail credentials
                //to fetch FROM mail credentials
                 string frommailtoAdmin=null;
                 string mailpasstoAdmin = null;
                var fromDetails = db.MailSendCredentials.Where(m => m.IsDeleted == 0).Select(m => new { m.FromMail, m.Password }).FirstOrDefault();
                if (fromDetails != null)
                {
                     frommailtoAdmin = fromDetails.FromMail;
                     mailpasstoAdmin = fromDetails.Password;
                }

                //to fetch To mailID
                string Tomail = db.BuhlerAdminCC.Where(m => m.Type == "admin").Select(m => m.EmailId).FirstOrDefault();
                string Adminname = db.BuhlerAdminCC.Where(m => m.Type == "admin").Select(m => m.Name).FirstOrDefault();
                //insert admin mailID into TO
                mailtoAdmin.To.Add(new MailAddress(Tomail));

                //get partner table detailsinto datatable
                string getCID = "SELECT * FROM ChannelPartners";
                SqlDataAdapter jSDA = new SqlDataAdapter(getCID, mc.ConnectionString);
                DataTable jcpid = new DataTable();
                jSDA.Fill(jcpid);

                List<string> namesofCPs = new List<string>();
                for (int i = 0; i < jcpid.Rows.Count; i++)
                {
                    int cpid = Convert.ToInt32(jcpid.Rows[i][0]);
                    //check if cpid is in targetset table 
                    if (targetIDs.Contains(cpid))
                    {
                        //do nothing
                    }
                    else
                    {
                        namesofCPs.Add(jcpid.Rows[i][2].ToString());
                    }
                }

                //send mail only if targets are to be set
                if (namesofCPs.Count > 0)
                {

                    mailtoAdmin.IsBodyHtml = true;
                    mailtoAdmin.From = new MailAddress(frommail);
                    mailtoAdmin.Subject = "Subject : Targets have not been set.";
                    mailtoAdmin.IsBodyHtml = true;
                    mailtoAdmin.Body = "<p><b>Dear  " + Adminname + ",</b></p>";
                    mailtoAdmin.Body += "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; This is to inform you that Targets have not been set to following Channel Partners . &nbsp;<span></b></p>";

                    for (int listlooper = 0; listlooper < namesofCPs.Count(); listlooper++)
                    {
                        mailtoAdmin.Body += "<p><b>" + namesofCPs[listlooper] + " &nbsp;<span></b></p>";
                    }

                    mailtoAdmin.Body += "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  Kindly make some time and set target's.&nbsp;<span></b></p>";
                    mailtoAdmin.Body += "<p></p><p></p><p></p>";
                    mailtoAdmin.Body += "<p>With Regards,</p>";
                    mailtoAdmin.Body += "<p>SYNERGY</p>";

                    SmtpClient smtpA = new SmtpClient();
                    smtpA.Host = "smtp.gmail.com";
                    smtpA.Port = 587;
                    smtpA.UseDefaultCredentials = false;
                    smtpA.Credentials = new System.Net.NetworkCredential(frommailtoAdmin, mailpasstoAdmin);
                    smtpA.EnableSsl = true;
                    smtpA.Send(mailtoAdmin);
                }
            }
            catch (Exception e)
            {

            }

        } 
        //end of SendTargetMails()

        public bool StoreLatLang(string fulladdress, int mdbid)
        {
            string l1 = null;
            string l2 = null;

            // Finds the Latitude and Longitude of the address
            string url = "http://maps.google.com/maps/api/geocode/xml?address=" + fulladdress + "&sensor=false";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    DataSet dsResult = new DataSet();
                    dsResult.ReadXml(reader);
                    DataTable dtCoordinates = new DataTable();
                    dtCoordinates.Columns.AddRange(new DataColumn[2] 
                        { 
                            //new DataColumn("Id", typeof(int)),
                            //new DataColumn("Address", typeof(string)),
                            new DataColumn("Latitude",typeof(string)),
                            new DataColumn("Longitude",typeof(string))
                        });
                    foreach (DataRow row in dsResult.Tables["result"].Rows)
                    {
                        string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                        DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                        //dtCoordinates.Rows.Add(row["result_id"], row["formatted_address"], location["lat"], location["lng"]);
                        dtCoordinates.Rows.Add(location["lat"], location["lng"]);

                        l1 = Convert.ToString(location["lat"]);
                        l2 = Convert.ToString(location["lng"]);
                    }
                }
            }
            LeadEnquiry MDB = db.LeadEnquiry.Find(mdbid);

            MDB.Latitude = l1;
            MDB.Longitude = l2;

            db.Entry(MDB).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

    }//end of class
}

