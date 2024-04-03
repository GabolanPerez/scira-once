using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Indicador-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsIndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            c_periodo_indicador periodoActual;
            int idPeriodoActual;


            try
            {

                periodoActual = db.c_periodo_indicador.Where(p => p.esta_activo).First();
                idPeriodoActual = periodoActual.id_periodo_indicador;
            }
            catch
            {
                try
                {
                    periodoActual = db.c_periodo_indicador.First();
                    idPeriodoActual = periodoActual.id_periodo_indicador;
                }
                catch
                {
                    periodoActual = null;
                    idPeriodoActual = 0;
                }
            }

            var periodos = GetPeriodos();

            //Filtros
            ViewBag.entidades = (MultiSelectList)DropDown.EntidadesMS();
            ViewBag.usuarios = (MultiSelectList)DropDown.UsuariosMS();
            ViewBag.areas = (MultiSelectList)DropDown.AreasMS();
            ViewBag.periodos = DropDown.PeriodosIndicadores(idPeriodoActual);
            ViewBag.periodosMS = DropDown.PeriodosIndicadoresMS(periodos.Select(p => p.id_periodo_indicador).ToArray());


            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var indicadores = Utilidades.Utilidades.RTCObject(us, db, "c_indicador").Cast<c_indicador>().ToList();

            string[] data = Data(indicadores, periodos, periodoActual);//obtener datos para las gráficas

            ViewBag.data0 = data[0];
            ViewBag.data1 = data[1];
            ViewBag.data2 = data[2];
            ViewBag.data3 = data[3];
            ViewBag.data4 = data[4];

            if (periodoActual != null)
            {
                ViewBag.periodoActual = periodoActual.nb_periodo_indicador;
            }
            else
            {
                ViewBag.periodoActual = "N/A";
                ViewBag.error = "1";
            }

            return View();
        }

        #region Detalle de las barras
        public ActionResult Details(string[] entidades, string[] usuarios, string[] areas, string check1, int tipo, int periodo)
        {
            //tipos 0=alerta;   1=regular;  2=bueno;    3=no calificado; 4=inactivos;
            var list = getFilteredElements(entidades, usuarios, areas, check1);
            var p = db.c_periodo_indicador.Find(periodo);

            var actives = list.Where(i => i.esta_activo).ToList();
            var inactives = list.Where(i => !i.esta_activo).ToList();

            var indicadoresEvaluados = actives.Where(i => i.k_evaluacion.Any(e => e.id_periodo_indicador == p.id_periodo_indicador)).ToList();
            var indicadoresNoEvaluados = actives.Where(i => !i.k_evaluacion.Any(e => e.id_periodo_indicador == p.id_periodo_indicador)).ToList();

            List<c_indicador> resultado0 = new List<c_indicador>();
            List<c_indicador> resultado1 = new List<c_indicador>();
            List<c_indicador> resultado2 = new List<c_indicador>();
            List<c_indicador> resultado3 = new List<c_indicador>();
            List<c_indicador> resultado4 = new List<c_indicador>();

            foreach (var ind in indicadoresEvaluados)
            {
                var evaluacion = ind.k_evaluacion.Where(e => e.id_periodo_indicador == p.id_periodo_indicador).First();
                string calif = "";
                try
                {
                    calif = evaluacion.c_calificacion_indicador.nb_calificacion_indicador;
                }
                catch
                {
                    calif = null;
                }
                switch (calif)
                {
                    case "Alerta":
                        resultado0.Add(ind);
                        break;
                    case "Regular":
                        resultado1.Add(ind);
                        break;
                    case "Bueno":
                        resultado2.Add(ind);
                        break;
                    case null:
                        resultado3.Add(ind);
                        break;
                }
            }
            resultado3.AddRange(indicadoresNoEvaluados);
            resultado4.AddRange(inactives);

            ViewBag.tipo = tipo;
            switch (tipo)
            {
                case 0:
                    ViewBag.label = Strings.getMSG("Alerta");
                    return PartialView(resultado0);
                case 1:
                    ViewBag.label = Strings.getMSG("GraphicsBDEIIndex013");
                    return PartialView(resultado1);
                case 2:
                    ViewBag.label = Strings.getMSG("GraphicsBDEIIndex014");
                    return PartialView(resultado2);
                case 3:
                    ViewBag.label = Strings.getMSG("GraphicsBDEIIndex015");
                    return PartialView(resultado3);
                case 4:
                    ViewBag.label = Strings.getMSG("Inactivos");
                    return PartialView(resultado4);
                default:
                    return null;
            }
        }

        public ActionResult DetailsIndicador(int id_ind, int tipo, int periodo)
        {
            ViewBag.tipo = tipo;
            ViewBag.periodo = db.c_periodo_indicador.Find(periodo);
            switch (tipo)
            {
                case 0:
                    ViewBag.label = Strings.getMSG("Alerta");
                    break;
                case 1:
                    ViewBag.label = Strings.getMSG("GraphicsBDEIIndex013");
                    break;
                case 2:
                    ViewBag.label = Strings.getMSG("GraphicsBDEIIndex014");
                    break;
                case 3:
                    ViewBag.label = Strings.getMSG("GraphicsBDEIIndex015");
                    break;
                case 4:
                    ViewBag.label = Strings.getMSG("Inactivos");
                    break;
                default:
                    break;
            }

            //obtener ultimos 12 (o menos) 

            var periodos = GetPeriodos();

            string dt1 = "";
            string dt2 = "";

            var indicador = db.c_indicador.Find(id_ind);

            foreach (var p in periodos)
            {
                dt1 += "'" + p.nb_periodo_indicador + "', ";
                k_evaluacion evaluacion;
                try
                {
                    evaluacion = indicador.k_evaluacion.Where(e => e.id_periodo_indicador == p.id_periodo_indicador).First();
                }
                catch
                {
                    evaluacion = null;
                }
                if (evaluacion != null)
                {
                    dt2 += evaluacion.medicion.ToString().Replace(',', '.') + ", ";
                }
                else
                {
                    dt2 += "0, ";
                }
            }

            string dataEI = @"
            {
                labels:[" + dt1 + @"],
                datasets:[
                    {label:'Medición',
                    data:[" + dt2 + "]}" +
                    "]" +
            "};";

            ViewBag.dataEI = dataEI;
            ViewBag.lineG = indicador.umbral100f.ToString().Replace(",", ".");
            ViewBag.lineY = indicador.umbral075f.ToString().Replace(",", ".");
            ViewBag.lineR = indicador.umbral000f > indicador.umbral050f ? indicador.umbral000f.ToString().Replace(",", ".") : indicador.umbral050f.ToString().Replace(",", ".");
            return PartialView(indicador);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] entidades, string[] usuarios, string[] areas, string check1, int periodo, string[] ps)
        {
            var list = getFilteredElements(entidades, usuarios, areas, check1);

            var p = db.c_periodo_indicador.Find(periodo);


            string[] data = Data(list, GetPeriodos(ps), p);//obtener datos para las gráficas

            ViewBag.data0 = data[0];
            ViewBag.data1 = data[1];
            ViewBag.data2 = data[2];
            ViewBag.data3 = data[3];
            ViewBag.data4 = data[4];

            return PartialView();
        }


        private List<c_indicador> getFilteredElements(string[] entidades, string[] usuarios, string[] areas, string check1)
        {
            bool ch1 = check1 == "on";

            var model = new List<c_indicador>();
            var modelAx = new List<c_indicador>();

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var indicadores = Utilidades.Utilidades.RTCObject(us, db, "c_indicador").Cast<c_indicador>().ToList();


            if (entidades != null)
            {
                foreach (var id in entidades)
                {
                    int ID = int.Parse(id);
                    model = model.Union(indicadores.Where(i => i.id_entidad == ID)).ToList();
                }
            }

            if (usuarios != null)
            {
                foreach (var id in usuarios)
                {
                    int ID = int.Parse(id);
                    modelAx = modelAx.Union(indicadores.Where(i => i.id_responsable == ID)).ToList();
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



            modelAx = new List<c_indicador>();
            if (areas != null)
            {
                foreach (var id in areas)
                {
                    int ID = int.Parse(id);
                    modelAx = modelAx.Union(indicadores.Where(i => i.id_area == ID)).ToList();
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

            if (entidades == null && usuarios == null && areas == null)
            {
                model = indicadores.ToList();
            }

            return model;
        }
        #endregion

        #region datos
        public string[] Data(List<c_indicador> model, List<c_periodo_indicador> periodos, c_periodo_indicador periodo = null) //genera la cadena con los datos
        {
            string[] resultado = new string[5];

            int totalInd = model.Count();

            //quedarse solo con indicadores activos
            int inactivos = model.Where(i => !i.esta_activo).Count();
            model = model.Where(i => i.esta_activo).ToList();


            int d1 = 0;
            int d2 = 0;
            int d3 = 0;
            int d4 = 0;

            string pr1 = "N/A";
            string pr2 = "N/A";
            string pr3 = "N/A";
            string pr4 = "N/A";
            string pr5 = "N/A";

            int[] cuenta;
            //obtener periodo actual
            if (periodo != null)
            {

                cuenta = getIndicatorsCount(model, periodo);

                d1 = cuenta[0];
                d2 = cuenta[1];
                d3 = cuenta[2];
                d4 = cuenta[3];

                pr1 = string.Format("{0:0.0}", (((double)(d1 * 100)) / (double)totalInd));
                pr2 = string.Format("{0:0.0}", (((double)(d2 * 100)) / (double)totalInd));
                pr3 = string.Format("{0:0.0}", (((double)(d3 * 100)) / (double)totalInd));
                pr4 = string.Format("{0:0.0}", (((double)(d4 * 100)) / (double)totalInd));
                pr5 = string.Format("{0:0.0}", (((double)(inactivos * 100)) / (double)totalInd));
            }


            string dtLabel = "'" + d1 + "-(" + pr1 + "%)','" + d2 + "-(" + pr2 + "%)','" + d3 + "-(" + pr3 + "%)','" + d4 + "-(" + pr4 + "%)','" + inactivos + "-(" + pr5 + "%)'";

            resultado[0] = @"
            {
                labels:['Alerta','Regulares','Buenos','No calificados','Inactivos'],
                datasets:[
                    {label:'Cantidad',
                    backgroundColor:['red','yellow','green','CornflowerBlue','gray'],
                    dataLabels:[" + dtLabel + @"],
                    data:[" + d1 + "," + d2 + "," + d3 + "," + d4 + "," + inactivos + "]}" +
                    "]" +
            "};";

            string dt1 = "";
            string dt2 = "";
            string dt3 = "";
            string dt4 = "";
            string dt5 = "";

            //var periodos = db.c_periodo_indicador.OrderBy(p => p.id_periodo_indicador).ToList();

            foreach (var p in periodos)
            {
                dt1 += "'" + p.nb_periodo_indicador + "',";
                cuenta = getIndicatorsCount(model, p);
                dt2 += cuenta[0] + ",";
                dt3 += cuenta[1] + ",";
                dt4 += cuenta[2] + ",";
                dt5 += cuenta[3] + ",";
            }


            resultado[1] = @"
            {
                labels:[" + dt1 + @"],
                datasets:[
                    {label:'Cantidad',
                    data:[" + dt2 + "]}" +
                    "]" +
            "};";


            resultado[2] = @"
            {
                labels:[" + dt1 + @"],
                datasets:[
                    {label:'Cantidad',
                    data:[" + dt3 + "]}" +
                    "]" +
            "};";

            resultado[3] = @"
            {
                labels:[" + dt1 + @"],
                datasets:[
                    {label:'Cantidad',
                    data:[" + dt4 + "]}" +
                    "]" +
            "};";

            resultado[4] = @"
            {
                labels:[" + dt1 + @"],
                datasets:[
                    {label:'Cantidad',
                    data:[" + dt5 + "]}" +
                    "]" +
            "};";


            return resultado;
        }


        //Cuenta de Evaluaciones de indicadores por periodo
        private int[] getIndicatorsCount(List<c_indicador> model, c_periodo_indicador periodo)
        {
            int[] resultado = new int[4];

            var indicadoresEvaluados = model.Where(i => i.k_evaluacion.Any(e => e.id_periodo_indicador == periodo.id_periodo_indicador)).ToList();
            var indicadoresNoEvaluados = model.Where(i => !i.k_evaluacion.Any(e => e.id_periodo_indicador == periodo.id_periodo_indicador)).ToList();

            resultado[0] = 0;
            resultado[1] = 0;
            resultado[2] = 0;
            resultado[3] = 0;

            foreach (var ind in indicadoresEvaluados)
            {
                var evaluacion = ind.k_evaluacion.Where(e => e.id_periodo_indicador == periodo.id_periodo_indicador).First();
                string calif = "";
                try
                {
                    calif = evaluacion.c_calificacion_indicador.nb_calificacion_indicador;
                }
                catch
                {
                    calif = null;
                }
                switch (calif)
                {
                    case "Alerta":
                        resultado[0]++;
                        break;
                    case "Regular":
                        resultado[1]++;
                        break;
                    case "Bueno":
                        resultado[2]++;
                        break;
                    case null:
                        resultado[3]++;
                        break;
                }
            }

            resultado[3] += indicadoresNoEvaluados.Count;
            return resultado;
        }

        #endregion

        #region Auxiliares
        private List<c_periodo_indicador> GetPeriodos(string[] ids = null)
        {
            List<c_periodo_indicador> periodos;

            if (ids == null)
            {
                periodos = db.c_periodo_indicador.OrderBy(p => p.id_periodo_indicador).ToList();

                if (periodos.Count > 12)
                {
                    var paux = new List<c_periodo_indicador>();
                    int lastIndex = periodos.Count;
                    for (int i = 12; i > 0; i--)
                    {
                        paux.Add(periodos.ElementAt(lastIndex - i));
                    }
                    periodos = paux;
                }
            }
            else
            {
                periodos = new List<c_periodo_indicador>();
                foreach (var id in ids)
                {
                    periodos.Add(db.c_periodo_indicador.Find(int.Parse(id)));
                }
            }


            return periodos;
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
