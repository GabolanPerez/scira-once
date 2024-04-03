using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepBDEI", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteGeneralBDEIController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var bdei = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "k_bdei").Cast<k_bdei>();


            return View(bdei);
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