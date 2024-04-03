using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "msgmkp", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class NotificationController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: 
        public ActionResult Index()
        {
            List<emailViewModel> lista = new List<emailViewModel>();
            string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            var mensajes = doc.SelectSingleNode("messages");
            XmlNode mensaje;
            emailViewModel m;

            string freq = "";

            //Oficio        -   1
            //Informe       -   2
            //Incidencia    -   3
            //Control       -   4   
            //Indicador     -   5
            //Planes Rem    -   6
            //Pendientes
            //del Usuario   -   7
            for (int i = 1; i < 8; i++)
            {
                mensaje = mensajes.SelectSingleNode("message" + i);
                m = new emailViewModel()
                {
                    send = mensaje.SelectSingleNode("send").InnerText == "true",
                    body = mensaje.SelectSingleNode("body").InnerText,
                    head = mensaje.SelectSingleNode("head").InnerText,
                    subject = mensaje.SelectSingleNode("subject").InnerText,
                    nb_email = "message" + i
                };
                lista.Add(m);

                if (i == 7) //Con la plantilla de los pendientes por usuario
                {
                    freq = mensaje.SelectSingleNode("freq").InnerText;
                }
            }

            ViewBag.frecuencias = Utilidades.DropDown.FrecuenciasMensajes(freq);

            return View(lista);
        }

        // GET: 
        public ActionResult Edit(int id)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");

            var mensaje = mensajes.SelectSingleNode("message" + id);

            //Obtenemos los atributos a modificar
            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var model = new emailViewModel();

            model.nb_email = "message" + id;
            model.head = Head.InnerText;
            model.body = Body.InnerText;
            model.subject = Subject.InnerText;

            ViewBag.id = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(emailViewModel model)
        {
            int id = int.Parse(model.nb_email.Substring(7, model.nb_email.Length - 7));
            if (ModelState.IsValid)
            {
                string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(path);



                //Obtenemos el elemento "message"
                var mensajes = doc.SelectSingleNode("messages");
                var mensaje = mensajes.SelectSingleNode(model.nb_email);
                //Obtenemos los atributos a modificar
                var Head = mensaje.SelectSingleNode("head");
                var Body = mensaje.SelectSingleNode("body");
                var Subject = mensaje.SelectSingleNode("subject");

                Head.InnerText = model.head;
                Body.InnerText = model.body;
                Subject.InnerText = model.subject;

                doc.Save(path);
                return RedirectToAction("Index");
            }
            ViewBag.id = id;
            return View(model);
        }

        public ActionResult ToggleActive(int id, string value)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);


            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message" + id);

            var Send = mensaje.SelectSingleNode("send");

            Send.InnerText = value;
            doc.Save(path);




            return null;
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
