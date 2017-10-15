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
    public partial class CalloutController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected CalloutController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult MoodMap()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodMap);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult GetMoodMapImage()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetMoodMapImage);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.JsonResult GetGlobalMoodReportData()
        {
            return new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.GetGlobalMoodReportData);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult MoodThermometer()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodThermometer);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult MoodBreakdown()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodBreakdown);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult MoodTower()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodTower);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public CalloutController Actions { get { return MVC.Callout; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Callout";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Callout";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string MoodMap = "MoodMap";
            public readonly string GetMoodMapImage = "GetMoodMapImage";
            public readonly string GetGlobalMoodReportData = "GetGlobalMoodReportData";
            public readonly string MoodThermometer = "MoodThermometer";
            public readonly string MoodBreakdown = "MoodBreakdown";
            public readonly string MoodTower = "MoodTower";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string MoodMap = "MoodMap";
            public const string GetMoodMapImage = "GetMoodMapImage";
            public const string GetGlobalMoodReportData = "GetGlobalMoodReportData";
            public const string MoodThermometer = "MoodThermometer";
            public const string MoodBreakdown = "MoodBreakdown";
            public const string MoodTower = "MoodTower";
        }


        static readonly ActionParamsClass_MoodMap s_params_MoodMap = new ActionParamsClass_MoodMap();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MoodMap MoodMapParams { get { return s_params_MoodMap; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MoodMap
        {
            public readonly string EventName = "EventName";
            public readonly string request = "request";
        }
        static readonly ActionParamsClass_GetMoodMapImage s_params_GetMoodMapImage = new ActionParamsClass_GetMoodMapImage();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetMoodMapImage GetMoodMapImageParams { get { return s_params_GetMoodMapImage; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetMoodMapImage
        {
            public readonly string EventName = "EventName";
            public readonly string Category = "Category";
            public readonly string ReportStart = "ReportStart";
            public readonly string ReportEnd = "ReportEnd";
            public readonly string MoodIsStaleMins = "MoodIsStaleMins";
            public readonly string Tension = "Tension";
            public readonly string ShowDataPoints = "ShowDataPoints";
            public readonly string Width = "Width";
            public readonly string Height = "Height";
        }
        static readonly ActionParamsClass_GetGlobalMoodReportData s_params_GetGlobalMoodReportData = new ActionParamsClass_GetGlobalMoodReportData();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetGlobalMoodReportData GetGlobalMoodReportDataParams { get { return s_params_GetGlobalMoodReportData; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetGlobalMoodReportData
        {
            public readonly string EventName = "EventName";
            public readonly string request = "request";
        }
        static readonly ActionParamsClass_MoodThermometer s_params_MoodThermometer = new ActionParamsClass_MoodThermometer();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MoodThermometer MoodThermometerParams { get { return s_params_MoodThermometer; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MoodThermometer
        {
            public readonly string EventName = "EventName";
            public readonly string request = "request";
        }
        static readonly ActionParamsClass_MoodBreakdown s_params_MoodBreakdown = new ActionParamsClass_MoodBreakdown();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MoodBreakdown MoodBreakdownParams { get { return s_params_MoodBreakdown; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MoodBreakdown
        {
            public readonly string EventName = "EventName";
            public readonly string request = "request";
        }
        static readonly ActionParamsClass_MoodTower s_params_MoodTower = new ActionParamsClass_MoodTower();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MoodTower MoodTowerParams { get { return s_params_MoodTower; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MoodTower
        {
            public readonly string EventName = "EventName";
            public readonly string request = "request";
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
                public readonly string GetMoodSnapshotData = "GetMoodSnapshotData";
                public readonly string MoodBreakdown = "MoodBreakdown";
                public readonly string MoodMap = "MoodMap";
                public readonly string MoodSnapshot = "MoodSnapshot";
                public readonly string MoodThermometer = "MoodThermometer";
                public readonly string MoodTower = "MoodTower";
            }
            public readonly string GetMoodSnapshotData = "~/Views/Callout/GetMoodSnapshotData.cshtml";
            public readonly string MoodBreakdown = "~/Views/Callout/MoodBreakdown.cshtml";
            public readonly string MoodMap = "~/Views/Callout/MoodMap.cshtml";
            public readonly string MoodSnapshot = "~/Views/Callout/MoodSnapshot.cshtml";
            public readonly string MoodThermometer = "~/Views/Callout/MoodThermometer.cshtml";
            public readonly string MoodTower = "~/Views/Callout/MoodTower.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_CalloutController : MyMood.Web.Controllers.CalloutController
    {
        public T4MVC_CalloutController() : base(Dummy.Instance) { }

        partial void MoodMapOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, MyMood.Web.Models.GlobalMoodReportRequestModel request);

        public override System.Web.Mvc.ActionResult MoodMap(string EventName, MyMood.Web.Models.GlobalMoodReportRequestModel request)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodMap);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "request", request);
            MoodMapOverride(callInfo, EventName, request);
            return callInfo;
        }

        partial void GetMoodMapImageOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, string Category, System.DateTime? ReportStart, System.DateTime? ReportEnd, int? MoodIsStaleMins, float? Tension, bool? ShowDataPoints, int? Width, int? Height);

        public override System.Web.Mvc.ActionResult GetMoodMapImage(string EventName, string Category, System.DateTime? ReportStart, System.DateTime? ReportEnd, int? MoodIsStaleMins, float? Tension, bool? ShowDataPoints, int? Width, int? Height)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetMoodMapImage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Category", Category);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ReportStart", ReportStart);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ReportEnd", ReportEnd);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "MoodIsStaleMins", MoodIsStaleMins);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Tension", Tension);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "ShowDataPoints", ShowDataPoints);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Width", Width);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "Height", Height);
            GetMoodMapImageOverride(callInfo, EventName, Category, ReportStart, ReportEnd, MoodIsStaleMins, Tension, ShowDataPoints, Width, Height);
            return callInfo;
        }

        partial void GetGlobalMoodReportDataOverride(T4MVC_System_Web_Mvc_JsonResult callInfo, string EventName, MyMood.Web.Models.GlobalMoodReportRequestModel request);

        public override System.Web.Mvc.JsonResult GetGlobalMoodReportData(string EventName, MyMood.Web.Models.GlobalMoodReportRequestModel request)
        {
            var callInfo = new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.GetGlobalMoodReportData);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "request", request);
            GetGlobalMoodReportDataOverride(callInfo, EventName, request);
            return callInfo;
        }

        partial void MoodThermometerOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, MyMood.Web.Models.MoodSnapshotRequestModel request);

        public override System.Web.Mvc.ActionResult MoodThermometer(string EventName, MyMood.Web.Models.MoodSnapshotRequestModel request)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodThermometer);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "request", request);
            MoodThermometerOverride(callInfo, EventName, request);
            return callInfo;
        }

        partial void MoodBreakdownOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, MyMood.Web.Models.MoodSnapshotRequestModel request);

        public override System.Web.Mvc.ActionResult MoodBreakdown(string EventName, MyMood.Web.Models.MoodSnapshotRequestModel request)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodBreakdown);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "request", request);
            MoodBreakdownOverride(callInfo, EventName, request);
            return callInfo;
        }

        partial void MoodTowerOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string EventName, MyMood.Web.Models.MoodSnapshotRequestModel request);

        public override System.Web.Mvc.ActionResult MoodTower(string EventName, MyMood.Web.Models.MoodSnapshotRequestModel request)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoodTower);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "EventName", EventName);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "request", request);
            MoodTowerOverride(callInfo, EventName, request);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
