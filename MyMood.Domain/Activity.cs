using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class Activity  : Entity
    {
         protected Activity()
            :base()
        {

        }

         public Activity(Event evnt, string title, DateTime eventTimeStamp, bool hasPrompt) :
            this()
        {
            this.Event = evnt;
            this.Title = title;
            this.TimeStamp = eventTimeStamp;
            this.HasPrompt = hasPrompt;
        }


         public string Title { get; set; }
         public DateTime TimeStamp { get; set; }
         public DateTime TimeStampLocal
         {
             get
             {
                 return this.Event.ConvertFromUTC(this.TimeStamp).Value;
             }
         }
         public bool HasPrompt { get; set; }

         public virtual Event Event { get; set; }

         public void ConvertAllToUTC()
         {
             this.TimeStamp = this.Event.ConvertToUTC(this.TimeStamp).Value;
         }

    }
}
