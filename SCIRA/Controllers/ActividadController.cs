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
    [Access(Funcion = "Actividad", ModuleCode = "MSICI007")]
    [CustomErrorHandler]
    public class ActividadController : Controller
    {

        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index()
        {
            var c_actividad = db.c_actividad.Include(c => c.c_entidad);
            return View(c_actividad.ToList());
        }

        // GET: Actividad/Create
        public ActionResult Create()
        {
            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad");
            return View();
        }

        // POST: Actividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_actividad,cl_actividad,nb_actividad,id_entidad")] c_actividad c_actividad)
        {
            if (ModelState.IsValid)
            {
                db.c_actividad.Add(c_actividad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad", c_actividad.id_entidad);
            return View(c_actividad);
        }

        // GET: Actividad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_actividad c_actividad = db.c_actividad.Find(id);
            if (c_actividad == null)
            {
                return HttpNotFound();
            }
            return View(c_actividad);
        }

        // POST: Actividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_actividad,cl_actividad,nb_actividad,id_entidad")] c_actividad c_actividad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_actividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_actividad);
        }

        // GET: Actividad/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_actividad c_actividad = db.c_actividad.Find(id);
            if (c_actividad == null)
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

            //solo incluiremos procesos de benchmark
            var r_procesos_b = db.c_proceso_benchmark.Where(b => b.id_actividad == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_procesos_b.Count > 0)
            {
                foreach (var proceso in r_procesos_b)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Procesos de Benchmarck";
                    rr.cl_registro = proceso.cl_proceso_benchmark;
                    rr.nb_registro = proceso.nb_proceso_benchmark;
                    rr.accion = "Delete";
                    rr.controlador = "ProcesoBenchmark";
                    rr.id_registro = proceso.id_proceso_benchmark.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_actividad);
        }

        // POST: Actividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_actividad c_actividad = db.c_actividad.Find(id);
            db.c_actividad.Remove(c_actividad);
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
