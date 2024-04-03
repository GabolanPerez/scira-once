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
    [Access(Funcion = "MROBDEI", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class MinimoRiesgoOperativoBDEIController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            return View(db.c_minimo_riesgo_operativo.Where(r => r.esta_activo ?? false).ToList());
        }

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_minimo_riesgo_operativo c_minimo_riesgo_operativo)
        {
            if (ModelState.IsValid)
            {
                c_minimo_riesgo_operativo.esta_activo = true;
                db.c_minimo_riesgo_operativo.Add(c_minimo_riesgo_operativo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_minimo_riesgo_operativo);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_minimo_riesgo_operativo c_minimo_riesgo_operativo = db.c_minimo_riesgo_operativo.Find(id);
            if (c_minimo_riesgo_operativo == null)
            {
                return HttpNotFound();
            }
            return View(c_minimo_riesgo_operativo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_minimo_riesgo_operativo c_minimo_riesgo_operativo)
        {
            if (ModelState.IsValid)
            {
                c_minimo_riesgo_operativo.esta_activo = true;
                db.Entry(c_minimo_riesgo_operativo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_minimo_riesgo_operativo);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_minimo_riesgo_operativo c_minimo_riesgo_operativo = db.c_minimo_riesgo_operativo.Find(id);
            if (c_minimo_riesgo_operativo == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //solo incluiremos bdei
            //var r_bdei = db.k_bdei.Where(b => b.id_minimo_riesgo_operativo == id).ToList();

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

            return View(c_minimo_riesgo_operativo);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_minimo_riesgo_operativo c_minimo_riesgo_operativo = db.c_minimo_riesgo_operativo.Find(id);
            c_minimo_riesgo_operativo.esta_activo = false;
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
