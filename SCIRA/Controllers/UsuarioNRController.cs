using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class UsuarioNRController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult ChangePassword()
        {
            //Restringir el acceso si se está usando ActiveDirectory
            if (((IdentityPersonalizado)User.Identity).activeD)
            {
                return RedirectToAction("Denied", "Error");
            }

            int segundos_restantes;
            string scc;

            try
            {
                segundos_restantes = Int32.Parse(HttpContext.Session["STCP"].ToString());
            }
            catch
            {
                segundos_restantes = 1;
            }
            try
            {
                scc = HttpContext.Session["SCC"].ToString();
            }
            catch
            {
                scc = "false";
            }



            if (segundos_restantes == -1)
            {
                ViewBag.Mensaje = Strings.getMSG("UsuarioCreate063");
            }
            else
            {
                ViewBag.Mensaje = "false";
            }

            if (scc == "true")
            {
                ViewBag.Mensaje = Strings.getMSG("UsuarioCreate064");
            }
            else
            {
                ViewBag.Mensaje = "false";
            }


            IdentityPersonalizado ident = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
            CambiarContrasenaViewModel model = new CambiarContrasenaViewModel();
            model.original_password = ident.Password;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "original_password,password,new_password,repeat_password")] CambiarContrasenaViewModel cambio)
        {
            IdentityPersonalizado ident = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;

            if (ModelState.IsValid)
            {
                string newPass = SeguridadUtilidades.SHA256Encripta(cambio.new_password);
                c_usuario c_usuario = db.c_usuario.Find(ident.Id_usuario);
                h_password Pass = new h_password();

                Pass.id_usuario = c_usuario.id_usuario;
                Pass.password = newPass;
                Pass.fe_actualizacion = DateTime.Now;
                db.h_password.Add(Pass);

                c_usuario.password = newPass;
                c_usuario.id_estatus_usuario = 2;
                c_usuario.fe_cambio_password = DateTime.Now;
                db.SaveChanges();
                HttpContext.Session["SCC"] = "false";
                return RedirectToAction("Success");
            }
            string scc;
            try
            {
                scc = HttpContext.Session["SCC"].ToString();
            }
            catch
            {
                scc = "false";
            }

            if (scc == "true")
            {
                ViewBag.Mensaje = Strings.getMSG("UsuarioCreate064");
            }
            else
            {
                ViewBag.Mensaje = "false";
            }
            return View();
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Editar()
        {
            //Restringir el acceso si se está usando ActiveDirectory
            if (((IdentityPersonalizado)User.Identity).activeD)
            {
                return RedirectToAction("Denied", "Error");
            }

            IdentityPersonalizado ident = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
            EditarDatosUsuarioViewModel model = new EditarDatosUsuarioViewModel();
            model.nb_usuario = ident.Nb_usuario;
            model.e_mail_principal = ident.E_mail_principal;
            model.e_mail_alterno = ident.E_mail_alterno;
            model.no_telefono = ident.No_telefono;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "nb_usuario,e_mail_principal,e_mail_alterno,no_telefono")] EditarDatosUsuarioViewModel cambio)
        {
            IdentityPersonalizado ident = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;

            if (ModelState.IsValid)
            {
                c_usuario c_usuario = db.c_usuario.Find(ident.Id_usuario);
                c_usuario.nb_usuario = cambio.nb_usuario;
                c_usuario.e_mail_principal = cambio.e_mail_principal;
                c_usuario.e_mail_alterno = cambio.e_mail_alterno;
                c_usuario.no_telefono = cambio.no_telefono;
                db.SaveChanges();

                FormsAuthentication.SignOut();

                return RedirectToAction("Index", "Home", "");
            }
            return View(cambio);
        }


        #region Archivos auxiliares BDEI
        public FileResult DisplayPDFcanal()
        {
            string path = "~/App_Data/BDEIAuxInfo/canal.pdf";
            var manual = File(path, "application/pdf");
            return manual;
        }

        public FileResult DisplayIMGcanal()
        {
            string path = "~/App_Data/BDEIAuxInfo/canal.png";
            var manual = File(path, "image/jpeg");
            return manual;
        }
        
        public FileResult DisplayPDFproceso()
        {
            string path = "~/App_Data/BDEIAuxInfo/proceso.pdf";
            var manual = File(path, "application/pdf");
            return manual;
        }

        public FileResult DisplayIMGproceso()
        {
            string path = "~/App_Data/BDEIAuxInfo/proceso.png";
            var manual = File(path, "image/jpeg");
            return manual;
        }
        public FileResult DisplayPDFsubTipoProducto()
        {
            string path = "~/App_Data/BDEIAuxInfo/subTipoProducto.pdf";
            var manual = File(path, "application/pdf");
            return manual;
        }

        public FileResult DisplayIMGsubTipoProducto()
        {
            string path = "~/App_Data/BDEIAuxInfo/subTipoProducto.png";
            var manual = File(path, "image/jpeg");
            return manual;
        }

        public FileResult DisplayPDFcategoriaLN()
        {
            string path = "~/App_Data/BDEIAuxInfo/categoriaLN.pdf";
            var manual = File(path, "application/pdf");
            return manual;
        }

        public FileResult DisplayIMGcategoriaLN()
        {
            string path = "~/App_Data/BDEIAuxInfo/categoriaLN.png";
            var manual = File(path, "image/jpeg");
            return manual;
        }
        #endregion

        #region Integridad de datos
        public string VerificarIntegridad() {

            Utilidades.Utilidades.VerifyIntegrity();

            return "ok"; 
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
