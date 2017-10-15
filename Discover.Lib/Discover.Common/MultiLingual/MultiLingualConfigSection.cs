using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Discover.MultiLingual
{
    public class MultiLingualConfigSection : ConfigurationSection
    {

        [ConfigurationProperty("enabled",
            DefaultValue = "false")]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }



        [ConfigurationProperty("defaultLanguage",
            DefaultValue = "en")]
        public string DefaultLanguage
        {
            get { return (string)base["defaultLanguage"]; }
            set { base["defaultLanguage"] = value; }
        }

        public static MultiLingualConfigSection Current
        {
            get
            {
                return (MultiLingualConfigSection)
                            ConfigurationManager.GetSection
                            ("Discover/MultiLingual");
            }
        }
    }
}
