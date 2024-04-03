using iTextSharp.text;
using iTextSharp.text.pdf;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using static iTextSharp.text.Font;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Contest", ModuleCode = "MSICI006")]
    [CustomErrorHandler]
    public class ContestacionOficioController : Controller
    {
        private SICIEntities db = new SICIEntities();


        public ActionResult Index()
        {
            var User = (IdentityPersonalizado)HttpContext.User.Identity;
            int id = User.Id_usuario;
            var usuario = db.c_usuario.Find(id);

            var oficios = Utilidades.Utilidades.RTCObject(usuario, db, "k_objeto").Cast<k_objeto>().ToList();

            oficios = oficios.Where(o => o.tipo_objeto == 1 || o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();

            if (User.Es_super_usuario) ViewBag.su = 1;

            return View(oficios);
        }

        public ActionResult ContestarOficio(int id)
        {
            var model = new r_contestacion_oficio();
            var oficio = db.k_objeto.Find(id);

            model.id_objeto = id;
            ViewBag.Oficio = oficio;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult ContestarOficio(r_contestacion_oficio model)
        {
            var oficio = db.k_objeto.Find(model.id_objeto);
            oficio.fe_contestacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(oficio).State = EntityState.Modified;
                db.r_contestacion_oficio.Add(model);
                db.SaveChanges();

                Utilidades.Utilidades.refreshNotifCount((int)oficio.id_responsable);

                if (oficio.tipo_objeto == 1)
                    Utilidades.Utilidades.removeRow(3, oficio.id_objeto, (int)oficio.id_responsable);
                else
                    Utilidades.Utilidades.removeRow(4, oficio.id_objeto, (int)oficio.id_responsable);
                return RedirectToAction("Index");
            }

            ViewBag.Oficio = oficio;
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var oficio = db.k_objeto.Find(id);
            var model = oficio.r_contestacion_oficio.First();

            ViewBag.Oficio = oficio;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(r_contestacion_oficio model)
        {
            var oficio = db.k_objeto.Find(model.id_objeto);
            //Si se edita la respuesta, la fecha de respuesta cambia
            oficio.fe_contestacion = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(oficio).State = EntityState.Modified;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Oficio = oficio;

            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            var model = db.r_contestacion_oficio.Find(id);
            var oficio = model.k_objeto;

            ViewBag.Oficio = oficio;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete(int id)
        {
            var model = db.r_contestacion_oficio.Find(id);
            var oficio = model.k_objeto;
            oficio.fe_contestacion = null;
            oficio.nb_archivo_2 = null;

            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Informes-Oficios/a2-" + oficio.id_objeto);
            System.IO.File.Delete(path);

            db.Entry(oficio).State = EntityState.Modified;
            db.r_contestacion_oficio.Remove(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        #region Visuzalizar PDF
        public FileResult DisplayPDF(int id)
        {
            //Obtenemos las incidencias ligadas al objeto así como su contestación
            var objeto = db.k_objeto.Find(id);
            var contestacion = objeto.r_contestacion_oficio.First();
            var incidencias = objeto.k_incidencia.ToList();

            var nb_obj = objeto.tipo_objeto == 1 ? "No. Oficio" : "Nombre de Informe";
            var ofiOinf = objeto.tipo_objeto == 1 ? "Oficio" : "Informe";
            var nb_autoridad = objeto.tipo_objeto == 1 ? objeto.c_origen_autoridad.cl_origen_autoridad + " - " + objeto.c_origen_autoridad.nb_origen_autoridad : "N/A";
            var nb_entidad = objeto.c_entidad.cl_entidad + " - " + objeto.c_entidad.nb_entidad;
            var fe_ven = objeto.tipo_objeto == 1 ? objeto.fe_vencimiento.ToString() : "N/A";
            var intro = contestacion.inicio_parrafo;
            var final = contestacion.final_parrafo;

            //--------------------------------    Crear PDF    ------------------------------

            //COLORES
            var colorP = new BaseColor(217, 35, 15);
            var color2 = new BaseColor(251, 141, 141);
            //COLORES

            MemoryStream os = new MemoryStream();


            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);


            Font fontH1 = new Font(null, 10, Font.BOLD);
            Font fontH1B = new Font(null, 10, Font.BOLD, BaseColor.WHITE);
            Font fontC1 = new Font(null, 10, Font.NORMAL);
            Font font = new Font(FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.WHITE);
            Font fontC = new Font(FontFamily.HELVETICA, 14, Font.BOLD);


            //217,35,15 ROJO para los titulos
            Paragraph inicio = new Paragraph("Introducción", fontC);
            Paragraph conclusion = new Paragraph("Conclusión", fontC);
            inicio.Alignment = Element.ALIGN_CENTER;
            Paragraph introP = new Paragraph(intro, fontC1);
            Paragraph conclusionP = new Paragraph(final, fontC1);
            Paragraph incTitle = new Paragraph("Incidencias", fontC);
            incTitle.Alignment = Element.ALIGN_CENTER;

            //DEFINICIN DE TABLAS
            //Tabla Datos Generales
            PdfPTable encabezado = new PdfPTable(6);
            encabezado.TotalWidth = 530f;
            encabezado.LockedWidth = true;

            PdfPCell head = new PdfPCell(new Phrase("Contestación del " + ofiOinf, font));
            head.BackgroundColor = colorP;
            head.Colspan = 6;
            head.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell t1c1 = new PdfPCell(new Phrase(nb_obj, fontH1B));
            t1c1.BackgroundColor = color2;
            PdfPCell t1c2 = new PdfPCell(new Phrase("Tipo de autoridad", fontH1B));
            t1c2.BackgroundColor = color2;
            PdfPCell t1c3 = new PdfPCell(new Phrase("Fecha del " + ofiOinf, fontH1B));
            t1c3.BackgroundColor = color2;
            PdfPCell t1c4 = new PdfPCell(new Phrase("Entidad", fontH1B));
            t1c4.BackgroundColor = color2;
            PdfPCell t1c5 = new PdfPCell(new Phrase(nb_entidad, fontC1));
            t1c5.Colspan = 3;
            PdfPCell t1c6 = new PdfPCell(new Phrase("Fecha de Vencimiento", fontH1B));
            t1c6.BackgroundColor = color2;
            PdfPCell t1c7 = new PdfPCell(new Phrase("Responsable", fontH1B));
            t1c7.BackgroundColor = color2;
            PdfPCell t1c8 = new PdfPCell(new Phrase(objeto.c_usuario.nb_usuario, fontC1));
            t1c8.Colspan = 3;
            PdfPCell t1c9 = new PdfPCell(new Phrase("Fecha de Contestación", fontH1B));
            t1c9.BackgroundColor = color2;


            //encabezado
            encabezado.AddCell(head);

            //Primera Fila
            encabezado.AddCell(t1c1);
            encabezado.AddCell(new PdfPCell(new Phrase(objeto.nb_objeto, fontC1)));
            encabezado.AddCell(t1c2);
            encabezado.AddCell(new PdfPCell(new Phrase(nb_autoridad, fontC1)));
            encabezado.AddCell(t1c3);
            encabezado.AddCell(new PdfPCell(new Phrase(objeto.fe_alta.ToString(), fontC1)));

            //Segunda Fila
            encabezado.AddCell(t1c4);
            encabezado.AddCell(t1c5);
            encabezado.AddCell(t1c6);
            encabezado.AddCell(new PdfPCell(new Phrase(fe_ven, fontC1)));

            //tercer fila
            encabezado.AddCell(t1c7);
            encabezado.AddCell(t1c8);
            encabezado.AddCell(t1c9);
            encabezado.AddCell(new PdfPCell(new Phrase(objeto.fe_contestacion.ToString(), fontC1)));
            // **********************  Añadir Elementos creados ***********************  //
            //Añadir elementos
            doc.Add(encabezado);
            doc.Add(new Paragraph(".", font));
            doc.Add(inicio);
            doc.Add(introP);
            doc.Add(incTitle);
            doc.Add(new Paragraph(".", font));

            // ************************  Tabla de incidencias *************************  //
            foreach (var incidencia in incidencias)
            {
                PdfPTable tableInc = new PdfPTable(6);
                tableInc.TotalWidth = 530f;
                tableInc.LockedWidth = true;

                var clasificacion = incidencia.c_clasificacion_incidencia.cl_clasificacion_incidencia + " - " + incidencia.c_clasificacion_incidencia.nb_clasificacion_incidencia;
                var lvl = incidencia.lvl_5 ?? incidencia.lvl_4 ?? incidencia.lvl_3 ?? incidencia.lvl_2 ?? incidencia.lvl_1 ?? "N/A";
                var respuesta = incidencia.r_respuesta.First();

                PdfPCell t2c1 = new PdfPCell(new Phrase("Responsable", fontH1B));
                t2c1.BackgroundColor = color2;
                PdfPCell t2c2 = new PdfPCell(new Phrase("¿Requiere Plan de Rem.?", fontH1B));
                t2c2.BackgroundColor = color2;
                PdfPCell t2c3 = new PdfPCell(new Phrase("ID Incidencia", fontH1B));
                t2c3.BackgroundColor = color2;
                PdfPCell t2c4 = new PdfPCell(new Phrase("Clasificación", fontH1B));
                t2c4.BackgroundColor = color2;
                PdfPCell t2c5 = new PdfPCell(new Phrase("Nivel", fontH1B));
                t2c5.BackgroundColor = color2;
                PdfPCell t2c6 = new PdfPCell(new Phrase("Fecha de Respuesta", fontH1B));
                t2c6.BackgroundColor = color2;

                PdfPCell t2c7 = new PdfPCell(new Phrase("Descripción", fontH1B));
                t2c7.BackgroundColor = color2;
                t2c7.Colspan = 6;
                t2c7.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell t2c8 = new PdfPCell(new Phrase(incidencia.ds_incidencia, fontH1B));
                t2c8.Colspan = 6;
                PdfPCell t2c9 = new PdfPCell(new Phrase("Respuesta", fontH1B));
                t2c9.BackgroundColor = color2;
                t2c9.Colspan = 6;
                t2c9.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell t2c10 = new PdfPCell(new Phrase(respuesta.contestacion, fontC1));
                t2c10.Colspan = 6;
                PdfPCell t2c11 = new PdfPCell(new Phrase("Observaciones", fontH1B));
                t2c11.BackgroundColor = color2;
                t2c11.Colspan = 6;
                t2c11.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell t2c12 = new PdfPCell(new Phrase(respuesta.observaciones ?? "", fontC1));
                t2c12.Colspan = 6;

                //agregar a la tabla
                tableInc.AddCell(t2c1);
                tableInc.AddCell(new PdfPCell(new Phrase(incidencia.c_usuario.nb_usuario, fontC1)));
                tableInc.AddCell(t2c2);
                tableInc.AddCell(new PdfPCell(new Phrase(incidencia.requiere_plan ? "Si" : "No", fontC1)));
                tableInc.AddCell(t2c3);
                tableInc.AddCell(new PdfPCell(new Phrase(incidencia.id_incidencia.ToString(), fontC1)));
                tableInc.AddCell(t2c4);
                tableInc.AddCell(new PdfPCell(new Phrase(clasificacion, fontC1)));
                tableInc.AddCell(t2c5);
                tableInc.AddCell(new PdfPCell(new Phrase(lvl, fontC1)));
                tableInc.AddCell(t1c6);
                tableInc.AddCell(new PdfPCell(new Phrase(respuesta.fe_solucion.ToString(), fontC1)));
                tableInc.AddCell(t2c7);
                tableInc.AddCell(t2c8);
                tableInc.AddCell(t2c9);
                tableInc.AddCell(t2c10);
                tableInc.AddCell(t2c11);
                tableInc.AddCell(t2c12);

                doc.Add(tableInc);
                doc.Add(new Paragraph(".", font));
            }

            doc.Add(new Paragraph(".", font));
            doc.Add(conclusion);
            doc.Add(conclusionP);

            doc.Close();
            return File(os.GetBuffer(), "application/pdf");
        }


        #endregion

        #region Visuzalizar Informe
        public FileResult DisplayInforme(int id)
        {
            //Obtenemos las incidencias ligadas al objeto así como su contestación
            var objeto = db.k_objeto.Find(id);
            var contestacion = objeto.r_contestacion_oficio.First();
            var incidencias = objeto.k_incidencia.ToList();


            //--------------------------------    Crear PDF    ------------------------------

            MemoryStream os = new MemoryStream();


            //Configuración del 
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            doc.Open();
            Rectangle rec2 = new Rectangle(PageSize.A4);
            doc.SetPageSize(rec2);


            Font fontH1 = new Font(null, 10, Font.BOLD);
            Font fontH1B = new Font(null, 10, Font.BOLD, BaseColor.WHITE);
            Font fontC1 = new Font(null, 10, Font.NORMAL);
            Font font = new Font(FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.WHITE);
            Font fontC = new Font(FontFamily.HELVETICA, 14, Font.BOLD);


            Paragraph nb_objeto = new Paragraph(objeto.tipo_objeto == 1 ? "Oficio: " + objeto.nb_objeto : "Informe: " + objeto.nb_objeto, fontC);
            nb_objeto.Alignment = Element.ALIGN_CENTER;
            Paragraph intro = new Paragraph(contestacion.inicio_parrafo, fontC1);
            intro.Alignment = Element.ALIGN_JUSTIFIED;
            Paragraph fin = new Paragraph(contestacion.final_parrafo, fontC1);
            fin.Alignment = Element.ALIGN_JUSTIFIED;


            doc.Add(nb_objeto);
            doc.Add(new Paragraph(".", font));
            doc.Add(intro);
            doc.Add(new Paragraph(".", font));

            int counter = 1;
            foreach (var incidencia in incidencias)
            {
                var respuesta = incidencia.r_respuesta.First();
                var obs = "Incidencia " + (counter++) + ": " + incidencia.ds_incidencia;
                var resp = "Respuesta: " + respuesta.contestacion;

                Paragraph obsP = new Paragraph(obs, fontC1);
                Paragraph respP = new Paragraph(resp, fontC1);
                obsP.Alignment = Element.ALIGN_JUSTIFIED;
                respP.Alignment = Element.ALIGN_JUSTIFIED;
                doc.Add(obsP);
                doc.Add(respP);
                doc.Add(new Paragraph(".", font));
            }

            doc.Add(fin);
            doc.Close();
            return File(os.GetBuffer(), "application/pdf");
        }


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
