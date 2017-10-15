using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using MyMood.Domain;


namespace MyMood.Web.Models
{
    public class EditEventModel
    {

        public Guid Id { get; set; }

        [Required]
        [RegularExpression(Discover.ValidationHelper.AlphaNumericNoSpacesRegex, ErrorMessage="Name must be alphanumerice with no spaces.")]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Start Date/Time")]
        public ModelTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date/Time")]
        public ModelTime EndDate { get; set; }

        public ApplicationConfigModel ApplicationConfig { get; set; }

        public IEnumerable<SelectListItem> AvailableTimeZones { get; set; }
        public IEnumerable<SelectListItem> AvailableSyncModes { get; set; }
    }

    public class ApplicationConfigModel
    {
        [Required]
        [Display(Name = "Application Pass Code")]
        public Guid AppPassCode { get; set; }

        [Required]
        [Display(Name = "Reporting Pass Code")]
        public Guid ReportPassCode { get; set; }

        [Display(Name = "Go Live Date ?")]
        public bool HasGoLiveDate { get; set; }

        [Display(Name = "Go Live Date/Time")]
        public ModelTime GoLiveDate { get; set; }

        [Required]
        [Display(Name = "Web Service Uri")]
        public string WebServiceUri { get; set; }

        [Required]
        [Display(Name = "Lan Service Uri")]
        public string LanServiceUri { get; set; }

        [Required]
        [Display(Name = "Data Sync Interval")]
        public int SyncDataInterval { get; set; }

        [Required]
        [Display(Name = "Report Sync Interval")]
        public int SyncReportInterval { get; set; }

        [Required]
        [Display(Name = "Warn Sync Fail Delay (mins)")]
        public int WarnSyncFailureAfterMins { get; set; }

        [Required]
        [Display(Name = "Current Version")]
        public string CurrentVersion { get; set; }

        [Display(Name = "Force Update")]
        public bool ForceUpdate { get; set; }

        [Display(Name = "App Update Uri")]
        public string UpdateAppUri { get; set; }

        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        [Required]
        [Display(Name = "Report Mood Stale after (mins)")]
        public int ReportMoodIsStaleMins { get; set; }

        [Required]
        [Display(Name = "Report Spline Tension")]
        public double ReportSplineTension { get; set; }

        [Required]
        [Display(Name = "Sync Mode")]
        public SyncMode SyncMode { get; set; }
    }

}