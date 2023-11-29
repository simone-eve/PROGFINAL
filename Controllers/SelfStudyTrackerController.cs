using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;
using PROG6212Library.Utils; 
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace PROG6212POE.Controllers
{
    public class SelfStudyTrackerController : Controller
    {
        private readonly PROGPOEContext _context;

        public SelfStudyTrackerController(PROGPOEContext context)
        {
            _context = context;
        }

        // GET: SelfStudyTracker
        public async Task<IActionResult> Index()
        {
              return _context.SelfStudyTrackers != null ? 
                          View(await _context.SelfStudyTrackers.ToListAsync()) :
                          Problem("Entity set 'PROGPOEContext.SelfStudyTrackers'  is null.");
        }

        // GET: SelfStudyTracker/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SelfStudyTrackers == null)
            {
                return NotFound();
            }

            var selfStudyTracker = await _context.SelfStudyTrackers
                .FirstOrDefaultAsync(m => m.TrackerId == id);
            if (selfStudyTracker == null)
            {
                return NotFound();
            }

            return View(selfStudyTracker);
        }

        // GET: SelfStudyTracker/Create
        public IActionResult Create()
        {
            int userId = 0;
            if (UserIDUtils.user.Count > 0) //error handling to ensure there is data in the list 
            {
                userId = UserIDUtils.user[0].UserId; //fetching the username saved into the list 
            }
            List<string> moduleNames = new List<string>();
            var modules = _context.Modules.Where(u => u.UsersId == userId).ToList(); //searching for all the modules connected to the UserID
            foreach (var mod in modules) //adding all the found modules to a list
            {
                moduleNames.Add(mod.ModuleName);
            }
            ViewBag.Modules = new SelectList(moduleNames); //creating a viewbag to load the drop down box with the module names
            return View();
        }

        // POST: SelfStudyTracker/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrackerId,ModuleName,StudyHours,CurrentDate")] SelfStudyTracker selfStudyTracker)
        {
            string selectedModuleName = selfStudyTracker.ModuleName; //assigning the users entered module name to a variable
            if (ModelState.IsValid)
            {
                _context.Add(selfStudyTracker); //adding to the db 
                await _context.SaveChangesAsync();

                //calculating the start and end days of the week that the user entered
                DateTime firstDay = selfStudyTracker.CurrentDate.AddDays(-(int)selfStudyTracker.CurrentDate.DayOfWeek);
                DateTime endDay = firstDay.AddDays(6);

                // Retrieve a list of study records within the current week (from firstDay to endDay)
                var studyRecord = _context.SelfStudyTrackers 
                .Where(record => record.CurrentDate.Date >= firstDay && record.CurrentDate.Date <= endDay)
                .ToList();

                // Calculate the total study hours for the specific module within the current week
                var totalStudyHours = studyRecord
                .Where(record => record.ModuleName == selfStudyTracker.ModuleName)
                .Select(g => new { ModuleName = g.ModuleName, TotalHours = g.StudyHours })
                .FirstOrDefault();

                // Retrieve the module information from the database based on the selected module name
                var updateList = _context.Modules.FirstOrDefault(record => record.ModuleName == selectedModuleName);

                int remainingHours = 0;

                if (updateList != null)
                {
                    //calculate remaining study hours
                    remainingHours = updateList.SelfStudyHours - totalStudyHours.TotalHours;

                    //update the selfstudyhours variable with the new amount
                    updateList.SelfStudyHours = remainingHours;

                    await _context.SaveChangesAsync();  // Save changes after updating 

                    //informing the user how many self-study hours they have remaining
                    ViewBag.SSMMessage = string.Format("You have " + remainingHours + " hours of selfstudy time for " + selectedModuleName + " remaining :)");
                  
                }

            }
            return View();
        }


        // GET: SelfStudyTracker/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SelfStudyTrackers == null)
            {
                return NotFound();
            }

            var selfStudyTracker = await _context.SelfStudyTrackers.FindAsync(id);
            if (selfStudyTracker == null)
            {
                return NotFound();
            }
            return View(selfStudyTracker);
        }

        // POST: SelfStudyTracker/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrackerId,ModuleName,StudyHours,CurrentDate")] SelfStudyTracker selfStudyTracker)
        {
            if (id != selfStudyTracker.TrackerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(selfStudyTracker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SelfStudyTrackerExists(selfStudyTracker.TrackerId))
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
            return View(selfStudyTracker);
        }

        // GET: SelfStudyTracker/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SelfStudyTrackers == null)
            {
                return NotFound();
            }

            var selfStudyTracker = await _context.SelfStudyTrackers
                .FirstOrDefaultAsync(m => m.TrackerId == id);
            if (selfStudyTracker == null)
            {
                return NotFound();
            }

            return View(selfStudyTracker);
        }

        // POST: SelfStudyTracker/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SelfStudyTrackers == null)
            {
                return Problem("Entity set 'PROGPOEContext.SelfStudyTrackers'  is null.");
            }
            var selfStudyTracker = await _context.SelfStudyTrackers.FindAsync(id);
            if (selfStudyTracker != null)
            {
                _context.SelfStudyTrackers.Remove(selfStudyTracker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SelfStudyTrackerExists(int id)
        {
          return (_context.SelfStudyTrackers?.Any(e => e.TrackerId == id)).GetValueOrDefault();
        }

      
    }
}
