using Microsoft.Win32;
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
using System.Linq.Dynamic.Core;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "MRyC", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class ControlController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: Control
        public ActionResult Index()
        {
            var k_control = db.k_control.Include(k => k.c_categoria_control).Include(k => k.c_frecuencia_control).Include(k => k.c_grado_cobertura).Include(k => k.c_naturaleza_control).Include(k => k.c_sub_proceso).Include(k => k.c_tipo_evidencia).Include(k => k.c_tipologia_control).Include(k => k.c_usuario).Include(k => k.c_usuario1);
            return View(k_control.ToList());
        }

        #region Create section
        public ActionResult Agregar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_riesgo k_riesgo = db.k_riesgo.Find(id);
            if (k_riesgo == null)
            {
                return HttpNotFound();
            }

            string prefijo = k_riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);


            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_riesgo.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;


            //string code = Utilidades.Utilidades.CCodeGen(k_riesgo.c_sub_proceso);

            //Mandamos las listas necesarias para la creacion de la incidencia en caso de ser necesaria
            ViewBag.id_responsable_i = Utilidades.DropDown.Usuario();
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();


            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            if (prefijo == "MP")
            {
                AgregarControlViewModel model = new AgregarControlViewModel();

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;
                //model.relacion_control = code;

                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_ejecutor);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);
                ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS();



                ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
                ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
                ViewBag.MCError = new string[20];

                return View(model);
            }
            else
            {
                AgregarControlMGViewModel model = new AgregarControlMGViewModel();

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;
                //model.relacion_control = code;

                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);

                ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
                ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
                ViewBag.MCError = new string[20];

                return View("AgregarMG", model);
            }
        }

        // POST: Control/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Agregar(AgregarControlViewModel model, int[] id_aseveracion, int[] files)
        {
            k_riesgo k_riesgo;
            k_control k_control;
            bool aux = false;
            bool aux2 = false;

            int nAseveraciones = 0;

            if (id_aseveracion != null) nAseveraciones = id_aseveracion.Count();

            //Resolver inconsistencias que tienen que ver con el sub proceso y la


            aux2 = ValidarIncidencia(ModelState, model);

            if (model.id_responsable == null || model.id_responsable == 0)
            {
                ModelState.AddModelError("id_responsable", "El responsable del control es un campo obligatorio.");
            }
            else
            {
                aux = true;
            }

            if (model.tiene_accion_correctora && aux && aux2)
            {
                k_riesgo = db.k_riesgo.Find(model.k_riesgo.id_riesgo);
                k_control = new k_control();
                k_control.id_control = 0;
                k_control.id_sub_proceso = model.id_sub_proceso;
                k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                k_control.accion_correctora = model.accion_correctora;
                k_control.id_responsable = model.id_responsable;
                //agregar nuevo control
                db.k_control.Add(k_control);
                //agregar entrada en k_riesgo_control
                k_control.k_riesgo.Add(k_riesgo);

                db.SaveChanges();

                //Utilidades.Utilidades.disposeCParam(model.relacion_control, k_control.id_sub_proceso);

                //Agregamos la Incidencia
                var incidencia = new k_incidencia()
                {
                    id_control = k_control.id_control,
                    id_responsable = (int)model.id_responsable_i,
                    id_clasificacion_incidencia = (int)model.id_clasificacion_incidencia,
                    ds_incidencia = model.ds_incidencia,
                    requiere_plan = model.requiere_plan,
                    js_incidencia = model.js_incidencia
                };

                db.k_incidencia.Add(incidencia);
                db.SaveChanges();
                Utilidades.Notification.IncidenciaAsignada(incidencia);
                Utilidades.Utilidades.TaskAsigned(k_control);
                return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });
            }
            else
            {
                //Validamos los campos extra
                bool valid = ValidarCE(model);

                if (ModelState.IsValid && valid)
                {
                    k_riesgo = db.k_riesgo.Find(model.k_riesgo.id_riesgo);
                    k_control = new k_control();
                    k_control.id_control = 0;
                    k_control.id_sub_proceso = model.id_sub_proceso;
                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_ejecutor = model.id_ejecutor;
                    k_control.id_responsable = model.id_responsable;
                    k_control.es_control_clave = model.es_control_clave;
                    k_control.id_grado_cobertura = model.id_grado_cobertura;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;
                    k_control.nb_aplicacion = model.nb_aplicacion;
                    k_control.id_tipologia_control = model.id_tipologia_control;
                    k_control.id_categoria_control = model.id_categoria_control;
                    k_control.id_tipo_evidencia = model.id_tipo_evidencia;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = "";
                    k_control = llenarCamposExtra(model, k_control);


                    //agregar nuevo control
                    db.k_control.Add(k_control);
                    //agregar entrada en k_riesgo_control
                    k_control.k_riesgo.Add(k_riesgo);

                    db.SaveChanges();

                    //archivos
                    if (files != null)
                    {
                        foreach (int file in files)
                         {
                            c_archivo archivo = db.c_archivo.Find(file);
                    
                            k_control.c_archivo.Add(archivo);
                         }

                     db.SaveChanges();
                    }


                    if (nAseveraciones > 0)
                    {
                        db = new SICIEntities();

                        var modelControl = db.k_control.Find(k_control.id_control);

                        for (int i = 0; i < nAseveraciones; i++)
                        {
                            var aseveracion = db.c_aseveracion.Find(id_aseveracion[i]);

                            modelControl.c_aseveracion.Add(aseveracion);
                        }

                        db.SaveChanges();
                    }


                    //Utilidades.Utilidades.disposeCParam(model.relacion_control, k_control.id_sub_proceso);
                    Utilidades.Utilidades.TaskAsigned(k_control);
                    return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });
                }
            }

            k_riesgo = new k_riesgo();
            k_riesgo = db.k_riesgo.Find(model.k_riesgo.id_riesgo);

            c_sub_proceso c_sub_proceso = new c_sub_proceso();
            c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.k_riesgo = k_riesgo;
            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

            ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
            ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
            ViewBag.id_sub_proceso = new SelectList(db.c_sub_proceso, "id_sub_proceso", "cl_sub_proceso");
            ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
            ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
            ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_ejecutor);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);
            ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(id_aseveracion);



            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //Mandamos las listas necesarias para la creacion de la incidencia en caso de ser necesaria
            ViewBag.id_responsable_i = Utilidades.DropDown.Usuario();
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + model.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }



            ViewBag.JsonData = JsonData;

            return View(model);
        }

        // POST: Control/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AgregarMG(AgregarControlMGViewModel model)
        {
            k_riesgo k_riesgo;
            k_control k_control;
            bool aux = false;
            bool aux2 = false;

            aux2 = ValidarIncidencia(ModelState, null, model);

            //archivos


            if (model.id_responsable == null || model.id_responsable == 0)
            {
                ModelState.AddModelError("id_responsable", "El responsable del control es un campo obligatorio.");
            }
            else
            {
                aux = true;
            }

            if (model.tiene_accion_correctora && aux && aux2)
            {
                k_riesgo = db.k_riesgo.Find(model.k_riesgo.id_riesgo);
                k_control = new k_control();
                k_control.id_control = 0;
                k_control.id_sub_proceso = model.id_sub_proceso;
                k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                k_control.accion_correctora = model.accion_correctora;
                k_control.id_responsable = model.id_responsable;
                //agregar nuevo control
                db.k_control.Add(k_control);
                //agregar entrada en k_riesgo_control
                k_control.k_riesgo.Add(k_riesgo);

                db.SaveChanges();

                //Utilidades.Utilidades.disposeCParam(model.relacion_control, k_control.id_sub_proceso);

                //Agregamos la Incidencia
                var incidencia = new k_incidencia()
                {
                    id_control = k_control.id_control,
                    id_responsable = (int)model.id_responsable_i,
                    id_clasificacion_incidencia = (int)model.id_clasificacion_incidencia,
                    ds_incidencia = model.ds_incidencia,
                    requiere_plan = model.requiere_plan,
                    js_incidencia = model.js_incidencia
                };

                db.k_incidencia.Add(incidencia);
                db.SaveChanges();
                Utilidades.Notification.IncidenciaAsignada(incidencia);
                Utilidades.Utilidades.TaskAsigned(k_control);
                return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });
            }
            else
            {
                //Validamos los campos extra
                bool valid = ValidarCE(model);

                if (ModelState.IsValid && valid)
                {
                    k_riesgo = db.k_riesgo.Find(model.k_riesgo.id_riesgo);
                    k_control = new k_control();
                    k_control.id_control = 0;
                    k_control.id_sub_proceso = model.id_sub_proceso;
                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_responsable = model.id_responsable;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = model.accion_correctora;
                    k_control = llenarCamposExtra(model, k_control);


                    //agregar nuevo control
                    db.k_control.Add(k_control);
                    //agregar entrada en k_riesgo_control
                    k_control.k_riesgo.Add(k_riesgo);

                    db.SaveChanges();

                    //Utilidades.Utilidades.disposeCParam(model.relacion_control, k_control.id_sub_proceso);
                    Utilidades.Utilidades.TaskAsigned(k_control);
                    return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });
                }
            }

            k_riesgo = new k_riesgo();
            k_riesgo = db.k_riesgo.Find(model.k_riesgo.id_riesgo);

            c_sub_proceso c_sub_proceso = new c_sub_proceso();
            c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.k_riesgo = k_riesgo;
            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;


            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", model.id_frecuencia_control);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", model.id_naturaleza_control);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);

            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //Mandamos las listas necesarias para la creacion de la incidencia en caso de ser necesaria
            ViewBag.id_responsable_i = Utilidades.DropDown.Usuario();
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + model.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            return View("AgregarMG", model);
        }

        // GET: Control/CreateRR/5
        public ActionResult CreateRR(int? id, bool readOnly = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }
            k_riesgo_residual RiesgoResidual = new k_riesgo_residual();
            RiesgoResidual.id_control = (int)id;

            //campos cobertura control
            ViewBag.cccInfo = db.a_campo_cobertura_control.ToList();

            //Listas para el cuestionario
            ViewBag.dc_ejecucion_sistematica = listaCampoCoberturaControl(1, 1);
            ViewBag.dc_pistas_auditoria = listaCampoCoberturaControl(1, 2);
            ViewBag.dc_cumplimiento_politicas_entidad = listaCampoCoberturaControl(1, 3);
            ViewBag.dc_umbrales_adecuados_indicadores = listaCampoCoberturaControl(1, 4);
            ViewBag.dc_metodologia_reporte = listaCampoCoberturaControl(1, 5);
            ViewBag.dc_evaluacion_diseño = listaCampoCoberturaControl(1, 6);
            ViewBag.dc_responsable_diseño = listaCampoCoberturaControl(1, 7);
            ViewBag.dc_entrada_datos = listaCampoCoberturaControl(1, 8);
            ViewBag.dc_resultado_sin_manipulacion = listaCampoCoberturaControl(1, 9);

            ViewBag.ec_planes_remediacion = listaCampoCoberturaControl(2, 1);
            ViewBag.ec_eventos_relacionados = listaCampoCoberturaControl(2, 2);
            ViewBag.ec_evaluacion_efectividad = listaCampoCoberturaControl(2, 3);
            ViewBag.ec_efectividad_indicadores = listaCampoCoberturaControl(2, 4);
            ViewBag.ec_tiempo_madurez_control = listaCampoCoberturaControl(2, 5);

            var riesgo = k_control.k_riesgo.First();
            ViewBag.Riesgo_Inherente = String.Format("${0:N2}", ((double)riesgo.c_magnitud_impacto.magnitud_impacto * (double)riesgo.c_probabilidad_ocurrencia.pr_probabilidad_ocurrencia) * .01);

            ViewBag.readOnly = readOnly;

            return PartialView("CreateRR", RiesgoResidual);
        }

        // POST: Control/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult CreateRR(k_riesgo_residual model)
        {
            if (ModelState.IsValid)
            {
                db.k_riesgo_residual.Add(model);
                db.SaveChanges();
                return RedirectToAction("DetailsRR", new { id = model.id_control });
            }
            //Listas para el cuestionario
            ViewBag.dc_ejecucion_sistematica = listaCampoCoberturaControl(1, 1);
            ViewBag.dc_pistas_auditoria = listaCampoCoberturaControl(1, 2);
            ViewBag.dc_cumplimiento_politicas_entidad = listaCampoCoberturaControl(1, 3);
            ViewBag.dc_umbrales_adecuados_indicadores = listaCampoCoberturaControl(1, 4);
            ViewBag.dc_metodologia_reporte = listaCampoCoberturaControl(1, 5);
            ViewBag.dc_evaluacion_diseño = listaCampoCoberturaControl(1, 6);
            ViewBag.dc_responsable_diseño = listaCampoCoberturaControl(1, 7);
            ViewBag.dc_entrada_datos = listaCampoCoberturaControl(1, 8);
            ViewBag.dc_resultado_sin_manipulacion = listaCampoCoberturaControl(1, 9);

            ViewBag.ec_planes_remediacion = listaCampoCoberturaControl(2, 1);
            ViewBag.ec_eventos_relacionados = listaCampoCoberturaControl(2, 2);
            ViewBag.ec_evaluacion_efectividad = listaCampoCoberturaControl(2, 3);
            ViewBag.ec_efectividad_indicadores = listaCampoCoberturaControl(2, 4);
            ViewBag.ec_tiempo_madurez_control = listaCampoCoberturaControl(2, 5);

            var control = db.k_control.Find(model.id_control);
            var riesgo = control.k_riesgo.First();
            ViewBag.Riesgo_Inherente = String.Format("${0:N2}", ((double)riesgo.c_magnitud_impacto.magnitud_impacto * (double)riesgo.c_probabilidad_ocurrencia.pr_probabilidad_ocurrencia));
            ViewBag.readOnly = false;

            return PartialView("CreateRR", model);
        }

        #endregion

        #region Edit section
        // GET: Control/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
           
            if (k_control == null)
            {
                return HttpNotFound();
            }

            if (k_control.tiene_accion_correctora)
            {
                //k_control.relacion_control = Utilidades.Utilidades.CCodeGen(k_control.c_sub_proceso);
                k_control.relacion_control = "";
            }

          
            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_control.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            //datos del indicador relacionado
            if (db.c_indicador.Where(i => i.id_control == id).Count() > 0)
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == id).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }


            string prefijo = k_control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);

            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            ViewBag.lu = k_control.id_responsable;

            if (prefijo == "MP")
            {
                AgregarControlViewModel model = new AgregarControlViewModel();

                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_control = k_control; //busque en el model de k_control al c_archivo para los archivos

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

                model.id_control = k_control.id_control;
                model.actividad_control = k_control.actividad_control;
                model.relacion_control = k_control.relacion_control; //codigo de control
                model.evidencia_control = k_control.evidencia_control;
                model.es_control_clave = k_control.es_control_clave;
                model.nb_aplicacion = k_control.nb_aplicacion;
                model.tiene_accion_correctora = k_control.tiene_accion_correctora;
                model.accion_correctora = k_control.accion_correctora;

                //CAMPOS EXTRA
                model = obtenerCamposExtra(model, k_control);

                ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
                ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control", k_control.id_categoria_control);
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura", k_control.id_grado_cobertura);
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia", k_control.id_tipo_evidencia);
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control", k_control.id_tipologia_control);
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_ejecutor);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);
                ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(k_control.c_aseveracion.Select(a => a.id_aseveracion).ToArray());



                ViewBag.MCError = new string[20];


                //Riesgo Residual

                if (k_control.k_riesgo_residual.Count == 0)//Si no tiene riesgo residual
                {
                    ViewBag.RiesgoResidual = "null";
                }
                else
                {
                    ViewBag.RiesgoResidual = k_control.k_riesgo_residual.First();
                }


                return View(model);
            }
            else
            {
                AgregarControlMGViewModel model = new AgregarControlMGViewModel();

             

                //CAMPOS EXTRA
                model = obtenerCamposExtra(model, k_control);

                ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
                ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

                model.id_control = k_control.id_control;
                model.actividad_control = k_control.actividad_control;
                model.relacion_control = k_control.relacion_control; //codigo de control
                model.evidencia_control = k_control.evidencia_control;
                model.tiene_accion_correctora = k_control.tiene_accion_correctora;
                model.accion_correctora = k_control.accion_correctora;

                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);

                ViewBag.MCError = new string[20];
                return View("EditMG", model);
            }

        }

        // POST: Control/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(AgregarControlViewModel model, int lu, int[] id_aseveracion, int[] files)
        {
            k_riesgo k_riesgo;
            k_control k_control;
            bool aux = false;
            bool aux2 = false;

           

            int nAseveraciones = 0;
            if (id_aseveracion != null) nAseveraciones = id_aseveracion.Count();


            if (model.tiene_accion_correctora)
            {
                if (model.accion_correctora == "" || model.accion_correctora == null)
                {
                    ModelState.AddModelError("accion_correctora", "La acción correctora no puede quedar vacia");
                }
                else
                {
                    aux2 = true;
                }
            }

            if (model.id_responsable == null || model.id_responsable == 0)
            {
                ModelState.AddModelError("id_responsable", "El responsable del control es un campo obligatorio.");
            }
            else
            {
                aux = true;
            }


            if (model.tiene_accion_correctora && aux && aux2)
            {
                k_control = db.k_control.Find(model.id_control);

                recordChange(k_control);

                k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                k_control.accion_correctora = model.accion_correctora;
                k_control.id_responsable = model.id_responsable;
                db.SaveChanges();

                if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);

                return RedirectToAction("Controles", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
            }
            else
            {
                bool valid = ValidarCE(model);

                if (ModelState.IsValid && valid)
                {
                    k_control = db.k_control.Find(model.id_control);
                  

                    //agregar los archivos
                    if (ModelState.IsValid)
                    {
                       db.SaveChanges();

                       if (files != null)
                            foreach (var idar in files)
                            {
                               var file = db.c_archivo.Find(idar);
                                k_control.c_archivo.Add(file);
                            }
                       
                        db.SaveChanges();

                        return RedirectToAction("Controles", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
                    }

                    recordChange(k_control);

                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_ejecutor = model.id_ejecutor;
                    k_control.id_responsable = model.id_responsable;
                    k_control.es_control_clave = model.es_control_clave;
                    k_control.id_grado_cobertura = model.id_grado_cobertura;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;
                    k_control.nb_aplicacion = model.nb_aplicacion;
                    k_control.id_tipologia_control = model.id_tipologia_control;
                    k_control.id_categoria_control = model.id_categoria_control;
                    k_control.id_tipo_evidencia = model.id_tipo_evidencia;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = model.accion_correctora;

                    k_control = llenarCamposExtra(model, k_control);
                    db.SaveChanges();

                    if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);


                    //Actualizar las aseveraciones
                    db = new SICIEntities();
                    k_control = db.k_control.Find(k_control.id_control);
                    k_control.c_aseveracion.Clear();
                    db.SaveChanges();

                    if (nAseveraciones > 0)
                    {
                        for (int i = 0; i < nAseveraciones; i++)
                        {
                            var aseveracion = db.c_aseveracion.Find(id_aseveracion[i]);

                            k_control.c_aseveracion.Add(aseveracion);
                        }

                        db.SaveChanges();
                    }

                    return RedirectToAction("Controles", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
                }
            }

            k_control = db.k_control.Find(model.id_control);

            k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.k_riesgo = k_riesgo;
            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

            ViewBag.id_control = db.k_control.Find(k_control.id_control);

            ViewBag.lu = lu;

            ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control", k_control.id_categoria_control);
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
            ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura", k_control.id_grado_cobertura);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
            ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia", k_control.id_tipo_evidencia);
            ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control", k_control.id_tipologia_control);
            ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_ejecutor);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);
            ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(id_aseveracion);


            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //datos del indicador relacionado
            if (db.c_indicador.Any(i => i.id_control == model.id_control))
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == model.id_control).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_control.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            return View(model);
        }

        // POST: Control/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditMG(AgregarControlMGViewModel model, int lu)
        {
            k_riesgo k_riesgo;
            k_control k_control;
            bool aux = false;
            bool aux2 = false;

            if (model.tiene_accion_correctora)
            {
                if (model.accion_correctora == "" || model.accion_correctora == null)
                {
                    ModelState.AddModelError("accion_correctora", "La acción correctora no puede quedar vacia");
                }
                else
                {
                    aux2 = true;
                }
            }

            if (model.id_responsable == null || model.id_responsable == 0)
            {
                ModelState.AddModelError("id_responsable", "El responsable del control es un campo obligatorio.");
            }
            else
            {
                aux = true;
            }


            if (model.tiene_accion_correctora && aux && aux2)
            {
                k_control = db.k_control.Find(model.id_control);

                recordChange(k_control);

                k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                k_control.accion_correctora = model.accion_correctora;
                k_control.id_responsable = model.id_responsable;

                db.SaveChanges();
                if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);


                return RedirectToAction("Controles", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
            }
            else
            {
                bool valid = ValidarCE(model);

                if (ModelState.IsValid && valid)
                {
                    k_control = db.k_control.Find(model.id_control);

                   

                    recordChange(k_control);

                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_responsable = model.id_responsable;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = "";

                    k_control = llenarCamposExtra(model, k_control);

                    db.SaveChanges();
                    if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);

                    return RedirectToAction("Controles", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
                }
            }

            k_control = db.k_control.Find(model.id_control);

            k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.k_riesgo = k_riesgo;
            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

            ViewBag.lu = lu;
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);

            //datos del indicador relacionado
            if (db.c_indicador.Where(i => i.id_control == k_control.id_control).Count() > 0)
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == k_control.id_control).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }

            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_control.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            return View("EditMG", model);
        }

        // GET: Control/Edit/5
        public ActionResult EditR(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);

            if (k_control == null)
            {
                return HttpNotFound();
            }

            if (k_control.tiene_accion_correctora)
            {
                //k_control.relacion_control = Utilidades.Utilidades.CCodeGen(k_control.c_sub_proceso);
                k_control.relacion_control = "";
            }

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_control.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;


            //datos del indicador relacionado
            if (db.c_indicador.Any(i => i.id_control == id))
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == id).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }


            string prefijo = k_control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);

            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            ViewBag.MCError = new string[20];

            if (prefijo == "MP")
            {
                AgregarControlViewModel model = new AgregarControlViewModel();

                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_control = k_control; //busque en el model de k_control al c_archivo

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

                model.id_control = k_control.id_control;
                model.actividad_control = k_control.actividad_control;
                model.relacion_control = k_control.relacion_control; //codigo de control
                model.evidencia_control = k_control.evidencia_control;
                model.es_control_clave = k_control.es_control_clave;
                model.nb_aplicacion = k_control.nb_aplicacion;
                model.tiene_accion_correctora = k_control.tiene_accion_correctora;
                model.accion_correctora = k_control.accion_correctora;

               

                model = obtenerCamposExtra(model, k_control);


                ViewBag.lu = k_control.id_responsable;
                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control", k_control.id_categoria_control);
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura", k_control.id_grado_cobertura);
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia", k_control.id_tipo_evidencia);
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control", k_control.id_tipologia_control);
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_ejecutor);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);
                ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(k_control.c_aseveracion.Select(a => a.id_aseveracion).ToArray());


                //Riesgo Residual
                if (k_control.k_riesgo_residual.Count == 0)//Si no tiene riesgo residual
                {
                    ViewBag.RiesgoResidual = "null";
                }
                else
                {
                    ViewBag.RiesgoResidual = k_control.k_riesgo_residual.First();
                }

                return View(model);
            }
            else
            {
                AgregarControlMGViewModel model = new AgregarControlMGViewModel();

                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

                model.id_control = k_control.id_control;
                model.actividad_control = k_control.actividad_control;
                model.relacion_control = k_control.relacion_control; //codigo de control
                model.evidencia_control = k_control.evidencia_control;
                model.tiene_accion_correctora = k_control.tiene_accion_correctora;
                model.accion_correctora = k_control.accion_correctora;

                model = obtenerCamposExtra(model, k_control);

                ViewBag.lu = k_control.id_responsable;
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);

                return View("EditRMG", model);
            }
        }

        // POST: Control/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditR(AgregarControlViewModel model, int lu, int[] id_aseveracion, int[] files)
        {
            k_riesgo k_riesgo;
            k_control k_control;
            bool aux = false;
            bool aux2 = false;

            int nAseveraciones = 0;
            if (id_aseveracion != null) nAseveraciones = id_aseveracion.Count();

            if (model.tiene_accion_correctora)
            {
                if (model.accion_correctora == "" || model.accion_correctora == null)
                {
                    ModelState.AddModelError("accion_correctora", "La acción correctora no puede quedar vacia");
                }
                else
                {
                    aux2 = true;
                }
            }

            if (model.id_responsable == null || model.id_responsable == 0)
            {
                ModelState.AddModelError("id_responsable", "El responsable del control es un campo obligatorio.");
            }
            else
            {
                aux = true;
            }


            if (model.tiene_accion_correctora && aux && aux2)
            {
                k_control = db.k_control.Find(model.id_control);

                recordChange(k_control);

                k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                k_control.accion_correctora = model.accion_correctora;
                k_control.id_responsable = model.id_responsable;
                db.SaveChanges();
                if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);

                return RedirectToAction("Editar", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
            }
            else
            {
                bool valid = ValidarCE(model);

                if (ModelState.IsValid && valid)
                {
                    k_control = db.k_control.Find(model.id_control);


                    //agregar los archivos
                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();

                        if (files != null)
                            foreach (var idar in files)
                            {
                                var file = db.c_archivo.Find(idar);
                                k_control.c_archivo.Add(file);
                            }

                        db.SaveChanges();


                    }

                    recordChange(k_control);


                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_ejecutor = model.id_ejecutor;
                    k_control.id_responsable = model.id_responsable;
                    k_control.es_control_clave = model.es_control_clave;
                    k_control.id_grado_cobertura = model.id_grado_cobertura;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;
                    k_control.nb_aplicacion = model.nb_aplicacion;
                    k_control.id_tipologia_control = model.id_tipologia_control;
                    k_control.id_categoria_control = model.id_categoria_control;
                    k_control.id_tipo_evidencia = model.id_tipo_evidencia;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = "";

                    k_control = llenarCamposExtra(model, k_control);


                    db.SaveChanges();
                    if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);

                    //Actualizar las aseveraciones
                    db = new SICIEntities();
                    k_control = db.k_control.Find(k_control.id_control);
                    k_control.c_aseveracion.Clear();
                    db.SaveChanges();

                    if (nAseveraciones > 0)
                    {
                        for (int i = 0; i < nAseveraciones; i++)
                        {
                            var aseveracion = db.c_aseveracion.Find(id_aseveracion[i]);

                            k_control.c_aseveracion.Add(aseveracion);
                        }

                        db.SaveChanges();
                    }

                    return RedirectToAction("Editar", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
                }
            }

            k_control = db.k_control.Find(model.id_control);

            k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.k_riesgo = k_riesgo;
            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;


            ViewBag.lu = lu;
            ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control", k_control.id_categoria_control);
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
            ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura", k_control.id_grado_cobertura);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
            ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia", k_control.id_tipo_evidencia);
            ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control", k_control.id_tipologia_control);
            ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_ejecutor);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);
            ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(id_aseveracion);


            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //datos del indicador relacionado
            if (db.c_indicador.Any(i => i.id_control == model.id_control))
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == model.id_control).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_control.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            return View(model);
        }

        // POST: Control/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditRMG(AgregarControlMGViewModel model, int lu)
        {
            k_riesgo k_riesgo;
            k_control k_control;
            bool aux = false;
            bool aux2 = false;

            if (model.tiene_accion_correctora)
            {
                if (model.accion_correctora == "" || model.accion_correctora == null)
                {
                    ModelState.AddModelError("accion_correctora", "La acción correctora no puede quedar vacia");
                }
                else
                {
                    aux2 = true;
                }
            }

            if (model.id_responsable == null || model.id_responsable == 0)
            {
                ModelState.AddModelError("id_responsable", "El responsable del control es un campo obligatorio.");
            }
            else
            {
                aux = true;
            }


            if (model.tiene_accion_correctora && aux && aux2)
            {
                k_control = db.k_control.Find(model.id_control);

                recordChange(k_control);

                k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                k_control.accion_correctora = model.accion_correctora;
                k_control.id_responsable = model.id_responsable;

                db.SaveChanges();
                if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);

                return RedirectToAction("Editar", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
            }
            else
            {
                bool valid = ValidarCE(model);

                if (ModelState.IsValid)
                {
                    k_control = db.k_control.Find(model.id_control);

                    recordChange(k_control);

                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_responsable = model.id_responsable;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = "";

                    k_control = llenarCamposExtra(model, k_control);

                    db.SaveChanges();
                    if (lu != model.id_responsable) Utilidades.Utilidades.TaskAsigned(k_control, lu);

                    return RedirectToAction("Editar", "Riesgo", new { id = k_control.k_riesgo.First().id_riesgo });
                }
            }
            AgregarControlMGViewModel model2 = new AgregarControlMGViewModel();

            k_control = db.k_control.Find(model.id_control);

            k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model2.k_riesgo = k_riesgo;
            model2.c_sub_proceso = c_sub_proceso;
            model2.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;


            model2.id_control = k_control.id_control;
            model2.actividad_control = k_control.actividad_control;
            model2.relacion_control = k_control.relacion_control; //codigo de control
            model2.evidencia_control = k_control.evidencia_control;
            model2.tiene_accion_correctora = k_control.tiene_accion_correctora;
            model2.accion_correctora = k_control.accion_correctora;

            ViewBag.lu = lu;

            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);

            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //datos del indicador relacionado
            if (db.c_indicador.Where(i => i.id_control == k_control.id_control).Count() > 0)
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == k_control.id_control).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + k_control.c_sub_proceso.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            return View("EditRMG", model2);
        }

        // GET: Control/EditRR/5
        public ActionResult EditRR(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }
            k_riesgo_residual RiesgoResidual = k_control.k_riesgo_residual.First();


            //campos cobertura control
            ViewBag.cccInfo = db.a_campo_cobertura_control.ToList();

            //Listas para el cuestionario
            ViewBag.dc_ejecucion_sistematica = listaCampoCoberturaControl(1, 1, RiesgoResidual.dc_ejecucion_sistematica);
            ViewBag.dc_pistas_auditoria = listaCampoCoberturaControl(1, 2, RiesgoResidual.dc_pistas_auditoria);
            ViewBag.dc_cumplimiento_politicas_entidad = listaCampoCoberturaControl(1, 3, RiesgoResidual.dc_cumplimiento_politicas_entidad);
            ViewBag.dc_umbrales_adecuados_indicadores = listaCampoCoberturaControl(1, 4, RiesgoResidual.dc_umbrales_adecuados_indicadores);
            ViewBag.dc_metodologia_reporte = listaCampoCoberturaControl(1, 5, RiesgoResidual.dc_metodologia_reporte);
            ViewBag.dc_evaluacion_diseño = listaCampoCoberturaControl(1, 6, RiesgoResidual.dc_evaluacion_diseño);
            ViewBag.dc_responsable_diseño = listaCampoCoberturaControl(1, 7, RiesgoResidual.dc_responsable_diseño);
            ViewBag.dc_entrada_datos = listaCampoCoberturaControl(1, 8, RiesgoResidual.dc_entrada_datos);
            ViewBag.dc_resultado_sin_manipulacion = listaCampoCoberturaControl(1, 9, RiesgoResidual.dc_resultado_sin_manipulacion);

            ViewBag.ec_planes_remediacion = listaCampoCoberturaControl(2, 1, RiesgoResidual.ec_planes_remediacion);
            ViewBag.ec_eventos_relacionados = listaCampoCoberturaControl(2, 2, RiesgoResidual.ec_eventos_relacionados);
            ViewBag.ec_evaluacion_efectividad = listaCampoCoberturaControl(2, 3, RiesgoResidual.ec_evaluacion_efectividad);
            ViewBag.ec_efectividad_indicadores = listaCampoCoberturaControl(2, 4, RiesgoResidual.ec_efectividad_indicadores);
            ViewBag.ec_tiempo_madurez_control = listaCampoCoberturaControl(2, 5, RiesgoResidual.ec_tiempo_madurez_control);

            var riesgo = k_control.k_riesgo.First();
            ViewBag.Riesgo_Inherente = String.Format("${0:N2}", ((double)riesgo.c_magnitud_impacto.magnitud_impacto * (double)riesgo.c_probabilidad_ocurrencia.pr_probabilidad_ocurrencia));

            ViewBag.Edit = "true";  // variable que nos servira para mostrar el boton de guardar dependiendo de si se esta editando o creando
            ViewBag.readOnly = false;


            return PartialView("CreateRR", RiesgoResidual);
        }

        // POST: Control/EditRR/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditRR(k_riesgo_residual model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DetailsRR", new { id = model.id_control });
            }
            //Listas para el cuestionario
            ViewBag.dc_ejecucion_sistematica = listaCampoCoberturaControl(1, 1, model.dc_ejecucion_sistematica);
            ViewBag.dc_pistas_auditoria = listaCampoCoberturaControl(1, 2, model.dc_pistas_auditoria);
            ViewBag.dc_cumplimiento_politicas_entidad = listaCampoCoberturaControl(1, 3, model.dc_cumplimiento_politicas_entidad);
            ViewBag.dc_umbrales_adecuados_indicadores = listaCampoCoberturaControl(1, 4, model.dc_umbrales_adecuados_indicadores);
            ViewBag.dc_metodologia_reporte = listaCampoCoberturaControl(1, 5, model.dc_metodologia_reporte);
            ViewBag.dc_evaluacion_diseño = listaCampoCoberturaControl(1, 6, model.dc_evaluacion_diseño);
            ViewBag.dc_responsable_diseño = listaCampoCoberturaControl(1, 7, model.dc_responsable_diseño);
            ViewBag.dc_entrada_datos = listaCampoCoberturaControl(1, 8, model.dc_entrada_datos);
            ViewBag.dc_resultado_sin_manipulacion = listaCampoCoberturaControl(1, 9, model.dc_resultado_sin_manipulacion);

            ViewBag.ec_planes_remediacion = listaCampoCoberturaControl(2, 1, model.ec_planes_remediacion);
            ViewBag.ec_eventos_relacionados = listaCampoCoberturaControl(2, 2, model.ec_eventos_relacionados);
            ViewBag.ec_evaluacion_efectividad = listaCampoCoberturaControl(2, 3, model.ec_evaluacion_efectividad);
            ViewBag.ec_efectividad_indicadores = listaCampoCoberturaControl(2, 4, model.ec_efectividad_indicadores);
            ViewBag.ec_tiempo_madurez_control = listaCampoCoberturaControl(2, 5, model.ec_tiempo_madurez_control);

            var control = db.k_control.Find(model.id_control);
            var riesgo = control.k_riesgo.First();
            ViewBag.Riesgo_Inherente = String.Format("${0:N2}", ((double)riesgo.c_magnitud_impacto.magnitud_impacto * (double)riesgo.c_probabilidad_ocurrencia.pr_probabilidad_ocurrencia));

            ViewBag.Edit = "true";  // variable que nos servira para mostrar el boton de guardar dependiendo de si se esta editando o creando
            ViewBag.readOnly = false;

            return PartialView("CreateRR", model);
        }

        #endregion

        #region Delete section
        // GET: Control/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }

            Utilidades.DeleteActions.checkRedirect(redirect); //subrutina para saber si hay que volver a alguna pantalla

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Incluiremos K_certificacion_control y c_indicador
            var r_indicador = db.c_indicador.Where(b => b.id_control == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_indicador.Count > 0)
            {
                foreach (var indicador in r_indicador)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Indicadores";
                    rr.cl_registro = indicador.cl_indicador;
                    rr.nb_registro = indicador.nb_indicador;
                    rr.accion = "Delete";
                    rr.controlador = "Indicador";
                    rr.id_registro = indicador.id_indicador.ToString();

                    RR.Add(rr);
                }
            }

            var r_certificacion = db.k_certificacion_control.Where(b => b.id_control == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_certificacion.Count > 0)
            {
                foreach (var certificacion in r_certificacion)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Certificación de controles";
                    rr.cl_registro = certificacion.cl_certificacion_control;
                    rr.nb_registro = certificacion.ds_procedimiento_certificacion;
                    rr.accion = "Delete";
                    rr.controlador = "Certificacion";
                    rr.id_registro = certificacion.id_certificacion_control.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(k_control);
        }

        // POST: Control/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            k_control k_control = db.k_control.Find(id);
            k_riesgo k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

            if (k_riesgo.k_control.ToList().Count() > 1)
            {
                k_riesgo.k_control.Remove(k_control);

                Utilidades.DeleteActions.DeleteControlObjects(k_control, db);


                db.k_control.Remove(k_control);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("CantErase", "Error", null);
                }

                //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
                int ns;
                try
                {
                    ns = (int)HttpContext.Session["JumpCounter"];
                }
                catch
                {
                    ns = 0;
                }
                //Si ns es 0 redireccionamos al index de este controlador
                if (ns == 0)
                {
                    return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });

                }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                else
                {
                    List<string> directions = new List<string>();
                    try
                    {
                        directions = (List<string>)HttpContext.Session["Directions"];
                    }
                    catch
                    {
                        directions = null;
                    }

                    if (directions == null)
                    {
                        return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });
                    }
                    else
                    {
                        string direction = directions.Last();
                        DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                        //disminuimos ns y eliminamos el ultimo elemento de directions
                        ns--;
                        directions.RemoveAt(ns);

                        //Guardamos ambas variables de sesion para seguir trabajando
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;

                        return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                    }
                }
            }
            ViewBag.Error = Strings.getMSG("No se puede eliminar, todo riesgo debe tener al menos un Control.");
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();
            ViewBag.RR = RR;

            return View("Delete", k_control);

        }


        // GET: Control/DeleteR/5
        public ActionResult DeleteR(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }

            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Session["JumpCounter"];
                    }
                    catch
                    {
                        ns = 0;
                    }
                    //Si ns es 0, creamos un nuevo array, agregamos la direccion actual y lo asignamos a la variable "Directions" y establecemos "JumpCounter" = 1
                    if (ns == 0)
                    {
                        List<string> directions = new List<string>();
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = 1;
                        HttpContext.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Session["JumpCounter"] = null;
                HttpContext.Session["Directions"] = null;
            }

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            //Incluiremos K_certificacion_control y c_indicador
            var r_indicador = db.c_indicador.Where(b => b.id_control == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_indicador.Count > 0)
            {
                foreach (var indicador in r_indicador)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Indicadores";
                    rr.cl_registro = indicador.cl_indicador;
                    rr.nb_registro = indicador.nb_indicador;
                    rr.accion = "Delete";
                    rr.controlador = "Indicador";
                    rr.id_registro = indicador.id_indicador.ToString();

                    RR.Add(rr);
                }
            }

            var r_certificacion = db.k_certificacion_control.Where(b => b.id_control == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_certificacion.Count > 0)
            {
                foreach (var certificacion in r_certificacion)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Certificación de controles";
                    rr.cl_registro = certificacion.cl_certificacion_control;
                    rr.nb_registro = certificacion.ds_procedimiento_certificacion;
                    rr.accion = "Delete";
                    rr.controlador = "Certificacion";
                    rr.id_registro = certificacion.id_certificacion_control.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;

            return View(k_control);
        }

        // POST: Control/Delete/5
        [HttpPost, ActionName("DeleteR")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmedR(int id)
        {
            k_control k_control = db.k_control.Find(id);
            k_riesgo k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);


            if (k_riesgo.k_control.ToList().Count() > 1)
            {
                k_riesgo.k_control.Remove(k_control);

                Utilidades.DeleteActions.DeleteControlObjects(k_control, db);

                db.k_control.Remove(k_control);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("CantErase", "Error", null);
                }

                //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
                int ns;
                try
                {
                    ns = (int)HttpContext.Session["JumpCounter"];
                }
                catch
                {
                    ns = 0;
                }
                //Si ns es 0 redireccionamos al index de este controlador
                if (ns == 0)
                {
                    return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });

                }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                else
                {
                    List<string> directions = new List<string>();
                    try
                    {
                        directions = (List<string>)HttpContext.Session["Directions"];
                    }
                    catch
                    {
                        directions = null;
                    }

                    if (directions == null)
                    {
                        return RedirectToAction("Controles", "Riesgo", new { id = k_riesgo.id_riesgo });
                    }
                    else
                    {
                        string direction = directions.Last();
                        DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                        //disminuimos ns y eliminamos el ultimo elemento de directions
                        ns--;
                        directions.RemoveAt(ns);

                        //Guardamos ambas variables de sesion para seguir trabajando
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;

                        return RedirectToAction("Editar", "Riesgo", new { id = k_riesgo.id_riesgo, redirect = "bfo" });
                    }
                }

            }
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();
            ViewBag.RR = RR;
            ViewBag.Error = Strings.getMSG("No se puede eliminar, todo riesgo debe tener al menos un Control.");
            return View("DeleteR", k_control);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRR(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control control = db.k_control.Find(id);
            if (control == null)
            {
                return HttpNotFound();
            }
            var RR = control.k_riesgo_residual.First();
            db.k_riesgo_residual.Remove(RR);
            db.SaveChanges();

            return RedirectToAction("CreateRR", new { id = control.id_control });
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


        #region Extra functions
        private bool ValidarIncidencia(ModelStateDictionary modelState, AgregarControlViewModel m1 = null, AgregarControlMGViewModel m2 = null)
        {
            bool valid = true;
            if (m1 != null)
            {
                //La incidencia se valida solo en caso de tener una accion Correctora
                if (m1.tiene_accion_correctora)
                {
                    if (m1.accion_correctora == "" || m1.accion_correctora == null)
                    {
                        modelState.AddModelError("accion_correctora", Strings.getMSG("RiesgoCreate064"));
                        valid = false;
                    }
                    if (m1.ds_incidencia == null)
                    {
                        modelState.AddModelError("ds_incidencia", Strings.getMSG("IndicadorDiarioCreate006"));
                        valid = false;
                    }
                    if (m1.id_responsable_i <= 0 || m1.id_responsable_i == null)
                    {
                        modelState.AddModelError("id_responsable_i", Strings.getMSG("RiesgoCreate065"));
                        valid = false;
                    }
                    if (m1.id_clasificacion_incidencia <= 0 || m1.id_clasificacion_incidencia == null)
                    {
                        modelState.AddModelError("id_clasificacion_incidencia", Strings.getMSG("RiesgoCreate066"));
                        valid = false;
                    }
                    if (!m1.requiere_plan)
                    {
                        if (m1.js_incidencia == null)
                        {
                            modelState.AddModelError("js_incidencia", Strings.getMSG("IndicadorDiarioCreate006"));
                            valid = false;
                        }
                    }
                }
            }

            if (m2 != null)
            {
                //La incidencia se valida solo en caso de tener una accion Correctora
                if (m2.tiene_accion_correctora)
                {
                    if (m2.accion_correctora == "" || m2.accion_correctora == null)
                    {
                        modelState.AddModelError("accion_correctora", Strings.getMSG("RiesgoCreate064"));
                        valid = false;
                    }
                    if (m2.ds_incidencia == null)
                    {
                        modelState.AddModelError("ds_incidencia", Strings.getMSG("IndicadorDiarioCreate006"));
                        valid = false;
                    }
                    if (m2.id_responsable_i <= 0 || m2.id_responsable_i == null)
                    {
                        modelState.AddModelError("id_responsable_i", Strings.getMSG("RiesgoCreate065"));
                        valid = false;
                    }
                    if (m2.id_clasificacion_incidencia <= 0 || m2.id_clasificacion_incidencia == null)
                    {
                        modelState.AddModelError("id_clasificacion_incidencia", Strings.getMSG("RiesgoCreate066"));
                        valid = false;
                    }
                    if (!m2.requiere_plan)
                    {
                        if (m2.js_incidencia == null)
                        {
                            modelState.AddModelError("js_incidencia", Strings.getMSG("IndicadorDiarioCreate006"));
                            valid = false;
                        }
                    }
                }
            }

            return valid;
        }

        private bool ValidarCE(AgregarControlViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            campo[0] = model.campo01; campo[1] = model.campo02;
            campo[2] = model.campo03; campo[3] = model.campo04;
            campo[4] = model.campo05; campo[5] = model.campo06;
            campo[6] = model.campo07; campo[7] = model.campo08;
            campo[8] = model.campo09; campo[9] = model.campo10;
            campo[10] = model.campo11; campo[11] = model.campo12;
            campo[12] = model.campo13; campo[13] = model.campo14;
            campo[14] = model.campo15; campo[15] = model.campo16;
            campo[16] = model.campo17; campo[17] = model.campo18;
            campo[18] = model.campo19; campo[19] = model.campo20;

            var info = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            bool response = true;

            for (int i = 0; i < 20; i++)
            {
                var inf = info[i];

                bool validate = inf.es_visible;

                //Si el campo es visible, comenzamos las validaciones, de otra forma lo ignoramos
                if (validate)
                {
                    if (inf.es_requerido && (campo[i] == null || campo[i] == ""))
                    {
                        errores[i] = "El campo \"" + inf.nb_campo + "\" es un campo requerido.";
                        response = false;
                    }
                    else
                    {
                        errores[i] = "";
                    }


                    if (campo[i] != null && campo[i] != "")
                    {
                        if (inf.longitud_campo < campo[i].Length)
                        {
                            errores[i] = "La longitud máxima de el campo " + inf.nb_campo + " es de: " + inf.longitud_campo + " caracteres.";
                            response = false;
                        }
                        else
                        {
                            errores[i] = "";
                        }
                    }
                }
            }

            ControllerContext.Controller.ViewBag.MCError = errores;
            return response;
        }

        private bool ValidarCE(AgregarControlMGViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            campo[0] = model.campo01; campo[1] = model.campo02;
            campo[2] = model.campo03; campo[3] = model.campo04;
            campo[4] = model.campo05; campo[5] = model.campo06;
            campo[6] = model.campo07; campo[7] = model.campo08;
            campo[8] = model.campo09; campo[9] = model.campo10;
            campo[10] = model.campo11; campo[11] = model.campo12;
            campo[12] = model.campo13; campo[13] = model.campo14;
            campo[14] = model.campo15; campo[15] = model.campo16;
            campo[16] = model.campo17; campo[17] = model.campo18;
            campo[18] = model.campo19; campo[19] = model.campo20;

            var info = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            bool response = true;
            for (int i = 0; i < 20; i++)
            {
                var inf = info[i];
                bool validate = inf.aparece_en_mg && inf.es_visible;

                //Si el campo es visible, comenzamos las validaciones, de otra forma lo ignoramos
                if (validate)
                {
                    if (inf.es_requerido && (campo[i] == null || campo[i] == ""))
                    {
                        errores[i] = "El campo \"" + inf.nb_campo + "\" es un campo requerido.";
                        response = false;
                    }
                    else
                    {
                        errores[i] = "";
                    }


                    if (campo[i] != null && campo[i] != "")
                    {
                        if (inf.longitud_campo < campo[i].Length)
                        {
                            errores[i] = "La longitud máxima de el campo " + inf.nb_campo + " es de: " + inf.longitud_campo + " caracteres.";
                            response = false;
                        }
                        else
                        {
                            errores[i] = "";
                        }
                    }
                }
            }

            ControllerContext.Controller.ViewBag.MCError = errores;
            return response;
        }

        k_control llenarCamposExtra(AgregarControlViewModel model, k_control control)
        {
            control.campo01 = model.campo01; control.campo02 = model.campo02;
            control.campo03 = model.campo03; control.campo04 = model.campo04;
            control.campo05 = model.campo05; control.campo06 = model.campo06;
            control.campo07 = model.campo07; control.campo08 = model.campo08;
            control.campo09 = model.campo09; control.campo10 = model.campo10;
            control.campo11 = model.campo11; control.campo12 = model.campo12;
            control.campo13 = model.campo13; control.campo14 = model.campo14;
            control.campo15 = model.campo15; control.campo16 = model.campo16;
            control.campo17 = model.campo17; control.campo18 = model.campo18;
            control.campo19 = model.campo19; control.campo20 = model.campo20;

            return control;
        }

        k_control llenarCamposExtra(AgregarControlMGViewModel model, k_control control)
        {
            control.campo01 = model.campo01; control.campo02 = model.campo02;
            control.campo03 = model.campo03; control.campo04 = model.campo04;
            control.campo05 = model.campo05; control.campo06 = model.campo06;
            control.campo07 = model.campo07; control.campo08 = model.campo08;
            control.campo09 = model.campo09; control.campo10 = model.campo10;
            control.campo11 = model.campo11; control.campo12 = model.campo12;
            control.campo13 = model.campo13; control.campo14 = model.campo14;
            control.campo15 = model.campo15; control.campo16 = model.campo16;
            control.campo17 = model.campo17; control.campo18 = model.campo18;
            control.campo19 = model.campo19; control.campo20 = model.campo20;

            return control;
        }

        AgregarControlViewModel obtenerCamposExtra(AgregarControlViewModel control, k_control model)
        {
            control.campo01 = model.campo01; control.campo02 = model.campo02;
            control.campo03 = model.campo03; control.campo04 = model.campo04;
            control.campo05 = model.campo05; control.campo06 = model.campo06;
            control.campo07 = model.campo07; control.campo08 = model.campo08;
            control.campo09 = model.campo09; control.campo10 = model.campo10;
            control.campo11 = model.campo11; control.campo12 = model.campo12;
            control.campo13 = model.campo13; control.campo14 = model.campo14;
            control.campo15 = model.campo15; control.campo16 = model.campo16;
            control.campo17 = model.campo17; control.campo18 = model.campo18;
            control.campo19 = model.campo19; control.campo20 = model.campo20;

            return control;
        }

        AgregarControlMGViewModel obtenerCamposExtra(AgregarControlMGViewModel control, k_control model)
        {
            control.campo01 = model.campo01; control.campo02 = model.campo02;
            control.campo03 = model.campo03; control.campo04 = model.campo04;
            control.campo05 = model.campo05; control.campo06 = model.campo06;
            control.campo07 = model.campo07; control.campo08 = model.campo08;
            control.campo09 = model.campo09; control.campo10 = model.campo10;
            control.campo11 = model.campo11; control.campo12 = model.campo12;
            control.campo13 = model.campo13; control.campo14 = model.campo14;
            control.campo15 = model.campo15; control.campo16 = model.campo16;
            control.campo17 = model.campo17; control.campo18 = model.campo18;
            control.campo19 = model.campo19; control.campo20 = model.campo20;

            return control;
        }

        List<SelectListItem> listaCampoCoberturaControl(int catalogo, int cl_campo, int idCampo = 0)
        {
            List<SelectListItem> Lista = new List<SelectListItem>();

            var campos = db.a_campo_cobertura_control.Where(a => a.cl_catalogo == catalogo && a.cl_campo == cl_campo);


            if (idCampo != 0)
            {
                foreach (var campo in campos)
                {
                    if (idCampo == campo.id_campo_cobertura_control)
                    {
                        SelectListItem item = new SelectListItem()
                        {
                            Text = campo.nb_campo + " - " + String.Format("{0:0.00}", campo.valor) + "%",
                            Value = campo.id_campo_cobertura_control.ToString()
                        };
                        Lista.Add(item);
                        break;
                    }
                }
                foreach (var campo in campos)
                {
                    if (idCampo != campo.id_campo_cobertura_control)
                    {
                        SelectListItem item = new SelectListItem()
                        {
                            Text = campo.nb_campo + " - " + String.Format("{0:0.00}", campo.valor) + "%",
                            Value = campo.id_campo_cobertura_control.ToString()
                        };
                        Lista.Add(item);
                    }
                }
            }
            else
            {
                foreach (var campo in campos)
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = campo.nb_campo + " - " + String.Format("{0:0.00}", campo.valor) + "%",
                        Value = campo.id_campo_cobertura_control.ToString()
                    };
                    Lista.Add(item);
                }
            }



            return Lista;
        }

        public ActionResult DetailsRR(int? id, bool readOnly = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }
            var model = k_control.k_riesgo_residual.First();
            var riesgo = k_control.k_riesgo.First();

            double Riesgo_Inherente = ((double)riesgo.c_magnitud_impacto.magnitud_impacto * (double)riesgo.c_probabilidad_ocurrencia.pr_probabilidad_ocurrencia);
            Riesgo_Inherente = Riesgo_Inherente / 100;
            ViewBag.Riesgo_Inherente = String.Format("${0:N2}", Riesgo_Inherente);
            double CC = (double)model.a_campo_cobertura_control.valor +
                (double)model.a_campo_cobertura_control1.valor +
                (double)model.a_campo_cobertura_control2.valor +
                (double)model.a_campo_cobertura_control3.valor +
                (double)model.a_campo_cobertura_control4.valor +
                (double)model.a_campo_cobertura_control5.valor +
                (double)model.a_campo_cobertura_control6.valor +
                (double)model.a_campo_cobertura_control7.valor +
                (double)model.a_campo_cobertura_control8.valor +
                (double)model.a_campo_cobertura_control9.valor +
                (double)model.a_campo_cobertura_control10.valor +
                (double)model.a_campo_cobertura_control11.valor +
                (double)model.a_campo_cobertura_control12.valor +
                (double)model.a_campo_cobertura_control13.valor;

            ViewBag.Cobertura_Control = String.Format("{0:N2}%", CC);

            ViewBag.Riesgo_Residual = String.Format("${0:N2}", Riesgo_Inherente * ((100 - CC) / 100));

            ViewBag.readOnly = readOnly;

            return PartialView(model);
        }


        bool recordChange(k_control control)
        {
            var registro = new r_control();
            registro.campo01 = control.campo01; registro.campo02 = control.campo02;
            registro.campo03 = control.campo03; registro.campo04 = control.campo04;
            registro.campo05 = control.campo05; registro.campo06 = control.campo06;
            registro.campo07 = control.campo07; registro.campo08 = control.campo08;
            registro.campo09 = control.campo09; registro.campo10 = control.campo10;
            registro.campo11 = control.campo11; registro.campo12 = control.campo12;
            registro.campo13 = control.campo13; registro.campo14 = control.campo14;
            registro.campo15 = control.campo15; registro.campo16 = control.campo16;
            registro.campo17 = control.campo17; registro.campo18 = control.campo18;
            registro.campo19 = control.campo19; registro.campo20 = control.campo20;

            registro.actividad_control = control.actividad_control;
            registro.accion_correctora = control.accion_correctora;
            registro.ds_notificacion_incidencia = control.ds_notificacion_incidencia;
            registro.es_control_clave = control.es_control_clave;
            registro.evidencia_control = control.evidencia_control;
            registro.fe_modificacion = DateTime.Now;
            registro.id_categoria_control = control.id_categoria_control;
            registro.id_control = control.id_control;
            registro.id_ejecutor = control.id_ejecutor;
            registro.id_frecuencia_control = control.id_frecuencia_control;
            registro.id_grado_cobertura = control.id_grado_cobertura;
            registro.id_naturaleza_control = control.id_naturaleza_control;
            registro.id_responsable = control.id_responsable;
            registro.id_tipologia_control = control.id_tipologia_control;
            registro.id_tipo_evidencia = control.id_tipo_evidencia;
            registro.id_usuario = ((IdentityPersonalizado)HttpContext.User.Identity).Id_usuario;
            registro.nb_aplicacion = control.nb_aplicacion;
            registro.relacion_control = control.relacion_control;
            registro.tiene_accion_correctora = control.tiene_accion_correctora;

            db.r_control.Add(registro);

            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
