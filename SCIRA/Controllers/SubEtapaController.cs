using SCIRA.Models;
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
    [Access(Funcion = "SubEtapas", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class SubEtapaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: SubEtapa
        public ActionResult Index()
        {
            return View(db.c_sub_etapa.ToList());
        }

        #region Create
        // GET: SubEtapa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_etapa c_sub_etapa = db.c_sub_etapa.Find(id);
            if (c_sub_etapa == null)
            {
                return HttpNotFound();
            }
            return View(c_sub_etapa);
        }

        // GET: SubEtapa/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: SubEtapa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_sub_etapa,cl_sub_etapa,nb_sub_etapa")] c_sub_etapa c_sub_etapa)
        {
            if (ModelState.IsValid)
            {
                db.c_sub_etapa.Add(c_sub_etapa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_sub_etapa);
        }
        #endregion

        #region Edit
        // GET: SubEtapa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_etapa c_sub_etapa = db.c_sub_etapa.Find(id);
            if (c_sub_etapa == null)
            {
                return HttpNotFound();
            }
            return View(c_sub_etapa);
        }

        // POST: SubEtapa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_sub_etapa,cl_sub_etapa,nb_sub_etapa")] c_sub_etapa c_sub_etapa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_sub_etapa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_sub_etapa);
        }
        #endregion

        #region Delete
        // GET: SubEtapa/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_etapa c_sub_etapa = db.c_sub_etapa.Find(id);
            if (c_sub_etapa == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_sub_etapa);
        }

        // POST: SubEtapa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_sub_etapa c_sub_etapa = db.c_sub_etapa.Find(id);

            Utilidades.DeleteActions.DeleteSubEtapasObjects(c_sub_etapa, db);

            db.c_sub_etapa.Remove(c_sub_etapa);
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
