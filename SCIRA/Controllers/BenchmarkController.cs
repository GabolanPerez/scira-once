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
    [Access(Funcion = "BDC", ModuleCode = "MSICI007")]
    [CustomErrorHandler]
    public class BenchmarkController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: k_benchmarck
        public ActionResult Index()
        {
            ViewBag.Entidades = db.c_entidad.ToList();
            return View(db.k_benchmarck.ToList());
        }

        // GET: k_benchmarck/Create
        public ActionResult Create(int? id)
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

            //Declarar modelo para Agregar Benchmark
            AgregarBenchmarkViewModel model = new AgregarBenchmarkViewModel();
            model.id_entidad = (int)id;

            //Añadiendo actividades a ViewBag.Actividades
            var Actividades = new List<SelectListItem>();
            var actividades = db.c_actividad.
                Where(a => a.id_entidad == id || a.id_entidad == null).
                OrderBy(a => a.id_entidad).ToList();

            foreach (var actividad in actividades)
            {
                var Actividad = new SelectListItem()
                {
                    Value = actividad.id_actividad.ToString(),
                    Text = actividad.cl_actividad + " - " + actividad.nb_actividad
                };
                Actividades.Add(Actividad);
            }
            model.MacroProcesos = Actividades;



            try
            {
                IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                int id_responsable = identity.Id_usuario;
                bool super_usuario = identity.Es_super_usuario;

                List<ListaSubProcesosViewModel> subProcesos = new List<ListaSubProcesosViewModel>();
                List<c_sub_proceso> sps = new List<c_sub_proceso>();

                if (super_usuario)
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == id).ToList();
                else
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == id && sp.id_responsable == id_responsable).ToList();

                foreach (var sp in sps)
                {
                    subProcesos.Add(new ListaSubProcesosViewModel(sp));
                }

                ViewBag.Sub_Procesos = subProcesos;


                //string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
                //var subProcesos = db.Database.SqlQuery<ListaSubProcesosViewModel>(sql).ToList();

                //var sub_procesos = new List<ListaSubProcesosViewModel>();

                //foreach(var sp in subProcesos)
                //{
                //    c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(sp.id_sub_proceso);
                //    if (c_sub_proceso.c_proceso.c_macro_proceso.id_entidad == id)
                //    {
                //        sub_procesos.Add(sp);
                //    }
                //}
                //ViewBag.Sub_Procesos = sub_procesos;
            }
            catch
            {
                return View("Error");
            }

            return View(model);
        }

        /*[AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ObtieneSubProcesosBenchmark(string idProceso)
        {
            if (String.IsNullOrEmpty(idProceso))
            {
                throw new ArgumentNullException("idProceso");
                //idProceso = "1";
            }
            int id = 0;
            bool isValid = Int32.TryParse(idProceso, out id);
            var subProcesos = db.c_sub_proceso_benchmark.Where(spb => spb.id_proceso_benchmark.ToString() == idProceso).ToList();
            var result = (from s in subProcesos
                          select new
                          {
                              id = s.id_sub_proceso_benchmark,
                              name = s.cl_sub_proceso_benchmark + " - " + s.nb_sub_proceso_benchmark
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }*/


        // POST: k_benchmarck/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(AgregarBenchmarkViewModel model)
        {
            if (ModelState.IsValid)
            {
                k_benchmarck k_benchmark = new k_benchmarck();
                k_benchmark.id_evento_riesgo = model.id_evento_riesgo;
                k_benchmark.id_sub_proceso = model.id_sub_proceso;
                db.k_benchmarck.Add(k_benchmark);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Añadiendo actividades a ViewBag.Actividades
            var Actividades = new List<SelectListItem>();
            var actividades = db.c_actividad.
                Where(a => a.id_entidad == model.id_entidad || a.id_entidad == null).
                OrderBy(a => a.id_entidad).ToList();

            foreach (var actividad in actividades)
            {
                var Actividad = new SelectListItem()
                {
                    Value = actividad.id_actividad.ToString(),
                    Text = actividad.cl_actividad + " - " + actividad.nb_actividad
                };
                Actividades.Add(Actividad);
            }
            model.MacroProcesos = Actividades;

            if (model.id_actividad > 0)
            {
                var Procesos = db.c_proceso_benchmark.Where(p => p.id_actividad == model.id_actividad).ToList();
                foreach (var proceso in Procesos)
                {
                    model.Procesos.Add(new SelectListItem
                    {
                        Value = proceso.id_proceso_benchmark.ToString(),
                        Text = proceso.cl_proceso_benchmark + " - " + proceso.nb_proceso_benchmark
                    });
                }
            }

            if (model.id_proceso_benchmark > 0)
            {
                var SubProcesos = db.c_sub_proceso_benchmark.Where(p => p.id_proceso_benchmark == model.id_proceso_benchmark).ToList();
                foreach (var sp in SubProcesos)
                {
                    model.SubProcesos.Add(new SelectListItem
                    {
                        Value = sp.id_sub_proceso_benchmark.ToString(),
                        Text = sp.cl_sub_proceso_benchmark + " - " + sp.nb_sub_proceso_benchmark
                    });
                }
            }

            if (model.id_sub_proceso_benchmark > 0)
            {
                var EventosRiesgo = db.c_evento_riesgo.Where(p => p.id_sub_proceso_benchmark == model.id_sub_proceso_benchmark).ToList();
                foreach (var er in EventosRiesgo)
                {
                    model.EventosRiesgo.Add(new SelectListItem
                    {
                        Value = er.id_evento_riesgo.ToString(),
                        Text = er.cl_evento_riesgo + " - " + er.nb_evento_riesgo
                    });
                }
            }

            try
            {
                IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                int id_responsable = identity.Id_usuario;
                bool super_usuario = identity.Es_super_usuario;

                List<ListaSubProcesosViewModel> subProcesos = new List<ListaSubProcesosViewModel>();
                List<c_sub_proceso> sps = new List<c_sub_proceso>();

                if (super_usuario)
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == model.id_entidad).ToList();
                else
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == model.id_entidad && sp.id_responsable == id_responsable).ToList();

                foreach (var sp in sps)
                {
                    subProcesos.Add(new ListaSubProcesosViewModel(sp));
                }

                ViewBag.Sub_Procesos = subProcesos;

                //IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                //int id_responsable = identity.Id_usuario;
                //bool super_usuario = identity.Es_super_usuario;

                //string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
                //var subProcesos = db.Database.SqlQuery<ListaSubProcesosViewModel>(sql).ToList();

                //var sub_procesos = new List<ListaSubProcesosViewModel>();

                //foreach (var sp in subProcesos)
                //{
                //    c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(sp.id_sub_proceso);
                //    if (c_sub_proceso.c_proceso.c_macro_proceso.id_entidad == model.id_entidad)
                //    {
                //        sub_procesos.Add(sp);
                //    }
                //}
                //ViewBag.Sub_Procesos = sub_procesos;
            }
            catch
            {
                return View("Error");
            }

            return View(model);
        }

        // GET: k_benchmarck/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_benchmarck k_benchmarck = db.k_benchmarck.Find(id);
            if (k_benchmarck == null)
            {
                return HttpNotFound();
            }

            int id_entidad = k_benchmarck.c_sub_proceso.c_proceso.c_macro_proceso.id_entidad;
            AgregarBenchmarkViewModel model = new AgregarBenchmarkViewModel();

            model.id_actividad = k_benchmarck.c_evento_riesgo.c_sub_proceso_benchmark.c_proceso_benchmark.id_actividad;
            model.id_benchmark = k_benchmarck.id_benchmark;
            model.id_entidad = id_entidad;
            model.id_evento_riesgo = k_benchmarck.id_evento_riesgo;
            model.id_sub_proceso = k_benchmarck.id_sub_proceso;
            model.id_sub_proceso_benchmark = k_benchmarck.c_evento_riesgo.id_sub_proceso_benchmark;
            model.id_proceso_benchmark = k_benchmarck.c_evento_riesgo.c_sub_proceso_benchmark.id_proceso_benchmark;

            //Añadiendo actividades a ViewBag.Actividades
            var Actividades = new List<SelectListItem>();
            var actividades = db.c_actividad.
                Where(a => a.id_entidad == id_entidad || a.id_entidad == null).
                OrderBy(a => a.id_entidad).ToList();

            foreach (var actividad in actividades)
            {
                var Actividad = new SelectListItem()
                {
                    Value = actividad.id_actividad.ToString(),
                    Text = actividad.cl_actividad + " - " + actividad.nb_actividad
                };
                Actividades.Add(Actividad);
            }
            model.MacroProcesos = Actividades;

            var Procesos = db.c_proceso_benchmark.Where(p => p.id_actividad == model.id_actividad).ToList();
            foreach (var proceso in Procesos)
            {
                model.Procesos.Add(new SelectListItem
                {
                    Value = proceso.id_proceso_benchmark.ToString(),
                    Text = proceso.cl_proceso_benchmark + " - " + proceso.nb_proceso_benchmark
                });
            }

            var SubProcesos = db.c_sub_proceso_benchmark.Where(p => p.id_proceso_benchmark == model.id_proceso_benchmark).ToList();
            foreach (var sp in SubProcesos)
            {
                model.SubProcesos.Add(new SelectListItem
                {
                    Value = sp.id_sub_proceso_benchmark.ToString(),
                    Text = sp.cl_sub_proceso_benchmark + " - " + sp.nb_sub_proceso_benchmark
                });
            }

            var EventosRiesgo = db.c_evento_riesgo.Where(p => p.id_sub_proceso_benchmark == model.id_sub_proceso_benchmark).ToList();
            foreach (var er in EventosRiesgo)
            {
                model.EventosRiesgo.Add(new SelectListItem
                {
                    Value = er.id_evento_riesgo.ToString(),
                    Text = er.cl_evento_riesgo + " - " + er.nb_evento_riesgo
                });
            }

            try
            {
                IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                int id_responsable = identity.Id_usuario;
                bool super_usuario = identity.Es_super_usuario;

                List<ListaSubProcesosViewModel> subProcesos = new List<ListaSubProcesosViewModel>();
                List<c_sub_proceso> sps = new List<c_sub_proceso>();

                var idEntidad = k_benchmarck.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.id_entidad;

                if (super_usuario)
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == idEntidad).ToList();
                else
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == idEntidad && sp.id_responsable == id_responsable).ToList();

                foreach (var sp in sps)
                {
                    subProcesos.Add(new ListaSubProcesosViewModel(sp));
                }

                ViewBag.Sub_Procesos = subProcesos;

                //IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                //int id_responsable = identity.Id_usuario;
                //bool super_usuario = identity.Es_super_usuario;

                //string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
                //var subProcesos = db.Database.SqlQuery<ListaSubProcesosViewModel>(sql).ToList();

                //var sub_procesos = new List<ListaSubProcesosViewModel>();

                //foreach (var sp in subProcesos)
                //{
                //    c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(sp.id_sub_proceso);
                //    if (c_sub_proceso.c_proceso.c_macro_proceso.id_entidad == id_entidad)
                //    {
                //        sub_procesos.Add(sp);
                //    }
                //}
                //ViewBag.Sub_Procesos = sub_procesos;
            }
            catch
            {
                return View("Error");
            }



            return View(model);
        }

        // POST: k_benchmarck/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(AgregarBenchmarkViewModel model)
        {
            if (ModelState.IsValid)
            {
                k_benchmarck k_benchmarck = db.k_benchmarck.Find(model.id_benchmark);
                k_benchmarck.id_evento_riesgo = model.id_evento_riesgo;
                k_benchmarck.id_sub_proceso = model.id_sub_proceso;
                db.Entry(k_benchmarck).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int id_entidad = model.id_entidad;

            //Añadiendo actividades a ViewBag.Actividades
            var Actividades = new List<SelectListItem>();
            var actividades = db.c_actividad.
                Where(a => a.id_entidad == id_entidad || a.id_entidad == null).
                OrderBy(a => a.id_entidad).ToList();

            foreach (var actividad in actividades)
            {
                var Actividad = new SelectListItem()
                {
                    Value = actividad.id_actividad.ToString(),
                    Text = actividad.cl_actividad + " - " + actividad.nb_actividad
                };
                Actividades.Add(Actividad);
            }
            model.MacroProcesos = Actividades;

            var Procesos = db.c_proceso_benchmark.Where(p => p.id_actividad == model.id_actividad).ToList();
            foreach (var proceso in Procesos)
            {
                model.Procesos.Add(new SelectListItem
                {
                    Value = proceso.id_proceso_benchmark.ToString(),
                    Text = proceso.cl_proceso_benchmark + " - " + proceso.nb_proceso_benchmark
                });
            }

            var SubProcesos = db.c_sub_proceso_benchmark.Where(p => p.id_proceso_benchmark == model.id_proceso_benchmark).ToList();
            foreach (var sp in SubProcesos)
            {
                model.SubProcesos.Add(new SelectListItem
                {
                    Value = sp.id_sub_proceso_benchmark.ToString(),
                    Text = sp.cl_sub_proceso_benchmark + " - " + sp.nb_sub_proceso_benchmark
                });
            }

            var EventosRiesgo = db.c_evento_riesgo.Where(p => p.id_sub_proceso_benchmark == model.id_sub_proceso_benchmark).ToList();
            foreach (var er in EventosRiesgo)
            {
                model.EventosRiesgo.Add(new SelectListItem
                {
                    Value = er.id_evento_riesgo.ToString(),
                    Text = er.cl_evento_riesgo + " - " + er.nb_evento_riesgo
                });
            }

            try
            {
                IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                int id_responsable = identity.Id_usuario;
                bool super_usuario = identity.Es_super_usuario;

                List<ListaSubProcesosViewModel> subProcesos = new List<ListaSubProcesosViewModel>();
                List<c_sub_proceso> sps = new List<c_sub_proceso>();

                if (super_usuario)
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == model.id_entidad).ToList();
                else
                    sps = db.c_sub_proceso.Where(sp => sp.c_proceso.c_macro_proceso.c_entidad.id_entidad == model.id_entidad && sp.id_responsable == id_responsable).ToList();

                foreach (var sp in sps)
                {
                    subProcesos.Add(new ListaSubProcesosViewModel(sp));
                }

                ViewBag.Sub_Procesos = subProcesos;

                //IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
                //int id_responsable = identity.Id_usuario;
                //bool super_usuario = identity.Es_super_usuario;

                //string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
                //var subProcesos = db.Database.SqlQuery<ListaSubProcesosViewModel>(sql).ToList();

                //var sub_procesos = new List<ListaSubProcesosViewModel>();

                //foreach (var sp in subProcesos)
                //{
                //    c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(sp.id_sub_proceso);
                //    if (c_sub_proceso.c_proceso.c_macro_proceso.id_entidad == id_entidad)
                //    {
                //        sub_procesos.Add(sp);
                //    }
                //}
                //ViewBag.Sub_Procesos = sub_procesos;
            }
            catch
            {
                return View("Error");
            }

            return View(model);
        }

        // GET: k_benchmarck/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_benchmarck k_benchmarck = db.k_benchmarck.Find(id);
            if (k_benchmarck == null)
            {
                return HttpNotFound();
            }

            // redirect contendrá una direccion como Controlador/Delete/57
            //se implementará una funcion que los parta en 3 palabras y regrese un objeto
            //de la clase RedirectViewModel [Controlador,Accion,Id]
            //Serán necesarias 2 variables de sesion extra, una llevara la cuenta de cuantas redirecciones se han acumulado
            //La otra variable contendrá las direcciones [Controlador/Accion/ID] para saber a cual regresar después de cada accion de borrado
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



            //



            return View(k_benchmarck);
        }

        // POST: k_benchmarck/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            k_benchmarck k_benchmarck = db.k_benchmarck.Find(id);
            db.k_benchmarck.Remove(k_benchmarck);
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
