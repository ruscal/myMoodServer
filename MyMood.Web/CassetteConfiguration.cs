using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;
using System.Linq;

namespace MyMood.Web
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            var discoverCore = typeof(Discover.Web.Mvc.HtmlHelperExtensions).Assembly;

            bundles.Add<StylesheetBundle>("Styles/install", new string[]
            {
                "~/Content/css/install.css"
            });

            bundles.Add<StylesheetBundle>("Styles/report", new string[]
            {
                "~/Content/css/report.css"
            });

            bundles.Add<StylesheetBundle>("Styles/app", new string[]
            {
                "~/Content/CSS/app.css",
                "~/Content/CSS/jquery-ui.css"
            },
            b => 
            {
                b.Assets.Insert(0, new ResourceAsset("Discover.Web.Css.reset.css", discoverCore));
                b.Assets.Insert(1, new ResourceAsset("Discover.Web.Css.common.css", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Css.jquery.qtip.css", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Css.jquery.dropdownchecklist.css", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Css.forms.css", discoverCore));
            });

            bundles.Add<ScriptBundle>("Scripts/common", new string[]
            {
                "~/Scripts/moment.js",
                "~/Scripts/common.js"
            }
            , b =>
            {
                b.Assets.Insert(0, new ResourceAsset("Discover.Web.Scripts.ms.strings.js", discoverCore));
                b.Assets.Insert(2, new ResourceAsset("Discover.Web.Scripts.discover.common.js", discoverCore));
            });

            bundles.Add<ScriptBundle>("Scripts/jquery", Enumerable.Empty<string>(), b =>
            {
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.json.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.defaults.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.iframe-transport.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.serialization.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.linq.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.cookie.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.caret.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.placeholder.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.qtip.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.flashmessage.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.validate.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.validate.unobtrusive.js", discoverCore));
            });

            bundles.Add<ScriptBundle>("Scripts/ui", Enumerable.Empty<string>(), b =>
            {
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery-ui.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery-ui.defaults.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.dropdownchecklist.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.rateit.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.datatables.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.datatables.extensions.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.jquery.htmlarea.js", discoverCore));
            });

            bundles.Add<ScriptBundle>("Scripts/html5", Enumerable.Empty<string>(), b =>
            {
                b.Condition = "lte IE 8";
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.html5.js", discoverCore));
                b.Assets.Add(new ResourceAsset("Discover.Web.Scripts.excanvas.js", discoverCore));
            });

            bundles.Add<ScriptBundle>("Scripts/callouts", new string[]
            {
                "~/Scripts/fabric.js",
                "~/Scripts/mood.breakdown.js",
                "~/Scripts/mood.meter.js",
                "~/Scripts/mood.tower.js",
                "~/Scripts/mood.map.js"
            }, 
            b =>
            {
                b.Assets.Insert(0, new ResourceAsset("Discover.Web.Scripts.ms.strings.js", discoverCore));
            });
        }
    }
}