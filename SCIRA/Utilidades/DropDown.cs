using SCIRA.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using static iTextSharp.text.pdf.AcroFields;

namespace SCIRA.Utilidades
{
    public static class DropDown
    {
        static private SICIEntities db = new SICIEntities();

        #region Entidad
        public static List<SelectListItem> Entidades(int selected = 0)
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();
            var entidades = db.c_entidad.ToList();
            foreach (var entidad in entidades)
            {
                lista.Add(
                    new SelectListItem
                    {
                        Value = entidad.id_entidad.ToString(),
                        Text = entidad.cl_entidad + "-" + entidad.nb_entidad,
                        Selected = selected == entidad.id_entidad
                    }
                );
            }
            return lista;
        }
        #endregion

        #region MacroProcesos 
        public static List<SelectListItem> MacroProcesos(int selected = 0, string smp = "MP", int id_entidad = 0) //c_entidad.id_entidad == c_macro_procesos.id_entidad
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();

            //var macroprocesos = db.c_entidad.Where(mp => mp.id_entidad == mp.id_entidad).ToList();

            var entidades = db.c_macro_proceso.ToList().Where(mp => mp.cl_macro_proceso.StartsWith(smp) && mp.id_entidad == id_entidad); //biien
                                                                                                                                         //var entidadesMG = db.c_entidad.ToList().Where(mp => mp.cl_entidad.StartsWith("MG"));

