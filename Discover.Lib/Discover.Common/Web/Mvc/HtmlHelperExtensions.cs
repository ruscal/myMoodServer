using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Web.WebPages;
using System.Web;

namespace Discover.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Enables generation of appropriate client-side validation attributes when rendering inputs in a partial view (without requiring the inputs to be wrapped within Html.BeginForm/EndForm calls)
        /// </summary>
        /// <param name="helper"></param>
        public static void EnablePartialViewValidation(this HtmlHelper helper)
        {
            if (helper.ViewContext.FormContext == null)
            {
                helper.ViewContext.FormContext = new FormContext();
            }
        }

        /// <summary>
        /// Renders the specified partial view as a HTML-encoded string.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="partialViewName"></param>
        /// <param name="model"></param>
        /// <param name="htmlFieldPrefix"></param>
        /// <returns></returns>
        public static MvcHtmlString Partial(this HtmlHelper helper, string partialViewName, object model, string htmlFieldPrefix)
        {
            var viewData = new ViewDataDictionary(helper.ViewData);
            if(!string.IsNullOrEmpty(htmlFieldPrefix))
            {
                viewData.TemplateInfo = new TemplateInfo()
                {
                    HtmlFieldPrefix = string.IsNullOrEmpty(viewData.TemplateInfo.HtmlFieldPrefix) ? htmlFieldPrefix : string.Concat(viewData.TemplateInfo.HtmlFieldPrefix, ".", htmlFieldPrefix)
                };
            }
            return helper.Partial(partialViewName, model, viewData);
        }

        /// <summary>
        /// Returns the nominated string resource, localised to the current UI culture
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public static string LocalisedText(this HtmlHelper helper, string classKey, string resourceKey)
        {
            return LocalisedText(helper, classKey, resourceKey, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Returns the nominated string resource, localised to the given culture
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public static string LocalisedText(this HtmlHelper helper, string classKey, string resourceKey, string cultureName)
        {
            return LocalisedText(helper, classKey, resourceKey, new System.Globalization.CultureInfo(cultureName));
        }

        /// <summary>
        /// Returns the nominated string resource, localised to the given culture
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string LocalisedText(this HtmlHelper helper, string classKey, string resourceKey, System.Globalization.CultureInfo culture)
        {
            return helper.ViewContext.HttpContext.GetGlobalResourceObject(classKey, resourceKey, culture) as string;
        }


        public static MvcHtmlString ClientIdFor<TModel, TValue>(this HtmlHelper<TModel> html,
           Expression<Func<TModel, TValue>> expression)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string inputFieldId = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName);
            return MvcHtmlString.Create(inputFieldId);
        }

        public static MvcHtmlString ClientId(this HtmlHelper html, string htmlFieldName)
        {
            string inputFieldId = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName);
            return MvcHtmlString.Create(inputFieldId);
        }

        public static IHtmlString Resource(this HtmlHelper htmlHelper, Func<object, HelperResult> template, string resourceKey)
        {
            resourceKey = string.Format("Resources.{0}", resourceKey);
            if (htmlHelper.ViewContext.HttpContext.Items[resourceKey] != null) ((List<Func<object, HelperResult>>)htmlHelper.ViewContext.HttpContext.Items[resourceKey]).Add(template);
            else htmlHelper.ViewContext.HttpContext.Items[resourceKey] = new List<Func<object, HelperResult>>() { template };

            return new HtmlString(String.Empty);
        }

        public static IHtmlString RenderResources(this HtmlHelper htmlHelper, string resourceKey)
        {
            resourceKey = string.Format("Resources.{0}", resourceKey);
            if (htmlHelper.ViewContext.HttpContext.Items[resourceKey] != null)
            {
                List<Func<object, HelperResult>> Resources = (List<Func<object, HelperResult>>)htmlHelper.ViewContext.HttpContext.Items[resourceKey];

                foreach (var Resource in Resources)
                {
                    if (Resource != null) htmlHelper.ViewContext.Writer.Write(Resource(null));
                }
            }

            return new HtmlString(String.Empty);
        }


    }
}
