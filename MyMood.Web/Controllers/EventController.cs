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
using Discover.Web.Mvc;
using Discover.Common;

namespace MyMood.Web.Controllers
{
    public partial class EventController : EventControllerBase
    {
        public EventController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
        }

        [HttpGet]
        public virtual ActionResult MonitorByName(string EventName)
        {
            return Monitor(GetEvent(EventName));
        }

        [HttpGet]
        public virtual ActionResult Monitor(Guid id)
        {
            return Monitor(GetEvent(id));
        }

        private ActionResult Monitor(Event e)
        {
            var model = new EventModel
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Title = e.Title,
                ReportPasscode = e.ApplicationConfig.ReportPassCode,
                StartDate = e.StartDateLocal.ToWebDateTime(),
                StartDateUTC = e.StartDate.ToWebDateTimeUTC(),
                EndDate = e.EndDateLocal.ToWebDateTime(),
                EndDateUTC = e.EndDate.ToWebDateTimeUTC(),
                TimeZone = e.ApplicationConfig.TimeZone,
                RegisteredApps = e.Responders.Count(),
                ResponderCount = (from s in e.Responders
                                  where s.Responses.Count() > 0
                                  select s).Count(),
                Moods = from m in e.MoodCategories.SelectMany(x => x.Moods)
                        orderby m.DisplayIndex
                        select new MoodModel
                        {
                            Id = m.Id,
                            Name = m.Name,
                            DisplayColor = m.DisplayColor,
                            DisplayIndex = m.DisplayIndex,
                            MoodType = m.MoodType
                        },
                MoodPromptCount = e.MoodPrompts.Count(),
                PushNotificationCount = e.PushNotifications.Count(),
                IndependentActivityCount = e.IndependentActivities.Count(),
                MoodSnapShot = GetTotals(e.MoodCategories.First(), 0, true),
                LatestSnapShot = GetSnapshot(e.MoodCategories.First(), DateTime.UtcNow, true),
                TotalReportLines = GetTotalsByPrompt(e)
            };


            if (Request.IsAjaxRequest())
                return PartialView(MVC.Event.Views.Monitor, model);

