using SCIRA.Models;
using SCIRA.Seguridad;
using Syncfusion.EJ2.Diagrams;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    public class DiagramaTestController : Controller
    {
        private SICIEntities db = new SICIEntities();
        //string darkColor = "#C7D4DF";
        //string lightColor = "#f5f5f5";
        string pathData = "M 120 24.9999 C 120 38.8072 109.642 50 96.8653 50 L 23.135 50 C 10.3578 50 0 38.8072 0 24.9999 L 0 24.9999 C 0 11.1928 10.3578 0 23.135 0 L 96.8653 0 C 109.642 0 120 11.1928 120 24.9999 Z";

        public ActionResult Entidad()
        {
            List<c_entidad> entidades;

            var user = (IdentityPersonalizado)User.Identity;
            var Usuario = db.c_usuario.Find(user.Id_usuario);

            var su = user.Es_super_usuario;
            ViewBag.su = user.Es_super_usuario;

            entidades = Utilidades.Utilidades.RTCObject(Usuario, db, "c_entidad").Cast<c_entidad>().OrderBy(x => x.cl_entidad).ToList();
            return View(entidades);
        }

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

            return View(mps);
        }

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
            return View(ps);
        }

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

        public ActionResult Diagrama()
        {
            List<DiagramNode> Nodes = new List<DiagramNode>();
            GenerateDiagramNodes(Nodes);

            List<DiagramConnector> Connectors = new List<DiagramConnector>();
            GenerateDiagramConnectors(Connectors);

            ViewBag.nodes = Nodes;
            ViewBag.connectors = Connectors;
            ViewBag.getNodeDefaults = "getNodeDefaults";
            ViewBag.getConnectorDefaults = "getConnectorDefaults";
            List<DiagramPort> ports = new List<DiagramPort>();
            ports.Add(new DiagramPort() { Id = "Port1", Offset = new DiagramPoint() { X = 0, Y = 0.5 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port2", Offset = new DiagramPoint() { X = 0.5, Y = 0 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port3", Offset = new DiagramPoint() { X = 1, Y = 0.5 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port4", Offset = new DiagramPoint() { X = 0.5, Y = 1 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });


            List<DiagramNode> SymbolPaletee = new List<DiagramNode>();
            Dictionary<string, object> addInfo = new Dictionary<string, object>();
            addInfo.Add("tooltip", "Terminator");
            SymbolPaletee.Add(new DiagramNode() { Id = "Terminator", AddInfo = addInfo, Ports = ports, Width = 50, Height = 50, Shape = new { type = "Flow", shape = "Terminator" } });
            Dictionary<string, object> addInfo1 = new Dictionary<string, object>();
            addInfo1.Add("tooltip", "Process");
            SymbolPaletee.Add(new DiagramNode() { Id = "Process", AddInfo = addInfo1, Ports = ports, Width = 50, Height = 50, Shape = new { type = "Flow", shape = "Process" } });
            Dictionary<string, object> addInfo2 = new Dictionary<string, object>();
            addInfo2.Add("tooltip", "Document");
            SymbolPaletee.Add(new DiagramNode() { Id = "Document", AddInfo = addInfo2, Ports = ports, Width = 50, Height = 50, Shape = new { type = "Flow", shape = "Document" } });
            Dictionary<string, object> addInfo3 = new Dictionary<string, object>();
            addInfo3.Add("tooltip", "Predefined process");
            SymbolPaletee.Add(new DiagramNode() { Id = "PreDefinedProcess", AddInfo = addInfo3, Ports = ports, Width = 50, Height = 50, Shape = new { type = "Flow", shape = "PreDefinedProcess" } });
            Dictionary<string, object> addInfo4 = new Dictionary<string, object>();
            addInfo4.Add("tooltip", "Data");
            SymbolPaletee.Add(new DiagramNode() { Id = "data", AddInfo = addInfo4, Ports = ports, Width = 50, Height = 50, Shape = new { type = "Flow", shape = "Data" } });
            List<Lane> lanes = new List<Lane>();
            Lane lane1 = new Lane();
            lane1.Id = "lane1";
            lane1.Height = 60;
            lane1.Width = 150;
            lane1.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes.Add(lane1);

            List<Lane> lanes1 = new List<Lane>();
            Lane lane2 = new Lane();
            lane2.Id = "lane2";
            lane2.Height = 150;
            lane2.Width = 60;
            lane2.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes1.Add(lane2);


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
            Dictionary<string, object> addInfo6 = new Dictionary<string, object>();
            addInfo6.Add("tooltip", "Vertical swimlane");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "stackCanvas2",
                Height = 140,
                Width = 60,
                AddInfo = addInfo6,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Lanes = lanes1,
                    Orientation = "Vertical",
                    IsLane = true
                },
                OffsetX = 70,
                OffsetY = 30
            });
            Dictionary<string, object> addInfo7 = new Dictionary<string, object>();
            addInfo7.Add("tooltip", "Vertical phase");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "verticalPhase",
                Height = 60,
                Width = 140,
                AddInfo = addInfo7,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Orientation = "Horizontal",
                    IsPhase = true
                }
            });
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

            List<DiagramConnector> SymbolPaletteConnectors = new List<DiagramConnector>();
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link1",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 27, Y = 27 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link2",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 27, Y = 27 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeDashArray = "4,4", StrokeColor = "#757575" }
            });

            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link4",
                Type = Segments.Straight,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });

            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link5",
                Type = Segments.Straight,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeDashArray = "4,4", StrokeColor = "#757575" }
            });

            List<SymbolPalettePalette> Palette = new List<SymbolPalettePalette>
            {
                new SymbolPalettePalette() { Id = "basic", Expanded = true, Symbols = SymbolPaletee, IconCss = "e-ddb-icons e-flow", Title = "Basic Shapes" },
                new SymbolPalettePalette() { Id = "swimlane", Expanded = true, Symbols = swimlanePalette, IconCss = "e-ddb-icons e-basic", Title = "Swimlane" },
                new SymbolPalettePalette() { Id = "connectors", Expanded = true, Symbols = SymbolPaletteConnectors, IconCss = "e-ddb-icons e-connector", Title = "Connectors" }
            };

            List<MenuItems> items = new List<MenuItems>();
            items.Add(new MenuItems() { Id = "Clone", Text = "Clone", Target = ".e-diagramcontent" });
            items.Add(new MenuItems() { Id = "Cut", Text = "Cut", Target = ".e-diagramcontent" });
            items.Add(new MenuItems() { Id = "InsertLaneBefore", Text = "InsertLaneBefore", Target = ".e-diagramcontent" });
            items.Add(new MenuItems() { Id = "InsertLaneAfter", Text = "InsertLaneAfter", Target = ".e-diagramcontent" });
            ViewBag.items = items;

            SymbolPaletteMargin margin = new SymbolPaletteMargin() { Left = 8, Right = 8, Top = 8, Bottom = 8 };

            ViewBag.margin = margin;
            ViewBag.Palette = Palette;

            return View();
        }

        public ActionResult Test(int id = 1)
        {
            ViewBag.type = id;

            if (id == 1)
            {
                List<DiagramNode> nodes = new List<DiagramNode>();
                List<DiagramNodeAnnotation> Node1 = new List<DiagramNodeAnnotation>();
                Node1.Add(new DiagramNodeAnnotation() { Content = "node1", Style = new DiagramTextStyle() { Color = "White", StrokeColor = "None" } });
                nodes.Add(new DiagramNode()
                {
                    Id = "node1",
                    Width = 100,
                    Height = 100,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    AddInfo = "node1",
                    OffsetX = 100,
                    OffsetY = 100,
                    Annotations = Node1
                });
                ViewBag.nodes = nodes;


                return View();
            }
            else if (id == 2)
            {
                List<DiagramNode> Nodes = new List<DiagramNode>();
                List<DiagramNodeAnnotation> Node1 = new List<DiagramNodeAnnotation>();
                Node1.Add(new DiagramNodeAnnotation() { Content = "node1" });
                List<DiagramNodeAnnotation> Node2 = new List<DiagramNodeAnnotation>();
                Node2.Add(new DiagramNodeAnnotation() { Content = "node2" });
                List<DiagramNodeAnnotation> Node3 = new List<DiagramNodeAnnotation>();
                Nodes.Add(new DiagramNode()
                {
                    Id = "node1",
                    Annotations = Node1,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetX = 100,
                    OffsetY = 100,
                    Width = 100,
                    Height = 100

                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node2",
                    Annotations = Node2,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetX = 300,
                    OffsetY = 100,
                    Width = 100,
                    Height = 100
                });
                List<DiagramConnector> Connectors = new List<DiagramConnector>();
                Connectors.Add(new DiagramConnector() { Id = "connector", SourceID = "node1", TargetID = "node2", });
                ViewBag.nodes = Nodes;
                ViewBag.connectors = Connectors;
                return View();
            }
            else if (id == 3)
            {
                List<DiagramNode> Nodes = new List<DiagramNode>();
                List<DiagramNodeAnnotation> Node1 = new List<DiagramNodeAnnotation>();
                Node1.Add(new DiagramNodeAnnotation() { Content = "node1" });
                List<DiagramNodeAnnotation> Node2 = new List<DiagramNodeAnnotation>();
                Node2.Add(new DiagramNodeAnnotation() { Content = "node2" });
                List<DiagramNodeAnnotation> Node3 = new List<DiagramNodeAnnotation>();
                Nodes.Add(new DiagramNode()
                {
                    Id = "node1",
                    Annotations = Node1,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetY = 100,
                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node2",
                    Annotations = Node2,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetY = 300,
                });
                List<DiagramConnector> Connectors = new List<DiagramConnector>();
                Connectors.Add(new DiagramConnector() { Id = "connector", SourceID = "node1", TargetID = "node2", });
                ViewBag.nodes = Nodes;
                ViewBag.connectors = Connectors;
                return View();
            }
            else if (id == 4)
            {
                List<DiagramNode> Nodes = new List<DiagramNode>();
                List<DiagramNodeAnnotation> Node1 = new List<DiagramNodeAnnotation>();
                Node1.Add(new DiagramNodeAnnotation() { Content = "node1" });
                List<DiagramNodeAnnotation> Node2 = new List<DiagramNodeAnnotation>();
                Node2.Add(new DiagramNodeAnnotation() { Content = "node2" });
                List<DiagramNodeAnnotation> Node3 = new List<DiagramNodeAnnotation>();
                Node3.Add(new DiagramNodeAnnotation() { Content = "i < 10?" });
                List<DiagramNodeAnnotation> Node4 = new List<DiagramNodeAnnotation>();
                Node4.Add(new DiagramNodeAnnotation() { Content = "print(hello!!)", Style = new DiagramTextStyle() { Fill = "White" } });
                List<DiagramNodeAnnotation> Node5 = new List<DiagramNodeAnnotation>();
                Node5.Add(new DiagramNodeAnnotation() { Content = "i++;" });
                List<DiagramNodeAnnotation> Node6 = new List<DiagramNodeAnnotation>();
                Node6.Add(new DiagramNodeAnnotation() { Content = "End" });
                List<DiagramConnectorAnnotation> connector1 = new List<DiagramConnectorAnnotation>();
                connector1.Add(new DiagramConnectorAnnotation() { Content = "Yes" });
                List<DiagramConnectorAnnotation> connector2 = new List<DiagramConnectorAnnotation>();
                connector2.Add(new DiagramConnectorAnnotation() { Content = "No" });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node1",
                    Annotations = Node1,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    Shape = new { type = "Flow", shape = "Terminator" },
                    OffsetY = 50,
                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node2",
                    Annotations = Node2,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetY = 140,
                    Shape = new { type = "Flow", shape = "Process" },
                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node3",
                    Annotations = Node3,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetY = 230,
                    Shape = new { type = "Flow", shape = "Decision" },
                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node4",
                    Annotations = Node4,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetY = 320,
                    Shape = new { type = "Flow", shape = "PreDefinedProcess" },
                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node5",
                    Annotations = Node5,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },
                    OffsetY = 410,
                    Shape = new { type = "Flow", shape = "Process" },
                });
                Nodes.Add(new DiagramNode()
                {
                    Id = "node6",
                    Annotations = Node6,
                    Style = new NodeStyleNodes() { Fill = "darkcyan" },

                    OffsetY = 500,
                    Shape = new { type = "Flow", shape = "Terminator" },
                });
                List<DiagramConnector> Connectors = new List<DiagramConnector>();
                Connectors.Add(new DiagramConnector() { Id = "connector1", SourceID = "node1", TargetID = "node2", });
                Connectors.Add(new DiagramConnector() { Id = "connector2", SourceID = "node2", TargetID = "node3", });
                Connectors.Add(new DiagramConnector() { Id = "connector3", SourceID = "node3", TargetID = "node4", Annotations = connector1, });
                Connectors.Add(new DiagramConnector() { Id = "connector4", SourceID = "node3", TargetID = "node6", Annotations = connector2, });
                Connectors.Add(new DiagramConnector() { Id = "connector5", SourceID = "node4", TargetID = "node5" });
                Connectors.Add(new DiagramConnector() { Id = "connector6", SourceID = "node5", TargetID = "node3" });
                ViewBag.nodes = Nodes;
                ViewBag.connectors = Connectors;
                return View();
            }


            return View();
        }


        public ActionResult DefaultFunctionalities()
        {
            List<DiagramNode> nodes = new List<DiagramNode>();
            List<DiagramNodeAnnotation> Node1 = new List<DiagramNodeAnnotation>();
            Node1.Add(new DiagramNodeAnnotation() { Content = "Place Order", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node2 = new List<DiagramNodeAnnotation>();
            Node2.Add(new DiagramNodeAnnotation() { Content = "Start Transaction", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node3 = new List<DiagramNodeAnnotation>();
            Node3.Add(new DiagramNodeAnnotation() { Content = "Verification", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node4 = new List<DiagramNodeAnnotation>();
            Node4.Add(new DiagramNodeAnnotation() { Content = "Credit Card ValId?", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node5 = new List<DiagramNodeAnnotation>();
            Node5.Add(new DiagramNodeAnnotation() { Content = "Funds Available", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node6 = new List<DiagramNodeAnnotation>();
            Node6.Add(new DiagramNodeAnnotation() { Content = "Enter Payment Method", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node7 = new List<DiagramNodeAnnotation>();
            Node7.Add(new DiagramNodeAnnotation() { Content = "Log Transaction", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node8 = new List<DiagramNodeAnnotation>();
            Node8.Add(new DiagramNodeAnnotation() { Content = "Reconcile the entries", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node9 = new List<DiagramNodeAnnotation>();
            Node9.Add(new DiagramNodeAnnotation() { Content = "Complete Transaction", Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node10 = new List<DiagramNodeAnnotation>();
            Node10.Add(new DiagramNodeAnnotation() { Content = "Send E-mail", Margin = new DiagramMargin() { Left = 25, Right = 25 }, Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramNodeAnnotation> Node11 = new List<DiagramNodeAnnotation>();
            Node11.Add(new DiagramNodeAnnotation() { Content = "Customer Database", Margin = new DiagramMargin() { Left = 25, Right = 25 }, Style = new DiagramTextStyle() { Color = "white", Fill = "transparent" } });

            List<DiagramConnectorAnnotation> Connector1 = new List<DiagramConnectorAnnotation>();
            Connector1.Add(new DiagramConnectorAnnotation() { Content = "Yes", Style = new DiagramTextStyle() { Fill = "White" } });

            List<DiagramConnectorAnnotation> Connector2 = new List<DiagramConnectorAnnotation>();
            Connector2.Add(new DiagramConnectorAnnotation() { Content = "Yes", Style = new DiagramTextStyle() { Fill = "White" } });

            List<DiagramConnectorAnnotation> Connector3 = new List<DiagramConnectorAnnotation>();
            Connector3.Add(new DiagramConnectorAnnotation() { Content = "No", Style = new DiagramTextStyle() { Fill = "White" } });

            nodes.Add(new DiagramNode()
            {
                Id = "NewIdea",
                OffsetY = 80,
                OffsetX = 340,
                Height = 60,
                Annotations = Node1,
                Shape = new { type = "Flow", shape = "Terminator" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Meeting",
                OffsetX = 340,
                OffsetY = 160,
                Height = 60,
                Annotations = Node2,
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "BoardDecision",
                OffsetX = 340,
                OffsetY = 240,
                Height = 60,
                Annotations = Node3,
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Project",
                OffsetX = 340,
                OffsetY = 330,
                Height = 60,
                Annotations = Node4,
                Shape = new { type = "Flow", shape = "Decision" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "End",
                OffsetY = 430,
                OffsetX = 340,
                Height = 60,
                Annotations = Node5,
                Shape = new { type = "Flow", shape = "Decision" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "node11",
                OffsetY = 330,
                OffsetX = 550,
                Height = 60,
                Annotations = Node6,
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "transaction_entered",
                OffsetY = 630,
                OffsetX = 340,
                Height = 60,
                Annotations = Node7,
                Shape = new { type = "Flow", shape = "Terminator" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "node12",
                OffsetY = 630,
                OffsetX = 550,
                Height = 60,
                Annotations = Node8,
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "transaction_completed",
                OffsetY = 530,
                OffsetX = 340,
                Height = 60,
                Annotations = Node9,
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Data",
                OffsetY = 530,
                OffsetX = 120,
                Height = 60,
                Annotations = Node10,
                Shape = new { type = "Flow", shape = "Data" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "node10",
                OffsetY = 530,
                OffsetX = 550,
                Height = 60,
                Annotations = Node11,
                Shape = new { type = "Flow", shape = "DirectData" }
            });

            List<DiagramConnector> Connectors = new List<DiagramConnector>();
            Connectors.Add(new DiagramConnector() { Id = "connector1", SourceID = "NewIdea", TargetID = "Meeting", });
            Connectors.Add(new DiagramConnector() { Id = "connector2", SourceID = "Meeting", TargetID = "BoardDecision" });
            Connectors.Add(new DiagramConnector() { Id = "connector3", SourceID = "BoardDecision", TargetID = "Project" });
            Connectors.Add(new DiagramConnector()
            {
                Id = "connector4",
                SourceID = "Project",
                TargetID = "End",
                Annotations = Connector1
            });
            Connectors.Add(new DiagramConnector()
            {
                Id = "connector5",
                SourceID = "End",
                TargetID = "transaction_completed",
                Annotations = Connector2
            });
            Connectors.Add(new DiagramConnector() { Id = "connector6", SourceID = "transaction_completed", TargetID = "transaction_entered" });
            Connectors.Add(new DiagramConnector() { Id = "connector7", SourceID = "transaction_completed", TargetID = "Data" });
            Connectors.Add(new DiagramConnector() { Id = "connector8", SourceID = "transaction_completed", TargetID = "node10" });
            Connectors.Add(new DiagramConnector() { Id = "connector9", SourceID = "node11", TargetID = "Meeting" });
            Connectors.Add(new DiagramConnector() { Id = "connector10", SourceID = "End", TargetID = "node11" });
            Connectors.Add(new DiagramConnector()
            {
                Id = "connector11",
                SourceID = "Project",
                TargetID = "node11",
                Annotations = Connector3
            });
            Connectors.Add(new DiagramConnector()
            {
                Id = "connector12",
                SourceID = "transaction_entered",
                TargetID = "node12",
                Style = new DiagramStrokeStyle() { StrokeDashArray = "2,2" }
            });
            ViewBag.nodes = nodes;
            ViewBag.connectors = Connectors;

            List<Syncfusion.EJ2.Diagrams.DiagramNode> flowShapes = new List<Syncfusion.EJ2.Diagrams.DiagramNode>();
            flowShapes.Add(new DiagramNode() { Id = "Terminator", Shape = new { type = "Flow", shape = "Terminator" } });
            flowShapes.Add(new DiagramNode() { Id = "Process", Shape = new { type = "Flow", shape = "Process" } });
            flowShapes.Add(new DiagramNode() { Id = "Decision", Shape = new { type = "Flow", shape = "Decision" } });
            flowShapes.Add(new DiagramNode() { Id = "Document", Shape = new { type = "Flow", shape = "Document" } });
            flowShapes.Add(new DiagramNode() { Id = "PreDefinedProcess", Shape = new { type = "Flow", shape = "PreDefinedProcess" } });
            flowShapes.Add(new DiagramNode() { Id = "PaperTap", Shape = new { type = "Flow", shape = "PaperTap" } });
            flowShapes.Add(new DiagramNode() { Id = "DirectData", Shape = new { type = "Flow", shape = "DirectData" } });
            flowShapes.Add(new DiagramNode() { Id = "SequentialData", Shape = new { type = "Flow", shape = "SequentialData" } });
            flowShapes.Add(new DiagramNode() { Id = "Sort", Shape = new { type = "Flow", shape = "Sort" } });
            flowShapes.Add(new DiagramNode() { Id = "MultiDocument", Shape = new { type = "Flow", shape = "MultiDocument" } });
            flowShapes.Add(new DiagramNode() { Id = "Collate", Shape = new { type = "Flow", shape = "Collate" } });
            flowShapes.Add(new DiagramNode() { Id = "SummingJunction", Shape = new { type = "Flow", shape = "SummingJunction" } });
            flowShapes.Add(new DiagramNode() { Id = "Or", Shape = new { type = "Flow", shape = "Or" } });
            flowShapes.Add(new DiagramNode() { Id = "InternalStorage", Shape = new { type = "Flow", shape = "InternalStorage" } });
            flowShapes.Add(new DiagramNode() { Id = "Extract", Shape = new { type = "Flow", shape = "Extract" } });
            flowShapes.Add(new DiagramNode() { Id = "ManualOperation", Shape = new { type = "Flow", shape = "ManualOperation" } });
            flowShapes.Add(new DiagramNode() { Id = "Merge", Shape = new { type = "Flow", shape = "Merge" } });
            flowShapes.Add(new DiagramNode() { Id = "OffPageReference", Shape = new { type = "Flow", shape = "OffPageReference" } });
            flowShapes.Add(new DiagramNode() { Id = "SequentialAccessStorage", Shape = new { type = "Flow", shape = "SequentialAccessStorage" } });
            flowShapes.Add(new DiagramNode() { Id = "Annotation", Shape = new { type = "Flow", shape = "Annotation" } });
            flowShapes.Add(new DiagramNode() { Id = "Annotation2", Shape = new { type = "Flow", shape = "Annotation2" } });
            flowShapes.Add(new DiagramNode() { Id = "Data", Shape = new { type = "Flow", shape = "Data" } });
            flowShapes.Add(new DiagramNode() { Id = "Card", Shape = new { type = "Flow", shape = "Card" } });
            flowShapes.Add(new DiagramNode() { Id = "Delay", Shape = new { type = "Flow", shape = "Delay" } });


            List<DiagramConnector> SymbolPaletteConnectors = new List<DiagramConnector>();
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link1",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link2",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link3",
                Type = Segments.Straight,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link4",
                Type = Segments.Straight,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            SymbolPaletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link5",
                Type = Segments.Bezier,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.None },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });


            //Swimlane
            List<Lane> lanes = new List<Lane>();
            Lane lane1 = new Lane();
            lane1.Id = "lane1";
            lane1.Height = 60;
            lane1.Width = 150;
            lane1.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes.Add(lane1);

            List<Lane> lanes1 = new List<Lane>();
            Lane lane2 = new Lane();
            lane2.Id = "lane2";
            lane2.Height = 150;
            lane2.Width = 60;
            lane2.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes1.Add(lane2);

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
            Dictionary<string, object> addInfo6 = new Dictionary<string, object>();
            addInfo6.Add("tooltip", "Vertical swimlane");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "stackCanvas2",
                Height = 140,
                Width = 60,
                AddInfo = addInfo6,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Lanes = lanes1,
                    Orientation = "Vertical",
                    IsLane = true
                },
                OffsetX = 70,
                OffsetY = 30
            });
            Dictionary<string, object> addInfo7 = new Dictionary<string, object>();
            addInfo7.Add("tooltip", "Vertical phase");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "verticalPhase",
                Height = 60,
                Width = 140,
                AddInfo = addInfo7,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Orientation = "Horizontal",
                    IsPhase = true
                }
            });
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


            List<SymbolPalettePalette> Palette = new List<SymbolPalettePalette>();
            Palette.Add(new SymbolPalettePalette() { Id = "flow", Expanded = true, Symbols = flowShapes, IconCss = "shapes", Title = "Flow Shapes" });
            Palette.Add(new SymbolPalettePalette() { Id = "swimlane", Expanded = true, Symbols = swimlanePalette, IconCss = "shapes", Title = "Swimlane" });
            Palette.Add(new SymbolPalettePalette() { Id = "connectors", Expanded = true, Symbols = SymbolPaletteConnectors, IconCss = "shapes", Title = "Connectors" });

            ViewBag.Palette = Palette;

            double[] intervals = { 1, 9, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75 };
            DiagramGridlines grIdLines = new DiagramGridlines()
            { LineColor = "#e0e0e0", LineIntervals = intervals };
            ViewBag.gridLines = grIdLines;

            DiagramMargin margin = new DiagramMargin() { Left = 15, Right = 15, Bottom = 15, Top = 15 };
            ViewBag.margin = margin;

            return View();
        }

        public ActionResult Serialization()
        {
            #region Nodes
            List<DiagramNode> nodes = new List<DiagramNode>();
            nodes.Add(new DiagramNode()
            {
                Id = "Start",
                OffsetX = 150,
                OffsetY = 80,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#d0f0f1", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                new DiagramNodeAnnotation() { Content = "Start" }
                },
                Shape = new { type = "Flow", shape = "Terminator" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Alarm",
                OffsetX = 150,
                OffsetY = 160,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#fbfdc5", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                new DiagramNodeAnnotation() { Content = "Alarm Rings" }},
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Ready",
                OffsetX = 150,
                OffsetY = 240,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#c5efaf", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                    new DiagramNodeAnnotation() { Content = "Ready to Get Up" }
                },
                Shape = new { type = "Flow", shape = "Decision" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Climb",
                OffsetX = 150,
                OffsetY = 330,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#fbfdc5", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                new DiagramNodeAnnotation() { Content = "Climb Out of Bed" }},
                Shape = new { type = "Flow", shape = "Process" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "End",
                OffsetX = 150,
                OffsetY = 430,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#d0f0f1", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                new DiagramNodeAnnotation() { Content = "End" }},
                Shape = new { type = "Flow", shape = "Terminator" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Relay",
                OffsetX = 350,
                OffsetY = 160,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#f8eee5", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                new DiagramNodeAnnotation() { Content = "Relay" }},
                Shape = new { type = "Flow", shape = "Delay" }
            });
            nodes.Add(new DiagramNode()
            {
                Id = "Hit",
                OffsetX = 350,
                OffsetY = 240,
                Width = 100,
                Height = 50,
                Style = new DiagramShapeStyle() { Fill = "#fbfdc5", StrokeColor = "#797979" },
                Annotations = new List<DiagramNodeAnnotation>() {
                new DiagramNodeAnnotation() { Content = "Hit Snooze Button", Margin = new DiagramMargin() { Left = 10, Right = 10, Bottom = 10, Top = 10 } }},
                Shape = new { type = "Flow", shape = "Process" }
            });
            #endregion

            #region Connectors
            List<DiagramConnector> connectors = new List<DiagramConnector>();
            connectors.Add(new DiagramConnector() { Id = "connector1", SourceID = "Start", TargetID = "Alarm" });
            connectors.Add(new DiagramConnector() { Id = "connector2", SourceID = "Alarm", TargetID = "Ready" });
            connectors.Add(new DiagramConnector() { Id = "connector3", SourceID = "Ready", TargetID = "Climb" });
            connectors.Add(new DiagramConnector() { Id = "connector4", SourceID = "Climb", TargetID = "End" });
            connectors.Add(new DiagramConnector() { Id = "connector5", SourceID = "Ready", TargetID = "Hit" });
            connectors.Add(new DiagramConnector() { Id = "connector6", SourceID = "Hit", TargetID = "Relay" });
            connectors.Add(new DiagramConnector() { Id = "connector7", SourceID = "Relay", TargetID = "Alarm" });
            #endregion


            ViewBag.nodes = nodes;
            ViewBag.connectors = connectors;

            List<DiagramNode> flowShapes = new List<DiagramNode>();
            flowShapes.Add(new DiagramNode() { Id = "Terminator", Shape = new { type = "Flow", shape = "Terminator" } });
            flowShapes.Add(new DiagramNode() { Id = "Process", Shape = new { type = "Flow", shape = "Process" } });
            flowShapes.Add(new DiagramNode() { Id = "Decision", Shape = new { type = "Flow", shape = "Decision" } });
            flowShapes.Add(new DiagramNode() { Id = "Document", Shape = new { type = "Flow", shape = "Document" } });
            flowShapes.Add(new DiagramNode() { Id = "PreDefinedProcess", Shape = new { type = "Flow", shape = "PreDefinedProcess" } });
            flowShapes.Add(new DiagramNode() { Id = "PaperTap", Shape = new { type = "Flow", shape = "PaperTap" } });
            flowShapes.Add(new DiagramNode() { Id = "DirectData", Shape = new { type = "Flow", shape = "DirectData" } });
            flowShapes.Add(new DiagramNode() { Id = "SequentialData", Shape = new { type = "Flow", shape = "SequentialData" } });
            flowShapes.Add(new DiagramNode() { Id = "Sort", Shape = new { type = "Flow", shape = "Sort" } });
            flowShapes.Add(new DiagramNode() { Id = "MultiDocument", Shape = new { type = "Flow", shape = "MultiDocument" } });
            flowShapes.Add(new DiagramNode() { Id = "Collate", Shape = new { type = "Flow", shape = "Collate" } });
            flowShapes.Add(new DiagramNode() { Id = "SummingJunction", Shape = new { type = "Flow", shape = "SummingJunction" } });
            flowShapes.Add(new DiagramNode() { Id = "Or", Shape = new { type = "Flow", shape = "Or" } });
            flowShapes.Add(new DiagramNode() { Id = "InternalStorage", Shape = new { type = "Flow", shape = "InternalStorage" } });
            flowShapes.Add(new DiagramNode() { Id = "Extract", Shape = new { type = "Flow", shape = "Extract" } });
            flowShapes.Add(new DiagramNode() { Id = "ManualOperation", Shape = new { type = "Flow", shape = "ManualOperation" } });
            flowShapes.Add(new DiagramNode() { Id = "Merge", Shape = new { type = "Flow", shape = "Merge" } });
            flowShapes.Add(new DiagramNode() { Id = "OffPageReference", Shape = new { type = "Flow", shape = "OffPageReference" } });
            flowShapes.Add(new DiagramNode() { Id = "SequentialAccessStorage", Shape = new { type = "Flow", shape = "SequentialAccessStorage" } });
            flowShapes.Add(new DiagramNode() { Id = "Annotation", Shape = new { type = "Flow", shape = "Annotation" } });
            flowShapes.Add(new DiagramNode() { Id = "Annotation2", Shape = new { type = "Flow", shape = "Annotation2" } });
            flowShapes.Add(new DiagramNode() { Id = "Data", Shape = new { type = "Flow", shape = "Data" } });
            flowShapes.Add(new DiagramNode() { Id = "Card", Shape = new { type = "Flow", shape = "Card" } });
            flowShapes.Add(new DiagramNode() { Id = "Delay", Shape = new { type = "Flow", shape = "Delay" } });

            List<DiagramNode> linkShapes = new List<DiagramNode>();
            linkShapes.Add(new DiagramNode() { Id = "Control", Shape = new { type = "Basic", shape = "Plus" } });
            linkShapes.Add(new DiagramNode() { Id = "Riesgo", Shape = new { type = "Basic", shape = "Star" } });


            List<DiagramConnector> paletteConnectors = new List<DiagramConnector>();
            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link1",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
                TargetDecorator = new ConnectorTargetDecoratorConnectors() { Shape = DecoratorShapes.Arrow, Style = new DiagramShapeStyle() { StrokeColor = "#757575", Fill = "#757575" } },
                Style = new DiagramStrokeStyle() { StrokeWidth = 2, StrokeColor = "#757575" }
            });
            paletteConnectors.Add(new DiagramConnector()
            {
                Id = "Link2",
                Type = Segments.Orthogonal,
                SourcePoint = new DiagramPoint() { X = 0, Y = 0 },
                TargetPoint = new DiagramPoint() { X = 40, Y = 40 },
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

            //Swimlane
            List<Lane> lanes = new List<Lane>();
            Lane lane1 = new Lane();
            lane1.Id = "lane1";
            lane1.Height = 60;
            lane1.Width = 150;
            lane1.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes.Add(lane1);

            List<Lane> lanes1 = new List<Lane>();
            Lane lane2 = new Lane();
            lane2.Id = "lane2";
            lane2.Height = 150;
            lane2.Width = 60;
            lane2.Header = new Header() { Width = 50, Height = 50, Style = new DiagramTextStyle() { FontSize = 11, StrokeColor = "#757575" } };
            lanes1.Add(lane2);

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
            Dictionary<string, object> addInfo6 = new Dictionary<string, object>();
            addInfo6.Add("tooltip", "Vertical swimlane");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "stackCanvas2",
                Height = 140,
                Width = 60,
                AddInfo = addInfo6,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Lanes = lanes1,
                    Orientation = "Vertical",
                    IsLane = true
                },
                OffsetX = 70,
                OffsetY = 30
            });
            Dictionary<string, object> addInfo7 = new Dictionary<string, object>();
            addInfo7.Add("tooltip", "Vertical phase");
            swimlanePalette.Add(new DiagramNode()
            {
                Id = "verticalPhase",
                Height = 60,
                Width = 140,
                AddInfo = addInfo7,
                Shape = new SwimLaneModel()
                {
                    Type = "SwimLane",
                    Orientation = "Horizontal",
                    IsPhase = true
                }
            });
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
            palettes.Add(new SymbolPalettePalette() { Id = "links", Expanded = false, Symbols = linkShapes, IconCss = "shapes", Title = "Ligas" });
            palettes.Add(new SymbolPalettePalette() { Id = "swimlane", Expanded = true, Symbols = swimlanePalette, IconCss = "shapes", Title = "Swimlane" });
            palettes.Add(new SymbolPalettePalette() { Id = "connectors", Expanded = true, Symbols = paletteConnectors, IconCss = "shapes", Title = "Connectors" });

            ViewBag.Palette = palettes;

            ViewBag.Spconnectors = paletteConnectors;

            double[] intervals = { 1, 9, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75, 0.25, 9.75 };
            DiagramGridlines grIdLines = new DiagramGridlines()
            { LineColor = "#e0e0e0", LineIntervals = intervals };
            ViewBag.gridLines = grIdLines;

            DiagramMargin margin = new DiagramMargin() { Left = 15, Right = 15, Bottom = 15, Top = 15 };
            ViewBag.margin = margin;

            return View();
        }
        public void GenerateDiagramNodes(List<DiagramNode> Nodes)
        {
            List<DiagramPort> ports = new List<DiagramPort>();
            ports.Add(new DiagramPort() { Id = "Port1", Offset = new DiagramPoint() { X = 0, Y = 0.5 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port2", Offset = new DiagramPoint() { X = 0.5, Y = 0 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port3", Offset = new DiagramPoint() { X = 1, Y = 0.5 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });
            ports.Add(new DiagramPort() { Id = "Port4", Offset = new DiagramPoint() { X = 0.5, Y = 1 }, Visibility = PortVisibility.Connect | PortVisibility.Hover, Constraints = PortConstraints.Default | PortConstraints.Draw });


            //Add lanes children
            List<DiagramNode> firstLaneChildren = new List<DiagramNode>();
            List<DiagramNodeAnnotation> node1Annotation = new List<DiagramNodeAnnotation>();
            node1Annotation.Add(new DiagramNodeAnnotation() { Content = "Consumer learns \n of product", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node1 = new DiagramNode() { Id = "node1", Annotations = node1Annotation, Margin = new DiagramMargin() { Left = 60, Top = 30 }, Height = 40, Width = 100, Ports = ports };
            firstLaneChildren.Add(node1);

            List<DiagramNodeAnnotation> node2Annotation = new List<DiagramNodeAnnotation>();
            node2Annotation.Add(new DiagramNodeAnnotation() { Content = "Does \n Consumer want \n the product", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node2 = new DiagramNode() { Id = "node2", Annotations = node2Annotation, Margin = new DiagramMargin() { Left = 200, Top = 20 }, Height = 60, Width = 120, Shape = new { type = "Flow", shape = "Decision" }, Ports = ports };
            firstLaneChildren.Add(node2);

            List<DiagramNodeAnnotation> node3Annotation = new List<DiagramNodeAnnotation>();
            node3Annotation.Add(new DiagramNodeAnnotation() { Content = "No sales lead", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node3 = new DiagramNode() { Id = "node3", Shape = new { type = "Path", data = pathData }, Annotations = node3Annotation, Margin = new DiagramMargin() { Left = 370, Top = 30 }, Height = 40, Width = 100, Ports = ports };
            firstLaneChildren.Add(node3);

            List<DiagramNodeAnnotation> node4Annotation = new List<DiagramNodeAnnotation>();
            node4Annotation.Add(new DiagramNodeAnnotation() { Content = "Sell to consumer", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node4 = new DiagramNode() { Id = "node4", Shape = new { type = "Path", data = pathData }, Annotations = node4Annotation, Margin = new DiagramMargin() { Left = 510, Top = 30 }, Height = 40, Width = 100, Ports = ports };
            firstLaneChildren.Add(node4);


            List<DiagramNode> secondLaneChildren = new List<DiagramNode>();
            List<DiagramNodeAnnotation> node5Annotation = new List<DiagramNodeAnnotation>();
            node5Annotation.Add(new DiagramNodeAnnotation() { Content = "Create marketing campaigns", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node5 = new DiagramNode() { Id = "node5", Annotations = node5Annotation, Margin = new DiagramMargin() { Left = 60, Top = 20 }, Height = 40, Width = 100, Ports = ports };
            secondLaneChildren.Add(node5);

            List<DiagramNodeAnnotation> node6Annotation = new List<DiagramNodeAnnotation>();
            node6Annotation.Add(new DiagramNodeAnnotation() { Content = "Marketing finds sales leads", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node6 = new DiagramNode() { Id = "node6", Annotations = node6Annotation, Margin = new DiagramMargin() { Left = 210, Top = 20 }, Height = 40, Width = 100, Ports = ports };
            secondLaneChildren.Add(node6);

            List<DiagramNode> thirdLaneChildren = new List<DiagramNode>();
            List<DiagramNodeAnnotation> node7Annotation = new List<DiagramNodeAnnotation>();
            node7Annotation.Add(new DiagramNodeAnnotation() { Content = "Sales receives lead", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node7 = new DiagramNode() { Id = "node7", Annotations = node7Annotation, Margin = new DiagramMargin() { Left = 210, Top = 30 }, Height = 40, Width = 100, Ports = ports };
            thirdLaneChildren.Add(node7);

            List<DiagramNode> fourthLaneChildren = new List<DiagramNode>();
            List<DiagramNodeAnnotation> node8Annotation = new List<DiagramNodeAnnotation>();
            node8Annotation.Add(new DiagramNodeAnnotation() { Content = "Success helps \n retain consumer \n as a customer", Style = new DiagramTextStyle() { FontSize = 11 } });
            DiagramNode node8 = new DiagramNode() { Id = "node8", Annotations = node8Annotation, Margin = new DiagramMargin() { Left = 510, Top = 20 }, Height = 50, Width = 100, Ports = ports };
            fourthLaneChildren.Add(node8);

            //Create swimlane
            DiagramNode swimlane = new DiagramNode();
            swimlane.Id = "swimlane";
            swimlane.Width = 650;
            swimlane.Height = 100;
            swimlane.OffsetX = 440;
            swimlane.OffsetY = 320;
            //Create lanes
            List<Lane> Lanes = new List<Lane>();
            Lanes.Add(new Lane()
            {
                Id = "stackCanvas1",
                Height = 100,
                Header = new Header()
                {
                    Annotation = new DiagramNodeAnnotation() { Content = "Consumer" },
                    Width = 50,
                },
                Children = firstLaneChildren
            });

            Lanes.Add(new Lane()
            {
                Id = "stackCanvas2",
                Height = 100,
                Header = new Header()
                {
                    Annotation = new DiagramNodeAnnotation() { Content = "Marketing" },
                    Width = 50,
                },
                Children = secondLaneChildren
            });

            Lanes.Add(new Lane()
            {
                Id = "stackCanvas3",
                Height = 100,
                Header = new Header()
                {
                    Annotation = new DiagramNodeAnnotation() { Content = "Sales" },
                    Width = 50,
                },
                Children = thirdLaneChildren
            });

            Lanes.Add(new Lane()
            {
                Id = "stackCanvas4",
                Height = 100,
                Header = new Header()
                {
                    Annotation = new DiagramNodeAnnotation() { Content = "Success" },
                    Width = 50,
                },
                Children = fourthLaneChildren
            });

            //Create phases
            List<Phase> Phases = new List<Phase>();
            Phases.Add(new Phase()
            {
                Id = "phase1",
                Offset = 170,
                Header = new Header()
                {
                    Annotation = new DiagramNodeAnnotation() { Content = "Phase" },
                },
            });

            swimlane.Shape = new SwimLane()
            {
                Type = "SwimLane",
                PhaseSize = 20,
                Header = new Header()
                {
                    Annotation = new DiagramNodeAnnotation() { Content = "SALES PROCESS FLOW CHART" },
                    Height = 50,
                    Orientation = "Horizontal",
                    Style = new DiagramTextStyle() { FontSize = 11 }
                },
                Lanes = Lanes,
                Phases = Phases
            };
            Nodes.Add(swimlane);
        }

        #region Crea Conectores
        //Create connectors
        public void GenerateDiagramConnectors(List<DiagramConnector> Connectors)
        {
            Connectors.Add(CreateSwimlaneConnector("connector1", "node1", "node2", ""));
            Connectors.Add(CreateSwimlaneConnector("connector2", "node2", "node3", "No"));
            Connectors.Add(CreateSwimlaneConnector("connector3", "node4", "node8", ""));
            Connectors.Add(CreateSwimlaneConnector("connector4", "node2", "node6", "Yes"));
            Connectors.Add(CreateSwimlaneConnector("connector5", "node5", "node1", ""));
            Connectors.Add(CreateSwimlaneConnector("connector6", "node6", "node7", ""));
            Connectors.Add(CreateSwimlaneConnector("connector7", "node4", "node7", ""));
        }
        public DiagramConnector CreateSwimlaneConnector(string id, string sourceID, string targetID, string label)
        {
            DiagramConnector connector = new DiagramConnector();
            connector.Id = id;
            connector.Type = Segments.Orthogonal;
            connector.SourceID = sourceID;
            connector.TargetID = targetID;
            if (sourceID == "node4" && targetID == "node7")
            {
                connector.SourcePortID = "Port1";
                connector.TargetPortID = "Port3";
            }
            if (label != "")
            {
                connector.Annotations = new List<DiagramConnectorAnnotation>()
                {
                    new DiagramConnectorAnnotation()
                    {
                        Content = label,
                        Style = new DiagramTextStyle()
                        {
                            Fill = "white"
                        }
                    }
                };
            }
            return connector;
        }

        #endregion
    }

    //#region Clases Públicas
    //public class SwimLaneModel
    //{
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("type")]
    //    [JsonProperty("type")]
    //    public string Type { get; set; }

    //    [DefaultValue(null)]
    //    [HtmlAttributeName("lanes")]
    //    [JsonProperty("lanes")]
    //    public List<Lane> Lanes { get; set; }

    //    [DefaultValue(null)]
    //    [HtmlAttributeName("orientation")]
    //    [JsonProperty("orientation")]
    //    public string Orientation { get; set; }

    //    [DefaultValue(null)]
    //    [HtmlAttributeName("isLane")]
    //    [JsonProperty("isLane")]
    //    public bool IsLane { get; set; }

    //    [DefaultValue(null)]
    //    [HtmlAttributeName("isPhase")]
    //    [JsonProperty("isPhase")]
    //    public bool IsPhase { get; set; }
    //}

    //public class SwimLane
    //{
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("type")]
    //    [JsonProperty("type")]
    //    public string Type
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("header")]
    //    [JsonProperty("header")]
    //    public Header Header
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("lanes")]
    //    [JsonProperty("lanes")]
    //    public List<Lane> Lanes
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("phases")]
    //    [JsonProperty("phases")]
    //    public List<Phase> Phases
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("phaseSize")]
    //    [JsonProperty("phaseSize")]
    //    public double PhaseSize
    //    {
    //        get;
    //        set;
    //    }
    //}

    //public class Header
    //{
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("annotation")]
    //    [JsonProperty("annotation")]
    //    public object Annotation
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("height")]
    //    [JsonProperty("height")]
    //    public double Height
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("width")]
    //    [JsonProperty("width")]
    //    public double Width
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("orientation")]
    //    [JsonProperty("orientation")]
    //    public string Orientation
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("style")]
    //    [JsonProperty("style")]
    //    public DiagramTextStyle Style
    //    {
    //        get;
    //        set;
    //    }
    //}

    //public class Lane
    //{
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("id")]
    //    [JsonProperty("id")]
    //    public string Id
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("header")]
    //    [JsonProperty("header")]
    //    public Header Header
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("style")]
    //    [JsonProperty("style")]
    //    public DiagramTextStyle Style
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("height")]
    //    [JsonProperty("height")]
    //    public double Height
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("width")]
    //    [JsonProperty("width")]
    //    public double Width
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("children")]
    //    [JsonProperty("children")]
    //    public List<DiagramNode> Children
    //    {
    //        get;
    //        set;
    //    }
    //}

    //public class Phase
    //{
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("id")]
    //    [JsonProperty("id")]
    //    public string Id
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("offset")]
    //    [JsonProperty("offset")]
    //    public double Offset
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("style")]
    //    [JsonProperty("style")]
    //    public DiagramTextStyle Style
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("header")]
    //    [JsonProperty("header")]
    //    public Header Header
    //    {
    //        get;
    //        set;
    //    }
    //}

    //public class MenuItems
    //{
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("text")]
    //    [JsonProperty("text")]
    //    public string Text
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("id")]
    //    [JsonProperty("id")]
    //    public string Id
    //    {
    //        get;
    //        set;
    //    }
    //    [DefaultValue(null)]
    //    [HtmlAttributeName("target")]
    //    [JsonProperty("target")]
    //    public string Target
    //    {
    //        get;
    //        set;
    //    }
    //}
    //#endregion
}