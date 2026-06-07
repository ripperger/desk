using Desk.Constants;
using Desk.Models.TestCategoryA;
using Desk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Desk.Data;
using Desk.Contracts.Services;
using Desk.Models.Authorization;

namespace Desk.Controllers.Authorization
{
    public class NewPasswordController : SpecialPropertiesController
    {
        public NewPasswordController(DeskContext context, ILogger<NewPasswordController> logger, IEmailService emailService) : base(context, logger, emailService) { }

        /// <summary>
        /// GET: ControllerName/Details/{id}}
        /// Displays the details of a specific ticket
        /// ViewData["Statuses"] and ViewData["Suppliers"] are set
        /// </summary>
        /// <param name="id">The id of the ticket</param>
        /// <returns>The Details.cshtml view and the specific SpecialPropertiesViewModel</returns>
        public override async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

            ViewData["Statuses"] = GetStatuses();
            //ViewData["Suppliers"] = GetSuppliers();

            return View(ViewPath + "Details.cshtml", await GetSpecialPropertiesViewModel((int)id!));
        }

        /// <summary>
        /// GET: ControllerName/Create
        /// Displays the create view
        /// ViewData["Users"],ViewData["Priorities"] and ViewData["LoggedInUser"] are set
        /// </summary>
        /// <returns>The Create.cshtml view and a new SpecialPropertiesViewModel</returns>
        public override IActionResult Create()
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
        public async Task<IActionResult> Create([Bind("Description,Priority,ReportedById")] Ticket ticket, [Bind] NewPassword specialProperties)
        {
            // Server side validation
            if (!ModelState.IsValid)
            {
                LogValidationErrors();

                Create();   // Reload ViewData

                // Give back the given (invalid) data
                return View(ViewPath + "Create.cshtml", CreateSpecialPropertiesViewModel(ticket, specialProperties));
            }


            // For concurrency control

            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //    try
            //    {

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

            //    transaction.Commit();
            //}
            //catch (Exception)
            //{
            //    transaction.Rollback();
            //}
            //}

            return RedirectToAction(nameof(Index), "Tickets");
        }

        public override void AddSpecialProperties(SpecialProperties specialProperties)
        {
            _context.NewPassword.Add((NewPassword)specialProperties);
        }

        /// <summary>
        /// Gets a specific SpecialPropertiesViewModel
        /// For the Details action
        /// </summary>
        /// <param name="id">The id of the ticket</param>
        /// <returns>The specific SpecialPropertiesViewModel</returns>
        /// <exception cref="ArgumentNullException">If the Ticket or SpecialProperties is null</exception>
        public new async Task<NewPasswordViewModel> GetSpecialPropertiesViewModel(int id)
        {
            var specialPropertiesVM = new NewPasswordViewModel
            {
                Ticket = await GetTicket((int)id),
                SpecialProperties = await _context.NewPassword
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
        public new NewPasswordViewModel CreateSpecialPropertiesViewModel()
        {
            var specialPropertiesVM = new NewPasswordViewModel();

            return specialPropertiesVM;
        }

        /// <summary>
        /// Creates a new SpecialPropertiesViewModel with the given Ticket and SpecialProperties
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="specialProperties"></param>
        /// <returns></returns>
        public NewPasswordViewModel CreateSpecialPropertiesViewModel(Ticket ticket, NewPassword specialProperties)
        {
            var specialPropertiesVM = new NewPasswordViewModel
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
        protected override string SetViewPath()
        {
            return "~/Views/Authorization/NewPassword/";
        }

    }
}
