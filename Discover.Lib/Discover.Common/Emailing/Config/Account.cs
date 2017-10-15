using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Discover.Emailing.Config
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class Account : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("emailAddress", IsRequired = true)]
        public string EmailAddress
        {
            get
            {
                return this["emailAddress"] as string;
            }
        }

        [ConfigurationProperty("emailName", IsRequired = true)]
        public string EmailFromName
        {
            get
            {
                return this["emailName"] as string;
            }
        }

        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get
            {
                return this["server"] as string;
            }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
        }

        [ConfigurationProperty("serverUsername", IsRequired = true)]
        public string ServerUsername
        {
            get
            {
                return this["serverUsername"] as string;
            }
        }

        [ConfigurationProperty("serverPassword", IsRequired = true)]
        public string ServerPassword
        {
            get
            {
                return this["serverPassword"] as string;
            }
        }

        [ConfigurationProperty("certificateFilePath", IsRequired = false, DefaultValue = "")]
        public string CertificateFilePath
        {
            get
            {
                return this["certificateFilePath"] as string;
            }
        }

        [ConfigurationProperty("certificatePassword", IsRequired = false, DefaultValue = "")]
        public string CertificatePassword
        {
            get
            {
                return this["certificatePassword"] as string;
            }
        }


    }
}
