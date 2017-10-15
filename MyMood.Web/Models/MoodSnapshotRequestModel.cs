using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class MoodSnapshotRequestModel
    {
        public string CategoryName { get; set; }
        public DateTime? TimeOfSnapshot { get; set; }
        public int? MoodIsStaleMins { get; set; }
    }
}