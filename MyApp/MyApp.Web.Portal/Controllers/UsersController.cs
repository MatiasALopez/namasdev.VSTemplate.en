using System;
using System.Collections.Generic;
using System.Web.Mvc;

using AutoMapper;
using Microsoft.AspNet.Identity;
using namasdev.Core.Validation;
using namasdev.Web.Helpers;
using namasdev.Web.Models;

using MyApp.Entities;
using MyApp.Entities.Metadata;
using MyApp.Entities.Values;
using MyApp.Data;
using MyApp.Business;
using MyApp.Business.DTO.Emails;
using MyApp.Business.DTO.Users;
using MyApp.Web.Portal.Metadata.Views;
using MyApp.Web.Portal.Models.Users;
using MyApp.Web.Portal.ViewModels.Users;

namespace MyApp.Web.Portal.Controllers
{
    [Authorize(Roles = AspNetRoles.ADMINISTRATOR)]
    public class UsersController : ControllerBase
    {
        public const string NAME = "Users";

        private const string USERS_NOT_FOUND_MESSAGE = "User not found.";
        private const string USER_ALREADY_ACTIVATED_MESSAGE = "User already activated.";
        private const string USER_NOT_ACTIVATED_MESSAGE = "User not activated.";
        private const string DELETE_LOGGED_USER_ERROR_MESSAGE = "You cannot delete your own user.";
        private const string USER_ALREADY_ACTIVE_MESSAGE = "User already active.";

        private readonly IUsersRepository _usersRepository;
        private readonly IUsersBusiness _usersBusiness;
        private readonly IEmailsBusiness _emailsBusiness;

        public UsersController(IUsersRepository usersRepository, IUsersBusiness usersBusiness, IEmailsBusiness emailsBusiness, IMapper mapper)
            : base(mapper)
        {
            Validator.ValidateRequiredArgumentAndThrow(usersRepository, nameof(usersRepository));
            Validator.ValidateRequiredArgumentAndThrow(usersBusiness, nameof(usersBusiness));
            Validator.ValidateRequiredArgumentAndThrow(emailsBusiness, nameof(emailsBusiness));

            _usersRepository = usersRepository;
            _usersBusiness = usersBusiness;
            _emailsBusiness = emailsBusiness;
        }

        #region Actions

        public ActionResult Active(
            string role = null, string search = null,
            string order = null,
            int page = 1)
        {
            var modelo = new UsersViewModel
            {
                Role = role,
                Search = search,
                Page = page,
                Order = order,
            };

            LoadUsersViewModel(modelo, UsersPageMode.Active);
            return View(UsersViews.USERS, modelo);
        }

        public ActionResult Inactive(
            string role = null, string search = null,
            string order = null,
            int page = 1)
        {
            var modelo = new UsersViewModel
            {
                Role = role,
                Search = search,
                Page = page,
                Order = order,
            };

            LoadUsersViewModel(modelo, UsersPageMode.Inactive);
            return View(UsersViews.USERS, modelo);
        }

        public ActionResult Add()
        {
            var model = new UserViewModel();
            LoadUserViewModel(model, PageMode.Add);

            return View(UsersViews.USER, model);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Add(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = string.Empty;
                    if (_usersRepository.ExistsDeletedByEmail(model.Email, out userId))
                    {
                        model.UnsetDeletedUserId = userId;
                    }
                    else
                    {
                        var user = _usersBusiness.Add(Map<AddParameters>(model));
                        SendActivationEmail(user);

                        ControllerHelper.SetMessageSuccess(UserMetadata.Messages.ADD_OK);

                        ModelState.Clear();
                        model = new UserViewModel();
                    }
                }
            }
            catch (Exception ex)
            {
                ControllerHelper.SetMessageError(ex.Message);
            }

            LoadUserViewModel(model, PageMode.Add);
            return View(UsersViews.USER, model);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult UnsetDeleted(string id)
        {
            _usersBusiness.UnsetDeleted(new UnsetDeletedParameters
            {
                Id = id,
                LoggedUserId = UserId
            });

            return Json(new { success = true });
        }

        public ActionResult Edit(string id)
        {
            var user = _usersRepository.Get(id, loadRoles: true);
            if (user == null)
            {
                return RedirectToAction(nameof(Active));
            }

            var model = Map<UserViewModel>(user);
            LoadUserViewModel(model, PageMode.Edit);

            return View(UsersViews.USER, model);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _usersBusiness.Update(Map<UpdateParameters>(model));
                    
                    ControllerHelper.SetMessageSuccess(UserMetadata.Messages.UPDATE_OK);
                }
            }
            catch (Exception ex)
            {
                ControllerHelper.SetMessageError(ex.Message);
            }

