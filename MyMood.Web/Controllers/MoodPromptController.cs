using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMood.Web.Models;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Domain;
using Discover.Mail;
using Discover.HtmlTemplates;
using Discover.Web.Mvc;

namespace MyMood.Web.Controllers
{
    public partial class MoodPromptController : EventControllerBase
    {
        private Event e;
        private MoodPrompt mp;

        public MoodPromptController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        public virtual ViewResult Index(string EventName)
        {
            e = GetEvent(EventName);
            ViewBag.Title = String.Format("Mood Prompts for: {0}", e.Title);

            var model = e.MoodPrompts.OrderBy(x => x.Activity.TimeStamp).Select(x => new MoodPromptModel()
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Activity = new ActivityModel()
                {
                    Id = x.Activity.Id,
                    Title = x.Activity.Title,
                    TimeStamp = x.Activity.TimeStampLocal,
                    TimeStampUTC = x.Activity.TimeStamp.ToWebDateTimeUTC()
                },
                NotificationText = x.NotificationText,
                ActiveFrom = x.ActiveFromLocal,
                ActiveFromUTC = x.ActiveFrom.ToWebDateTimeUTC(),
                ActiveTil = x.ActiveTilLocal,
                ActiveTillUTC = x.ActiveTil.ToWebDateTimeUTC()
            }).ToList();

            return View(MVC.MoodPrompt.Views.Index, model);
        }


        public virtual ViewResult Details(string EventName, string id)
        {
            var now = DateTime.UtcNow.AddMinutes(1);
            e = GetEvent(EventName);
            mp = e.MoodPrompts.FirstOrDefault(x => mp.Id == new Guid(id));
            if (mp == null) return View();

            var model = new MoodPromptModel()
            {
                Id = mp.Id.ToString(),
                Name = mp.Name,
                Activity = new ActivityModel()
                {
                    Id = mp.Activity.Id,
                    Title = mp.Activity.Title,
                    TimeStamp = mp.Activity.TimeStampLocal
                },
                NotificationText = mp.NotificationText,
                ActiveFrom = mp.ActiveFromLocal,
                ActiveTil = mp.ActiveTilLocal,
                CanDelete = now < mp.Activity.TimeStamp
            };
            return View(model);
        }

        [HttpGet]
        public virtual ActionResult Add(Guid eventId)
        {
            e = GetEvent(eventId);
            var model = new EditMoodPromptModel
            {
                EventId = eventId,
                Name = string.Format("Mood Prompt {0}", e.MoodPrompts.Count() + 1),
                Activity = new EditActivityModel()
            };

            return PartialView(MVC.MoodPrompt.Views.Edit, model);
        } 


        [HttpGet]
        public virtual ActionResult Edit(string EventName, Guid id)
        {

            try 
            {
                e = GetEvent(EventName);
                mp = e.MoodPrompts.Where(x => x.Id ==id).First();
            }
            catch(Exception)
            {
                throw new HttpException(404, "Mood Prompt not found");
            }
        
            var model = new EditMoodPromptModel()
            {
                Id = mp.Id,
                EventId = e.Id,
                Name = mp.Name,
                Activity = new EditActivityModel()
                {
                    Id = mp.Activity.Id,
                    Title = mp.Activity.Title,
                    TimeStamp = new ModelTime { Date = mp.Activity.TimeStampLocal }
                },
                NotificationText = mp.NotificationText,
                ActiveFrom = new ModelTime { Date = mp.ActiveFromLocal },
                ActiveTil = new ModelTime { Date = mp.ActiveTilLocal }
            };
            return PartialView(model);
        }


        [HttpPost]
        public virtual ActionResult Save(EditMoodPromptModel model)
        {
            if (ModelState.IsValid)
            {
                try {
                    MoodPrompt mp;
                    var e = GetEvent(model.EventId);

                    if (model.Id == Guid.Empty)
                    {
                        mp = e.AddMoodPrompt(model.Name,
                                             model.Activity.Title,
                                             model.NotificationText,
                                             model.Activity.TimeStamp.FullDate,
                                             model.ActiveFrom.FullDate,
                                             model.ActiveTil.FullDate);
                    }
                    else 
                    {
                        mp = e.MoodPrompts.Where(x => x.Id == model.Id).First();

                        mp.Name = model.Name;
                        mp.Activity.Title = model.Activity.Title;
                        mp.Activity.TimeStamp = model.Activity.TimeStamp.FullDate;
                        mp.NotificationText = model.NotificationText;
                        mp.ActiveFrom = model.ActiveFrom.FullDate;
                        mp.ActiveTil = model.ActiveTil.FullDate;
                        mp.ConvertAllToUTC();
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.MoodPrompt.Views.Edit, model) });
        }

        [HttpPost]
        public virtual ActionResult Delete(Guid id, Guid eventId)
        {
            try 
            {
                e = GetEvent(eventId);
                mp = e.MoodPrompts.Where(x => x.Id == id).First();
                db.Remove(mp.Activity);
                db.Remove(mp);
                db.SaveChanges();
                
                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

    }
}