using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

namespace MyMood.Web
{
    public class TokenAuthenticationRequired : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var passCode = filterContext.RouteData.Values["PassCode"];

            if (passCode == null || string.IsNullOrEmpty((string)passCode) || !IsTokenValid((string)passCode)) filterContext.Result = new HttpUnauthorizedResult();
            base.OnActionExecuting(filterContext);
        }


        protected bool IsTokenValid(string token)
        {
            string validToken = Configuration.WebConfiguration.ApplicationPassCode;
            if (string.IsNullOrEmpty(validToken)) return true;
            return token.Equals(validToken, StringComparison.InvariantCultureIgnoreCase);
        }

    }

}