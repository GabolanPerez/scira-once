using SCIRA.Models;
using SCIRA.Validaciones;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Criticidad", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class CriticidadContenidoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: PruebaAutoEvaluacion
        public ActionResult Index()
        {

            var cont = db.c_contenido_normatividad.ToList();
            var impactoMonetario = db.c_impacto_monetario.ToList().OrderBy(po => po.cl_impacto_monetario);
            var factibilidad = db.c_factibilidad.ToList().OrderBy(pi => pi.id_factibilidad);

            int NPO = impactoMonetario.Count();
            int NMI = factibilidad.Count();

            c_criticidad1[,] criticidades = new c_criticidad1[NPO, NMI];
            int i = 0;
            int j = 0;

   
          
            return View();
        }    

   
     
    }
}
