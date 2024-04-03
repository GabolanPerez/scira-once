using SCIRA.Seguridad;
using System;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class OnlySuperUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IdentityPersonalizado Ident = (IdentityPersonalizado)HttpContext.Current.User.Identity;
            if (!Ident.Es_super_usuario)
            {
                Utilidades.Utilidades.notifyUser(Ident.Id_usuario, "Su usuario no cuenta con permiso para realizar esta acción", "warning");
                filterContext.Result = new RedirectResult(string.Format("~/Error/OnlySuperUser", filterContext.HttpContext.Request.Url.Host));
            }
            else
            {
                return;
            }
        }
    }
}