using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class RequestPersonalMoodReportModel : AppRequestModelBase
    {
        public string ReportRecipient { get; set; }

    }
}