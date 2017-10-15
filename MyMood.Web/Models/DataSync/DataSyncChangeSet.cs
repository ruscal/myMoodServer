using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models.DataSync
{
    public class DataSyncChangeSet
    {
        public IEnumerable<MoodPromptInfo> MoodPrompts { get; set; }
        public IEnumerable<ActivityInfo> Activities { get; set; }
        public IEnumerable<ResponderInfo> Responders { get; set; }
        public IEnumerable<MoodResponseInfo> MoodResponses { get; set; }

        public bool AnyEntities()
        {
            return (Activities != null && Activities.Any()) ||
                (MoodPrompts != null && MoodPrompts.Any()) ||
                (Responders != null && Responders.Any()) ||
                (MoodResponses != null && MoodResponses.Any());
        }
    }

    public class EntityInfo
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastEditedDate { get; set; }
    }

    public class ActivityInfo : EntityInfo
    {
        public string Title { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class MoodPromptInfo : EntityInfo
    {
        public string Name { get; set; }
        public ActivityInfo Activity { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTil { get; set; }
        public string NotificationText { get; set; }
    }

    public class ResponderInfo : EntityInfo
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string Region { get; set; }
        public DateTime? LastSync { get; set; }
    }

    public class MoodResponseInfo : EntityInfo
    {
        public Guid ResponderId { get; set; }
        public Guid MoodId { get; set; }
        public Guid? MoodPromptId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}