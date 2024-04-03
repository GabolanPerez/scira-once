using Hangfire;
using NCrontab;
using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml;

namespace SCIRA.Utilidades
{
    public static class Notification
    {
        static private SICIEntities db = new SICIEntities();
        private static bool send = true;

        #region Nuevo admin auditor
        public static void NewAdminAudit(int idAdmin, int idNewAdminAudit)
        {
            db = new SICIEntities();
            var adminUser = db.c_usuario.Find(idAdmin);
            var newAdminAudit = db.c_usuario.Find(idNewAdminAudit);

            var AdminAudits = db.c_usuario.Where(u => u.es_auditor_admin && u.id_usuario != idNewAdminAudit);


            var message = "El usuario <b>" + newAdminAudit.nb_usuario + "</b> fue establecido como un Auditor Administrador por el Administrador del sistema: <b>" + adminUser.nb_usuario + "</b>";

            var body = buildGeneralHtml("Nuevo usuario auditor administrador agregado", message);

            send2(AdminAudits.ToList(), "Notificaciones SCIRA", body, "Nuevo Auditor Administrador");
        }

        #endregion

        #region Eventos (Fichas)
        public static void lanzarRecordatorioR(int id_evento)
        {
            try
            {
                db = new SICIEntities();
                Utilidades.DeleteBackgoundJobs(id_evento, db, false);
                lanzarRecordatorio(id_evento);
            }
            catch
            {

            }
            return;
        }


        public static void lanzarRecordatorioRE(int id_evento)
        {
            try
            {
                db = new SICIEntities();
                var evento = db.r_evento.Find(id_evento);
                var registro = Utilidades.GetLastReg(evento, db);


                if (!registro.terminado)
                    lanzarRecordatorio(id_evento);
            }
            catch
            {

            }

            return;
        }

        public static void lanzarRecordatorioFL(int id_evento)
        {
            db = new SICIEntities();
            var evento = db.r_evento.Find(id_evento);
            var registro = Utilidades.GetLastReg(evento, db);

            if (!registro.terminado)
                lanzarRecordatorio(id_evento, false);
            return;
        }

        private static void lanzarRecordatorio(int id_evento, bool recurrente = true)
        {
            db = new SICIEntities();
            var evento = db.r_evento.Find(id_evento);
            string textoCuerpo = BuildEVMessage(evento, recurrente);

            send2(evento.c_usuario1.ToList(), evento.nb_evento, textoCuerpo, "Recordatorios SCIRA");

            //intentar eliminar el parametro con el id del recordatorio
            try
            {
                var param = db.c_parametro.Where(p => p.nb_parametro == "EV" + evento.id_evento).First();
                db.c_parametro.Remove(param);
            }
            catch
            {

            }


            //si tiene fecha limite, programar un recordatorio nuevo
            if (!recurrente)
            {

                //crear nueva tarea y nuevo parametro si queda más de 1 día para el evento
                if (((DateTime)evento.fe_vencimiento).AddHours(-25) > DateTime.Now)
                {
                    var job_id = BackgroundJob.Schedule(() => lanzarRecordatorioFL(evento.id_evento), TimeSpan.FromHours(24));

                    var param = new c_parametro()
                    {
                        nb_parametro = "EV" + evento.id_evento,
                        valor_parametro = job_id
                    };

                    db.c_parametro.Add(param);
                }
                db.SaveChanges();
            }
            else //Si es recurrente
            {
                var cron = CrontabSchedule.Parse(Utilidades.NormalizeCron(evento.perioricidad));
                var NeOc = cron.GetNextOccurrence(DateTime.Now);

                //crear nueva tarea y nuevo parametro si queda más de 1 día para el evento
                if ((NeOc).AddHours(-25) > DateTime.Now)
                {
                    var job_id = BackgroundJob.Schedule(() => lanzarRecordatorioRE(evento.id_evento), TimeSpan.FromHours(24));

                    var param = new c_parametro()
                    {
                        nb_parametro = "EV" + evento.id_evento,
                        valor_parametro = job_id
                    };

                    db.c_parametro.Add(param);
                }
                else
                {

                }
                db.SaveChanges();
            }

            return;
        }


