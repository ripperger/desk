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
    public class ReportsController : Controller
    {
        private readonly DeskContext _context;
        private readonly ILogger<ReportsController> _logger;
        private readonly IUserService _userService;

        public ReportsController(DeskContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = new UserService(context);

        }

        public User LoggedInUser { get { return _userService.GetUserByUserName(_userService.LoggedInUserName(User)); } }

        // GET: Reports
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

        

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
