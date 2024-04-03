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
    [Access(Funcion = "Indicadores", ModuleCode = "MSICI001")]
    [CustomErrorHandler]
    public class IndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Indicador
        public ActionResult Index()
        {
            var c_indicador = db.c_indicador.Include(c => c.c_area).Include(c => c.c_entidad).Include(c => c.c_frecuencia_indicador).Include(c => c.c_usuario).Include(c => c.c_unidad_indicador);
            return View(c_indicador.ToList());
        }

        #region Agregar

        // GET: Indicador/Create
        public ActionResult Create()
        {
            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area");
            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad");
            ViewBag.id_frecuencia_indicador = new SelectList(db.c_frecuencia_indicador, "id_frecuencia_indicador", "nb_frecuencia_indicador");
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo), "id_usuario", "nb_usuario");
            ViewBag.id_unidad_indicador = new SelectList(db.c_unidad_indicador, "id_unidad_indicador", "nb_unidad_indicador");
            ViewBag.id_control = new SelectList(db.k_control.Where(c => !c.tiene_accion_correctora), "id_control", "relacion_control");

            ViewBag.Model = "null";

            var model = new c_indicador();
            return View();
        }

        // POST: Indicador/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_indicador c_indicador)
        {

            if (ModelState.IsValid)
            {
                db.c_indicador.Add(c_indicador);
                db.SaveChanges();

                Utilidades.Utilidades.TaskAsigned(c_indicador);
                return RedirectToAction("Index");
            }

            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area", c_indicador.id_area);
            ViewBag.id_entidad = new SelectList(db.c_entidad, "id_entidad", "nb_entidad", c_indicador.id_entidad);
            ViewBag.id_frecuencia_indicador = new SelectList(db.c_frecuencia_indicador, "id_frecuencia_indicador", "nb_frecuencia_indicador", c_indicador.id_frecuencia_indicador);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo), "id_usuario", "nb_usuario", c_indicador.id_responsable);
            ViewBag.id_unidad_indicador = new SelectList(db.c_unidad_indicador, "id_unidad_indicador", "nb_unidad_indicador", c_indicador.id_unidad_indicador);
            ViewBag.id_control = new SelectList(db.k_control.Where(c => !c.tiene_accion_correctora), "id_control", "relacion_control", c_indicador.id_control);

            return View(c_indicador);
        }
        #endregion

        #region Editar
        // GET: Indicador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_indicador c_indicador = db.c_indicador.Find(id);
            if (c_indicador == null)
            {
                return HttpNotFound();
            }
            var entidad = db.c_entidad.Find(c_indicador.id_entidad);

            ViewBag.cnb_entidad = entidad.cl_entidad + " - " + entidad.nb_entidad;
            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area", c_indicador.id_area);
            ViewBag.id_frecuencia_indicador = new SelectList(db.c_frecuencia_indicador, "id_frecuencia_indicador", "nb_frecuencia_indicador", c_indicador.id_frecuencia_indicador);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo), "id_usuario", "nb_usuario", c_indicador.id_responsable);
            ViewBag.id_unidad_indicador = new SelectList(db.c_unidad_indicador, "id_unidad_indicador", "nb_unidad_indicador", c_indicador.id_unidad_indicador);
            ViewBag.id_control = new SelectList(db.k_control.Where(c => !c.tiene_accion_correctora), "id_control", "relacion_control", c_indicador.id_control);
            ViewBag.lu = c_indicador.id_responsable;

            return View(c_indicador);
        }



        // POST: Indicador/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_indicador c_indicador, int lu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_indicador).State = EntityState.Modified;
                db.SaveChanges();

                if (lu != c_indicador.id_responsable) Utilidades.Utilidades.TaskAsigned(c_indicador, lu);

                return RedirectToAction("Index");
            }

            var entidad = db.c_entidad.Find(c_indicador.id_entidad);

            ViewBag.cnb_entidad = entidad.cl_entidad + " - " + entidad.nb_entidad;
            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area", c_indicador.id_area);
            ViewBag.id_frecuencia_indicador = new SelectList(db.c_frecuencia_indicador, "id_frecuencia_indicador", "nb_frecuencia_indicador", c_indicador.id_frecuencia_indicador);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo), "id_usuario", "nb_usuario", c_indicador.id_responsable);
            ViewBag.id_unidad_indicador = new SelectList(db.c_unidad_indicador, "id_unidad_indicador", "nb_unidad_indicador", c_indicador.id_unidad_indicador);
            ViewBag.id_control = new SelectList(db.k_control.Where(c => !c.tiene_accion_correctora), "id_control", "relacion_control", c_indicador.id_control);
            ViewBag.lu = lu;


            return View(c_indicador);
        }
        #endregion

        #region Borrar

        // GET: Indicador/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_indicador c_indicador = db.c_indicador.Find(id);
            if (c_indicador == null)
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
            var r_evaluacion = db.k_evaluacion.Where(b => b.id_indicador == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_evaluacion.Count > 0)
            {
                foreach (var evaluacion in r_evaluacion)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Evaluacion de Indicadores";
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



            return View(c_indicador);
        }

        // POST: Indicador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_indicador c_indicador = db.c_indicador.Find(id);
            db.c_indicador.Remove(c_indicador);
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
