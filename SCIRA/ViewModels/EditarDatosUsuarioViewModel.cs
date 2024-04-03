using SCIRA.Properties;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{

    public class EditarDatosUsuarioViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud100")]
        public string nb_usuario { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud100")]
        public string e_mail_principal { get; set; }
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud100")]
        public string e_mail_alterno { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Phone(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public string no_telefono { get; set; }
    }
}