using SCIRA.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AccionCorrectoraAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var model = (AgregarRiesgoViewModel)validationContext.ObjectInstance;
                bool tiene_accion_correctora = model.tiene_accion_correctora;

                if (tiene_accion_correctora)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    if (value != null)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("El campo es obligatorio.");
                    }
                }
            }
            catch
            {
                var model = (AgregarRiesgoMGViewModel)validationContext.ObjectInstance;
                bool tiene_accion_correctora = model.tiene_accion_correctora;

                if (tiene_accion_correctora)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    if (value != null)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("El campo es obligatorio.");
                    }
                }
            }

        }
    }
}