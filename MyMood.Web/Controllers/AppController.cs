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
using Discover.Security;

namespace MyMood.Web.Controllers
{
    public partial class AppController : EventControllerBase
    {
        #region Background processing bits to improve mood submission response times

        protected static System.Threading.Tasks.TaskFactory backgroundTaskFactory = new System.Threading.Tasks.TaskFactory(new Discover.Threading.LimitedConcurrencyLevelTaskScheduler(1));
        protected static System.Collections.Concurrent.ConcurrentQueue<MoodResponseWorkItem> responseSubmissionQueue = new System.Collections.Concurrent.ConcurrentQueue<MoodResponseWorkItem>();
        
        protected class MoodResponseWorkItem
        {
            public string EventName { get; set; }
            public string ResponderId { get; set; }
            public string Region { get; set; }
            public string DeviceId { get; set; }
            public IEnumerable<MoodResponseUpdateModel> MoodResponses { get; set; }
        }

        #endregion

        public AppController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        [AllowAnonymous]
        public virtual ActionResult Install(string EventName)
        {
            Event evnt = GetEvent(EventName);
            return View();
        }

        [AllowAnonymous]
        public virtual ActionResult Manifest(string EventName)
        {
            Event evnt = GetEvent(EventName);
            return View();
        }

        [AllowAnonymous]
        public virtual ActionResult OurMoodInstall(string EventName)
        {
            Event evnt = GetEvent(EventName);
            return View();
        }

