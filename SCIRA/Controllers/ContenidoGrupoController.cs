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
    [Access(Funcion = "CNG", ModuleCode = "MSICI008")]
    [CustomErrorHandler]
    public class ContenidoGrupoController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        #region Index
        public ActionResult Index(int? id)
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

            return View(lista);
        }
        #endregion

        #region Create
        public ActionResult Create(int? id)
        {
            var model = new c_contenido_grupo();
            model.id_contenido_grupo_padre = id;


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

            return View(model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult Create(c_contenido_grupo model)
        {
            if (ModelState.IsValid)
            {
                db.c_contenido_grupo.Add(model);
                db.SaveChanges();


                if (model.id_contenido_grupo_padre != null)
                {
                    var padre = db.c_contenido_grupo.Find(model.id_contenido_grupo_padre);
                    var indicadores = padre.c_indicador_diario.ToList();

                    //para cada indicador de su padre, crearemos una tabla de evaluación
                    foreach (var ind in indicadores)
                    {
                        foreach (var us in ind.c_usuario)
                        {
                            k_evaluacion_diaria eval = new k_evaluacion_diaria()
                            {
                                id_indicador_diario = ind.id_indicador_diario,
                                id_contenido_grupo = model.id_contenido_grupo,
                                id_usuario = us.id_usuario,
                                fe_evaluacion = DateTime.Now,
                            };

                            db.k_evaluacion_diaria.Add(eval);
                        }
                    }
                    db.SaveChanges();
                }


                return RedirectToAction("Index", new { id = model.id_contenido_grupo_padre });
            }

            return RedirectToAction("Create", new { id = model.id_contenido_grupo_padre });
        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var model = db.c_contenido_grupo.Find(id);

            int? id_padre = model.id_contenido_grupo_padre;
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

            return View(model);
        }

        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public ActionResult Edit(c_contenido_grupo model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", new { id = model.id_contenido_grupo_padre });
            }

            return RedirectToAction("Edit", new { id = model.id_contenido_grupo });
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_contenido_grupo model = db.c_contenido_grupo.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }


            int? id_padre = model.id_contenido_grupo_padre;
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


            return View(model);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_contenido_grupo contenido = db.c_contenido_grupo.Find(id);
            //Borramos todos los niveles inferiores
            int? id_padre = contenido.id_contenido_grupo_padre;


            Utilidades.DeleteActions.DeleteContenidoGrupoObjects(contenido, db);

            db.c_contenido_grupo.Remove(contenido);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = id_padre });
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
