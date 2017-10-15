using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MyMood.Web
{
    public class ReportHelper
    {
        public static byte[] PersonalMoodReportBytes(string eventName, Guid responderId, Guid reportPasscode)
        {
            string url = string.Format("{0}Event/{1}/Report/PersonalMoodReport/{2}/{3}",
              Discover.Common.Config.DiscoverApplicationConfigSection.Config.LocalUri,
              eventName,
              responderId.ToString(),
              reportPasscode);

            return UrlToPdfHelper.ToPdfBytes(url, EvoPdf.HtmlToPdf.PdfPageOrientation.Portrait);

        }
    }
}