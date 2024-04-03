using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "INDD-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsIndicadorDiarioController : Controller
    {
        private SICIEntities db = new SICIEntities();

        #region Index
        public ActionResult Index()
        {
            var fe_actual = DateTime.Now;
            var fe_inicial = fe_actual.AddDays(-30);


            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var evaluaciones = Utilidades.Utilidades.RTCObject(us, db, "k_evaluacion_diaria").Cast<k_evaluacion_diaria>().ToList();

            ViewBag.d1 = Datos(1, fe_inicial, fe_actual, evaluaciones);
            ViewBag.d2 = Datos(2, fe_inicial, fe_actual, evaluaciones);
            ViewBag.d3 = Datos(3, fe_inicial, fe_actual, evaluaciones);
            ViewBag.d4 = Datos(4, fe_inicial, fe_actual, evaluaciones);

            ViewBag.areas = Utilidades.DropDown.AreasMS();
            ViewBag.usuarios = Utilidades.DropDown.UsuariosPorAreaMS();
            ViewBag.indicadores = Utilidades.DropDown.IndicadoresDiariosMS();

            return View();
        }

        #endregion

        #region Filtros

        public ActionResult loadFilters(int[] areas, int[] usuarios, int[] indicadores, int? id_grupo)
        {
            var evaluaciones = GetFilteredElements(areas, usuarios, indicadores, id_grupo);

            var fe_actual = DateTime.Now;
            var fe_inicial = fe_actual.AddDays(-30);


            ViewBag.d1 = Datos(1, fe_inicial, fe_actual, evaluaciones);
            ViewBag.d2 = Datos(2, fe_inicial, fe_actual, evaluaciones);
            ViewBag.d3 = Datos(3, fe_inicial, fe_actual, evaluaciones);
            ViewBag.d4 = Datos(4, fe_inicial, fe_actual, evaluaciones);


            return PartialView();
        }


        public List<k_evaluacion_diaria> GetFilteredElements(int[] areas, int[] usuarios, int[] indicadores, int? id_grupo)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var evaluaciones = Utilidades.Utilidades.RTCObject(us, db, "k_evaluacion_diaria").Cast<k_evaluacion_diaria>().ToList();

            //filtrar por áreas
            if (areas != null)
            {
                List<int> id_users = new List<int>();
                var ar = db.c_area.Where(a => areas.Contains(a.id_area)).ToList();

                foreach (var a in ar)
                {
                    id_users.AddRange(a.c_usuario.Select(u => u.id_usuario));
                }

                evaluaciones = evaluaciones.Where(e => id_users.Contains(e.id_usuario)).ToList();
            }

            //filtrar por usuarios
            if (usuarios != null)
            {
                evaluaciones = evaluaciones.Where(e => usuarios.Contains(e.id_usuario)).ToList();
            }

            //filtrar por indicadores
            if (indicadores != null)
            {
                evaluaciones = evaluaciones.Where(e => indicadores.Contains(e.id_indicador_diario)).ToList();
            }


            //filtrar por grupo
            if (id_grupo != null)
            {
                var ids_indicadores = db.c_contenido_grupo.Find((int)id_grupo).c_indicador_diario.Select(i => i.id_indicador_diario).ToList();

                evaluaciones = evaluaciones.Where(e => ids_indicadores.Contains(e.id_indicador_diario)).ToList();
            }

            return evaluaciones;
        }

        #endregion

        #region Datos Gráficas



        private string Datos(int tipo, DateTime fe_inicial, DateTime fe_final, List<k_evaluacion_diaria> evaluacion_Diarias)
        {
            //Tipos
            //1.- Buenos 
            //2.- Regulares
            //3.- Alertas
            //4.- No Calificados
            var evals = Utilidades.Utilidades.GetEvaluationsByTipe(tipo, evaluacion_Diarias);

            var fe_aux = fe_inicial;
            List<string> labels = new List<string>();
            List<double> data = new List<double>();

            //ajustar que pasa cuando es un año diferente
            while (fe_final.Year > fe_aux.Year)
            {
                var EvaluacionesDelDia = evals.Where(e => ((DateTime)e.fe_evaluacion).DayOfYear == fe_aux.DayOfYear).ToList();

                data.Add(EvaluacionesDelDia.Count);
                labels.Add(string.Format("{0:dd/MM/yyyy}", fe_aux));

                fe_aux = fe_aux.AddDays(1);
            }
            while (fe_aux.DayOfYear < fe_final.DayOfYear + 1)
            {
                var EvaluacionesDelDia = evals.Where(e => ((DateTime)e.fe_evaluacion).DayOfYear == fe_aux.DayOfYear).ToList();

                data.Add(EvaluacionesDelDia.Count);
                labels.Add(string.Format("{0:dd/MM/yyyy}", fe_aux));

                fe_aux = fe_aux.AddDays(1);
            }


            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("No. Evaluaciones"), tipo, data.ToArray(), "white", null);

            GraphicsViewModel.Dataset[] datasets = { dataset };

            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels.ToArray(),
                datasets = datasets
            };



            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }





        #endregion

        #region Otros
        public ActionResult ContenidoGrupo(int? id)
        {
            List<c_contenido_grupo> lista = new List<c_contenido_grupo>();


            if (id == null) //Si no hay id, enviamos una lista de los contenidos a nivel raiz
            {
                lista = db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList();
            }
            else //si existe id, enviamos los descendientes del registro
            {
                var contenido = db.c_contenido_grupo.Find((int)id);
                lista = contenido.c_contenido_grupo1.ToList();
            }

            //necesitamos id y el nombre del padre del padre del contenido actual, hasta llegar a un contenido sin padre
            int? id_padre = id;
            List<int?> aux = new List<int?>();
            List<string> aux2 = new List<string>();
            int auxCont = 0;
            while (id_padre != null)
            {
                c_contenido_grupo aux3 = db.c_contenido_grupo.Find(id_padre);
                aux.Add(aux3.id_contenido_grupo_padre);
                aux2.Add(aux3.cl_contenido_grupo);
                id_padre = aux3.id_contenido_grupo_padre;
                auxCont++;
            }


            ViewBag.IDs = aux;
            ViewBag.Claves = aux2;
            ViewBag.Contador = auxCont;

            //enviamos el id del padre
            ViewBag.id_padre = id;

            return PartialView("ContenidoGrupo", lista);
        }

        public ActionResult ContenidoGrupo2(int? id)
        {
            var lista = GetContent(id);

            return PartialView("ContenidoGrupo2", lista);
        }


        private List<c_contenido_grupo> GetContent(int? id)
        {
            List<c_contenido_grupo> lista = new List<c_contenido_grupo>();


            if (id == null) //Si no hay id, enviamos una lista de los contenidos a nivel raiz
            {
                lista = db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList();
            }
            else //si existe id, enviamos los descendientes del registro
            {
                var contenido = db.c_contenido_grupo.Find((int)id);
                lista = contenido.c_contenido_grupo1.ToList();
            }

            //necesitamos id y el nombre del padre del padre del contenido actual, hasta llegar a un contenido sin padre
            int? id_padre = id;
            List<int?> aux = new List<int?>();
            List<string> aux2 = new List<string>();
            int auxCont = 0;
            while (id_padre != null)
            {
                c_contenido_grupo aux3 = db.c_contenido_grupo.Find(id_padre);
                aux.Add(aux3.id_contenido_grupo_padre);
                aux2.Add(aux3.cl_contenido_grupo);
                id_padre = aux3.id_contenido_grupo_padre;
                auxCont++;
            }


            ViewBag.IDs = aux;
            ViewBag.Claves = aux2;
            ViewBag.Contador = auxCont;

            //enviamos el id del padre
            ViewBag.id_padre = id;

            return lista;
        }


        public ActionResult ContentDetails(int id)
        {
            var contenido = db.c_contenido_grupo.Find(id);
            var YEvals = Utilidades.Utilidades.getYesterdayEvals(contenido);

            ViewBag.type = 1;
            ViewBag.id_content = id;

            return PartialView("DetailViews/DetailsEINDD", YEvals);
        }

        public ActionResult IndicatorDetails(int id_ind, int id_content)
        {
            var ind = db.c_indicador_diario.Find(id_ind);

            ViewBag.id_content = id_content;

            return PartialView("SingleDetailViews/DetailINDD", ind);
        }

        public ActionResult GraphDetails(int[] areas, int[] usuarios, int[] indicadores, int? id_grupo, int tipo, string fe_eval)
        {
            var evals = GetFilteredElements(areas, usuarios, indicadores, id_grupo);

            var ThisTypeEvals = Utilidades.Utilidades.GetEvaluationsByTipe(tipo, evals);

            var date = DateTime.Parse(fe_eval);

            var ThisDateEvals = ThisTypeEvals.Where(e => ((DateTime)e.fe_evaluacion).DayOfYear == date.DayOfYear && ((DateTime)e.fe_evaluacion).Year == date.Year).ToList();


            ViewBag.type = 2;
            ViewBag.fe_eval = fe_eval;
            ViewBag.tipo = tipo;

            return PartialView("DetailViews/DetailsEINDD", ThisDateEvals);
        }

        public ActionResult IndicatorDetails2(int id_ind, string fe_eval, int tipo)
        {
            var ind = db.c_indicador_diario.Find(id_ind);

            ViewBag.tipo = tipo;
            ViewBag.fe_eval = fe_eval;

            return PartialView("SingleDetailViews/DetailINDD", ind);
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
