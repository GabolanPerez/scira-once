using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RC-Stats", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class GraphicsRevisionControlController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            //DropDowns Filtros
            ViewBag.entidades = (MultiSelectList)Utilidades.DropDown.EntidadesMS();
            ViewBag.puestos = (MultiSelectList)Utilidades.DropDown.PuestosMS();
            ViewBag.usuarios = (MultiSelectList)Utilidades.DropDown.UsuariosMS();
            ViewBag.niveles = (MultiSelectList)Utilidades.DropDown.NivelesPuestosMS();

            var user = (IdentityPersonalizado)User.Identity;
            var us = db.c_usuario.Find(user.Id_usuario);

            var Controles = Utilidades.Utilidades.RTCObject(us, db, "k_control", "1").Cast<k_control>().ToList();


            ViewBag.d0 = DatosG0(Controles);


            ViewBag.revisiones = revisiones(Controles);

            return View();
        }

        #region Detalle de las barras
        public ActionResult Details(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int TPg0)
        {
            var Controles = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            switch (TPg0)
            {
                case 0:
                    ViewBag.title = Strings.getMSG("Controles");
                    break;
                case 1:
                    ViewBag.title = Strings.getMSG("Controles Revisados");
                    break;
                case 2:
                    ViewBag.title = Strings.getMSG("Controles en Proceso de Revisión");
                    break;
                case 3:
                    ViewBag.title = Strings.getMSG("ReporteFallasIndex004");
                    break;
                case 4:
                    ViewBag.title = Strings.getMSG("Controles con Revisión Concluida");
                    break;

            }

            var model = ListaControlesG0(Controles, TPg0);

            return PartialView("DetailViews/DetailsCTR", model);
        }
        #endregion

        #region Filtros1
        public ActionResult loadFilters(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3, int? TPg0, int? TPg1)
        {
            var controles = getFilteredElements(entidades, puestos, usuarios, niveles, check1, check2, check3);

            ViewBag.revisiones = revisiones(controles);

            ViewBag.d0 = DatosG0(controles);

            return PartialView();
        }

        #region Obtener Elementos Filtrados
        private List<k_control> getFilteredElements(string[] entidades, string[] puestos, string[] usuarios, string[] niveles, string check1, string check2, string check3)
        {
            List<k_control> Controles = new List<k_control>();
            List<k_control> ControlesA = new List<k_control>();


            bool ch1 = check1 == "on";
            bool ch2 = check2 == "on";
            bool ch3 = check3 == "on";

            List<c_usuario> LAU = new List<c_usuario>();
            List<c_puesto> LAPU = new List<c_puesto>();
            List<c_puesto> LAPU2 = new List<c_puesto>();
            List<c_puesto> LAPU3 = new List<c_puesto>();

            var user = (IdentityPersonalizado)User.Identity;
            var Us = db.c_usuario.Find(user.Id_usuario);

            var TC = Utilidades.Utilidades.TramoControlInferior(Us.id_usuario, db);

            var CTRLS = Utilidades.Utilidades.RTCObject(Us, db, "k_control", "1").Cast<k_control>().ToList();

            if (usuarios != null)
            {
                foreach (var id in usuarios)
                {
                    int ID = int.Parse(id);
                    var us = db.c_usuario.Find(ID);

                    if (TC.Contains(us)) Controles.AddRange(us.k_control1);
                }
            }

            //obtener lista de Controles filtrando por entidad
            if (entidades != null)
            {
                foreach (var id in entidades)
                {
                    int ID = int.Parse(id);
                    var entidad = db.c_entidad.Find(ID);

                    foreach (var ctrl in CTRLS)
                    {
                        if (ctrl.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad.id_entidad == ID)
                        {
                            ControlesA.Add(ctrl);
                        }
                    }

                    if (ch1) //Unimos
                    {
                        Controles = Controles.Union(ControlesA).ToList();
                    }
                    else if (usuarios != null)//Intersectamos
                    {
                        Controles = Controles.Intersect(ControlesA).ToList();
                    }
                    else
                    {
                        Controles = ControlesA;
                    }
                }
            }



            //Obtener lista de elementos al filtrar por puestos
            //primero se debeb encontrar todos los usuarios pertenecientes a los puestos

            //incluir condicion para obtener puestos inferiores si la opcion está habilitada
            ControlesA = new List<k_control>();

            if (puestos != null)
            {
                foreach (var id in puestos)
                {
                    int ID = int.Parse(id);
                    var puesto = db.c_puesto.Find(ID);
                    LAPU.Add(puesto);
                    if (ch2) //Incluir puestos inferiores
                    {
                        LAPU2 = Utilidades.Utilidades.puestosInferiores(puesto, db);
                        LAPU = LAPU.Union(LAPU2).ToList();
                    }

                }
                LAPU = LAPU.Union(LAPU).ToList();

                foreach (var ps in LAPU)
                {
                    var users = ps.c_usuario.ToList();

                    LAU = LAU.Union(users).ToList();
                }//Aquí tenemos la lista de todos los usuarios pertenecientes a los puestos seleccionados

                foreach (var us in LAU)
                {
                    if (TC.Contains(us)) ControlesA.AddRange(us.k_control1);
                }

                if (ch1) //Unimos
                {
                    Controles = Controles.Union(ControlesA).ToList();
                }
                else if (usuarios != null || entidades != null)//Intersectamos
                {
                    Controles = Controles.Intersect(ControlesA).ToList();
                }
                else
                {
                    Controles = ControlesA;
                }
            }

            ControlesA = new List<k_control>();

            //obtenemos los puestos que tengan los niveles seleccionados
            if (niveles != null)
            {
                LAPU = new List<c_puesto>();
                foreach (var lvl in niveles)
                {
                    var cl = string.Format("{0:00}", int.Parse(lvl));
                    LAPU2 = db.c_puesto.Where(p => p.cl_puesto == cl).ToList();
                    LAPU.AddRange(LAPU2);
                    if (ch3)
                    {
                        foreach (var ps in LAPU2)
                        {
                            LAPU.AddRange(Utilidades.Utilidades.puestosInferiores(ps, db));
                        }
                    }
                }
                LAPU = LAPU.Union(LAPU).ToList();

                foreach (var ps in LAPU)
                {
                    var users = ps.c_usuario.ToList();

                    LAU = LAU.Union(users).ToList();
                }//Aquí tenemos la lista de todos los usuarios pertenecientes a los puestos seleccionados

                foreach (var us in LAU)
                {
                    if (TC.Contains(us)) ControlesA.AddRange(us.k_control1);
                }

                if (ch1) //Unimos
                {
                    Controles = Controles.Union(ControlesA).ToList();
                }
                else if (usuarios != null || puestos != null || entidades != null)//Intersectamos
                {
                    Controles = Controles.Intersect(ControlesA).ToList();
                }
                else
                {
                    Controles = ControlesA;
                }
            }


            if (usuarios == null && puestos == null && niveles == null && entidades == null)
            {
                Controles = CTRLS;
            }
            return Controles;
        }
        #endregion

        #endregion

        #region Funciones Cliente


        #endregion

        #region Datos
        private string DatosG0(List<k_control> Controles) //datos para la gráfica 0 "Conteo General"
        {

            var ControlesSeparados = GetG0Count(Controles);

            var todos = ControlesSeparados.Todos;
            var SR = ControlesSeparados.SinRevision;
            var EP = ControlesSeparados.EnProceso;
            var RE = ControlesSeparados.Revisados;
            var CO = ControlesSeparados.Concluidos;


            //datos de entrada

            double[] data = { (double)todos.Count, (double)RE.Count, (double)EP.Count, (double)SR.Count, (double)CO.Count };

            string[] bgc = Utilidades.Utilidades.getColorArray(5);

            var dataset = new GraphicsViewModel.Dataset(Strings.getMSG("Controles"), 1, data, bgc, null);

            string[] labels = { Strings.getMSG("PlantillaIndex002"), Strings.getMSG("Revisados"), Strings.getMSG("En Proceso"), Strings.getMSG("Sin Revisión"), Strings.getMSG("ReporteIyPIndex018") };
            GraphicsViewModel.Dataset[] datasets = { dataset };

            var Data0 = new GraphicsViewModel.Data()
            {
                labels = labels,
                datasets = datasets
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data0);
        }

        #endregion

        #region Auxiliares
        private ControlesG0 GetG0Count(List<k_control> Controles)
        {
            ControlesG0 model = new ControlesG0();
            model.Todos = Controles;


            foreach (var control in Controles)
            {
                var status = Utilidades.Utilidades.GetStatus(control);

                if (status == Strings.getMSG("Revisado"))
                {
                    model.Revisados.Add(control);
                }
                else
                {
                    if (status == Strings.getMSG("RevisionControlCreate116"))
                    {
                        model.Concluidos.Add(control);
                    }
                    else
                    {
                        if (status == Strings.getMSG("No Revisado"))
                        {
                            model.SinRevision.Add(control);
                        }
                        else
                        {
                            model.EnProceso.Add(control);
                        }
                    }
                }

            }
            return model;
        }

        private List<k_control> ListaControlesG0(List<k_control> Controles, int TP)
        {
            var Controles1 = new List<k_control>();

            ControlesG0 ControlesSeparados = GetG0Count(Controles);

            if (TP == 0) Controles1 = ControlesSeparados.Todos;
            if (TP == 1) Controles1 = ControlesSeparados.Revisados;
            if (TP == 2) Controles1 = ControlesSeparados.EnProceso;
            if (TP == 3) Controles1 = ControlesSeparados.SinRevision;
            if (TP == 4) Controles1 = ControlesSeparados.Concluidos;

            return Controles1;
        }

        private List<k_revision_control> revisiones(List<k_control> controles)
        {
            var revisiones = new List<k_revision_control>();

            foreach (var control in controles)
            {
                revisiones.AddRange(control.k_revision_control);
            }


            return revisiones;
        }

        #region Clases
        private class ControlesG0
        {
            public List<k_control> SinRevision { get; set; }
            public List<k_control> EnProceso { get; set; }
            public List<k_control> Revisados { get; set; }
            public List<k_control> Concluidos { get; set; }
            public List<k_control> Todos { get; set; }

            public ControlesG0()
            {
                SinRevision = new List<k_control>();
                EnProceso = new List<k_control>();
                Revisados = new List<k_control>();
                Concluidos = new List<k_control>();
                Todos = new List<k_control>();
            }
        }
        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
