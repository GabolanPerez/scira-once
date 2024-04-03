namespace SCIRA.ViewModels
{
    public class GraphicsViewModel
    {
        public class Dataset
        {
            public string label { get; set; }
            public double[] data { get; set; }
            public string[] backgroundColor { get; set; }
            public string[] dataLabels { get; set; }
            public int id { get; set; }


            //constructors
            public Dataset(string label, int id, double[] data, string[] backgroundColor, string[] dataLabels)
            {
                string[] dl = new string[data.Length];
                if (dataLabels == null)
                {
                    for (int i = 0; i < data.Length; i++) dl[i] = data[i].ToString();
                }
                else
                {
                    dl = dataLabels;
                }

                this.label = label;
                this.id = id;
                this.data = data;
                this.backgroundColor = backgroundColor;
                this.dataLabels = dl;
            }
            public Dataset(string label, int id, double[] data, string backgroundColor, string[] dataLabels)
            {
                string[] dl = new string[data.Length];
                if (dataLabels == null)
                {
                    for (int i = 0; i < data.Length; i++) dl[i] = data[i].ToString();
                }
                else
                {
                    dl = dataLabels;
                }

                string[] bg = new string[data.Length];
                for (int i = 0; i < data.Length; i++) bg[i] = backgroundColor;

                this.label = label;
                this.id = id;
                this.data = data;
                this.backgroundColor = bg;
                this.dataLabels = dataLabels;
            }

        }

        public class Data
        {
            public string[] labels { get; set; }
            public Dataset[] datasets { get; set; }
            public int[] ids { get; set; }
        }
    }
}