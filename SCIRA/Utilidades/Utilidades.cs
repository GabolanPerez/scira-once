using Hangfire;
using Microsoft.AspNet.SignalR;
using NCrontab;
using Newtonsoft.Json;
using SCIRA.Models;
using SCIRA.Seguridad;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SCIRA.Utilidades
{
    public static class Utilidades
    {
        static private SICIEntities db = new SICIEntities();
        static private IHubContext HubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

        #region Bloqueo de Modulos

        public static bool ModuleState(string parametro)
        {
            db = new SICIEntities();

            c_parametro param;
            bool isActive = false;

            if (parametro == "MSICI000") return true;

            try
            {
                param = db.c_parametro.Where(p => p.nb_parametro == parametro).First();


                string dateVig = "";
                //comprobar nombre de modulo correcto y obtener fecha
                var clearText = SDFK(getKey(), param.valor_parametro);
                var splitWords = clearText.Split(' ');

                if (splitWords[1] == parametro)
                {
                    dateVig = splitWords[0];
                }
                else
                {
                    dateVig = "01/01/1000";
                }


                //completar con la conversion de fecha
                var vigencia = DateTime.ParseExact(dateVig, "dd/MM/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture);
                if (vigencia > DateTime.Now)
                    isActive = true;
            }
            catch
            {
                //param = new c_parametro()
                //{
                //    nb_parametro = parametro,
                //    valor_parametro = DateTime.MinValue.ToShortDateString()
                //};

                //db.c_parametro.Add(param);
                //db.SaveChanges();
            }

            return isActive;
        }

        #endregion

        #region Cadena de conexion
        public static void ProtectConnectionString()
        {
            ToggleConnectionStringProtection
        //For Windows
        //(System.Windows.Forms.Application.ExecutablePath, true);
        //For Web
        (null, true);
        }

        public static void UnprotectConnectionString()
        {
            ToggleConnectionStringProtection
        //For Windows
        //(System.Windows.Forms.Application.ExecutablePath, false);
        //For Web
        (null, false);
        }

        private static void ToggleConnectionStringProtection
                (string pathName, bool protect)
        {
            // Define the Dpapi provider name.
            string strProvider = "DataProtectionConfigurationProvider";
            // string strProvider = "RSAProtectedConfigurationProvider";

            System.Configuration.Configuration oConfiguration = null;
            System.Configuration.ConnectionStringsSection oSection = null;

            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.

                // For Web!
                oConfiguration = System.Web.Configuration.
                                  WebConfigurationManager.OpenWebConfiguration("~");

                // For Windows!
                // Takes the executable file name without the config extension.
                //oConfiguration = System.Configuration.ConfigurationManager.
                //                                OpenExeConfiguration(pathName);

                if (oConfiguration != null)
                {
                    bool blnChanged = false;

                    oSection = oConfiguration.GetSection("connectionStrings") as
                System.Configuration.ConnectionStringsSection;

                    if (oSection != null)
                    {
                        if ((!(oSection.ElementInformation.IsLocked)) &&
                (!(oSection.SectionInformation.IsLocked)))
                        {
                            if (protect)
                            {
                                if (!(oSection.SectionInformation.IsProtected))
                                {
                                    blnChanged = true;

                                    // Encrypt the section.
                                    oSection.SectionInformation.ProtectSection
                                (strProvider);
                                    oConfiguration.AppSettings.Settings["Cypher"].Value = "true";
                                }
                            }
                            else
                            {
                                if (oSection.SectionInformation.IsProtected)
                                {
                                    blnChanged = true;

                                    // Remove encryption.
                                    oSection.SectionInformation.UnprotectSection();
                                    System.Configuration.ConnectionStringSettings connString = new ConnectionStringSettings();
                                    if (0 < oConfiguration.ConnectionStrings.ConnectionStrings.Count)
                                    {
                                        connString =
                                            oConfiguration.ConnectionStrings.ConnectionStrings["SICIEntities"];
                                    }
                                    string con = connString.ConnectionString;
                                    int inicio = con.IndexOf("\"");
                                    int fin = con.IndexOf("\"", inicio + 1);
                                    string conectionString = con.Substring(inicio + 1, fin - inicio - 1);

                                }
                            }
                        }

                        if (blnChanged)
                        {
                            // Indicates whether the associated configuration section 
                            // will be saved even if it has not been modified.
                            oSection.SectionInformation.ForceSave = true;

                            // Save the current configuration.
                            oConfiguration.Save();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw (ex);
            }
            finally
            {
            }
        }

        public static string GetConnectionString()
        {
            string conectionString;

            try
            {
                if (HttpContext.Current != null)
                {
                    conectionString = HttpContext.Current.Application["ConnectionString"] == null ? null : HttpContext.Current.Application["ConnectionString"].ToString();

                }
                else //Si se viene de una opción donde no exista httpcontext
                {
                    conectionString = Globals.ConnString == null ? null : Globals.ConnString;
                }
                if (conectionString != null) return conectionString;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }


            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/ccc.cfg");
            //string path = HttpRuntime.AppDomainAppPath + "ccc.cfg";

            System.IO.FileStream sr = new
                    System.IO.FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

            byte[] bytes = new byte[sr.Length];

            sr.Read(bytes, 0, (int)sr.Length);
            sr.Close();

            int I = 0;
            int II = 0;
            string data1 = "";
            string data2 = "";
            string data3 = "";
            string data4 = "";
            string data5 = "";

            byte[] bdata1;
            byte[] bdata2;
            byte[] bdata3;
            byte[] bdata4;
            byte[] bdata5;



            string ax1 = "data source";
            string ax2 = "initial catalog";
            string ax3 = "persist security info";
            string ax4 = "user id";
            string ax5 = "password";

            string cad1 = "";
            string cad2 = "";

            string cadena = System.Text.Encoding.Default.GetString(bytes);

            //obtener los datos cifrados de la cadena de conexion
            I = cadena.IndexOf(ax1);
            II = cadena.IndexOf(";" + ax2, I);
            cad1 = cadena.Substring(0, I);

            data1 = cadena.Substring(I + ax1.Length + 1, II - (I + ax1.Length + 1));

            I = cadena.IndexOf(ax2);
            II = cadena.IndexOf(";" + ax3, I);
            data2 = cadena.Substring(I + ax2.Length + 1, II - (I + ax2.Length + 1));

            I = cadena.IndexOf(ax3);
            II = cadena.IndexOf(";" + ax4, I);
            data3 = cadena.Substring(I + ax3.Length + 1, II - (I + ax3.Length + 1));

            I = cadena.IndexOf(ax4);
            II = cadena.IndexOf(";" + ax5, I);
            data4 = cadena.Substring(I + ax4.Length + 1, II - (I + ax4.Length + 1));

            I = cadena.IndexOf(ax5);
            II = cadena.IndexOf(";MultipleActiveResultSets", I);
            data5 = cadena.Substring(I + ax5.Length + 1, II - (I + ax5.Length + 1));

            I = II + 1;
            II = cadena.Length - I;
            cad2 = cadena.Substring(I, II);

            //Desencriptar datos
            using (TripleDESCryptoServiceProvider myTripleDES = new TripleDESCryptoServiceProvider())
            {
                byte[] Key = { 115, 96, 94, 217, 148, 212, 105, 222, 20, 6, 167, 52, 243, 3, 153, 144, 123, 183, 121, 25, 217, 65, 132, 161 };
                byte[] IV = { 0, 87, 122, 7, 46, 77, 41, 94 };

                bdata1 = denormalize(data1);
                bdata2 = denormalize(data2);
                bdata3 = denormalize(data3);
                bdata4 = denormalize(data4);
                bdata5 = denormalize(data5);

                data1 = DecryptStringFromBytes(bdata1, Key, IV);
                data2 = DecryptStringFromBytes(bdata2, Key, IV);
                data3 = DecryptStringFromBytes(bdata3, Key, IV);
                data4 = DecryptStringFromBytes(bdata4, Key, IV);
                data5 = DecryptStringFromBytes(bdata5, Key, IV);
            }

            string cc = cad1 + ax1 + "=" + data1 + ";" + ax2 + "=" + data2 + ";" + ax3 + "=" + data3 + ";" + ax4 + "=" + data4 + ";" + ax5 + "=" + data5 + ";" + cad2;

            int inicio = cc.IndexOf("&quot;");
            int fin = cc.IndexOf("&quot;", inicio + 6);
            conectionString = cc.Substring(inicio + 6, fin - inicio - 6);

            Globals.ConnString = conectionString;

            try
            {
                HttpContext.Current.Application["ConnectionString"] = conectionString;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return conectionString;
        }

        private static byte[] denormalize(string encMess)
        {
            string[] chars = encMess.Split(new Char[] { ',' });
            byte[] bytes = new byte[chars.Count()];

            for (int i = 0; i < chars.Count(); i++)
            {
                bytes[i] = (byte)Int16.Parse(chars[i]);
            }

            return bytes;
        }

        private static byte[] denormalize2(string encMess)
        {
            int NoParts = encMess.Length / 3;
            byte[] bytes = new byte[NoParts];

            //Debug.WriteLine("Números recuperados");

            for (int i = 0; i < NoParts; i++)
            {
                var startIndex = i * 3;
                var sbst = encMess.Substring(startIndex, 3);
                //Debug.WriteLine(sbst);
                bytes[i] = (byte)Int16.Parse(sbst);
            }

            return bytes;
        }
        #endregion

        #region Parametros de Seguridad
        public static double SegundosTiempoCaducidad()
        {

            //separar la fecha en dia/mes/anio  y horas/minutos/segundos
            string[] FECHAYHORA = GetSecurityProp("TiempoCaducidad", "30/00/00 00:00:00").Split(new Char[] { ' ' });
            bool aplica = FECHAYHORA.Length == 2;

            int dias = 0;
            int meses = 0;
            int anios = 0;
            int horas = 0;
            int minutos = 0;
            int segundos = 0;

            double total_segundos = 0;

            if (aplica)
            {
                string[] DDMMAA = FECHAYHORA[0].Split(new Char[] { '/' });
                string[] HORA = FECHAYHORA[1].Split(new Char[] { ':' });
                //si ambas cadenas tienen 3 elementos [{dias,meses,anios}{horas,minutos,segundos}]
                if (DDMMAA.Length == 3 && HORA.Length == 3)
                {
                    dias = Int32.Parse(DDMMAA[0]);
                    /*meses = Int32.Parse(DDMMAA[1]);
                    anios = Int32.Parse(DDMMAA[2]);
                    horas = Int32.Parse(HORA[0]);
                    minutos = Int32.Parse(HORA[1]);
                    segundos = Int32.Parse(HORA[2]);*/

                    total_segundos = segundos
                        + (minutos * 60)
                        + (horas * 3600)
                        + (dias * 86400)
                        + (meses * 2592000)
                        + (anios * 31536000);

                    return total_segundos;

                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        public static double SegundosBloqueoNoIngreso()
        {

            //separar la fecha en dia/mes/anio  y horas/minutos/segundos
            string[] FECHAYHORA = GetSecurityProp("BloqueoNoIngreso", "30/00/00 00:00:00").Split(new Char[] { ' ' });
            bool aplica = FECHAYHORA.Length == 2;

            int dias = 0;
            int meses = 0;
            int anios = 0;
            int horas = 0;
            int minutos = 0;
            int segundos = 0;

            double total_segundos = 0;

            if (aplica)
            {
                string[] DDMMAA = FECHAYHORA[0].Split(new Char[] { '/' });
                string[] HORA = FECHAYHORA[1].Split(new Char[] { ':' });
                //si ambas cadenas tienen 3 elementos [{dias,meses,anios}{horas,minutos,segundos}]
                if (DDMMAA.Length == 3 && HORA.Length == 3)
                {
                    dias = Int32.Parse(DDMMAA[0]);
                    /*meses = Int32.Parse(DDMMAA[1]);
                    anios = Int32.Parse(DDMMAA[2]);
                    horas = Int32.Parse(HORA[0]);
                    minutos = Int32.Parse(HORA[1]);
                    segundos = Int32.Parse(HORA[2]);*/

                    total_segundos = segundos
                        + (minutos * 60)
                        + (horas * 3600)
                        + (dias * 86400)
                        + (meses * 2592000)
                        + (anios * 31536000);

                    return total_segundos;

                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an TripleDESCryptoServiceProvider object
            // with the specified key and IV.
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.Key = Key;
                tdsAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = tdsAlg.CreateDecryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        public static string getKey()
        {
            if (string.IsNullOrEmpty(Globals.SystemKey))
            {
                //Identificador unico
                ManagementObjectCollection mbsList = null;
                ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
                mbsList = mbs.Get();
                string id = "";
                foreach (ManagementObject mo in mbsList)
                {
                    id = mo["ProcessorID"].ToString();
                }

                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectCollection moc = mos.Get();
                string motherBoard = "";
                foreach (ManagementObject mo in moc)
                {
                    motherBoard = (string)mo["SerialNumber"];
                }

                string myUniqueID = id + motherBoard;
                //Console.WriteLine(myUniqueID);
                //var macAddr =
                //            (
                //                from nic in NetworkInterface.GetAllNetworkInterfaces()
                //                where nic.OperationalStatus == OperationalStatus.Up
                //                select nic.GetPhysicalAddress().ToString()
                //            ).FirstOrDefault();




                //var macCipher = SeguridadUtilidades.SHA256Encripta(macAddr);
                //var subs24 = macCipher.Substring(0, 24);

                myUniqueID += "00000000000000000000000";
                var subs24 = myUniqueID.Substring(0, 24);

                Globals.SystemKey = subs24;
            }


            return Globals.SystemKey;
        }

        public static string SDFK(string Key, string cipherText)
        {
            byte[] key = Encoding.ASCII.GetBytes(Key);
            byte[] encMess = denormalize2(cipherText);
            byte[] IV = { 0, 87, 122, 7, 46, 77, 41, 94 };

            var plainText = DecryptStringFromBytes(encMess, key, IV);

            return plainText;
        }

        public static string SCFK(string Key, string plainText)
        {
            var encString = "";
            byte[] encrypted;

            using (TripleDESCryptoServiceProvider myTripleDES = new TripleDESCryptoServiceProvider())
            {
                byte[] key = Encoding.ASCII.GetBytes(Key);
                byte[] IV = { 0, 87, 122, 7, 46, 77, 41, 94 };

                // Encrypt the string to an array of bytes.
                encrypted = EncryptStringToBytes(plainText, key, IV);

                encString = normalize(encrypted);//convertir de caracteres a valores numericos
            }

            return encString;
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an TripleDESCryptoServiceProvider object
            // with the specified key and IV.
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.Key = Key;
                tdsAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = tdsAlg.CreateEncryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string normalize(byte[] encMess)
        {
            string result = "";
            foreach (byte ch in encMess)
            {
                string naux = ((int)ch).ToString();
                //Debug.WriteLine(naux.ToString().PadLeft(3, '0'));
                result += naux.ToString().PadLeft(3, '0');
            }
            return result;
        }

        public static string GetSecurityProp(string nombre, string defaultValue = "")
        {
            db = new SICIEntities();

            string aux;
            c_parametro parametro;

            try
            {
                parametro = db.c_parametro.Where(p => p.nb_parametro == nombre).First();
                aux = parametro.valor_parametro;
            }
            catch
            {
                parametro = new c_parametro();
                parametro.nb_parametro = nombre;
                parametro.valor_parametro = defaultValue;
                db.c_parametro.Add(parametro);
                db.SaveChanges();

                return defaultValue;
            }

            return aux;
        }

        //Intenta devolver un valor entero obtenido desde una cadena, en caso de error devuelve 0
        public static int GetIntSecurityProp(string nombre, string defaultValue = "")
        {
            string value = GetSecurityProp(nombre, defaultValue);
            int result;
            return Int32.TryParse(value, out result) ? result : 0;
        }

        //Intenta devolver un valor booleano obtenido desde una cadena, en caso de error devuelve false
        public static bool GetBoolSecurityProp(string nombre, string defaultValue = "")
        {
            string value = GetSecurityProp(nombre, defaultValue);
            return value.ToLower() == "true" ? true : false;
        }

        public static bool SetSecurityProp(string nombre, string value)
        {
            try
            {
                c_parametro parametro = db.c_parametro.Where(p => p.nb_parametro == nombre).First();
                parametro.valor_parametro = value;
                db.SaveChanges();
                return true;
            }
            catch
            {
                c_parametro parametro = new c_parametro();
                parametro.nb_parametro = nombre;
                parametro.valor_parametro = value;
                db.c_parametro.Add(parametro);
                db.SaveChanges();
                return false;
            }
        }
        #endregion

        #region Campos extra
        public static List<c_meta_campo> infoCamposExtra(string nb_entidad, int no_campos)
        {
            SICIEntities db = new SICIEntities();
            if (db.c_meta_campo.Any(mc => mc.nb_entidad == nb_entidad))
            {
                var lista = db.c_meta_campo.Where(mc => mc.nb_entidad == nb_entidad).OrderBy(mc => mc.cl_campo).ToList();
                if (lista.Count == no_campos) return lista;
                else
                {
                    //En caso de que no existan los "n" campos solicitados, eliminamos todos, volvemos a crear y enviamos la respuesta.
                    foreach (var mc in lista)
                    {
                        db.c_meta_campo.Remove(mc);
                    }
                }
            }
            //creacion de los "n" metacampos con sus atributos por defecto
            for (int i = 1; i <= no_campos; i++)
            {
                var mc = new c_meta_campo();
                mc.nb_entidad = nb_entidad;
                mc.cl_campo = "c" + string.Format("{0:00}", i);
                mc.cl_color_borde = "0";
                mc.cl_color_fondo = "0";
                mc.cl_tipo_campo = "T";
                mc.msg_ayuda = "Mensaje no cargado.";
                mc.aparece_en_mg = false;
                mc.es_editable = false;
                mc.es_requerido = false;
                mc.es_visible = false;
                mc.longitud_campo = 255; //Longitud maximá por defecto
                mc.nb_campo = "Campo extra " + string.Format("{0:00}", i);
                db.c_meta_campo.Add(mc);
            }
            db.SaveChanges();
            return db.c_meta_campo.Where(mc => mc.nb_entidad == nb_entidad).OrderBy(mc => mc.cl_campo).ToList();
        }

        public static string[] valCamposExtra(string nb_entidad, int no_campos, int id)
        {
            db = new SICIEntities();
            string[] res = new string[20];
            Type m_tipo = null;
            PropertyInfo[] m_propiedades = null;
            int ini = 0;

            if (nb_entidad == "k_riesgo")
            {
                var r = db.k_riesgo.Find(id);
                m_tipo = r.GetType();
                m_propiedades = m_tipo.GetProperties();

                foreach (var prop in m_propiedades)
                {
                    if (prop.Name == "campo01") break;
                    ini++;
                }

                for (int i = 0; i < no_campos; i++)
                {
                    var propiedad = m_propiedades[i + ini];
                    res[i] = ToUnicode((string)propiedad.GetValue(r, null) ?? "");
                }
            }
            if (nb_entidad == "k_control")
            {
                var r = db.k_control.Find(id);
                m_tipo = r.GetType();
                m_propiedades = m_tipo.GetProperties();
                foreach (var prop in m_propiedades)
                {
                    if (prop.Name == "campo01") break;
                    ini++;
                }
                for (int i = 0; i < no_campos; i++)
                {
                    var propiedad = m_propiedades[i + ini];
                    res[i] = ToUnicode(propiedad.GetValue(r, null).ToString() ?? "");
                }
            }
            if (nb_entidad == "c_sub_proceso")
            {
                var r = db.c_sub_proceso.Find(id);
                m_tipo = r.GetType();
                m_propiedades = m_tipo.GetProperties();
                foreach (var prop in m_propiedades)
                {
                    if (prop.Name == "campo01") break;
                    ini++;
                }
                for (int i = 0; i < no_campos; i++)
                {
                    var propiedad = m_propiedades[i + ini];
                    res[i] = ToUnicode(propiedad.GetValue(r, null).ToString() ?? "");
                }
            }
            return res;
        }

        #endregion

        #region Copiado de Objetos
        public static object CopyObject(object from, object to)
        {
            Type fromType, toType;
            PropertyInfo[] fromProps, toProps;

            fromType = from.GetType();
            toType = to.GetType();

            fromProps = fromType.GetProperties();
            toProps = toType.GetProperties();

            foreach (var prop in fromProps)
            {
                var propDest = toProps.Where(p => p.Name == prop.Name).FirstOrDefault();
                if (propDest != null)
                {
                    var value = prop.GetValue(from, null);
                    propDest.SetValue(to, value, null);
                }
            }

            return to;
        }

        #endregion

        #region Otros
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static DirectionViewModel getDirection(string direction)
        {
            string[] splitString = direction.Split(new Char[] { '/' });
            DirectionViewModel dir = new DirectionViewModel();
            dir.Controller = splitString[0];
            dir.Action = splitString[1];
            dir.Id = splitString[2];
            return dir;
        }

        public static IList<SelectListItem> TiposCampo()
        {
            IList<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem { Value = "t", Text = "Texto" });
            lista.Add(new SelectListItem { Value = "a", Text = "Área de Texto" });
            lista.Add(new SelectListItem { Value = "n", Text = "Numérico" });

            return lista;
        }

        public static IList<SelectListItem> ColoresMetaCampos()
        {
            IList<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem { Value = "0", Text = "Ninguno" });
            lista.Add(new SelectListItem { Value = "AliceBlue", Text = "Azul Celeste" });
            lista.Add(new SelectListItem { Value = "AntiqueWhite", Text = "Blanco Antiguo" });
            lista.Add(new SelectListItem { Value = "Aqua", Text = "Agua" });
            lista.Add(new SelectListItem { Value = "Aquamarine", Text = "Agua Marina" });
            lista.Add(new SelectListItem { Value = "Beige", Text = "Beige" });
            lista.Add(new SelectListItem { Value = "Black", Text = "Negro" });
            lista.Add(new SelectListItem { Value = "BlanchedAlmond", Text = "Almendra" });
            lista.Add(new SelectListItem { Value = "Blue", Text = "Azul" });
            lista.Add(new SelectListItem { Value = "BlueViolet", Text = "Violeta" });
            lista.Add(new SelectListItem { Value = "Brown", Text = "Café" });
            lista.Add(new SelectListItem { Value = "CadetBlue", Text = "Azul cadete" });
            lista.Add(new SelectListItem { Value = "Chocolate", Text = "Chocolate" });
            lista.Add(new SelectListItem { Value = "Coral", Text = "Coral" });
            lista.Add(new SelectListItem { Value = "CornflowerBlue", Text = "Azul Anciano" });
            lista.Add(new SelectListItem { Value = "Crimson", Text = "Carmesí" });
            lista.Add(new SelectListItem { Value = "Cyan", Text = "Cian" });
            lista.Add(new SelectListItem { Value = "DarkBlue", Text = "Azul Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkCyan", Text = "Cian Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkGoldenRod", Text = "Dorado Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkGray", Text = "Gris Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkGreen", Text = "Verde Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkMagenta", Text = "Violeta Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkOrange ", Text = "Naranja Oscuro" });
            lista.Add(new SelectListItem { Value = "DarkRed ", Text = "Rojo Oscuro" });
            lista.Add(new SelectListItem { Value = "DeepSkyBlue ", Text = "Azul cielo profundo" });
            lista.Add(new SelectListItem { Value = "Gray", Text = "Gris" });
            lista.Add(new SelectListItem { Value = "Gold ", Text = "Amarillo" });
            lista.Add(new SelectListItem { Value = "Green ", Text = "Verde" });
            lista.Add(new SelectListItem { Value = "GreenYellow ", Text = "Verde Limón" });
            lista.Add(new SelectListItem { Value = "Red", Text = "Rojo" });
            lista.Add(new SelectListItem { Value = "Salmon", Text = "Salmón" });
            lista.Add(new SelectListItem { Value = "White", Text = "Blanco" });


            return lista;
        }

        public static string getFontColor(string bgc)
        {
            var color = "";
            switch (bgc)
            {
                case "0":
                    color = "black";
                    break;
                case "AliceBlue":
                    color = "black";
                    break;
                case "AntiqueWhite":
                    color = "black";
                    break;
                case "Aqua":
                    color = "black";
                    break;
                case "Aquamarine":
                    color = "black";
                    break;
                case "Beige":
                    color = "black";
                    break;
                case "Black":
                    color = "white";
                    break;
                case "BlanchedAlmond":
                    color = "black";
                    break;
                case "Blue":
                    color = "white";
                    break;
                case "BlueViolet":
                    color = "white";
                    break;
                case "Brown":
                    color = "white";
                    break;
                case "CadetBlue":
                    color = "white";
                    break;
                case "Chocolate":
                    color = "white";
                    break;
                case "Coral":
                    color = "white";
                    break;
                case "CornflowerBlue":
                    color = "white";
                    break;
                case "Crimson":
                    color = "white";
                    break;
                case "Cyan":
                    color = "black";
                    break;
                case "DarkBlue":
                    color = "white";
                    break;
                case "DarkCyan":
                    color = "white";
                    break;
                case "DarkGoldenRod":
                    color = "white";
                    break;
                case "DarkGray":
                    color = "white";
                    break;
                case "DarkGreen":
                    color = "white";
                    break;
                case "DarkMagenta":
                    color = "white";
                    break;
                case "DarkOrange":
                    color = "white";
                    break;
                case "DarkRed":
                    color = "white";
                    break;
                case "DeepSkyBlue":
                    color = "black";
                    break;
                case "Gray":
                    color = "white";
                    break;
                case "Gold":
                    color = "black";
                    break;
                case "Green":
                    color = "white";
                    break;
                case "GreenYellow":
                    color = "black";
                    break;
                case "Red":
                    color = "white";
                    break;
                case "Salmon":
                    color = "white";
                    break;
                case "White":
                    color = "black";
                    break;
                default:
                    color = "black";
                    break;
            }
            return color;
        }

        public static string ToUnicode(string input)
        {
            byte[] unibyte = Encoding.Unicode.GetBytes(input);
            string uniString = string.Empty;
            foreach (byte b in unibyte)
            {
                uniString += string.Format("{0}{1}", @"\u", b.ToString("X"));
            }
            return uniString;
        }


        public static string getLastConnection()
        {
            //string lc;
            //try
            //{
            //    lc = HttpContext.Current.Application["UltimoAcceso"].ToString();
            //}
            //catch
            //{
            //    lc = "--/--/----";
            //}
            //return lc;

            return ((HttpContext.Current.Application["UltimoAcceso"]) != null ? HttpContext.Current.Application["UltimoAcceso"].ToString() : "--/--/----");
        }

        public static string GetDateFormat()
        {
            var lang = Strings.getMSG("DateFormat");
            return lang;
            //string lang = HttpContext.Current.Request.UserLanguages[0];
            //if (lang.ToLower().Contains("en"))
            //{
            //    //return "DD/MM/YYYY";
            //    return "MM/DD/YYYY";
            //}
            //else
            //{
            //    return "DD/MM/YYYY";
            //}
        }

        public static int NRiesgosPorCriticidad(int id_probabilidad_ocurrencia, int id_magnitud_impacto, int id_us)
        {
            db = new SICIEntities();

            var us = db.c_usuario.Find(id_us);

            var Riesgos = RTCRiesgo(us, db);
            var riesgos = Riesgos.Where(c => c.id_magnitud_impacto == id_magnitud_impacto && c.id_probabilidad_ocurrencia == id_probabilidad_ocurrencia).ToList();
            return riesgos.Count();
        }

        public static string TutorialMessage(string clave)
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/Plantillas/TutorialMessages.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Obtenemos el elemento "Messages"
            var mensajes = doc.SelectSingleNode("Messages");

            string msg = "";
            try
            {
                var mensaje = mensajes.SelectSingleNode(clave);
                msg = mensaje.InnerText;
            }
            catch
            {

            }
            return msg;
        }

        static public string NormalizeCron(string cron)
        {
            var res = "";

            var splitedCron = cron.Split(new char[] { ' ' });
            res = splitedCron[0]
                + " " + splitedCron[1]
                + " " + splitedCron[2]
                + " " + splitedCron[3]
                + " " + splitedCron[4].Replace('7', '0');

            return res;
        }


        static public string[] getColorArray(int Ncolors)
        {
            string[] arrayRes;
            string[] colorsBase =
            {
                "#1CA9E6",
                "#5CE61C",
                "#E69A0B",
                "#E62B17",
                "#AF00FA",
                "#0DFDF4",
                "#C3FD0D",
                "#FD8205",
                "#FD0850",
                "#6918AC",
                "#570DFD",
                "#0DFDD1",
                "#DCFD05",
                "#FDA208",
                "#6500FC",
                "#040AC2",
                "#04C25B",
                "#C2B10A",
                "#C25B00",
                "#FF0DFF",
                "#005DFA",
                "#01FA1B",
                "#FAC611",
                "#FA4904",
                "#A504C2"
            };

            if (Ncolors <= 25) arrayRes = colorsBase.Take(Ncolors).ToArray();
            else
            {
                int NRepeats = Ncolors / 25;
                int extra = Ncolors % 25;
                arrayRes = new string[Ncolors];

                for (int i = 0; i < NRepeats; i++)
                {
                    colorsBase.CopyTo(arrayRes, i * 25);
                }
                colorsBase.Take(extra).ToArray().CopyTo(arrayRes, NRepeats * 25);

            }


            return arrayRes;
        }



        #endregion

        #region Riesgos y Controles

        public static string[] RRData(k_control control)
        {
            string[] data = new string[2];
            if (control.k_riesgo_residual.Count > 0)
            {
                var Model = db.a_campo_cobertura_control.ToList();
                var rr = control.k_riesgo_residual.First();


                decimal aux11 = 0; a_campo_cobertura_control campo11 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux12 = 0; a_campo_cobertura_control campo12 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux13 = 0; a_campo_cobertura_control campo13 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux14 = 0; a_campo_cobertura_control campo14 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux15 = 0; a_campo_cobertura_control campo15 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux16 = 0; a_campo_cobertura_control campo16 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux17 = 0; a_campo_cobertura_control campo17 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux18 = 0; a_campo_cobertura_control campo18 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux19 = 0; a_campo_cobertura_control campo19 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };

                decimal aux21 = 0; a_campo_cobertura_control campo21 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux22 = 0; a_campo_cobertura_control campo22 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux23 = 0; a_campo_cobertura_control campo23 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux24 = 0; a_campo_cobertura_control campo24 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };
                decimal aux25 = 0; a_campo_cobertura_control campo25 = new a_campo_cobertura_control { nb_campo = "No se encontró ningún registro" };

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 1))
                {
                    aux11 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 1).Max(c => c.valor);
                    campo11 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 1 && c.valor == aux11).First();
                }


                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 2))
                {
                    aux12 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 2).Max(c => c.valor);
                    campo12 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 2 && c.valor == aux12).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 3))
                {
                    aux13 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 3).Max(c => c.valor);
                    campo13 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 3 && c.valor == aux13).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 4))
                {
                    aux14 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 4).Max(c => c.valor);
                    campo14 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 4 && c.valor == aux14).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 5))
                {
                    aux15 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 5).Max(c => c.valor);
                    campo15 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 5 && c.valor == aux15).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 6))
                {
                    aux16 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 6).Max(c => c.valor);
                    campo16 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 6 && c.valor == aux16).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 7))
                {
                    aux17 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 7).Max(c => c.valor);
                    campo17 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 7 && c.valor == aux17).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 8))
                {
                    aux18 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 8).Max(c => c.valor);
                    campo18 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 8 && c.valor == aux18).First();
                }

                if (Model.Any(c => c.cl_catalogo == 1 && c.cl_campo == 9))
                {
                    aux19 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 9).Max(c => c.valor);
                    campo19 = Model.Where(c => c.cl_catalogo == 1 && c.cl_campo == 9 && c.valor == aux19).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 1))
                {
                    aux21 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 1).Max(c => c.valor);
                    campo21 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 1 && c.valor == aux21).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 2))
                {
                    aux22 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 2).Max(c => c.valor);
                    campo22 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 2 && c.valor == aux22).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 3))
                {
                    aux23 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 3).Max(c => c.valor);
                    campo23 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 3 && c.valor == aux23).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 4))
                {
                    aux24 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 4).Max(c => c.valor);
                    campo24 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 4 && c.valor == aux24).First();
                }

                if (Model.Any(c => c.cl_catalogo == 2 && c.cl_campo == 5))
                {
                    aux25 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 5).Max(c => c.valor);
                    campo25 = Model.Where(c => c.cl_catalogo == 2 && c.cl_campo == 5 && c.valor == aux25).First();
                }

                var total1 = aux11 + aux12 + aux13 + aux14 + aux15 + aux16 + aux17 + aux18 + aux19;
                var total2 = aux21 + aux22 + aux23 + aux24 + aux25;

                var totalr1 = rr.a_campo_cobertura_control.valor
                    + rr.a_campo_cobertura_control1.valor
                    + rr.a_campo_cobertura_control2.valor
                    + rr.a_campo_cobertura_control3.valor
                    + rr.a_campo_cobertura_control4.valor
                    + rr.a_campo_cobertura_control5.valor
                    + rr.a_campo_cobertura_control6.valor
                    + rr.a_campo_cobertura_control7.valor
                    + rr.a_campo_cobertura_control8.valor;

                var totalr2 = rr.a_campo_cobertura_control9.valor
                    + rr.a_campo_cobertura_control10.valor
                    + rr.a_campo_cobertura_control11.valor
                    + rr.a_campo_cobertura_control12.valor
                    + rr.a_campo_cobertura_control13.valor;

                data[0] = string.Format("Diseño del Control {0:0.0}% de {1:0.0}%", totalr1, total1);
                data[1] = string.Format("Efectividad del Control {0:0.0}% de {1:0.0}%", totalr2, total2);
            }
            else
            {
                data[0] = null;
                data[1] = null;
            }

            return data;
        }


        public static string RCodeGen(c_sub_proceso sp)
        {
            string code = "";
            c_parametro parametro = new c_parametro();

            try
            {
                parametro = db.c_parametro.Where(pr => pr.nb_parametro == sp.id_sub_proceso.ToString() + "R").First();
                int valor = int.Parse(parametro.valor_parametro) + 1;
                code = "R" + sp.cl_sub_proceso + string.Format("-{0:00}", valor);
                parametro.valor_parametro = valor.ToString();
                db.SaveChanges();
                return code;
            }
            catch
            {

            }

            int maxCode = 0;
            var riesgos = sp.k_riesgo.ToList();
            if (riesgos.Count() > 0)
            {
                foreach (var riesgo in riesgos)
                {
                    var c1 = riesgo.nb_riesgo.Split(new char[] { '-' });
                    if (c1.Count() == 2)
                    {
                        int a1;
                        maxCode = int.TryParse(c1[1], out a1) ? (a1 > maxCode ? a1 : maxCode) : maxCode;
                    }
                }
                code = "R" + sp.cl_sub_proceso + "-" + string.Format("{0:00}", maxCode + 1);
            }
            else
            {
                code = "R" + sp.cl_sub_proceso + "-01";
            }

            c_parametro p = new c_parametro();
            p.nb_parametro = sp.id_sub_proceso.ToString() + "R";
            p.valor_parametro = (maxCode + 1).ToString();
            db.c_parametro.Add(p);
            db.SaveChanges();

            return code;
        }

        public static string CCodeGen(c_sub_proceso sp)
        {
            string code = "";
            c_parametro parametro = new c_parametro();

            try
            {
                parametro = db.c_parametro.Where(pr => pr.nb_parametro == sp.id_sub_proceso.ToString() + "C").First();
                int valor = int.Parse(parametro.valor_parametro) + 1;
                code = "C" + sp.cl_sub_proceso + string.Format("-{0:00}", valor);
                parametro.valor_parametro = valor.ToString();
                db.SaveChanges();
                return code;
            }
            catch
            {

            }

            int maxCode = 0;
            var controles = sp.k_control.Where(c => c.k_riesgo.Count() == 1).ToList();
            if (controles.Count() > 0)
            {
                foreach (var control in controles)
                {
                    string s1;
                    try
                    {
                        s1 = control.relacion_control;
                        var c1 = s1.Split(new char[] { '-' });
                        if (c1.Count() == 2)
                        {
                            int a1;
                            maxCode = int.TryParse(c1[1], out a1) ? (a1 > maxCode ? a1 : maxCode) : maxCode;
                        }
                    }
                    catch
                    {
                        s1 = "";
                    }
                }
                code = "C" + sp.cl_sub_proceso + "-" + string.Format("{0:00}", maxCode + 1);
            }
            else
            {
                code = "C" + sp.cl_sub_proceso + "-01";
            }

            c_parametro p = new c_parametro();
            p.nb_parametro = sp.id_sub_proceso.ToString() + "C";
            p.valor_parametro = (maxCode + 1).ToString();
            db.c_parametro.Add(p);
            db.SaveChanges();

            return code;
        }

        public static void disposeRParam(string rCode, int id_sp)
        {
            int val = int.Parse(rCode.Split(new char[] { '-' })[1]);
            string nb = id_sp.ToString() + "R";

            try
            {
                var parametro = db.c_parametro.Where(c => c.nb_parametro == nb && c.valor_parametro == val.ToString()).First();
                db.c_parametro.Remove(parametro);
                db.SaveChanges();
            }
            catch
            {

            }
            return;
        }

        public static void disposeCParam(string cCode, int id_sp)
        {
            int val = int.Parse(cCode.Split(new char[] { '-' })[1]);
            string nb = id_sp.ToString() + "C";

            try
            {
                var parametro = db.c_parametro.Where(c => c.nb_parametro == nb && c.valor_parametro == val.ToString()).First();
                db.c_parametro.Remove(parametro);
                db.SaveChanges();
            }
            catch
            {

            }
            return;
        }

        public static void ValidateCR()
        {
            db = new SICIEntities();
            if (db.c_criticidad_riesgo.ToList().Count == 0)
            {
                c_criticidad_riesgo cr = new c_criticidad_riesgo();
                cr.cl_color_campo = "0";
                cr.cl_criticidad_riesgo = "N/A";
                cr.nb_criticidad_riesgo = "Ninguno";
            }
            return;
        }

        public static void ValidateCR1()
        {
            db = new SICIEntities();
            if (db.c_criticidad_normatividad.ToList().Count == 0)
            {
                c_criticidad_normatividad cr = new c_criticidad_normatividad();
                cr.cl_color_campo = "0";
                cr.cl_criticidad_normatividad = "N/A";
                cr.nb_criticidad_normatividad = "Ninguno";
            }
            return;
        }

        public static void ValidateCRR()
        {
            db = new SICIEntities();
            if (db.c_criticidad_riesgo.ToList().Count == 0)
            {
                c_criticidad_riesgo cr = new c_criticidad_riesgo();
                cr.cl_color_campo = "0";
                cr.cl_criticidad_riesgo = "N/A";
                cr.nb_criticidad_riesgo = "Ninguno";
            }
            return;
        }

        public static int idOfFirsCrit(SICIEntities db)
        {
            try
            {
                return db.c_criticidad_riesgo.First().id_criticidad_riesgo;
            }
            catch
            {
                c_criticidad_riesgo crit = new c_criticidad_riesgo()
                {
                    cl_criticidad_riesgo = "01",
                    nb_criticidad_riesgo = "Default",
                    cl_color_campo = "0"
                };

                db.c_criticidad_riesgo.Add(crit);
                db.SaveChanges();
                return crit.id_criticidad_riesgo;
            }
        }

        public static int idOfFirsCrit1(SICIEntities db)
        {
            try
            {
                return db.c_criticidad_normatividad.First().id_criticidad_normatividad;
            }
            catch
            {
                c_criticidad_normatividad crit = new c_criticidad_normatividad()
                {
                    cl_criticidad_normatividad = "01",
                    nb_criticidad_normatividad = "Default",
                    cl_color_campo = "0"
                };

                db.c_criticidad_normatividad.Add(crit);
                db.SaveChanges();
                return crit.id_criticidad_normatividad;
            }
        }
        #endregion

        #region BDEI
        public static int idOfFirsCritRO(SICIEntities db)
        {
            try
            {
                return db.c_criticidad_riesgo_ro.First().id_criticidad_riesgo_ro;
            }
            catch
            {
                c_criticidad_riesgo_ro crit = new c_criticidad_riesgo_ro()
                {
                    cl_criticidad_riesgo_ro = "01",
                    nb_criticidad_riesgo_ro = "Default",
                    cl_color_campo = "0"
                };

                db.c_criticidad_riesgo_ro.Add(crit);
                db.SaveChanges();
                return crit.id_criticidad_riesgo_ro;
            }
        }

        public static string Criticidad(k_bdei model)
        {
            db = new SICIEntities();

            if (model.c_frecuencia_riesgo_operacional != null && model.c_impacto_riesgo_operacional != null)
                return db.c_criticidad_ro.Where(c => c.id_frecuencia_riesgo_operacional == model.id_frecuencia_riesgo_operacional && c.id_impacto_riesgo_operacional == model.id_impacto_riesgo_operacional).First().c_criticidad_riesgo_ro.nb_criticidad_riesgo_ro;
            else
                return "";
        }

        public static string Criticidad(int idfr, int idi, bool conClave = false)
        {
            db = new SICIEntities();


            //Debug.WriteLine("Buscando criticidad para idf: " + idfr + " idi: " + idi);
            var crt = db.c_criticidad_ro.Where(c => c.id_frecuencia_riesgo_operacional == idfr && c.id_impacto_riesgo_operacional == idi).First();

            string criticidad = "";
            if (conClave)
            {
                criticidad = crt.c_criticidad_riesgo_ro.cl_criticidad_riesgo_ro + " - ";
            }
            criticidad += crt.c_criticidad_riesgo_ro.nb_criticidad_riesgo_ro;

            return criticidad;
        }

        #endregion

        #region Organigrama
        public static string getChildData(c_puesto puesto, SICIEntities db)
        {
            string data = "";
            string dataEnd = "}";

            //obtener hijos de este nodo:
            var hijos = db.c_puesto.Where(ph => ph.id_puesto_padre == puesto.id_puesto).ToList();
            if (hijos.Count > 0)
            {
                int THijos = hijos.Count;
                dataEnd = ",'children': [";
                for (int i = 0; i < THijos; i++)
                {
                    var hijo = hijos.ElementAt(i);
                    dataEnd += getChildData(hijo, db);
                    if (i != THijos - 1)
                    {
                        dataEnd += ",";
                    }
                    else
                    {
                        dataEnd += "]}";
                    }
                }
            }

            data = "{'name': '" + EliminarComillas(puesto.cl_puesto) + "','title': '" + EliminarComillas(puesto.nb_puesto) + "', 'id':'" + puesto.id_puesto + "'" + dataEnd;
            return data;
        }

        public static string getChildData(c_contenido_manual manual, SICIEntities db)
        {
            string data = "";
            string dataEnd = "}";

            //obtener hijos de este nodo:
            var hijos = db.c_contenido_manual.Where(c => c.id_contenido_manual_padre == manual.id_contenido_manual).OrderBy(c => c.no_orden).ToList();
            if (hijos.Count > 0)
            {
                int THijos = hijos.Count;
                dataEnd = ",'children': [";
                for (int i = 0; i < THijos; i++)
                {
                    var hijo = hijos.ElementAt(i);
                    dataEnd += getChildData(hijo, db);
                    if (i != THijos - 1)
                    {
                        dataEnd += ",";
                    }
                    else
                    {
                        dataEnd += "]}";
                    }
                }
            }

            var nivel = db.c_nivel_manual.Find(manual.id_nivel_manual);

            data = "{'name': '" + EliminarComillas(nivel.nb_nivel_manual) + "','title': '" + EliminarComillas(manual.cl_contenido_manual) + "', 'id':'" + manual.id_contenido_manual + "'" + dataEnd;
            return data;
        }

        public static c_puesto getRoot(SICIEntities db)
        {
            c_puesto puestoRaiz;

            try
            {
                puestoRaiz = db.c_puesto.Where(p => p.id_puesto_padre == null).First();
            }
            catch
            {
                puestoRaiz = null;
            }

            if (puestoRaiz == null)
            {
                db.c_puesto.Add(new c_puesto() { cl_puesto = "01", nb_puesto = "Raíz", se_notifica = true });
                db.SaveChanges();
                puestoRaiz = db.c_puesto.Where(p => p.id_puesto_padre == null).First();
            }

            return puestoRaiz;
        }

        public static c_contenido_manual getRoot(SICIEntities db, c_contenido_manual contenido)
        {
            c_contenido_manual cont = contenido;


            while (cont.id_contenido_manual_padre != null)
            {
                cont = db.c_contenido_manual.Find(cont.id_contenido_manual_padre);
            }

            return cont;
        }

        public static List<c_puesto> puestosInferiores(c_puesto puesto, SICIEntities db)
        {
            List<c_puesto> lista = new List<c_puesto>();

            try
            {
                //obtenemos hijos
                var hijos = db.c_puesto.Where(ph => ph.id_puesto_padre == puesto.id_puesto).ToList();
                //añadimos los hijos a la lista y buscamos los descendientes de cada hijo
                foreach (var hijo in hijos)
                {
                    lista.Add(hijo);
                    var inferiores = puestosInferiores(hijo, db);
                    lista = lista.Union(inferiores).ToList();

                }
            }
            catch
            {
                return new List<c_puesto>();
            }
            return lista;
        }

        public static List<c_puesto> puestosSuperiores(c_puesto puesto, SICIEntities db)
        {
            List<c_puesto> lista = new List<c_puesto>();

            try
            {
                while (puesto.id_puesto_padre != null)
                {
                    puesto = puesto.c_puesto2;
                    lista.Add(puesto);
                }
            }
            catch
            {
                return new List<c_puesto>();
            }
            return lista;
        }

        public static List<c_contenido_manual> manualesInferiores(c_contenido_manual manual)
        {
            List<c_contenido_manual> lista = new List<c_contenido_manual>();

            try
            {
                //obtenemos hijos
                var hijos = db.c_contenido_manual.Where(ph => ph.id_contenido_manual_padre == manual.id_contenido_manual).ToList();
                //añadimos los hijos a la lista y buscamos los descendientes de cada hijo
                foreach (var hijo in hijos)
                {
                    lista.Add(hijo);
                    var inferiores = manualesInferiores(hijo);
                    lista = lista.Union(inferiores).ToList();

                }
            }
            catch
            {
                return new List<c_contenido_manual>();
            }
            return lista;
        }

        public static List<c_usuario> usuariosPorPuestos(List<c_puesto> puestos)
        {
            var lista = new List<c_usuario>();

            foreach (var puesto in puestos)
            {
                var upp = puesto.c_usuario.ToList(); //usuarios por puesto
                lista = lista.Union(upp).ToList();
            }

            return lista;
        }

        public static string PuestoUsuario(int id)
        {
            string puesto;
            try
            {
                var user = db.c_usuario.Find(id);

                if (user.c_puesto.Count > 0)
                    puesto = user.c_puesto.First().nb_puesto;
                else
                    puesto = "N/A";
            }
            catch
            {
                puesto = "N/A";
            }

            return puesto;
        }

        public static string JefeDirecto(int id)
        {
            string res = "";
            c_puesto puesto;
            try
            {
                var user = db.c_usuario.Find(id);
                puesto = user.c_puesto.First();

                try
                {
                    var puestoPadre = db.c_puesto.Find(puesto.id_puesto_padre);
                    var users = puestoPadre.c_usuario.ToList();
                    if (users.Count > 0)
                    {
                        foreach (var u in users)
                        {
                            res += u.nb_usuario + "\n";
                        }
                    }
                    else
                    {
                        res = "N/A";
                    }
                }
                catch
                {
                    res = "N/A";
                }

            }
            catch
            {
                res = "N/A";
            }

            return res;
        }

        public static string PuestoJefeDirecto(int id)
        {
            string res = "";
            c_puesto puesto;
            try
            {
                var user = db.c_usuario.Find(id);
                puesto = user.c_puesto.First();

                try
                {
                    var puestoPadre = db.c_puesto.Find(puesto.id_puesto_padre);

                    res = puestoPadre.nb_puesto;
                }
                catch
                {
                    res = "N/A";
                }

            }
            catch
            {
                res = "N/A";
            }

            return res;
        }

        public static List<c_usuario> TramoControlSuperior(int id, SICIEntities db)
        {
            var us = db.c_usuario.Find(id);
            c_puesto puesto;
            try
            {
                puesto = us.c_puesto.First();
            }
            catch
            {
                puesto = null;
            }

            var res = new List<c_usuario>();

            if (puesto != null)
            {
                var ps = puestosSuperiores(puesto, db);

                foreach (var p in ps)
                {
                    res.AddRange(p.c_usuario.ToList());
                }
            }
            return res;
        }

        public static List<c_usuario> TramoControlInferior(int id, SICIEntities db)
        {
            var us = db.c_usuario.Find(id);
            c_puesto puesto;

            if(us.c_puesto.Count > 0)
            {
                puesto = us.c_puesto.First();
            }
            else
            {
                puesto = null;
            }

            var res = new List<c_usuario>();

            if (puesto != null)
            {
                var ps = puestosInferiores(puesto, db);

                foreach (var p in ps)
                {
                    res.AddRange(p.c_usuario.ToList());
                }
            }
            return res;
        }


        #region Cuenta de Objetos
        public static int TCC_Count(int id, int tipo, SICIEntities db)
        {
            var us = db.c_usuario.Find(id);
            var tramoControl = TramoControlInferior(id, db);
            var suma = 0;


            switch (tipo)
            {
                case 1:
                    foreach (var u in tramoControl)
                    {
                        suma += u.c_entidad.Count();
                    }
                    break;
                case 2:
                    foreach (var u in tramoControl)
                    {
                        suma += u.c_macro_proceso.Count();
                    }
                    break;
                case 3:
                    foreach (var u in tramoControl)
                    {
                        suma += u.c_proceso.Count();
                    }
                    break;
                case 4:
                    foreach (var u in tramoControl)
                    {
                        suma += u.c_sub_proceso.Count();
                    }
                    break;
                case 5:
                    foreach (var u in tramoControl)
                    {
                        suma += u.k_control1.Count();
                    }
                    break;
                case 6:
                    foreach (var u in tramoControl)
                    {
                        suma += u.c_indicador.Count();
                    }
                    break;
                case 7:
                    foreach (var u in tramoControl)
                    {
                        suma += u.k_objeto.Where(o => o.tipo_objeto == 1).Count();
                    }
                    break;
                case 8:
                    foreach (var u in tramoControl)
                    {
                        suma += u.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).Count();
                    }
                    break;
                case 9:
                    foreach (var u in tramoControl)
                    {
                        var incidenciasObjetos = u.k_incidencia.Where(inc => inc.id_objeto != null).ToList();
                        suma += incidenciasObjetos.Where(inc => inc.k_objeto.tipo_objeto == 1 || inc.k_objeto.tipo_objeto == 2 || inc.k_objeto.tipo_objeto == 3).Count();
                    }
                    break;
                case 10:
                    foreach (var u in tramoControl)
                    {
                        suma += u.k_plan.Count();
                    }
                    break;
                case 11:
                    foreach (var u in tramoControl)
                    {
                        suma += u.r_evento.Count();
                    }
                    break;
                case 12:
                    foreach (var u in tramoControl)
                    {
                        suma += u.c_indicador_diario.Count();
                    }
                    break;
            }

            return suma;
        }
        #endregion

        #region Listas de Objetos
        public static object InferiorObjects(int id, int IoT, int type)
        {
            db = new SICIEntities();
            var user = db.c_usuario.Find(id);

            if (IoT == 1)
            {
                switch (type)
                {
                    case 1:
                        return user.c_entidad.ToList();
                    case 2:
                        return user.c_macro_proceso.ToList();
                    case 3:
                        return user.c_proceso.ToList();
                    case 4:
                        return user.c_sub_proceso.ToList();
                    case 5:
                        return user.k_control1.ToList();
                    case 6:
                        return user.c_indicador.ToList();
                    case 7:
                        return user.k_objeto.Where(o => o.tipo_objeto == 1).ToList();
                    case 8:
                        return user.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();
                    case 9:
                        var incidenciasObjetos = user.k_incidencia.Where(inc => inc.id_objeto != null).ToList();
                        return incidenciasObjetos.Where(inc => inc.k_objeto.tipo_objeto == 1 || inc.k_objeto.tipo_objeto == 2 || inc.k_objeto.tipo_objeto == 3).ToList();
                    case 10:
                        return user.k_plan.ToList();
                    case 11:
                        return user.r_evento.ToList();
                }
            }
            else
            {
                var tramo_control = TramoControlInferior(id, db);

                switch (type)
                {
                    case 1:
                        var result1 = new List<c_entidad>();
                        foreach (var u in tramo_control)
                        {
                            result1.AddRange(u.c_entidad.ToList());
                        }
                        return result1;
                    case 2:
                        var result2 = new List<c_macro_proceso>();
                        foreach (var u in tramo_control)
                        {
                            result2.AddRange(u.c_macro_proceso.ToList());
                        }
                        return result2;
                    case 3:
                        var result3 = new List<c_proceso>();
                        foreach (var u in tramo_control)
                        {
                            result3.AddRange(u.c_proceso.ToList());
                        }
                        return result3;
                    case 4:
                        var result4 = new List<c_sub_proceso>();
                        foreach (var u in tramo_control)
                        {
                            result4.AddRange(u.c_sub_proceso.ToList());
                        }
                        return result4;
                    case 5:
                        var result5 = new List<k_control>();
                        foreach (var u in tramo_control)
                        {
                            result5.AddRange(u.k_control1.ToList());
                        }
                        return result5;
                    case 6:
                        var result6 = new List<c_indicador>();
                        foreach (var u in tramo_control)
                        {
                            result6.AddRange(u.c_indicador.ToList());
                        }
                        return result6;
                    case 7:
                        var result7 = new List<k_objeto>();
                        foreach (var u in tramo_control)
                        {
                            result7.AddRange(u.k_objeto.Where(o => o.tipo_objeto == 1).ToList());
                        }
                        return result7;
                    case 8:
                        var result8 = new List<k_objeto>();
                        foreach (var u in tramo_control)
                        {
                            result8.AddRange(u.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList());
                        }
                        return result8;
                    case 9:
                        var result9 = new List<k_incidencia>();
                        foreach (var u in tramo_control)
                        {
                            var incObj = u.k_incidencia.Where(inc => inc.id_objeto != null).ToList();
                            result9.AddRange(incObj.Where(inc => inc.k_objeto.tipo_objeto == 1 || inc.k_objeto.tipo_objeto == 2 || inc.k_objeto.tipo_objeto == 3).ToList());
                        }
                        return result9;
                    case 10:
                        var result10 = new List<k_plan>();
                        foreach (var u in tramo_control)
                        {
                            result10.AddRange(u.k_plan.ToList());
                        }
                        return result10;
                    case 11:
                        var result11 = new List<r_evento>();
                        foreach (var u in tramo_control)
                        {
                            result11.AddRange(u.r_evento.ToList());
                        }
                        return result11;
                }
            }

            return null;
        }
        #endregion

        #endregion

        #region Notificaciones
        public static string getNotifCount()
        {
            db = new SICIEntities();
            try
            {
                var user = (IdentityPersonalizado)HttpContext.Current.User.Identity;
                return getNotifCountByUser(user.Id_usuario);
            }
            catch
            {
                return JsonConvert.SerializeObject(new NotificationsViewModel());
            }
        }

        public static string getNotifCountByUser(int id, bool includeTC = false)
        {
            db = new SICIEntities();
            var model = new NotificationsViewModel();
            var us = db.c_usuario.Find(id);

            var indicadores = us.c_indicador.Where(i => i.esta_activo).ToList();
            var controles = us.k_control1.Where(c => !c.tiene_accion_correctora).ToList();
            var oficios = us.k_objeto.Where(o => o.tipo_objeto == 1).ToList();
            var informes = us.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();
            var incidencias = us.k_incidencia.Where(i => i.id_objeto != null).ToList();
            incidencias = incidencias.Where(i => i.k_objeto.tipo_objeto == 1 || i.k_objeto.tipo_objeto == 2 || i.k_objeto.tipo_objeto == 3).ToList();
            var planes = us.k_plan.ToList();
            var fichas = us.r_evento.ToList();
            var indicadores_diarios = us.c_indicador_diario.Where(i => i.esta_activo).ToList();
            //Certificación estructura
            var entidades = us.c_entidad.ToList();
            var mps = us.c_macro_proceso.ToList();
            var prs = us.c_proceso.ToList();
            var sps = us.c_sub_proceso.ToList();



            model.t_indicadores = indicadores.Count();
            model.t_controles = controles.Count();
            model.t_oficios = oficios.Count();
            model.t_informes = informes.Count();
            model.t_incidencias = incidencias.Count();
            model.t_planes = planes.Count();
            model.t_fichas = fichas.Count();
            model.t_indicadores_diarios = indicadores_diarios.Count();
            model.t_entidades = entidades.Count();
            model.t_macro_procesos = mps.Count();
            model.t_procesos = prs.Count();
            model.t_sub_procesos = sps.Count();

            model.n_indicadores = CountPendig(indicadores);
            model.n_controles = CountPendig(controles);
            model.n_oficios = CountPendig(oficios);
            model.n_informes = CountPendig(informes);
            model.n_incidencias = CountPendig(incidencias);
            model.n_planes = CountPendig(planes);
            model.n_fichas = CountPendig(fichas);
            model.n_indicadores_diarios = CountPendig(indicadores_diarios, id);
            model.n_entidades = CountPendig(entidades);
            model.n_macro_procesos = CountPendig(mps);
            model.n_procesos = CountPendig(prs);
            model.n_sub_procesos = CountPendig(sps);


            //Total pendientes
            model.Total = model.n_entidades + model.n_macro_procesos + model.n_procesos + model.n_sub_procesos +  model.n_indicadores + model.n_controles + model.n_oficios + model.n_informes + model.n_incidencias + model.n_planes + model.n_fichas + model.n_indicadores_diarios;
            //Total existentes
            model.Global = model.t_entidades + model.t_macro_procesos + model.t_procesos + model.t_sub_procesos + model.t_indicadores + model.t_controles + model.t_oficios + model.t_informes + model.t_incidencias + model.t_planes + model.t_fichas + model.t_indicadores_diarios;


            //Tramo de control

            if (includeTC)
            {
                var TC = TramoControlInferior(id, db);

                List<c_indicador> indicadoresTC = new List<c_indicador>();
                List<k_control> controlesTC = new List<k_control>();
                List<k_objeto> oficiosTC = new List<k_objeto>();
                List<k_objeto> informesTC = new List<k_objeto>();
                List<k_incidencia> incidenciasTCA = new List<k_incidencia>();
                List<k_incidencia> incidenciasTC = new List<k_incidencia>();
                List<k_plan> planesTC = new List<k_plan>();
                List<r_evento> fichasTC = new List<r_evento>();
                List<c_indicador_diario> indicadores_diariosTC = new List<c_indicador_diario>();
                List<c_entidad> entidadesTC = new List<c_entidad>();
                List<c_macro_proceso> mpsTC = new List<c_macro_proceso>();
                List<c_proceso> prsTC = new List<c_proceso>();
                List<c_sub_proceso> spsTC = new List<c_sub_proceso>();

                model.n_indicadores_diariosTC = 0;

                foreach (var u in TC)
                {
                    indicadoresTC = indicadoresTC.Union(u.c_indicador.Where(i => i.esta_activo)).ToList();
                    controlesTC = controlesTC.Union(u.k_control1.Where(c => !c.tiene_accion_correctora)).ToList();
                    oficiosTC = oficiosTC.Union(u.k_objeto.Where(o => o.tipo_objeto == 1)).ToList();
                    informesTC = informesTC.Union(u.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3)).ToList();
                    incidenciasTCA = u.k_incidencia.Where(i => i.id_objeto != null).ToList();
                    incidenciasTC = incidenciasTC.Union(incidenciasTCA.Where(i => i.k_objeto.tipo_objeto == 1 || i.k_objeto.tipo_objeto == 2 || i.k_objeto.tipo_objeto == 3)).ToList();
                    planesTC = planesTC.Union(u.k_plan).ToList();
                    fichasTC = fichasTC.Union(u.r_evento).ToList();
                    indicadores_diariosTC = indicadores_diariosTC.Union(u.c_indicador_diario.Where(i => i.esta_activo)).ToList();

                    model.n_indicadores_diariosTC += CountPendig(indicadores_diariosTC, u.id_usuario);

                    entidadesTC = entidadesTC.Union(u.c_entidad.ToList()).ToList();
                    mpsTC = mpsTC.Union(u.c_macro_proceso.ToList()).ToList();
                    prsTC = prsTC.Union(u.c_proceso.ToList()).ToList();
                    spsTC = spsTC.Union(u.c_sub_proceso.ToList()).ToList();
                }


                model.t_indicadoresTC = indicadoresTC.Count();
                model.t_controlesTC = controlesTC.Count();
                model.t_oficiosTC = oficiosTC.Count();
                model.t_informesTC = informesTC.Count();
                model.t_incidenciasTC = incidenciasTC.Count();
                model.t_planesTC = planesTC.Count();
                model.t_fichasTC = fichasTC.Count();
                model.t_indicadores_diariosTC = indicadores_diariosTC.Count();
                model.t_entidadesTC = entidadesTC.Count();
                model.t_macro_procesosTC = mpsTC.Count();
                model.t_procesosTC = prsTC.Count();
                model.t_sub_procesosTC = spsTC.Count();

                model.n_indicadoresTC = CountPendig(indicadoresTC);
                model.n_controlesTC = CountPendig(controlesTC);
                model.n_oficiosTC = CountPendig(oficiosTC);
                model.n_informesTC = CountPendig(informesTC);
                model.n_incidenciasTC = CountPendig(incidenciasTC);
                model.n_planesTC = CountPendig(planesTC);
                model.n_fichasTC = CountPendig(fichasTC);
                model.n_entidadesTC = CountPendig(entidadesTC);
                model.n_macro_procesosTC = CountPendig(mpsTC);
                model.n_procesosTC = CountPendig(prsTC);
                model.n_sub_procesosTC = CountPendig(spsTC);



                model.TotalTC = model.n_entidadesTC + model.n_macro_procesosTC + model.n_procesosTC + model.n_sub_procesosTC + model.n_indicadoresTC + model.n_controlesTC + model.n_oficiosTC + model.n_informesTC + model.n_incidenciasTC + model.n_planesTC + model.n_fichasTC + model.n_indicadores_diariosTC;
                model.GlobalTC = model.t_entidadesTC + model.t_macro_procesosTC + model.t_procesosTC + model.t_sub_procesosTC + model.t_indicadoresTC + model.t_controlesTC + model.t_oficiosTC + model.t_informesTC + model.t_incidenciasTC + model.t_planesTC + model.t_fichasTC + model.t_indicadores_diariosTC;
            }

            model.includeTC = includeTC;

            return JsonConvert.SerializeObject(model);
        }

        public static NotificationsViewModel getNotifCountByUserM(int id, bool includeTC = false)
        {
            db = new SICIEntities();
            var model = new NotificationsViewModel();
            var us = db.c_usuario.Find(id);

            var indicadores = us.c_indicador.Where(i => i.esta_activo).ToList();
            var controles = us.k_control1.Where(c => !c.tiene_accion_correctora).ToList();
            var oficios = us.k_objeto.Where(o => o.tipo_objeto == 1).ToList();
            var informes = us.k_objeto.Where(o => o.tipo_objeto == 2 || o.tipo_objeto == 3).ToList();
            var incidencias = us.k_incidencia.Where(i => i.id_objeto != null).ToList();
            incidencias = incidencias.Where(i => i.k_objeto.tipo_objeto == 1 || i.k_objeto.tipo_objeto == 2 || i.k_objeto.tipo_objeto == 3).ToList();
            var planes = us.k_plan.ToList();
            var fichas = us.r_evento.ToList();
            var indicadores_diarios = us.c_indicador_diario.Where(i => i.esta_activo).ToList();
            //Certificación estructura
            var entidades = us.c_entidad.ToList();
            var mps = us.c_macro_proceso.ToList();
            var prs = us.c_proceso.ToList();
            var sps = us.c_sub_proceso.ToList();



            model.t_indicadores = indicadores.Count();
            model.t_controles = controles.Count();
            model.t_oficios = oficios.Count();
            model.t_informes = informes.Count();
            model.t_incidencias = incidencias.Count();
            model.t_planes = planes.Count();
            model.t_fichas = fichas.Count();
            model.t_indicadores_diarios = indicadores_diarios.Count();
            model.t_entidades = entidades.Count();
            model.t_macro_procesos = mps.Count();
            model.t_procesos = prs.Count();
            model.t_sub_procesos = sps.Count();

            model.n_indicadores = CountPendig(indicadores);
            model.n_controles = CountPendig(controles);
            model.n_oficios = CountPendig(oficios);
            model.n_informes = CountPendig(informes);
            model.n_incidencias = CountPendig(incidencias);
            model.n_planes = CountPendig(planes);
            model.n_fichas = CountPendig(fichas);
            model.n_indicadores_diarios = CountPendig(indicadores_diarios, id);
            model.n_entidades = CountPendig(entidades);
            model.n_macro_procesos = CountPendig(mps);
            model.n_procesos = CountPendig(prs);
            model.n_sub_procesos = CountPendig(sps);


            //Total pendientes
            model.Total = model.n_entidades + model.n_macro_procesos + model.n_procesos + model.n_sub_procesos + model.n_indicadores + model.n_controles + model.n_oficios + model.n_informes + model.n_incidencias + model.n_planes + model.n_fichas + model.n_indicadores_diarios;
            //Total existentes
            model.Global = model.t_entidades + model.t_macro_procesos + model.t_procesos + model.t_sub_procesos + model.t_indicadores + model.t_controles + model.t_oficios + model.t_informes + model.t_incidencias + model.t_planes + model.t_fichas + model.t_indicadores_diarios;

            return model;
        }


        #region Contar Registros sin terminar
        //Contar indicadores pendientes
        public static int CountPendig(List<c_indicador_diario> indicadores, int id_usuario)
        {
            var db = new SICIEntities();
            var user = db.c_usuario.Find(id_usuario);
            int res = 0;

            foreach (var ind in indicadores)
            {
                var grupo = ind.c_contenido_grupo;
                var fe_actual = DateTime.Now;

                if (grupo.c_contenido_grupo1.Count > 0)
                {
                    var contenido = grupo.c_contenido_grupo1.First();

                    var eval = GetLastEval(user, ind, contenido);

                    if (eval != null)
                    {
                        if (EvalNote(eval) == 4)
                        {
                            res++;
                        }
                    }
                }
            }

            return res;
        }

        //Contar indicadores pendientes
        public static int CountPendig(List<c_indicador> indicadores)
        {
            var db = new SICIEntities();

            int res = 0;
            if (db.c_periodo_indicador.Any(p => p.esta_activo))
            {
                var periodoIndicador = db.c_periodo_indicador.Where(p => p.esta_activo).First();
                foreach (var ind in indicadores)
                {
                    if (!ind.k_evaluacion.Any(e => e.id_periodo_indicador == periodoIndicador.id_periodo_indicador)) res++;
                }
            }

            return res;
        }

        public static int CountPendig(List<k_control> controles)
        {
            var db = new SICIEntities();

            int res = 0;
            if (db.c_periodo_certificacion.Any(p => p.esta_activo))
            {
                var periodoCertificacion = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                foreach (var control in controles)
                {
                    if (!control.k_certificacion_control.Any(c => c.id_periodo_certificacion == periodoCertificacion.id_periodo_certificacion)) res++;
                }
            }

            return res;
        }

        public static int CountPendig(List<k_objeto> objetos)
        {
            return objetos.Where(o => o.fe_contestacion == null).Count();
        }
        public static int CountPendig(List<k_incidencia> incidencias)
        {
            return incidencias.Where(i => i.r_respuesta.Count() == 0).Count();
        }
        public static int CountPendig(List<k_plan> planes)
        {
            return planes.Where(p => p.r_conclusion_plan.Count() == 0).Count();
        }

        public static int CountPendig(List<r_evento> fichas)
        {
            db = new SICIEntities();

            int res = 0;
            foreach (var ficha in fichas)
            {
                var reg = GetLastReg(ficha, db);
                if (!reg.terminado) res++;
            }

            return res;
        }

        public static int CountPendig(List<c_entidad> regs)
        {
            var db = new SICIEntities();

            int res = 0;
            if (db.c_periodo_certificacion.Any(p => p.esta_activo))
            {
                var periodoCertificacion = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                foreach (var r in regs)
                {
                    if (!r.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == periodoCertificacion.id_periodo_certificacion)) res++;
                }
            }

            return res;
        }

        public static int CountPendig(List<c_macro_proceso> regs)
        {
            var db = new SICIEntities();

            int res = 0;
            if (db.c_periodo_certificacion.Any(p => p.esta_activo))
            {
                var periodoCertificacion = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                foreach (var r in regs)
                {
                    if (!r.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == periodoCertificacion.id_periodo_certificacion)) res++;
                }
            }

            return res;
        }

        public static int CountPendig(List<c_proceso> regs)
        {
            var db = new SICIEntities();

            int res = 0;
            if (db.c_periodo_certificacion.Any(p => p.esta_activo))
            {
                var periodoCertificacion = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                foreach (var r in regs)
                {
                    if (!r.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == periodoCertificacion.id_periodo_certificacion)) res++;
                }
            }

            return res;
        }

        public static int CountPendig(List<c_sub_proceso> regs)
        {
            var db = new SICIEntities();

            int res = 0;
            if (db.c_periodo_certificacion.Any(p => p.esta_activo))
            {
                var periodoCertificacion = db.c_periodo_certificacion.Where(p => p.esta_activo).First();
                foreach (var r in regs)
                {
                    if (!r.k_certificacion_estructura.Any(c => c.id_periodo_certificacion == periodoCertificacion.id_periodo_certificacion)) res++;
                }
            }

            return res;
        }
        #endregion


        #region Tareas Completadas
        public static void TaskCompleted(r_evento registro)
        {
            refreshNotifCount(registro.id_responsable);
            removeRow(7, registro.id_evento, registro.id_responsable);

            //actualizar la cuenta de los registros en el dashboard para los superiores
        }
        #endregion

        #region Tareas Asignadas

        public static void TaskAsigned(k_control registro, int lu = 0)
        {
            TaskAsigned((int)registro.id_responsable, "Se le ha asignado el control: " + registro.relacion_control, lu);
            Notification.ControlAsignado(registro); //enviar notificación al correo
            ObjectAsigned((int)registro.id_responsable, 5, lu, new SICIEntities());
        }

        public static void TaskAsigned(c_indicador registro, int lu = 0)
        {
            TaskAsigned((int)registro.id_responsable, "Se le ha asignado el indicador: " + registro.nb_indicador, lu);
            Notification.IndicadorAsignado(registro); //enviar notificación al correo
            ObjectAsigned((int)registro.id_responsable, 6, lu, new SICIEntities());
        }

        public static void TaskAsigned(k_objeto registro, int lu = 0)
        {
            //Aplica para oficios e informes
            if (registro.tipo_objeto == 1)
            {
                TaskAsigned((int)registro.id_responsable, "Se le ha asignado el oficio: " + registro.nb_objeto, lu);
                Notification.OficioAsignado(registro); //enviar notificación al correo
                ObjectAsigned((int)registro.id_responsable, 7, lu, new SICIEntities());
            }
            else
            {
                TaskAsigned((int)registro.id_responsable, "Se le ha asignado el informe: " + registro.nb_objeto, lu);
                Notification.InformeAsignado(registro); //enviar notificación al correo
                ObjectAsigned((int)registro.id_responsable, 8, lu, new SICIEntities());
            }
        }

        public static void TaskAsigned(k_incidencia registro, int lu = 0)
        {
            TaskAsigned((int)registro.id_responsable, "Se le ha asignado la incidencia: " + registro.id_incidencia, lu);
            Notification.IncidenciaAsignada(registro); //enviar notificación al correo
            ObjectAsigned((int)registro.id_responsable, 9, lu, new SICIEntities());
        }

        public static void TaskAsigned(k_plan registro, int lu = 0)
        {
            TaskAsigned((int)registro.id_responsable, "Se le ha asignado el plan de remediación: " + registro.nb_plan, lu);
            Notification.planAsignado(registro); //enviar notificación al correo
            ObjectAsigned((int)registro.id_responsable, 10, lu, new SICIEntities());
        }

        public static void TaskAsigned(r_evento registro, int lu = 0)
        {
            TaskAsigned(registro.id_responsable, "Se ha asignado como responsable de una ficha", lu);
            ObjectAsigned((int)registro.id_responsable, 11, lu, new SICIEntities());
        }

        private static void TaskAsigned(int id, string message, int lu = 0)
        {
            notifyUser(id, message, "info");
            refreshNotifCount(id);



            if (lu != 0) refreshNotifCount(lu);

            //actualizar la cuenta de los registros en el dashboard

            //actualizar la cuenta de los registros en el dashboard para los superiores
        }

        #endregion

        #region Objetos Asignados

        public static void ObjectAsigned(c_entidad registro, int lu = 0)
        {
            db = new SICIEntities();
            var message = "Se ha asignado como responsable de la entidad: " + registro.nb_entidad;
            notifyUser(registro.id_responsable, message, "info");

            ObjectAsigned(registro.id_responsable, 1, lu, db);
        }

        public static void ObjectAsigned(c_macro_proceso registro, int lu = 0)
        {
            db = new SICIEntities();
            var message = "Se ha asignado como responsable del MacroProceso: " + registro.cl_macro_proceso;
            notifyUser(registro.id_responsable, message, "info");

            ObjectAsigned(registro.id_responsable, 2, lu, db);
        }

        public static void ObjectAsigned(c_proceso registro, int lu = 0)
        {
            db = new SICIEntities();
            var message = "Se ha asignado como responsable del proceso: " + registro.cl_proceso;
            notifyUser(registro.id_responsable, message, "info");

            ObjectAsigned(registro.id_responsable, 3, lu, db);
        }

        public static void ObjectAsigned(c_sub_proceso registro, int lu = 0)
        {
            db = new SICIEntities();
            var message = "Se ha asignado como responsable del sub proceso: " + registro.cl_sub_proceso;
            notifyUser(registro.id_responsable, message, "info");

            ObjectAsigned(registro.id_responsable, 4, lu, db);
        }

        private static void ObjectAsigned(int id, int tipoObjeto, int lu, SICIEntities db)
        {

            //refrescar cuenta del registro dashboard para responsable, ultimo usuario y superiores

            var user = db.c_usuario.Find(id);
            execute(id, AssignmentExpression(tipoObjeto, user));

            var ListaUsuarios = TramoControlSuperior(id, db);
            foreach (var us in ListaUsuarios)
            {
                execute(us.id_usuario, AssignmentExpression(tipoObjeto, us, false));
            }

            if (lu != 0)
            {
                execute(lu, AssignmentExpression(tipoObjeto, db.c_usuario.Find(lu)));
                ListaUsuarios = TramoControlSuperior(lu, db);
                foreach (var us in ListaUsuarios)
                {
                    execute(us.id_usuario, AssignmentExpression(tipoObjeto, us, false));
                }

            }
        }

        private static string AssignmentExpression(int tipoObjeto, c_usuario usuario, bool resposable = true)
        {
            //Tipos
            //Entidades     = 1
            //MacroProcesos = 2
            //Procesos      = 3
            //SubProcesos   = 4
            //Controles     = 5
            //Indicadores   = 6
            //Oficios       = 7
            //Informes      = 8
            //Incidencias   = 9
            //Planes R      = 10
            //Fichas        = 11


            string tipo = "";
            int noRegistros = 0;

            switch (tipoObjeto)
            {
                case 1:
                    tipo = "Entidades";
                    noRegistros = resposable ? usuario.c_entidad.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 2:
                    tipo = "MacroProcesos";
                    noRegistros = resposable ? usuario.c_macro_proceso.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 3:
                    tipo = "Procesos";
                    noRegistros = resposable ? usuario.c_proceso.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 4:
                    tipo = "SubProcesos";
                    noRegistros = resposable ? usuario.c_sub_proceso.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 5:
                    tipo = "Controles";
                    noRegistros = resposable ? usuario.k_control1.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 6:
                    tipo = "Indicadores";
                    noRegistros = resposable ? usuario.c_indicador.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 7:
                    tipo = "Oficios";
                    noRegistros = resposable ? usuario.c_entidad.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 8:
                    tipo = "Informes";
                    noRegistros = resposable ? usuario.k_objeto.Where(o => o.tipo_objeto == 1).Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 9:
                    tipo = "Incidencias";
                    noRegistros = resposable ? usuario.k_incidencia.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 10:
                    tipo = "Planes";
                    noRegistros = resposable ? usuario.k_plan.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                case 11:
                    tipo = "Fichas";
                    noRegistros = resposable ? usuario.r_evento.Count() : TCC_Count(usuario.id_usuario, tipoObjeto, new SICIEntities());
                    break;
                default:
                    return "";
            }

            string expression;

            if (resposable) expression = "$('.UC-" + tipo + "').text('" + noRegistros + "')";
            else expression = "$('.TCC-" + tipo + "').text('" + noRegistros + "')";

            return expression;
        }

        #endregion

        #endregion

        #region Acciones en tiempo real
        public static void refreshNotifCount(int id, bool RTCS = true)
        {
            //Se eliminó el método para realizar los procesos de finalización de manera más rápida

            return;

            //var Json = getNotifCountByUser(id);
            //HubContext.Clients.All.refreshNotif(id, Json);


            //if (RTCS)
            //{
            //    var TC = TramoControlSuperior(id, new SICIEntities());
            //    foreach (var us in TC)
            //    {
            //        refreshNotifCount(us.id_usuario, false);
            //    }
            //}

            //return;
        }

        public static void removeRow(int typeTable, int id, int id_usuario)
        {
            HubContext.Clients.All.removeRow(typeTable, id, id_usuario);
            return;
        }

        public static void notifyUser(int id, string message, string notifType)
        {
            HubContext.Clients.All.notifyUser(id, message, notifType);
            return;
        }

        public static void execute(int id, string expression)
        {
            HubContext.Clients.All.execute(id, expression);
            return;
        }

        public static void BackUpProgress1(string progress, string message)
        {
            HubContext.Clients.All.BackUpProgress1(progress, message);
            return;
        }
        #endregion

        #region Normatividad
        public static c_contenido_normatividad getRoot(SICIEntities db, c_contenido_normatividad contenido)
        {
            c_contenido_normatividad cont = contenido;


            while (cont.id_contenido_normatividad_padre != null)
            {
                cont = db.c_contenido_normatividad.Find(cont.id_contenido_normatividad_padre);
            }

            return cont;
        }

        public static string getRuta(c_contenido_normatividad contenido)
        {
            string ruta = contenido.cl_contenido_normatividad;

            var padre = contenido.c_contenido_normatividad2;

            while (padre != null)
            {
                ruta = padre.cl_contenido_normatividad + " >> " + ruta;
                padre = padre.c_contenido_normatividad2;
            }

            return ruta;
        }


        public static string CeldasAnterioresNorm(c_contenido_normatividad cont)
        {
            if (cont.id_contenido_normatividad_padre == null) return null;
            else
            {
                var padre = cont.c_contenido_normatividad2;
                var inicioCadena = CeldasAnterioresNorm(padre);
                return inicioCadena + "<td>" + padre.cl_contenido_normatividad + "</td><td>" + padre.ds_contenido_normatividad + "</td>";
            }
        }

        public static List<string> CeldasAnterioresNormL(c_contenido_normatividad cont)
        {
            var celdas = new List<string>();

            if (cont.id_contenido_normatividad_padre == null) return celdas;
            else
            {
                var padre = cont.c_contenido_normatividad2;
                var inicioCadena = CeldasAnterioresNormL(padre);

                celdas.AddRange(inicioCadena);
                celdas.Add(padre.cl_contenido_normatividad);
                celdas.Add(padre.ds_contenido_normatividad);

                return celdas;
            }
        }

        public static string SubProcesosLigados(c_contenido_normatividad cont)
        {
            var result = "";
            //var sps = cont.c_sub_proceso_normatividad.ToList();
            var sps = getLinkedSPS(cont);


            if (sps.Count > 0)
            {
                foreach (var sp in sps)
                {
                    result += sp.c_sub_proceso.cl_sub_proceso + " - " + sp.c_sub_proceso.nb_sub_proceso + "\n";
                }
            }
            else
            {
                result = "N/A";
            }

            return result;
        }


        private static List<c_sub_proceso_normatividad> getLinkedSPS(c_contenido_normatividad contenido)
        {
            var aux = contenido;
            IEnumerable<c_sub_proceso_normatividad> Lista = new List<c_sub_proceso_normatividad>();

            while (aux.id_contenido_normatividad_padre != null)
            {
                Lista = Lista.Union(aux.c_sub_proceso_normatividad);
                aux = aux.c_contenido_normatividad2;
            }
            Lista = Lista.Union(aux.c_sub_proceso_normatividad);

            return Lista.ToList();
        }

        #endregion

        #region Consultas Especificas

        static public string nb_criticidad(string nb_po, string nb_mi)
        {
            try
            {
                var po = db.c_probabilidad_ocurrencia.Where(p => p.nb_probabilidad_ocurrencia == nb_po).First();
                var mi = db.c_magnitud_impacto.Where(m => m.nb_magnitud_impacto == nb_mi).First();

                var crit = db.c_criticidad.Where(c => c.id_magnitud_impacto == mi.id_magnitud_impacto && c.id_probabilidad_ocurrencia == po.id_probabilidad_ocurrencia).First();
                return crit.c_criticidad_riesgo.nb_criticidad_riesgo;
            }
            catch
            {
                return "N/A";
            }

        }

        static public string JustificacionNoPlan(int id)
        {
            db = new SICIEntities();
            var inc = db.k_incidencia.Find(id);

            return inc.js_incidencia;
        }

        static public List<k_control> GetLinkedControls(List<k_control> controlList)
        {
            List<k_control> c2 = new List<k_control>();

            foreach (var control in controlList)
            {
                if (!(control.k_riesgo.Count() < 1))
                {
                    c2.Add(control);
                }
            }
            return c2;
        }

        static public string LineasNegocio(int? id)
        {
            db = new SICIEntities();
            if (id == null)
            {
                return "Sub Proceso no valido";
            }
            //Encontrar sub proceso
            var sp = db.c_sub_proceso.Find(id);
            //Lista de sus lineas de negocio
            var lineas = sp.c_linea_negocio.ToList();
            var len = lineas.Count();
            string lns = "";

            if (len == 0)
            {
                lns = "No cuenta con Líneas de Negocio";
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    if (i != len - 1)
                    {
                        lns += lineas[i].nb_linea_negocio + ", ";
                    }
                    else
                    {
                        lns += lineas[i].nb_linea_negocio;
                    }
                }
            }

            return lns;
        }

        public static List<c_contenido_normatividad> getContents(c_contenido_normatividad cont)
        {
            db = new SICIEntities();

            List<c_contenido_normatividad> contenidos = new List<c_contenido_normatividad>();
            List<c_contenido_normatividad> aux = new List<c_contenido_normatividad>();

            var sql = "select * from c_contenido_normatividad where id_contenido_normatividad_padre = " + cont.id_contenido_normatividad;
            var hijos = db.c_contenido_normatividad.SqlQuery(sql).ToList();

            hijos = hijos.OrderBy(m => m.cl_contenido_normatividad).ToList();

            if (hijos.Count == 0)
            {
                contenidos.Add(cont);
            }
            else
            {
                foreach (var hijo in hijos)
                {
                    aux = getContents(hijo);
                    contenidos.AddRange(aux);
                }
            }

            return contenidos;
        }

        public static bool setPendingListLabel(string label)
        {

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Plantillas/Plantillas.xml");
            //string path = HttpRuntime.AppDomainAppPath + "App_Data/Plantillas/Plantillas.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);


            //Obtenemos el elemento "message"
            var mensajes = doc.SelectSingleNode("messages");
            var mensaje = mensajes.SelectSingleNode("message7");

            var Freq = mensaje.SelectSingleNode("freq");

            Freq.InnerText = label;
            doc.Save(path);

            return true;
        }

        #endregion

        #region HangFire Acciones Programadas   
        public static void BackUpReports()
        {
            //notifyUser(72, "tarea iniciada a las " + DateTime.Now.ToShortTimeString(), "info");

            Controllers.UtilidadesController utilidadesController = new Controllers.UtilidadesController();

            utilidadesController.BackUpActions();

            //notifyUser(72, "tarea Terminada a las " + DateTime.Now.ToShortTimeString(), "info");
        }

        public static void randomProgress()
        {
            var seed = DateTime.Now.Ticks;
            var random = new Random((int)seed);
            int progress = random.Next(0, 100);


            BackUpProgress1(progress.ToString(), progress + "%");
        }

        public static bool SetPendingListFrequency(string freq, string label)
        {
            bool res = false;

            try
            {
                RecurringJob.AddOrUpdate("NotifPending", () => Notification.notifPendientes(), freq, TimeZoneInfo.FindSystemTimeZoneById(GetSecurityProp("TimeZone", TimeZoneInfo.Local.Id)));

                res = setPendingListLabel(label);
            }
            catch
            {
                res = false;
            }

            return res;
        }



        public static void ClearParams()
        {
            var db = new SICIEntities();
            var basura = db.c_parametro.Where(p => p.valor_parametro == "01/01/0001");
            db.c_parametro.RemoveRange(basura);

            db.SaveChanges();

            var parametros = db.c_parametro;

            //List<string> mensajes = new List<string>();
            List<string> analizados = new List<string>();

            //Eliminar todos los parametros que son basura 

            foreach (var parametro in parametros)
            {
                if (!analizados.Any(p => p == parametro.nb_parametro))
                {
                    var ParaBorrar = parametros.Where(p => p.nb_parametro == parametro.nb_parametro && p.id_parametro != parametro.id_parametro);
                    analizados.Add(parametro.nb_parametro);

                    db.c_parametro.RemoveRange(ParaBorrar);

                    //mensajes.Add("Parametro " + parametro.nb_parametro + " repetido " + ParaBorrar.Count() + " veces.");
                }
            }

            db.SaveChanges();
            return;
        }
        #endregion

        #region Tratamiento de Texto

        public static string getFileClass(c_archivo file)
        {
            var clase = "";

            if (file.extension == "jpeg"
                            || file.extension == "jpg"
                            || file.extension == "gif"
                            || file.extension == "png"
                            || file.extension == "tiff"
                            || file.extension == "tif"
                            || file.extension == "raw"
                            || file.extension == "bmp"
                            || file.extension == "psd"
                            )
            {
                clase += "-image-o";
            }

            if (file.extension == "pdf"
                )
            {
                clase += "-pdf-o";
            }

            if (file.extension == "xls"
                || file.extension == "xlsx"
                || file.extension == "xlsm"
                || file.extension == "xlsb"
                || file.extension == "xltx"
                || file.extension == "xlt"
                || file.extension == "xml"
                || file.extension == "xlam"
                || file.extension == "xla"
                || file.extension == "xlw"
                || file.extension == "xlr"
                )
            {
                clase += "-excel-o";
            }


            if (file.extension == "txt"
                || file.extension == "prn"
                || file.extension == "csv"
                || file.extension == "dif"
                || file.extension == "slk"
                )
            {
                clase += "-text-o";
            }

            if (file.extension == "avi"
                || file.extension == "mp4"
                || file.extension == "mpeg-4"
                || file.extension == "mkv"
                || file.extension == "flv"
                || file.extension == "mov"
                || file.extension == "wmv"
                || file.extension == "mpeg"
                )
            {
                clase += "-video-o";
            }


            if (file.extension == "pptx"
                || file.extension == "pptm"
                || file.extension == "ppt"
                || file.extension == "xps"
                || file.extension == "potx"
                || file.extension == "potm"
                || file.extension == "pot"
                || file.extension == "thmx"
                || file.extension == "ppsx"
                || file.extension == "ppsm"
                || file.extension == "pps"
                || file.extension == "ppam"
                || file.extension == "ppa"
                )
            {
                clase += "-powerpoint-o";
            }

            if (file.extension == "mp3"
                || file.extension == "mid"
                || file.extension == "midi"
                || file.extension == "wav"
                || file.extension == "wma"
                || file.extension == "cda"
                || file.extension == "ogg"
                || file.extension == "ogm"
                || file.extension == "aac"
                || file.extension == "ac3"
                || file.extension == "flac"
                || file.extension == "ppam"
                || file.extension == "ppa"
                )
            {
                clase += "-audio-o";
            }

            if (file.extension == "zip"
                || file.extension == "rar"
                || file.extension == "7z"
                || file.extension == "sitx"
                || file.extension == "gz"
                )
            {
                clase += "-archive-o";
            }

            if (file.extension == "docx"
                || file.extension == "docm"
                || file.extension == "dotx"
                || file.extension == "dotm"
                || file.extension == "doc"
                || file.extension == "dot"
                )
            {
                clase += "-word-o";
            }

            return clase;
        }

        public static string NormalizarNombreArchivo(string name)
        {
            name = name.Replace('/', '-');
            name = name.Replace('\\', '-');
            name = name.Replace(';', '.');
            name = name.Replace('?', '¿');
            name = name.Replace('*', 'x');
            name = name.Replace('"', '´');
            name = name.Replace('<', ' ');
            name = name.Replace('>', ' ');
            name = name.Replace('|', ' ');

            return name;
        }

        public static string EliminarComillas(string name)
        {
            name = name.Replace("'", "´");
            name = name.Replace("\"", "´´");

            return name;
        }
        #endregion

        #region Tratamiento de archivos
        //eliminar los archivos de hace más de un día, que no tengan ligado ningún registro

        static public void deleteOldFiles()
        {
            var todayDate = DateTime.Now.AddDays(-1);
            var db = new SICIEntities();
            var oldFiles = db.c_archivo.Where(a =>
                a.r_evento.Count == 0
                && a.k_revision_control.Count == 0
                && a.k_certificacion_estructura.Count == 0
                && a.k_programa_trabajo.Count == 0
                && a.k_programa_trabajo1.Count == 0
                && a.r_conclusion_plan.Count == 0
                && a.r_seguimiento.Count == 0
                && a.k_plan.Count == 0
                && a.k_auditoria1.Count == 0
                && a.k_auditoria.Count == 0
                && a.fe_alta <= todayDate
            ).ToList();

            //eliminamos todos los archivos obtenidos
            foreach (var file in oldFiles)
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/a" + file.id_archivo);
                //string path = HttpRuntime.AppDomainAppPath + "App_Data/Archivos/a" + file.id_archivo;
                try
                {
                    System.IO.File.Delete(path);
                }
                catch
                {
                    //si el archivo ya no existe, no hacemos nada
                }
                try
                {
                    db.c_archivo.Remove(file);
                    db.SaveChanges();
                }
                catch
                {
                    //si el archivo ya no existe en base de datos, no hacemos nada
                }
            }
            //para cada archivo que aun tenga relacion con alguna tabla, revisar si existe el archivo en el fileSystem
            var existingFiles = db.c_archivo.ToList();
            foreach (var file in existingFiles)
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos/a" + file.id_archivo);
                if (!File.Exists(path)) //si el archivo no existe, se elimina el registro
                {
                    DeleteActions.DeleteArchivoObjects(file,db);

                    db.c_archivo.Remove(file);
                    db.SaveChanges();
                }
            }


            #region Eliminar Reportes de Revision de Control Antiguos
            var serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Archivos");

            DirectoryInfo dir = new DirectoryInfo(serverPath);
            var files = dir.GetFiles();

            var tempFiles = files.Where(f => f.Name.Length >= 4);
            tempFiles = tempFiles.Where(f => f.Name.Substring(0, 4) == "Temp");

            foreach (var file in tempFiles)
            {
                var fe_creacion = file.CreationTime.AddHours(1);
                if (fe_creacion < DateTime.Now)
                {
                    System.IO.File.Delete(file.FullName);
                }
            }


            #endregion

            return;
        }


        #endregion

        #region Incidencias

        public static string incSource(k_incidencia incidencia)
        {
            var nb_source = "";

            if (incidencia.id_objeto != null)
            {
                var objeto = incidencia.k_objeto;
                var prefix = "";
                switch (objeto.tipo_objeto)
                {
                    case 1:
                        prefix = "Oficio";
                        break;
                    case 2:
                        prefix = "Informe de Auditoría Externa";
                        break;
                    case 3:
                        prefix = "Informe de Auditoría Interna";
                        break;
                    case 6:
                        prefix = "Otros";
                        break;
                    default:
                        break;
                }

                nb_source = prefix + ": " + objeto.nb_objeto;

            }
            else if (incidencia.id_certificacion_control != null)
            {
                var cert = incidencia.k_certificacion_control;
                var control = cert.k_control;

                nb_source = "Certificación del Control: " + control.relacion_control;

            }
            else if (incidencia.id_control != null)
            {
                var control = incidencia.k_control;

                nb_source = "Control: " + control.relacion_control;
            }

            return nb_source;
        }

        #endregion

        #region Recordatorios/Fichas

        #region Nuevo Registro de seguimiento
        public static void NewReg(int id_evento)
        { 
            db = new SICIEntities();
            var evento = db.r_evento.Find(id_evento);

            CreateControlReg(evento, db);
            //refreshNotifCount(evento.id_responsable);


            var cronO = CrontabSchedule.Parse(NormalizeCron(evento.perioricidad));
            var NeOc = cronO.GetNextOccurrence(DateTime.Now);

            var diasAntes = evento.no_dias_antes_de_vencer ?? 0;

            while (NeOc.AddDays(-1 * (int)evento.no_dias_antes_de_vencer) < DateTime.Now) NeOc = NeOc.AddDays(1);


            DateTimeOffset fe_recordatorio = DateTime.SpecifyKind(NeOc.AddDays(-1 * diasAntes).AddHours(6), DateTimeKind.Utc);



            DateTimeOffset fe_aux = DateTime.SpecifyKind(DateTime.Now.AddHours(6), DateTimeKind.Utc);

            if (fe_recordatorio > fe_aux)
            {
                var job_id = BackgroundJob.Schedule(() => Notification.lanzarRecordatorioR(evento.id_evento), fe_recordatorio);

                var param = new c_parametro()
                {
                    nb_parametro = "EVR" + evento.id_evento,
                    valor_parametro = job_id
                };

                db.c_parametro.Add(param);
                db.SaveChanges();
            }
            return;
        }

        #endregion

        #region Eliminar Tareas en Segundo Plano
        public static bool DeleteBackgoundJobs(int id_evento, SICIEntities db, bool delEVR = true)
        {
            //eliminar tarea recurrente

            if (delEVR)
            {
                try
                {
                    RecurringJob.RemoveIfExists("EVR" + id_evento);
                }
                catch
                {
                    //No se encontró la tarea
                }
            }



            try
            {
                var param = db.c_parametro.Where(p => p.nb_parametro == "EVR" + id_evento).First();
                BackgroundJob.Delete(param.valor_parametro);
                db.c_parametro.Remove(param);
                db.SaveChanges();
            }
            catch
            {
                //No se encontró el parametro
            }

            try
            {
                var param = db.c_parametro.Where(p => p.nb_parametro == "EV" + id_evento).First();
                BackgroundJob.Delete(param.valor_parametro);
                db.c_parametro.Remove(param);
                db.SaveChanges();
            }
            catch
            {
                //No se encontró el parametro
            }

            return true;
        }

        #endregion

        #region Obtener Ficha
        public static r_evento getFicha(c_contenido_normatividad contenido, SICIEntities db)
        {
            var fichas = db.r_evento.Where(e => e.tipo == "0001").ToList();

            foreach (var ficha in fichas)
            {
                var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);
                if (conf.id == contenido.id_contenido_normatividad) return ficha;
            }

            return new r_evento();
        }

        public static r_evento getFicha(k_objeto objeto, SICIEntities db)
        {
            var fichas = db.r_evento.Where(e => e.tipo == "0002").ToList();

            foreach (var ficha in fichas)
            {
                var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(ficha.config);
                if (conf.id == objeto.id_objeto) return ficha;
            }

            return new r_evento();
        }

        public static r_evento getFicha(k_incidencia objeto, SICIEntities db)
        {
            var fichas = db.r_evento.Where(e => e.tipo == "0003").ToList();

            foreach (var ficha in fichas)
            {
                var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(ficha.config);
                if (conf.id == objeto.id_incidencia) return ficha;
            }

            return new r_evento();
        }

        public static r_evento getFicha(k_plan objeto, SICIEntities db)
        {
            var fichas = db.r_evento.Where(e => e.tipo == "0004").ToList();

            foreach (var ficha in fichas)
            {
                var conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0004>(ficha.config);
                if (conf.id == objeto.id_plan) return ficha;
            }

            return new r_evento();
        }
        #endregion

        public static DateTime getFeLim(r_evento evento)
        {
            DateTime fe_limite = new DateTime();

            if (evento.recordar_antes_de_vencer)
            {
                fe_limite = (DateTime)evento.fe_vencimiento;
            }
            else
            {
                var cron = CrontabSchedule.Parse(NormalizeCron(evento.perioricidad));
                fe_limite = cron.GetNextOccurrence(DateTime.Now);
            }

            return fe_limite;
        }

        public static r_registro_evento CreateControlReg(r_evento evento, SICIEntities db)
        {
            DateTime fe_limite = getFeLim(evento);

            var reg = new r_registro_evento()
            {
                fe_limite = fe_limite,
                id_evento = evento.id_evento
            };

            try
            {
                db.r_registro_evento.Add(reg);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CreateControlReg", e.Message, EventLogEntryType.Error);
                return null;
            }

            return reg;
        }

        public static r_registro_evento GetLastReg(r_evento evento, SICIEntities db)
        {
            r_registro_evento rSeguimiento = new r_registro_evento();

            try
            {
                rSeguimiento = db.r_registro_evento.Where(r => r.id_evento == evento.id_evento).ToList().Last();
            }
            catch
            {
                rSeguimiento = CreateControlReg(evento, db);
            }

            return rSeguimiento;
        }

        public static string GetStatus(r_evento evento)
        {
            db = new SICIEntities();

            var reg = Utilidades.GetLastReg(evento, db);
            string status;
            var felim = Utilidades.getFeLim(evento);

            if (reg.terminado)
            {
                status = "Atendida";
            }
            else
            {
                if (felim > DateTime.Now)
                {
                    status = "Pendiente";
                }
                else
                {
                    status = "Vencida";
                }
            }


            return status;
        }

        public static string GetStatus(k_control control)
        {
            db = new SICIEntities();
            var revisiones = control.k_revision_control.ToList();

            string status;
            if (revisiones.Count == 0)
            {
                status = "No Revisado";
            }
            else
            {
                var ultimaRevision = revisiones.Last();

                bool concluido = false;
                try
                {
                    if (ultimaRevision.cg_calificacion == 1) concluido = true;
                }
                catch
                {
                    concluido = false;
                }

                if (concluido)
                {
                    status = "Concluido";
                }
                else
                {
                    if (ultimaRevision.cg_conclusion == "" || ultimaRevision.cg_conclusion == null)
                    {
                        status = "En Proceso";
                    }
                    else
                    {
                        status = "Revisado";
                    }
                }

            }


            return status;
        }

        public static string registroLigado(r_evento ficha)
        {
            string registro_ligado;

            if (ficha.tipo == "0001")
            {
                ConfiguracionesEventosViewModel.Config0001 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);
                var cont = db.c_contenido_normatividad.Find(conf.id);

                if (cont == null)
                    return null;

                var norm = Utilidades.getRoot(db, cont);

                if (cont.id_contenido_normatividad_padre != null)
                {
                    registro_ligado = cont.cl_contenido_normatividad + " de la normatividad " + norm.ds_contenido_normatividad;
                }
                else
                {
                    registro_ligado = "Normatividad " + norm.ds_contenido_normatividad;
                }

            }
            else if (ficha.tipo == "0002")
            {
                ConfiguracionesEventosViewModel.Config0002 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(ficha.config);
                var objeto = db.k_objeto.Find(conf.id);

                if (objeto == null)
                    return null;

                string oficio_o_informe = objeto.tipo_objeto == 1 ? "Oficio" : objeto.tipo_objeto == 2 ? "Informe de auditoria externa" : "Informe de auditoria interna";

                registro_ligado = oficio_o_informe + ": " + objeto.nb_objeto;
            }
            else if (ficha.tipo == "0003")
            {
                ConfiguracionesEventosViewModel.Config0003 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(ficha.config);
                var objeto = db.k_incidencia.Find(conf.id);

                if (objeto == null)
                    return null;

                registro_ligado = "Incidencia perteneciente a: " + Utilidades.incSource(objeto);
            }
            else if (ficha.tipo == "0004")
            {
                ConfiguracionesEventosViewModel.Config0004 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0004>(ficha.config);
                var objeto = db.k_plan.Find(conf.id);

                if (objeto == null)
                    return null;

                registro_ligado = "Plan de Remediación: " + objeto.nb_plan;
            }
            else
            {
                registro_ligado = "N/A";
            }

            return registro_ligado;
        }

        public static string tipoFicha(r_evento ficha)
        {
            return ficha.tipo == "0001" ? "Normatividad" :
                ficha.tipo == "0002" ? "Oficios/Informes" :
                ficha.tipo == "0003" ? "Incidencias" :
                ficha.tipo == "0004" ? "Planes de Remediación" :
                "Desconocido";
        }

        public static int idRegistroLigado(r_evento ficha)
        {
            int id;

            if (ficha.tipo == "0001")
            {
                ConfiguracionesEventosViewModel.Config0001 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);
                id = conf.id;
            }
            else if (ficha.tipo == "0002")
            {
                ConfiguracionesEventosViewModel.Config0002 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(ficha.config);
                id = conf.id;
            }
            else if (ficha.tipo == "0003")
            {
                ConfiguracionesEventosViewModel.Config0003 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(ficha.config);
                id = conf.id;
            }
            else if (ficha.tipo == "0004")
            {
                ConfiguracionesEventosViewModel.Config0004 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0004>(ficha.config);
                id = conf.id;
            }
            else
            {
                id = 0;
            }


            return id;
        }

        public static string rutaRegistroLigado(r_evento ficha)
        {
            string ruta = "";

            if (ficha.tipo == "0001")
            {
                ConfiguracionesEventosViewModel.Config0001 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0001>(ficha.config);

                var contenido = db.c_contenido_normatividad.Find(conf.id);

                ruta = getRuta(contenido);
            }
            else if (ficha.tipo == "0002")
            {
                ConfiguracionesEventosViewModel.Config0002 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0002>(ficha.config);
                var objeto = db.k_objeto.Find(conf.id);

                if (objeto != null) {
                    string oficio_o_informe = objeto.tipo_objeto == 1 ? "Oficio" : objeto.tipo_objeto == 2 ? "Informe de auditoria externa" : "Informe de auditoria interna";

                    ruta = oficio_o_informe + ": " + objeto.nb_objeto;
                }
            }
            else if (ficha.tipo == "0003")
            {
                ConfiguracionesEventosViewModel.Config0003 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0003>(ficha.config);
                var objeto = db.k_incidencia.Find(conf.id);

                if (objeto != null) {

                    ruta = "Incidencia perteneciente a: " + Utilidades.incSource(objeto);
                }
                

            }
            else if (ficha.tipo == "0004")
            {
                ConfiguracionesEventosViewModel.Config0004 conf = JsonConvert.DeserializeObject<ConfiguracionesEventosViewModel.Config0004>(ficha.config);
                var objeto = db.k_plan.Find(conf.id);

                if (objeto != null) { 
                    ruta = "Plan de Remediación: " + objeto.nb_plan + "de incidencia perteneciente a " + incSource(objeto.k_incidencia);
                }

            }
            else
            {
                ruta = "";
            }


            return ruta;
        }

        #endregion

        #region Indicadores Diarios
        public static List<k_evaluacion_diaria> GetEvaluationsByTipe(int type, List<k_evaluacion_diaria> evaluaciones)
        {
            var SplitEvals = GetSplitEvaluations(evaluaciones);


            return SplitEvals.ElementAt(type - 1);
        }

        public static List<List<k_evaluacion_diaria>> GetSplitEvaluations(List<k_evaluacion_diaria> evaluaciones)
        {
            List<List<k_evaluacion_diaria>> res = new List<List<k_evaluacion_diaria>>();

            var ET1 = new List<k_evaluacion_diaria>(); //Evaluaciones Buenas
            var ET2 = new List<k_evaluacion_diaria>(); //Evaluaciones Regulares
            var ET3 = new List<k_evaluacion_diaria>(); //Evaluaciones Malas
            var ET4 = new List<k_evaluacion_diaria>(); //Evaluaciones No Calificadas

            foreach (var eval in evaluaciones)
            {
                var note = EvalNote(eval);

                if (note == 1) ET1.Add(eval);
                if (note == 2) ET2.Add(eval);
                if (note == 3) ET3.Add(eval);
                if (note == 4) ET4.Add(eval);

            }

            res.Add(ET1);
            res.Add(ET2);
            res.Add(ET3);
            res.Add(ET4);

            return res;
        }

        public static string getRuta(c_contenido_grupo contenido)
        {
            string ruta = contenido.cl_contenido_grupo;

            var padre = contenido.c_contenido_grupo2;

            while (padre != null)
            {
                ruta = padre.cl_contenido_grupo + " >> " + ruta;
                padre = padre.c_contenido_grupo2;
            }

            return ruta;
        }

        public static void CreateDailyEvaluationRegisters()
        {
            db = new SICIEntities();
            //obtenemos los indicadores activos y creamos nuevas tablas de registros para estos
            var indicadores = db.c_indicador_diario.Where(i => i.esta_activo).ToList();
            var FechaActual = DateTime.Now;


            foreach (var ind in indicadores)
            {
                var grupo = ind.c_contenido_grupo;
                var contenidos = grupo.c_contenido_grupo1;

                foreach (var us in ind.c_usuario)
                {
                    foreach (var contenido in contenidos)
                    {
                        var eval = new k_evaluacion_diaria()
                        {
                            id_indicador_diario = ind.id_indicador_diario,
                            id_contenido_grupo = contenido.id_contenido_grupo,
                            id_usuario = us.id_usuario,
                            fe_evaluacion = FechaActual
                        };

                        db.k_evaluacion_diaria.Add(eval);
                    }
                }
            }

            db.SaveChanges();

            return;
        }

        public static int NodeNote(c_contenido_grupo contenido)
        {
            /*
            Las calificaciones tendrán los siguientes valores 
            Bueno               1
            Regular             2
            Malo                3
            No Calificado       4
            Sin Indicador       5
            */


            //En esta lista se guardarán las calificaciones de este contenido
            List<int> calificaciones = new List<int>();

            int calificacion = 4;


            //si el conenido tiene hijos
            if (contenido.c_contenido_grupo1.Count > 0)
            {
                foreach (var hijo in contenido.c_contenido_grupo1)
                {
                    calificaciones.Add(NodeNote(hijo));
                }
            }


            //calificación del nodo actual del día anterior
            var YEvals = getYesterdayEvals(contenido);
            foreach (var Eval in YEvals)
            {
                calificaciones.Add(EvalNote(Eval));
            }

            //aplicar jerarquia para saber la calificación del nodo
            if (calificaciones.Contains(3))
            {
                calificacion = 3;
            }
            else if (calificaciones.Contains(4))
            {
                calificacion = 4;
            }
            else if (calificaciones.Contains(2) || calificaciones.Contains(1))
            {
                if (calificaciones.Where(c => c == 2).Count() >= calificaciones.Where(c => c == 1).Count())
                {
                    calificacion = 2;
                }
                else
                {
                    calificacion = 1;
                }
            }
            else
            {
                calificacion = 5;
            }

            return calificacion;
        }


        public static int EvalNote(k_evaluacion_diaria eval)
        {
            var ind = eval.c_indicador_diario;

            if (eval.numerador == null || eval.denominador == null || eval.numerador == 0 || eval.denominador == 0) return 4;

            var medicion = (eval.numerador / eval.denominador) * 100;
            int calificacion = 4;


            if ((medicion >= ind.umbral000i && medicion <= ind.umbral000f) || (medicion >= ind.umbral050i && medicion <= ind.umbral050f))  // Alerta
            {
                calificacion = 3;
            }
            if (medicion >= ind.umbral075i && medicion <= ind.umbral075f)  //Regular
            {
                calificacion = 2;
            }
            if (medicion >= ind.umbral100i && medicion <= ind.umbral100f)  //Bueno
            {
                calificacion = 1;
            }


            return calificacion;
        }


        public static List<k_evaluacion_diaria> getYesterdayEvals(c_contenido_grupo contenido)
        {
            var evaluaciones = contenido.k_evaluacion_diaria.ToList();

            List<k_evaluacion_diaria> YEvals;

            //fecha del día de ayer
            var YDate = DateTime.Now.AddDays(-1);
            //numero del día anterior
            int YDoY = YDate.DayOfYear;

            //si la ultima evaluación tiene fecha del día de hoy
            try
            {
                YEvals = evaluaciones.Where(e =>
                    ((DateTime)e.fe_evaluacion).DayOfYear == YDoY
                    && ((DateTime)e.fe_evaluacion).Year == YDate.Year
                ).ToList();
            }
            catch
            {
                YEvals = new List<k_evaluacion_diaria>();
            }

            return YEvals;
        }


        public static k_evaluacion_diaria GetLastEval(c_usuario us, c_indicador_diario ind, c_contenido_grupo cont)
        {
            var today = DateTime.Now;
            var todayDoY = today.DayOfYear;
            var todayY = today.Year;

            var TodayEvals = cont.k_evaluacion_diaria.Where(e =>
                    ((DateTime)e.fe_evaluacion).DayOfYear == todayDoY
                    && ((DateTime)e.fe_evaluacion).Year == todayY
                ).ToList();

            try
            {
                return TodayEvals.Where(e => e.id_usuario == us.id_usuario && e.id_indicador_diario == ind.id_indicador_diario).First();
            }
            catch
            {
                return null;
            }
        }

        public static List<k_evaluacion_diaria> GetLastEvals(c_indicador_diario ind, c_contenido_grupo cont)
        {
            var today = DateTime.Now;
            var todayDoY = today.DayOfYear;
            var todayY = today.Year;

            var TodayEvals = cont.k_evaluacion_diaria.Where(e =>
                    ((DateTime)e.fe_evaluacion).DayOfYear == todayDoY
                    && ((DateTime)e.fe_evaluacion).Year == todayY
                ).ToList();

            try
            {
                return TodayEvals.Where(e => e.id_indicador_diario == ind.id_indicador_diario).ToList();
            }
            catch
            {
                return new List<k_evaluacion_diaria>();
            }
        }
        #endregion

        #region Registros por Tramo de Control

        #region Método Maestro
        //este método recibe un usuario y una instancia de base de datos, así como una cadena especificando el tipo de objeto que se desea obtener
        //y en caso de ser necesario un indice que indica si por ejemplo se obtendrá user.k_control1 en vez de user.k_control
        //si se trata de un super usuario, el método devolverá una lista con todos los objetos del tipo solicitado que se encuentren en la base de datos
        //en caso contrario se obtendrán dinámicamente todos los objetos del tipo solicitado que esten a cargo del usuario, así como 
        //los usuarios en el tramo de control inferior del usuario.

        public static IEnumerable<dynamic> RTCObject(c_usuario user, SICIEntities db, string typeObject, string index = "")
        {
            Type m_tipo;
            PropertyInfo[] m_propiedades;
            PropertyInfo prop;

            if (user.es_super_usuario)
            {
                m_tipo = db.GetType();
                m_propiedades = m_tipo.GetProperties();

                prop = m_propiedades.Where(p => p.Name == typeObject).First();

                return (IEnumerable<dynamic>)prop.GetValue(db, null);
            }

            m_tipo = user.GetType();
            m_propiedades = m_tipo.GetProperties();
            prop = m_propiedades.Where(p => p.Name == typeObject + index).First();

            var value = prop.GetValue(user, null);
            IEnumerable<dynamic> res = (IEnumerable<dynamic>)value;

            var tramoControl = TramoControlInferior(user.id_usuario, db);
            foreach (var us in tramoControl)
            {
                value = prop.GetValue(us, null);
                res = res.Concat<dynamic>((IEnumerable<dynamic>)value);
            }

            return res;
        }
        #endregion

        #region Riesgos
        public static List<k_riesgo> RTCRiesgo(c_usuario user, SICIEntities db)
        {
            if (user.es_super_usuario)
            {
                return db.k_riesgo.ToList();
            }
            else
            {
                List<c_sub_proceso> SPS = RTCObject(user, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();
                var idsSPS = SPS.Select(e => e.id_sub_proceso).ToArray();

                //Controles de los Sub Procesos a los que el usuario tiene acceso
                var objFromSp = db.k_riesgo.Where(o => idsSPS.Contains(o.c_sub_proceso.id_sub_proceso)).ToList();

                return objFromSp;
            }
        }
        #endregion

        #region Riesgos Derogados
        public static List<k_riesgo_derogado> RTCRiesgoDerogado(c_usuario user, SICIEntities db)
        {
            if (user.es_super_usuario)
            {
                return db.k_riesgo_derogado.ToList();
            }
            else
            {
                List<c_sub_proceso> SPS = RTCObject(user, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();

                var idsSPS = SPS.Select(e => e.id_sub_proceso).ToArray();

                //Controles de los Sub Procesos a los que el usuario tiene acceso
                var objFromSp = db.k_riesgo_derogado.Where(o => idsSPS.Contains(o.c_sub_proceso.id_sub_proceso)).ToList();

                return objFromSp;
            }
        }
        #endregion

        #region Benchmark
        public static List<k_benchmarck> RTCBenchmark(c_usuario user, SICIEntities db)
        {
            if (user.es_super_usuario)
            {
                return db.k_benchmarck.ToList();
            }
            else
            {
                List<c_sub_proceso> SPS = RTCObject(user, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();
                var idsSPS = SPS.Select(e => e.id_sub_proceso).ToArray();

                //Registros de los Sub Procesos a los que el usuario tiene acceso
                var objFromSp = db.k_benchmarck.Where(o => idsSPS.Contains(o.c_sub_proceso.id_sub_proceso)).ToList();

                return objFromSp.ToList();
            }
        }
        #endregion

        #region Certificacion Control
        public static List<k_certificacion_control> RTCCertificacion(c_usuario user, SICIEntities db)
        {
            if (user.es_super_usuario)
            {
                return db.k_certificacion_control.ToList();
            }
            else
            {
                List<k_control> CTRL = RTCObject(user, db, "k_control", "1").Cast<k_control>().ToList();
                var idsCTRL = CTRL.Select(e => e.id_control).ToArray();

                //Registros de los Sub Procesos a los que el usuario tiene acceso
                var objFromCtrl = db.k_certificacion_control.Where(o => idsCTRL.Contains(o.k_control.id_control)).ToList();

                return objFromCtrl.ToList();
            }
        }


        public static List<k_certificacion_estructura> RTCCertificacionEstructura(c_usuario user, SICIEntities db, string type)
        {
            if (type == "E")
            {
                if (user.es_super_usuario)
                {
                    return db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "E").ToList();
                }
                else
                {
                    List<c_entidad> Entidades = RTCObject(user, db, "c_entidad").Cast<c_entidad>().ToList();

                    List<k_certificacion_estructura> certificaciones = new List<k_certificacion_estructura>();

                    foreach (var en in Entidades)
                    {
                        certificaciones.AddRange(en.k_certificacion_estructura);
                    }

                    return certificaciones;
                }
            }
            else if (type == "M")
            {
                if (user.es_super_usuario)
                {
                    return db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "M").ToList();
                }
                else
                {
                    List<c_macro_proceso> mps = RTCObject(user, db, "c_macro_proceso").Cast<c_macro_proceso>().ToList();

                    List<k_certificacion_estructura> certificaciones = new List<k_certificacion_estructura>();

                    foreach (var mp in mps)
                    {
                        certificaciones.AddRange(mp.k_certificacion_estructura);
                    }

                    return certificaciones;
                }
            }
            else if (type == "P")
            {
                if (user.es_super_usuario)
                {
                    return db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "P").ToList();
                }
                else
                {
                    List<c_proceso> prs = RTCObject(user, db, "c_proceso").Cast<c_proceso>().ToList();

                    List<k_certificacion_estructura> certificaciones = new List<k_certificacion_estructura>();

                    foreach (var pr in prs)
                    {
                        certificaciones.AddRange(pr.k_certificacion_estructura);
                    }

                    return certificaciones;
                }
            }
            else if (type == "S")
            {
                if (user.es_super_usuario)
                {
                    return db.k_certificacion_estructura.Where(c => c.cl_certificacion_estructura == "S").ToList();
                }
                else
                {
                    List<c_sub_proceso> sps = RTCObject(user, db, "c_sub_proceso").Cast<c_sub_proceso>().ToList();

                    List<k_certificacion_estructura> certificaciones = new List<k_certificacion_estructura>();

                    foreach (var sp in sps)
                    {
                        certificaciones.AddRange(sp.k_certificacion_estructura);
                    }

                    return certificaciones;
                }
            }

            return new List<k_certificacion_estructura>();
        }
        #endregion

        #endregion

        #region registro de errores
        public static void CreateErrorReg(string error, string metodo)
        {
            db = new SICIEntities();

            db.h_excepcion.Add(new h_excepcion
            {
                ds_excepcion = error,
                fe_excepcion = DateTime.Now,
                id_funcion = null,
                nb_metodo = metodo
            });

            db.SaveChanges();

        }

        #endregion

        #region Activiades de costeo
        public static async void AgregarActividadASubProcesos(int id_area_costeo)
        {
            var auxDB = new SICIEntities();
            var idsSps = auxDB.c_sub_proceso.Select(s => s.id_sub_proceso).ToArray();

            var sps = auxDB.c_sub_proceso.ToList();
            int counter = 0;

            List<c_area_costeo_sub_proceso> range = new List<c_area_costeo_sub_proceso>();
            foreach (var idsp in idsSps)
            {
                var acsp = new c_area_costeo_sub_proceso { id_area_costeo = id_area_costeo, id_sub_proceso = idsp };
                range.Add(acsp);
                
                if(counter%100 == 0)
                    Debug.WriteLine("Agregados " + counter + " registros");
                counter++;
            }

            auxDB.c_area_costeo_sub_proceso.AddRange(range);

            await auxDB.SaveChangesAsync();
        }


        #endregion

        #region Activiades de costeo AuditoriA
        public static async void AgregarActividadAauditoria(int id_rango_costeo)
        {
            var auxDB = new SICIEntities();
            var idsSps = auxDB.c_auditoria.Select(s => s.id_auditoria).ToArray();

            var sps = auxDB.c_auditoria.ToList();
            int counter = 0;

            List<c_rango_costeo_auditoria> range = new List<c_rango_costeo_auditoria>();
            foreach (var idsp in idsSps)
            {
                var acsp = new c_rango_costeo_auditoria { id_rango_costeo = id_rango_costeo, id_auditoria = idsp };
                range.Add(acsp);

                if (counter % 100 == 0)
                    Debug.WriteLine("Agregados " + counter + " registros");
                counter++;
            }

            auxDB.c_rango_costeo_auditoria.AddRange(range);

            await auxDB.SaveChangesAsync();
        }


        #endregion

        #region Verificar carpetas necesarias

        public static void VerifyFolders()
        {
            List<string> folders = new List<string>
            {
                "Archivos",
                "Certificacion",
                "Diagramas",
                "Diagramas\\EN",
                "Diagramas\\MP",
                "Diagramas\\PR",
                "Diagramas\\SP",
                "HistorialReportes",
                "Incidencias",
                "Informes-Oficios",
                "InformesAuditoria",
                "RIncidencias",
                "BDEIAuxInfo",
                "Manuales"
            };

            string appDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
            
            foreach(string folder in folders)
            {
                string path = Path.Combine(appDataPath, folder);
                if (Directory.Exists(path))
                {
                    Debug.WriteLine("Existe " + path);
                }
                else
                {
                    Debug.WriteLine("Creado " + path);
                    Directory.CreateDirectory(path);
                }
            }

        }


        #endregion

        #region Verificar integridad de datos

        public static void VerifyIntegrity() {

            var db = new SICIEntities();

            var fichas = db.r_evento.ToList();

            foreach (var ficha in fichas) {
                if (ficha.recordar_antes_de_vencer && !ficha.fe_vencimiento.HasValue) {

                    var fechaMinima = DateTime.MinValue.AddYears(1753);
                    ficha.fe_vencimiento = fechaMinima;
                    db.Entry(ficha).State = System.Data.Entity.EntityState.Modified;

                    //Agregar una entrada al reporte de errores
                    var tipo = tipoFicha(ficha);
                    string registro_ligado = registroLigado(ficha);

                    h_excepcion error = new h_excepcion()
                    {
                        id_funcion = null,
                        nb_metodo = "Integridad de datos",
                        ds_excepcion = "La ficha " + ficha.nb_evento + "("+ficha.id_evento+") del tipo: " + tipo + " ligada a '" + registro_ligado + "' no contaba con fecha de vencimiento.",
                        fe_excepcion = DateTime.Now
                    };

                    db.h_excepcion.Add(error);
                    db.SaveChanges();

                }
            }
        }

        

        #endregion
    }
}