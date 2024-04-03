using SCIRA.Properties;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.ViewModels
{
    public class AgregarNormatividadViewModel
    {
        //Normatividad
        public int id_normatividad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_normatividad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_normatividad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "IndicadorCreate026")]
        public string ds_normatividad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NormatividadCreate011")]
        public Nullable<System.DateTime> fe_publicacion_dof { get; set; }
        [StringLength(512, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NormatividadCreate020")]
        public string ds_sectores { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "NormatividadCreate038")]
        public Nullable<int> id_tipo_normatividad { get; set; }
        public Nullable<int> id_root_contenido { get; set; }

        //los parametros para la tabla c_contenido_normatividad, se tomaran de los ya existentes
        public int id_contenido_normatividad { get; set; }
        public bool aparece_en_reporte { get; set; }

        //Nivel de Normatividad
        public int id_nivel { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud20")]
        public string cl_nivel_normatividad { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validacion001")]
        [StringLength(256, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Longitud256")]
        public string nb_nivel_normatividad { get; set; }
        public short no_orden { get; set; }
        //tomar el id de normatividad de la normatividad creada

    }
}