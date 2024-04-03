using SCIRA.Models;
using SCIRA.Properties;
using SCIRA.Validaciones;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public /*partial*/ class AgregarProcesoViewModel
    {
        public AgregarProcesoViewModel()
        {
            Entidades = new List<SelectListItem>();
            MacroProcesos = new List<SelectListItem>();
        }

        public int id_entidad { get; set; }
        public IList<SelectListItem> Entidades { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_macro_proceso { get; set; }
        public IList<SelectListItem> MacroProcesos { get; set; }

        public int id_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(maximumLength: 50,MinimumLength = 2, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud49")]
        //[RegularExpression("^(P)[0-9]{2}$", ErrorMessage = "La clave debe estar conformada por P y 2 dígitos.")]
        [pCode]
        public string cl_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_responsable { get; set; }

        // public virtual c_macro_proceso c_macro_proceso { get; set; }
        public virtual c_usuario c_usuario { get; set; }
    }
}