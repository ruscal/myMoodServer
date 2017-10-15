using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyMood.Web.Models
{
    

    public class ViewEventLogModel
    {

        public ViewEventLogModel()
        {
            EarliestDate = new ModelTime();
            LatestDate = new ModelTime();
        }

        [Display(Name = "Earliest Date")]
        public ModelTime EarliestDate { get; set; }

        [Display(Name = "Latest Date")]
        public ModelTime LatestDate { get; set; }

        [Display(Name = "Error Text")]
        public string SearchText { get; set; }

        [Display(Name = "Error Level")]
        public string ErrorLevel { get; set; }


        public IEnumerable<string> ErrorLevels { get { return new List<string> {"All", "Info", "Error"}; } }
        public IEnumerable<EventLogEntryModel> EventLogEntries { get; set; }
        
    }

    public class EventLogEntryModel
    {
        public long Id { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string TimeStamp { get; set; }
    }
}