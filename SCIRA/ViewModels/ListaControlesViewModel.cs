namespace SCIRA.ViewModels
{
    public class ListaControlesViewModel
    {
        public int id_control { get; set; }
        public string codigo_control { get; set; }
        public string actividad_control { get; set; }
        public string nb_ejecutor { get; set; }
        public string nb_responsable { get; set; }
        public bool tiene_accion_correctora { get; set; }
        public string accion_correctora { get; set; }
    }
}