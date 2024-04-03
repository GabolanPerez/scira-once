using SCIRA.Properties;
using SCIRA.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public /*partial*/ class AgregarRolViewModel
    {
        public int id_rol { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_rol { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        [Exists(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RolCreate010")]
        public string nb_rol { get; set; }
        public int[] id_funcion { get; set; }
    }
}