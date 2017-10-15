using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Discover.Web.Mvc
{
    public static class ControllerHelper
    {
        /// <summary>
        /// Renders a partial view to a string (typically so that it can be embedded inside another type of ActionResult, e.g. JsonResult)
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static string RenderPartialViewToString(this Controller controller)
        {
            return RenderPartialViewToString(controller, null, null, null);
        }

        /// <summary>
        /// Renders a partial view to a string (typically so that it can be embedded inside another type of ActionResult, e.g. JsonResult)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName)
        {
            return RenderPartialViewToString(controller, viewName, null, null);
        }

        /// <summary>
        /// Renders a partial view to a string (typically so that it can be embedded inside another type of ActionResult, e.g. JsonResult)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderPartialViewToString(this Controller controller, object model)
        {
            return RenderPartialViewToString(controller, null, model, null);
        }

        /// <summary>
        /// Renders a partial view to a string (typically so that it can be embedded inside another type of ActionResult, e.g. JsonResult)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName, object model)
        {
            return RenderPartialViewToString(controller, viewName, model, null);
        }

        /// <summary>
        /// Renders a partial view to a string (typically so that it can be embedded inside another type of ActionResult, e.g. JsonResult)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <param name="htmlFieldPrefix" remarks="optional"></param>
        /// <returns></returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName, object model, string htmlFieldPrefix = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(controller.ViewData) { Model = model };

            if (!string.IsNullOrEmpty(htmlFieldPrefix))
            {
                viewData.TemplateInfo = new TemplateInfo()
                {
                    HtmlFieldPrefix = string.IsNullOrEmpty(viewData.TemplateInfo.HtmlFieldPrefix) ? htmlFieldPrefix : string.Concat(viewData.TemplateInfo.HtmlFieldPrefix, ".", htmlFieldPrefix)
                };
            }

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, viewData, controller.TempData, sw);
                
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
