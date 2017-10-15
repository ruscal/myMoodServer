using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMood.Web.Models;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Domain;
using Discover.Web.Mvc;
using System.Drawing;
using Discover.Mail;
using Discover.HtmlTemplates;

namespace MyMood.Web.Controllers
{
    public partial class ResponderController : EventControllerBase
    {

        public ResponderController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult PersonalMoodReport(string EventName, Guid responderId)
        {
            try
            {
                Event evnt = GetEvent(EventName);
                if (responderId == null) throw new ArgumentException("Invalid responder");

                Responder responder = this.db.Get<Responder>().FirstOrDefault(r => r.Id == responderId);

                return View(new PersonalMoodReportViewModel()
                {
                    ResponderId = responderId.ToString(),
                    Responses = responder.Responses.OrderBy(r => r.TimeStamp).Select(r => new PersonalMoodResponse()
                    {
                        Mood = r.Mood.Name,
                        TimeStamp = r.TimeStamp,
                        Title = r.Prompt == null ? "My Mood" : r.Prompt.Activity.Title
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed generating personal report - responder=[{0}]  error=[{1}]", responderId, ex.Message));
                return View(new PersonalMoodReportViewModel()
                {
                    ResponderId = responderId.ToString(),
                    Responses = new List<PersonalMoodResponse>()
                });
            }
        }

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult PersonalMoodReportPdf(string EventName, Guid responderId)
        {
            Event evnt = GetEvent(EventName);
            return File(ReportHelper.PersonalMoodReportBytes(EventName, responderId, evnt.ApplicationConfig.ReportPassCode), "application/octet", "myMoodReport.pdf");
        }

    }
}
