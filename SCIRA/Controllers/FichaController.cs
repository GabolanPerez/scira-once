using Hangfire;
using NCrontab;
using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class FichaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Prueba de archivos
        public ActionResult Index()
        {

            return PartialView();
        }


        //Tipos de Fichas
        //  0000 - Recordatorio Simple
        //  0001 - ContenidoNormatividad
        //  0002 - Oficios/Informes
        //  0003 - Incidencias
        //  0004 - Planes



        #region Contenido Normatividad
        public ActionResult ContenidoNormatividadI(int id)
        {
            ViewBag.id_contenido = id;

            var cont = db.c_contenido_normatividad.Find(id);
            var norm = Utilidades.Utilidades.getRoot(db, cont);
            ViewBag.root = cont.id_contenido_normatividad_padre == null;


            ViewBag.id_padre = cont.id_contenido_normatividad_padre;
            ViewBag.id_lvl = cont.c_nivel_normatividad.id_nivel_normatividad;

            if (cont.id_contenido_normatividad_padre != null)
            {
                ViewBag.title = cont.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            }
            else
            {
                ViewBag.title = Strings.getMSG("Normatividad") + norm.ds_contenido_normatividad;
            }


            return PartialView("ContenidoNormatividad/Index");
        }

        public ActionResult ContenidoNormatividadSA(int id)
        {
            var contenido = db.c_contenido_normatividad.Find(id);

            ViewBag.id_objeto = id;
            ViewBag.tipo = "0001";
            ViewBag.showEditButton = true;

            var ficha = Utilidades.Utilidades.getFicha(contenido, db);

            ViewBag.title = Strings.getMSG("Ficha para:") + Utilidades.Utilidades.registroLigado(ficha);

            return PartialView("SA", ficha);
        }


        public ActionResult ContenidoNormatividadDetails(int id)
        {
            var contenido = db.c_contenido_normatividad.Find(id);
            var norm = Utilidades.Utilidades.getRoot(db, contenido);
            var nivel = contenido.c_nivel_normatividad;

            if (contenido.id_contenido_normatividad_padre != null)
                ViewBag.title = Strings.getMSG("Ficha para") + contenido.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            else
                ViewBag.title = Strings.getMSG("Ficha para:") + contenido.cl_contenido_normatividad;

            var ficha = Utilidades.Utilidades.getFicha(contenido, db);

            return PartialView("Details", ficha);
        }

        public ActionResult ContenidoNormatividad(int id)
        {
            var contenido = db.c_contenido_normatividad.Find(id);
            var norm = Utilidades.Utilidades.getRoot(db, contenido);
            var nivel = contenido.c_nivel_normatividad;

            if (contenido.id_contenido_normatividad_padre != null)
                ViewBag.title = Strings.getMSG("Ficha para:") + contenido.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            else
                ViewBag.title = Strings.getMSG("Ficha para:") + contenido.cl_contenido_normatividad;


            var ficha = Utilidades.Utilidades.getFicha(contenido, db);

            var ids1 = ficha.c_usuario1.Select(u => u.id_usuario).ToArray();
            var ids2 = ficha.c_usuario2.Select(u => u.id_usuario).ToArray();

            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(ficha.id_responsable);
            ViewBag.UsuariosMS = Utilidades.DropDown.UsuariosMS(ids1);
            ViewBag.Usuarios2MS = Utilidades.DropDown.UsuariosMS(ids2);
            ViewBag.id_objeto = id;

            ViewBag.FromCN = true;
            ViewBag.root = contenido.id_contenido_normatividad_padre == null;
            ViewBag.id_lvl = contenido.id_contenido_normatividad_padre == null;


            ViewBag.CreateURL = "ContenidoNormatividadC";

            return PartialView("Edit", ficha);
        }


        public ActionResult EditFromSomewhere(int id)
        {
            var ficha = db.r_evento.Find(id);
            Utilidades.Utilidades.tipoFicha(ficha);



            //var contenido = db.c_contenido_normatividad.Find(id);
            //var norm = Utilidades.Utilidades.getRoot(db, contenido);
            //var nivel = contenido.c_nivel_normatividad;

            //if (contenido.id_contenido_normatividad_padre != null)
            //    ViewBag.title = Strings.getMSG("Ficha para:") + contenido.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            //else
            //    ViewBag.title = Strings.getMSG("Ficha para:") + contenido.cl_contenido_normatividad;

            ViewBag.title = Strings.getMSG("Ficha para:") + Utilidades.Utilidades.registroLigado(ficha);

            //var ficha = Utilidades.Utilidades.getFicha(contenido, db);

            var ids1 = ficha.c_usuario1.Select(u => u.id_usuario).ToArray();
            var ids2 = ficha.c_usuario2.Select(u => u.id_usuario).ToArray();

            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(ficha.id_responsable);
            ViewBag.UsuariosMS = Utilidades.DropDown.UsuariosMS(ids1);
            ViewBag.Usuarios2MS = Utilidades.DropDown.UsuariosMS(ids2);
            ViewBag.id_objeto = Utilidades.Utilidades.idRegistroLigado(ficha);

            //al llamar el método editFromSomewhere, se acivará un boton de regresar que 
            //ejecutará el metodo menuModal.bodyLoad(lastURL);
            //por lo que en el index de la pantalla que mande llamar al modal 
            //se tiene que tener una variable llamada lastURL
            //que contenga la url de retorno
            ViewBag.FromSomewhere = true;

            return PartialView("Edit", ficha);
        }


        [ValidateAntiForgeryToken, NotOnlyRead, HttpPost]
        public bool ContenidoNormatividadC(r_evento model, int[] files, int[] ustr, int[] ustrf, int id_objeto)
        {
            //asignar tipo y crear configuracion
            model.tipo = "0001";

            //Configuracion
            var cont = db.c_contenido_normatividad.Find(id_objeto);
            var config = new ConfiguracionesEventosViewModel.Config0001()
            {
                id = cont.id_contenido_normatividad,
                clave = cont.cl_contenido_normatividad,
                nombre = cont.ds_contenido_normatividad,
                nivel = cont.c_nivel_normatividad.cl_nivel_normatividad + " - " + cont.c_nivel_normatividad.nb_nivel_normatividad
            };
            model.config = JsonConvert.SerializeObject(config);

            return CreateFicha(model, files, ustr, ustrf);
        }
        #endregion

        #region Oficios/Informes
        public ActionResult Oficio(int id)
        {
            var objeto = db.k_objeto.Find(id);

            string oficio_o_informe = objeto.tipo_objeto == 1 ? Strings.getMSG("oficio") : objeto.tipo_objeto == 2 ? Strings.getMSG("informe de auditoria externa") : Strings.getMSG("informe de auditoria interna");
            ViewBag.title = Strings.getMSG("Ficha para el") + oficio_o_informe + ": " + objeto.nb_objeto;


            var ficha = Utilidades.Utilidades.getFicha(objeto, db);

            var ids1 = ficha.c_usuario1.Select(u => u.id_usuario).ToArray();
            var ids2 = ficha.c_usuario2.Select(u => u.id_usuario).ToArray();

            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(ficha.id_responsable);
            ViewBag.UsuariosMS = Utilidades.DropDown.UsuariosMS(ids1);
            ViewBag.Usuarios2MS = Utilidades.DropDown.UsuariosMS(ids2);
            ViewBag.id_objeto = id;

            ViewBag.CreateURL = "OficioC";

            return PartialView("Edit", ficha);
        }

        [ValidateAntiForgeryToken, NotOnlyRead, HttpPost]
        public bool OficioC(r_evento model, int[] files, int[] ustr, int[] ustrf, int id_objeto)
        {
            //asignar tipo y crear configuracion
            model.tipo = "0002";

            //Configuracion
            var obj = db.k_objeto.Find(id_objeto);
            var config = new ConfiguracionesEventosViewModel.Config0002()
            {
                id = obj.id_objeto,
                nombre = obj.nb_objeto,
            };
            model.config = JsonConvert.SerializeObject(config);

            return CreateFicha(model, files, ustr, ustrf);
        }

        public ActionResult OficioSA(int id)
        {
            var objeto = db.k_objeto.Find(id);

            string oficio_o_informe = objeto.tipo_objeto == 1 ? Strings.getMSG("oficio") : objeto.tipo_objeto == 2 ? Strings.getMSG("informe de auditoria externa") : Strings.getMSG("informe de auditoria interna");
            ViewBag.title = Strings.getMSG("Ficha para el") + oficio_o_informe + ": " + objeto.nb_objeto;

            ViewBag.id_objeto = id;
            ViewBag.tipo = "0002";
            ViewBag.showEditButton = true;


            var ficha = Utilidades.Utilidades.getFicha(objeto, db);

            return PartialView("SA", ficha);
        }

        #endregion

        #region Incidencias
        public ActionResult Incidencia(int id)
        {
            var objeto = db.k_incidencia.Find(id);

            ViewBag.title = Strings.getMSG("Ficha para la Incidencia perteneciente a") + Utilidades.Utilidades.incSource(objeto);

            var ficha = Utilidades.Utilidades.getFicha(objeto, db);

            var ids1 = ficha.c_usuario1.Select(u => u.id_usuario).ToArray();
            var ids2 = ficha.c_usuario2.Select(u => u.id_usuario).ToArray();

            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(ficha.id_responsable);
            ViewBag.UsuariosMS = Utilidades.DropDown.UsuariosMS(ids1);
            ViewBag.Usuarios2MS = Utilidades.DropDown.UsuariosMS(ids2);
            ViewBag.id_objeto = id;

            ViewBag.CreateURL = "IncidenciaC";

            return PartialView("Edit", ficha);
        }

        [ValidateAntiForgeryToken, NotOnlyRead, HttpPost]
        public bool IncidenciaC(r_evento model, int[] files, int[] ustr, int[] ustrf, int id_objeto)
        {
            //asignar tipo y crear configuracion
            model.tipo = "0003";

            //Configuracion
            var obj = db.k_incidencia.Find(id_objeto);
            var config = new ConfiguracionesEventosViewModel.Config0003()
            {
                id = obj.id_incidencia,
                origen = Utilidades.Utilidades.incSource(obj),
            };
            model.config = JsonConvert.SerializeObject(config);

            return CreateFicha(model, files, ustr, ustrf);
        }

        public ActionResult IncidenciaSA(int id)
        {
            var objeto = db.k_incidencia.Find(id);

            ViewBag.title = Strings.getMSG("Ficha para la Incidencia perteneciente a") + Utilidades.Utilidades.incSource(objeto);

            ViewBag.id_objeto = id;
            ViewBag.tipo = "0003";
            ViewBag.showEditButton = true;

            var ficha = Utilidades.Utilidades.getFicha(objeto, db);

            return PartialView("SA", ficha);
        }

        #endregion

        #region Planes
        public ActionResult Plan(int id)
        {
            var objeto = db.k_plan.Find(id);

            ViewBag.title = Strings.getMSG("Ficha para el Plan de Remediación") + objeto.nb_plan;

            var ficha = Utilidades.Utilidades.getFicha(objeto, db);

            var ids1 = ficha.c_usuario1.Select(u => u.id_usuario).ToArray();
            var ids2 = ficha.c_usuario2.Select(u => u.id_usuario).ToArray();

            ViewBag.id_responsableL = Utilidades.DropDown.Usuario(ficha.id_responsable);
            ViewBag.UsuariosMS = Utilidades.DropDown.UsuariosMS(ids1);
            ViewBag.Usuarios2MS = Utilidades.DropDown.UsuariosMS(ids2);
            ViewBag.id_objeto = id;

            ViewBag.CreateURL = "PlanC";

            return PartialView("Edit", ficha);
        }

        [ValidateAntiForgeryToken, NotOnlyRead, HttpPost]
        public bool PlanC(r_evento model, int[] files, int[] ustr, int[] ustrf, int id_objeto)
        {
            //asignar tipo y crear configuracion
            model.tipo = "0004";

            //Configuracion
            var obj = db.k_plan.Find(id_objeto);
            var config = new ConfiguracionesEventosViewModel.Config0004()
            {
                id = obj.id_plan,
                nb_plan = obj.nb_plan,
            };
            model.config = JsonConvert.SerializeObject(config);

            return CreateFicha(model, files, ustr, ustrf);
        }

        public ActionResult PlanSA(int id)
        {
            var objeto = db.k_plan.Find(id);

            ViewBag.title = Strings.getMSG("Ficha para el Plan de Remediación") + objeto.nb_plan;

            ViewBag.id_objeto = id;
            ViewBag.tipo = "0004";
            ViewBag.showEditButton = true;

            var ficha = Utilidades.Utilidades.getFicha(objeto, db);

            return PartialView("SA", ficha);
        }

        #endregion


        #region auxiliares

        #region Crear Ficha
        private bool CreateFicha(r_evento model, int[] files, int[] ustr, int[] ustrf)
        {
            try
            {
                // diferenciar si la ficha será recurrente o con fecha limite

                if (!model.recordar_antes_de_vencer)//si es recurrente
                {
                    //revisar que sea un formato cron completo (5 cifras)
                    //guardar tarea recurrente para notificar a los usuarios en las listas

                    var cads = model.perioricidad.Split(new char[] { ' ' });
                    if (cads.Last() == "")
                    {
                        model.perioricidad += "1";
                    }

                    model.fe_vencimiento = null;
                }
                else //si tiene fecha limite
                {
                    model.perioricidad = null;
                }

                db.r_evento.Add(model);


                //añadir los usuarios a los que se enviaran los correos
                if (ustr != null)
                    foreach (var us in ustr)
                    {
                        var user = db.c_usuario.Find(us);
                        model.c_usuario1.Add(user);
                    }

                //añadir los ids de los archivos
                if (files != null)
                    foreach (var idar in files)
                    {
                        var file = db.c_archivo.Find(idar);
                        model.c_archivo.Add(file);
                    }



                if (model.recordar_antes_de_vencer)
                {
                    //guardar usuarios a los que se les notificará la finalizacion
                    if (ustrf != null)
                        foreach (var us in ustrf)
                        {
                            var user = db.c_usuario.Find(us);
                            model.c_usuario2.Add(user);
                        }
                }

                db.SaveChanges();


                //crear registro para este evento
                Utilidades.Utilidades.CreateControlReg(model, db);
                //Programar recordatorios
                CreateBackgoundJobs(model);
                //Notificar asignación

                Utilidades.Utilidades.TaskAsigned(model);

                return true;
            }
            catch (Exception e)
            {
                var error = new h_excepcion()
                {
                    id_funcion = null,
                    ds_excepcion = e.Message,
                    fe_excepcion = DateTime.Now,
                    nb_metodo = "Creacion De Ficha"
                };

                db.h_excepcion.Add(error);
                db.SaveChanges();

                return false;
            }
        }
        #endregion

        #region Editar Ficha
        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public bool EditFicha(r_evento model, int[] files, int[] ustr, int[] ustrf, int lu)
        {
            try
            {
                // diferenciar si la ficha será recurrente o con fecha limite
                if (!model.recordar_antes_de_vencer)//si es recurrente
                {
                    //revisar que sea un formato cron completo (5 cifras)
                    //guardar tarea recurrente para notificar a los usuarios en las listas

                    var cads = model.perioricidad.Split(new char[] { ' ' });
                    if (cads.Last() == "")
                    {
                        model.perioricidad += "1";
                    }

                    model.fe_vencimiento = null;

                }
                else //si tiene fecha limite
                {
                    model.perioricidad = null;
                }

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                db = new SICIEntities();
                model = db.r_evento.Find(model.id_evento);

                model.c_usuario1.Clear();
                model.c_usuario2.Clear();
                db.SaveChanges();
                //añadir los usuarios a los que se enviaran los correos

                if (ustr != null)
                    foreach (var us in ustr)
                    {
                        var user = db.c_usuario.Find(us);
                        model.c_usuario1.Add(user);
                    }

                //añadir los ids de los archivos
                if (files != null)
                    foreach (var idar in files)
                    {
                        var file = db.c_archivo.Find(idar);
                        model.c_archivo.Add(file);
                    }



                if (model.recordar_antes_de_vencer)
                {
                    //guardar usuarios a los que se les notificará la finalizacion
                    if (ustrf != null)
                        foreach (var us in ustrf)
                        {
                            var user = db.c_usuario.Find(us);
                            model.c_usuario2.Add(user);
                        }
                }

                db.SaveChanges();

                //Manejo del registro de segumiento
                r_registro_evento rSeguimiento = Utilidades.Utilidades.GetLastReg(model, db);

                var fe_lim = Utilidades.Utilidades.getFeLim(model);

                bool tryy = !rSeguimiento.fe_limite.Equals(fe_lim);

                if (tryy) //si la fecha cambió
                {
                    if (rSeguimiento.fe_limite > DateTime.Now)
                    {
                        if (rSeguimiento.terminado)
                        {
                            Utilidades.Utilidades.CreateControlReg(model, db);
                        }
                        else
                        {
                            rSeguimiento.fe_limite = fe_lim;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Utilidades.Utilidades.CreateControlReg(model, db);
                    }
                }
                else //Si la fecha no cambio
                {
                    if (files != null)//si se añadieron archivos
                    {
                        rSeguimiento.terminado = true;
                        db.SaveChanges();
                        Utilidades.Utilidades.refreshNotifCount(model.id_responsable);
                        Utilidades.Utilidades.removeRow(7, model.id_evento, model.id_responsable);
                    }
                }

                CreateBackgoundJobs(model);

                //si el usuario cambió, notificar al nuevo usuario y eliminar la tarea del anterior
                if (lu != model.id_responsable)
                {
                    Utilidades.Utilidades.TaskAsigned(model, lu);
                }

                return true;
            }
            catch (Exception e)
            {
                var error = new h_excepcion()
                {
                    id_funcion = null,
                    ds_excepcion = e.Message,
                    fe_excepcion = DateTime.Now,
                    nb_metodo = "Creacion De Ficha"
                };

                db.h_excepcion.Add(error);
                db.SaveChanges();

                return false;
            }
        }

        #endregion

        #region Subir Archivos a Ficha
        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public int FichaSA(int id_evento, int[] files)
        {
            r_evento evento = db.r_evento.Find(id_evento);

            if (files != null)
            {
                foreach (int file in files)
                {
                    c_archivo archivo = db.c_archivo.Find(file);

                    evento.c_archivo.Add(archivo);
                }

                var reg = Utilidades.Utilidades.GetLastReg(evento, db);
                reg.terminado = true;

                db.SaveChanges();

                Utilidades.Utilidades.TaskCompleted(evento);
            }
            return id_evento;
        }
        #endregion

        #region Finalizar Tarea
        [HttpPost, NotOnlyRead, ValidateAntiForgeryToken]
        public int EndTask(int id)
        {
            var reg = db.r_registro_evento.Find(id);

            reg.terminado = true;
            db.SaveChanges();

            Utilidades.Utilidades.TaskCompleted(reg.r_evento);
            return id;
        }
        #endregion

        #region ReCrearTareas
        public string RecreateBackgroundJobs()
        {
            var eventos = db.r_evento.ToList();

            foreach (var item in eventos)
            {
                CreateBackgoundJobs(item);
            }

            return "Se recrearon " + eventos.Count + " eventos." ;
        }


        #endregion


        #region Creación de Tareas en Segundo Plano
        private bool CreateBackgoundJobs(r_evento evento)
        {
            Utilidades.Utilidades.DeleteBackgoundJobs(evento.id_evento, db);

            try
            {
                if (!evento.recordar_antes_de_vencer)//Recurrente
                {
                    var cronO = CrontabSchedule.Parse(Utilidades.Utilidades.NormalizeCron(evento.perioricidad));
                    var NeOc = cronO.GetNextOccurrence(DateTime.Now);

                    var diasAntes = evento.no_dias_antes_de_vencer ?? 0;

                    while (NeOc.AddDays(-1 * (int)evento.no_dias_antes_de_vencer) < DateTime.Now) NeOc = NeOc.AddDays(1);


                    //DateTimeOffset fe_recordatorio = DateTime.SpecifyKind(NeOc.AddDays(-1 * diasAntes).AddHours(5), DateTimeKind.Local);
                    DateTimeOffset fe_recordatorio = DateTime.SpecifyKind(NeOc.AddDays(-1 * diasAntes), DateTimeKind.Local);


                    DateTimeOffset fe_aux = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

                    if (fe_recordatorio > fe_aux)
                    {
                        var job_id = BackgroundJob.Schedule(() => Utilidades.Notification.lanzarRecordatorioR(evento.id_evento), fe_recordatorio);

                        var param = new c_parametro()
                        {
                            nb_parametro = "EVR" + evento.id_evento,
                            valor_parametro = job_id
                        };

                        db.c_parametro.Add(param);
                        db.SaveChanges();
                    }

                    //var splitCron = evento.perioricidad.Split(new char[] { ' ' });
                    //var newCron = splitCron[0] + " " + (int.Parse(splitCron[1]) + 5) % 24 + " " + splitCron[2] + " " + splitCron[3] + " " + splitCron[4];
                    RecurringJob.AddOrUpdate("EVR" + evento.id_evento, () => Utilidades.Utilidades.NewReg(evento.id_evento), evento.perioricidad,TimeZoneInfo.FindSystemTimeZoneById(Utilidades.Utilidades.GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id)));
                }
                else//Fecha límite
                {
                    DateTime fe_vencimiento = (DateTime)evento.fe_vencimiento;

                    while (fe_vencimiento.AddDays(-1 * (int)evento.no_dias_antes_de_vencer) < DateTime.Now) fe_vencimiento = fe_vencimiento.AddDays(1);

                    //DateTimeOffset fe_recordatorio = DateTime.SpecifyKind(fe_vencimiento.AddDays(-1 * (int)evento.no_dias_antes_de_vencer).AddHours(5), DateTimeKind.Local);
                    DateTimeOffset fe_recordatorio = DateTime.SpecifyKind(fe_vencimiento.AddDays(-1 * (int)evento.no_dias_antes_de_vencer), DateTimeKind.Local);


                    DateTimeOffset fe_aux = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                    //DateTimeOffset fe_aux = DateTime.SpecifyKind(DateTime.Now.AddHours(5), DateTimeKind.Utc);

                    if (fe_recordatorio > fe_aux)
                    {
                        var job_id = BackgroundJob.Schedule(() => Utilidades.Notification.lanzarRecordatorioFL(evento.id_evento), fe_recordatorio);

                        var param = new c_parametro()
                        {
                            nb_parametro = "EV" + evento.id_evento,
                            valor_parametro = job_id
                        };

                        db.c_parametro.Add(param);
                        db.SaveChanges();
                    }

                }
            }
            catch
            {
                return false;
            }

            return true;
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
        #endregion

    }
}
