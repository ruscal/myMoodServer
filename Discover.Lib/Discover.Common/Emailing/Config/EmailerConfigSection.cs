using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Discover.Emailing.Config
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class EmailerConfigSection : ConfigurationSection
    {
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("exceptionEmailTitle",
            DefaultValue = "An error occurred")]
        public string ExceptionEmailTitle
        {
            get { return (string)base["exceptionEmailTitle"]; }
            set { base["exceptionEmailTitle"] = value; }
        }

        [ConfigurationProperty("accounts")]
        public AccountCollection Accounts
        {
            get { return (AccountCollection)base["accounts"]; }
        }

        [ConfigurationProperty("enabled",
            DefaultValue = "true")]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("externalEmailLink",
            DefaultValue = "/")]
        public string ExternalEmailLink
        {
            get { return (string)base["externalEmailLink"]; }
            set { base["externalEmailLink"] = value; }
        }

        [IntegerValidator(MinValue = 1, MaxValue = 50)]
        [ConfigurationProperty("connectionLimit",
            DefaultValue = 10)]
        public int ConnectionLimit
        {
            get { return (int)base["connectionLimit"]; }
            set { base["connectionLimit"] = value; }
        }

        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultAccount",
            DefaultValue = "Default")]
        public string DefaultAccountName
        {
            get { return (string)base["defaultAccount"]; }
            set { base["defaultAccount"] = value; }
        }


        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("administrativeAddress",
           DefaultValue = "russ@discover-corporation.com")]
        public string AdministrativeAddress
        {
            get { return (string)base["administrativeAddress"]; }
            set { base["administrativeAddress"] = value; }
        }

        [IntegerValidator(MinValue = -1)]
        [ConfigurationProperty("failedMailRetryPeriod",
            DefaultValue = 10)]
        public int FailedMailRetryPeriod
        {
            get { return (int)base["failedMailRetryPeriod"]; }
            set { base["failedMailRetryPeriod"] = value; }
        }

        [IntegerValidator(MinValue = 0)]
        [ConfigurationProperty("suspendMailAfterAttempts",
            DefaultValue = 10)]
        public int SuspendMailAfterAttempts
        {
            get { return (int)base["suspendMailAfterAttempts"]; }
            set { base["suspendMailAfterAttempts"] = value; }
        }

        [ConfigurationProperty("storeSuccessfulMails",
            DefaultValue = "false")]
        public bool StoreSuccessfulMails
        {
            get { return (bool)base["storeSuccessfulMails"]; }
            set { base["storeSuccessfulMails"] = value; }
        }

        public static EmailerConfigSection Current
        {
            get
            {
                return (EmailerConfigSection)
                          ConfigurationManager.GetSection
                          ("Discover/Emailer");
            }
        }
    }
}
