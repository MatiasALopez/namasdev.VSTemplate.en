using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using namasdev.Web.ViewModels;

using MyApp.Web.Portal.Models.Users;
using MyApp.Entities.Metadata;

namespace MyApp.Web.Portal.ViewModels.Users
{
    public class UsersViewModel : PaginatedListViewModel<UserItemModel>
    {
        public UsersViewModel()
        {
            ItemsPerPage = 10;
        }

        public UsersPageMode PageMode { get; set; }

        [Display(Name = "Search (First Name, Last Name, Email)")]
        public string Search { get; set; }

        [Display(Name = AspNetRoleMetadata.LABEL)]
        public string Role { get; set; }

        public bool? Activated { get; set; }

        public bool AddAvailable 
        {
            get { return PageMode == UsersPageMode.Active; }
        }

        public SelectList RolesSelectList { get; set; }
    }
}