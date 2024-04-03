using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Costeo", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class CosteoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: SubProceso
        public ActionResult Index()
        {
            var user = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            List<c_sub_proceso> c_sub_proceso = Utilidades.Utilidades.RTCObject(us, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();

            return View(c_sub_proceso.ToList());
        }

        #region Details
        public ActionResult Details(int id)
        {
            ViewBag.id_sp = id;

            return PartialView("Modales/Index");
        }

        public ActionResult SPDetails(int id)
        {
            var sp = db.c_sub_proceso.Find(id);

            return PartialView("Modales/SPDetails", sp);
        }

        public ActionResult Participantes(int id)
        {
            var sp = db.c_sub_proceso.Find(id);

            return PartialView("Modales/Participantes", sp);
        }

        public ActionResult ActividadesCosteo(int id)
        {
            var sp = db.c_sub_proceso.Find(id);

            return PartialView("Modales/ActividadesCosteo", sp);
        }
        #endregion

        #region LVL2
        public ActionResult LVL2(int id_sp, int id_ac)
        {
            var sp = db.c_sub_proceso.Find(id_sp);
            var ac = db.c_area_costeo.Find(id_ac);

            ViewBag.sp = sp;
            ViewBag.ac = ac;

            var SubActividades = ac.c_area_costeo_n2.ToList();

            return PartialView("N2/Index", SubActividades);
        }


        #endregion

        #region LVL3
        public ActionResult LVL3(int id_sp, int id_ac_2)
        {
            var sp = db.c_sub_proceso.Find(id_sp);
            var ac2 = db.c_area_costeo_n2.Find(id_ac_2);

            ViewBag.sp = sp;
            ViewBag.ac2 = ac2;

            var atributos = ac2.c_area_costeo_n3.ToList();

            return PartialView("N3/Index", atributos);
        }

        [ValidateAntiForgeryToken, HttpPost, NotOnlyRead]
        public int SaveLVL3(int id_sp, int[] ids_attr, string[] vals)
        {
            var sp = db.c_sub_proceso.Find(id_sp);

            for (int i = 0; i < vals.Length; i++)
            {
                var id = ids_attr[i];
                var val = decimal.Parse(vals[i].Replace('.', ','));

                //verificar si existe la tabla c_area_costeo_n3_sub_proceso
                if (sp.c_area_costeo_n3_sub_proceso.Where(ac3 => ac3.id_area_costeo_n3 == id).Count() > 0)
                {
                    //obtenemos el registro
                    var acsp3 = sp.c_area_costeo_n3_sub_proceso.Where(ac => ac.id_area_costeo_n3 == id).First();

                    if (val == 0) //si el nuevo valor es 0, eliminamos la tabla
                    {
                        db.c_area_costeo_n3_sub_proceso.Remove(acsp3);
                    }
                    else
                    {
                        acsp3.porcentaje = val;
                    }
                }
                else //Si no existe
                {
                    if (val > 0) //si el valor es mayor a 0, creamos la tabla
                    {
                        var acsp3 = new c_area_costeo_n3_sub_proceso()
                        {
                            id_area_costeo_n3 = id,
                            id_sub_proceso = id_sp,
                            porcentaje = val
                        };
                        db.c_area_costeo_n3_sub_proceso.Add(acsp3);

                    }//si el valor es igual a 0, no crearemos la tabla
                }
            }

            //guardamos los cambios
            db.SaveChanges();

            return id_sp;
        }
        #endregion

        #region Otros
        // GET: SubProceso/Costeo/5
        public ActionResult Costeo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(id);
            if (c_sub_proceso == null)
            {
                return HttpNotFound();
            }

            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            return View(c_sub_proceso);
        }


        [NotOnlyRead]
        public string TiempoParticipante(int id_sp, int id_us, int tiempo_usuario)
        {
            var cusp = db.c_usuario_sub_proceso.Where(c => c.id_sub_proceso == id_sp && c.id_usuario == id_us).First();

            cusp.tiempo_sub_proceso = tiempo_usuario;
            db.SaveChanges();

            return "{\"id\":" + id_us + ",\"val\":" + tiempo_usuario + " }";
        }


        [NotOnlyRead]
        public int PorcentajeAreaCosteo(int id_sp, int[] ids_areas, string[] vals)
        {
            var sp = db.c_sub_proceso.Find(id_sp);

            for (int i = 0; i < vals.Length; i++)
            {
                var id = ids_areas[i];
                var val = decimal.Parse(vals[i].Replace('.', ','));

                sp.c_area_costeo_sub_proceso.Where(acsp => acsp.id_area_costeo == id).First().pr_costeo = val;
            }

            db.SaveChanges();



            /*
            c_area_costeo_sub_proceso original = db.c_area_costeo_sub_proceso.Where(c => c.id_area_costeo == id_ac && c.id_sub_proceso == id_sp).First();
            decimal suma = db.c_area_costeo_sub_proceso.Where(c => c.id_sub_proceso == id_sp).Sum(c => c.pr_costeo);
            suma = suma + pr_costeo - original.pr_costeo;

            if(suma > 100)
            {
                ViewBag.ErrorCosteo = "Todos los porcentajes sumados no pueden ser mayores a 100%";

                string data = "{";
                data += "\"error\":\"Todos los porcentajes sumados no pueden ser mayores a 100%\",\"originalVal\": " + original.pr_costeo.ToString().Replace(',','.') + ",\"id\":" + id_ac +"}";

                return data;
            }

            original.pr_costeo = pr_costeo;

            db.Entry(original).State = EntityState.Modified;
            db.SaveChanges();*/

            return id_sp;
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
