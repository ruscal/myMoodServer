using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.TaskRunner.Configuration
{
    public static class AppConfiguration
    {
        public static string ApplicationPassCode
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MyMood.HtmlTemplatesFolder"];
            }
        }
    }
}
