using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MyMood.Web.Models
{
    public class ForgottenPasswordFormModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}