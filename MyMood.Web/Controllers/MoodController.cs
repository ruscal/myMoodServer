using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMood.Web.Models;
using Discover.DomainModel;
using Discover.Logging;
using System.Web.Configuration;
using MyMood.Domain;
using Discover.HtmlTemplates;
using Discover.Mail;

namespace MyMood.Web.Controllers
{
    public partial class MoodController : ControllerBase
    {

        public MoodController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }





    }
}
