using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Logging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;

namespace Discover.HtmlTemplates.Themed
{
    public class ThemedHtmlTemplateManager : IHtmlTemplateManager
    {
        public ThemedHtmlTemplateManager(ILogger logger, string templateFolder)
        {
            _logger = logger;
            _templateFolderPath = templateFolder;
            Initialize();
        }

        private string _templateFolderPath = "";
        private string _configFilePath = "";
        private DirectoryInfo _templateFolderDir;
        private ILogger _logger;
        private TemplateFolder _root = new TemplateFolder();
        private DateTime _configLastModifiedOn;
        private XmlNamespaceManager _namespaceManager;
        private const string _namespace = "http://www.discover-corporation.com";

        public TemplateFolder Root
        {
            get { return _root; }
        }


        public HtmlTemplate GetHtmlTemplate(string name)
        {
            return GetHtmlTemplate(name, "");
        }

        public HtmlTemplate GetHtmlTemplate(string name, string theme)
        {
            CheckConfigForRefresh();
            return new HtmlTemplate(name, GetTemplateHtml(name, theme));
        }

        public List<HtmlTemplate> GetHtmlTemplates(string path)
        {
            CheckConfigForRefresh();
            TemplateFolder folder = FindFolder(path, Root);
            return (from t in folder.Templates select new HtmlTemplate(t.Name, GetTemplateHtml(t.Path, ""))).ToList();
        }

        public string GetHtml(string name)
        {
            return GetHtml(name, "", null);
        }

        public string GetHtml(string name, string theme)
        {
            string html = GetTemplateHtml(name, theme);
            return ReplaceParameters(html, null);
        }

        public string GetHtml(string name, Dictionary<string, string> parameters)
        {
            return GetHtml(name, "", parameters);
        }

        public string GetHtml(string name, string theme, Dictionary<string, string> parameters)
        {
            string html = GetTemplateHtml(name, theme);
            return ReplaceParameters(html, parameters);
        }

        public void RefreshTemplates()
        {
            // refresh all templates
            _root = new TemplateFolder();
            LoadTemplates();
        }



        private string GetTemplateHtml(string path, string theme)
        {
            if(path.EndsWith("/")) throw new ArgumentException(string.Format("Invalid path - ends with /.  path=[{0}]", path));
            TemplateFolder folder = FindFolder(path, Root);
            string templateName = path;
            if (templateName.IndexOf("/") > 0)
            {
                templateName = templateName.Substring(templateName.LastIndexOf("/") + 1);
            }
            ThemedTemplate template = (from t in folder.Templates where t.Name.ToLower() == templateName.ToLower() select t).SingleOrDefault();
            if (template == null) throw new ArgumentException(string.Format("Could not find template. name=[{0}] path=[{1}]", templateName, path));
            CheckTemplateForRefresh(template);
            return GetTemplateHtml(folder, template, theme);
        }

        private string GetTemplateHtml(TemplateFolder folder, ThemedTemplate template, string theme)
        {
            string body = "{content}";
            if (template.MasterTemplateName != "")
            {
                ThemedTemplate master = (from m in folder.Masters where m.Name.ToLower() == template.MasterTemplateName.ToLower() select m).SingleOrDefault();
                if (master != null)
                {
                    body = GetTemplateHtml(folder, master, theme);
                }
            }
            return body.Replace("{content}", template.GetHtml(theme));
        }

        private TemplateFolder FindFolder(string path, TemplateFolder parent)
        {
            while(path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            int nextIndex = path.IndexOf('/');
            if (nextIndex > 0)
            {
                string currentName = path.Substring(0, nextIndex).ToLower();
                TemplateFolder folder = (from f in parent.Folders where f.Name.ToLower() == currentName select f).SingleOrDefault();
                if(folder == null) throw new ArgumentException(string.Format("Could not find Template Folder folder=[{0}]", currentName));
                return FindFolder(path.Substring(nextIndex + 1), folder);
            }
            else
            {
                return parent;
            }
        }

        private void CheckConfigForRefresh()
        {
            FileInfo fi = new FileInfo(_configFilePath);
            if (fi.LastWriteTime > _configLastModifiedOn)
            {
                //config has changed, refresh everything
                RefreshTemplates();
            }
        }

        private void CheckTemplateForRefresh(ThemedTemplate template)
        {
            FileInfo fi = new FileInfo(template.Path);
            if (fi.LastWriteTime > template.FileLastModified)
            {
                template.Html = LoadHtml(template.Path);
                template.FileLastModified = fi.LastWriteTime;
            }
            foreach (string theme in template.Themes.Keys)
            {
                CheckThemeForRefresh(template.Themes[theme]);
            }
        }

        private void CheckThemeForRefresh(Template template)
        {
            FileInfo fi = new FileInfo(template.Path);
            if (fi.LastWriteTime > template.FileLastModified)
            {
                template.Html = LoadHtml(template.Path);
                template.FileLastModified = fi.LastWriteTime;
            }
        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(_templateFolderPath)) throw new ArgumentException("Template folder argument is null or empty.");
            _templateFolderDir = new DirectoryInfo(_templateFolderPath);
            if (!_templateFolderDir.Exists) throw new ArgumentException("Template folder does not exist.");
            _configFilePath = Path.Combine(_templateFolderDir.FullName, "HtmlTemplates.Config");
            FileInfo fi = new FileInfo(_configFilePath);
            if (!fi.Exists) throw new FileNotFoundException("HtmlTemplates.config file not found.");
            _configLastModifiedOn = fi.LastWriteTime;
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            try
            {

                XmlReader reader = XmlReader.Create(_configFilePath);
                XElement doc = XElement.Load(reader);
                XmlNameTable nameTable = reader.NameTable;
                _namespaceManager = new XmlNamespaceManager(nameTable);
                _namespaceManager.AddNamespace("dc", _namespace);

                XElement rootFolder = doc.XPathSelectElement("//dc:templateFolder[@name='root']", _namespaceManager);
                if (rootFolder == null) throw new XmlException("No root template folder specified.");

                _root = LoadTemplateFolder(rootFolder,  "");
            }
            catch (Exception ex)
            {
                _logger.Error(this.GetType(), ex, "Error loading templates for ThemedHtmlTemplateManager. configpath=[{0}]", _configFilePath);
                throw new Exception("Could not load templates for ThemedHtmlTemplateManager.", ex);
            }
        }