        private static string BuildEVMessage(r_evento evento, bool recurrente)
        {
            db = new SICIEntities();
            string titulo = "";


            switch (evento.tipo)
            {
                case "0001": //Ligado a Normatividad
                    var conf1 = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(evento.config);
                    var contenido = db.c_contenido_normatividad.Find(conf1.id);
                    var norm = Utilidades.getRoot(db, contenido);

                    titulo = "Recordatorio de " + contenido.cl_contenido_normatividad + " de la normatividad: " + norm.ds_contenido_normatividad;
                    break;
                case "0002": //Ligado a Oficios/Informes
                    var conf2 = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(evento.config);
                    var objeto2 = db.k_objeto.Find(conf2.id);

                    string oficio_o_informe = objeto2.tipo_objeto == 1 ? "oficio" : objeto2.tipo_objeto == 2 ? "informe de auditoria externa" : "informe de auditoria interna";

                    titulo = "Recordatorio del " + oficio_o_informe + ": " + objeto2.nb_objeto;
                    break;
                case "0003": //Ligado a Incidencias
                    var conf3 = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(evento.config);
                    var objeto3 = db.k_incidencia.Find(conf3.id);

                    titulo = "Recordatorio de la Incidencia perteneciente a " + Utilidades.incSource(objeto3);
                    break;
            }



            var mensaje = evento.ds_evento;

            //si el aviso es con fecha límite añadimos el aviso de vencimiento
            if (!recurrente)
            {
                //var aviso = "<br/><br/><i><b>Este recordatorio tiene como fecha límite: " + ((DateTime)evento.fe_vencimiento).ToShortDateString() + "</b></i>";
                //mensaje += aviso;
            }
            else
            {
                //var cronO = CrontabSchedule.Parse(Utilidades.NormalizeCron(evento.perioricidad));
                //var NeOc = cronO.GetNextOccurrence(DateTime.Now);

                //var aviso = "<br/><br/><i><b>Este recordatorio tiene como fecha límite: " + (NeOc).ToShortDateString() + "</b></i>";
                //mensaje += aviso;
            }

            return buildGeneralHtml(titulo, mensaje);
        }


        #endregion

        #region Incidencia
        public static void IncidenciaAsignada(k_incidencia incidencia)
        {
            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var usuario = db.c_usuario.Find(incidencia.id_responsable);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message3");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = replaceParameters(Head.InnerText, incidencia);
            var body = replaceParameters(Body.InnerText, incidencia);
            var subject = replaceParameters(Subject.InnerText, incidencia);

            var lista = new List<c_usuario>();
            lista.Add(usuario);

            if (send)
            {
                send2(lista, subject, body, head);
            }
            return;
        }
        #endregion

        #region Oficio/Informes
        public static void OficioAsignado(k_objeto objeto)
        {
            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var usuario = db.c_usuario.Find(objeto.id_responsable);


            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message1");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = replaceParameters(Head.InnerText, objeto);
            var body = replaceParameters(Body.InnerText, objeto);
            var subject = replaceParameters(Subject.InnerText, objeto);

            var lista = new List<c_usuario>();
            lista.Add(usuario);

            if (send)
            {
                send2(lista, subject, body, head);
            }
            return;
        }

        public static void InformeAsignado(k_objeto objeto)
        {
            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var usuario = db.c_usuario.Find(objeto.id_responsable);


            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message2");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = replaceParameters(Head.InnerText, objeto);
            var body = replaceParameters(Body.InnerText, objeto);
            var subject = replaceParameters(Subject.InnerText, objeto);

            var lista = new List<c_usuario>();
            lista.Add(usuario);

            if (send)
            {
                send2(lista, subject, body, head);
            }
            return;
        }
        #endregion

