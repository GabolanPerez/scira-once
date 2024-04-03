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
    public class BDEIR2812Controller : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var model = db.kg_r2812;
            ViewBag.entidadesL = Utilidades.DropDown.Entidades();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = db.kg_r2812.Find(id);
            
            return View(model);
        }


        [HttpPost,NotOnlyRead]
        public ActionResult Generate(int anno,int id_entidad)
        {
            var user = (IdentityPersonalizado)User.Identity;

            db.genera_r2812(0, anno,user.Nb_usuario, id_entidad);

            return RedirectToAction("Index");
        }

        [HttpPost, NotOnlyRead]
        public ActionResult ReGenerate(int id)
        {
            var user = (IdentityPersonalizado)User.Identity;

            db.genera_r2812(id, 0, user.Nb_usuario,0);

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
