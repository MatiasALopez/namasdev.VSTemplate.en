using System.ComponentModel.DataAnnotations;

using MyApp.Entities.Metadata;

namespace MyApp.Web.Portal.Models.Users
{
    public class UserItemModel
    {
        public string Id { get; set; }

        [Display(Name = UserMetadata.Properties.FirstName.LABEL)]
        public string FirstName { get; set; }

        [Display(Name = UserMetadata.Properties.LastName.LABEL)]
        public string LastName { get; set; }

        [Display(Name = UserMetadata.Properties.Email.LABEL)]
        public string Email { get; set; }

        [Display(Name = AspNetRoleMetadata.LABEL)]
        public string Role { get; set; }

        public bool Activated { get; set; }
    }
}