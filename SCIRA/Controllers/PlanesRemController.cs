using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "PlRem", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class PlanesRemController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult Index()
        {
            var User = (IdentityPersonalizado)HttpContext.User.Identity;
            int id = User.Id_usuario;


            var usuario = db.c_usuario.Find(id);


            var planes = Utilidades.Utilidades.RTCObject(usuario, db, "k_plan", "1").Cast<k_plan>().ToList();
            //var planes = db.k_plan.Where(p => p.id_responsable_seguimiento == id).ToList();

            //if (User.Es_super_usuario)
            //{
            //    planes = db.k_plan.ToList();
            //    ViewBag.su = 1;
            //}


            

            return View(planes);
        }

        public ActionResult Seguimientos(int id)
        {
            var plan = db.k_plan.Find(id);
            var seguimientos = plan.r_seguimiento.ToList();

            ViewBag.nb_plan = plan.nb_plan;
            ViewBag.id_plan = plan.id_plan;

            return View(seguimientos);
        }

        #region Create
        public ActionResult Create(int? id)
        {
            var plan = db.k_plan.Find(id);
            var incidencia = plan.k_incidencia;

            ViewBag.Plan = plan;
            ViewBag.Incidencia = incidencia;
            var model = new r_seguimiento();
            model.id_plan = plan.id_plan;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(r_seguimiento model, int[] files)
        {
            model.fe_seguimiento = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.r_seguimiento.Add(model);
                db.SaveChanges();

                model = db.r_seguimiento.Find(model.id_seguimiento);

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("Seguimientos", new { id = model.id_plan });
            }

            var plan = db.k_plan.Find(model.id_plan);
            var incidencia = plan.k_incidencia;

            ViewBag.Plan = plan;
            ViewBag.Incidencia = incidencia;

            return View(model);
        }
        #endregion


        #region Edit
        public ActionResult Edit(int? id)
        {
            var model = db.r_seguimiento.Find(id);
            var plan = db.k_plan.Find(model.id_plan);
            var incidencia = plan.k_incidencia;

            ViewBag.Plan = plan;
            ViewBag.Incidencia = incidencia;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(r_seguimiento model, int[] files)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                model = db.r_seguimiento.Find(model.id_seguimiento);

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }
                return RedirectToAction("Seguimientos", new { id = model.id_plan });
            }

            var plan = db.k_plan.Find(model.id_plan);
            var incidencia = plan.k_incidencia;

            ViewBag.Plan = plan;
            ViewBag.Incidencia = incidencia;

            return View(model);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var seguimiento = db.r_seguimiento.Find(id);
            if (seguimiento == null)
            {
                return HttpNotFound();
            }

            return View(seguimiento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete(int id)
        {
            var seguimiento = db.r_seguimiento.Find(id);
            int id_plan = seguimiento.id_plan;

            Utilidades.DeleteActions.DeleteSeguimientoObjects(seguimiento, db);

            db.r_seguimiento.Remove(seguimiento);
            db.SaveChanges();

            return RedirectToAction("Seguimientos", new { id = id_plan });
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
