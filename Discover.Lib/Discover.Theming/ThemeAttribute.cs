using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Discover.Theming
{
    public class ThemeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            filterContext.Controller.ViewData.Add("__renderTheme", true);

            base.OnActionExecuting(filterContext);
        }
    }
}
