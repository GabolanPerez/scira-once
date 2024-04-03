using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "NivelesNormatividad", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class NivelNormatividadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: NivelNormatividad
        public ActionResult Index()
        {
            List<List<c_nivel_normatividad>> Niveles = new List<List<Models.c_nivel_normatividad>>();
            List<c_normatividad> normatividades = db.c_normatividad.ToList();
            foreach (var normatividad in normatividades)
            {
                List<c_nivel_normatividad> niveles = db.c_nivel_normatividad
                        .Where(n => n.id_normatividad == normatividad.id_normatividad)
                        .OrderBy(n => n.no_orden).ToList();
                Niveles.Add(niveles);

                foreach (var nivel in niveles)
                {

                }
            }
            ViewBag.Niveles = Niveles;

            return View();
        }

        // GET: NivelNormatividad/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_normatividad c_normatividad = db.c_normatividad.Find(id);
            if (c_normatividad == null)
            {
                return HttpNotFound();
            }
            c_nivel_normatividad model = new c_nivel_normatividad();
            model.id_normatividad = c_normatividad.id_normatividad;
            ViewBag.nb_normatividad = c_normatividad.nb_normatividad;

            return View(model);
        }

        // POST: NivelNormatividad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_nivel_normatividad c_nivel_normatividad)
        {
            c_normatividad normatividad = db.c_normatividad.Find(c_nivel_normatividad.id_normatividad);
            if (ModelState.IsValid)
            {
                c_nivel_normatividad.no_orden = (Int16)(normatividad.c_nivel_normatividad.Max(c => c.no_orden) + 1);

                db.c_nivel_normatividad.Add(c_nivel_normatividad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.nb_normatividad = normatividad.nb_normatividad;
            return View(c_nivel_normatividad);
        }

        // GET: NivelNormatividad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_nivel_normatividad c_nivel_normatividad = db.c_nivel_normatividad.Find(id);
            if (c_nivel_normatividad == null)
            {
                return HttpNotFound();
            }
            return View(c_nivel_normatividad);
        }

        // POST: NivelNormatividad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_nivel_normatividad,cl_nivel_normatividad,nb_nivel_normatividad,no_orden,id_normatividad")] c_nivel_normatividad c_nivel_normatividad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_nivel_normatividad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_nivel_normatividad);
        }

        // GET: NivelNormatividad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_nivel_normatividad c_nivel_normatividad = db.c_nivel_normatividad.Find(id);
            if (c_nivel_normatividad == null)
            {
                return HttpNotFound();
            }

            c_normatividad c_normatividad = db.c_normatividad.Find(c_nivel_normatividad.id_normatividad);
            ViewBag.nb_normatividad = c_normatividad.nb_normatividad;
            if (c_nivel_normatividad.no_orden == 0)
            {
                ViewBag.Error = Strings.getMSG("No se puede borrar el nivel 0 de la normatividad");
            }
            return View(c_nivel_normatividad);
        }

        // POST: NivelNormatividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_nivel_normatividad c_nivel_normatividad = db.c_nivel_normatividad.Find(id);

            if (c_nivel_normatividad.no_orden == 0)
            {
                c_normatividad c_normatividad = db.c_normatividad.Find(c_nivel_normatividad.id_normatividad);
                ViewBag.nb_normatividad = c_normatividad.nb_normatividad;
                ViewBag.Error = Strings.getMSG("No se puede borrar el nivel 0 de la normatividad");
                return View("Delete", c_nivel_normatividad);
            }

            if (Utilidades.DeleteActions.DeleteNivelNormatividadObjects(c_nivel_normatividad, db))
            {
                db.c_nivel_normatividad.Remove(c_nivel_normatividad);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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
