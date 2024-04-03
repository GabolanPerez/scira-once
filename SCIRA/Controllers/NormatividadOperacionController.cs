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
    [Access(Funcion = "NormatividadOP", ModuleCode = "MSICI002")]
    [CustomErrorHandler]
    public class NormatividadOperacionController : Controller
    {
        private SICIEntities db = new SICIEntities();

        public ActionResult Index()
        {
            var User = (IdentityPersonalizado)HttpContext.User.Identity;
            int id = User.Id_usuario;
            var usuario = db.c_usuario.Find(id);


            var fichas = Utilidades.Utilidades.RTCObject(usuario,db,"r_evento").Cast<r_evento>().ToList();
            //var fichas = user.Es_super_usuario ? db.r_evento.ToList() : db.r_evento.Where(e => e.id_responsable == user.Id_usuario).ToList();

            var model = new List<FichasViewModel>();


            foreach (var ficha in fichas)
            {
                //Comprobar que la ficha aun tenga su registro ligado
                var tipo = Utilidades.Utilidades.tipoFicha(ficha);
                //string registro_ligado = Utilidades.Utilidades.registroLigado(ficha);
                string registro_ligado = Utilidades.Utilidades.rutaRegistroLigado(ficha);
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

        #region Delete

        [HttpPost, NotOnlyRead]
        public int Delete(int id)
        {
            var model = db.r_evento.Find(id);

            DeleteActions.DeleteFichaObjects(model, db);

            db.r_evento.Remove(model);
            db.SaveChanges();

            return id;
        }
        #endregion

        #region Modales
        public ActionResult ContenidoNormatividadI(int id)
        {
            ViewBag.id_contenido = id;

            var cont = db.c_contenido_normatividad.Find(id);
            var norm = Utilidades.Utilidades.getRoot(db, cont);

            if (cont.id_contenido_normatividad_padre != null)
            {
                ViewBag.title = cont.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            }
            else
            {
                ViewBag.title = Strings.getMSG("Normatividad") + norm.ds_contenido_normatividad;
            }


            return PartialView("Modales/Index");
        }

        #endregion

        #region Otros
        public ActionResult LigarSP(int id)
        {
            var cont = db.c_contenido_normatividad.Find(id);


            var selected = cont.c_sub_proceso_normatividad.Select(spn => spn.id_sub_proceso).ToArray();

            var norm = Utilidades.Utilidades.getRoot(db, cont);

            if (cont.id_contenido_normatividad_padre != null)
            {
                ViewBag.title = Strings.getMSG("Sub Procesos ligados con") + cont.cl_contenido_normatividad + Strings.getMSG("de la normatividad") + norm.ds_contenido_normatividad;
            }
            else
            {
                ViewBag.title = Strings.getMSG("Sub Procesos ligados con la normatividad") + norm.ds_contenido_normatividad;
            }
            ViewBag.id_contenido = id;
            ViewBag.tspL = Utilidades.Utilidades.SubProcesosLigados(cont);
            ViewBag.spL = Utilidades.DropDown.SubProcesosMS(selected);

            return PartialView("Modales/LigarSP");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public int LigarSP(int id_contenido, int[] sps)
        {
            var cont = db.c_contenido_normatividad.Find(id_contenido);
            var ligas = cont.c_sub_proceso_normatividad.ToList();


            foreach (var liga in ligas)
            {
                db.c_sub_proceso_normatividad.Remove(liga);
            }

            db.SaveChanges();

            if (sps != null)
            {
                foreach (var idsp in sps)
                {
                    var spn = new c_sub_proceso_normatividad()
                    {
                        id_contenido_normatividad = id_contenido,
                        id_sub_proceso = idsp,
                        es_raiz = true
                    };

                    db.c_sub_proceso_normatividad.Add(spn);
                }
            }

            db.SaveChanges();

            return id_contenido;
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
