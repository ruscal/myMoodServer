using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual
{
    public class MLangHelper
    {
        public const string MLangHtmlWrapperFormat = "<mlang mlid={0}>{1}</mlang>";
        public const string MLangNonHtmlWrapperFormat = "@mlang[{0}][{1}]";

        public static string ToHtmlPhraseReference(int prId, string text)
        {
            return string.Format(MLangHtmlWrapperFormat, (prId == 0) ? "@" : prId.ToString(), text);
        }

        public static string ToNonHtmlPhraseReference(int prId, string text)
        {
            return string.Format(MLangNonHtmlWrapperFormat, (prId == 0) ? "@" : prId.ToString(), text);
        }
    }
}
