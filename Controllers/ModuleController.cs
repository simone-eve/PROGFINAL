using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;
using PROG6212Library;
using PROG6212Library.Utils;
using System.Data.SqlClient;
using NuGet.LibraryModel;
using NuGet.Packaging;

namespace PROG6212POE.Controllers
{
    public class ModuleController : Controller
    {
        private readonly PROGPOEContext _context;
        PROGPOEContext db = new PROGPOEContext();
        public ModuleController(PROGPOEContext context)
        {
            _context = context;
        }

       
        // GET: Module
        public async Task<IActionResult> Index()
        {
            //__________________code attribution______________________
            //The following method was taken from PROG6211 - ASP.net Core MVC - State Management
            //Author: Ebrahim Adam
            //Link: https://www.youtube.com/watch?v=xDf-qltFNcY&list=PL480DYS-b_kcPXdBwVnnH6wDN-cqyMbyH&index=11
            if (HttpContext.Session.GetString("LoggedIn") != null) //ensuring the user is logged in
            {
                int userId = 0;
                if (UserIDUtils.user.Count > 0) //error handling to ensure there is data in the list 
                {
                    userId = UserIDUtils.user[0].UserId; //fetching the username saved into the list 
                }

                var pROGPOEContext = _context.Modules.Include(m => m.Users);
                return View(_context.Modules.Where(m => m.UsersId.Equals(userId)).ToList());
            }
            else
            {
                TempData["LoginFirst"] = "Please login first.";
                return RedirectToAction("Login", "Login");
            }
            //__________________end______________________


        }

        // GET: Module/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .Include(m => m.Users)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Module/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("LoggedIn") != null) //ensuring the user is logged in 
            {
                List<string> weekDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                ViewBag.WeekDays = new SelectList(weekDays); //creating a viewbag to load the drop down box with the days of the week
                ViewData["UsersId"] = new SelectList(_context.RegisteredUsers, "UsersId", "Password");
                return View();
            }
            else
            {
                TempData["LoginFirst"] = "Please login first.";
                return RedirectToAction("Login", "Login");
            }
           
        }

        // POST: Module/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleCode,ModuleName,NumberOfCredits,WeeklyHours, WeekDayName")] Module @module)
        {
            POECalculation calc = new POECalculation();
            int userId = 0;
            if (UserIDUtils.user.Count > 0) //error handling to ensure there is data in the list 
            {
                userId = UserIDUtils.user[0].UserId; //fetching the username saved into the list 
            }
            int semWeeks = calc.GetSemesterWeeks(userId); //getting the semester weeks from the library
            int selfStudyHours = calc.SelfStudyCalculation(module.NumberOfCredits, semWeeks, module.WeeklyHours); //getting the self study hours from the library

            if (ModelState.IsValid)
            {
                
                var newModule = new Module //saving the new module into the db
                {
                    ModuleCode = module.ModuleCode,
                    ModuleName = module.ModuleName, 
                    NumberOfCredits = module.NumberOfCredits,
                    WeeklyHours = module.WeeklyHours,
                    SelfStudyHours = selfStudyHours,
                    TotalSelfStudyHours = selfStudyHours,
                    WeekDayName = module.WeekDayName,
                    UsersId = userId
                };

                _context.Add(newModule);
                await _context.SaveChangesAsync();
                ViewBag.Message = string.Format("Module created!");

            }
            else { ViewBag.ModuleError = "Incorrect Details. Please try again."; }
            return View();
        }

        // GET: Module/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            ViewData["UsersId"] = new SelectList(_context.RegisteredUsers, "UsersId", "Password", @module.UsersId);
            return View(@module);
        }

        // POST: Module/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleId,ModuleCode,ModuleName,NumberOfCredits,WeeklyHours,SelfStudyHours,TotalSelfStudyHours,UsersId")] Module @module)
        {
            if (id != @module.ModuleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.ModuleId))
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
            ViewData["UsersId"] = new SelectList(_context.RegisteredUsers, "UsersId", "Password", @module.UsersId);
            return View(@module);
        }

        // GET: Module/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .Include(m => m.Users)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Modules == null)
            {
                return Problem("Entity set 'PROGPOEContext.Modules'  is null.");
            }
            var @module = await _context.Modules.FindAsync(id);
            if (@module != null)
            {
                _context.Modules.Remove(@module);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
          return (_context.Modules?.Any(e => e.ModuleId == id)).GetValueOrDefault();
        }
    }
}
