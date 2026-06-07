using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Desk.Constants;

namespace Desk.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Archivált")]
        public bool IsArchived { get; set; } = false;

        [Display(Name = "Létrehozás ideje")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MMM d. HH:mm}")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Display(Name = "Lezárás időpontja")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MMM d. HH:mm}")]
        public DateTime? ClosedAt { get; set; }
        
        [Display(Name = "Állapot")]
        public string Status { get; set; } = Dictionaries.TicketStatus.Keys.First();

        [Display(Name = "Prioritás")]
        public string Priority { get; set; } = Dictionaries.TicketPriority.Keys.First();

        // Users
        [ForeignKey(nameof(CreatedBy))]
        [Display(Name = "Létrehozta")]
        public int CreatedById { get; set; }
        [Display(Name = "Létrehozta")]
        public User? CreatedBy { get; set; }

        // Users
        [ForeignKey(nameof(ReportedBy))]
        [Display(Name = "Bejelentő")]
        public int ReportedById { get; set; }
        [Display(Name = "Bejelentő")]
        public User? ReportedBy { get; set; }


        [Display(Name = "Leírás")]
        [StringLength(280, ErrorMessage = "Nem lehet hosszabb 280 karakternél.")]
        public string? Description { get; set; }

        [Display(Name = "Kategória")]
        public string? Categorization { get; set; }     // typeof(SpecialProp).Namespace

        // Users
        [ForeignKey(nameof(AssignedToOperator))]
        [Display(Name = "Hozzárendelt operátor")]
        public int? AssignedToOperatorId { get; set; }
        [Display(Name = "Hozzárendelt operátor")]
        public User? AssignedToOperator { get; set; }

        // Groups
        [ForeignKey(nameof(AssignedToGroup))]
        [Display(Name = "Hozzárendelt csoport")]
        public int? AssignedToGroupId { get; set; }
        [Display(Name = "Hozzárendelt csoport")]
        public Group? AssignedToGroup { get; set; }

        // Suppliers
        [ForeignKey(nameof(Supplier))]
        [Display(Name = "Beszállító")]
        public int? SupplierId { get; set; }
        [Display(Name = "Beszállító")]
        public Supplier? Supplier { get; set; }

        // Action Extras
        [Display(Name = "Befejezés időpontja")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MMM d. HH:mm}")]
        public DateTime? ResolvedAt { get; set; }


        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        //public ICollection<Tag>? Tags { get; set; }



        // Methods

        /// <summary>
        /// Returns a reference ID, for example: A-220101-001
        /// First char of the (translated) categorization, timestamp and the last 3 digit of the ID
        /// For a readable ID, with information about the category, timestamp and ID
        /// </summary>
        /// <returns></returns>
        public string ReferenceId()
        {
            // First char of the (translated) Categorization, timestamp and the last 3 digit of the ID
            return $"{Dictionaries.Category[Categorization!.Split('.')[0]][0]}-{Timestamp:yyMMdd}-{Id % 1000:D3}";
        }

        /// <summary>
        /// Returns the role that a user should have to see the ticket
        /// </summary>
        /// <returns></returns>
        public string GetRole()
        {
            if (AssignedToGroup != null) { return AssignedToGroup.Role ?? "ADMIN_ROLE"; }
            // TODO: complete implementation

            return "ADMIN_ROLE";
        }

        /// <summary>
        /// Returns the URL to the details page.
        /// The URL is based on the current request.
        /// For example: https://localhost:5001/Ticket/Details/1
        /// It is for the body of the email notification.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string GetUrlForDetails(HttpContext httpContext)
        {
            string? pathBase = !string.IsNullOrEmpty(httpContext.Request.PathBase.Value) ? "/" + httpContext.Request.PathBase.Value : "";
            return $"https://{httpContext.Request.Host.Value}{pathBase}/{Helper.GetController(Categorization!)}/Details/{Id}";
        }

        public void UpdateStatus(string newStatus)
        {
            // TODO: implement
        }

        /// <summary>
        /// Returns the summary of the ticket
        /// </summary>
        /// <returns></returns>
        public string GetSummary()
        {
            return Helper.FormatCategory(Categorization) + " - " + Description ?? "";
        }

        /// <summary>
        /// Archives the ticket (only if it is closed and after a certain period of time)
        /// </summary>
        public void Archive()
        {
            int archiveAfterDays = 30;

            if (!IsClosed()) { return; }
            if (ClosedAt >= DateTime.Now.AddDays(-archiveAfterDays)) { return; }

            IsArchived = true;
        }

        /// <summary>
        /// Returns true if the ticket is overdue.
        /// The deadline value is set to 1 day. So if the ticket was registered 1 day ago, it is overdue.
        /// </summary>
        public bool IsOverdue()
        {
            // Deadline in days
            int deadline = 1;

            return Timestamp.AddDays(deadline) < DateTime.Now;
        }

        /// <summary>
        /// Returns true if the ticket is HIGH or VIP priority
        /// </summary>
        /// <returns></returns>
        public bool IsImportant()
        {
            List<string> importantPriorities = new List<string>
            {
                "HIGH",
                "VIP"
            };

            return importantPriorities.Contains(Priority);
        }

        /// <summary>
        /// Returns true if the ticket is closed
        /// </summary>
        /// <returns></returns>
        public bool IsClosed()
        {
            List<string> closedStatuses = new List<string>
            {
                "RESOLVED",
                "REJECTED",
                "CLOSED_BY_USER"
            };

            return closedStatuses.Contains(Status);
        }

        /// <summary>
        /// Returns true if the ticket is new
        /// </summary>
        /// <returns></returns>
        public bool IsNew()
        {
            List<string> newStatuses = new List<string>
            {
                "NEW",
                "REOPENED"
            };
            return newStatuses.Contains(Status);
        }

        /// <summary>
        /// Returns the HTML style of the text of the priority
        /// </summary>
        /// <returns>Bootstrap classes</returns>
        public string GetPriorityStyle()
        {
            return IsImportant() ? Styles.Important : "";
        }

        /// <summary>
        /// Returns the HTML style of the text of the status
        /// </summary>
        /// <returns>Bootstrap classes</returns>
        public string GetStatusStyle()
        {
            return IsNew() ? Styles.New : "";
        }

        /// <summary>
        /// Returns the HTML style of the ticket list item.
        /// If the ticket is closed, the item is grayed out
        /// If the ticket is new, the item is bold
        /// If the ticket is overdue, the item is red
        /// </summary>
        /// <returns>Bootstrap classes</returns>
        public string GetListItemStyle()
        {
            if (IsClosed()) { return Styles.Closed; }

            string style = "";

            if (IsNew()) { style = Styles.New; }

            if (IsOverdue()) { style += Styles.Overdue; }
            else { style += Styles.Default; }

            return style;
        }
    }
}
