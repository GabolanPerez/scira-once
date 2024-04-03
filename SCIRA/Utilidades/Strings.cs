using Microsoft.Ajax.Utilities;
using SCIRA.Properties;
using SCIRA.Seguridad;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Web;

namespace SCIRA.Utilidades
{
    public static class Strings
    {
        private static ResourceManager res_mng = new ResourceManager(typeof(Resources));

        public static string getMSG(string code)
        {
            //if(code== "EntidadCreate003")
            //   {
            //       int y = 0;
            //   }
            //db = new SICIEntities();

            IdentityPersonalizado user = null;
            string parametro  = null;


            try
            {
                user = (IdentityPersonalizado)HttpContext.Current.User.Identity;
                parametro = Globals.GetLan(user.Id_usuario);
            }
            catch {
                
            }

            

            //var parametro = db.c_parametro.FirstOrDefault(pa => pa.nb_parametro == "lan" + user.Id_usuario);

            if (parametro == null)
            {

            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(parametro);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(parametro);
            }

            var res = res_mng.GetString(code);

            return res;
        }
    }
}