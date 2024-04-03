using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "MRyC", ModuleCode = "MSICI003")]
    [CustomErrorHandler]
    public class RiesgoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        //private IEstructuraRepository _repository;
        private ISelectListRepository _repository;

        //public RiesgoController() : this(new EstructuraRepository())
        public RiesgoController() : this(new SelectListRepository())
        {
        }

        //public RiesgoController(IEstructuraRepository repository)
        public RiesgoController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        #region Index


        // Página inicial.  Muestra los subprocesos asociados al usuario firmado en el sistema
        public ActionResult SubProcesos()
        {
            //try
            //{
            //    IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
            //    int id_responsable = identity.Id_usuario;
            //    bool super_usuario = identity.Es_super_usuario;

            //    string sql = "exec obtiene_sub_procesos " + (super_usuario ? "0" : id_responsable.ToString());
            //    var subProcesos = db.Database.SqlQuery<ListaSubProcesosViewModel>(sql).ToList();

            //    return View(subProcesos);
            //}
            //catch
            //{
            //    return View("Error");
            //}

            IdentityPersonalizado identity = (IdentityPersonalizado)ControllerContext.HttpContext.User.Identity;
            int id_responsable = identity.Id_usuario;
            bool super_usuario = identity.Es_super_usuario;
            var usuario = db.c_usuario.Find(id_responsable);

            var subProcesos = Utilidades.Utilidades.RTCObject(usuario, db, "c_sub_proceso").Cast<c_sub_proceso>().
                OrderBy(x => x.c_proceso.c_macro_proceso.c_entidad.cl_entidad).
                OrderBy(x => x.c_proceso.c_macro_proceso.cl_macro_proceso).
                OrderBy(x => x.c_proceso.cl_proceso).
                OrderBy(x => x.cl_sub_proceso).ToList();

            List<ListaSubProcesosViewModel> model = new List<ListaSubProcesosViewModel>();

            foreach (var sp in subProcesos)
            {
                var p = sp.c_proceso;
                var mp = p.c_macro_proceso;
                var en = mp.c_entidad;

                model.Add(new ListaSubProcesosViewModel
                {
                    cn_entidad = en.cl_entidad + " - " + en.nb_entidad,
                    responsable_entidad = en.c_usuario.nb_usuario,
                    cn_macro_proceso = mp.cl_macro_proceso + " - " + mp.nb_macro_proceso,
                    responsable_macro_proceso = mp.c_usuario.nb_usuario,
                    cn_proceso = p.cl_proceso + " - " + p.nb_proceso,
                    responsable_proceso = p.c_usuario.nb_usuario,
                    cn_sub_proceso = sp.cl_sub_proceso + " - " + sp.nb_sub_proceso,
                    responsable_sub_proceso = sp.c_usuario.nb_usuario,
                    id_sub_proceso = sp.id_sub_proceso,
                    no_riesgos = sp.k_riesgo.Count
                }) ;
            }

            return View(model);
        }

        // Lista Riesgos.  Muestra la lista de riesgos asociados al subproceso enviado como parámetro.
        public ActionResult Riesgos(int id)
        {
            string sql;
            sql =
                "select SP.cl_sub_proceso, SP.nb_sub_proceso" +
                "  from c_sub_proceso SP" +
                " where SP.id_sub_proceso = " + id.ToString();
            c_sub_proceso subProceso = db.c_sub_proceso.Find(id);

            string prefijo = subProceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);

            ViewBag.id_sub_proceso = subProceso.id_sub_proceso;
            ViewBag.cl_sub_proceso = subProceso.cl_sub_proceso;
            ViewBag.nb_sub_proceso = subProceso.nb_sub_proceso;
            ViewBag.riesgos_derogados = db.k_riesgo_derogado.Where(rd => rd.id_sub_proceso == id).ToList();



            if (prefijo == "MP")
            {
                sql =
                    "select R.id_riesgo, R.cl_riesgo, R.nb_riesgo, R.evento, TR.nb_tipo_riesgo" +
                    "  from k_riesgo R" +
                    " inner join c_tipo_riesgo TR on R.id_tipo_riesgo = TR.id_tipo_riesgo" +
                    " where R.id_sub_proceso = " + id.ToString();
                var riesgos = db.Database.SqlQuery<ListaRiesgosViewModel>(sql).ToList();

                return View(riesgos);
            }
            else
            {
                List<k_riesgo> lista_riesgos = db.k_riesgo.Where(r => r.id_sub_proceso == id).ToList();
                List<ListaRiesgosViewModel> lista = new List<ListaRiesgosViewModel>();

                foreach (k_riesgo riesgo in lista_riesgos)
                {
                    ListaRiesgosViewModel aux = new ListaRiesgosViewModel();
                    aux.cl_riesgo = riesgo.cl_riesgo;
                    aux.id_riesgo = riesgo.id_riesgo;
                    aux.evento = riesgo.evento;
                    aux.nb_riesgo = riesgo.nb_riesgo;
                    aux.nb_tipo_riesgo = "N/A";
                    lista.Add(aux);
                }
                return View(lista);
            }

        }

        // Lista Controles.  Muestra la lista de controles asociados al riesgo enviado como parámetro.
        public ActionResult Controles(int id)
        {
            string sql;

            k_riesgo riesgo = db.k_riesgo.Find(id);
            ViewBag.cl_riesgo = riesgo.cl_riesgo;
            ViewBag.nb_riesgo = riesgo.nb_riesgo;
            ViewBag.id_riesgo = riesgo.id_riesgo;
            ViewBag.id_sub_proceso = riesgo.c_sub_proceso.id_sub_proceso;
            

            sql =
                "select C.relacion_control as codigo_control, C.id_control, C.actividad_control, C.tiene_accion_correctora, C.accion_correctora, E.nb_usuario nb_ejecutor, R.nb_usuario nb_responsable" +
                "  from k_control C" +
                "  left outer join c_usuario E on C.id_ejecutor = E.id_usuario" +
                "  left outer join c_usuario R on C.id_responsable = R.id_usuario" +
                " where C.id_control in (select id_control from k_control_riesgo where id_riesgo = " + id.ToString() + ")";
            var controles = db.Database.SqlQuery<ListaControlesViewModel>(sql).ToList();




            return View(controles);
        }

        #endregion

        #region Agregar

        // GET: Riesgo/Agregar
        // Agrega un riesgo asociado a un subproceso que se envia como parámetro
        public ActionResult Agregar(int id)
        {
            c_sub_proceso c_sub_proceso = new c_sub_proceso();
            c_sub_proceso = db.c_sub_proceso.Find(id);

            c_macro_proceso c_macro_proceso = c_sub_proceso.c_proceso.c_macro_proceso;
            string prefijo = c_macro_proceso.cl_macro_proceso.Substring(0, 2);

            //Campos extra Riesgos y Controles
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.MCError = new string[20];
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.MRError = new string[20];
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);


            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + id);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;


            //asignaremos el código del riesgo
            //string codigoR = Utilidades.Utilidades.RCodeGen(c_sub_proceso);
            //string codigoC = Utilidades.Utilidades.CCodeGen(c_sub_proceso);
            //Asignamos el código de control


            //Mandamos las listas necesarias para la creacion de la incidencia en caso de ser necesaria
            ViewBag.id_responsable_i = Utilidades.DropDown.Usuario();
            ViewBag.id_clasificacion_incidencia = Utilidades.DropDown.ClasificacionIncidencia();



            if (prefijo == "MP")
            {
                AgregarRiesgoViewModel model = new AgregarRiesgoViewModel();
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = id; // c_sub_proceso.id_sub_proceso;

                model.CategoriasRiesgo.Add(new SelectListItem { Text = Strings.getMSG("RiesgoCreate019"), Value = "0" });
                var categorias = _repository.ObtieneCategoriasRiesgo().OrderBy(x => x.cl_categoria_riesgo);
                foreach (var categoria in categorias)
                {
                    model.CategoriasRiesgo.Add(new SelectListItem()
                    {
                        Text = categoria.cl_categoria_riesgo + " - " + categoria.nb_categoria_riesgo,
                        Value = categoria.id_categoria_riesgo.ToString()
                    });
                }

                var clases = _repository.ObtieneClasesTipologiaRiesgo().OrderBy(x => x.cl_clase_tipologia_riesgo);
                foreach (var clase in clases)
                {
                    model.ClasesTipologiaRiesgo.Add(new SelectListItem()
                    {
                        Text = clase.cl_clase_tipologia_riesgo + " - " + clase.nb_clase_tipologia_riesgo,
                        Value = clase.id_clase_tipologia_riesgo.ToString()
                    });
                }

                ViewBag.id_magnitud_impacto = new SelectList(db.c_magnitud_impacto.OrderBy(c => c.cl_magnitud_impacto), "id_magnitud_impacto", "nb_magnitud_impacto");
                ViewBag.id_probabilidad_ocurrencia = new SelectList(db.c_probabilidad_ocurrencia.OrderBy(c => c.cl_probabilidad_ocurrencia), "id_probabilidad_ocurrencia", "nb_probabilidad_ocurrencia");
                ViewBag.id_tipo_impacto = new SelectList(db.c_tipo_impacto, "id_tipo_impacto", "nb_tipo_impacto");
                //ViewBag.id_tipo_riesgo = new SelectList(db.c_tipo_riesgo, "id_tipo_riesgo", "nb_tipo_riesgo");
                //ViewBag.id_tipologia_riesgo = new SelectList(db.c_tipologia_riesgo, "id_tipologia_riesgo", "nb_tipologia_riesgo");

                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_ejecutor);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);
                ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS();

                //region catalogos de riesgo operativo
                ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacionalR();
                ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacionalR();
                ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacionalR();
                ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacionalR();
                ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional();
                ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional();


                //Enviar todos los datos de la tabla de Criticidad
                ViewBag.Criticidad = db.c_criticidad.ToList();

                //model.nb_riesgo = codigoR;
                ///model.relacion_control = codigoC;
                return View(model);
            }
            else
            {
                AgregarRiesgoMGViewModel model = new AgregarRiesgoMGViewModel();
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = id; // c_sub_proceso.id_sub_proceso;

                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);

                //model.nb_riesgo = codigoR;
                //model.relacion_control = codigoC;
                return View("AgregarMG", model);
            }
        }

        // POST: Riesgo/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Agregar(AgregarRiesgoViewModel model, int[] id_aseveracion, int[] files)
        {
            int nAseveraciones = 0;
            if (id_aseveracion != null) nAseveraciones = id_aseveracion.Count();

            bool valid = ValidarIncidencia(ModelState, model);

            model.cl_riesgo = "0";

            valid = ValidarCE(model) && valid;

            valid = ValidarRiesgoOperativo(ModelState, model) && valid;

            if (ModelState.IsValid && valid)
            {
                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo.id_riesgo = 0;
                k_riesgo.cl_riesgo = model.cl_riesgo;
                k_riesgo.id_sub_proceso = model.id_sub_proceso;
                k_riesgo.nb_riesgo = model.nb_riesgo;
                k_riesgo.evento = model.evento;
                k_riesgo.id_tipo_riesgo = model.id_tipo_riesgo;
                k_riesgo.id_tipologia_riesgo = model.id_tipologia_riesgo;
                k_riesgo.id_probabilidad_ocurrencia = model.id_probabilidad_ocurrencia;
                k_riesgo.id_tipo_impacto = model.id_tipo_impacto;
                k_riesgo.id_magnitud_impacto = model.id_magnitud_impacto;
                k_riesgo.criticidad = model.criticidad;
                k_riesgo.tiene_afectacion_contable = model.tiene_afectacion_contable;
                k_riesgo.supuesto_normativo = model.supuesto_normativo;
                k_riesgo.euc = model.euc;

               // k_riesgo.monto_impacto = model.monto_impacto; //NUEVO CAMPO EN LA BASE DE DATOS 

                k_riesgo.es_riesgo_operativo = model.es_riesgo_operativo;
                k_riesgo.id_frecuencia_riesgo_operacional = model.id_frecuencia_riesgo_operacional;
                k_riesgo.id_impacto_riesgo_operacional = model.id_impacto_riesgo_operacional;
                k_riesgo.id_linea_negocio_riesgo_operacional = model.id_linea_negocio_riesgo_operacional;
                k_riesgo.id_proceso_riesgo_operacional = model.id_proceso_riesgo_operacional;
                k_riesgo.id_sub_tipo_producto_riesgo_operacional = model.id_sub_tipo_producto_riesgo_operacional;
                k_riesgo.id_sub_tipo_riesgo_operacional = model.id_sub_tipo_riesgo_operacional;

                k_riesgo = llenarCamposExtra(model, k_riesgo);

                db.k_riesgo.Add(k_riesgo);
                db.SaveChanges();


                k_control k_control = new k_control();
                k_control.id_control = 0;
                k_control.id_sub_proceso = model.id_sub_proceso;



                //agregar los archivos
               if (files != null)
               {
                    foreach (int file in files)
                    {
                       c_archivo archivo = db.c_archivo.Find(file);

                       k_control.c_archivo.Add(archivo);
                    }

                   db.SaveChanges();
               }



                if (!model.tiene_accion_correctora)
                {
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

                    k_control = llenarCamposExtra(model, k_control);
                }
                else
                {
                    k_control.id_responsable = model.id_responsable;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = model.accion_correctora;
                }

                k_riesgo.k_control.Add(k_control);

                db.k_control.Add(k_control);
                db.SaveChanges();


                if (model.tiene_accion_correctora)
                {
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


                Utilidades.Utilidades.TaskAsigned(k_control);
                return RedirectToAction("Riesgos", "Riesgo", new { id = k_riesgo.id_sub_proceso });
            }

            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(model.id_sub_proceso);
            model.c_sub_proceso = c_sub_proceso;

            model.CategoriasRiesgo.Add(new SelectListItem { Text = Strings.getMSG("RiesgoCreate019"), Value = "0" });
            var categorias = _repository.ObtieneCategoriasRiesgo().OrderBy(x => x.cl_categoria_riesgo);
            foreach (var categoria in categorias)
            {
                model.CategoriasRiesgo.Add(new SelectListItem()
                {
                    Text = categoria.cl_categoria_riesgo + " - " + categoria.nb_categoria_riesgo,
                    Value = categoria.id_categoria_riesgo.ToString()
                });
            }

            var clases = _repository.ObtieneClasesTipologiaRiesgo().OrderBy(x => x.cl_clase_tipologia_riesgo);
            foreach (var clase in clases)
            {
                model.ClasesTipologiaRiesgo.Add(new SelectListItem()
                {
                    Text = clase.cl_clase_tipologia_riesgo + " - " + clase.nb_clase_tipologia_riesgo,
                    Value = clase.id_clase_tipologia_riesgo.ToString()
                });
            }

            if (model.id_categoria_riesgo != 0)
            {
                var TiposRiesgo = db.c_tipo_riesgo.Where(tr => tr.id_categoria_riesgo == model.id_categoria_riesgo).ToList();
                foreach (var TipoRiesgo in TiposRiesgo)
                {
                    model.TiposRiesgo.Add(new SelectListItem()
                    {
                        Text = TipoRiesgo.cl_tipo_riesgo + " - " + TipoRiesgo.nb_tipo_riesgo,
                        Value = TipoRiesgo.id_tipo_riesgo.ToString()
                    });
                }
            }

            if (model.id_clase_tipologia_riesgo != 0)
            {
                var SubClases = db.c_sub_clase_tipologia_riesgo.Where(sctr => sctr.id_clase_tipologia_riesgo == model.id_clase_tipologia_riesgo).ToList();
                foreach (var SC in SubClases)
                {
                    model.SubClasesTipologiaRiesgo.Add(new SelectListItem()
                    {
                        Text = SC.cl_sub_clase_tipologia_riesgo + " - " + SC.nb_sub_clase_tipologia_riesgo,
                        Value = SC.id_sub_clase_tipologia_riesgo.ToString()
                    });
                }
            }

            if (model.id_sub_clase_tipologia_riesgo != 0)
            {
                var TipologiasRiesgo = db.c_tipologia_riesgo.Where(tr => tr.id_sub_clase_tipologia_riesgo == model.id_sub_clase_tipologia_riesgo).ToList();
                foreach (var TipologiaRiesgo in TipologiasRiesgo)
                {
                    model.TipologiasRiesgo.Add(new SelectListItem()
                    {
                        Text = TipologiaRiesgo.cl_tipologia_riesgo + " - " + TipologiaRiesgo.nb_tipologia_riesgo,
                        Value = TipologiaRiesgo.id_tipologia_riesgo.ToString()
                    });
                }
            }



            ViewBag.id_magnitud_impacto = new SelectList(db.c_magnitud_impacto.OrderBy(c => c.cl_magnitud_impacto), "id_magnitud_impacto", "nb_magnitud_impacto");
            ViewBag.id_probabilidad_ocurrencia = new SelectList(db.c_probabilidad_ocurrencia.OrderBy(c => c.cl_probabilidad_ocurrencia), "id_probabilidad_ocurrencia", "nb_probabilidad_ocurrencia");
            ViewBag.id_tipo_impacto = new SelectList(db.c_tipo_impacto, "id_tipo_impacto", "nb_tipo_impacto");

            ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
            ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
            ViewBag.id_sub_proceso = new SelectList(db.c_sub_proceso, "id_sub_proceso", "nb_sub_proceso");
            ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
            ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
            ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_ejecutor);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);
            ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(id_aseveracion);

            //region catalogos de riesgo operativo
            ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacionalR(model.id_proceso_riesgo_operacional ?? 0);
            ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacionalR(model.id_sub_tipo_producto_riesgo_operacional ?? 0);
            ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacionalR(model.id_sub_tipo_riesgo_operacional ?? 0);
            ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacionalR(model.id_linea_negocio_riesgo_operacional ?? 0);
            ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional(model.id_frecuencia_riesgo_operacional ?? 0);
            ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional(model.id_impacto_riesgo_operacional ?? 0);


            //Campos extra
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //Enviar todos los datos de la tabla de Criticidad
            ViewBag.Criticidad = db.c_criticidad.ToList();

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

        // POST: Riesgo/AgregarMG
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AgregarMG(AgregarRiesgoMGViewModel model)
        {
            bool valid = ValidarIncidencia(ModelState, null, model);

            model.cl_riesgo = "0";

            valid = ValidarCE(model);

            if (ModelState.IsValid && valid)
            {
                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo.id_riesgo = 0;
                k_riesgo.id_sub_proceso = model.id_sub_proceso;
                k_riesgo.cl_riesgo = model.cl_riesgo;
                k_riesgo.nb_riesgo = model.nb_riesgo;
                k_riesgo.evento = model.evento;

                k_riesgo = llenarCamposExtra(model, k_riesgo);

                db.k_riesgo.Add(k_riesgo);
                db.SaveChanges();


                k_control k_control = new k_control();
                k_control.id_control = 0;
                k_control.id_sub_proceso = model.id_sub_proceso;
                if (!model.tiene_accion_correctora)
                {
                    k_control.actividad_control = model.actividad_control;
                    k_control.relacion_control = model.relacion_control;
                    k_control.evidencia_control = model.evidencia_control;
                    k_control.id_responsable = model.id_responsable;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.id_frecuencia_control = model.id_frecuencia_control;
                    k_control.id_naturaleza_control = model.id_naturaleza_control;

                    k_control = llenarCamposExtra(model, k_control);
                }
                else
                {
                    k_control.id_responsable = model.id_responsable;
                    k_control.tiene_accion_correctora = model.tiene_accion_correctora;
                    k_control.accion_correctora = model.accion_correctora;
                }

                k_riesgo.k_control.Add(k_control);

                db.k_control.Add(k_control);
                db.SaveChanges();

                //Utilidades.Utilidades.disposeRParam(k_riesgo.nb_riesgo, k_riesgo.id_sub_proceso);
                //Utilidades.Utilidades.disposeCParam(model.relacion_control, k_riesgo.id_sub_proceso);

                if (model.tiene_accion_correctora)
                {
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
                }

                Utilidades.Utilidades.TaskAsigned(k_control);
                return RedirectToAction("Riesgos", "Riesgo", new { id = k_riesgo.id_sub_proceso });
            }
            c_sub_proceso c_sub_proceso = db.c_sub_proceso.Find(model.id_sub_proceso);
            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", model.id_frecuencia_control);
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", model.id_naturaleza_control);
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", model.id_responsable);

            //Campos extra
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
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

        #endregion

        #region Borrar

        [HttpPost]
        [NotOnlyRead]
        public int DeleteRD(int id)
        {
            k_riesgo_derogado rd = db.k_riesgo_derogado.Find(id);

            db.k_riesgo_derogado.Remove(rd);
            db.SaveChanges();

            return id;
        }


        // GET: Riesgo/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
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
            string sql;
            sql =
                "select C.relacion_control as codigo_control, C.id_control, C.actividad_control, E.nb_usuario nb_ejecutor, R.nb_usuario nb_responsable" +
                "  from k_control C" +
                "  left outer join c_usuario E on C.id_ejecutor = E.id_usuario" +
                "  left outer join c_usuario R on C.id_responsable = R.id_usuario" +
                " where C.id_control in (select id_control from k_control_riesgo where id_riesgo = " + id.ToString() + ")";
            var controles = db.Database.SqlQuery<ListaControlesViewModel>(sql).ToList();
            ViewBag.controles = controles;

            string prefijo = k_riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);
            var rd = new k_riesgo_derogado();
            rd.id_sub_proceso = k_riesgo.id_sub_proceso;
            try
            {
                rd.evento = k_riesgo.nb_riesgo + " - " + k_riesgo.evento;
            }
            catch
            {
                rd.evento = "N/A - " + k_riesgo.evento;
            }


            Utilidades.DeleteActions.checkRedirect(redirect); //subrutina para saber si hay que volver a alguna pantalla

            if (prefijo == "MP")
            {
                ViewBag.Criticidad = db.c_criticidad.Where(c => c.id_magnitud_impacto == k_riesgo.id_magnitud_impacto && c.id_probabilidad_ocurrencia == k_riesgo.id_probabilidad_ocurrencia).First().c_criticidad_riesgo.nb_criticidad_riesgo;
                ViewBag.k_riesgo = k_riesgo;

                return View(rd);
            }
            else
            {
                ViewBag.k_riesgo = k_riesgo;
                return View("DeleteMG", rd);
            }
        }

        // POST: Riesgo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id, k_riesgo_derogado model)
        {
            k_riesgo k_riesgo = db.k_riesgo.Find(id);
            if (k_riesgo == null)
            {
                return HttpNotFound();
            }
            int id_sp = k_riesgo.id_sub_proceso;
            var controles = k_riesgo.k_control.ToList();

            if (ModelState.IsValid)
            {
                if (Utilidades.DeleteActions.DeleteRiesgoObjects(k_riesgo, db, true))
                {
                    db.SaveChanges();

                    model.cl_riesgo = k_riesgo.cl_riesgo;
                    model.nb_riesgo = k_riesgo.nb_riesgo;
                    model.criticidad = k_riesgo.criticidad;

                   // model.monto_impacto = k_riesgo.monto_impacto;

                    model.es_riesgo_operativo = k_riesgo.es_riesgo_operativo;
                    model.euc = k_riesgo.euc;
                    try { model.frecuencia_riesgo_operacional = k_riesgo.c_frecuencia_riesgo_operacional.cl_frecuencia_riesgo_operacional + " - " + k_riesgo.c_frecuencia_riesgo_operacional.nb_frecuencia_riesgo_operacional; } catch { model.frecuencia_riesgo_operacional = "N/A"; }
                    try { model.impacto_riesgo_operacional = k_riesgo.c_impacto_riesgo_operacional.cl_impacto_riesgo_operacional + " - " + k_riesgo.c_impacto_riesgo_operacional.nb_impacto_riesgo_operacional; } catch { model.impacto_riesgo_operacional = "N/A"; }
                    try { model.linea_negocio_riesgo_operacional = k_riesgo.c_linea_negocio_riesgo_operacional.cl_linea_negocio_riesgo_operacional + " - " + k_riesgo.c_linea_negocio_riesgo_operacional.nb_linea_negocio_riesgo_operacional; } catch { model.linea_negocio_riesgo_operacional = "N/A"; }
                    try { model.proceso_riesgo_operacional = k_riesgo.c_proceso_riesgo_operacional.c_ambito_riesgo_operacional.cl_ambito_riesgo_operacional + " - " + k_riesgo.c_proceso_riesgo_operacional.c_ambito_riesgo_operacional.nb_ambito_riesgo_operacional + "/" + k_riesgo.c_proceso_riesgo_operacional.cl_proceso_riesgo_operacional + " - " + k_riesgo.c_proceso_riesgo_operacional.nb_proceso_riesgo_operacional; } catch { model.proceso_riesgo_operacional = "N/A"; }
                    try { model.sub_tipo_producto_riesgo_operacional = k_riesgo.c_sub_tipo_producto_riesgo_operacional.c_producto_riesgo_operacional.cl_producto_riesgo_operacional + " - " + k_riesgo.c_sub_tipo_producto_riesgo_operacional.c_producto_riesgo_operacional.nb_producto_riesgo_operacional + "/" + k_riesgo.c_sub_tipo_producto_riesgo_operacional.cl_sub_tipo_producto_riesgo_operacional + " - " + k_riesgo.c_sub_tipo_producto_riesgo_operacional.nb_sub_tipo_producto_riesgo_operacional; } catch { model.sub_tipo_producto_riesgo_operacional = "N/A"; }
                    try { model.sub_tipo_riesgo_operacional = k_riesgo.c_sub_tipo_riesgo_operacional.c_tipo_riesgo_operacional.cl_tipo_riesgo_operacional + " - " + k_riesgo.c_sub_tipo_riesgo_operacional.c_tipo_riesgo_operacional.nb_tipo_riesgo_operacional + "/" + k_riesgo.c_sub_tipo_riesgo_operacional.cl_sub_tipo_riesgo_operacional + " - " + k_riesgo.c_sub_tipo_riesgo_operacional.nb_sub_tipo_riesgo_operacional; } catch { model.sub_tipo_riesgo_operacional = "N/A"; }
                    try { model.magnitud_impacto = k_riesgo.c_magnitud_impacto.cl_magnitud_impacto + " - " + k_riesgo.c_magnitud_impacto.nb_magnitud_impacto; } catch { model.magnitud_impacto = "N/A"; }
                    try { model.tipo_riesgo = k_riesgo.c_tipo_riesgo.cl_tipo_riesgo + " - " + k_riesgo.c_tipo_riesgo.nb_tipo_riesgo; } catch { model.tipo_riesgo = "N/A"; }
                    try { model.tipologia_riesgo = k_riesgo.c_tipologia_riesgo.cl_tipologia_riesgo + " - " + k_riesgo.c_tipologia_riesgo.nb_tipologia_riesgo; } catch { model.tipologia_riesgo = "N/A"; }
                    try { model.probabilidad_ocurrencia = k_riesgo.c_probabilidad_ocurrencia.cl_probabilidad_ocurrencia + " - " + k_riesgo.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia; } catch { model.probabilidad_ocurrencia = "N/A"; }
                    try { model.tipo_impacto = k_riesgo.c_tipo_impacto.cl_tipo_impacto + " - " + k_riesgo.c_tipo_impacto.nb_tipo_impacto; } catch { model.tipo_impacto = "N/A"; }
                    try { model.criticidadRO = Utilidades.Utilidades.Criticidad(k_riesgo.id_frecuencia_riesgo_operacional ?? 0,k_riesgo.id_impacto_riesgo_operacional ?? 0,true); } catch { model.criticidadRO = "N/A"; }

                    //Debug.WriteLine("Criticidad: " + model.criticidadRO);

                    model.tiene_afectacion_contable = k_riesgo.tiene_afectacion_contable;
                    model.supuesto_normativo = k_riesgo.supuesto_normativo;




                    model.campo01 = k_riesgo.campo01;
                    model.campo02 = k_riesgo.campo02;
                    model.campo03 = k_riesgo.campo03;
                    model.campo04 = k_riesgo.campo04;
                    model.campo05 = k_riesgo.campo05;
                    model.campo06 = k_riesgo.campo06;
                    model.campo07 = k_riesgo.campo07;
                    model.campo08 = k_riesgo.campo08;
                    model.campo09 = k_riesgo.campo09;
                    model.campo10 = k_riesgo.campo10;
                    model.campo11 = k_riesgo.campo11;
                    model.campo12 = k_riesgo.campo12;
                    model.campo13 = k_riesgo.campo13;
                    model.campo14 = k_riesgo.campo14;
                    model.campo15 = k_riesgo.campo15;
                    model.campo16 = k_riesgo.campo16;
                    model.campo17 = k_riesgo.campo17;
                    model.campo18 = k_riesgo.campo18;
                    model.campo19 = k_riesgo.campo19;
                    model.campo20 = k_riesgo.campo20;


                    db.k_riesgo.Remove(k_riesgo);
                    Utilidades.DeleteActions.DelUnlinkedControls(id_sp, db);
                    try
                    {
                        db.k_riesgo_derogado.Add(model);
                        db.SaveChanges();
                    }
                    catch
                    {
                        return RedirectToAction("CantErase", "Error", null);
                    }
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
                    return RedirectToAction("Riesgos", "Riesgo", new { id = id_sp });

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
                        return RedirectToAction("Riesgos", "Riesgo", new { id = id_sp });
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


            string sql;
            sql =
                "select C.relacion_control as codigo_control, C.id_control, C.actividad_control, E.nb_usuario nb_ejecutor, R.nb_usuario nb_responsable" +
                "  from k_control C" +
                "  left outer join c_usuario E on C.id_ejecutor = E.id_usuario" +
                "  left outer join c_usuario R on C.id_responsable = R.id_usuario" +
                " where C.id_control in (select id_control from k_control_riesgo where id_riesgo = " + id.ToString() + ")";
            var controless = db.Database.SqlQuery<ListaControlesViewModel>(sql).ToList();
            ViewBag.controles = controless;

            //volver a revisar si es MG o MP
            ViewBag.k_riesgo = k_riesgo;
            if (k_riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) == "MP")
                return View(model);
            else
                return View("DeleteMG", model);
        }

        #endregion

        #region Editar
        public ActionResult Editar(int? id, bool MuestraControles = false)
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


            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraRV = Utilidades.Utilidades.valCamposExtra("k_riesgo", 20, (int)id);
            ViewBag.MRError = new string[20];
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            if (prefijo == "MP")
            {
                EditarRiesgoViewModel model = new EditarRiesgoViewModel();

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso;

                //LLenar y elegir combo de categorías de Riesgo
                var categorias = _repository.ObtieneCategoriasRiesgo().OrderBy(x => x.cl_categoria_riesgo);
                foreach (var categoria in categorias)
                {
                    model.CategoriasRiesgo.Add(new SelectListItem()
                    {
                        Text = categoria.cl_categoria_riesgo + " - " + categoria.nb_categoria_riesgo,
                        Value = categoria.id_categoria_riesgo.ToString()
                    });
                }

                //LLenar y elegir combo de Clases de tipología de Riesgo
                var clases = _repository.ObtieneClasesTipologiaRiesgo().OrderBy(x => x.cl_clase_tipologia_riesgo);
                foreach (var clase in clases)
                {
                    model.ClasesTipologiaRiesgo.Add(new SelectListItem()
                    {
                        Text = clase.cl_clase_tipologia_riesgo + " - " + clase.nb_clase_tipologia_riesgo,
                        Value = clase.id_clase_tipologia_riesgo.ToString(),
                    });
                }

                //LLenar y elegir combo de Tipos de Riesgo
                var TiposRiesgo = db.c_tipo_riesgo.Where(tr => tr.id_categoria_riesgo == k_riesgo.c_tipo_riesgo.id_categoria_riesgo).ToList();
                foreach (var TipoRiesgo in TiposRiesgo)
                {
                    model.TiposRiesgo.Add(new SelectListItem()
                    {
                        Text = TipoRiesgo.cl_tipo_riesgo + " - " + TipoRiesgo.nb_tipo_riesgo,
                        Value = TipoRiesgo.id_tipo_riesgo.ToString()
                    });
                }

                //llenar y elegir combo de Sub Clases de tipología de Riesgo
                var SubClases = db.c_sub_clase_tipologia_riesgo.Where(sctr => sctr.id_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo).ToList();
                foreach (var SC in SubClases)
                {
                    model.SubClasesTipologiaRiesgo.Add(new SelectListItem()
                    {
                        Text = SC.cl_sub_clase_tipologia_riesgo + " - " + SC.nb_sub_clase_tipologia_riesgo,
                        Value = SC.id_sub_clase_tipologia_riesgo.ToString()
                    });
                }

                //Llenar y elegir combo de tipologías de Riesgo
                var TipologiasRiesgo = db.c_tipologia_riesgo.Where(tr => tr.id_sub_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo).ToList();
                foreach (var TipologiaRiesgo in TipologiasRiesgo)
                {
                    model.TipologiasRiesgo.Add(new SelectListItem()
                    {
                        Text = TipologiaRiesgo.cl_tipologia_riesgo + " - " + TipologiaRiesgo.nb_tipologia_riesgo,
                        Value = TipologiaRiesgo.id_tipologia_riesgo.ToString()
                    });
                }

                ViewBag.id_magnitud_impacto = new SelectList(db.c_magnitud_impacto.OrderBy(c => c.cl_magnitud_impacto), "id_magnitud_impacto", "nb_magnitud_impacto", k_riesgo.id_magnitud_impacto);
                ViewBag.id_probabilidad_ocurrencia = new SelectList(db.c_probabilidad_ocurrencia.OrderBy(c => c.cl_probabilidad_ocurrencia), "id_probabilidad_ocurrencia", "nb_probabilidad_ocurrencia", k_riesgo.id_probabilidad_ocurrencia);
                ViewBag.id_tipo_impacto = new SelectList(db.c_tipo_impacto, "id_tipo_impacto", "nb_tipo_impacto", k_riesgo.id_tipo_impacto);

                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
                ViewBag.id_sub_proceso = new SelectList(db.c_sub_proceso, "id_sub_proceso", "nb_sub_proceso");
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario");
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario");

                //region catalogos de riesgo operativo
                ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacionalR(k_riesgo.id_proceso_riesgo_operacional ?? 0);
                ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacionalR(k_riesgo.id_sub_tipo_producto_riesgo_operacional ?? 0);
                ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacionalR(k_riesgo.id_sub_tipo_riesgo_operacional ?? 0);
                ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacionalR(k_riesgo.id_linea_negocio_riesgo_operacional ?? 0);
                ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional(k_riesgo.id_frecuencia_riesgo_operacional ?? 0);
                ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional(k_riesgo.id_impacto_riesgo_operacional ?? 0);


                //Llenamos la informacion de los combos
                model.id_categoria_riesgo = (int)k_riesgo.c_tipo_riesgo.id_categoria_riesgo;
                model.id_tipo_riesgo = (int)k_riesgo.id_tipo_riesgo;
                model.id_clase_tipologia_riesgo = (int)k_riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo;
                model.id_sub_clase_tipologia_riesgo = (int)k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo;
                model.id_tipologia_riesgo = (int)k_riesgo.id_tipologia_riesgo;

                model.id_riesgo = k_riesgo.id_riesgo;
                model.cl_riesgo = k_riesgo.cl_riesgo;
                model.nb_riesgo = k_riesgo.nb_riesgo;
                model.evento = k_riesgo.evento;
                model.criticidad = k_riesgo.criticidad;
                model.tiene_afectacion_contable = k_riesgo.tiene_afectacion_contable;
                model.supuesto_normativo = k_riesgo.supuesto_normativo;
                model.euc = k_riesgo.euc;

               // model.monto_impacto = k_riesgo.monto_impacto;

                //Riesgo Operativo
                model.es_riesgo_operativo = k_riesgo.es_riesgo_operativo;
                //model.id_sub_tipo_riesgo_operacional = k_riesgo.id_sub_tipo_riesgo_operacional;
                //model.id_producto_riesgo_operacional = k_riesgo.id_producto_riesgo_operacional;
                //model.id_proceso_riesgo_operacional = k_riesgo.id_proceso_riesgo_operacional;
                //model.id_linea_negocio_riesgo_operacional = k_riesgo.id_linea_negocio_riesgo_operacional;
                model.id_impacto_riesgo_operacional = k_riesgo.id_impacto_riesgo_operacional;
                model.id_frecuencia_riesgo_operacional = k_riesgo.id_frecuencia_riesgo_operacional;


                string sql;
                sql =
                    "select C.relacion_control as codigo_control, C.id_control, C.actividad_control, C.tiene_accion_correctora, C.accion_correctora, E.nb_usuario nb_ejecutor, R.nb_usuario nb_responsable" +
                    "  from k_control C" +
                    "  left outer join c_usuario E on C.id_ejecutor = E.id_usuario" +
                    "  left outer join c_usuario R on C.id_responsable = R.id_usuario" +
                    " where C.id_control in (select id_control from k_control_riesgo where id_riesgo = " + id.ToString() + ")";
                var controles = db.Database.SqlQuery<ListaControlesViewModel>(sql).ToList();

                ViewBag.controles = controles;


               

                if (MuestraControles)
                {
                    ViewBag.MControles = true;
                }
                else
                {
                    ViewBag.MControles = false;
                }

                //Enviar todos los datos de la tabla de Criticidad
                ViewBag.Criticidad = db.c_criticidad.ToList();

                //lenar datos de campos extra
                model = obtenerCamposExtra(model, k_riesgo);


                return View(model);
            }
            else
            {
                EditarRiesgoMGViewModel model = new EditarRiesgoMGViewModel();
                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso;

                model.id_riesgo = k_riesgo.id_riesgo;
                model.cl_riesgo = k_riesgo.cl_riesgo;
                model.nb_riesgo = k_riesgo.nb_riesgo;
                model.evento = k_riesgo.evento;

                var controls = k_riesgo.k_control.ToList();

                List<ListaControlesViewModel> controles = new List<ListaControlesViewModel>();

                foreach (k_control control in controls)
                {
                    ListaControlesViewModel aux = new ListaControlesViewModel();
                    aux.id_control = control.id_control;
                    aux.codigo_control = control.relacion_control;
                    aux.nb_ejecutor = "N/A";
                    aux.nb_responsable = control.c_usuario1.nb_usuario;
                    aux.actividad_control = control.actividad_control;
                    controles.Add(aux);
                }

                ViewBag.controles = controles;
                if (MuestraControles)
                {
                    ViewBag.MControles = true;
                }
                else
                {
                    ViewBag.MControles = false;
                }

                //lenar datos de campos extra
                model = obtenerCamposExtra(model, k_riesgo);

                return View("EditarMG", model);
            }


        }

        // POST: Riesgo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Editar(EditarRiesgoViewModel model)
        {
            k_riesgo k_riesgo = db.k_riesgo.Find(model.id_riesgo);
            if (k_riesgo == null)
            {
                return HttpNotFound();
            }
            model.cl_riesgo = "0";

            bool valid = ValidarCE(model);

            valid = valid && ValidarRiesgoOperativo(ModelState, model);

            if (ModelState.IsValid && valid)
            {
                recordChange(k_riesgo);


                k_riesgo.cl_riesgo = model.cl_riesgo;
                k_riesgo.nb_riesgo = model.nb_riesgo;
                k_riesgo.evento = model.evento;
                k_riesgo.id_tipo_riesgo = model.id_tipo_riesgo;
                k_riesgo.id_tipologia_riesgo = model.id_tipologia_riesgo;
                k_riesgo.id_probabilidad_ocurrencia = model.id_probabilidad_ocurrencia;
                k_riesgo.id_tipo_impacto = model.id_tipo_impacto;
                k_riesgo.id_magnitud_impacto = model.id_magnitud_impacto;
                k_riesgo.criticidad = model.criticidad;
                k_riesgo.tiene_afectacion_contable = model.tiene_afectacion_contable;
                k_riesgo.supuesto_normativo = model.supuesto_normativo;
                k_riesgo.euc = model.euc;

               // k_riesgo.monto_impacto = model.monto_impacto;

                k_riesgo.es_riesgo_operativo = model.es_riesgo_operativo;
                k_riesgo.id_frecuencia_riesgo_operacional = model.id_frecuencia_riesgo_operacional;
                k_riesgo.id_impacto_riesgo_operacional = model.id_impacto_riesgo_operacional;
                k_riesgo.id_linea_negocio_riesgo_operacional = model.id_linea_negocio_riesgo_operacional;
                k_riesgo.id_proceso_riesgo_operacional = model.id_proceso_riesgo_operacional;
                k_riesgo.id_sub_tipo_producto_riesgo_operacional = model.id_sub_tipo_producto_riesgo_operacional;
                k_riesgo.id_sub_tipo_riesgo_operacional = model.id_sub_tipo_riesgo_operacional;

                k_riesgo = llenarCamposExtra(model, k_riesgo);

                db.SaveChanges();

                return RedirectToAction("Riesgos", "Riesgo", new { id = k_riesgo.id_sub_proceso });
            }

            c_sub_proceso c_sub_proceso = new c_sub_proceso();
            c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso;

            //LLenar y elegir combo de categorías de Riesgo
            var categorias = _repository.ObtieneCategoriasRiesgo().OrderBy(x => x.cl_categoria_riesgo);
            foreach (var categoria in categorias)
            {
                model.CategoriasRiesgo.Add(new SelectListItem()
                {
                    Text = categoria.cl_categoria_riesgo + " - " + categoria.nb_categoria_riesgo,
                    Value = categoria.id_categoria_riesgo.ToString()
                });
            }

            //LLenar y elegir combo de Clases de tipología de Riesgo
            var clases = _repository.ObtieneClasesTipologiaRiesgo().OrderBy(x => x.cl_clase_tipologia_riesgo);
            foreach (var clase in clases)
            {
                model.ClasesTipologiaRiesgo.Add(new SelectListItem()
                {
                    Text = clase.cl_clase_tipologia_riesgo + " - " + clase.nb_clase_tipologia_riesgo,
                    Value = clase.id_clase_tipologia_riesgo.ToString(),
                });
            }

            //LLenar y elegir combo de Tipos de Riesgo
            var TiposRiesgo = db.c_tipo_riesgo.Where(tr => tr.id_categoria_riesgo == k_riesgo.c_tipo_riesgo.id_categoria_riesgo).ToList();
            foreach (var TipoRiesgo in TiposRiesgo)
            {
                model.TiposRiesgo.Add(new SelectListItem()
                {
                    Text = TipoRiesgo.cl_tipo_riesgo + " - " + TipoRiesgo.nb_tipo_riesgo,
                    Value = TipoRiesgo.id_tipo_riesgo.ToString()
                });
            }

            //llenar y elegir combo de Sub Clases de tipología de Riesgo
            var SubClases = db.c_sub_clase_tipologia_riesgo.Where(sctr => sctr.id_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo).ToList();
            foreach (var SC in SubClases)
            {
                model.SubClasesTipologiaRiesgo.Add(new SelectListItem()
                {
                    Text = SC.cl_sub_clase_tipologia_riesgo + " - " + SC.nb_sub_clase_tipologia_riesgo,
                    Value = SC.id_sub_clase_tipologia_riesgo.ToString()
                });
            }

            //Llenar y elegir combo de tipologías de Riesgo
            var TipologiasRiesgo = db.c_tipologia_riesgo.Where(tr => tr.id_sub_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo).ToList();
            foreach (var TipologiaRiesgo in TipologiasRiesgo)
            {
                model.TipologiasRiesgo.Add(new SelectListItem()
                {
                    Text = TipologiaRiesgo.cl_tipologia_riesgo + " - " + TipologiaRiesgo.nb_tipologia_riesgo,
                    Value = TipologiaRiesgo.id_tipologia_riesgo.ToString()
                });
            }

            ViewBag.id_magnitud_impacto = new SelectList(db.c_magnitud_impacto.OrderBy(c => c.cl_magnitud_impacto), "id_magnitud_impacto", "nb_magnitud_impacto", k_riesgo.id_magnitud_impacto);
            ViewBag.id_probabilidad_ocurrencia = new SelectList(db.c_probabilidad_ocurrencia.OrderBy(c => c.cl_probabilidad_ocurrencia), "id_probabilidad_ocurrencia", "nb_probabilidad_ocurrencia", k_riesgo.id_probabilidad_ocurrencia);
            ViewBag.id_tipo_impacto = new SelectList(db.c_tipo_impacto, "id_tipo_impacto", "nb_tipo_impacto", k_riesgo.id_tipo_impacto);
            ViewBag.id_tipo_riesgo = new SelectList(db.c_tipo_riesgo.Where(z => z.id_categoria_riesgo == k_riesgo.c_tipo_riesgo.id_categoria_riesgo), "id_tipo_riesgo", "nb_tipo_riesgo", k_riesgo.id_tipo_riesgo);
            ViewBag.id_tipologia_riesgo = new SelectList(db.c_tipologia_riesgo.Where(z => z.id_sub_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo), "id_tipologia_riesgo", "nb_tipologia_riesgo", k_riesgo.id_tipologia_riesgo);
            ViewBag.id_sub_clase_tipologia_riesgo = new SelectList(db.c_sub_clase_tipologia_riesgo.Where(z => z.id_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo), "id_sub_clase_tipologia_riesgo", "nb_sub_clase_tipologia_riesgo", k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo);

            ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
            ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
            ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
            ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
            ViewBag.id_sub_proceso = new SelectList(db.c_sub_proceso, "id_sub_proceso", "nb_sub_proceso");
            ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
            ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
            ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario");
            ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario");

            //region catalogos de riesgo operativo
            ViewBag.id_proceso_riesgo_operacionalL = Utilidades.DropDown.ProcesoRiesgoOperacionalR(model.id_proceso_riesgo_operacional ?? 0);
            ViewBag.id_sub_tipo_producto_riesgo_operacionalL = Utilidades.DropDown.SubTipoProductoRiesgoOperacionalR(model.id_sub_tipo_producto_riesgo_operacional ?? 0);
            ViewBag.id_sub_tipo_riesgo_operacionalL = Utilidades.DropDown.SubTipoRiesgoOperacionalR(model.id_sub_tipo_riesgo_operacional ?? 0);
            ViewBag.id_linea_negocio_riesgo_operacionalL = Utilidades.DropDown.LineaNegocioRiesgoOperacionalR(model.id_linea_negocio_riesgo_operacional ?? 0);
            ViewBag.id_frecuencia_riesgo_operacionalL = Utilidades.DropDown.FrecuenciaRiesgoOperacional(model.id_frecuencia_riesgo_operacional ?? 0);
            ViewBag.id_impacto_riesgo_operacionalL = Utilidades.DropDown.ImpactoRiesgoOperacional(model.id_impacto_riesgo_operacional ?? 0);

            string sql;
            sql =
                "select C.relacion_control as codigo_control, C.id_control, C.actividad_control, C.tiene_accion_correctora, C.accion_correctora, E.nb_usuario nb_ejecutor, R.nb_usuario nb_responsable" +
                "  from k_control C" +
                "  left outer join c_usuario E on C.id_ejecutor = E.id_usuario" +
                "  left outer join c_usuario R on C.id_responsable = R.id_usuario" +
                " where C.id_control in (select id_control from k_control_riesgo where id_riesgo = " + model.id_riesgo.ToString() + ")";
            var controles = db.Database.SqlQuery<ListaControlesViewModel>(sql).ToList();
            ViewBag.controles = controles;
            ViewBag.MControles = false;

            //Enviar todos los datos de la tabla de Criticidad
            ViewBag.Criticidad = db.c_criticidad.ToList();

            //Campos extra
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

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

        // POST: Riesgo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult EditarMG(EditarRiesgoMGViewModel model)
        {
            k_riesgo k_riesgo = db.k_riesgo.Find(model.id_riesgo);
            if (k_riesgo == null)
            {
                return HttpNotFound();
            }

            model.cl_riesgo = "0";

            bool valid = ValidarCE(model);

            if (ModelState.IsValid && valid)
            {
                recordChange(k_riesgo);

                k_riesgo.cl_riesgo = model.cl_riesgo;
                k_riesgo.nb_riesgo = model.nb_riesgo;
                k_riesgo.evento = model.evento;

                k_riesgo = llenarCamposExtra(model, k_riesgo);

                db.SaveChanges();

                return RedirectToAction("Riesgos", "Riesgo", new { id = k_riesgo.id_sub_proceso });
            }

            c_sub_proceso c_sub_proceso = new c_sub_proceso();
            c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

            model.c_sub_proceso = c_sub_proceso;
            model.id_sub_proceso = c_sub_proceso.id_sub_proceso;

            var controls = k_riesgo.k_control.ToList();

            List<ListaControlesViewModel> controles = new List<ListaControlesViewModel>();

            foreach (k_control control in controls)
            {
                ListaControlesViewModel aux = new ListaControlesViewModel();
                aux.id_control = control.id_control;
                aux.codigo_control = control.relacion_control;
                aux.nb_ejecutor = "N/A";
                aux.nb_responsable = control.c_usuario1.nb_usuario;
                aux.actividad_control = control.actividad_control;
                aux.tiene_accion_correctora = control.tiene_accion_correctora;
                aux.accion_correctora = control.accion_correctora;
                controles.Add(aux);
            }

            ViewBag.controles = controles;
            ViewBag.MControles = false;

            //Campos extra
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            //Datos del diagrama
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/" + model.id_sub_proceso);

            var JsonData = "";

            //Si el archivo existe
            if (System.IO.File.Exists(path))
            {
                JsonData = System.IO.File.ReadAllText(path);
            }

            ViewBag.JsonData = JsonData;

            return View("EditarMG", model);
        }
        #endregion

        #region Otros,Validaciones
        public string GetCriticidad(int idfr, int idi)
        {
            return Utilidades.Utilidades.Criticidad(idfr, idi);
        }


        public ActionResult infoMI()
        {
            return PartialView("DetailViews/infoMI");
        }


        private bool ValidarRiesgoOperativo(ModelStateDictionary modelState, AgregarRiesgoViewModel model)
        {
            bool valid = true;

            //validar los campos
            if (model.es_riesgo_operativo)
            {
                if(model.id_proceso_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_proceso_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_sub_tipo_producto_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_sub_tipo_producto_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_sub_tipo_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_sub_tipo_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_linea_negocio_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_linea_negocio_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_frecuencia_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_frecuencia_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_impacto_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_impacto_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
            }
            else
            {
                model.id_proceso_riesgo_operacional = null;
                model.id_sub_tipo_producto_riesgo_operacional = null;
                model.id_frecuencia_riesgo_operacional = null;
                model.id_impacto_riesgo_operacional = null;
                model.id_sub_tipo_riesgo_operacional = null;
                model.id_linea_negocio_riesgo_operacional = null;
            }

            return valid;
        }

        private bool ValidarRiesgoOperativo(ModelStateDictionary modelState, EditarRiesgoViewModel model)
        {
            bool valid = true;

            //validar los campos
            if (model.es_riesgo_operativo)
            {
                if (model.id_proceso_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_proceso_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_sub_tipo_producto_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_producto_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_sub_tipo_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_sub_tipo_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_linea_negocio_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_linea_negocio_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_frecuencia_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_frecuencia_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
                if (model.id_impacto_riesgo_operacional == null)
                {
                    ModelState.AddModelError("id_impacto_riesgo_operacional", Strings.getMSG("IndicadorDiarioCreate004"));
                    valid = false;
                }
            }
            else
            {
                model.id_proceso_riesgo_operacional = null;
                model.id_sub_tipo_producto_riesgo_operacional = null;
                model.id_frecuencia_riesgo_operacional = null;
                model.id_impacto_riesgo_operacional = null;
                model.id_sub_tipo_riesgo_operacional = null;
                model.id_linea_negocio_riesgo_operacional = null;
            }

            return valid;
        }



        private bool ValidarIncidencia(ModelStateDictionary modelState, AgregarRiesgoViewModel m1 = null, AgregarRiesgoMGViewModel m2 = null)
        {
            bool valid = true;
            if (m1 != null)
            {
                //La incidencia se valida solo en caso de tener una accion Correctora
                if (m1.tiene_accion_correctora)
                {
                    if (m1.accion_correctora == "" || m1.accion_correctora == null)
                    {
                        modelState.AddModelError("accion_correctora", "La acción correctora es un campo requerido");
                        valid = false;
                    }
                    if (m1.ds_incidencia == null)
                    {
                        modelState.AddModelError("ds_incidencia", Strings.getMSG("IndicadorDiarioCreate004"));
                        valid = false;
                    }
                    if (m1.id_responsable_i <= 0 || m1.id_responsable_i == null)
                    {
                        modelState.AddModelError("id_responsable_i", "Seleccione un responsable para la Incidencia");
                        valid = false;
                    }
                    if (m1.id_clasificacion_incidencia <= 0 || m1.id_clasificacion_incidencia == null)
                    {
                        modelState.AddModelError("id_clasificacion_incidencia", "Seleccione una clasificación para la incidencia");
                        valid = false;
                    }
                    if (!m1.requiere_plan)
                    {
                        if (m1.js_incidencia == null)
                        {
                            modelState.AddModelError("js_incidencia", Strings.getMSG("IndicadorDiarioCreate004"));
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
                        modelState.AddModelError("accion_correctora", "La acción correctora es un campo requerido");
                        valid = false;
                    }
                    if (m2.ds_incidencia == null)
                    {
                        modelState.AddModelError("ds_incidencia", Strings.getMSG("IndicadorDiarioCreate004"));
                        valid = false;
                    }
                    if (m2.id_responsable_i <= 0 || m2.id_responsable_i == null)
                    {
                        modelState.AddModelError("id_responsable_i", "Seleccione un responsable para la Incidencia");
                        valid = false;
                    }
                    if (m2.id_clasificacion_incidencia <= 0 || m2.id_clasificacion_incidencia == null)
                    {
                        modelState.AddModelError("id_clasificacion_incidencia", "Seleccione una clasificación para la incidencia");
                        valid = false;
                    }
                    if (!m2.requiere_plan)
                    {
                        if (m2.js_incidencia == null)
                        {
                            modelState.AddModelError("js_incidencia", Strings.getMSG("IndicadorDiarioCreate004"));
                            valid = false;
                        }
                    }
                }
            }

            return valid;
        }

        private bool ValidarCE(AgregarRiesgoViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            bool response = true;

            //Asignacion de valores
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

            //Informacion de Meta Campos Controles
            var info = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = info[i];

                bool validate = inf.es_visible && !model.tiene_accion_correctora;

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

            errores = new string[20];
            //Validacion del Riesgo
            //Asignacion de valores
            campo[0] = model.campor01; campo[1] = model.campor02;
            campo[2] = model.campor03; campo[3] = model.campor04;
            campo[4] = model.campor05; campo[5] = model.campor06;
            campo[6] = model.campor07; campo[7] = model.campor08;
            campo[8] = model.campor09; campo[9] = model.campor10;
            campo[10] = model.campor11; campo[11] = model.campor12;
            campo[12] = model.campor13; campo[13] = model.campor14;
            campo[14] = model.campor15; campo[15] = model.campor16;
            campo[16] = model.campor17; campo[17] = model.campor18;
            campo[18] = model.campor19; campo[19] = model.campor20;

            //Informacion de Meta Campos Riesgos
            var infor = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = infor[i];

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

            ControllerContext.Controller.ViewBag.MRError = errores;
            return response;
        }

        private bool ValidarCE(AgregarRiesgoMGViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            bool response = true;

            //Asignacion de valores
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

            //Informacion de Meta Campos Controles
            var info = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = info[i];

                bool validate = inf.aparece_en_mg && inf.es_visible && !model.tiene_accion_correctora;

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


            errores = new string[20];
            //Validacion del Riesgo
            //Asignacion de valores
            campo[0] = model.campor01; campo[1] = model.campor02;
            campo[2] = model.campor03; campo[3] = model.campor04;
            campo[4] = model.campor05; campo[5] = model.campor06;
            campo[6] = model.campor07; campo[7] = model.campor08;
            campo[8] = model.campor09; campo[9] = model.campor10;
            campo[10] = model.campor11; campo[11] = model.campor12;
            campo[12] = model.campor13; campo[13] = model.campor14;
            campo[14] = model.campor15; campo[15] = model.campor16;
            campo[16] = model.campor17; campo[17] = model.campor18;
            campo[18] = model.campor19; campo[19] = model.campor20;

            //Informacion de Meta Campos Riesgos
            var infor = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = infor[i];

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

            ControllerContext.Controller.ViewBag.MRError = errores;
            return response;
        }

        private bool ValidarCE(EditarRiesgoViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            bool response = true;

            //Validacion del Riesgo
            //Asignacion de valores
            campo[0] = model.campor01; campo[1] = model.campor02;
            campo[2] = model.campor03; campo[3] = model.campor04;
            campo[4] = model.campor05; campo[5] = model.campor06;
            campo[6] = model.campor07; campo[7] = model.campor08;
            campo[8] = model.campor09; campo[9] = model.campor10;
            campo[10] = model.campor11; campo[11] = model.campor12;
            campo[12] = model.campor13; campo[13] = model.campor14;
            campo[14] = model.campor15; campo[15] = model.campor16;
            campo[16] = model.campor17; campo[17] = model.campor18;
            campo[18] = model.campor19; campo[19] = model.campor20;

            //Informacion de Meta Campos Riesgos
            var infor = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = infor[i];

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

            ControllerContext.Controller.ViewBag.MRError = errores;
            return response;
        }

        private bool ValidarCE(EditarRiesgoMGViewModel model)
        {
            string[] campo = new string[20];
            string[] errores = new string[20];
            bool response = true;

            //Validacion del Riesgo
            //Asignacion de valores
            campo[0] = model.campor01; campo[1] = model.campor02;
            campo[2] = model.campor03; campo[3] = model.campor04;
            campo[4] = model.campor05; campo[5] = model.campor06;
            campo[6] = model.campor07; campo[7] = model.campor08;
            campo[8] = model.campor09; campo[9] = model.campor10;
            campo[10] = model.campor11; campo[11] = model.campor12;
            campo[12] = model.campor13; campo[13] = model.campor14;
            campo[14] = model.campor15; campo[15] = model.campor16;
            campo[16] = model.campor17; campo[17] = model.campor18;
            campo[18] = model.campor19; campo[19] = model.campor20;

            //Informacion de Meta Campos Riesgos
            var infor = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);

            //Validacion
            for (int i = 0; i < 20; i++)
            {
                var inf = infor[i];

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

            ControllerContext.Controller.ViewBag.MRError = errores;
            return response;
        }

        k_control llenarCamposExtra(AgregarRiesgoViewModel model, k_control control)
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

        k_control llenarCamposExtra(AgregarRiesgoMGViewModel model, k_control control)
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

        k_riesgo llenarCamposExtra(AgregarRiesgoViewModel model, k_riesgo riesgo)
        {
            riesgo.campo01 = model.campor01; riesgo.campo02 = model.campor02;
            riesgo.campo03 = model.campor03; riesgo.campo04 = model.campor04;
            riesgo.campo05 = model.campor05; riesgo.campo06 = model.campor06;
            riesgo.campo07 = model.campor07; riesgo.campo08 = model.campor08;
            riesgo.campo09 = model.campor09; riesgo.campo10 = model.campor10;
            riesgo.campo11 = model.campor11; riesgo.campo12 = model.campor12;
            riesgo.campo13 = model.campor13; riesgo.campo14 = model.campor14;
            riesgo.campo15 = model.campor15; riesgo.campo16 = model.campor16;
            riesgo.campo17 = model.campor17; riesgo.campo18 = model.campor18;
            riesgo.campo19 = model.campor19; riesgo.campo20 = model.campor20;

            return riesgo;
        }

        k_riesgo llenarCamposExtra(AgregarRiesgoMGViewModel model, k_riesgo riesgo)
        {
            riesgo.campo01 = model.campor01; riesgo.campo02 = model.campor02;
            riesgo.campo03 = model.campor03; riesgo.campo04 = model.campor04;
            riesgo.campo05 = model.campor05; riesgo.campo06 = model.campor06;
            riesgo.campo07 = model.campor07; riesgo.campo08 = model.campor08;
            riesgo.campo09 = model.campor09; riesgo.campo10 = model.campor10;
            riesgo.campo11 = model.campor11; riesgo.campo12 = model.campor12;
            riesgo.campo13 = model.campor13; riesgo.campo14 = model.campor14;
            riesgo.campo15 = model.campor15; riesgo.campo16 = model.campor16;
            riesgo.campo17 = model.campor17; riesgo.campo18 = model.campor18;
            riesgo.campo19 = model.campor19; riesgo.campo20 = model.campor20;

            return riesgo;
        }

        k_riesgo llenarCamposExtra(EditarRiesgoViewModel model, k_riesgo riesgo)
        {
            riesgo.campo01 = model.campor01; riesgo.campo02 = model.campor02;
            riesgo.campo03 = model.campor03; riesgo.campo04 = model.campor04;
            riesgo.campo05 = model.campor05; riesgo.campo06 = model.campor06;
            riesgo.campo07 = model.campor07; riesgo.campo08 = model.campor08;
            riesgo.campo09 = model.campor09; riesgo.campo10 = model.campor10;
            riesgo.campo11 = model.campor11; riesgo.campo12 = model.campor12;
            riesgo.campo13 = model.campor13; riesgo.campo14 = model.campor14;
            riesgo.campo15 = model.campor15; riesgo.campo16 = model.campor16;
            riesgo.campo17 = model.campor17; riesgo.campo18 = model.campor18;
            riesgo.campo19 = model.campor19; riesgo.campo20 = model.campor20;

            return riesgo;
        }

        k_riesgo llenarCamposExtra(EditarRiesgoMGViewModel model, k_riesgo riesgo)
        {
            riesgo.campo01 = model.campor01; riesgo.campo02 = model.campor02;
            riesgo.campo03 = model.campor03; riesgo.campo04 = model.campor04;
            riesgo.campo05 = model.campor05; riesgo.campo06 = model.campor06;
            riesgo.campo07 = model.campor07; riesgo.campo08 = model.campor08;
            riesgo.campo09 = model.campor09; riesgo.campo10 = model.campor10;
            riesgo.campo11 = model.campor11; riesgo.campo12 = model.campor12;
            riesgo.campo13 = model.campor13; riesgo.campo14 = model.campor14;
            riesgo.campo15 = model.campor15; riesgo.campo16 = model.campor16;
            riesgo.campo17 = model.campor17; riesgo.campo18 = model.campor18;
            riesgo.campo19 = model.campor19; riesgo.campo20 = model.campor20;

            return riesgo;
        }

        EditarRiesgoViewModel obtenerCamposExtra(EditarRiesgoViewModel model, k_riesgo riesgo)
        {
            model.campor01 = riesgo.campo01; model.campor02 = riesgo.campo02;
            model.campor03 = riesgo.campo03; model.campor04 = riesgo.campo04;
            model.campor05 = riesgo.campo05; model.campor06 = riesgo.campo06;
            model.campor07 = riesgo.campo07; model.campor08 = riesgo.campo08;
            model.campor09 = riesgo.campo09; model.campor10 = riesgo.campo10;
            model.campor11 = riesgo.campo11; model.campor12 = riesgo.campo12;
            model.campor13 = riesgo.campo13; model.campor14 = riesgo.campo14;
            model.campor15 = riesgo.campo15; model.campor16 = riesgo.campo16;
            model.campor17 = riesgo.campo17; model.campor18 = riesgo.campo18;
            model.campor19 = riesgo.campo19; model.campor20 = riesgo.campo20;

            return model;
        }

        EditarRiesgoMGViewModel obtenerCamposExtra(EditarRiesgoMGViewModel model, k_riesgo riesgo)
        {
            model.campor01 = riesgo.campo01; model.campor02 = riesgo.campo02;
            model.campor03 = riesgo.campo03; model.campor04 = riesgo.campo04;
            model.campor05 = riesgo.campo05; model.campor06 = riesgo.campo06;
            model.campor07 = riesgo.campo07; model.campor08 = riesgo.campo08;
            model.campor09 = riesgo.campo09; model.campor10 = riesgo.campo10;
            model.campor11 = riesgo.campo11; model.campor12 = riesgo.campo12;
            model.campor13 = riesgo.campo13; model.campor14 = riesgo.campo14;
            model.campor15 = riesgo.campo15; model.campor16 = riesgo.campo16;
            model.campor17 = riesgo.campo17; model.campor18 = riesgo.campo18;
            model.campor19 = riesgo.campo19; model.campor20 = riesgo.campo20;

            return model;
        }

        bool recordChange(k_riesgo riesgo)
        {
            var registro = new r_riesgo();
            registro.campo01 = riesgo.campo01; registro.campo02 = riesgo.campo02;
            registro.campo03 = riesgo.campo03; registro.campo04 = riesgo.campo04;
            registro.campo05 = riesgo.campo05; registro.campo06 = riesgo.campo06;
            registro.campo07 = riesgo.campo07; registro.campo08 = riesgo.campo08;
            registro.campo09 = riesgo.campo09; registro.campo10 = riesgo.campo10;
            registro.campo11 = riesgo.campo11; registro.campo12 = riesgo.campo12;
            registro.campo13 = riesgo.campo13; registro.campo14 = riesgo.campo14;
            registro.campo15 = riesgo.campo15; registro.campo16 = riesgo.campo16;
            registro.campo17 = riesgo.campo17; registro.campo18 = riesgo.campo18;
            registro.campo19 = riesgo.campo19; registro.campo20 = riesgo.campo20;

            registro.tiene_afectacion_contable = riesgo.tiene_afectacion_contable;
            registro.nb_riesgo = riesgo.nb_riesgo;
            registro.supuesto_normativo = riesgo.supuesto_normativo;
            registro.cl_riesgo = riesgo.cl_riesgo;
            registro.criticidad = riesgo.criticidad;
            registro.euc = riesgo.euc;
            registro.evento = riesgo.evento;
            registro.fe_modificacion = DateTime.Now;
            registro.id_magnitud_impacto = riesgo.id_magnitud_impacto;
            registro.id_probabilidad_ocurrencia = riesgo.id_probabilidad_ocurrencia;
            registro.id_riesgo = riesgo.id_riesgo;
            registro.id_tipologia_riesgo = riesgo.id_tipologia_riesgo;
            registro.id_tipo_impacto = riesgo.id_tipo_impacto;
            registro.id_tipo_riesgo = riesgo.id_tipo_riesgo;
            registro.id_usuario = ((IdentityPersonalizado)HttpContext.User.Identity).Id_usuario;

            db.r_riesgo.Add(registro);

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

        public ActionResult Historial(int? id)
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

            var historial = db.r_riesgo.Where(r => r.id_riesgo == id).OrderByDescending(r => r.fe_modificacion).ToList();

            ViewBag.MG = k_riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) == "MG";
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);


            return View(historial);
        }


        public ActionResult HistorialControl(int? id)
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

            var historial = db.r_control.Where(r => r.id_control == id).OrderByDescending(r => r.fe_modificacion).ToList();

            ViewBag.MG = k_control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) == "MG";
            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

            return View(historial);
        }

        public FileResult GetPDFManual(int id, int sp = 0)
        {
            var rootContent = db.c_contenido_manual.Find(id);
            var bytes = Utilidades.GenerateDoc.Manual(rootContent, sp);

            var name = Utilidades.Utilidades.NormalizarNombreArchivo(rootContent.c_nivel_manual.nb_nivel_manual + " " + rootContent.cl_contenido_manual);

            return File(bytes, "application/pdf");
            //return File(bytes, "application/pdf", name + ".pdf");
        }

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
