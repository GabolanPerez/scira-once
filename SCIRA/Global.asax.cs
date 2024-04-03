using SCIRA.Seguridad;
using SCIRA.Utilidades;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SCIRA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

            //Configurar syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDM5NzA1QDMxMzkyZTMxMmUzMEMxeVZzNytpbTFsQi9QTFY2ZDdSOGNlWVYxd0RLcytCd2VzZjVNL2NBTzQ9");
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ0S0J+XE9BdFRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TdUVlW35bcnFVR2VaUA==");
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ORg4AjUWIQA/Gnt2VVhkQlFac11JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkZjX31fdXJVRGFbV0w=");

            //Iniciar Hangfire
            HangfireBootstrapper.Instance.Start();

            Application["ConnectionString"] = Utilidades.Utilidades.GetConnectionString();
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                var identity = new IdentityPersonalizado(HttpContext.Current.User.Identity);
                var principal = new PrincipalPersonalizado(identity);
                HttpContext.Current.User = principal;
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //terminar hangfire
            HangfireBootstrapper.Instance.Stop();
        }
    }
}
