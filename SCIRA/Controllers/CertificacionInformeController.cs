using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "CertificacionINF", ModuleCode = "MSICI004")]
    [CustomErrorHandler]
    public class CertificacionInformeController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index(int? id)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            //regresa las entidades por tramo de control
            var entidades = Utilidades.Utilidades.RTCObject(us, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();
            List<c_entidad> entidadesCertificadas = new List<c_entidad>();

            c_periodo_certificacion c_periodo_certificacion;

            if (id == null)
                c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.esta_activo == true).FirstOrDefault();
            else
                c_periodo_certificacion = db.c_periodo_certificacion.Where(p => p.id_periodo_certificacion == id).FirstOrDefault();

            if (c_periodo_certificacion != null)
            {
                //Encontrar cuantos macro procesos están certificados en el periodo actual
                foreach (var en in entidades)
                {
                    if (en.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == c_periodo_certificacion.id_periodo_certificacion))
                        entidadesCertificadas.Add(en);
                }
            }

            ViewBag.periodo = c_periodo_certificacion;


            return View(entidadesCertificadas);
        }


        public ActionResult UniversoCertificado(int id, int id_periodo)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            //regresa las entidades por tramo de control
            var entidades = Utilidades.Utilidades.RTCObject(us, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();

            var entidad = entidades.FirstOrDefault(e => e.id_entidad == id);
            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Find(id_periodo);

            if (entidad == null)
                return RedirectToAction("Denied", "Error");

            ViewBag.periodo = c_periodo_certificacion;


            return View(entidad);
        }


        public ActionResult IncidenciasDetectadas(int id, int id_periodo)
        {
            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            //regresa las entidades por tramo de control
            var entidades = Utilidades.Utilidades.RTCObject(us, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();

            var entidad = entidades.FirstOrDefault(e => e.id_entidad == id);
            c_periodo_certificacion c_periodo_certificacion = db.c_periodo_certificacion.Find(id_periodo);

            if (entidad == null)
                return RedirectToAction("Denied", "Error");

            ViewBag.periodo = c_periodo_certificacion;


            return View(entidad);
        }


        public ActionResult SelectorPeriodo()
        {
            var periodos = db.c_periodo_certificacion.OrderBy(p => p.anio);

            return PartialView(periodos);
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
