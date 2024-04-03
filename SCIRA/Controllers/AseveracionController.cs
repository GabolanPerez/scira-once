using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Aseveracion", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class AseveracionController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        #region Index
        public ActionResult Index()
        {
            return View(db.c_aseveracion.ToList());
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Create(c_aseveracion model)
        {
            if (ModelState.IsValid)
            {
                db.c_aseveracion.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_aseveracion c_aseveracion = db.c_aseveracion.Find(id);
            if (c_aseveracion == null)
            {
                return HttpNotFound();
            }
            return View(c_aseveracion);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Edit(c_aseveracion model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            c_aseveracion c_aseveracion = db.c_aseveracion.Find(id);

            if (c_aseveracion == null)
            {
                return HttpNotFound();
            }

            DeleteActions.checkRedirect(redirect);

            return View(c_aseveracion);
        }

        // POST: Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_aseveracion c_aseveracion = db.c_aseveracion.Find(id);
            DeleteActions.DeleteAseveracionObjects(c_aseveracion, db);


            db.c_aseveracion.Remove(c_aseveracion);
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

        #region Otros
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
