using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROG6212Library.Utils;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class SemesterController : Controller
    {
        private readonly PROGPOEContext _context;

        public SemesterController(PROGPOEContext context)
        {
            _context = context;
        }

        // GET: Semester
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("LoggedIn") != null) //ensuring the user is logged in
            {
                int userId = 0;
                if (UserIDUtils.user.Count > 0) //error handling to ensure there is data in the list 
                {
                    userId = UserIDUtils.user[0].UserId; //fetching the username saved into the list 
                } 
                var pROGPOEContext = _context.Semesters.Include(s => s.Users);
                return View(_context.Semesters.Where(m => m.UsersId.Equals(userId)).ToList());
            }
            else
            {
                TempData["LoginFirst"] = "Please login first.";
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: Semester/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters
                .Include(s => s.Users)
                .FirstOrDefaultAsync(m => m.SemesterId == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // GET: Semester/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("LoggedIn") != null) //ensuring the user is logged in
            {
                ViewData["UsersId"] = new SelectList(_context.RegisteredUsers, "UsersId", "Password");
                return View();
            }
            else
            {
                TempData["LoginFirst"] = "Please login first.";
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: Semester/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SemesterWeeks,SemesterDate")] Semester semester)
        {
            if (ModelState.IsValid)
            {
                int userId = 0;
                if (UserIDUtils.user.Count > 0) //error handling to ensure there is data in the list 
                {
                    userId = UserIDUtils.user[0].UserId; //fetching the username saved into the list 
                }

                var newSemester = new Semester //adding the new semester to the db
                {
                    SemesterWeeks = semester.SemesterWeeks,
                    SemesterDate = semester.SemesterDate,
                    UsersId = userId 
                };

                 _context.Add(newSemester);
                await _context.SaveChangesAsync();
                ViewBag.SemMessage = string.Format("Semester created!");
                return View();
            }
            ViewBag.SemesterError = "Incorrect Details. Please try again.";
            return View(semester);
        }

        // GET: Semester/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters.FindAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            ViewData["UsersId"] = new SelectList(_context.RegisteredUsers, "UsersId", "Password", semester.UsersId);
            return View(semester);
        }

        // POST: Semester/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SemesterId,SemesterWeeks,SemesterDate,UsersId")] Semester semester)
        {
            if (id != semester.SemesterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(semester);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SemesterExists(semester.SemesterId))
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
            ViewData["UsersId"] = new SelectList(_context.RegisteredUsers, "UsersId", "Password", semester.UsersId);
            return View(semester);
        }

        // GET: Semester/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters
                .Include(s => s.Users)
                .FirstOrDefaultAsync(m => m.SemesterId == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // POST: Semester/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Semesters == null)
            {
                return Problem("Entity set 'PROGPOEContext.Semesters'  is null.");
            }
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SemesterExists(int id)
        {
          return (_context.Semesters?.Any(e => e.SemesterId == id)).GetValueOrDefault();
        }
    }
}
