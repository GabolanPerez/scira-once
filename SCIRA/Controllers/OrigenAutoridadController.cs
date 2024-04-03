using SCIRA.Models;
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
    [Access(Funcion = "OrigenAutoridad", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class OrigenAutoridadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: OrigenAutoridad
        public ActionResult Index()
        {
            var origenAutoridad = db.c_origen_autoridad.ToList();
            return View(origenAutoridad);
        }

        // GET: OrigenAutoridad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrigenAutoridad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_origen_autoridad c_origen_autoridad)
        {
            if (ModelState.IsValid)
            {
                db.c_origen_autoridad.Add(c_origen_autoridad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_origen_autoridad);
        }

        // GET: OrigenAutoridad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_origen_autoridad c_origen_autoridad = db.c_origen_autoridad.Find(id);
            if (c_origen_autoridad == null)
            {
                return HttpNotFound();
            }
            return View(c_origen_autoridad);
        }

        // POST: OrigenAutoridad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_origen_autoridad c_origen_autoridad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_origen_autoridad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_origen_autoridad);
        }

        // GET: OrigenAutoridad/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_origen_autoridad c_origen_autoridad = db.c_origen_autoridad.Find(id);
            if (c_origen_autoridad == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos Oficios
            var r_oficios = db.k_objeto.Where(o => o.id_autoridad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_oficios.Count > 0)
            {
                foreach (var oficio in r_oficios)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Oficios";
                    rr.cl_registro = "N/A";
                    rr.nb_registro = oficio.nb_objeto;
                    rr.accion = "DeleteOficioExt";
                    rr.controlador = "IyP";
                    rr.id_registro = oficio.id_objeto.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_origen_autoridad);
        }

        // POST: OrigenAutoridad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_origen_autoridad c_origen_autoridad = db.c_origen_autoridad.Find(id);
            db.c_origen_autoridad.Remove(c_origen_autoridad);
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
