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
using Desk.Contracts.Services;

namespace Desk.Controllers
{
    public class UsersController : Controller
    {
        private readonly DeskContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(DeskContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _userService = new UserService(_context);
            _logger = logger;
        }

        [BindProperty]
        public UserViewModel UserVM { get; set; } = default!;

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
                .ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Groups"] = new SelectList(_context.Groups, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel userVM)
        {
            //[Bind("Id,Sid,IsDeleted,UserName,FullName,Email,Phone,Department,IsVip,LastUpdated")] 
            
            if (ModelState.IsValid)
            {
                if (userVM.SelectedGroupIds[0] != 0)
                {
                    userVM.UserGroups = userVM.SelectedGroupIds.Select(groupId => new UserGroup
                    {
                        GroupId = groupId
                    }).ToList();
                }

                _context.Add(userVM);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userVM);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sid,IsDeleted,UserName,FullName,Email,Phone,Department,IsVip,LastLogin")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
