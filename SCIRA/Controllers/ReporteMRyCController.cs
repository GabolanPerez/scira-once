using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "GeneralMRyC", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteMRyCController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteMRyC
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);

            var controles = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "k_control", "1").Cast<k_control>().ToList();
            ViewBag.RiesgoDerogado = Utilidades.Utilidades.RTCRiesgoDerogado(db.c_usuario.Find(user.Id_usuario), db);

            ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);



            //Creación del model con datos

            List<MRyCRepModel> model = new List<MRyCRepModel>();
            var criticidades = db.c_criticidad.ToList();

            foreach (var item in controles.Cast<k_control>())
            {
                var registro = new MRyCRepModel();

                Type m_tipo = null;
                PropertyInfo[] props_sp = null;
                PropertyInfo[] props_r = null;
                PropertyInfo[] props_c = null;
                PropertyInfo[] props_reg = null;

                var riesgo = item.k_riesgo.First();
                var sp = item.c_sub_proceso;
                var pr = sp.c_proceso;
                var mp = pr.c_macro_proceso;
                var en = mp.c_entidad;

                var respPr = pr.c_usuario;
                var psrePr = respPr.c_puesto.Count != 0 ? respPr.c_puesto.First().nb_puesto : "";
                var respMp = mp.c_usuario;
                var psreMp = respMp.c_puesto.Count != 0 ? respMp.c_puesto.First().nb_puesto : "";
                var respEn = en.c_usuario;
                var psreEn = respEn.c_puesto.Count != 0 ? respEn.c_puesto.First().nb_puesto : "";
                var respSp = sp.c_usuario;
                var psreSp = respSp.c_puesto.Count != 0 ? respSp.c_puesto.First().nb_puesto : "";

                //Obtener propiedades de SP para lectura dinamica
                m_tipo = sp.GetType();
                props_sp = m_tipo.GetProperties();
                //Obtener propiedades de Riesgo para lectura dinamica
                m_tipo = riesgo.GetType();
                props_r = m_tipo.GetProperties();
                //Obtener propiedades de Control para lectura dinamica
                m_tipo = item.GetType();
                props_c = m_tipo.GetProperties();
                //Obtener propiedades de Registro para lectura dinamica
                m_tipo = registro.GetType();
                props_reg = m_tipo.GetProperties();


                //Datos del SP
                registro.en = en.cl_entidad + " - " + en.nb_entidad;
                registro.respEn = respEn.nb_usuario + " (" + psreEn + ")";
                registro.mp = mp.cl_macro_proceso + " - " + mp.nb_macro_proceso;
                registro.respMp = respMp.nb_usuario + " (" + psreMp + ")";
                registro.pr = pr.cl_proceso + " - " + pr.nb_proceso;
                registro.respPr = respPr.nb_usuario + " (" + psrePr + ")";
                registro.sp = sp.cl_sub_proceso + " - " + sp.nb_sub_proceso;
                registro.descripcionSp = sp.ds_sub_proceso;  //NUEVO
                registro.SpAnterior = sp.cl_sp_anterior;  //NUEVO
                registro.SpSiguiente = sp.cl_sp_siguiente;   //NUEVO
                registro.tipologia_sp = sp.c_tipologia_sub_proceso.nb_tipologia_sub_proceso;  //NUEVO
                registro.respSp = respSp.nb_usuario + " (" + psreSp + ")";
                registro.etapa = sp.c_etapa != null ? sp.c_etapa.nb_etapa : "N/A";
                registro.sub_Etapa = sp.c_sub_etapa != null ? sp.c_sub_etapa.nb_sub_etapa : "N/A";
                registro.areas_involucradas = sp.ds_areas_involucradas;
                registro.aplicaciones_relacionadas = sp.ds_aplicaciones_relacionadas;
                registro.clave_manual = sp.cl_manual;
                foreach (var ln in sp.c_linea_negocio)
                {
                    registro.lineas_negocio += ln.nb_linea_negocio + "\n";
                }

                //Para los datos del riesgo se diferenciará entre MP y MG
                bool es_mp = mp.cl_macro_proceso.Substring(0, 2) == "MP";

                //Datos del Riesgo y Control

                var categoriaRiesgo = "N/A";
                var tipoRiesgo = "N/A";
                var claseTipologiaRiesgo = "N/A";
                var subClaseTipologiaRiesgo = "N/A";
                var tipologiaRiesgo = "N/A";
                var tipoImpacto = "N/A";
                var magnitudImpacto = "N/A";
                var probabilidadOcurrencia = "N/A";
                var criticidad = "N/A";

                var naturalezaControl = "N/A";
                var frecuenciaControl = "N/A";
                var categoriaControl = "N/A";
                var tipologiaControl = "N/A";
                var tipoEvidencia = "N/A";
                var gradoCobertura = "N/A";

                var esRiesgoOperativo = "N/A";
                var productoRO = "N/A";
                var subTipoRO = "N/A";
                var procesoRO = "N/A";
                var lineaNegocioRO = "N/A";
                var frecuenciaRO = "N/A";
                var impactoRO = "N/A";
                var criticidadRO = "N/A";

                try { categoriaRiesgo = es_mp ? riesgo.c_tipo_riesgo.c_categoria_riesgo.nb_categoria_riesgo : "N/A"; } catch { }
                try { tipoRiesgo = es_mp ? riesgo.c_tipo_riesgo.nb_tipo_riesgo : "N/A"; } catch { }
                try { claseTipologiaRiesgo = es_mp ? riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.c_clase_tipologia_riesgo.nb_clase_tipologia_riesgo : "N/A"; } catch { }
                try { subClaseTipologiaRiesgo = es_mp ? riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.nb_sub_clase_tipologia_riesgo : "N/A"; } catch { }
                try { tipologiaRiesgo = es_mp ? riesgo.c_tipologia_riesgo.nb_tipologia_riesgo : "N/A"; } catch { }
                try { tipoImpacto = es_mp ? riesgo.c_tipo_impacto.nb_tipo_impacto : "N/A"; } catch { }
                try { magnitudImpacto = es_mp ? riesgo.c_magnitud_impacto.nb_magnitud_impacto : "N/A"; } catch { }
                try { probabilidadOcurrencia = es_mp ? riesgo.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia : "N/A"; } catch { }
                try { criticidad = es_mp ? criticidades.Where(c => c.id_probabilidad_ocurrencia == riesgo.id_probabilidad_ocurrencia && c.id_magnitud_impacto == riesgo.id_magnitud_impacto).First().c_criticidad_riesgo.nb_criticidad_riesgo : "N/A"; } catch { }
                
                try { esRiesgoOperativo = es_mp ? (riesgo.es_riesgo_operativo ? Strings.getMSG("UsuarioIndex004") : Strings.getMSG("UsuarioIndex005")) : "N/A"; } catch { }
                if (riesgo.es_riesgo_operativo)
                {
                    try { productoRO = es_mp ? riesgo.c_sub_tipo_producto_riesgo_operacional.nb_sub_tipo_producto_riesgo_operacional : "N/A"; } catch { }
                    try { subTipoRO = es_mp ? riesgo.c_sub_tipo_riesgo_operacional.nb_sub_tipo_riesgo_operacional : "N/A"; } catch { Debug.WriteLine(riesgo.nb_riesgo + " No tiene sub tipo de producto."); }
                    try { procesoRO = es_mp ? riesgo.c_proceso_riesgo_operacional.nb_proceso_riesgo_operacional : "N/A"; } catch { }
                    try { lineaNegocioRO = es_mp ? riesgo.c_linea_negocio_riesgo_operacional.nb_linea_negocio_riesgo_operacional : "N/A"; } catch { }
                    try { frecuenciaRO = es_mp ? riesgo.c_frecuencia_riesgo_operacional.nb_frecuencia_riesgo_operacional : "N/A"; } catch { }
                    try { impactoRO = es_mp ? riesgo.c_impacto_riesgo_operacional.nb_impacto_riesgo_operacional : "N/A"; } catch { }
                    try { criticidadRO = es_mp ? Utilidades.Utilidades.Criticidad(riesgo.id_frecuencia_riesgo_operacional ?? 0, riesgo.id_impacto_riesgo_operacional ?? 0) : "N/A"; } catch { }
                }
                


                bool acc = item.tiene_accion_correctora;

                try { naturalezaControl = item.c_naturaleza_control.nb_naturaleza_control; } catch { }
                try { frecuenciaControl = item.c_frecuencia_control.nb_frecuencia_control; } catch { }
                try { categoriaControl = acc || !es_mp ? "N/A" : item.c_categoria_control.nb_categoria_control; } catch { }
                try { tipologiaControl = acc || !es_mp ? "N/A" : item.c_tipologia_control.nb_tipologia_control; } catch { }
                try { tipoEvidencia = acc || !es_mp ? "N/A" : item.c_tipo_evidencia.nb_tipo_evidencia; } catch { }
                try { gradoCobertura = acc || !es_mp ? "N/A" : item.c_grado_cobertura.nb_grado_cobertura; } catch { }


                registro.cl_riesgo = riesgo.nb_riesgo;
                registro.evento_riesgo = riesgo.evento;
                registro.categoria_riesgo = categoriaRiesgo;
                registro.tipo_riesgo = tipoRiesgo;
                registro.clase_tipologia_riesgo = claseTipologiaRiesgo;
                registro.sub_clase_tipologia_riesgo = subClaseTipologiaRiesgo;
                registro.tipologia_riesgo = tipologiaRiesgo;
                registro.tipo_impacto = tipoImpacto;
                registro.magnitud_impacto = magnitudImpacto;
                registro.probabilidad_ocurrencia = probabilidadOcurrencia;
                registro.criticidad = criticidad;
                registro.afectacion_contable = es_mp ? (riesgo.tiene_afectacion_contable ? Strings.getMSG("UsuarioIndex004") : Strings.getMSG("UsuarioIndex005")) : "N/A";
                registro.supuesto_normativo = es_mp ? riesgo.supuesto_normativo : "N/A";
                registro.euc = es_mp ? riesgo.euc : "N/A";


                registro.es_riesgo_operativo = esRiesgoOperativo;
                registro.nb_proceso_ro = procesoRO;
                registro.nb_producto_ro = productoRO;
                registro.nb_sub_tipo_ro = subTipoRO;
                registro.nb_linea_negocio_ro = lineaNegocioRO;
                registro.nb_frecuencia_ro = frecuenciaRO;
                registro.nb_impacto_ro = impactoRO;
                registro.criticidad_ro = criticidadRO;

                registro.responsable_control = item.c_usuario1.nb_usuario;
                registro.cl_control = acc ? "N/A" : item.relacion_control;
                registro.actividad_control = acc ? "N/A" : item.actividad_control;
                registro.evidencia_control = acc ? "N/A" : item.evidencia_control;
                registro.aplicacion = item.nb_aplicacion;
                registro.naturaleza_control = naturalezaControl;
                registro.frecuencia_control = frecuenciaControl;
                registro.accion_correctora = acc ? item.accion_correctora : "N/A";
                registro.categoria_control = categoriaControl;
                registro.tipologia_control = tipologiaControl;
                registro.tipo_evidencia = tipoEvidencia;
                registro.grado_cobertura = gradoCobertura;
                registro.ejecutor_control = acc || !es_mp ? "N/A" : item.c_usuario.nb_usuario;
                registro.control_clave = acc || !es_mp ? "N/A" : item.es_control_clave ? Strings.getMSG("UsuarioIndex004") : Strings.getMSG("UsuarioIndex005");

                #region Copiar Campos Extra
                for (int i = 0; i < 20; i++)
                {
                    var ceSp = props_sp.Where(p => p.Name == string.Format("campo{0:00}", i + 1)).First();
                    var ceR = props_r.Where(p => p.Name == string.Format("campo{0:00}", i + 1)).First();
                    var ceC = props_c.Where(p => p.Name == string.Format("campo{0:00}", i + 1)).First();

                    var ceRegSp = props_reg.Where(p => p.Name == string.Format("campo_extra_sp{0:00}", i + 1)).First();
                    var ceRegR = props_reg.Where(p => p.Name == string.Format("campo_extra_r{0:00}", i + 1)).First();
                    var ceRegC = props_reg.Where(p => p.Name == string.Format("campo_extra_c{0:00}", i + 1)).First();

                    ceRegSp.SetValue(registro, ceSp.GetValue(sp, null), null);
                    ceRegR.SetValue(registro, ceR.GetValue(riesgo, null), null);
                    ceRegC.SetValue(registro, ceC.GetValue(item, null), null);
                }
                #endregion

                model.Add(registro);
            }

            return View(model);
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