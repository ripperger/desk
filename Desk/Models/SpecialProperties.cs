using Desk.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations.Schema;

namespace Desk.Models
{
    public class SpecialProperties
    {
        [BindNever]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]   // To not use autoincrement
        public int Id { get; set; }

        /// <summary>
        /// Creates a category from the type of the ticket (from the special properties part).
        /// </summary>
        /// <returns>A raw category as string</returns>
        public string GetCategory()
        {
            Type type = GetType();
            var category = type.Namespace!.Replace("Desk.Models.", "") + "." + type.Name;

            return category;
        }

        public string GetDefaultGroupName()
        {
            try
            {
                return Dictionaries.DefaultGroup[GetType().Name];
            }
            catch (KeyNotFoundException)
            {
                // returns the first default group in the dictionary
                return Dictionaries.DefaultGroup.First().Value;
            }
        }
    }
}
