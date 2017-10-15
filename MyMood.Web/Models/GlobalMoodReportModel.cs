using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class GlobalMoodReportModel
    {
        public string ReportId { get; set; }
        public IEnumerable<GlobalActivityModel> Activities { get; set; }
        public IEnumerable<GlobalActivityModel> Prompts { get; set; }
        public IEnumerable<MoodSnaphotReportModel> Snapshots { get; set; }
        public IEnumerable<MoodModel> Moods { get; set; }
    }

    //public class GlobalMoodStatsModel
    //{
    //    public string LabelText { get; set; }
    //    public IEnumerable<GlobalMoodDataPointModel> DataPoints { get; set; }
    //}

    //public class GlobalMoodDataPointModel
    //{
    //    public DateTime TimeStamp { get; set; }
    //    public int Count { get; set; }
    //}

    public class GlobalActivityModel
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
    }

    
}