using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyMood.Web.Models
{
    public class EditActivityModel
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }

        [Required]
        [Display(Name = "Title")]
        //[MinLength(10, ErrorMessage="The Title must be at least 10 characters")]
        public string Title { get; set; }

        [Display(Name = "Time Stamp (L)")]
        public ModelTime TimeStamp { get; set; }
        public string TimeStampUTC { get; set; }
    }
}