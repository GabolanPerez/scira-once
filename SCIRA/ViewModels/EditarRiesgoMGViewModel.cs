
using SCIRA.Models;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class EditarRiesgoMGViewModel
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