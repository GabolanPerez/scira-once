using SCIRA.Models;
using SCIRA.Utilidades;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CustomErrorHandlerAttribute : HandleErrorAttribute
    {
        //private string _funcion;
        private SeguridadUtilidades utilidades = new SeguridadUtilidades();
        private SICIEntities db = new SICIEntities();


        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            if (new HttpException(null, filterContext.Exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }

            h_excepcion excepcion = new h_excepcion();

            excepcion.fe_excepcion = DateTime.Now;
            excepcion.ds_excepcion = filterContext.Exception.StackTrace;
            excepcion.nb_metodo = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString();

            var aux = filterContext.Controller.GetType().GetCustomAttributesData();
            string funcion = "";

            foreach (var attribute in aux)
            {

                //if (attribute.AttributeType.Name == "OverloadAvoiderAttribute") return;

                if (attribute.AttributeType.Name == "AccessAttribute" || attribute.AttributeType.Name == "AccessAuditAttribute")
                {
                    funcion = (string)attribute.NamedArguments[0].TypedValue.Value;
                    if (funcion == null)
                    {
                        //Regresar una vista con buena presentacion con los datos del error
                        return;
                    }
                    break;
                }
            }

            int id_f = utilidades.IdFuncion(funcion);

            if (id_f != 0)
            {
                excepcion.id_funcion = id_f;
                db.h_excepcion.Add(excepcion);
                db.SaveChanges();
            }
            else
            {
                Debug.WriteLine(excepcion.ds_excepcion);
            }


            //Regresar una vista con buena presentacion con los datos del error
            return;
        }
    }
}