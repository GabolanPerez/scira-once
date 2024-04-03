using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using Syncfusion.EJ2;
using Syncfusion.EJ2.Diagrams;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [CustomErrorHandler]
    public class DiagramaController : Controller
    {
        private SICIEntities db = new SICIEntities();
        //private IEstructuraRepository _repository;
        private ISelectListRepository _repository;

        public DiagramaController() : this(new SelectListRepository())
        {
        }

        public DiagramaController(ISelectListRepository repository)
        {
            _repository = repository;
        }

        [Access(Funcion = "DiagEN", ModuleCode = "MSICI012")]
        public ActionResult Entidad()
        {
            List<c_entidad> entidades;

            var user = (IdentityPersonalizado)User.Identity;
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            entidades = Utilidades.Utilidades.RTCObject(Usuario, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();

            List<int> HasDiagram = new List<int>();
            List<int> HasHistorial = new List<int>();

            foreach (var r in entidades)
            {
                string baseFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/EN/");
                string[] files = Directory.GetFiles(baseFilePath, r.id_entidad + "*");

                if (files.Length > 0)
                {
                    // Si hay algún archivo con el formato "id_fechaHora"
                    HasDiagram.Add(r.id_entidad);

                    // Si hay más de un archivo, agregamos el id de la entidad a HasHistorial
                    if (files.Length > 1)
                    {
                        HasHistorial.Add(r.id_entidad);
                    }
                    else
                    {
                        // Verificar si el único archivo tiene el formato "id" sin fecha
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[0]);
                        string[] parts = fileNameWithoutExtension.Split('_');
                        if (parts.Length == 1)
                        {
                            // Si solo hay un archivo con el formato "id", completarlo con la fecha actual
                            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string newFilePath = $"{baseFilePath}{r.id_entidad}_{timeStamp}";
                            System.IO.File.Move(files[0], newFilePath);
                        }
                    }
                }
            }

            ViewBag.hasDiagram = HasDiagram;
            ViewBag.HasHistorial = HasHistorial;
            return View(entidades);
        }

        [Access(Funcion = "DiagMP", ModuleCode = "MSICI012")]
        public ActionResult MacroProceso()
        {
            List<c_macro_proceso> mps;

            var user = ((Seguridad.IdentityPersonalizado)User.Identity);
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            mps = Utilidades.Utilidades.RTCObject(Usuario, db, "c_macro_proceso").Cast<c_macro_proceso>()
                .OrderBy(x => x.c_entidad.cl_entidad)
                .OrderBy(x => x.cl_macro_proceso).ToList();


            List<int> HasDiagram = new List<int>();
            List<int> HasHistorial = new List<int>();

            foreach (var r in mps)
            {
                string baseFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/MP/");
                string[] files = Directory.GetFiles(baseFilePath, r.id_macro_proceso + "*");

                if (files.Length > 0)
                {
                    // Si hay algún archivo con el formato "id_fechaHora"
                    HasDiagram.Add(r.id_macro_proceso);

                    // Si hay más de un archivo, agregamos el id de la entidad a HasHistorial
                    if (files.Length > 1)
                    {
                        HasHistorial.Add(r.id_macro_proceso);
                    }
                    else
                    {
                        // Verificar si el único archivo tiene el formato "id" sin fecha
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[0]);
                        string[] parts = fileNameWithoutExtension.Split('_');
                        if (parts.Length == 1)
                        {
                            // Si solo hay un archivo con el formato "id", completarlo con la fecha actual
                            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string newFilePath = $"{baseFilePath}{r.id_macro_proceso}_{timeStamp}";
                            System.IO.File.Move(files[0], newFilePath);
                        }
                    }
                }
            }

            ViewBag.hasDiagram = HasDiagram;
            ViewBag.HasHistorial = HasHistorial;

            return View(mps);
        }

        [Access(Funcion = "DiagPR", ModuleCode = "MSICI012")]
        public ActionResult Proceso()
        {
            List<c_proceso> ps;

            var user = ((Seguridad.IdentityPersonalizado)User.Identity);
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            ps = Utilidades.Utilidades.RTCObject(Usuario, db, "c_proceso").Cast<c_proceso>()
                .OrderBy(x => x.c_macro_proceso.c_entidad.cl_entidad)
                .OrderBy(x => x.c_macro_proceso.cl_macro_proceso)
                .OrderBy(x => x.cl_proceso).ToList();

            List<int> HasDiagram = new List<int>();
            List<int> HasHistorial = new List<int>();

            foreach (var r in ps)
            {
                string baseFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/PR/");
                string[] files = Directory.GetFiles(baseFilePath, r.id_proceso + "*");

                if (files.Length > 0)
                {
                    // Si hay algún archivo con el formato "id_fechaHora"
                    HasDiagram.Add(r.id_proceso);

                    // Si hay más de un archivo, agregamos el id de la entidad a HasHistorial
                    if (files.Length > 1)
                    {
                        HasHistorial.Add(r.id_proceso);
                    }
                    else
                    {
                        // Verificar si el único archivo tiene el formato "id" sin fecha
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[0]);
                        string[] parts = fileNameWithoutExtension.Split('_');
                        if (parts.Length == 1)
                        {
                            // Si solo hay un archivo con el formato "id", completarlo con la fecha actual
                            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string newFilePath = $"{baseFilePath}{r.id_proceso}_{timeStamp}";
                            System.IO.File.Move(files[0], newFilePath);
                        }
                    }
                }
            }

            ViewBag.hasDiagram = HasDiagram;
            ViewBag.HasHistorial = HasHistorial;

            return View(ps);
        }

        [Access(Funcion = "DiagSP", ModuleCode = "MSICI012")]
        public ActionResult SubProceso()
        {
            List<c_sub_proceso> sps;

            var user = ((Seguridad.IdentityPersonalizado)User.Identity);
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            sps = Utilidades.Utilidades.RTCObject(Usuario, db, "c_sub_proceso").Cast<c_sub_proceso>().
                OrderBy(x => x.c_proceso.c_macro_proceso.c_entidad.cl_entidad).
                OrderBy(x => x.c_proceso.c_macro_proceso.cl_macro_proceso).
                OrderBy(x => x.c_proceso.cl_proceso).
                OrderBy(x => x.cl_sub_proceso).ToList();


            List<int> HasDiagram = new List<int>();
            List<int> HasHistorial = new List<int>();

            foreach (var r in sps)
            {
                string baseFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/SP/");
                string[] files = Directory.GetFiles(baseFilePath, r.id_sub_proceso + "*");

                if (files.Length > 0)
                {
                    // Si hay algún archivo con el formato "id_fechaHora"
                    HasDiagram.Add(r.id_sub_proceso);

                    // Si hay más de un archivo, agregamos el id de la entidad a HasHistorial
                    if (files.Length > 1)
                    {
                        HasHistorial.Add(r.id_sub_proceso);
                    }
                    else
                    {
                        // Verificar si el único archivo tiene el formato "id" sin fecha
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[0]);
                        string[] parts = fileNameWithoutExtension.Split('_');
                        if (parts.Length == 1)
                        {
                            // Si solo hay un archivo con el formato "id", completarlo con la fecha actual
                            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string newFilePath = $"{baseFilePath}{r.id_sub_proceso}_{timeStamp}";
                            System.IO.File.Move(files[0], newFilePath);
                        }
                    }
                }
            }

            ViewBag.hasDiagram = HasDiagram;
            ViewBag.HasHistorial = HasHistorial;
            return View(sps);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Access(Funcion = "DiagEN", ModuleCode = "MSICI012")]
        public ActionResult EditorEN(int id,string cl_fecha = null)
        {
            var en = db.c_entidad.Find(id);

            return RedirectToAction("Editor", new { id = id, clave = "EN", nbRegistro = $"Entidad {en.cl_entidad} - {en.nb_entidad}", cl_fecha = cl_fecha });
        }

        [Access(Funcion = "DiagMP", ModuleCode = "MSICI012")]
        public ActionResult EditorMP(int id,string cl_fecha = null)
        {
            var mp = db.c_macro_proceso.Find(id);

            return RedirectToAction("Editor", new { id = id, clave = "MP", nbRegistro = $"Macro Proceso {mp.cl_macro_proceso} - {mp.nb_macro_proceso}", cl_fecha = cl_fecha  });
        }

        [Access(Funcion = "DiagPR", ModuleCode = "MSICI012")]
        public ActionResult EditorPR(int id,string cl_fecha = null)
        {
            var pr = db.c_proceso.Find(id);

            return RedirectToAction("Editor", new { id = id, clave = "PR", nbRegistro = $"Proceso {pr.cl_proceso} - {pr.nb_proceso}", cl_fecha = cl_fecha  });
        }


        [Access(Funcion = "DiagSP", ModuleCode = "MSICI012")]
        public ActionResult EditorSP(int id,string cl_fecha = null)
        {
            var sp = db.c_sub_proceso.Find(id);

            return RedirectToAction("Editor", new { id = id, clave = "SP", nbRegistro = $"Sub Proceso {sp.cl_sub_proceso} - {sp.nb_sub_proceso}", cl_fecha = cl_fecha  });
        }

        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        public ActionResult Editor(int id, string clave, string nbRegistro,string cl_fecha = null)
        {
            // Se mandarán los datos de la clave e id a la pantalla para realizar las acciones
            ViewBag.clave = clave;
            ViewBag.id = id;
            ViewBag.nbRegistro = nbRegistro;
            ViewBag.cl_fecha = cl_fecha;

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/" + clave + "/");

            var JsonData = "";

            // Verificar si existen archivos con el formato "id_fechaHora"
            string[] files = Directory.GetFiles(path, id + "_*");

            if (files.Length > 0)
            {
                // Obtener el archivo más reciente según la fecha en el nombre del archivo
                var latestFile = files
                    .OrderByDescending(file => GetDateTimeFromFileName(file))
                    .FirstOrDefault();

                if (latestFile != null)
                {
                    // Leer el contenido del archivo más reciente
                    JsonData = System.IO.File.ReadAllText(latestFile);
                }
            }


            ViewBag.path = clave;
            //ViewBag.JsonData = JsonData;


            #region Puertos
            List<DiagramPort> ports = new List<DiagramPort>();
            ports.Add(new DiagramPort() { Id = "Port1", Offset = new DiagramPoint() { X = 0, Y = 0.5 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port2", Offset = new DiagramPoint() { X = 0.5, Y = 0 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port3", Offset = new DiagramPoint() { X = 1, Y = 0.5 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port4", Offset = new DiagramPoint() { X = 0.5, Y = 1 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });

            #endregion


            #region Palette
            List<DiagramNode> flowShapes = new List<DiagramNode>();
            flowShapes.Add(new DiagramNode() { Id = "Terminator", Ports = ports, Shape = new { type = "Flow", shape = "Terminator" } });
            flowShapes.Add(new DiagramNode() { Id = "Process", Ports = ports, Shape = new { type = "Flow", shape = "Process" } });
            flowShapes.Add(new DiagramNode() { Id = "Decision", Ports = ports, Shape = new { type = "Flow", shape = "Decision" } });
            flowShapes.Add(new DiagramNode() { Id = "Document", Ports = ports, Shape = new { type = "Flow", shape = "Document" } });
            flowShapes.Add(new DiagramNode() { Id = "PreDefinedProcess", Ports = ports, Shape = new { type = "Flow", shape = "PreDefinedProcess" } });
            flowShapes.Add(new DiagramNode() { Id = "PaperTap", Ports = ports, Shape = new { type = "Flow", shape = "PaperTap" } });
            flowShapes.Add(new DiagramNode() { Id = "DirectData", Ports = ports, Shape = new { type = "Flow", shape = "DirectData" } });
            flowShapes.Add(new DiagramNode() { Id = "OffPageReference", Ports = ports, Shape = new { type = "Flow", shape = "OffPageReference" } });
            flowShapes.Add(new DiagramNode() { Id = "Delay", Ports = ports, Shape = new { type = "Flow", shape = "Delay" } });
            flowShapes.Add(new DiagramNode() { Id = "Ellipse", Ports = ports, Shape = new { type = "Basic", shape = "Ellipse" } });

            List<DiagramNode> Risks = new List<DiagramNode>();
            List<DiagramNode> Controls = new List<DiagramNode>();

            if (clave == "SP")
            {
                var sp = db.c_sub_proceso.Find(id);
                var riesgos = sp.k_riesgo;
                var controles = sp.k_control;

                foreach (var r in riesgos)
                {
                    Risks.Add(new DiagramNode() { Id = "r" + r.nb_riesgo, AddInfo = new { id = r.id_riesgo.ToString() }, Shape = new { type = "Basic", shape = "Triangle" } });
                }

                foreach (var c in controles)
                {
                    Controls.Add(new DiagramNode() { Id = "c" + c.relacion_control, AddInfo = new { id = c.id_control.ToString() }, Shape = new { type = "Basic", shape = "Rectangle" } });
                }
            }
            //List<DiagramNode> linkShapes = new List<DiagramNode>();
            //linkShapes.Add(new DiagramNode() { Id = "Control", Shape = new { type = "Basic", shape = "Plus" }});
            //linkShapes.Add(new DiagramNode() { Id = "Riesgo", Shape = new { type = "Basic", shape = "Star" }});


            List<DiagramConnector> paletteConnectors = new List<DiagramConnector>();
            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link1",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 27, Y = 27 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });

            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link2",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 27, Y = 27 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });

            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link3",
                Type = Segments.Straight,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link4",
                Type = Segments.Straight,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link5",
                Type = Segments.Bezier,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link6",
                Type = Segments.Bezier,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });


            //Swimlane
            //List<Lane> lanes = new List<Lane>();
            //Lane lane1 = new Lane();
            //lane1.Id = "lane1";
            //lane1.Height = 1350;
            //lane1.Width = 1080;
            //lane1.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            //lanes.Add(lane1);

            List<Lane> lanes = new List<Lane>();
            Lane lane1 = new Lane();
            lane1.Id = "lane1";
            lane1.Height = 120;
            lane1.Width = 300;
            lane1.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes.Add(lane1);

            List<Lane> lanes1 = new List<Lane>();
            Lane lane2 = new Lane();
            lane2.Id = "lane2";
            lane2.Height = 1380;
            lane2.Width = 1060;
            lane2.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes1.Add(lane2);

            //List<Lane> lanes1 = new List<Lane>();
            //Lane lane2 = new Lane();
            //lane2.Id = "lane2";
            //lane2.Height = 150;
            //lane2.Width = 60;
            //lane2.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            //lanes1.Add(lane2);

            List<DiagramNode> swimlanePalette = new List<DiagramNode>();
            Dictionary<string, object> addInfo5 = new Dictionary<string, object>();
            addInfo5.Add("tooltip", "Horizontal swimlane");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "stackCanvas1",
                Height = 60,
                Width = 140,
                AddInfo = addInfo5,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Lanes = lanes,
                    Orientation = "Horizontal",
                    IsLane = true
                },
                OffsetX = 70,
                OffsetY = 30
            });
            //Dictionary<string, object> addInfo6 = new Dictionary<string, object>();
            //addInfo6.Add("tooltip", "Vertical swimlane");
            //swimlanePalette.Add(new DiagramNode()
            //{
            //    Id = "stackCanvas2",
            //    Height = 140,
            //    Width = 60,
            //    AddInfo = addInfo6,
            //    Shape = new SwimLaneModel()
            //    {
            //        Type = "SwimLane",
            //        Lanes = lanes1,
            //        Orientation = "Vertical",
            //        IsLane = true
            //    },
            //    OffsetX = 70,
            //    OffsetY = 30
            //});
            //Dictionary<string, object> addInfo7 = new Dictionary<string, object>();
            //addInfo7.Add("tooltip", "Vertical phase");
            //swimlanePalette.Add(new DiagramNode()
            //{
            //    Id = "verticalPhase",
            //    Height = 60,
            //    Width = 140,
            //    AddInfo = addInfo7,
            //    Shape = new SwimLaneModel()
            //    {
            //        Type = "SwimLane",
            //        Orientation = "Horizontal",
            //        IsPhase = true
            //    }
            //});
            Dictionary<string, object> addInfo8 = new Dictionary<string, object>();
            addInfo8.Add("tooltip", "Horizontal phase");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "horizontalPhase",
                Height = 60,
                Width = 140,
                AddInfo = addInfo8,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Orientation = "Vertical",
                    IsPhase = true
                }
            });

            List<SymbolPalettePalette> palettes = new List<SymbolPalettePalette>();
            palettes.Add(new SymbolPalettePalette() { Id = "flow", Expanded = true, Symbols = flowShapes, IconCss = "shapes", Title = "Flow Shapes" });
            //palettes.Add(new SymbolPalettePalette() { Id = "links", Expanded = false, Symbols = linkShapes, IconCss = "shapes", Title = "Ligas" });
            if (clave == "SP")
            {
                palettes.Add(new SymbolPalettePalette() { Id = "Riesgos", Expanded = false, Symbols = Risks, IconCss = "shapes", Title = "Riesgos" });
                palettes.Add(new SymbolPalettePalette() { Id = "Controles", Expanded = false, Symbols = Controls, IconCss = "shapes", Title = "Controles" });

            }


            palettes.Add(new SymbolPalettePalette() { Id = "swimlane", Expanded = true, Symbols = swimlanePalette, IconCss = "shapes", Title = "Swimlane" });
            palettes.Add(new SymbolPalettePalette() { Id = "connectors", Expanded = true, Symbols = paletteConnectors, IconCss = "shapes", Title = "Connectors" });

            ViewBag.Palette = palettes;

            //ViewBag.Spconnectors = paletteConnectors;


            #endregion

            //double[] intervals1 = { 1, 9, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75 };
            //DiagramGridlines grIdLines1 = new DiagramGridlines()
            //{ LineColor = "#e0e0e0", LineIntervals = intervals1 };
            //ViewBag.gridLines1 = grIdLines1;
            //double[] intervals = { 1, 9, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75 };
            double[] intervals = { 5, 1075 };
            DiagramGridlines gridLines = new DiagramGridlines()
            {
                LineColor = "#a0a0a0",
                LineIntervals = intervals,
                LineDashArray = "10"//LineDashArray = "2,2"
            };
            ViewBag.gridLines = gridLines;
            //double[] intervalsv = { 1, 9, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75 };
            double[] intervalsv = { 5, 1415 };
            DiagramGridlines gridLinesv = new DiagramGridlines()
            {
                LineColor = "#a0a0a0",
                LineIntervals = intervalsv,
                LineDashArray = "10"//LineDashArray = "2,2"
            };
            ViewBag.gridLinesv = gridLinesv;


            DiagramMargin margin = new DiagramMargin() { Left = 15, Right = 15, Bottom = 15, Top = 15 };
            ViewBag.margin = margin;

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        [ValidateInput(false)]
        public ActionResult Save(string JsonData, int id, string clave)
        {
            // Construir la ruta base del archivo
            string baseFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/" + clave + "/" + id);
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePathWithTimestamp = $"{baseFilePath}_{timeStamp}";

            // Escribir los nuevos datos en el archivo con el id y la fecha actual
            System.IO.File.WriteAllText(filePathWithTimestamp, JsonData);

            // Redireccionar según la clave
            return RedirectToAction(GetRedirectActionName(clave));
        }

        // Método auxiliar para obtener el nombre de la acción de redireccionamiento según la clave
        private string GetRedirectActionName(string clave)
        {
            switch (clave)
            {
                case "EN":
                    return "Entidad";
                case "MP":
                    return "MacroProceso";
                case "PR":
                    return "Proceso";
                case "SP":
                    return "SubProceso";
                default:
                    return "Index"; // Redireccionar a una acción predeterminada en caso de clave desconocida
            }
        }

        [Access(Funcion = "DiagSP", ModuleCode = "MSICI012")]
        public ActionResult SelectControl(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control k_control = db.k_control.Find(id);
            if (k_control == null)
            {
                return HttpNotFound();
            }

            if (k_control.tiene_accion_correctora)
            {
                //k_control.relacion_control = Utilidades.Utilidades.CCodeGen(k_control.c_sub_proceso);
                k_control.relacion_control = "";
            }

            //datos del indicador relacionado
            if (db.c_indicador.Where(i => i.id_control == id).Count() > 0)
            {
                var c_indicador = db.c_indicador.Where(i => i.id_control == id).First();

                ViewBag.ind_nb_indicador = c_indicador.nb_indicador + " - " + c_indicador.nb_indicador;
                ViewBag.ind_nb_entidad = c_indicador.c_entidad.cl_entidad + " - " + c_indicador.c_entidad.nb_entidad;
                ViewBag.ind_descripcion = c_indicador.ds_indicador;
                ViewBag.ind_descripcion_nume = c_indicador.ds_numerador;
                ViewBag.ind_descripcion_denum = c_indicador.ds_denominador;
                ViewBag.ind_frecuencia = c_indicador.c_frecuencia_indicador.nb_frecuencia_indicador;
                ViewBag.ind_unidad = c_indicador.c_unidad_indicador.nb_unidad_indicador;
                ViewBag.ind_control_asociado = c_indicador.k_control.relacion_control;
                ViewBag.ind_peso = c_indicador.peso;
                ViewBag.ind_u000i = c_indicador.umbral000i;
                ViewBag.ind_u000f = c_indicador.umbral000f;
                ViewBag.ind_u050i = c_indicador.umbral050i;
                ViewBag.ind_u050f = c_indicador.umbral050f;
                ViewBag.ind_u075i = c_indicador.umbral075i;
                ViewBag.ind_u075f = c_indicador.umbral075f;
                ViewBag.ind_u100i = c_indicador.umbral100i;
                ViewBag.ind_u100f = c_indicador.umbral100f;
                ViewBag.ind_area = c_indicador.c_area.nb_area;
                ViewBag.ind_responsable = c_indicador.c_usuario.nb_usuario;
            }
            else
            {
                ViewBag.indicador = "false";
            }


            string prefijo = k_control.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);

            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);
            ViewBag.lu = k_control.id_responsable;

            if (prefijo == "MP")
            {
                AgregarControlViewModel model = new AgregarControlViewModel();

                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

                model.id_control = k_control.id_control;
                model.actividad_control = k_control.actividad_control;
                model.relacion_control = k_control.relacion_control; //codigo de control
                model.evidencia_control = k_control.evidencia_control;
                model.es_control_clave = k_control.es_control_clave;
                model.nb_aplicacion = k_control.nb_aplicacion;
                model.tiene_accion_correctora = k_control.tiene_accion_correctora;
                model.accion_correctora = k_control.accion_correctora;

                //CAMPOS EXTRA
                model = obtenerCamposExtra(model, k_control);

                ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
                ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control", k_control.id_categoria_control);
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura", k_control.id_grado_cobertura);
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia", k_control.id_tipo_evidencia);
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control", k_control.id_tipologia_control);
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_ejecutor);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);
                ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(k_control.c_aseveracion.Select(a => a.id_aseveracion).ToArray());



                ViewBag.MCError = new string[20];


                //Riesgo Residual

                if (k_control.k_riesgo_residual.Count == 0)//Si no tiene riesgo residual
                {
                    ViewBag.RiesgoResidual = "null";
                }
                else
                {
                    ViewBag.RiesgoResidual = k_control.k_riesgo_residual.First();
                }


                return PartialView("DetailViews/ExtendedDetailsCTR", model);
            }
            else
            {
                AgregarControlMGViewModel model = new AgregarControlMGViewModel();

                //CAMPOS EXTRA
                model = obtenerCamposExtra(model, k_control);

                ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
                ViewBag.CamposExtraControl = Utilidades.Utilidades.infoCamposExtra("k_control", 20);

                k_riesgo k_riesgo = new k_riesgo();
                k_riesgo = db.k_riesgo.Find(k_control.k_riesgo.First().id_riesgo);

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.k_riesgo = k_riesgo;
                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso; // c_sub_proceso.id_sub_proceso;

                model.id_control = k_control.id_control;
                model.actividad_control = k_control.actividad_control;
                model.relacion_control = k_control.relacion_control; //codigo de control
                model.evidencia_control = k_control.evidencia_control;
                model.tiene_accion_correctora = k_control.tiene_accion_correctora;
                model.accion_correctora = k_control.accion_correctora;

                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control", k_control.id_frecuencia_control);
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control", k_control.id_naturaleza_control);
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario", k_control.id_responsable);

                ViewBag.MCError = new string[20];

                return PartialView("DetailViews/ExtendedDetailsMGCTR", model);
                //return View("EditMG", model);
            }
            //return PartialView(model);
        }

        [Access(Funcion = "DiagSP", ModuleCode = "MSICI012")]
        public ActionResult SelectRiesgo(int? id, bool MuestraControles = true)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_riesgo k_riesgo = db.k_riesgo.Find(id);
            if (k_riesgo == null)
            {
                return HttpNotFound();
            }

            string prefijo = k_riesgo.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2);

            ViewBag.CamposExtraRiesgo = Utilidades.Utilidades.infoCamposExtra("k_riesgo", 20);
            ViewBag.CamposExtraRV = Utilidades.Utilidades.valCamposExtra("k_riesgo", 20, (int)id);
            ViewBag.MRError = new string[20];
            ViewBag.CamposExtraSubProceso = Utilidades.Utilidades.infoCamposExtra("c_sub_proceso", 20);

            if (prefijo == "MP")
            {
                EditarRiesgoViewModel model = new EditarRiesgoViewModel();

                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso;

                //LLenar y elegir combo de categorías de Riesgo
                var categorias = _repository.ObtieneCategoriasRiesgo().OrderBy(x => x.cl_categoria_riesgo);
                foreach (var categoria in categorias)
                {
                    model.CategoriasRiesgo.Add(new SelectListItem()
                    {
                        Text = categoria.cl_categoria_riesgo + " - " + categoria.nb_categoria_riesgo,
                        Value = categoria.id_categoria_riesgo.ToString()
                    });
                }

                //LLenar y elegir combo de Clases de tipología de Riesgo
                var clases = _repository.ObtieneClasesTipologiaRiesgo().OrderBy(x => x.cl_clase_tipologia_riesgo);
                foreach (var clase in clases)
                {
                    model.ClasesTipologiaRiesgo.Add(new SelectListItem()
                    {
                        Text = clase.cl_clase_tipologia_riesgo + " - " + clase.nb_clase_tipologia_riesgo,
                        Value = clase.id_clase_tipologia_riesgo.ToString(),
                    });
                }

                //LLenar y elegir combo de Tipos de Riesgo
                var TiposRiesgo = db.c_tipo_riesgo.Where(tr => tr.id_categoria_riesgo == k_riesgo.c_tipo_riesgo.id_categoria_riesgo).ToList();
                foreach (var TipoRiesgo in TiposRiesgo)
                {
                    model.TiposRiesgo.Add(new SelectListItem()
                    {
                        Text = TipoRiesgo.cl_tipo_riesgo + " - " + TipoRiesgo.nb_tipo_riesgo,
                        Value = TipoRiesgo.id_tipo_riesgo.ToString()
                    });
                }

                //llenar y elegir combo de Sub Clases de tipología de Riesgo
                var SubClases = db.c_sub_clase_tipologia_riesgo.Where(sctr => sctr.id_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo).ToList();
                foreach (var SC in SubClases)
                {
                    model.SubClasesTipologiaRiesgo.Add(new SelectListItem()
                    {
                        Text = SC.cl_sub_clase_tipologia_riesgo + " - " + SC.nb_sub_clase_tipologia_riesgo,
                        Value = SC.id_sub_clase_tipologia_riesgo.ToString()
                    });
                }

                //Llenar y elegir combo de tipologías de Riesgo
                var TipologiasRiesgo = db.c_tipologia_riesgo.Where(tr => tr.id_sub_clase_tipologia_riesgo == k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo).ToList();
                foreach (var TipologiaRiesgo in TipologiasRiesgo)
                {
                    model.TipologiasRiesgo.Add(new SelectListItem()
                    {
                        Text = TipologiaRiesgo.cl_tipologia_riesgo + " - " + TipologiaRiesgo.nb_tipologia_riesgo,
                        Value = TipologiaRiesgo.id_tipologia_riesgo.ToString()
                    });
                }

                ViewBag.id_magnitud_impacto = new SelectList(db.c_magnitud_impacto.OrderBy(c => c.cl_magnitud_impacto), "id_magnitud_impacto", "nb_magnitud_impacto", k_riesgo.id_magnitud_impacto);
                ViewBag.id_probabilidad_ocurrencia = new SelectList(db.c_probabilidad_ocurrencia.OrderBy(c => c.cl_probabilidad_ocurrencia), "id_probabilidad_ocurrencia", "nb_probabilidad_ocurrencia", k_riesgo.id_probabilidad_ocurrencia);
                ViewBag.id_tipo_impacto = new SelectList(db.c_tipo_impacto, "id_tipo_impacto", "nb_tipo_impacto", k_riesgo.id_tipo_impacto);

                ViewBag.id_categoria_control = new SelectList(db.c_categoria_control, "id_categoria_control", "nb_categoria_control");
                ViewBag.id_frecuencia_control = new SelectList(db.c_frecuencia_control, "id_frecuencia_control", "nb_frecuencia_control");
                ViewBag.id_grado_cobertura = new SelectList(db.c_grado_cobertura, "id_grado_cobertura", "nb_grado_cobertura");
                ViewBag.id_naturaleza_control = new SelectList(db.c_naturaleza_control, "id_naturaleza_control", "nb_naturaleza_control");
                ViewBag.id_sub_proceso = new SelectList(db.c_sub_proceso, "id_sub_proceso", "nb_sub_proceso");
                ViewBag.id_tipo_evidencia = new SelectList(db.c_tipo_evidencia, "id_tipo_evidencia", "nb_tipo_evidencia");
                ViewBag.id_tipologia_control = new SelectList(db.c_tipologia_control, "id_tipologia_control", "nb_tipologia_control");
                ViewBag.id_ejecutor = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario");
                ViewBag.id_responsable = new SelectList(db.c_usuario.Where(u => u.esta_activo).OrderBy(x => x.nb_usuario), "id_usuario", "nb_usuario");

                //Llenamos la informacion de los combos
                model.id_categoria_riesgo = (int)k_riesgo.c_tipo_riesgo.id_categoria_riesgo;
                model.id_tipo_riesgo = (int)k_riesgo.id_tipo_riesgo;
                model.id_clase_tipologia_riesgo = (int)k_riesgo.c_tipologia_riesgo.c_sub_clase_tipologia_riesgo.id_clase_tipologia_riesgo;
                model.id_sub_clase_tipologia_riesgo = (int)k_riesgo.c_tipologia_riesgo.id_sub_clase_tipologia_riesgo;
                model.id_tipologia_riesgo = (int)k_riesgo.id_tipologia_riesgo;

                model.id_riesgo = k_riesgo.id_riesgo;
                model.cl_riesgo = k_riesgo.cl_riesgo;
                model.nb_riesgo = k_riesgo.nb_riesgo;
                model.evento = k_riesgo.evento;
                model.criticidad = k_riesgo.criticidad;
                model.tiene_afectacion_contable = k_riesgo.tiene_afectacion_contable;
                model.supuesto_normativo = k_riesgo.supuesto_normativo;
                model.euc = k_riesgo.euc;

                string sql;
                sql =
                    "select C.relacion_control as codigo_control, C.id_control, C.actividad_control, C.tiene_accion_correctora, C.accion_correctora, E.nb_usuario nb_ejecutor, R.nb_usuario nb_responsable" +
                    "  from k_control C" +
                    "  left outer join c_usuario E on C.id_ejecutor = E.id_usuario" +
                    "  left outer join c_usuario R on C.id_responsable = R.id_usuario" +
                    " where C.id_control in (select id_control from k_control_riesgo where id_riesgo = " + id.ToString() + ")";
                var controles = db.Database.SqlQuery<ListaControlesViewModel>(sql).ToList();
                ViewBag.controles = controles;
                if (MuestraControles)
                {
                    ViewBag.MControles = true;
                }
                else
                {
                    ViewBag.MControles = false;
                }

                //Enviar todos los datos de la tabla de Criticidad
                ViewBag.Criticidad = db.c_criticidad.ToList();

                //lenar datos de campos extra
                model = obtenerCamposExtra(model, k_riesgo);


                return PartialView("DetailViews/ExtendedDetailsRSG", model);
            }
            else
            {
                EditarRiesgoMGViewModel model = new EditarRiesgoMGViewModel();
                c_sub_proceso c_sub_proceso = new c_sub_proceso();
                c_sub_proceso = db.c_sub_proceso.Find(k_riesgo.id_sub_proceso);

                model.c_sub_proceso = c_sub_proceso;
                model.id_sub_proceso = c_sub_proceso.id_sub_proceso;

                model.id_riesgo = k_riesgo.id_riesgo;
                model.cl_riesgo = k_riesgo.cl_riesgo;
                model.nb_riesgo = k_riesgo.nb_riesgo;
                model.evento = k_riesgo.evento;

                var controls = k_riesgo.k_control.ToList();

                List<ListaControlesViewModel> controles = new List<ListaControlesViewModel>();

                foreach (k_control control in controls)
                {
                    ListaControlesViewModel aux = new ListaControlesViewModel();
                    aux.id_control = control.id_control;
                    aux.codigo_control = control.relacion_control;
                    aux.nb_ejecutor = "N/A";
                    aux.nb_responsable = control.c_usuario1.nb_usuario;
                    aux.actividad_control = control.actividad_control;
                    controles.Add(aux);
                }

                ViewBag.controles = controles;
                if (MuestraControles)
                {
                    ViewBag.MControles = true;
                }
                else
                {
                    ViewBag.MControles = false;
                }

                //lenar datos de campos extra
                model = obtenerCamposExtra(model, k_riesgo);

                return PartialView("DetailViews/ExtendedDetailsMGRSG", model);
                //return View("EditarMG", model);
            }

            //return PartialView(model);
        }

        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        public string Exist(int idReg, int idSP, string type)
        {
            var sp = db.c_sub_proceso.Find(idSP);


            //Se retornará una cadena con el formato "true,Codigo" en donde se enviará información de si el registro aun existe 
            //junto con el nombre que este tiene
            bool exist = false;
            string code = "N/A";

            if (type == "Control")
            {
                exist = sp.k_control.Any(c => c.id_control == idReg);
                if (exist) //Aun existe el control?
                {
                    code = sp.k_control.First(c => c.id_control == idReg).relacion_control;
                }
            }
            if (type == "Riesgo")
            {
                exist = sp.k_riesgo.Any(c => c.id_riesgo == idReg);
                if (exist) //Aun existe el control?
                {
                    code = sp.k_riesgo.First(c => c.id_riesgo == idReg).nb_riesgo;
                }
            }

            return (exist ? "true," : "false,") + code;
        }


        #region Historial
        [Access(Funcion = "DiagEN", ModuleCode = "MSICI012")]
        public ActionResult HistorialEN(int id)
        {
            var r = db.c_entidad.Find(id);
            return RedirectToAction("Historial", new { id = id, clave = "EN", nbRegistro = "Entidad " + r.cl_entidad + " - " + r.nb_entidad });
        }
		
		[Access(Funcion = "DiagMP", ModuleCode = "MSICI012")]
        public ActionResult HistorialMP(int id)
        {
            var r = db.c_macro_proceso.Find(id);
            return RedirectToAction("Historial", new { id = id, clave = "MP", nbRegistro = "Macro Proceso " + r.cl_macro_proceso + " - " + r.nb_macro_proceso});
        }
		
		[Access(Funcion = "DiagPR", ModuleCode = "MSICI012")]
        public ActionResult HistorialPR(int id)
        {
            var r = db.c_proceso.Find(id);
            return RedirectToAction("Historial", new { id = id, clave = "PR", nbRegistro = "Proceso " + r.cl_proceso + " - " + r.nb_proceso });
        }
		
		[Access(Funcion = "DiagSP", ModuleCode = "MSICI012")]
        public ActionResult HistorialSP(int id)
        {
            var r = db.c_sub_proceso.Find(id);
            return RedirectToAction("Historial", new { id, clave = "SP", nbRegistro = "Sub Proceso " + r.cl_sub_proceso + " - " + r.nb_sub_proceso});
        }

        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        public ActionResult Historial(int id, string clave,string nbRegistro)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/" + clave + "/");
            // Verificar si existen archivos con el formato "id_fechaHora"
            string[] historyFiles = Directory.GetFiles(path, id + "_*");

            // Obtener la lista de archivos con sus fechas
            List<Tuple<string, DateTime>> filesWithDates = new List<Tuple<string, DateTime>>();
            foreach (var filePath in historyFiles)
            {
                DateTime fileDate = GetDateTimeFromFileName(filePath);
                filesWithDates.Add(new Tuple<string, DateTime>(Path.GetFileNameWithoutExtension(filePath), fileDate));
            }


            // Pasar la lista de archivos de historial a la vista para visualización
            ViewBag.filesWithDates = filesWithDates;
            ViewBag.nbRegistro = nbRegistro;
            ViewBag.clave = clave;

            return View();
        }
            #endregion


            #region Borrar
            [HttpPost, NotOnlyRead]
        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        [ValidateInput(false)]
        public string Delete(int id, string clave)
        {
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/" + clave + "/" + id);
                System.IO.File.Delete(path);
                return "ok";
            }
            catch
            {
                return "error";
            }
        }
        #endregion


        #region Auxiliares
        // Método auxiliar para obtener un DateTime desde el nombre del archivo
        private DateTime GetDateTimeFromFileName(string fileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string[] parts = fileNameWithoutExtension.Split('_');
            string dateTimePart = $"{parts[1]}_{parts[2]}"; // La parte más reciente en el nombre del archivo
            return DateTime.ParseExact(dateTimePart, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        }

        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        public string JsonData(string clave,int id,string cl_fecha = null)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Diagramas/" + clave + "/");
            var JsonData = "";

            if (cl_fecha != null && cl_fecha != "" )
            {
                //Si este atributo es diferente a null, significa que venimos desde la pantalla de historial
                var file = Directory.GetFiles(path, id + "_" + cl_fecha).First();
                JsonData = System.IO.File.ReadAllText(file);

            }
            else
            {
                // Verificar si existen archivos con el formato "id_fechaHora"
                string[] files = Directory.GetFiles(path, id + "_*");
                if (files.Length > 0)
                {
                    // Obtener el archivo más reciente según la fecha en el nombre del archivo
                    var latestFile = files
                        .OrderByDescending(file => GetDateTimeFromFileName(file))
                        .FirstOrDefault();

                    if (latestFile != null)
                    {
                        // Leer el contenido del archivo más reciente
                        JsonData = System.IO.File.ReadAllText(latestFile);
                    }
                }
            }

            return JsonData;
        }


        [Access(Funcion = "UsuarioNR", ModuleCode = "MSICI000")]
        public string ImHere()
        {
            return "SEE U";
        }
        AgregarControlViewModel obtenerCamposExtra(AgregarControlViewModel control, k_control model)
        {
            control.campo01 = model.campo01; control.campo02 = model.campo02;
            control.campo03 = model.campo03; control.campo04 = model.campo04;
            control.campo05 = model.campo05; control.campo06 = model.campo06;
            control.campo07 = model.campo07; control.campo08 = model.campo08;
            control.campo09 = model.campo09; control.campo10 = model.campo10;
            control.campo11 = model.campo11; control.campo12 = model.campo12;
            control.campo13 = model.campo13; control.campo14 = model.campo14;
            control.campo15 = model.campo15; control.campo16 = model.campo16;
            control.campo17 = model.campo17; control.campo18 = model.campo18;
            control.campo19 = model.campo19; control.campo20 = model.campo20;

            return control;
        }

        AgregarControlMGViewModel obtenerCamposExtra(AgregarControlMGViewModel control, k_control model)
        {
            control.campo01 = model.campo01; control.campo02 = model.campo02;
            control.campo03 = model.campo03; control.campo04 = model.campo04;
            control.campo05 = model.campo05; control.campo06 = model.campo06;
            control.campo07 = model.campo07; control.campo08 = model.campo08;
            control.campo09 = model.campo09; control.campo10 = model.campo10;
            control.campo11 = model.campo11; control.campo12 = model.campo12;
            control.campo13 = model.campo13; control.campo14 = model.campo14;
            control.campo15 = model.campo15; control.campo16 = model.campo16;
            control.campo17 = model.campo17; control.campo18 = model.campo18;
            control.campo19 = model.campo19; control.campo20 = model.campo20;

            return control;
        }

        EditarRiesgoViewModel obtenerCamposExtra(EditarRiesgoViewModel model, k_riesgo riesgo)
        {
            model.campor01 = riesgo.campo01; model.campor02 = riesgo.campo02;
            model.campor03 = riesgo.campo03; model.campor04 = riesgo.campo04;
            model.campor05 = riesgo.campo05; model.campor06 = riesgo.campo06;
            model.campor07 = riesgo.campo07; model.campor08 = riesgo.campo08;
            model.campor09 = riesgo.campo09; model.campor10 = riesgo.campo10;
            model.campor11 = riesgo.campo11; model.campor12 = riesgo.campo12;
            model.campor13 = riesgo.campo13; model.campor14 = riesgo.campo14;
            model.campor15 = riesgo.campo15; model.campor16 = riesgo.campo16;
            model.campor17 = riesgo.campo17; model.campor18 = riesgo.campo18;
            model.campor19 = riesgo.campo19; model.campor20 = riesgo.campo20;

            return model;
        }

        EditarRiesgoMGViewModel obtenerCamposExtra(EditarRiesgoMGViewModel model, k_riesgo riesgo)
        {
            model.campor01 = riesgo.campo01; model.campor02 = riesgo.campo02;
            model.campor03 = riesgo.campo03; model.campor04 = riesgo.campo04;
            model.campor05 = riesgo.campo05; model.campor06 = riesgo.campo06;
            model.campor07 = riesgo.campo07; model.campor08 = riesgo.campo08;
            model.campor09 = riesgo.campo09; model.campor10 = riesgo.campo10;
            model.campor11 = riesgo.campo11; model.campor12 = riesgo.campo12;
            model.campor13 = riesgo.campo13; model.campor14 = riesgo.campo14;
            model.campor15 = riesgo.campo15; model.campor16 = riesgo.campo16;
            model.campor17 = riesgo.campo17; model.campor18 = riesgo.campo18;
            model.campor19 = riesgo.campo19; model.campor20 = riesgo.campo20;

            return model;
        }

        #endregion
    }

    #region Clases Públicas
    public class SwimLaneModel
    {
        [DefaultValue(null)]
        [HtmlAttributeName("type")]
        [JsonProperty("type")]
        public string Type { get; set; }

        [DefaultValue(null)]
        [HtmlAttributeName("lanes")]
        [JsonProperty("lanes")]
        public List<Lane> Lanes { get; set; }

        [DefaultValue(null)]
        [HtmlAttributeName("orientation")]
        [JsonProperty("orientation")]
        public string Orientation { get; set; }

        [DefaultValue(null)]
        [HtmlAttributeName("isLane")]
        [JsonProperty("isLane")]
        public bool IsLane { get; set; }

        [DefaultValue(null)]
        [HtmlAttributeName("isPhase")]
        [JsonProperty("isPhase")]
        public bool IsPhase { get; set; }
    }

    public class SwimLane
    {
        [DefaultValue(null)]
        [HtmlAttributeName("type")]
        [JsonProperty("type")]
        public string Type
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("header")]
        [JsonProperty("header")]
        public Header Header
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("lanes")]
        [JsonProperty("lanes")]
        public List<Lane> Lanes
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("phases")]
        [JsonProperty("phases")]
        public List<Phase> Phases
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("phaseSize")]
        [JsonProperty("phaseSize")]
        public double PhaseSize
        {
            get;
            set;
        }
    }

    public class Header
    {
        [DefaultValue(null)]
        [HtmlAttributeName("annotation")]
        [JsonProperty("annotation")]
        public object Annotation
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("height")]
        [JsonProperty("height")]
        public double Height
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("width")]
        [JsonProperty("width")]
        public double Width
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("orientation")]
        [JsonProperty("orientation")]
        public string Orientation
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("style")]
        [JsonProperty("style")]
        public DiagramTextStyle Style
        {
            get;
            set;
        }
    }

    public class Lane
    {
        [DefaultValue(null)]
        [HtmlAttributeName("id")]
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("header")]
        [JsonProperty("header")]
        public Header Header
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("style")]
        [JsonProperty("style")]
        public DiagramTextStyle Style
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("height")]
        [JsonProperty("height")]
        public double Height
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("width")]
        [JsonProperty("width")]
        public double Width
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("children")]
        [JsonProperty("children")]
        public List<DiagramNode> Children
        {
            get;
            set;
        }
    }

    public class Phase
    {
        [DefaultValue(null)]
        [HtmlAttributeName("id")]
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("offset")]
        [JsonProperty("offset")]
        public double Offset
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("style")]
        [JsonProperty("style")]
        public DiagramTextStyle Style
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("header")]
        [JsonProperty("header")]
        public Header Header
        {
            get;
            set;
        }
    }

    public class MenuItems
    {
        [DefaultValue(null)]
        [HtmlAttributeName("text")]
        [JsonProperty("text")]
        public string Text
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("id")]
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }
        [DefaultValue(null)]
        [HtmlAttributeName("target")]
        [JsonProperty("target")]
        public string Target
        {
            get;
            set;
        }
    }
    #endregion
}