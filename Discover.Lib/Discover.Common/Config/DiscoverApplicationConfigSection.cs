using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Discover.Common.Config
{
    public class DiscoverApplicationConfigSection : ConfigurationSection
    {
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("uri",
            DefaultValue = "/")]
        public string Uri
        {
            get { return (string)base["uri"]; }
            set { base["uri"] = value; }
        }


        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("rootFileFolder",
            DefaultValue = "/")]
        public string RootFileFolder
        {
            get { return (string)base["rootFileFolder"]; }
            set { base["rootFileFolder"] = value; }
        }

        [ConfigurationProperty("localUri",
            DefaultValue = "/")]
        public string LocalUri
        {
            get { return (string)base["localUri"]; }
            set { base["localUri"] = value; }
        }


        public static DiscoverApplicationConfigSection Config
        {
            get
            {
                return (DiscoverApplicationConfigSection)
                          ConfigurationManager.GetSection
                          ("Discover/Application");
            }
        }
    }
}