        private TemplateFolder LoadTemplateFolder(XElement el, string path)
        {
            TemplateFolder tf = new TemplateFolder(el.Attribute("name").Value, path);
            XNamespace ns = el.GetDefaultNamespace();
            List<XElement> subFolders = el.Elements(ns + "templateFolder").ToList();
            foreach (XElement f in subFolders)
            {
                string name = f.Attribute("name").Value;
                tf.Folders.Add(LoadTemplateFolder(f, string.Format("{0}/{1}", path, name)));
            }
            //load themes
            XElement elThemes = el.Element(ns + "themes");
            if (elThemes != null)
            {
                List<XElement> themes = elThemes.Elements(ns + "theme").ToList();
                foreach (XElement th in themes)
                {
                    tf.Themes.Add(th.Attribute("name").Value);
                }
            }
            List<XElement> masters = el.Elements(ns + "master").ToList();
            foreach (XElement m in masters)
            {
                ThemedTemplate template = LoadTemplate(tf.Path, m, tf.Themes);
                if (template != null) tf.Masters.Add(template);
            }

            List<XElement> templates = el.Elements(ns + "template").ToList();
            foreach (XElement t in templates)
            {
                ThemedTemplate template = LoadTemplate(tf.Path, t, tf.Themes);
                if (template != null) tf.Templates.Add(template);
            }
            return tf;
        }

        private ThemedTemplate LoadTemplate(string folderPath, XElement el, List<string> themes)
        {
            ThemedTemplate template = new ThemedTemplate(el.Attribute("name").Value,
                el.Attribute("path").Value,
                "",
                el.Attribute("themesEnabled") == null ? true : bool.Parse(el.Attribute("themesEnabled").Value),
                el.Attribute("master") == null ? "" : el.Attribute("master").Value,
                el.Attribute("path").Value);

            string filename = Path.Combine(_templateFolderDir.FullName, template.Path);
            //Path.Combine(filename, template.Path);
            FileInfo tempFile = new FileInfo(filename);
            if (tempFile.Exists)
            {
                template.FileLastModified = tempFile.LastWriteTime;
                template.FilePath = tempFile.FullName;
                //template.Path = folderPath + template.Path;
                template.Html = LoadHtml(template.FilePath);

                //load themes
                foreach (string theme in themes)
                {
                    string themeDirPath = Path.Combine(tempFile.Directory.FullName, "_themes\\" + theme);
                    DirectoryInfo themeDir = new DirectoryInfo(themeDirPath);
                    if (themeDir.Exists)
                    {
                        FileInfo themeFile = new FileInfo(Path.Combine(themeDir.FullName, template.Name + ".htm"));
                        if (themeFile.Exists)
                        {
                            Template themedTemplate = new Template(template.Name, template.Path, LoadHtml(themeFile.FullName), template.MasterTemplateName, filename);
                            themedTemplate.FileLastModified = themeFile.LastWriteTime;
                            themedTemplate.FilePath = themeFile.FullName;
                            template.Themes.Add(theme, themedTemplate);
                        }
                    }
                }
                return template;
            }
            return null;
        }

        private string LoadHtml(string path)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(path);
                string html = "";
                string line = "";
                do
                {
                    html += line;
                    line = sr.ReadLine();
                } while (!(line == null));
                return html;
            }
            catch (Exception ex)
            {
                _logger.Error(this.GetType(), ex, "Error loading html for ThemedHtmlTemplateManager. path=[{0}]", path);
            }
            finally
            {
                if (sr != null) sr.Close();
                sr = null;
            }
            return "";
        }

        string ReplaceParameters(string source, Dictionary<string, string> parameters)
        {
            return (string.IsNullOrEmpty(source) || parameters == null || !parameters.Any()) ?
                source :
                ParameterReplacementTokenRegex.Replace(source, new MatchEvaluator(m =>
                    {
                        var token = m.Groups[1].Value;
                        return parameters.ContainsKey(token) ? parameters[token] : m.Value;
                    }));
        }

        private static Regex ParameterReplacementTokenRegex = new Regex("{([^{}]+)}", RegexOptions.Compiled);
    }
}
