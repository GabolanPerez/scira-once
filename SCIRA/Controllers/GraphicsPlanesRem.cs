using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "PlanesRem-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsPlanesRemController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var planes = Utilidades.Utilidades.RTCObject(us, db, "k_plan").Cast<k_plan>().ToList();


            return View(planes);
        }

        #region Detalles
        public ActionResult Details(int EST, int ORI, int CLAS)
        {
            var Lista = new List<k_plan>();

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var planes = Utilidades.Utilidades.RTCObject(us, db, "k_plan").Cast<k_plan>().ToList();


            switch (EST)
            {
                case 1: //Planes Concluidos
                    Lista = planes.Where(p => p.r_conclusion_plan.Count > 0).ToList();
                    break;
                case 2: //Planes Vigentes
                    Lista = planes.Where(p => p.r_conclusion_plan.Count == 0).Where(p => p.fe_estimada_implantacion > DateTime.Now).ToList();
                    break;
                case 3: //Planes Vencidos
                    Lista = planes.Where(p => p.r_conclusion_plan.Count == 0).Where(p => p.fe_estimada_implantacion <= DateTime.Now).ToList();
                    break;
                case 4: //Planes Total
                    Lista = planes.ToList();
                    break;
                default:
                    return null;
            }

            if (ORI < 7 && ORI > 0)
            {
                switch (ORI)
                {
                    case 1: //Perteneciente a OFICIOS
                        Lista = Lista.Where(p => p.k_incidencia.k_objeto != null).Where(p => p.k_incidencia.k_objeto.tipo_objeto == 1).ToList();
                        break;
                    case 2: //Perteneciente a A. EXTERNA
                        Lista = Lista.Where(p => p.k_incidencia.k_objeto != null).Where(p => p.k_incidencia.k_objeto.tipo_objeto == 2).ToList();
                        break;
                    case 3: //Perteneciente a A. INTERNA
                        Lista = Lista.Where(p => p.k_incidencia.k_objeto != null).Where(p => p.k_incidencia.k_objeto.tipo_objeto == 3).ToList();
                        break;
                    case 4: //Perteneciente a CERTIFICACIÓN
                        Lista = Lista.Where(p => p.k_incidencia.k_certificacion_control != null).ToList();
                        break;
                    case 5: //Perteneciente a MRyC
                        Lista = Lista.Where(p => p.k_incidencia.k_control != null).ToList();
                        break;
                    case 6: //Perteneciente a OTROS
                        Lista = Lista.Where(p => p.k_incidencia.k_objeto != null).Where(p => p.k_incidencia.k_objeto.tipo_objeto == 6).ToList();
                        break;
                }
            }


            if (CLAS > 0)
            {
                Lista = Lista.Where(p => p.k_incidencia.id_clasificacion_incidencia == CLAS).ToList();
            }


            return PartialView(Lista);
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
