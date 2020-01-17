using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class WeddingController : Controller
    {
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }
        private MyContext dbContext;
        public WeddingController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            // Redirect to register/login page if no user in session
            if (UserSession == null)
                return RedirectToAction("Index", "Home");

            // Get all weddings with included responses ordered by date
            var AllWeddings = dbContext.Weddings
                .Include(w => w.Responses)
                .OrderByDescending(w => w.Date)
                .ToList();
                
            ViewBag.UserId = UserSession;
            return View(AllWeddings);
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Wedding newWedding)
        {
            if (UserSession == null)
                    return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {   
                // Crete new wedding with UserId set to current session user's id
                newWedding.UserId = (int)UserSession;
                dbContext.Weddings.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("New");
        }

        [HttpGet("Show/{weddingId}")]
        public IActionResult Show(int weddingId)
        {
            var thisWedding = dbContext.Weddings
            .Include(w => w.Responses)
            .ThenInclude(r => r.Guest)
            .FirstOrDefault(w => w.WeddingId == weddingId);
            return View(thisWedding);
        }

        [HttpGet("Delete/{weddingId}")]
        public IActionResult Delete(int weddingId)
        {
            if (UserSession == null)
                return RedirectToAction("Index", "Home");

            Wedding toDelete = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);
            
            if (toDelete == null)
                return RedirectToAction("Dashboard");
            // Redirect to dashboard if user trying to delete isn't the wedding creator
            if (toDelete.UserId != UserSession)
                return RedirectToAction("Dashboard");
            
            dbContext.Weddings.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}