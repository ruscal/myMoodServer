using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual.Models
{
    public class TranslationsModel
    {
        private List<Translation> _translations = new List<Translation>();
        private List<Language> _languages = new List<Language>();

        public List<Language> Languages
        {
            get { return _languages; }
            set { _languages = value; }
        }
        
        public string Language { get; set; }
        
        public List<Translation> Translations
        {
            get { return _translations; }
            set { _translations = value; }
        }

    }
}
