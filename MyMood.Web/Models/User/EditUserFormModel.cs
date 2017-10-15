using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Discover;

namespace MyMood.Web.Models
{
    public class EditUserFormModel
    {
        public EditUserFormModel()
        {
            AssignedRoles = new string[0];
        }

        [Required]
        [HiddenInput(DisplayValue=false)]
        public Guid UserId { get; set; }

        [Required]
        [RegularExpression(@"^\S+$", ErrorMessage = "User Name cannot contain white-space characters")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Display(Name = "Email Address")]
        [RegularExpression(ValidationHelper.EmailAddressRegex, ErrorMessage = "This does not appear to be an email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Enable Access")]
        public bool Enabled { get; set; }

        [Display(Name = "Security Roles")]
        public string[] AssignedRoles { get; set; }

        public IEnumerable<SelectListItem> AvailableRoles { get; set; }
    }
}