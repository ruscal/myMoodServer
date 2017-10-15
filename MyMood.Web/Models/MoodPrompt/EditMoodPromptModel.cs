using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyMood.Web.Models
{
	public class EditMoodPromptModel
	{
		public Guid Id { get; set;}
        public Guid EventId { get; set; }

        public EditActivityModel Activity { get; set; }

        [Required]
        [Display(Name = "Notication Text")]
        public string NotificationText { get; set; }

        [Display(Name = "Active From (L)")]
        public ModelTime ActiveFrom { get; set; }

        [Display(Name = "Active Till (L)")]
        public ModelTime ActiveTil { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
	}
}

