using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.HtmlTemplates
{
    public class HtmlTemplate
    {
        public HtmlTemplate(string name, string html)
        {
            Name = name;
            Html = html;
        }

        public string Name { get; set; }
        public string Html { get; set; }
    }
}
