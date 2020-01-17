using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }
        
        private MyContext dbContext;
        
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(IndexViewModel newUsr)
        {
            if (ModelState.IsValid)
            {
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == newUsr.NewUser.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("NewUser.Email", "Email is already in use!");

                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                string hashedPw = hasher.HashPassword(newUsr.NewUser, newUsr.NewUser.Password);
                newUsr.NewUser.Password = hashedPw;
                dbContext.Users.Add(newUsr.NewUser);
                dbContext.SaveChanges();
                UserSession = newUsr.NewUser.UserId;
                return RedirectToAction("Dashboard", "Wedding");
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult Login(IndexViewModel currUser)
        {
            if (ModelState.IsValid)
            {
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == currUser.CurrentUser.Email);
                if (existingUser == null)
                {
                    ModelState.AddModelError("CurrentUser.Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(currUser.CurrentUser, existingUser.Password, currUser.CurrentUser.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("CurrentUser.Email", "Invalid Email/Password");
                    return View("Index");
                }
                UserSession = existingUser.UserId;
                return RedirectToAction("Dashboard", "Wedding");
            }
            return View("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
