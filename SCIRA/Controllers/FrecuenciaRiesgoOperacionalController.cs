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
    [Access(Funcion = "FrecuenciaRO", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class FrecuenciaRiesgoOperacionalController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: FrecuenciaRiesgoOperacional
        public ActionResult Index()
        {
            return View(db.c_frecuencia_riesgo_operacional.Where(r => r.esta_activo ?? false).ToList());
        }

        #region Create
        // GET: FrecuenciaRiesgoOperacional/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FrecuenciaRiesgoOperacional/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_frecuencia_riesgo_operacional c_frecuencia_riesgo_operacional)
        {
            if (ModelState.IsValid)
            {
                c_frecuencia_riesgo_operacional.esta_activo = true;
                db.c_frecuencia_riesgo_operacional.Add(c_frecuencia_riesgo_operacional);
                db.SaveChanges();

                //crear criticidades
                Utilidades.Utilidades.ValidateCRR();
                var impactos = db.c_impacto_riesgo_operacional.ToList();
                foreach (var impacto in impactos) {
                    var criticidad = new c_criticidad_ro {
                        id_frecuencia_riesgo_operacional = c_frecuencia_riesgo_operacional.id_frecuencia_riesgo_operacional,
                        id_impacto_riesgo_operacional = impacto.id_impacto_riesgo_operacional,
                        id_criticidad_riesgo_ro = Utilidades.Utilidades.idOfFirsCritRO(db)
                    };

                    db.c_criticidad_ro.Add(criticidad);
                }

                db.SaveChanges();




                return RedirectToAction("Index");
            }

            return View(c_frecuencia_riesgo_operacional);
        }
        #endregion

        #region Edit
        // GET: FrecuenciaRiesgoOperacional/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_frecuencia_riesgo_operacional c_frecuencia_riesgo_operacional = db.c_frecuencia_riesgo_operacional.Find(id);
            if (c_frecuencia_riesgo_operacional == null)
            {
                return HttpNotFound();
            }
            return View(c_frecuencia_riesgo_operacional);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_frecuencia_riesgo_operacional c_frecuencia_riesgo_operacional)
        {
            if (ModelState.IsValid)
            {
                c_frecuencia_riesgo_operacional.esta_activo = true;
                db.Entry(c_frecuencia_riesgo_operacional).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_frecuencia_riesgo_operacional);
        }
        #endregion

        #region Delete
        // GET: FrecuenciaRiesgoOperacional/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_frecuencia_riesgo_operacional c_frecuencia_riesgo_operacional = db.c_frecuencia_riesgo_operacional.Find(id);
            if (c_frecuencia_riesgo_operacional == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();



            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;
            return View(c_frecuencia_riesgo_operacional);
        }

        // POST: FrecuenciaRiesgoOperacional/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_frecuencia_riesgo_operacional c_frecuencia_riesgo_operacional = db.c_frecuencia_riesgo_operacional.Find(id);

            //Utilidades.DeleteActions.DeleteFrecuenciaROObjects(c_frecuencia_riesgo_operacional, db);
            c_frecuencia_riesgo_operacional.esta_activo = false;
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
