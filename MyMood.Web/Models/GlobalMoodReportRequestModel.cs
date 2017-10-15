using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class GlobalMoodReportRequestModel
    {
        public string CategoryName { get; set; }
        public DateTime? ReportStart { get; set; }
        public DateTime? ReportEnd { get; set; }
        public int? MoodIsStaleMins { get; set; }
        public float? Tension { get; set; }
    }
}