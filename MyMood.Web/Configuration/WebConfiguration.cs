using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Configuration
{
    public static class WebConfiguration
    {
        public static string ApplicationPassCode
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MyMood.PassCode"];
            }
        }

        public static string ServerName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MyMood.ServerName"];
            }
        }

        public static int ServerSyncIntervalMinutes
        {
            get
            {
                int interval;
                return int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MyMood.ServerSyncIntervalMinutes"], out interval) ? interval : 15;
            }
        }

        public static string RegisteredInterestRecipient
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MyMood.RegisteredInterestRecipient"];
            }
        }

        public static string WebDateTimeFormat
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WebDateTimeFormat"];
            }
        }

        public static string WebDateFormat
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WebDateFormat"];
            }
        }

        public static string APNSCertificatePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MyMood.APNSCertificate"];
            }
        }

        public static bool DisableAppSyncs
        {
            get
            {
                var val = System.Configuration.ConfigurationManager.AppSettings["MyMood.DisableAppSyncs"];
                return val == null ? false : bool.Parse(val);
            }
        }
    }
}