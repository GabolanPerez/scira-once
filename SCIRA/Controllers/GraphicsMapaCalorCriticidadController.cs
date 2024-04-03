using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Criticidad-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsMapaCalorCriticidadController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: PruebaAutoEvaluacion
        public ActionResult Index()
        {
            var ProbabilidadesOcurrencia = db.c_probabilidad_ocurrencia.ToList().OrderBy(po => po.cl_probabilidad_ocurrencia);
            var MagnitudesImpacto = db.c_magnitud_impacto.ToList().OrderByDescending(mi => mi.cl_magnitud_impacto);
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
                        //Comprobar que exista un registro en c_criticidad_riesgo
                        Utilidades.Utilidades.ValidateCR();
                        var criticidad = new c_criticidad();
                        criticidad.id_magnitud_impacto = MI.id_magnitud_impacto;
                        criticidad.id_probabilidad_ocurrencia = PO.id_probabilidad_ocurrencia;
                        criticidad.id_criticidad_riesgo = db.c_criticidad_riesgo.First().id_criticidad_riesgo;
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

            return View();
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

        public ActionResult DetalleRiesgos(int po, int mi)
        {
            var criticidad = db.c_criticidad.Where(r => r.id_probabilidad_ocurrencia == po && r.id_magnitud_impacto == mi).First();
            var cr = db.c_criticidad_riesgo.Where(r => r.id_criticidad_riesgo == criticidad.id_criticidad_riesgo).First();

            ViewBag.nbpo = criticidad.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia;
            ViewBag.nbmi = criticidad.c_magnitud_impacto.nb_magnitud_impacto;
            ViewBag.nbcr = cr.nb_criticidad_riesgo;

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);
            var Riesgos = Utilidades.Utilidades.RTCRiesgo(us, db);

            var riesgos = Riesgos.Where(r => r.id_probabilidad_ocurrencia == po && r.id_magnitud_impacto == mi).ToList();
            return PartialView(riesgos);
        }
    }
}
