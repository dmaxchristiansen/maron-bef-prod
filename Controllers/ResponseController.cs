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
    public class ResponseController : Controller
    {
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }
        private MyContext dbContext;
        public ResponseController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("RSVP/{weddingId}")]
        public IActionResult RSVP(int weddingId)
        {
            if (UserSession == null)
                return RedirectToAction("Index", "Home");

            // Create a new response with the given weddingId and current userId
            Response newResponse = new Response()
            {
                WeddingId = weddingId,
                UserId = (int)UserSession
            };
            dbContext.Responses.Add(newResponse);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard", "Wedding");
        }

        [HttpGet("UnRSVP/{weddingId}")]
        public IActionResult UnRSVP(int weddingId)
        {
            if (UserSession == null)
                return RedirectToAction("Index", "Home");
            
            // Query to grab the appropriate response to remove
            Response toDelete = dbContext.Responses.FirstOrDefault(r => r.WeddingId == weddingId && r.UserId == UserSession);
            
            // Redirect to dashboard if no match for response in db
            if (toDelete == null)
                return RedirectToAction("Dashboard", "Wedding");

            dbContext.Responses.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard", "Wedding");
        }
    }
}