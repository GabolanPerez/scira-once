using Newtonsoft.Json.Linq;
using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepHist", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class HistorialReportesController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var path = Server.MapPath("~/App_Data/HistorialReportes/");
            var rutas = Directory.GetDirectories(path).ToList();
            List<string> carpetas = new List<string>();

            foreach (var ruta in rutas)
            {
                carpetas.Add(ruta.Split(new char[] { '\\' }).Last());
            }


            return View(carpetas);
        }

        public ActionResult ReportList(string folder)
        {
            var path = Server.MapPath("~/App_Data/HistorialReportes/" + folder);
            var files = Directory.GetFiles(path).ToList();

            //List<string> ReportNames = new List<string>();
            List<HistoricReportViewModel> model = new List<HistoricReportViewModel>();


            foreach (var file in files)
            {
                StreamReader reader = new StreamReader(file);
                string repString = reader.ReadToEnd();

                try
                {
                    JObject repJson = JObject.Parse(repString);
                    JProperty repName = repJson.Property("RepName");

                    //ReportNames.Add(repName.Value.ToString());

                    var historicReport = new HistoricReportViewModel()
                    {
                        file_name = file.Split(new char[] { '\\' }).Last(),
                        nb_reporte = repName.Value.ToString()
                    };

                    model.Add(historicReport);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }


            ViewBag.folder = folder;

            //return View(ReportNames);
            return View(model);
        }

        public ActionResult ShowRep(string repName, string folder)
        {
            var path = Server.MapPath("~/App_Data/HistorialReportes/" + folder + "/" + repName);

            StreamReader file = new StreamReader(path);
            string clearText = file.ReadToEnd();
            JObject Json = JObject.Parse(clearText);

            ViewBag.folder = folder;

            return View(Json);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
