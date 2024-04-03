using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "EINDD", ModuleCode = "MSICI008")]
    [CustomErrorHandler]
    public class EvaluacionIndicadorDiarioController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        #region Index
        public ActionResult Index()
        {
            var identity = (IdentityPersonalizado)User.Identity;
            var user = db.c_usuario.Find(identity.Id_usuario);

            var indicadores = user.c_indicador_diario.Where(i => i.esta_activo);

            List<c_indicador_diario> pendientes = new List<c_indicador_diario>();
            List<c_indicador_diario> evaluados = new List<c_indicador_diario>();

            foreach (var ind in indicadores)
            {
                var grupo = ind.c_contenido_grupo;

                var fe_actual = DateTime.Now;

                if (grupo.c_contenido_grupo1.Count > 0)
                {
                    var contenido = grupo.c_contenido_grupo1.First();

                    var eval = Utilidades.Utilidades.GetLastEval(user, ind, contenido);

                    if (eval == null)
                    {
                        eval = new k_evaluacion_diaria()
                        {
                            id_contenido_grupo = contenido.id_contenido_grupo,
                            id_indicador_diario = ind.id_indicador_diario,
                            id_usuario = user.id_usuario,
                            fe_evaluacion = DateTime.Now
                        };
                        pendientes.Add(ind);
                        db.k_evaluacion_diaria.Add(eval);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (eval.numerador != null && eval.denominador != null)
                        {
                            evaluados.Add(ind);
                        }
                        else
                        {
                            pendientes.Add(ind);
                        }
                    }
                }
            }

            ViewBag.evaluados = evaluados;
            ViewBag.pendientes = pendientes;


            return View();
        }
        #endregion

        #region Create
        public ActionResult Create(int id)
        {
            var identity = (IdentityPersonalizado)User.Identity;
            var user = db.c_usuario.Find(identity.Id_usuario);

            var indicador = db.c_indicador_diario.Find(id);
            if (!indicador.esta_activo) return RedirectToAction("Index");


            var grupo = indicador.c_contenido_grupo;

            var Evaluaciones = new List<k_evaluacion_diaria>();

            foreach (var contenido in grupo.c_contenido_grupo1)
            {
                var eval = Utilidades.Utilidades.GetLastEval(user, indicador, contenido);

                Evaluaciones.Add(eval);
            }

            ViewBag.indicador = indicador;

            return View(Evaluaciones);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult Create(int[] id_eval, int[] numerador, int[] denominador)
        {
            var count = id_eval.Count();

            for (int i = 0; i < count; i++)
            {
                var eval = db.k_evaluacion_diaria.Find(id_eval[i]);

                eval.numerador = numerador[i];
                eval.denominador = denominador[i];

            }

            db.SaveChanges();

            var user = (IdentityPersonalizado)User.Identity;
            var ev = db.k_evaluacion_diaria.Find(id_eval[0]);


            Utilidades.Utilidades.refreshNotifCount(user.Id_usuario);
            Utilidades.Utilidades.removeRow(8, ev.c_indicador_diario.id_indicador_diario, user.Id_usuario);

            return RedirectToAction("Index");
        }
        #endregion

        #region otros
        public ActionResult Contenido(int? id)
        {
            ViewBag.fcf = true;
            c_contenido_grupo contenido;
            if (id == null)
            {
                return PartialView("Grupos", db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList());
            }
            else
            {
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

                contenido = db.c_contenido_grupo.Find(id);
                return PartialView("Grupos", contenido.c_contenido_grupo1.ToList());
            }
        }

        public string ObtenerRuta(int id)
        {
            var contenido = db.c_contenido_grupo.Find(id);
            string res = contenido.cl_contenido_grupo;

            var padre = contenido.c_contenido_grupo2;
            while (padre != null)
            {

                res = padre.cl_contenido_grupo + " >> " + res;
                padre = padre.c_contenido_grupo2;
            }

            return res;
        }

        private void CreateEvalRegs(c_indicador_diario indicador)
        {
            var grupo = db.c_contenido_grupo.Find(indicador.id_contenido_grupo);


            foreach (var us in indicador.c_usuario)
            {
                foreach (var contenido in grupo.c_contenido_grupo1)
                {
                    k_evaluacion_diaria eval = new k_evaluacion_diaria()
                    {
                        id_indicador_diario = indicador.id_indicador_diario,
                        id_contenido_grupo = contenido.id_contenido_grupo,
                        id_usuario = us.id_usuario,
                        fe_evaluacion = DateTime.Now,
                    };

                    db.k_evaluacion_diaria.Add(eval);
                }
            }

            db.SaveChanges();
        }

        private void EditEvalRegs(c_indicador_diario indicador, int[] last_ids)
        {
            var new_ids = indicador.c_usuario.Select(u => u.id_usuario).ToArray();
            var grupo = indicador.c_contenido_grupo;

            foreach (var id in new_ids)
            {
                //si el id es nuevo en la lista
                if (!last_ids.Contains(id))
                {
                    foreach (var contenido in grupo.c_contenido_grupo1)
                    {
                        k_evaluacion_diaria eval = new k_evaluacion_diaria()
                        {
                            id_indicador_diario = indicador.id_indicador_diario,
                            id_contenido_grupo = contenido.id_contenido_grupo,
                            id_usuario = id,
                            fe_evaluacion = DateTime.Now,
                        };

                        db.k_evaluacion_diaria.Add(eval);
                    }
                }
                else
                {
                    //no se hace nada
                }
            }

            foreach (var id in last_ids)
            {
                //si el id ya no aparece en la lista
                if (!new_ids.Contains(id))
                {
                    foreach (var contenido in grupo.c_contenido_grupo1)
                    {
                        try
                        {
                            var eval = db.k_evaluacion_diaria.Where(
                            e => e.id_contenido_grupo == contenido.id_contenido_grupo
                            && e.id_usuario == id
                            && e.id_indicador_diario == indicador.id_indicador_diario
                            ).Last();

                            //si no se ha completado la evaluación, borrar el registro
                            if (eval.denominador == null || eval.numerador == null)
                            {
                                db.k_evaluacion_diaria.Remove(eval);
                            }
                        }
                        catch
                        {

                        }

                    }
                }
                else
                {
                    //no se hace nada
                }
            }

            db.SaveChanges();
        }


        public ActionResult EvalDetails(int id)
        {
            var ind = db.c_indicador_diario.Find(id);
            var grupo = ind.c_contenido_grupo;
            var identity = (IdentityPersonalizado)User.Identity;
            var user = db.c_usuario.Find(identity.Id_usuario);


            List<k_evaluacion_diaria> evals = new List<k_evaluacion_diaria>();

            foreach (var cont in grupo.c_contenido_grupo1)
            {
                var eval = Utilidades.Utilidades.GetLastEval(user, ind, cont);

                evals.Add(eval);
            }

            ViewBag.type = 3;

            return PartialView("DetailViews/DetailsEINDD", evals);
        }

        public ActionResult IndicatorDetails(int id_ind)
        {
            var ind = db.c_indicador_diario.Find(id_ind);

            ViewBag.FromType3 = 1;

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
