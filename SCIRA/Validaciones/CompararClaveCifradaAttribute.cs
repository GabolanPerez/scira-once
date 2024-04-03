using SCIRA.Utilidades;
using SCIRA.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CompararClaveCifradaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CambiarContrasenaViewModel)validationContext.ObjectInstance;
            string Original = model.original_password;
            if (value != null)
            {
                string pass = SeguridadUtilidades.SHA256Encripta(value.ToString());
                if (pass == Original)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("La contraseña es incorrecta.");
                }
            }
            else
            {
                return new ValidationResult("Ingresa tu contraseña.");
            }
        }
    }
}