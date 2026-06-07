namespace Desk.Constants
{
    public static class Messages
    {
        // For validation with attributes (attributes require const properties)
        public const string NotEmailFormat = "Nem emailcím formátum.";
        public const string Required = "Kötelező megadni.";
        public const string StringLength30 = "Legalább 3, lefeljebb 30 karakter engedélyezett.";
        public const string StringLength60 = "Legalább 3, lefeljebb 60 karakter engedélyezett.";
        public const string RangeNotNegative = "Nem lehet negatív érték.";

        // TempData error messages
        public const string SendingEmailFailed = "Az email értesítés kiküldése nem sikerült";

        // TempData success messages
        public const string NewRequestSuccess = "A bejelentés rögzítése sikerült";

        // Exception string parameters for description
        public const string ArgumentNullException = "Bejelentés nem található";
    }
}
