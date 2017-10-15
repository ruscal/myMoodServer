using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class EventsViewModel
    {
        public IEnumerable<EventModel> Events { get; set; }
    }
}