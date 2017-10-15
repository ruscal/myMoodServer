using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.HtmlTemplates.Themed
{
    public class ThemedTemplate : Template
    {
        public ThemedTemplate(string name, string path, string defaultHtml, bool themesEnabled, string masterTemplateName, string filePath)
            : base(name, path, defaultHtml, masterTemplateName, filePath)
        {
            ThemesEnabled = themesEnabled;
        }

        private Dictionary<string, Template> _themes = new Dictionary<string,Template>();

        public bool ThemesEnabled { get; set; }
        public Dictionary<string, Template> Themes
        {
            get
            {
                return _themes;
            }
        }

        public string GetHtml(string theme)
        {
            if (!string.IsNullOrEmpty(theme) && ThemesEnabled && Themes.ContainsKey(theme))
            {
                Template template = Themes[theme];
                return template.Html;
            }
            return Html;
        }
    }
}
