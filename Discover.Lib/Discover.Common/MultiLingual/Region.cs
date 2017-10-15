using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual
{
    public class Region
    {
        public const string DEFAULT_CODE = "UK";
        public const int DEFAULT_DIFF = 0;
        public const string DEFAULT_FORMAT = "dd MMMM yyyy HH:mm";
        public const string DEFAULT_TIMECODE = "GMT";
       

        private string _code = DEFAULT_CODE;
        private int _dateDiff = DEFAULT_DIFF;
        private string _dateFormat = DEFAULT_FORMAT;
        private string _defaultLanguage = Language.LANG_DefaultCode;
        private string _timeCode = DEFAULT_TIMECODE;

        public string TimeCode
        {
            get { return _timeCode; }
            set { _timeCode = value; }
        }


        public string DefaultLanguage
        {
            get { return _defaultLanguage; }
            set { _defaultLanguage = value; }
        }


        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }


        public int DateDiff
        {
            get { return _dateDiff; }
            set { _dateDiff = value; }
        }


        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string ToLocalDateString(DateTime dateTime)
        {
            DateTime newDateTime = dateTime.AddHours(DateDiff);
            return newDateTime.ToString(DateFormat);
        }

        public string ToLocalDateString(DateTime dateTime, bool includeTimeCode)
        {
            DateTime newDateTime = dateTime.AddHours(DateDiff);
            return string.Format("{0} ({1})", newDateTime.ToString(DateFormat), TimeCode);
        }

        public DateTime ToLocalDateTime(DateTime dateTime)
        {
            DateTime newDateTime = dateTime.AddHours(DateDiff);
            return newDateTime;
        }

        public DateTime ToUKDateTime(DateTime dateTime)
        {
            DateTime ukDateTime = dateTime.AddHours(-DateDiff);
            return ukDateTime;
        }
    }
}
