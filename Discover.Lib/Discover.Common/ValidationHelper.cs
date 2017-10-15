using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Discover
{
    public static class ValidationHelper
    {
        public const string EmailAddressRegex = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
        public const string AlphaNumericNoSpacesRegex = @"^[a-zA-Z0-9_]*$";

        private static Regex _EmailAddressRegex = new Regex(EmailAddressRegex, RegexOptions.Compiled);
        private static Regex _AlphaNumericNoSpacesRegex = new Regex(AlphaNumericNoSpacesRegex, RegexOptions.Compiled);

        public static bool IsValidEmailAddress(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? false : _EmailAddressRegex.IsMatch(value);
        }
    }
}
