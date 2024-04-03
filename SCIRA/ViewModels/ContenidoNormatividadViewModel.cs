using System;

namespace SCIRA.ViewModels
{
    public class ContenidoNormatividadViewModel
    {
        public int id_contenido_normatividad { get; set; }
        public Nullable<int> id_contenido_normatividad_padre { get; set; }
        public string cl_contenido_normatividad { get; set; }
        public string ds_contenido_normatividad { get; set; }
        public int id_nivel_normatividad { get; set; }
        public string nb_nivel_normatividad { get; set; }
        public string sig_nivel { get; set; }
        public string aparece_en_reporte { get; set; }
    }
}