using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;
using Microsoft.AspNetCore.Http;
using PROG6212Library;
using PROG6212Library.Utils;
using Microsoft.AspNetCore.Html;
using System.Text;
using System.Security.Cryptography;

namespace PROG6212POE.Controllers
{
    
    public class LoginController : Controller
    {
        PROGPOEContext db = new PROGPOEContext();
        public LoginController(PROGPOEContext context) 
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
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

        [HttpPost]
        public IActionResult Login(string username, string password) 
        {
            string hashPassword = GetHashPassword(password);
            POECalculation calc = new POECalculation();
            string currentDay = DateTime.Now.DayOfWeek.ToString(); //calculating the currrent day of the week
            var weekDays = db.Modules.Where(m => m.WeekDayName == currentDay).ToList(); //searching for the module that has the same week day as the current day
            
            //__________________code attribution______________________
            //The following method was taken from PROG6211 - ASP.net Core MVC - State Management
            //Author: Ebrahim Adam
            //Link: https://www.youtube.com/watch?v=xDf-qltFNcY&list=PL480DYS-b_kcPXdBwVnnH6wDN-cqyMbyH&index=11
            RegisteredUser reg = db.RegisteredUsers.Where(reg => reg.Username.Equals(username) && reg.Password.Equals(hashPassword)).FirstOrDefault(); //searching for the user that matches the entered username and password
            Module mod = db.Modules.Where(mod => mod.WeekDayName.Equals(currentDay)).FirstOrDefault(); //finding the module name for the subject the user has to study today 
            //__________________end______________________
            
            if (reg != null) 
            {
                UserIDUtils.user.Add(new PROG6212Library.Utils.UserID { UserId = reg.UsersId });

                TempData["username"] = reg.Username;
                if (weekDays != null && mod != null) //displaying the reminder to the user if they have a module set for that day
                {
                    //__________________code attribution______________________
                    //The following method was taken from ASPSNIPPETS
                    //Author: Mudassar Khan
                    //Link: https://www.aspsnippets.com/Articles/1642/ASPNet-MVC-Display-Message-from-Controller-in-View-using-JavaScript-Alert-MessageBox/
                    ViewBag.Message = string.Format("Login successful! Today is " + currentDay + ". Don't forget to do " + mod.ModuleName + " today!");
                    ViewBag.Message = new HtmlString(ViewBag.Message);
                    //__________________end______________________

                }
                else
                {
                    ViewBag.Message = string.Format("Login successful!");
                }

                //__________________code attribution______________________
                //The following method was taken from PROG6211 - ASP.net Core MVC - State Management
                //Author: Ebrahim Adam
                //Link: https://www.youtube.com/watch?v=xDf-qltFNcY&list=PL480DYS-b_kcPXdBwVnnH6wDN-cqyMbyH&index=11
                HttpContext.Session.SetString("LoggedIn", reg.Username);
                //__________________end______________________

            }
            else 
            {
                ViewBag.LoginError = "Incorrect Details. Please try again."; //telling the user to attempt to login again if their details are incorrect
                
            }
            
            if(calc.DoesUserHaveSemesterInfo(reg.UsersId))
            {
                return View(); 
            }
            else { return RedirectToAction("Create", "Semester"); }
            
        }

        public IActionResult Logout()
        {
            //__________________code attribution______________________
            //The following method was taken from PROG6211 - ASP.net Core MVC - State Management
            //Author: Ebrahim Adam
            //Link: https://www.youtube.com/watch?v=xDf-qltFNcY&list=PL480DYS-b_kcPXdBwVnnH6wDN-cqyMbyH&index=11
            HttpContext.Session.Clear();
            //__________________end______________________

            return RedirectToAction("Login", "Login");
        }




    }
}
