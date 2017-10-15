using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using MyMood.Domain;

namespace MyMood.Web.Models
{
    public class MoodSnaphotReportModel
    {
        [Display(Name = "Time of Snapshot")]
        public DateTime t { get; set; }

        [Display(Name = "Mood Snapshot Data")]
        public IEnumerable<MoodSnapshotDataModel> d { get; set; }

        [Display(Name = "Total Responses")]
        public int r { get; set; }

        [Display(Name = "Moods")]
        public IEnumerable<MoodModel> m { get; set; }
    }

    public class MoodSnapshotDataModel
    {
        [Display(Name = "Display Index")]
        public int i { get; set; }

        [Display(Name = "Percentage of Total")]
        public decimal p { get; set; }

        [Display(Name = "Count")]
        public int c { get; set; }
    }
}