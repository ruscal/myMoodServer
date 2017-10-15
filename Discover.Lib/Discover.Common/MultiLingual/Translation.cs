using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Common;

namespace Discover.MultiLingual
{
    public class Translation : AuditedObject
    {
        public Translation(int phraseId, string languageCode)
            :base()
        {
            _phraseId = phraseId;
            _languageCode = languageCode;
        }

        public Translation(int id, int phraseId, string languageCode, string text, TranslationStatus status, string createdBy, DateTime createdOn, string lastEditedBy, DateTime lastEditedOn)
            : base(createdBy, createdOn, lastEditedBy, lastEditedOn)
        {
            _id = id;
            _phraseId = phraseId;
            _languageCode = languageCode;
            _text = text;
            _status = status;
        }

        private int _id;
        private int _phraseId;
        private string _languageCode = "";
        private string _text;
        private TranslationStatus _status = TranslationStatus.AwaitingTranslation;

        public TranslationStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }


        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


        public string LanguageCode
        {
            get { return _languageCode; }
            set { _languageCode = value; }
        }
        

        public int PhraseId
        {
            get { return _phraseId; }
            set { _phraseId = value; }
        }


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
