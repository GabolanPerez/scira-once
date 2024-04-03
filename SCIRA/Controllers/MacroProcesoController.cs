using LinqToExcel.Extensions;
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
using System.Web.UI.WebControls;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Macroprocesos", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class MacroProcesoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        private ISelectListRepository _repository;

        public MacroProcesoController() : this(new SelectListRepository())
        {
        }

        public MacroProcesoController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        // GET: MacroProceso
        public ActionResult Index()
        {

            var user = ((Seguridad.IdentityPersonalizado)User.Identity);
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            //var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            //if (su)
            //{
            //    mps = db.c_macro_proceso.
            //    OrderBy(x => x.c_entidad.cl_entidad).
            //    OrderBy(x => x.cl_macro_proceso).ToList();
            //}
            //else
            //{
            //    mps = db.c_macro_proceso.Where(mp=> mp.id_responsable == user.Id_usuario).
            //    OrderBy(x => x.c_entidad.cl_entidad).
            //    OrderBy(x => x.cl_macro_proceso).ToList();
            //}


            //var mps2 = Utilidades.Utilidades.RTCObject(Usuario, db, "c_macro_proceso").Cast<c_macro_proceso>()
            //    .OrderBy(x => x.c_entidad.cl_entidad)
            //    .OrderBy(x => x.cl_macro_proceso).ToList();

            var mps = Utilidades.Utilidades.RTCObject(Usuario, db, "c_macro_proceso");
            //List<c_macro_proceso> l1 = ret1.ToList();




            return View(mps);
        }

        #region Agregar
        // GET: MacroProceso/Create
        public ActionResult Create()
        {

            var user = (IdentityPersonalizado)User.Identity;

            if (!user.Funciones.Contains("AgregarMP"))
                return RedirectToAction("Denied", "Error");

            List<SelectListItem> paso = new List<SelectListItem>();
            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                paso.Add(new SelectListItem()
                {
                    Value = entidad.id_entidad.ToString(),
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad
                });
            }

            ViewBag.id_entidad = Utilidades.DropDown.Entidades();
            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return View();
        }

        // POST: MacroProceso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Create(c_macro_proceso c_macro_proceso)
        {
            if (ModelState.IsValid)
            {
                db.c_macro_proceso.Add(c_macro_proceso);
                db.SaveChanges();

                Utilidades.Utilidades.ObjectAsigned(c_macro_proceso);
                return RedirectToAction("Index");
            }

            ViewBag.id_entidad = Utilidades.DropDown.Entidades(c_macro_proceso.id_entidad);
            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_macro_proceso.id_responsable);
            return View(c_macro_proceso);
        }
        #endregion

        #region Editar

        // GET: MacroProceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_macro_proceso c_macro_proceso = db.c_macro_proceso.Find(id);
            if (c_macro_proceso == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_entidadL = Utilidades.DropDown.Entidades(c_macro_proceso.id_entidad);
            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(c_macro_proceso.id_responsable);
            return View(c_macro_proceso);
        }

        // POST: MacroProceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_macro_proceso c_macro_proceso, int lu)
        {
            if (ModelState.IsValid)
            {
                var user = (IdentityPersonalizado)User.Identity;


                //por medio del id c_macro_proceso obtenemos el registro original
                c_macro_proceso mpo = db.c_macro_proceso.Find(c_macro_proceso.id_macro_proceso);
                //creamos un r_macro_proceso
                //r_macro_proceso r_macro_proceso = new r_macro_proceso();


                //r_macro_proceso = (r_macro_proceso)Utilidades.Utilidades.CopyObject(c_macro_proceso_original, r_macro_proceso);
                //r_macro_proceso.nb_entidad = c_macro_proceso_original.c_entidad.nb_entidad;
                //r_macro_proceso.nb_responsable = c_macro_proceso_original.c_usuario.nb_usuario;
                //r_macro_proceso.id_usuario = user.Id_usuario;
                //r_macro_proceso.fe_modificacion = DateTime.Now;


                //db.r_macro_proceso.Add(r_macro_proceso);

                recordChange(mpo);

                mpo.cl_macro_proceso = c_macro_proceso.cl_macro_proceso;
                mpo.nb_macro_proceso = c_macro_proceso.nb_macro_proceso;
                mpo.id_responsable = c_macro_proceso.id_responsable;
                mpo.id_macro_proceso = c_macro_proceso.id_macro_proceso;
                mpo.id_entidad = c_macro_proceso.id_entidad;

                //db.Entry(c_macro_proceso).State = EntityState.Modified;
                db.SaveChanges();

                if (c_macro_proceso.id_responsable != lu) Utilidades.Utilidades.ObjectAsigned(c_macro_proceso, lu);
                return RedirectToAction("Index");
            }
            ViewBag.id_entidad = Utilidades.DropDown.Entidades(c_macro_proceso.id_entidad);
            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_macro_proceso.id_responsable);

            //agregado para prueba
            c_macro_proceso c_macro_proceso2 = db.c_macro_proceso.Find(c_macro_proceso.id_macro_proceso);
            c_macro_proceso2.cl_macro_proceso = c_macro_proceso.cl_macro_proceso;
            c_macro_proceso2.nb_macro_proceso = c_macro_proceso.nb_macro_proceso;
            c_macro_proceso2.id_responsable = c_macro_proceso.id_responsable;

            return View(c_macro_proceso2);
        }


        bool recordChange(c_macro_proceso mp) {

            var user = (IdentityPersonalizado)User.Identity;

            var registro = new r_macro_proceso();

            registro = (r_macro_proceso)Utilidades.Utilidades.CopyObject(mp, registro);

            registro.fe_modificacion = DateTime.Now;
            registro.nb_entidad = $"{mp.c_entidad.cl_entidad} - {mp.c_entidad.nb_entidad}";
            registro.nb_responsable = mp.c_usuario.nb_usuario;
            registro.id_usuario = user.Id_usuario;

            db.r_macro_proceso.Add(registro);

            try
            {
                return true;
                //return db.SaveChanges() > 0;
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
            c_macro_proceso c_macro_proceso = db.c_macro_proceso.Find(id);
            if (c_macro_proceso == null)
            {
                return HttpNotFound();
            }

            var historial = c_macro_proceso.r_macro_proceso.OrderByDescending(r => r.fe_modificacion).ToList();
            ViewBag.mp = c_macro_proceso;
            return View(historial);
        }
        #endregion

        #region Borrar

        // GET: MacroProceso/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_macro_proceso c_macro_proceso = db.c_macro_proceso.Find(id);
            if (c_macro_proceso == null)
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

            //solo incluiremos proceso
            var r_proceso = db.c_proceso.Where(b => b.id_macro_proceso == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_proceso.Count > 0)
            {
                foreach (var proceso in r_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Procesos";
                    rr.cl_registro = proceso.cl_proceso;
                    rr.nb_registro = proceso.nb_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "Proceso";
                    rr.id_registro = proceso.id_proceso.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_macro_proceso);
        }

        // POST: MacroProceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_macro_proceso c_macro_proceso = db.c_macro_proceso.Find(id);
            db.c_macro_proceso.Remove(c_macro_proceso);
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
        #endregion

        #region Otros

        // GET: MacroProceso/Upload
        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult DirectDelete(int id)
        {
            var mp = db.c_macro_proceso.Find(id);

            Utilidades.DeleteActions.DeleteMacroProcesoObjects(mp, db, true);
            db.c_macro_proceso.Remove(mp);
            db.SaveChanges();

            return RedirectToAction("Index");
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
