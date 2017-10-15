using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class MoodSnapshotModel
    {
        public string Orientation { get; set; }
        public string PassCode { get; set; }
        public int MoodIsStaleMins { get; set; }

        public MoodSnaphotReportModel Data { get; set; }
    }
}