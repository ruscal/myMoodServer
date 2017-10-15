using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Discover.DomainModel;
using Discover.Logging;
using Discover.Security;
using Discover.Web.Mvc;
using MyMood.Domain;
using MyMood.Web.Models;

namespace MyMood.Web.Controllers
{
    public partial class UserController : ControllerBase
    {
        public UserController(IDomainDataContext db, ILogger log, Discover.Mail.IMailDispatchService mailer, Discover.HtmlTemplates.IHtmlTemplateManager htmlTemplates)
            : base (db, log, mailer, htmlTemplates)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult LogIn()
        {
            if (Request.IsAuthenticated) Response.Redirect("~/");
            return View(new UserLogInFormModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult LogIn(UserLogInFormModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Get<User>().Where(u => u.UserName == model.UserName).FirstOrDefault();

                if (user != null && user.VerifyLoginAttempt(model.Password))
                {
                    db.SaveChanges();

                    FormsAuthentication.SetAuthCookie(user.UserName, model.RememberMe);

                    return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You have entered your User Name or Password incorrectly (or your account is locked)");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public virtual ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("LogIn");
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult ForgottenPassword()
        {
            return View(new ForgottenPasswordFormModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult ForgottenPassword(ForgottenPasswordFormModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Get<User>().Where(u => u.UserName == model.UserName).FirstOrDefault();

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Please check that you have entered your User Name correctly, as we cannot find your account");
                }
                else if (string.IsNullOrEmpty(user.EmailAddress))
                {
                    ModelState.AddModelError(string.Empty, "Sorry, we cannot automatically reset your password, as we do not have an email address associated with your account.  Please contact your systems administrator.");
                }
                else
                {
                    user.ResetPassword();

                    db.SaveChanges();

                    ViewBag.ResetSuccessful = true;
                }
            }

            return View(model);
        }

        [HttpGet]
        public virtual ActionResult ChangeMyPassword()
        {
            return Request.IsAjaxRequest() ?
                PartialView(MVC.User.Views.ChangePassword, new ChangePasswordFormModel()) as ActionResult :
                View(MVC.User.Views.ChangeMyPassword, new ChangePasswordFormModel()) as ActionResult;
        }

        [HttpPost]
        public virtual ActionResult ChangeMyPassword(ChangePasswordFormModel model)
        {
            if (ModelState.IsValid)
            {
                var errors = User.Identity.UserInfo.ValidateNewPassword(model.NewPassword);

                if (errors.Any())
                {
                    ModelState.Add(errors);
                }
                else
                {
                    try
                    {
                        if (User.Identity.UserInfo.PasswordEquals(model.CurrentPassword))
                        {
                            User.Identity.UserInfo.SetPassword(model.NewPassword);

                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("CurrentPassword", "The value you entered does not match your current password");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            if (Request.IsAjaxRequest())
            {
                return ModelState.IsValid ?
                    Json(new { success = true }) :
                    Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.User.Views.ChangePassword, model) });
            }
            else
            {
                return ModelState.IsValid ?
                    RedirectToRoute("Home").WithFlashMessage("Your password has been updated") as ActionResult :
                    View(MVC.User.Views.ChangeMyPassword, model) as ActionResult;
            }
        }

        [HttpGet]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult ManageUsers()
        {
            var model = new ManageUsersViewModel()
            {
                Users = (from u in db.Get<User>()
                         select new UserViewModel
                         {
                             Id = u.Id,
                             UserName = u.UserName,
                             DisplayName = u.DisplayName,
                             EmailAddress = u.EmailAddress,
                             LastLoggedIn = u.LastLoginDate,
                             IsApproved = u.IsApproved,
                             IsLockedOut = u.IsLockedOut
                         })
                         .ToArray(),
                AvailableSecurityRoles = UserRole.GetRoles().Select(r => new SelectListItem { Text = r.Value, Value = r.Key }).ToArray()
            };

            return View(model);
        }

        [HttpGet]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult AddUser()
        {
            var model = new AddUserFormModel()
            {
                AvailableRoles = UserRole.GetRoles().Select(r => new SelectListItem { Text = r.Value, Value = r.Key }).ToArray()
            };

            return PartialView(MVC.User.Views.AddUser, model, "NewUser");
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult AddUser([Bind(Prefix = "NewUser")]AddUserFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (this.db.Get<User>().Any(u => u.UserName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ModelState.AddModelError("NewUser.UserName", "A user with that user name already exists");
                    }
                    else
                    {
                        var newUser = Domain.User.RegisterNewUser(db, 
                            model.UserName, 
                            Domain.User.GeneratePassword(), 
                            model.EmailAddress, 
                            model.DisplayName, 
                            model.Enabled,
                            UserRole.GetRoles().Where(r => model.AssignedRoles.Contains(r.Key)).Select(r => r.Key).ToArray());

                        newUser.ExpirePassword();

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("NewUser", ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = UserRole.GetRoles().Select(r => new SelectListItem { Text = r.Value, Value = r.Key, Selected = model.AssignedRoles.Contains(r.Key) }).ToArray();
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.User.Views.AddUser, model, "NewUser") });
        }

        [HttpGet]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult EditUser(Guid userId)
        {
            var user = db.Get<User>().Where(u => u.Id == userId).Single();

            var model = new EditUserFormModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                EmailAddress = user.EmailAddress,
                DisplayName = user.DisplayName,
                Enabled = user.IsApproved,
                AssignedRoles = user.Roles.ToArray(),
                AvailableRoles = UserRole.GetRoles().Select(r => new SelectListItem { Text = r.Value, Value = r.Key, Selected = user.IsInRole(r.Key) }).ToArray()
            };

            return PartialView(MVC.User.Views.EditUser, model, "ExistingUser");
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult EditUser(Guid userId, [Bind(Prefix = "ExistingUser")]EditUserFormModel model)
        {
            var user = db.Get<User>().Where(u => u.Id == userId).Single();

            if (ModelState.IsValid)
            {
                try
                {
                    if (this.db.Get<User>().Any(u => u.UserName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase) && u.Id != user.Id))
                    {
                        ModelState.AddModelError("ExistingUser.UserName", "A user with that user name already exists");
                    }
                    else
                    {
                        user.SetUserName(model.EmailAddress);
                        user.SetEmailAddress(model.EmailAddress);
                        user.SetDisplayName(model.DisplayName);
                        user.SetApprovalStatus(model.Enabled);

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ExistingUser", ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = UserRole.GetRoles().Select(r => new SelectListItem { Text = r.Value, Value = r.Key, Selected = model.AssignedRoles.Contains(r.Key) }).ToArray();
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.User.Views.EditUser, model, "ExistingUser") });
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult RemoveUser(Guid userId)
        {
            try
            {
                var user = db.Get<User>().Where(u => u.Id == userId).Single();

                if (user.Id == User.Identity.UserInfo.Id) throw new InvalidOperationException("Users cannot remove themselves from the system");

                db.Remove(user);

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult ResetPassword(Guid userId)
        {
            var userToReset = db.Get<User>().Where(u => u.Id == userId).Single();

            var model = new ResetPasswordFormModel()
            {
                UserId = userToReset.Id,
                UserName = userToReset.UserName,
                SendToEmailAddress = userToReset.EmailAddress
            };

            return PartialView(MVC.User.Views.ResetPassword, model, "ResetPassword");
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult ResetPassword(Guid userId, [Bind(Prefix = "ResetPassword")]ResetPasswordFormModel model)
        {
            var userToReset = db.Get<User>().Where(u => u.Id == userId).Single();

            if (ModelState.IsValid)
            {
                try
                {
                    var newPassword = userToReset.ResetPassword();

                    if (!string.Equals(model.SendToEmailAddress, userToReset.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var mailContent = htmlTemplateManager.GetHtml("MailTemplates/UserPasswordReset")
                            .Replace("{Username}", userToReset.UserName)
                            .Replace("{Password}", newPassword)
                            .Replace("{SignInUrl}", Url.RouteUrl("Home"));

                        var message = new Discover.Mail.MailMessage();

                        message.To.Add(new Discover.Mail.MailAddress() { Address = model.SendToEmailAddress });
                        message.Subject = "Your Password Has Been Reset";
                        message.Body = mailContent;
                        message.IsBodyHtml = true;

                        mailer.Send(message);
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ResetPassword", ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                model.UserName = userToReset.UserName;
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.User.Views.ResetPassword, model, "ResetPassword") });
        }

        [HttpPost]
        [AuthorizeRoles(UserRole.SystemAdmin)]
        public virtual ActionResult UnlockUser(Guid userId)
        {
            var user = db.Get<User>().Where(u => u.Id == userId).Single();

            if (ModelState.IsValid)
            {
                try
                {
                    user.Unlock();

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex);
                }
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, modelState = ModelState.ToErrorInfoObjects() });
        }

    }
}
