namespace SCIRA.ViewModels
{
    public class RangoModel
    {
        //Datos Sub Proceso
        public int id_rango_costeo { get; set; }
        public string cl_rango { get; set; }
        public string nb_rango { get; set; }
        public int valor { get; set; }
        public string cl_rango_costeo { get; set; }
        public decimal pr_costeo { get; set; }
        public int id_rango { get; set; }


        public RangoModel(int id_rango, string cl_rango)
        {
            id_rango_costeo = id_rango;
            cl_rango_costeo = cl_rango;
        }

        public RangoModel(int id_rango)
        {

        }
        public RangoModel()
        {

        }
    }
}