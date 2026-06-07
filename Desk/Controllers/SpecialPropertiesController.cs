using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Desk.Data;
using Desk.Models;
using Desk.Services;
using Desk.Constants;
using Desk.Contracts.Services;
using Desk.Models.TestCategoryA;
using Desk.Models.Device.MultiFunctionPrinter;
using Desk.Controllers.Device.MultiFunctionPrinter;


namespace Desk.Controllers
{
    public class SpecialPropertiesController : Controller
    {
        protected readonly DeskContext _context;
        protected readonly IUserService _userService;
        private readonly ILogger<SpecialPropertiesController> _logger;
        protected readonly IEmailService _emailService;

        /// <summary>
        /// The path to the views - because of the non-default folder structure
        /// </summary>
        protected string ViewPath;

        /// <summary>
        /// The currently logged in user
        /// </summary>
        protected User LoggedInUser { get { return _userService.GetUserByUserNameAsync(_userService.LoggedInUserName(User)).Result; } }

        public SpecialPropertiesController(DeskContext context, ILogger<SpecialPropertiesController> logger, IEmailService emailService)
        {
            _context = context;
            _userService = new UserService(context);
            _logger = logger;
            _emailService = emailService;
            ViewPath = SetViewPath();
        }

        // OVERRIDE THE FOLLOWING METHODS IN THE CHILD CONTROLLERS

        //public SpecialPropertiesController(DeskContext context, ILogger<SpecialPropertiesController> logger, IEmailService emailService) : base(context, logger, emailService) { }    // CHANGE THIS(2) (name of constructor and TResult type of ILogger)

        /// <summary>
        /// GET: ControllerName/Details/{id}}
        /// Displays the details of a specific ticket
        /// ViewData["Statuses"] and ViewData["Suppliers"] are set
        /// </summary>
        /// <param name="id">The id of the ticket</param>
        /// <returns>The Details.cshtml view and the specific SpecialPropertiesViewModel</returns>
        public virtual async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

            ViewData["Statuses"] = GetStatuses();
            ViewData["Suppliers"] = GetSuppliers();

            return View(ViewPath + "Details.cshtml", await GetSpecialPropertiesViewModel((int)id!));
        }

        /// <summary>
        /// GET: ControllerName/Create
        /// Displays the create view
        /// ViewData["Users"],ViewData["Priorities"] and ViewData["LoggedInUser"] are set
        /// </summary>
        /// <returns>The Create.cshtml view and a new SpecialPropertiesViewModel</returns>
        public virtual IActionResult Create()
        {
            ViewData["Users"] = GetUsers();
            ViewData["Priorities"] = GetPriorities();
            ViewData["LoggedInUser"] = LoggedInUser;

            return View(ViewPath + "Create.cshtml", CreateSpecialPropertiesViewModel());
        }

        /// <summary>
        /// POST: SpecialProperties/Create
        /// (To protect from overposting attacks, enable the specific properties you want to bind to.)
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="specialProperties"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBase([Bind("Description,Priority,ReportedById")] Ticket ticket, [Bind] SpecialProperties specialProperties)      // CHANGE THIS(2) (name of method and type of specialProperties)
        {
            // Server side validation
            if (!ModelState.IsValid)
            {
                LogValidationErrors();

                Create();   // Reload ViewData

                // Give back the given (invalid) data
                return View(ViewPath + "Create.cshtml", CreateSpecialPropertiesViewModel(ticket, specialProperties));
            }


            // Setting general properties for the Ticket:

            // Reporter and creator Users
            ticket.CreatedById = LoggedInUser.Id;
            if (!_userService.IsDevUser(LoggedInUser.UserName) && !User.IsInRole(Dictionaries.Role["ADMIN_ROLE"]))
            {
                ticket.ReportedById = LoggedInUser.Id;  // If the logged in user is not admin, must be the reporter user
            }

            // Category
            ticket.Categorization = specialProperties.GetCategory();

            // Assigning a Group for the Ticket
            var group = _context.Groups.FirstOrDefault(g => g.Name == specialProperties.GetDefaultGroupName());
            if (group != null) ticket.AssignedToGroupId = group.Id;

            // Adding the new Ticket to db
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Adding the Special Properties part of the Ticket to db
            specialProperties.Id = ticket.Id;
            AddSpecialProperties(specialProperties);
            await _context.SaveChangesAsync();

            try
            {
                // Sending Email to reporter user
                await _emailService.SendEmailToReportedByAsync(
                                        ticket.GetUrlForDetails(HttpContext),       // url
                                        ticket,                                     // ticket
                                        Email.NewTicketSubject,                     // subject
                                        Email.NewTicketMessage                      // message
                                        );


                // Sending Email to Assigned Group
                await _emailService.SendEmailToAssignedAsync(
                                        ticket.GetUrlForDetails(HttpContext),       // url
                                        ticket,                                     // ticket
                                        Email.NewTicketSubject,                     // subject
                                        Email.NewTicketMessage                      // message
                                        );
            }
            catch (Exception)
            {
                TempData["error"] = Messages.SendingEmailFailed;
            }
            finally
            {
                _emailService.DisposeSmtpClientAsync();
            }

            // Instant feedback to user
            TempData["success"] = Messages.NewRequestSuccess;

            return RedirectToAction(nameof(Index), "Tickets");
        }

