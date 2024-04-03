using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class UtilidadesController : Controller
    {
        private SICIEntities db = new SICIEntities();



        public bool SetPendingListFrequency(string freq, string label)
        {
            return Utilidades.Utilidades.SetPendingListFrequency(freq, label);
        }

        #region Regresar
        public ActionResult Return(int catalogo, int? id)
        {

            var redirect = false;

            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este catalogo
            if (ns == 0)
            {
                redirect = true;
            }
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
                    redirect = true;
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


            if (redirect)
            {
                switch (catalogo)
                {
                    case 1:
                        return RedirectToAction("Index", "Entidad", null);
                    case 2:
                        return RedirectToAction("Index", "MacroProceso", null);
                    case 3:
                        return RedirectToAction("Index", "Proceso", null);
                    case 4:
                        return RedirectToAction("Index", "SubProceso", null);
                    case 5:
                        return RedirectToAction("Index", "Area", null);
                    case 6:
                        return RedirectToAction("Index", "LineaNegocio", null);
                    case 7:
                        return RedirectToAction("Index", "Etapa", null);
                    case 8:
                        return RedirectToAction("Index", "SubEtapa", null);
                    case 9:
                        return RedirectToAction("Index", "CuentaContable", null);
                    case 10:
                        return RedirectToAction("Index", "CentroCosto", null);
                    case 11:
                        //return RedirectToAction("Index", "OrigenIncidencia", null);
                        return RedirectToAction("Denied", "Error", null);
                    case 12:
                        return RedirectToAction("Index", "Indicador", null);
                    case 13:
                        return RedirectToAction("Index", "TipoEvaluacion", null);
                    case 14:
                        return RedirectToAction("Index", "ActividadCosteo", null);
                    case 15:
                        return RedirectToAction("Index", "TipologiaSubProceso", null);
                    case 16:
                        return RedirectToAction("Index", "PeriodoIndicador", null);
                    case 17:
                        return RedirectToAction("Index", "PeriodoCertificacion", null);
                    case 18:
                        return RedirectToAction("Index", "Normatividad", null);
                    case 19:
                        return RedirectToAction("Index", "CategoriaRiesgo", null);
                    case 20:
                        return RedirectToAction("Index", "TipoRiesgo", null);
                    case 21:
                        return RedirectToAction("Index", "ClaseTipologiaRiesgo", null);
                    case 22:
                        return RedirectToAction("Index", "SubClaseTipologiaRiesgo", null);
                    case 23:
                        return RedirectToAction("Index", "TipologiaRiesgo", null);
                    case 24:
                        return RedirectToAction("Index", "ProbabilidadOcurrencia", null);
                    case 25:
                        return RedirectToAction("Index", "TipoImpacto", null);
                    case 26:
                        return RedirectToAction("Index", "MagnitudImpacto", null);
                    case 27:
                        return RedirectToAction("Index", "GradoCobertura", null);
                    case 28:
                        return RedirectToAction("Index", "FrecuenciaControl", null);
                    case 29:
                        return RedirectToAction("Index", "NaturalezaControl", null);
                    case 30:
                        return RedirectToAction("Index", "TipologiaControl", null);
                    case 31:
                        return RedirectToAction("Index", "CategoriaControl", null);
                    case 32:
                        return RedirectToAction("Index", "TipoEvidencia", null);
                    case 33:
                        return RedirectToAction("Index", "EstatusBDEI", null);
                    case 34:
                        //return RedirectToAction("Index", "CategoriaPerdida", null);
                        return RedirectToAction("Denied", "Error", null);
                    case 35:
                        //return RedirectToAction("Index", "CategoriaError", null);
                        return RedirectToAction("Denied", "Error", null);
                    case 36:
                        return RedirectToAction("Index", "Moneda", null);
                    case 37:
                        return RedirectToAction("Index", "TipoSolucion", null);
                    case 38:
                        return RedirectToAction("Index", "Actividad", null);
                    case 39:
                        return RedirectToAction("Index", "ProcesoBenchmark", null);
                    case 40:
                        return RedirectToAction("Index", "SubProcesoBenchmark", null);
                    case 41:
                        return RedirectToAction("Index", "EventoRiesgo", null);
                    case 42:
                        return RedirectToAction("Index", "UnidadIndicador", null);
                    case 43:
                        return RedirectToAction("Index", "Usuario", null);
                    case 44:
                        return RedirectToAction("Riesgos", "Riesgo", new { id = (int)id });
                    case 45:
                        return RedirectToAction("Controles", "Riesgo", new { id = (int)id });
                    case 46:
                        return RedirectToAction("Editar", "Riesgo", new { id = (int)id });
                    case 47:
                        return RedirectToAction("Certificados", "Certificacion", new { id = (int)id });
                    case 48:
                        return RedirectToAction("Index", "BDEI", null);
                    case 49:
                        return RedirectToAction("Evaluaciones", "EvaluacionIndicador", new { id = (int)id });
                    case 50:
                        return RedirectToAction("Index", "Benchmark", null);
                    case 51:
                        return RedirectToAction("Index", "OrigenAutoridad", null);
                    case 52:
                        return RedirectToAction("Index", "ClasificacionIncidencia", null);
                    case 53:
                        return RedirectToAction("Index", "DescripcionEvento", null);
                    case 54:
                        return RedirectToAction("Index", "TipoRiesgoOperacional", null);
                    case 55:
                        return RedirectToAction("Index", "SubTipoRiesgoOperacional", null);
                    case 56:
                        return RedirectToAction("Index", "ClaseEventoRiesgoOperacional", null);
                    case 57:
                        return RedirectToAction("Index", "AmbitoRiesgoOperacional", null);
                    case 58:
                        return RedirectToAction("Index", "ProcesoRiesgoOperacional", null);
                    case 59:
                        return RedirectToAction("Index", "ProductoRiesgoOperacional", null);
                    case 60:
                        return RedirectToAction("Index", "SubTipoProductoRiesgoOperacional", null);
                    case 61:
                        return RedirectToAction("Index", "CanalRiesgoOperacional", null);
                    case 62:
                        return RedirectToAction("Index", "CategoriaLineaNegocioRiesgoOperacional", null);
                    case 63:
                        return RedirectToAction("Index", "LineaNegocioRiesgoOperacional", null);
                    case 64:
                        return RedirectToAction("Index", "FrecuenciaRiesgoOperacional", null);
                    case 65:
                        return RedirectToAction("Index", "ImpactoRiesgoOperacional", null);
                    case 66:
                        return RedirectToAction("IndexSolicitanteAuditoria", "Auditoria", null);
                    case 67:
                        return RedirectToAction("IndexPeriodoAuditoria", "Auditoria", null);
                    case 68:
                        return RedirectToAction("IndexRatingAuditoria", "Auditoria", null);
                    case 69:
                        return RedirectToAction("IndexCampoAuditoria", "Auditoria", null);
                    case 70:
                        return RedirectToAction("IndexCampoPrograma", "Auditoria", null);
                    case 71:
                        return RedirectToAction("IndexUniversoAuditable", "Auditoria", null);
                    case 72:
                        return RedirectToAction("IndexPlaneacion", "ProcesosAuditoria", null);
                    case 73:
                        return RedirectToAction("IndexPruebaAuditoria", "Auditoria", null);
                    case 74:
                        return RedirectToAction("Index", "RiesgoAsociadoBDEI", null);
                    case 75:
                        return RedirectToAction("Index", "MinimoRiesgoOperativoBDEI", null);
                    case 76:
                        return RedirectToAction("Index", "CausaBDEI", null);
                    case 77:
                        return RedirectToAction("Index", "CatalogoConceptoBDEI", null);
                    case 78:
                        return RedirectToAction("Certificados", "CertificacionEntidad", new { id = (int)id });
                    case 79:
                        return RedirectToAction("Certificados", "CertificacionMacroProceso", new { id = (int)id });
                    case 80:
                        return RedirectToAction("Certificados", "CertificacionProceso", new { id = (int)id });
                    case 81:
                        return RedirectToAction("Certificados", "CertificacionSubProceso", new { id = (int)id });
                    default:
                        return RedirectToAction("Index", "Home", null);
                }
            }
            return null;
        }
        #endregion

        #region Respaldo de Reportes
        public string BackUpActions()
        {
            #region Declaración de Variables

            List<JObject> Reports = new List<JObject>();

            Report rep1 = new Report();                 //Reporte de Accesos
            Report rep2 = new Report();                 //Reporte de Benchmark
            Report rep3_1 = new Report();               //Reporte de CambiosMRyC Riesgos
            Report rep3_2 = new Report();               //Reporte de CambiosMRyC Controles
            Report rep4 = new Report();                 //Reporte de Certificación
            Report rep5 = new Report();                 //Reporte de Costeo
            Report rep6 = new Report();                 //Reporte de Estructura
            Report rep7 = new Report();                 //Reporte de Fallas
            Report rep8 = new Report();                 //Reporte de Flujos y Narrativas
            Report rep9 = new Report();                 //Reporte de BDEI
            Report rep10 = new Report();                //Reporte de Indicador
            Report rep11_1 = new Report();              //Reporte de Incidencias y Planes (Oficios)
            Report rep11_2 = new Report();              //Reporte de Incidencias y Planes (Auditoría Externa)
            Report rep11_3 = new Report();              //Reporte de Incidencias y Planes (Auditoría Interna)
            Report rep11_4 = new Report();              //Reporte de Incidencias y Planes (Certificación)
            Report rep11_5 = new Report();              //Reporte de Incidencias y Planes (MRyC)
            Report rep11_6 = new Report();              //Reporte de Incidencias y Planes (Otros)
            Report rep12_1 = new Report();              //Reporte de Planes Rem Pendientes
            Report rep12_2 = new Report();              //Reporte de Planes Rem Concluidos
            Report rep13_1 = new Report();              //Reporte de Riesgo Residual (Existente)
            Report rep13_2 = new Report();              //Reporte de Riesgo Residual (Inexistente)
            Report rep14_1 = new Report();                //Reporte de MRyC
            Report rep14_2 = new Report();                //Reporte de MRyC (Riesgos Eliminados)
            List<Report> rep15 = new List<Report>();    //Reportes de Sub Procesos - Normatividad
            Report rep16 = new Report();                //Reporte de Sub Actividades de costeo
            Report rep17 = new Report();                //Reporte de Personal
            Report rep18 = new Report();                //Reporte de Fichas
            Report rep19 = new Report();                //Reporte de Indicadores Diarios
            Report rep20_1 = new Report();                //Reporte de Revisión de Controles Rep Nro 28
            Report rep20_2 = new Report();                //Reporte de Controles no Revisados Rep Nro 29
            Report rep21 = new Report();                //Reporte de Certificación de entidades
            Report rep22 = new Report();                //Reporte de Certificación de macro procesos
            Report rep23 = new Report();                //Reporte de Certificación de procesos
            Report rep24 = new Report();                //Reporte de Certificación de sub procesos


            int totalReportes = 33 + db.c_normatividad.Count();
            int repCounter = 0;
            int percentaje = 0;

            List<string> colNames = new List<string>();
            List<string> row = new List<string>();


            var CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);
            var CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            var CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            #endregion

            #region Rep Accesos     
            percentaje = (repCounter++ * 100) / totalReportes;

            JObject rep1Json = new JObject();
            try
            {
                Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), Strings.getMSG("UsuarioCreate065"));
                var today = DateTime.Now.AddDays(-31);
                var repAccesos = db.h_acceso.Where(r => r.fe_acceso > today).OrderBy(a => a.c_usuario.nb_usuario).ToList();
                colNames.Add("Usuario");
                colNames.Add("Función");
                colNames.Add("Fecha");

                rep1.RepName = "Reporte de Accesos";

                rep1.ColNames = colNames;
                rep1.NoCols = 3;
                foreach (var entrada in repAccesos)
                {
                    row = new List<string>();
                    row.Add(entrada.c_usuario.nb_usuario);
                    row.Add(entrada.nb_funcion);
                    row.Add(entrada.fe_acceso.ToShortDateString());

                    rep1.Rows.Add(row);
                }

                rep1Json = (JObject)JToken.FromObject(rep1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, Strings.getMSG("UsuarioCreate066"));
            }




            #endregion

            #region Rep Benchmark   
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), Strings.getMSG("UsuarioCreate067"));

            JObject rep2Json = new JObject();
            try
            {
                var repBenchmark = db.k_benchmarck.ToList();

                colNames = new List<string>();
                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("ResponsableProceso");
                colNames.Add("Sub Proceso");
                colNames.Add("ResponsableSub Proceso");
                colNames.Add("Macro Proceso Benchmark");
                colNames.Add("Proceso Benchmark");
                colNames.Add("Sub Proceso Benchmark");
                colNames.Add("Evento de Riesgo");

                rep2.RepName = "Reporte de Benchmark";

                rep2.ColNames = colNames;
                rep2.NoCols = colNames.Count;
                foreach (var item in repBenchmark)
                {
                    var sp = item.c_sub_proceso;
                    var pr = sp.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var en = mp.c_entidad;
                    var respPr = pr.c_usuario;
                    var respMp = mp.c_usuario;
                    var respEn = en.c_usuario;
                    var respSp = sp.c_usuario;
                    var evB = item.c_evento_riesgo;
                    var spB = evB.c_sub_proceso_benchmark;
                    var prB = spB.c_proceso_benchmark;
                    var mpB = prB.c_actividad;



                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(respEn.nb_usuario);
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(respMp.nb_usuario);
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(respPr.nb_usuario);
                    row.Add(sp.cl_sub_proceso + " - " + sp.nb_sub_proceso);
                    row.Add(respSp.nb_usuario);
                    row.Add(mpB.cl_actividad + " - " + mp.nb_macro_proceso);
                    row.Add(prB.cl_proceso_benchmark + " - " + prB.nb_proceso_benchmark);
                    row.Add(spB.cl_sub_proceso_benchmark + " - " + spB.nb_sub_proceso_benchmark);
                    row.Add(evB.cl_evento_riesgo + " - " + evB.nb_evento_riesgo);
                    rep2.Rows.Add(row);
                }

                rep2Json = (JObject)JToken.FromObject(rep2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de benchmark");
            }
            #endregion

            #region Rep Cambios MRyC
            var rep3riesgos = db.k_riesgo.Where(r => r.r_riesgo.Count() > 0).ToList();
            var rep3controlesT = db.k_control.Where(c => c.r_control.Count() > 0).ToList();
            List<k_control> rep3controles = Utilidades.Utilidades.GetLinkedControls(rep3controlesT);



            Type m_tipo = null;
            PropertyInfo[] m_propiedades = null;
            PropertyInfo[] m_propiedades_a = null;

            #region Riesgos
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Cambios en los Riesgos de la MRyC");

            JObject rep3_1Json = new JObject();
            try
            {
                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Código de Riesgo");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Categoría de Riesgo");
                colNames.Add("Tipo de Riesgo");
                colNames.Add("Clase de Tipología de Riesgo");
                colNames.Add("Sub Clase de Tipología de Riesgo");
                colNames.Add("Tipología de Riesgo");
                colNames.Add("Tipo de Impacto");
                colNames.Add("Magnitud de Impacto");
                colNames.Add("Probabilidad de Ocurrencia");
                colNames.Add("Criticidad");
                colNames.Add("Afectación Contable");
                colNames.Add("Supuesto Normativo");
                colNames.Add("EUC");

                //agregar dinamicamente los nombres de los campos extra
                rep3_1.RepName = "Reporte Cambios Riesgos";

                foreach (var ce in CamposExtraRiesgo)
                {
                    if (ce.es_visible)
                    {
                        colNames.Add(ce.nb_campo);
                    }
                }
                colNames.Add("Fecha de Modificación");

                rep3_1.ColNames = colNames;
                rep3_1.NoCols = colNames.Count;

                foreach (var riesgo in rep3riesgos)
                {
                    bool MG = riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) == "MG";
                    var registros = riesgo.r_riesgo.ToList().OrderByDescending(ri => ri.fe_modificacion);
                    var riesgoA = registros.First();
                    int count = registros.Count();

                    row = new List<string>();
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad + " - " + riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.nb_entidad);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + riesgo.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.c_sub_proceso.c_proceso.cl_proceso + " - " + riesgo.c_sub_proceso.c_proceso.nb_proceso);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.c_sub_proceso.cl_sub_proceso + " - " + riesgo.c_sub_proceso.nb_sub_proceso);
                    row.Add(riesgo.c_sub_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.nb_riesgo + (riesgo.nb_riesgo == riesgoA.nb_riesgo ? "" : " (cambió)"));
                    row.Add(riesgo.evento + (riesgo.evento == riesgoA.evento ? "" : " (cambió)"));
                    if (!MG)
                    {
                        row.Add(riesgo.c_tipo_riesgo.c_categoria_riesgo.cl_categoria_riesgo + " - " + riesgo.c_tipo_riesgo.c_categoria_riesgo.nb_categoria_riesgo + (riesgo.c_tipo_riesgo.c_categoria_riesgo == riesgoA.c_tipo_riesgo.c_categoria_riesgo ? "" : " (Cambió)"));
                        row.Add(riesgo.c_tipo_riesgo.cl_tipo_riesgo + " - " + riesgo.c_tipo_riesgo.nb_tipo_riesgo + (riesgo.c_tipo_riesgo == riesgoA.c_tipo_riesgo ? "" : "( Cambió)"));
                        row.Add(riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.c_clase_tipologia_riesgo.cl_clase_tipologia_riesgo + " - " + riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.c_clase_tipologia_riesgo.nb_clase_tipologia_riesgo + (riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo == riesgoA.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo ? "" : " (Cambió)"));
                        row.Add(riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.cl_sub_clase_tipologia_riesgo + " - " + riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.nb_sub_clase_tipologia_riesgo + (riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo == riesgoA.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo ? "" : " (Cambió)"));
                        row.Add(riesgo.c_tipologia_riesgo.cl_tipologia_riesgo + " - " + riesgo.c_tipologia_riesgo.nb_tipologia_riesgo + (riesgo.c_tipologia_riesgo == riesgoA.c_tipologia_riesgo ? "" : " (Cambió)"));
                        row.Add(riesgo.c_tipo_impacto.cl_tipo_impacto + " - " + riesgo.c_tipo_impacto.nb_tipo_impacto + (riesgo.c_tipo_impacto == riesgoA.c_tipo_impacto ? "" : " (Cambió)"));
                        row.Add(riesgo.c_magnitud_impacto.cl_magnitud_impacto + " - " + riesgo.c_magnitud_impacto.nb_magnitud_impacto + (riesgo.c_magnitud_impacto == riesgoA.c_magnitud_impacto ? "" : " (Cambió)"));
                        row.Add(riesgo.c_probabilidad_ocurrencia.cl_probabilidad_ocurrencia + " - " + riesgo.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia + (riesgo.c_probabilidad_ocurrencia == riesgoA.c_probabilidad_ocurrencia ? "" : " (Cambió)"));
                        row.Add(riesgo.criticidad + (riesgo.criticidad == riesgoA.criticidad ? "" : " (Cambió)"));
                        row.Add((riesgo.tiene_afectacion_contable ? "Si" : "No") + (riesgo.tiene_afectacion_contable == riesgoA.tiene_afectacion_contable ? "" : " (Cambió)"));
                        row.Add(riesgo.supuesto_normativo + (riesgo.supuesto_normativo == riesgoA.supuesto_normativo ? "" : " (Cambió)"));
                        row.Add(riesgo.euc + (riesgo.euc == riesgoA.euc ? "" : " (Cambió)"));
                    }
                    else
                    {
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                    }
                    //campos extra riesgo
                    m_tipo = riesgo.GetType();
                    m_propiedades = m_tipo.GetProperties();
                    m_tipo = riesgoA.GetType();
                    m_propiedades_a = m_tipo.GetProperties();
                    for (int i = 0; i < 20; i++)
                    {
                        var prop = m_propiedades.Where(m => m.Name == string.Format("campo{0:00}", i + 1)).First();
                        var propA = m_propiedades_a.Where(m => m.Name == string.Format("campo{0:00}", i + 1)).First();
                        var ce = CamposExtraRiesgo[i];
                        var v1 = "";
                        var v2 = "";
                        try
                        {
                            v1 = (prop.GetValue(riesgo, null)??"").ToString();
                        }
                        catch
                        {
                            v1 = "";
                        }
                        try
                        {
                            v2 = (propA.GetValue(riesgoA, null) ?? "").ToString();
                        }
                        catch
                        {
                            v2 = "";
                        }
                        if (MG)
                        {
                            if (ce.es_visible && ce.aparece_en_mg)
                            {
                                row.Add(v1 + (v1 == v2 ? "" : "( Cambió)"));
                            }
                            else if (ce.es_visible)
                            {
                                row.Add("N/A");
                            }
                        }
                        else
                        {
                            if (ce.es_visible)
                            {
                                row.Add(v1 + (v1 == v2 ? "" : "( Cambió)"));
                            }
                        }
                    }
                    row.Add("N/A");
                    rep3_1.Rows.Add(row);

                    //Agregar Fila con información del registro del historíal
                    row = new List<string>();

                    var registro = registros.ElementAt(0);
                    r_riesgo registroA;
                    registroA = registro;

                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad + " - " + riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.nb_entidad);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + riesgo.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.c_sub_proceso.c_proceso.cl_proceso + " - " + riesgo.c_sub_proceso.c_proceso.nb_proceso);
                    row.Add(riesgo.c_sub_proceso.c_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.c_sub_proceso.cl_sub_proceso + " - " + riesgo.c_sub_proceso.nb_sub_proceso);
                    row.Add(riesgo.c_sub_proceso.c_usuario.nb_usuario);
                    row.Add(registro.nb_riesgo);
                    row.Add(registro.evento);

                    if (!MG)
                    {
                        row.Add(registro.c_tipo_riesgo.c_categoria_riesgo.cl_categoria_riesgo + " - " + registro.c_tipo_riesgo.c_categoria_riesgo.nb_categoria_riesgo);
                        row.Add(registro.c_tipo_riesgo.cl_tipo_riesgo + " - " + registro.c_tipo_riesgo.nb_tipo_riesgo);
                        row.Add(registro.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.c_clase_tipologia_riesgo.cl_clase_tipologia_riesgo + " - " + registro.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.c_clase_tipologia_riesgo.nb_clase_tipologia_riesgo);
                        row.Add(registro.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.cl_sub_clase_tipologia_riesgo + " - " + registro.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.nb_sub_clase_tipologia_riesgo);
                        row.Add(registro.c_tipologia_riesgo.cl_tipologia_riesgo + " - " + registro.c_tipologia_riesgo.nb_tipologia_riesgo);
                        row.Add(registro.c_tipo_impacto.cl_tipo_impacto + " - " + registro.c_tipo_impacto.nb_tipo_impacto);
                        row.Add(registro.c_magnitud_impacto.cl_magnitud_impacto + " - " + registro.c_magnitud_impacto.nb_magnitud_impacto);
                        row.Add(registro.c_probabilidad_ocurrencia.cl_probabilidad_ocurrencia + " - " + registro.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia);
                        row.Add(registro.criticidad);
                        row.Add((registro.tiene_afectacion_contable ? "Si" : "No"));
                        row.Add(registro.supuesto_normativo);
                        row.Add(registro.euc);
                    }
                    else
                    {
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                    }

                    m_tipo = registro.GetType();
                    m_propiedades = m_tipo.GetProperties();
                    m_tipo = registroA.GetType();
                    m_propiedades_a = m_tipo.GetProperties();
                    for (int j = 0; j < 20; j++)
                    {
                        var prop = m_propiedades.Where(m => m.Name == string.Format("campo{0:00}", j + 1)).First();
                        var propA = m_propiedades_a.Where(m => m.Name == string.Format("campo{0:00}", j + 1)).First();
                        var ce = CamposExtraRiesgo[j];
                        var v1 = "";
                        var v2 = "";
                        try
                        {
                            v1 = (prop.GetValue(registro, null)??"").ToString();
                        }
                        catch
                        {
                            v1 = "";
                        }
                        try
                        {
                            v2 = (prop.GetValue(registroA, null)??"").ToString();
                        }
                        catch
                        {
                            v2 = "";
                        }
                        if (MG)
                        {
                            if (ce.es_visible && ce.aparece_en_mg)
                            {
                                row.Add(v1);
                            }
                            else if (ce.es_visible)
                            {
                                row.Add("N/A");
                            }
                        }
                        else
                        {
                            if (ce.es_visible)
                            {
                                row.Add(v1);
                            }
                        }
                    }
                    row.Add(registro.fe_modificacion.ToString());
                    rep3_1.Rows.Add(row);

                }
                rep3_1Json = (JObject)JToken.FromObject(rep3_1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de riesgos");
            }


            #endregion

            #region Controles
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Cambios en los Controles de la MRyC");

            JObject rep3_2Json = new JObject();
            try
            {
                colNames = new List<string>();

                rep3_2.RepName = "Reporte Cambios Controles";

                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Código de Riesgo");
                colNames.Add("Código de Control");
                colNames.Add("Actividad de Control/Acción Correctora");
                colNames.Add("Evidencia del Control");
                colNames.Add("Ejecutor del Control");
                colNames.Add("Responsable del Control");
                colNames.Add("Es Control Clave");
                colNames.Add("Grado de Cobertura");
                colNames.Add("Frecuencia del Control");
                colNames.Add("Naturaleza del Control");
                colNames.Add("Nombre de la Aplicación");
                colNames.Add("Tipología del Control");
                colNames.Add("Categoría del Control");
                colNames.Add("Tipo de Evidencia");

                //agregar dinamicamente los nombres de los campos extra
                foreach (var ce in CamposExtraControl)
                {
                    if (ce.es_visible)
                    {
                        colNames.Add(ce.nb_campo);
                    }
                }
                colNames.Add("Fecha de Modificación");

                rep3_2.ColNames = colNames;
                rep3_2.NoCols = colNames.Count;

                foreach (var control in rep3controles)
                {
                    bool MG = control.k_riesgo.First().c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) == "MG";
                    bool AC = control.tiene_accion_correctora;
                    var registros = control.r_control.ToList().OrderByDescending(ri => ri.fe_modificacion);
                    var controlA = registros.First();
                    var riesgo = control.k_riesgo.First();
                    int count = registros.Count();

                    row = new List<string>();
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad + " - " + control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.nb_entidad);
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario);
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + control.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso);
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.c_usuario.nb_usuario);
                    row.Add(control.c_sub_proceso.c_proceso.cl_proceso + " - " + control.c_sub_proceso.c_proceso.nb_proceso);
                    row.Add(control.c_sub_proceso.c_proceso.c_usuario.nb_usuario);
                    row.Add(control.c_sub_proceso.cl_sub_proceso + " - " + control.c_sub_proceso.nb_sub_proceso);
                    row.Add(control.c_sub_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.nb_riesgo);
                    if (!AC)
                    {
                        row.Add(control.relacion_control + (control.relacion_control == controlA.relacion_control ? "" : " (Cambió)"));
                        row.Add(control.actividad_control + (control.actividad_control == controlA.actividad_control ? "" : " (Cambió)"));
                        row.Add(control.evidencia_control + (control.evidencia_control == controlA.evidencia_control ? "" : " (Cambió)"));
                        if (!MG)
                        {
                            row.Add(control.c_usuario.nb_usuario + (control.c_usuario == controlA.c_usuario ? "" : " (Cambió)"));
                        }
                        else
                        {
                            row.Add("N/A");
                        }
                    }
                    else
                    {
                        row.Add("N/A");
                        row.Add("Acción Correctora: " + control.accion_correctora + (control.accion_correctora == controlA.accion_correctora ? "" : " (Cambió)"));
                        row.Add("N/A");
                        row.Add("N/A");
                    }
                    row.Add(control.c_usuario1.nb_usuario + (control.c_usuario1 == controlA.c_usuario1 ? "" : " (Cambió)"));
                    if (!AC)
                    {
                        if (!MG)
                        {
                            row.Add((control.es_control_clave ? "Si" : "No") + (control.es_control_clave == controlA.es_control_clave ? "" : " (Cambió)"));
                            row.Add(control.c_grado_cobertura.cl_grado_cobertura + " - " + control.c_grado_cobertura.nb_grado_cobertura + (control.c_grado_cobertura == controlA.c_grado_cobertura ? "" : " (Cambió)"));
                        }
                        else
                        {
                            row.Add("N/A");
                            row.Add("N/A");
                        }
                        row.Add(control.c_frecuencia_control.cl_frecuencia_control + " - " + control.c_frecuencia_control.nb_frecuencia_control + (control.c_frecuencia_control == controlA.c_frecuencia_control ? "" : " (Cambió)"));
                        row.Add(control.c_naturaleza_control.cl_naturaleza_control + " - " + control.c_naturaleza_control.nb_naturaleza_control + (control.c_naturaleza_control == controlA.c_naturaleza_control ? "" : " (Cambió)"));
                        if (!MG)
                        {
                            row.Add(control.nb_aplicacion + (control.nb_aplicacion == controlA.nb_aplicacion ? "" : " (Cambió)"));
                            row.Add(control.c_tipologia_control.cl_tipologia_control + " - " + control.c_tipologia_control.nb_tipologia_control + (control.c_tipologia_control == controlA.c_tipologia_control ? "" : " (Cambió)"));
                            row.Add(control.c_categoria_control.cl_categoria_control + " - " + control.c_categoria_control.nb_categoria_control + (control.c_categoria_control == controlA.c_categoria_control ? "" : " (Cambió)"));
                            row.Add(control.c_tipo_evidencia.cl_tipo_evidencia + " - " + control.c_tipo_evidencia.nb_tipo_evidencia + (control.c_tipo_evidencia == controlA.c_tipo_evidencia ? "" : " (Cambió)"));
                        }
                        else
                        {
                            row.Add("N/A");
                            row.Add("N/A");
                            row.Add("N/A");
                            row.Add("N/A");
                        }
                    }
                    else
                    {
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                    }


                    //campos extra control
                    m_tipo = control.GetType();
                    m_propiedades = m_tipo.GetProperties();
                    m_tipo = controlA.GetType();
                    m_propiedades_a = m_tipo.GetProperties();
                    for (int i = 0; i < 20; i++)
                    {
                        var prop = m_propiedades.Where(m => m.Name == string.Format("campo{0:00}", i + 1)).First();
                        var propA = m_propiedades_a.Where(m => m.Name == string.Format("campo{0:00}", i + 1)).First();
                        var ce = CamposExtraControl[i];
                        var v1 = "";
                        var v2 = "";
                        try
                        {
                            v1 = (prop.GetValue(control, null) ?? "").ToString();
                        }
                        catch
                        {
                            v1 = "";
                        }
                        try
                        {
                            v2 = (propA.GetValue(controlA, null) ?? "").ToString();
                        }
                        catch
                        {
                            v2 = "";
                        }
                        if (MG)
                        {
                            if (ce.es_visible && ce.aparece_en_mg)
                            {
                                row.Add(v1 + (v1 == v2 ? "" : " (Cambió)"));
                            }
                            else if (ce.es_visible)
                            {
                                row.Add("N/A");
                            }
                        }
                        else
                        {
                            if (ce.es_visible)
                            {
                                row.Add(v1 + (v1 == v2 ? "" : " (Cambió)"));
                            }
                        }
                    }
                    row.Add("N/A");
                    rep3_2.Rows.Add(row);

                    //Agregar Fila con información del registro del historíal
                    row = new List<string>();

                    var registro = registros.ElementAt(0);
                    r_control registroA;
                    AC = registro.tiene_accion_correctora;
                    registroA = registro;

                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad + " - " + control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.nb_entidad);
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario);
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + control.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso);
                    row.Add(control.c_sub_proceso.c_proceso.c_macro_proceso.c_usuario.nb_usuario);
                    row.Add(control.c_sub_proceso.c_proceso.cl_proceso + " - " + control.c_sub_proceso.c_proceso.nb_proceso);
                    row.Add(control.c_sub_proceso.c_proceso.c_usuario.nb_usuario);
                    row.Add(control.c_sub_proceso.cl_sub_proceso + " - " + control.c_sub_proceso.nb_sub_proceso);
                    row.Add(control.c_sub_proceso.c_usuario.nb_usuario);
                    row.Add(riesgo.nb_riesgo);
                    if (!AC)
                    {
                        row.Add(registro.relacion_control);
                        row.Add(registro.actividad_control);
                        row.Add(registro.evidencia_control);
                        if (!MG)
                        {
                            row.Add(registro.c_usuario.nb_usuario);
                        }
                        else
                        {
                            row.Add("N/A");
                        }
                    }
                    else
                    {
                        row.Add("N/A");
                        row.Add("Acción Correctora: " + registro.accion_correctora);
                        row.Add("N/A");
                        row.Add("N/A");
                    }
                    row.Add(registro.c_usuario1.nb_usuario);
                    if (!AC)
                    {
                        if (!MG)
                        {
                            row.Add((registro.es_control_clave ? "Si" : "No"));
                            row.Add(registro.c_grado_cobertura.cl_grado_cobertura + " - " + registro.c_grado_cobertura.nb_grado_cobertura);
                        }
                        else
                        {
                            row.Add("N/A");
                            row.Add("N/A");
                        }
                        row.Add(registro.c_frecuencia_control.cl_frecuencia_control + " - " + registro.c_frecuencia_control.nb_frecuencia_control);
                        row.Add(registro.c_naturaleza_control.cl_naturaleza_control + " - " + registro.c_naturaleza_control.nb_naturaleza_control);
                        if (!MG)
                        {
                            row.Add(registro.nb_aplicacion + (registro.nb_aplicacion == controlA.nb_aplicacion ? "" : " (Cambió)"));
                            row.Add(registro.c_tipologia_control.cl_tipologia_control + " - " + registro.c_tipologia_control.nb_tipologia_control);
                            row.Add(registro.c_categoria_control.cl_categoria_control + " - " + registro.c_categoria_control.nb_categoria_control);
                            row.Add(registro.c_tipo_evidencia.cl_tipo_evidencia + " - " + registro.c_tipo_evidencia.nb_tipo_evidencia);
                        }
                        else
                        {
                            row.Add("N/A");
                            row.Add("N/A");
                            row.Add("N/A");
                            row.Add("N/A");
                        }
                    }
                    else
                    {
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                        row.Add("N/A");
                    }


                    //campos extra control
                    m_tipo = registro.GetType();
                    m_propiedades = m_tipo.GetProperties();
                    m_tipo = registroA.GetType();
                    m_propiedades_a = m_tipo.GetProperties();
                    for (int i = 0; i < 20; i++)
                    {
                        var prop = m_propiedades.Where(m => m.Name == string.Format("campo{0:00}", i + 1)).First();
                        var propA = m_propiedades_a.Where(m => m.Name == string.Format("campo{0:00}", i + 1)).First();
                        var ce = CamposExtraControl[i];
                        var v1 = "";
                        var v2 = "";
                        try
                        {
                            v1 = (prop.GetValue(control, null) ?? "").ToString();
                        }
                        catch
                        {
                            v1 = "";
                        }
                        try
                        {
                            v2 = (propA.GetValue(controlA, null) ?? "").ToString();
                        }
                        catch
                        {
                            v2 = "";
                        }
                        if (MG)
                        {
                            if (ce.es_visible && ce.aparece_en_mg)
                            {
                                row.Add(v1);
                            }
                            else if (ce.es_visible)
                            {
                                row.Add("N/A");
                            }
                        }
                        else
                        {
                            if (ce.es_visible)
                            {
                                row.Add(v1);
                            }
                        }
                    }
                    row.Add(registro.fe_modificacion.ToString());
                    rep3_2.Rows.Add(row);

                }
                rep3_2Json = (JObject)JToken.FromObject(rep3_2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de controles");
            }


            #endregion

            #endregion

            #region Rep Certificacion 
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Certificación");

            JObject rep4Json = new JObject();
            try
            {
                var repCertificacion = db.k_certificacion_control.ToList();
                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Código de Riesgo");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Código de Control");
                colNames.Add("Actividad de Control/Acción Correctora");
                colNames.Add("Clave de la Certificación");
                colNames.Add("Periodo de Certificación");
                colNames.Add("Procedimiento de Certificación");
                colNames.Add("Tipo de Evaluación");
                colNames.Add("No de Pruebas Mínimo ");
                colNames.Add("No Semestre 1");
                colNames.Add("No Semestre 2");
                colNames.Add("No Pruebas Realizadas ");
                colNames.Add("¿Tiene Funcionamiento Efectivo?");
                colNames.Add("¿Tiene Diseño Efectivo?");
                colNames.Add("¿Tiene Archivos?");

                rep4.RepName = "Reporte de Certificación de Controles";

                rep4.ColNames = colNames;
                rep4.NoCols = colNames.Count;
                foreach (var item in repCertificacion)
                {
                    var control = item.k_control;
                    var riesgo = control.k_riesgo.First();
                    var sp = control.c_sub_proceso;
                    var pr = sp.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var en = mp.c_entidad;
                    var respPr = pr.c_usuario;
                    var respMp = mp.c_usuario;
                    var respEn = en.c_usuario;
                    var respSp = sp.c_usuario;

                    row = new List<string>();

                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(respEn.nb_usuario);
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(respMp.nb_usuario);
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(respPr.nb_usuario);
                    row.Add(sp.cl_sub_proceso + " - " + sp.nb_sub_proceso);
                    row.Add(respSp.nb_usuario);
                    row.Add(riesgo.nb_riesgo);
                    row.Add(riesgo.evento);
                    row.Add(control.relacion_control);
                    if (control.tiene_accion_correctora)
                    {
                        row.Add("<b>Accion Correctora: </b>" + control.accion_correctora);
                    }
                    else
                    {
                        row.Add(control.actividad_control);
                    }
                    row.Add(item.cl_certificacion_control);
                    row.Add(item.c_periodo_certificacion.nb_periodo_certificacion);
                    row.Add(item.ds_procedimiento_certificacion);
                    row.Add(item.c_tipo_evaluacion.nb_tipo_evaluacion);
                    row.Add(item.no_partidas_minimo.ToString());
                    row.Add(item.no_partidas_semestre1.ToString());
                    row.Add(item.no_partidas_semestre2.ToString());
                    row.Add(item.no_pruebas_realizadas.ToString());
                    row.Add(item.tiene_funcionamiento_efectivo ? "Si" : "No");
                    row.Add(item.tiene_disenio_efectivo ? "Si" : "No");
                    row.Add((item.nb_archivo_1 != null || item.nb_archivo_2 != null || item.nb_archivo_3 != null || item.nb_archivo_4 != null || item.nb_archivo_5 != null) ? "SI" : "No");

                    rep4.Rows.Add(row);
                }

                rep4Json = (JObject)JToken.FromObject(rep4);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de certificación de controles");
            }


            #endregion

            #region Rep Costeo
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Costeo");

            JObject rep5Json = new JObject();
            try
            {
                var repCosteo = new ActividadesCosteoViewModel();
                var actividades = db.c_area_costeo.ToList();
                var sub_procesos = db.c_sub_proceso.OrderBy(sp => sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad)
                    .OrderBy(sp => sp.c_proceso.c_macro_proceso.cl_macro_proceso)
                    .OrderBy(sp => sp.c_proceso.cl_proceso)
                    .OrderBy(sp => sp.cl_sub_proceso)
                    .ToList();

                //Establecer datos de las actividades de costeo
                foreach (var actividad in actividades)
                {
                    repCosteo.nb_actividades.Add(actividad.nb_area_costeo);
                }

                //llenar todos los atributos de los participantes
                //Para cada sub proceso, buscar sus participantes
                foreach (var sp in sub_procesos)
                {
                    var entidad = sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad + " - " + sp.c_proceso.c_macro_proceso.c_entidad.nb_entidad;
                    var respEn = sp.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario;
                    var macro_proceso = sp.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + sp.c_proceso.c_macro_proceso.nb_macro_proceso;
                    var respMp = sp.c_proceso.c_macro_proceso.c_usuario.nb_usuario;
                    var proceso = sp.c_proceso.cl_proceso + " - " + sp.c_proceso.nb_proceso;
                    var respPr = sp.c_proceso.c_usuario.nb_usuario;
                    var sub_proceso = sp.cl_sub_proceso + " - " + sp.nb_sub_proceso;
                    var respSp = sp.c_usuario.nb_usuario;
                    //obtenemos las tablas con el tiempo invertido y los usuarios correspondientes

                    List<c_usuario_sub_proceso> usuarios = new List<c_usuario_sub_proceso>();

                    usuarios = sp.c_usuario_sub_proceso.ToList();

                    foreach (var usuario in usuarios)
                    {
                        PSPCosteoViewModel us = new PSPCosteoViewModel();
                        us.cbn_entidad = entidad;
                        us.cbn_macro_proceso = macro_proceso;
                        us.cbn_proceso = proceso;
                        us.cbn_sub_proceso = sub_proceso;
                        us.nb_participante = usuario.c_usuario.nb_usuario;
                        us.resp_entidad = respEn;
                        us.resp_macro_proceso = respMp;
                        us.resp_proceso = respPr;
                        us.resp_sub_proceso = respSp;
                        //Encontrar las actividades de costeo que tiene el subproceso
                        var actividades_costeo_sp = sp.c_area_costeo_sub_proceso.ToList();
                        List<c_area_costeo> actividades_costeo = new List<c_area_costeo>();
                        foreach (var act in actividades_costeo_sp)
                        {
                            actividades_costeo.Add(act.c_area_costeo);
                        }

                        foreach (var actividad in actividades)
                        {
                            //si el sub proceso cuenta con la actividad, calcular el tiempo invertido del participante
                            if (actividades_costeo.Contains(actividad))
                            {
                                c_area_costeo_sub_proceso acsp = db.c_area_costeo_sub_proceso.Where(ac => ac.id_sub_proceso == sp.id_sub_proceso && ac.id_area_costeo == actividad.id_area_costeo).First();
                                c_usuario_sub_proceso usp = db.c_usuario_sub_proceso.Where(ussp => ussp.id_usuario == usuario.id_usuario && ussp.id_sub_proceso == sp.id_sub_proceso).First();
                                //calcular tiempo invertido en minutos
                                double TI = (double)acsp.pr_costeo * ((double)usp.tiempo_sub_proceso / (double)100);
                                //calcular porcentaje invertido
                                double percentajeC = (double)acsp.pr_costeo;
                                variables_costeo vc = new variables_costeo();
                                vc.porcentaje = (percentajeC.ToString() + "%");
                                vc.tiempo_invertido = (TI.ToString() + " min");
                                us.varcos.Add(vc);
                            }
                            //Si el proceso no cuenta con la actividad, fijar el tiempo invertido en 0
                            else
                            {
                                variables_costeo vc = new variables_costeo();
                                vc.porcentaje = ("0%");
                                vc.tiempo_invertido = ("0 min");
                                us.varcos.Add(vc);
                            }
                        }
                        us.tiempo_total = usuario.tiempo_sub_proceso.ToString();
                        repCosteo.participantes.Add(us);
                    }
                }

                rep5.enableHeadConfig = true;
                rep5.NoRowsHeader = 2;

                rep5.RepName = "Reporte de Costeo";

                HeaderConfig cellInfo = new HeaderConfig();
                HeaderConfig cellInfo2 = new HeaderConfig();
                HeaderConfig cellInfo3 = new HeaderConfig();
                cellInfo.colspan = 1;
                cellInfo.rowspan = 2;
                cellInfo.noFila = 1;

                cellInfo2.colspan = 1;
                cellInfo2.rowspan = 1;
                cellInfo2.noFila = 2;

                cellInfo3.colspan = 2;
                cellInfo3.rowspan = 1;
                cellInfo3.noFila = 1;

                colNames = new List<string>();

                colNames.Add("Entidad");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Entidad");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Macro Proceso");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Macro Proceso");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Proceso");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Proceso");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Sub Proceso");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Sub Proceso");
                rep5.HeadConfig.Add(cellInfo);
                colNames.Add("Participante");
                rep5.HeadConfig.Add(cellInfo);

                rep5.NoCols = colNames.Count;
                foreach (var item in repCosteo.nb_actividades)
                {
                    colNames.Add(item);
                    rep5.HeadConfig.Add(cellInfo3);
                    colNames.Add("Tiempo Invertido");
                    rep5.HeadConfig.Add(cellInfo2);
                    colNames.Add("Porcentaje");
                    rep5.HeadConfig.Add(cellInfo2);
                    rep5.NoCols += 3;
                }
                colNames.Add("Tiempo Invertido Total");
                rep5.HeadConfig.Add(cellInfo);
                rep5.ColNames = colNames;

                foreach (var item in repCosteo.participantes)
                {
                    row = new List<string>();

                    row.Add(item.cbn_entidad);
                    row.Add(item.resp_entidad);
                    row.Add(item.cbn_macro_proceso);
                    row.Add(item.resp_macro_proceso);
                    row.Add(item.cbn_proceso);
                    row.Add(item.resp_proceso);
                    row.Add(item.cbn_sub_proceso);
                    row.Add(item.resp_sub_proceso);
                    row.Add(item.nb_participante);

                    foreach (var vc in item.varcos)
                    {
                        row.Add(vc.tiempo_invertido);
                        row.Add(vc.porcentaje);
                    }

                    row.Add(item.tiempo_total);
                    rep5.Rows.Add(row);
                }



                rep5Json = (JObject)JToken.FromObject(rep5);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de costeo");
            }


            #endregion

            #region Rep Estructura

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Estructura");

            JObject rep6Json = new JObject();
            try
            {
                var repEstructura = db.c_sub_proceso.ToList();

                colNames = new List<string>();
                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Clave Manual");

                rep6.RepName = "Reporte de Estructura";

                rep6.ColNames = colNames;
                rep6.NoCols = colNames.Count;
                foreach (var item in repEstructura)
                {
                    var pr = item.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var en = mp.c_entidad;
                    var respPr = pr.c_usuario;
                    var respMp = mp.c_usuario;
                    var respEn = en.c_usuario;
                    var respSp = item.c_usuario;

                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(respEn.nb_usuario);
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(respMp.nb_usuario);
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(respPr.nb_usuario);
                    row.Add(item.cl_sub_proceso + " - " + item.nb_sub_proceso);
                    row.Add(respSp.nb_usuario);
                    row.Add(item.cl_manual);
                    rep6.Rows.Add(row);
                }

                rep6Json = (JObject)JToken.FromObject(rep6);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de estructura");
            }


            #endregion

            #region Rep Fallas     
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Fallas");

            JObject rep7Json = new JObject();
            try
            {
                var repFallas = db.h_excepcion.OrderBy(a => a.c_funcion.nb_funcion).ToList();

                colNames = new List<string>();
                colNames.Add("Función");
                colNames.Add("Método");
                colNames.Add("Descripción del Error");
                colNames.Add("Fecha");

                rep7.RepName = "Reporte de Fallas";

                rep7.ColNames = colNames;
                rep7.NoCols = colNames.Count;
                foreach (var entrada in repFallas)
                {
                    var nb_funcion = "";
                    if(entrada.c_funcion != null)
                    {
                        nb_funcion = entrada.c_funcion.nb_funcion;
                    }
                    else
                    {
                        nb_funcion = "N/A";
                    }

                    row = new List<string>();
                    row.Add(nb_funcion);
                    row.Add(entrada.nb_metodo);
                    row.Add(entrada.ds_excepcion);
                    row.Add(entrada.fe_excepcion.ToShortDateString());
                    rep7.Rows.Add(row);
                }

                rep7Json = (JObject)JToken.FromObject(rep7);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de fallas");
            }


            #endregion

            #region Rep Flujos y Narrativas    
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Flujos y narrativas");

            JObject rep8Json = new JObject();
            try
            {
                var repFlujosNarrativas = db.c_sub_proceso.ToList();

                colNames = new List<string>();
                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Archivo Flujo");
                colNames.Add("Archivo Narrativa");

                rep8.RepName = "Reporte de Flujos y Narrativas";

                rep8.ColNames = colNames;
                rep8.NoCols = colNames.Count;
                foreach (var item in repFlujosNarrativas)
                {
                    var pr = item.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var en = mp.c_entidad;

                    var respEn = en.c_usuario;
                    var respMp = mp.c_usuario;
                    var respPr = pr.c_usuario;
                    var respSp = item.c_usuario;

                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(respEn.nb_usuario);
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(respMp.nb_usuario);
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(respPr.nb_usuario);
                    row.Add(item.cl_sub_proceso + " - " + item.nb_sub_proceso);
                    row.Add(respSp.nb_usuario);
                    row.Add(item.nb_archivo_flujo);
                    row.Add(item.nb_archivo_manual);
                    rep8.Rows.Add(row);
                }

                rep8Json = (JObject)JToken.FromObject(rep8);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de flujos y narrativas");
            }


            #endregion

            #region Rep BDEI     
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de BDEI");

            JObject rep9Json = new JObject();
            try
            {
                var repBDEI = db.k_bdei.ToList();

                colNames = new List<string>();
                colNames.Add("ID");
                colNames.Add("Entidad");
                colNames.Add("Estatus");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable SP");
                colNames.Add("Responsable de Captura");


                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha de Ocurrencia");
                colNames.Add("Hora de Ocurrencia");
                colNames.Add("Duración");
                colNames.Add("Fecha registro evento reporte");
                colNames.Add("Fecha de Solución");
                colNames.Add("Número de evento sencillo");
                colNames.Add("Número de evento mútiple");
                colNames.Add("Folio del Riesgo Operacional R28 a 2812 asociado a la pérdida");
                colNames.Add("Tipo de Solución");
                colNames.Add("Descripción de la Incidencia");

                colNames.Add("Tipo de Riesgo");
                colNames.Add("Sub Tipo de Riesgo");
                colNames.Add("Clase de Evento");
                colNames.Add("Ámbito");
                colNames.Add("Proceso");
                colNames.Add("No. de procesos afectados");
                colNames.Add("No. de productos afectados");
                colNames.Add("Producto");
                colNames.Add("Sub Tipo Producto");
                colNames.Add("No. de líneas de negocio afectadas");
                colNames.Add("Categoría Líneas de Negocio");
                colNames.Add("Descripción Líneas de Negocio");
                colNames.Add("Frecuencia");
                colNames.Add("Impacto");
                colNames.Add("Calificación del Riesgo Operacional");
                colNames.Add("Riesgo asociado");
                colNames.Add("Mínimo riesgo operativo");
                colNames.Add("Causa");


                colNames.Add("Moneda");
                colNames.Add("Cuenta Contable Pérdida");
                colNames.Add("Catálogo de conceptos");
                colNames.Add("Monto de Exposición");
                colNames.Add("Monto de la Pérdida");
                colNames.Add("Centro de Costo");
                colNames.Add("Fecha Contable del Evento");
                colNames.Add("Fecha de Quebranto");
                colNames.Add("Monto de Recuperación");
                colNames.Add("Responsable de Recuperación");
                colNames.Add("Cuenta Contable Recuperación");
                colNames.Add("Fecha de Registro de Recuperación");
                colNames.Add("Via de Recuperación");
                colNames.Add("Monto del Gasto Asociado");
                colNames.Add("Cuenta Contable Costo Recuperación");
                colNames.Add("Fecha de Registro del Costo Recuperación");

                rep9.RepName = "Reporte BDEI";

                rep9.ColNames = colNames;
                rep9.NoCols = colNames.Count;

                string SP;
                string ResponsableSP;
                string ResponsableRec;
                string CuentaContable;
                string CuentaContableCosto;
                string Criticidad;
                //string CC;
                string RiesgoAsociado;


                foreach (var item in repBDEI)
                {
                    try
                    {
                        Criticidad = Utilidades.Utilidades.Criticidad(item);
                    }
                    catch
                    {
                        Criticidad = "N/A";
                    }

                    if (item.c_sub_proceso != null)
                    {
                        SP = item.c_sub_proceso.cl_sub_proceso + " - " + item.c_sub_proceso.nb_sub_proceso;
                        ResponsableSP = item.c_sub_proceso.c_usuario.nb_usuario;
                    }
                    else
                    {
                        ResponsableSP = SP = "N/A";
                    }


                    if(item.c_usuario != null)
                    {
                        ResponsableRec = item.c_usuario.nb_usuario;
                    }
                    else
                    {
                        ResponsableRec = "N/A";
                    }


                    if(item.c_cuenta_contable2 != null)
                    {
                        CuentaContable = item.c_cuenta_contable2.cl_cuenta_contable + item.c_cuenta_contable2.nb_cuenta_contable;
                    }
                    else
                    {
                        CuentaContable = "N/A";
                    }

                    if (item.c_cuenta_contable3 != null)
                    {
                        CuentaContableCosto = item.c_cuenta_contable3.cl_cuenta_contable + item.c_cuenta_contable3.nb_cuenta_contable;
                    }
                    else
                    {
                        CuentaContableCosto = "N/A";
                    }


                    if(item.c_riesgo_asociado_bdei != null)
                    {
                        RiesgoAsociado = item.c_riesgo_asociado_bdei.cl_riesgo_asociado_bdei + " - " + item.c_riesgo_asociado_bdei.nb_riesgo_asociado_bdei;
                    }
                    else
                    {
                        RiesgoAsociado = "N/A";
                    }


                    row = new List<string>();
                    row.Add( item.id_bdei.ToString());
                    row.Add( item.c_entidad.nb_entidad);
                    row.Add( item.c_estatus_bdei.nb_estatus_bdei);
                    row.Add( SP);
                    row.Add( ResponsableSP);
                    row.Add( item.c_usuario1.nb_usuario);

                    string feAlta = "";
                    string feOcurrencia = "";
                    string feOcurrenciaManual = "";
                    string feSolucion = "";
                    try { feAlta = item.fe_alta.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }
                    try { feOcurrencia = item.fe_ocurrencia.Value.ToString("dd/MM/yyyy"); } catch { }
                    try { feOcurrenciaManual = item.fe_ocurrencia_manual.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }
                    try { feSolucion = item.fe_solucion.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }

                    row.Add( feAlta );
                    row.Add( feOcurrencia);
                    row.Add( item.hr_ocurrencia);
                    row.Add( item.duracion.ToString());
                    row.Add( feOcurrenciaManual);
                    row.Add(feSolucion);
                    row.Add( item.no_evento_sencillo);
                    row.Add( item.no_evento_multiple);
                    row.Add( item.folio_riesgo_operacional);
                    row.Add( item.c_tipo_solucion.nb_tipo_solucion);
                    row.Add( item.comentarios);

                    row.Add(item.c_clase_evento.c_sub_tipo_riesgo_operacional.c_tipo_riesgo_operacional.cl_tipo_riesgo_operacional + " - " + @item.c_clase_evento.c_sub_tipo_riesgo_operacional.c_tipo_riesgo_operacional.nb_tipo_riesgo_operacional);
                    row.Add(item.c_clase_evento.c_sub_tipo_riesgo_operacional.cl_sub_tipo_riesgo_operacional + " - " + @item.c_clase_evento.c_sub_tipo_riesgo_operacional.nb_sub_tipo_riesgo_operacional);
                    row.Add(item.c_clase_evento.cl_clase_evento + " - " + @item.c_clase_evento.nb_clase_evento);
                    row.Add(item.c_proceso_riesgo_operacional.c_ambito_riesgo_operacional.cl_ambito_riesgo_operacional + " - " + @item.c_proceso_riesgo_operacional.c_ambito_riesgo_operacional.nb_ambito_riesgo_operacional);
                    row.Add(item.c_proceso_riesgo_operacional.cl_proceso_riesgo_operacional + " - " + @item.c_proceso_riesgo_operacional.nb_proceso_riesgo_operacional);
                    row.Add(item.no_procesos_afectados.ToString());
                    row.Add(item.no_productos_afectados.ToString());
                    row.Add(item.c_sub_tipo_producto_riesgo_operacional.c_producto_riesgo_operacional.cl_producto_riesgo_operacional + " - " + @item.c_sub_tipo_producto_riesgo_operacional.c_producto_riesgo_operacional.nb_producto_riesgo_operacional );
                    row.Add(item.c_sub_tipo_producto_riesgo_operacional.cl_sub_tipo_producto_riesgo_operacional + " - " + @item.c_sub_tipo_producto_riesgo_operacional.nb_sub_tipo_producto_riesgo_operacional);
                    row.Add(item.no_lineas_negocio_afectadas.ToString());
                    row.Add(item.c_linea_negocio_riesgo_operacional.c_categoria_linea_negocio_riesgo_operacional.cl_categoria_linea_negocio_riesgo_operacional + " - " + @item.c_linea_negocio_riesgo_operacional.c_categoria_linea_negocio_riesgo_operacional.nb_categoria_linea_negocio_riesgo_operacional);
                    row.Add(item.c_linea_negocio_riesgo_operacional.cl_linea_negocio_riesgo_operacional + " - " + @item.c_linea_negocio_riesgo_operacional.nb_linea_negocio_riesgo_operacional);
                    row.Add(item.c_frecuencia_riesgo_operacional.cl_frecuencia_riesgo_operacional + " - " + @item.c_frecuencia_riesgo_operacional.nb_frecuencia_riesgo_operacional);
                    row.Add(item.c_impacto_riesgo_operacional.cl_impacto_riesgo_operacional + " - " + @item.c_impacto_riesgo_operacional.nb_impacto_riesgo_operacional);
                    row.Add(Criticidad);
                    row.Add(RiesgoAsociado);
                    row.Add(item.c_minimo_riesgo_operativo.cl_minimo_riesgo_operativo + " - " + @item.c_minimo_riesgo_operativo.nb_minimo_riesgo_operativo);
                    row.Add(item.c_causa_bdei.cl_causa_bdei + " - " + @item.c_causa_bdei.nb_causa_bdei);


                    string feRegistroPerdida = "";
                    string feQuebranto = "";
                    string feRegistroRecuperacion = "";
                    string feRegistroCosto = "";
                    try { feRegistroPerdida = item.fe_registro_perdida.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }
                    try { feQuebranto = item.fe_quebranto.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }
                    try { feRegistroRecuperacion = item.fe_registro_recuperacion.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }
                    try { feRegistroCosto = item.fe_registro_costo_recuperacion.Value.ToString("dd/MM/yyyy hh:mm"); } catch { }


                    row.Add( item.c_moneda.nb_moneda);
                    row.Add( item.c_cuenta_contable1.cl_cuenta_contable + " - " + item.c_cuenta_contable1.nb_cuenta_contable);
                    row.Add(item.c_catalogo_concepto.cl_catalogo_concepto + " - " + @item.c_catalogo_concepto.nb_catalogo_concepto);
                    row.Add( (item.mn_exposicion ?? 0).ToString());
                    row.Add( (item.mn_quebranto ?? 0).ToString());
                    row.Add( item.c_centro_costo.cl_centro_costo + " - " + item.c_centro_costo.nb_centro_costo);
                    row.Add(feRegistroPerdida);
                    row.Add( feQuebranto);
                    row.Add( (item.mn_recuperacion ?? 0).ToString());
                    row.Add( ResponsableRec);
                    row.Add( CuentaContable);
                    row.Add(feRegistroRecuperacion);
                    row.Add( item.via_recuperacion);
                    row.Add( (item.mn_costo_recuperacion ?? 0).ToString());
                    row.Add( CuentaContableCosto);
                    row.Add( feRegistroCosto);
                    rep9.Rows.Add(row);
                }

                rep9Json = (JObject)JToken.FromObject(rep9);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de BDEI");
            }


            #endregion

            #region Rep Indicadores
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Indicadores");

            JObject rep10Json = new JObject();
            try
            {
                var repIndicadores = db.k_evaluacion.ToList();

                colNames = new List<string>();
                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Clave");
                colNames.Add("Nombre");
                colNames.Add("Descripción");
                colNames.Add("Descripción Numerador");
                colNames.Add("Descripción Denominador");
                colNames.Add("Frecuencia");
                colNames.Add("Unidad");
                colNames.Add("Control Asociado");
                colNames.Add("Peso");
                colNames.Add("Umbral (0.0)");
                colNames.Add("Umbral (0.0 - 5.0)");
                colNames.Add("Umbral (5.0 - 7.5)");
                colNames.Add("Umbral (7.5 - 10.0)");
                colNames.Add("Activo");
                colNames.Add("Área");
                colNames.Add("Responsable");
                colNames.Add("Periodo Indicador");
                colNames.Add("Numerador");
                colNames.Add("Denominador");
                colNames.Add("Medición");
                colNames.Add("Calificación");

                rep10.RepName = "Reporte de Indicadores";

                rep10.ColNames = colNames;
                rep10.NoCols = colNames.Count;
                foreach (var item in repIndicadores)
                {
                    var indicador = item.c_indicador;

                    var en = indicador.c_entidad;
                    var ar = indicador.c_area;
                    var resp = indicador.c_usuario;
                    var frec = indicador.c_frecuencia_indicador;
                    var unidad = indicador.c_unidad_indicador;
                    var control_a = indicador.k_control != null ? indicador.k_control.relacion_control : "N/A";
                    var periodo = item.c_periodo_indicador;
                    var calificacion = item.c_calificacion_indicador != null ? item.c_calificacion_indicador.nb_calificacion_indicador : "N/A";

                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(en.c_usuario.nb_usuario);
                    row.Add(indicador.cl_indicador);
                    row.Add(indicador.nb_indicador);
                    row.Add(indicador.ds_indicador);
                    row.Add(indicador.ds_numerador);
                    row.Add(indicador.ds_denominador);
                    row.Add(frec.nb_frecuencia_indicador);
                    row.Add(unidad.nb_unidad_indicador);
                    row.Add(control_a);
                    row.Add(((float)indicador.peso).ToString().Replace(",", "."));
                    row.Add(((double)indicador.umbral000i).ToString() + " - " + ((double)indicador.umbral000f).ToString());
                    row.Add(((double)indicador.umbral050i).ToString() + " - " + ((double)indicador.umbral050f).ToString());
                    row.Add(((double)indicador.umbral075i).ToString() + " - " + ((double)indicador.umbral075f).ToString());
                    row.Add(((double)indicador.umbral100i).ToString() + " - " + ((double)indicador.umbral100f).ToString());
                    row.Add(indicador.esta_activo ? "Si" : "No");
                    row.Add(ar.nb_area);
                    row.Add(resp.nb_usuario);
                    row.Add(periodo.nb_periodo_indicador);
                    row.Add(((float)item.numerador).ToString().Replace(",", "."));
                    row.Add(((float)item.denominador).ToString().Replace(",", "."));
                    row.Add(((float)item.medicion).ToString().Replace(",", "."));
                    row.Add(calificacion);

                    rep10.Rows.Add(row);
                }

                rep10Json = (JObject)JToken.FromObject(rep10);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de indicadores");
            }


            #endregion

            #region Rep Incidencias y Planes

            rep11_1.RepName = "Reporte de Incidencias y Planes: Oficios";
            rep11_2.RepName = "Reporte de Incidencias y Planes: Informes de Auditoría Externa";
            rep11_3.RepName = "Reporte de Incidencias y Planes: Informes de Auditoría Interna";
            rep11_4.RepName = "Reporte de Incidencias y Planes: Certificación";
            rep11_5.RepName = "Reporte de Incidencias y Planes: MRyC";
            rep11_6.RepName = "Reporte de Incidencias y Planes: Riesgo Operativo/Otros";

            #region rep11_1 Oficios
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Incidencias y Planes: Oficios");

            JObject rep11_1Json = new JObject();
            try
            {
                var repIyPOficios = db.v_IyP_t1.ToList();

                colNames = new List<string>();
                colNames.Add("Número de Oficio");
                colNames.Add("Entidad");
                colNames.Add("Tipo de Autoridad");
                colNames.Add("Responsable del Oficio");
                colNames.Add("Número de Incidencias a Capturar");
                colNames.Add("Fecha del Oficio");
                colNames.Add("Fecha de Vencimiento");
                colNames.Add("Fecha de Contestación");
                colNames.Add("Id de la Incidencia");
                colNames.Add("Nivel de la Incidencia");
                colNames.Add("Descripción de la Incidencia");
                colNames.Add("Responsable de la Incidencia");
                colNames.Add("Clasificación de la Incidencia");
                colNames.Add("Requiere Plan de Remediación");
                colNames.Add("Id del plan");
                colNames.Add("Nombre del Plan");
                colNames.Add("Descripción del Plan");
                colNames.Add("Área Responsable");
                colNames.Add("Responsable del Plan");
                colNames.Add("Responsable del Seguimiento");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de implantación");
                colNames.Add("Fecha real de solución");
                colNames.Add("Comentarios de Seguimiento");
                colNames.Add("Fecha del Seguimiento");

                rep11_1.ColNames = colNames;
                rep11_1.NoCols = 25;
                foreach (var item in repIyPOficios)
                {
                    row = new List<string>();

                    row.Add(item.nb_objeto);
                    row.Add(item.cnb_entidad);
                    row.Add(item.cnb_autoridad);
                    row.Add(item.nb_resp_oficio);
                    row.Add(item.no_incidencias.ToString());
                    row.Add(item.fe_objeto.ToString());
                    row.Add(item.fe_vencimiento.ToString());
                    row.Add(item.fe_contestacion.ToString());
                    row.Add(item.id_incidencia.ToString());
                    row.Add((item.lvl_5 ?? item.lvl_4 ?? item.lvl_3 ?? item.lvl_2 ?? item.lvl_1 ?? "N/A"));
                    row.Add(item.ds_incidencia);
                    row.Add(item.nb_resp_inc);
                    row.Add(item.cnb_clasificacion_incidencia);
                    row.Add((item.requiere_plan != false ? "Si" : "No <b>Justificación: </b> " + Utilidades.Utilidades.JustificacionNoPlan((int)item.id_incidencia)));
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.cnb_area);
                    row.Add(item.nb_resp_plan);
                    row.Add(item.nb_resp_seguimiento);
                    row.Add(item.fe_alta_p.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());
                    row.Add(item.obs_seguimiento);
                    row.Add(item.fe_seguimiento.ToString());

                    rep11_1.Rows.Add(row);
                }

                rep11_1Json = (JObject)JToken.FromObject(rep11_1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de incidencias y planes: Oficios");
            }



            #endregion

            #region rep11_2 Informes Aud Ext

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Incidencias y Planes: Informes de Auditoría Externa");

            JObject rep11_2Json = new JObject();
            try
            {
                var repIyPAudEx = db.v_IyP_t2.ToList();

                colNames = new List<string>();
                colNames.Add("Nombre de Informe");
                colNames.Add("Entidad");
                colNames.Add("Responsable del Informe");
                colNames.Add("Número de Incidencias a Capturar");
                colNames.Add("Fecha del Informe");
                colNames.Add("Fecha de Contestación");
                colNames.Add("Id de la Incidencia");
                colNames.Add("Nivel de la Incidencia");
                colNames.Add("Descripción de la Incidencia");
                colNames.Add("Responsable de la Incidencia");
                colNames.Add("Clasificación de la Incidencia");
                colNames.Add("Requiere Plan de Remediación");
                colNames.Add("Id del plan");
                colNames.Add("Nombre del Plan");
                colNames.Add("Descripción del Plan");
                colNames.Add("Área Responsable");
                colNames.Add("Responsable del Plan");
                colNames.Add("Responsable del Seguimiento");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de implantación");
                colNames.Add("Fecha real de solución");
                colNames.Add("Comentarios de Seguimiento");
                colNames.Add("Fecha del Seguimiento");

                rep11_2.ColNames = colNames;
                rep11_2.NoCols = 23;
                foreach (var item in repIyPAudEx)
                {
                    row = new List<string>();

                    row.Add(item.nb_objeto);
                    row.Add(item.cnb_entidad);
                    row.Add(item.nb_resp_oficio);
                    row.Add(item.no_incidencias.ToString());
                    row.Add(item.fe_objeto.ToString());
                    row.Add(item.fe_contestacion.ToString());
                    row.Add(item.id_incidencia.ToString());
                    row.Add((item.lvl_5 ?? item.lvl_4 ?? item.lvl_3 ?? item.lvl_2 ?? item.lvl_1 ?? "N/A"));
                    row.Add(item.ds_incidencia);
                    row.Add(item.nb_resp_inc);
                    row.Add(item.cnb_clasificacion_incidencia);
                    row.Add((item.requiere_plan != false ? "Si" : "No <b>Justificación: </b> " + Utilidades.Utilidades.JustificacionNoPlan((int)item.id_incidencia)));
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.cnb_area);
                    row.Add(item.nb_resp_plan);
                    row.Add(item.nb_resp_seguimiento);
                    row.Add(item.fe_alta_p.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());
                    row.Add(item.obs_seguimiento);
                    row.Add(item.fe_seguimiento.ToString());

                    rep11_2.Rows.Add(row);
                }

                rep11_2Json = (JObject)JToken.FromObject(rep11_2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de incidencias y planes: Informes de auditoría Externa");
            }



            #endregion

            #region rep11_3 Informes Aud Int

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Incidencias y Planes: Informes de Auditoría Interna");

            JObject rep11_3Json = new JObject();
            try
            {
                var repIyPAudIn = db.v_IyP_t3.ToList();

                colNames = new List<string>();
                colNames.Add("Nombre de Informe");
                colNames.Add("Entidad");
                colNames.Add("Responsable del Informe");
                colNames.Add("Número de Incidencias a Capturar");
                colNames.Add("Fecha del Informe");
                colNames.Add("Fecha de Contestación");
                colNames.Add("Id de la Incidencia");
                colNames.Add("Nivel de la Incidencia");
                colNames.Add("Descripción de la Incidencia");
                colNames.Add("Responsable de la Incidencia");
                colNames.Add("Clasificación de la Incidencia");
                colNames.Add("Requiere Plan de Remediación");
                colNames.Add("Id del plan");
                colNames.Add("Nombre del Plan");
                colNames.Add("Descripción del Plan");
                colNames.Add("Área Responsable");
                colNames.Add("Responsable del Plan");
                colNames.Add("Responsable del Seguimiento");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de implantación");
                colNames.Add("Fecha real de solución");
                colNames.Add("Comentarios de Seguimiento");
                colNames.Add("Fecha del Seguimiento");

                rep11_3.ColNames = colNames;
                rep11_3.NoCols = 23;
                foreach (var item in repIyPAudIn)
                {
                    row = new List<string>();

                    row.Add(item.nb_objeto);
                    row.Add(item.cnb_entidad);
                    row.Add(item.nb_resp_oficio);
                    row.Add(item.no_incidencias.ToString());
                    row.Add(item.fe_objeto.ToString());
                    row.Add(item.fe_contestacion.ToString());
                    row.Add(item.id_incidencia.ToString());
                    row.Add((item.lvl_5 ?? item.lvl_4 ?? item.lvl_3 ?? item.lvl_2 ?? item.lvl_1 ?? "N/A"));
                    row.Add(item.ds_incidencia);
                    row.Add(item.nb_resp_inc);
                    row.Add(item.cnb_clasificacion_incidencia);
                    row.Add((item.requiere_plan != false ? "Si" : "No <b>Justificación: </b> " + Utilidades.Utilidades.JustificacionNoPlan((int)item.id_incidencia)));
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.cnb_area);
                    row.Add(item.nb_resp_plan);
                    row.Add(item.nb_resp_seguimiento);
                    row.Add(item.fe_alta_p.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());
                    row.Add(item.obs_seguimiento);
                    row.Add(item.fe_seguimiento.ToString());

                    rep11_3.Rows.Add(row);
                }

                rep11_3Json = (JObject)JToken.FromObject(rep11_3);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de incidencias y planes: Informes de auditoría Interna");
            }

            #endregion

            #region rep11_4 Certificación

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Incidencias y Planes: Certificación");

            JObject rep11_4Json = new JObject();
            try
            {
                var repIyPCertif = db.v_IyP_t4.ToList();

                colNames = new List<string>();
                colNames.Add("Ruta del Control");
                colNames.Add("Clave del Control");
                colNames.Add("Clave de la Certificación");
                colNames.Add("Periodo de Certificación");
                colNames.Add("Procedimiento de Certificación");
                colNames.Add("Id de la Incidencia");
                colNames.Add("Nivel de la Incidencia");
                colNames.Add("Descripción de la Incidencia");
                colNames.Add("Responsable de la Incidencia");
                colNames.Add("Clasificación de la Incidencia");
                colNames.Add("Requiere Plan de Remediación");
                colNames.Add("Id del plan");
                colNames.Add("Nombre del Plan");
                colNames.Add("Descripción del Plan");
                colNames.Add("Área Responsable");
                colNames.Add("Responsable del Plan");
                colNames.Add("Responsable del Seguimiento");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de implantación");
                colNames.Add("Fecha real de solución");
                colNames.Add("Comentarios de Seguimiento");
                colNames.Add("Fecha del Seguimiento");

                rep11_4.ColNames = colNames;
                rep11_4.NoCols = 22;
                foreach (var item in repIyPCertif)
                {
                    row = new List<string>();

                    row.Add(item.ruta_control);
                    row.Add(item.codigo_control);
                    row.Add(item.cl_certificacion_control);
                    row.Add(item.periodo_certificacion);
                    row.Add(item.ds_procedimiento_certificacion);
                    row.Add(item.id_incidencia.ToString());
                    row.Add((item.lvl_5 ?? item.lvl_4 ?? item.lvl_3 ?? item.lvl_2 ?? item.lvl_1 ?? "N/A"));
                    row.Add(item.ds_incidencia);
                    row.Add(item.nb_resp_inc);
                    row.Add(item.cnb_clasificacion_incidencia);
                    row.Add((item.requiere_plan != false ? "Si" : "No <b>Justificación: </b> " + Utilidades.Utilidades.JustificacionNoPlan((int)item.id_incidencia)));
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.cnb_area);
                    row.Add(item.nb_resp_plan);
                    row.Add(item.nb_resp_seguimiento);
                    row.Add(item.fe_alta_p.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());
                    row.Add(item.obs_seguimiento);
                    row.Add(item.fe_seguimiento.ToString());

                    rep11_4.Rows.Add(row);
                }

                rep11_4Json = (JObject)JToken.FromObject(rep11_4);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Incidencias y Planes: Certificación");
            }



            #endregion

            #region rep11_5 MRyC

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Incidencias y Planes: MRyC");

            JObject rep11_5Json = new JObject();
            try
            {
                var repIyPMRyC = db.v_IyP_t5.ToList();

                colNames = new List<string>();
                colNames.Add("Ruta Riesgo");
                colNames.Add("Código de Riesgo");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Acción Correctora");
                colNames.Add("Id de la Incidencia");
                colNames.Add("Nivel de la Incidencia");
                colNames.Add("Descripción de la Incidencia");
                colNames.Add("Responsable de la Incidencia");
                colNames.Add("Clasificación de la Incidencia");
                colNames.Add("Requiere Plan de Remediación");
                colNames.Add("Id del plan");
                colNames.Add("Nombre del Plan");
                colNames.Add("Descripción del Plan");
                colNames.Add("Área Responsable");
                colNames.Add("Responsable del Plan");
                colNames.Add("Responsable del Seguimiento");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de implantación");
                colNames.Add("Fecha real de solución");
                colNames.Add("Comentarios de Seguimiento");
                colNames.Add("Fecha del Seguimiento");

                rep11_5.ColNames = colNames;
                rep11_5.NoCols = 21;
                foreach (var item in repIyPMRyC)
                {
                    row = new List<string>();

                    row.Add(item.ruta_control);
                    row.Add(item.codigo_riesgo);
                    row.Add(item.evento_riesgo);
                    row.Add(item.accion_correctora);
                    row.Add(item.id_incidencia.ToString());
                    row.Add((item.lvl_5 ?? item.lvl_4 ?? item.lvl_3 ?? item.lvl_2 ?? item.lvl_1 ?? "N/A"));
                    row.Add(item.ds_incidencia);
                    row.Add(item.nb_resp_inc);
                    row.Add(item.cnb_clasificacion_incidencia);
                    row.Add((item.requiere_plan != false ? "Si" : "No <b>Justificación: </b> " + Utilidades.Utilidades.JustificacionNoPlan((int)item.id_incidencia)));
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.cnb_area);
                    row.Add(item.nb_resp_plan);
                    row.Add(item.nb_resp_seguimiento);
                    row.Add(item.fe_alta_p.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());
                    row.Add(item.obs_seguimiento);
                    row.Add(item.fe_seguimiento.ToString());

                    rep11_5.Rows.Add(row);
                }

                rep11_5Json = (JObject)JToken.FromObject(rep11_5);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Incidencias y Planes: MRyC");
            }



            #endregion

            #region rep11_6 Otros

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Incidencias y Planes: Otros");


            JObject rep11_6Json = new JObject();
            try
            {
                var repIyPOtros = db.v_IyP_t6.ToList();

                colNames = new List<string>();
                colNames.Add("Origen del Objeto");
                colNames.Add("Entidad");
                colNames.Add("Descripción del Objeto");
                colNames.Add("Fecha");
                colNames.Add("Id de la Incidencia");
                colNames.Add("Nivel de la Incidencia");
                colNames.Add("Descripción de la Incidencia");
                colNames.Add("Responsable de la Incidencia");
                colNames.Add("Clasificación de la Incidencia");
                colNames.Add("Requiere Plan de Remediación");
                colNames.Add("Id del plan");
                colNames.Add("Nombre del Plan");
                colNames.Add("Descripción del Plan");
                colNames.Add("Área Responsable");
                colNames.Add("Responsable del Plan");
                colNames.Add("Responsable del Seguimiento");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de implantación");
                colNames.Add("Fecha real de solución");
                colNames.Add("Comentarios de Seguimiento");
                colNames.Add("Fecha del Seguimiento");


                rep11_6.ColNames = colNames;
                rep11_6.NoCols = 21;
                foreach (var item in repIyPOtros)
                {
                    row = new List<string>();

                    row.Add(item.nb_objeto);
                    row.Add(item.cnb_entidad);
                    row.Add(item.ds_objeto);
                    row.Add(item.fe_objeto.ToString());
                    row.Add(item.id_incidencia.ToString());
                    row.Add((item.lvl_5 ?? item.lvl_4 ?? item.lvl_3 ?? item.lvl_2 ?? item.lvl_1 ?? "N/A"));
                    row.Add(item.ds_incidencia);
                    row.Add(item.nb_resp_inc);
                    row.Add(item.cnb_clasificacion_incidencia);
                    row.Add((item.requiere_plan != false ? "Si" : "No <b>Justificación: </b> " + Utilidades.Utilidades.JustificacionNoPlan((int)item.id_incidencia)));
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.cnb_area);
                    row.Add(item.nb_resp_plan);
                    row.Add(item.nb_resp_seguimiento);
                    row.Add(item.fe_alta_p.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());
                    row.Add(item.obs_seguimiento);
                    row.Add(item.fe_seguimiento.ToString());

                    rep11_6.Rows.Add(row);
                }

                rep11_6Json = (JObject)JToken.FromObject(rep11_6);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Incidencias y Planes: Otros");
            }



            #endregion

            #endregion

            #region Planes de Remediación 

            rep12_1.RepName = "Reporte de Planes de Remediación Pendientes";
            rep12_2.RepName = "Reporte de Planes de Remediación Concluidos";

            #region rep12_1 PlanesRem Pendientes

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Planes de Remediación Pendientes");

            JObject rep12_1Json = new JObject();
            try
            {
                var repPlanesRemPendientes = db.k_plan.Where(p => p.r_conclusion_plan.Count() == 0).ToList();

                colNames = new List<string>();
                colNames.Add("Origen Incidencia");
                colNames.Add("Nombre del objeto origen");
                colNames.Add("ID");
                colNames.Add("Nombre");
                colNames.Add("Plan");
                colNames.Add("Área");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de Implantación");


                rep12_1.ColNames = colNames;
                rep12_1.NoCols = colNames.Count;
                foreach (var item in repPlanesRemPendientes)
                {
                    var incidencia = item.k_incidencia;
                    string origen = "";
                    string nbOrigen = "";


                    var objeto = incidencia.k_objeto;
                    var cert = incidencia.k_certificacion_control;
                    var control = incidencia.k_control;

                    if (objeto != null)
                    {
                        var tipoObjeto = objeto.tipo_objeto;


                        switch (tipoObjeto)
                        {
                            case 1:
                                origen = "Oficios";
                                nbOrigen = objeto.nb_objeto;
                                break;
                            case 2:
                                origen = "Informes Auditoría Externa";
                                nbOrigen = objeto.nb_objeto;
                                break;
                            case 3:
                                origen = "Informes Auditoría Interna";
                                nbOrigen = objeto.nb_objeto;
                                break;
                                //case 4:
                                //    origen = "Certificación";
                                //    nbOrigen = objeto.nb_objeto;
                                //    break;
                                //case 5:
                                //    origen = "MRyC";
                                //    nbOrigen = objeto.nb_objeto;
                            case 6:
                                origen = "Riesgo Operativo/Otros";
                                nbOrigen = objeto.nb_objeto;
                                break;
                        }
                    }
                    else if (cert != null)
                    {
                        origen = "Certificación";
                        nbOrigen = "Certificación del control " + cert.k_control.relacion_control;
                    }
                    else if (control != null)
                    {
                        origen = "Control";
                        nbOrigen = control.relacion_control;
                    }

                    row = new List<string>();

                    row.Add(origen);
                    row.Add(nbOrigen);
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.c_area.cl_area + " - " + item.c_area.nb_area);
                    row.Add(item.fe_alta.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());


                    rep12_1.Rows.Add(row);
                }

                rep12_1Json = (JObject)JToken.FromObject(rep12_1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Planes de Remediación Pendientes");
            }



            #endregion

            #region rep12_2 PlanesRem Concluidos

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Planes de Remediación Concluidos");

            JObject rep12_2Json = new JObject();
            try
            {
                var repPlanesRemConcluidos = db.k_plan.Where(p => p.r_conclusion_plan.Count() == 1).ToList();

                colNames = new List<string>();
                colNames.Add("Origen Incidencia");
                colNames.Add("Nombre del objeto origen");
                colNames.Add("ID");
                colNames.Add("Nombre");
                colNames.Add("Plan");
                colNames.Add("Área");
                colNames.Add("Conclusión");
                colNames.Add("Fecha de Alta");
                colNames.Add("Fecha estimada de Implantación");
                colNames.Add("Fecha real de Solución");


                rep12_2.ColNames = colNames;
                rep12_2.NoCols = colNames.Count;
                foreach (var item in repPlanesRemConcluidos)
                {
                    var incidencia = item.k_incidencia;
                    string origen = "";
                    string nbOrigen = "";


                    var objeto = incidencia.k_objeto;
                    var cert = incidencia.k_certificacion_control;
                    var control = incidencia.k_control;

                    if (objeto != null)
                    {
                        var tipoObjeto = objeto.tipo_objeto;


                        switch (tipoObjeto)
                        {
                            case 1:
                                origen = "Oficios";
                                nbOrigen = objeto.nb_objeto;
                                break;
                            case 2:
                                origen = "Informes Auditoría Externa";
                                nbOrigen = objeto.nb_objeto;
                                break;
                            case 3:
                                origen = "Informes Auditoría Interna";
                                nbOrigen = objeto.nb_objeto;
                                break;
                            //case 4:
                            //    origen = "Certificación";
                            //    nbOrigen = objeto.nb_objeto;
                            //    break;
                            //case 5:
                            //    origen = "MRyC";
                            //    nbOrigen = objeto.nb_objeto;
                            case 6:
                                origen = "Riesgo Operativo/Otros";
                                nbOrigen = objeto.nb_objeto;
                                break;
                        }
                    }
                    else if (cert != null)
                    {
                        origen = "Certificación";
                        nbOrigen = "Certificación del control " + cert.k_control.relacion_control;
                    }
                    else if (control != null)
                    {
                        origen = "Control";
                        nbOrigen = control.relacion_control;
                    }

                    row = new List<string>();

                    row.Add(origen);
                    row.Add(nbOrigen);
                    row.Add(item.id_plan.ToString());
                    row.Add(item.nb_plan);
                    row.Add(item.ds_plan);
                    row.Add(item.c_area.cl_area + " - " + item.c_area.nb_area);
                    row.Add(item.r_conclusion_plan.First().observaciones);
                    row.Add(item.fe_alta.ToString());
                    row.Add(item.fe_estimada_implantacion.ToString());
                    row.Add(item.fe_real_solucion.ToString());


                    rep12_2.Rows.Add(row);
                }

                rep12_2Json = (JObject)JToken.FromObject(rep12_2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo Planes de Remediación Concluidos");
            }


            #endregion

            #endregion

            #region Riesgo Residual

            rep13_1.RepName = "Reporte de Controles con Riesgo Residual";
            rep13_2.RepName = "Reporte de Controles sin Riesgo Residual";

            var controles = db.k_control.ToList();

            List<k_control> conRR = new List<k_control>();
            List<k_control> sinRR = new List<k_control>();


            foreach (var control in controles)
            {
                //evitar que se usen controles sin riesgo
                if (control.k_riesgo.Count() > 0)
                {
                    if (control.k_riesgo_residual.Count() > 0)
                    {
                        conRR.Add(control);
                    }
                    else
                    {
                        string prefijo = control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);
                        if (prefijo == "MP")
                            sinRR.Add(control);
                    }
                }
            }

            #region rep13_1 Con Riesgo residual
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Controles con Riesgo Residual");

            JObject rep13_1Json = new JObject();
            try
            {
                colNames = new List<string>();
                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Riesgo");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Control");
                colNames.Add("Actividad de Control");
                colNames.Add("Riesgo Inherente");
                colNames.Add("Cobertura de Control");
                colNames.Add("Riesgo Residual");


                rep13_1.ColNames = colNames;
                rep13_1.NoCols = colNames.Count;
                foreach (var item in conRR)
                {
                    var RR = item.k_riesgo_residual.First();
                    var cc = RR.a_campo_cobertura_control.valor + RR.a_campo_cobertura_control1.valor + RR.a_campo_cobertura_control2.valor + RR.a_campo_cobertura_control3.valor + RR.a_campo_cobertura_control4.valor + RR.a_campo_cobertura_control5.valor + RR.a_campo_cobertura_control6.valor + RR.a_campo_cobertura_control7.valor + RR.a_campo_cobertura_control8.valor + RR.a_campo_cobertura_control9.valor + RR.a_campo_cobertura_control10.valor + RR.a_campo_cobertura_control11.valor + RR.a_campo_cobertura_control12.valor + RR.a_campo_cobertura_control13.valor;
                    var ri = item.k_riesgo.First().c_magnitud_impacto.magnitud_impacto * item.k_riesgo.First().c_probabilidad_ocurrencia.pr_probabilidad_ocurrencia * (decimal).01;

                    row = new List<string>();

                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad) + " - " + (item.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.nb_entidad));
                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario));
                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso) + " - " + (item.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso));
                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.c_usuario.nb_usuario));
                    row.Add((item.c_sub_proceso.c_proceso.cl_proceso) + " - " + (item.c_sub_proceso.c_proceso.nb_proceso));
                    row.Add((item.c_sub_proceso.c_proceso.c_usuario.nb_usuario));
                    row.Add((item.c_sub_proceso.cl_sub_proceso) + " - " + (item.c_sub_proceso.nb_sub_proceso));
                    row.Add((item.c_sub_proceso.c_usuario.nb_usuario));
                    row.Add((item.k_riesgo.First().nb_riesgo));
                    row.Add((item.k_riesgo.First().evento));
                    row.Add((item.relacion_control));
                    row.Add((item.actividad_control));
                    row.Add(String.Format("${0}", ((double)ri).ToString().Replace(",", ".")));
                    row.Add(String.Format("{0}%", ((double)cc).ToString().Replace(",", ".")));
                    row.Add(String.Format("${0}", ((double)(ri * (100 - cc)) / 100)).ToString().Replace(",", "."));


                    rep13_1.Rows.Add(row);
                }

                rep13_1Json = (JObject)JToken.FromObject(rep13_1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Controles con Riesgo Residual");
            }



            #endregion

            #region rep13_2 Sin Riesgo residual

            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Controles sin Riesgo Residual");

            JObject rep13_2Json = new JObject();
            try
            {
                colNames = new List<string>();
                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Riesgo");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Control");
                colNames.Add("Actividad de Control");


                rep13_2.ColNames = colNames;
                rep13_2.NoCols = colNames.Count;
                foreach (var item in sinRR)
                {
                    row = new List<string>();

                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.cl_entidad) + " - " + (item.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.nb_entidad));
                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.c_usuario.nb_usuario));
                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso) + " - " + (item.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso));
                    row.Add((item.c_sub_proceso.c_proceso.c_macro_proceso.c_usuario.nb_usuario));
                    row.Add((item.c_sub_proceso.c_proceso.cl_proceso) + " - " + (item.c_sub_proceso.c_proceso.nb_proceso));
                    row.Add((item.c_sub_proceso.c_proceso.c_usuario.nb_usuario));
                    row.Add((item.c_sub_proceso.cl_sub_proceso) + " - " + (item.c_sub_proceso.nb_sub_proceso));
                    row.Add((item.c_sub_proceso.c_usuario.nb_usuario));
                    row.Add((item.k_riesgo.First().nb_riesgo));
                    row.Add((item.k_riesgo.First().evento));
                    row.Add((item.relacion_control));
                    row.Add((item.actividad_control));


                    rep13_2.Rows.Add(row);
                }

                rep13_2Json = (JObject)JToken.FromObject(rep13_2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Controles sin Riesgo Residual");
            }



            #endregion

            #endregion

            #region Rep MRyC  

            rep14_1.RepName = "Reporte General MRyC";
            rep14_2.RepName = "Reporte de Riesgos Derogados";

            List<MRyCRepModel> vista = new List<MRyCRepModel>();
            var criticidades = db.c_criticidad.ToList();

            foreach (var item in db.k_control)
            {
                var registro = new MRyCRepModel();

                m_tipo = null;
                PropertyInfo[] props_sp = null;
                PropertyInfo[] props_r = null;
                PropertyInfo[] props_c = null;
                PropertyInfo[] props_reg = null;

                var riesgo = item.k_riesgo.FirstOrDefault() ?? new k_riesgo() { nb_riesgo = "El control " + item.relacion_control + "(" + item.id_control +") no tiene riesgo" };
                var sp = item.c_sub_proceso;
                var pr = sp.c_proceso;
                var mp = pr.c_macro_proceso;
                var en = mp.c_entidad;

                var respPr = pr.c_usuario;
                var psrePr = Utilidades.Utilidades.PuestoUsuario(pr.id_responsable);
                var respMp = mp.c_usuario;
                var psreMp = Utilidades.Utilidades.PuestoUsuario(mp.id_responsable);
                var respEn = en.c_usuario;
                var psreEn = Utilidades.Utilidades.PuestoUsuario(en.id_responsable);
                var respSp = sp.c_usuario;
                var psreSp = Utilidades.Utilidades.PuestoUsuario(sp.id_responsable);

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

                try
                {
                    foreach (var ln in sp.c_linea_negocio)
                    {
                        registro.lineas_negocio += ln.nb_linea_negocio + "\n";
                    }
                }
                catch
                {
                    registro.lineas_negocio = "N/A";
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

                try { esRiesgoOperativo = es_mp ? (riesgo.es_riesgo_operativo ? "Si" : "No") : "N/A"; } catch { }
                try { productoRO = es_mp ? riesgo.c_sub_tipo_producto_riesgo_operacional.nb_sub_tipo_producto_riesgo_operacional : "N/A"; } catch { }
                try { subTipoRO = es_mp ? riesgo.c_sub_tipo_riesgo_operacional.nb_sub_tipo_riesgo_operacional : "N/A"; } catch { }
                try { procesoRO = es_mp ? riesgo.c_proceso_riesgo_operacional.nb_proceso_riesgo_operacional : "N/A"; } catch { }
                try { lineaNegocioRO = es_mp ? riesgo.c_linea_negocio_riesgo_operacional.nb_linea_negocio_riesgo_operacional : "N/A"; } catch { }
                try { frecuenciaRO = es_mp ? riesgo.c_frecuencia_riesgo_operacional.nb_frecuencia_riesgo_operacional : "N/A"; } catch { }
                try { impactoRO = es_mp ? riesgo.c_impacto_riesgo_operacional.nb_impacto_riesgo_operacional : "N/A"; } catch { }
                try { criticidadRO = es_mp ? Utilidades.Utilidades.Criticidad(riesgo.id_frecuencia_riesgo_operacional ?? 0, riesgo.id_impacto_riesgo_operacional ?? 0) : "N/A"; } catch { }


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
                registro.afectacion_contable = es_mp ? (riesgo.tiene_afectacion_contable ? "Si" : "No") : "N/A";
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
                registro.control_clave = acc || !es_mp ? "N/A" : item.es_control_clave ? "Si" : "No";

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

                vista.Add(registro);
            }



            var riesgoDerogado = db.k_riesgo_derogado.ToList();

            #region rep14_1 MRyC
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte General MRyC");

            JObject rep14_1Json = new JObject();
            try
            {
                colNames = new List<string>();



                colNames.Add("Entidad");
                colNames.Add("Responsable Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Responsable Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Descripción Sub Proceso");
                colNames.Add("Responsable Sub Proceso");
                colNames.Add("Sub Proceso Anterior");
                colNames.Add("Sub Proceso Siguiente");
                colNames.Add("Tipología de Sub Proceso");
                colNames.Add("Líneas de Negocio");
                colNames.Add("Etapa de Líneas de Negocio");
                colNames.Add("Sub Etapa de Líneas de Negocio");
                colNames.Add("Áreas involucradas");
                colNames.Add("Aplicaciones relacionadas");
                colNames.Add("Clave Manual");
                foreach (var ce in CamposExtraSubProceso)
                {
                    if (ce.es_visible)
                    {
                        colNames.Add(ce.nb_campo);
                    }
                }
                colNames.Add("Código de Riesgo");
                colNames.Add("Evento");
                colNames.Add("Factores de Riesgo");
                colNames.Add("Código de Fallas");
                colNames.Add("Clase de Evento");
                colNames.Add("Tipo de Evento");
                colNames.Add("Sub Tipo de Evento");
                colNames.Add("Clasificación del Riesgo");
                colNames.Add("Magnitud de Impacto");
                colNames.Add("Probabilidad de Ocurrencia");
                colNames.Add("Criticidad");
                colNames.Add("Afectación Contable");
                colNames.Add("Supuesto Normativo");
                colNames.Add("EUC");
                colNames.Add("¿Es riesgo operativo?");
                colNames.Add("Proceso Riesgo Operativo");
                colNames.Add("Sub Tipo Producto Producto Riesgo Operativo");
                colNames.Add("Sub Tipo Riesgo Operativo");
                colNames.Add("Línea Negocio Riesgo Operativo");
                colNames.Add("Frecuencia Riesgo Operativo");
                colNames.Add("Impacto Riesgo Operativo");
                colNames.Add("Criticidad Riesgo Operativo");
                foreach (var ce in CamposExtraRiesgo)
                {
                    if (ce.es_visible)
                    {
                        colNames.Add(ce.nb_campo);
                    }
                }
                colNames.Add("Código de Control");
                colNames.Add("Actividad del Control");
                colNames.Add("Evidencia del Control");
                colNames.Add("Aplicación");
                colNames.Add("Acción Correctora");
                colNames.Add("Naturaleza del Control");
                colNames.Add("Frecuencia del Control");
                colNames.Add("Categoría del Control");
                colNames.Add("Tipología del Control");
                colNames.Add("Tipo de Evidencia");
                colNames.Add("Grado de Cobertura");
                colNames.Add("Responsable Control");
                colNames.Add("Ejecutor Control");
                colNames.Add("¿Es Control Clave?");
                foreach (var ce in CamposExtraControl)
                {
                    if (ce.es_visible)
                    {
                        colNames.Add(ce.nb_campo);
                    }
                }

                rep14_1.ColNames = colNames;
                rep14_1.NoCols = colNames.Count;

                m_tipo = new MRyCRepModel().GetType();
                m_propiedades = m_tipo.GetProperties();

                foreach (var item in vista)
                {
                    row = new List<string>();
                    row.Add(item.en);
                    row.Add(item.respEn);
                    row.Add(item.mp);
                    row.Add(item.respMp);
                    row.Add(item.pr);
                    row.Add(item.respPr);
                    row.Add(item.sp);
                    row.Add(item.descripcionSp);
                    row.Add(item.respSp);
                    row.Add(item.SpAnterior);
                    row.Add(item.SpSiguiente);
                    row.Add(item.tipologia_sp);
                    row.Add(item.lineas_negocio);
                    row.Add(item.etapa);
                    row.Add(item.sub_Etapa);
                    row.Add(item.areas_involucradas);
                    row.Add(item.aplicaciones_relacionadas);
                    row.Add(item.clave_manual);

                    for (int i = 0; i < 20; i++)
                    {
                        if (CamposExtraSubProceso[i].es_visible)
                        {
                            var prop = m_propiedades.Where(p => p.Name == string.Format("campo_extra_sp{0:00}", i + 1)).First();
                            try
                            {
                                row.Add((prop.GetValue(item, null) ?? "").ToString());
                            }
                            catch
                            {
                                row.Add("");
                            }
                        }
                    }

                    row.Add(item.cl_riesgo);
                    row.Add(item.evento_riesgo);
                    row.Add(item.categoria_riesgo);
                    row.Add(item.tipo_riesgo);
                    row.Add(item.clase_tipologia_riesgo);
                    row.Add(item.sub_clase_tipologia_riesgo);
                    row.Add(item.tipologia_riesgo);
                    row.Add(item.tipo_impacto);
                    row.Add(item.magnitud_impacto);
                    row.Add(item.probabilidad_ocurrencia);
                    row.Add(item.criticidad);
                    row.Add(item.afectacion_contable);
                    row.Add(item.supuesto_normativo);
                    row.Add(item.euc);

                    row.Add(item.es_riesgo_operativo);
                    row.Add(item.nb_proceso_ro);
                    row.Add(item.nb_producto_ro);
                    row.Add(item.nb_sub_tipo_ro);
                    row.Add(item.nb_linea_negocio_ro);
                    row.Add(item.nb_frecuencia_ro);
                    row.Add(item.nb_impacto_ro);
                    row.Add(item.criticidad_ro);

                    for (int i = 0; i < 20; i++)
                    {
                        if (CamposExtraRiesgo[i].es_visible)
                        {
                            var prop = m_propiedades.Where(p => p.Name == string.Format("campo_extra_r{0:00}", i + 1)).First();
                            try
                            {
                                row.Add((prop.GetValue(item, null) ?? "").ToString());
                            }
                            catch
                            {
                                row.Add("");
                            }
                        }
                    }

                    row.Add(item.cl_control);
                    row.Add(item.actividad_control);
                    row.Add(item.evidencia_control);
                    row.Add(item.aplicacion);
                    row.Add(item.accion_correctora);
                    row.Add(item.naturaleza_control);
                    row.Add(item.frecuencia_control);
                    row.Add(item.categoria_control);
                    row.Add(item.tipologia_control);
                    row.Add(item.tipo_evidencia);
                    row.Add(item.grado_cobertura);
                    row.Add(item.responsable_control);
                    row.Add(item.ejecutor_control);
                    row.Add(item.control_clave);

                    for (int i = 0; i < 20; i++)
                    {
                        if (CamposExtraControl[i].es_visible)
                        {
                            var prop = m_propiedades.Where(p => p.Name == string.Format("campo_extra_c{0:00}", i + 1)).First();
                            try
                            {
                                row.Add((prop.GetValue(item, null) ?? "").ToString());
                            }
                            catch
                            {
                                row.Add("");
                            }
                        }
                    }



                    rep14_1.Rows.Add(row);
                }

                rep14_1Json = (JObject)JToken.FromObject(rep14_1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.StackTrace, "Respaldo reporte de General MRyC");
            }


            #endregion

            #region rep14_2 Riesgos Derogaos
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Riesgos Derogaos");

            JObject rep14_2Json = new JObject();
            try
            {
                colNames = new List<string>();

                colNames.Add("Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Justificación");

                rep14_2.NoCols = colNames.Count;
                rep14_2.ColNames = colNames;

                foreach (var riesgo in riesgoDerogado)
                {

                    row = new List<string>();

                    row.Add(riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso + " - " + riesgo.c_sub_proceso.c_proceso.c_macro_proceso.nb_macro_proceso);
                    row.Add(riesgo.c_sub_proceso.c_proceso.cl_proceso + " - " + riesgo.c_sub_proceso.c_proceso.nb_proceso);
                    row.Add(riesgo.c_sub_proceso.cl_sub_proceso + " - " + riesgo.c_sub_proceso.nb_sub_proceso);
                    row.Add(riesgo.evento);
                    row.Add(riesgo.justificacion);

                    rep14_2.Rows.Add(row);
                }

                rep14_2Json = (JObject)JToken.FromObject(rep14_2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Riesgos Derogaos");
            }


            #endregion

            #endregion

            #region Sub Proceso Normatividad

            var Normatividades = db.c_normatividad.ToList();
            List<ConfiguracionesEventosViewModel.Config0001> config0001s = new List<ConfiguracionesEventosViewModel.Config0001>();

            var fichasNorm = db.r_evento.Where(r => r.tipo == "0001");
            foreach (var ficha in fichasNorm)
            {
                var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);
                config0001s.Add(conf);
            }


            foreach (var norm in Normatividades)
            {
                percentaje = (repCounter++ * 100) / totalReportes;
                Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Sub Procesos - Normatividad: " + norm.nb_normatividad);

                try
                {
                    var raiz = db.c_contenido_normatividad.Find(norm.id_root_contenido);
                    var niveles = db.c_nivel_normatividad.Where(n => n.id_normatividad == norm.id_normatividad).OrderBy(n => n.no_orden).ToList();
                    int nNiveles = niveles.Count() - 1;
                    List<c_contenido_normatividad> contenidos = new List<c_contenido_normatividad>();
                    contenidos = Utilidades.Utilidades.getContents(raiz);

                    Report aux15 = new Report();

                    aux15.NoCols = 1;
                    //Nombres de Columnas
                    colNames = new List<string>();

                    foreach (var nivel in niveles)
                    {
                        colNames.Add(nivel.cl_nivel_normatividad);
                        colNames.Add(nivel.nb_nivel_normatividad);
                        aux15.NoCols += 2;
                    }
                    colNames.Add("¿Tiene Ficha?");
                    colNames.Add("Sub Procesos Ligados");
                    aux15.ColNames = colNames;
                    //Filas
                    foreach (var cont in contenidos)
                    {
                        row = new List<string>();
                        var celdasIniciales = Utilidades.Utilidades.CeldasAnterioresNormL(cont);
                        var i = cont.c_nivel_normatividad.no_orden;

                        row.AddRange(celdasIniciales);

                        row.Add(cont.cl_contenido_normatividad);
                        row.Add(cont.ds_contenido_normatividad);
                        for (int j = i; j < nNiveles; j++)
                        {
                            row.Add("");
                            row.Add("");
                        }
                        row.Add(config0001s.Exists(c => c.id == cont.id_contenido_normatividad) ? "Si" : "No");
                        row.Add(Utilidades.Utilidades.SubProcesosLigados(cont));
                        aux15.Rows.Add(row);
                    }

                    aux15.RepName = "Reporte de Sub Procesos - Normatividad: " + norm.nb_normatividad;
                    rep15.Add(aux15);
                }
                catch (Exception e)
                {
                    Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Normatividad: " + norm.nb_normatividad);
                }


            }

            List<JObject> rep15Json = new List<JObject>();

            string compiladoSPNM = "";

            foreach (var rep in rep15)
            {
                var ax15 = (JObject)JToken.FromObject(rep);
                rep15Json.Add(ax15);

                compiladoSPNM += ax15.ToString();
            }
            #endregion

            #region Rep Sub Actividades Costeo
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Sub Actividades de Costeo");

            JObject rep16Json = new JObject();
            try
            {
                var model16 = db.c_sub_proceso.OrderBy(sp => sp.c_proceso.c_macro_proceso.c_entidad.cl_entidad)
                .OrderBy(sp => sp.c_proceso.c_macro_proceso.cl_macro_proceso)
                .OrderBy(sp => sp.c_proceso.cl_proceso)
                .OrderBy(sp => sp.cl_sub_proceso)
                .ToList();

                var actividades = db.c_area_costeo.ToList();


                rep16.enableHeadConfig = true;
                rep16.NoRowsHeader = 3;

                rep16.RepName = "Reporte de Sub Actividades de Costeo";

                var cellInfo = new HeaderConfig();
                var cellInfo2 = new HeaderConfig();
                var cellInfo3 = new HeaderConfig();

                cellInfo.colspan = 1;
                cellInfo.rowspan = 3;
                cellInfo.noFila = 1;

                cellInfo2.colspan = 1;
                cellInfo2.rowspan = 1;
                cellInfo2.noFila = 3;

                colNames = new List<string>();

                colNames.Add("Entidad");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Entidad");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Macro Proceso");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Macro Proceso");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Proceso");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Proceso");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Sub Proceso");
                rep16.HeadConfig.Add(cellInfo);
                colNames.Add("Responsable Sub Proceso");
                rep16.HeadConfig.Add(cellInfo);

                rep16.NoCols = 8;
                foreach (var ac in actividades)
                {
                    var ac2L = ac.c_area_costeo_n2.ToList();
                    List<c_area_costeo_n3> ac3L = new List<c_area_costeo_n3>();
                    foreach (var ac2 in ac2L)
                    {
                        ac3L.AddRange(ac2.c_area_costeo_n3.ToList());
                    }
                    if (ac3L.Count() > 0)
                    {
                        var cellAux = new HeaderConfig()
                        {
                            colspan = ac3L.Count(),
                            noFila = 1,
                            rowspan = 1,
                        };

                        colNames.Add(ac.nb_area_costeo);
                        rep16.HeadConfig.Add(cellAux);
                    }
                }

                foreach (var ac in actividades)
                {
                    var ac2L = ac.c_area_costeo_n2.ToList();
                    foreach (var ac2 in ac2L)
                    {
                        if (ac2.c_area_costeo_n3.Count() > 0)
                        {
                            var cellAux = new HeaderConfig()
                            {
                                colspan = ac2.c_area_costeo_n3.Count(),
                                noFila = 2,
                                rowspan = 1,
                            };

                            colNames.Add(ac2.nb_area_costeo_n2);
                            rep16.HeadConfig.Add(cellAux);
                        }
                    }
                }

                foreach (var ac in actividades)
                {
                    var ac2L = ac.c_area_costeo_n2.ToList();
                    foreach (var ac2 in ac2L)
                    {
                        foreach (var ac3 in ac2.c_area_costeo_n3)
                        {
                            colNames.Add(ac3.nb_area_costeo_n3);
                            rep16.HeadConfig.Add(cellInfo2);
                            rep16.NoCols++;
                        }
                    }
                }

                rep16.ColNames = colNames;

                foreach (var sp in model16)
                {
                    var p = sp.c_proceso;
                    var mp = p.c_macro_proceso;
                    var e = mp.c_entidad;

                    row = new List<string>();
                    row.Add(e.cl_entidad + "-" + e.nb_entidad);
                    row.Add(e.c_usuario.nb_usuario);
                    row.Add(mp.cl_macro_proceso + " - " + @mp.nb_macro_proceso);
                    row.Add(mp.c_usuario.nb_usuario);
                    row.Add(p.cl_proceso + " - " + @p.nb_proceso);
                    row.Add(p.c_usuario.nb_usuario);
                    row.Add(sp.cl_sub_proceso + " - " + @sp.nb_sub_proceso);
                    row.Add(sp.c_usuario.cl_usuario + " - " + @sp.c_usuario.nb_usuario);


                    foreach (var ac in actividades)
                    {
                        var ac2L = ac.c_area_costeo_n2.ToList();
                        if (sp.c_area_costeo_sub_proceso.Where(axc => axc.id_area_costeo == ac.id_area_costeo).Count() > 0)
                        {

                            foreach (var ac2 in ac2L)
                            {
                                var ac3L = ac2.c_area_costeo_n3.ToList();

                                foreach (var ac3 in ac3L)
                                {
                                    if (sp.c_area_costeo_n3_sub_proceso.Where(axc3 => axc3.id_area_costeo_n3 == ac3.id_area_costeo_n3).Count() > 0)
                                    {
                                        row.Add(((double)sp.c_area_costeo_n3_sub_proceso.Where(axc3 => axc3.id_area_costeo_n3 == ac3.id_area_costeo_n3).First().porcentaje).ToString().Replace(",", "."));
                                    }
                                    else
                                    {
                                        row.Add("0");
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var ac2 in ac2L)
                            {
                                var ac3L = ac2.c_area_costeo_n3.ToList();
                                foreach (var ac3 in ac3L)
                                {
                                    row.Add("N / A");
                                }
                            }


                        }
                    }

                    rep16.Rows.Add(row);
                }




                rep16Json = (JObject)JToken.FromObject(rep16);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de sub Actividades de costeo");
            }


            #endregion

            #region Rep Personal     
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Personal");

            JObject rep17Json = new JObject();
            try
            {
                var repPersonal = db.c_usuario.ToList();

                colNames = new List<string>();
                colNames.Add("Nombre");
                colNames.Add("Puesto");
                colNames.Add("Correo Electrónico");
                colNames.Add("Área");
                colNames.Add("Jefe Directo");
                colNames.Add("Puesto Jefe Directo");
                colNames.Add("¿Es super Usuario?");
                colNames.Add("¿Es de solo lectura?");
                colNames.Add("¿Está activo?");
                colNames.Add("Roles");
                colNames.Add("Funciones");
                colNames.Add("Conteo de Macro Procesos");
                colNames.Add("Conteo de Procesos");
                colNames.Add("Conteo de Sub Procesos");
                colNames.Add("Conteo de Controles");
                colNames.Add("Conteo de Indicadores");
                colNames.Add("Conteo de Oficios");
                colNames.Add("Conteo de Informes");
                colNames.Add("Conteo de Incidencias");
                colNames.Add("Conteo de Planes de Remediación");
                colNames.Add("Conteo de Fichas");
                colNames.Add("Tiempo total registrado");


                rep17.RepName = "Reporte de Personal";

                rep17.ColNames = colNames;
                rep17.NoCols = colNames.Count;

                foreach (var item in repPersonal)
                {
                    var roles = item.c_rol.ToList();
                    List<c_funcion> funciones = new List<c_funcion>();
                    string rls = "";
                    string fcs = "";
                    foreach (var r in roles)
                    {
                        rls += r.nb_rol + ", ";
                        funciones = funciones.Union(r.c_funcion).ToList();
                    }
                    foreach (var f in funciones)
                    {
                        fcs += f.nb_funcion + ", ";
                    }

                    var tiempos = item.c_usuario_sub_proceso;
                    int tiempoTotal = 0;
                    foreach (var t in tiempos)
                    {
                        tiempoTotal += t.tiempo_sub_proceso;
                    }

                    row = new List<string>();

                    row.Add(item.nb_usuario);
                    row.Add(Utilidades.Utilidades.PuestoUsuario(item.id_usuario));
                    row.Add(item.e_mail_principal);
                    row.Add(item.c_area.nb_area);
                    row.Add(Utilidades.Utilidades.JefeDirecto(item.id_usuario).Replace("\n", "<br />"));
                    row.Add(Utilidades.Utilidades.PuestoJefeDirecto(item.id_usuario));
                    row.Add(item.es_super_usuario ? "Si" : "No");
                    row.Add(item.solo_lectura ? "Si" : "No");
                    row.Add(item.esta_activo ? "Si" : "No");
                    row.Add(rls);
                    row.Add(fcs);
                    row.Add(item.c_macro_proceso.Count.ToString());
                    row.Add(item.c_proceso.Count.ToString());
                    row.Add(item.c_sub_proceso.Count.ToString());
                    row.Add(item.k_control1.Count.ToString());
                    row.Add(item.c_indicador.Count.ToString());
                    row.Add(item.k_objeto.Where(o => o.tipo_objeto == 1).Count().ToString());
                    row.Add(item.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).Count().ToString());
                    row.Add(item.k_incidencia.Count.ToString());
                    row.Add(item.k_plan.Count.ToString());
                    row.Add(item.r_evento.Count.ToString());
                    row.Add(tiempoTotal.ToString());

                    rep17.Rows.Add(row);
                }


                rep17Json = (JObject)JToken.FromObject(rep17);

            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Personal");
            }


            #endregion

            #region Rep Fichas    
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Fichas");

            JObject rep18Json = new JObject();
            try
            {
                var repFichas = db.r_evento.ToList();

                colNames = new List<string>();
                colNames.Add("Titulo");
                colNames.Add("Responsable");
                colNames.Add("Tipo de Ficha");
                colNames.Add("Registro Ligado");
                colNames.Add("Contenido");
                colNames.Add("Tipo de Recordatorio");
                colNames.Add("Fecha Límite");
                colNames.Add("Estado");

                rep18.RepName = "Reporte de Fichas";
                rep18.ColNames = colNames;
                rep18.NoCols = colNames.Count;

                foreach (var item in repFichas)
                {
                    var tipo = Utilidades.Utilidades.tipoFicha(item);
                    string registro_ligado = "";
                    var reg = Utilidades.Utilidades.GetLastReg(item, db);

                    if (item.tipo == "0001")
                    {
                        ConfiguracionesEventosViewModel.Config0001 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(item.config);
                        var cont = db.c_contenido_normatividad.Find(conf.id);
                        var norm = Utilidades.Utilidades.getRoot(db, cont);

                        if (cont.id_contenido_normatividad_padre != null)
                        {
                            registro_ligado = cont.cl_contenido_normatividad + " de la normatividad " + norm.ds_contenido_normatividad;
                        }
                        else
                        {
                            registro_ligado = "Normatividad " + norm.ds_contenido_normatividad;
                        }

                    }
                    else
                    {
                        registro_ligado = "N/A";
                    }

                    row = new List<string>();

                    row.Add(item.nb_evento);
                    row.Add(item.c_usuario.nb_usuario);
                    row.Add(tipo);
                    row.Add(registro_ligado);
                    row.Add(item.ds_evento);
                    row.Add((item.recordar_antes_de_vencer ? "Único" : "Recurrente"));
                    row.Add(Utilidades.Utilidades.getFeLim(item).ToString());
                    row.Add(Utilidades.Utilidades.GetStatus(item));

                    rep18.Rows.Add(row);
                }


                rep18Json = (JObject)JToken.FromObject(rep18);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Fichas");
            }


            #endregion

            #region Rep Indicadores Diarios    
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Indicadores Diarios");

            JObject rep19Json = new JObject();
            try
            {
                var repIndicadoresDiarios = db.k_evaluacion_diaria.OrderBy(e => e.fe_evaluacion).ToList();

                colNames = new List<string>();
                colNames.Add("Indicador");
                colNames.Add("Nombre");
                colNames.Add("Descripción");
                colNames.Add("Descripción Numerador");
                colNames.Add("Descripción Denominador");
                colNames.Add("Umbral(0.0)");
                colNames.Add("Umbral(0.0 - 5.0)");
                colNames.Add("Umbral(5.0 - 7.5)");
                colNames.Add("Umbral(7.5 - 10.0)");
                colNames.Add("Activo");
                colNames.Add("Usuario");
                colNames.Add("Sucursal / Área");
                colNames.Add("Contenido de Grupo");
                colNames.Add("Fecha de Evaluación");
                colNames.Add("Numerador");
                colNames.Add("Denominador");
                colNames.Add("Porcentaje");
                colNames.Add("Calificación");


                rep19.RepName = "Reporte de Indicadores Diarios";
                rep19.ColNames = colNames;
                rep19.NoCols = colNames.Count;

                foreach (var item in repIndicadoresDiarios)
                {
                    var ind = item.c_indicador_diario;
                    var cont = item.c_contenido_grupo;
                    var user = item.c_usuario;

                    var calificacion = SCIRA.Utilidades.Utilidades.EvalNote(item);

                    string numerador = "", denominador = "", porcentaje = "", calif = "";

                    if (calificacion != 4)
                    {

                        numerador = ((double)item.numerador).ToString();
                        denominador = ((double)item.denominador).ToString();

                        var medicion = (item.numerador / item.denominador) * 100;
                        porcentaje = ((double)medicion).ToString() + "%";
                    }
                    else
                    {
                        numerador = denominador = porcentaje = "N/A";
                    }

                    switch (calificacion)
                    {
                        case 1:
                            calif = "Bueno";
                            break;
                        case 2:
                            calif = "Regular";
                            break;
                        case 3:
                            calif = "Malo";
                            break;
                        case 4:
                            calif = "No calificado";
                            break;
                    }
                    row = new List<string>();


                    row.Add(ind.cl_indicador_diario);
                    row.Add(ind.nb_indicador_diario);
                    row.Add(ind.ds_indicador_diario);
                    row.Add(ind.ds_numerador_diario);
                    row.Add(ind.ds_denominador);
                    row.Add(((float)ind.umbral000i).ToString().Replace(",", ".") + " - " + ((float)ind.umbral000f).ToString().Replace(",", "."));
                    row.Add(((float)ind.umbral050i).ToString().Replace(",", ".") + " - " + ((float)ind.umbral050f).ToString().Replace(",", "."));
                    row.Add(((float)ind.umbral075i).ToString().Replace(",", ".") + " - " + ((float)ind.umbral075f).ToString().Replace(",", "."));
                    row.Add(((float)ind.umbral100i).ToString().Replace(",", ".") + " - " + ((float)ind.umbral100f).ToString().Replace(",", "."));
                    row.Add(ind.esta_activo ? "Si" : "No");
                    row.Add(user.cl_usuario + " - " + user.nb_usuario);
                    row.Add(user.c_area.cl_area + " - " + user.c_area.nb_area);
                    row.Add(SCIRA.Utilidades.Utilidades.getRuta(cont));
                    row.Add(((DateTime)item.fe_evaluacion).ToShortDateString());
                    row.Add(numerador);
                    row.Add(denominador);
                    row.Add(porcentaje);
                    row.Add(calif);

                    rep19.Rows.Add(row);
                }


                rep19Json = (JObject)JToken.FromObject(rep19);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Indicadores Diarios");
            }


            #endregion

            #region Rep Revisión Control  
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Revisión de Controles");

            JObject rep20_1Json = new JObject();
            try
            {
                var ctrls = db.k_control.Where(c => !c.tiene_accion_correctora).ToList();

                //no añadir controles que provengan de un MG
                ctrls = ctrls.Where(c => c.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) != "MG").ToList();

                //controles sin revision
                var controlesSR = ctrls.Where(c => c.k_revision_control.Count() == 0).ToList();
                var revisiones = db.k_revision_control.ToList();

                var icec1 = CamposExtraControl.ElementAt(0);
                var icec2 = CamposExtraControl.ElementAt(1);
                var icec3 = CamposExtraControl.ElementAt(2);

                var calificaciones = db.c_calificacion.ToList();


                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Entidad Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Macro Proceso");
                colNames.Add("Macro Proceso Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Proceso");
                colNames.Add("Proceso Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Sub Proceso");
                colNames.Add("Sub Proceso Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Tarea");
                colNames.Add("Tarea Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Nombre del Revisor");

                colNames.Add("Código del Riesgo");
                colNames.Add("Evento de Riesgo");
                colNames.Add("Evento de Riesgo Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Magnitud de Impacto");
                colNames.Add("Magnitud de Impacto Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Probabilidad de Ocurrencia");
                colNames.Add("Probabilidad de Ocurrencia Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");

                colNames.Add("Código del Control");
                colNames.Add("Código del Manual");
                colNames.Add("Código del Manual Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Actividad del Control");
                colNames.Add("Actividad del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Evidencia del Control");
                colNames.Add("Evidencia del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("@icec1.nb_campo");
                colNames.Add("@icec2.nb_campo");
                colNames.Add("@icec1.nb_campo y @icec2.nb_campo Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("@icec3.nb_campo");
                colNames.Add("@icec3.nb_campo Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Área Responsable");
                colNames.Add("Área Responsable Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Responsable");
                colNames.Add("Responsable Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Puesto Responsable");
                colNames.Add("Puesto Responsable Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Ejecutor");
                colNames.Add("Ejecutor Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Puesto Ejecutor");
                colNames.Add("Puesto Ejecutor Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Tipología del Control");
                colNames.Add("Tipología del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Frecuencia del Control");
                colNames.Add("Frecuencia del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Aseveraciones");
                colNames.Add("Aseveraciones Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("¿Es Control Clave?");
                colNames.Add("¿Es Control Clave? Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("¿Depende de otro Control?");
                colNames.Add("¿Depende de otro Control? Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");


                colNames.Add("Información de Entrada"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Naturaleza del Control");
                colNames.Add("Naturaleza del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Tipología del Control");
                colNames.Add("Tipología del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Tipo de Evidencia");
                colNames.Add("Tipo de Evidencia Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Categoría del Control");
                colNames.Add("Categoría del Control Revisión"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Procedimientos realizados para probar su adecuado diseño"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("COSO 1 - Idoneidad del propósito del control y su correlación con el riesgo asociado y aseveración"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("COSO 2 - Competencia y autoridad del responsable del control"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("COSO 3 - Frecuencia y consistencia con la cual se realiza el control"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("COSO 4 - Nivel de agregación y predictibilidad"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("COSO 5 - Criterios de Investigación (umbrales) y procesos de seguimiento predictibilidad"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Calificación del control en cuanto a su diseño");
                colNames.Add("¿El diseño del control es correcto?"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Conclusión de efectividad del diseño"); colNames.Add("Observaciones");
                colNames.Add("Calificación");

                colNames.Add("Calificación del control en cuanto a su efectividad");
                colNames.Add("¿El control es eficaz?"); colNames.Add("Observaciones");
                colNames.Add("Calificación");
                colNames.Add("Conclusión de efectividad eficacia operativa"); colNames.Add("Observaciones");
                colNames.Add("Calificación");

                colNames.Add("Conclusión General"); colNames.Add("Observaciones");
                colNames.Add("Estatus");
                colNames.Add("Comentarios de Revisión");

                foreach (var calif in calificaciones)
                {
                    colNames.Add("No. campos " + calif.nb_calificacion);
                }

                colNames.Add("No. campos no Calificados");



                rep20_1.RepName = "Reporte de Revisión de Controles";
                rep20_1.ColNames = colNames;
                rep20_1.NoCols = colNames.Count;

                foreach (var r in revisiones)
                {
                    var control = r.k_control;
                    var riesgo = control.k_riesgo.First();
                    var sp = control.c_sub_proceso;
                    var pr = sp.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var entidad = mp.c_entidad;
                    var responsable = control.c_usuario1;
                    var ejecutor = control.c_usuario;
                    var area = responsable.c_area;

                    var rrData = Utilidades.Utilidades.RRData(control);

                    string asev = "";
                    string asevr = "";

                    foreach (var A in control.c_aseveracion)
                    {
                        asev += A.nb_aseveracion + "\n";
                    }

                    foreach (var A in r.c_aseveracion)
                    {
                        asevr += A.nb_aseveracion + "\n";
                    }


                    var rcTipologia = "";
                    var rcFrecuencia = "";

                    try
                    {
                        rcTipologia = r.c_tipologia_control.cl_tipologia_control + " - " + r.c_tipologia_control.nb_tipologia_control;
                    }
                    catch { }
                    try
                    {
                        rcFrecuencia = r.c_frecuencia_control.cl_frecuencia_control + " - " + r.c_frecuencia_control.nb_frecuencia_control;
                    }
                    catch { }


                    var edcNaturaleza = "";
                    var edcTipologia = "";
                    var edcTipoEvidencia = "";
                    var edcCategoriaControl = "";

                    try
                    {
                        edcNaturaleza = r.c_naturaleza_control.cl_naturaleza_control + " - " + r.c_naturaleza_control.nb_naturaleza_control;
                    }
                    catch { }
                    try
                    {
                        edcTipologia = r.c_tipologia_control1.cl_tipologia_control + " - " + r.c_tipologia_control1.nb_tipologia_control;
                    }
                    catch { }
                    try
                    {
                        edcTipoEvidencia = r.c_tipo_evidencia.cl_tipo_evidencia + " - " + r.c_tipo_evidencia.nb_tipo_evidencia;
                    }
                    catch { }
                    try
                    {
                        edcCategoriaControl = r.c_categoria_control.cl_categoria_control + " - " + r.c_categoria_control.nb_categoria_control;
                    }
                    catch { }


                    m_tipo = null;
                    m_propiedades = null;
                    m_tipo = r.GetType();
                    m_propiedades = m_tipo.GetProperties();

                    int[] ncpc = new int[calificaciones.Count()];
                    for (int i = 0; i < calificaciones.Count(); i++)
                    {
                        ncpc[i] = 0;
                    }

                    int prueba;
                    int NoCalificados = 0;

                    for (int i = 1; i < 41; i++)
                    {
                        if (i != 36)
                        {
                            if (i != 37)
                            {
                                string nb_campo = "id_cc" + i;

                                var prop = m_propiedades.Where(c => c.Name == nb_campo).First();
                                try
                                {
                                    prueba = int.Parse((prop.GetValue(r, null) ?? "").ToString());
                                }
                                catch
                                {
                                    prueba = 0;
                                }


                                if (prueba != 0)
                                {
                                    var calif = calificaciones.Where(c => c.id_calificacion == prueba).First();
                                    var index = calificaciones.IndexOf(calif);

                                    ncpc[index]++;
                                }
                                else
                                {
                                    NoCalificados++;
                                }
                            }
                        }
                    }


                    row = new List<string>();

                    row.Add(entidad.cl_entidad + " - " + entidad.nb_entidad);
                    row.Add(r.dg_entidad);
                    row.Add(r.dg_obs_1);
                    row.Add((r.c_calificacion != null ? r.c_calificacion.nb_calificacion.ToString() : ""));
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(r.dg_marco_proceso);
                    row.Add(r.dg_obs_2);
                    row.Add((r.c_calificacion1 != null ? r.c_calificacion1.nb_calificacion.ToString() : ""));
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(r.dg_proceso);
                    row.Add(r.dg_obs_3);
                    row.Add((r.c_calificacion2 != null ? r.c_calificacion2.nb_calificacion.ToString() : ""));
                    row.Add(sp.cl_sub_proceso + " - " + sp.nb_archivo_flujo);
                    row.Add(r.dg_sub_proceso);
                    row.Add(r.dg_obs_4);
                    row.Add((r.c_calificacion3 != null ? r.c_calificacion3.nb_calificacion.ToString() : ""));
                    row.Add(sp.ds_sub_proceso);
                    row.Add(r.dg_tarea);
                    row.Add(r.dg_obs_5);
                    row.Add((r.c_calificacion4 != null ? r.c_calificacion4.nb_calificacion.ToString() : ""));
                    row.Add(r.dg_nb_responsable_revision);

                    row.Add(riesgo.nb_riesgo);
                    row.Add(riesgo.evento);
                    row.Add(r.rr_evento_riesgo);
                    row.Add(r.rr_obs_1);
                    row.Add((r.c_calificacion5 != null ? r.c_calificacion5.nb_calificacion.ToString() : ""));
                    row.Add(riesgo.c_magnitud_impacto.cl_magnitud_impacto + " - " + riesgo.c_magnitud_impacto.nb_magnitud_impacto);
                    row.Add(r.rr_magnitud_impacto);
                    row.Add(r.rr_obs_2);
                    row.Add((r.c_calificacion6 != null ? r.c_calificacion6.nb_calificacion.ToString() : ""));
                    row.Add(riesgo.c_probabilidad_ocurrencia.cl_probabilidad_ocurrencia + " - " + riesgo.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia);
                    row.Add(r.rr_probabilidad_ocurrencia);
                    row.Add(r.rr_obs_3);
                    row.Add((r.c_calificacion7 != null ? r.c_calificacion7.nb_calificacion.ToString() : ""));


                    row.Add(control.relacion_control);
                    row.Add(sp.cl_manual);
                    row.Add(r.rc_codigo_manual);
                    row.Add(r.rc_obs_1);
                    row.Add((r.c_calificacion8 != null ? r.c_calificacion8.nb_calificacion.ToString() : ""));
                    row.Add(control.actividad_control);
                    row.Add(r.rc_actividad_control);
                    row.Add(r.rc_obs_2);
                    row.Add((r.c_calificacion9 != null ? r.c_calificacion9.nb_calificacion.ToString() : ""));
                    row.Add(control.evidencia_control);
                    row.Add(r.rc_evidencia_control);
                    row.Add(r.rc_obs_3);
                    row.Add((r.c_calificacion39 != null ? r.c_calificacion39.nb_calificacion.ToString() : ""));
                    row.Add(control.campo01);
                    row.Add(control.campo02);
                    row.Add(r.rc_ds_control);
                    row.Add(r.rc_obs_4);
                    row.Add((r.c_calificacion10 != null ? r.c_calificacion10.nb_calificacion.ToString() : ""));
                    row.Add(control.campo03);
                    row.Add(r.rc_dir_general);
                    row.Add(r.rc_obs_5);
                    row.Add((r.c_calificacion11 != null ? r.c_calificacion11.nb_calificacion.ToString() : ""));
                    row.Add(area.cl_area + " - " + area.nb_area);
                    row.Add(r.rc_area);
                    row.Add(r.rc_obs_6);
                    row.Add((r.c_calificacion12 != null ? r.c_calificacion12.nb_calificacion.ToString() : ""));
                    row.Add(responsable.nb_usuario);
                    row.Add(r.rc_responsable);
                    row.Add(r.rc_obs_7);
                    row.Add((r.c_calificacion13 != null ? r.c_calificacion13.nb_calificacion.ToString() : ""));
                    row.Add((Utilidades.Utilidades.PuestoUsuario(responsable.id_usuario)));
                    row.Add(r.rc_puesto_responsable);
                    row.Add(r.rc_obs_8);
                    row.Add((r.c_calificacion14 != null ? r.c_calificacion14.nb_calificacion.ToString() : ""));
                    row.Add(ejecutor.nb_usuario);
                    row.Add(r.rc_ejecutor);
                    row.Add(r.rc_obs_9);
                    row.Add((r.c_calificacion15 != null ? r.c_calificacion15.nb_calificacion.ToString() : ""));
                    row.Add((Utilidades.Utilidades.PuestoUsuario(ejecutor.id_usuario)));
                    row.Add(r.rc_puesto_ejecutor);
                    row.Add(r.rc_obs_10);
                    row.Add((r.c_calificacion16 != null ? r.c_calificacion16.nb_calificacion.ToString() : ""));
                    row.Add(control.c_tipologia_control.cl_tipologia_control + " - " + control.c_tipologia_control.nb_tipologia_control);
                    row.Add(rcTipologia);
                    row.Add(r.rc_obs_11);
                    row.Add((r.c_calificacion17 != null ? r.c_calificacion17.nb_calificacion.ToString() : ""));
                    row.Add(control.c_frecuencia_control.cl_frecuencia_control + " - " + control.c_frecuencia_control.nb_frecuencia_control);
                    row.Add(rcFrecuencia);
                    row.Add(r.rc_obs_12);
                    row.Add((r.c_calificacion18 != null ? r.c_calificacion18.nb_calificacion.ToString() : ""));
                    row.Add(asev.Replace("\n", "<br/>"));
                    row.Add(asevr.Replace("\n", "<br/>"));
                    row.Add(r.rc_obs_13);
                    row.Add((r.c_calificacion19 != null ? r.c_calificacion19.nb_calificacion.ToString() : ""));
                    row.Add((control.es_control_clave ? "Si" : "No"));
                    row.Add(r.rc_obs_14);
                    row.Add((r.rc_control_clave ? "Si" : "No"));
                    row.Add((r.c_calificacion20 != null ? r.c_calificacion20.nb_calificacion.ToString() : ""));
                    row.Add(("N/A"));
                    row.Add((r.rc_control_dependiente ? "Si" : "No"));
                    row.Add(r.rc_obs_15);
                    row.Add((r.c_calificacion21 != null ? r.c_calificacion21.nb_calificacion.ToString() : ""));



                    row.Add(r.edc_informacion_inputs);
                    row.Add(r.edc_obs_1);
                    row.Add((r.c_calificacion22 != null ? r.c_calificacion22.nb_calificacion.ToString() : ""));
                    row.Add(control.c_naturaleza_control.cl_naturaleza_control + " - " + control.c_naturaleza_control.nb_naturaleza_control);
                    row.Add(edcNaturaleza);
                    row.Add(r.edc_obs_2);
                    row.Add((r.c_calificacion23 != null ? r.c_calificacion23.nb_calificacion.ToString() : ""));
                    row.Add(control.c_tipologia_control.cl_tipologia_control + " - " + control.c_tipologia_control.nb_tipologia_control);
                    row.Add(edcTipologia);
                    row.Add(r.edc_obs_3);
                    row.Add((r.c_calificacion24 != null ? r.c_calificacion24.nb_calificacion.ToString() : ""));
                    row.Add(control.c_tipo_evidencia.cl_tipo_evidencia + " - " + control.c_tipo_evidencia.nb_tipo_evidencia);
                    row.Add(edcTipoEvidencia);
                    row.Add(r.edc_obs_4);
                    row.Add((r.c_calificacion25 != null ? r.c_calificacion25.nb_calificacion.ToString() : ""));
                    row.Add(control.c_categoria_control.cl_categoria_control + " - " + control.c_categoria_control.nb_categoria_control);
                    row.Add(edcCategoriaControl);
                    row.Add(r.edc_obs_5);
                    row.Add((r.c_calificacion26 != null ? r.c_calificacion26.nb_calificacion.ToString() : ""));
                    row.Add(r.edc_proc_cert);
                    row.Add(r.edc_obs_6);
                    row.Add((r.c_calificacion27 != null ? r.c_calificacion27.nb_calificacion.ToString() : ""));
                    row.Add(r.edc_fc1);
                    row.Add(r.edc_obs_7);
                    row.Add((r.c_calificacion28 != null ? r.c_calificacion28.nb_calificacion.ToString() : ""));
                    row.Add(r.edc_fc2);
                    row.Add(r.edc_obs_8);
                    row.Add((r.c_calificacion29 != null ? r.c_calificacion29.nb_calificacion.ToString() : ""));
                    row.Add(r.edc_fc3);
                    row.Add(r.edc_obs_9);
                    row.Add((r.c_calificacion30 != null ? r.c_calificacion30.nb_calificacion.ToString() : ""));
                    row.Add(r.edc_fc4);
                    row.Add(r.edc_obs_10);
                    row.Add((r.c_calificacion31 != null ? r.c_calificacion31.nb_calificacion.ToString() : ""));
                    row.Add(r.edc_fc5);
                    row.Add(r.edc_obs_11);
                    row.Add((r.c_calificacion32 != null ? r.c_calificacion32.nb_calificacion.ToString() : ""));
                    row.Add(rrData[0]);
                    row.Add((r.edc_diseño_efectivo ? "Si" : "No"));
                    row.Add(r.edc_obs_12);
                    row.Add((r.c_calificacion33 != null ? r.c_calificacion33.nb_calificacion.ToString() : ""));
                    row.Add((r.edc_efectividad));
                    row.Add(r.edc_obs_13);
                    row.Add((r.c_calificacion34 != null ? r.c_calificacion34.nb_calificacion.ToString() : ""));

                    row.Add(rrData[1]);
                    row.Add((r.eeo_diseño_efectivo ? "Si" : "No"));
                    row.Add((r.eeo_obs_1));
                    row.Add((r.c_calificacion37 != null ? r.c_calificacion37.nb_calificacion.ToString() : ""));
                    row.Add((r.eeo_efectividad));
                    row.Add((r.eeo_obs_2));
                    row.Add((r.c_calificacion38 != null ? r.c_calificacion38.nb_calificacion.ToString() : ""));

                    row.Add(r.cg_conclusion);
                    row.Add((r.cg_obs_1));
                    row.Add(r.c_calificacion_revision != null ? r.c_calificacion_revision.nb_calificacion_revision : "Sin Calificar");
                    row.Add(r.cg_comentarios_revision);


                    for (int i = 0; i < calificaciones.Count(); i++)
                    {
                        row.Add(ncpc[i].ToString());
                    }

                    row.Add(NoCalificados.ToString());


                    rep20_1.Rows.Add(row);
                }


                rep20_1Json = (JObject)JToken.FromObject(rep20_1);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Revisión de Controles");
            }




            JObject rep20_2Json = new JObject();
            try
            {
                //reporte de controles sin revisiones
                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Macro Proceso");
                colNames.Add("Proceso");
                colNames.Add("Sub Proceso");
                colNames.Add("Código de Riesgo");
                colNames.Add("Código de Control");

                var ctrls = db.k_control.Where(c => !c.tiene_accion_correctora).ToList();

                //no añadir controles que provengan de un MG
                ctrls = ctrls.Where(c => c.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) != "MG").ToList();
                var controlesSR = ctrls.Where(c => c.k_revision_control.Count() == 0).ToList();

                rep20_2.RepName = "Reporte de Controles sin Revisión";
                rep20_2.ColNames = colNames;
                rep20_2.NoCols = colNames.Count;

                foreach (var control in controlesSR)
                {
                    var riesgo = control.k_riesgo.First();
                    var sp = control.c_sub_proceso;
                    var pr = sp.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var entidad = mp.c_entidad;


                    row = new List<string>();


                    row.Add(entidad.cl_entidad + " - " + entidad.nb_entidad);
                    row.Add(mp.cl_macro_proceso + " - " + mp.cl_macro_proceso);
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(sp.cl_sub_proceso + " - " + sp.nb_sub_proceso);
                    row.Add(riesgo.nb_riesgo);
                    row.Add(control.relacion_control);

                    rep20_2.Rows.Add(row);
                }

                rep20_2Json = (JObject)JToken.FromObject(rep20_2);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Controles sin Revisión");
            }





            #endregion

            #region Rep Certificacion Entidades  
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Certificación de Entidades");

            JObject rep21Json = new JObject();
            try
            {
                var certsEnt = db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "E");


                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Periodo de Certificación");
                colNames.Add("Leyenda de Certificación");
                colNames.Add("Comentarios");
                colNames.Add("¿Certificación satisfactoria?");

                rep21.RepName = "Reporte de Certificación de Entidades";
                rep21.ColNames = colNames;
                rep21.NoCols = colNames.Count;

                foreach (var item in certsEnt)
                {
                    var en = item.c_entidad;
                    var periodo = item.c_periodo_certificacion;


                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(en.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(en.id_responsable));
                    row.Add(periodo.nb_periodo_certificacion + " - " + periodo.anio);
                    row.Add(item.leyenda_certificacion_estructura);
                    row.Add(item.comentarios);
                    row.Add(item.resultado ? "Si":"No");


                    rep21.Rows.Add(row);
                }


                rep21Json = (JObject)JToken.FromObject(rep21);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Certificación de Entidades");
            }
            #endregion

            #region Rep Certificacion Macro Procesos  
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Certificación de Macro Procesos");

            JObject rep22Json = new JObject();
            try
            {
                var certsEnt = db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "M");


                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Periodo de Certificación");
                colNames.Add("Leyenda de Certificación");
                colNames.Add("Comentarios");
                colNames.Add("¿Certificación satisfactoria?");

                rep22.RepName = "Reporte de Certificación de Macro Procesos";
                rep22.ColNames = colNames;
                rep22.NoCols = colNames.Count;

                foreach (var item in certsEnt)
                {
                    var mp = item.c_macro_proceso;
                    var en = mp.c_entidad;
                    var periodo = item.c_periodo_certificacion;


                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(en.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(en.id_responsable));
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(mp.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(mp.id_responsable));
                    row.Add(periodo.nb_periodo_certificacion + " - " + periodo.anio);
                    row.Add(item.leyenda_certificacion_estructura);
                    row.Add(item.comentarios);
                    row.Add(item.resultado ? "Si" : "No");


                    rep22.Rows.Add(row);
                }


                rep22Json = (JObject)JToken.FromObject(rep22);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Certificación de Macro Procesos");
            }
            #endregion

            #region Rep Certificacion Procesos  
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Certificación de Procesos");

            JObject rep23Json = new JObject();
            try
            {
                var certsEnt = db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "P");


                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Proceso");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Periodo de Certificación");
                colNames.Add("Leyenda de Certificación");
                colNames.Add("Comentarios");
                colNames.Add("¿Certificación satisfactoria?");

                rep23.RepName = "Reporte de Certificación de Procesos";
                rep23.ColNames = colNames;
                rep23.NoCols = colNames.Count;

                foreach (var item in certsEnt)
                {
                    var pr = item.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var en = mp.c_entidad;
                    var periodo = item.c_periodo_certificacion;


                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(en.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(en.id_responsable));
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(mp.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(mp.id_responsable));
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(pr.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(pr.id_responsable));
                    row.Add(periodo.nb_periodo_certificacion + " - " + periodo.anio);
                    row.Add(item.leyenda_certificacion_estructura);
                    row.Add(item.comentarios);
                    row.Add(item.resultado ? "Si" : "No");


                    rep23.Rows.Add(row);
                }


                rep23Json = (JObject)JToken.FromObject(rep23);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Certificación de Procesos");
            }
            #endregion

            #region Rep Certificacion Sub Procesos  
            percentaje = (repCounter++ * 100) / totalReportes;
            Utilidades.Utilidades.BackUpProgress1(percentaje.ToString(), "Respaldando Reporte de Certificación de Sub Procesos");

            JObject rep24Json = new JObject();
            try
            {
                var certsEnt = db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "S");


                colNames = new List<string>();

                colNames.Add("Entidad");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Macro Proceso");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Proceso");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Sub Proceso");
                colNames.Add("Responsable");
                colNames.Add("Puesto");
                colNames.Add("Periodo de Certificación");
                colNames.Add("Leyenda de Certificación");
                colNames.Add("Comentarios");
                colNames.Add("¿Certificación satisfactoria?");

                rep24.RepName = "Reporte de Certificación de Sub Procesos";
                rep24.ColNames = colNames;
                rep24.NoCols = colNames.Count;

                foreach (var item in certsEnt)
                {
                    var sp = item.c_sub_proceso;
                    var pr = sp.c_proceso;
                    var mp = pr.c_macro_proceso;
                    var en = mp.c_entidad;
                    var periodo = item.c_periodo_certificacion;


                    row = new List<string>();
                    row.Add(en.cl_entidad + " - " + en.nb_entidad);
                    row.Add(en.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(en.id_responsable));
                    row.Add(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso);
                    row.Add(mp.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(mp.id_responsable));
                    row.Add(pr.cl_proceso + " - " + pr.nb_proceso);
                    row.Add(pr.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(pr.id_responsable));
                    row.Add(sp.cl_sub_proceso + " - " + sp.nb_sub_proceso);
                    row.Add(sp.c_usuario.nb_usuario);
                    row.Add(SCIRA.Utilidades.Utilidades.PuestoUsuario(sp.id_responsable));
                    row.Add(periodo.nb_periodo_certificacion + " - " + periodo.anio);
                    row.Add(item.leyenda_certificacion_estructura);
                    row.Add(item.comentarios);
                    row.Add(item.resultado ? "Si" : "No");


                    rep24.Rows.Add(row);
                }


                rep24Json = (JObject)JToken.FromObject(rep24);
            }
            catch (Exception e)
            {
                Utilidades.Utilidades.CreateErrorReg(e.Message, "Respaldo reporte de Certificación de Sub Procesos");
            }
            #endregion

            #region Salvar Reportes
            Utilidades.Utilidades.BackUpProgress1("101", "Guardando Cambios");

            Reports.Add(rep1Json);
            Reports.Add(rep2Json);
            Reports.Add(rep3_1Json);
            Reports.Add(rep3_2Json);
            Reports.Add(rep4Json);
            Reports.Add(rep5Json);
            Reports.Add(rep6Json);
            Reports.Add(rep7Json);
            Reports.Add(rep8Json);
            Reports.Add(rep9Json);
            Reports.Add(rep10Json);
            Reports.Add(rep11_1Json);
            Reports.Add(rep11_2Json);
            Reports.Add(rep11_3Json);
            Reports.Add(rep11_4Json);
            Reports.Add(rep11_5Json);
            Reports.Add(rep11_6Json);
            Reports.Add(rep12_1Json);
            Reports.Add(rep12_2Json);
            Reports.Add(rep13_1Json);
            Reports.Add(rep13_2Json);
            Reports.Add(rep14_1Json);
            Reports.Add(rep14_2Json);
            Reports.AddRange(rep15Json);
            Reports.Add(rep16Json);
            Reports.Add(rep17Json);
            Reports.Add(rep18Json);
            Reports.Add(rep19Json);
            Reports.Add(rep20_1Json);
            Reports.Add(rep20_2Json);
            Reports.Add(rep21Json);
            Reports.Add(rep22Json);
            Reports.Add(rep23Json);
            Reports.Add(rep24Json);

            //Creamos un Folder nuevo que tendrá como nombre la fecha actual
            string FolderName = DateTime.Now.ToShortDateString().Replace('/', '-');
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/HistorialReportes/" + FolderName);
            //var path = HttpRuntime.AppDomainAppPath + "App_Data/HistorialReportes/" + FolderName;
            int Cont = 1;

            //Si ya existe el directorio, lo eliminamos:
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            DirectoryInfo di = Directory.CreateDirectory(path);

            //Una vez creado el directorio, guardamos cada Json en un archivo diferente.
            foreach (JObject report in Reports)
            {
                string fileName = string.Format("/r{0:000}", Cont++);
                var bytes = Encoding.UTF8.GetBytes(report.ToString());

                FileStream AuxFile = System.IO.File.Create(path + fileName);
                AuxFile.Write(bytes, 0, bytes.Length);

                AuxFile.Close();
            }

            Utilidades.Utilidades.BackUpProgress1("102", FolderName);
            #endregion

            return "Reportes Generados en " + path;
        }

        private class HeaderConfig
        {
            public int noFila { get; set; }
            public int rowspan { get; set; }
            public int colspan { get; set; }
        }

        private class Report
        {
            public Report()
            {
                ColNames = new List<string>();
                Rows = new List<List<string>>();
                HeadConfig = new List<HeaderConfig>();
                enableHeadConfig = false;
            }

            public string RepName { get; set; }
            public List<string> ColNames { get; set; }
            public List<List<string>> Rows { get; set; }
            public int NoCols { get; set; }

            public List<HeaderConfig> HeadConfig { get; set; }
            public int NoRowsHeader { get; set; }
            public bool enableHeadConfig { get; set; }
        }
        #endregion

        #region Estructura de mensajes
        public string MessageESTR(emailViewModel model)
        {
            //var html = Utilidades.ViewRenderer.RenderPartialView("~/Views/Utilidades/MessageESTR.cshtml", model, ControllerContext);

            return View(model).ToString();
        }
        #endregion

        #region IndicadoresDiarios
        public string ObtenerRutaINDD(int id)
        {
            var contenido = db.c_contenido_grupo.Find(id);
            string res = contenido.cl_contenido_grupo;

            var padre = contenido.c_contenido_grupo2;
            while (padre != null)
            {

                res = padre.cl_contenido_grupo + " >> " + res;
                padre = padre.c_contenido_grupo2;
            }

            return res;
        }
        #endregion

        #region Idioma
        public string SetLanguage(string ln)
        {
            try
            {
                var user = (IdentityPersonalizado)User.Identity;

                //Establecer el parametro en base de datos
                var Establecido = Utilidades.Utilidades.SetSecurityProp("lan" + user.Id_usuario, ln);

                //Establecer el parametro en Globals
                if (Establecido)
                    Globals.SetLan(user.Id_usuario, ln);

                return "ok ";
            }
            catch
            {
                return "Error";
            }
        }
        #endregion

        #region Limpiar parametros
        public string ClearParams()
        {
            var basura = db.c_parametro.Where(p => p.valor_parametro == "01/01/0001");
            db.c_parametro.RemoveRange(basura);

            db.SaveChanges();

            var parametros = db.c_parametro;

            List<string> mensajes = new List<string>();
            List<string> analizados = new List<string>();

            //Eliminar todos los parametros que son basura 



            foreach (var parametro in parametros)
            {
                if (!analizados.Any(p => p == parametro.nb_parametro))
                {
                    var ParaBorrar = parametros.Where(p => p.nb_parametro == parametro.nb_parametro && p.id_parametro != parametro.id_parametro);
                    analizados.Add(parametro.nb_parametro);

                    db.c_parametro.RemoveRange(ParaBorrar);

                    mensajes.Add("Parametro " + parametro.nb_parametro + " repetido " + ParaBorrar.Count() + " veces.");
                }
            }

            db.SaveChanges();

            return JsonConvert.SerializeObject(mensajes);
        }
        #endregion

        #region Eliminar archivos viejos
        public string DeleteOldFiles()
        {
            Utilidades.Utilidades.deleteOldFiles();

            return "Ok";
        }
        #endregion

        #region BDEI
        public string SetCatalogs()
        {
            var Estatus = db.c_estatus_bdei;
            var Moneda = db.c_moneda;
            var TipoSolucion = db.c_tipo_solucion;
            var GruposCC = db.c_grupo_cuenta_contable;
            var CuentasContables = db.c_cuenta_contable;
            var DsEventos = db.c_ds_evento;
            var Ambitos = db.c_ambito_riesgo_operacional;
            var Canales = db.c_canal_riesgo_operacional;
            var CLN = db.c_categoria_linea_negocio_riesgo_operacional;
            var CentrosCosto = db.c_centro_costo;
            var ClasesEvento = db.c_clase_evento;
            var Frecuencias = db.c_frecuencia_riesgo_operacional;
            var Impactos = db.c_impacto_riesgo_operacional;
            var LineasNegocio = db.c_linea_negocio_riesgo_operacional;
            var Procesos = db.c_proceso_riesgo_operacional;
            var Productos = db.c_producto_riesgo_operacional;
            var SubTiposProducto = db.c_sub_tipo_producto_riesgo_operacional;
            var SubTiposRiesgo = db.c_sub_tipo_riesgo_operacional;
            var TiposRiesgoAsociado = db.c_tipo_riesgo_asociado;
            var TipoRiesgoOperacional = db.c_tipo_riesgo_operacional;



            foreach (var r in Estatus)
            {
                r.esta_activo = true;
            }
            foreach (var r in Moneda)
            {
                r.esta_activo = true;
            }
            foreach (var r in TipoSolucion)
            {
                r.esta_activo = true;
            }
            foreach (var r in GruposCC)
            {
                r.esta_activo = true;
            }
            foreach (var r in CuentasContables)
            {
                r.esta_activo = true;
            }
            foreach (var r in DsEventos)
            {
                r.esta_activo = true;
            }
            foreach (var r in Ambitos)
            {
                r.esta_activo = true;
            }
            foreach (var r in Canales)
            {
                r.esta_activo = true;
            }
            foreach (var r in CLN)
            {
                r.esta_activo = true;
            }
            foreach (var r in CentrosCosto)
            {
                r.esta_activo = true;
            }
            foreach (var r in ClasesEvento)
            {
                r.esta_activo = true;
            }
            foreach (var r in Frecuencias)
            {
                r.esta_activo = true;
            }
            foreach (var r in Impactos)
            {
                r.esta_activo = true;
            }
            foreach (var r in LineasNegocio)
            {
                r.esta_activo = true;
            }
            foreach (var r in Procesos)
            {
                r.esta_activo = true;
            }
            foreach (var r in Productos)
            {
                r.esta_activo = true;
            }
            foreach (var r in SubTiposProducto)
            {
                r.esta_activo = true;
            }
            foreach (var r in SubTiposRiesgo)
            {
                r.esta_activo = true;
            }
            foreach (var r in TipoRiesgoOperacional)
            {
                r.esta_activo = true;
            }
            foreach (var r in TiposRiesgoAsociado)
            {
                r.esta_activo = true;
            }


            db.SaveChanges();

            return "ok";
        }

        #endregion

        #region Contador de Pendientes

        public ActionResult EarringCounter()
        {
            try
            {
                var user = (IdentityPersonalizado)User.Identity;

                var model = Utilidades.Utilidades.getNotifCountByUserM(user.Id_usuario);

                return PartialView("EarringCounter", model);
            }
            catch
            {
                return PartialView("EarringCounter", new NotificationsViewModel());
            }
        }

        #endregion


        #region Otros

        public string TimeZones() {
            var res = "";
            ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            res += string.Format("The local system has the following {0} time zones", zones.Count);
            foreach (TimeZoneInfo zone in zones)
                res += "\n" + zone.Id;

            return res;
        }


        [OnlySuperUser]
        public string SecurityProp(string propertie, string value = null) {
            if (value != null)
            {
                Utilidades.Utilidades.SetSecurityProp(propertie, value);
                return propertie + ":" + value;
            }
            else 
            {
                return propertie + ":" + Utilidades.Utilidades.GetSecurityProp(propertie);
            }
        }

        #endregion


        public string TimeOut(int seconds)
        {
            Thread.Sleep(seconds * 1000);

            return "You wait " + seconds + " segundos";
        }
    }
}
