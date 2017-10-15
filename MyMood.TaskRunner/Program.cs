using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using MyMood.TaskRunner.Configuration;
using MyMood.Services;
using Discover.Mail;
using Discover.Logging;
using Discover.DomainModel;
using Discover.Mail.MailBee;

namespace MyMood.TaskRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MyMood.Infrastructure.EntityFramework.MyMoodDbContext();
            var logger = new Discover.Logging.ConsoleLogger();
            var mailer = new MailBeeMailServiceProvider(db);

            ScheduledWorkService service = new ScheduledWorkService(db,
                mailer,
                logger);

            service.DoScheduledWork();
        }
    }
}
