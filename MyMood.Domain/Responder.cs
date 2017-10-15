using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class Responder : Entity
    {
        protected Responder()
            : base()
        {
            this.ResponsesCollection = new List<MoodResponse>();
           // this.SyncReportsCollection = new List<SyncReport>();
        }

        public Responder(Event evnt, Guid id, string region, string deviceId)
            : this()
        {
            this.Event = evnt;
            this.Id = id;
            this.Region = region;
            this.DeviceId = deviceId;
            this.ForceReset = false;
        }

        public string Name { get; set; }
        public string DeviceId { get; set; }
        public string Region { get; set; }
        public DateTime? LastSync { get; set; }
        public bool ForceReset { get; set; }

        public virtual Event Event { get; set; }

        public IEnumerable<MoodResponse> Responses { get { return ResponsesCollection; } }
        protected virtual ICollection<MoodResponse> ResponsesCollection { get; set; }

        //public IEnumerable<SyncReport> SyncReports { get { return SyncReportsCollection; } }
        //protected virtual ICollection<SyncReport> SyncReportsCollection { get; set; }

        //public SyncReport AddSyncReport(MoodServer server)
        //{
        //    SyncReport log = new SyncReport(this, server, DateTime.UtcNow);
        //    this.SyncReportsCollection.Add(log);
        //    return log;
        //}

        //public SyncReport AddSyncReport(Guid logId, MoodServer server, DateTime timeStamp)
        //{
        //    SyncReport log = this.SyncReportsCollection.FirstOrDefault(l => l.Id == logId);
        //    if (log == null)
        //    {
        //        log = new SyncReport(logId, this, server, timeStamp);
        //        this.SyncReportsCollection.Add(log);
        //    }
        //    return log;
        //}

        public MoodResponse AddResponse(Mood mood, DateTime timeStamp, MoodPrompt prompt)
        {
            MoodResponse response = new MoodResponse(this, mood, timeStamp, prompt);
            this.ResponsesCollection.Add(response);
            return response;
        }

        public MoodResponse AddResponse(Guid responseId, Mood mood, DateTime timeStamp, MoodPrompt prompt)
        {
            MoodResponse response = this.Responses.FirstOrDefault(r => r.Id == responseId);
            if (response == null)
            {
                response = new MoodResponse(responseId, this, mood, timeStamp, prompt);
                this.ResponsesCollection.Add(response);
            }
            return response;
        }
    }
}
