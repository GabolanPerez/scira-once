using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "TiemposSP", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class TiemposSubProcesosController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var sps = db.c_sub_proceso.ToList();
            List<c_sub_proceso> model = new List<c_sub_proceso>();


            foreach (var sp in sps)
            {
                var usps = sp.c_usuario_sub_proceso.ToList();
                if (usps.Where(usp => usp.id_usuario == user.Id_usuario).Count() > 0)
                {
                    model.Add(sp);
                }
            }

            ViewBag.id_usuario = user.Id_usuario;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public int SaveTime(int id_us, int id_sp, int val)
        {
            db.c_usuario_sub_proceso.Where(usp => usp.id_usuario == id_us && usp.id_sub_proceso == id_sp).First().tiempo_sub_proceso = val;

            db.SaveChanges();

            return id_sp;
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
