using System.ComponentModel.DataAnnotations;
using Desk.Constants;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Desk.Validation
{
    public class TaxNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) { return new ValidationResult(Messages.Required); }

            string number = (string)value;
            if (number.Length == 10
                && number[0] == '8'
                && GetBirthDate(number) < DateTime.Today
                && MathCheck(number))
            { return ValidationResult.Success; }

            return new ValidationResult("Helytelen adószám.");
        }

        public static bool MathCheck(string number)
        {
            int sum = 0;
            for (int i = 0; i < number.Length - 1; i++)
            {
                sum += int.Parse(number[i].ToString()) * (i + 1);
            }

            return sum % 11 != 10 && sum % 11 == int.Parse(number.Last().ToString());
        }

        public static DateTime GetBirthDate(string number)
        {
            DateTime start = new DateTime(1867, 1, 1);
            return start.AddDays(int.Parse(number.Substring(1, 5)));
        }
    }
}
