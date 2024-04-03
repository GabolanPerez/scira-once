using System.Web.Optimization;

namespace SCIRA
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.2.4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/general").Include(
                      "~/Scripts/respond.js",
                      "~/Scripts/popper.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/DataTables/jquery.dataTables.min.js",
                      "~/Scripts/DataTables/dataTables.bootstrap.min.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/sharedScripts.js",
                      "~/Scripts/jquery.signalR-2.3.0.min.js",
                      "~/Scripts/dropzone.js",
                      "~/Scripts/kinetic.min.js",       //Scripts para los tutoriales
                      "~/Scripts/enjoyhint.js",         //Scripts para los tutoriales
                      "~/Scripts/Tutorial.js",         //Scripts para los tutoriales
                      "~/signalr/hubs"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Extras.css",
                      "~/Content/bootstrap-simplex.css",
                      "~/Content/AnimacionCarga.css",
                      "~/Content/toastr.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/AddedContent.css",
                      "~/Content/Site.css",
                      "~/Content/popper.css",
                      "~/Content/dropzone.css",
                      "~/Content/enjoyhint.css",        //Estilo para los tutoriales
                      "~/Content/DataTables/css/dataTables.bootstrap.min.css"));
        }
    }
}
