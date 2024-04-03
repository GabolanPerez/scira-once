using SCIRA.Models;
using SCIRA.Properties;
using SCIRA.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SCIRA.ViewModels
{
    public /*partial*/ class AgregarSubProcesoViewModel
    {
        public AgregarSubProcesoViewModel()
        {
            Entidades = new List<SelectListItem>();
            MacroProcesos = new List<SelectListItem>();
            Procesos = new List<SelectListItem>();
        }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "CuentaContableCreate004")]
        public int id_entidad { get; set; }
        public IList<SelectListItem> Entidades { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate007")]
        public int id_macro_proceso { get; set; }
        public IList<SelectListItem> MacroProcesos { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate008")]
        public int id_proceso { get; set; }
        public IList<SelectListItem> Procesos { get; set; }

        public int id_sub_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate051")]
        //[RegularExpression("^(SP)[0-9]{4}$", ErrorMessage = "La clave debe estar conformada por SP y 4 dígitos.")]
        [spCode]
        public string cl_sub_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_sub_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate020")]
        public string ds_sub_proceso { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EntidadCreate005")]
        public int id_responsable { get; set; }

        public int[] id_responsables { get; set; }
        public int[] id_lineas_negocio { get; set; }
        public int[] id_areas_costeo { get; set; }
        //public List<UsuarioMarcado> responsables { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_manual { get; set; }

        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string cl_sp_anterior { get; set; }
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string cl_sp_siguiente { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        public Nullable<int> id_tipologia_sub_proceso { get; set; }
        public Nullable<int> id_etapa { get; set; }
        public Nullable<int> id_sub_etapa { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate022")]
        public string ds_areas_involucradas { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SubProcesoCreate022")]
        public string ds_aplicaciones_relacionadas { get; set; }
        public string nb_archivo_manual { get; set; }
        public string nb_archivo_flujo { get; set; }

        public string campo01 { get; set; }
        public string campo02 { get; set; }
        public string campo03 { get; set; }
        public string campo04 { get; set; }
        public string campo05 { get; set; }
        public string campo06 { get; set; }
        public string campo07 { get; set; }
        public string campo08 { get; set; }
        public string campo09 { get; set; }
        public string campo10 { get; set; }
        public string campo11 { get; set; }
        public string campo12 { get; set; }
        public string campo13 { get; set; }
        public string campo14 { get; set; }
        public string campo15 { get; set; }
        public string campo16 { get; set; }
        public string campo17 { get; set; }
        public string campo18 { get; set; }
        public string campo19 { get; set; }
        public string campo20 { get; set; }

        public virtual c_etapa c_etapa { get; set; }
        public virtual c_proceso c_proceso { get; set; }
        public virtual c_sub_etapa c_sub_etapa { get; set; }
        public virtual c_usuario c_usuario { get; set; }
        public virtual c_tipologia_sub_proceso c_tipologia_sub_proceso { get; set; }
    }
}