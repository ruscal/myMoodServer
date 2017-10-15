using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class MoodPrompt : Entity
    {
         protected MoodPrompt()
            :base()
        {
            
        }

         public MoodPrompt(Event evnt, string name, Activity activity, string notificationText, DateTime activeFrom, DateTime activeTil) :
            this()
        {
            this.Event = evnt;
            this.Name = name;
            this.Activity = activity;
            this.NotificationText = notificationText;
            this.ActiveFrom = activeFrom;
            this.ActiveTil = activeTil;
        }

         public MoodPrompt(Event evnt, string name, string title, DateTime eventTimeStamp,  string notificationText, DateTime activeFrom, DateTime activeTil) :
             this(evnt, name, new Activity(evnt, title, eventTimeStamp, true), notificationText, activeFrom, activeTil)
         {
         }

         public virtual Activity Activity { get; set; }
         public string NotificationText { get; set; }
         public DateTime ActiveFrom { get; set; }
         public DateTime ActiveFromLocal
         {
             get
             {
                 return this.Event.ConvertFromUTC(this.ActiveFrom).Value;
             }
         }
         public DateTime ActiveTil { get; set; }
         public DateTime ActiveTilLocal
         {
             get
             {
                 return this.Event.ConvertFromUTC(this.ActiveTil).Value;
             }
         }
         public string Name { get; set; }

         public virtual Event Event { get; set; }

         public void ConvertAllToUTC()
         {
             this.ActiveFrom = Event.ConvertToUTC(this.ActiveFrom).Value;
             this.ActiveTil = Event.ConvertToUTC(this.ActiveTil).Value;
             this.Activity.TimeStamp = Event.ConvertToUTC(this.Activity.TimeStamp).Value;
         }
    }
}
