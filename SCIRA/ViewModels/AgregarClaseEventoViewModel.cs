using SCIRA.Properties;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public class AgregarClaseEventoViewModel
    {
        public AgregarClaseEventoViewModel()
        {
            TipoRiesgoOperacional = new List<SelectListItem>();
            SubTipoRiesgoOperacional = new List<SelectListItem>();
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ClaseEventoRiesgoOperacionalCreate004")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ClaseEventoRiesgoOperacionalCreate004")]
        public int id_tipo_riesgo_operacional { get; set; }
        public IList<SelectListItem> TipoRiesgoOperacional { get; set; }
        public IList<SelectListItem> SubTipoRiesgoOperacional { get; set; }

        public int id_clase_evento { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ClaseEventoRiesgoOperacionalCreate005")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ClaseEventoRiesgoOperacionalCreate005")]
        public int id_sub_tipo_riesgo_operacional { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_clase_evento { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(1024, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud1024")]
        public string nb_clase_evento { get; set; }

    }
}