using System;

namespace SCIRA.ViewModels
{
    public class IyP1RepModel
    {
        public long ID { get; set; }
        public string nb_objeto { get; set; }
        public Nullable<int> no_incidencias { get; set; }
        public string lvl_1 { get; set; }
        public string lvl_2 { get; set; }
        public string lvl_3 { get; set; }
        public string lvl_4 { get; set; }
        public string lvl_5 { get; set; }
        public Nullable<System.DateTime> fe_objeto { get; set; }
        public Nullable<System.DateTime> fe_vencimiento { get; set; }
        public Nullable<System.DateTime> fe_contestacion { get; set; }
        public string cnb_entidad { get; set; }
        public int tipo_objeto { get; set; }
        public string cnb_autoridad { get; set; }
        public string nb_resp_oficio { get; set; }
        public Nullable<int> id_incidencia { get; set; }
        public string ds_incidencia { get; set; }
        public string nb_resp_inc { get; set; }
        public string cnb_clasificacion_incidencia { get; set; }
        public Nullable<bool> requiere_plan { get; set; }
        public Nullable<int> id_plan { get; set; }
        public string nb_plan { get; set; }
        public string ds_plan { get; set; }
        public string cnb_area { get; set; }
        public string nb_resp_plan { get; set; }
        public string nb_resp_seguimiento { get; set; }
        public Nullable<System.DateTime> fe_alta_p { get; set; }
        public Nullable<System.DateTime> fe_estimada_implantacion { get; set; }
        public Nullable<System.DateTime> fe_real_solucion { get; set; }
        public Nullable<System.DateTime> fe_seguimiento { get; set; }
        public string obs_seguimiento { get; set; }
        public string ruta_control { get; set; }
        public string codigo_control { get; set; }
        public string cl_certificacion_control { get; set; }
        public string periodo_certificacion { get; set; }
        public string ds_procedimiento_certificacion { get; set; }
        public string codigo_riesgo { get; set; }
        public string evento_riesgo { get; set; }
        public string accion_correctora { get; set; }
        public string ds_objeto { get; set; }
    }
}