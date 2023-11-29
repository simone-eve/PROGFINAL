using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;
using PROG6212Library;
using System.Text;
using System.Security.Cryptography;
using PROG6212Library.Utils;

namespace PROG6212POE.Controllers
{
    public class RegisteredUserController : Controller
    {
        private readonly PROGPOEContext _context;

        public RegisteredUserController(PROGPOEContext context)
        {
            _context = context;
        }

        // GET: RegisteredUser
        public async Task<IActionResult> Index()
        {
              return _context.RegisteredUsers != null ? 
                          View(await _context.RegisteredUsers.ToListAsync()) :
                          Problem("Entity set 'PROGPOEContext.RegisteredUsers'  is null.");
        }

        // GET: RegisteredUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RegisteredUsers == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers
                .FirstOrDefaultAsync(m => m.UsersId == id);
            if (registeredUser == null)
            {
                return NotFound();
            }

            return View(registeredUser);
        }

        // GET: RegisteredUser/Create
        public IActionResult Create()
        {
            return View();
        }

        static string GetHashPassword(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // POST: RegisteredUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsersId,Username,Password")] RegisteredUser registeredUser)
        {
           string hashPassword = GetHashPassword(registeredUser.Password);
            if (ModelState.IsValid)
            {

                var newUser = new RegisteredUser //saving the new module into the db
                {
                    Username = registeredUser.Username,
                    Password = hashPassword
                };

                _context.Add(newUser);
                await _context.SaveChangesAsync();
                ViewBag.RMessage = string.Format("Register successful!");
                return RedirectToAction("Login", "Login"); ;

            }
            
            
            return View(registeredUser);
        }

        // GET: RegisteredUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RegisteredUsers == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser == null)
            {
                return NotFound();
            }
            return View(registeredUser);
        }

        // POST: RegisteredUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsersId,Username,Password")] RegisteredUser registeredUser)
        {
            if (id != registeredUser.UsersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registeredUser);
                    await _context.SaveChangesAsync();
                    ViewBag.RMessage = string.Format("Register successful!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisteredUserExists(registeredUser.UsersId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View();
            }
            return View();
        }

        // GET: RegisteredUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RegisteredUsers == null)
            {
                return NotFound();
            }

            var registeredUser = await _context.RegisteredUsers
                .FirstOrDefaultAsync(m => m.UsersId == id);
            if (registeredUser == null)
            {
                return NotFound();
            }

            return View(registeredUser);
        }

        // POST: RegisteredUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RegisteredUsers == null)
            {
                return Problem("Entity set 'PROGPOEContext.RegisteredUsers'  is null.");
            }
            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser != null)
            {
                _context.RegisteredUsers.Remove(registeredUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisteredUserExists(int id)
        {
          return (_context.RegisteredUsers?.Any(e => e.UsersId == id)).GetValueOrDefault();
        }
    }
}
