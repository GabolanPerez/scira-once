using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepSACosteo", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteSubActividadesCosteoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteCosteo
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            List<c_sub_proceso> subProcesos = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();


            return View(subProcesos);
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