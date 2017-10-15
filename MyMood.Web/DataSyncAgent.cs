using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Web.Models.DataSync;
using MyMood.Domain;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace MyMood.Web
{
    public interface IDataSyncClient
    {
        void SynchroniseOutstanding();
    }

    public class DataSyncAgent : IDataSyncClient
    {
        protected readonly IDomainDataContext db;
        protected readonly ILogger logger;

        public DataSyncAgent(IDomainDataContext db, ILogger logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public void SynchroniseOutstanding()
        {
            var thisServer = db.Get<MyMood.Domain.MoodServer>().Where(s => s.Name == Configuration.WebConfiguration.ServerName).FirstOrDefault();

            if (thisServer != null)
            {
                var otherServers = (from s in db.Get<MyMood.Domain.MoodServer>()
                                    where s.Id != thisServer.Id && (!s.LastSuccessfulSync.HasValue || s.LastSuccessfulSync.Value.AddMinutes(Configuration.WebConfiguration.ServerSyncIntervalMinutes) < DateTime.UtcNow)
                                    select s)
                                    .ToArray();

                var eventsToSync = (from e in db.Get<Event>()
                                    select e)
                                    .ToArray();

                var client = new HttpClient();

                foreach (var otherServer in otherServers)
                {
                    client.BaseAddress = new Uri(otherServer.BaseAddress);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    foreach (var evt in eventsToSync)
                    {
                        try
                        {
                            var outgoingChanges = this.GetChangesNewerThan(evt, otherServer.LastSuccessfulSync ?? DateTime.MinValue);

                            var response = client.PostAsJsonAsync<DataSyncChangeSet>("api/datasync?eventName=" + evt.Name, outgoingChanges).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var incomingChanges = response.Content.ReadAsAsync<DataSyncChangeSet>().Result;

                                this.SyncEntityChanges(evt, incomingChanges, null);
                            }
                            else
                            {
                                this.logger.Error(this.GetType(), string.Format("Failed during data sync between {0} and {1} for event {2} - HTTP {3} {4}", thisServer.Name, otherServer.Name, (int)response.StatusCode, response.ReasonPhrase));
                            }

                            otherServer.LastSuccessfulSync = DateTime.UtcNow;

                            db.Add(new MoodServerSyncReport(thisServer, otherServer, DateTime.UtcNow));

                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            this.logger.Error(this.GetType(), ex, string.Format("Failed during data sync between {0} and {1}", thisServer.Name, otherServer.Name));
                        }
                    }
                }                
            }
        }

        public void SyncEntityChanges(Event evt, DataSyncChangeSet incomingChanges, DataSyncChangeSet outgoingChanges)
        {
            if (incomingChanges == null || !incomingChanges.AnyEntities()) return;

            if (incomingChanges.MoodPrompts != null)
            {
                var suppressReturnChangesForMoodPrompts = new List<Guid>();

                foreach (var incomingPrompt in incomingChanges.MoodPrompts)
                {
                    var prompt = db.Get<MoodPrompt>().Where(a => a.Id == incomingPrompt.Id).SingleOrDefault();

                    if (prompt == null)
                    {
                        // entity does not exist locally, so create
                        prompt = evt.AddMoodPrompt(incomingPrompt.Name, incomingPrompt.Activity.Title, incomingPrompt.NotificationText, incomingPrompt.Activity.TimeStamp, incomingPrompt.ActiveFrom, incomingPrompt.ActiveTil);
                        prompt.OverrideIdentity(incomingPrompt.Id, incomingPrompt.CreationDate);
                        prompt.LastEditedDate = incomingPrompt.LastEditedDate;
                        prompt.Activity.OverrideIdentity(incomingPrompt.Activity.Id, incomingPrompt.Activity.CreationDate);
                        prompt.Activity.LastEditedDate = incomingPrompt.Activity.LastEditedDate;
                    }
                    else if (prompt.LastEditedDate < incomingPrompt.LastEditedDate)
                    {
                        // pushed version is newer, so update local version
                        prompt.LastEditedDate = prompt.LastEditedDate;
                        prompt.ActiveFrom = incomingPrompt.ActiveFrom;
                        prompt.ActiveTil = incomingPrompt.ActiveTil;
                        prompt.Name = incomingPrompt.Name;
                        prompt.NotificationText = incomingPrompt.NotificationText;

                        // also make sure we aren't pushing local changes back to caller (as they are no longer relevant)
                        suppressReturnChangesForMoodPrompts.Add(incomingPrompt.Id);
                    }
                    else
                    {
                        // local version is newer, so disregard (and should have automatically been included in outgoing/return entities)
                    }
                }

                if (outgoingChanges != null && outgoingChanges.MoodPrompts != null && suppressReturnChangesForMoodPrompts.Any())
                {
                    outgoingChanges.MoodPrompts = outgoingChanges.MoodPrompts.Where(mp => !suppressReturnChangesForMoodPrompts.Contains(mp.Id));
                }
            }

            if (incomingChanges.Activities != null)
            {
                var suppressReturnChangesForActivities = new List<Guid>();

                foreach (var incomingActivity in incomingChanges.Activities)
                {
                    var activity = db.Get<Activity>().Where(a => a.Id == incomingActivity.Id).SingleOrDefault();

                    if (activity == null)
                    {
                        // entity does not exist locally, so create
                        activity = evt.AddActivity(incomingActivity.Title, incomingActivity.TimeStamp);
                        activity.OverrideIdentity(incomingActivity.Id, incomingActivity.CreationDate);
                        activity.LastEditedDate = incomingActivity.LastEditedDate;
                    }
                    else if (activity.LastEditedDate < incomingActivity.LastEditedDate)
                    {
                        // pushed version is newer, so update local version
                        activity.LastEditedDate = incomingActivity.LastEditedDate;
                        activity.TimeStamp = incomingActivity.TimeStamp;
                        activity.Title = incomingActivity.Title;

                        // also make sure we aren't pushing local changes back to caller (as they are no longer relevant)
                        suppressReturnChangesForActivities.Add(incomingActivity.Id);
                    }
                    else
                    {
                        // local version is newer, so disregard (and should have automatically been included in outgoing/return entities)
                    }
                }

                if (outgoingChanges != null && outgoingChanges.Activities != null && suppressReturnChangesForActivities.Any())
                {
                    outgoingChanges.Activities = outgoingChanges.Activities.Where(a => !suppressReturnChangesForActivities.Contains(a.Id));
                }
            }

            if (incomingChanges.Responders != null)
            {
                var suppressReturnChangesForResponders = new List<Guid>();

                foreach (var incomingResponder in incomingChanges.Responders)
                {
                    var responder = db.Get<Responder>().Where(r => r.Id == incomingResponder.Id).SingleOrDefault();

                    if (responder == null)
                    {
                        // entity does not exist locally, so create
                        responder = evt.AddResponder(incomingResponder.Id, incomingResponder.Region, incomingResponder.DeviceId);
                        responder.OverrideIdentity(incomingResponder.Id, incomingResponder.CreationDate);
                        responder.Name = incomingResponder.Name;
                        responder.LastSync = incomingResponder.LastSync;
                        responder.LastEditedDate = incomingResponder.LastEditedDate;
                    }
                    else if (responder.LastEditedDate < incomingResponder.LastEditedDate)
                    {
                        // pushed version is newer, so update local version
                        responder.Name = incomingResponder.Name;
                        responder.DeviceId = incomingResponder.DeviceId;
                        responder.Region = incomingResponder.Region;
                        responder.LastSync = incomingResponder.LastSync;
                        responder.LastEditedDate = incomingResponder.LastEditedDate;

                        // also make sure we aren't pushing local changes back to caller (as they are no longer relevant)
                        suppressReturnChangesForResponders.Add(incomingResponder.Id);
                    }
                    else
                    {
                        // local version is newer, so disregard (and should have automatically been included in outgoing/return entities)
                    }
                }

                if (outgoingChanges != null && outgoingChanges.Responders != null && suppressReturnChangesForResponders.Any())
                {
                    outgoingChanges.Responders = outgoingChanges.Responders.Where(r => !suppressReturnChangesForResponders.Contains(r.Id));
                }
            }

            if (incomingChanges.MoodResponses != null)
            {
                var suppressReturnChangesForResponses = new List<Guid>();

                foreach (var incomingResponse in incomingChanges.MoodResponses)
                {
                    var moodResponse = db.Get<MoodResponse>().Where(r => r.Id == incomingResponse.Id).SingleOrDefault();
                    var mood = evt.MoodCategories.SelectMany(mc => mc.Moods).Single(m => m.Id == incomingResponse.MoodId);
                    var moodPrompt = incomingResponse.MoodPromptId.HasValue ? evt.MoodPrompts.Single(mp => mp.Id == incomingResponse.MoodPromptId) : null;

                    if (moodResponse == null)
                    {
                        // entity does not exist locally, so create
                        moodResponse = evt.Responders.Single(r => r.Id == incomingResponse.ResponderId).AddResponse(mood, incomingResponse.TimeStamp, moodPrompt);
                        moodResponse.OverrideIdentity(incomingResponse.Id, incomingResponse.CreationDate);
                        moodResponse.LastEditedDate = incomingResponse.LastEditedDate;
                    }
                    else if (moodResponse.LastEditedDate < incomingResponse.LastEditedDate)
                    {
                        // pushed version is newer, so update local version
                        moodResponse.Mood = mood;
                        moodResponse.Prompt = moodPrompt;
                        moodResponse.TimeStamp = incomingResponse.TimeStamp;
                        moodResponse.LastEditedDate = incomingResponse.LastEditedDate;

                        // also make sure we aren't pushing local changes back to caller (as they are no longer relevant)
                        suppressReturnChangesForResponses.Add(incomingResponse.Id);
                    }
                    else
                    {
                        // local version is newer, so disregard (and should have automatically been included in outgoing/return entities)
                    }
                }

                if (outgoingChanges != null && outgoingChanges.MoodResponses != null && suppressReturnChangesForResponses.Any())
                {
                    outgoingChanges.MoodResponses = outgoingChanges.MoodResponses.Where(mr => !suppressReturnChangesForResponses.Contains(mr.Id));
                }
            }
        }

        public DataSyncChangeSet GetChangesNewerThan(Event evt, DateTime newerThan)
        {
            return new DataSyncChangeSet()
            {
                MoodPrompts = (from mp in db.Get<MoodPrompt>()
                               where mp.Event.Id == evt.Id && mp.LastEditedDate > newerThan
                               select new MoodPromptInfo
                               {
                                   Id = mp.Id,
                                   CreationDate = mp.CreationDate,
                                   LastEditedDate = mp.LastEditedDate,
                                   ActiveFrom = mp.ActiveFrom,
                                   ActiveTil = mp.ActiveTil,
                                   Activity = new ActivityInfo
                                   {
                                       Id = mp.Activity.Id,
                                       CreationDate = mp.Activity.CreationDate,
                                       LastEditedDate = mp.Activity.LastEditedDate,
                                       TimeStamp = mp.Activity.TimeStamp,
                                       Title = mp.Activity.Title
                                   },
                                   Name = mp.Name,
                                   NotificationText = mp.NotificationText
                               }),
                Activities = (from a in db.Get<Activity>()
                              where a.Event.Id == evt.Id && a.LastEditedDate > newerThan
                              select new ActivityInfo
                              {
                                  Id = a.Id,
                                  CreationDate = a.CreationDate,
                                  LastEditedDate = a.LastEditedDate,
                                  TimeStamp = a.TimeStamp,
                                  Title = a.Title
                              }),
                Responders = (from r in db.Get<Responder>()
                              where r.Event.Id == evt.Id && r.LastEditedDate > newerThan
                              select new ResponderInfo
                              {
                                  Id = r.Id,
                                  CreationDate = r.CreationDate,
                                  LastEditedDate = r.LastEditedDate,
                                  DeviceId = r.DeviceId,
                                  LastSync = r.LastSync,
                                  Name = r.Name,
                                  Region = r.Region
                              }),
                MoodResponses = (from mr in db.Get<MoodResponse>()
                                 where mr.Event.Id == evt.Id && mr.LastEditedDate > newerThan
                                 select new MoodResponseInfo
                                 {
                                     Id = mr.Id,
                                     CreationDate = mr.CreationDate,
                                     LastEditedDate = mr.LastEditedDate,
                                     MoodId = mr.Mood.Id,
                                     MoodPromptId = mr.Prompt != null ? mr.Prompt.Id : (Guid?)null,
                                     ResponderId = mr.Responder.Id,
                                     TimeStamp = mr.TimeStamp
                                 })
            };
        }
    }
}