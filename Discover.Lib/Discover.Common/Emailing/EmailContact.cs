using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class EmailContact
    {
        public EmailContact()
        {

        }

        public EmailContact(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public EmailContact(string emailAddress, string displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }

        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return EmailAddress;
        }
    }
}
