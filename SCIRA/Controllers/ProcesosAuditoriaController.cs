using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static iTextSharp.text.pdf.AcroFields;

namespace SCIRA.Controllers
{
    [Authorize]
    [CustomErrorHandler]
    public class ProcesosAuditoriaController : Controller
    {
        private SICIEntities db = new SICIEntities();

        #region Planeación anual
        [AccessAudit(Funcion = "PlaAud")]
        public ActionResult IndexPlaneacion()
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var model = db.k_auditoria.Where(k => idsDivisiones.Contains(k.c_auditoria.id_division_auditoria ?? 0)).ToList();

            List<string> años = new List<string>();

            foreach (var item in model)
            {
                if (item.fe_inicial_planeada.HasValue)
                {
                    var año = item.fe_inicial_planeada.Value.Year;

                    if (!años.Contains(año.ToString()))
                        años.Add(año.ToString());
                }
            }

            ViewBag.años = años;
            return View(model);
        }

        [AccessAudit(Funcion = "PlaAud")]
        public ActionResult IndexTablePlaneacion(int year)
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var model = db.k_auditoria.Where(k => idsDivisiones.Contains(k.c_auditoria.id_division_auditoria ?? 0)).ToList();
            List<k_auditoria> Model = new List<k_auditoria>();

            foreach (var item in model)
            {
                if (item.fe_inicial_planeada.HasValue)
                {
                    var año = item.fe_inicial_planeada.Value.Year;

                    if (año == year)
                        Model.Add(item);
                }
            }

