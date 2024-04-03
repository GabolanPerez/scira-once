using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "INDD", ModuleCode = "MSICI008")]
    [CustomErrorHandler]
    public class IndicadorDiarioController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        #region Index
        public ActionResult Index()
        {
            var model = db.c_indicador_diario.ToList();


            return View(model);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            var model = new c_indicador_diario();

            ViewBag.usuariosL = DropDown.UsuariosPorAreaMS();
            ViewBag.Contenido_Grupo = db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList();
            return View(model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult Create(c_indicador_diario model, int[] id_usuarios)
        {
            if (ModelState.IsValid)
            {
                db.c_indicador_diario.Add(model);
                db.SaveChanges();

                if (id_usuarios != null)
                {
                    foreach (var id_u in id_usuarios)
                    {
                        var us = db.c_usuario.Find(id_u);
                        model.c_usuario.Add(us);
                    }

                    db.SaveChanges();

                    foreach (var id_u in id_usuarios)
                    {
                        Utilidades.Utilidades.refreshNotifCount(id_u);
                    }
                }

                if (model.esta_activo) CreateEvalRegs(model);

                return RedirectToAction("Index");
            }

            ViewBag.usuariosL = DropDown.UsuariosMS();
            ViewBag.Contenido_Grupo = db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList();

            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var model = db.c_indicador_diario.Find(id);

            ViewBag.usuariosL = DropDown.UsuariosMS(model.c_usuario.Select(u => u.id_usuario).ToArray());
            ViewBag.Contenido_Grupo = db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList();

            ViewBag.lg = model.id_contenido_grupo;

            return View(model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult Edit(c_indicador_diario model, int[] id_usuarios, int id_last_group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                db = new SICIEntities();
                var indicador = db.c_indicador_diario.Find(model.id_indicador_diario);

                var last_ids = indicador.c_usuario.Select(u => u.id_usuario).ToArray();

                indicador.c_usuario.Clear();
                db.SaveChanges();

                if (id_usuarios != null)
                {
                    foreach (var id_u in id_usuarios)
                    {
                        var us = db.c_usuario.Find(id_u);
                        indicador.c_usuario.Add(us);
                    }

                    db.SaveChanges();
                }

                var lastGroup = db.c_contenido_grupo.Find(id_last_group);

                EditEvalRegs(indicador, last_ids, lastGroup);

                return RedirectToAction("Index");
            }

            ViewBag.usuariosL = DropDown.UsuariosMS();
            ViewBag.Contenido_Grupo = db.c_contenido_grupo.Where(g => g.id_contenido_grupo_padre == null).ToList();

            ViewBag.lg = id_last_group;

            return View(model);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_indicador_diario model = db.c_indicador_diario.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_indicador_diario indicador = db.c_indicador_diario.Find(id);

            Utilidades.DeleteActions.DeleteIndicadorDiarioObjects(indicador, db);

            db.c_indicador_diario.Remove(indicador);
            db.SaveChanges();

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

        private void CreateEvalRegs(c_indicador_diario indicador)
        {
            var grupo = db.c_contenido_grupo.Find(indicador.id_contenido_grupo);

            var fechaActual = DateTime.Now;


            foreach (var us in indicador.c_usuario)
            {
                foreach (var contenido in grupo.c_contenido_grupo1)
                {
                    k_evaluacion_diaria eval = new k_evaluacion_diaria()
                    {
                        id_indicador_diario = indicador.id_indicador_diario,
                        id_contenido_grupo = contenido.id_contenido_grupo,
                        id_usuario = us.id_usuario,
                        fe_evaluacion = fechaActual,
                    };

                    db.k_evaluacion_diaria.Add(eval);
                }
            }

            db.SaveChanges();
        }

        private void EditEvalRegs(c_indicador_diario indicador, int[] last_ids, c_contenido_grupo last_group)
        {
            var new_ids = indicador.c_usuario.Select(u => u.id_usuario).ToArray();
            var grupo = indicador.c_contenido_grupo;

            var fechaActual = DateTime.Now;
            var actualDayOfYear = fechaActual.DayOfYear;

            foreach (var id in new_ids)
            {
                //si el id es nuevo en la lista
                if (!last_ids.Contains(id) && indicador.esta_activo)
                {
                    foreach (var contenido in grupo.c_contenido_grupo1)
                    {
                        //Se crea una evaluación para cada contenido
                        k_evaluacion_diaria eval = new k_evaluacion_diaria()
                        {
                            id_indicador_diario = indicador.id_indicador_diario,
                            id_contenido_grupo = contenido.id_contenido_grupo,
                            id_usuario = id,
                            fe_evaluacion = fechaActual,
                        };

                        db.k_evaluacion_diaria.Add(eval);
                    }
                }
                else //Si el id ya existía, crear tabla si no existe
                {
                    //Si los grupos cambiaron, se eliminan las evaluaciones actuales y se crean nuevas
                    //en caso contrario, solamente se actualizan las evaluaciones
                    if (last_group.id_contenido_grupo != indicador.id_contenido_grupo)
                    {
                        var contenidos = last_group.c_contenido_grupo1;
                        foreach (var contenido in contenidos)
                        {
                            try
                            {
                                var LastEval = db.k_evaluacion_diaria.Where(e => e.id_indicador_diario == indicador.id_indicador_diario && e.id_usuario == id && e.id_contenido_grupo == contenido.id_contenido_grupo).ToList().Last();
                                db.k_evaluacion_diaria.Remove(LastEval);
                            }
                            catch
                            {

                            }
                        }


                        if (indicador.esta_activo)
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

                    }
                    else //Si sigue siendo el mismo grupo
                    {
                        if (indicador.esta_activo)
                        {
                            foreach (var contenido in grupo.c_contenido_grupo1)
                            {
                                var LastEval = db.k_evaluacion_diaria.ToList().LastOrDefault(e => e.id_indicador_diario == indicador.id_indicador_diario && e.id_usuario == id && e.id_contenido_grupo == contenido.id_contenido_grupo);
                                var LastEvalDate = (DateTime)LastEval.fe_evaluacion;

                                var DayOfYear = LastEvalDate.DayOfYear;

                                if (DayOfYear != actualDayOfYear)
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
                        }
                    }
                }
            }

            foreach (var id in last_ids)
            {
                //si el id ya no aparece en la lista
                if (!new_ids.Contains(id))
                {
                    foreach (var contenido in last_group.c_contenido_grupo1)
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
