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
    [Access(Funcion = "PRBN", ModuleCode = "MSICI007")]
    [CustomErrorHandler]
    public class ProcesoBenchmarkController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private ISelectListRepository _repository;

        public ProcesoBenchmarkController() : this(new SelectListRepository())
        {
        }

        public ProcesoBenchmarkController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        // GET: ProcesoBenchmark
        public ActionResult Index()
        {
            var c_proceso_benchmark = db.c_proceso_benchmark.ToList();
            return View(c_proceso_benchmark);
        }

        // GET: ProcesoBenchmark/Create
        public ActionResult Create()
        {
            AgregarProcesoBenchViewModel model = new AgregarProcesoBenchViewModel();
            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                model.Entidades.Add(new SelectListItem()
                {
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad,
                    Value = entidad.id_entidad.ToString()
                });
            }
            var mps = db.c_actividad.Where(a => a.id_entidad == null).ToList();
            foreach (var mp in mps)
            {
                model.MacroProcesos.Add(new SelectListItem()
                {
                    Text = mp.cl_actividad + " - " + mp.nb_actividad,
                    Value = mp.id_actividad.ToString()
                });
            }

            return View(model);
        }

        // POST: ProcesoBenchmark/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(AgregarProcesoBenchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var proceso = new c_proceso_benchmark
                {
                    cl_proceso_benchmark = model.cl_proceso_benchmark,
                    id_actividad = model.id_actividad,
                    nb_proceso_benchmark = model.nb_proceso_benchmark
                };

                db.c_proceso_benchmark.Add(proceso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var entidades = _repository.ObtieneEntidades();
            foreach (var entidad in entidades)
            {
                model.Entidades.Add(new SelectListItem()
                {
                    Text = entidad.cl_entidad + " - " + entidad.nb_entidad,
                    Value = entidad.id_entidad.ToString()
                });
            }

            var MacroProcesos = db.c_actividad.Where(mp => mp.id_entidad == model.id_entidad).ToList();
            foreach (var MacroProceso in MacroProcesos)
            {
                model.MacroProcesos.Add(new SelectListItem()
                {
                    Text = MacroProceso.cl_actividad + " - " + MacroProceso.nb_actividad,
                    Value = MacroProceso.id_actividad.ToString()
                });
            }

            return View(model);
        }

        // GET: ProcesoBenchmark/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_proceso_benchmark c_proceso_benchmark = db.c_proceso_benchmark.Include(p => p.c_actividad.c_entidad).Include(p => p.c_actividad).Where(p => p.id_proceso_benchmark == id).First();
            if (c_proceso_benchmark == null)
            {
                return HttpNotFound();
            }
            return View(c_proceso_benchmark);
        }

        // POST: ProcesoBenchmark/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_proceso_benchmark c_proceso_benchmark)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_proceso_benchmark).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_proceso_benchmark);
        }

        // GET: ProcesoBenchmark/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_proceso_benchmark c_proceso_benchmark = db.c_proceso_benchmark.Find(id);
            if (c_proceso_benchmark == null)
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

            //solo incluiremos sub_proceso_benchmark
            var r_sub_proceso_benchmarck = db.c_sub_proceso_benchmark.Where(b => b.id_proceso_benchmark == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_sub_proceso_benchmarck.Count > 0)
            {
                foreach (var spb in r_sub_proceso_benchmarck)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Sub Procesos de Benchmarck";
                    rr.cl_registro = spb.cl_sub_proceso_benchmark;
                    rr.nb_registro = spb.nb_sub_proceso_benchmark;
                    rr.accion = "Delete";
                    rr.controlador = "SubProcesoBenchmark";
                    rr.id_registro = spb.id_sub_proceso_benchmark.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_proceso_benchmark);
        }

        // POST: ProcesoBenchmark/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_proceso_benchmark c_proceso_benchmark = db.c_proceso_benchmark.Find(id);
            db.c_proceso_benchmark.Remove(c_proceso_benchmark);
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
