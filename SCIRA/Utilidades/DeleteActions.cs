using SCIRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCIRA.Utilidades
{
    public static class DeleteActions
    {
        #region Entidad
        public static bool DeleteEntidadObjects(c_entidad entidad, SICIEntities db, bool force = false)
        {
            try
            {
                //eliminar oficios e informes
                var oficiosInformes = entidad.k_objeto.ToList();

                foreach (var objeto in oficiosInformes)
                {
                    DeleteObjetoObjects(objeto, db);
                    db.k_objeto.Remove(objeto);
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Control
        public static bool DeleteControlObjects(k_control control, SICIEntities db, bool force = false)
        {
            //para el control se verifica que se eliminen todas las certificaciones, historial de cambios, riesgo residual y apariciones en indicadores
            try
            {
                //eliminamos registros
                var hc = db.r_control.Where(rc => rc.id_control == control.id_control).ToList();
                foreach (var r in hc)
                {
                    db.r_control.Remove(r);
                }

                //eliminamos el riesgo residual
                var rr = db.k_riesgo_residual.Where(c => c.id_control == control.id_control).ToList();
                foreach (var r in rr)
                {
                    db.k_riesgo_residual.Remove(r);
                }

                //Eliminamos las incidencias
                var incidencias = control.k_incidencia.ToList();
                foreach (var incidencia in incidencias)
                {
                    DeleteIncidenciaObjects(incidencia, db);
                    db.k_incidencia.Remove(incidencia);
                }

                //Eliminamos las revisiones de los controles
                var revisiones = control.k_revision_control.ToList();
                foreach (var revision in revisiones)
                {
                    DeleteRevisionControlObjects(revision, db);
                    db.k_revision_control.Remove(revision);
                }

                control.k_riesgo.Clear();
                control.c_aseveracion.Clear();

                if (force)
                {
                    var certificaciones = db.k_certificacion_control.Where(c => c.id_control == control.id_control).ToList();
                    foreach (var certificacion in certificaciones)
                    {
                        DeleteCertificacionObjects(certificacion, db);
                        db.k_certificacion_control.Remove(certificacion);
                    }
                    var indicadores = db.c_indicador.Where(i => i.id_control == control.id_control).ToList();
                    foreach (var indicador in indicadores)
                    {
                        indicador.id_control = null;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Riesgo
        public static bool DeleteRiesgoObjects(k_riesgo riesgo, SICIEntities db, bool force = false)
        {
            //para el control se verifica que se eliminen todas las certificaciones, historial de cambios, y riesgo residual
            try
            {
                //eliminar historial de cambios
                var registros = db.r_riesgo.Where(r => r.id_riesgo == riesgo.id_riesgo);
                foreach (var registro in registros)
                {
                    db.r_riesgo.Remove(registro);
                }

                riesgo.c_error_contable.Clear();

                if (force)
                {
                    var controles = riesgo.k_control.ToList();
                    foreach (var control in controles)
                    {
                        int id = control.id_control;
                        if (DeleteControlObjects(control, db, true))
                        {
                            db.k_control.Remove(control);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    controles = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Certificacion
        public static bool DeleteCertificacionObjects(k_certificacion_control certificacion, SICIEntities db, bool force = false)
        {
            //para el control se verifica que se eliminen todas las certificaciones, historial de cambios, y riesgo residual
            try
            {
                //Eliminamos las incidencias
                var incidencias = certificacion.k_incidencia.ToList();
                foreach (var incidencia in incidencias)
                {
                    DeleteIncidenciaObjects(incidencia, db);
                    db.k_incidencia.Remove(incidencia);
                }

                int id = certificacion.id_certificacion_control;
                //eliminar los archivos ligados a la certificacin
                for (int i = 1; i < 6; i++)
                {
                    string nombre = "ac" + i + "-" + id;
                    string path = HttpContext.Current.Server.MapPath("~/App_Data/Certificacion/" + nombre);
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }


        public static bool DeleteCertificacionObjects(k_certificacion_estructura certificacion, SICIEntities db, bool force = false)
        {
            try
            {

                //eliminar los archivos ligados a la certificacin
                certificacion.c_archivo.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Macro Proceso
        public static bool DeleteMacroProcesoObjects(c_macro_proceso mp, SICIEntities db, bool force = false)
        {
            try
            {
                if (force)
                {
                    var prs = mp.c_proceso.ToList();

                    foreach (var pr in prs)
                    {
                        DeleteProcesoObjects(pr, db, true);
                        db.c_proceso.Remove(pr);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Proceso
        public static bool DeleteProcesoObjects(c_proceso pr, SICIEntities db, bool force = false)
        {
            try
            {
                if (force)
                {
                    var sps = pr.c_sub_proceso.ToList();

                    foreach (var sp in sps)
                    {
                        DeleteSubProcesoObjects(sp, db, true);
                        db.c_sub_proceso.Remove(sp);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Sub Proceso
        public static bool DeleteSubProcesoObjects(c_sub_proceso sp, SICIEntities db, bool force = false)
        {
            try
            {
                //Sub Proceso Normatividad
                sp.c_sub_proceso_normatividad.Clear();

                //Usuario Sub Proceso
                sp.c_usuario_sub_proceso.Clear();

                //Usuario Contenido Manual
                sp.c_contenido_manual.Clear();

                //Linea de Negocio
                sp.c_linea_negocio.Clear();

                //Riesgo Derogado
                var RD = sp.k_riesgo_derogado.ToList();
                foreach (var r in RD)
                {
                    var rs = db.k_riesgo_derogado.Where(ar => ar.id_riesgo_derogado == r.id_riesgo_derogado).First();
                    db.k_riesgo_derogado.Remove(rs);
                }

                //Area Costeo Sub Proceso
                var acspL = sp.c_area_costeo_sub_proceso.ToList();
                foreach (var acsp in acspL)
                {
                    db.c_area_costeo_sub_proceso.Remove(acsp);
                }

                var acsp3L = sp.c_area_costeo_n3_sub_proceso.ToList();
                foreach (var acsp3 in acsp3L)
                {
                    db.c_area_costeo_n3_sub_proceso.Remove(acsp3);
                }

                //Eliminar archivos de flujo y narrativa
                string flujo = "f" + sp.id_sub_proceso;
                string narrativa = "n" + sp.id_sub_proceso;
                System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/App_Data/" + flujo));
                System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/App_Data/" + narrativa));

                DelUnlinkedControls(sp.id_sub_proceso, db);


                if (force)
                {
                    //BDEI                  IMPLEMENTAR DeleteBDEIObjects
                    var bdeis = sp.k_bdei.ToList();
                    foreach (var bdei in bdeis)
                    {
                        db.k_bdei.Remove(bdei);
                    }
                    //Benchmarck            IMPLEMENTAR DeleteBenchmarckObjects
                    var benchmark = sp.k_benchmarck.ToList();
                    foreach (var bench in benchmark)
                    {
                        db.k_benchmarck.Remove(bench);
                    }
                    //Control               se elimina al eliminar los riesgos
                    //Riesgo
                    var Riesgos = sp.k_riesgo.ToList();
                    foreach (var riesgo in Riesgos)
                    {
                        DeleteRiesgoObjects(riesgo, db, force);
                        db.k_riesgo.Remove(riesgo);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Etapas
        public static bool DeleteEtapasObjects(c_etapa etapa, SICIEntities db, bool force = false)
        {
            try
            {
                //Eliminar la etapa de todos los Sub Procesos en dónde aparezca
                etapa.c_sub_proceso.Clear();


                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Sub Etapas
        public static bool DeleteSubEtapasObjects(c_sub_etapa subEtapa, SICIEntities db, bool force = false)
        {
            try
            {
                //Eliminar la etapa de todos los Sub Procesos en dónde aparezca
                subEtapa.c_sub_proceso.Clear();


                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Líneas de Negocio
        public static bool DeleteLineaNegocioObjects(c_linea_negocio lineaNegocio, SICIEntities db, bool force = false)
        {
            try
            {
                //Eliminar la etapa de todos los Sub Procesos en dónde aparezca
                lineaNegocio.c_sub_proceso.Clear();

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Contenido Normatividad
        public static bool DeleteContenidoNormatividadObjects(c_contenido_normatividad contenido, SICIEntities db, bool force = false)
        {
            try
            {
                List<c_contenido_normatividad> hijos = db.c_contenido_normatividad.Where(c => c.id_contenido_normatividad_padre == contenido.id_contenido_normatividad).ToList();
                foreach (var cont in hijos)
                {
                    DeleteContenidoNormatividadObjects(cont, db);
                }
                contenido.c_sub_proceso_normatividad.Clear();
                db.c_contenido_normatividad.Remove(contenido);

                var ficha = Utilidades.getFicha(contenido, db);
                if (ficha.id_evento != 0)
                {
                    DeleteFichaObjects(ficha, db);
                    db.r_evento.Remove(ficha);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Nivel Normatividad
        public static bool DeleteNivelNormatividadObjects(c_nivel_normatividad nivel, SICIEntities db, bool force = false)
        {
            try
            {
                var contenidos = nivel.c_contenido_normatividad.ToList();
                foreach (var cont in contenidos)
                {
                    DeleteContenidoNormatividadObjects(cont, db);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Normatividad
        public static bool DeleteNormatividadObjects(c_normatividad normatividad, SICIEntities db, bool force = false)
        {
            try
            {
                var niveles = normatividad.c_nivel_normatividad.ToList();
                foreach (var nivel in niveles)
                {
                    DeleteNivelNormatividadObjects(nivel, db);
                    db.c_nivel_normatividad.Remove(nivel);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Planes
        public static bool DeletePlanRemediacionObjects(k_plan plan, SICIEntities db, bool force = false)
        {
            try
            {
                plan.c_archivo.Clear();

                var seguimientos = plan.r_seguimiento.ToList();
                foreach (var seguimiento in seguimientos)
                {
                    DeleteSeguimientoObjects(seguimiento,db);
                    db.r_seguimiento.Remove(seguimiento);
                }

                var conclusiones = plan.r_conclusion_plan.ToList();
                foreach (var conclusion in conclusiones)
                {
                    DeleteConclusionPlanObjects(conclusion, db);
                    db.r_conclusion_plan.Remove(conclusion);
                }

                var ficha = Utilidades.getFicha(plan, db);
                if (ficha.id_evento != 0)
                {
                    DeleteFichaObjects(ficha, db);
                    db.r_evento.Remove(ficha);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Seguimientos
        public static bool DeleteSeguimientoObjects(r_seguimiento seguimiento, SICIEntities db, bool force = false)
        {
            try
            {
                seguimiento.c_archivo.Clear();

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Conclusion Plan
        public static bool DeleteConclusionPlanObjects(r_conclusion_plan conclusion, SICIEntities db, bool force = false)
        {
            try
            {
                conclusion.c_archivo.Clear();

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Incidencias
        public static bool DeleteIncidenciaObjects(k_incidencia incidencia, SICIEntities db, bool force = false)
        {
            try
            {
                var planes = incidencia.k_plan.ToList();
                foreach (var plan in planes)
                {
                    DeletePlanRemediacionObjects(plan, db);
                    db.k_plan.Remove(plan);
                }

                var contestaciones = incidencia.r_respuesta.ToList();
                foreach (var contestacion in contestaciones)
                {
                    DeleteRespuestaIncidenciaObjects(contestacion, db);
                    db.r_respuesta.Remove(contestacion);
                }

                var ficha = Utilidades.getFicha(incidencia, db);
                if (ficha.id_evento != 0)
                {
                    DeleteFichaObjects(ficha, db);
                    db.r_evento.Remove(ficha);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Oficios,Informes,Otros
        public static bool DeleteObjetoObjects(k_objeto objeto, SICIEntities db, bool force = false)
        {
            try
            {
                var incidencias = objeto.k_incidencia.ToList();
                foreach (var incidencia in incidencias)
                {
                    DeleteIncidenciaObjects(incidencia, db);
                    db.k_incidencia.Remove(incidencia);
                }

                var contestaciones = objeto.r_contestacion_oficio.ToList();
                foreach (var contestacion in contestaciones)
                {
                    db.r_contestacion_oficio.Remove(contestacion);
                }

                objeto.k_objeto2.Clear();

                objeto.k_objeto1.Clear();

                //Eliminar archivo
                string nombre = "a" + 1 + "-" + objeto.id_objeto;
                string path = HttpContext.Current.Server.MapPath("~/App_Data/Informes-Oficios/" + nombre);
                System.IO.File.Delete(path);
                nombre = "a" + 2 + "-" + objeto.id_objeto;
                path = HttpContext.Current.Server.MapPath("~/App_Data/Informes-Oficios/" + nombre);
                System.IO.File.Delete(path);


                //Eliminar Ficha
                var ficha = Utilidades.getFicha(objeto, db);
                if (ficha.id_evento != 0)
                {
                    DeleteFichaObjects(ficha, db);
                    db.r_evento.Remove(ficha);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Respuesta Incidencias
        public static bool DeleteRespuestaIncidenciaObjects(r_respuesta respuesta, SICIEntities db, bool force = false)
        {
            try
            {
                //Buscamos el oficio y borramos su contestacion
                var objeto = respuesta.k_incidencia.k_objeto;
                var contestaciones = objeto.r_contestacion_oficio.ToList();

                objeto.fe_contestacion = null;
                foreach (var contestacion in contestaciones)
                {
                    db.r_contestacion_oficio.Remove(contestacion);
                }

                int id = respuesta.id_respuesta;
                //eliminar los archivos ligados a la respuesta
                for (int i = 1; i < 4; i++)
                {
                    string nombre = "ac" + i + "-" + id;
                    string path = HttpContext.Current.Server.MapPath("~/App_Data/RIncidencias/" + nombre);
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Puesto
        public static bool DeletePuestoObjects(c_puesto puesto, SICIEntities db, bool force = false)
        {
            try
            {
                //Buscamos el oficio y borramos su contestacion
                var hijos = puesto.c_puesto1.ToList();
                foreach (var hijo in hijos)
                {
                    DeletePuestoObjects(hijo, db);
                    db.c_puesto.Remove(hijo);
                }
                puesto.c_usuario.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Estructura Manual
        public static bool DeleteEstructuraManualObjects(c_estructura_manual estructura, SICIEntities db, bool force = false)
        {
            try
            {
                var niveles = estructura.c_nivel_manual.ToList();
                foreach (var nivel in niveles)
                {
                    DeleteNivelManualObjects(nivel, db);
                    db.c_nivel_manual.Remove(nivel);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Nivel Manual
        public static bool DeleteNivelManualObjects(c_nivel_manual nivel, SICIEntities db, bool force = false)
        {
            try
            {
                var contenidos = nivel.c_contenido_manual.ToList();
                foreach (var cont in contenidos)
                {
                    DeleteContenidoManualObjects(cont, db);
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Contenido Manual
        public static bool DeleteContenidoManualObjects(c_contenido_manual contenido, SICIEntities db, bool force = false)
        {
            try
            {
                List<c_contenido_manual> hijos = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == contenido.id_contenido_manual).ToList();
                foreach (var cont in hijos)
                {
                    DeleteContenidoManualObjects(cont, db);
                }
                contenido.c_sub_proceso.Clear();
                contenido.c_usuario.Clear();

                db.c_contenido_manual.Remove(contenido);

                

                var fName = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Manuales/c" + contenido.id_contenido_manual + ".png");
                if (System.IO.File.Exists(fName))
                    System.IO.File.Delete(fName);

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Contenido Grupo
        public static bool DeleteContenidoGrupoObjects(c_contenido_grupo contenido, SICIEntities db, bool force = false)
        {
            try
            {
                List<c_contenido_grupo> hijos = db.c_contenido_grupo.Where(c => c.id_contenido_grupo_padre == contenido.id_contenido_grupo).ToList();
                foreach (var cont in hijos)
                {
                    DeleteContenidoGrupoObjects(cont, db);
                    db.c_contenido_grupo.Remove(cont);
                }

                //borrar indicadores
                var indicadores = contenido.c_indicador_diario.ToList();
                foreach (var ind in indicadores)
                {
                    DeleteIndicadorDiarioObjects(ind, db);
                    db.c_indicador_diario.Remove(ind);
                }


                //borrar evaluaciones
                var evaluaciones = contenido.k_evaluacion_diaria.ToList();
                foreach (var eval in evaluaciones)
                {
                    db.k_evaluacion_diaria.Remove(eval);
                }



                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Indicador Diaria
        public static bool DeleteIndicadorDiarioObjects(c_indicador_diario indicador, SICIEntities db, bool force = false)
        {
            try
            {
                var evaluaciones = indicador.k_evaluacion_diaria.ToList();
                foreach (var eval in evaluaciones)
                {
                    db.k_evaluacion_diaria.Remove(eval);
                }

                indicador.c_usuario.Clear();



                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  Catálogos Relacionados con RIESGOS

        #region Tipo de Riesgo
        public static bool DeleteTipoRiesgoObjects(c_tipo_riesgo model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_riesgos = model.r_riesgo.ToList();
                foreach (var r in r_riesgos)
                {
                    db.r_riesgo.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Tipologia de Riesgo
        public static bool DeleteTipologiaRiesgoObjects(c_tipologia_riesgo model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_riesgos = model.r_riesgo.ToList();
                foreach (var r in r_riesgos)
                {
                    db.r_riesgo.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Probabilidad de Ocurrencia
        public static bool DeleteProbabilidadOcurrenciaObjects(c_probabilidad_ocurrencia model, SICIEntities db, bool force = false)
        {
            try
            {
                var criticidades = model.c_criticidad.ToList();
                foreach (var criticidad in criticidades)
                {
                    db.c_criticidad.Remove(criticidad);
                }

                var r_riesgos = model.r_riesgo.ToList();
                foreach (var r in r_riesgos)
                {
                    db.r_riesgo.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Magnitud de Impacto
        public static bool DeleteMagnitudImpactoObjects(c_magnitud_impacto model, SICIEntities db, bool force = false)
        {
            try
            {
                var criticidades = model.c_criticidad.ToList();
                foreach (var criticidad in criticidades)
                {
                    db.c_criticidad.Remove(criticidad);
                }

                var r_riesgos = model.r_riesgo.ToList();
                foreach (var r in r_riesgos)
                {
                    db.r_riesgo.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Tipo de Impacto
        public static bool DeleteTipoImpactoObjects(c_tipo_impacto model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_riesgos = model.r_riesgo.ToList();
                foreach (var r in r_riesgos)
                {
                    db.r_riesgo.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #endregion

        #region Catálogos Relacionados con CONTROLES

        #region Grado de Cobertura
        public static bool DeleteGradoCoberturaObjects(c_grado_cobertura model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_controles = model.r_control.ToList();
                foreach (var r in r_controles)
                {
                    db.r_control.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Frecuencia de Control
        public static bool DeleteFrecuenciaControlObjects(c_frecuencia_control model, SICIEntities db, bool force = false)
        {
            try
            {
                //Eliminar pruebas de autoevaluacion ligadas a la frecuencia de control para evitar errores de llave foranea
                var pruebas = model.c_prueba_auto_eval.ToList();
                foreach (var prueba in pruebas)
                {
                    db.c_prueba_auto_eval.Remove(prueba);
                }

                var r_controles = model.r_control.ToList();
                foreach (var r in r_controles)
                {
                    db.r_control.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Naturaleza de Control
        public static bool DeleteNaturalezaControlObjects(c_naturaleza_control model, SICIEntities db, bool force = false)
        {
            try
            {
                //Eliminar pruebas de autoevaluacion ligadas a la frecuencia de control para evitar errores de llave foranea
                var pruebas = model.c_prueba_auto_eval.ToList();
                foreach (var prueba in pruebas)
                {
                    db.c_prueba_auto_eval.Remove(prueba);
                }

                var r_controles = model.r_control.ToList();
                foreach (var r in r_controles)
                {
                    db.r_control.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Tipología del Control
        public static bool DeleteTipologiaControlObjects(c_tipologia_control model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_controles = model.r_control.ToList();
                foreach (var r in r_controles)
                {
                    db.r_control.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Categoría del Control
        public static bool DeleteCategoriaControlObjects(c_categoria_control model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_controles = model.r_control.ToList();
                foreach (var r in r_controles)
                {
                    db.r_control.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Tipo de Evidencia
        public static bool DeleteTipoEvidenciaObjects(c_tipo_evidencia model, SICIEntities db, bool force = false)
        {
            try
            {
                var r_controles = model.r_control.ToList();
                foreach (var r in r_controles)
                {
                    db.r_control.Remove(r);
                }


                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #endregion

        #region Cátalogos Relacionados con BDEI

        #region Ambito RO
        public static bool DeleteAmbitoROObjects(c_ambito_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var procesos = model.c_proceso_riesgo_operacional.ToList();
                foreach (var r in procesos)
                {
                    r.esta_activo = false;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Impacto RO
        public static bool DeleteImpactoROObjects(c_impacto_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var criticidades = model.c_criticidad_ro.ToList();
                foreach (var criticidad in criticidades)
                {
                    db.c_criticidad_ro.Remove(criticidad);
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Frecuencia RO
        public static bool DeleteFrecuenciaROObjects(c_frecuencia_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var criticidades = model.c_criticidad_ro.ToList();
                foreach (var criticidad in criticidades)
                {
                    db.c_criticidad_ro.Remove(criticidad);
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Categoría Línea Negocio RO
        public static bool DeleteCategoriaLineaNegocioROObjects(c_categoria_linea_negocio_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var lineas = model.c_linea_negocio_riesgo_operacional.ToList();
                foreach (var r in lineas)
                {
                    r.esta_activo = false;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Grupo de cuentas contables RO
        public static bool DeleteGrupoCuentaContableROObjects(c_grupo_cuenta_contable model, SICIEntities db, bool force = false)
        {
            try
            {
                var cuentas = model.c_cuenta_contable.ToList();
                foreach (var r in cuentas)
                {
                    r.esta_activo = false;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Producto RO
        public static bool DeleteProductoROObjects(c_producto_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var stp = model.c_sub_tipo_producto_riesgo_operacional.ToList();
                foreach (var r in stp)
                {
                    r.esta_activo = false;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Sub Tipo RO
        public static bool DeleteSubTipoRiesgoObjects(c_sub_tipo_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var clases = model.c_clase_evento.ToList();
                foreach (var r in clases)
                {
                    r.esta_activo = false;
                }

                if (force)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        #endregion

        #region Tipo RO
        public static bool DeleteTipoROObjects(c_tipo_riesgo_operacional model, SICIEntities db, bool force = false)
        {
            try
            {
                var subtipos = model.c_sub_tipo_riesgo_operacional.ToList();
                foreach (var r in subtipos)
                {
                    DeleteSubTipoRiesgoObjects(r, db);
                    r.esta_activo = false;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #endregion

        #region Cátalogos Relacionados con Actividades de Costeo
        public static bool DeleteActividadesCosteoObjects(c_area_costeo actividad, SICIEntities db, bool force = false)
        {
            try
            {
                var acspL = actividad.c_area_costeo_sub_proceso.ToList();
                foreach (var acsp in acspL)
                {
                    db.c_area_costeo_sub_proceso.Remove(acsp);
                }

                var ac2L = actividad.c_area_costeo_n2.ToList();
                foreach (var ac2 in ac2L)
                {
                    DeleteActividadesCosteoLVL2Objects(ac2, db);
                    db.c_area_costeo_n2.Remove(ac2);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteActividadesCosteoLVL2Objects(c_area_costeo_n2 actividad, SICIEntities db, bool force = false)
        {
            try
            {
                var ac3L = actividad.c_area_costeo_n3.ToList();
                foreach (var ac3 in ac3L)
                {
                    DeleteActividadesCosteoLVL3Objects(ac3, db);
                    db.c_area_costeo_n3.Remove(ac3);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteActividadesCosteoLVL3Objects(c_area_costeo_n3 actividad, SICIEntities db, bool force = false)
        {
            try
            {
                var acspL = actividad.c_area_costeo_n3_sub_proceso.ToList();
                foreach (var acsp in acspL)
                {
                    db.c_area_costeo_n3_sub_proceso.Remove(acsp);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteActividadRangoCosteoAuditoriaObjects(c_rango_costeo actividad, SICIEntities db, bool force = false)
        {
            try
            {
                var acspL = actividad.c_rango_costeo_auditoria.ToList();
                foreach (var acsp in acspL)
                {
                    db.c_rango_costeo_auditoria.Remove(acsp);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteMontoImpactoObjects(c_impacto_monetario actividad, SICIEntities db, bool force = false)
        {
            try
            {
                var acspL = actividad.c_criticidad1.ToList();
                foreach (var acsp in acspL)
                {
                    db.c_criticidad1.Remove(acsp);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFactibilidadObjects(c_factibilidad actividad, SICIEntities db, bool force = false)
        {
            try
            {
                var acspL = actividad.c_criticidad1.ToList();
                foreach (var acsp in acspL)
                {
                    db.c_criticidad1.Remove(acsp);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

    

        #endregion

        #region Archivos
        public static bool DeleteArchivoObjects(c_archivo archivo, SICIEntities db, bool force = false)
        {
            try
            {
                archivo.r_evento.Clear();
                archivo.k_revision_control.Clear();
                archivo.k_programa_trabajo.Clear();
                archivo.k_programa_trabajo1.Clear();
                archivo.k_certificacion_estructura.Clear();
                archivo.k_auditoria.Clear();
                archivo.k_auditoria1.Clear();
                archivo.r_conclusion_plan.Clear();
                archivo.r_seguimiento.Clear();
                archivo.k_plan.Clear();
                archivo.k_control.Clear();

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Fichas
        public static bool DeleteFichaObjects(r_evento ficha, SICIEntities db, bool force = false)
        {
            try
            {
                var registros = ficha.r_registro_evento.ToList();
                foreach (var reg in registros)
                {
                    db.r_registro_evento.Remove(reg);
                }

                var archivos = ficha.c_archivo.ToList();
                foreach (var archivo in archivos)
                {
                    DeleteArchivoObjects(archivo, db);
                    db.c_archivo.Remove(archivo);
                }

                ficha.c_usuario1.Clear();
                ficha.c_usuario2.Clear();

                Utilidades.DeleteBackgoundJobs(ficha.id_evento, db);

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Aseveraciones
        public static bool DeleteAseveracionObjects(c_aseveracion aseveracion, SICIEntities db, bool force = false)
        {
            try
            {
                aseveracion.k_revision_control.Clear();
                aseveracion.k_control.Clear();

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Revision de Controles
        public static bool DeleteRevisionControlObjects(k_revision_control revision, SICIEntities db, bool force = false)
        {
            try
            {
                revision.c_aseveracion.Clear();

                var inputs = revision.a_input_rc.ToList();
                var pruebas = revision.a_procedimiento_prueba_rc.ToList();

                foreach (var input in inputs)
                {
                    db.a_input_rc.Remove(input);
                }
                foreach (var prueba in pruebas)
                {
                    db.a_procedimiento_prueba_rc.Remove(prueba);
                }


                var archivos = revision.c_archivo.ToList();
                foreach (var archivo in archivos)
                {
                    DeleteArchivoObjects(archivo, db);
                    db.c_archivo.Remove(archivo);
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Calificaciones
        public static bool DeleteCalificacionObjects(c_calificacion calificacion, SICIEntities db, bool force = false)
        {
            try
            {
                List<k_revision_control> revisiones = new List<k_revision_control>();

                if (calificacion.k_revision_control.Count > 0) revisiones.AddRange(calificacion.k_revision_control);
                if (calificacion.k_revision_control1.Count > 0) revisiones.AddRange(calificacion.k_revision_control1);
                if (calificacion.k_revision_control2.Count > 0) revisiones.AddRange(calificacion.k_revision_control2);
                if (calificacion.k_revision_control3.Count > 0) revisiones.AddRange(calificacion.k_revision_control3);
                if (calificacion.k_revision_control4.Count > 0) revisiones.AddRange(calificacion.k_revision_control4);
                if (calificacion.k_revision_control5.Count > 0) revisiones.AddRange(calificacion.k_revision_control5);
                if (calificacion.k_revision_control6.Count > 0) revisiones.AddRange(calificacion.k_revision_control6);
                if (calificacion.k_revision_control7.Count > 0) revisiones.AddRange(calificacion.k_revision_control7);
                if (calificacion.k_revision_control8.Count > 0) revisiones.AddRange(calificacion.k_revision_control8);
                if (calificacion.k_revision_control9.Count > 0) revisiones.AddRange(calificacion.k_revision_control9);
                if (calificacion.k_revision_control10.Count > 0) revisiones.AddRange(calificacion.k_revision_control10);
                if (calificacion.k_revision_control11.Count > 0) revisiones.AddRange(calificacion.k_revision_control11);
                if (calificacion.k_revision_control12.Count > 0) revisiones.AddRange(calificacion.k_revision_control12);
                if (calificacion.k_revision_control13.Count > 0) revisiones.AddRange(calificacion.k_revision_control13);
                if (calificacion.k_revision_control14.Count > 0) revisiones.AddRange(calificacion.k_revision_control14);
                if (calificacion.k_revision_control15.Count > 0) revisiones.AddRange(calificacion.k_revision_control15);
                if (calificacion.k_revision_control16.Count > 0) revisiones.AddRange(calificacion.k_revision_control16);
                if (calificacion.k_revision_control17.Count > 0) revisiones.AddRange(calificacion.k_revision_control17);
                if (calificacion.k_revision_control18.Count > 0) revisiones.AddRange(calificacion.k_revision_control18);
                if (calificacion.k_revision_control19.Count > 0) revisiones.AddRange(calificacion.k_revision_control19);
                if (calificacion.k_revision_control20.Count > 0) revisiones.AddRange(calificacion.k_revision_control20);
                if (calificacion.k_revision_control21.Count > 0) revisiones.AddRange(calificacion.k_revision_control21);
                if (calificacion.k_revision_control22.Count > 0) revisiones.AddRange(calificacion.k_revision_control22);
                if (calificacion.k_revision_control23.Count > 0) revisiones.AddRange(calificacion.k_revision_control23);
                if (calificacion.k_revision_control24.Count > 0) revisiones.AddRange(calificacion.k_revision_control24);
                if (calificacion.k_revision_control25.Count > 0) revisiones.AddRange(calificacion.k_revision_control25);
                if (calificacion.k_revision_control26.Count > 0) revisiones.AddRange(calificacion.k_revision_control26);
                if (calificacion.k_revision_control27.Count > 0) revisiones.AddRange(calificacion.k_revision_control27);
                if (calificacion.k_revision_control28.Count > 0) revisiones.AddRange(calificacion.k_revision_control28);
                if (calificacion.k_revision_control29.Count > 0) revisiones.AddRange(calificacion.k_revision_control29);
                if (calificacion.k_revision_control30.Count > 0) revisiones.AddRange(calificacion.k_revision_control30);
                if (calificacion.k_revision_control31.Count > 0) revisiones.AddRange(calificacion.k_revision_control31);
                if (calificacion.k_revision_control32.Count > 0) revisiones.AddRange(calificacion.k_revision_control32);
                if (calificacion.k_revision_control33.Count > 0) revisiones.AddRange(calificacion.k_revision_control33);
                if (calificacion.k_revision_control34.Count > 0) revisiones.AddRange(calificacion.k_revision_control34);
                if (calificacion.k_revision_control35.Count > 0) revisiones.AddRange(calificacion.k_revision_control35);
                if (calificacion.k_revision_control36.Count > 0) revisiones.AddRange(calificacion.k_revision_control36);
                if (calificacion.k_revision_control37.Count > 0) revisiones.AddRange(calificacion.k_revision_control37);
                if (calificacion.k_revision_control38.Count > 0) revisiones.AddRange(calificacion.k_revision_control38);
                if (calificacion.k_revision_control39.Count > 0) revisiones.AddRange(calificacion.k_revision_control39);

                //lista re las revisiones que incluyen esta calificación
                revisiones = revisiones.Union(revisiones).ToList();

                foreach (var r in revisiones)
                {
                    if (r.id_cc1 == calificacion.id_calificacion) r.id_cc1 = null;
                    if (r.id_cc2 == calificacion.id_calificacion) r.id_cc2 = null;
                    if (r.id_cc3 == calificacion.id_calificacion) r.id_cc3 = null;
                    if (r.id_cc4 == calificacion.id_calificacion) r.id_cc4 = null;
                    if (r.id_cc5 == calificacion.id_calificacion) r.id_cc5 = null;
                    if (r.id_cc6 == calificacion.id_calificacion) r.id_cc6 = null;
                    if (r.id_cc7 == calificacion.id_calificacion) r.id_cc7 = null;
                    if (r.id_cc8 == calificacion.id_calificacion) r.id_cc8 = null;
                    if (r.id_cc9 == calificacion.id_calificacion) r.id_cc9 = null;
                    if (r.id_cc10 == calificacion.id_calificacion) r.id_cc10 = null;
                    if (r.id_cc11 == calificacion.id_calificacion) r.id_cc11 = null;
                    if (r.id_cc12 == calificacion.id_calificacion) r.id_cc12 = null;
                    if (r.id_cc13 == calificacion.id_calificacion) r.id_cc13 = null;
                    if (r.id_cc14 == calificacion.id_calificacion) r.id_cc14 = null;
                    if (r.id_cc15 == calificacion.id_calificacion) r.id_cc15 = null;
                    if (r.id_cc16 == calificacion.id_calificacion) r.id_cc16 = null;
                    if (r.id_cc17 == calificacion.id_calificacion) r.id_cc17 = null;
                    if (r.id_cc18 == calificacion.id_calificacion) r.id_cc18 = null;
                    if (r.id_cc19 == calificacion.id_calificacion) r.id_cc19 = null;
                    if (r.id_cc20 == calificacion.id_calificacion) r.id_cc20 = null;
                    if (r.id_cc21 == calificacion.id_calificacion) r.id_cc21 = null;
                    if (r.id_cc22 == calificacion.id_calificacion) r.id_cc22 = null;
                    if (r.id_cc23 == calificacion.id_calificacion) r.id_cc23 = null;
                    if (r.id_cc24 == calificacion.id_calificacion) r.id_cc24 = null;
                    if (r.id_cc25 == calificacion.id_calificacion) r.id_cc25 = null;
                    if (r.id_cc26 == calificacion.id_calificacion) r.id_cc26 = null;
                    if (r.id_cc27 == calificacion.id_calificacion) r.id_cc27 = null;
                    if (r.id_cc28 == calificacion.id_calificacion) r.id_cc28 = null;
                    if (r.id_cc29 == calificacion.id_calificacion) r.id_cc29 = null;
                    if (r.id_cc30 == calificacion.id_calificacion) r.id_cc30 = null;
                    if (r.id_cc31 == calificacion.id_calificacion) r.id_cc31 = null;
                    if (r.id_cc32 == calificacion.id_calificacion) r.id_cc32 = null;
                    if (r.id_cc33 == calificacion.id_calificacion) r.id_cc33 = null;
                    if (r.id_cc34 == calificacion.id_calificacion) r.id_cc34 = null;
                    if (r.id_cc35 == calificacion.id_calificacion) r.id_cc35 = null;
                    if (r.id_cc36 == calificacion.id_calificacion) r.id_cc36 = null;
                    if (r.id_cc37 == calificacion.id_calificacion) r.id_cc37 = null;
                    if (r.id_cc38 == calificacion.id_calificacion) r.id_cc38 = null;
                    if (r.id_cc39 == calificacion.id_calificacion) r.id_cc39 = null;
                    if (r.id_cc40 == calificacion.id_calificacion) r.id_cc40 = null;
                }


                List<a_input_rc> inputs = calificacion.a_input_rc.ToList();
                List<a_procedimiento_prueba_rc> pruebas = calificacion.a_procedimiento_prueba_rc.ToList();

                foreach (var input in inputs)
                {
                    input.id_calificacion = null;
                }

                foreach (var prueba in pruebas)
                {
                    prueba.id_calificacion = null;
                }


                if (force)
                {

                }
            }
            catch 
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Calificaciones Revision
        public static bool DeleteCalificacionRevisionObjects(c_calificacion_revision calificacion, SICIEntities db, bool force = false)
        {
            try
            {
                List<k_revision_control> revisiones = calificacion.k_revision_control.ToList();

                foreach (var r in revisiones)
                {
                    r.id_calificacion_revision = null;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Rating de auditoria
        public static bool DeleteRatingAuditoriaObjects(c_rating_auditoria rating, SICIEntities db, bool force = false)
        {
            try
            {
                List<k_auditoria> kAuditorias = rating.k_auditoria.ToList();

                foreach (var r in kAuditorias)
                {
                    r.id_rating_auditoria = null;
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region CamposExtraAuditoria
        public static bool DeleteCampoExtraAuditoriaObjects(c_campo_auditoria campo, SICIEntities db, bool force = false)
        {
            try
            {
                List<k_campo_auditoria> kCampos = campo.k_campo_auditoria.ToList();

                foreach (var r in kCampos)
                {
                    db.k_campo_auditoria.Remove(r);
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region CamposExtraPrograma
        public static bool DeleteCampoExtraProgramaObjects(c_campo_programa campo, SICIEntities db, bool force = false)
        {
            try
            {
                List<k_campo_programa> kCampos = campo.k_campo_programa.ToList();

                foreach (var r in kCampos)
                {
                    db.k_campo_programa.Remove(r);
                }

                if (force)
                {

                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Auditoria
        public static bool DeleteAuditoriaObjects(c_auditoria aud, SICIEntities db, bool force = false)
        {
            try
            {
                if (force)
                {
                    var kAuds = aud.k_auditoria.ToList();
                    var pruebas = aud.c_prueba_auditoria.ToList();

                    foreach (var kAud in kAuds)
                    {
                        DeleteKAuditoriaObjects(kAud, db, true);
                        db.k_auditoria.Remove(kAud);
                    }

                    foreach (var prueba in pruebas)
                    {
                        db.c_prueba_auditoria.Remove(prueba);
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

        #region KAuditoria
        public static bool DeleteKAuditoriaObjects(k_auditoria aud, SICIEntities db, bool force = false)
        {
            try
            {
                aud.c_archivo.Clear();
                aud.c_archivo1.Clear();

                if (force)
                {
                    var kCampos = aud.k_campo_auditoria.ToList();
                    var kProgramas = aud.k_programa_trabajo.ToList();
                    foreach (var kCampo in kCampos)
                    {
                        db.k_campo_auditoria.Remove(kCampo);
                    }

                    foreach (var kPrograma in kProgramas)
                    {
                        DeleteKProgramaObjects(kPrograma, db, true);
                        db.k_programa_trabajo.Remove(kPrograma);
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

        #region KPrograma
        public static bool DeleteKProgramaObjects(k_programa_trabajo programa, SICIEntities db, bool force = false)
        {
            try
            {
                if (force)
                {
                    var kCampos = programa.k_campo_programa.ToList();
                    var comentarios = programa.k_comentario_programa_trabajo.ToList();
                    var archivos = programa.c_archivo.ToList();
                    archivos.AddRange(programa.c_archivo1);

                    foreach (var kCampo in kCampos)
                    {
                        db.k_campo_programa.Remove(kCampo);
                    }

                    foreach (var archivo in archivos)
                    {
                        DeleteArchivoObjects(archivo, db);
                        db.c_archivo.Remove(archivo);
                    }

                    foreach (var comentario in comentarios)
                    {
                        db.k_comentario_programa_trabajo.Remove(comentario);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Otros
        public static bool DelUnlinkedControls(int id_sp, SICIEntities db)
        {
            var sp = db.c_sub_proceso.Find(id_sp);
            //eliminar todos los controles no ligados a un riesgo
            //pertenecientes a un cierto sub proceso
            try
            {
                var Ctrl = sp.k_control.ToList();
                foreach (var ct in Ctrl)
                {
                    if (ct.k_riesgo.Count() < 1)
                    {
                        var c = db.k_control.Find(ct.id_control);
                        DeleteControlObjects(c, db, true);
                        db.k_control.Remove(c);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public static void checkRedirect(string redirect)
        {
            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Current.Session["JumpCounter"];
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
                        HttpContext.Current.Session["JumpCounter"] = 1;
                        HttpContext.Current.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Current.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Current.Session["JumpCounter"] = ns;
                        HttpContext.Current.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Current.Session["JumpCounter"] = null;
                HttpContext.Current.Session["Directions"] = null;
            }
        }

        #endregion


    }
}