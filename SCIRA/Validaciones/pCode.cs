using SCIRA.Models;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class pCodeAttribute : ValidationAttribute
    {
        SICIEntities db = new SICIEntities();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = (AgregarProcesoViewModel)validationContext.ObjectInstance;
                return (db.c_proceso.Any(x => x.id_macro_proceso == model.id_macro_proceso && x.cl_proceso == (string)value)) ? new ValidationResult("El código de Proceso ya está en uso.") : (ValidationResult.Success);
            }
            catch
            {
                try
                {
                    var model = (c_proceso)validationContext.ObjectInstance;
                    if (db.c_proceso.Any(u => u.cl_proceso == (string)value))
                    {
                        List<c_proceso> c_proceso_list = db.c_proceso.Where(o => o.cl_proceso == (string)value).ToList();
                        foreach (c_proceso c_proceso in c_proceso_list)
                        {
                            if (model.id_macro_proceso == c_proceso.id_macro_proceso)
                            {
                                return model.id_proceso == c_proceso.id_proceso ? (ValidationResult.Success) : (new ValidationResult("El código de Proceso ya está en uso."));
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
}