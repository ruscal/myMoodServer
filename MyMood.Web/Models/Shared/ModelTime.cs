using System;

namespace MyMood.Web.Models
{
    public class ModelTime
    {
        private int? _hour;
        private int? _minute;

        public DateTime? Date { get; set; }
        public int Hour
        {
            get
            {
                return _hour ?? this.Date.Value.Hour;
            }
            set
            {
                _hour = value;
            }
        }

        public int Minute
        {
            get
            {
                return _minute ?? this.Date.Value.Minute;
            }
            set
            {
                _minute = value;
            }
        }

        public DateTime FullDate
        {
            get
            {
                return this.Date.Value.Date.AddHours(this.Hour).AddMinutes(this.Minute);
            }
        }

        public DateTime? ToUtc(string timeZone) {

            return ToUtc(TimeZoneInfo.FindSystemTimeZoneById(timeZone));
        }

        public DateTime? ToUtc(TimeZoneInfo timeZoneInfo)
        {

            if (!this.Date.HasValue)
                throw new ArgumentNullException("Date has no value to convert");

            var convertDate = DateTime.SpecifyKind(this.FullDate, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(convertDate, timeZoneInfo);
        }

        public DateTime? FromUtc(string timeZone)
        {
            return FromUtc(TimeZoneInfo.FindSystemTimeZoneById(timeZone));
        }

        public DateTime? FromUtc(TimeZoneInfo timeZoneInfo)
        {
            if (!this.Date.HasValue)
                throw new ArgumentNullException("Date has no value to convert");

            var convertDate = DateTime.SpecifyKind(this.FullDate, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(convertDate, timeZoneInfo);
        }

    }
}