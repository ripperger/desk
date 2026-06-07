using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Desk.Data;
using Desk.Models;

namespace Desk.Controllers
{
    public class ADUsersController : Controller
    {
        private readonly DeskContext _context;

        public ADUsersController(DeskContext context)
        {
            _context = context;
        }

        // GET: ADUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.ADUsers.OrderBy(a => a.FullName).ToListAsync());
        }

        // GET: ADUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aDUser = await _context.ADUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aDUser == null)
            {
                return NotFound();
            }

            return View(aDUser);
        }

        // GET: ADUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ADUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sid,IsDeleted,UserName,FullName,Email,Phone,Department,IsVip")] ADUser aDUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aDUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aDUser);
        }

        // GET: ADUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aDUser = await _context.ADUsers.FindAsync(id);
            if (aDUser == null)
            {
                return NotFound();
            }
            return View(aDUser);
        }

        // POST: ADUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sid,IsDeleted,UserName,FullName,Email,Phone,Department,IsVip")] ADUser aDUser)
        {
            if (id != aDUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aDUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ADUserExists(aDUser.Id))
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
            return View(aDUser);
        }

        // GET: ADUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aDUser = await _context.ADUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aDUser == null)
            {
                return NotFound();
            }

            return View(aDUser);
        }

        // POST: ADUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aDUser = await _context.ADUsers.FindAsync(id);
            if (aDUser != null)
            {
                _context.ADUsers.Remove(aDUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ADUserExists(int id)
        {
            return _context.ADUsers.Any(e => e.Id == id);
        }
    }
}
