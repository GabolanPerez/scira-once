using SCIRA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public class EditarRiesgoViewModel
    {
        public EditarRiesgoViewModel()
        {
            CategoriasRiesgo = new List<SelectListItem>();
            TiposRiesgo = new List<SelectListItem>();

            ClasesTipologiaRiesgo = new List<SelectListItem>();
            SubClasesTipologiaRiesgo = new List<SelectListItem>();
            TipologiasRiesgo = new List<SelectListItem>();
        }

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

        //
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int id_categoria_riesgo { get; set; }
        public IList<SelectListItem> CategoriasRiesgo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int id_tipo_riesgo { get; set; }
        public IList<SelectListItem> TiposRiesgo { get; set; }

        //
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int id_clase_tipologia_riesgo { get; set; }
        public IList<SelectListItem> ClasesTipologiaRiesgo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int id_sub_clase_tipologia_riesgo { get; set; }
        public IList<SelectListItem> SubClasesTipologiaRiesgo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int id_tipologia_riesgo { get; set; }
        public IList<SelectListItem> TipologiasRiesgo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public Nullable<int> id_probabilidad_ocurrencia { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public Nullable<int> id_tipo_impacto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public Nullable<int> id_magnitud_impacto { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public string criticidad { get; set; }
        public bool tiene_afectacion_contable { get; set; }
        public string supuesto_normativo { get; set; }
        public string euc { get; set; }

        //riesgo operativo
        public bool es_riesgo_operativo { get; set; }
        public int? id_proceso_riesgo_operacional { get; set; }
        public int? id_sub_tipo_producto_riesgo_operacional { get; set; }
        public int? id_sub_tipo_riesgo_operacional { get; set; }
        public int? id_linea_negocio_riesgo_operacional { get; set; }
        public int? id_frecuencia_riesgo_operacional { get; set; }
        public int? id_impacto_riesgo_operacional { get; set; }

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
    }
}