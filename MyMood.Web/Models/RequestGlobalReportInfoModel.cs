using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class RequestGlobalReportInfoModel
    {
        public DateTime? LastReportRequested { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}