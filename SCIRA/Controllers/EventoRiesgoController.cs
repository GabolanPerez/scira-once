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
    [Access(Funcion = "EventoRiesgo", ModuleCode = "MSICI007")]
    [CustomErrorHandler]
    public class EventoRiesgoController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private ISelectListRepository _repository;

        public EventoRiesgoController() : this(new SelectListRepository())
        {
        }

        public EventoRiesgoController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        // GET: EventoRiesgo
        public ActionResult Index()
        {
            var c_evento_riesgo = db.c_evento_riesgo.ToList();
            return View(c_evento_riesgo);
        }

        // GET: EventoRiesgo/Create
        public ActionResult Create()
        {
            AgregarEventoRiesgoViewModel model = new AgregarEventoRiesgoViewModel();
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

        // POST: EventoRiesgo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(AgregarEventoRiesgoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var er = new c_evento_riesgo
                {
                    id_sub_proceso_benchmark = model.id_sub_proceso_benchmark,
                    cl_evento_riesgo = model.cl_evento_riesgo,
                    nb_evento_riesgo = model.nb_evento_riesgo
                };

                db.c_evento_riesgo.Add(er);
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
            var mps = db.c_actividad.Where(a => a.id_entidad == null).ToList();
            foreach (var mp in mps)
            {
                model.MacroProcesos.Add(new SelectListItem()
                {
                    Text = mp.cl_actividad + " - " + mp.nb_actividad,
                    Value = mp.id_actividad.ToString()
                });
            }

            if (model.id_actividad > 0)
            {
                var Procesos = db.c_proceso_benchmark.Where(p => p.id_actividad == model.id_actividad).ToList();
                foreach (var Proceso in Procesos)
                {
                    model.Procesos.Add(new SelectListItem
                    {
                        Text = Proceso.cl_proceso_benchmark + " - " + Proceso.nb_proceso_benchmark,
                        Value = Proceso.id_proceso_benchmark.ToString()
                    });
                }
            }

            if (model.id_proceso_benchmark > 0)
            {
                var SubProcesos = db.c_sub_proceso_benchmark.Where(sp => sp.id_proceso_benchmark == model.id_proceso_benchmark).ToList();
                foreach (var sp in SubProcesos)
                {
                    model.SubProcesos.Add(new SelectListItem
                    {
                        Text = sp.cl_sub_proceso_benchmark + " - " + sp.nb_sub_proceso_benchmark,
                        Value = sp.id_sub_proceso_benchmark.ToString()
                    });
                }
            }

            return View(model);
        }

        // GET: EventoRiesgo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_evento_riesgo c_evento_riesgo = db.c_evento_riesgo.Find(id);
            if (c_evento_riesgo == null)
            {
                return HttpNotFound();
            }
            return View(c_evento_riesgo);
        }

        // POST: EventoRiesgo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_evento_riesgo c_evento_riesgo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_evento_riesgo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_evento_riesgo);
        }

        // GET: EventoRiesgo/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_evento_riesgo c_evento_riesgo = db.c_evento_riesgo.Find(id);
            if (c_evento_riesgo == null)
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

            //solo incluiremos benchmark
            var r_benchmarck = db.k_benchmarck.Where(b => b.id_evento_riesgo == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_benchmarck.Count > 0)
            {
                foreach (var benchmarck in r_benchmarck)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Benchmarck";
                    rr.cl_registro = benchmarck.id_benchmark.ToString();
                    rr.nb_registro = "Benchmarck ligado al Sub Proceso" + benchmarck.c_sub_proceso.nb_sub_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "Benchmark";
                    rr.id_registro = benchmarck.id_benchmark.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;



            return View(c_evento_riesgo);
        }

        // POST: EventoRiesgo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_evento_riesgo c_evento_riesgo = db.c_evento_riesgo.Find(id);
            db.c_evento_riesgo.Remove(c_evento_riesgo);
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
