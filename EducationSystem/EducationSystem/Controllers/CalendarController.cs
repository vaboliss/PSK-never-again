﻿using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}