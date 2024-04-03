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
    [Access(Funcion = "ClasInc", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class ClasificacionIncidenciaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ClasificacionIncidencia
        public ActionResult Index()
        {
            var clasificaciones = db.c_clasificacion_incidencia.ToList();
            return View(clasificaciones);
        }

        // GET: ClasificacionIncidencia/Create
        public ActionResult Create()
        {
            var colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.color = colores;
            return View();
        }

        // POST: ClasificacionIncidencia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_clasificacion_incidencia c_clasificacion_incidencia)
        {
            if (ModelState.IsValid)
            {
                db.c_clasificacion_incidencia.Add(c_clasificacion_incidencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.color = colores;

            return View(c_clasificacion_incidencia);
        }

        // GET: ClasificacionIncidencia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_clasificacion_incidencia c_clasificacion_incidencia = db.c_clasificacion_incidencia.Find(id);
            if (c_clasificacion_incidencia == null)
            {
                return HttpNotFound();
            }

            var colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.color = colores;
            return View(c_clasificacion_incidencia);
        }

        // POST: ClasificacionIncidencia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_clasificacion_incidencia c_clasificacion_incidencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_clasificacion_incidencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.color = colores;
            return View(c_clasificacion_incidencia);
        }

        // GET: ClasificacionIncidencia/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_clasificacion_incidencia c_clasificacion_incidencia = db.c_clasificacion_incidencia.Find(id);
            if (c_clasificacion_incidencia == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos Incidencias
            var r_incidencias = db.k_incidencia.Where(i => i.id_clasificacion_incidencia == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_incidencias.Count > 0)
            {
                foreach (var incidencia in r_incidencias)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Incidencias";
                    rr.cl_registro = "N/A";
                    rr.nb_registro = incidencia.ds_incidencia;
                    rr.accion = "DeleteIncidenciaExt";
                    rr.controlador = "IyP";
                    rr.id_registro = incidencia.id_incidencia.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_clasificacion_incidencia);
        }

        // POST: ClasificacionIncidencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_clasificacion_incidencia c_clasificacion_incidencia = db.c_clasificacion_incidencia.Find(id);
            db.c_clasificacion_incidencia.Remove(c_clasificacion_incidencia);
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
