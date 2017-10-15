using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMood.Web.Models
{
    public class ManageUsersViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
        public IEnumerable<SelectListItem> AvailableSecurityRoles { get; set; }
    }

    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? LastLoggedIn { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }
    }
}