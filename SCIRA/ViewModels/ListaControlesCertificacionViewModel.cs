namespace SCIRA.ViewModels
{
    public class ListaControlesCertificacionViewModel
    {
        public int id_sub_proceso { get; set; }
        public string cl_riesgo { get; set; }
        public string nb_riesgo { get; set; }
        public string evento { get; set; }
        public string relacion_control { get; set; }
        public string actividad_control { get; set; }
        public string accion_correctora { get; set; }
        public int id_control { get; set; }
        public string cn_sub_proceso { get; set; }
    }
}