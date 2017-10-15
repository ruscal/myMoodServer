using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyMood.Web.Models
{
	public class EditPushNotificationModel
	{
		public Guid Id { get; set;}
        public Guid EventId { get; set; }

        [Required]
        [Display(Name = "Message Text")]
        [MinLength(10, ErrorMessage = "The Messages must be at least 10 characters")]
        public string Message { get; set; }

        [Display(Name = "Send Immediately")]
        public bool SendImmediately { get; set; }

        [Display(Name = "Send Time (L)")]
        public ModelTime SendDate { get; set; }
        public string SendDateUTC { get; set; }

        [Display(Name = "Play Sound")]
        public bool PlaySound { get; set; }

        public bool Sent { get; set; }
	}
}

