namespace Desk.Constants
{
    public static class Dictionaries
    {
        public static readonly Dictionary<string, string> Category = new Dictionary<string, string>
        {
            { "SpecialProperties", "Alapkategória" },    // default
            { "TestCategoryA", "Teszt Kategória 'A'" },
            { "TestSpecPropsAA", "Teszt Alkategória 'AA'" },
            { "Device", "Eszköz bejelentő" },
            { "MultiFunctionPrinter", "Multifunkcionális eszköz" },
            { "Authorization", "Jogosultságok" },
            { "NewPassword", "Új jelszó" },
        };

        public static readonly Dictionary<string, string> AuthorizationRequestType = new Dictionary<string, string>
        {
            { "ADD", "Jogosultság beállítása" },       // default
            { "REMOVE", "Visszavonás" },
        };
        
        public static readonly Dictionary<string, string> EventType = new Dictionary<string, string>
        {
            { "STATUS_CHANGE", "Állapotváltozás" },    // default
            { "SUPPLIER_CHANGE", "Beszállító-változás" },
            { "COMMENT", "Hozzászólás" },
            { "EMAIL_NOTIFICATION", "Email értesítés" },
            { "ASSIGNMENT", "Hozzárendelés" },
            { "ATTACHMENT", "Melléklet" },
            { "EDITING", "Szerkesztés" }        // Az eredeti jegy módosítása
        };
        

        public static readonly Dictionary<string, string> PrinterRequestType = new Dictionary<string, string>
        {
            { "INCIDENT", "Hiba" },       // default
            { "SUPPLIES", "Kellékanyag" },
            { "INCIDENT_AND_SUPPLIES", "Hiba és kellékanyag" },
            { "ADMINISTRATION", "Adminisztráció" }
        };


        /// <summary>
        /// </summary>
        public static readonly Dictionary<string, string> TicketPriority = new Dictionary<string, string>
        {
            { "MEDIUM", "Átlagos" },    // default
            { "HIGH", "Sürgős" },
            { "VIP", "VIP" }
        };

        /// <summary>
        /// </summary>
        public static readonly Dictionary<string, string> TicketStatus = new Dictionary<string, string>
        {
            { "NEW", "Regisztrálva (új)" },    // default
            { "RESOLVED", "Lezárva (elvégezve)" },
            { "REJECTED", "Lezárva (elutasítva)" },
            { "CLOSED_BY_USER", "Lezárva (felhasználó által)" },
            { "ASSIGNED_TO_SUPPLIER", "Továbbítva (beszállítónak)" },
            { "PENDING", "Függőben" },
            { "REOPENED", "Visszanyitva" }
        };
    }
}