            LoadUserViewModel(model, PageMode.Edit);
            return View(UsersViews.USER, model);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult ResendActivation(string id)
        {
            var userIdentity = UserManager.FindById(id);
            if (userIdentity == null)
            {
                return Json(new { success = false, message = USERS_NOT_FOUND_MESSAGE });
            }

            if (UserManager.IsEmailConfirmed(userIdentity.Id))
            {
                return Json(new { success = false, message = USER_ALREADY_ACTIVATED_MESSAGE });
            }

            var user = _usersRepository.Get(userIdentity.Id);
            if (user == null)
            {
                return Json(new { success = false, message = USERS_NOT_FOUND_MESSAGE });
            }

            SendActivationEmail(user);

            return Json(new { success = true });
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string id)
        {
            var userIdentity = UserManager.FindById(id);
            if (userIdentity == null)
            {
                return Json(new { success = false, message = USERS_NOT_FOUND_MESSAGE });
            }

            if (!UserManager.IsEmailConfirmed(userIdentity.Id))
            {
                return Json(new { success = false, message = USER_NOT_ACTIVATED_MESSAGE });
            }

            var user = _usersRepository.Get(userIdentity.Id);
            if (user == null)
            {
                return Json(new { success = false, message = USERS_NOT_FOUND_MESSAGE });
            }

            _emailsBusiness.SendResetPassword(
                new SendResetPasswordParameters
                {
                    Email = user.Email,
                    FullName = user.ToString(),
                    ResetPasswordUrl = UrlBuilder.BuildAbsoluteUrl(Url.Action(nameof(AccountController.ResetPassword), AccountController.NAME, new { id = userIdentity.Id, code = UserManager.GeneratePasswordResetToken(userIdentity.Id) })),
                });

            return Json(new { success = true });
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var user = _usersBusiness.Get(id,
                validateExists: false);
            if (user == null)
            {
                return Json(new { success = false, message = USERS_NOT_FOUND_MESSAGE });
            }

            if (user.Id == UserId)
            {
                return Json(new { success = false, message = DELETE_LOGGED_USER_ERROR_MESSAGE });
            }

            try
            {
                _usersBusiness.SetDeleted(new SetDeletedParameters
                {
                    Id = id,
                    LoggedUserId = UserId
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = UserMetadata.Messages.DELETE_ERROR });
            }

            return Json(new { success = true });
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Reactivate(string id)
        {
            var user = _usersRepository.Get(id,
                includeDeleted: true);
            if (user == null)
            {
                return Json(new { success = false, message = USERS_NOT_FOUND_MESSAGE });
            }

            if (!user.Deleted)
            {
                return Json(new { success = false, message = USER_ALREADY_ACTIVE_MESSAGE });
            }

            try
            {
                _usersBusiness.UnsetDeleted(new UnsetDeletedParameters
                {
                    Id = id,
                    LoggedUserId = UserId
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = UserMetadata.Messages.REACTIVATE_ERROR });
            }

            return Json(new { success = true });
        }

        #endregion Actions

        #region Methods

        private void LoadUsersViewModel(UsersViewModel model, UsersPageMode pageMode)
        {
            Validator.ValidateRequiredArgumentAndThrow(model, nameof(model));

            model.PageMode = pageMode;

            var op = model.CreateOrderAndPagingParameters();

            model.Items = Map<List<UserItemModel>>(_usersRepository.GetList(
                deleted: pageMode == UsersPageMode.Active ? false : true,
                search: model.Search, 
                role: model.Role,
                loadRoles: true,
                op: op));

            var idsNotActivated = UserManager.GetIdsNotActivated();
            foreach (var i in model.Items)
            {
                i.Activated = !idsNotActivated.Contains(i.Id);
            }

            model.SetPagination(op);

            model.RolesSelectList = ListHelper.GetRolesSelectList(_usersRepository.GetRoles());
        }

        private void LoadUserViewModel(UserViewModel model, PageMode pageMode)
        {
            Validator.ValidateRequiredArgumentAndThrow(model, nameof(model));

            model.PageMode = pageMode;
            model.RolesSelectList = ListHelper.GetRolesSelectList(_usersRepository.GetRoles());
        }

        private void SendActivationEmail(User user)
        {
            _emailsBusiness.SendActivateAccount(
                new SendActivateAccountParameters
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    ActivateAccountUrl = UrlBuilder.BuildAbsoluteUrl(Url.Action(nameof(AccountController.ActivateAccount), AccountController.NAME, new { id = user.Id, code = UserManager.GenerateEmailConfirmationToken(user.Id) })),
                });
        }

        #endregion Methods
    }
}