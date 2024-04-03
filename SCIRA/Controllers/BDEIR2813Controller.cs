using SCIRA.Models;
using SCIRA.Seguridad;
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
    //[Access(Funcion = "BDEI", ModuleCode = "MSICI005")]
    //[CustomErrorHandler]
    public class BDEIR2813Controller : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var model = db.kg_r2813;

            ViewBag.trimestresL = Utilidades.DropDown.Trimestres();
            ViewBag.entidadesL = Utilidades.DropDown.Entidades();

            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = db.kg_r2813.Find(id);
            
            return View(model);
        }


        [HttpPost,NotOnlyRead]
        public ActionResult Generate(int periodo,int anno,int id_entidad)
        {
            //¿que pasa si se requiere generar un reporte en un periodo y año que ya existe?
            var user = (IdentityPersonalizado)User.Identity;

            db.genera_r2813(0, anno, periodo, user.Nb_usuario, id_entidad);

            return RedirectToAction("Index");
        }

        [HttpPost, NotOnlyRead]
        public ActionResult ReGenerate(int id)
        {
            //¿que pasa si se requiere generar un reporte en un periodo y año que ya existe?
            var user = (IdentityPersonalizado)User.Identity;

            db.genera_r2813(id, 0, 0, user.Nb_usuario,0);

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
