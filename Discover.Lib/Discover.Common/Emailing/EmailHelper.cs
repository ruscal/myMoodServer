using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public static class EmailHelper
    {

        public static List<string> GetValidEmailList(List<string> emailAddresses)
        {
            return emailAddresses.Where(IsValidEmail).ToList();
        }

        public static bool IsValidEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) return false;
            const string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                         + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                         + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                         + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                         + @"[a-zA-Z]{2,}))$";

            Regex reStrict = new Regex(patternStrict);

            if (!reStrict.IsMatch(emailAddress))
            {
                string s = "";
            }

            return reStrict.IsMatch(emailAddress);
        }

    }
}
