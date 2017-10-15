using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class PersonalMoodReportViewModel
    {
        public string ResponderId { get; set; }
        public IEnumerable<PersonalMoodResponse> Responses { get; set; }
    }

    public class PersonalMoodResponse
    {
        public string Mood { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
    }
}