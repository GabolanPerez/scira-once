using SCIRA.Models;
using SCIRA.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public /*partial*/ class AgregarSubProcesoBenchViewModel
    {
        public AgregarSubProcesoBenchViewModel()
        {
            Entidades = new List<SelectListItem>();
            MacroProcesos = new List<SelectListItem>();
            Procesos = new List<SelectListItem>();
        }

        public Nullable<int> id_entidad { get; set; }
        public IList<SelectListItem> Entidades { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ProcesoBenchmarkCreate003")]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ProcesoBenchmarkCreate003")]
        public int id_actividad { get; set; }
        public IList<SelectListItem> MacroProcesos { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoBenchmarkCreate008")]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoBenchmarkCreate008")]
        public int id_proceso_benchmark { get; set; }
        public IList<SelectListItem> Procesos { get; set; }

        public int id_sub_proceso_benchmark { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_sub_proceso_benchmark { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_sub_proceso_benchmark { get; set; }


        public virtual c_proceso_benchmark c_proceso_benchmark { get; set; }
    }
}