using SCIRA.Models;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "MCRiesgo", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class MCRiesgoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: MCRiesgo
        public ActionResult Index()
        {
            var c_meta_campo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            return View(c_meta_campo);
        }

        // GET: MCRiesgo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_meta_campo c_meta_campo = db.c_meta_campo.Find(id);
            if (c_meta_campo == null)
            {
                return HttpNotFound();
            }
            var colores = Utilidades.Utilidades.ColoresMetaCampos();
            var cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_meta_campo);
        }

        // POST: MCRiesgo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_meta_campo,nb_entidad,cl_color_borde,cl_color_fondo,cl_tipo_campo,es_editable,es_requerido,es_visible,longitud_campo,nb_campo,cl_campo,aparece_en_mg,msg_ayuda")] c_meta_campo c_meta_campo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c_meta_campo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            IList<SelectListItem> colores = Utilidades.Utilidades.ColoresMetaCampos();
            IList<SelectListItem> cl_tipo_campo = Utilidades.Utilidades.TiposCampo();

            ViewBag.cl_tipo_campo = cl_tipo_campo;
            ViewBag.cl_color_borde = colores;
            ViewBag.cl_color_fondo = colores;

            return View(c_meta_campo);
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
