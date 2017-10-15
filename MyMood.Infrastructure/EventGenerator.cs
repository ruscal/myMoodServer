using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.DomainModel;
using MyMood.Domain;

namespace MyMood
{
    public class EventGenerator
    {
        IDomainDataContext db;

        public EventGenerator(IDomainDataContext db)
        {
            this.db = db;
        }

        public Event GenerateNewEvent(string name, string title)
        {
            Event evnt = this.db.Get<Event>().FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (evnt == null)
            {
                evnt = new Event(name, title);
                evnt.ApplicationConfig = new ApplicationConfig();
                evnt.ApplicationConfig.AppPassCode = System.Guid.NewGuid();
                evnt.ApplicationConfig.CurrentVersion = "1.0";
                evnt.ApplicationConfig.LanServiceUri = "http://192.168.100.4:8080";
                evnt.ApplicationConfig.ReportPassCode = System.Guid.NewGuid();
                evnt.ApplicationConfig.WebServiceUri = "https://www.learning-performance.com/MyMood/";
                evnt.ApplicationConfig.UpdateAppUri = "https://www.learning-performance.com/MyMood/";

                evnt.ApplicationConfig.DayStartsOn = new DateTime(2013, 1, 1, 6, 0, 0);
                evnt.ApplicationConfig.DayEndsOn = new DateTime(2013, 1, 1, 21, 0, 0);
                this.db.Add(evnt);

                var category = evnt.AddCategory("Default");

                category.AddMood("Passionate", MoodType.Positive, "#f2932f", 1);
                category.AddMood("Excited", MoodType.Positive, "#f9a924", 2);
                category.AddMood("Proud", MoodType.Positive, "#fdbc18", 3);
                category.AddMood("Engaged", MoodType.Positive, "#ffcc03", 4);
                category.AddMood("Optimistic", MoodType.Positive, "#ffda00", 5);

                category.AddMood("Frustrated", MoodType.Negative, "#eb641c", 6);
                category.AddMood("Worried", MoodType.Negative, "#c85424", 7);
                category.AddMood("Bored", MoodType.Negative, "#a3442a", 8);
                category.AddMood("Deflated", MoodType.Negative, "#8e4c31", 9);
                category.AddMood("Disengaged", MoodType.Negative, "#775536", 10);

                this.db.SaveChanges();
            }

            return evnt;

        }
    }
}
