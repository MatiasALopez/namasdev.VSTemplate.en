using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using namasdev.Core.Validation;
using namasdev.Web.Helpers;

using MyApp.Data;
using MyApp.Business;
using MyApp.Business.DTO.Emails;
using MyApp.Web.Portal.Metadata;
using MyApp.Web.Portal.Metadata.Views;
using MyApp.Web.Portal.Models;
using MyApp.Web.Portal.ViewModels;

namespace MyApp.Web.Portal.Controllers
{
    [Authorize]
    public class AccountController : ControllerBase
    {
        public const string NAME = "Account";

        private ApplicationSignInManager _signInManager;

        private readonly IUsersRepository _usersRepository;
        private readonly IUsersBusiness _usersBusiness;
        private readonly IEmailsBusiness _emailsBusiness;

        public AccountController(IUsersRepository usersRepository, IUsersBusiness usersBusiness, IEmailsBusiness emailsBusiness, IMapper mapper)
            : base(mapper)
        {
            Validator.ValidateRequiredArgumentAndThrow(usersRepository, nameof(usersRepository));
            Validator.ValidateRequiredArgumentAndThrow(usersBusiness, nameof(usersBusiness));
            Validator.ValidateRequiredArgumentAndThrow(emailsBusiness, nameof(emailsBusiness));

            _usersRepository = usersRepository;
            _usersBusiness = usersBusiness;
            _emailsBusiness = emailsBusiness;
        }

        public AccountController(ApplicationSignInManager signInManager, IMapper mapper)
            : base(mapper)
        {
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        #region Actions

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View(SharedViews.ACCESS_DENIED);

                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", Text.INVALID_DATA);
                    return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            SignOutAndClearSession();

            return RedirectToAction(nameof(HomeController.Index), HomeController.NAME);
        }

        [AllowAnonymous]
        public ActionResult ActivateAccount(string id, string code)
        {
            if (String.IsNullOrWhiteSpace(id) || String.IsNullOrWhiteSpace(code))
            {
                return View(SharedViews.ERROR);
            }

            var userIdentity = UserManager.FindById(id);
            if (userIdentity == null)
            {
                return View(SharedViews.ERROR);
            }

            var model = new ActivateAccountViewModel
            {
                Code = code,
                Email = userIdentity.Email,
            };

            return View(model);
        }

        [HttpPost,
        AllowAnonymous]
        public async Task<ActionResult> ActivateAccount(string id, ActivateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdentity = UserManager.FindById(id);
                if (userIdentity == null)
                {
                    return View(SharedViews.ERROR);
                }

                var result = await UserManager.ConfirmEmailAsync(id, model.Code);
                if (result.Succeeded)
                {
                    result = UserManager.AddPassword(id, model.Password);
                    if (result.Succeeded)
                    {
                        string emailPrevious = userIdentity.Email;
                        if (!String.Equals(emailPrevious, model.Email, StringComparison.CurrentCultureIgnoreCase))
                        {
                            UpdateEmailAndUserNameForApplicationUser(userIdentity, model.Email);

                            try
                            {
                                UpdateActivatedAccountEmailForEntity(userIdentity.Id, model.Email);
                            }
                            catch (Exception)
                            {
                                // NOTA: "manual rollback" for Identity
                                UpdateEmailAndUserNameForApplicationUser(userIdentity, emailPrevious);

                                throw;
                            }
                        }

                        return View(AccountViews.ACTIVATE_ACCOUNT_CONFIRMATION);
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                else
                {
                    AddErrors(result);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        throw new Exception("No user found with the provided email.");
                    }

                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        throw new Exception("Your account is not activated yet.");
                    }

                    _emailsBusiness.SendResetPassword(
                        new SendResetPasswordParameters 
                        { 
                            Email = model.Email,
                            FullName = GetUserFullName(user.Id),
                            ResetPasswordUrl = UrlBuilder.BuildAbsoluteUrl(Url.Action(nameof(ResetPassword), new { id = user.Id, code = UserManager.GeneratePasswordResetToken(user.Id) }))
                        });

                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
            }
            catch (Exception ex)
            {
                ControllerHelper.SetMessageError(ex.Message);
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string id, string code)
        {
            if (String.IsNullOrWhiteSpace(id) || String.IsNullOrWhiteSpace(code))
            {
                return View(SharedViews.ERROR);
            }

            var user = UserManager.FindById(id);
            if (user == null)
            {
                return View(SharedViews.ERROR);
            }

            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id, ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindById(id);

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View(AccountViews.RESET_PASSWORD_CONFIRMATION);
            }

            AddErrors(result);

            return View(model);
        }

        #endregion

        #region Methods

        private void ValidarIdentityResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception(String.Join(Environment.NewLine, result.Errors));
            }
        }

        private void UpdateEmailAndUserNameForApplicationUser(ApplicationUser user, string email)
        {
            user.Email = user.UserName = email;

            var result = UserManager.Update(user);
            ValidarIdentityResult(result);
        }

        private void UpdateActivatedAccountEmailForEntity(string userId, string email)
        {
            var user = _usersRepository.Get(userId);
            if (user != null)
            {
                user.Email = email;
                user.ModifiedBy = userId;
                user.ModifiedAt = DateTime.Now;
                _usersRepository.Update(user);
            }
        }

        private string GetUserFullName(string id)
        {
            var user = _usersRepository.Get(id);
            if (user != null)
            {
                return user.FullName;
            }
            else
            {
                throw new Exception(Validator.Messages.EntityNotFound("User", id));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), HomeController.NAME);
        }

        #endregion
    }
}