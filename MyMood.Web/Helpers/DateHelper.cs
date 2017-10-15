using System;
using MyMood.Web.Configuration;

namespace MyMood.Web
{
    public static class DateHelpers
    {

        public static string ToWebDateTime(this DateTime dateTime)
        {
            return dateTime.ToString(WebConfiguration.WebDateTimeFormat);
        }

        public static string ToWebDateTime(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToWebDateTime() : " - " ;
        }

        public static string ToWebDate(this DateTime dateTime)
        {
            return dateTime.ToString(WebConfiguration.WebDateFormat);
        }

        public static string ToWebDate(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToWebDate() : " - ";
        }

        public static string ToWebDateTimeUTC(this DateTime dateTime)
        {
            return string.Format("{0} z", dateTime.ToWebDateTime());
        }

        public static string ToWebDateTimeUTC(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToWebDateTimeUTC() : " - ";
        }
    

    }
}