using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "CPlRem", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class ConclusionPlanController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult Index()
        {
            var User = (IdentityPersonalizado)HttpContext.User.Identity;
            int id = User.Id_usuario;
            var usuario = db.c_usuario.Find(id);

            var planes = Utilidades.Utilidades.RTCObject(usuario, db, "k_plan").Cast<k_plan>().ToList();
            //var planes = db.k_plan.Where(p => p.id_responsable == User.Id_usuario).ToList();

            //if (User.Es_super_usuario)
            //{
            //    planes = db.k_plan.ToList();
            //    ViewBag.su = 1;
            //}


            return View(planes);
        }

        public ActionResult ConcluirPlan(int id)
        {
            var model = new r_conclusion_plan();
            var plan = db.k_plan.Find(id);

            model.id_plan = id;
            ViewBag.Plan = plan;

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult ConcluirPlan(r_conclusion_plan model, int[] files)
        {
            var plan = db.k_plan.Find(model.id_plan);
            plan.fe_real_solucion = model.fe_conclusion;

            if (ModelState.IsValid)
            {
                db.Entry(plan).State = EntityState.Modified;
                db.r_conclusion_plan.Add(model);
                db.SaveChanges();

                model = db.r_conclusion_plan.Find(model.id_conclusion_plan);
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


                Utilidades.Utilidades.refreshNotifCount(plan.id_responsable);
                Utilidades.Utilidades.removeRow(6, plan.id_plan, plan.id_responsable);
                return RedirectToAction("Index");
            }

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.Plan = plan;
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var model = db.r_conclusion_plan.Find(id);
            var plan = model.k_plan;

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.Plan = plan;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(r_conclusion_plan model, int[] files)
        {
            var plan = db.k_plan.Find(model.id_plan);
            //Si se edita la respuesta, la fecha de respuesta cambia
            plan.fe_real_solucion = model.fe_conclusion;

            if (ModelState.IsValid)
            {
                db.Entry(plan).State = EntityState.Modified;
                db.Entry(model).State = EntityState.Modified;


                model = db.r_conclusion_plan.Find(model.id_conclusion_plan);
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

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.Plan = plan;

            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            var model = db.r_conclusion_plan.Find(id);
            var plan = model.k_plan;

            ViewBag.Plan = plan;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete(int id)
        {
            var model = db.r_conclusion_plan.Find(id);
            var plan = db.k_plan.Find(model.id_plan);
            plan.fe_real_solucion = null;

            Utilidades.DeleteActions.DeleteConclusionPlanObjects(model, db);

            db.Entry(plan).State = EntityState.Modified;
            db.r_conclusion_plan.Remove(model);
            db.SaveChanges();

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
