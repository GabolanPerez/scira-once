using SCIRA.Models;
using SCIRA.Validaciones;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "EstructuraManual", ModuleCode = "MSICI010")]
    [CustomErrorHandler]
    public class EstructuraManualController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult Index(int? id = 0)
        {
            var model = db.c_estructura_manual.ToList();

            if (id == 0)
            {
                try
                {
                    id = db.c_estructura_manual.First().id_estructura_manual;
                }
                catch
                {
                }
            }
            ViewBag.IDshow = id;
            return View(model);
        }

        #region Create  
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_estructura_manual c_estructura_manual)
        {
            if (ModelState.IsValid)
            {
                db.c_estructura_manual.Add(c_estructura_manual);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_estructura_manual);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_estructura_manual c_estructura_manual = db.c_estructura_manual.Find(id);
            if (c_estructura_manual == null)
            {
                return HttpNotFound();
            }
            return View(c_estructura_manual);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(c_estructura_manual c_estructura_manual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_estructura_manual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_estructura_manual);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_estructura_manual c_estructura_manual = db.c_estructura_manual.Find(id);
            if (c_estructura_manual == null)
            {
                return HttpNotFound();
            }

            return View(c_estructura_manual);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_estructura_manual c_estructura_manual = db.c_estructura_manual.Find(id);

            if (Utilidades.DeleteActions.DeleteEstructuraManualObjects(c_estructura_manual, db))
            {
                db.c_estructura_manual.Remove(c_estructura_manual);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region CreateLvl
        public ActionResult CreateLvl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_estructura_manual c_estructura_manual = db.c_estructura_manual.Find(id);
            if (c_estructura_manual == null)
            {
                return HttpNotFound();
            }
            c_nivel_manual model = new c_nivel_manual();


            model.id_estructura_manual = c_estructura_manual.id_estructura_manual;
            ViewBag.cl_estructura_manual = c_estructura_manual.cl_estructura_manual;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateLvl(c_nivel_manual c_nivel_manual)
        {
            c_estructura_manual c_estructura_manual = db.c_estructura_manual.Find(c_nivel_manual.id_estructura_manual);
            if (ModelState.IsValid)
            {
                try
                {
                    c_nivel_manual.no_orden = (Int16)(c_estructura_manual.c_nivel_manual.Max(c => c.no_orden) + 1);
                }
                catch
                {
                    c_nivel_manual.no_orden = 1;
                }


                db.c_nivel_manual.Add(c_nivel_manual);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = c_nivel_manual.id_estructura_manual });
            }

            ViewBag.cl_estructura_manual = c_estructura_manual.cl_estructura_manual;
            return View(c_nivel_manual);
        }
        #endregion

        #region EditLvl
        public ActionResult EditLvl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_nivel_manual c_nivel_manual = db.c_nivel_manual.Find(id);
            if (c_nivel_manual == null)
            {
                return HttpNotFound();
            }
            return View(c_nivel_manual);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditLvl(c_nivel_manual c_nivel_manual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_nivel_manual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = c_nivel_manual.id_estructura_manual });
            }
            return View(c_nivel_manual);
        }
        #endregion

        #region DeleteLvl
        public ActionResult DeleteLvl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_nivel_manual c_nivel_manual = db.c_nivel_manual.Find(id);
            if (c_nivel_manual == null)
            {
                return HttpNotFound();
            }

            c_estructura_manual c_estructura_manual = c_nivel_manual.c_estructura_manual;
            ViewBag.cl_estructura_manual = c_estructura_manual.cl_estructura_manual;
            return View(c_nivel_manual);
        }


        [HttpPost, ActionName("DeleteLvl")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteLvlConfirmed(int id)
        {
            c_nivel_manual c_nivel_manual = db.c_nivel_manual.Find(id);
            int id_estructura = c_nivel_manual.id_estructura_manual;


            if (Utilidades.DeleteActions.DeleteNivelManualObjects(c_nivel_manual, db))
            {
                db.c_nivel_manual.Remove(c_nivel_manual);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = id_estructura });
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