        [AllowAnonymous]
        public virtual ActionResult OurMoodManifest(string EventName)
        {
            Event evnt = GetEvent(EventName);
            return View();
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult SyncData(string EventName, UpdateServiceFromAppModel model)
        {
            model.Responses = model.Responses ?? Enumerable.Empty<MoodResponseUpdateModel>();
            Event evnt = null;

            try
            {
                if (string.IsNullOrEmpty(EventName)) throw new ArgumentException("Invalid event");
                if (model == null) throw new ArgumentException("Invalid model");

                this.logger.Info("App", string.Format("Request to sync data - {0}", model.rid));

                var syncTimestamp = DateTime.UtcNow;
                
                evnt = db.Get<Event>().FirstOrDefault(e => e.Name.Equals(EventName, StringComparison.InvariantCultureIgnoreCase));
                
                var responder = evnt.AddResponder(new Guid(model.rid), model.reg, model.apn);

                var lastSync = responder.LastSync < model.LastUpdate ? responder.LastSync : model.LastUpdate;

                // NB: assume new set will commit fine on the background queue. 
                // If a failure occurs, it will be picked up on the next (or later) sync, and trigger a full "recovery" sync.
                var resError = model.ResTotal != (responder.Responses.Count() + model.Responses.Count());

                responder.LastSync = syncTimestamp;

                db.SaveChanges();

                if (model.Responses.Any())
                {
                    responseSubmissionQueue.Enqueue(new MoodResponseWorkItem
                    {
                        EventName = EventName,
                        ResponderId = model.rid,
                        Region = model.reg,
                        DeviceId = model.apn,
                        MoodResponses = model.Responses
                    });

                    backgroundTaskFactory.StartNew(ProcessQueuedMoodResponseWorkItems);
                }

                return Json(GetServiceUpdates(evnt, true, resError, lastSync, syncTimestamp, responder.ForceReset));
            }
            catch (Exception ex)
            {
                logger.Error("App", ex, "Failed sync from app" + (model == null ? " - model is null" : string.Empty));

                return Json(GetServiceUpdates(evnt, false, false, model.LastUpdate, DateTime.UtcNow, false));
            }
        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult SubmitMoodResponse(string EventName, SubmitResponseModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(EventName)) throw new ArgumentException("Invalid event");
                if (model == null) throw new ArgumentException("Invalid model");

                responseSubmissionQueue.Enqueue(new MoodResponseWorkItem 
                {
                    EventName = EventName,
                    ResponderId = model.rid,
                    Region = model.reg,
                    DeviceId = model.apn,
                    MoodResponses = model.r != null ? new MoodResponseUpdateModel[] { model.r } : Enumerable.Empty<MoodResponseUpdateModel>()
                });

                backgroundTaskFactory.StartNew(ProcessQueuedMoodResponseWorkItems);
                
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                logger.Error("App", ex, "Failed submitting response from app" + (model == null ? " - model is null" : string.Empty));
                
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult RequestGlobalReportInfo(string EventName, RequestGlobalReportInfoModel model)
        {
            try
            {
                Event evnt = GetEvent(EventName);
                this.logger.Info("App", string.Format("Request for global report info - lastRequest=[{0}]", model.LastReportRequested));
                return Json(GetGlobalMoodReportInfo(evnt, model.LastReportRequested, model.LastUpdate));

            }
            catch (Exception ex)
            {
                this.logger.Error("App", ex, "Failed request for personal mood report from app - model is null");

                return Json(new { Success = false });
            }
        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult RequestGlobalReportData(string EventName, GlobalMoodReportDataRequestModel requestModel)
        {
            try
            {
                Event evnt = GetEvent(EventName);
                this.logger.Info("App", string.Format("Request for global report data - reportId=[{0}] startDate=[{0}] endDate=[{0}]", requestModel.ReportId, requestModel.StartDate, requestModel.EndDate));
                GlobalMoodReportModel model = GetGlobalMoodReport(evnt, evnt.MoodCategories.First(), requestModel.StartDate, requestModel.EndDate, evnt.ApplicationConfig.ReportMoodIsStaleMins, false);
                model.ReportId = requestModel.ReportId;
                return Json(model);

            }
            catch (Exception ex)
            {
                this.logger.Error("App", ex, "Failed request for personal mood report from app - model is null");

                return Json(new { Success = false });
            }
        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult RequestPersonalMoodReport(string EventName, RequestPersonalMoodReportModel model)
        {
            try
            {
                Guid responderId;

                if (string.IsNullOrEmpty(EventName)) throw new ArgumentException("Invalid event");
                if (model == null) throw new ArgumentException("Invalid model");
                if (!Guid.TryParse(model.rid, out responderId)) throw new ArgumentException("Invalid responder Id");
                if (!ValidationHelper.IsValidEmailAddress(model.ReportRecipient)) throw new ArgumentException("Could not generate report - invalid recipient email address");

                this.logger.Info("App", string.Format("Request for personal mood report - responder=[{0}]", model.rid));

                var mailTemplate = this.htmlTemplateManager.GetHtml("MailTemplates/PersonalMoodReportMail").Replace("{ResponderId}", model.rid);
                            
                backgroundTaskFactory.StartNew(() =>
                    {
                        var db = DependencyResolver.Current.GetService<IDomainDataContext>();
                        var mailer = DependencyResolver.Current.GetService<IMailDispatchService>();
                        var logger = DependencyResolver.Current.GetService<ILogger>();

                        try
                        {
                            var responder = db.Get<Responder>().FirstOrDefault(r => r.Id == responderId && r.Event.Name.Equals(EventName, StringComparison.InvariantCultureIgnoreCase));

                            if (responder == null) throw new ArgumentException("Could not generate report - invalid respondent");

                            mailer.Send(new MailMessage()
                            {
                                To = new List<MailAddress>() { new MailAddress() { Address = model.ReportRecipient } },
                                Subject = "myMood - Timeline Report",
                                Body = mailTemplate,
                                IsBodyHtml = true,
                                Attachments = new List<MailAttachment>()
                                {
                                    new MailAttachment() 
                                    {
                                        Content = ReportHelper.PersonalMoodReportBytes(EventName, responderId, responder.Event.ApplicationConfig.ReportPassCode),
                                        ContentType = "application/octet",
                                        Name = "myMoodReport.pdf"
                                    }
                                }
                            });

                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            logger.Error("App", ex, string.Format("Failed request for personal mood report - responder=[{0}]  error=[{1}] recipient=[{2}]", model.rid, ex.Message, model.ReportRecipient));
                        }
                        finally
                        {
                            new StructureMap.Pipeline.HybridLifecycle().FindCache().DisposeAndClear();
                        }
                    });

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                if (model == null)
                {
                    this.logger.Error("App", ex, "Failed request for personal mood report from app - model is null");
                }
                else
                {
                    this.logger.Error("App", ex, string.Format("Failed request for personal mood report - responder=[{0}]  error=[{1}] recipient=[{2}]", model.rid, ex.Message, model.ReportRecipient));
                }
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult RegisterForAPNS(string EventName, AppRequestModelBase model)
        {
            try
            {
                Event evnt = GetEvent(EventName);
                if (model == null || string.IsNullOrWhiteSpace(model.apn)) throw new ArgumentException("Invalid model or APNSId");

                this.logger.Info(this.GetType(), string.Format("Registered for APNS - responder=[{0}] deviceId=[{1}]", model.rid, model.apn));

                var responderId = new Guid(model.rid);
                Responder responder = evnt.AddResponder(new Guid(model.rid), model.reg, model.apn);

                this.db.SaveChanges();

                return Json(new { Success = true });

            }
            catch (Exception ex)
            {
                if (model == null)
                {
                    this.logger.Error("App", ex, "Failed to register APNS Id from app - model is null");
                }
                else
                {
                    this.logger.Error("App", ex, string.Format("Failed to register APNS id  - responder=[{0}] APNSId=[{1}] error=[{2}] ", model.rid, model.apn, ex.Message));
                }
                return Json(new { Success = false });
            }

        }

        [HttpPost]
        [AppPassCodeOrAuthenticationRequired]
        public virtual JsonResult RegisterInterestInApp(string EventName, RegisterInterestInAppModel model)
        {
            try
            {
                Event evnt = GetEvent(EventName);
                if (model == null) throw new ArgumentException("Invalid model");
                if (!ValidationHelper.IsValidEmailAddress(model.InterestedParty)) throw new ArgumentException("Could not register interest - invalid email address");

                this.logger.Info("App", string.Format("Registered interest in app - responder=[{0}]", model.rid));

                //evnt.AddRegisteredInterest(model.InterestedParty);
                //this.db.SaveChanges();

                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                    var alertMailTemplate = htmlTemplateManager.GetHtml("MailTemplates/InterestRegisteredMail");
                    alertMailTemplate = alertMailTemplate.Replace("{InterestedParty}", model.InterestedParty);

                    var acknowledgeMailTemplate = htmlTemplateManager.GetHtml("MailTemplates/InterestAcknowledgedMail");

                    this.mailer.Send(new MailMessage()
                    {
                        To = new List<MailAddress>() { new MailAddress() { Address = Configuration.WebConfiguration.RegisteredInterestRecipient } },
                        Subject = "myMood - An attendee has registered interest in using myMood",
                        Body = alertMailTemplate,
                        IsBodyHtml = true
                    });

                    this.mailer.Send(new MailMessage()
                    {
                        To = new List<MailAddress>() { new MailAddress() { Address = model.InterestedParty } },
                        Subject = "myMood - Thank you for registering your interest",
                        Body = acknowledgeMailTemplate,
                        IsBodyHtml = true
                    });

                    this.db.SaveChanges();
                //});
                 
                return Json(new { Success = true });

            }
            catch (Exception ex)
            {
                if (model == null)
                {
                    this.logger.Error("App", ex, "Failed to register interest from app - model is null");
                }
                else
                {
                    this.logger.Error("App", ex, string.Format("Failed to register interest  - responder=[{0}] emailAddress=[{1}] error=[{2}] ", model.rid, model.InterestedParty, ex.Message));
                }
                return Json(new { Success = false });
            }

        }

        protected UpdateAppFromServiceModel GetServiceUpdates(Event evnt, bool syncSuccess, bool resError,  DateTime? lastSuccessfulServiceUpdate, DateTime syncTimestamp, bool forceReset)
        {
            if (evnt == null) return new UpdateAppFromServiceModel() { SyncSuccess = syncSuccess };
            var hasPromptUpdates = Configuration.WebConfiguration.DisableAppSyncs ? false : evnt.MoodPrompts.Any(p => lastSuccessfulServiceUpdate == null || p.LastEditedDate > lastSuccessfulServiceUpdate);
            var hasAppUpdates = Configuration.WebConfiguration.DisableAppSyncs ? false : lastSuccessfulServiceUpdate == null || evnt.ApplicationConfig.LastEditedDate > lastSuccessfulServiceUpdate.Value || forceReset;
            return new UpdateAppFromServiceModel()
            {
                HasPromptUpdates = hasPromptUpdates,
                SyncSuccess = syncSuccess,
                SyncTimestamp = syncTimestamp,
                ResError = resError,
                Application = hasAppUpdates ?
                    new ApplicationStateModel()
                    {
                        CurrentVersion = evnt.ApplicationConfig.CurrentVersion,
                        ForceUpdate = evnt.ApplicationConfig.ForceUpdate,
                        GoLiveDate = forceReset ? DateTime.UtcNow : evnt.ApplicationConfig.GoLiveDate,
                        LANWebServiceUri = evnt.ApplicationConfig.LanServiceUri,
                        SyncDataInterval = evnt.ApplicationConfig.SyncDataInterval,
                        SyncReportInterval = evnt.ApplicationConfig.SyncReportInterval,
                        SyncMode = evnt.ApplicationConfig.SyncMode.ToString(),
                        WANWebServiceUri = evnt.ApplicationConfig.WebServiceUri,
                        WarnSyncFailureAfterMins = evnt.ApplicationConfig.WarnSyncFailureAfterMins,
                        EventTimeZone = evnt.ApplicationConfig.TimeZone,
                        EventTimeOffset = GetEventUtcOffset(evnt),
                        UpdateAppUri = evnt.ApplicationConfig.UpdateAppUri,
                        ConnectionTimeout = evnt.ApplicationConfig.ConnectionTimeout
                    } : null,
                Prompts = hasPromptUpdates ? evnt.MoodPrompts.ToList().OrderBy(mp => mp.Activity.TimeStamp).Select(mp => new MoodPromptModel()
                {
                    Id = mp.Id.ToString(),
                    Name = mp.Name,
                    Activity = new ActivityModel()
                    {
                        Id = mp.Activity.Id,
                        Title = mp.Activity.Title,
                        TimeStamp = mp.Activity.TimeStamp
                    },
                    NotificationText = mp.NotificationText,
                    ActiveFrom = mp.ActiveFrom,
                    ActiveTil = mp.ActiveTil
                }) : null
            };
        }

        protected static void ProcessQueuedMoodResponseWorkItems()
        {
            var db = DependencyResolver.Current.GetService<IDomainDataContext>() as System.Data.Entity.DbContext;
            var logger = DependencyResolver.Current.GetService<ILogger>();

            MoodResponseWorkItem workItem;

            try
            {
                // try and process as many submissions as we can with a single db context
                while (responseSubmissionQueue.TryDequeue(out workItem))
                {
                    //var evnt = db.Get<Event>().FirstOrDefault(e => e.Name.Equals(workItem.EventName, StringComparison.InvariantCultureIgnoreCase));
                    //if (evnt == null) throw new ArgumentException("Invalid event");

                    //var responder = evnt.AddResponder(new Guid(workItem.ResponderId), workItem.Submission.reg, workItem.Submission.apn);

                    foreach (var moodResponse in workItem.MoodResponses)
                    {
                        var processingStartTime = DateTime.UtcNow;

                        try
                        {
                            logger.Info("App", string.Format("Request to set new mood response - mood=[{0}] responder=[{1}] responseId=[{2}]", moodResponse.m, workItem.ResponderId, moodResponse.i));

                            db.Database.ExecuteSqlCommand("exec SubmitResponse {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                                workItem.EventName,
                                workItem.ResponderId,
                                workItem.Region,
                                workItem.DeviceId,
                                moodResponse.i,
                                moodResponse.m,
                                moodResponse.p,
                                moodResponse.t,
                                MoodResponse.GetRoundedResponseTime(moodResponse.t)
                                );
                            
                            //if (workItem.Submission.r != null)
                            //{
                            //    logger.Info("App", string.Format("Request to set new mood response - mood=[{0}] responder=[{1}] responseId=[{2}]", workItem.Submission.r.m, workItem.Submission.rid, workItem.Submission.r.i));

                            //    Guid responseId;
                            //    if (!Guid.TryParse(workItem.Submission.r.i, out responseId)) throw new ArgumentException("Invalid response = id is not a Guid");

                            //    var mood = evnt.MoodCategories.SelectMany(mc => mc.Moods).FirstOrDefault(m => m.Name.Equals(workItem.Submission.r.m, StringComparison.InvariantCultureIgnoreCase));
                            //    //var mood = db.Get<Mood>().FirstOrDefault(m => m.Category.Event.Id == evnt.Id && m.Name.Equals(workItem.Submission.r.m, StringComparison.InvariantCultureIgnoreCase));
                            //    if (mood == null) throw new ArgumentException("Invalid mood");

                            //    var prompt = string.IsNullOrWhiteSpace(workItem.Submission.r.p) ? null : evnt.MoodPrompts.FirstOrDefault(p => p.Id == new Guid(workItem.Submission.r.p));
                            //    //var prompt = string.IsNullOrWhiteSpace(workItem.Submission.r.p) ? null : db.Get<MoodPrompt>().FirstOrDefault(p => p.Id == new Guid(workItem.Submission.r.p));

                            //    responder.AddResponse(responseId, mood, workItem.Submission.r.t, prompt);
                            //}

                            //db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            logger.Error("App", ex, string.Format("Failed submitting response - responder=[{0}]  error=[{1}] model=[{2}]", workItem.ResponderId, ex.Message, moodResponse.m));
                        }

                        System.Diagnostics.Debug.WriteLine(string.Format("Thread {0} processed submission from {1} in {2}ms", System.Threading.Thread.CurrentThread.ManagedThreadId, workItem.ResponderId, DateTime.UtcNow.Subtract(processingStartTime).TotalMilliseconds));
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Format("Thread {0} found no more work items", System.Threading.Thread.CurrentThread.ManagedThreadId));
            }
            finally
            {
                new StructureMap.Pipeline.HybridLifecycle().FindCache().DisposeAndClear();
            }
                    
        }
    }
}
