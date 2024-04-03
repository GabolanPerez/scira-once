using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using System;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AccessAttribute : ActionFilterAttribute
    {
        private string _funcion;
        private string _moduleCode = null;

        private SeguridadUtilidades utilidades = new SeguridadUtilidades();
        private SICIEntities db = new SICIEntities();

        public string Funcion
        {
            get { return _funcion ?? String.Empty; }
            set
            {
                _funcion = value;
            }
        }

        public string ModuleCode
        {
            get { return _moduleCode ?? String.Empty; }
            set
            {
                _moduleCode = value;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //HttpContext.Current.Session["culture"] = "en-US";

            if (_moduleCode == null)
            {
                filterContext.Result = new RedirectResult(string.Format("~/Error/Denied", filterContext.HttpContext.Request.Url.Host));
            }
            else
            {
                var ModuleState = Utilidades.Utilidades.ModuleState(_moduleCode);

                if (!ModuleState && _moduleCode != "MSICI000") filterContext.Result = new RedirectResult(string.Format("~/Error/Denied", filterContext.HttpContext.Request.Url.Host));
            }


            if (_funcion == "UsuarioNR")
            {
                return;
            }
            if (_funcion == "Locked")
            {
                filterContext.Result = new RedirectResult(string.Format("~/Error/Denied", filterContext.HttpContext.Request.Url.Host));
            }

            IdentityPersonalizado Ident = (IdentityPersonalizado)HttpContext.Current.User.Identity;

            bool checkedpass = (HttpContext.Current.Session["CHECKEDPASS"] != null) ? (bool)HttpContext.Current.Session["CHECKEDPASS"] : true;


            //try
            //{
            //    checkedpass = (bool)HttpContext.Current.Session["CHECKEDPASS"];
            //}
            //catch
            //{
            //    checkedpass = true;
            //}


            if (!checkedpass)
            {
                double total_segundos = Utilidades.Utilidades.SegundosTiempoCaducidad();

                if (total_segundos > -1)
                {
                    //obtener fecha actual y fecha de ultimo cambio de contraseña
                    DateTime actual = DateTime.Now;
                    DateTime ultima = Ident.Fe_cambio_password;
                    //la diferencia se medira en segundos
                    double diferencia = actual.Subtract(ultima).TotalSeconds;

                    double tiempo_restante = total_segundos - diferencia;

                    //que pasa si quedan menos de 3 días pero mas de 0 segundo
                    if (tiempo_restante < 253800 && tiempo_restante > 0)
                    {

                        //fijamos la variable de sesion con el tiempo restante
                        HttpContext.Current.Session["STCP"] = ((int)tiempo_restante).ToString();
                        //Marcamos la variable de sesion para evitar dobles validaciones
                        HttpContext.Current.Session["CHECKEDPASS"] = true;
                        //redirigimos a la funcion home/index
                        filterContext.Result = new RedirectResult(string.Format("~/Home/Index", filterContext.HttpContext.Request.Url.Host));
                    }

                    //que pasa si es negativo el resultado
                    if (tiempo_restante <= 0)
                    {
                        //fijamos la variable de sesion con un -1 para mostrar un aviso al cargar la vista de cambio de contraseña
                        HttpContext.Current.Session["STCP"] = "-1";
                        //redirigimos a la funcion home/index
                        filterContext.Result = new RedirectResult(string.Format("~/UsuarioNR/ChangePassword", filterContext.HttpContext.Request.Url.Host));
                    }
                }
            }

            //Si la contraseña fue establecida por el administrador, solicitar el cambio
            if (Ident.Id_estatus_usuario == 1 && !Globals.StartOnActiveDIrectory)
            {
                HttpContext.Current.Session["SCC"] = "true";
                filterContext.Result = new RedirectResult(string.Format("~/UsuarioNR/ChangePassword", filterContext.HttpContext.Request.Url.Host));
            }

            if (_funcion == null)
            {
                return;
            }
            string[] funciones = utilidades.ObtenerFunciones();

            foreach (string fn in funciones)
            {
                if (_funcion.Equals(fn))
                {
                    return;
                }
            }

            filterContext.Result = new RedirectResult(string.Format("~/Error/Denied", filterContext.HttpContext.Request.Url.Host));
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string controlador = context.Controller.ControllerContext.RouteData.Values["controller"].ToString();
            string action = context.Controller.ControllerContext.RouteData.Values["action"].ToString();
            string mensaje = controlador + "/" + action;
            string ruta = context.Controller.ControllerContext.RouteData.ToString();
            if (_funcion == "UsuarioNR")
            {
                return;
            }
            if (_funcion != null)
            {
                IdentityPersonalizado Ident = (IdentityPersonalizado)HttpContext.Current.User.Identity;

                h_acceso acceso = new h_acceso();

                acceso.fe_acceso = DateTime.Now;
                acceso.id_usuario = Ident.Id_usuario;
                acceso.nb_funcion = mensaje;

                db.h_acceso.Add(acceso);
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }

            }

            return;
        }
    }
}