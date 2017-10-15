using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Discover;

namespace MyMood.Web.Models
{
    public class ResetPasswordFormModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        [Required]
        [Display(Name = "Send To Email Address")]
        [RegularExpression(ValidationHelper.EmailAddressRegex, ErrorMessage = "This does not appear to be an email address")]
        public string SendToEmailAddress { get; set; }
    }
}