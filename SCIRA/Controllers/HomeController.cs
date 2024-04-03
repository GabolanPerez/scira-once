using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = null, ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class HomeController : Controller
    {
        //private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            //Obtenemos el valor de la variable de sesion
            //int segundos_restantes;
            //try
            //{
            //    segundos_restantes = Int32.Parse(HttpContext.Session["STCP"].ToString());
            //}
            //catch
            //{
            //    segundos_restantes = 253800;
            //}

            int segundos_restantes = (HttpContext.Session["STCP"] != null) ? Int32.Parse(HttpContext.Session["STCP"].ToString()) : 253800;

            //si quedan menos de 253800 segundos (3 días) construir el mensaje que se mostrará
            if (segundos_restantes < 253800)
            {
                int dias = segundos_restantes / 86400;
                segundos_restantes = segundos_restantes % 86400;
                int horas = segundos_restantes / 3600;
                segundos_restantes = segundos_restantes % 3600;
                int minutos = segundos_restantes / 60;
                string mensaje = "Quedan: " + dias + " días " + horas + " horas y " + minutos + " minutos para que la contraseña actual expire";
                ViewBag.Mensaje = mensaje;
            }
            else
            {
                ViewBag.Mensaje = "false";
            }

            HttpContext.Session["STCP"] = "253800";


            return View();
        }

        public ActionResult dsb1()
        {
            return PartialView();
        }


        public ActionResult ObjectDetails(int IoT, int type)
        {
            var user = (IdentityPersonalizado)User.Identity;

            var model = Utilidades.Utilidades.InferiorObjects(user.Id_usuario, IoT, type);

            var complemento = IoT != 1 ? " (Tramo de Control)" : "";

            switch (type)
            {
                case 1:
                    ViewBag.title = "Detalle de Entidades" + complemento;
                    return PartialView("DetailViews/DetailsENT", model);
                case 2:
                    ViewBag.title = "Detalle de Macro Procesos" + complemento;
                    return PartialView("DetailViews/DetailsMP", model);
                case 3:
                    ViewBag.title = "Detalle de Procesos" + complemento;
                    return PartialView("DetailViews/DetailsP", model);
                case 4:
                    ViewBag.title = "Detalle de Sub Procesos" + complemento;
                    return PartialView("DetailViews/DetailsSP", (model));
                case 5:
                    ViewBag.title = "Detalle de Controles" + complemento;
                    return PartialView("DetailViews/DetailsCTR", model);
                case 6:
                    ViewBag.title = "Detalle de Indicadores" + complemento;
                    return PartialView("DetailViews/DetailsIND", model);
                case 7:
                    ViewBag.title = "Detalle de Oficios" + complemento;
                    return PartialView("DetailViews/DetailsOFC", model);
                case 8:
                    ViewBag.title = "Detalle de Informes" + complemento;
                    return PartialView("DetailViews/DetailsINF", model);
                case 9:
                    ViewBag.title = "Detalle de Incidencias" + complemento;
                    return PartialView("DetailViews/DetailsINC", model);
                case 10:
                    ViewBag.title = "Detalle de Planes de Remediación" + complemento;
                    return PartialView("DetailViews/DetailsPLN", model);
                case 11:
                    ViewBag.title = "Detalle de Fichas" + complemento;
                    return PartialView("DetailViews/DetailsFIC", model);
            }



            return PartialView();
        }
    }
}