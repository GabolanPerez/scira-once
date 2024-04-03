using Hangfire;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(SCIRA.Startup))]
namespace SCIRA
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();


            //Configuración de HangFire
            //GlobalConfiguration.Configuration.UseSqlServerStorage(Utilidades.Utilidades.GetConnectionString());
            //Ahora se realiza en hangfirebootstraper


            app.UseHangfireDashboard();
            app.UseHangfireServer();

            //Inicializar tarea de notificar pendientes
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(path);
            var mensajes = doc.SelectSingleNode("messages");
            System.Xml.XmlNode mensaje;
            mensaje = mensajes.SelectSingleNode("message7");
            var freq = mensaje.SelectSingleNode("freq").InnerText;
            var frecuencias  = Utilidades.DropDown.FrecuenciasMensajes(freq);

            foreach (var item in frecuencias)
            {
                if (item.Text == freq) {
                    Utilidades.Utilidades.SetPendingListFrequency(item.Value, item.Text);
                }
            }


            



            //Inicializar tareas recurrentes
            RecurringJob.AddOrUpdate("RepHistory", () => Utilidades.Utilidades.BackUpReports(), "0 0 1 * *", TimeZoneInfo.FindSystemTimeZoneById(Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id))); // Realizar respaldo de los reportes cada primero de mes
            RecurringJob.AddOrUpdate("ClearParams", () => Utilidades.Utilidades.ClearParams(), "0 0 ? * 1/7", TimeZoneInfo.FindSystemTimeZoneById(Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id))); // Eliminar basura de c_parametro
            RecurringJob.AddOrUpdate("DelOldFiles", () => Utilidades.Utilidades.deleteOldFiles(), "0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById(Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id))); // Eliminar archivos viejos sin ligas en el sistema
            RecurringJob.AddOrUpdate("VerifyIntegrity", () => Utilidades.Utilidades.VerifyIntegrity(), "0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById(Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id))); // Eliminar archivos viejos sin ligas en el sistema
            RecurringJob.AddOrUpdate("DailyIndicators", () => Utilidades.Utilidades.CreateDailyEvaluationRegisters(), "0 0 * * *", TimeZoneInfo.FindSystemTimeZoneById(Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id))); // Crear registros de evaluacion de los indicadores diarios


            //veriticar folders necesarios e integridad de datos
            //Utilidades.Utilidades.VerifyFolders();
            //Utilidades.Utilidades.VerifyIntegrity();
            //Utilidades.Utilidades.deleteOldFiles();

            //Establecer si se usa ActiveDirectory
            Utilidades.Globals.StartOnActiveDIrectory = System.Configuration.ConfigurationManager.AppSettings["ActiveDirectory"] == "true";
        }
    }
}