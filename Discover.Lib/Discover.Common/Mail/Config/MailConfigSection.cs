using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Discover.Mail.Config
{
    public class MailConfigSection : ConfigurationSection
    {
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("exceptionEmailTitle", DefaultValue = "An error occurred")]
        public string ExceptionEmailTitle
        {
            get { return (string)base["exceptionEmailTitle"]; }
            set { base["exceptionEmailTitle"] = value; }
        }

        [ConfigurationProperty("accounts")]
        public MailAccountCollection Accounts
        {
            get { return (MailAccountCollection)base["accounts"]; }
        }

        [ConfigurationProperty("enabled", DefaultValue = "true")]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("externalEmailLink", DefaultValue = "/")]
        public string ExternalEmailLink
        {
            get { return (string)base["externalEmailLink"]; }
            set { base["externalEmailLink"] = value; }
        }

        [IntegerValidator(MinValue = 1, MaxValue = 50)]
        [ConfigurationProperty("connectionLimit", DefaultValue = 10)]
        public int ConnectionLimit
        {
            get { return (int)base["connectionLimit"]; }
            set { base["connectionLimit"] = value; }
        }

        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultAccount", DefaultValue = "Default")]
        public string DefaultAccountName
        {
            get { return (string)base["defaultAccount"]; }
            set { base["defaultAccount"] = value; }
        }

        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("administrativeAddress", DefaultValue = "russ@discover-corporation.com")]
        public string AdministrativeAddress
        {
            get { return (string)base["administrativeAddress"]; }
            set { base["administrativeAddress"] = value; }
        }

        [IntegerValidator(MinValue = -1)]
        [ConfigurationProperty("failedMailRetryPeriod", DefaultValue = 10)]
        public int FailedMailRetryPeriod
        {
            get { return (int)base["failedMailRetryPeriod"]; }
            set { base["failedMailRetryPeriod"] = value; }
        }

        [IntegerValidator(MinValue = 0)]
        [ConfigurationProperty("suspendMailAfterAttempts", DefaultValue = 10)]
        public int SuspendMailAfterAttempts
        {
            get { return (int)base["suspendMailAfterAttempts"]; }
            set { base["suspendMailAfterAttempts"] = value; }
        }

        [ConfigurationProperty("storeSuccessfulMails", DefaultValue = "false")]
        public bool StoreSuccessfulMails
        {
            get { return (bool)base["storeSuccessfulMails"]; }
            set { base["storeSuccessfulMails"] = value; }
        }

        public static MailConfigSection Current
        {
            get { return (MailConfigSection)ConfigurationManager.GetSection("Discover/Mail"); }
        }
    }

    public class MailAccountCollection : ConfigurationElementCollection
    {
        public int Count
        {
            get { return base.Count; }
        }

        public MailAccount this[int index]
        {
            get { return base.BaseGet(index) as MailAccount; }
            set 
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public MailAccount this[string name]
        {
            get 
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Name == name) return this[i];
                }
                return null;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MailAccount();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MailAccount)element).Name;
        }

        //IEnumerator<MailAccount> IEnumerable<MailAccount>.GetEnumerator()
        //{
        //    return (IEnumerator<MailAccount>)base.GetEnumerator();
        //}

        public System.Collections.IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }

    public class MailAccount : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("senderEmailAddress", IsRequired = true)]
        public string SenderEmailAddress
        {
            get { return this["senderEmailAddress"] as string; }
        }

        [ConfigurationProperty("senderDisplayName", IsRequired = true)]
        public string SenderDisplayName
        {
            get { return this["senderDisplayName"] as string; }
        }

        [ConfigurationProperty("serverAddress", IsRequired = true)]
        public string ServerAddress
        {
            get { return this["serverAddress"] as string; }
        }

        [ConfigurationProperty("serverPort", IsRequired = true)]
        public int ServerPort
        {
            get { return (int)this["serverPort"]; }
        }

        [ConfigurationProperty("serverUsername", IsRequired = true)]
        public string ServerUsername
        {
            get { return this["serverUsername"] as string; }
        }

        [ConfigurationProperty("serverPassword", IsRequired = true)]
        public string ServerPassword
        {
            get { return this["serverPassword"] as string; }
        }

        [ConfigurationProperty("certificateFilePath", IsRequired = false, DefaultValue = "")]
        public string CertificateFilePath
        {
            get { return this["certificateFilePath"] as string; }
        }

        [ConfigurationProperty("certificatePassword", IsRequired = false, DefaultValue = "")]
        public string CertificatePassword
        {
            get { return this["certificatePassword"] as string; }
        }

        [ConfigurationProperty("replyTo", IsRequired = false, DefaultValue = null)]
        public string ReplyTo
        {
            get { return this["replyTo"] as string; }
        }
    }
}
