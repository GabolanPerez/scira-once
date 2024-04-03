using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AjustesMailViewModel
    {
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public string mail { get; set; }
        public string password { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public string host { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public int puerto { get; set; }
        public bool ssl { get; set; }
        public bool credenciales { get; set; }

    }
}