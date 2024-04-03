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
    public class CriticidadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: PruebaAutoEvaluacion
        public ActionResult Index()
        {
            var ProbabilidadesOcurrencia = db.c_probabilidad_ocurrencia.ToList().OrderBy(po => po.cl_probabilidad_ocurrencia);
            var MagnitudesImpacto = db.c_magnitud_impacto.ToList().OrderBy(mi => mi.cl_magnitud_impacto);
            int NPO = ProbabilidadesOcurrencia.Count();
            int NMI = MagnitudesImpacto.Count();

            c_criticidad[,] criticidades = new c_criticidad[NMI, NPO];
            int i = 0;
            int j = 0;

            foreach (var MI in MagnitudesImpacto)
            {
                foreach (var PO in ProbabilidadesOcurrencia)
                {
                    //Si no existe la criticidad para este par de magnitud de impacto y probabilidad, se creará una nueva
                    if (!db.c_criticidad.Any(cr => cr.id_magnitud_impacto == MI.id_magnitud_impacto && cr.id_probabilidad_ocurrencia == PO.id_probabilidad_ocurrencia))
                    {
                        //Comprobar que exista un registro en c_criticidad
                        Utilidades.Utilidades.ValidateCR();
                        var criticidad = new c_criticidad();
                        criticidad.id_magnitud_impacto = MI.id_magnitud_impacto;
                        criticidad.id_probabilidad_ocurrencia = PO.id_probabilidad_ocurrencia;
                        criticidad.id_criticidad_riesgo = Utilidades.Utilidades.idOfFirsCrit(db);
                        db.c_criticidad.Add(criticidad);
                        criticidades[i, j] = criticidad;
                    }
                    else //Si la criticidad existe
                    {
                        criticidades[i, j] = db.c_criticidad.Where(cr => cr.id_magnitud_impacto == MI.id_magnitud_impacto && cr.id_probabilidad_ocurrencia == PO.id_probabilidad_ocurrencia).First();
                    }
                    j++;
                }
                i++;
                j = 0;
            }

            db.SaveChanges();

            ViewBag.Colores = Utilidades.Utilidades.ColoresMetaCampos();
            ViewBag.PO = ProbabilidadesOcurrencia;
            ViewBag.NPO = NPO;
            ViewBag.MI = MagnitudesImpacto;
            ViewBag.NMI = NMI;
            ViewBag.CRS = criticidades;

            var c = Utilidades.Utilidades.ColoresMetaCampos();
            string cadena = c.First().Text;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateCR(c_criticidad_riesgo criticidad)
        {

            if (ModelState.IsValid)
            {
                var model = new c_criticidad_riesgo();
                model.cl_criticidad_riesgo = criticidad.cl_criticidad_riesgo;
                model.nb_criticidad_riesgo = criticidad.nb_criticidad_riesgo;
                model.cl_color_campo = criticidad.cl_color_campo;
                db.c_criticidad_riesgo.Add(model);
                db.SaveChanges();
                return RedirectToAction("CriticidadRiesgo");
            }
            return RedirectToAction("CriticidadRiesgo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditCR(c_criticidad_riesgo criticidad)
        {

            if (ModelState.IsValid)
            {

                db.Entry(criticidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CriticidadRiesgo");
            }
            return RedirectToAction("CriticidadRiesgo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteCR(c_criticidad_riesgo criticidad)
        {
            var model = db.c_criticidad_riesgo.Find(criticidad.id_criticidad_riesgo);
            if (!db.c_criticidad_riesgo.Any(cr => cr.id_criticidad_riesgo != model.id_criticidad_riesgo))
            {
                var CR = new c_criticidad_riesgo();
                CR.cl_color_campo = "0";
                CR.cl_criticidad_riesgo = "N";
                CR.nb_criticidad_riesgo = "Ninguno";
                db.c_criticidad_riesgo.Add(CR);
                db.SaveChanges();
            }
            var CRaux = db.c_criticidad_riesgo.Where(cr => cr.id_criticidad_riesgo != model.id_criticidad_riesgo).First();
            var Criticidades = model.c_criticidad.ToList();

            foreach (var cr in Criticidades)
            {
                cr.id_criticidad_riesgo = CRaux.id_criticidad_riesgo;
            }

            db.c_criticidad_riesgo.Remove(model);
            db.SaveChanges();
            return RedirectToAction("CriticidadRiesgo", new { idA = model.id_criticidad_riesgo, idS = CRaux.id_criticidad_riesgo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(int id_probabilidad_ocurrencia, int id_magnitud_impacto, int id_criticidad_riesgo)
        {
            var criticidades = db.c_criticidad.ToList();
            var criticidad = db.c_criticidad.Where(c => c.id_magnitud_impacto == id_magnitud_impacto && c.id_probabilidad_ocurrencia == id_probabilidad_ocurrencia).First();
            criticidad.id_criticidad_riesgo = id_criticidad_riesgo;
            db.SaveChanges();
            return null;
        }

        public ActionResult CriticidadRiesgo(int idA = 0, int idS = 0)
        {
            if (idA != 0)
            {
                ViewBag.idcra = idA;
                ViewBag.idcr = idS;
            }

            var CR = db.c_criticidad_riesgo.ToList().OrderBy(c => c.id_criticidad_riesgo);

            return PartialView(CR);
        }
    }
}
