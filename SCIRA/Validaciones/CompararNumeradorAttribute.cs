using SCIRA.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SCIRA.Validaciones
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CompararNumeradorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (k_evaluacion)validationContext.ObjectInstance;
            decimal numerador = model.numerador;
            if (value == null)
            {
                return new ValidationResult("El Denominador es un campo obligatorio");
            }
            if ((decimal)value <= 0)
            {
                return new ValidationResult("El Denominador no puede ser igual o menor a 0");
            }
            if ((decimal)value < numerador)
            {
                return new ValidationResult("El Numerador no puede ser más grande que el Denominador.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}