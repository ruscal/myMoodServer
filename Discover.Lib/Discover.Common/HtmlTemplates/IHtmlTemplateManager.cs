using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.HtmlTemplates
{
    public interface IHtmlTemplateManager
    {
        HtmlTemplate GetHtmlTemplate(string name);
        HtmlTemplate GetHtmlTemplate(string name, string theme);
        List<HtmlTemplate> GetHtmlTemplates(string path);
        string GetHtml(string name);
        string GetHtml(string name, string theme);
        string GetHtml(string name, Dictionary<string, string> parameters);
        string GetHtml(string name, string theme, Dictionary<string, string> parameters);
        
        void RefreshTemplates();
    }
}
