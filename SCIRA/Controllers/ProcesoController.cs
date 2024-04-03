using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Procesos", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ProcesoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        private ISelectListRepository _repository;

        public ProcesoController() : this(new SelectListRepository())
        {
        }

        public ProcesoController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        // GET: Proceso
        public ActionResult Index()
        {
            List<c_proceso> ps;

            var user = ((Seguridad.IdentityPersonalizado)User.Identity);
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            //if (su)
            //{
            //    ps = db.c_proceso.
            //    OrderBy(x => x.c_macro_proceso.c_entidad.cl_entidad).
            //    OrderBy(x => x.c_macro_proceso.cl_macro_proceso).
            //    OrderBy(x => x.cl_proceso).ToList();
            //}
            //else
            //{
            //    ps = db.c_proceso.Where(p=>p.id_responsable == user.Id_usuario).
            //    OrderBy(x => x.c_macro_proceso.c_entidad.cl_entidad).
            //    OrderBy(x => x.c_macro_proceso.cl_macro_proceso).
            //    OrderBy(x => x.cl_proceso).ToList();
            //}

            ps = Utilidades.Utilidades.RTCObject(Usuario, db, "c_proceso").Cast<c_proceso>()
                .OrderBy(x => x.c_macro_proceso.c_entidad.cl_entidad)
                .OrderBy(x => x.c_macro_proceso.cl_macro_proceso)
                .OrderBy(x => x.cl_proceso).ToList();

            return View(ps);
        }

        #region Agregar
        public ActionResult Agregar()
        {
            var user = (IdentityPersonalizado)User.Identity;

            if (!user.Funciones.Contains("AgregarPR"))
                return RedirectToAction("Denied", "Error");

            AgregarProcesoViewModel model = new AgregarProcesoViewModel();

            model.Entidades.Add(new SelectListItem { Text = "-Seleccione Entidad-", Value = "0" });

            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                model.Entidades.Add(new SelectListItem()
                {
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad,
                    Value = entidad.id_entidad.ToString()
                });
            }
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();

            return View(model);
        }

        // POST: Proceso/Agregar
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Agregar(AgregarProcesoViewModel model)
        {
            if (ModelState.IsValid)
            {
                c_proceso c_proceso = new c_proceso();

                c_proceso.id_macro_proceso = model.id_macro_proceso;
                c_proceso.id_responsable = model.id_responsable;
                c_proceso.cl_proceso = model.cl_proceso;
                c_proceso.nb_proceso = model.nb_proceso;

                db.c_proceso.Add(c_proceso);
                db.SaveChanges();

                Utilidades.Utilidades.ObjectAsigned(c_proceso);

                return RedirectToAction("Index");
            }

            //AgregarProcesoViewModel model = new AgregarProcesoViewModel();

            model.Entidades.Add(new SelectListItem { Text = "-Seleccione Entidad-", Value = "0" });

            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                model.Entidades.Add(new SelectListItem()
                {
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad,
                    Value = entidad.id_entidad.ToString()
                });
            }

            if (model.id_entidad > 0)
            {
                var MacroProcesos = db.c_macro_proceso.Where(mp => mp.id_entidad == model.id_entidad);
                foreach (var MacroProceso in MacroProcesos)
                {
                    model.MacroProcesos.Add(new SelectListItem()
                    {
                        Text = MacroProceso.cl_macro_proceso + " - " + MacroProceso.nb_macro_proceso,
                        Value = MacroProceso.id_macro_proceso.ToString()
                    });
                }
            }


            ViewBag.id_responsable = ViewBag.id_responsable = Utilidades.DropDown.Usuario(model.id_responsable);

            return View(model);
        }

        #endregion

        #region Editar

        // GET: Proceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_proceso c_proceso = db.c_proceso.Find(id);
            if (c_proceso == null)
            {
                return HttpNotFound();
            }
            int id_entidad = c_proceso.c_macro_proceso.id_entidad;

            ViewBag.id_macro_proceso = Utilidades.DropDown.MacroProcesos(c_proceso.id_macro_proceso, c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2), c_proceso.c_macro_proceso.id_entidad);
            ViewBag.clave = c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2); //clave si es mp o mg
            //ViewBag.id_macro_proceso = Utilidades.DropDown.MacroProcesos(c_proceso.id_macro_proceso, c_proceso.c_macro_proceso.cl_macro_proceso,id_entidad);

            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_proceso.id_responsable);
            return View(c_proceso);
        }

        // POST: Proceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_proceso c_proceso, int lu, string clave)
        {
            var id_mp_original = db.c_proceso.Where(x => x.id_proceso == c_proceso.id_proceso).Select(x => x.id_macro_proceso).First();

            if (id_mp_original != c_proceso.id_macro_proceso)
            {
                if (db.c_proceso.Any(x => x.id_macro_proceso == c_proceso.id_macro_proceso && x.cl_proceso == c_proceso.cl_proceso))
                {
                    ModelState.AddModelError("cl_proceso", "La clave de este proceso ya se encuentra en uso.");
                }
            }

            if (ModelState.IsValid)
            {
                c_proceso original = db.c_proceso.Find(c_proceso.id_proceso);

                recordChange(original);

                original.cl_proceso = c_proceso.cl_proceso;
                original.id_macro_proceso = c_proceso.id_macro_proceso;
                original.id_responsable = c_proceso.id_responsable;
                original.nb_proceso = c_proceso.nb_proceso;

                db.SaveChanges();

                if (c_proceso.id_responsable != lu) Utilidades.Utilidades.ObjectAsigned(c_proceso, lu);
                return RedirectToAction("Index");
            }
            c_proceso c_proceso_2 = db.c_proceso.Find(c_proceso.id_proceso);
            c_proceso_2.cl_proceso = c_proceso.cl_proceso;
            c_proceso_2.id_responsable = c_proceso.id_responsable;
            c_proceso_2.nb_proceso = c_proceso.nb_proceso;

            ViewBag.id_macro_proceso = Utilidades.DropDown.MacroProcesos(c_proceso.id_macro_proceso, clave, c_proceso_2.c_macro_proceso.id_entidad);
            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_proceso.id_responsable);
            return View(c_proceso_2);

            //ViewBag.id_macro_proceso = new SelectList(db.c_macro_proceso, "id_macro_proceso", "nb_macro_proceso", c_proceso.id_macro_proceso);
            //ViewBag.id_responsable = ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_proceso.id_responsable);
            //return View(c_proceso_2);
        }

        bool recordChange(c_proceso pr)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var registro = new r_proceso();
            registro = (r_proceso)Utilidades.Utilidades.CopyObject(pr, registro);
            registro.fe_modificacion = DateTime.Now;
            registro.nb_entidad = $"{pr.c_macro_proceso.c_entidad.cl_entidad} - {pr.c_macro_proceso.c_entidad.nb_entidad}";
            registro.nb_macro_proceso = $"{pr.c_macro_proceso.cl_macro_proceso} - {pr.c_macro_proceso.nb_macro_proceso}";
            registro.nb_responsable = pr.c_usuario.nb_usuario;
            registro.id_usuario = user.Id_usuario;
            db.r_proceso.Add(registro);
            try
            {
                return db.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Historial
        public ActionResult Historial(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_proceso c_proceso = db.c_proceso.Find(id);
            if (c_proceso == null)
            {
                return HttpNotFound();
            }

            var historial = c_proceso.r_proceso.OrderByDescending(r => r.fe_modificacion).ToList();
            ViewBag.pr = c_proceso;
            return View(historial);
        }
        #endregion

        #region Borrar

        // GET: Proceso/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_proceso c_proceso = db.c_proceso.Find(id);
            if (c_proceso == null)
            {
                return HttpNotFound();
            }

            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Session["JumpCounter"];
                    }
                    catch
                    {
                        ns = 0;
                    }
                    //Si ns es 0, creamos un nuevo array, agregamos la direccion actual y lo asignamos a la variable "Directions" y establecemos "JumpCounter" = 1
                    if (ns == 0)
                    {
                        List<string> directions = new List<string>();
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = 1;
                        HttpContext.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Session["JumpCounter"] = null;
                HttpContext.Session["Directions"] = null;
            }

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos sub procesos
            var r_sub_proceso = db.c_sub_proceso.Where(b => b.id_proceso == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_sub_proceso.Count > 0)
            {
                foreach (var sp in r_sub_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Sub Procesos";
                    rr.cl_registro = sp.cl_sub_proceso;
                    rr.nb_registro = sp.nb_sub_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "SubProceso";
                    rr.id_registro = sp.id_sub_proceso.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_proceso);
        }

        // POST: Proceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_proceso c_proceso = db.c_proceso.Find(id);
            db.c_proceso.Remove(c_proceso);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }
            //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este controlador
            if (ns == 0)
            {
                return RedirectToAction("Index");

            }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
            else
            {
                List<string> directions = new List<string>();
                try
                {
                    directions = (List<string>)HttpContext.Session["Directions"];
                }
                catch
                {
                    directions = null;
                }

                if (directions == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string direction = directions.Last();
                    DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                    //disminuimos ns y eliminamos el ultimo elemento de directions
                    ns--;
                    directions.RemoveAt(ns);

                    //Guardamos ambas variables de sesion para seguir trabajando
                    HttpContext.Session["JumpCounter"] = ns;
                    HttpContext.Session["Directions"] = directions;

                    return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                }

            }
        }

        //public ActionResult DirectDelete(int id)
        //{
        //    var pr = db.c_proceso.Find(id);

        //    Utilidades.DeleteActions.DeleteProcesoObjects(pr, db, true);

        //    db.c_proceso.Remove(pr);

        //    db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        #endregion

        #region Otros

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneMacroProcesos(string IdEntidad)
        {
            if (String.IsNullOrEmpty(IdEntidad))
            {
                // throw new ArgumentNullException("IdEntidad");
                IdEntidad = "1";
            }
            int id = 0;
            bool isValid = Int32.TryParse(IdEntidad, out id);
            var macroProcesos = _repository.ObtieneMacroProcesos(id);
            var result = (from s in macroProcesos
                          select new
                          {
                              id = s.id_macro_proceso,
                              name = s.cl_macro_proceso + " - " + s.nb_macro_proceso
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
