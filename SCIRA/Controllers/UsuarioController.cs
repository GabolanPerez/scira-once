using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.Utilidades;
using SCIRA.Validaciones;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SCIRA.Controllers
{
    [Authorize]
    [Access(Funcion = "Usuarios", ModuleCode = "MSICI000")]
    [CustomErrorHandler]
    public class UsuarioController : Controller
    {
        private SICIEntities db = new SICIEntities();

        //Estatus de usuario
        // 1.-  Nuevo
        // 2.-  Activo
        // 3.-  Inactivo
        // 4.-  Bloqueado

        // GET: Usuario
        public ActionResult Index()
        {
            var c_usuario = db.c_usuario.Include(c => c.c_area);
            return View(c_usuario.ToList());
        }

        #region Clase para Datos
        private class OrgChart
        {
            public int id { get; set; }
            public int pid { get; set; }
            public string Nombre { get; set; }
            public string Clave { get; set; }
        }
        #endregion

        #region Create

        public ActionResult Agregar()
        {
            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area");

            ViewBag.JsonData = JsonData();

            var model = new c_usuario();

            if (((IdentityPersonalizado)User.Identity).activeD)
            {
                model.password = "password";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Agregar([Bind(Include = "id_usuario,cl_usuario,nb_usuario,password,e_mail_principal,e_mail_alterno,no_telefono,esta_activo,es_super_usuario,nb_puesto,id_area,solo_lectura,es_auditor,es_auditor_admin")] c_usuario c_usuario, int id_puesto = 0)
        {

            if (ModelState.IsValid)
            {
                c_usuario.esta_activo = true;
                c_usuario.id_estatus_usuario = 1;
                c_usuario.password = SeguridadUtilidades.SHA256Encripta(c_usuario.password);
                c_usuario.fe_cambio_password = DateTime.Now;
                db.c_usuario.Add(c_usuario);

                try
                {
                    db.SaveChanges();

                    //Agregamos la contraseña al historico del usuario
                    h_password Pass = new h_password();
                    Pass.id_usuario = c_usuario.id_usuario;
                    Pass.password = c_usuario.password;
                    Pass.fe_actualizacion = DateTime.Now;
                    db.h_password.Add(Pass);

                    if (id_puesto > 0)
                    {
                        c_usuario.c_puesto.Add(db.c_puesto.Find(id_puesto));
                    }

                    db.SaveChanges();


                    if (c_usuario.es_auditor_admin)
                        Notification.NewAdminAudit(((IdentityPersonalizado)User.Identity).Id_usuario, c_usuario.id_usuario);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    int ErrorCode = Int32.Parse(ex.InnerException.InnerException.HResult.ToString());

                    if (ErrorCode == -2146232060)
                    {
                        ModelState.AddModelError("es_super_usuario", "Asegurese de que la clave y el Correo electrónico principal no esten en uso.");
                    }
                    System.Console.WriteLine("");
                }

            }

            if (id_puesto > 0) ViewBag.puesto = id_puesto;

            ViewBag.JsonData = JsonData();

            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area", c_usuario.id_area);
            return View(c_usuario);
        }
        #endregion

        #region Edit
        // GET: Usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_usuario c_usuario = db.c_usuario.Find(id);
            if (c_usuario == null)
            {
                return HttpNotFound();
            }

            c_puesto puesto;
            try
            {
                puesto = c_usuario.c_puesto.First();
            }
            catch
            {
                puesto = null;
            }
            if (puesto != null)
            {
                ViewBag.puesto = puesto.id_puesto;
                ViewBag.nbPuesto = puesto.nb_puesto;
            }

            ViewBag.JsonData = JsonData();

            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area", c_usuario.id_area);
            return View(c_usuario);
        }

        // POST: Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult Edit([Bind(Include = "id_usuario,cl_usuario,nb_usuario,password,e_mail_principal,e_mail_alterno,no_telefono,esta_activo,es_super_usuario,nb_puesto,id_area,solo_lectura,id_estatus_usuario,es_auditor,es_auditor_admin")] c_usuario c_usuario, int id_puesto = 0)
        {
            if (ModelState.IsValid)
            {
                c_usuario user = db.c_usuario.Find(c_usuario.id_usuario);

                var newAdminAudit = !user.es_auditor_admin && c_usuario.es_auditor_admin;

                if (c_usuario.esta_activo)
                {
                    c_usuario.id_estatus_usuario = 2;
                }
                if (user.password != c_usuario.password)
                {
                    c_usuario.password = SeguridadUtilidades.SHA256Encripta(c_usuario.password);
                    c_usuario.fe_cambio_password = DateTime.Now;
                    c_usuario.fe_ultimo_acceso = DateTime.Now;

                    //Agregamos la contraseña al historico del usuario
                    h_password Pass = new h_password();
                    Pass.id_usuario = c_usuario.id_usuario;
                    Pass.password = c_usuario.password;
                    Pass.fe_actualizacion = DateTime.Now;
                    db.h_password.Add(Pass);

                    //fijamos el id en 1 para que su estatus sea nuevo, y se requiera un cambio de contraseña
                    if (c_usuario.id_estatus_usuario != 4)
                    {
                        c_usuario.id_estatus_usuario = 1;
                    }
                }
                user.cl_usuario = c_usuario.cl_usuario;
                user.nb_usuario = c_usuario.nb_usuario;
                user.password = c_usuario.password;
                user.e_mail_principal = c_usuario.e_mail_principal;
                user.e_mail_alterno = c_usuario.e_mail_alterno;
                user.no_telefono = c_usuario.no_telefono;
                user.esta_activo = c_usuario.esta_activo;
                user.nb_puesto = c_usuario.nb_puesto;
                user.id_area = c_usuario.id_area;
                user.es_super_usuario = c_usuario.es_super_usuario;
                user.solo_lectura = c_usuario.solo_lectura;
                user.es_auditor = c_usuario.es_auditor;
                user.es_auditor_admin = c_usuario.es_auditor_admin;
                user.id_estatus_usuario = c_usuario.id_estatus_usuario;
                /*db.Entry(c_usuario).State = EntityState.Modified;*/
                try
                {
                    c_puesto puestoA;

                    try
                    {
                        puestoA = db.c_puesto.Where(p => p.c_usuario.Any(u => u.id_usuario == user.id_usuario)).First();
                    }
                    catch
                    {
                        puestoA = null;
                    }

                    user.c_puesto.Clear();
                    if (id_puesto != 0)
                    {
                        user.c_puesto.Add(db.c_puesto.Find(id_puesto));
                    }

                    db.SaveChanges();

                    if (newAdminAudit)
                        Notification.NewAdminAudit(((IdentityPersonalizado)User.Identity).Id_usuario, c_usuario.id_usuario);


                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    int ErrorCode = Int32.Parse(ex.InnerException.InnerException.HResult.ToString());

                    if (ErrorCode == -2146232060)
                    {
                        ModelState.AddModelError("es_super_usuario", "Asegurese de que la clave y el Correo electrónico principal no esten en uso.");
                    }
                    System.Console.WriteLine("");
                }
            }

            if (id_puesto > 0)
            {
                var p = db.c_puesto.Find(id_puesto);
                ViewBag.puesto = p.id_puesto;
                ViewBag.nbPuesto = p.nb_puesto;
            }

            ViewBag.JsonData = JsonData();
            ViewBag.id_area = new SelectList(db.c_area, "id_area", "nb_area", c_usuario.id_area);
            return View(c_usuario);
        }
        #endregion

        #region Delete
        // GET: Usuario/Delete/5
        public ActionResult Delete(int? id, string redirect = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_usuario c_usuario = db.c_usuario.Find(id);
            if (c_usuario == null)
            {
                return HttpNotFound();
            }

            if (redirect != null)
            {
                if (redirect != "bfo")
                {
                    //obtenemos el valor del numero de salto
                    int ns;
                    try
                    {
                        ns = (int)HttpContext.Session["JumpCounter"];
                    }
                    catch
                    {
                        ns = 0;
                    }
                    //Si ns es 0, creamos un nuevo array, agregamos la direccion actual y lo asignamos a la variable "Directions" y establecemos "JumpCounter" = 1
                    if (ns == 0)
                    {
                        List<string> directions = new List<string>();
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = 1;
                        HttpContext.Session["Directions"] = directions;

                    }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
                    else
                    {
                        ns++;
                        List<string> directions = (List<string>)HttpContext.Session["Directions"];
                        directions.Add(redirect);
                        HttpContext.Session["JumpCounter"] = ns;
                        HttpContext.Session["Directions"] = directions;
                    }
                }
            }
            else
            {
                HttpContext.Session["JumpCounter"] = null;
                HttpContext.Session["Directions"] = null;
            }

            //Obtener todos los elementos a los que puede estar ligado este elemento.
            //creamos la lista que contendra a todos los registros relacionados
            List<RegistrosRelacionadosViewModel> RR = new List<RegistrosRelacionadosViewModel>();

            var r_bdei = db.k_bdei.Where(b => b.id_responsable_captura == id || b.id_responsable_recuperacion == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_bdei.Count > 0)
            {
                foreach (var bdei in r_bdei)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "BDEI";
                    rr.cl_registro = bdei.id_bdei.ToString();
                    rr.nb_registro = "BDEI ligado a la entidad: " + bdei.c_entidad.nb_entidad;
                    rr.accion = "Delete";
                    rr.controlador = "BDEI";
                    rr.id_registro = bdei.id_bdei.ToString();

                    RR.Add(rr);
                }
            }

            var r_control = db.k_control.Where(b => b.id_responsable == id || b.id_ejecutor == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_control.Count > 0)
            {
                foreach (var control in r_control)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Controles";
                    rr.cl_registro = control.relacion_control;
                    rr.nb_registro = control.tiene_accion_correctora ? control.accion_correctora : control.actividad_control;
                    rr.accion = "Delete";
                    rr.controlador = "Control";
                    rr.id_registro = control.id_control.ToString();

                    RR.Add(rr);
                }
            }

            var r_entidad = db.c_entidad.Where(b => b.id_responsable == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_entidad.Count > 0)
            {
                foreach (var entidad in r_entidad)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Entidades";
                    rr.cl_registro = entidad.cl_entidad;
                    rr.nb_registro = entidad.nb_entidad;
                    rr.accion = "Delete";
                    rr.controlador = "Entidad";
                    rr.id_registro = entidad.id_entidad.ToString();

                    RR.Add(rr);
                }
            }

            var r_indicador = db.c_indicador.Where(b => b.id_responsable == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_indicador.Count > 0)
            {
                foreach (var indicador in r_indicador)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Indicadores";
                    rr.cl_registro = indicador.cl_indicador;
                    rr.nb_registro = indicador.nb_indicador;
                    rr.accion = "Delete";
                    rr.controlador = "Indicador";
                    rr.id_registro = indicador.id_indicador.ToString();

                    RR.Add(rr);
                }
            }

            //c_macro_proceso
            var r_macro_proceso = db.c_macro_proceso.Where(b => b.id_responsable == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_macro_proceso.Count > 0)
            {
                foreach (var mp in r_macro_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Macro Proceso";
                    rr.cl_registro = mp.cl_macro_proceso;
                    rr.nb_registro = mp.nb_macro_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "MacroProceso";
                    rr.id_registro = mp.id_macro_proceso.ToString();

                    RR.Add(rr);
                }
            }

            var r_proceso = db.c_proceso.Where(b => b.id_responsable == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_proceso.Count > 0)
            {
                foreach (var proceso in r_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Procesos";
                    rr.cl_registro = proceso.cl_proceso;
                    rr.nb_registro = proceso.nb_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "Proceso";
                    rr.id_registro = proceso.id_proceso.ToString();

                    RR.Add(rr);
                }
            }

            var r_sub_proceso = db.c_sub_proceso.Where(b => b.id_responsable == id).ToList();

            //creamos un objeto de tipo RegistrosRelacionadosViewModel para cada uno de estos elementos y lo incluimos en la lista RR
            if (r_sub_proceso.Count > 0)
            {
                foreach (var sp in r_sub_proceso)
                {
                    RegistrosRelacionadosViewModel rr = new RegistrosRelacionadosViewModel();
                    rr.nb_catalogo = "Sub Procesos";
                    rr.cl_registro = sp.cl_sub_proceso;
                    rr.nb_registro = sp.nb_sub_proceso;
                    rr.accion = "Delete";
                    rr.controlador = "SubProceso";
                    rr.id_registro = sp.id_sub_proceso.ToString();

                    RR.Add(rr);
                }
            }

            //Si RR contiene al menos un elemento, enviamos los datos a la vista
            ViewBag.RR = RR;


            return View(c_usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult DeleteConfirmed(int id)
        {
            c_usuario c_usuario = db.c_usuario.Find(id);
            c_usuario.c_rol.Clear();
            var usps = db.c_usuario_sub_proceso.Where(u => u.id_usuario == id).ToList();
            foreach (var usp in usps)
            {
                db.c_usuario_sub_proceso.Remove(usp);
            }


            //fijar su estado en 0
            c_usuario.esta_activo = false;
            c_usuario.id_estatus_usuario = 3;
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("CantErase", "Error", null);
            }

            //En caso de que el registro se haya eliminado correctamente, redireccionar dependiendo desde donde se haya accesado al menú de eliminar
            int ns;
            try
            {
                ns = (int)HttpContext.Session["JumpCounter"];
            }
            catch
            {
                ns = 0;
            }
            //Si ns es 0 redireccionamos al index de este controlador
            if (ns == 0)
            {
                return RedirectToAction("Index");

            }//En caso de que ns sea distinto a 0, obtenemos el Array "Directions", agregamos la direccion actual, aumentamos el contador y salvamos ambas variables globales
            else
            {
                List<string> directions = new List<string>();
                try
                {
                    directions = (List<string>)HttpContext.Session["Directions"];
                }
                catch
                {
                    directions = null;
                }

                if (directions == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string direction = directions.Last();
                    DirectionViewModel dir = Utilidades.Utilidades.getDirection(direction);
                    //disminuimos ns y eliminamos el ultimo elemento de directions
                    ns--;
                    directions.RemoveAt(ns);

                    //Guardamos ambas variables de sesion para seguir trabajando
                    HttpContext.Session["JumpCounter"] = ns;
                    HttpContext.Session["Directions"] = directions;

                    return RedirectToAction(dir.Action, dir.Controller, new { id = dir.Id, redirect = "bfo" });
                }
            }
        }
        #endregion

        #region Roles
        public ActionResult AsignaRol(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_usuario c_usuario = db.c_usuario.Find(id);
            if (c_usuario == null)
            {
                return HttpNotFound();
            }
            AsignaRolUsuarioViewModel Usuario = new AsignaRolUsuarioViewModel();
            Usuario.id_usuario = c_usuario.id_usuario;
            ViewBag.nb_usuario = c_usuario.nb_usuario;
            string sql = "select id_rol from c_rol_usuario where id_usuario = " + Usuario.id_usuario;
            var roles = db.Database.SqlQuery<int>(sql).ToArray();

            List<c_rol> Roles = new List<c_rol>();

            var rolesL = db.c_rol;

            foreach (var rol in rolesL)
            {
                var funcion = rol.c_funcion.FirstOrDefault();
                if (funcion != null)
                {
                    if (funcion.c_menu_funcion.cl_menu_funcion != "AUDI")
                        Roles.Add(rol);
                }
                else
                {
                    db.c_rol.Remove(rol);
                }
            }
            db.SaveChanges();


            ViewBag.roles = new MultiSelectList(Roles.OrderBy(x => x.nb_rol), "id_rol", "nb_rol", roles);
            return View(Usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult AsignaRol([Bind(Include = "id_usuario,id_rol")] AsignaRolUsuarioViewModel Usuario)
        {
            c_usuario c_usuario = db.c_usuario.Find(Usuario.id_usuario);
            if (c_usuario == null)
            {
                return HttpNotFound();
            }

            try
            {
                c_usuario.c_rol.Clear();
                if (Usuario.id_rol == null)
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                foreach (int id_rl in Usuario.id_rol)
                {
                    c_rol r = db.c_rol.Find(id_rl);
                    c_usuario.c_rol.Add(r);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.nb_usuario = c_usuario.nb_usuario;
                string sql = "select id_rol from c_rol_usuario where id_usuario = " + Usuario.id_usuario;
                var roles = db.Database.SqlQuery<int>(sql).ToArray();
                ViewBag.roles = new MultiSelectList(db.c_rol.OrderBy(x => x.nb_rol), "id_rol", "nb_rol", roles);
                return View(Usuario);
            }
        }
        #endregion

        #region Otros
        private string JsonData()
        {
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

                var org = new OrgChart
                {
                    id = puesto.id_puesto,
                    pid = pid,
                    Nombre = puesto.nb_puesto,
                    Clave = puesto.cl_puesto
                };
                data.Add(org);
            }

            return JsonConvert.SerializeObject(data);
        }

        public ActionResult unlockUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_usuario c_usuario = db.c_usuario.Find(id);
            if (c_usuario == null)
            {
                return HttpNotFound();
            }


            c_usuario.id_estatus_usuario = 2;
            c_usuario.esta_activo = true;
            c_usuario.fe_cambio_password = DateTime.Now;
            c_usuario.fe_ultimo_intento_acceso = DateTime.Now;
            c_usuario.no_intento_acceso = 0;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult GetUserObjects(int id)
        {
            ViewBag.macro_procesos = Utilidades.DropDown.MacroProcesosMS(responsable: id);
            ViewBag.procesos = Utilidades.DropDown.ProcesosMS(responsable: id);
            ViewBag.sub_procesos = Utilidades.DropDown.SubProcesosMS(responsable: id);
            ViewBag.controles = Utilidades.DropDown.ControlesMS(responsable: id);
            ViewBag.indicadores = Utilidades.DropDown.IndicadoresMS(responsable: id);
            ViewBag.fichas = Utilidades.DropDown.FichasMS(responsable: id);
            ViewBag.oficios = Utilidades.DropDown.KObjetoMS(responsable: id, tipo: 1); //NUEVO
            ViewBag.auditoria_ext = Utilidades.DropDown.KObjetoMS(responsable: id, tipo: 2); //NUEVO
            ViewBag.auditoria_int = Utilidades.DropDown.KObjetoMS(responsable: id, tipo: 3); //NUEVO
            ViewBag.otros = Utilidades.DropDown.KObjetoMS(responsable: id, tipo: 6); //NUEVO
            ViewBag.planes_remediacion = Utilidades.DropDown.KPlanMS(responsable: id, esSeguimiento: false); //NUEVO
            ViewBag.incidencias = Utilidades.DropDown.KIncidenciaMS(responsable: id); //NUEVO
            ViewBag.seguimientos = Utilidades.DropDown.KPlanMS(responsable: id, esSeguimiento: true); //NUEVO

            ViewBag.usuarios = Utilidades.DropDown.Usuario();

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NotOnlyRead]
        public ActionResult GetUserObjects(
            int[] mps,
            int[] ps,
            int[] sps,
            int[] ctrs,
            int[] inds,
            int[] fics,
            int[] oficios,
            int[] externa,
            int[] interna,
            int[] otros,
            int[] planes,
            int[] incidencias,
            int[] seguimientos,
            int usuarioDest)
        {
            var user = (IdentityPersonalizado)HttpContext.User.Identity;

            //traspasar macro procesos
            if (mps != null)
                foreach (var id in mps)
                {
                    //var mp = db.c_macro_proceso.Find(id);
                    db.c_macro_proceso.Find(id).id_responsable = usuarioDest;
                }
            //traspasar procesos
            if (ps != null)
                foreach (var id in ps)
                {
                    db.c_proceso.Find(id).id_responsable = usuarioDest;
                }
            //traspasar sub procesos
            if (sps != null)
                foreach (var id in sps)
                {
                    db.c_sub_proceso.Find(id).id_responsable = usuarioDest;
                }
            //traspasar controles
            if (ctrs != null)
                foreach (var id in ctrs)
                {
                    db.k_control.Find(id).id_responsable = usuarioDest;
                }
            //traspasar indicadores
            if (inds != null)
                foreach (var id in inds)
                {
                    db.c_indicador.Find(id).id_responsable = usuarioDest;
                }
            //traspasar fichas
            if (fics != null)
                foreach (var id in fics)
                {
                    db.r_evento.Find(id).id_responsable = usuarioDest;
                }
            //traspasar oficios
            if (oficios != null)
                foreach (var id in oficios)
                {
                    db.k_objeto.Find(id).id_responsable = usuarioDest;
                }
            //traspasar auditoría externa
            if (externa != null)
                foreach (var id in externa)
                {
                    db.k_objeto.Find(id).id_responsable = usuarioDest;
                }
            //traspasar auditoría interna
            if (interna != null)
                foreach (var id in interna)
                {
                    db.k_objeto.Find(id).id_responsable = usuarioDest;
                }
            //traspasar otros
            if (otros != null)
                foreach (var id in otros)
                {
                    db.k_objeto.Find(id).id_responsable = usuarioDest;
                }
            //traspasar planes de remediacion (k_plan)
            if (planes != null)
                foreach (var id in planes)
                {
                    db.k_plan.Find(id).id_responsable = usuarioDest;
                }
            //traspasar incidencias
            if (incidencias != null)
                foreach (var id in incidencias)
                {
                    db.k_incidencia.Find(id).id_responsable = usuarioDest;
                }
            //traspasar seguimiento
            if (seguimientos != null)
                foreach (var id in seguimientos)
                {
                    db.k_plan.Find(id).id_responsable_seguimiento = usuarioDest;
                }

            db.SaveChanges();

            Utilidades.Utilidades.notifyUser(user.Id_usuario, Strings.getMSG("UsuarioCreate062"), "success");

            return null;
        }

        public string activeUser(int id)
        {
            db.c_usuario.Find(id).esta_activo = true;
            db.SaveChanges();

            return "ok";
        }

        public string inactiveUser(int id)
        {
            var user = db.c_usuario.Find(id);

            //if (user.c_entidad.Count > 0)
            //    return "error";
            if (user.c_indicador.Count > 0)
                return "error";
            if (user.c_macro_proceso.Count > 0)
                return "error";
            if (user.c_proceso.Count > 0)
                return "error";
            if (user.c_sub_proceso.Count > 0)
                return "error";
            //if (user.c_usuario_sub_proceso.Count > 0)
            //    return "error";
            //if (user.k_bdei.Count > 0)
            //    return "error";
            //if (user.k_control.Count > 0)  //Ejecutor control
            //    return "error";
            if (user.k_control1.Count > 0)
                return "error";
            //if (user.k_incidencia.Count > 0)
            //    return "error";
            //if (user.k_plan.Count > 0)
            //    return "error";
            //if (user.k_plan1.Count > 0)
            //    return "error";
            //if (user.c_indicador_diario.Count > 0)
            //    return "error";
            //if (user.c_contenido_manual.Count > 0)
            //    return "error";
            if (user.k_objeto.Count > 0)
                return "error";

            user.esta_activo = false;
            db.SaveChanges();

            return "ok";
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
