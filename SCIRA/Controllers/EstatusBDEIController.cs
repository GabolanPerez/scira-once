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
    [Access(Funcion = "EstatusInc", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class EstatusBDEIController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: c_estatus_bdei
        public ActionResult Index()
        {
            return View(db.c_estatus_bdei.Where(r => r.esta_activo ?? false).ToList());
        }

        // GET: c_estatus_bdei/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_estatus_bdei/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_estatus_bdei,cl_estatus_bdei,nb_estatus_bdei")] c_estatus_bdei c_estatus_bdei)
        {
            if (ModelState.IsValid)
            {
                c_estatus_bdei.esta_activo = true;
                db.c_estatus_bdei.Add(c_estatus_bdei);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_estatus_bdei);
        }

        // GET: c_estatus_bdei/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_estatus_bdei c_estatus_bdei = db.c_estatus_bdei.Find(id);
            if (c_estatus_bdei == null)
            {
                return HttpNotFound();
            }
            return View(c_estatus_bdei);
        }

        // POST: c_estatus_bdei/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_estatus_bdei,cl_estatus_bdei,nb_estatus_bdei")] c_estatus_bdei c_estatus_bdei)
        {
            if (ModelState.IsValid)
            {
                c_estatus_bdei.esta_activo = true;
                db.Entry(c_estatus_bdei).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_estatus_bdei);
        }

        // GET: c_estatus_bdei/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_estatus_bdei c_estatus_bdei = db.c_estatus_bdei.Find(id);
            if (c_estatus_bdei == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();


            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_estatus_bdei);
        }

        // POST: c_estatus_bdei/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_estatus_bdei c_estatus_bdei = db.c_estatus_bdei.Find(id);
            c_estatus_bdei.esta_activo = false;
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
