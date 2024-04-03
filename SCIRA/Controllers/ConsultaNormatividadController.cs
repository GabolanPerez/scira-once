using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "ConsultaNormatividad", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class ConsultaNormatividadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: NivelNormatividad
        public ActionResult Index()
        {
            var tipos = db.c_tipo_normatividad.OrderBy(t => t.cl_tipo_normatividad).ToList();

            return View(tipos);
        }


        public ActionResult Levels(int id)
        {
            var norm = db.c_normatividad.Find(id);
            ViewBag.title = Strings.getMSG("Niveles de la normatividad") + norm.nb_normatividad;
            var model = norm.c_nivel_normatividad.ToList();

            return PartialView(model);
        }

        public ActionResult Content(int id)
        {
            var nivel = db.c_nivel_normatividad.Find(id);
            ViewBag.title = Strings.getMSG("Contenidos del nivel") + nivel.nb_nivel_normatividad + Strings.getMSG("de la normatividad") + nivel.c_normatividad.nb_normatividad;
            var model = nivel.c_contenido_normatividad.ToList();

            ViewBag.nivelesL = Utilidades.DropDown.NivelesNormatividad(nivel.id_normatividad);

            return PartialView(model);
        }

        public ActionResult DescendantContent(int id)
        {
            var contenido = db.c_contenido_normatividad.Find(id);
            var root = Utilidades.Utilidades.getRoot(db, contenido);
            ViewBag.title = Strings.getMSG("ConsultaNormatividadIndex002") + contenido.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + root.ds_contenido_normatividad;
            var model = contenido.c_contenido_normatividad1.ToList();

            if (contenido.id_contenido_normatividad_padre != null)
            {
                ViewBag.id_padre = contenido.id_contenido_normatividad_padre;
            }

            ViewBag.nivelesL = Utilidades.DropDown.NivelesNormatividad(contenido.c_nivel_normatividad.id_normatividad);

            return PartialView("Content", model);
        }

        #region Documentos
        public FileResult DescargaContenido(int id)
        {
            var cont = db.c_contenido_normatividad.Find(id);


            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + id);
            var bytes = Utilidades.GenerateDoc.ContenidoNormatiidad(cont);


            return File(bytes, "application/pdf", "Contenido " + cont.cl_contenido_normatividad + ".pdf");
        }


        #endregion


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
