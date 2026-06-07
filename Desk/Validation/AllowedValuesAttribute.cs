using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Desk.Validation
{
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues;

        public AllowedValuesAttribute(string[] allowedValues)
        {
            _allowedValues = allowedValues;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || _allowedValues.Contains(value.ToString()))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Az érték nem megfelelő.");
        }
    }

}
