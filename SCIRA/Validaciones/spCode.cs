using SCIRA.Models;
using SCIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class spCodeAttribute : ValidationAttribute
    {
        SICIEntities db = new SICIEntities();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = (AgregarSubProcesoViewModel)validationContext.ObjectInstance;
                c_proceso c_proceso = db.c_proceso.Find(model.id_proceso);

                if (db.c_sub_proceso.Any(u => u.cl_sub_proceso == (string)value))
                {
                    List<c_sub_proceso> c_sub_proceso_list = db.c_sub_proceso.Where(o => o.cl_sub_proceso == (string)value).ToList();
                    foreach (c_sub_proceso c_sub_proceso in c_sub_proceso_list)
                    {
                        if (c_proceso.c_macro_proceso.id_entidad == c_sub_proceso.c_proceso.c_macro_proceso.id_entidad)
                        {
                            return model.id_sub_proceso == c_sub_proceso.id_sub_proceso ? (ValidationResult.Success) : (new ValidationResult("El código de Sub Proceso ya está en uso."));
                        }
                    }
                }
                return ValidationResult.Success;
            }
            catch
            {
                try
                {
                    var model = (c_sub_proceso)validationContext.ObjectInstance;
                    c_proceso c_proceso = db.c_proceso.Find(model.id_proceso);

                    if (db.c_sub_proceso.Any(u => u.cl_sub_proceso == (string)value))
                    {
                        List<c_sub_proceso> c_sub_proceso_list = db.c_sub_proceso.Where(o => o.cl_sub_proceso == (string)value).ToList();
                        foreach (c_sub_proceso c_sub_proceso in c_sub_proceso_list)
                        {
                            if (c_proceso.c_macro_proceso.id_entidad == c_sub_proceso.c_proceso.c_macro_proceso.id_entidad)
                            {
                                return model.id_sub_proceso == c_sub_proceso.id_sub_proceso ? (ValidationResult.Success) : (new ValidationResult("El código de Sub Proceso ya está en uso."));
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