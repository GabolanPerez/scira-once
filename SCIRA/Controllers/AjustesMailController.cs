using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Web.Mvc;
using System.Xml;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "AjustesMail", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class AjustesMailController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Actividad
        public ActionResult Index(string message = null)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var msgs = doc.SelectSingleNode("messages");
            var sm = msgs.SelectSingleNode("senderMail");

            var mail = sm.SelectSingleNode("mail");
            var host = sm.SelectSingleNode("host");
            var puerto = sm.SelectSingleNode("puerto");
            var ssl = sm.SelectSingleNode("ssl");
            var credenciales = sm.SelectSingleNode("credenciales");

            ViewBag.hostL = Utilidades.DropDown.MailHosts(host.InnerText);

            var model = new AjustesMailViewModel()
            {
                mail = mail.InnerText,
                host = host.InnerText,
                puerto = int.Parse(puerto.InnerText),
                ssl = ssl.InnerText == "true" ? true : false,
                credenciales = credenciales.InnerText == "true" ? true : false,
            };


            ViewBag.message = message;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult SetMailConfig(AjustesMailViewModel model)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var msgs = doc.SelectSingleNode("messages");
            var sm = msgs.SelectSingleNode("senderMail");

            var mail = sm.SelectSingleNode("mail");
            var password = sm.SelectSingleNode("password");
            var host = sm.SelectSingleNode("host");
            var puerto = sm.SelectSingleNode("puerto");
            var ssl = sm.SelectSingleNode("ssl");
            var credenciales = sm.SelectSingleNode("credenciales");

            mail.InnerText = model.mail;
            password.InnerText = model.password;
            host.InnerText = model.host;
            puerto.InnerText = model.puerto.ToString();
            ssl.InnerText = model.ssl ? "true" : "false";
            credenciales.InnerText = model.credenciales ? "true" : "false";

            doc.Save(path);

            return RedirectToAction("Index", new { message = Strings.getMSG("CosteoIndex006") });
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
