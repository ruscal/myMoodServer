using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
	public class MoodPromptModel
	{
		public string Id { get; set;}

        public ActivityModel Activity { get; set; }

        public string NotificationText { get; set; }
		public DateTime ActiveFrom { get; set; }
        public string ActiveFromUTC { get; set; }

		public DateTime ActiveTil { get; set; }
        public string ActiveTillUTC { get; set; }

        public string Name { get; set; }
        public bool CanDelete { get; set; }
	}
}

