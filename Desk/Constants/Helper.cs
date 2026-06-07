using Desk.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Desk.Constants
{
    /// <summary>
    /// Contains helper methods, mostly related to the Dictionaries class.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Creates a select list from a type. (Not used yet)
        /// Values are taken from the fields of the type.
        /// Text is the name of the field.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SelectList GetSelectList(Type type)
        {
            var properties = type.GetFields();
            var propertyValues = properties.Select(p => new { Value = p.GetValue(null), Text = p.Name }).ToList();

            return new SelectList(propertyValues, "Text", "Value");
        }

        /// <summary>
        /// Creates a select list from a dictionary.
        /// Value is the key of the dictionary.
        /// Text is the value of the dictionary.
        /// </summary>
        /// <param name="dictionary">E.g. Constants.Dictionaries</param>
        /// <returns></returns>
        public static SelectList GetSelectList(Dictionary<string, string> dictionary)
        {
            return new SelectList(dictionary.ToList(), "Key", "Value");
        }

        /// <summary>
        /// Creates a category from the type of the ticket (from the special properties part).
        /// </summary>
        /// <param name="type">The type of the ticket (the special properties part)</param>
        /// <returns>A raw category as string</returns>
        public static string GetCategory(Type type)
        {
            var category = type.Namespace!.Replace("Desk.Models.", "") + "." + type.Name;

            return category;
        }

        /// <summary>
        /// Formats the (raw) category of the ticket. Translates it to a user friendly string.
        /// </summary>
        /// <param name="category">A raw category of the ticket</param>
        /// <returns></returns>
        public static string? FormatCategory(string? category)
        {
            if (string.IsNullOrEmpty(category)) return "Kategória nincs kitöltve!";

            var splittedCategory = category.Split('.');
            var formattedCategory = "";

            foreach (var tag in splittedCategory)
            {
                formattedCategory += Dictionaries.Category[tag];

                if (tag != splittedCategory.Last())
                    formattedCategory += " > ";
            }

            return formattedCategory;
        }
        /// <summary>
        /// Creates a short version of the formatted category. Basically the type of the special properties part of the ticket but translated.
        /// </summary>
        /// <param name="category">The unformatted category of the ticket</param>
        /// <returns></returns>
        public static string? FormatCategoryShort(string category)
        {
            return Dictionaries.Category[category.Split('.').Last()];
        }

        /// <summary>
        /// Returns the controller of the special properties part of the ticket. (Without the "Controller" part)
        /// Basically the type of the special properties part of the ticket.
        /// </summary>
        /// <param name="category">The unformatted category of the ticket</param>
        /// <returns></returns>
        public static string GetController(string category)
        {
            return category.Split('.').Last();
        }

        /// <summary>
        /// Checks if the user is a developer user.
        /// </summary>
        /// <param name="userName">Pre-formatted username (LoggedInUser.UserName)</param>
        /// <returns>True if the user is a developer user</returns>
        public static bool IsDevUser(string? userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;

            return userName.ToLower() == Dictionaries.Role["DEV_USER"].ToLower();
        }
    }
}
