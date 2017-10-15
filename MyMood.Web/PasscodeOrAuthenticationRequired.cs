using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Discover.DomainModel;
using StructureMap;
using MyMood.Domain;
using Discover.Security;

namespace MyMood.Web
{
    public class PasscodeOrAuthenticationRequired : AuthorizeAttribute
    {
        public Action<System.Web.HttpContextBase> _onAuthorizationPassed;

        public PasscodeOrAuthenticationRequired() { }

        public PasscodeOrAuthenticationRequired(Action<System.Web.HttpContextBase> onAuthorizationPassed)
        {
            _onAuthorizationPassed = onAuthorizationPassed;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var passCode = filterContext.RouteData.Values["PassCode"];
            var eventName = filterContext.RouteData.Values["EventName"];


            var skipAuthorisation = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false);
            var reportPassCodeRequired = filterContext.ActionDescriptor.IsDefined(typeof(ReportPassCodeOrAuthenticationRequired), false) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(ReportPassCodeOrAuthenticationRequired), false);
            var appPassCodeRequired = filterContext.ActionDescriptor.IsDefined(typeof(AppPassCodeOrAuthenticationRequired), false) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AppPassCodeOrAuthenticationRequired), false);
            var passed = eventName != null && passCode != null && (reportPassCodeRequired && IsReportPassCodeValid(eventName.ToString(), new Guid(passCode.ToString()))) || (appPassCodeRequired && IsAppPassCodeValid(eventName.ToString(), new Guid(passCode.ToString())));
            
            if (!skipAuthorisation && !passed)
            {
                base.OnAuthorization(filterContext);
            }
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);

            if (_onAuthorizationPassed != null)
            {
                _onAuthorizationPassed(httpContext);
            }

            return isAuthorized;
        }

        protected bool IsAppPassCodeValid(string eventName, Guid passCode)
        {
            eventName = eventName.ToLower();

            IDomainDataContext db = ObjectFactory.GetInstance<IDomainDataContext>();

            return db.Get<Event>().Any(e => e.Name.ToLower() == eventName && e.ApplicationConfig.AppPassCode == passCode);

          
        }

         protected bool IsReportPassCodeValid(string eventName, Guid passCode)
        {
            eventName = eventName.ToLower();

            IDomainDataContext db = ObjectFactory.GetInstance<IDomainDataContext>();

            return db.Get<Event>().Any(e => e.Name.ToLower() == eventName && e.ApplicationConfig.ReportPassCode == passCode);

          
        }

    }
}