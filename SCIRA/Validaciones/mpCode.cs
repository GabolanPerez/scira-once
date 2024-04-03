
using SCIRA.Models;
using SCIRA.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class mpCodeAttribute : ValidationAttribute
    {
        SICIEntities db = new SICIEntities();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = (c_macro_proceso)validationContext.ObjectInstance;


                string prefix = ((string)value).Substring(0, 2);
                string code = ((string)value).Substring(2, ((string)value).Length - 2);
                //int i = 0;
                //bool number = int.TryParse(code, out i);

                //if (prefix != "MP" && prefix != "MG" || code.Length == 0 || code.Length > 47 || !number)
                if (prefix != "MP" && prefix != "MG" || code.Length == 0 || code.Length > 47)
                {
                    return new ValidationResult(Strings.getMSG("MacroProcesoCreate016"));
                }


                if (db.c_macro_proceso.Any(u => u.cl_macro_proceso == (string)value))
                {
                    List<c_macro_proceso> c_macro_proceso_list = db.c_macro_proceso.Where(o => o.cl_macro_proceso == (string)value).ToList();
                    foreach (c_macro_proceso c_macro_proceso in c_macro_proceso_list)
                    {
                        if (model.id_entidad == c_macro_proceso.id_entidad)
                        {
                            return model.id_macro_proceso == c_macro_proceso.id_macro_proceso ? (ValidationResult.Success) : (new ValidationResult(Strings.getMSG("MacroProcesoCreate006")));
                        }
                    }
                }


                return ValidationResult.Success;
            }
            catch
            {
                return (new ValidationResult("No se pudo conectar con la base de datos."));
            }
        }
    }
}