            return PartialView(Model);
        }


        #region Create
        [AccessAudit(Funcion = "PlaAud")]
        public ActionResult CreatePlaneacion()
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var model = new k_auditoria();

            ViewBag.id_auditoria = DropDown.Auditorias(idsDivisiones:idsDivisiones);
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();


            return View(model);
        }


        // POST: BDEI/Create
        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead, AccessAudit(Funcion = "PlaAud")]
        public ActionResult CreatePlaneacion(k_auditoria model, string[] campoe, int[] idcampoe)
        {
            bool valid = validateCE(campoe, idcampoe);
            int len;

            if (ModelState.IsValid && valid)
            {
                db.k_auditoria.Add(model);
                db.SaveChanges();

                if (campoe != null && idcampoe != null)
                {
                    len = campoe.Length;
                    for (int i = 0; i < len; i++)
                    {
                        var idCE = idcampoe[i];
                        var valCE = campoe[i];

                        var kcampo = new k_campo_auditoria
                        {
                            idd_auditoria = model.idd_auditoria,
                            id_campo_auditoria = idCE,
                            aplica = true,
                            valor = valCE
                        };

                        db.k_campo_auditoria.Add(kcampo);
                    }
                }


                db.SaveChanges();

                return RedirectToAction("IndexPlaneacion");
            }

            //incluir los campos extra en los datos de viewBag
            Dictionary<int, string> DCE = new Dictionary<int, string>();

            if (campoe != null && idcampoe != null)
            {
                len = campoe.Length;
                for (int i = 0; i < len; i++)
                {
                    DCE.Add(idcampoe[i], campoe[i]);
                }
            }


            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.id_auditoria = Utilidades.DropDown.Auditorias(idsDivisiones:idsDivisiones);
            ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            return View(model);
        }
        #endregion

        #region Edit

        [AccessAudit(Funcion = "PlaAud")]
        public ActionResult EditPlaneacion(int id)
        {
            var model = db.k_auditoria.Find(id);

            Dictionary<int, string> DCE = new Dictionary<int, string>();
            var kces = model.k_campo_auditoria;

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }

            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            ViewBag.id_auditoria = Utilidades.DropDown.Auditorias(model.id_auditoria,idsDivisiones);
            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead, AccessAudit(Funcion = "PlaAud")]
        public ActionResult EditPlaneacion([Bind(Include = "idd_auditoria,id_auditoria,no_auditores,no_horas_estimadas,fe_inicial_planeada,fe_final_planeada,justificacion_incumplimiento")] k_auditoria model, string[] campoe, int[] idcampoe)
        {
            var register = db.k_auditoria.Find(model.idd_auditoria);
            bool valid = validateCE(campoe, idcampoe);
            int len;

            if (ModelState.IsValid && valid)
            {

                var kCampos = register.k_campo_auditoria.ToList();


                register.no_auditores = model.no_auditores;
                register.no_horas_estimadas = model.no_horas_estimadas;
                register.fe_inicial_planeada = model.fe_inicial_planeada;
                register.fe_final_planeada = model.fe_final_planeada;
                register.justificacion_incumplimiento = model.justificacion_incumplimiento;

                db.SaveChanges();




                if (campoe != null && idcampoe != null)
                {
                    len = campoe.Length;
                    for (int i = 0; i < len; i++)
                    {
                        var idCE = idcampoe[i];
                        var valCE = campoe[i];



                        //si ya existe un k_campo ligado al c_campo Solo se editará
                        if (kCampos.Any(c => c.id_campo_auditoria == idCE))
                        {
                            var campo = kCampos.FirstOrDefault(cext => cext.id_campo_auditoria == idCE);
                            campo.valor = valCE;
                        }
                        else //En caso contrario se creará un nuevo campo
                        {
                            var kcampo = new k_campo_auditoria
                            {
                                idd_auditoria = model.idd_auditoria,
                                id_campo_auditoria = idCE,
                                aplica = true,
                                valor = valCE
                            };

                            db.k_campo_auditoria.Add(kcampo);
                        }
                    }
                }


                db.SaveChanges();

                return RedirectToAction("IndexPlaneacion");
            }

            //incluir los campos extra en los datos de viewBag
            Dictionary<int, string> DCE = new Dictionary<int, string>();


            if (campoe != null && idcampoe != null)
            {
                len = campoe.Length;
                for (int i = 0; i < len; i++)
                {
                    DCE.Add(idcampoe[i], campoe[i]);
                }
            }

            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.id_auditoria = Utilidades.DropDown.Auditorias(idsDivisiones:idsDivisiones);
            ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            return View(register);
        }

        #endregion

        #region Delete

        [AccessAudit(Funcion = "PlaAud")]
        public ActionResult DeletePlaneacion(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            k_auditoria k_auditoria = db.k_auditoria.Find(id);
            if (k_auditoria == null)
            {
                return HttpNotFound();
            }

            return View(k_auditoria);
        }

        [HttpPost, ActionName("DeletePlaneacion")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        [AccessAudit(Funcion = "PlaAud")]
        public ActionResult DeleteConfirmedPlaneacion(int id)
        {
            k_auditoria k_auditoria = db.k_auditoria.Find(id);

            DeleteActions.DeleteKAuditoriaObjects(k_auditoria, db, true);

            db.SaveChanges();

            try
            {
                db.k_auditoria.Remove(k_auditoria);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return RedirectToAction("CantErase", "Error", new
                {
                    errorMSG = e.InnerException.Message
                });
            }

            return RedirectToAction("IndexPlaneacion");
        }

        #endregion

        private bool validateCE(string[] campoe, int[] idcampoe)
        {
            if (campoe != null && idcampoe != null)
            {
                var len = campoe.Length;
                Dictionary<int, string> Errors = new Dictionary<int, string>();


                for (int i = 0; i < len; i++)
                {
                    var value = campoe[i];
                    var idCampo = idcampoe[i];

                    var CE = db.c_campo_auditoria.Find(idCampo);

                    if (CE.es_requerido && string.IsNullOrEmpty(value))
                    {
                        Errors.Add(idCampo, "El campo \"" + CE.nb_campo + "\" es un campo requerido.");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(value))
                            if (value.Length > CE.longitud)
                                Errors.Add(idCampo, "La longitud máxima de el campo " + CE.nb_campo + " es de: " + CE.longitud + " caracteres.");

                    }

                }

                ControllerContext.Controller.ViewBag.Errors = Errors;

                return Errors.Count == 0;
            }

            return true;
        }

        public string InfoAuditoria(int id_auditoria)
        {
            var audit = db.c_auditoria.Find(id_auditoria);

            infoAudit info;


            if (audit == null)
            {
                info = new infoAudit();
            }
            else
            {
                info = new infoAudit
                {
                    Entidad = audit.c_entidad.nb_entidad,
                    Area = audit.c_area.nb_area,
                    Tipo = audit.es_regulada ? "Regulada" : "Normal",
                    Solicitante = audit.c_solicitante_auditoria.nb_solicitante_auditoria,
                    Division = audit.c_division_auditoria.ds_division_auditoria
                };
            }



            var res = JsonConvert.SerializeObject(info);

            return res;
        }


        class infoAudit
        {
            public string Entidad { get; set; }
            //public string Año { get; set; }
            public string Area { get; set; }
            public string Tipo { get; set; }
            public string Solicitante { get; set; }
            public string Division { get; set; }
        }
        #endregion

        #region Ejecución
        [AccessAudit(Funcion = "EjeAud")]
        public ActionResult IndexEjecucion()
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var model = db.k_auditoria.Where(k => idsDivisiones.Contains(k.c_auditoria.id_division_auditoria ?? 0)).ToList();

            return View(model);
        }

        #region Edit
        [AccessAudit(Funcion = "EjeAud")]
        public ActionResult EditEjecucion(int id)
        {
            //incluiur datos de la auditoria
            var model = db.k_auditoria.Find(id);

            Dictionary<int, string> DCE = new Dictionary<int, string>();
            var kces = model.k_campo_auditoria.Where(kce => !kce.c_campo_auditoria.aparece_en_informe && !kce.c_campo_auditoria.aparece_en_planeacion);

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }
            ViewBag.InfoCamposExtraPA = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            var AuditoriaRegulada = model.c_auditoria.es_regulada;

            //Campos extra para la ejecución
            DCE = new Dictionary<int, string>();
            kces = model.k_campo_auditoria.Where(kce => kce.c_campo_auditoria.aparece_en_planeacion).ToList();

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion && !c.es_auditoria_regulada).ToList();
            }


            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.DCEPD = DCE;




            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead, AccessAudit(Funcion = "EjeAud")]
        public ActionResult EditEjecucion([Bind(Include = "idd_auditoria,antecedentes,datos_relevantes,responsable_revision,auditores_participantes,fe_final_real,fe_inicial_real")] k_auditoria model, string[] campoe, int[] idcampoe, int[] files)
        {
            var register = db.k_auditoria.Find(model.idd_auditoria);
            bool valid = validateCE(campoe, idcampoe);
            int len;

            if (valid)
            {

                var kCampos = register.k_campo_auditoria.ToList();


                register.antecedentes = model.antecedentes;
                register.datos_relevantes = model.datos_relevantes;
                register.responsable_revision = model.responsable_revision;
                register.auditores_participantes = model.auditores_participantes;
                register.fe_final_real = model.fe_final_real;
                register.fe_inicial_real = model.fe_inicial_real;

                db.SaveChanges();

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        register.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }


                if (campoe != null && idcampoe != null)
                {
                    len = campoe.Length;
                    for (int i = 0; i < len; i++)
                    {
                        var idCE = idcampoe[i];
                        var valCE = campoe[i];



                        //si ya existe un k_campo ligado al c_campo Solo se editará
                        if (kCampos.Any(c => c.id_campo_auditoria == idCE))
                        {
                            var campo = kCampos.FirstOrDefault(cext => cext.id_campo_auditoria == idCE);
                            campo.valor = valCE;
                        }
                        else //En caso contrario se creará un nuevo campo
                        {
                            var kcampo = new k_campo_auditoria
                            {
                                idd_auditoria = model.idd_auditoria,
                                id_campo_auditoria = idCE,
                                aplica = true,
                                valor = valCE
                            };

                            db.k_campo_auditoria.Add(kcampo);
                        }
                    }
                }

                db.SaveChanges();

                return RedirectToAction("IndexEjecucion");
            }

            //incluir los campos extra en los datos de viewBag
            Dictionary<int, string> DCE = new Dictionary<int, string>();
            var kces = register.k_campo_auditoria.Where(kce => !kce.c_campo_auditoria.aparece_en_informe && !kce.c_campo_auditoria.aparece_en_planeacion);

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }
            ViewBag.InfoCamposExtraPA = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            var AuditoriaRegulada = register.c_auditoria.es_regulada;

            //Campos extra para la ejecución
            DCE = new Dictionary<int, string>();

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion && !c.es_auditoria_regulada).ToList();
            }

            if (campoe != null && idcampoe != null)
            {
                len = campoe.Length;
                for (int i = 0; i < len; i++)
                {
                    DCE.Add(idcampoe[i], campoe[i]);
                }
            }

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.DCEPD = DCE;

            return View(register);
        }
        #endregion

        #region Programa de trabajo
        [AccessAudit(Funcion = "EjeAud")]
        public ActionResult PruebasEjecucion(int id)
        {
            //incluiur datos de la auditoria
            var kAuditoria = db.k_auditoria.Find(id);

            ViewBag.kAuditoria = kAuditoria;
            var model = kAuditoria.c_auditoria.c_prueba_auditoria.ToList();

            return View(model);
        }


        [AccessAudit(Funcion = "EjeAud")]
        public ActionResult EditPrueba(int id, int idd_auditoria)
        {
            //incluiur datos de la auditoria
            var prueba = db.c_prueba_auditoria.Find(id);
            ViewBag.prueba = prueba;

            var model = prueba.k_programa_trabajo.FirstOrDefault(m => m.idd_auditoria == idd_auditoria);

            ViewBag.InfoCamposExtra = db.c_campo_programa.ToList();

            if (model == null)
            {
                model = new k_programa_trabajo
                {
                    id_prueba_auditoria = id,
                    idd_auditoria = idd_auditoria
                };
            }
            else
            {
                Dictionary<int, string> DCE = new Dictionary<int, string>();

                var campos = model.k_campo_programa.ToList();
                foreach (var ce in campos)
                {
                    DCE.Add(ce.id_campo_programa, ce.valor);
                }

                ViewBag.DCE = DCE;

            }

    
            
            ViewBag.id_estatus_programa_trabajo = DropDown.EstatusPrograma(model.id_estatus_programa_trabajo ?? 0);
            ViewBag.id_criticidad_programa_trabajo = DropDown.CriticidadPrograma(model.id_criticidad_programa_trabajo ?? 0);
            ViewBag.nb_division_auditoria = (prueba.c_auditoria.id_division_auditoria ?? 0) != 0 ? prueba.c_auditoria.c_division_auditoria.ds_division_auditoria : "N/A";


            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead, AccessAudit(Funcion = "EjeAud")]
        public ActionResult EditPrueba(k_programa_trabajo model, string[] campoe, int[] idcampoe, int[] files)
        {
            k_programa_trabajo register = model;
            List<k_campo_programa> kCampos = new List<k_campo_programa>();


            bool valid = validateCEP(campoe, idcampoe);
            int len;

            if (ModelState.IsValid && valid)
            {
                if (model.idd_programa_trabajo == 0)
                {
                    db.k_programa_trabajo.Add(model);
                    db.SaveChanges();
                    register = db.k_programa_trabajo.Find(model.idd_programa_trabajo);
                }
                else
                {
                    register = db.k_programa_trabajo.Find(model.idd_programa_trabajo);

                    kCampos = register.k_campo_programa.ToList();

                    register.resultado = model.resultado;
                    register.tiene_incidencia = model.tiene_incidencia;
                    register.incidencia_detectada = model.tiene_incidencia ? model.incidencia_detectada : "";
                    register.recomendaciones = model.tiene_incidencia ? model.recomendaciones : "";
                    register.id_estatus_programa_trabajo = model.tiene_incidencia ? model.id_estatus_programa_trabajo : null;
                    register.id_criticidad_programa_trabajo = model.tiene_incidencia ? model.id_criticidad_programa_trabajo : null;
                    register.tema = model.tiene_incidencia ? model.tema : null;
                    register.responsable = model.tiene_incidencia ? model.responsable : null;
                    register.es_duplicada = model.tiene_incidencia ? model.es_duplicada : false;

                    db.SaveChanges();
                }


                if (campoe != null && idcampoe != null)
                {
                    len = campoe.Length;
                    for (int i = 0; i < len; i++)
                    {
                        var idCE = idcampoe[i];
                        var valCE = campoe[i];



                        //si ya existe un k_campo ligado al c_campo Solo se editará
                        if (kCampos.Any(c => c.id_campo_programa == idCE))
                        {
                            var campo = kCampos.FirstOrDefault(cext => cext.id_campo_programa == idCE);
                            campo.valor = valCE;
                        }
                        else //En caso contrario se creará un nuevo campo
                        {
                            var kcampo = new k_campo_programa
                            {
                                idd_programa_trabajo = model.idd_programa_trabajo,
                                id_campo_programa = idCE,
                                aplica = true,
                                valor = valCE
                            };

                            db.k_campo_programa.Add(kcampo);
                        }
                    }
                }

                if (files != null)
                    foreach (var idar in files)
                    {
                        var file = db.c_archivo.Find(idar);
                        register.c_archivo.Add(file);
                    }


                db.SaveChanges();

                return RedirectToAction("PruebasEjecucion", new { id = register.idd_auditoria });
            }

            //incluir los campos extra en los datos de viewBag
            Dictionary<int, string> DCE = new Dictionary<int, string>();


            if (campoe != null && idcampoe != null)
            {
                len = campoe.Length;
                for (int i = 0; i < len; i++)
                {
                    DCE.Add(idcampoe[i], campoe[i]);
                }
            }

            var prueba = db.c_prueba_auditoria.Find(model.id_prueba_auditoria);
            ViewBag.prueba = prueba;

            ViewBag.DateFormat = Utilidades.Utilidades.GetDateFormat();
            ViewBag.InfoCamposExtra = db.c_campo_programa.ToList();
            ViewBag.DCE = DCE;

            ViewBag.id_estatus_programa_trabajo = DropDown.EstatusPrograma(model.id_estatus_programa_trabajo ?? 0);
            ViewBag.id_criticidad_programa_trabajo = DropDown.CriticidadPrograma(model.id_criticidad_programa_trabajo ?? 0);

            return View(register);
        }


        private bool validateCEP(string[] campoe, int[] idcampoe)
        {
            if (campoe != null && idcampoe != null)
            {
                var len = campoe.Length;
                Dictionary<int, string> Errors = new Dictionary<int, string>();


                for (int i = 0; i < len; i++)
                {
                    var value = campoe[i];
                    var idCampo = idcampoe[i];

                    var CE = db.c_campo_programa.Find(idCampo);

                    if (CE.es_requerido && string.IsNullOrEmpty(value))
                    {
                        Errors.Add(idCampo, "El campo \"" + CE.nb_campo + "\" es un campo requerido.");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(value))
                            if (value.Length > CE.longitud)
                                Errors.Add(idCampo, "La longitud máxima de el campo " + CE.nb_campo + " es de: " + CE.longitud + " caracteres.");

                    }

                }

                ControllerContext.Controller.ViewBag.Errors = Errors;

                return Errors.Count == 0;
            }

            return true;
        }
        #endregion

        #endregion

        #region Informe
        [AccessAudit(Funcion = "InfAud")]
        public ActionResult IndexInformes()
        {
            var id_usuario = ((IdentityPersonalizado)User.Identity).Id_usuario;
            var us = db.c_usuario.Find(id_usuario);

            var idsDivisiones = us.c_division_auditoria.Select(d => d.id_division_auditoria).ToList();

            var model = db.k_auditoria.Where(k => idsDivisiones.Contains(k.c_auditoria.id_division_auditoria ?? 0)).ToList();

            return View(model);
        }

        #region Edit
        [AccessAudit(Funcion = "InfAud")]
        public ActionResult EditInforme(int id)
        {
            //incluiur datos de la auditoria
            var model = db.k_auditoria.Find(id);

            Dictionary<int, string> DCE = new Dictionary<int, string>();
            var kces = model.k_campo_auditoria.Where(kce => !kce.c_campo_auditoria.aparece_en_informe && !kce.c_campo_auditoria.aparece_en_planeacion);

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }
            ViewBag.InfoCamposExtraPA = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            var AuditoriaRegulada = model.c_auditoria.es_regulada;

            //Campos extra para la ejecución
            DCE = new Dictionary<int, string>();
            kces = model.k_campo_auditoria.Where(kce => kce.c_campo_auditoria.aparece_en_planeacion).ToList();

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtraEJ = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtraEJ = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion && !c.es_auditoria_regulada).ToList();
            }

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }

            ViewBag.DCEPD = DCE;


            //campos extra informe
            DCE = new Dictionary<int, string>();
            kces = model.k_campo_auditoria.Where(kce => kce.c_campo_auditoria.aparece_en_informe).ToList();

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_informe).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_informe && !c.es_auditoria_regulada).ToList();
            }

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }

            ViewBag.DCEINF = DCE;

            ViewBag.id_rating_auditoria = Utilidades.DropDown.RatingAuditoria(model.id_rating_auditoria ?? 0);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead, AccessAudit(Funcion = "InfAud")]
        public ActionResult EditInforme([Bind(Include = "idd_auditoria,id_rating_auditoria")] k_auditoria model, string[] campoe, int[] idcampoe, int[] files)
        {
            var register = db.k_auditoria.Find(model.idd_auditoria);
            bool valid = validateCE(campoe, idcampoe);
            int len;

            //validar rating
            if(model.id_rating_auditoria == null)
            {
                ModelState.AddModelError("id_rating_auditoria", "Este campo es obligatorio.");
                valid = false;
            }


            if (valid)
            {
                register.id_rating_auditoria = model.id_rating_auditoria;
                var kCampos = register.k_campo_auditoria.ToList();


                if (campoe != null && idcampoe != null)
                {
                    len = campoe.Length;
                    for (int i = 0; i < len; i++)
                    {
                        var idCE = idcampoe[i];
                        var valCE = campoe[i];



                        //si ya existe un k_campo ligado al c_campo Solo se editará
                        if (kCampos.Any(c => c.id_campo_auditoria == idCE))
                        {
                            var campo = kCampos.FirstOrDefault(cext => cext.id_campo_auditoria == idCE);
                            campo.valor = valCE;
                        }
                        else //En caso contrario se creará un nuevo campo
                        {
                            var kcampo = new k_campo_auditoria
                            {
                                idd_auditoria = model.idd_auditoria,
                                id_campo_auditoria = idCE,
                                aplica = true,
                                valor = valCE
                            };

                            db.k_campo_auditoria.Add(kcampo);
                        }
                    }
                }

                db.SaveChanges();

                //agregar los archivos
                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        register.c_archivo1.Add(archivo);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("IndexInformes");
            }


            Dictionary<int, string> DCE = new Dictionary<int, string>();
            var kces = register.k_campo_auditoria.Where(kce => !kce.c_campo_auditoria.aparece_en_informe && !kce.c_campo_auditoria.aparece_en_planeacion);

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }
            ViewBag.InfoCamposExtraPA = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();
            ViewBag.DCE = DCE;

            var AuditoriaRegulada = register.c_auditoria.es_regulada;

            //Campos extra para la ejecución
            DCE = new Dictionary<int, string>();
            kces = register.k_campo_auditoria.Where(kce => kce.c_campo_auditoria.aparece_en_planeacion).ToList();

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtraEJ = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtraEJ = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion && !c.es_auditoria_regulada).ToList();
            }

            foreach (var kce in kces)
            {
                DCE.Add(kce.id_campo_auditoria, kce.valor);
            }

            ViewBag.DCEPD = DCE;





            //Campos extra informe
            DCE = new Dictionary<int, string>();

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_informe).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_informe && !c.es_auditoria_regulada).ToList();
            }

            if (campoe != null && idcampoe != null)
            {
                len = campoe.Length;
                for (int i = 0; i < len; i++)
                {
                    DCE.Add(idcampoe[i], campoe[i]);
                }
            }

            ViewBag.DCEINF = DCE;

            ViewBag.id_rating_auditoria = Utilidades.DropDown.RatingAuditoria(register.id_rating_auditoria ?? 0);

            return View(register);
        }
        #endregion

        #region GenerarInforme
        [AccessAudit(Funcion = "InfAud")]
        public ActionResult Informe(int id)
        {
            var model = db.k_auditoria.Find(id);

            ViewBag.InfoCamposExtraPA = db.c_campo_auditoria.Where(c => !c.aparece_en_planeacion && !c.aparece_en_informe).ToList();

            var AuditoriaRegulada = model.c_auditoria.es_regulada;

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtraEJ = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtraEJ = db.c_campo_auditoria.Where(c => c.aparece_en_planeacion && !c.es_auditoria_regulada).ToList();
            }

            if (AuditoriaRegulada)
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_informe).ToList();
            }
            else
            {
                ViewBag.InfoCamposExtra = db.c_campo_auditoria.Where(c => c.aparece_en_informe && !c.es_auditoria_regulada).ToList();
            }

            return PartialView(model);
        }

        public int GenInforme(InfAudViewModel model)
        {
            var TD = DateTime.Now;
            var stringUUID = TD.Minute.ToString() + TD.Second;
            var UUID = int.Parse(stringUUID);
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + UUID);


            var type = model.type;

            using (FileStream fs = System.IO.File.Create(path))
            {
                byte[] info = Utilidades.GenerateDoc.InfAud(model);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }


            return UUID;
        }

        public FileResult GetInforme(int id)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + id);
            var bytes = System.IO.File.ReadAllBytes(path);


            return File(bytes, "application/pdf");
        }

        public string SetInformeDef(int id,int idd_auditoria)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + id);
            var Spath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/InformesAuditoria/" + idd_auditoria);
            var tempFile = System.IO.File.Open(path,FileMode.Open);

            var file = System.IO.File.Create(Spath);

            tempFile.CopyTo(file);

            tempFile.Close();
            file.Close();

            return "ok";
        }
        #endregion

        #region Informe Definitivo
        public ActionResult InformeDefinitivo(int id)
        {
            var model = db.k_auditoria.Find(id);

            var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/InformesAuditoria/"));

            var exist = false;

            foreach(var file in files)
            {
                if(file.Split(new char[] {'\\' }).Last() == id.ToString())
                {
                    exist = true;
                    break;
                }
            }

            ViewBag.hasFile = exist;

            return View(model);
        }



        public FileResult GetInformeDef(int id)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/InformesAuditoria/" + id);
            var bytes = System.IO.File.ReadAllBytes(path);


            return File(bytes, "application/pdf");
        }

        [HttpPost]
        [NotOnlyRead]
        public ActionResult InformeDefP(HttpPostedFileBase archivo,int id)
        {
            if (archivo == null)
            {
                return RedirectToAction("IndexInformes");
            }

            try
            {
                archivo.SaveAs(Server.MapPath("~/App_Data/InformesAuditoria/" + id));
                return RedirectToAction("IndexInformes");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("IndexInformes");
            }
        }

        #endregion

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
