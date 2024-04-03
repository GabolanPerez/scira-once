using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = null, ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class PendientesController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: TipoRiesgo
        public ActionResult Index()
        {
            var c_tipo_riesgo = db.c_tipo_riesgo.Include(c => c.c_categoria_riesgo);
            return View(c_tipo_riesgo.ToList());
        }


        #region Entidades
        public ActionResult Entidades()
        {
            try
            {
                var periodo = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                ViewBag.periodo = periodo.nb_periodo_certificacion;

                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var registros = us.c_entidad.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();

                return View(registros);
            }
            catch
            {
                if (!db.c_periodo_certificacion.Any(p => p.esta_activo)) ViewBag.periodo = "N/A";
                return View(new List<c_entidad>());
            }
        }
        #endregion

        #region MacroProcesos
        public ActionResult MacroProcesos()
        {
            try
            {
                var periodo = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                ViewBag.periodo = periodo.nb_periodo_certificacion;

                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var registros = us.c_macro_proceso.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();

                return View(registros);
            }
            catch
            {
                if (!db.c_periodo_certificacion.Any(p => p.esta_activo)) ViewBag.periodo = "N/A";
                return View(new List<c_macro_proceso>());
            }
        }
        #endregion

        #region Procesos
        public ActionResult Procesos()
        {
            try
            {
                var periodo = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                ViewBag.periodo = periodo.nb_periodo_certificacion;

                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var registros = us.c_proceso.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();

                return View(registros);
            }
            catch
            {
                if (!db.c_periodo_certificacion.Any(p => p.esta_activo)) ViewBag.periodo = "N/A";
                return View(new List<c_proceso>());
            }
        }
        #endregion

        #region SubProcesos
        public ActionResult SubProcesos()
        {
            try
            {
                var periodo = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                ViewBag.periodo = periodo.nb_periodo_certificacion;

                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var registros = us.c_sub_proceso.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();

                return View(registros);
            }
            catch
            {
                if (!db.c_periodo_certificacion.Any(p => p.esta_activo)) ViewBag.periodo = "N/A";
                return View(new List<c_sub_proceso>());
            }
        }
        #endregion

        #region Controles
        public ActionResult Controles()
        {
            try
            {
                var periodo = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                ViewBag.periodo = periodo.nb_periodo_certificacion;

                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var controles = us.k_control1.Where(c => !c.tiene_accion_correctora && !c.k_certificacion_control.Any(cer => cer.id_periodo_certificacion == periodo.id_periodo_certificacion)).ToList();

                return View(controles);
            }
            catch
            {
                if (!db.c_periodo_certificacion.Any(p => p.esta_activo)) ViewBag.periodo = "N/A";
                return View(new List<k_control>());
            }
        }
        #endregion

        #region Indicadores
        public ActionResult Indicadores()
        {


            try
            {
                var periodo = db.c_periodo_indicador.Where(p => p.esta_activo).First();
                ViewBag.periodo = periodo.nb_periodo_indicador;

                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var indicadores = us.c_indicador.Where(i => !i.k_evaluacion.Any(ev => ev.id_periodo_indicador == periodo.id_periodo_indicador) && i.esta_activo).ToList();

                return View(indicadores);
            }
            catch
            {
                if (!db.c_periodo_indicador.Any(p => p.esta_activo)) ViewBag.periodo = "N/A";
                return View(new List<c_indicador>());
            }
        }
        #endregion

        #region Oficios
        public ActionResult Oficios()
        {
            try
            {
                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var oficios = us.k_objeto.Where(o => o.fe_contestacion == null && o.tipo_objeto == 1).ToList();

                return View(oficios);
            }
            catch
            {
                return View(new List<k_objeto>());
            }
        }
        #endregion

        #region Informes
        public ActionResult Informes()
        {
            try
            {
                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var informes = us.k_objeto.Where(o => o.fe_contestacion == null && (o.tipo_objeto == 2 || o.tipo_objeto == 3)).ToList();

                return View(informes);
            }
            catch
            {
                return View(new List<k_objeto>());
            }
        }
        #endregion

        #region Incidencias
        public ActionResult Incidencias()
        {
            try
            {
                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var incidencias = us.k_incidencia.Where(i => i.id_objeto != null).ToList();
                incidencias = incidencias.Where(i => i.k_objeto.tipo_objeto == 1 || i.k_objeto.tipo_objeto == 2 || i.k_objeto.tipo_objeto == 3).ToList();
                incidencias = incidencias.Where(i => i.r_respuesta.Count() == 0).ToList();

                return View(incidencias);
            }
            catch
            {
                return View(new List<k_incidencia>());
            }
        }
        #endregion

        #region Planes
        public ActionResult Planes()
        {
            try
            {
                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var planes = us.k_plan.Where(p => p.r_conclusion_plan.Count() == 0).ToList();

                return View(planes);
            }
            catch
            {
                return View(new List<k_plan>());
            }
        }
        #endregion

        #region Fichas
        public ActionResult Fichas()
        {
            try
            {
                var User = (IdentityPersonalizado)HttpContext.User.Identity;
                var us = db.c_usuario.Find(User.Id_usuario);

                var fichas = us.r_evento;
                List<r_evento> fichasP = new List<r_evento>();


                foreach (var ficha in fichas)
                {
                    string registro_ligado = Utilidades.Utilidades.registroLigado(ficha);
                    if (registro_ligado == null)
                    {
                        DeleteActions.DeleteFichaObjects(ficha, db, true);
                        db.r_evento.Remove(ficha);
                        db.SaveChanges();
                    }
                    else
                    {
                        var reg = Utilidades.Utilidades.GetLastReg(ficha, db);
                        if (!reg.terminado) fichasP.Add(ficha);
                    }
                }


                return View(fichasP);
            }
            catch
            {
                return View(new List<r_evento>());
            }
        }
        #endregion

        #region Indicadores Diarios
        public ActionResult IndicadoresDiarios()
        {
            var identity = (IdentityPersonalizado)User.Identity;
            var user = db.c_usuario.Find(identity.Id_usuario);

            var indicadores = user.c_indicador_diario.Where(i => i.esta_activo);

            List<c_indicador_diario> pendientes = new List<c_indicador_diario>();

            foreach (var ind in indicadores)
            {
                var grupo = ind.c_contenido_grupo;

                var fe_actual = DateTime.Now;

                if (grupo.c_contenido_grupo1.Count > 0)
                {
                    var contenido = grupo.c_contenido_grupo1.First();

                    //var eval = db.k_evaluacion_diaria.Where(e => e.id_contenido_grupo == contenido.id_contenido_grupo && e.id_usuario == user.id_usuario && e.id_indicador_diario == ind.id_indicador_diario && ((DateTime)e.fe_evaluacion).Year == fe_actual.Year && ((DateTime)e.fe_evaluacion).DayOfYear == fe_actual.DayOfYear).ToList().Last();
                    var eval = Utilidades.Utilidades.GetLastEval(user, ind, contenido);

                    if (eval.numerador != null && eval.denominador != null)
                    {

                    }
                    else
                    {
                        pendientes.Add(ind);
                    }

                }
            }

            return View(pendientes);
        }
        #endregion

        public ActionResult VerTodo()
        {
            var usuarios = new List<c_usuario>();
            //obtener el usuario actual
            var user = db.c_usuario.Find(((IdentityPersonalizado)User.Identity).Id_usuario);
            usuarios.Add(user);
            //obtener a los usarios que se encuentren abajo en jerarquia
            c_puesto puesto = new c_puesto();
            try
            {
                puesto = user.c_puesto.First();//Utilidades.Utilidades.PuestoUsuario(user.id_usuario);
            }
            catch
            {
                puesto = null;
            }
            List<c_usuario> usuariosInferiores = new List<c_usuario>();


            if (puesto != null)
            {
                usuariosInferiores = Utilidades.Utilidades.usuariosPorPuestos(Utilidades.Utilidades.puestosInferiores(puesto, db));
            }

            usuarios.AddRange(usuariosInferiores);
            //para todos los usuarios (incluido el actual) buscamos todos los pendientes y los agregamos al ViewBag

            var entidades = new List<c_entidad>();
            var mps = new List<c_macro_proceso>();
            var prs = new List<c_proceso>();
            var sps = new List<c_sub_proceso>();
            var controles = new List<k_control>();
            var indicadores = new List<c_indicador>();
            var oficios = new List<k_objeto>();
            var informes = new List<k_objeto>();
            var incidencias = new List<k_incidencia>();
            var planes = new List<k_plan>();
            var fichas = new List<r_evento>();
            var indicadores_diarios = new List<k_evaluacion_diaria>();


            List<k_incidencia> incs2 = db.k_incidencia.ToList();

            c_periodo_certificacion periodo_cert = null;
            c_periodo_indicador periodo_ind = null;

            try
            {
                periodo_cert = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
            }
            catch
            {

            }
            try
            {
                periodo_ind = db.c_periodo_indicador.Where(p => p.esta_activo).First();
            }
            catch
            {

            }


            foreach (var usuario in usuarios)
            {
                if (periodo_cert != null)
                {
                    try//Pendientes entidades
                    {
                        entidades.AddRange(usuario.c_entidad.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo_cert.id_periodo_certificacion)).ToList());
                    }
                    catch { }

                    try//Pendientes macroprocesos
                    {
                        mps.AddRange(usuario.c_macro_proceso.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo_cert.id_periodo_certificacion)).ToList());
                    }
                    catch { }

                    try//Pendientes procesos
                    {
                        prs.AddRange(usuario.c_proceso.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo_cert.id_periodo_certificacion)).ToList());
                    }
                    catch { }

                    try//Pendientes subprocesos
                    {
                        sps.AddRange(usuario.c_sub_proceso.Where(c => !c.k_certificacion_estructura.Any(cer => cer.id_periodo_certificacion == periodo_cert.id_periodo_certificacion)).ToList());
                    }
                    catch { }

                    try//Pendientes controles
                    {
                        controles.AddRange(usuario.k_control1.Where(c => !c.tiene_accion_correctora && !c.k_certificacion_control.Any(cer => cer.id_periodo_certificacion == periodo_cert.id_periodo_certificacion)).ToList());
                    }
                    catch { }
                }

                if (periodo_ind != null)
                {
                    try//Pendientes Indicadores
                    {
                        indicadores.AddRange(usuario.c_indicador.Where(i => !i.k_evaluacion.Any(ev => ev.id_periodo_indicador == periodo_ind.id_periodo_indicador) && i.esta_activo).ToList());
                    }
                    catch { }
                }

                try//Pendientes Oficios
                {
                    oficios.AddRange(usuario.k_objeto.Where(o => o.fe_contestacion == null && o.tipo_objeto == 1).ToList());
                }
                catch { }

                try//Pendientes Informes
                {
                    informes.AddRange(usuario.k_objeto.Where(o => o.fe_contestacion == null && (o.tipo_objeto == 2 || o.tipo_objeto == 3)).ToList());
                }
                catch { }

                try//Pendientes Incidencias
                {
                    var incs = incs2.Where(i => i.id_responsable == usuario.id_usuario).ToList();
                    incs = incs.Where(i => i.id_objeto != null).ToList();
                    incs = incs.Where(i => i.k_objeto.tipo_objeto == 1 || i.k_objeto.tipo_objeto == 2 || i.k_objeto.tipo_objeto == 3).ToList();
                    incs = incs.Where(i => i.r_respuesta.Count() == 0).ToList();

                    incidencias.AddRange(incs);
                }
                catch { }

                try//Pendientes Planes
                {
                    planes.AddRange(usuario.k_plan.Where(p => p.r_conclusion_plan.Count() == 0).ToList());
                }
                catch { }

                try//Pendientes Fichas
                {
                    var fichasAux = usuario.r_evento;
                    List<r_evento> fichasP = new List<r_evento>();

                    foreach (var ficha in fichasAux)
                    {
                        string registro_ligado = Utilidades.Utilidades.registroLigado(ficha);
                        if (registro_ligado == null)
                        {
                            DeleteActions.DeleteFichaObjects(ficha, db, true);
                            db.r_evento.Remove(ficha);
                            db.SaveChanges();
                        }
                        else
                        {
                            var reg = Utilidades.Utilidades.GetLastReg(ficha, db);
                            if (!reg.terminado) fichasP.Add(ficha);
                        }
                    }

                    fichas.AddRange(fichasP);
                }
                catch { }

                try //Indicadores Diarios
                {
                    //obtenemos indicadores diarios activos
                    var INDD = db.c_indicador_diario.Where(i => i.esta_activo).ToList();

                    var us_ids = usuarios.Select(u => u.id_usuario).ToArray();

                    foreach (var ind in INDD)
                    {
                        var grupo = ind.c_contenido_grupo;

                        foreach (var cont in grupo.c_contenido_grupo1)
                        {
                            var todayEvlas = Utilidades.Utilidades.GetLastEvals(ind, cont);

                            var TCCEvals = todayEvlas.Where(e => e.id_usuario == usuario.id_usuario).ToList();

                            if (TCCEvals.Count > 0)
                            {
                                if (Utilidades.Utilidades.EvalNote(TCCEvals.First()) == 4) indicadores_diarios.AddRange(TCCEvals);
                            }
                        }
                    }
                }
                catch
                {

                }

            }

            ViewBag.entidades = entidades;
            ViewBag.mps = mps;
            ViewBag.prs = prs;
            ViewBag.sps = sps;
            ViewBag.controles = controles;
            ViewBag.indicadores = indicadores;
            ViewBag.oficios = oficios;
            ViewBag.informes = informes;
            ViewBag.incidencias = incidencias;
            ViewBag.planes = planes;
            ViewBag.fichas = fichas;
            ViewBag.indicadores_diarios = indicadores_diarios;

            ViewBag.periodo_cert = periodo_cert == null ? "N/A" : periodo_cert.nb_periodo_certificacion;
            ViewBag.periodo_ind = periodo_ind == null ? "N/A" : periodo_ind.nb_periodo_indicador;

            return View();
        }

        public string SendMessage(emailViewModel email, int id_usuario)
        {
            var usuario = db.c_usuario.Where(u => u.id_usuario == id_usuario).ToList();
            var user = (IdentityPersonalizado)HttpContext.User.Identity;

            email.body += "<br><br><br> <p style=\"color:gray;\">Mensaje enviado por <b>" + user.Nb_usuario + "</b></p>";

            var res = Utilidades.Notification.send2(usuario, email.subject, email.body, email.head);

            return res ? "success" : "error";
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
