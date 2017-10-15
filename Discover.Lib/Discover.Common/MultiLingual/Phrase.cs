using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Common;

namespace Discover.MultiLingual
{
    public class Phrase : AuditedObject
    {
        public Phrase(string defaultLanguageCode)
            :base()
        {
            _defaultLanguageCode = defaultLanguageCode;
        }

        public Phrase(string defaultLanguageCode, int id, string defaultPhrase, string description, string source, string createdBy, DateTime createdOn, string lastEditedBy, DateTime lastEditedOn)
            : base(createdBy, createdOn, lastEditedBy, lastEditedOn)
        {
            _defaultLanguageCode = defaultLanguageCode;
            _id = id;
            _defaultPhrase = defaultPhrase;
            _description = description;
            _source = source;

        }

        public Phrase(string defaultLanguageCode, int id, string defaultPhrase, string description, string source, string createdBy, DateTime createdOn, string lastEditedBy, DateTime lastEditedOn, List<Translation> translations)
            : base(createdBy, createdOn, lastEditedBy, lastEditedOn)
        {
            _defaultLanguageCode = defaultLanguageCode;
            _id = id;
            _defaultPhrase = defaultPhrase;
            _description = description;
            _source = source;
            _translations.AddRange(translations);
        }

        private int _id;
        private string _defaultPhrase;
        private string _description;
        private string _source;
        private List<Translation> _translations = new List<Translation>();
        private string _defaultLanguageCode = "";

        public List<Translation> Translations
        {
            get { return _translations; }
            set { _translations = value; }
        }


        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }


        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        public string DefaultPhrase
        {
            get { return _defaultPhrase; }
            set { _defaultPhrase = value; }
        }


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Translation DefaultTranslation
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultLanguageCode)) return null;
                return (from t in Translations where t.LanguageCode == _defaultLanguageCode select t).SingleOrDefault();
            }
        }

    }
}
