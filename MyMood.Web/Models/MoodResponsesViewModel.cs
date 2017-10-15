using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class MoodResponsesViewModel
    {
        public IEnumerable<MoodResponseViewModel> Responses { get; set; }
    }

    public class MoodResponseViewModel
    {
        public Guid ResponderId { get; set; }
        public string Mood { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PromptText { get; set; }
    }
}