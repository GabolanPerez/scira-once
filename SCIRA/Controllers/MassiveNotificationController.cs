using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "MassiveMsg", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class MassiveNotificationController : Controller
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

            //Controles         -   1
            //Indicadores       -   2              
            for (int i = 1; i < 3; i++)
            {
                mensaje = mensajes.SelectSingleNode("massMessage" + i);
                m = new emailViewModel()
                {
                    send = mensaje.SelectSingleNode("send").InnerText == "true",
                    body = mensaje.SelectSingleNode("body").InnerText,
                    head = mensaje.SelectSingleNode("head").InnerText,
                    subject = mensaje.SelectSingleNode("subject").InnerText,
                    nb_email = "massMessage" + i
                };
                lista.Add(m);
            }

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

            var mensaje = mensajes.SelectSingleNode("massMessage" + id);

            //Obtenemos los atributos a modificar
            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var model = new emailViewModel();

            model.nb_email = "massMessage" + id;
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
            int id = int.Parse(model.nb_email.Substring(11, model.nb_email.Length - 11));
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

        public ActionResult WriteMessage()
        {
            ViewBag.entidadR = Utilidades.DropDown.entidadesResponsables();
            ViewBag.macroProcesoR = Utilidades.DropDown.macroProcesosResponsables();
            ViewBag.procesoR = Utilidades.DropDown.procesosResponsables();
            ViewBag.subProcesoR = Utilidades.DropDown.subProcesosResponsables();
            ViewBag.controlR = Utilidades.DropDown.controlesResponsables();
            ViewBag.indicadorR = Utilidades.DropDown.indicadoresResponsables();
            ViewBag.oficioR = Utilidades.DropDown.oficiosResponsables();
            ViewBag.informeR = Utilidades.DropDown.informesResponsables();
            ViewBag.incidenciaR = Utilidades.DropDown.incidenciasResponsables();
            ViewBag.planesR = Utilidades.DropDown.planesResponsables();
            ViewBag.fichasR = Utilidades.DropDown.fichasResponsables();
            ViewBag.users = Utilidades.DropDown.UsuariosMS();


            emailViewModel model = new emailViewModel();
            model.head = "Notificaciones SCIRA";


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public bool WriteMessage(emailViewModel model, int[] entidadR, int[] macroProcesoR, int[] procesoR, int[] subProcesoR, int[] controlR, int[] indicadorR, int[] oficioR, int[] informeR, int[] incidenciaR, int[] planesR, int[] fichasR, int[] users)
        {

            if (ModelState.IsValid)
            {
                List<int> sendTo = new List<int>();
                List<c_usuario> Users = new List<c_usuario>();

                if (entidadR != null) foreach (int aux in entidadR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (macroProcesoR != null) foreach (int aux in macroProcesoR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (procesoR != null) foreach (int aux in procesoR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (subProcesoR != null) foreach (int aux in subProcesoR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (controlR != null) foreach (int aux in controlR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (indicadorR != null) foreach (int aux in indicadorR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (oficioR != null) foreach (int aux in oficioR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (informeR != null) foreach (int aux in informeR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (incidenciaR != null) foreach (int aux in incidenciaR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (planesR != null) foreach (int aux in planesR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (fichasR != null) foreach (int aux in fichasR) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);
                if (users != null) foreach (int aux in users) if (!sendTo.Any(n => n == aux)) sendTo.Add(aux);

                foreach (int aux in sendTo) Users.Add(db.c_usuario.Find(aux));

                return Utilidades.Notification.send2(Users, model.subject, model.body, model.head);
            }
            return false;
        }


        public ActionResult ToggleActive(int id, string value)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);


            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("massMessage" + id);

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
