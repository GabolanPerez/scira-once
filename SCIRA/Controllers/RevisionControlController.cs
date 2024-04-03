using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RevisionControl", ModuleCode = "MSICI011")]
    [CustomErrorHandler]
    public class RevisionControlController : Controller
    {
        private SICIEntities db = new SICIEntities();

        #region Index
        public ActionResult Index()
        {
            var model = db.k_control.Where(c => !c.tiene_accion_correctora).ToList();

            //no añadir controles que provengan de un MG
            model = model.Where(c => c.c_sub_proceso.c_proceso.c_macro_proceso.cl_macro_proceso.Substring(0, 2) != "MG").ToList();

            return View(model);
        }

        public ActionResult Revisiones(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            k_control control = db.k_control.Find(id);
            if (control == null)
            {
                return HttpNotFound();
            }

            ViewBag.control = control;

            return View(control.k_revision_control.OrderBy(r => r.dg_fe_revision));
        }
        #endregion

        #region Create
        public ActionResult Create(int id)
        {
            k_control control = db.k_control.Find(id);

            ViewBag.control = control;

            var model = new k_revision_control()
            {
                id_control = id
            };


            ViewBag.califList = Utilidades.DropDown.Calificaciones();
            ViewBag.tipologiaCL = Utilidades.DropDown.TipologiaControl();
            ViewBag.frecuenciaCL = Utilidades.DropDown.FrecuenciaControl();
            ViewBag.naturalezaCL = Utilidades.DropDown.NaturalezaControl();
            ViewBag.tipoEvidenciaCL = Utilidades.DropDown.TipoEvidenciaControl();
            ViewBag.categoriaCL = Utilidades.DropDown.CategoriaControl();
            ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS();
            ViewBag.CalificacionRevision = Utilidades.DropDown.CalificacionesRevision();


            return View(model);
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Create(k_revision_control model, string[] nb_input, string[] ds_input, string[] obs_input, string[] tr_input, int[] calif_input, string[] ds_pp, string[] obs_pp, int[] calif_pp, int[] id_aseveracion, int[] files)
        {
            if (ModelState.IsValid)
            {
                int nInputs = 0;
                int nPruebas = 0;
                int nAseveraciones = 0;

                if (nb_input != null) nInputs = nb_input.Count();
                if (ds_pp != null) nPruebas = ds_pp.Count();
                if (id_aseveracion != null) nAseveraciones = id_aseveracion.Count();

                model.dg_fe_revision = DateTime.Now;

                db.k_revision_control.Add(model);
                db.SaveChanges();


                //agregar las inputs y las pruebas

                if (nInputs > 0)
                {
                    for (int i = 0; i < nInputs; i++)
                    {
                        int? id_calif = calif_input[i] != 0 ? (int?)calif_input[i] : null;

                        var input = new a_input_rc()
                        {
                            id_revision_control = model.id_revision_control,
                            id_calificacion = id_calif,
                            nb_input = nb_input[i],
                            ds_input = ds_input[i],
                            tr_input = tr_input[i],
                            observaciones = obs_input[i]
                        };

                        db.a_input_rc.Add(input);
                    }

                    db.SaveChanges();
                }

                if (nPruebas > 0)
                {
                    for (int i = 0; i < nPruebas; i++)
                    {
                        int? id_calif = calif_pp[i] != 0 ? (int?)calif_pp[i] : null;

                        var prueba = new a_procedimiento_prueba_rc()
                        {
                            id_revision_control = model.id_revision_control,
                            id_calificacion = id_calif,
                            ds_procedimiento_prueba = ds_pp[i],
                            observaciones = obs_pp[i]
                        };

                        db.a_procedimiento_prueba_rc.Add(prueba);
                    }

                    db.SaveChanges();
                }

                if (nAseveraciones > 0)
                {
                    db = new SICIEntities();

                    model = db.k_revision_control.Find(model.id_revision_control);

                    for (int i = 0; i < nAseveraciones; i++)
                    {
                        var aseveracion = db.c_aseveracion.Find(id_aseveracion[i]);

                        model.c_aseveracion.Add(aseveracion);
                    }

                    db.SaveChanges();
                }


                //archivos

                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("Revisiones", new { id = model.id_control });
            }


            return RedirectToAction("Create", new { id = model.id_control });
            //return View(model);
        }

        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            var revision = db.k_revision_control.Find(id);
            var control = revision.k_control;

            ViewBag.control = control;


            ViewBag.califList = Utilidades.DropDown.Calificaciones();
            ViewBag.tipologiaCL = Utilidades.DropDown.TipologiaControl();
            ViewBag.frecuenciaCL = Utilidades.DropDown.FrecuenciaControl();
            ViewBag.naturalezaCL = Utilidades.DropDown.NaturalezaControl();
            ViewBag.tipoEvidenciaCL = Utilidades.DropDown.TipoEvidenciaControl();
            ViewBag.categoriaCL = Utilidades.DropDown.CategoriaControl();
            ViewBag.aseveracionesMSL = Utilidades.DropDown.AseveracionesMS(revision.c_aseveracion.Select(a => a.id_aseveracion).ToArray());
            ViewBag.CalificacionRevision = Utilidades.DropDown.CalificacionesRevision();

            return View(revision);
        }


        [HttpPost, ValidateAntiForgeryToken, NotOnlyRead]
        public ActionResult Edit(k_revision_control model, int[] id_input, string[] nb_input, string[] ds_input, string[] obs_input, string[] tr_input, int[] calif_input, int[] id_pp, string[] ds_pp, string[] obs_pp, int[] calif_pp, int[] id_aseveracion, int[] files)
        {
            if (ModelState.IsValid)
            {
                int nInputs = 0;
                int nPruebas = 0;
                int nAseveraciones = 0;

                if (nb_input != null) nInputs = nb_input.Count();
                if (ds_pp != null) nPruebas = ds_pp.Count();
                if (id_aseveracion != null) nAseveraciones = id_aseveracion.Count();

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                //Modificar y agregar inputs y pruebas

                var InputsActuales = db.a_input_rc.Where(i => i.id_revision_control == model.id_revision_control).ToList();

                if (nInputs > 0)
                {
                    for (int i = 0; i < nInputs; i++)
                    {
                        if (id_input[i] == 0) //Si el id es 0, creamos un nuevo input
                        {
                            int? id_calif = calif_input[i] != 0 ? (int?)calif_input[i] : null;

                            var input = new a_input_rc()
                            {
                                id_revision_control = model.id_revision_control,
                                id_calificacion = id_calif,
                                nb_input = nb_input[i],
                                ds_input = ds_input[i],
                                tr_input = tr_input[i],
                                observaciones = obs_input[i]
                            };

                            db.a_input_rc.Add(input);
                        }
                        else //Si el id es diferente de 0, se modifica el input
                        {
                            var input = db.a_input_rc.Find(id_input[i]);

                            int? id_calif = calif_input[i] != 0 ? (int?)calif_input[i] : null;

                            input.nb_input = nb_input[i];
                            input.ds_input = ds_input[i];
                            input.observaciones = obs_input[i];
                            input.tr_input = tr_input[i];
                            input.id_calificacion = id_calif;

                            db.Entry(input).State = EntityState.Modified;

                        }
                    }
                    //eliminamos las inputs que ya no aparezcan en la revision
                    foreach (var input in InputsActuales)
                    {
                        if (!id_input.Contains(input.id_input))
                        {
                            db.a_input_rc.Remove(input);
                        }
                    }

                    db.SaveChanges();
                }
                else
                {
                    //eliminamos todas las inputs existentes
                    foreach (var input in InputsActuales)
                    {
                        db.a_input_rc.Remove(input);
                    }
                    db.SaveChanges();
                }



                //Actualizamos las Pruebas
                var PruebasActuales = db.a_procedimiento_prueba_rc.Where(i => i.id_revision_control == model.id_revision_control).ToList();

                if (nPruebas > 0)
                {
                    for (int i = 0; i < nPruebas; i++)
                    {
                        if (id_pp[i] == 0) //Si el id es 0, creamos una nueva prueba
                        {
                            int? id_calif = calif_pp[i] != 0 ? (int?)calif_pp[i] : null;

                            var prueba = new a_procedimiento_prueba_rc()
                            {
                                id_revision_control = model.id_revision_control,
                                id_calificacion = id_calif,
                                ds_procedimiento_prueba = ds_pp[i],
                                observaciones = obs_pp[i]
                            };

                            db.a_procedimiento_prueba_rc.Add(prueba);
                        }
                        else //Si el id es diferente de 0, se modifica la prueba
                        {
                            var prueba = db.a_procedimiento_prueba_rc.Find(id_pp[i]);

                            int? id_calif = calif_pp[i] != 0 ? (int?)calif_pp[i] : null;

                            prueba.ds_procedimiento_prueba = ds_pp[i];
                            prueba.observaciones = obs_pp[i];
                            prueba.id_calificacion = id_calif;

                            db.Entry(prueba).State = EntityState.Modified;

                        }
                    }
                    //eliminamos las pruebas que ya no aparezcan en la revision
                    foreach (var prueba in PruebasActuales)
                    {
                        if (!id_pp.Contains(prueba.id_procedimiento_prueba))
                        {
                            db.a_procedimiento_prueba_rc.Remove(prueba);
                        }
                    }

                    db.SaveChanges();
                }
                else
                {
                    //eliminamos todas las pruebas existentes
                    foreach (var prueba in PruebasActuales)
                    {
                        db.a_procedimiento_prueba_rc.Remove(prueba);
                    }
                    db.SaveChanges();
                }


                //Actualizar las aseveraciones
                db = new SICIEntities();
                model = db.k_revision_control.Find(model.id_revision_control);
                model.c_aseveracion.Clear();
                db.SaveChanges();

                if (nAseveraciones > 0)
                {
                    for (int i = 0; i < nAseveraciones; i++)
                    {
                        var aseveracion = db.c_aseveracion.Find(id_aseveracion[i]);

                        model.c_aseveracion.Add(aseveracion);
                    }

                    db.SaveChanges();
                }

                //archivos

                if (files != null)
                {
                    foreach (int file in files)
                    {
                        c_archivo archivo = db.c_archivo.Find(file);

                        model.c_archivo.Add(archivo);
                    }

                    db.SaveChanges();
                }


                return RedirectToAction("Revisiones", new { id = model.id_control });
            }



            return RedirectToAction("Revisiones", new { id = model.id_control });
        }



        #endregion

        #region Delete
        public ActionResult Delete(int id)
        {
            var revision = db.k_revision_control.Find(id);

            return View(revision);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            var revision = db.k_revision_control.Find(id);
            var id_c = revision.id_control;

            Utilidades.DeleteActions.DeleteRevisionControlObjects(revision, db);

            db.k_revision_control.Remove(revision);
            db.SaveChanges();

            return RedirectToAction("Revisiones", new { id = id_c });
        }
        #endregion

        #region Generacion de Documentos
        public FileResult GetPDF(int id)
        {
            var revision = db.k_revision_control.Find(id);
            var bytes = Utilidades.GenerateDoc.RevisionControl(revision);

            var name = Utilidades.Utilidades.NormalizarNombreArchivo("Revision del Control: " + revision.k_control.relacion_control + " " + ((DateTime)revision.dg_fe_revision).ToShortDateString());

            //var file = File(bytes, "application/pdf");
            //file.FileDownloadName = name;

            return File(bytes, "application/pdf");
            //return File(bytes, "application/pdf", name + ".pdf");
        }

        public int GenReporteGeneral(RGRevViewModel model)
        {
            var TD = DateTime.Now;
            var stringUUID = TD.Minute.ToString() + TD.Second;
            var UUID = int.Parse(stringUUID);
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + UUID);



            using (FileStream fs = System.IO.File.Create(path))
            {
                byte[] info = Utilidades.GenerateDoc.RGRev(model);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }


            return UUID;
        }

        public FileResult GetReporteGeneral(int id)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/Temp" + id);
            var bytes = System.IO.File.ReadAllBytes(path);


            return File(bytes, "application/pdf");
        }
        #endregion

        public ActionResult ReporteGeneral()
        {
            ViewBag.DireccionGeneralL = Utilidades.DropDown.DireccionGeneralRevisionMS();

            return PartialView();
        }

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
