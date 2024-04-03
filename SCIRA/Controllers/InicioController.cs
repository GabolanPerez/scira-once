using SCIRA.Models;
using SCIRA.Utilidades;
using System;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SCIRA.Controllers
{
    [AllowAnonymous]
    public class InicioController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Inicio
        public ActionResult Index()
        {
            ViewBag.Mensaje = "false";
            c_usuario model = new c_usuario();

            //Leer active Directory
            try
            {
                string UseActiveD = ConfigurationManager.AppSettings["ActiveDirectory"];
                ViewBag.UseActiveD = UseActiveD;
            }
            catch (Exception e)
            {
                ViewBag.UseActiveD = e.InnerException.Message;
            }
            //ViewBag.UseActiveD = "true";


            return View(model);
        }

        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(c_usuario model,string ReturnUrl)
        {
            string pass = model.password;

            var listUsers = db.c_usuario.ToList();

            try
            {
                model = db.c_usuario.Where(u => u.e_mail_principal == model.e_mail_principal).First();
            }
            catch
            {
                ViewBag.Mensaje = "false";
                ViewBag.Error = Strings.getMSG("Nombre de usuario o contraseña incorrectos.");
                return View(model);
            }

            //obtenemos El numero de intentos máximo y el tiempo entre intentos
            int NoIntentos = Utilidades.Utilidades.GetIntSecurityProp("IntentosMaximos", "3");
            int intentosRestantes = 0;

            //Tomaremos los tiempos en segundos
            int TiempoEntreIntentos = Utilidades.Utilidades.GetIntSecurityProp("TiempoEntreIntentos", "30") * 60;
            int TiempoDesdeUtimoIntento = (int)DateTime.Now.Subtract(model.fe_ultimo_intento_acceso ?? DateTime.Now).TotalSeconds;
            int TiempoSiguienteIntento = (TiempoEntreIntentos - TiempoDesdeUtimoIntento) / 60;

            if (TiempoDesdeUtimoIntento > TiempoEntreIntentos || TiempoDesdeUtimoIntento < 1)
            {
                model.no_intento_acceso = 0;
            }

            if (model.no_intento_acceso < NoIntentos)
            {
                model.no_intento_acceso++;
                model.fe_ultimo_intento_acceso = DateTime.Now;
            }
            intentosRestantes = NoIntentos - model.no_intento_acceso;



            if (Membership.ValidateUser(model.e_mail_principal, pass) && ((model.no_intento_acceso < NoIntentos) || (model.es_super_usuario)))
            {
                model.fe_ultimo_intento_acceso = DateTime.Now;
                model.no_intento_acceso = 0;

                db.SaveChanges();

                int aux;
                int TiempoSesion = Int32.TryParse(Utilidades.Utilidades.GetSecurityProp("TiempoSesion", "20"), out aux) ? ((aux < 0) ? 30 : aux) : (30);

                //Variable de sesion para realizar 1 vez la validacion de la caducidad de la contraseña
                FormsAuthentication.SetAuthCookie(model.e_mail_principal, false);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.e_mail_principal, DateTime.Now, DateTime.Now.AddMinutes(TiempoSesion), false, model.id_usuario.ToString());
                string encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = ticket.Expiration });
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Redirect(FormsAuthentication.GetRedirectUrl(model.e_mail_principal, false));
                HttpContext.Session["CHECKEDPASS"] = false;
                //Seconds To Change Password
                //Si quedan menos de 3 días para cambiar la contraseña, se mostrara un aviso en la pantalla de inicio
                HttpContext.Session["STCP"] = "253800";
                HttpContext.Session["SCC"] = "false";

                HttpContext.Session.Timeout = 30;

                var cadena = HttpContext.Session["SCC"].ToString();

                //Establecer el idioma que está usando el usario desde la base de datos
                Globals.SetLan(model.id_usuario, Utilidades.Utilidades.GetSecurityProp("lan" + model.id_usuario, "es"));

                return RedirectToRoute(ReturnUrl);
                
                //return null;
            }
            //en caso de que ValidateUser retorne false, verificar si el usuario esta bloqueado

            if (model.id_estatus_usuario == 4)
            {
                ViewBag.Mensaje = "Su usuario se encuentra bloqueado, por favor acuda con el Administrador del sistema";
            }
            else
            {
                ViewBag.Mensaje = "false";
                if (intentosRestantes == 0)
                {
                    if (Utilidades.Utilidades.GetBoolSecurityProp("BSI", "false"))
                    {
                        //Si está activada la opcion BSI el usuario será bloqueado
                        model.id_estatus_usuario = 4;
                        ViewBag.Mensaje = "Su usuario se encuentra bloqueado, por favor acuda con el Administrador del sistema";
                    }
                }


                if (intentosRestantes != 0)
                {
                    ViewBag.Error = "Nombre de usuario o contraseña incorrectos.\nQuedan " + intentosRestantes + " intentos para iniciar sesión";
                }
                else
                {
                    if (TiempoEntreIntentos != 0)
                    {
                        ViewBag.Error = "Ha superado el número de intentos máximo. Espere " + (TiempoSiguienteIntento + 1) + " Minutos para volver a intentarlo";
                    }
                    else
                    {
                        ViewBag.Error = "Su usuario supero el número de intentos permitidos y fue bloqueado, por favor acuda con el Administrador del sistema";
                    }
                }
            }
            db.SaveChanges();
            return View(model);
        }



        [HttpPost]
        public ActionResult ActiveDLogin(string user, string password)
        {
            string ADPath = ConfigurationManager.AppSettings["ADPath"];
            string ADDomain = ConfigurationManager.AppSettings["ADDomain"];
            var usuarioD = ADDomain + "\\" + user;

            string gruposCad = "";

            //bool esAdmin = false;
            bool esUsuario = false;
            string email = "";

            ViewBag.UseActiveD = "true";
            //Realizar consulta a active directory y obtener las variables de grupos y email
            try
            {
                DirectoryEntry directoryEntry = new DirectoryEntry(ADPath, usuarioD, password);
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);

                directorySearcher.SearchScope = System.DirectoryServices.SearchScope.Subtree;
                directorySearcher.Filter = "(&(objectcategory=user)(SAMAccountName= " + user + "))";
                SearchResult res = directorySearcher.FindOne();

                //mensaje.Text = string.Empty;
                string propiedades;
                string[] grupos;



                for (int i = 0; i <= (res.Properties["memberOf"].Count - 1); i++)
                {
                    propiedades = res.Properties["memberOf"][i].ToString();
                    grupos = propiedades.Split(',');

                    //mensaje.Text += grupos[0].ToString().Split('=')[1] + Environment.NewLine;

                    //if (grupos[0].ToString().Split('=')[1] == "SCIRA")
                    if (grupos[0].ToString().Split('=')[1] == "S-APP-SCIRA")
                        esUsuario = true;
                    //if (grupos[0].ToString().Split('=')[1] == "SCIRA-Usuarios" || grupos[0].ToString().Split('=')[1] == "SCIRA")
                    //    esUsuario = true;

                    //if (grupos[0].ToString().Split('=')[1] == "SCIRA-Administradores")
                    //    esAdmin = true;

                    gruposCad += "|" + grupos[0].ToString().Split('=')[1] + "|,";
                }

                try
                {
                    email = res.Properties["mail"][0].ToString();
                }
                catch (Exception)
                {
                    //email = "bserrano@consubanco.com";

                    email = string.Empty;
                    ViewBag.Mensaje = "false";
                    ViewBag.Error = "El usuario " + user + " no tiene un email definido en el Active Directory. " + (esUsuario ? "Es usuario " : "No es usuario");
                    return View("Index", new c_usuario());
                }
            }
            catch (Exception ex)
            {
                email = string.Empty;
                ViewBag.Mensaje = "false";
                //ViewBag.Error = "Las credenciales del usuario son incorrectas. " + ex.Message + " ADPath: " + ADPath + " ADDomain: " + ADDomain;
                ViewBag.Error = "Las credenciales del usuario son incorrectas. ";
                return View("Index", new c_usuario());
            }





            //if (esAdmin || esUsuario)
            //if (esUsuario)
            if(true)
            {
                var model = db.c_usuario.FirstOrDefault(u => u.e_mail_principal == email);
                if (model == null)
                {
                    ViewBag.Mensaje = "false";
                    ViewBag.Error = "No se encontro al usuario " + email + " en la base de datos.";
                    model.e_mail_principal = user;
                    model.password = "";
                    return View("Index", model);
                }

                //if (esAdmin)
                //    model.es_super_usuario = true;
                //else
                //    model.es_super_usuario = false;

                model.fe_ultimo_intento_acceso = DateTime.Now;
                model.no_intento_acceso = 0;

                db.SaveChanges();

                int aux;
                int TiempoSesion = Int32.TryParse(Utilidades.Utilidades.GetSecurityProp("TiempoSesion", "20"), out aux) ? ((aux < 0) ? 30 : aux) : (30);

                //Variable de sesion para realizar 1 vez la validacion de la caducidad de la contraseña
                FormsAuthentication.SetAuthCookie(model.e_mail_principal, false);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.e_mail_principal, DateTime.Now, DateTime.Now.AddMinutes(TiempoSesion), false, model.id_usuario.ToString());
                string encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = ticket.Expiration });
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Redirect(FormsAuthentication.GetRedirectUrl(model.e_mail_principal, false));
                HttpContext.Session["CHECKEDPASS"] = true;
                //Seconds To Change Password
                //Si quedan menos de 3 días para cambiar la contraseña, se mostrara un aviso en la pantalla de inicio
                HttpContext.Session["STCP"] = "253800";
                HttpContext.Session["SCC"] = "false";

                HttpContext.Session.Timeout = 30;

                var cadena = HttpContext.Session["SCC"].ToString();

                //Establecer el idioma que está usando el usario desde la base de datos
                Globals.SetLan(model.id_usuario, Utilidades.Utilidades.GetSecurityProp("lan" + model.id_usuario, "es"));


                return null;
                //en caso de que ValidateUser retorne false, verificar si el usuario esta bloqueado

                //if (model.id_estatus_usuario == 4)
                //{
                //    ViewBag.Mensaje = "Su usuario se encuentra bloqueado, por favor acuda con el Administrador del sistema";
                //}
                //db.SaveChanges();
                //return View(model);

                //return RedirectToAction("Index", "Home");
            }
            else
            {
                //ViewBag.Mensaje = "false";
                ////ViewBag.Error = "El usuario no pertenece al grupo de usuarios SCIRA.";
                //ViewBag.Error = "El usuario no pertenece al grupo de usuarios SCIRA. " + gruposCad;
                //return View("Index", new c_usuario());
            }
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult Logotipo()
        {
            var dir = Server.MapPath("~/App_Data");
            var path = Path.Combine(dir, "logotipo.png"); //validate the path for security or use other means to generate the path.
            return base.File(path, "image/jpeg");
        }

    }
}