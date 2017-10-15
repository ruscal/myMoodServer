using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class Event : Entity
    {
        protected Event()
            : base()
        {
            this.ActivitiesCollection = new List<Activity>();
            this.MoodCategoriesCollection = new List<MoodCategory>();
            this.MoodPromptsCollection = new List<MoodPrompt>();
            this.RegisteredInterestsCollection = new List<RegisteredInterest>();
            this.RespondersCollection = new List<Responder>();
            this.PushNotificationsCollection = new List<PushNotification>();
        }

        public Event(string name, string title) :
            this()
        {
            this.Name = name;
            this.Title = title;
            this.ApplicationConfig = new ApplicationConfig();
        }

        public string Name { get; set; }
        public string Title { get; set; }

        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDateLocal { get { return this.ConvertFromUTC(this.StartDate); } }
        public DateTime? EndDateLocal { get { return this.ConvertFromUTC(this.EndDate); } }

        public virtual ApplicationConfig ApplicationConfig { get; set; }

        public IEnumerable<Activity> Activities { get { return ActivitiesCollection; } }
        protected virtual ICollection<Activity> ActivitiesCollection { get; set; }

        public IEnumerable<Activity> IndependentActivities 
        { 
            get 
                {
                    var mpIds = from mp in this.MoodPrompts
                                select mp.Activity.Id;
                    return (from a in Activities
                                where !mpIds.Contains(a.Id)
                                select a).ToList();
                }
            } 
  
        public IEnumerable<RegisteredInterest> RegisteredInterests { get { return RegisteredInterestsCollection; } }
        protected virtual ICollection<RegisteredInterest> RegisteredInterestsCollection { get; set; }

        public IEnumerable<MoodCategory> MoodCategories { get { return MoodCategoriesCollection; } }
        protected virtual ICollection<MoodCategory> MoodCategoriesCollection { get; set; }

        public IEnumerable<MoodPrompt> MoodPrompts { get { return MoodPromptsCollection; } }
        protected virtual ICollection<MoodPrompt> MoodPromptsCollection { get; set; }

        public IEnumerable<Responder> Responders { get { return RespondersCollection; } }
        protected virtual ICollection<Responder> RespondersCollection { get; set; }

        public IEnumerable<PushNotification> PushNotifications { get { return PushNotificationsCollection; } }
        protected virtual ICollection<PushNotification> PushNotificationsCollection { get; set; }

        public PushNotification AddPushNotification(DateTime sendDate, string message, bool playSound)
        {
            var pn = new PushNotification(this, sendDate, message, playSound);
            pn.ConvertAllToUTC();
            this.PushNotificationsCollection.Add(pn);
            return pn;
        }

        public MoodCategory AddCategory(string name)
        {
            var cat = this.MoodCategoriesCollection.FirstOrDefault(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (cat == null)
            {
                cat = new MoodCategory(this, name);
                this.MoodCategoriesCollection.Add(cat);
            }
            return cat;
        }

        public Activity AddActivity(string title, DateTime timeStamp)
        {
            //var act = this.ActivitiesCollection.FirstOrDefault(c => c.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase));
            //if (act == null)
            //{
               var act = new Activity(this, title, timeStamp, false);
                act.ConvertAllToUTC();
                this.ActivitiesCollection.Add(act);
            //}
            return act;
        }

        public RegisteredInterest AddRegisteredInterest(string emailAddress)
        {
            var ri = this.RegisteredInterestsCollection.FirstOrDefault(c => c.EmailAddress.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
            if (ri == null)
            {
                ri = new RegisteredInterest(this, emailAddress);
                this.RegisteredInterestsCollection.Add(ri);
            }
            return ri;
        }

        public MoodPrompt AddMoodPrompt(string name, string title, string notificationText, DateTime timestamp, DateTime activeFrom, DateTime activeTil )
        {
            var mp = this.MoodPromptsCollection.FirstOrDefault(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (mp == null)
            {
                mp = new MoodPrompt(this, name, title, timestamp, notificationText, activeFrom, activeTil);
                mp.ConvertAllToUTC();
                this.MoodPromptsCollection.Add(mp);
            }
            return mp;
        }

        public Responder AddResponder(Guid id, string region, string deviceId)
        {
            var r = this.RespondersCollection.FirstOrDefault(c => c.Id == id);
            if (r == null)
            {
                r = new Responder(this, id, region, deviceId);
                this.RespondersCollection.Add(r);
            }
            else if (!string.IsNullOrWhiteSpace(region) && r.Region != region)
            {
                r.Region = region;
            }
            r.DeviceId = deviceId;
            return r;
        }

        public void ConvertAllToUTC()
        {
            this.StartDate = ConvertToUTC(this.StartDate);
            this.EndDate = ConvertToUTC(this.EndDate);
            this.ApplicationConfig.GoLiveDate = ConvertToUTC(this.ApplicationConfig.GoLiveDate);
        }

        public DateTime? ConvertToUTC(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;
            var convertDate = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(convertDate, TimeZoneInfo.FindSystemTimeZoneById(this.ApplicationConfig.TimeZone));
        }

        public DateTime? ConvertFromUTC(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;

            var convertDate = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(convertDate, TimeZoneInfo.FindSystemTimeZoneById(this.ApplicationConfig.TimeZone));
        }

        public DateTime NowLocal
        {
            get
            {
                return ConvertFromUTC(DateTime.UtcNow).Value;
            }
        }
    }


    public enum SyncMode
    {
        LANthenWAN = 1,
        WANthenLAN = 2,
        LANonly = 4,
        WANonly = 8
    }
}
