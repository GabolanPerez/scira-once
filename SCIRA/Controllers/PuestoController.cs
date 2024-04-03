using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Puestos", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class PuestoController : Controller
    {
        private SICIEntities db = new SICIEntities();

        // GET: 
        public ActionResult Index()
        {
            c_puesto puestoRaiz = Utilidades.Utilidades.getRoot(db);

            ViewBag.data = Utilidades.Utilidades.getChildData(puestoRaiz, db);

            return View();
        }

        public ActionResult Index2()
        {
            DelExtraRoot();

            var puestos = db.c_puesto.ToList();
            var data = new List<OrgChart>();

            foreach (var puesto in puestos)
            {
                int pid = 0;
                try
                {
                    pid = (int)puesto.id_puesto_padre;
                }
                catch
                {

                }

                string usuarios = "";
                foreach (var u in puesto.c_usuario)
                {
                    usuarios += u.nb_usuario + "\n";
                }

                var org = new OrgChart
                {
                    id = puesto.id_puesto,
                    pid = pid,
                    Nombre = puesto.nb_puesto,
                    Clave = puesto.cl_puesto,
                    Descripcion = puesto.tag_puesto,
                    Usuarios = usuarios
                };
                data.Add(org);
            }

            ViewBag.JsonData = JsonConvert.SerializeObject(data);

            return View();
        }

        #region Clase para Datos
        private class OrgChart
        {
            public int id { get; set; }
            public int pid { get; set; }
            public string Nombre { get; set; }
            public string Clave { get; set; }
            public string Descripcion { get; set; }
            public string Usuarios { get; set; }
        }
        #endregion




        #region Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create(c_puesto puesto)
        {
            if (ModelState.ContainsKey("id_puesto"))
                ModelState["id_puesto"].Errors.Clear();
            if (ModelState.ContainsKey("cl_puesto"))
                ModelState["cl_puesto"].Errors.Clear();


            if (ModelState.IsValid)
            {
                var puestoPadre = db.c_puesto.Find(puesto.id_puesto_padre);
                puesto.cl_puesto = string.Format("{0:00}", int.Parse(puestoPadre.cl_puesto) + 1);

                db.c_puesto.Add(puesto);
                db.SaveChanges();
                c_puesto puestoRaiz = Utilidades.Utilidades.getRoot(db);
                ViewBag.data = Utilidades.Utilidades.getChildData(puestoRaiz, db);

                return PartialView("LoadData");
            }

            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Create2(c_puesto puesto)
        {
            if (ModelState.ContainsKey("id_puesto"))
                ModelState["id_puesto"].Errors.Clear();
            if (ModelState.ContainsKey("cl_puesto"))
                ModelState["cl_puesto"].Errors.Clear();


            if (ModelState.IsValid)
            {
                var puestoPadre = db.c_puesto.Find(puesto.id_puesto_padre);
                puesto.cl_puesto = string.Format("{0:00}", int.Parse(puestoPadre.cl_puesto) + 1);

                db.c_puesto.Add(puesto);
                db.SaveChanges();

                string Json = JsonConvert.SerializeObject(new OrgChart()
                {
                    id = puesto.id_puesto,
                    pid = (int)puesto.id_puesto_padre,
                    Nombre = puesto.nb_puesto,
                    Clave = puesto.cl_puesto,
                    Descripcion = puesto.tag_puesto,
                    Usuarios = ""
                });

                ViewBag.data = Json;

                return PartialView("LoadData2");
            }

            return null;
        }
        #endregion

        #region Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit(int id_puesto, string cl_puesto, string nb_puesto)
        {
            try
            {
                var puesto = db.c_puesto.Find(id_puesto);
                puesto.cl_puesto = cl_puesto;
                puesto.nb_puesto = nb_puesto;
                db.SaveChanges();
                c_puesto puestoRaiz = Utilidades.Utilidades.getRoot(db);
                ViewBag.data = Utilidades.Utilidades.getChildData(puestoRaiz, db);

                return PartialView("LoadData");
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit2(int id_puesto, string cl_puesto, string nb_puesto, string tag_puesto)
        {
            try
            {
                var puesto = db.c_puesto.Find(id_puesto);
                puesto.cl_puesto = cl_puesto;
                puesto.nb_puesto = nb_puesto;
                puesto.tag_puesto = tag_puesto;
                db.SaveChanges();

                int pid = 0;
                if (puesto.id_puesto_padre != null) pid = (int)puesto.id_puesto_padre;

                string usuarios = "";
                foreach (var u in puesto.c_usuario)
                {
                    usuarios += u.nb_usuario + "\n";
                }

                string Json = JsonConvert.SerializeObject(new OrgChart()
                {
                    id = puesto.id_puesto,
                    pid = pid,
                    Nombre = puesto.nb_puesto,
                    Clave = puesto.cl_puesto,
                    Descripcion = puesto.tag_puesto,
                    Usuarios = usuarios
                });

                ViewBag.data = Json;

                return PartialView("LoadData3");
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete(int id)
        {
            try
            {
                var puesto = db.c_puesto.Find(id);

                Utilidades.DeleteActions.DeletePuestoObjects(puesto, db);

                db.c_puesto.Remove(puesto);
                db.SaveChanges();
                c_puesto puestoRaiz = Utilidades.Utilidades.getRoot(db);
                ViewBag.data = Utilidades.Utilidades.getChildData(puestoRaiz, db);

                return PartialView("LoadData");
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Delete2(int id)
        {
            try
            {
                var puesto = db.c_puesto.Find(id);
                List<int> ids = new List<int>();
                ids.Add(puesto.id_puesto);
                var puestosInferiores = Utilidades.Utilidades.puestosInferiores(puesto, db);
                foreach (var p in puestosInferiores)
                {
                    ids.Add(p.id_puesto);
                    //se agrego esta validación para que también se eliminen todas las relaciones con otros registros.
                }

                Utilidades.DeleteActions.DeletePuestoObjects(puesto, db);
                db.c_puesto.Remove(puesto);
                db.SaveChanges();

                string Json = JsonConvert.SerializeObject(ids);
                ViewBag.data = Json;

                return PartialView("LoadData4");
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Otros
        public ActionResult AssignUsers(int id)
        {
            AsignaUsuarioPuestoViewModel model = new AsignaUsuarioPuestoViewModel();
            model.id_puesto = id;

            var puesto = db.c_puesto.Find(id);
            var usuarios = puesto.c_usuario.ToList();

            int[] idUs = new int[usuarios.Count];

            for (int i = 0; i < usuarios.Count; i++)
            {
                idUs[i] = usuarios.ElementAt(i).id_usuario;
            }

            model.users = idUs;

            ViewBag.users = (MultiSelectList)Utilidades.DropDown.UsuariosYPuestoMS(idUs);
            ViewBag.id = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AssignUsers(AsignaUsuarioPuestoViewModel model)
        {
            var puesto = db.c_puesto.Find(model.id_puesto);

            puesto.c_usuario.Clear();


            if (model.users != null)
            {
                foreach (var intU in model.users)
                {
                    var user = db.c_usuario.Find(intU);
                    user.c_puesto.Clear();
                    user.c_puesto.Add(puesto);
                }
            }

            db.SaveChanges();

            return null;
        }

        [HttpPost]
        [NotOnlyRead]
        public ActionResult Move(int id, int idNewP)
        {
            var padre = db.c_puesto.Find(idNewP);
            var puesto = db.c_puesto.Find(id);
            puesto.id_puesto_padre = idNewP;

            NormalizeLvl(puesto, padre.cl_puesto);
            db.SaveChanges();

            List<OrgChart> lista = new List<OrgChart>();
            var puestosInf = Utilidades.Utilidades.puestosInferiores(puesto, db);
            puestosInf.Add(puesto);

            foreach (var p in puestosInf)
            {
                lista.Add(new OrgChart()
                {
                    id = p.id_puesto,
                    pid = (int)p.id_puesto_padre,
                    Clave = p.cl_puesto,
                    Nombre = p.nb_puesto,
                    Descripcion = puesto.tag_puesto
                });
            }

            string Json = JsonConvert.SerializeObject(lista);

            ViewBag.data = Json;

            return PartialView("LoadData5");
        }

        private bool NormalizeLvl(c_puesto puesto, string cl_puesto_padre)
        {
            puesto.cl_puesto = string.Format("{0:00}", int.Parse(cl_puesto_padre) + 1);

            var hijos = db.c_puesto.Where(p => p.id_puesto_padre == puesto.id_puesto).ToList();

            foreach (var hijo in hijos)
            {
                NormalizeLvl(hijo, puesto.cl_puesto);
            }

            return true;
        }

        private bool DelExtraRoot()
        {
            try
            {
                var root = db.c_puesto.Where(p => p.id_puesto_padre == null).ToList();

                root.Remove(root.First());

                foreach (var r in root)
                {
                    db.c_puesto.Remove(r);
                }
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
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
