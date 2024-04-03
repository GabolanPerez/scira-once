using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "RepFichas", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class ReporteFichasController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: ReporteGeneralBDEI
        public ActionResult Index()
        {
            var user = ((IdentityPersonalizado)User.Identity);
            var us = db.c_usuario.Find(user.Id_usuario);

            var fichas = Utilidades.Utilidades.RTCObject(us, db, "r_evento").Cast<r_evento>();


            var model = new List<FichasViewModel>();


            foreach (var ficha in fichas)
            {
                //Comprobar que la ficha aun tenga su registro ligado
                var tipo = Utilidades.Utilidades.tipoFicha(ficha);
                string registro_ligado = Utilidades.Utilidades.registroLigado(ficha);
                if (registro_ligado != null)
                {
                    var reg = Utilidades.Utilidades.GetLastReg(ficha, db);
                    int id_registro_ligado = Utilidades.Utilidades.idRegistroLigado(ficha);



                    var vm = new FichasViewModel
                    {
                        id_evento = ficha.id_evento,
                        nb_evento = ficha.nb_evento,
                        clase = ficha.tipo,
                        tipo = tipo,
                        ds_evento = ficha.ds_evento,
                        estatus = Utilidades.Utilidades.GetStatus(ficha),
                        feLim = Utilidades.Utilidades.getFeLim(ficha).ToString("dd/MM/yyyy HH:mm"),
                        nb_usuario = ficha.c_usuario.nb_usuario,
                        recordar_antes_de_vencer = ficha.recordar_antes_de_vencer ? "Único" : "Recurrente",
                        registro_ligado = registro_ligado,
                        id_registro_ligado = id_registro_ligado
                    };

                    model.Add(vm);
                }
                else
                {
                    DeleteActions.DeleteFichaObjects(ficha, db, true);
                    db.r_evento.Remove(ficha);
                    db.SaveChanges();
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