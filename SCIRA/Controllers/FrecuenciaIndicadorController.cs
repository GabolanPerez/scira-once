using SCIRA.Models;
using SCIRA.Validaciones;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "FrecuenciaIndicador", ModuleCode = "MSICI001")]
    [CustomErrorHandler]
    public class FrecuenciaIndicadorController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: FrecuenciaIndicador
        public ActionResult Index()
        {
            return View(db.c_frecuencia_indicador.ToList());
        }

        // GET: FrecuenciaIndicador/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_frecuencia_indicador c_frecuencia_indicador = db.c_frecuencia_indicador.Find(id);
            if (c_frecuencia_indicador == null)
            {
                return HttpNotFound();
            }
            return View(c_frecuencia_indicador);
        }

        // GET: FrecuenciaIndicador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FrecuenciaIndicador/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_frecuencia_indicador,cl_frecuencia_indicador,nb_frecuencia_indicador")] c_frecuencia_indicador c_frecuencia_indicador)
        {
            if (ModelState.IsValid)
            {
                db.c_frecuencia_indicador.Add(c_frecuencia_indicador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_frecuencia_indicador);
        }

        // GET: FrecuenciaIndicador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_frecuencia_indicador c_frecuencia_indicador = db.c_frecuencia_indicador.Find(id);
            if (c_frecuencia_indicador == null)
            {
                return HttpNotFound();
            }
            return View(c_frecuencia_indicador);
        }

        // POST: FrecuenciaIndicador/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_frecuencia_indicador,cl_frecuencia_indicador,nb_frecuencia_indicador")] c_frecuencia_indicador c_frecuencia_indicador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_frecuencia_indicador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_frecuencia_indicador);
        }

        // GET: FrecuenciaIndicador/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_frecuencia_indicador c_frecuencia_indicador = db.c_frecuencia_indicador.Find(id);
            if (c_frecuencia_indicador == null)
            {
                return HttpNotFound();
            }
            return View(c_frecuencia_indicador);
        }

        // POST: FrecuenciaIndicador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_frecuencia_indicador c_frecuencia_indicador = db.c_frecuencia_indicador.Find(id);
            db.c_frecuencia_indicador.Remove(c_frecuencia_indicador);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
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
