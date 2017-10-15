using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class ApplicationConfig : Entity
    {
        public ApplicationConfig()
            : base()
        {
            this.ForceUpdate = false;
            this.WarnSyncFailureAfterMins = 5;
            this.SyncDataInterval = 60;
            this.SyncReportInterval = 60;
            this.SyncMode = Domain.SyncMode.LANthenWAN;
            this.ReportMoodIsStaleMins = 0;
            this.ReportSplineTension = 0.5;
        }



        public Guid AppPassCode { get; set; }
        public Guid ReportPassCode { get; set; }
        public DateTime? GoLiveDate { get; set; }
        public string WebServiceUri { get; set; }
        public string LanServiceUri { get; set; }
        public int SyncDataInterval { get; set; }
        public int SyncReportInterval { get; set; }
        public int WarnSyncFailureAfterMins { get; set; }
        public string CurrentVersion { get; set; }
        public string UpdateAppUri { get; set; }
        public bool ForceUpdate { get; set; }
        public string TimeZone { get; set; }
        public int? ConnectionTimeout { get; set; }

        //report options
        public int ReportMoodIsStaleMins { get; set; }
        public double ReportSplineTension { get; set; }
        public DateTime? DayStartsOn { get; set; }
        public DateTime? DayEndsOn { get; set; }

        public SyncMode SyncMode
        {
            get { return (SyncMode)SyncModeEnumValue; }
            set { SyncModeEnumValue = (int)value; }
        }
        private int SyncModeEnumValue { get; set; }


        public DateTime? GoLiveDateLocal
        {
            get
            {
                if (!this.GoLiveDate.HasValue) return null;

                var convertDate = DateTime.SpecifyKind(this.GoLiveDate.Value, DateTimeKind.Utc);
                return TimeZoneInfo.ConvertTimeFromUtc(convertDate, TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone));
            }
        }
    }
}
