using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepPR", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReportePRController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var us = db.c_usuario.Find(user.Id_usuario);

            var planes = Utilidades.Utilidades.RTCObject(us, db, "k_plan").Cast<k_plan>().ToList();

            ViewBag.PC = planes.Where(p => p.r_conclusion_plan.Count() == 1).ToList();
            ViewBag.PNC = planes.Where(p => p.r_conclusion_plan.Count() == 0).ToList();

            return View();
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