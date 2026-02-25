using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using namasdev.Web.Models;

using MyApp.Entities.Metadata;

namespace MyApp.Web.Portal.ViewModels.Users
{
    public class UserViewModel : IValidatableObject
    {
        public PageMode PageMode { get; set; }

        public string Id { get; set; }

        [Display(Name = UserMetadata.Properties.FirstName.LABEL),
        Required(ErrorMessage = namasdev.Core.Validation.Validator.Messages.Formats.REQUIRED)]
        public string FirstName { get; set; }

        [Display(Name = UserMetadata.Properties.LastName.LABEL),
        Required(ErrorMessage = namasdev.Core.Validation.Validator.Messages.Formats.REQUIRED)]
        public string LastName { get; set; }

        [Display(Name = UserMetadata.Properties.Email.LABEL),
        Required(ErrorMessage = namasdev.Core.Validation.Validator.Messages.Formats.REQUIRED),
        EmailAddress]
        public string Email { get; set; }

        [Display(Name = AspNetRoleMetadata.LABEL),
        Required(ErrorMessage = namasdev.Core.Validation.Validator.Messages.Formats.REQUIRED)]
        public string Role { get; set; }

        public string UnsetDeletedUserId { get; set; }

        public SelectList RolesSelectList { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PageMode == PageMode.Edit
                && String.IsNullOrWhiteSpace(Id))
            {
                yield return new ValidationResult(namasdev.Core.Validation.Validator.Messages.Required(UserMetadata.LABEL), new string[] { nameof(Id) });
            }
        }
    }
}