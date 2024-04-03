using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Estructura", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteEstructuraController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteEstructura
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);

            var entidades = Utilidades.Utilidades.RTCObject(db.c_usuario.Find(user.Id_usuario), db, "c_entidad").Cast<c_entidad>().ToList();

            List<EstructuraRepModel> model = new List<EstructuraRepModel>();

            foreach(var en in entidades)
            {
                EstructuraRepModel rowEn = new EstructuraRepModel();
                rowEn.cl_en = $"{en.cl_entidad} - {en.nb_entidad}";
                rowEn.re_en = en.c_usuario.nb_usuario;

                var mps = en.c_macro_proceso.ToList();

                

                if (mps.Count == 0)
                {
                    var copyRow = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowEn, new EstructuraRepModel());
                    model.Add(copyRow);
                }
                else 
                {
                    foreach (var mp in mps) {
                        var rowMP = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowEn, new EstructuraRepModel());
                        rowMP.cl_mp = $"{mp.cl_macro_proceso} - {mp.nb_macro_proceso}";
                        rowMP.re_mp = mp.c_usuario.nb_usuario;

                        var pps = mp.c_proceso.ToList();
                        if(pps.Count== 0) 
                        {
                            var copyRow = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowMP, new EstructuraRepModel());
                            model.Add(copyRow);
                        }
                        else
                        {
                            foreach (var pr in pps) {
                                var rowPR = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowMP, new EstructuraRepModel());
                                rowPR.cl_pr = $"{pr.cl_proceso} - {pr.nb_proceso}";
                                rowPR.re_pr = pr.c_usuario.nb_usuario;

                                var sps = pr.c_sub_proceso.ToList();


                                if (sps.Count == 0)
                                {
                                    var copyRow = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowPR, new EstructuraRepModel());
                                    model.Add(copyRow);
                                }
                                else
                                {
                                    foreach (var sp in sps)
                                    {
                                        var rowSP = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowPR, new EstructuraRepModel());
                                        rowSP.cl_sp = $"{sp.cl_sub_proceso} - {sp.nb_sub_proceso}";
                                        rowSP.re_sp = sp.c_usuario.nb_usuario;
                                        rowSP.cl_manual = sp.cl_manual;

                                        var copyRow = (EstructuraRepModel)Utilidades.Utilidades.CopyObject(rowSP, new EstructuraRepModel());
                                        model.Add(copyRow);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return View(model);
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