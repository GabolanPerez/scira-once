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
    public class CriticidadNormatividadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: PruebaAutoEvaluacion
        public ActionResult Index()
        {
            var impactoMonetario = db.c_impacto_monetario.ToList().OrderBy(po => po.cl_impacto_monetario);
            var factibilidad = db.c_factibilidad.ToList().OrderBy(pi => pi.id_factibilidad);

            int NPO = impactoMonetario.Count();
            int NMI = factibilidad.Count();

            c_criticidad1[,] criticidades = new c_criticidad1[NPO, NMI];
            int i = 0;
            int j = 0;

            foreach (var MI in factibilidad)
            {
                foreach (var PO in impactoMonetario)
                {
                    //Si no existe la criticidad para este par de magnitud de impacto y probabilidad, se creará una nueva
                    if (!db.c_criticidad1.Any(cr => cr.id_impacto_monetario == PO.id_impacto_monetario && cr.id_factibilidad == MI.id_factibilidad))
                    {
                        //Comprobar que exista un registro en c_criticidad
                        Utilidades.Utilidades.ValidateCR1();
                        var criticidad = new c_criticidad1();
                        criticidad.id_impacto_monetario = PO.id_impacto_monetario;
                        criticidad.id_factibilidad = MI.id_factibilidad;
                        criticidad.id_criticidad_normatividad = Utilidades.Utilidades.idOfFirsCrit1(db);
                        db.c_criticidad1.Add(criticidad);
                        criticidades[i, j] = criticidad;
                    }
                    else //Si la criticidad existe
                    {
                        criticidades[i, j] = db.c_criticidad1.Where(cr => cr.id_impacto_monetario == PO.id_impacto_monetario && cr.id_factibilidad == MI.id_factibilidad).First();
                    }
                    j++;
                }
                i++;
                j = 0;
            }

            db.SaveChanges();

            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.PO = impactoMonetario;
            ViewBag.NPO = NPO;
            ViewBag.MI = factibilidad;
            ViewBag.NMI = NMI;
            ViewBag.CRS = criticidades;

            var c = Utilidades.Utilidades.ColoresMetaCampos();
            string cadena = c.First().Text;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateCR(c_criticidad_normatividad criticidad)
        {

            if (ModelState.IsValid)
            {
                var model = new c_criticidad_normatividad();
                model.cl_criticidad_normatividad = criticidad.cl_criticidad_normatividad;
                model.nb_criticidad_normatividad = criticidad.nb_criticidad_normatividad;
                model.cl_color_campo = criticidad.cl_color_campo;
                db.c_criticidad_normatividad.Add(model);
                db.SaveChanges();
                return RedirectToAction("CriticidadNormatividad");
            }
            return RedirectToAction("CriticidadNormatividad");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditCR(c_criticidad_normatividad criticidad)
        {

            if (ModelState.IsValid)
            {

                db.Entry(criticidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CriticidadNormatividad");
            }
            return RedirectToAction("CriticidadNormatividad");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteCR(c_criticidad_normatividad criticidad)
        {
            var model = db.c_criticidad_normatividad.Find(criticidad.id_criticidad_normatividad);
            if (!db.c_criticidad_normatividad.Any(cr => cr.id_criticidad_normatividad != model.id_criticidad_normatividad))
            {
                var CR = new c_criticidad_normatividad();
                CR.cl_color_campo = "0";
                CR.cl_criticidad_normatividad = "N";
                CR.nb_criticidad_normatividad = "Ninguno";
                db.c_criticidad_normatividad.Add(CR);
                db.SaveChanges();
            }
            var CRaux = db.c_criticidad_normatividad.Where(cr => cr.id_criticidad_normatividad != model.id_criticidad_normatividad).First();
            var Criticidades = model.c_criticidad1.ToList();

            foreach (var cr in Criticidades)
            {
                cr.id_criticidad_normatividad = CRaux.id_criticidad_normatividad;
            }

            db.c_criticidad_normatividad.Remove(model);
            db.SaveChanges();
            return RedirectToAction("CriticidadNormatividad", new { idA = model.id_criticidad_normatividad, idS = CRaux.id_criticidad_normatividad });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(int id_impacto_monetario, int id_factibilidad, int id_criticidad_normatividad)
        {
            var criticidades = db.c_criticidad1.ToList();
            var criticidad = db.c_criticidad1.Where(c => c.id_impacto_monetario == id_impacto_monetario && c.id_factibilidad == id_factibilidad).First();
            criticidad.id_criticidad_normatividad = id_criticidad_normatividad;
            db.SaveChanges();
            return null;
        }

        public ActionResult CriticidadNormatividad(int idA = 0, int idS = 0)
        {
            if (idA != 0)
            {
                ViewBag.idcra = idA;
                ViewBag.idcr = idS;
            }

            var CR = db.c_criticidad_normatividad.ToList().OrderBy(c => c.id_criticidad_normatividad);

            return PartialView(CR);
        }
    }
}
