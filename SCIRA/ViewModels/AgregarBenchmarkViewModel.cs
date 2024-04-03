using SCIRA.Models;
using SCIRA.Properties;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public /*partial*/ class AgregarBenchmarkViewModel
    {
        public AgregarBenchmarkViewModel()
        {
            Procesos = new List<SelectListItem>();
            SubProcesos = new List<SelectListItem>();
            MacroProcesos = new List<SelectListItem>();
            EventosRiesgo = new List<SelectListItem>();
        }

        //Listas
        public IList<SelectListItem> Procesos { get; set; }
        public IList<SelectListItem> SubProcesos { get; set; }
        public IList<SelectListItem> MacroProcesos { get; set; }
        public IList<SelectListItem> EventosRiesgo { get; set; }

        public int id_entidad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_proceso_benchmark { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_sub_proceso_benchmark { get; set; }
        public int id_benchmark { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_sub_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_actividad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public int id_evento_riesgo { get; set; }

        public virtual c_actividad c_actividad { get; set; }
        public virtual c_evento_riesgo c_evento_riesgo { get; set; }
        public virtual c_sub_proceso c_sub_proceso { get; set; }
        public virtual c_sub_proceso_benchmark c_sub_proceso_benchmark { get; set; }
    }
}