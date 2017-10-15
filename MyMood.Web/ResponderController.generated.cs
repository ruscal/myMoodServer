// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace MyMood.Web.Controllers
{
    public partial class ResponderController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ResponderController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult PersonalMoodReport()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PersonalMoodReport);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult PersonalMoodReportPdf()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PersonalMoodReportPdf);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ResponderController Actions { get { return MVC.Responder; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Responder";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Responder";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string PersonalMoodReport = "PersonalMoodReport";
            public readonly string PersonalMoodReportPdf = "PersonalMoodReportPdf";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string PersonalMoodReport = "PersonalMoodReport";
            public const string PersonalMoodReportPdf = "PersonalMoodReportPdf";
        }


        static readonly ActionParamsClass_PersonalMoodReport s_params_PersonalMoodReport = new ActionParamsClass_PersonalMoodReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_PersonalMoodReport PersonalMoodReportParams { get { return s_params_PersonalMoodReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_PersonalMoodReport
        {
            public readonly string EventName = "EventName";
            public readonly string responderId = "responderId";
        }
        static readonly ActionParamsClass_PersonalMoodReportPdf s_params_PersonalMoodReportPdf = new ActionParamsClass_PersonalMoodReportPdf();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_PersonalMoodReportPdf PersonalMoodReportPdfParams { get { return s_params_PersonalMoodReportPdf; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_PersonalMoodReportPdf
        {
            public readonly string EventName = "EventName";
            public readonly string responderId = "responderId";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
            }
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_ResponderController : MyMood.Web.Controllers.ResponderController
    {
        public T4MVC_ResponderController() : base(Dummy.Instance) { }

        partial void PersonalMoodReportOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, System.Guid responderId);

        public override System.Web.Mvc.ActionResult PersonalMoodReport(string EventName, System.Guid responderId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PersonalMoodReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "responderId", responderId);
            PersonalMoodReportOverride(callInfo, EventName, responderId);
            return callInfo;
        }

        partial void PersonalMoodReportPdfOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, System.Guid responderId);

        public override System.Web.Mvc.ActionResult PersonalMoodReportPdf(string EventName, System.Guid responderId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.PersonalMoodReportPdf);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "responderId", responderId);
            PersonalMoodReportPdfOverride(callInfo, EventName, responderId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
