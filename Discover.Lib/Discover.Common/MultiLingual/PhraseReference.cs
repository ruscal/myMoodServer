using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual
{
    public class PhraseReference
    {
        public PhraseReference(int id, Phrase phrase)
        {
            _id = id;
            _phrase = phrase;
        }

        private int _id;
        private Phrase _phrase;

        public Phrase Phrase
        {
            get { return _phrase; }
            set { _phrase = value; }
        }


        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string HtmlReferenceString
        {
            get
            {
                return MLangHelper.ToHtmlPhraseReference(Id, Phrase.DefaultPhrase);
            }
        }

        public string NonHtmlReferenceString
        {
            get
            {
                return MLangHelper.ToNonHtmlPhraseReference(Id, Phrase.DefaultPhrase);
            }
        }

    }
}