        #region Control
        public static void ControlAsignado(k_control control)
        {
            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var usuario = db.c_usuario.Find(control.id_responsable);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message4");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = replaceParameters(Head.InnerText, control);
            var body = replaceParameters(Body.InnerText, control);
            var subject = replaceParameters(Subject.InnerText, control);

            var lista = new List<c_usuario>();
            lista.Add(usuario);

            if (send)
            {
                send2(lista, subject, body, head);
            }
            return;
        }
        #endregion

        #region indicador
        public static void IndicadorAsignado(c_indicador indicador)
        {
            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var usuario = db.c_usuario.Find(indicador.id_responsable);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message5");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = replaceParameters(Head.InnerText, indicador);
            var body = replaceParameters(Body.InnerText, indicador);
            var subject = replaceParameters(Subject.InnerText, indicador);

            var lista = new List<c_usuario>();
            lista.Add(usuario);

            if (send)
            {
                send2(lista, subject, body, head);
            }
            return;
        }
        #endregion

        #region Plan de Remediación
        public static void planAsignado(k_plan plan)
        {
            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var usuario = db.c_usuario.Find(plan.id_responsable);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message6");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = replaceParameters(Head.InnerText, plan);
            var body = replaceParameters(Body.InnerText, plan);
            var subject = replaceParameters(Subject.InnerText, plan);

            var lista = new List<c_usuario>();
            lista.Add(usuario);

            if (send)
            {
                send2(lista, subject, body, head);
            }
            return;
        }
        #endregion

        #region Pendientes
        public static void notifPendientes()
        {
            //Obtenemos nombre del usuario y los datos del objeto
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            //string path = HttpRuntime.AppDomainAppPath + "App_Data/Plantillas/Plantillas.xml";

            //string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);


            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message7");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            string head = "";
            string body = "";
            string subject = "";

            var usuarios = db.c_usuario.ToList();

            foreach (var usuario in usuarios)
            {
                var lista = new List<c_usuario>();
                lista.Add(usuario);


                head = replaceParameters(Head.InnerText, usuario);
                body = replaceParameters(Body.InnerText, usuario);
                subject = replaceParameters(Subject.InnerText, usuario);


                if (send)
                {
                    send2(lista, subject, body, head);
                }
            }
            return;
        }
        #endregion

        #region MassMessages

        #region Controles
        public static void NuevoPeriodoCertificacion()
        {
            //obtenemos la lista de usuarios que sean responsables de un control
            var users = db.c_usuario.Where(u => u.k_control1.Count > 0);
            db = new SICIEntities();

            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("massMessage1");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = Head.InnerText;
            var body = Body.InnerText;
            var subject = Subject.InnerText;


            foreach (var user in users)
            {
                var lista = new List<c_usuario>();
                lista.Add(user);

                var listaControles = user.k_control1.ToList();

                string h = replaceParameters(head, listaControles);
                string b = replaceParameters(body, listaControles);
                string s = replaceParameters(subject, listaControles);

                if (send)
                {
                    if (!send2(lista, s, b, h)) break;
                }
            }
            return;
        }
        #endregion

        #region Indicadores
        public static void NuevoPeriodoIndicadores()
        {
            //obtenemos la lista de usuarios que sean responsables de un control
            var users = db.c_usuario.Where(u => u.c_indicador.Count > 0);
            //renovamos la base de datos
            db = new SICIEntities();


            //Obtenemos nombre del usuario y los datos del objeto

            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("massMessage2");

            if (mensaje.SelectSingleNode("send").InnerText != "true") return;

            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var head = Head.InnerText;
            var body = Body.InnerText;
            var subject = Subject.InnerText;


            foreach (var user in users)
            {
                var lista = new List<c_usuario>();
                lista.Add(user);

                var listaIndicadores = user.c_indicador.Where(i => i.esta_activo).ToList();

                string h = replaceParameters(head, listaIndicadores);
                string b = replaceParameters(body, listaIndicadores);
                string s = replaceParameters(subject, listaIndicadores);

                if (send)
                {
                    if (!send2(lista, s, b, h)) break;
                }
            }
            return;
        }
        #endregion

