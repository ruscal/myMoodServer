using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Discover.DomainModel;
using Discover.Logging;
using Discover.Mail;
using Discover.HtmlTemplates;
using MyMood.Web.Models.DataSync;
using MyMood.Domain;

namespace MyMood.Web.Controllers
{
    public partial class DataSyncController : System.Web.Http.ApiController
    {
        protected readonly IDomainDataContext db;
        protected readonly ILogger logger;
        protected readonly IMailDispatchService mailer;
        protected readonly IHtmlTemplateManager htmlTemplateManager;
        protected readonly DataSyncAgent syncAgent;

        protected DataSyncController()
        {
        }

        protected DataSyncController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager, DataSyncAgent syncAgent)
        {
            this.db = db;
            this.logger = logger;
            this.mailer = mailer;
            this.htmlTemplateManager = htmlTemplateManager;
            this.syncAgent = syncAgent;
        }

        [HttpGet]
        public DataSyncChangeSet EventData(string eventName)
        {
            return EventData(eventName, DateTime.MinValue);
        }

        [HttpGet]
        public DataSyncChangeSet EventData(string eventName, DateTime newerThan)
        {
            var evt = db.Get<Event>().Where(e => e.Name.Equals(eventName, StringComparison.InvariantCultureIgnoreCase)).First();

            return syncAgent.GetChangesNewerThan(evt, newerThan);
        }

        [HttpPost]
        public DataSyncChangeSet EventData(string eventName, DateTime newerThan, DataSyncChangeSet pushChanges)
        {
            var evt = db.Get<Event>().Where(e => e.Name.Equals(eventName, StringComparison.InvariantCultureIgnoreCase)).First();

            var outgoingChanges = syncAgent.GetChangesNewerThan(evt, newerThan);

            syncAgent.SyncEntityChanges(evt, outgoingChanges, pushChanges);

            db.SaveChanges();

            return outgoingChanges;
        }
    }
}
