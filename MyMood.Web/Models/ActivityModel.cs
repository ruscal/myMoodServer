using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class ActivityModel
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TimeStampUTC { get; set; }
    }
}