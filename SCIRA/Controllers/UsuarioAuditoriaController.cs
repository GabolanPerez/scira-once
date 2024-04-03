using SCIRA.Models;
using SCIRA.Utilidades;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    public class UsuarioAuditoriaController : Controller
    {
        private SICIEntities db = new SICIEntities();
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();

        // GET: UsuarioAuditoria
        public ActionResult Index()
        {
            return View();
        }
    }
}