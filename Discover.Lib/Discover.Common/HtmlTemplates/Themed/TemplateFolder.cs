using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.HtmlTemplates.Themed
{
    public class TemplateFolder
    {
        public TemplateFolder()
        {

        }

        public TemplateFolder(string name, string path)
        {
            Name = name;
            Path = path;
        }

        private List<ThemedTemplate> _templates =  new List<ThemedTemplate>();
        private List<ThemedTemplate> _masters = new List<ThemedTemplate>();
        private List<TemplateFolder> _folders = new List<TemplateFolder>();
        private List<string> _themes = new List<string>();

        public List<string> Themes
        {
            get { return _themes; }
            set { _themes = value; }
        }

        public List<ThemedTemplate> Masters
        {
            get { return _masters; }
            set { _masters = value; }
        }

        public List<TemplateFolder> Folders
        {
            get { return _folders; }
            set { _folders = value; }
        }


        public List<ThemedTemplate> Templates
        {
            get { return _templates; }
            set { _templates = value; }
        }

        public string Name { get; set; }
        public string Path { get; set; }

    }
}
