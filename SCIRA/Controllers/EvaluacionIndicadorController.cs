using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
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
    [Access(Funcion = "EIND", ModuleCode = "MSICI001")]
    [CustomErrorHandler]
    public class EvaluacionIndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Evaluacion
        public ActionResult Index()
        {
            var User = (IdentityPersonalizado)HttpContext.User.Identity;
            int id = User.Id_usuario;
            var usuario = db.c_usuario.Find(id);


            List<c_indicador> lista = Utilidades.Utilidades.RTCObject(usuario,db, "c_indicador").Cast<c_indicador>().ToList().Where(i=>i.esta_activo).ToList();

            //if (user.Es_super_usuario) lista = db.c_indicador.Where(i => i.esta_activo).ToList();
            //else lista = db.c_indicador.Where(i => i.esta_activo && i.id_responsable == user.Id_usuario).ToList();

            return View(lista);
        }



        // GET: Evaluacion
        public ActionResult Evaluaciones(int? id)
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

            ViewBag.nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
            ViewBag.id = id;
            var k_evaluacion = db.k_evaluacion.Include(k => k.c_calificacion_indicador).Include(k => k.c_indicador).Include(k => k.c_periodo_indicador).Where(e => e.id_indicador == id);
            return View(k_evaluacion.ToList());
        }

        // GET: Evaluacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_evaluacion k_evaluacion = db.k_evaluacion.Find(id);
            if (k_evaluacion == null)
            {
                return HttpNotFound();
            }
            return View(k_evaluacion);
        }

        // GET: Evaluacion/Create
        public ActionResult Create(int? id)
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

            ViewBag.nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
            ViewBag.nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
            ViewBag.descripcion = c_indicador.ds_indicador;
            ViewBag.descripcion_nume = c_indicador.ds_numerador;
            ViewBag.descripcion_denum = c_indicador.ds_denominador;
            ViewBag.frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
            ViewBag.unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
            try
            {
                ViewBag.control_asociado = c_indicador.k_control.relacion_control;
            }
            catch
            {
                ViewBag.control_asociado = Strings.getMSG("No cuenta con un control asociado.");
            }
            ViewBag.peso = c_indicador.peso;
            ViewBag.u000i = c_indicador.umbral000i;
            ViewBag.u000f = c_indicador.umbral000f;
            ViewBag.u050i = c_indicador.umbral050i;
            ViewBag.u050f = c_indicador.umbral050f;
            ViewBag.u075i = c_indicador.umbral075i;
            ViewBag.u075f = c_indicador.umbral075f;
            ViewBag.u100i = c_indicador.umbral100i;
            ViewBag.u100f = c_indicador.umbral100f;
            ViewBag.area = c_indicador.c_area.nb_area;
            ViewBag.responsable = c_indicador.c_usuario.nb_usuario;

            k_evaluacion k_evaluacion = new k_evaluacion();
            k_evaluacion.id_indicador = c_indicador.id_indicador;

            try
            {
                c_periodo_indicador c_periodo_indicador = db.c_periodo_indicador.Where(p => p.esta_activo == true).First();
                ViewBag.nb_periodo_indicador = c_periodo_indicador.nb_periodo_indicador;
                k_evaluacion.id_periodo_indicador = c_periodo_indicador.id_periodo_indicador;
            }
            catch
            {
                ViewBag.nb_periodo_indicador = Strings.getMSG("EvaluacionIndicadorCreate006");
                ViewBag.Error = Strings.getMSG("CertificacionCertificar034");
            }

            return View(k_evaluacion);
        }

        // POST: Evaluacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_evaluacion,id_indicador,id_periodo_indicador,numerador,denominador")] k_evaluacion k_evaluacion)
        {

            c_indicador c_indicador = db.c_indicador.Find(k_evaluacion.id_indicador);
            if (ModelState.IsValid && k_evaluacion.id_periodo_indicador > 0)
            {
                decimal num = (decimal)k_evaluacion.numerador;
                decimal den = (decimal)k_evaluacion.denominador;
                decimal medicion = 0;
                if (den > 0)
                {
                    medicion = (num * 100) / den;
                    k_evaluacion.medicion = medicion;
                }

                c_calificacion_indicador calificacion;


                if ((medicion >= c_indicador.umbral000i && medicion <= c_indicador.umbral000f) || (medicion >= c_indicador.umbral050i && medicion <= c_indicador.umbral050f))  //Esta en umbral 0.0 o en el umbral 0.0 - 5.0?
                {
                    calificacion = getGrade("Alerta", "A");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion >= c_indicador.umbral075i && medicion <= c_indicador.umbral075f)  //Esta en umbral 5.0 - 7.5?
                {
                    calificacion = getGrade("Regular", "R");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion >= c_indicador.umbral100i && medicion <= c_indicador.umbral100f)  //Esta en umbral 7.5 - 10.0?
                {
                    calificacion = getGrade("Bueno", "B");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion > 100 || medicion < 0)
                {
                    calificacion = getGrade("Fuera de rango", "F");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }

                db.k_evaluacion.Add(k_evaluacion);
                db.SaveChanges();

                Utilidades.Utilidades.refreshNotifCount((int)c_indicador.id_responsable);
                Utilidades.Utilidades.removeRow(2, c_indicador.id_indicador, (int)c_indicador.id_responsable);

                return RedirectToAction("Evaluaciones", new { id = c_indicador.id_indicador });
            }

            ViewBag.nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
            ViewBag.nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
            ViewBag.descripcion = c_indicador.ds_indicador;
            ViewBag.descripcion_nume = c_indicador.ds_numerador;
            ViewBag.descripcion_denum = c_indicador.ds_denominador;
            ViewBag.frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
            ViewBag.unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
            try
            {
                ViewBag.control_asociado = c_indicador.k_control.relacion_control;
            }
            catch
            {
                ViewBag.control_asociado = Strings.getMSG("No cuenta con un control asociado.");
            }
            ViewBag.peso = c_indicador.peso;
            ViewBag.u000i = c_indicador.umbral000i;
            ViewBag.u000f = c_indicador.umbral000f;
            ViewBag.u050i = c_indicador.umbral050i;
            ViewBag.u050f = c_indicador.umbral050f;
            ViewBag.u075i = c_indicador.umbral075i;
            ViewBag.u075f = c_indicador.umbral075f;
            ViewBag.u100i = c_indicador.umbral100i;
            ViewBag.u100f = c_indicador.umbral100f;
            ViewBag.area = c_indicador.c_area.nb_area;
            ViewBag.responsable = c_indicador.c_usuario.nb_usuario;

            try
            {
                c_periodo_indicador c_periodo_indicador = db.c_periodo_indicador.Where(p => p.esta_activo == true).First();
                ViewBag.nb_periodo_indicador = c_periodo_indicador.nb_periodo_indicador;
                k_evaluacion.id_periodo_indicador = c_periodo_indicador.id_periodo_indicador;
            }
            catch
            {
                ViewBag.nb_periodo_indicador = Strings.getMSG("EvaluacionIndicadorCreate006");
                ViewBag.Error = Strings.getMSG("CertificacionCertificar034");
            }

            return View(k_evaluacion);
        }

        // GET: Evaluacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_evaluacion k_evaluacion = db.k_evaluacion.Find(id);
            if (k_evaluacion == null)
            {
                return HttpNotFound();
            }
            if (!(k_evaluacion.c_periodo_indicador.esta_activo))
            {
                return RedirectToAction("Evaluaciones", new { id = k_evaluacion.id_indicador });
            }

            c_indicador c_indicador = db.c_indicador.Find(k_evaluacion.id_indicador);
            c_periodo_indicador periodo = db.c_periodo_indicador.Find(k_evaluacion.id_periodo_indicador);

            ViewBag.nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
            ViewBag.nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
            ViewBag.descripcion = c_indicador.ds_indicador;
            ViewBag.descripcion_nume = c_indicador.ds_numerador;
            ViewBag.descripcion_denum = c_indicador.ds_denominador;
            ViewBag.frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
            ViewBag.unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;

            try
            {
                ViewBag.control_asociado = c_indicador.k_control.relacion_control;
            }
            catch
            {
                ViewBag.control_asociado = Strings.getMSG("No cuenta con un control asociado.");
            }
            ViewBag.peso = c_indicador.peso;
            ViewBag.u000i = c_indicador.umbral000i;
            ViewBag.u000f = c_indicador.umbral000f;
            ViewBag.u050i = c_indicador.umbral050i;
            ViewBag.u050f = c_indicador.umbral050f;
            ViewBag.u075i = c_indicador.umbral075i;
            ViewBag.u075f = c_indicador.umbral075f;
            ViewBag.u100i = c_indicador.umbral100i;
            ViewBag.u100f = c_indicador.umbral100f;
            ViewBag.area = c_indicador.c_area.nb_area;
            ViewBag.responsable = c_indicador.c_usuario.nb_usuario;
            ViewBag.nb_periodo_indicador = periodo.nb_periodo_indicador;
            return View(k_evaluacion);
        }

        // POST: Evaluacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(k_evaluacion k_evaluacion)
        {
            c_indicador c_indicador = db.c_indicador.Find(k_evaluacion.id_indicador);

            if (ModelState.IsValid)
            {
                decimal num = (decimal)k_evaluacion.numerador;
                decimal den = (decimal)k_evaluacion.denominador;
                decimal medicion = 0;
                if (den > 0)
                {
                    medicion = (num * 100) / den;
                    k_evaluacion.medicion = medicion;
                }

                c_calificacion_indicador calificacion;


                if (medicion >= c_indicador.umbral000i && medicion <= c_indicador.umbral000f)  //Esta en umbral 0.0?
                {
                    calificacion = getGrade("Alerta", "A");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion >= c_indicador.umbral050i && medicion <= c_indicador.umbral050f)  //Esta en umbral 0.0 - 5.0?
                {
                    calificacion = getGrade("Alerta", "A");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion >= c_indicador.umbral075i && medicion <= c_indicador.umbral075f)  //Esta en umbral 5.0 - 7.5?
                {
                    calificacion = getGrade("Regular", "R");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion >= c_indicador.umbral100i && medicion <= c_indicador.umbral100f)  //Esta en umbral 7.5 - 10.0?
                {
                    calificacion = getGrade("Bueno", "B");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }
                if (medicion > 100 || medicion < 0) {
                    calificacion = getGrade("Fuera de rango", "F");
                    k_evaluacion.id_calificacion_indicador = calificacion.id_calificacion_indicador;
                }


                db.Entry(k_evaluacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Evaluaciones", new { id = k_evaluacion.id_indicador });
            }

            c_periodo_indicador periodo = db.c_periodo_indicador.Find(k_evaluacion.id_periodo_indicador);

            ViewBag.nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
            ViewBag.nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
            ViewBag.descripcion = c_indicador.ds_indicador;
            ViewBag.descripcion_nume = c_indicador.ds_numerador;
            ViewBag.descripcion_denum = c_indicador.ds_denominador;
            ViewBag.frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
            ViewBag.unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
            try
            {
                ViewBag.control_asociado = c_indicador.k_control.relacion_control;
            }
            catch
            {
                ViewBag.control_asociado = Strings.getMSG("No cuenta con un control asociado.");
            }
            ViewBag.peso = c_indicador.peso;
            ViewBag.u000i = c_indicador.umbral000i;
            ViewBag.u000f = c_indicador.umbral000f;
            ViewBag.u050i = c_indicador.umbral050i;
            ViewBag.u050f = c_indicador.umbral050f;
            ViewBag.u075i = c_indicador.umbral075i;
            ViewBag.u075f = c_indicador.umbral075f;
            ViewBag.u100i = c_indicador.umbral100i;
            ViewBag.u100f = c_indicador.umbral100f;
            ViewBag.area = c_indicador.c_area.nb_area;
            ViewBag.responsable = c_indicador.c_usuario.nb_usuario;
            ViewBag.nb_periodo_indicador = periodo.nb_periodo_indicador;
            return View(k_evaluacion);
        }

        // GET: Evaluacion/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_evaluacion k_evaluacion = db.k_evaluacion.Find(id);
            if (k_evaluacion == null)
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


            return View(k_evaluacion);
        }

        // POST: Evaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            k_evaluacion k_evaluacion = db.k_evaluacion.Find(id);
            int id_indicador = (int)k_evaluacion.id_indicador;
            db.k_evaluacion.Remove(k_evaluacion);
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
                return RedirectToAction("Evaluaciones", new { id = id_indicador });

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
                    return RedirectToAction("Evaluaciones", new { id = id_indicador });
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

        private c_calificacion_indicador getGrade(string name, string clave)
        {
            c_calificacion_indicador calificacion;
            try
            {
                calificacion = db.c_calificacion_indicador.Where(c => c.nb_calificacion_indicador == name).First();
            }
            catch
            {
                calificacion = new c_calificacion_indicador();
                calificacion.cl_calificacion_indicador = clave;
                calificacion.nb_calificacion_indicador = name;
                db.c_calificacion_indicador.Add(calificacion);
                db.SaveChanges();
            }
            return calificacion;
        }
    }
}
