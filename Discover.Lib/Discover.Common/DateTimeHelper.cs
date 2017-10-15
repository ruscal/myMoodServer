using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Common
{
    public static class DateTimeHelper
    {
        public static DateTime EndOfDay(this DateTime dt)
        {
            return dt.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime AddBusinessDays(this DateTime dt, int days)
        {
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    dt = dt.AddDays(sign);
                }
                while (dt.DayOfWeek == DayOfWeek.Saturday ||
                    dt.DayOfWeek == DayOfWeek.Sunday);
            }
            return dt;

        }

        public static DateTime ToLocalTime(this DateTime dt, string timeZoneId)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId)) return dt.ToLocalTime();
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            if (tz == null) throw new ArgumentException("Invalid timezone - " + timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dt.ToUniversalTime(), tz);
        }
    }


}
