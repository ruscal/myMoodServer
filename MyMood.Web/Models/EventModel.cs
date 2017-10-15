using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MyMood.Web.Models
{
    public class EventModel
    {
        public string Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Start Date (L)")]
        public string StartDate { get; set; }
        public string StartDateUTC { get; set; }

        [Display(Name = "End Date (L)")]
        public string EndDate { get; set; }
        public string EndDateUTC { get; set; }

        [Display(Name = "End Date (L)")]
        public string GoLiveDate { get; set; }
        public string GoLiveDateUTC { get; set; }

        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        
             [Display(Name = "Total Apps Registered")]
        public int RegisteredApps { get; set; }

        [Display(Name = "Total Responders")]
        public int ResponderCount { get; set; }

        public int MoodPromptCount { get; set; }
        public int PushNotificationCount { get; set; }
        public int IndependentActivityCount { get; set; }

        public Guid ReportPasscode { get; set; }
        
        public IEnumerable<MoodModel> Moods { get; set; }

        public MoodSnaphotReportModel MoodSnapShot { get; set; }

        public MoodSnaphotReportModel LatestSnapShot { get; set; }

        public IEnumerable<ReportRow> TotalReportLines {get; set;}
    }

}