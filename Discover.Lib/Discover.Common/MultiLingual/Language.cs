using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual
{
    public class Language
    {
        public Language()
        {

        }

        public Language(string code, string name, bool enabled, int displayIndex)
        {
            _code = code;
            _name = name;
            _enabled = enabled;
            _displayIndex = displayIndex;
        }

        public Language(string code, string name, bool enabled, int displayIndex, List<string> supportedCultures)
        {
            _code = code;
            _name = name;
            _enabled = enabled;
            _displayIndex = displayIndex;
            _supportedCultures.AddRange(supportedCultures);
        }

        public const string LANG_DefaultCode = "en";
        public const string LANG_DefaultName = "English";

        private string _code;
        private string _name;
        private bool _enabled = true;
        private int _displayIndex = 0;
        private List<string> _supportedCultures = new List<string>();

        public List<string> SupportedCultures
        {
            get { return _supportedCultures; }
            set { _supportedCultures = value; }
        }


        public int DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = value; }
        }


        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }


        
    }
}