        #endregion

        #region TEST
        public static void sendTest()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message1");
            //Obtenemos los atributos a modificar
            var Head = mensaje.SelectSingleNode("head");
            var Body = mensaje.SelectSingleNode("body");
            var Subject = mensaje.SelectSingleNode("subject");

            var h = Head.InnerText;
            var b = Body.InnerText;
            var s = Subject.InnerText;

            send2(new List<c_usuario>(), s, b, h, "cevi.13@outlook.com");
        }
        #endregion

        #region Otros
        //Herramienta para enviar un mensaje a uno o más usuarios
        //Configurable para cualquier tipo de mail
        public static bool send2(List<c_usuario> to, string subject, string body, string head, string emailTEST = null)
        {
            string path = HttpRuntime.AppDomainAppPath + "App_Data/Plantillas/Plantillas.xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var msgs = doc.SelectSingleNode("messages");
            var sm = msgs.SelectSingleNode("senderMail");

            var mailXML = sm.SelectSingleNode("mail");
            var passwordXML = sm.SelectSingleNode("password");
            var hostXML = sm.SelectSingleNode("host");
            var puertoXML = sm.SelectSingleNode("puerto");
            var sslXML = sm.SelectSingleNode("ssl");
            var credencialesXML = sm.SelectSingleNode("credenciales");

            string password = passwordXML.InnerText;
            string email = mailXML.InnerText;
            string server = hostXML.InnerText;

            int puerto = int.Parse(puertoXML.InnerText);
            bool ssl = sslXML.InnerText == "true" ? true : false;
            bool credenciales = credencialesXML.InnerText == "true" ? true : false;

            if (!definedMail(email)) return false;


            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

            foreach (var user in to)
            {
                //Solo se agregan los destinatarios que sean correos válidos
                if (Utilidades.IsValidEmail(user.e_mail_principal) && user.esta_activo)
                    mail.To.Add(user.e_mail_principal);
            }
            if (emailTEST != null) mail.To.Add(emailTEST);

            if (mail.To.Count == 0) {
                return false;
            }

            mail.From = new MailAddress(email, head, System.Text.Encoding.UTF8);
            mail.Subject = subject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = body += (Utilidades.GetSecurityProp("MailIncludeTime","false") == "true") ? "<br/><br/><center>" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " - " +TimeZoneInfo.Local.Id + "</center>" : "";
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(email, password);
            client.Port = puerto;
            client.Host = server;
            client.EnableSsl = ssl;
            if (password == "")
            {
                client.UseDefaultCredentials = credenciales;
            }

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }

                h_excepcion error = new h_excepcion()
                {
                    id_funcion = null,
                    nb_metodo = "Envio de Correo Electrónico",
                    ds_excepcion = errorMessage + " Correo destino: " + mail.To.ToString(),
                    fe_excepcion = DateTime.Now
                };

                db = new SICIEntities();
                db.h_excepcion.Add(error);
                db.SaveChanges();

