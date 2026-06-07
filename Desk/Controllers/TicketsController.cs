using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Desk.Data;
using Desk.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Desk.Services;
using Desk.Constants;
using Desk.Contracts.Services;
using System.Security.Claims;

namespace Desk.Controllers
{
    public class TicketsController : Controller
    {
        private readonly DeskContext _context;
        private readonly ILogger<TicketsController> _logger;
        private readonly IUserService _userService;

        public TicketsController(DeskContext context, ILogger<TicketsController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = new UserService(context);

        }

        public User LoggedInUser { get { return _userService.GetUserByUserName(_userService.LoggedInUserName(User)); } }

        // GET: Tickets
        public IActionResult Index()
        {
            var deskContext = _context.Tickets
                .Where(t => t.IsArchived == false)
                .Include(t => t.AssignedToGroup)
                .Include(t => t.AssignedToOperator)
                .Include(t => t.CreatedBy)
                .Include(t => t.ReportedBy)
                .Include(t => t.Supplier)
                .OrderByDescending(t => t.Id);

            if (User.IsInRole(Dictionaries.Role["ADMIN_ROLE"]) || _userService.IsDevUser(LoggedInUser.UserName))
            {
                return View(deskContext.ToList());
            }

            return View(deskContext.Where(t => t.ReportedById == LoggedInUser.Id).ToList());

        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                //.Include(t => t.AssignedToGroup)
                .Include(t => t.AssignedToOperator)
                .Include(t => t.CreatedBy)
                .Include(t => t.ReportedBy)
                .Include(t => t.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }



        // POST: Tickets/UpdateStatus/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, [Bind("Id,Status,SupplierId,ResolvedAt")] Ticket ticket, string? rejectReason, string? reopenReason)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            //if (LoggedInUser != ticket.ReportedBy) return Unauthorized();

            // TODO:
            // Check if the new status is the same as the current status
            // or if there are changes at all

            // _logger.LogInformation("{props}", ticket.CreatedById);

            var existingTicket = await _context.Tickets.FindAsync(id);
            existingTicket!.Status = ticket.Status;
            if (ticket.Status == "ASSIGNED_TO_SUPPLIER")
                existingTicket!.SupplierId = ticket.SupplierId;
            existingTicket!.ResolvedAt = ticket.ResolvedAt;

            // _logger.LogInformation("reason: {action}", rejectReason);

            // If there is additional information for the status update
            string reason = "";
            if (rejectReason != null) reason = " - " + rejectReason;
            if (reopenReason != null) reason = " - " + reopenReason;

            var newEvent = new Event()
            {
                Type = "STATUS_CHANGE",
                Description = Dictionaries.TicketStatus[existingTicket.Status] + reason,
                UserId = LoggedInUser.Id,
                TicketId = existingTicket.Id
            };

            try
            {
                _context.Update(existingTicket);
                _context.Add(newEvent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(existingTicket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            switch (existingTicket.Status)
            {
                case "CLOSED_BY_USER":
                    TempData["success"] = "A jegy sikeresen lezárva";
                    break;
                case "REOPENED":
                    TempData["success"] = "A jegy sikeresen visszanyitva";
                    break;
                default:
                    TempData["success"] = "A jegy állapota frissítve";
                    break;
            }

            return RedirectToAction(nameof(Details), Helper.GetController(existingTicket.Categorization!), new { id = existingTicket.Id });

        }

        // POST: Tickets/AddComment/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, [Bind("Id,Categorization")] Ticket ticket, [Bind("Text,TicketId,IsVisibleToReporter")] Comment comment)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                comment.UserId = LoggedInUser.Id;

                //_logger.LogInformation("Ticket ID: {id}", ticket.Id);
                //_logger.LogInformation("Categorization: {categ}", ticket.Categorization);

                _context.Add(comment);
                await _context.SaveChangesAsync();

                TempData["success"] = "Hozzászólás rögzítve";
            }

            return RedirectToAction(nameof(Details), Helper.GetController(ticket.Categorization!), new { id = ticket.Id });
        }

        // POST: Tickets/AssignToOperator/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToOperator(int id, string? userName)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null || userName == null)
                return NotFound();

            var assignedOperator = _userService.GetUserByUserName(userName.Split('\\')[1]);
            ticket.AssignedToOperator = assignedOperator;

            var newEvent = new Event()
            {
                Type = "ASSIGNMENT",
                Description = ticket.AssignedToOperator.FullName,
                UserId = LoggedInUser.Id,
                TicketId = ticket.Id
            };

            _context.Update(ticket);
            _context.Add(newEvent);
            await _context.SaveChangesAsync();

            TempData["success"] = "Jegy kiosztva";

            return RedirectToAction(nameof(Details), Helper.GetController(ticket.Categorization!), new { id = ticket.Id });
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
