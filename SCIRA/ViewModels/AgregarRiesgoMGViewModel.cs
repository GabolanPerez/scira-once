using SCIRA.Models;
using SCIRA.Validaciones;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AgregarRiesgoMGViewModel
    {
        public int id_riesgo { get; set; }
        public c_sub_proceso c_sub_proceso { get; set; }

        // Riesgo
        public int id_sub_proceso { get; set; }
        public string cl_riesgo { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [StringLength(20, ErrorMessage = "Este campo puede tener hasta 20 caracteres.")]
        public string nb_riesgo { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [StringLength(4000, ErrorMessage = "Este campo puede tener hasta 4000 caracteres.")]
        public string evento { get; set; }


        // Control
        public int id_control { get; set; }
        [AccionCorrectora(ErrorMessage = "Este campo es obligatorio.")]
        [StringLength(4000, ErrorMessage = "Este campo puede tener hasta 4000 caracteres.")]
        public string actividad_control { get; set; }
        [AccionCorrectora(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(1024, ErrorMessage = "Este campo puede tener hasta 1024 caracteres.")]
        public string relacion_control { get; set; }
        [AccionCorrectora(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(4000, ErrorMessage = "Este campo puede tener hasta 4000 caracteres.")]
        public string evidencia_control { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int id_responsable { get; set; }

        [AccionCorrectora(ErrorMessage = "Este campo es obligatorio.")]
        public Nullable<int> id_frecuencia_control { get; set; }
        [AccionCorrectora(ErrorMessage = "Este campo es obligatorio.")]
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


        public string campor01 { get; set; }
        public string campor02 { get; set; }
        public string campor03 { get; set; }
        public string campor04 { get; set; }
        public string campor05 { get; set; }
        public string campor06 { get; set; }
        public string campor07 { get; set; }
        public string campor08 { get; set; }
        public string campor09 { get; set; }
        public string campor10 { get; set; }
        public string campor11 { get; set; }
        public string campor12 { get; set; }
        public string campor13 { get; set; }
        public string campor14 { get; set; }
        public string campor15 { get; set; }
        public string campor16 { get; set; }
        public string campor17 { get; set; }
        public string campor18 { get; set; }
        public string campor19 { get; set; }
        public string campor20 { get; set; }

        //Datos para la Incidencia
        public int? id_responsable_i { get; set; }
        public int? id_clasificacion_incidencia { get; set; }
        public string ds_incidencia { get; set; }
        public string js_incidencia { get; set; }
        public bool requiere_plan { get; set; }

    }
}