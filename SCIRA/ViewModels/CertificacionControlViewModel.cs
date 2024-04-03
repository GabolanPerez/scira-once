using SCIRA.Properties;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class CertificacionControlViewModel
    {
        //Datos de la Certificación
        public int id_certificacion_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_certificacion_control { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el Control a evaluar")]
        public int id_control { get; set; }
        public int id_periodo_certificacion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud4000")]
        public string ds_procedimiento_certificacion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_tipo_evaluacion { get; set; }
        public Nullable<short> no_partidas_minimo { get; set; }
        public Nullable<short> no_partidas_semestre1 { get; set; }
        public Nullable<short> no_partidas_semestre2 { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(0, 32767, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "IndicadorCreate031")]
        public short no_pruebas_realizadas { get; set; }
        public bool tiene_funcionamiento_efectivo { get; set; }
        public bool tiene_disenio_efectivo { get; set; }
        public string ds_plan_remediacion { get; set; }
        public string nb_archivo_1 { get; set; }
        public string nb_archivo_2 { get; set; }
        public string nb_archivo_3 { get; set; }
        public string nb_archivo_4 { get; set; }
        public string nb_archivo_5 { get; set; }
        public System.DateTime fe_registro { get; set; }


        //Datos para la Incidencia
        public int? id_responsable_i { get; set; }
        public int? id_clasificacion_incidencia { get; set; }
        public string ds_incidencia { get; set; }
        public string js_incidencia { get; set; }
        public bool requiere_plan { get; set; }

    }
}