                return false;
            }
            return true;
        }

        private static bool definedMail(string mail)
        {
            if (!string.IsNullOrEmpty(mail))
            {
                return true;
            }
            else
            {
                try
                {
                    var user = (IdentityPersonalizado)HttpContext.Current.User.Identity;
                    if (user.Es_super_usuario)
                        Utilidades.notifyUser(user.Id_usuario, "No se encontró ninguna cuenta de correo configurada, configure una en el menú de Administración/Notificaciones/Ajustes", "error");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }
                return false;
            }
        }

        private static string buildGeneralHtml(string titulo, string mensaje)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            //var path = HttpRuntime.AppDomainAppPath + "App_Data/Plantillas/Plantillas.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            var mensajes = doc.SelectSingleNode("messages");
            var estructura = mensajes.SelectSingleNode("general0001");
            var cuerpo = estructura.SelectSingleNode("body");

            string textoCuerpo = cuerpo.InnerText.Replace("%titulo%", titulo).Replace("%mensaje%", mensaje.Replace(Environment.NewLine, "<br />"));

            string cad1 = cuerpo.InnerText;
            string cad2 = mensaje.Replace(Environment.NewLine, "<br />");
            string cad3 = cad1.Replace("%titulo%", titulo);
            string cad4 = cad3.Replace("%mensaje%", cad2);



            return (cad4);
        }

        private static string replaceParameters(string text, k_objeto objeto)
        {
            var usuario = db.c_usuario.Find(objeto.id_responsable);
            var entidad = db.c_entidad.Find(objeto.id_entidad);

            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("nb_entidad", entidad.nb_entidad);
            text = text.Replace("fecha_registro", objeto.fe_alta.ToString());
            text = text.Replace("no_incidencias", objeto.no_incidencias.ToString());
            text = text.Replace("nb_objeto", objeto.nb_objeto);


            if (objeto.tipo_objeto == 1)
            {
                text = text.Replace("fe_vencimiento", objeto.fe_vencimiento.ToString());
                var autoridad = db.c_origen_autoridad.Find(objeto.id_autoridad);
                text = text.Replace("origen_autoridad", autoridad.nb_origen_autoridad);
            }

            return text;
        }

        private static string replaceParameters(string text, k_incidencia incidencia)
        {
            var usuario = db.c_usuario.Find(incidencia.id_responsable);
            var clasificacion = db.c_clasificacion_incidencia.Find(incidencia.id_clasificacion_incidencia);
            string nb_padre = "";
            if (incidencia.id_objeto != null)
            {
                var objeto = db.k_objeto.Find(incidencia.id_objeto);
                if (objeto.tipo_objeto == 1)
                {
                    nb_padre = "el oficio: " + objeto.nb_objeto;
                }
                else if (objeto.tipo_objeto == 2 || objeto.tipo_objeto == 3)
                {
                    nb_padre = (objeto.tipo_objeto == 2 ? "el informe de auditoría externa: " : "el informe de auditoría interna: ") + objeto.nb_objeto;
                }
                else if (objeto.tipo_objeto == 6)
                {
                    nb_padre = "el objeto: " + objeto.nb_objeto;
                }
            }
            else if (incidencia.id_certificacion_control != null)
            {
                var certificacion = db.k_certificacion_control.Find(incidencia.id_certificacion_control);
                nb_padre = "la certificacion: " + certificacion.cl_certificacion_control;
            }
            if (incidencia.id_control != null)
            {
                var riesgo = db.k_control.Find(incidencia.id_control).k_riesgo.First();
                nb_padre = "al riesgo: " + riesgo.nb_riesgo;
            }

            text = text.Replace("lvl_inc", incidencia.lvl_5 ?? incidencia.lvl_4 ?? incidencia.lvl_3 ?? incidencia.lvl_2 ?? incidencia.lvl_1 ?? "N/A");
            text = text.Replace("desc_incidencia", incidencia.ds_incidencia);
            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("nb_clasificacion", clasificacion.nb_clasificacion_incidencia);
            text = text.Replace("requiere_plan", incidencia.requiere_plan ? "si" : "no");
            text = text.Replace("js_no_plan", incidencia.js_incidencia);
            text = text.Replace("nb_padre", nb_padre);

            return text;
        }

        private static string replaceParameters(string text, k_control control)
        {
            var c2 = db.k_control.Find(control.id_control);
            var usuario = db.c_usuario.Find(c2.id_responsable);
            var riesgo = c2.k_riesgo.First();
            var sp = c2.c_sub_proceso;
            var p = sp.c_proceso;
            var mp = p.c_macro_proceso;
            var en = mp.c_entidad;

            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("codigo_control", c2.tiene_accion_correctora ? "N/A" : c2.relacion_control);
            text = text.Replace("actividad/accionCorrectora", c2.tiene_accion_correctora ? c2.accion_correctora : c2.actividad_control);
            text = text.Replace("riesgo_control", riesgo.nb_riesgo);
            text = text.Replace("sub_proceso_c", sp.cl_sub_proceso + " - " + sp.nb_sub_proceso);
            text = text.Replace("proceso_c", p.cl_proceso + " - " + p.nb_proceso);
            text = text.Replace("macro_proceso_c", mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
            text = text.Replace("nb_entidad", en.cl_entidad + " - " + en.nb_entidad);
            return text;
        }

        private static string replaceParameters(string text, c_indicador indicador)
        {
            var usuario = db.c_usuario.Find(indicador.id_responsable);
            var entidad = db.c_entidad.Find(indicador.id_entidad);
            var area = db.c_area.Find(indicador.id_area);

            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("nb_entidad", entidad.cl_entidad + " - " + entidad.nb_entidad);
            text = text.Replace("cl_indicador", indicador.cl_indicador);
            text = text.Replace("nb_indicador", indicador.nb_indicador);
            text = text.Replace("ds_indicador", indicador.ds_indicador);
            text = text.Replace("nb_area", area.cl_area + " - " + area.nb_area);

            return text;
        }

        private static string replaceParameters(string text, k_plan plan)
        {
            var usuario = db.c_usuario.Find(plan.id_responsable);
            var area = db.c_area.Find(plan.id_area);

            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("id_plan", plan.id_plan.ToString());
            text = text.Replace("nb_plan", plan.nb_plan);
            text = text.Replace("ds_plan", plan.ds_plan);
            text = text.Replace("nb_area", area.cl_area + " - " + area.nb_area);
            text = text.Replace("fe_alta", plan.fe_alta.ToString());
            text = text.Replace("id_incidencia", plan.id_incidencia.ToString());

            return text;
        }

        private static string replaceParameters(string text, List<k_control> controls)
        {
            var usuario = controls.First().c_usuario1;
            controls = controls.Where(c => c.relacion_control != "" && c.relacion_control != null).ToList();

            int totalC = controls.Count;
            string controlList = "(";

            var periodo = db.c_periodo_certificacion.Where(p => p.esta_activo).First();


            for (int i = 0; i < totalC; i++)
            {
                var control = controls.ElementAt(i);
                if (i == totalC - 1)
                {
                    controlList = controlList + control.relacion_control + ")";
                }
                else
                {
                    controlList = controlList + control.relacion_control + ", ";
                }
            }

            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("%lista_controles%", controlList);
            text = text.Replace("%nb_periodo%", periodo.cl_periodo_certificacion + " - " + periodo.nb_periodo_certificacion);

            return text;
        }

        private static string replaceParameters(string text, List<c_indicador> indicators)
        {
            var usuario = indicators.First().c_usuario;
            int totalI = indicators.Count;
            string indicatorsList = "(";

            var periodo = db.c_periodo_indicador.Where(p => p.esta_activo).First();


            for (int i = 0; i < totalI; i++)
            {
                var indicator = indicators.ElementAt(i);
                if (i == totalI - 1)
                {
                    indicatorsList = indicatorsList + indicator.cl_indicador + ")";
                }
                else
                {
                    indicatorsList = indicatorsList + indicator.cl_indicador + ", ";
                }
            }

            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("%lista_indicadores%", indicatorsList);
            text = text.Replace("%nb_periodo%", periodo.cl_periodo_indicador + " - " + periodo.nb_periodo_indicador);

            return text;
        }

        private static string replaceParameters(string text, c_usuario usuario)
        {
            string entidadList = "(";
            string macroProcesoList = "(";
            string procesoList = "(";
            string subProcesoList = "(";
            string controlList = "(";
            string indicatorsList = "(";
            string oficiosList = "(";
            string informesList = "(";
            string incidenciasList = "(";
            string planesList = "(";
            string fichasList = "(";
             
            var periodoInd = db.c_periodo_indicador.Where(p => p.esta_activo).FirstOrDefault() ?? new c_periodo_indicador();
            var periodoCert = db.c_periodo_certificacion.Where(p => p.esta_activo).FirstOrDefault() ?? new c_periodo_certificacion();



            //obtener entidades sin certificar en el periodo actual
            var entidades = usuario.c_entidad.Where(c => !c.k_certificacion_estructura.Any(cert => cert.id_periodo_certificacion == periodoCert.id_periodo_certificacion));
            var totalEntidades = entidades.Count();

            for (int i = 0; i < totalEntidades; i++)
            {
                var entidad = entidades.ElementAt(i);
                if (i == totalEntidades - 1)
                {
                    entidadList += entidad.cl_entidad + ")";
                }
                else
                {
                    entidadList += entidad.cl_entidad + ", ";
                }
            }

            //obtener macro procesos sin certificar en el periodo actual
            var mps = usuario.c_macro_proceso.Where(c => !c.k_certificacion_estructura.Any(cert => cert.id_periodo_certificacion == periodoCert.id_periodo_certificacion));
            var totalMps = mps.Count();

            for (int i = 0; i < totalMps; i++)
            {
                var mp = mps.ElementAt(i);
                if (i == totalMps - 1)
                {
                    macroProcesoList += mp.cl_macro_proceso + ")";
                }
                else
                {
                    macroProcesoList += mp.cl_macro_proceso+ ", ";
                }
            }

            //obtener procesos sin certificar en el periodo actual
            var prs = usuario.c_proceso.Where(c => !c.k_certificacion_estructura.Any(cert => cert.id_periodo_certificacion == periodoCert.id_periodo_certificacion));
            var totalPrs = prs.Count();

            for (int i = 0; i < totalPrs; i++)
            {
                var pr = prs.ElementAt(i);
                if (i == totalPrs - 1)
                {
                    procesoList += pr.cl_proceso + ")";
                }
                else
                {
                    procesoList  += pr.cl_proceso + ", ";
                }
            }

            //obtener sub procesos sin certificar en el periodo actual
            var sps = usuario.c_sub_proceso.Where(c => !c.k_certificacion_estructura.Any(cert => cert.id_periodo_certificacion == periodoCert.id_periodo_certificacion));
            var totalSPs = sps.Count();

            for (int i = 0; i < totalSPs; i++)
            {
                var sp = sps.ElementAt(i);
                if (i == totalSPs - 1)
                {
                    subProcesoList += sp.cl_sub_proceso + ")";
                }
                else
                {
                    subProcesoList += sp.cl_sub_proceso + ", ";
                }
            }

            //obtener controles sin certificar en el periodo actual
            var controles = usuario.k_control.Where(c => !c.k_certificacion_control.Any(cert => cert.id_periodo_certificacion == periodoCert.id_periodo_certificacion));
            var totalControles = controles.Count();

            for (int i = 0; i < totalControles; i++)
            {
                var control = controles.ElementAt(i);
                if (i == totalControles - 1)
                {
                    controlList += control.relacion_control + ")";
                }
                else
                {
                    controlList += control.relacion_control + ", ";
                }
            }


            //obtener indicadores sin evaluacion
            var indicadores = usuario.c_indicador.Where(ind => !ind.k_evaluacion.Any(e => e.id_periodo_indicador == periodoInd.id_periodo_indicador));
            var totalIndicadores = indicadores.Count();

            for (int i = 0; i < totalIndicadores; i++)
            {
                var indicador = indicadores.ElementAt(i);
                if (i == totalIndicadores - 1)
                {
                    indicatorsList += indicador.cl_indicador + ")";
                }
                else
                {
                    indicatorsList += indicador.cl_indicador + ", ";
                }
            }


            //oficios
            var oficios = usuario.k_objeto.Where(o => o.tipo_objeto == 1).ToList();
            var oficiosPendientes = oficios.Where(o => o.fe_contestacion == null);
            var totalOficios = oficiosPendientes.Count();

            for (int i = 0; i < totalOficios; i++)
            {
                var oficio = oficiosPendientes.ElementAt(i);
                if (i == totalOficios - 1)
                {
                    oficiosList += oficio.nb_objeto + ")";
                }
                else
                {
                    oficiosList += oficio.nb_objeto + ", ";
                }
            }

            //informes
            var informes = usuario.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();
            var informesPendientes = informes.Where(o => o.fe_contestacion == null);
            var totalinformes = informesPendientes.Count();

            for (int i = 0; i < totalinformes; i++)
            {
                var informe = informesPendientes.ElementAt(i);
                if (i == totalinformes - 1)
                {
                    informesList += informe.nb_objeto + ")";
                }
                else
                {
                    informesList += informe.nb_objeto + ", ";
                }
            }


            //incidencias
            var incidencias = usuario.k_incidencia.Where(i => i.id_objeto != null).ToList();
            incidencias = incidencias.Where(i => i.k_objeto.tipo_objeto == 1 || i.k_objeto.tipo_objeto == 2 || i.k_objeto.tipo_objeto == 3).ToList();
            var incidenciasPendientes = incidencias.Where(o => o.r_respuesta.Count == 0);
            var totalincidencias = incidenciasPendientes.Count();

            for (int i = 0; i < totalincidencias; i++)
            {
                var incidencia = incidenciasPendientes.ElementAt(i);
                if (i == totalincidencias - 1)
                {
                    incidenciasList += incidencia.id_incidencia + ")";
                }
                else
                {
                    incidenciasList += incidencia.id_incidencia + ", ";
                }
            }

            //Planes
            var planes = usuario.k_plan.ToList();
            var plansPendientes = planes.Where(o => o.r_conclusion_plan.Count == 0);
            var totalplanes = plansPendientes.Count();

            for (int i = 0; i < totalplanes; i++)
            {
                var plan = plansPendientes.ElementAt(i);
                if (i == totalplanes - 1)
                {
                    planesList += plan.nb_plan + ")";
                }
                else
                {
                    planesList += plan.nb_plan + ", ";
                }
            }


            //fichas
            var fichas = usuario.r_evento;
            List<r_evento> fichasP = new List<r_evento>();


            foreach (var ficha in fichas)
            {
                string registro_ligado = Utilidades.registroLigado(ficha);
                if (registro_ligado == null)
                {
                    DeleteActions.DeleteFichaObjects(ficha, db, true);
                    db.r_evento.Remove(ficha);
                    db.SaveChanges();
                }
                else
                {
                    var reg = Utilidades.GetLastReg(ficha, db);
                    if (!reg.terminado) fichasP.Add(ficha);
                }
            }

            var totalFichas = fichasP.Count();

            for (int i = 0; i < totalFichas; i++)
            {
                var ficha = fichasP.ElementAt(i);
                if (i == totalFichas - 1)
                {
                    fichasList += ficha.nb_evento + ")";
                }
                else
                {
                    fichasList += ficha.nb_evento + ", ";
                }
            }


            text = text.Replace("nb_usuario", usuario.nb_usuario);
            text = text.Replace("%lista_entidades%", controlList);
            text = text.Replace("%lista_macro_procesos%", controlList);
            text = text.Replace("%lista_procesos%", controlList);
            text = text.Replace("%lista_sub_procesos%", controlList);
            text = text.Replace("%lista_controles%", controlList);
            text = text.Replace("%lista_indicadores%", indicatorsList);
            text = text.Replace("%lista_oficios%", oficiosList);
            text = text.Replace("%lista_informes%", informesList);
            text = text.Replace("%lista_incidencias%", incidenciasList);
            text = text.Replace("%lista_planes%", planesList);
            text = text.Replace("%lista_fichas%", fichasList);

            return text;
        }
        #endregion


    }
}