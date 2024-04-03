using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Entidades", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class EntidadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Entidad
        public ActionResult Index()
        {
            List<c_entidad> entidades;

            var user = (IdentityPersonalizado)User.Identity;

            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            //Obtiene los registros dependiendo de si es super usuario o en caso contrario, del tramo de control
            entidades = Utilidades.Utilidades.RTCObject(Usuario, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();

            return View(entidades);
        }

        // GET: Entidad/Create
        public ActionResult Create()
        {
            var user = (IdentityPersonalizado)User.Identity;

            if (!user.Funciones.Contains("AgregarEN"))
                return RedirectToAction("Denied", "Error");

            ViewBag.id_responsable = Utilidades.DropDown.Usuario();
            return View();
        }

        // POST: Entidad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Create(c_entidad c_entidad)
        {
            if (ModelState.IsValid)
            {
                db.c_entidad.Add(c_entidad);
                db.SaveChanges();

                Utilidades.Utilidades.ObjectAsigned(c_entidad);
                return RedirectToAction("Index");
            }

            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_entidad.id_responsable);
            return View(c_entidad);
        }

        // GET: Entidad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_entidad c_entidad = db.c_entidad.Find(id);
            if (c_entidad == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_entidad.id_responsable);
            return View(c_entidad);
        }

        // POST: Entidad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_entidad c_entidad, int lu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_entidad).State = EntityState.Modified;
                db.SaveChanges();

                if (c_entidad.id_responsable != lu) Utilidades.Utilidades.ObjectAsigned(c_entidad, lu);

                return RedirectToAction("Index");
            }
            ViewBag.id_responsable = Utilidades.DropDown.Usuario(c_entidad.id_responsable);
            return View(c_entidad);
        }

        // GET: Entidad/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_entidad c_entidad = db.c_entidad.Find(id);
            if (c_entidad == null)
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

            //incluiremos bdei
            var r_bdei = db.k_bdei.Where(b => b.id_entidad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_bdei.Count > 0)
            {
                foreach (var bdei in r_bdei)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "BDEI";
                    rr.cl_registro = bdei.id_bdei.ToString();
                    rr.nb_registro = "BDEI ligado a la entidad: " + bdei.c_entidad.nb_entidad;
                    rr.accion = "Delete";
                    rr.controlador = "BDEI";
                    rr.id_registro = bdei.id_bdei.ToString();

                    RR.Add(rr);
                }
            }

            //c_actividad
            var r_actividad = db.c_actividad.Where(b => b.id_entidad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_actividad.Count > 0)
            {
                foreach (var actividad in r_actividad)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Actividad";
                    rr.cl_registro = actividad.cl_actividad;
                    rr.nb_registro = actividad.nb_actividad;
                    rr.accion = "Delete";
                    rr.controlador = "Actividad";
                    rr.id_registro = actividad.id_actividad.ToString();

                    RR.Add(rr);
                }
            }

            //c_indicador
            var r_indicador = db.c_indicador.Where(b => b.id_entidad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_indicador.Count > 0)
            {
                foreach (var indicador in r_indicador)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Indicador";
                    rr.cl_registro = indicador.cl_indicador;
                    rr.nb_registro = indicador.nb_indicador;
                    rr.accion = "Delete";
                    rr.controlador = "Indicador";
                    rr.id_registro = indicador.id_indicador.ToString();

                    RR.Add(rr);
                }
            }

            //c_macro_proceso
            var r_macro_proceso = db.c_macro_proceso.Where(b => b.id_entidad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_macro_proceso.Count > 0)
            {
                foreach (var mp in r_macro_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Macro Proceso";
                    rr.cl_registro = mp.cl_macro_proceso;
                    rr.nb_registro = mp.nb_macro_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "MacroProceso";
                    rr.id_registro = mp.id_macro_proceso.ToString();

                    RR.Add(rr);
                }
            }

            //c_centro_costo
            var r_centro_costo = db.c_centro_costo.Where(b => b.id_entidad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_centro_costo.Count > 0)
            {
                foreach (var cc in r_centro_costo)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Centro de Costo";
                    rr.cl_registro = cc.cl_centro_costo;
                    rr.nb_registro = cc.nb_centro_costo;
                    rr.accion = "Delete";
                    rr.controlador = "CentroCosto";
                    rr.id_registro = cc.id_centro_costo.ToString();

                    RR.Add(rr);
                }
            }

            //c_grupo_cuenta_contable
            var r_grupo_cuenta_contable = db.c_grupo_cuenta_contable.Where(b => b.id_entidad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_grupo_cuenta_contable.Count > 0)
            {
                foreach (var cc in r_grupo_cuenta_contable)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Grupo Cuenta Contable";
                    rr.cl_registro = cc.cl_grupo_cuenta_contable;
                    rr.nb_registro = cc.nb_grupo_cuenta_contable;
                    rr.accion = "Delete";
                    rr.controlador = "GrupoCuentaContable";
                    rr.id_registro = cc.id_grupo_cuenta_contable.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_entidad);
        }

        // POST: Entidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_entidad c_entidad = db.c_entidad.Find(id);
            Utilidades.DeleteActions.DeleteEntidadObjects(c_entidad, db);

            db.c_entidad.Remove(c_entidad);
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
