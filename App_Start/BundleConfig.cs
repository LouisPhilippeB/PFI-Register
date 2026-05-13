using System.Web.Optimization;

namespace Wikimedia
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                            "~/Scripts/validation.js",
                            "~/Scripts/jquery-maskedinput.js",
                            "~/Scripts/bootbox.js",
                            "~/Scripts/selections.js",
                            "~/Scripts/SiteScripts.js",
                            "~/Scripts/pageManager.js",
                            "~/Scripts/session.js",
                            "~/Scripts/SiteNotificationsHandler.js",
                            "~/Scripts/autoRefreshPanel.js",
                            "~/Scripts/image-control.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/_layout.css",
                        "~/Content/Accounts.css",
                        "~/Content/popup.css",
                        "~/Content/Selections.css",
                        "~/Content/site.css",
                        "~/Content/menu.css",
                        "~/Content/Students.css",
                        "~/Content/Courses.css",
                        "~/Content/Teachers.css",
                        "~/Content/Icons.css",
                        "~/Content/image-control.css",
                        "~/Content/jqui-custom-datepicker.css"));
        }
    }
}