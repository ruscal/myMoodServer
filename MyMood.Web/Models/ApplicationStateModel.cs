using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class ApplicationStateModel
    {
        public string WANWebServiceUri
        {
            get;
            set;
        }

        public string LANWebServiceUri
        {
            get;
            set;
        }


        public int? SyncDataInterval
        {
            get;
            set;
        }

        public int? SyncReportInterval
        {
            get;
            set;
        }

        public DateTime? GoLiveDate { get; set; }
        public int WarnSyncFailureAfterMins { get; set; }
        public string CurrentVersion { get; set; }
        public bool ForceUpdate { get; set; }
        public string SyncMode { get; set; }
        public string EventTimeZone { get; set; }
        public int EventTimeOffset { get; set; }
        public string UpdateAppUri { get; set; }
        public int? ConnectionTimeout { get; set; }
    }
}

