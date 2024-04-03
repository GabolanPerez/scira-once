using System.Collections.Generic;

namespace SCIRA.ViewModels
{
    public class PSPCosteoViewModel
    {
        public PSPCosteoViewModel()
        {
            varcos = new List<variables_costeo>();
        }

        public int id { get; set; }
        public string cbn_entidad { get; set; }
        public string resp_entidad { get; set; }
        public string cbn_macro_proceso { get; set; }
        public string resp_macro_proceso { get; set; }
        public string cbn_proceso { get; set; }
        public string resp_proceso { get; set; }
        public string cbn_sub_proceso { get; set; }
        public string resp_sub_proceso { get; set; }
        public string nb_participante { get; set; }
        public List<variables_costeo> varcos { get; set; }
        public string tiempo_total { get; set; }
    }

    public class variables_costeo
    {
        public string tiempo_invertido { get; set; }
        public string porcentaje { get; set; }
    }
}