        public virtual void AddSpecialProperties(SpecialProperties specialProperties)
        {
            _context.TestSpecPropsAA.Add((TestSpecPropsAA)specialProperties);   // CHANGE THIS(2) (dbset type and cast type)
        }

        /// <summary>
        /// Gets a specific SpecialPropertiesViewModel
        /// For the Details action
        /// </summary>
        /// <param name="id">The id of the ticket</param>
        /// <returns>The specific SpecialPropertiesViewModel</returns>
        /// <exception cref="ArgumentNullException">If the Ticket or SpecialProperties is null</exception>
        public async Task<SpecialPropertiesViewModel> GetSpecialPropertiesViewModel(int id)         // CHANGE THIS (TResult type)
        {
            var specialPropertiesVM = new SpecialPropertiesViewModel         // CHANGE THIS (object type)
            {
                Ticket = await GetTicket((int)id),
                SpecialProperties = await _context.TestSpecPropsAA         // CHANGE THIS (dbset type)
                                .FirstOrDefaultAsync(sp => sp.Id == id)
            };

            if (specialPropertiesVM.SpecialProperties == null || specialPropertiesVM.Ticket == null)
            {
                throw new ArgumentNullException(Messages.ArgumentNullException);
            }
            return specialPropertiesVM;
        }

        /// <summary>
        /// Creates a new SpecialPropertiesViewModel
        /// </summary>
        /// <returns>A new SpecialPropertiesViewModel</returns>
        public SpecialPropertiesViewModel CreateSpecialPropertiesViewModel()         // CHANGE THIS (return type)
        {
            var specialPropertiesVM = new SpecialPropertiesViewModel();         // CHANGE THIS (object type)

            return specialPropertiesVM;
        }

        /// <summary>
        /// Creates a new SpecialPropertiesViewModel with the given Ticket and SpecialProperties
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="specialProperties"></param>
        /// <returns></returns>
        public SpecialPropertiesViewModel CreateSpecialPropertiesViewModel(Ticket ticket, SpecialProperties specialProperties)         // CHANGE THIS(2) (return type and type of specialProperties)
        {
            var specialPropertiesVM = new SpecialPropertiesViewModel         // CHANGE THIS (object type)
            {
                Ticket = ticket,
                SpecialProperties = specialProperties
            };

            return specialPropertiesVM;
        }

        /// <summary>
        /// Sets the path to the views
        /// </summary>
        /// <returns></returns>
        protected virtual string SetViewPath()
        {
            return "~/Views/";      // CHANGE THIS (the path to the views)
        }

        // NO NEED TO OVERRIDE THE FOLLOWING METHODS

        protected SelectList GetPriorities()
        {
            return new SelectList(Dictionaries.TicketPriority.ToList(), "Key", "Value");
        }

        protected SelectList GetPrinterRequestTypes()
        {
            return new SelectList(Dictionaries.PrinterRequestType.ToList(), "Key", "Value");
        }

        protected SelectList GetStatuses()
        {
            return new SelectList(Dictionaries.TicketStatus.ToList(), "Key", "Value");
        }

        protected SelectList GetSuppliers()
        {
            return new SelectList(_context.Suppliers, "Id", "Name");
        }

        /// <summary>
        /// Gets a specific Ticket by id
        /// Includes all related entities (AssignedToGroup, CreatedBy, ReportedBy, Supplier, Events with User)
        /// </summary>
        /// <param name="id">The id of the ticket</param>
        /// <returns>The specific Ticket or null</returns>
        protected async Task<Ticket?> GetTicket(int id)
        {
            return await _context.Tickets
                            .Include(t => t.AssignedToGroup)
                            //.Include(t => t.AssignedToOperator)
                            .Include(t => t.CreatedBy)
                            .Include(t => t.ReportedBy)
                            .Include(t => t.Supplier)
                            .Include(t => t.Events)
                            .Include(t => t.Comments)
                            .ThenInclude(e => e.User)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected SelectList GetUsers()
        {
            return new SelectList(_context.Users.OrderBy(u => u.FullName), "Id", "FullName");
        }

        /// <summary>
        /// Logs the validation errors as warnings
        /// </summary>
        protected void LogValidationErrors()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            foreach (var error in errors)
            {
                _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
            }
        }

    }
}
