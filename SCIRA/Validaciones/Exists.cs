using SCIRA.Models;
using SCIRA.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ExistsAttribute : ValidationAttribute
    {
        SICIEntities db = new SICIEntities();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (db.c_rol.Any(r => r.nb_rol == value.ToString()))
                {
                    var model = (AgregarRolViewModel)validationContext.ObjectInstance;
                    var rol = db.c_rol.Where(r => r.nb_rol == value.ToString()).First();
                    return model.id_rol == rol.id_rol ? (ValidationResult.Success) : (new ValidationResult("Ya existe un rol con ese nombre."));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            catch
            {
                return (new ValidationResult("No se pudo conectar con la base de datos."));
            }
        }
    }
}