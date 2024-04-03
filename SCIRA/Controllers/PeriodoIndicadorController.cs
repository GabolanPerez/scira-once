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
    [Access(Funcion = "PeriodosIndicadores", ModuleCode = "MSICI001")]
    [CustomErrorHandler]
    public class PeriodoIndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: PeriodoIndicador
        public ActionResult Index()
        {
            return View(db.c_periodo_indicador.ToList());
        }

        // GET: PeriodoIndicador/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_periodo_indicador c_periodo_indicador = db.c_periodo_indicador.Find(id);
            if (c_periodo_indicador == null)
            {
                return HttpNotFound();
            }
            return View(c_periodo_indicador);
        }

        // GET: PeriodoIndicador/Create
        public ActionResult Create()
        {
            c_periodo_indicador model = new c_periodo_indicador();

            return View(model);
        }

        // POST: PeriodoIndicador/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_periodo_indicador,cl_periodo_indicador,nb_periodo_indicador")] c_periodo_indicador c_periodo_indicador)
        {
            if (ModelState.IsValid)
            {

                c_periodo_indicador actual;

                try
                {
                    actual = db.c_periodo_indicador.Where(p => p.esta_activo == true).First();
                }
                catch
                {
                    actual = null;
                }

                if (actual != null)
                {
                    actual.esta_activo = false;
                    db.Entry(actual).State = EntityState.Modified;
                }
                c_periodo_indicador.esta_activo = true;

                db.c_periodo_indicador.Add(c_periodo_indicador);
                db.SaveChanges();
                Utilidades.Notification.NuevoPeriodoIndicadores();
                return RedirectToAction("Index");
            }

            return View(c_periodo_indicador);
        }

        // GET: PeriodoIndicador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_periodo_indicador c_periodo_indicador = db.c_periodo_indicador.Find(id);
            if (c_periodo_indicador == null)
            {
                return HttpNotFound();
            }

            return View(c_periodo_indicador);
        }

        // POST: PeriodoIndicador/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_periodo_indicador,cl_periodo_indicador,nb_periodo_indicador,esta_activo")] c_periodo_indicador c_periodo_indicador)
        {
            if (ModelState.IsValid)
            {
                if (c_periodo_indicador.esta_activo && db.c_periodo_indicador.Any(i => i.esta_activo && i.id_periodo_indicador != c_periodo_indicador.id_periodo_indicador))
                {
                    c_periodo_indicador actual;
                    try
                    {
                        actual = db.c_periodo_indicador.Where(p => p.esta_activo == true).First();
                    }
                    catch
                    {
                        actual = null;
                    }

                    if (actual != null && actual.id_periodo_indicador != c_periodo_indicador.id_periodo_indicador)
                    {
                        actual.esta_activo = false;
                        db.Entry(actual).State = EntityState.Modified;
                        db.Entry(c_periodo_indicador).State = EntityState.Modified;
                        db.SaveChanges();
                        Utilidades.Notification.NuevoPeriodoIndicadores();
                    }
                    else
                    {
                        db.Entry(c_periodo_indicador).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.Entry(c_periodo_indicador).State = EntityState.Modified;
                    db.SaveChanges();
                }


                return RedirectToAction("Index");
            }
            return View(c_periodo_indicador);
        }

        // GET: PeriodoIndicador/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_periodo_indicador c_periodo_indicador = db.c_periodo_indicador.Find(id);
            if (c_periodo_indicador == null)
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

            //solo incluiremos evaluacion
            var r_evaluacion = db.k_evaluacion.Where(b => b.id_periodo_indicador == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_evaluacion.Count > 0)
            {
                foreach (var evaluacion in r_evaluacion)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Evaluación de Indicadores";
                    rr.cl_registro = evaluacion.id_evaluacion.ToString();
                    rr.nb_registro = "Evaluación del periodo " + evaluacion.c_periodo_indicador.nb_periodo_indicador;
                    rr.accion = "Delete";
                    rr.controlador = "EvaluacionIndicador";
                    rr.id_registro = evaluacion.id_evaluacion.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_periodo_indicador);
        }

        // POST: PeriodoIndicador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_periodo_indicador c_periodo_indicador = db.c_periodo_indicador.Find(id);
            db.c_periodo_indicador.Remove(c_periodo_indicador);
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
