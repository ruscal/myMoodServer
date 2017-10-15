using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Common
{
    public class ExtendedDateFormatProvider : IFormatProvider, ICustomFormatter
    {
        public static readonly ExtendedDateFormatProvider Instance = new ExtendedDateFormatProvider();

        public object GetFormat(Type service)
        {
            if (service == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (String.IsNullOrEmpty(format))
                return String.Format("{0}", arg);

            if (string.IsNullOrEmpty(format.Trim()))
                return String.Format("{0}", arg);

            if (!(arg is DateTime))
                return String.Format("{0}", arg);

            DateTime dt = (DateTime)arg;
            format = format.Replace("~", GetDayNumberSuffix(dt));
            return dt.ToString(format);
        }

        private static string GetDayNumberSuffix(DateTime date)
        {
            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "\\s\\t";
                case 2:
                case 22:
                    return "\\n\\d";
                case 3:
                case 23:
                    return "\\r\\d";
                default:
                    return "\\t\\h";
            }
        }
    }
}
