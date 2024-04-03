namespace SCIRA.ViewModels
{
    public class MRyCRepModel
    {
        //Datos Sub Proceso
        public string en { get; set; }
        public string respEn { get; set; }
        public string mp { get; set; }
        public string respMp { get; set; }
        public string pr { get; set; }
        public string respPr { get; set; }
        public string sp { get; set; }
        public string descripcionSp { get; set; } //NUEVO
        public string respSp { get; set; }
        public string SpAnterior { get; set; } //NUEVO
        public string SpSiguiente { get; set; } //NUEVO
        public string tipologia_sp { get; set; } //NUEVO
        public string lineas_negocio { get; set; }
        public string etapa { get; set; }
        public string sub_Etapa { get; set; }
        public string areas_involucradas { get; set; }
        public string aplicaciones_relacionadas { get; set; }
        public string clave_manual { get; set; }


        //Datos Riesgo
        public string cl_riesgo { get; set; }
        public string evento_riesgo { get; set; }
        public string categoria_riesgo { get; set; }
        public string tipo_riesgo { get; set; }
        public string clase_tipologia_riesgo { get; set; }
        public string sub_clase_tipologia_riesgo { get; set; }
        public string tipologia_riesgo { get; set; }
        public string tipo_impacto { get; set; }
        public string magnitud_impacto { get; set; }
        public string probabilidad_ocurrencia { get; set; }
        public string criticidad { get; set; }
        public string afectacion_contable { get; set; }
        public string supuesto_normativo { get; set; }
        public string euc { get; set; }


        //Riesgo operativo
        public string es_riesgo_operativo { get; set; }
        public string nb_proceso_ro { get; set; }
        public string nb_producto_ro { get; set; }
        public string nb_sub_tipo_ro { get; set; }
        public string nb_linea_negocio_ro { get; set; }
        public string nb_frecuencia_ro { get; set; }
        public string nb_impacto_ro { get; set; }
        public string criticidad_ro { get; set; }



        //Datos Control

        public string cl_control { get; set; }
        public string actividad_control { get; set; }
        public string evidencia_control { get; set; }
        public string aplicacion { get; set; }
        public string accion_correctora { get; set; }
        public string naturaleza_control { get; set; }
        public string frecuencia_control { get; set; }
        public string categoria_control { get; set; }
        public string tipologia_control { get; set; }
        public string tipo_evidencia { get; set; }
        public string grado_cobertura { get; set; }
        public string responsable_control { get; set; }
        public string ejecutor_control { get; set; }
        public string control_clave { get; set; }


        //campos extra
        #region Campos Extra SP
        public string campo_extra_sp01 { get; set; }
        public string campo_extra_sp02 { get; set; }
        public string campo_extra_sp03 { get; set; }
        public string campo_extra_sp04 { get; set; }
        public string campo_extra_sp05 { get; set; }
        public string campo_extra_sp06 { get; set; }
        public string campo_extra_sp07 { get; set; }
        public string campo_extra_sp08 { get; set; }
        public string campo_extra_sp09 { get; set; }
        public string campo_extra_sp10 { get; set; }
        public string campo_extra_sp11 { get; set; }
        public string campo_extra_sp12 { get; set; }
        public string campo_extra_sp13 { get; set; }
        public string campo_extra_sp14 { get; set; }
        public string campo_extra_sp15 { get; set; }
        public string campo_extra_sp16 { get; set; }
        public string campo_extra_sp17 { get; set; }
        public string campo_extra_sp18 { get; set; }
        public string campo_extra_sp19 { get; set; }
        public string campo_extra_sp20 { get; set; }
        #endregion

        #region Campos Extra Riesgo
        public string campo_extra_r01 { get; set; }
        public string campo_extra_r02 { get; set; }
        public string campo_extra_r03 { get; set; }
        public string campo_extra_r04 { get; set; }
        public string campo_extra_r05 { get; set; }
        public string campo_extra_r06 { get; set; }
        public string campo_extra_r07 { get; set; }
        public string campo_extra_r08 { get; set; }
        public string campo_extra_r09 { get; set; }
        public string campo_extra_r10 { get; set; }
        public string campo_extra_r11 { get; set; }
        public string campo_extra_r12 { get; set; }
        public string campo_extra_r13 { get; set; }
        public string campo_extra_r14 { get; set; }
        public string campo_extra_r15 { get; set; }
        public string campo_extra_r16 { get; set; }
        public string campo_extra_r17 { get; set; }
        public string campo_extra_r18 { get; set; }
        public string campo_extra_r19 { get; set; }
        public string campo_extra_r20 { get; set; }
        #endregion

        #region Campos Extra Control
        public string campo_extra_c01 { get; set; }
        public string campo_extra_c02 { get; set; }
        public string campo_extra_c03 { get; set; }
        public string campo_extra_c04 { get; set; }
        public string campo_extra_c05 { get; set; }
        public string campo_extra_c06 { get; set; }
        public string campo_extra_c07 { get; set; }
        public string campo_extra_c08 { get; set; }
        public string campo_extra_c09 { get; set; }
        public string campo_extra_c10 { get; set; }
        public string campo_extra_c11 { get; set; }
        public string campo_extra_c12 { get; set; }
        public string campo_extra_c13 { get; set; }
        public string campo_extra_c14 { get; set; }
        public string campo_extra_c15 { get; set; }
        public string campo_extra_c16 { get; set; }
        public string campo_extra_c17 { get; set; }
        public string campo_extra_c18 { get; set; }
        public string campo_extra_c19 { get; set; }
        public string campo_extra_c20 { get; set; }
        #endregion
    }
}