            //entidades.
            foreach (var entidad in entidades)
            {

                lista.Add(
                    new SelectListItem
                    {
                        Value = entidad.id_macro_proceso.ToString(),
                        Text = entidad.cl_macro_proceso + "-" + entidad.nb_macro_proceso,
                        Selected = selected == entidad.id_macro_proceso

                    }
                );
            }
            return lista;
        }


        #endregion

        #region Sub Procesos
        public static List<SelectListItem> SubProcesos(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_sub_proceso.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_sub_proceso.ToString(),
                        Text = item.cl_sub_proceso + "-" + item.nb_sub_proceso,
                        Selected = selected == item.id_sub_proceso
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Area
        public static List<SelectListItem> Areas(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_area.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_area.ToString(),
                        Text = item.cl_area + "-" + item.nb_area,
                        Selected = selected == item.id_area
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Etapas
        public static List<SelectListItem> Etapas(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_etapa.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_etapa.ToString(),
                        Text = item.cl_etapa + "-" + item.nb_etapa,
                        Selected = selected == item.id_etapa
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Sub Etapas
        public static List<SelectListItem> SubEtapas(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_sub_etapa.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_sub_etapa.ToString(),
                        Text = item.cl_sub_etapa + "-" + item.nb_sub_etapa,
                        Selected = selected == item.id_sub_etapa
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Tipologia Sub Proceso
        public static List<SelectListItem> TipologiaSubProceso(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_tipologia_sub_proceso.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_tipologia_sub_proceso.ToString(),
                        Text = item.cl_tipologia_sub_proceso + "-" + item.nb_tipologia_sub_proceso,
                        Selected = selected == item.id_tipologia_sub_proceso
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Oficios, Informes, Incidencias y Planes
        #region OrigenAutoridad
        public static List<SelectListItem> OrigenAutoridad()
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_origen_autoridad.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_origen_autoridad.ToString(),
                        Text = item.cl_origen_autoridad + "-" + item.nb_origen_autoridad
                    }
                );
            }
            return dropdown;
        }



        #endregion

        #region ClasificacionIncidencia
        public static List<SelectListItem> ClasificacionIncidencia()
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_clasificacion_incidencia.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_clasificacion_incidencia.ToString(),
                        Text = item.cl_clasificacion_incidencia + "-" + item.nb_clasificacion_incidencia
                    }
                );
            }
            return dropdown;
        }
        #endregion
        #endregion

        #region Usuarios
        public static List<SelectListItem> Usuario(int selected = 0, bool soloActivos = true)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            List<c_usuario> lista = new List<c_usuario>();

            if (soloActivos) lista = db.c_usuario.Where(u => u.esta_activo).OrderBy(u => u.nb_usuario).ToList();
            else lista = db.c_usuario.OrderBy(u => u.nb_usuario).ToList();

            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_usuario.ToString(),
                        Text = item.nb_usuario,
                        Selected = selected == item.id_usuario
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Categoria De Riesgo
        public static List<SelectListItem> CategoriaRiesgo(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_categoria_riesgo.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_categoria_riesgo.ToString(),
                        Text = item.cl_categoria_riesgo + "-" + item.nb_categoria_riesgo,
                        Selected = selected == item.id_categoria_riesgo
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Estructura de Manual
        public static List<SelectListItem> EstructuraManual(int selected = 0, bool needLevels = false)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            List<c_estructura_manual> lista = new List<c_estructura_manual>();
            if (needLevels)
            {
                lista = db.c_estructura_manual.Where(c => c.c_nivel_manual.Count > 0).ToList();
            }
            else
            {
                lista = db.c_estructura_manual.ToList();
            }
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_estructura_manual.ToString(),
                        Text = item.cl_estructura_manual,

                        Selected = selected == item.id_estructura_manual
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Auxiliares
        public static List<SelectListItem> Muestra()
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();
            for (int i = 0; i < 10; i++)
            {
                lista.Add(
                    new SelectListItem
                    {
                        Value = (i + 1).ToString(),
                        Text = "Element" + string.Format("{0:00}", i + 1)
                    }
                );
            }
            return lista;
        }

        public static List<SelectListItem> FrecuenciasMensajes(string selected)
        {
            if ((selected ?? "") == "")
            {
                selected = "Mensual";
            }
            List<SelectListItem> lista = new List<SelectListItem>();
            //                                      minuto    hora      Dia/mes       Mes/año    dia/sem
            //                                      *           *           *           *           *        
            lista.Add(new SelectListItem { Value = "0 7 * * 1", Text = "Semanal", Selected = selected == "Semanal" });

            lista.Add(new SelectListItem { Value = "0 7 1 * *", Text = "Mensual", Selected = selected == "Mensual" });
            lista.Add(new SelectListItem { Value = "0 7 1 1/2 *", Text = "Bimestral", Selected = selected == "Bimestral" });
            lista.Add(new SelectListItem { Value = "0 7 1 1/3 *", Text = "Trimestral", Selected = selected == "Trimestral" });
            lista.Add(new SelectListItem { Value = "0 7 1 1/6 *", Text = "Semestral", Selected = selected == "Semestral" });
            lista.Add(new SelectListItem { Value = "0 7 1 1/12 *", Text = "Anual", Selected = selected == "Anual" });


            return lista;
        }

        public static List<SelectListItem> MailHosts(string selected)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            lista.Add(new SelectListItem { Value = "Smtp.1and1.com", Text = "1&1", Selected = selected == "Smtp.1and1.com" });
            lista.Add(new SelectListItem { Value = "Mail.airmail.net", Text = "Airmail", Selected = selected == "Mail.airmail.net" });
            lista.Add(new SelectListItem { Value = "Smtp.aol.com", Text = "AOL", Selected = selected == "Smtp.aol.com" });
            lista.Add(new SelectListItem { Value = "Outbound.att.net", Text = "AT&T", Selected = selected == "Outbound.att.net" });
            lista.Add(new SelectListItem { Value = "Smtpauths.bluewin.ch", Text = "Bluewin", Selected = selected == "Smtpauths.bluewin.ch" });
            lista.Add(new SelectListItem { Value = "Mail.btconnect.tom", Text = "BT Connect", Selected = selected == "Mail.btconnect.tom" });
            lista.Add(new SelectListItem { Value = "Smtp.comcast.net", Text = "Comcast", Selected = selected == "Smtp.comcast.net" });
            lista.Add(new SelectListItem { Value = "Smtpauth.earthlink.net", Text = "Earthlink", Selected = selected == "Smtpauth.earthlink.net" });
            lista.Add(new SelectListItem { Value = "Smtp.gmail.com", Text = "Gmail", Selected = selected == "Smtp.gmail.com" });
            lista.Add(new SelectListItem { Value = "Mail.gmx.net", Text = "Gmx", Selected = selected == "Mail.gmx.net" });
            lista.Add(new SelectListItem { Value = "Mail.hotpop.com", Text = "HotPop", Selected = selected == "Mail.hotpop.com" });
            lista.Add(new SelectListItem { Value = "Mail.libero.it", Text = "Libero", Selected = selected == "Mail.libero.it" });
            lista.Add(new SelectListItem { Value = "Smtp.lycos.com", Text = "Lycos", Selected = selected == "Smtp.lycos.com" });
            lista.Add(new SelectListItem { Value = "Smtp.o2.com", Text = "O2", Selected = selected == "Smtp.o2.com" });
            lista.Add(new SelectListItem { Value = "Smtp.orange.net", Text = "Orange", Selected = selected == "Smtp.orange.net" });
            lista.Add(new SelectListItem { Value = "smtp-mail.outlook.com", Text = "Outlook", Selected = selected == "smtp-mail.outlook.com" });
            lista.Add(new SelectListItem { Value = "Mail.tin.it", Text = "Tin", Selected = selected == "Mail.tin.it" });
            lista.Add(new SelectListItem { Value = "Smtp.tiscali.co.uk", Text = "Tiscali", Selected = selected == "Smtp.tiscali.co.uk" });
            lista.Add(new SelectListItem { Value = "Outgoing.verizon.net", Text = "Verizon", Selected = selected == "Outgoing.verizon.net" });
            lista.Add(new SelectListItem { Value = "Smtp.virgin.net", Text = "Virgin", Selected = selected == "Smtp.virgin.net" });
            lista.Add(new SelectListItem { Value = "Smtp.wanadoo.fr", Text = "Wanadoo", Selected = selected == "Smtp.wanadoo.fr" });
            lista.Add(new SelectListItem { Value = "Smtp.mail.yahoo.com", Text = "Yahoo", Selected = selected == "Smtp.mail.yahoo.com" });


            return lista;
        }

        static private void renewDB()
        {
            db = new SICIEntities();
        }
        #endregion

        #region Cátalogos asociados a la BDEI

        #region Moneda
        public static List<SelectListItem> Moneda(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            List<c_moneda> lista = new List<c_moneda>();

            lista = db.c_moneda.ToList();


            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_moneda == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_moneda.ToString(),
                            Text = item.nb_moneda,
                            Selected = selected == item.id_moneda
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Estatus
        public static List<SelectListItem> EstatusBDEI(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_estatus_bdei.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_estatus_bdei == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_estatus_bdei.ToString(),
                            Text = item.cl_estatus_bdei + "-" + item.nb_estatus_bdei,
                            Selected = selected == item.id_estatus_bdei
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Tipo de Solución
        public static List<SelectListItem> TipoSolucion(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_tipo_solucion.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_tipo_solucion == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_tipo_solucion.ToString(),
                            Text = item.cl_tipo_solucion + "-" + item.nb_tipo_solucion,
                            Selected = selected == item.id_tipo_solucion
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region CuentaContable
        public static List<SelectListItem> CuentaContable(int idEN, int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();

            var entidad = db.c_entidad.Find(idEN);
            var grupos = entidad.c_grupo_cuenta_contable;

            foreach (var grupo in grupos)
            {
                if (grupo.esta_activo ?? false || grupo.c_cuenta_contable.Any(cc => cc.id_cuenta_contable == selected))
                {
                    SelectListGroup slGroup = new SelectListGroup
                    {
                        Disabled = false,
                        Name = grupo.nb_grupo_cuenta_contable
                    };

                    foreach (var cc in grupo.c_cuenta_contable)
                    {
                        if ((cc.esta_activo ?? false) || cc.id_cuenta_contable == selected)
                            dropdown.Add(
                                new SelectListItem
                                {
                                    Value = cc.id_cuenta_contable.ToString(),
                                    Text = cc.cl_cuenta_contable + "-" + cc.nb_cuenta_contable,
                                    Selected = selected == cc.id_cuenta_contable,
                                    Group = slGroup
                                }
                            );
                    }


                }
            }

            return dropdown;
        }
        #endregion

        #region Centro de Costo
        public static List<SelectListItem> CentroCosto(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_centro_costo.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_centro_costo == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_centro_costo.ToString(),
                            Text = item.cl_centro_costo + "-" + item.nb_centro_costo,
                            Selected = selected == item.id_centro_costo
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Tipo De Riesgo Operacional
        public static List<SelectListItem> TipoRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_tipo_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_tipo_riesgo_operacional == selected)
                    dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_tipo_riesgo_operacional.ToString(),
                        Text = item.cl_tipo_riesgo_operacional + "-" + item.nb_tipo_riesgo_operacional,
                        Selected = selected == item.id_tipo_riesgo_operacional
                    }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Sub Tipo De Riesgo Operacional
        public static List<SelectListItem> SubTipoRiesgoOperacional(int padre, int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_sub_tipo_riesgo_operacional.Where(st => st.id_tipo_riesgo_operacional == padre).ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_sub_tipo_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_sub_tipo_riesgo_operacional.ToString(),
                            Text = item.cl_sub_tipo_riesgo_operacional + "-" + item.nb_sub_tipo_riesgo_operacional,
                            Selected = selected == item.id_sub_tipo_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Sub Tipo De Riesgo Operacional - Riesgo
        public static List<SelectListItem> SubTipoRiesgoOperacionalR(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();

            var grupos = db.c_tipo_riesgo_operacional;

            foreach (var grupo in grupos)
            {
                if (grupo.esta_activo ?? false || grupo.c_sub_tipo_riesgo_operacional.Any(cc => cc.id_sub_tipo_riesgo_operacional == selected))
                {
                    SelectListGroup slGroup = new SelectListGroup
                    {
                        Disabled = false,
                        Name = grupo.nb_tipo_riesgo_operacional
                    };

                    foreach (var cc in grupo.c_sub_tipo_riesgo_operacional)
                    {
                        if ((cc.esta_activo ?? false) || cc.id_sub_tipo_riesgo_operacional == selected)
                            dropdown.Add(
                                new SelectListItem
                                {
                                    Value = cc.id_sub_tipo_riesgo_operacional.ToString(),
                                    Text = cc.cl_sub_tipo_riesgo_operacional + "-" + cc.nb_sub_tipo_riesgo_operacional,
                                    Selected = selected == cc.id_sub_tipo_riesgo_operacional,
                                    Group = slGroup
                                }
                            );
                    }


                }
            }

            return dropdown;
        }
        #endregion

        #region Clase de Evento
        public static List<SelectListItem> ClaseEventoRiesgoOperacional(int padre, int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_clase_evento.Where(i => i.id_sub_tipo_riesgo_operacional == padre).ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_clase_evento == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_clase_evento.ToString(),
                            Text = item.cl_clase_evento + "-" + item.nb_clase_evento,
                            Selected = selected == item.id_clase_evento
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Ambito De Riesgo Operacional
        public static List<SelectListItem> AmbitoRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_ambito_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if (item.id_ambito_riesgo_operacional == selected || (item.esta_activo ?? false))
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_ambito_riesgo_operacional.ToString(),
                            Text = item.cl_ambito_riesgo_operacional + "-" + item.nb_ambito_riesgo_operacional,
                            Selected = selected == item.id_ambito_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Proceso
        public static List<SelectListItem> ProcesoRiesgoOperacional(int padre, int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_proceso_riesgo_operacional.Where(i => i.id_ambito_riesgo_operacional == padre).ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_proceso_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_proceso_riesgo_operacional.ToString(),
                            Text = item.cl_proceso_riesgo_operacional + "-" + item.nb_proceso_riesgo_operacional,
                            Selected = selected == item.id_proceso_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Proceso - Riesgo
        public static List<SelectListItem> ProcesoRiesgoOperacionalR(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();

            var grupos = db.c_ambito_riesgo_operacional;

            foreach (var grupo in grupos)
            {
                if (grupo.esta_activo ?? false || grupo.c_proceso_riesgo_operacional.Any(cc => cc.id_proceso_riesgo_operacional == selected))
                {
                    SelectListGroup slGroup = new SelectListGroup
                    {
                        Disabled = false,
                        Name = grupo.nb_ambito_riesgo_operacional
                    };

                    foreach (var cc in grupo.c_proceso_riesgo_operacional)
                    {
                        if ((cc.esta_activo ?? false) || cc.id_proceso_riesgo_operacional == selected)
                            dropdown.Add(
                                new SelectListItem
                                {
                                    Value = cc.id_proceso_riesgo_operacional.ToString(),
                                    Text = cc.cl_proceso_riesgo_operacional + "-" + cc.nb_proceso_riesgo_operacional,
                                    Selected = selected == cc.id_proceso_riesgo_operacional,
                                    Group = slGroup
                                }
                            );
                    }


                }
            }

            return dropdown;
        }
        #endregion

        #region Producto De Riesgo Operacional
        public static List<SelectListItem> ProductoRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_producto_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_producto_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_producto_riesgo_operacional.ToString(),
                            Text = item.cl_producto_riesgo_operacional + "-" + item.nb_producto_riesgo_operacional,
                            Selected = selected == item.id_producto_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Sub Tipo Producto
        public static List<SelectListItem> SubTipoProductoRiesgoOperacional(int padre, int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_sub_tipo_producto_riesgo_operacional.Where(i => i.id_producto_riesgo_operacional == padre).ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_sub_tipo_producto_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_sub_tipo_producto_riesgo_operacional.ToString(),
                            Text = item.cl_sub_tipo_producto_riesgo_operacional + "-" + item.nb_sub_tipo_producto_riesgo_operacional,
                            Selected = selected == item.id_sub_tipo_producto_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Sub Tipo Producto - Riesgo
        public static List<SelectListItem> SubTipoProductoRiesgoOperacionalR(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();

            var grupos = db.c_producto_riesgo_operacional;

            foreach (var grupo in grupos)
            {
                if (grupo.esta_activo ?? false || grupo.c_sub_tipo_producto_riesgo_operacional.Any(cc => cc.id_sub_tipo_producto_riesgo_operacional == selected))
                {
                    SelectListGroup slGroup = new SelectListGroup
                    {
                        Disabled = false,
                        Name = grupo.nb_producto_riesgo_operacional
                    };

                    foreach (var cc in grupo.c_sub_tipo_producto_riesgo_operacional)
                    {
                        if ((cc.esta_activo ?? false) || cc.id_sub_tipo_producto_riesgo_operacional == selected)
                            dropdown.Add(
                                new SelectListItem
                                {
                                    Value = cc.id_sub_tipo_producto_riesgo_operacional.ToString(),
                                    Text = cc.cl_sub_tipo_producto_riesgo_operacional + "-" + cc.nb_sub_tipo_producto_riesgo_operacional,
                                    Selected = selected == cc.id_sub_tipo_producto_riesgo_operacional,
                                    Group = slGroup
                                }
                            );
                    }


                }
            }

            return dropdown;
        }


        #endregion

        #region Categoría Linea De Negocio Riesgo Operacional
        public static List<SelectListItem> CategoriaLineaNegocioRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_categoria_linea_negocio_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_categoria_linea_negocio_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_categoria_linea_negocio_riesgo_operacional.ToString(),
                            Text = item.cl_categoria_linea_negocio_riesgo_operacional + "-" + item.nb_categoria_linea_negocio_riesgo_operacional,
                            Selected = selected == item.id_categoria_linea_negocio_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Línea de Negocio
        public static List<SelectListItem> LineaNegocioRiesgoOperacional(int padre, int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_linea_negocio_riesgo_operacional.Where(i => i.id_categoria_linea_negocio_riesgo_operacional == padre).ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_linea_negocio_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_linea_negocio_riesgo_operacional.ToString(),
                            Text = item.cl_linea_negocio_riesgo_operacional + "-" + item.nb_linea_negocio_riesgo_operacional,
                            Selected = selected == item.id_linea_negocio_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Línea de Negocio - Riesgo
        public static List<SelectListItem> LineaNegocioRiesgoOperacionalR(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();

            var grupos = db.c_categoria_linea_negocio_riesgo_operacional;

            foreach (var grupo in grupos)
            {
                if (grupo.esta_activo ?? false || grupo.c_linea_negocio_riesgo_operacional.Any(cc => cc.id_linea_negocio_riesgo_operacional == selected))
                {
                    SelectListGroup slGroup = new SelectListGroup
                    {
                        Disabled = false,
                        Name = grupo.nb_categoria_linea_negocio_riesgo_operacional
                    };

                    foreach (var cc in grupo.c_linea_negocio_riesgo_operacional)
                    {
                        if ((cc.esta_activo ?? false) || cc.id_linea_negocio_riesgo_operacional == selected)
                            dropdown.Add(
                                new SelectListItem
                                {
                                    Value = cc.id_linea_negocio_riesgo_operacional.ToString(),
                                    Text = cc.cl_linea_negocio_riesgo_operacional + "-" + cc.nb_linea_negocio_riesgo_operacional,
                                    Selected = selected == cc.id_linea_negocio_riesgo_operacional,
                                    Group = slGroup
                                }
                            );
                    }


                }
            }

            return dropdown;
        }
        #endregion

        #region Canal
        public static List<SelectListItem> CanalRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_canal_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_canal_riesgo_operacional == selected)
                    dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_canal_riesgo_operacional.ToString(),
                        Text = item.cl_canal_riesgo_operacional + "-" + item.nb_canal_riesgo_operacional,
                        Selected = selected == item.id_canal_riesgo_operacional
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Frecuencia
        public static List<SelectListItem> FrecuenciaRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_frecuencia_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_frecuencia_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_frecuencia_riesgo_operacional.ToString(),
                            Text = item.cl_frecuencia_riesgo_operacional + "-" + item.nb_frecuencia_riesgo_operacional,
                            Selected = selected == item.id_frecuencia_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Impacto
        public static List<SelectListItem> ImpactoRiesgoOperacional(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_impacto_riesgo_operacional.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_impacto_riesgo_operacional == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_impacto_riesgo_operacional.ToString(),
                            Text = item.cl_impacto_riesgo_operacional + "-" + item.nb_impacto_riesgo_operacional,
                            Selected = selected == item.id_impacto_riesgo_operacional
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Riesgo Asociado
        public static List<SelectListItem> RiesgoAsociadoBDEI(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_riesgo_asociado_bdei.ToList();
            foreach (var item in lista)
            {

                if ((item.esta_activo ?? false) || item.id_riesgo_asociado_bdei == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_riesgo_asociado_bdei.ToString(),
                            Text = item.cl_riesgo_asociado_bdei + "-" + item.nb_riesgo_asociado_bdei,
                            Selected = selected == item.id_riesgo_asociado_bdei
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Minimo riesgo operativo
        public static List<SelectListItem> MinimoRiesgoOperativo(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_minimo_riesgo_operativo.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_minimo_riesgo_operativo == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_minimo_riesgo_operativo.ToString(),
                            Text = item.cl_minimo_riesgo_operativo + "-" + item.nb_minimo_riesgo_operativo,
                            Selected = selected == item.id_minimo_riesgo_operativo
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Causa
        public static List<SelectListItem> CausaBDEI(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_causa_bdei.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_causa_bdei == selected)
                    dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_causa_bdei.ToString(),
                        Text = item.cl_causa_bdei + "-" + item.nb_causa_bdei,
                        Selected = selected == item.id_causa_bdei
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Catalogo de Conceptos
        public static List<SelectListItem> CatalogoConceptosBDEI(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_catalogo_concepto.ToList();
            foreach (var item in lista)
            {
                if ((item.esta_activo ?? false) || item.id_catalogo_concepto == selected)
                    dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_catalogo_concepto.ToString(),
                        Text = item.cl_catalogo_concepto + "-" + item.nb_catalogo_concepto,
                        Selected = selected == item.id_catalogo_concepto
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #endregion

        #region Catálogos asociados a Indicadores

        #region Periodos de Indicadores

        public static List<SelectListItem> PeriodosIndicadores(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_periodo_indicador.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_periodo_indicador.ToString(),
                        Text = item.nb_periodo_indicador,
                        Selected = selected == item.id_periodo_indicador
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #endregion

        #region Catálogos asociados a Controles

        #region Tipología Control
        public static List<SelectListItem> TipologiaControl(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_tipologia_control.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_tipologia_control.ToString(),
                        Text = item.cl_tipologia_control + "-" + item.nb_tipologia_control,
                        Selected = selected == item.id_tipologia_control
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Frecuencia Control
        public static List<SelectListItem> FrecuenciaControl(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_frecuencia_control.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_frecuencia_control.ToString(),
                        Text = item.cl_frecuencia_control + "-" + item.nb_frecuencia_control,
                        Selected = selected == item.id_frecuencia_control
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Naturaleza Control
        public static List<SelectListItem> NaturalezaControl(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_naturaleza_control.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_naturaleza_control.ToString(),
                        Text = item.cl_naturaleza_control + "-" + item.nb_naturaleza_control,
                        Selected = selected == item.id_naturaleza_control
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Categoría Control
        public static List<SelectListItem> CategoriaControl(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_categoria_control.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_categoria_control.ToString(),
                        Text = item.cl_categoria_control + "-" + item.nb_categoria_control,
                        Selected = selected == item.id_categoria_control
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Tipo de Evidencia Control
        public static List<SelectListItem> TipoEvidenciaControl(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_tipo_evidencia.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_tipo_evidencia.ToString(),
                        Text = item.cl_tipo_evidencia + "-" + item.nb_tipo_evidencia,
                        Selected = selected == item.id_tipo_evidencia
                    }
                );
            }
            return dropdown;
        }
        #endregion




        #endregion

        #region Calificaciones
        public static List<SelectListItem> Calificaciones(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_calificacion.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_calificacion.ToString(),
                        Text = item.nb_calificacion,
                        Selected = selected == item.id_calificacion
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Calificaciones Revisión
        public static List<SelectListItem> CalificacionesRevision(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_calificacion_revision.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_calificacion_revision.ToString(),
                        Text = item.nb_calificacion_revision,
                        Selected = selected == item.id_calificacion_revision
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Normatividad
        public static List<SelectListItem> NivelesNormatividad(int id_norm, int selected = 0)
        {
            renewDB();

            var norm = db.c_normatividad.Find(id_norm);

            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = norm.c_nivel_normatividad.OrderBy(n => n.no_orden).ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_nivel_normatividad.ToString(),
                        Text = item.cl_nivel_normatividad + " - " + item.nb_nivel_normatividad,
                        Selected = selected == item.id_nivel_normatividad
                    }
                );
            }
            return dropdown;
        }

        #endregion

        #region Auditorias
        public static List<SelectListItem> Auditorias(int selected = 0, List<int> idsDivisiones = null)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_auditoria.ToList();
            foreach (var item in lista)
            {
                if ((idsDivisiones != null && idsDivisiones.Contains(item.id_division_auditoria ?? 0)) || idsDivisiones == null)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_auditoria.ToString(),
                            Text = item.nb_auditoria,
                            Selected = selected == item.id_auditoria
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Divisiones de auditoria
        public static List<SelectListItem> DivisionesAuditoria(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_division_auditoria.ToList();
            foreach (var item in lista)
            {
                if (item.esta_activo || item.id_division_auditoria == selected)
                    dropdown.Add(
                        new SelectListItem
                        {
                            Value = item.id_division_auditoria.ToString(),
                            Text = item.ds_division_auditoria,
                            Selected = selected == item.id_division_auditoria
                        }
                    );
            }
            return dropdown;
        }
        #endregion

        #region Solicitantes de Auditoría
        public static List<SelectListItem> SolicitantesAuditoria(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_solicitante_auditoria.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_solicitante_auditoria.ToString(),
                        Text = item.cl_solicitante_auditoria + "-" + item.nb_solicitante_auditoria,
                        Selected = selected == item.id_solicitante_auditoria
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Periodos de Auditoría
        public static List<SelectListItem> PeriodosAuditoria(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_periodo_auditoria.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_periodo_auditoria.ToString(),
                        Text = item.cl_periodo_auditoria + "-" + item.nb_periodo_auditoria,
                        Selected = selected == item.id_periodo_auditoria
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Raitings
        public static List<SelectListItem> RatingAuditoria(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_rating_auditoria.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_rating_auditoria.ToString(),
                        Text = item.nb_rating_auditoria,
                        Selected = selected == item.id_rating_auditoria
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Estatus
        public static List<SelectListItem> EstatusPrograma(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_estatus_programa_trabajo.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_estatus_programa_trabajo.ToString(),
                        Text = item.nb_estatus_programa_trabajo,
                        Selected = selected == item.id_estatus_programa_trabajo
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Estatus
        public static List<SelectListItem> CriticidadPrograma(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_criticidad_programa_trabajo.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_criticidad_programa_trabajo.ToString(),
                        Text = item.nb_criticidad_programa_trabajo,
                        Selected = selected == item.id_criticidad_programa_trabajo
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Meses del Año (Usados de inicio en Auditoría)
        public static List<SelectListItem> Meses(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();
            var lista = db.c_mes.ToList();
            foreach (var item in lista)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = item.id_mes.ToString(),
                        Text = item.nb_mes,
                        Selected = selected == item.id_mes
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region Trimestres del año (Usados de inicio en Reportes regulatorios R28)
        public static List<SelectListItem> Trimestres(int selected = 0)
        {
            renewDB();
            List<SelectListItem> dropdown = new List<SelectListItem>();

            string[] trimestres = new string[] {
                "Enero - Marzo",
                "Abril - Junio",
                "Julio - Septiembre",
                "Octubre - Diciembre"
            };


            for (int i = 1; i <= 4; i++)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = trimestres[i - 1],
                        Selected = selected == i
                    }
                );
            }
            return dropdown;
        }
        #endregion

        #region TimeZones
        public static List<SelectListItem> TimeZones(string selected = "")
        {
            List<SelectListItem> dropdown = new List<SelectListItem>();

            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            selected = selected == "" ? TimeZoneInfo.Local.Id : selected;

            foreach (var timeZone in timeZones)
            {
                dropdown.Add(
                    new SelectListItem
                    {
                        Value = timeZone.Id,
                        Text = timeZone.Id,
                        Selected = selected == timeZone.Id
                    }
                );
            }

            return dropdown;
        }
        #endregion

        #region MultiSelect

        #region OficiosMSL
        public static MultiSelectList OficiosMSL(k_objeto objeto = null)
        {
            renewDB();
            int id_objeto = objeto != null ? objeto.id_objeto : 0;

            string sql = "select id_oficio_padre from c_oficio_relacionado where id_oficio_hijo = " + id_objeto;
            var oficios = db.Database.SqlQuery<int>(sql).ToArray();

            IEnumerable<k_objeto> ObjetoList;
            if (objeto != null)
            {
                ObjetoList = db.k_objeto.Where(o => o.tipo_objeto == 1 && o.id_objeto != objeto.id_objeto);
            }
            else
            {
                ObjetoList = db.k_objeto.Where(o => o.tipo_objeto == 1);
            }

            return new MultiSelectList(ObjetoList, "id_objeto", "nb_objeto", oficios);
        }
        #endregion

        #region Origen de Autoridad
        public static MultiSelectList OrigenAutoridadMS(int[] selected = null)
        {
            renewDB();
            List<c_origen_autoridad> lista;
            lista = db.c_origen_autoridad.ToList();
            return new MultiSelectList(lista, "id_origen_autoridad", "nb_origen_autoridad", selected);
        }
        #endregion

        #region Lineas de NegicioMSL
        public static MultiSelectList LineasNegocioMS(int[] selected = null)
        {
            renewDB();
            if (selected != null)
                return new MultiSelectList(db.c_linea_negocio.OrderBy(x => x.nb_linea_negocio), "id_linea_negocio", "nb_linea_negocio", selected);
            else
                return new MultiSelectList(db.c_linea_negocio.OrderBy(x => x.nb_linea_negocio), "id_linea_negocio", "nb_linea_negocio");
        }
        #endregion

        #region Lineas de Areas de Costeo MS
        public static MultiSelectList AreasCosteoMS(int[] selected = null)
        {
            renewDB();
            if (selected != null)
                return new MultiSelectList(db.c_area_costeo.OrderBy(x => x.nb_area_costeo), "id_area_costeo", "nb_area_costeo", selected);
            else
                return new MultiSelectList(db.c_area_costeo.OrderBy(x => x.nb_area_costeo), "id_area_costeo", "nb_area_costeo");
        }
        #endregion

        #region Entidades
        public static MultiSelectList EntidadesMS(int[] selected = null)
        {
            renewDB();
            List<c_entidad> lista;
            lista = db.c_entidad.ToList();
            return new MultiSelectList(lista, "id_entidad", "nb_entidad", selected);
        }
        #endregion

        #region Áreas
        public static MultiSelectList AreasMS(int[] selected = null)
        {
            renewDB();
            List<c_area> lista;
            lista = db.c_area.ToList();
            return new MultiSelectList(lista, "id_area", "nb_area", selected);
        }
        #endregion

        #region Periodos Certificación
        public static MultiSelectList PeriodosCertificacionMS(int[] selected = null)
        {
            renewDB();
            List<c_periodo_certificacion> lista;
            lista = db.c_periodo_certificacion.ToList();

            return new MultiSelectList(lista, "id_periodo_certificacion", "nb_periodo_certificacion", selected);
        }
        #endregion

        #region Periodos Indicadores
        public static MultiSelectList PeriodosIndicadoresMS(int[] selected = null)
        {
            renewDB();
            List<c_periodo_indicador> lista;
            lista = db.c_periodo_indicador.ToList();

            return new MultiSelectList(lista, "id_periodo_indicador", "nb_periodo_indicador", selected);
        }
        #endregion

        #region Puestos
        public static MultiSelectList PuestosMS(int[] selected = null)
        {
            renewDB();
            List<SelectListItem> lista;
            var puestoRaiz = Utilidades.getRoot(db);

            lista = addChildDataP(puestoRaiz).OrderBy(p => p.Text).ToList();
            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        public static MultiSelectList NivelesPuestosMS(int[] selected = null)
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();
            var puestos = db.c_puesto.ToList();

            int max = 0;

            //Encontramos el valor más alto de los niveles
            foreach (var puesto in puestos)
            {
                int cl = int.Parse(puesto.cl_puesto);
                if (cl > max) max = cl;
            }//En este punto ya tenemos la clave con el nivel máximo

            for (int i = 1; i <= max; i++)
            {
                lista.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = "Nivel " + i
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        private static List<SelectListItem> addChildDataP(c_puesto puesto)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            //añadimos el puesto actual
            lista.Add(new SelectListItem()
            {
                Text = puesto.cl_puesto + " - " + puesto.nb_puesto,
                Value = puesto.id_puesto.ToString()
            });
            //Buscamos todos los hijos de este puesto y los añadimos
            var hijos = db.c_puesto.Where(ph => ph.id_puesto_padre == puesto.id_puesto).ToList();

            foreach (var hijo in hijos)
            {
                lista = lista.Union(addChildDataP(hijo)).ToList();
            }
            return lista;
        }



        #endregion

        #region Usuarios
        public static MultiSelectList UsuariosMS(int[] selected = null, bool solo_activos = true)
        {
            renewDB();
            List<c_usuario> users;

            if (solo_activos)
            {
                users = db.c_usuario.Where(u => u.esta_activo).ToList();
            }
            else
            {
                users = db.c_usuario.ToList();
            }
            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }

        public static MultiSelectList UsuariosAuditoriaMS(int[] selected = null, bool solo_activos = true)
        {
            renewDB();
            List<c_usuario> users;

            if (solo_activos)
            {
                users = db.c_usuario.Where(u => u.esta_activo && (u.es_auditor || u.es_auditor_admin)).ToList();
            }
            else
            {
                users = db.c_usuario.Where(u => u.es_auditor || u.es_auditor_admin).ToList();
            }
            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }

        public static MultiSelectList UsuariosYPuestoMS(int[] selected = null, bool solo_activos = true)
        {
            renewDB();
            List<c_usuario> users;

            if (solo_activos)
            {
                users = db.c_usuario.Where(u => u.esta_activo).ToList();
            }
            else
            {
                users = db.c_usuario.ToList();
            }

            List<SelectListItem> Lista = new List<SelectListItem>();

            foreach (var user in users)
            {
                c_puesto puesto;
                try
                {
                    puesto = db.c_puesto.Where(p => p.c_usuario.Any(u => u.id_usuario == user.id_usuario)).First();
                }
                catch
                {
                    puesto = null;
                }
                string nb_puesto = puesto == null ? "" : " - " + puesto.nb_puesto;

                Lista.Add(new SelectListItem()
                {
                    Text = user.nb_usuario + nb_puesto,
                    Value = user.id_usuario.ToString()
                });
            }

            return new MultiSelectList(Lista, "Value", "Text", selected);
        }
        #endregion

        #region Usuarios por Área
        public static MultiSelectList UsuariosPorAreaMS(int[] selected = null, int[] ids_areas = null)
        {
            renewDB();

            List<c_area> areas = new List<c_area>();

            if (ids_areas != null)
            {
                areas = db.c_area.Where(a => ids_areas.Contains(a.id_area)).ToList();
            }
            else
            {
                areas = db.c_area.ToList();
            }

            var lista = new List<SelectListItem>();

            foreach (var area in areas)
            {
                var group = new SelectListGroup { Name = area.nb_area };
                var usuarios = area.c_usuario.OrderBy(u => u.nb_usuario).ToList();

                foreach (var usuario in usuarios)
                {
                    lista.Add(new SelectListItem()
                    {
                        Text = usuario.nb_usuario,
                        Value = usuario.id_usuario.ToString(),
                        Group = group
                    });
                }
            }

            return new MultiSelectList(lista, "Value", "Text", "Group.Name", selected);
        }
        #endregion

        #region Macro Procesos
        public static MultiSelectList MacroProcesosMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<c_macro_proceso> macroProcesos = new List<c_macro_proceso>();

            if (responsable != 0)
            {
                macroProcesos = db.c_macro_proceso.Where(mp => mp.id_responsable == responsable).ToList();
            }
            else
            {
                macroProcesos = db.c_macro_proceso.ToList();
            }

            return new MultiSelectList(macroProcesos, "id_macro_proceso", "cl_macro_proceso", selected);
        }
        #endregion

        #region Procesos
        public static MultiSelectList ProcesosMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<c_proceso> Procesos = new List<c_proceso>();

            if (responsable != 0)
            {
                Procesos = db.c_proceso.Where(p => p.id_responsable == responsable).ToList();
            }
            else
            {
                Procesos = db.c_proceso.ToList();
            }


            List<SelectListItem> lista = new List<SelectListItem>();
            foreach (var p in Procesos)
            {
                lista.Add(new SelectListItem()
                {
                    Value = p.id_proceso.ToString(),
                    Text = p.c_macro_proceso.cl_macro_proceso + " - " + p.cl_proceso
                });
            }


            return new MultiSelectList(lista, "Value", "Text", selected);
        }
        #endregion

        #region Sub procesos
        public static MultiSelectList SubProcesosMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<c_sub_proceso> subProcesos = new List<c_sub_proceso>();

            if (responsable != 0)
            {
                subProcesos = db.c_sub_proceso.Where(sp => sp.id_responsable == responsable).ToList();
            }
            else
            {
                subProcesos = db.c_sub_proceso.ToList();
            }

            return new MultiSelectList(subProcesos, "id_sub_proceso", "cl_sub_proceso", selected);
        }
        #endregion

        #region Indicadores
        public static MultiSelectList IndicadoresMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<c_indicador> indicadores = new List<c_indicador>();

            if (responsable != 0)
            {
                indicadores = db.c_indicador.Where(mp => mp.id_responsable == responsable).ToList();
            }
            else
            {
                indicadores = db.c_indicador.ToList();
            }

            return new MultiSelectList(indicadores, "id_indicador", "cl_indicador", selected);
        }
        #endregion

        #region Indicadores Diarios
        public static MultiSelectList IndicadoresDiariosMS(int[] selected = null)
        {
            renewDB();

            List<c_indicador_diario> indicadores = db.c_indicador_diario.ToList();

            return new MultiSelectList(indicadores, "id_indicador_diario", "cl_indicador_diario", selected);
        }
        #endregion

        #region Fichas
        public static MultiSelectList FichasMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<r_evento> fichas = new List<r_evento>();

            if (responsable != 0)
            {
                fichas = db.r_evento.Where(mp => mp.id_responsable == responsable).ToList();
            }
            else
            {
                fichas = db.r_evento.ToList();
            }

            return new MultiSelectList(fichas, "id_evento", "nb_evento", selected);
        }
        #endregion

        #region k_objeto x usuario x tipo (para oficio, auditoria interna/externa, y otros)
        public static MultiSelectList KObjetoMS(int[] selected = null, int responsable = 0, int tipo = 1)
        {
            renewDB();

            List<k_objeto> kObjeto = new List<k_objeto>();

            if (responsable != 0)
            {
                kObjeto = db.k_objeto.Where(mp => mp.id_responsable == responsable && mp.tipo_objeto == tipo).ToList();
            }
            else
            {
                kObjeto = db.k_objeto.ToList();
            }

            return new MultiSelectList(kObjeto, "id_objeto", "nb_objeto", selected);
        }
        #endregion

        #region k_plan x usuario (para planes de remediación y seguimientos)
        public static MultiSelectList KPlanMS(int[] selected = null, int responsable = 0, bool esSeguimiento = false)
        {
            renewDB();

            List<k_plan> kPlan = new List<k_plan>();

            if (responsable != 0)
            {
                if (esSeguimiento)
                    kPlan = db.k_plan.Where(mp => mp.id_responsable_seguimiento == responsable).ToList();
                else
                    kPlan = db.k_plan.Where(mp => mp.id_responsable == responsable).ToList();
            }
            else
            {
                kPlan = db.k_plan.ToList();
            }

            return new MultiSelectList(kPlan, "id_plan", "nb_plan", selected);
        }
        #endregion

        #region k_incidencia x usuario (para incidencias)
        public static MultiSelectList KIncidenciaMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<k_incidencia> kIncidencia = new List<k_incidencia>();

            if (responsable != 0)
            {
                kIncidencia = db.k_incidencia.Where(mp => mp.id_responsable == responsable).ToList();
            }
            else
            {
                kIncidencia = db.k_incidencia.ToList();
            }

            return new MultiSelectList(kIncidencia, "id_incidencia", "ds_incidencia", selected);
        }
        #endregion

        #region Controles
        public static MultiSelectList ControlesMS(int[] selected = null, int responsable = 0)
        {
            renewDB();

            List<k_control> Controles = new List<k_control>();

            if (responsable != 0)
            {
                Controles = db.k_control.Where(c => c.id_responsable == responsable).ToList();
            }
            else
            {
                Controles = db.k_control.ToList();
            }

            return new MultiSelectList(Controles, "id_control", "relacion_control", selected);
        }
        #endregion

        #region Responsables de entidades
        public static MultiSelectList entidadesResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.c_entidad.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de macro procesos
        public static MultiSelectList macroProcesosResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.c_macro_proceso.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de procesos
        public static MultiSelectList procesosResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.c_proceso.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de sub procesos
        public static MultiSelectList subProcesosResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.c_sub_proceso.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de controles
        public static MultiSelectList controlesResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.k_control1.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de indicadores
        public static MultiSelectList indicadoresResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.c_indicador.Where(i => i.esta_activo).Count() > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de Oficios
        public static MultiSelectList oficiosResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.k_objeto.Where(o => o.tipo_objeto == 1).ToList().Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de Informes
        public static MultiSelectList informesResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.k_objeto.Where(o => o.tipo_objeto == 2).ToList().Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de Incidencias
        public static MultiSelectList incidenciasResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.k_incidencia.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de Planes de Remediación
        public static MultiSelectList planesResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.k_plan.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Responsables de Fichas
        public static MultiSelectList fichasResponsables(int[] selected = null)
        {
            renewDB();
            var users = db.c_usuario.Where(u => u.r_evento.Count > 0);

            return new MultiSelectList(users.OrderBy(u => u.nb_usuario), "id_usuario", "nb_usuario", selected);
        }
        #endregion

        #region Funciones
        public static MultiSelectList FuncionesMS(int[] selected = null, bool audit = false)
        {
            renewDB();
            var menus = db.c_menu_funcion.ToList();

            var lista = new List<SelectListItem>();

            foreach (var menu in menus)
            {
                if (audit && menu.cl_menu_funcion == "AUDI")
                {
                    var group = new SelectListGroup { Name = menu.nb_menu_funcion };
                    var funciones = menu.c_funcion.ToList();

                    foreach (var funcion in funciones)
                    {
                        lista.Add(new SelectListItem()
                        {
                            Text = funcion.nb_funcion,
                            Value = funcion.id_funcion.ToString(),
                            Group = group
                        });
                    }
                }
                else if (!audit && menu.cl_menu_funcion != "AUDI")
                {
                    var group = new SelectListGroup { Name = menu.nb_menu_funcion };
                    var funciones = menu.c_funcion.ToList();

                    foreach (var funcion in funciones)
                    {
                        lista.Add(new SelectListItem()
                        {
                            Text = funcion.nb_funcion,
                            Value = funcion.id_funcion.ToString(),
                            Group = group
                        });
                    }
                }

            }



            return new MultiSelectList(lista, "Value", "Text", "Group.Name", selected);
        }
        #endregion

        #region Aseveraciones
        public static MultiSelectList AseveracionesMS(int[] selected = null)
        {
            renewDB();
            var regs = db.c_aseveracion.ToList();

            var lista = new List<SelectListItem>();

            foreach (var reg in regs)
            {
                lista.Add(new SelectListItem()
                {
                    Text = reg.cl_aseveracion + " - " + reg.nb_aseveracion,
                    Value = reg.id_aseveracion.ToString()
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }
        #endregion

        #region Direcciones Generales Revision
        public static MultiSelectList DireccionGeneralRevisionMS()
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();

            var revisiones = db.k_revision_control.ToList();
            List<string> DireccionesGenerales = new List<string>();

            foreach (var rev in revisiones)
            {
                if (!DireccionesGenerales.Contains(rev.rc_dir_general)) DireccionesGenerales.Add(rev.rc_dir_general);
            }


            foreach (var dg in DireccionesGenerales)
            {
                lista.Add(
                    new SelectListItem
                    {
                        Value = dg,
                        Text = dg
                    }
                );
            }

            return new MultiSelectList(lista, "Value", "Text");
        }
        #endregion

        #region InformesAuditoria
        public static MultiSelectList InformesAuditoriaMS(int[] selected = null)
        {
            renewDB();
            List<k_auditoria> lista;

            lista = db.k_auditoria.ToList();

            var SLL = new List<SelectListItem>();

            foreach (var item in lista)
            {
                SLL.Add(new SelectListItem
                {
                    Value = item.idd_auditoria.ToString(),
                    Text = "Informe: " + item.c_auditoria.nb_auditoria + item.fe_inicial_planeada.Value.ToString("yyyy/MM/dd")
                });
            }


            return new MultiSelectList(SLL, "Value", "Text", selected);
        }
        #endregion

        #endregion

        #region RangoCosteo
        public static List<SelectListItem> ValorRango(int selected = 0)//string selected) int selected = 0//
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();

            //lista.Add(new SelectListItem { Value = "0", Text = "-- Seleccionar --", Selected = true });

            var rangos = db.c_rango.Where(mp => mp.id_rango == mp.id_rango).ToList();

            foreach (var en in rangos)
            {

                lista.Add(
                    new SelectListItem
                    {
                        Value = en.id_rango.ToString(),
                        //Value = en.valor,
                        Text = en.valor + " - " + en.nb_rango,
                        //Selected = false,
                        Selected = selected == en.id_rango ? true : false

                    }
                );
            }
            return lista;
        }


        #endregion

        #region RangoCosteo
        public static List<SelectListItem> NombreRango()
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();

            //var macroprocesos = db.c_entidad.Where(mp => mp.id_entidad == mp.id_entidad).ToList();
            var rangos = db.c_rango.Where(mp => mp.id_rango == mp.id_rango).ToList();
            //var entidades = db.c_macro_proceso.ToList().Where(mp => mp.cl_macro_proceso.StartsWith(smp) && mp.id_entidad == id_entidad); // EJEMPLOS
            //var entidadesMG = db.c_entidad.ToList().Where(mp => mp.cl_entidad.StartsWith("MG"));

            //entidades.
            foreach (var en in rangos)
            {

                lista.Add(
                    new SelectListItem
                    {
                        Value = en.id_rango.ToString(),
                        Text = en.nb_rango,
                        Selected = false
                        //Selected = selected == en.id_rango

                    }
                );
            }
            return lista;
        }


        #endregion


        #region Aseveraciones
        public static MultiSelectList CumplimientoNormatividad (int[] selected = null)
        {
            renewDB();
            var regs = db.c_cumplimiento.ToList();

            var lista = new List<SelectListItem>();

            foreach (var reg in regs)
            {
                lista.Add(new SelectListItem()
                {
                    Text = reg.cl_cumplimiento + " - " + reg.nb_cumplimiento,
                    Value = reg.id_cumplimiento.ToString()
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        public static MultiSelectList AreaNormatividad(int[] selected = null)
        {
            renewDB();
            var regs = db.c_area.ToList();

            var lista = new List<SelectListItem>();

            foreach (var reg in regs)
            {
                lista.Add(new SelectListItem()
                {
                    Text = reg.cl_area + " - " + reg.nb_area,
                    Value = reg.id_area.ToString()
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        public static MultiSelectList FrecuenciaNormatividad(int[] selected = null)
        {
            renewDB();
            var regs = db.c_frecuencia.ToList();

            var lista = new List<SelectListItem>();

            foreach (var reg in regs)
            {
                lista.Add(new SelectListItem()
                {
                    Text = reg.cl_frecuencia + " - " + reg.nb_frecuencia,
                    Value = reg.id_frecuencia.ToString()
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        public static MultiSelectList ComiteNormatividad(int[] selected = null)
        {
            renewDB();
            var regs = db.c_comite.ToList();

            var lista = new List<SelectListItem>();

            foreach (var reg in regs)
            {
                lista.Add(new SelectListItem()
                {
                    Text = reg.cl_comite + " - " + reg.nb_comite,
                    Value = reg.id_comite.ToString()
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        public static MultiSelectList ObligacionNormatividad(int[] selected = null)
        {
            renewDB();
            var regs = db.c_obligacion.ToList();

            var lista = new List<SelectListItem>();

            foreach (var reg in regs)
            {
                lista.Add(new SelectListItem()
                {
                    Text = reg.cl_obligacion + " - " + reg.nb_obligacion,
                    Value = reg.id_obligacion.ToString()
                });
            }

            return new MultiSelectList(lista, "Value", "Text", selected);
        }

        public static List<SelectListItem> RequiereFicha(int selected = 0)//string selected) int selected = 0//
        {
            renewDB();
            List<SelectListItem> lista = new List<SelectListItem>();

            //lista.Add(new SelectListItem { Value = "0", Text = "-- Seleccionar --", Selected = true });

            var ficha = db.c_requiere_ficha.Where(mp => mp.id_requiere_ficha == mp.id_requiere_ficha).ToList();

            foreach (var en in ficha)
            {

                lista.Add(
                    new SelectListItem
                    {
                        Value = en.nb_requiere_ficha.ToString(),
                        //Value = en.valor,
                        Text = en.nb_requiere_ficha,
                        Selected = false,
                        //Selected = selected == en.id_rango ? true : false

                    }
                );
            }
            return lista;
        }

      
        #endregion

    }
}