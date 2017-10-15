using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.HtmlTemplates.Themed
{
    public class Template
    {
        public Template(string name, string path, string html, string masterTemplateName, string filePath)
        {
            Name = name;
            Path = path;
            Html = html;
            MasterTemplateName = masterTemplateName;
            FilePath = filePath;
        }

        public string Name { get; set; }
        public string Html { get; set; }
        public string Path { get; set; }
        public string FilePath { get; set; }
        public DateTime FileLastModified { get; set; }
        public string MasterTemplateName { get; set; }
    }
}
