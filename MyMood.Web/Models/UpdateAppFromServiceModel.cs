using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class UpdateAppFromServiceModel
    {


        public bool HasPromptUpdates
        {
            get;
            set;
        }

        public bool SyncSuccess { get; set; }
        public DateTime SyncTimestamp { get; set; }

        public bool ResError { get; set; }

        public DateTime UpdatedOn
        {
            get;
            set;
        }

        public ApplicationStateModel Application
        {
            get;
            set;
        }

        public IEnumerable<MoodCategoryModel> Categories
        {
            get;
            set;
        }

        public IEnumerable<MoodPromptModel> Prompts
        {
            get;
            set;
        }
    }
}