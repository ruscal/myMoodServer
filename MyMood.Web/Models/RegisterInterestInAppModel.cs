using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class RegisterInterestInAppModel : AppRequestModelBase
    {
        public string InterestedParty { get; set; }
    }
}