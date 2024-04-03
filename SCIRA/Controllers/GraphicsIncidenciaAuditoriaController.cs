using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [AccessAudit(Funcion = "InAud-Stats")]
    [CustomErrorHandler]
    public class GraphicsIncidenciaAuditoriaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            ViewBag.informes = (MultiSelectList)Utilidades.DropDown.InformesAuditoriaMS();

            var programasTrabajo = db.k_programa_trabajo.Where(p => p.tiene_incidencia && !p.esta_atendida).ToList();


            ViewBag.data0 = Data(programasTrabajo);

            return View();
        }

        #region otros

        #endregion

        #region Detalle de las barras
        public ActionResult Details(string[] informes,int id_criticidad)
        {
            var model = getFilteredElements(informes);

            model = model.Where(p => p.id_criticidad_programa_trabajo == id_criticidad).ToList();

            ViewBag.criticidad = db.c_criticidad_programa_trabajo.Find(id_criticidad).nb_criticidad_programa_trabajo;

            return PartialView("Details", model);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] informes)
        {
            var model = getFilteredElements(informes);

            ViewBag.data0 = Data(model);

            return PartialView();
        }


        private List<k_programa_trabajo> getFilteredElements(string[] informes)
        {
            List<k_programa_trabajo> model = db.k_programa_trabajo.Where(p => p.tiene_incidencia && !p.esta_atendida).ToList();

            List<k_programa_trabajo> res = new List<k_programa_trabajo>();
            //Obtener lista de elementos al filtrar por entidades
            if (informes != null)
            {
                foreach (var id in informes)
                {
                    int ID = int.Parse(id);

                    res = res.Union(model.Where(p => p.idd_auditoria == ID)).ToList();
                }
            }



            if (informes == null)
            {
                res = model;
            }

            return res;
        }


        #endregion

        #region Datos
        public string Data(List<k_programa_trabajo> programas_Trabajo)
        {
            var criticidades = db.c_criticidad_programa_trabajo.ToList();


            List<double> DataAux = new List<double>();
            List<string> LabelsAux = new List<string>();
            List<int> Ids = new List<int>();

            foreach(var criticidad in criticidades)
            {
                //Añadir el conteo de registros que cumplen la condición
                DataAux.Add(programas_Trabajo.Where(p => p.id_criticidad_programa_trabajo == criticidad.id_criticidad_programa_trabajo).Count());

                //Añadir la etiqueta de la barra
                LabelsAux.Add(criticidad.nb_criticidad_programa_trabajo);

                //Añadir los ids a los datos
                Ids.Add(criticidad.id_criticidad_programa_trabajo);
            }

            double[] data = DataAux.ToArray();

            string[] bgc = Utilidades.Utilidades.getColorArray(criticidades.Count());

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Incidencias"), 1, data, bgc, null);

            string[] labels = LabelsAux.ToArray();
            GraphicsViewModel.Dataset[] datasets = { dataset };

            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets,
                ids = Ids.ToArray()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }
        #endregion



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
