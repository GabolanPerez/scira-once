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
    [Access(Funcion = "ClaseEventoRO", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class ClaseEventoRiesgoOperacionalController : Controller
    {
        private SICIEntities db = new SICIEntities();
        //private ISelectListRepository _repository;

        public ActionResult Index()
        {
            return View(db.c_clase_evento.Where(r => r.esta_activo ?? false).ToList());
        }

        public ActionResult Create()
        {
            AgregarClaseEventoViewModel model = new AgregarClaseEventoViewModel();
            model.TipoRiesgoOperacional = Utilidades.DropDown.TipoRiesgoOperacional();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(AgregarClaseEventoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sp = new c_clase_evento
                {
                    cl_clase_evento = model.cl_clase_evento,
                    nb_clase_evento = model.nb_clase_evento,
                    id_sub_tipo_riesgo_operacional = model.id_sub_tipo_riesgo_operacional,
                    esta_activo = true
                };

                db.c_clase_evento.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            model.TipoRiesgoOperacional = Utilidades.DropDown.TipoRiesgoOperacional(model.id_tipo_riesgo_operacional);

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_clase_evento c_clase_evento = db.c_clase_evento.Find(id);
            if (c_clase_evento == null)
            {
                return HttpNotFound();
            }
            return View(c_clase_evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_clase_evento c_clase_evento)
        {
            if (ModelState.IsValid)
            {
                c_clase_evento.esta_activo = true;
                db.Entry(c_clase_evento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_clase_evento);
        }

        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_clase_evento c_clase_evento = db.c_clase_evento.Find(id);
            if (c_clase_evento == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            ////solo incluiremos bdei
            //var r_bdei = db.k_bdei.Where(b => b.id_clase_evento == id).ToList();

            ////creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            //if (r_bdei.Count > 0)
            //{
            //    foreach (var bdei in r_bdei)
            //    {
            //        RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
            //        rr.nb_catalogo = "BDEI";
            //        rr.cl_registro = bdei.id_bdei.ToString();
            //        rr.nb_registro = "BDEI ligado a la entidad: " + bdei.c_entidad.nb_entidad;
            //        rr.accion = "Delete";
            //        rr.controlador = "BDEI";
            //        rr.id_registro = bdei.id_bdei.ToString();

            //        RR.Add(rr);
            //    }
            //}

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_clase_evento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_clase_evento c_clase_evento = db.c_clase_evento.Find(id);
            c_clase_evento.esta_activo = false;

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
