using SCIRA.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AgregarControlMGViewModel
    {

        public k_riesgo k_riesgo { get; set; }
        public c_sub_proceso c_sub_proceso { get; set; }

        // Control
        public int id_sub_proceso { get; set; }
        public int id_control { get; set; }
        [Required(ErrorMessage = "La actividad de control es un campo obligatorio.")]
        [StringLength(4000, ErrorMessage = "La actividad de control puede tener hasta 4000 caracteres.")]
        public string actividad_control { get; set; }
        [Required(ErrorMessage = "El código del control es un campo obligatorio.")]
        [StringLength(1024, ErrorMessage = "El código de control puede tener hasta 1024 caracteres.")]
        public string relacion_control { get; set; }
        [StringLength(4000, ErrorMessage = "La evidencia del control puede tener hasta 4000 caracteres.")]
        [Required(ErrorMessage = "La evidencia del control es un campo obligatorio.")]
        public string evidencia_control { get; set; }
        [Required(ErrorMessage = "El responsable del control es un campo obligatorio.")]
        public Nullable<int> id_responsable { get; set; }

        [Required(ErrorMessage = "La frecuencia del control es un campo requerido")]
        public Nullable<int> id_frecuencia_control { get; set; }
        [Required(ErrorMessage = "La naturaleza del control es un campo requerido")]
        public Nullable<int> id_naturaleza_control { get; set; }
        public bool tiene_accion_correctora { get; set; }
        public string accion_correctora { get; set; }

        public string campo01 { get; set; }
        public string campo02 { get; set; }
        public string campo03 { get; set; }
        public string campo04 { get; set; }
        public string campo05 { get; set; }
        public string campo06 { get; set; }
        public string campo07 { get; set; }
        public string campo08 { get; set; }
        public string campo09 { get; set; }
        public string campo10 { get; set; }
        public string campo11 { get; set; }
        public string campo12 { get; set; }
        public string campo13 { get; set; }
        public string campo14 { get; set; }
        public string campo15 { get; set; }
        public string campo16 { get; set; }
        public string campo17 { get; set; }
        public string campo18 { get; set; }
        public string campo19 { get; set; }
        public string campo20 { get; set; }

        //Datos para la Incidencia
        public int? id_responsable_i { get; set; }
        public int? id_clasificacion_incidencia { get; set; }
        public string ds_incidencia { get; set; }
        public string js_incidencia { get; set; }
        public bool requiere_plan { get; set; }
    }
}