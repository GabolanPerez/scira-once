using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public class emailViewModel
    {
        public string nb_email { get; set; }
        public bool send { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [AllowHtml]
        public string head { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [AllowHtml]
        public string subject { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [AllowHtml]
        public string body { get; set; }
    }
}