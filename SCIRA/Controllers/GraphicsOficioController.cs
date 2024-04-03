    using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Oficio-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsOficioController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            //Filtros
            ViewBag.entidades = (MultiSelectList)Utilidades.DropDown.EntidadesMS();
            ViewBag.usuarios = (MultiSelectList)Utilidades.DropDown.UsuariosMS();
            ViewBag.origenAutoridad = (MultiSelectList)Utilidades.DropDown.OrigenAutoridadMS();

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var objetos = Utilidades.Utilidades.RTCObject(us, db, "k_objeto").Cast<k_objeto>().ToList();

            string[] data = Data(objetos.Where(o => o.tipo_objeto == 1 || o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList());//obtener datos para las gráficas



            ViewBag.data0 = data[0];
            ViewBag.data1 = data[1];

            return View();
        }

        #region Detalle de las barras
        public ActionResult Details(string[] entidades, string[] usuarios, string[] origenAutoridad, string check1, int tipoG1)
        {
            //tipos 0=Por Vencer;   1=Vencidos;  2=Contestados;    3=Recibidos;
            var list = getFilteredElements(entidades, usuarios, origenAutoridad, check1);

            var OfC = list.Where(o => o.fe_contestacion != null).ToList();
            var OfNC = list.Where(o => o.fe_contestacion == null).ToList();

            var OfVen = OfNC.Where(o => o.fe_vencimiento <= DateTime.Now).ToList();
            var OfVig = OfNC.Where(o => o.fe_vencimiento > DateTime.Now || o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();


            switch (tipoG1)
            {
                case 0: //Por Vencer
                    return PartialView(OfVig);
                case 1: //Vencidos
                    return PartialView(OfVen);
                case 2: //Contestados
                    return PartialView(OfC);
                case 3: //Por Vencer
                    return PartialView(list);
                default:
                    return null;
            }
        }

        public ActionResult Details2(string[] entidades, string[] usuarios, string[] origenAutoridad, string check1, int tipoG2, int clasG2)
        {
            //tipos 0=Contestados;   1=Vencidos;  2=Por Vencer;
            var list = getFilteredElements(entidades, usuarios, origenAutoridad, check1);

            var OfC = list.Where(o => o.fe_contestacion != null).ToList();
            var OfNC = list.Where(o => o.fe_contestacion == null).ToList();

            var OfVen = OfNC.Where(o => o.fe_vencimiento <= DateTime.Now).ToList();
            var OfVig = OfNC.Where(o => o.fe_vencimiento > DateTime.Now || o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();


            switch (tipoG2)
            {
                case 0: //Contestados
                    list = OfC;
                    break;
                case 1: //Vencidos
                    list = OfVen;
                    break;
                case 2: //Por Vencer
                    list = OfVig;
                    break;
                default:
                    return null;
            }

            List<k_incidencia> incidencias = new List<k_incidencia>();

            foreach (var oficio in list)
            {
                incidencias.AddRange(oficio.k_incidencia.Where(i => i.id_clasificacion_incidencia == clasG2).ToList());
            }

            return PartialView(incidencias);
        }


        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] entidades, string[] usuarios, string[] origenAutoridad, string check1)
        {
            var list = getFilteredElements(entidades, usuarios, origenAutoridad, check1);

            string[] data = Data(list);//obtener datos para las gráficas

            ViewBag.data0 = data[0];
            ViewBag.data1 = data[1];

            return PartialView();
        }


        private List<k_objeto> getFilteredElements(string[] entidades, string[] usuarios, string[] origenAutoridad, string check1)
        {
            bool ch1 = check1 == "on";

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var objetos = Utilidades.Utilidades.RTCObject(us, db, "k_objeto").Cast<k_objeto>().ToList();

            var oficios = objetos.Where(o => o.tipo_objeto == 1 || o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();

            var model = new List<k_objeto>();
            var modelAx = new List<k_objeto>();


            if (entidades != null)
            {
                foreach (var id in entidades)
                {
                    int ID = int.Parse(id);
                    model = model.Union(oficios.Where(i => i.id_entidad == ID)).ToList();
                }
            }

            if (usuarios != null)
            {
                foreach (var id in usuarios)
                {
                    int ID = int.Parse(id);
                    modelAx = modelAx.Union(oficios.Where(i => i.id_responsable == ID)).ToList();
                }
                if (ch1) //unimos
                {
                    model = model.Union(modelAx).ToList();
                }
                else if (entidades != null)//intersectamos
                {
                    model = model.Intersect(modelAx).ToList();
                }
                else
                {
                    model = modelAx;
                }
            }

            modelAx = new List<k_objeto>();
            if (origenAutoridad != null)
            {
                foreach (var id in origenAutoridad)
                {
                    int ID = int.Parse(id);
                    modelAx = modelAx.Union(oficios.Where(i => i.id_autoridad == ID)).ToList();
                }

                if (ch1) //unimos
                {
                    model = model.Union(modelAx).ToList();
                }
                else if (entidades != null || usuarios != null)//intersectamos
                {
                    model = model.Intersect(modelAx).ToList();
                }
                else
                {
                    model = modelAx;
                }
            }

            if (entidades == null && usuarios == null && origenAutoridad == null)
            {
                model = oficios;
            }

            return model;
        }
        #endregion

        #region datos
        public string[] Data(List<k_objeto> model) //genera la cadena con los datos para las criticidades
        {
            string[] resultado = new string[2];

            int total = model.Count();

            var OfC = model.Where(o => o.fe_contestacion != null).ToList(); //Contestados
            var OfNC = model.Where(o => o.fe_contestacion == null).ToList(); //No contestados

            var OfVen = OfNC.Where(o => o.fe_vencimiento <= DateTime.Now).ToList(); //No contestados Vencidos
            //En los vigentes se agregaran por defecto los informes
            var OfVig = OfNC.Where(o => o.fe_vencimiento > DateTime.Now || o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList(); //No contestados Vigentes


            int d1 = OfVig.Count;
            int d2 = OfVen.Count;
            int d3 = OfC.Count;
            int d4 = total;

            string pr1 = string.Format("{0:0.0}", (((double)(d1 * 100)) / (double)total));
            string pr2 = string.Format("{0:0.0}", (((double)(d2 * 100)) / (double)total));
            string pr3 = string.Format("{0:0.0}", (((double)(d3 * 100)) / (double)total));
            string pr4 = "100";

            string dtLabel = "'" + d1 + "-(" + pr1 + "%)','" + d2 + "-(" + pr2 + "%)','" + d3 + "-(" + pr3 + "%)','" + d4 + "-(" + pr4 + "%)'";

            resultado[0] = @"
            {
                labels:['Oficios/Informes Por Vencer','Oficios/Informes Vencidos','Oficios/Informes Contestados','Oficios/Informes Recibidos'],
                datasets:[
                    {label:'Cantidad',
                    backgroundColor:['gold','red','green','CornflowerBlue'],
                    dataLabels:[" + dtLabel + @"],
                    data:[" + d1 + "," + d2 + "," + d3 + "," + d4 + "]}" +
                    "]" +
            "};";

            var Clasif = db.c_clasificacion_incidencia.ToList();

            List<k_incidencia> incC = new List<k_incidencia>();
            List<k_incidencia> incVen = new List<k_incidencia>();
            List<k_incidencia> incVig = new List<k_incidencia>();


            foreach (var oficio in OfC)
            {
                incC.AddRange(oficio.k_incidencia.ToList());
            }

            foreach (var oficio in OfVig)
            {
                incVig.AddRange(oficio.k_incidencia.ToList());
            }

            foreach (var oficio in OfVen)
            {
                incVen.AddRange(oficio.k_incidencia.ToList());
            }

            string dataSets = "";

            foreach (var clas in Clasif)
            {
                d1 = incC.Where(i => i.id_clasificacion_incidencia == clas.id_clasificacion_incidencia).Count();
                d2 = incVen.Where(i => i.id_clasificacion_incidencia == clas.id_clasificacion_incidencia).Count();
                d3 = incVig.Where(i => i.id_clasificacion_incidencia == clas.id_clasificacion_incidencia).Count();

                dataSets += "{ label: '" + clas.nb_clasificacion_incidencia + "',id: '" + clas.id_clasificacion_incidencia + "', backgroundColor: '" + clas.color + "',data:[" + d1 + ", " + d2 + ", " + d3 + "]},";
            }

            resultado[1] = @"
            {
                labels:['En Oficios/Informes Contestados','En Oficios/Informes Vencidos','En Oficios/Informes Por Vencer'],
                datasets:[" + dataSets + @"]
            };";

            return resultado;
        }


        ////Cuenta de Evaluaciones de indicadores por periodo
        //private int[] getIndicatorsCount(List<c_indicador> model, c_periodo_indicador periodo)
        //{
        //    int[] resultado = new int[4];

        //    var indicadoresEvaluados = model.Where(i => i.k_evaluacion.Any(e => e.id_periodo_indicador == periodo.id_periodo_indicador)).ToList();
        //    var indicadoresNoEvaluados = model.Where(i => !i.k_evaluacion.Any(e => e.id_periodo_indicador == periodo.id_periodo_indicador)).ToList();

        //    resultado[0] = 0;
        //    resultado[1] = 0;
        //    resultado[2] = 0;
        //    resultado[3] = 0;

        //    foreach (var ind in indicadoresEvaluados)
        //    {
        //        var evaluacion = ind.k_evaluacion.Where(e => e.id_periodo_indicador == periodo.id_periodo_indicador).First();
        //        string calif = "";
        //        try
        //        {
        //            calif = evaluacion.c_calificacion_indicador.nb_calificacion_indicador;
        //        }
        //        catch
        //        {
        //            calif = null;
        //        }
        //        switch (calif)
        //        {
        //            case "Alerta":
        //                resultado[0]++;
        //                break;
        //            case "Regular":
        //                resultado[1]++;
        //                break;
        //            case "Bueno":
        //                resultado[2]++;
        //                break;
        //            case null:
        //                resultado[3]++;
        //                break;
        //        }
        //    }

        //    resultado[3] += indicadoresNoEvaluados.Count;
        //    return resultado;
        //}

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
