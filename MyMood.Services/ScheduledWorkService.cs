using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Discover.DomainModel;
using Discover.Mail;
using Discover.Logging;

namespace MyMood.Services
{
    public class ScheduledWorkService
    {
        private readonly IDomainDataContext db;
        private readonly IMailDeliveryService mailer;
        //private readonly IDataSyncClient dataSync;
        private readonly ILogger logger;

        public ScheduledWorkService(IDomainDataContext db, IMailDeliveryService mailer,  ILogger logger)
        {
            this.db = db;
            this.mailer = mailer;
            //this.dataSync = dataSync;
            this.logger = logger;
        }

        public void DoScheduledWork()
        {
            RunMailerTasks();
            RunDataSync();
            //RunAPNSTasks();
        }

        protected void RunMailerTasks()
        {
            this.mailer.DeliverPendingMessages();
            //this.db.SaveChanges();
        }

        protected void RunDataSync()
        {
            //this.dataSync.SynchroniseOutstanding();
        }

        //protected void RunAPNSTasks()
        //{
        //    PushNotificationManager pushMan = new PushNotificationManager(this.db, this.logger);
        //    pushMan.CheckAndSendNotifications();
        //}

    }
}