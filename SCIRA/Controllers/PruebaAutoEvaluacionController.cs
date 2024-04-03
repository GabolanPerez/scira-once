using SCIRA.Models;
using SCIRA.Validaciones;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "PA", ModuleCode = "MSICI004")]
    [CustomErrorHandler]
    public class PruebaAutoEvaluacionController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: PruebaAutoEvaluacion
        public ActionResult Index()
        {
            var c_frecuencia_control = db.c_frecuencia_control;
            var c_naturaleza_control = db.c_naturaleza_control;
            c_prueba_auto_eval prueba;

            foreach (c_frecuencia_control freq in c_frecuencia_control)
            {
                foreach (c_naturaleza_control natu in c_naturaleza_control)
                {

                    if (!(db.c_prueba_auto_eval.Where(p => p.id_frecuencia_control == freq.id_frecuencia_control && p.id_naturaleza_control == natu.id_naturaleza_control).Any()))
                    {
                        prueba = new c_prueba_auto_eval();
                        prueba.id_frecuencia_control = freq.id_frecuencia_control;
                        prueba.id_naturaleza_control = natu.id_naturaleza_control;
                        prueba.no_partidas_minimo = 0;
                        prueba.no_partidas_semestre1 = 0;
                        prueba.no_partidas_semestre2 = 0;

                        db.c_prueba_auto_eval.Add(prueba);
                    }
                }
            }

            db.SaveChanges();

            var c_prueba_auto_eval = db.c_prueba_auto_eval.Include(c => c.c_frecuencia_control).Include(c => c.c_naturaleza_control);
            return View(c_prueba_auto_eval.ToList());
        }

        // GET: PruebaAutoEvaluacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_prueba_auto_eval c_prueba_auto_eval = db.c_prueba_auto_eval.Find(id);
            if (c_prueba_auto_eval == null)
            {
                return HttpNotFound();
            }
            return View(c_prueba_auto_eval);
        }

        // GET: PruebaAutoEvaluacion/Create
        public ActionResult Create()
        {
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "cl_frecuencia_control");
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "cl_naturaleza_control");
            return View();
        }

        // POST: PruebaAutoEvaluacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_naturaleza_control,id_frecuencia_control,no_partidas_minimo,no_partidas_semestre1,no_partidas_semestre2")] c_prueba_auto_eval c_prueba_auto_eval)
        {
            if (ModelState.IsValid)
            {
                db.c_prueba_auto_eval.Add(c_prueba_auto_eval);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "cl_frecuencia_control", c_prueba_auto_eval.id_frecuencia_control);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "cl_naturaleza_control", c_prueba_auto_eval.id_naturaleza_control);
            return View(c_prueba_auto_eval);
        }

        // GET: PruebaAutoEvaluacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_prueba_auto_eval c_prueba_auto_eval = db.c_prueba_auto_eval.Find(id);
            if (c_prueba_auto_eval == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "cl_frecuencia_control", c_prueba_auto_eval.id_frecuencia_control);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "cl_naturaleza_control", c_prueba_auto_eval.id_naturaleza_control);
            return View(c_prueba_auto_eval);
        }

        // POST: PruebaAutoEvaluacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_naturaleza_control,id_frecuencia_control,no_partidas_minimo,no_partidas_semestre1,no_partidas_semestre2")] c_prueba_auto_eval c_prueba_auto_eval)
        {
            if (ModelState.IsValid)
            {
                c_prueba_auto_eval prueba = db.c_prueba_auto_eval.Where(p => p.id_frecuencia_control == c_prueba_auto_eval.id_frecuencia_control && p.id_naturaleza_control == c_prueba_auto_eval.id_naturaleza_control).First();
                prueba.no_partidas_minimo = c_prueba_auto_eval.no_partidas_minimo;
                prueba.no_partidas_semestre1 = c_prueba_auto_eval.no_partidas_semestre1;
                prueba.no_partidas_semestre2 = c_prueba_auto_eval.no_partidas_semestre2;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var c_prueba_auto_eval2 = db.c_prueba_auto_eval.Include(c => c.c_frecuencia_control).Include(c => c.c_naturaleza_control);
            return View("Index", c_prueba_auto_eval2.ToList());
        }

        // GET: PruebaAutoEvaluacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_prueba_auto_eval c_prueba_auto_eval = db.c_prueba_auto_eval.Find(id);
            if (c_prueba_auto_eval == null)
            {
                return HttpNotFound();
            }
            return View(c_prueba_auto_eval);
        }

        // POST: PruebaAutoEvaluacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_prueba_auto_eval c_prueba_auto_eval = db.c_prueba_auto_eval.Find(id);
            db.c_prueba_auto_eval.Remove(c_prueba_auto_eval);
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
