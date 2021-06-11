using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using SRKSSynergy.Filters;
using SRKSSynergy.Models;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Web.UI;
using Common.Logging;
using System.Net;
using System.Net.Mail;
using SRKSSynergy.Chart;
using SRKSSynergy.MailServices;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Transactions;


namespace SRKSSynergy.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private SRKS_Synergy db = new SRKS_Synergy();
        //
        // GET: /Account/Login

        //Json AutoComplete
        public JsonResult AutocompleteCP(string term)
        {
            var userid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID }).SingleOrDefault();

            var result = (from r in db.ChannelPartners
                          where (r.CPName.ToLower().Contains(term.ToLower()) && r.CPID != loginname.CPID)
                          select new { r.CPName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsUserMailIDAlreadyExist(string UserMailID)
        {
            int result = 0;//false;
            if (!string.IsNullOrEmpty(UserMailID))
            {
                //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.
                var obj = db.UserLogins.Where(m => m.email == UserMailID && m.IsDeactivate == 0).ToList();
                if (obj.Count > 0)
                {
                    result = obj.Count;//true;//returns true or >0 is user mail id already exist
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BLog()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, String fp = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (fp == null)
            {
                ViewBag.Success = false;
            }
            else
            {
                ViewBag.Success = true;
            }
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    var userid = (Guid)Membership.GetUser(model.UserName).ProviderUserKey;
                    var IsActive = db.UserLogins.Where(m => m.UserID == userid).Where(m => m.IsDeactivate == 0).SingleOrDefault();
                    if (IsActive != null)
                    {
                        //Get CPID from based on user id
                        var loginname = db.UserLogins.Where(m => m.UserID == userid).Select(m => new { m.CPID, m.ZoneID,m.UserID }).SingleOrDefault();
                        //TempData["userID"] = loginname.UserID;
                        Session["userID"] = loginname.UserID;
                        Session["logincpid"] = loginname.CPID;
                        Session["Zoneid"] = loginname.ZoneID;
                        Session["userid"] = userid;
                        Session["username"] = model.UserName;

                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        var role = Roles.GetRolesForUser(model.UserName);

                        //LoginData lg = new LoginData();
                        //lg.UserId = userid;
                        //lg.UserName = model.UserName;
                        //lg.ValidityPeriod = System.DateTime.Now.TimeOfDay;

                        //string test = Convert.ToString(lg.ValidityPeriod);
                        //TimeSpan ts = TimeSpan.Parse(test);
                        //lg.channelpartnerid = Convert.ToString(loginname.CPID);
                        //lg.IsStatus = 1;
                        //lg.LoginTime = (ts.ToString(@"hh\:mm"));
                        //lg.LoginDate = System.DateTime.Now;
                        //db.LoginData.Add(lg);
                        //db.SaveChanges();
                        System.Guid uid = userid;
                        string uname = model.UserName;
                        string cpid = loginname.CPID.ToString();
                        int cid=Convert.ToInt32(cpid);
                        string zid = loginname.ZoneID.ToString();
                        if (zid == "0")
                        {
                           int zoid = db.ChannelPartners.Where(m => m.IsDeleted == 0).Where(m => m.CPID == cid).Select(m => m.ZoneID).SingleOrDefault();
                           zid = zoid.ToString();
                        }
                        updatelogindata(uid,uname,cpid,zid);
                        //if (Url.IsLocalUrl(returnUrl))
                        if (role.Contains("Administrator"))
                        {
                            //return Redirect(returnUrl);
                            //return RedirectToAction("MainDashboard", "MainDashboard");
                            return RedirectToAction("LiveDashBoard", "Account");

                        }
                        if (role.Contains("ZonalManager"))
                        {
                            return RedirectToAction("CPMDB", "MDB");
                        }
                        else
                        {
                            return RedirectToAction("Synergy", "Synergy");
                        }
                    }
                    return RedirectToAction("Login", "Account");
                }
            }

            ViewBag.Success = false;
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        public void updatelogindata(System.Guid uid, string uname, string cpid, string zid)
        {
            DateTime datetime = DateTime.Now;

            LoginData ld = new LoginData();
            ld.UserId = uid;
            ld.UserName = uname;
            ld.channelpartnerid = cpid;
            ld.ZoneId = zid;
            ld.IsStatus = 0;
            ld.LoginDate = datetime;
            db.LoginData.Add(ld);
            db.SaveChanges();


        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //LoginData lg = new LoginData();

            var uid = Session["userid"];
            var uname = Session["username"];

            //var username = db.LoginData.Where(m => m.UserId == (Guid)uid).Where(m => m.UserName == uname).Where(m => m.IsStatus == 1).SingleOrDefault();
            //var logindataid = username.UserLoginID;
            //LoginData lg = db.LoginData.Find(logindataid);

            //lg.ValidityPeriod1 = System.DateTime.Now.TimeOfDay;

            //string test = Convert.ToString(lg.ValidityPeriod);
            //string test1 = Convert.ToString(lg.ValidityPeriod1);
            //TimeSpan ts = TimeSpan.Parse(test);
            //TimeSpan tp = TimeSpan.Parse(test1);

            //lg.LogOutTime = (tp.ToString(@"hh\:mm"));
            //lg.LogOutDate = System.DateTime.Now;

            //TimeSpan dt = TimeSpan.FromHours(ts.Hours);
            //TimeSpan dt1 = TimeSpan.FromHours(tp.Hours);

            //TimeSpan dt2 = TimeSpan.FromMinutes(ts.Minutes);
            //TimeSpan dt3 = TimeSpan.FromMinutes(tp.Minutes);

            //TimeSpan span3 = dt1.Subtract(dt);
            //TimeSpan span4 = dt3.Subtract(dt2);

            //TimeSpan span5 = span3.Add(span4);
            //lg.Duration = (span5.ToString(@"hh\:mm"));
            //lg.IsStatus = 2;
            //db.SaveChanges();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    MembershipUser NewUser = Membership.CreateUser(model.UserName, model.Password);
                    NewUser.Email = model.Email;
                    NewUser.ChangePasswordQuestionAndAnswer(model.Password, model.Question, model.Answer);
                    NewUser.IsApproved = true;
                    //WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Login", "Account");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in user is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the user from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = Membership.GetUser(User.Identity.Name).ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        //Get:/Account/UserLogin
        public ActionResult UserLogin()
        {
            ViewData["ZoneName"] = new SelectList(db.Zone, "ZoneID", "ZoneName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(UserRegistration model)
        {
            var duplicate = (from s in db.UserLogins
                             where s.Username == model.RegisterModel.UserName && s.email == model.RegisterModel.Email
                             select s).ToList();
            if (model.RegisterModel.UserName != null && model.RegisterModel.UserName != "" && model.UserLogin.FirstName != null && model.UserLogin.FirstName != "")
            {
                if (duplicate.Count > 0)
                {
                    ViewBag.Duplicate = "User already exists for this UseName and Email";
                }
                else
                {
                    // Attempt to register the user
                    try
                    {
                        MembershipCreateStatus status;
                        MembershipUser newUser = Membership.CreateUser(model.RegisterModel.UserName, model.RegisterModel.Password,
                                                       model.RegisterModel.Email, model.RegisterModel.Question,
                                                       model.RegisterModel.Answer, true, out status);
                        var userid = (Guid)Membership.GetUser(model.RegisterModel.UserName).ProviderUserKey;
                        model.UserLogin.UserID = userid;
                        var adminid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                        var role = Roles.GetRolesForUser(User.Identity.Name);
                        if (role.Contains("Administrator"))
                        {
                            if (model.UserLogin.IsZoneManager == 1)
                            {
                                Roles.AddUserToRole(model.RegisterModel.UserName, "ZonalManager");
                            }
                            else
                            {
                                Roles.AddUserToRole(model.RegisterModel.UserName, "Administrator");
                            }
                        }
                        else if (role.Contains("ChannelPartnerAdmin"))
                        {
                            Roles.AddUserToRole(model.RegisterModel.UserName, "ChannelPartnerUser");
                        }
                        var cpid = db.UserLogins.Where(m => m.UserID == adminid).Select(m => m.CPID).First();
                        model.UserLogin.CPID = cpid;
                        model.UserLogin.Answer = model.RegisterModel.Answer;
                        model.UserLogin.Username = model.RegisterModel.UserName;
                        model.UserLogin.email = model.RegisterModel.Email;
                        db.UserLogins.Add(model.UserLogin);
                        db.SaveChanges();
                        TempData["notice"] = "User Successfully registered";
                        ViewData["ZoneName"] = new SelectList(db.Zone, "ZoneID", "ZoneName");
                        return RedirectToAction("UserLogin", "Account");
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["ZoneName"] = new SelectList(db.Zone.Where(m => m.IsDeactive == 0), "ZoneID", "ZoneName");
            return View(model);
        }

        public ActionResult CPUserRegistration(UserRegistration model, int cpid = 0)
        {
            UserRegistration UR = new UserRegistration();

            if (cpid != 0)
            {
                var cpi = db.ChannelPartners.Where(m => m.CPID == cpid).First();

                ViewBag.FirstName = cpi.CPContactPersonData.Select(m => m.FirstName).First();
                ViewBag.MiddleName = cpi.CPContactPersonData.Select(m => m.MiddleName).First();
                ViewBag.LastName = cpi.CPContactPersonData.Select(m => m.LastName).First();
                ViewBag.Email = cpi.CPContactPersonData.Select(m => m.EmailID).First();
                ViewBag.Designation = cpi.CPContactPersonData.Select(m => m.Designation).First();
                ViewBag.CPID = cpi.CPID;
                return View(UR);
            }
            if (cpid == 0 && model != null)
            {
                var cpid1 = model.UserLogin.CPID;
                var cpi = db.ChannelPartners.Where(m => m.CPID == cpid1).First();
                ViewBag.FirstName = cpi.CPContactPersonData.Select(m => m.FirstName).First();
                ViewBag.MiddleName = cpi.CPContactPersonData.Select(m => m.MiddleName).First();
                ViewBag.LastName = cpi.CPContactPersonData.Select(m => m.LastName).First();
                ViewBag.Email = cpi.CPContactPersonData.Select(m => m.EmailID).First();
                ViewBag.Designation = cpi.CPContactPersonData.Select(m => m.Designation).First();
                ViewBag.CPID = cpi.CPID;
                return View(UR);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CPUserRegistration(UserRegistration model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    MembershipCreateStatus status;
                    MembershipUser newUser = Membership.CreateUser(model.RegisterModel.UserName, model.RegisterModel.Password,
                                                   model.RegisterModel.Email, model.RegisterModel.Question,
                                                   model.RegisterModel.Answer, true, out status);
                    var userid = (Guid)Membership.GetUser(model.RegisterModel.UserName).ProviderUserKey;
                    model.UserLogin.UserID = userid;
                    model.UserLogin.Answer = model.RegisterModel.Answer;
                    model.UserLogin.email = model.RegisterModel.Email;
                    model.UserLogin.Username = model.RegisterModel.UserName;
                    db.UserLogins.Add(model.UserLogin);
                    db.SaveChanges();
                    var adminid = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var role = Roles.GetRolesForUser(User.Identity.Name);
                    Roles.AddUserToRole(model.RegisterModel.UserName, "ChannelPartnerAdmin");
                    return RedirectToAction("Index", "ChannelPartner");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            ViewBag.FirstName = model.UserLogin.FirstName;
            ViewBag.MiddleName = model.UserLogin.MiddleName;
            ViewBag.LastName = model.UserLogin.LastName;
            ViewBag.Email = model.RegisterModel.Email;
            ViewBag.Designation = model.UserLogin.Designation;
            ViewBag.CPID = model.UserLogin.CPID;
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel FPM)
        {
            var isvalid = Membership.GetUser(FPM.UserName).IsApproved;
            if (isvalid)
            {
                var PassQue = Membership.GetUser(FPM.UserName).PasswordQuestion;
                if (PassQue.Trim() == FPM.Question.Trim())
                {
                    var userid = (Guid)Membership.GetUser(FPM.UserName).ProviderUserKey;
                    var SecAns = db.UserLogins.Where(m => m.UserID == userid).First();
                    var answer = SecAns.Answer;
                    if (FPM.Answer == answer)
                    {
                        return RedirectToAction("ChangePassword", "Account", new { UserName = FPM.UserName, ID = answer });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The Security Answer is invalid.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The Security Question is invalid.");
                }
            }
            else
            {
                ModelState.AddModelError("", "The UserName is invalid.");
            }
            return View(FPM);
        }

        [AllowAnonymous]
        public ActionResult ChangePassword(String UserName = null, String ID = null)
        {
            if (UserName != null)
            {
                ChangePasswordModel CPM = new ChangePasswordModel();
                CPM.UserName = UserName;
                CPM.ID = ID;
                return View(CPM);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel CPM)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = Membership.GetUser(CPM.UserName);
                var pass = user.ResetPassword(CPM.ID);
                //CPM.OldPassword = Membership.GetUser(CPM.UserName).GetPassword();
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = Membership.GetUser(CPM.UserName).ChangePassword(pass, CPM.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded)
                {
                    ViewBag.Success = true;
                    return RedirectToAction("Login", "Account", new { fp = "1" });
                }
                else
                {
                    ModelState.AddModelError("", "The new password is invalid.");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(CPM);
        }

        //
        //User View
        const int pageSize = 10;
        public ActionResult UserView(int page = 1, int sortBy = 1, bool isAsc = true, string custnm = null)
        {
            IEnumerable<UserLogin> userall = null;
            if (custnm == null)
            {
                userall = db.UserLogins.Where(m => m.IsDeactivate == 0).ToList();
            }
            else
            {
                var cpid = db.ChannelPartners.Where(m => m.CPName == custnm).Select(m => m.CPID).SingleOrDefault();
                if (cpid != null)
                {
                    userall = db.UserLogins.Where(m => m.CPID == cpid).Where(m => m.IsDeactivate == 0);
                    var chpart = db.ChannelPartners.Where(p => p.CPID == cpid).SingleOrDefault();
                    if (chpart == null)
                    {
                        TempData["wrong"] = "Please enter a valid Channel Partner Name and search again";
                        return RedirectToAction("CPMDB", "MDB");
                    }
                }
            }
            #region Sorting
            switch (sortBy)
            {
                case 1:
                    userall = isAsc ? userall.OrderBy(p => p.UserID) : userall.OrderByDescending(p => p.UserID);
                    break;

                case 2:
                    userall = isAsc ? userall.OrderBy(p => p.ContactName) : userall.OrderByDescending(p => p.ContactName);
                    break;

                case 3:
                    userall = isAsc ? userall.OrderBy(p => p.Username) : userall.OrderByDescending(p => p.Username);
                    break;

                case 4:
                    userall = isAsc ? userall.OrderBy(p => p.Designation) : userall.OrderByDescending(p => p.Designation);
                    break;

                case 5:
                    userall = isAsc ? userall.OrderBy(p => p.email) : userall.OrderByDescending(p => p.email);
                    break;

                default:
                    userall = isAsc ? userall.OrderBy(p => p.UserID) : userall.OrderByDescending(p => p.UserID);
                    break;
            }
            #endregion

            ViewBag.TotalPages = (int)Math.Ceiling((double)userall.Count() / pageSize);

            userall = userall
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.Search = custnm;

            ViewBag.SortBy = sortBy;
            ViewBag.IsAsc = isAsc;
            if (custnm != null)
                ViewBag.IsSearch = true;
            return View(userall);
        }

        //
        //Deactivate User
        public ActionResult DeactivateUser(Guid id)
        {
            var deac = db.UserLogins.Where(m => m.UserID == id).Where(m => m.IsDeactivate == 0).Single();
            return View(deac);
        }

        //
        //Post: DeactivateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeactivateUser(UserLogin user)
        {
            if (Request.Form["Yes"] != null)
            {
                user.IsDeactivate = 1;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("UserView");
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult LiveDashBoard()
        {
            //target mailer code
            //#region
            ////check if today is 15th or month end and send mails

            //DateTime jnow = DateTime.Now;
            //string MonthYear = jnow.ToString("MMM") + " " + jnow.Year.ToString();

            ////First, insert a row for this particular month in targetmailing table.
            //var mailRowStatus = db.MailingTargets.Where(m=>m.MonthYear == MonthYear).SingleOrDefault();
            //if (mailRowStatus == null)
            //{
            //    MailingTargets mt = new MailingTargets();
            //    mt.HalfMonthMail = 0;
            //    mt.MonthEndMail = 0;
            //    mt.MonthYear = MonthYear;
            //    db.MailingTargets.Add(mt);
            //    db.SaveChanges();
            //}

            ////Calculations for HalfMonthMails
            //int HalfMonthEnd = 28;
            //int today = jnow.Day;

            //var leadsDate = new DateTime(jnow.Year, jnow.Month, 1);
            //string leadsDateAsString = leadsDate.ToString();

            //if (today>=15 && today <= HalfMonthEnd )
            //{
            //    //see if mail has been sent for halfmonth, if not send now.
            //    var halfMonthStatus = db.MailingTargets.Where(m => m.MonthYear == MonthYear).Select(m => m.HalfMonthMail).SingleOrDefault();
            //    int halfMonthStatusinteger = Convert.ToInt32(halfMonthStatus);
            //    if (halfMonthStatusinteger == 0)
            //    {
            //        //send target mails
            //        mailservices1 ms1 = new mailservices1();
            //        ms1.SendTargetMails(MonthYear, leadsDateAsString);

            //        //update the mailingtargets -> HalfMonthMail column.
            //        int mtiddb = db.MailingTargets.Where(m => m.MonthYear == MonthYear).Select(m => m.MTID).SingleOrDefault();
            //        MailingTargets mt = db.MailingTargets.Find(mtiddb);
            //        mt.HalfMonthMail = 1;
            //        db.Entry(mt).State = EntityState.Modified;
            //        db.SaveChanges();


            //    }
            //}

            ////Calculations for MonthEndMails
            //DateTime Currentdt = DateTime.Now;
            //var currentMonth = Currentdt.Month;
            //var currentYear = Currentdt.Year;
            //int currentMonthInteger = Convert.ToInt32(currentMonth);
            //string suitableYear = null;
            //if (Currentdt.Day >= 1 && Currentdt.Day <= 5)
            //{

            //    // prevMonth will be lesser than currentMonth except jan, so then get (year-1) else get currentYear

            //    DateTime prevdt = DateTime.Now;
            //    prevdt = prevdt.AddMonths(-1);
            //    int prevMonthInteger = prevdt.Month;
            //    int prevYear = prevdt.Year;
            //    string prevMonthAsString = prevdt.ToString("MMM");

            //    if (prevMonthInteger > currentMonthInteger)
            //    {
            //        suitableYear = prevYear.ToString();
            //    }
            //    else
            //    {
            //        suitableYear = currentYear.ToString();
            //    }

            //    string newMonthYear = prevMonthAsString + " " + suitableYear;
            //    var MonthEndStatus = db.MailingTargets.Where(m => m.MonthYear == newMonthYear).Select(m => m.MonthEndMail).SingleOrDefault();
            //    int MonthEndStatusinteger = Convert.ToInt32(MonthEndStatus);


            //    var leadsDate2 = new DateTime(prevdt.Year, prevdt.Month, 1);
            //    string leadsDateAsString2 = leadsDate2.ToString();

            //    if (MonthEndStatusinteger == 0)
            //    {
            //        //send target mails
            //        mailservices1 ms1 = new mailservices1();
            //        ms1.SendTargetMails(newMonthYear, leadsDateAsString2);

            //        //update the mailingtargets -> HalfMonthMail column.
            //        int mtiddb = db.MailingTargets.Where(m => m.MonthYear == newMonthYear).Select(m => m.MTID).SingleOrDefault();
            //        MailingTargets mt = db.MailingTargets.Find(mtiddb);
            //        mt.MonthEndMail = 1;
            //        db.Entry(mt).State = EntityState.Modified;
            //        db.SaveChanges();
            //    }
            //}
            //#endregion
            //end of target mailer code.

            //var listdesi = db.OCMBrake.Where(m => m.IsDeleted == 0).ToList();
            //foreach (var listd in listdesi)
            //{
            //    //var dept = db.Channels_Tbl.Where(m => m.ChannelID == listd.ChannelID).SingleOrDefault();
            //    //var entity = db.BusinessUnit_Tbl.Where(m => m.BUID == listd.BUID).SingleOrDefault();

            //    listd.BrakeFullName = listd.BrakeDescription + "-" + listd.BrakeType;
            //    db.Entry(listd).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = System.DateTime.Now;
            var yesterday = DateTime.Today.AddDays(-1);

            //takes the Count of Total Leads
            //var count = db.LeadEnquiry.Where(m => m.CreatedOn > startDate && m.CreatedOn < enddate).Where(m => m.IsDeleted == 0).Count();

            var count = (from r in db.LeadEnquiry
                         where r.IsDeleted == 0 && r.CreatedOn >= startDate && r.CreatedOn <= enddate
                         select r).Count();

            ViewBag.totalleads = count;

            //takes the count of Quotation Generated
            var list = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.QuotationDate >= startDate && m.QuotationDate <= enddate).Count();
            int exp = list;
            ViewBag.exp = exp;

            //takes the count of Oder Generated
            var oglist = db.OAEquipGeneralData.Where(m => m.ApprovalStatus == 1 || m.ApprovalStatus == 2).Where(m => m.OAStatus == 1).Where(m => m.OADate >= startDate && m.OADate <= enddate).Count();
            int og = oglist;
            ViewBag.odergen = og;

            LeadEnquiry lead = new LeadEnquiry();

            //Notification Part start
            #region
            //Return value of Total New Leads
            //int leadcount = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.IsStatus == 0).Where(m => m.IsTime == 0).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate).Count();
            int leadcount = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.IsStatus == 0).Where(m => m.IsTime == 0).Where(m => m.CreatedOn >= yesterday && m.CreatedOn <= enddate).Count();
            if (leadcount != 0)
            {
                var test2 = db.LeadEnquiry.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LEID).First();

                string test = Convert.ToString(test2.LeadTime);
                string test1 = Convert.ToString(System.DateTime.Now.TimeOfDay);
                TimeSpan ts = TimeSpan.Parse(Convert.ToDateTime(test).TimeOfDay.ToString());
                TimeSpan tp = TimeSpan.Parse(test1);

                TimeSpan dt = TimeSpan.FromHours(ts.Hours);
                TimeSpan dt1 = TimeSpan.FromHours(tp.Hours);

                TimeSpan dt2 = TimeSpan.FromMinutes(ts.Minutes);
                TimeSpan dt3 = TimeSpan.FromMinutes(tp.Minutes);

                TimeSpan span3 = dt1.Subtract(dt);
                TimeSpan span4 = dt3.Subtract(dt2);

                TimeSpan span5 = span3.Add(span4);
                if (span5.Days > 0)
                    ViewBag.newlead = String.Format("{0} {1} ago",
                    span5.Days, span5.Days == 1 ? "day" : "days");
                else if (span5.Hours > 0)
                    ViewBag.newlead = String.Format("{0} {1} ago",
                    span5.Hours, span5.Hours == 1 ? "hour" : "hours");
                else if (span5.Minutes > 0)
                    ViewBag.newlead = String.Format("{0} {1} ago",
                    span5.Minutes, span5.Minutes == 1 ? "min" : "mins");
                else if (span5.Seconds <= 5)
                    ViewBag.newlead = "just now";
            }
            ViewBag.newleadcount = leadcount;
            //end of new leads

            //Return Dropped Leads Counts
            int dropcount = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.IsTime == 0).Where(m => m.IsStatus == 2).Where(m => m.CreatedOn >= startDate && m.CreatedOn <= enddate).Count();

            if (dropcount != 0)
            {
                var test22 = db.LeadEnquiry.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.LEID).First();
                string testd = Convert.ToString(test22.LeadTime);
                string test11 = Convert.ToString(System.DateTime.Now.TimeOfDay);
                TimeSpan tsd = TimeSpan.Parse(Convert.ToDateTime(testd).TimeOfDay.ToString());
                TimeSpan tpd = TimeSpan.Parse(test11);

                TimeSpan dtd = TimeSpan.FromHours(tsd.Hours);
                TimeSpan dt1d = TimeSpan.FromHours(tpd.Hours);

                TimeSpan dt2d = TimeSpan.FromMinutes(tsd.Minutes);
                TimeSpan dt3d = TimeSpan.FromMinutes(tpd.Minutes);

                TimeSpan span3d = dt1d.Subtract(dtd);
                TimeSpan span4d = dt3d.Subtract(dt2d);

                TimeSpan span5d = span3d.Add(span4d);
                if (span5d.Days > 0)
                    ViewBag.dropleads = String.Format("{0} {1} ago",
                    span5d.Days, span5d.Days == 1 ? "day" : "days");
                else if (span5d.Hours > 0)
                    ViewBag.dropleads = String.Format("{0} {1} ago",
                    span5d.Hours, span5d.Hours == 1 ? "hour" : "hours");
                else if (span5d.Minutes > 0)
                    ViewBag.dropleads = String.Format("{0} {1} ago",
                    span5d.Minutes, span5d.Minutes == 1 ? "min" : "mins");
                else if (span5d.Seconds <= 5)
                    ViewBag.dropleads = "just now";
            }
            ViewBag.dropleadscount = dropcount;
            //Ends of dropped leads

            //New Quotation Generated
            //int newquotationcount = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).Where(m => m.IsTime == 0).Where(m => m.QuotationDate >= startDate && m.QuotationDate <= enddate).Count();

            var newquotationcount = (from r in db.QGEquipGeneralData
                                     where r.IsRiceMill == 0 && r.IsTime == 0 && r.QuotationDate >= startDate && r.QuotationDate <= enddate
                                     select r).Count();

            if (newquotationcount != 0)
            {
                var test220 = db.QGEquipGeneralData.Where(m => m.IsRiceMill == 0).OrderByDescending(m => m.QGID).First();
                string testdq = Convert.ToString(test220.LeadTime);
                string test11q = Convert.ToString(System.DateTime.Now.TimeOfDay);
                TimeSpan tsdq = TimeSpan.Parse(Convert.ToDateTime(testdq).TimeOfDay.ToString());
                TimeSpan tpdq = TimeSpan.Parse(test11q);

                TimeSpan dtdq = TimeSpan.FromHours(tsdq.Hours);
                TimeSpan dt1dq = TimeSpan.FromHours(tpdq.Hours);

                TimeSpan dt2dq = TimeSpan.FromMinutes(tsdq.Minutes);
                TimeSpan dt3dq = TimeSpan.FromMinutes(tpdq.Minutes);

                TimeSpan span3dq = dt1dq.Subtract(dtdq);
                TimeSpan span4dq = dt3dq.Subtract(dt2dq);

                TimeSpan span5dq = span3dq.Add(span4dq);
                if (span5dq.Days > 0)
                    ViewBag.newquotation = String.Format("{0} {1} ago",
                    span5dq.Days, span5dq.Days == 1 ? "day" : "days");
                else if (span5dq.Hours > 0)
                    ViewBag.newquotation = String.Format("{0} {1} ago",
                    span5dq.Hours, span5dq.Hours == 1 ? "hour" : "hours");
                else if (span5dq.Minutes > 0)
                    ViewBag.newquotation = String.Format("{0} {1} ago",
                    span5dq.Minutes, span5dq.Minutes == 1 ? "min" : "mins");
                else if (span5dq.Seconds <= 5)
                    ViewBag.newquotation = "just now";
            }
            ViewBag.newquotationcount = newquotationcount;
            //Quotation Ends


            //Order generated
            var ordergeneratedcount = (from r in db.OAEquipGeneralData
                                       where r.OAStatus == 1 && r.ApprovalStatus == 1 && r.OADate >= startDate && r.OADate <= enddate
                                       select r).Count();

            if (ordergeneratedcount != 0)
            {
                var test220 = db.OAEquipGeneralData.Where(m => m.OAStatus == 1).Where(m => m.ApprovalStatus == 1).OrderByDescending(m => m.OAID).First();
                DateTime timebreak = Convert.ToDateTime(test220.OADate);
                string timebreak1 = timebreak.ToString("hh:mm:ss");
                string testdq = Convert.ToString(timebreak1);
                string test11q = Convert.ToString(System.DateTime.Now.TimeOfDay);
                TimeSpan tsdq = TimeSpan.Parse(testdq);
                TimeSpan tpdq = TimeSpan.Parse(test11q);

                TimeSpan dtdq = TimeSpan.FromHours(tsdq.Hours);
                TimeSpan dt1dq = TimeSpan.FromHours(tpdq.Hours);

                TimeSpan dt2dq = TimeSpan.FromMinutes(tsdq.Minutes);
                TimeSpan dt3dq = TimeSpan.FromMinutes(tpdq.Minutes);

                TimeSpan span3dq = dt1dq.Subtract(dtdq);
                TimeSpan span4dq = dt3dq.Subtract(dt2dq);

                TimeSpan span5dq = span3dq.Add(span4dq);
                if (span5dq.Days > 0)
                    ViewBag.odergenerated = String.Format("{0} {1} ago",
                    span5dq.Days, span5dq.Days == 1 ? "day" : "days");
                else if (span5dq.Hours > 0)
                    ViewBag.odergenerated = String.Format("{0} {1} ago",
                    span5dq.Hours, span5dq.Hours == 1 ? "hour" : "hours");
                else if (span5dq.Minutes > 0)
                    ViewBag.odergenerated = String.Format("{0} {1} ago",
                    span5dq.Minutes, span5dq.Minutes == 1 ? "min" : "mins");
                else if (span5dq.Seconds <= 5)
                    ViewBag.odergenerated = "just now";
            }
            ViewBag.ordergeneratedcount = ordergeneratedcount;
            //Order generated Ends Here
            #endregion
            //Notofication Ends Here

            //Auto Mailer to Channel Partner Lead & SOT
            #region
            var type = db.AutoMailer.ToList();
            int lperiod = 0;
            foreach (var t in type)
            {
                string module = t.Module;
                lperiod = t.OTPeriod;
                if (module == "Leads")
                {
                    var cpidlist = db.ChannelPartners.ToList();
                    foreach (var c in cpidlist)
                    {
                        var leadlist = db.LeadEnquiry.Where(m => m.IsDeleted == 0).Where(m => m.CPID == c.CPID).OrderByDescending(m => m.CreatedOn).FirstOrDefault();

                        if (leadlist != null)
                        {
                            DateTime lastdate = Convert.ToDateTime(leadlist.CreatedOn);
                            TimeSpan span = DateTime.Now - lastdate;
                            int day = span.Days;
                            if (day > lperiod)
                            {
                                int cpid = leadlist.CPID;

                                int mailcount = db.AutoMailSystem.Where(m => m.CPID == cpid).Where(m => m.MailSentDate == yesterday).Where(m => m.IsSent == 1).Count();
                                if (mailcount != 1)
                                {
                                    int flag = 0;
                                    mailservices1 objMD = new mailservices1();
                                    objMD.sendMailLEAD(cpid, module, lastdate); // calling method sendMailLEAD

                                    ////inserting into automailing system
                                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
                                    {
                                        int channelpertnerid = cpid;
                                        DateTime mailsentdat = System.DateTime.Now;
                                        int issent = 1;
                                        SqlCommand cmd = new SqlCommand("Sp_Insert_AutoMailSystem", con);
                                        cmd.Parameters.AddWithValue("CPID", @channelpertnerid);
                                        cmd.Parameters.AddWithValue("MailDate", @mailsentdat);
                                        cmd.Parameters.AddWithValue("IsSent", @issent);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        SqlDataAdapter da = new SqlDataAdapter();
                                        da.SelectCommand = cmd;
                                        DataTable dt = new DataTable();
                                        da.Fill(dt);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var cpidlist = db.ChannelPartners.ToList();
                    foreach (var c in cpidlist)
                    {
                        var leadlist = db.SOT.Where(m => m.CPID == c.CPID).OrderByDescending(m => m.ModifiedOn).FirstOrDefault();

                        if (leadlist != null)
                        {
                            DateTime lastdate = Convert.ToDateTime(leadlist.ModifiedOn);
                            TimeSpan span = DateTime.Now - lastdate;
                            int day = span.Days;
                            if (day > lperiod)
                            {
                                int cpid = leadlist.CPID;
                                int mailcount = db.AutoMailSystem.Where(m => m.CPID == cpid).Where(m => m.MailSentDate == yesterday).Where(m => m.IsSent == 1).Count();
                                if (mailcount != 1)
                                {
                                    int flag = 0;
                                    mailservices1 objMD = new mailservices1();
                                    objMD.sendMailSOT(cpid, module, lastdate); // calling method sendMailLEAD

                                    ////inserting into automailing system
                                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SRKS_Synergy"].ToString()))
                                    {
                                        int channelpertnerid = cpid;
                                        DateTime mailsentdat = System.DateTime.Now;
                                        int issent = 1;
                                        SqlCommand cmd = new SqlCommand("Sp_Insert_AutoMailSystem", con);
                                        cmd.Parameters.AddWithValue("CPID", @channelpertnerid);
                                        cmd.Parameters.AddWithValue("MailDate", @mailsentdat);
                                        cmd.Parameters.AddWithValue("IsSent", @issent);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        SqlDataAdapter da = new SqlDataAdapter();
                                        da.SelectCommand = cmd;
                                        DataTable dt = new DataTable();
                                        da.Fill(dt);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            //Ends of Auto Mailer

            //// Storing Latitude and Longitude
            //#region
            //var master = db.LeadEnquiry.Where(m => m.Latitude == 11111111).ToList();
            //int countmdbid = master.Count;
            //try
            //{
            //    foreach (var MDB in master)
            //    {
            //        int mdbid = MDB.LEID;
            //        var fulladdress = MDB.AddressLine1 + "," + MDB.AddressLine2 + "," + MDB.City + "," + MDB.Pincode + "," + MDB.State + "," + MDB.Country;

            //        mailservices1 objMD = new mailservices1();
            //        objMD.StoreLatLang(fulladdress, mdbid);

            //    }
            //}
            //catch (Exception ex)
            //{
            //    foreach (var MDB in master)
            //    {
            //        int mdbid = MDB.LEID;
            //        var fulladdress = MDB.State + "," + MDB.Country;

            //        mailservices1 objMD = new mailservices1();
            //        objMD.StoreLatLang(fulladdress, mdbid);
            //    }

            //}
            //#endregion

            ////ends latitude and longitude
            return View();
        }

        public static string TimeAgo(string Duration)
        {
            DateTime d = Convert.ToDateTime(Duration);
            TimeSpan span = DateTime.Now - d;

            if (span.Days > 0)
                return String.Format("about {0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("about {0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("about {0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "min" : "mins");
            if (span.Seconds <= 5)
                return "just now";
            return string.Empty;
        }

        public ActionResult GraphChart()
        {
            return View();
        }

        //public JsonResult Piechart()
        //{
        //    mydataservice objMD = new mydataservice();

        //    var chartsdata = objMD.Listdata(); // calling method Listdata
        //    return Json(chartsdata, JsonRequestBehavior.AllowGet); // returning list from here.
        //}

        //TO check Total Lead Generation
        public JsonResult Piechart2()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date

            mydataservice objMD = new mydataservice();

            var chartsdata1 = objMD.Listdata1(); // calling method Listdata
            return Json(chartsdata1, JsonRequestBehavior.AllowGet); // returning list from here.
        }

        //TO check Open/ closed Leads
        public JsonResult Piechart4()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date
            //DateTime today = DateTime.Today;
            //DateTime endOfMonth = new DateTime(today.Year,today.Month,DateTime.DaysInMonth(today.Year,today.Month));

            mydataservice objMD = new mydataservice();
            var chartsdata3 = objMD.Listdata3(); // calling method Listdata
            return Json(chartsdata3, JsonRequestBehavior.AllowGet); // returning list from here.
        }

        //TO check Quotation Generation
        public JsonResult Piechart3()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date
            mydataservice objMD = new mydataservice();
            var chartsdata2 = objMD.Listdata2(); // calling method Listdata
            return Json(chartsdata2, JsonRequestBehavior.AllowGet); // returning list from here.
        }

        //TO check Quotation Generation
        public JsonResult Piechart5()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enddate = startDate.AddMonths(1).AddDays(-1);
            //var enddate = System.DateTime.Now; // it will access present (system) date
            mydataservice objMD = new mydataservice();
            var chartsdata4 = objMD.Listdata4(); // calling method Listdata
            return Json(chartsdata4, JsonRequestBehavior.AllowGet); // returning list from here.
        }



    }
}