            return View(MVC.Event.Views.Monitor, model);
        }

        [HttpGet]
        public virtual ActionResult Add()
        {
            var model = new EditEventModel
            {
                StartDate = new ModelTime{ Date = DateTime.Today, Hour = 9, Minute = 0 },
                EndDate = new ModelTime { Date = DateTime.Today.AddDays(1), Hour = 18, Minute = 0 },
                ApplicationConfig = new ApplicationConfigModel
                {
                    CurrentVersion = "1.0",
                    AppPassCode = Guid.NewGuid(),
                    ReportPassCode = Guid.NewGuid(),
                    HasGoLiveDate = false,
                    WebServiceUri = "https://www.learning-performance.com/MyMood/",
                    LanServiceUri = "http://192.168.100.4:8080", 
                    SyncDataInterval = 60, 
                    SyncReportInterval = 0, 
                    WarnSyncFailureAfterMins = 5, 
                    ForceUpdate = false, 
                    UpdateAppUri = "https://www.learning-performance.com/MyMood/",
                    ReportMoodIsStaleMins = 0, 
                    ReportSplineTension = 0.5, 
                    TimeZone = TimeZoneInfo.Utc.Id, 
                    SyncMode = SyncMode.LANthenWAN
                }
            };
            
            PopulateModelLists(model);
            return PartialView(MVC.Event.Views.Edit, model);
        }

        [HttpGet]
        public virtual ActionResult Edit(Guid id)
        {
            var e = GetEvent(id);

            var config = new ApplicationConfigModel()
            {
                AppPassCode = e.ApplicationConfig.AppPassCode,
                CurrentVersion = e.ApplicationConfig.CurrentVersion,
                ForceUpdate = e.ApplicationConfig.ForceUpdate,
                UpdateAppUri = e.ApplicationConfig.UpdateAppUri,
                HasGoLiveDate = e.ApplicationConfig.GoLiveDate.HasValue,
                GoLiveDate = new ModelTime { Date = e.ApplicationConfig.GoLiveDateLocal ?? DateTime.Now },
                LanServiceUri = e.ApplicationConfig.LanServiceUri,
                ReportMoodIsStaleMins = e.ApplicationConfig.ReportMoodIsStaleMins,
                ReportPassCode = e.ApplicationConfig.ReportPassCode,
                ReportSplineTension = e.ApplicationConfig.ReportSplineTension,
                SyncDataInterval = e.ApplicationConfig.SyncDataInterval,
                SyncMode = e.ApplicationConfig.SyncMode,
                SyncReportInterval = e.ApplicationConfig.SyncReportInterval,
                TimeZone = e.ApplicationConfig.TimeZone,
                WarnSyncFailureAfterMins = e.ApplicationConfig.WarnSyncFailureAfterMins,
                WebServiceUri = e.ApplicationConfig.WebServiceUri
            };

            var model = new EditEventModel()
            {
                Id = e.Id,
                Name = e.Name,
                Title = e.Title,
                StartDate = new ModelTime { Date = e.StartDateLocal ?? DateTime.Today },
                EndDate = new ModelTime { Date = e.EndDateLocal ?? DateTime.Today },
                ApplicationConfig = config
            };

            PopulateModelLists(model);

            return PartialView(model);

        }

        [HttpPost]
        public virtual ActionResult Save(EditEventModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    Event e;
                    if (model.Id == Guid.Empty)
                    {
                        if (db.Get<Event>().Any(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            ModelState.AddModelError("Name", "An event with that name already exists");
                        }
                        else
                        {
                            e = new Event(model.Name, model.Title);
                            BindEventModelToEvent(model, e);
                            db.Add(e);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        e = GetEvent(model.Id);
                        BindEventModelToEvent(model, e);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateModelLists(model);
                return Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.Event.Views.Edit, model) });
            }

            return Json(new { success = true });
                
        }

        private void BindEventModelToEvent(EditEventModel model, Event e)
        {
            e.Title = model.Title;
            e.Name = model.Name;
            e.ApplicationConfig.CurrentVersion = model.ApplicationConfig.CurrentVersion;
            e.StartDate = model.StartDate.FullDate;
            e.EndDate = model.EndDate.FullDate;
            e.ApplicationConfig.AppPassCode = model.ApplicationConfig.AppPassCode;
            e.ApplicationConfig.ReportPassCode = model.ApplicationConfig.ReportPassCode;
            e.ApplicationConfig.GoLiveDate = null;
            if (model.ApplicationConfig.HasGoLiveDate) e.ApplicationConfig.GoLiveDate = model.ApplicationConfig.GoLiveDate.FullDate;
            e.ApplicationConfig.WebServiceUri = model.ApplicationConfig.WebServiceUri;
            e.ApplicationConfig.LanServiceUri = model.ApplicationConfig.LanServiceUri;
            e.ApplicationConfig.SyncDataInterval = model.ApplicationConfig.SyncDataInterval;
            e.ApplicationConfig.SyncReportInterval = model.ApplicationConfig.SyncReportInterval;
            e.ApplicationConfig.WarnSyncFailureAfterMins = model.ApplicationConfig.WarnSyncFailureAfterMins;
            e.ApplicationConfig.ForceUpdate = model.ApplicationConfig.ForceUpdate;
            e.ApplicationConfig.UpdateAppUri = model.ApplicationConfig.UpdateAppUri;
            e.ApplicationConfig.ReportMoodIsStaleMins = model.ApplicationConfig.ReportMoodIsStaleMins;
            e.ApplicationConfig.ReportSplineTension = model.ApplicationConfig.ReportSplineTension;
            e.ApplicationConfig.TimeZone = model.ApplicationConfig.TimeZone;
            e.ApplicationConfig.SyncMode = model.ApplicationConfig.SyncMode;
            e.ConvertAllToUTC();
        }

        public virtual ActionResult Remove(Guid id)
        {
            try
            {
                var e = GetEvent(id);

                db.Remove(e.ApplicationConfig);
                db.Remove(e);

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed removing event {0}", id));
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        private void PopulateModelLists(EditEventModel model)
        {
            model.AvailableSyncModes = EnumHelper.GetSelectListItemsFor<SyncMode>();
            model.AvailableTimeZones = (from tx in TimeZoneInfo.GetSystemTimeZones()
                                        select new SelectListItem
                                        {
                                            Text = tx.DisplayName,
                                            Value = tx.Id,
                                        })
                                        .ToArray();
        }
    }
}
