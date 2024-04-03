using System;
using System.Web.Mvc;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class OverloadAvoiderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;
        }
    }
}