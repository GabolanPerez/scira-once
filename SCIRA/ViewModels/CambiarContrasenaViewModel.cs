using SCIRA.Properties;
using SCIRA.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class CambiarContrasenaViewModel
    {

        public string original_password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [CompararClaveCifrada]
        public string password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Compare("repeat_password", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "UsuarioNRChangePasswordIndex009")]
        [PasswordSettings]
        public string new_password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "UsuarioNRChangePasswordIndex008")]
        public string repeat_password { get; set; }

    }
}