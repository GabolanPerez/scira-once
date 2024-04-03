using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepCosteo", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteCosteoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteCosteo
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            List<c_sub_proceso> sub_procesos = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();

            //ViewBag.usuariosL = Utilidades.DropDown.Usuario();
            ViewBag.usuariosL = usuariosDesencientes(sub_procesos);



            return View(new ActividadesCosteoViewModel());
        }



        public ActionResult FilteredUsers(int id = 0)
        {
            var user = ((IdentityPersonalizado)User.Identity);

            var model = new ActividadesCosteoViewModel();
            var actividades = db.c_area_costeo.ToList();
            List<c_sub_proceso> sub_procesos = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();



            sub_procesos = sub_procesos.OrderBy(sp => sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad)
                            .OrderBy(sp => sp.c_proceso.c_macro_proceso.cl_macro_proceso)
                            .OrderBy(sp => sp.c_proceso.cl_proceso)
                            .OrderBy(sp => sp.cl_sub_proceso)
                            .ToList();

            //Establecer datos de las actividades de costeo
            foreach (var actividad in actividades)
            {
                model.nb_actividades.Add(actividad.nb_area_costeo);
            }

            if (actividades.Count == 0)
            {
                ViewBag.error = "1";
            }

            //llenar todos los atributos de los participantes
            //Para cada sub proceso, buscar sus participantes
            foreach (var sp in sub_procesos)
            {
                var entidad = sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad + " - " + sp.c_proceso.c_macro_proceso.c_entidad.nb_entidad;
                var respEn = sp.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario;
                var macro_proceso = sp.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + sp.c_proceso.c_macro_proceso.nb_macro_proceso;
                var respMp = sp.c_proceso.c_macro_proceso.c_usuario.nb_usuario;
                var proceso = sp.c_proceso.cl_proceso + " - " + sp.c_proceso.nb_proceso;
                var respPr = sp.c_proceso.c_usuario.nb_usuario;
                var sub_proceso = sp.cl_sub_proceso + " - " + sp.nb_sub_proceso;
                var respSp = sp.c_usuario.nb_usuario;
                //obtenemos las tablas con el tiempo invertido y los usuarios correspondientes

                List<c_usuario_sub_proceso> usuarios = new List<c_usuario_sub_proceso>();

                if (id == 0)
                    usuarios = sp.c_usuario_sub_proceso.ToList();
                else
                    usuarios = sp.c_usuario_sub_proceso.Where(u => u.id_usuario == id).ToList();

                foreach (var usuario in usuarios)
                {
                    PSPCosteoViewModel us = new PSPCosteoViewModel();
                    us.cbn_entidad = entidad;
                    us.cbn_macro_proceso = macro_proceso;
                    us.cbn_proceso = proceso;
                    us.cbn_sub_proceso = sub_proceso;
                    us.nb_participante = usuario.c_usuario.nb_usuario;
                    us.resp_entidad = respEn;
                    us.resp_macro_proceso = respMp;
                    us.resp_proceso = respPr;
                    us.resp_sub_proceso = respSp;
                    //Encontrar las actividades de costeo que tiene el subproceso
                    var actividades_costeo_sp = sp.c_area_costeo_sub_proceso.ToList();
                    List<c_area_costeo> actividades_costeo = new List<c_area_costeo>();
                    foreach (var act in actividades_costeo_sp)
                    {
                        actividades_costeo.Add(act.c_area_costeo);
                    }

                    foreach (var actividad in actividades)
                    {
                        //si el sub proceso cuenta con la actividad, calcular el tiempo invertido del participante
                        if (actividades_costeo.Contains(actividad))
                        {
                            c_area_costeo_sub_proceso acsp = db.c_area_costeo_sub_proceso.Where(ac => ac.id_sub_proceso == sp.id_sub_proceso && ac.id_area_costeo == actividad.id_area_costeo).First();
                            c_usuario_sub_proceso usp = db.c_usuario_sub_proceso.Where(ussp => ussp.id_usuario == usuario.id_usuario && ussp.id_sub_proceso == sp.id_sub_proceso).First();
                            //calcular tiempo invertido en minutos
                            double TI = (double)acsp.pr_costeo * ((double)usp.tiempo_sub_proceso / (double)100);
                            //calcular porcentaje invertido
                            double percentaje = (double)acsp.pr_costeo;
                            variables_costeo vc = new variables_costeo();
                            vc.porcentaje = (percentaje.ToString() + "%");
                            vc.tiempo_invertido = (TI.ToString() + " min");
                            us.varcos.Add(vc);
                        }
                        //Si el proceso no cuenta con la actividad, fijar el tiempo invertido en 0
                        else
                        {
                            variables_costeo vc = new variables_costeo();
                            vc.porcentaje = ("0%");
                            vc.tiempo_invertido = ("0 min");
                            us.varcos.Add(vc);
                        }
                    }
                    us.tiempo_total = usuario.tiempo_sub_proceso.ToString();
                    model.participantes.Add(us);
                }
            }

            if (id != 0)
            {
                ViewBag.ustat = "filtered";
            }
            else
            {
                ViewBag.ustat = "mixed";
            }

            return PartialView(model);
        }


        List<SelectListItem> usuariosDesencientes(List<c_sub_proceso> sps)
        {
            var res = new List<SelectListItem>();
            var Users = new List<c_usuario>();

            foreach (var sp in sps)
            {
                var usuarios = sp.c_usuario_sub_proceso.ToList();
                foreach (var usuario in usuarios)
                {
                    Users.Add(usuario.c_usuario);
                }
            }

            Users = Users.Union(Users).ToList();

            foreach (var us in Users)
            {
                res.Add(new SelectListItem()
                {
                    Text = us.nb_usuario,
                    Value = us.id_usuario.ToString()
                });
            }

            return res.OrderBy(r => r.Text).ToList();
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