using SCIRA.Models;

namespace SCIRA.ViewModels
{
    public class ListaSubProcesosViewModel
    {
        public string cn_entidad { get; set; }
        public string responsable_entidad { get; set; }
        public string cn_macro_proceso { get; set; }
        public string responsable_macro_proceso { get; set; }
        public string cn_proceso { get; set; }
        public string responsable_proceso { get; set; }
        public string cn_sub_proceso { get; set; }
        public string responsable_sub_proceso { get; set; }
        public int id_sub_proceso { get; set; }
        public int no_riesgos { get; set; }


        public ListaSubProcesosViewModel()
        {
        }

        public ListaSubProcesosViewModel(c_sub_proceso model)
        {
            var pr = model.c_proceso;
            var mp = pr.c_macro_proceso;
            var en = mp.c_entidad;

            cn_entidad = en.cl_entidad + " - " + en.nb_entidad;
            cn_macro_proceso = mp.cl_macro_proceso + " - " + mp.nb_macro_proceso;
            cn_sub_proceso = model.cl_sub_proceso + " - " + model.nb_sub_proceso;
            id_sub_proceso = model.id_sub_proceso;
            no_riesgos = model.k_riesgo.Count;
        }

    }
}