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
    [Access(Funcion = "TipoSolucion", ModuleCode = "MSICI005")]
    [CustomErrorHandler]
    public class TipoSolucionController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: TipoSolucion
        public ActionResult Index()
        {
            return View(db.c_tipo_solucion.Where(r => r.esta_activo ?? false).ToList());
        }

        // GET: TipoSolucion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoSolucion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create([Bind(Include = "id_tipo_solucion,cl_tipo_solucion,nb_tipo_solucion")] c_tipo_solucion c_tipo_solucion)
        {
            if (ModelState.IsValid)
            {
                c_tipo_solucion.esta_activo = true;
                db.c_tipo_solucion.Add(c_tipo_solucion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_tipo_solucion);
        }

        // GET: TipoSolucion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipo_solucion c_tipo_solucion = db.c_tipo_solucion.Find(id);
            if (c_tipo_solucion == null)
            {
                return HttpNotFound();
            }
            return View(c_tipo_solucion);
        }

        // POST: TipoSolucion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_tipo_solucion,cl_tipo_solucion,nb_tipo_solucion")] c_tipo_solucion c_tipo_solucion)
        {
            if (ModelState.IsValid)
            {
                c_tipo_solucion.esta_activo = true;
                db.Entry(c_tipo_solucion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(c_tipo_solucion);
        }

        // GET: TipoSolucion/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_tipo_solucion c_tipo_solucion = db.c_tipo_solucion.Find(id);
            if (c_tipo_solucion == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect);

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();


            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(c_tipo_solucion);
        }

        // POST: TipoSolucion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_tipo_solucion c_tipo_solucion = db.c_tipo_solucion.Find(id);
            c_tipo_solucion.esta_activo = false;
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
