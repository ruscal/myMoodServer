using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMood.Web.Models;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Domain;
using Discover.Mail;
using Discover;
using Discover.HtmlTemplates;
using MyMood.Web;


namespace MyMood.Web.Controllers
{
    public partial class HomeController : ControllerBase
    {
        public HomeController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        public virtual ActionResult Index()
        {

            var events = (from e in db.Get<Event>()
                          orderby e.StartDate 
                          select e).ToList();

            EventsViewModel model = new EventsViewModel()
            {
                Events = (from e in events
                          select new EventModel()
                          {
                              Id = e.Id.ToString(),
                              Name = e.Name,
                              Title = e.Title,
                              StartDate = e.StartDateLocal.ToWebDateTime(),
                              StartDateUTC = e.StartDate.ToWebDateTimeUTC(),
                              EndDate = e.EndDateLocal.ToWebDateTime(),
                              EndDateUTC = e.EndDate.ToWebDateTimeUTC(),
                              GoLiveDate = e.ApplicationConfig.GoLiveDateLocal.ToWebDateTime(),
                              GoLiveDateUTC = e.ApplicationConfig.GoLiveDate.ToWebDateTimeUTC(),
                              MoodPromptCount = e.MoodPrompts.Count(),
                              PushNotificationCount = e.PushNotifications.Count(),
                              IndependentActivityCount = e.IndependentActivities.Count(),
                              TimeZone = e.ApplicationConfig.TimeZone
                          }).ToList()
            };

            return View(MVC.Home.Views.HomeIndex, model);
        }

        public virtual ActionResult Error(int? id)
        {
            
            return (id == null) ? 
                    View(MVC.Shared.Views.Error) :
                    View(string.Format("~/Views/Shared/{0}.cshtml", id));
        }

    }
}
