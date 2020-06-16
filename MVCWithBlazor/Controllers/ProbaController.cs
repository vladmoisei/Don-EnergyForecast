using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MVCWithBlazor.Controllers
{
    public class ProbaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}