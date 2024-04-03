using iTextSharp.text;
using iTextSharp.text.pdf;
using SCIRA.Models;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCIRA.Utilidades
{
    public static class GenerateDoc
    {
        static private SICIEntities db = new SICIEntities();

        #region Definición de Fuentes y variables auxiliares
        static private Font fontC4 = new Font(null, 4, Font.NORMAL);
        static private Font fontC4B = new Font(null, 4, Font.BOLD);
        static private Font fontC4W = new Font(null, 4, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC4BW = new Font(null, 4, Font.BOLD, BaseColor.WHITE);

        static private Font fontC5 = new Font(null, 5, Font.NORMAL);
        static private Font fontC5B = new Font(null, 5, Font.BOLD);
        static private Font fontC5W = new Font(null, 5, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC5BW = new Font(null, 5, Font.BOLD, BaseColor.WHITE);

        static private Font fontC6 = new Font(null, 6, Font.NORMAL);
        static private Font fontC6B = new Font(null, 6, Font.BOLD);
        static private Font fontC6W = new Font(null, 6, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC6BW = new Font(null, 6, Font.BOLD, BaseColor.WHITE);

        static private Font fontC7 = new Font(null, 7, Font.NORMAL);
        static private Font fontC7B = new Font(null, 7, Font.BOLD);
        static private Font fontC7W = new Font(null, 7, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC7BW = new Font(null, 7, Font.BOLD, BaseColor.WHITE);

        static private Font fontC8 = new Font(null, 8, Font.NORMAL);
        static private Font fontC8B = new Font(null, 8, Font.BOLD);
        static private Font fontC8W = new Font(null, 8, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC8BW = new Font(null, 8, Font.BOLD, BaseColor.WHITE);

        static private Font fontC9 = new Font(null, 9, Font.NORMAL);
        static private Font fontC9B = new Font(null, 9, Font.BOLD);
        static private Font fontC9W = new Font(null, 9, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC9BW = new Font(null, 9, Font.BOLD, BaseColor.WHITE);

        static private Font fontC10 = new Font(null, 10, Font.NORMAL);
        static private Font fontC10B = new Font(null, 10, Font.BOLD);
        static private Font fontC10W = new Font(null, 10, Font.NORMAL, BaseColor.WHITE);
        static private Font fontC10BW = new Font(null, 10, Font.BOLD, BaseColor.WHITE);

        static private Paragraph ph1, ph2;//, ph3;
        //static private Phrase p1, p2, p3;
        static private Chunk ch1, ch2;//, ch3, ch4, ch5;
        static private PdfPCell c1, c2, c3, c4, c5, c6, c7;


        static private BaseColor BGGray = new BaseColor(128, 128, 128);
        static private BaseColor BGGray2 = new BaseColor(170, 170, 170);
        static private BaseColor BGClearGray = new BaseColor(90, 90, 90);
        #endregion

        #region Manuales
        public static byte[] Manual(c_contenido_manual rootContent, int sp)
        {
            RenewDB();

            //generación del pdf
            MemoryStream os = new MemoryStream();

            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);

            writeSoons(doc, 0, rootContent, sp);

            doc.Close();

            return os.GetBuffer();
        }


        private static void writeSoons(Document doc, int ident, c_contenido_manual rootContent, int sp)
        {
            string sps = "";

            if (sp == 1) sps = GetSpNames(rootContent);

            var fName = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Manuales/c" + rootContent.id_contenido_manual + ".png");


            ph1 = new Paragraph(rootContent.cl_contenido_manual + sps, fontC10B);
            ph1.IndentationLeft = 15 * ident;
            doc.Add(ph1);

            



            ph1 = new Paragraph(rootContent.ds_contenido_manual, fontC8);
            ph1.IndentationLeft = 15 * ident;
            ph1.Alignment = Element.ALIGN_JUSTIFIED;
            doc.Add(ph1);


            //Si existe imagen agregarla
            if (System.IO.File.Exists(fName))
            {
                Image jpg = Image.GetInstance(fName);

                //if(jpg.Width > 500)
                //    jpg.ScaleToFit(500f, 500f);

                ////Give space before image
                //jpg.SpacingBefore = 10f;

                ////Give some space after the image
                //jpg.SpacingAfter = 10f;

                jpg.ScaleToFit(450f, 450f);

                jpg.SpacingAfter = 12f;

                jpg.SpacingBefore = 12f;

                jpg.Alignment = Element.ALIGN_CENTER;

                ph1 = new Paragraph();
                ph1.Add(jpg);
                ph1.Add(new Chunk("\n"));
                //ph1.Alignment = Element.ALIGN_CENTER;


                doc.Add(ph1);
                doc.NewPage();
            }
            

            var hijos = db.c_contenido_manual.Where(m => m.id_contenido_manual_padre == rootContent.id_contenido_manual).OrderBy(c => c.no_orden).ToList();

            foreach (var hijo in hijos)
            {
                writeSoons(doc, ident + 1, hijo, sp);
            }

            return;
        }

        private static string GetSpNames(c_contenido_manual rootContent)
        {
            var sps = rootContent.c_sub_proceso.ToList();

            if (sps.Count == 0) return "";

            string aux = "  (";

            for (int i = 0; i < sps.Count; i++)
            {
                var sp = sps.ElementAt(i);
                if (i == sps.Count - 1) aux += sp.cl_sub_proceso;
                else aux += sp.cl_sub_proceso + ", ";
            }
            aux += ")";

            return aux;
        }

        #endregion

        #region Revision del Control
        public static byte[] RevisionControl(k_revision_control Model)
        {
            #region Variables Auxiliares
            var control = Model.k_control;
            var entidad = control.c_sub_proceso.c_proceso.c_macro_proceso.c_entidad;
            var mp = control.c_sub_proceso.c_proceso.c_macro_proceso;
            var pr = control.c_sub_proceso.c_proceso;
            var sp = control.c_sub_proceso;
            var riesgo = control.k_riesgo.First();
            var responsable = control.c_usuario1;
            var ejecutor = control.c_usuario;
            var area = responsable.c_area;
            var aseveraciones = control.c_aseveracion.ToList();
            var rrData = Utilidades.RRData(control);

            var infoCEC = Utilidades.infoCamposExtra("k_control", 20);
            var icec1 = infoCEC.ElementAt(0);
            var icec2 = infoCEC.ElementAt(1);
            var icec3 = infoCEC.ElementAt(2);

            string asev = "";
            string asevR = "";

            var inputs = Model.a_input_rc.ToList();
            var pruebas = Model.a_procedimiento_prueba_rc.ToList();

            foreach (var ase in aseveraciones)
            {
                asev += ase.cl_aseveracion + " " + ase.nb_aseveracion + "\n";
            }

            foreach (var ase in Model.c_aseveracion)
            {
                asevR += ase.cl_aseveracion + " " + ase.nb_aseveracion + "\n";
            }


            var rcTipologia = "";
            var rcFrecuencia = "";

            try
            {
                rcTipologia = Model.c_tipologia_control.cl_tipologia_control + " - " + Model.c_tipologia_control.nb_tipologia_control;
            }
            catch { }
            try
            {
                rcFrecuencia = Model.c_frecuencia_control.cl_frecuencia_control + " - " + Model.c_frecuencia_control.nb_frecuencia_control;
            }
            catch { }


            var edcNaturaleza = "";
            var edcTipologia = "";
            var edcTipoEvidencia = "";
            var edcCategoriaControl = "";

            try
            {
                edcNaturaleza = Model.c_naturaleza_control.cl_naturaleza_control + " - " + Model.c_naturaleza_control.nb_naturaleza_control;
            }
            catch { }
            try
            {
                edcTipologia = Model.c_tipologia_control1.cl_tipologia_control + " - " + Model.c_tipologia_control1.nb_tipologia_control;
            }
            catch { }
            try
            {
                edcTipoEvidencia = Model.c_tipo_evidencia.cl_tipo_evidencia + " - " + Model.c_tipo_evidencia.nb_tipo_evidencia;
            }
            catch { }
            try
            {
                edcCategoriaControl = Model.c_categoria_control.cl_categoria_control + " - " + Model.c_categoria_control.nb_categoria_control;
            }
            catch { }
            #endregion

            //generación del pdf
            MemoryStream os = new MemoryStream();

            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);


            ph1 = new Paragraph("Ficha de Revisión del Control: " + control.relacion_control, fontC10B);
            ph1.Alignment = Element.ALIGN_CENTER;
            doc.Add(ph1);
            ph1 = new Paragraph("\nNombre del revisor: " + Model.dg_nb_responsable_revision, fontC8);
            doc.Add(ph1);
            ph1 = new Paragraph("Fecha de Revisión: " + ((DateTime)Model.dg_fe_revision).ToShortDateString() + "\n\n", fontC8);
            doc.Add(ph1);


            #region Tabla de Datos Generales
            PdfPTable t1 = new PdfPTable(5);
            t1.TotalWidth = 530f;
            t1.LockedWidth = true;

            //Encabezado
            c1 = new PdfPCell(new Phrase("Datos Generales", fontC9BW))
            {
                BackgroundColor = BGGray,
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            t1.AddCell(c1);

            c2 = new PdfPCell(new Paragraph("Campo", fontC8B));
            c3 = new PdfPCell(new Paragraph("Óriginal", fontC8B));
            c4 = new PdfPCell(new Paragraph("Revisión", fontC8B));
            c5 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c6 = new PdfPCell(new Paragraph("Calificación", fontC8B));
            t1.AddCell(c2);
            t1.AddCell(c3);
            t1.AddCell(c4);
            t1.AddCell(c5);
            t1.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Entidad", fontC7B));
            c3 = new PdfPCell(new Paragraph(entidad.cl_entidad + " - " + entidad.nb_entidad, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.dg_entidad, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.dg_obs_1, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion != null ? Model.c_calificacion.nb_calificacion : "", fontC7));
            t1.AddCell(c2);
            t1.AddCell(c3);
            t1.AddCell(c4);
            t1.AddCell(c5);
            t1.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Macro Proceso", fontC7B));
            c3 = new PdfPCell(new Paragraph(mp.cl_macro_proceso + " - " + mp.nb_macro_proceso, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.dg_marco_proceso, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.dg_obs_2, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion1 != null ? Model.c_calificacion1.nb_calificacion : "", fontC7));
            t1.AddCell(c2);
            t1.AddCell(c3);
            t1.AddCell(c4);
            t1.AddCell(c5);
            t1.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Proceso", fontC7B));
            c3 = new PdfPCell(new Paragraph(pr.cl_proceso + " - " + pr.nb_proceso, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.dg_proceso, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.dg_obs_3, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion2 != null ? Model.c_calificacion2.nb_calificacion : "", fontC7));
            t1.AddCell(c2);
            t1.AddCell(c3);
            t1.AddCell(c4);
            t1.AddCell(c5);
            t1.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Sub Proceso", fontC7B));
            c3 = new PdfPCell(new Paragraph(sp.cl_sub_proceso + " - " + sp.nb_sub_proceso, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.dg_sub_proceso, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.dg_obs_4, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion3 != null ? Model.c_calificacion3.nb_calificacion : "", fontC7));
            t1.AddCell(c2);
            t1.AddCell(c3);
            t1.AddCell(c4);
            t1.AddCell(c5);
            t1.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Tarea", fontC7B));
            c3 = new PdfPCell(new Paragraph(sp.ds_sub_proceso, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.dg_tarea, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.dg_obs_5, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion4 != null ? Model.c_calificacion4.nb_calificacion : "", fontC7));
            t1.AddCell(c2);
            t1.AddCell(c3);
            t1.AddCell(c4);
            t1.AddCell(c5);
            t1.AddCell(c6);

            doc.Add(t1);
            #endregion

            #region Tabla Resumen del Riesgo
            doc.Add(new Paragraph("\n\n"));
            PdfPTable t2 = new PdfPTable(5);
            t2.TotalWidth = 530f;
            t2.LockedWidth = true;

            //Encabezado
            c1.Phrase = new Phrase("Resumen del Riesgo", fontC9BW);
            t2.AddCell(c1);

            c2 = new PdfPCell(new Paragraph("Campo", fontC8B));
            c3 = new PdfPCell(new Paragraph("Óriginal", fontC8B));
            c4 = new PdfPCell(new Paragraph("Revisión", fontC8B));
            c5 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c6 = new PdfPCell(new Paragraph("Calificación", fontC8B));
            t2.AddCell(c2);
            t2.AddCell(c3);
            t2.AddCell(c4);
            t2.AddCell(c5);
            t2.AddCell(c6);

            c6 = new PdfPCell()
            {
                BackgroundColor = BGGray
            };

            c2 = new PdfPCell(new Paragraph("Código del Riesgo", fontC7B));
            c3 = new PdfPCell(new Paragraph(riesgo.nb_riesgo, fontC7));
            t2.AddCell(c2);
            t2.AddCell(c3);
            t2.AddCell(c6);
            t2.AddCell(c6);
            t2.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Evento de Riesgo", fontC7B));
            c3 = new PdfPCell(new Paragraph(riesgo.evento, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rr_evento_riesgo, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rr_obs_1, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion5 != null ? Model.c_calificacion5.nb_calificacion : "", fontC7));
            t2.AddCell(c2);
            t2.AddCell(c3);
            t2.AddCell(c4);
            t2.AddCell(c5);
            t2.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Magnitud de Impacto", fontC7B));
            c3 = new PdfPCell(new Paragraph(riesgo.c_magnitud_impacto.cl_magnitud_impacto + " - " + riesgo.c_magnitud_impacto.nb_magnitud_impacto, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rr_magnitud_impacto, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rr_obs_2, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion6 != null ? Model.c_calificacion6.nb_calificacion : "", fontC7));
            t2.AddCell(c2);
            t2.AddCell(c3);
            t2.AddCell(c4);
            t2.AddCell(c5);
            t2.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Probabilidad de Ocurrencia", fontC7B));
            c3 = new PdfPCell(new Paragraph(riesgo.c_probabilidad_ocurrencia.cl_probabilidad_ocurrencia + " - " + riesgo.c_probabilidad_ocurrencia.nb_probabilidad_ocurrencia, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rr_probabilidad_ocurrencia, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rr_obs_2, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion7 != null ? Model.c_calificacion7.nb_calificacion : "", fontC7));
            t2.AddCell(c2);
            t2.AddCell(c3);
            t2.AddCell(c4);
            t2.AddCell(c5);
            t2.AddCell(c6);



            doc.Add(t2);
            #endregion

            #region Tabla Resumen del Control
            doc.Add(new Paragraph("\n\n"));
            PdfPTable t3 = new PdfPTable(5);
            t3.TotalWidth = 530f;
            t3.LockedWidth = true;

            //Encabezado
            c1.Phrase = new Phrase("Resumen del Control", fontC9BW);
            t3.AddCell(c1);

            c2 = new PdfPCell(new Paragraph("Campo", fontC8B));
            c3 = new PdfPCell(new Paragraph("Óriginal", fontC8B));
            c4 = new PdfPCell(new Paragraph("Revisión", fontC8B));
            c5 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c6 = new PdfPCell(new Paragraph("Calificación", fontC8B));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);


            c6 = new PdfPCell()
            {
                BackgroundColor = BGGray
            };

            c2 = new PdfPCell(new Paragraph("Código del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.relacion_control, fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c6);
            t3.AddCell(c6);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Código del Manual", fontC7B));
            c3 = new PdfPCell(new Paragraph(sp.cl_manual, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_codigo_manual, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_1, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion8 != null ? Model.c_calificacion8.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Actividad del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.actividad_control, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_actividad_control, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_2, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion9 != null ? Model.c_calificacion9.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Evidencia del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.evidencia_control, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_evidencia_control, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_3, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion39 != null ? Model.c_calificacion39.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph(icec1.nb_campo, fontC7B));
            c3 = new PdfPCell(new Paragraph(control.campo01, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_ds_control, fontC7))
            {
                Rowspan = 2
            };
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_4, fontC7))
            {
                Rowspan = 2
            };
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion10 != null ? Model.c_calificacion10.nb_calificacion : "", fontC7))
            {
                Rowspan = 2
            };
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);
            c2 = new PdfPCell(new Paragraph(icec2.nb_campo, fontC7B));
            c3 = new PdfPCell(new Paragraph(control.campo02, fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);

            c2 = new PdfPCell(new Paragraph(icec3.nb_campo, fontC7B));
            c3 = new PdfPCell(new Paragraph(control.campo03, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_dir_general, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_5, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion11 != null ? Model.c_calificacion11.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Área Responsable", fontC7B));
            c3 = new PdfPCell(new Paragraph(area.cl_area + " - " + area.nb_area, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_area, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_6, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion12 != null ? Model.c_calificacion12.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Responsable del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(responsable.nb_puesto, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_responsable, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_7, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion13 != null ? Model.c_calificacion13.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Puesto del Responsable", fontC7B));
            c3 = new PdfPCell(new Paragraph(Utilidades.PuestoUsuario(responsable.id_usuario), fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_puesto_responsable, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_8, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion14 != null ? Model.c_calificacion14.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Ejecutor del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(ejecutor.nb_usuario, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_ejecutor, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_9, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion15 != null ? Model.c_calificacion15.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Puesto del Ejecutor", fontC7B));
            c3 = new PdfPCell(new Paragraph(Utilidades.PuestoUsuario(ejecutor.id_usuario), fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_puesto_ejecutor, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_10, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion16 != null ? Model.c_calificacion16.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Tipología del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.c_tipologia_control.cl_tipologia_control + " - " + control.c_tipologia_control.nb_tipologia_control, fontC7));
            c4 = new PdfPCell(new Paragraph(rcTipologia, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_11, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion17 != null ? Model.c_calificacion17.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Frecuencia del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.c_frecuencia_control.cl_frecuencia_control + " - " + control.c_frecuencia_control.nb_frecuencia_control, fontC7));
            c4 = new PdfPCell(new Paragraph(rcFrecuencia, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_12, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion18 != null ? Model.c_calificacion18.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Aseveraciones", fontC7B));
            c3 = new PdfPCell(new Paragraph(asev, fontC7));
            c4 = new PdfPCell(new Paragraph(asevR, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_13, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion19 != null ? Model.c_calificacion19.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("¿Es control clave?", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.es_control_clave ? "Si" : "No", fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_control_clave ? "Si" : "No", fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_14, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion20 != null ? Model.c_calificacion20.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("¿Depende de otro control?", fontC7B));
            c3 = new PdfPCell(new Paragraph("", fontC7));
            c4 = new PdfPCell(new Paragraph(Model.rc_control_dependiente ? "Si" : "No", fontC7));
            c5 = new PdfPCell(new Paragraph(Model.rc_obs_15, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion21 != null ? Model.c_calificacion21.nb_calificacion : "", fontC7));
            t3.AddCell(c2);
            t3.AddCell(c3);
            t3.AddCell(c4);
            t3.AddCell(c5);
            t3.AddCell(c6);

            doc.Add(t3);
            #endregion

            #region Tabla Evaluación del Diseño del Control
            doc.Add(new Paragraph("\n\n"));
            PdfPTable t4 = new PdfPTable(5);
            t4.TotalWidth = 530f;
            t4.LockedWidth = true;

            //Encabezado
            c1.Phrase = new Phrase("Evaluación del Diseño del Control", fontC9BW);
            t4.AddCell(c1);



            c2 = new PdfPCell(new Paragraph("Información de Entrada", fontC8B));
            c3 = new PdfPCell(new Paragraph(Model.edc_informacion_inputs, fontC7));
            c3.Colspan = 2;
            c4 = new PdfPCell(new Paragraph("Calificacion", fontC8B));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion22 != null ? Model.c_calificacion22.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c3 = new PdfPCell(new Paragraph(Model.edc_obs_1, fontC7));
            c2.Colspan = 2;
            c3.Colspan = 3;
            t4.AddCell(c2);
            t4.AddCell(c3);


            if (inputs.Count > 0)
            {
                c7 = new PdfPCell(new Paragraph("Inputs", fontC8BW))
                {
                    Colspan = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = BGClearGray
                };
                t4.AddCell(c7);

                c2 = new PdfPCell(new Paragraph("Nombre Input", fontC8B));
                c3 = new PdfPCell(new Paragraph("Descripción", fontC8B));
                c4 = new PdfPCell(new Paragraph("Trazabilidad", fontC8B));
                c5 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
                c6 = new PdfPCell(new Paragraph("Calificación", fontC8B));
                t4.AddCell(c2);
                t4.AddCell(c3);
                t4.AddCell(c4);
                t4.AddCell(c5);
                t4.AddCell(c6);
            }


            foreach (var input in inputs)
            {
                c2 = new PdfPCell(new Paragraph(input.nb_input, fontC7));
                c3 = new PdfPCell(new Paragraph(input.ds_input, fontC7));
                c4 = new PdfPCell(new Paragraph(input.tr_input, fontC7));
                c5 = new PdfPCell(new Paragraph(input.observaciones, fontC7));
                c6 = new PdfPCell(new Paragraph(input.c_calificacion != null ? input.c_calificacion.nb_calificacion : "", fontC7));
                t4.AddCell(c2);
                t4.AddCell(c3);
                t4.AddCell(c4);
                t4.AddCell(c5);
                t4.AddCell(c6);
            }


            c2 = new PdfPCell(new Paragraph("Campo", fontC8B));
            c3 = new PdfPCell(new Paragraph("Óriginal", fontC8B));
            c4 = new PdfPCell(new Paragraph("Revisión", fontC8B));
            c5 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c6 = new PdfPCell(new Paragraph("Calificación", fontC8B));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);
            t4.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Naturaleza del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.c_naturaleza_control.cl_naturaleza_control + " - " + control.c_naturaleza_control.nb_naturaleza_control, fontC7));
            c4 = new PdfPCell(new Paragraph(edcNaturaleza, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.edc_obs_2, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion23 != null ? Model.c_calificacion23.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);
            t4.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Tipología del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.c_tipologia_control.cl_tipologia_control + " - " + control.c_tipologia_control.nb_tipologia_control, fontC7));
            c4 = new PdfPCell(new Paragraph(edcTipologia, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.edc_obs_3, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion24 != null ? Model.c_calificacion24.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);
            t4.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Tipo de Evidencia", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.c_tipo_evidencia.cl_tipo_evidencia + " - " + control.c_tipo_evidencia.nb_tipo_evidencia, fontC7));
            c4 = new PdfPCell(new Paragraph(edcTipoEvidencia, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.edc_obs_4, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion25 != null ? Model.c_calificacion25.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);
            t4.AddCell(c6);

            c2 = new PdfPCell(new Paragraph("Categoría del Control", fontC7B));
            c3 = new PdfPCell(new Paragraph(control.c_categoria_control.cl_categoria_control + " - " + control.c_categoria_control.nb_categoria_control, fontC7));
            c4 = new PdfPCell(new Paragraph(edcCategoriaControl, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.edc_obs_5, fontC7));
            c6 = new PdfPCell(new Paragraph(Model.c_calificacion26 != null ? Model.c_calificacion26.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);
            t4.AddCell(c6);

            c7 = new PdfPCell(new Paragraph("Procedimientos realizados para probar su adecuado diseño", fontC8BW))
            {
                Colspan = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = BGClearGray
            };
            t4.AddCell(c7);

            c2 = new PdfPCell(new Paragraph("Descripción de los Procedimientos", fontC8B));
            c3 = new PdfPCell(new Paragraph(Model.edc_proc_cert, fontC7));
            c3.Colspan = 2;
            c4 = new PdfPCell(new Paragraph("Calificacion", fontC8B));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion27 != null ? Model.c_calificacion27.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c3 = new PdfPCell(new Paragraph(Model.edc_obs_6, fontC7));
            c2.Colspan = 2;
            c3.Colspan = 3;
            t4.AddCell(c2);
            t4.AddCell(c3);

            c7.Phrase = new Paragraph("Factores del COSO", fontC7BW);
            t4.AddCell(c7);

            c2 = new PdfPCell(new Paragraph("Nombre Factor", fontC8B));
            c3 = new PdfPCell(new Paragraph("Descripción", fontC8B)) { Colspan = 2 };
            c4 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c5 = new PdfPCell(new Paragraph("Calificación", fontC8B));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("1 - Idoneidad del propósito del control y su correlación con el riesgo asociado y aseveración", fontC7));
            c3 = new PdfPCell(new Paragraph(Model.edc_fc1, fontC7)) { Colspan = 2 };
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_7, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion28 != null ? Model.c_calificacion28.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("2 - Competencia y autoridad del responsable del control", fontC7));
            c3 = new PdfPCell(new Paragraph(Model.edc_fc2, fontC7)) { Colspan = 2 };
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_8, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion29 != null ? Model.c_calificacion29.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("3 - Frecuencia y consistencia con la cual se realiza el control", fontC7));
            c3 = new PdfPCell(new Paragraph(Model.edc_fc3, fontC7)) { Colspan = 2 };
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_9, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion30 != null ? Model.c_calificacion30.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("4 - Nivel de agregación y predictibilidad", fontC7));
            c3 = new PdfPCell(new Paragraph(Model.edc_fc4, fontC7)) { Colspan = 2 };
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_10, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion31 != null ? Model.c_calificacion31.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("5 - Criterios de Investigación (umbrales) y procesos de seguimiento predictibilidad", fontC7));
            c3 = new PdfPCell(new Paragraph(Model.edc_fc5, fontC7)) { Colspan = 2 };
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_11, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion32 != null ? Model.c_calificacion32.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);


            c7.Phrase = new Paragraph("Conclusiones", fontC7BW);
            t4.AddCell(c7);

            c2 = new PdfPCell(new Paragraph("Calificación del control en cuanto a su diseño", fontC7B)) { Colspan = 3 };
            c3 = new PdfPCell(new Paragraph(rrData[0], fontC7)) { Colspan = 2 };
            t4.AddCell(c2);
            t4.AddCell(c3);

            c2 = new PdfPCell(new Paragraph("¿El diseño del control es correcto?", fontC7B)) { Colspan = 2 };
            c3 = new PdfPCell(new Paragraph(Model.edc_diseño_efectivo ? "Si" : "No", fontC7));
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_12, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion33 != null ? Model.c_calificacion33.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("Conclusión de efectividad del diseño", fontC7B)) { Colspan = 2 };
            c3 = new PdfPCell(new Paragraph(Model.edc_efectividad, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.edc_obs_13, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion34 != null ? Model.c_calificacion34.nb_calificacion : "", fontC7));
            t4.AddCell(c2);
            t4.AddCell(c3);
            t4.AddCell(c4);
            t4.AddCell(c5);

            doc.Add(t4);
            #endregion

            #region Evaluación de la eficacia operativa del Control
            doc.Add(new Paragraph("\n\n"));
            PdfPTable t5 = new PdfPTable(4);
            t5.TotalWidth = 530f;
            t5.LockedWidth = true;

            //Encabezado
            c1.Phrase = new Phrase("Evaluación de la eficacia operativa del Control", fontC9BW);
            t5.AddCell(c1);

            if (pruebas.Count > 0)
            {
                c7.Phrase = new Paragraph("Pruebas", fontC8BW);
                t5.AddCell(c7);

                c2 = new PdfPCell(new Paragraph("Descripción", fontC8B)) { Colspan = 2 };
                c3 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
                c4 = new PdfPCell(new Paragraph("Calificación", fontC8B));
                t5.AddCell(c2);
                t5.AddCell(c3);
                t5.AddCell(c4);
            }

            foreach (var prueba in pruebas)
            {
                c2 = new PdfPCell(new Paragraph(prueba.ds_procedimiento_prueba, fontC7)) { Colspan = 2 };
                c3 = new PdfPCell(new Paragraph(prueba.observaciones, fontC7));
                c4 = new PdfPCell(new Paragraph(prueba.c_calificacion != null ? prueba.c_calificacion.nb_calificacion : "", fontC7));
                t5.AddCell(c2);
                t5.AddCell(c3);
                t5.AddCell(c4);
            }


            c7.Phrase = new Paragraph("Conclusiones", fontC7BW);
            t5.AddCell(c7);

            c2 = new PdfPCell(new Paragraph("Calificación del control en cuanto a su efectividad", fontC7B)) { Colspan = 2 };
            c3 = new PdfPCell(new Paragraph(rrData[1], fontC7)) { Colspan = 2 };
            t5.AddCell(c2);
            t5.AddCell(c3);

            c2 = new PdfPCell(new Paragraph("¿El diseño del control es eficaz?", fontC7B));
            c3 = new PdfPCell(new Paragraph(Model.eeo_diseño_efectivo ? "Si" : "No", fontC7));
            c4 = new PdfPCell(new Paragraph(Model.eeo_obs_1, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion37 != null ? Model.c_calificacion37.nb_calificacion : "", fontC7));
            t5.AddCell(c2);
            t5.AddCell(c3);
            t5.AddCell(c4);
            t5.AddCell(c5);

            c2 = new PdfPCell(new Paragraph("Conclusión de efectividad eficacia operativa", fontC7B));
            c3 = new PdfPCell(new Paragraph(Model.eeo_efectividad, fontC7));
            c4 = new PdfPCell(new Paragraph(Model.eeo_obs_2, fontC7));
            c5 = new PdfPCell(new Paragraph(Model.c_calificacion38 != null ? Model.c_calificacion38.nb_calificacion : "", fontC7));
            t5.AddCell(c2);
            t5.AddCell(c3);
            t5.AddCell(c4);
            t5.AddCell(c5);


            doc.Add(t5);
            #endregion

            #region Tabla de conclusion general
            doc.Add(new Paragraph("\n\n"));
            PdfPTable t6 = new PdfPTable(4);
            t6.TotalWidth = 530f;
            t6.LockedWidth = true;

            //Encabezado
            c1.Phrase = new Phrase("Conclusión General", fontC9BW);
            t6.AddCell(c1);

            c2 = new PdfPCell(new Paragraph("Conclusiones", fontC8B)) { Colspan = 2 };
            c3 = new PdfPCell(new Paragraph("Observaciones", fontC8B));
            c4 = new PdfPCell(new Paragraph("Estatus de la Revisión", fontC8B));
            t6.AddCell(c2);
            t6.AddCell(c3);
            t6.AddCell(c4);

            c2 = new PdfPCell(new Phrase(Model.cg_conclusion, fontC7)) { Colspan = 2 };
            c3 = new PdfPCell(new Phrase(Model.cg_obs_1, fontC7));
            c4 = new PdfPCell(new Phrase(Model.c_calificacion_revision != null ? Model.c_calificacion_revision.nb_calificacion_revision : "Sin Calificar", fontC7));
            t6.AddCell(c2);
            t6.AddCell(c3);
            t6.AddCell(c4);

            doc.Add(t6);
            #endregion

            doc.Close();

            return os.GetBuffer();
        }
        #endregion

        #region Reporte General Revisión Control
        public static byte[] RGRev(RGRevViewModel model)
        {
            RenewDB();

            //generación del pdf
            MemoryStream os = new MemoryStream();

            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);

            //ph1.IndentationLeft = 15 * ident;

            #region Variables auxiliares

            var revisiones = db.k_revision_control.ToList();
            List<string> DireccionesGenerales = new List<string>();

            foreach (var rev in revisiones)
            {
                if (!DireccionesGenerales.Contains(rev.rc_dir_general)) DireccionesGenerales.Add(rev.rc_dir_general);
            }

            bool showDG = model.en != null || model.mp != null || model.pr != null || model.sp != null || model.tarea != null;
            bool showRR = model.codR != null || model.evR != null;
            bool showRC = model.codC != null || model.actC != null || model.eviC != null || model.arC != null || model.respC != null;
            bool showED = model.inputs != null || model.procPD != null || model.concD != null || model.concE != null;
            bool showCG = model.concG != null || model.comentR != null;

            #endregion


            ph1 = new Paragraph("Reporte General de Revisión de Controles", fontC10B);
            ph1.Alignment = Element.ALIGN_CENTER;
            doc.Add(ph1);


            ph1 = new Paragraph("\n" + model.entrada, fontC9);
            doc.Add(ph1);

            foreach (var dg in DireccionesGenerales)
            {
                if (model.direccionesG == null || model.direccionesG.Contains(dg))
                {
                    ch1 = new Chunk("\nDirección General Responsable: ", fontC9B);
                    ch2 = new Chunk(dg, fontC9);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    doc.Add(ph2);

                    var revsInDG = revisiones.Where(r => r.rc_dir_general == dg).ToList();

                    // para cada Revisión
                    foreach (var r in revsInDG)
                    {
                        ch1 = new Chunk("Control: ", fontC7B);
                        ch2 = new Chunk(r.k_control.relacion_control, fontC8);
                        ph1 = new Paragraph();
                        ph1.Add(ch1);
                        ph1.Add(ch2);
                        ph1.IndentationLeft = 15;
                        doc.Add(ph1);

                        #region Datos Generales
                        if (showDG)
                        {
                            ph1 = new Paragraph("Datos Generales", fontC8B);
                            ph1.IndentationLeft = 30;
                            doc.Add(ph1);

                            if (model.en != null)
                            {
                                ch1 = new Chunk("Entidad: ", fontC7B);
                                ch2 = new Chunk(r.dg_entidad, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.mp != null)
                            {
                                ch1 = new Chunk("Macro Proceso: ", fontC7B);
                                ch2 = new Chunk(r.dg_marco_proceso, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.pr != null)
                            {
                                ch1 = new Chunk("Proceso: ", fontC7B);
                                ch2 = new Chunk(r.dg_proceso, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.sp != null)
                            {
                                ch1 = new Chunk("Sub Proceso: ", fontC7B);
                                ch2 = new Chunk(r.dg_proceso, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.tarea != null)
                            {
                                ch1 = new Chunk("Tarea: ", fontC7B);
                                ch2 = new Chunk(r.dg_tarea, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                        }
                        #endregion

                        #region Resumen del Riesgo
                        if (showRR)
                        {
                            ph1 = new Paragraph("Resumen del Riesgo", fontC8B);
                            ph1.IndentationLeft = 30;
                            doc.Add(ph1);

                            if (model.codR != null)
                            {
                                ch1 = new Chunk("Código del Riesgo: ", fontC7B);
                                ch2 = new Chunk(r.k_control.k_riesgo.First().nb_riesgo, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.evR != null)
                            {
                                ch1 = new Chunk("Evento: ", fontC7B);
                                ch2 = new Chunk(r.rr_evento_riesgo, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }
                        }
                        #endregion

                        #region Resumen del Control
                        if (showRC)
                        {
                            ph1 = new Paragraph("Resumen del Control", fontC8B);
                            ph1.IndentationLeft = 30;
                            doc.Add(ph1);

                            if (model.codC != null)
                            {
                                ch1 = new Chunk("Código del Control: ", fontC7B);
                                ch2 = new Chunk(r.k_control.relacion_control, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.actC != null)
                            {
                                ch1 = new Chunk("Actividad: ", fontC7B);
                                ch2 = new Chunk(r.rc_actividad_control, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.eviC != null)
                            {
                                ch1 = new Chunk("Evidencia: ", fontC7B);
                                ch2 = new Chunk(r.rc_evidencia_control, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.arC != null)
                            {
                                ch1 = new Chunk("Área responsable: ", fontC7B);
                                ch2 = new Chunk(r.rc_area, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.respC != null)
                            {
                                ch1 = new Chunk("Responsable: ", fontC7B);
                                ch2 = new Chunk(r.rc_responsable, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }
                        }
                        #endregion

                        #region Evaluación del Diseño

                        if (showED)
                        {
                            ph1 = new Paragraph("Evaluación del Diseño", fontC8B);
                            ph1.IndentationLeft = 30;
                            doc.Add(ph1);

                            if (model.inputs != null)
                            {
                                ch1 = new Chunk("Información de Entrada: ", fontC7B);
                                ch2 = new Chunk(r.edc_informacion_inputs, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.concD != null)
                            {
                                ch1 = new Chunk("Conclusión del Diseño: ", fontC7B);
                                ch2 = new Chunk(r.edc_efectividad, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.concE != null)
                            {
                                ch1 = new Chunk("Conclusión de la Efectividad: ", fontC7B);
                                ch2 = new Chunk(r.eeo_efectividad, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }
                        }
                        #endregion

                        if (showCG)
                        {
                            ph1 = new Paragraph("Conlusión General", fontC8B);
                            ph1.IndentationLeft = 30;
                            doc.Add(ph1);

                            if (model.concG != null)
                            {
                                ch1 = new Chunk("Conclusión General: ", fontC7B);
                                ch2 = new Chunk(r.cg_conclusion, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }

                            if (model.comentR != null)
                            {
                                ch1 = new Chunk("Comentarios de Revisión: ", fontC7B);
                                ch2 = new Chunk(r.cg_comentarios_revision, fontC7);
                                ph2 = new Paragraph();
                                ph2.Add(ch1);
                                ph2.Add(ch2);
                                ph2.IndentationLeft = 45;
                                doc.Add(ph2);
                            }
                        }
                    }
                }
            }

            ph1 = new Paragraph("\n" + model.salida, fontC8);
            doc.Add(ph1);

            doc.Close();


            return os.GetBuffer();
        }


        #endregion

        #region Contenido Normatividad
        public static byte[] ContenidoNormatiidad(c_contenido_normatividad rootContent)
        {
            RenewDB();

            //generación del pdf
            MemoryStream os = new MemoryStream();

            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);

            writeSoons(doc, 0, rootContent);

            doc.Close();

            return os.GetBuffer();
        }


        private static void writeSoons(Document doc, int ident, c_contenido_normatividad rootContent)
        {
            ph1 = new Paragraph(rootContent.cl_contenido_normatividad, fontC10B);
            ph1.IndentationLeft = 15 * ident;
            doc.Add(ph1);
            ph1 = new Paragraph(rootContent.ds_contenido_normatividad, fontC8);
            ph1.IndentationLeft = 15 * ident;
            ph1.Alignment = Element.ALIGN_JUSTIFIED;
            Chunk ch1 = new Chunk("\nRuta: ", fontC6B);
            Chunk ch2 = new Chunk(Utilidades.getRuta(rootContent), fontC6);
            ph1.Add(ch1);
            ph1.Add(ch2);
            doc.Add(ph1);

            var hijos = rootContent.c_contenido_normatividad1.OrderBy(c => c.cl_contenido_normatividad).ToList();

            foreach (var hijo in hijos)
            {
                writeSoons(doc, ident + 1, hijo);
            }

            return;
        }
        #endregion

        #region Informe Auditoria
        public static byte[] InfAud(InfAudViewModel model)
        {
            RenewDB();

            //generación del pdf
            MemoryStream os = new MemoryStream();

            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);

            //ph1.IndentationLeft = 15 * ident;

            #region Variables auxiliares

            var kAuditoria = db.k_auditoria.Find(model.idd_auditoria);
            var auditoria = kAuditoria.c_auditoria;
            var entidad = auditoria.c_entidad;
            var rating = kAuditoria.id_rating_auditoria == null ? new c_rating_auditoria() : kAuditoria.c_rating_auditoria;
            var area = auditoria.c_area;
            var solicitante = auditoria.c_solicitante_auditoria;

            List<c_campo_auditoria> cePA = new List<c_campo_auditoria>();
            List<c_campo_auditoria> ceEJ = new List<c_campo_auditoria>();
            List<c_campo_auditoria> ceINF = new List<c_campo_auditoria>();

            if (model.campoe != null && model.idcampoe != null)
            {
                var len = model.campoe.Length;

                for (int i = 0; i < len; i++)
                {
                    var idCE = model.idcampoe[i];
                    var valCE = model.campoe[i];

                    var cCampo = db.c_campo_auditoria.Find(idCE);

                    if (valCE == "on")
                    {
                        if (!cCampo.aparece_en_informe && !cCampo.aparece_en_planeacion)
                        {
                            cePA.Add(cCampo);
                        }
                        else if (cCampo.aparece_en_planeacion)
                        {
                            ceEJ.Add(cCampo);
                        }
                        else
                        {
                            ceINF.Add(cCampo);
                        }
                    }

                }

            }




            bool showPA = model.en != null ||
                model.ar != null ||
                model.tp != null ||
                model.sl != null ||
                model.noaud != null ||
                model.año != null ||
                model.feinp != null ||
                model.fefip != null ||
                cePA.Count > 0;


            bool showEJ = model.ant != null ||
                model.datosr != null ||
                model.responsabler != null ||
                model.auditoresp != null ||
                model.feinr != null ||
                model.fefir != null ||
                ceEJ.Count > 0;

            bool showINF = ceINF.Count > 0;

            #endregion

            if (model.type == "informe")
            {
                ph1 = new Paragraph("Informe de la auditoría " + kAuditoria.c_auditoria.nb_auditoria, fontC10B);
            }
            else
            {
                ph1 = new Paragraph("Planeación de la auditoría " + kAuditoria.c_auditoria.nb_auditoria, fontC10B);
            }
            ph1.Alignment = Element.ALIGN_CENTER;
            doc.Add(ph1);


            //ch1 = new Chunk("\nDirección General Responsable: ", fontC9B);
            //ch2 = new Chunk(dg, fontC9);
            //ph2 = new Paragraph();
            //ph2.Add(ch1);
            //ph2.Add(ch2);
            //        doc.Add(ph2);

            //        var revsInDG = revisiones.Where(r => r.rc_dir_general == dg).ToList();

            //// para cada Revisión
            //foreach (var r in revsInDG)
            //{
            //ch1 = new Chunk("Control: ", fontC7B);
            //ch2 = new Chunk(r.k_control.relacion_control, fontC8);
            //ph1 = new Paragraph();
            //ph1.Add(ch1);
            //ph1.Add(ch2);
            //ph1.IndentationLeft = 15;
            //doc.Add(ph1);

            #region Planeacion Anual
            if (showPA)
            {
                //ph1 = new Paragraph("Planeación anual", fontC8B);
                //ph1.IndentationLeft = 30;
                //doc.Add(ph1);

                if (model.en != null)
                {
                    ch1 = new Chunk("Entidad: ", fontC7B);
                    ch2 = new Chunk(entidad.nb_entidad, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.ar != null)
                {
                    ch1 = new Chunk("Área: ", fontC7B);
                    ch2 = new Chunk(area.nb_area, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.tp != null)
                {
                    ch1 = new Chunk("Tipo: ", fontC7B);
                    ch2 = new Chunk(auditoria.es_regulada ? "Regulada" : "Normal", fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.sl != null)
                {
                    ch1 = new Chunk("Solicitante: ", fontC7B);
                    ch2 = new Chunk(solicitante.nb_solicitante_auditoria, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.noaud != null)
                {
                    ch1 = new Chunk("Número de auditores: ", fontC7B);
                    ch2 = new Chunk(kAuditoria.no_auditores.ToString(), fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.año != null)
                {
                    ch1 = new Chunk("Año: ", fontC7B);
                    ch2 = new Chunk(kAuditoria.fe_inicial_planeada.Value.Year.ToString(), fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.feinp != null)
                {
                    ch1 = new Chunk("Fecha de inicio planeada: ", fontC7B);
                    ch2 = new Chunk(string.Format("{0:dd/MM/yyyy}", kAuditoria.fe_inicial_planeada), fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.fefip != null)
                {
                    ch1 = new Chunk("Fecha de terminación planeada: ", fontC7B);
                    ch2 = new Chunk(string.Format("{0:dd/MM/yyyy}", kAuditoria.fe_final_planeada), fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }


                if (cePA.Count > 0)
                {
                    //ph1 = new Paragraph("Campos extra", fontC8B);
                    //ph1.IndentationLeft = 45;
                    //doc.Add(ph1);

                    foreach (var ce in cePA)
                    {
                        var kce = db.k_campo_auditoria.FirstOrDefault(k => k.idd_auditoria == kAuditoria.idd_auditoria && k.id_campo_auditoria == ce.id_campo_auditoria);

                        string val = kce == null ? "" : kce.valor;

                        ch1 = new Chunk(ce.nb_campo + ": ", fontC7B);
                        ch2 = new Chunk(val, fontC7);
                        ph2 = new Paragraph();
                        ph2.Add(ch1);
                        ph2.Add(ch2);
                        ph2.IndentationLeft = 30;
                        doc.Add(ph2);
                    }
                }
            }
            #endregion

            #region Planeacion detallada
            if (showEJ)
            {
                //ph1 = new Paragraph("Planeación detallada", fontC8B);
                //ph1.IndentationLeft = 30;
                //doc.Add(ph1);

                if (model.ant != null)
                {
                    ch1 = new Chunk("Antecedentes: ", fontC7B);
                    ch2 = new Chunk(kAuditoria.antecedentes, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.datosr != null)
                {
                    ch1 = new Chunk("Datos relevantes: ", fontC7B);
                    ch2 = new Chunk(kAuditoria.datos_relevantes, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.responsabler != null)
                {
                    ch1 = new Chunk("Responsable de revisión: ", fontC7B);
                    ch2 = new Chunk(kAuditoria.responsable_revision, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.feinr != null)
                {
                    ch1 = new Chunk("Fecha de inicio real: ", fontC7B);
                    ch2 = new Chunk(string.Format("{0:dd/MM/yyyy}", kAuditoria.fe_inicial_real), fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }

                if (model.fefir != null)
                {
                    ch1 = new Chunk("Fecha de terminación real: ", fontC7B);
                    ch2 = new Chunk(string.Format("{0:dd/MM/yyyy}", kAuditoria.fe_final_real), fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }


                if (ceEJ.Count > 0)
                {
                    ph1 = new Paragraph("Campos extra", fontC8B);
                    ph1.IndentationLeft = 30;
                    doc.Add(ph1);

                    foreach (var ce in ceEJ)
                    {
                        var kce = db.k_campo_auditoria.FirstOrDefault(k => k.idd_auditoria == kAuditoria.idd_auditoria && k.id_campo_auditoria == ce.id_campo_auditoria);

                        string val = kce == null ? "" : kce.valor;

                        ch1 = new Chunk(ce.nb_campo + ": ", fontC7B);
                        ch2 = new Chunk(val, fontC7);
                        ph2 = new Paragraph();
                        ph2.Add(ch1);
                        ph2.Add(ch2);
                        ph2.IndentationLeft = 30;
                        doc.Add(ph2);
                    }
                }
            }
            #endregion

            #region Informe

            if (model.rat != null)
            {


                ch1 = new Chunk("Rating: ", fontC7B);
                ch2 = new Chunk(rating.nb_rating_auditoria, fontC7);
                ph2 = new Paragraph();
                ph2.Add(ch1);
                ph2.Add(ch2);
                ph2.IndentationLeft = 30;
                doc.Add(ph2);
            }

            if (ceINF.Count > 0)
            {
                //ph1 = new Paragraph("Datos del informe", fontC8B);
                //ph1.IndentationLeft = 30;
                //doc.Add(ph1);

                foreach (var ce in ceINF)
                {
                    var kce = db.k_campo_auditoria.FirstOrDefault(k => k.idd_auditoria == kAuditoria.idd_auditoria && k.id_campo_auditoria == ce.id_campo_auditoria);

                    string val = kce == null ? "" : kce.valor;

                    ch1 = new Chunk(ce.nb_campo + ": ", fontC7B);
                    ch2 = new Chunk(val, fontC7);
                    ph2 = new Paragraph();
                    ph2.Add(ch1);
                    ph2.Add(ch2);
                    ph2.IndentationLeft = 30;
                    doc.Add(ph2);
                }
            }


            #endregion

            doc.Close();


            return os.GetBuffer();
        }
        #endregion

        #region Otros
        static private void RenewDB()
        {
            db = new SICIEntities();
            return;
        }
        #endregion

    }
}