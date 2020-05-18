using System;
using System.ComponentModel.DataAnnotations;

namespace Triceratops.Libraries.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PortAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (ushort.TryParse(value?.ToString() ?? "", out var port) && port > 0)
            {
                return true;
            }

            return false;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!IsValid(value))
            {
                return new ValidationResult("Ports must be an unsigned integer between 1 and 65535");
            }

            return ValidationResult.Success;
        }
    }
}
